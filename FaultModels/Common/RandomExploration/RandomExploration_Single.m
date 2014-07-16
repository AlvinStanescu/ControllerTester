%  RandomExploration_Single
%
%  Executes the model in an explorative manner, attempting to provide a 
%  view of the input space
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
    run(CT_ModelConfigurationFile);
    % double the model simulation time set in the GUI because we need two values for the step fault model
    simulationTime = CT_ModelSimulationTime * 2;
    CT_SetSimulationTime(simulationTime);
    
    % retrieve the model simulation step, as it might have been changed by
    % the configuration script
    CT_ModelTimeStep = CT_GetSimulationTimeStep();
    CT_SimulationSteps=int64(simulationTime/CT_ModelTimeStep);
    
    % pre-allocate space
    ObjectiveFunctionValues = zeros(CT_Regions, CT_PointsPerRegion, 7);    
    DesiredValues = zeros(CT_Regions, CT_PointsPerRegion, 1);
        
    parfor RegionCnt = 1 : CT_Regions
        evalin('base', strcat('run(''',CT_ModelConfigurationFile,''')'));

        RegionDesiredValueRangeStart = CT_DesiredValueRangeStart + ((CT_DesiredValueRangeEnd - CT_DesiredValueRangeStart)/CT_Regions) * RegionCnt;
        RegionDesiredValueRangeEnd = CT_DesiredValueRangeStart + ((CT_DesiredValueRangeEnd - CT_DesiredValueRangeStart)/CT_Regions) * (RegionCnt + 1);
        
        % generate a random starting point p
        DesiredValue = RegionDesiredValueRangeStart + (RegionDesiredValueRangeEnd - RegionDesiredValueRangeStart) * rand(1);
        
        CurrentDesiredValues = zeros(CT_PointsPerRegion, 1);
        CurrentObjectiveFunctionValues = zeros(CT_PointsPerRegion, 7);
        
        % configure the model in the worker's workspace
        
        for PointCnt = 1 : CT_PointsPerRegion
            % save the generated point p
            CurrentDesiredValues(PointCnt, :) = DesiredValue;
            
            % simulate the model and capture the results
            CurrentObjectiveFunctionValues(PointCnt, :) = SimulateModelSingle(CT_ModelFile, DesiredValue, CT_ActualValueRangeStart, CT_ActualValueRangeEnd, 0, CT_SimulationSteps, CT_ModelTimeStep, CT_DesiredVariableName, CT_ActualVariableName, CT_TimeStable, CT_TimeStable, CT_SmoothnessStartDifference, CT_ResponsivenessClose, CT_AccelerationDisabled, CT_ModelConfigurationFile);
            
            % generate a new point p
            if CT_UseAdaptiveRandomSearch == 0
                DesiredValue = RandomExploration_Single_RandomSearchGenerateNewPoint(CurrentDesiredValues, PointCnt, RegionDesiredValueRangeStart, RegionDesiredValueRangeEnd);
            else
                DesiredValue = RandomExploration_Single_AdaptiveRandomSearchGenerateNewPoint(CurrentDesiredValues, PointCnt, RegionDesiredValueRangeStart, RegionDesiredValueRangeEnd);
            end
        end
        
        % copy the desired values
        DesiredValues(RegionCnt, :, :) = CurrentDesiredValues(:, :);
        % copy the objective functions
        ObjectiveFunctionValues(RegionCnt, :, :) = CurrentObjectiveFunctionValues(:, :);
    end  
    
    RandomExploration_Single_SaveResults(DesiredValues, ObjectiveFunctionValues, CT_Regions, CT_PointsPerRegion, CT_TempPath);

    display('Successful termination of the random exploration process.');
catch e
    display('Error during random exploration: ');
    display(getReport(e));
end
