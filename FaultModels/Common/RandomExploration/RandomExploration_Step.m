%  RandomExploration_Step
%
%  Executes the model with the step fault model as a basis in an
%  explorative manner, attempting to provide a view of the input space
%
%  Author: Alvin Stanescu
%
try
    % add all the paths containing the model and the functions we use
    addpath(CT_ModelPath);
    addpath(strcat(CT_ScriptsPath, '\ModelExecution'));
    addpath(strcat(CT_ScriptsPath, '\ObjectiveFunctions'));
    addpath(strcat(CT_ScriptsPath, '\Util'));
    % configure the static model configuration parameters and load the
    % model into the system memory
    load_system(CT_ModelFile);
    CT_CheckCorrectOutput(CT_ActualVariableName);
    run(CT_ModelConfigurationFile);
    % double the model simulation time set in the GUI because we need two values for the step fault model
    simulationTime = CT_ModelSimulationTime * 2;
    CT_SetSimulationTime(simulationTime);
    
    % retrieve the model simulation step, as it might have been changed by
    % the configuration script
    CT_ModelTimeStep = CT_GetSimulationTimeStep();
    CT_SimulationSteps=simulationTime/CT_ModelTimeStep;
    
    % compute the total number of regions
    CT_TotalRegions = CT_Regions * CT_Regions;
    
    % build the model if needed
    if (CT_ModelConfigurationFile)
        evalin('base', strcat('run(''',CT_ModelConfigurationFile,''')'));
    end
    assignin('base', CT_DesiredVariableName, CT_GenerateStepSignal(CT_SimulationSteps, CT_ModelTimeStep, 0, 0, CT_ModelSimulationTime));
    assignin('base', CT_DisturbanceVariableName, CT_GenerateConstantSignal(1, CT_SimulationSteps*CT_ModelTimeStep, 0));
    accelbuild(gcs);
        
    % pre-allocate space
    ObjectiveFunctionValues = zeros(CT_TotalRegions, CT_PointsPerRegion, 7);    
    DesiredValues = zeros(CT_TotalRegions, CT_PointsPerRegion, 2);
        
    parfor RegionCnt = 1 : CT_TotalRegions
        evalin('base', strcat('run(''',CT_ModelConfigurationFile,''')'));
        if (~strcmp(which(gcs), CT_ModelFile))
            load_system(CT_ModelFile);
            CT_SetSimulationTime(simulationTime);
        end
	
        RegionXIndex = floor((RegionCnt-1) / CT_Regions);
        RegionYIndex = floor(mod(RegionCnt-1, CT_Regions));
        
        RegionXDesiredValueRangeStart = CT_DesiredValueRangeStart + ((CT_DesiredValueRangeEnd - CT_DesiredValueRangeStart)/CT_Regions) * RegionXIndex;
        RegionXDesiredValueRangeEnd = CT_DesiredValueRangeStart + ((CT_DesiredValueRangeEnd - CT_DesiredValueRangeStart)/CT_Regions) * (RegionXIndex + 1);
        
        RegionYDesiredValueRangeStart = CT_DesiredValueRangeStart + ((CT_DesiredValueRangeEnd - CT_DesiredValueRangeStart)/CT_Regions) * RegionYIndex;
        RegionYDesiredValueRangeEnd = CT_DesiredValueRangeStart + ((CT_DesiredValueRangeEnd - CT_DesiredValueRangeStart)/CT_Regions) * (RegionYIndex + 1);
        
        % generate a random starting point p
        InitialDesiredValue = RegionXDesiredValueRangeStart + (RegionXDesiredValueRangeEnd - RegionXDesiredValueRangeStart) * rand(1);
        DesiredValue = RegionYDesiredValueRangeStart + (RegionYDesiredValueRangeEnd - RegionYDesiredValueRangeStart) * rand(1);
        
        CurrentDesiredValues = zeros(CT_PointsPerRegion, 2);
        CurrentObjectiveFunctionValues = zeros(CT_PointsPerRegion, 7);
        
        % configure the model in the worker's workspace
        
        for PointCnt = 1 : CT_PointsPerRegion
            % save the generated point p
            CurrentDesiredValues(PointCnt, :) = [InitialDesiredValue, DesiredValue];
            
            CurrentObjectiveFunctionValues(PointCnt, :) = SimulateModelStep(CT_ModelFile, InitialDesiredValue, DesiredValue, CT_ActualValueRangeStart, CT_ActualValueRangeEnd, 0, CT_SimulationSteps, CT_ModelTimeStep, CT_DesiredVariableName, CT_ActualVariableName, CT_DisturbanceVariableName, CT_TimeStable, CT_TimeStable, CT_SmoothnessStartDifference, CT_ResponsivenessClose, CT_AccelerationDisabled, CT_ModelConfigurationFile);
            
            % generate a new point p
            if CT_UseAdaptiveRandomSearch == 0
                [InitialDesiredValue, DesiredValue] = RandomExploration_GenerateNew2DPoint(CurrentDesiredValues, PointCnt, RegionXDesiredValueRangeStart, RegionXDesiredValueRangeEnd, RegionYDesiredValueRangeStart, RegionYDesiredValueRangeEnd);
            else
                [InitialDesiredValue, DesiredValue] = RandomExploration_GenerateNew2DPointAdaptive(CurrentDesiredValues, PointCnt, RegionXDesiredValueRangeStart, RegionXDesiredValueRangeEnd, RegionYDesiredValueRangeStart, RegionYDesiredValueRangeEnd);
            end
        end
        
        % copy the desired values
        DesiredValues(RegionCnt, :, :) = CurrentDesiredValues(:, :);
        % copy the objective functions
        ObjectiveFunctionValues(RegionCnt, :, :) = CurrentObjectiveFunctionValues(:, :);
    end  
    
    LimitDesiredValues = [CT_DesiredValueRangeStart, CT_DesiredValueRangeStart; CT_DesiredValueRangeStart, CT_DesiredValueRangeEnd; CT_DesiredValueRangeEnd, CT_DesiredValueRangeStart; CT_DesiredValueRangeEnd, CT_DesiredValueRangeEnd];
    LimitObjectiveFunctionValues = zeros(4, 7);
    
    % temporary variables to avoid communication overhead in parfor loop
    LimitDesiredValuesInit = LimitDesiredValues(:,1);
    LimitDesiredValuesFinal = LimitDesiredValues(:,2);
    
    for LimitTestCases = 1 : 4
        LimitObjectiveFunctionValues(LimitTestCases, :) = SimulateModelStep(CT_ModelFile, LimitDesiredValuesInit(LimitTestCases,1), LimitDesiredValuesFinal(LimitTestCases,1), CT_ActualValueRangeStart, CT_ActualValueRangeEnd, 0, CT_SimulationSteps, CT_ModelTimeStep, CT_DesiredVariableName, CT_ActualVariableName, CT_DisturbanceVariableName, CT_TimeStable, CT_TimeStable, CT_SmoothnessStartDifference, CT_ResponsivenessClose, CT_AccelerationDisabled, CT_ModelConfigurationFile);         
    end  
    
    RandomExploration_Step_SaveResults(DesiredValues, ObjectiveFunctionValues, LimitDesiredValues, LimitObjectiveFunctionValues, CT_TotalRegions, CT_PointsPerRegion, CT_TempPath);

    display('Successful termination of the random exploration process.');
catch e
    display('Error during random exploration: ');
    display(getReport(e));
end
