%  SingleStateSearch_Step
%
%  Executes the model with the step fault model as a basis in an
%  explorative manner, attempting to provide a view of the input space
%
%  Author: Alvin Stanescu
%

% add all the paths containing the model and the functions we use
addpath(CT_ModelPath);
addpath(strcat(CT_ScriptsPath, '\ModelExecution'));
addpath(strcat(CT_ScriptsPath, '\ObjectiveFunctions'));
addpath(strcat(CT_ScriptsPath, '\Regression'));
addpath(strcat(CT_ScriptsPath, '\Util'));

% begin logging 
CT_DiaryInit(strcat(CT_UserTempPath,'\ControllerTesterOutput.log'));
try
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
    
    % compute the region bounds
    RegionXStart = CT_DesiredValueRangeStart + ((CT_DesiredValueRangeEnd - CT_DesiredValueRangeStart)/CT_Regions) * CT_RegionXIndex;
    RegionXEnd = CT_DesiredValueRangeStart + ((CT_DesiredValueRangeEnd - CT_DesiredValueRangeStart)/CT_Regions) * (CT_RegionXIndex + 1);

    RegionYStart = CT_DesiredValueRangeStart + ((CT_DesiredValueRangeEnd - CT_DesiredValueRangeStart)/CT_Regions) * CT_RegionYIndex;
    RegionYEnd = CT_DesiredValueRangeStart + ((CT_DesiredValueRangeEnd - CT_DesiredValueRangeStart)/CT_Regions) * (CT_RegionYIndex + 1);
    
    % convert the problem to a minimization problem for use with the
    % (global) optimization toolbox
    hSimulation = @(p)(-1*SimulateModelStep(CT_ModelFile, p(1), p(2), CT_ActualValueRangeStart, CT_ActualValueRangeEnd, CT_MaxObjectiveFunctionIndex, CT_SimulationSteps, CT_ModelTimeStep, CT_DesiredVariableName, CT_ActualVariableName, CT_DisturbanceVariableName, CT_TimeStable, CT_TimeStable, CT_SmoothnessStartDifference, CT_ResponsivenessClose, CT_AccelerationDisabled, CT_ModelConfigurationFile));
    lb = [RegionXStart, RegionYStart];
    ub = [RegionXEnd, RegionYEnd];
    
    % build the model if needed
    if (CT_ModelConfigurationFile)
        evalin('base', strcat('run(''',CT_ModelConfigurationFile,''')'));
    end
    assignin('base', CT_DesiredVariableName, CT_GenerateStepSignal(CT_SimulationSteps, CT_ModelTimeStep, 0, 0, CT_ModelSimulationTime));
    assignin('base', CT_DisturbanceVariableName, CT_GenerateConstantSignal(1, CT_SimulationSteps*CT_ModelTimeStep, 0));
    accelbuild(gcs);
    error = false;
    
    switch CT_OptimizationAlgorithm 
        case 'SimulatedAnnealing'
            tic;
            
            realSaPlot = @(options,optimvalues,flag)(SingleStateSearch_LogIteration(options,optimvalues,flag,10,'SimulatedAnnealing'));
            saoptions = saoptimset('AnnealingFcn',@annealingfast,'MaxIter',1500,'OutputFcns',realSaPlot);
            [p, objectiveFunctionValue, exitFlag, output] = simulannealbnd(hSimulation, CT_StartPoint, lb, ub, saoptions);
            
            % convert the obj. function value back
            objectiveFunctionValue = -1 * objectiveFunctionValue;
            toc;
        case 'PatternSearch'
            tic;
            options = psoptimset('CompletePoll','on','UseParallel',true,'Display','iter');
            [p, objectiveFunctionValue] = patternsearch(hSimulation, CT_StartPoint, [], [], [], [], lb, ub, [], options);
            objectiveFunctionValue = -1 * objectiveFunctionValue;
            toc;
        case 'MultiStart'   
            tic;
            problem = createOptimProblem('fmincon', ...
                'objective', hSimulation, ...
                'x0', CT_StartPoint, ...
                'lb', lb, ...
                'ub', ub, ...
                'options', optimoptions(@fmincon,'Display','off'));
            ms = MultiStart('UseParallel',true,'Display','iter');
            [p, objectiveFunctionValue] = run(ms,problem,20);
            duration = toc;
            display(strcat('runningTime=', num2str(duration)));
            objectiveFunctionValue = -1 * objectiveFunctionValue;
        case 'GlobalSearch'  
            tic;
            problem = createOptimProblem('fmincon', ...
                'objective', hSimulation, ...
                'x0', CT_StartPoint, ...
                'lb', lb, ...
                'ub', ub, ...
                'options', optimoptions(@fmincon,'Display','off'));
            gs = GlobalSearch('Display','iter');
            [p, objectiveFunctionValue] = run(gs,problem);
            objectiveFunctionValue = -1 * objectiveFunctionValue;
            toc;

        case 'GeneticAlgorithm'
            %% run the genetic algorithm to find a point close to the minimum
            tic;
            rng('default') % for reproducibility
            gaoptions = gaoptimset('Generations',10,'UseParallel',true,'TolFun',1/10000, 'PlotFcns',{@gaplotbestf,@gaplotmaxconstr},'Display','iter');
            [p, objectiveFunctionValue] = ga(hSimulation, 2, [], [], [], [], lb, ub, [], gaoptions);
            disp('Genetic algorithm finished');

            %% finish it off with fmincon
            options = optimoptions(@fmincon,'UseParallel',true,'Algorithm','sqp','Display','iter');
            [p, objectiveFunctionValue] = fmincon(hSimulation, p, [], [], [], [], [RegionXStart RegionYStart], [RegionXEnd RegionYEnd], [],options);
            disp('Fmincon finished');
            objectiveFunctionValue = -1 * objectiveFunctionValue;
            toc;

    end
    
    if ~error
	    SingleStateSearch_Step_SaveResults(p, objectiveFunctionValue, CT_TempPath);
        display('Successful termination of the random exploration process.');
    end
    diary off;
catch e
    display('Error during random exploration: ');
    display(getReport(e));
    diary off;
end
