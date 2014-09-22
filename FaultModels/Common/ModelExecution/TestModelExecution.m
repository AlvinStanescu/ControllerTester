% this script executes the model with a single uniform desired value
% there is no inherent fault model behind it, its main purpose being to
% verify that simulating the system works as intended
try
    % add all the paths containing the model and the functions we use
    addpath(CT_ModelPath);
    addpath(strcat(CT_ScriptsPath, '/ObjectiveFunctions'));
    addpath(strcat(CT_ScriptsPath, '/Util'));

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
           
    % pre-allocate space
	ObjectiveFunctionValues = zeros(7,1);
    
    % build the model if needed
    if (CT_ModelConfigurationFile)
        evalin('base', strcat('run(''',CT_ModelConfigurationFile,''')'));
    end
    
    assignin('base', CT_DesiredVariableName, CT_GenerateStepSignal(CT_SimulationSteps, CT_ModelTimeStep, 0, 0, CT_ModelSimulationTime));
    assignin('base', CT_DisturbanceVariableName, CT_GenerateConstantSignal(1, CT_SimulationSteps*CT_ModelTimeStep, 0));
    accelbuild(gcs);
    
    % start the timer to measure the running time of the model together
    % with the objective function computation
    tic;
    % generate the time for the desired value  
    assignin('base', CT_DesiredVariableName, CT_GenerateStepSignal(CT_SimulationSteps, CT_ModelTimeStep, CT_InitialDesiredValue, CT_DesiredValue, CT_ModelSimulationTime));
            
    % run the simulation in accelerated mode
    if (CT_AccelerationDisabled)
        simOut = sim(CT_ModelFile, 'SaveOutput','on');
    else
        simOut = sim(CT_ModelFile, 'SimulationMode', 'accelerator', 'SaveOutput','on');
    end

    actualValue = simOut.get(CT_ActualVariableName);    
    
    % get the starting index for stability, precision and steadiness
    indexStableStart = CT_GetIndexForTimeStep(actualValue.time, CT_ModelSimulationTime + CT_TimeStable);
    % get the starting index for smoothness and responsiveness
    indexMidStart = CT_GetIndexForTimeStep(actualValue.time, CT_ModelSimulationTime);

    
    % calculate the objective functions
    ObjectiveFunctionValues(1) = ObjectiveFunction_Stability(actualValue, indexStableStart);
    ObjectiveFunctionValues(2) = ObjectiveFunction_Precision(actualValue, CT_DesiredValue, indexStableStart);
    ObjectiveFunctionValues(3) = ObjectiveFunction_Smoothness(actualValue, CT_DesiredValue, indexMidStart, CT_SmoothnessStartDifference);
    ObjectiveFunctionValues(4) = ObjectiveFunction_Responsiveness(actualValue, CT_DesiredValue, indexMidStart, CT_ResponsivenessClose);
    [ObjectiveFunctionValues(5), ObjectiveFunctionValues(6)] = ObjectiveFunction_Steadiness(actualValue, indexStableStart);
    ObjectiveFunctionValues(7) = ObjectiveFunction_PhysicalRange(actualValue, CT_ActualValueRangeStart, CT_ActualValueRangeEnd);
                       
    % stop the timer
    duration = toc;
    % output the model running time (?)
    display('Successful execution of the model');
    display(strcat('runningTime=', num2str(duration)));
catch e
    display('Error during model execution');
    display(getReport(e));
end
diary off;
