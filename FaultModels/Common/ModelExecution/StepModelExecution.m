% this script executes the model with the step fault model as a basis
try
    % add all the paths containing the model and the functions we use
    addpath(CT_ModelPath);
    addpath(strcat(CT_ScriptsPath, '/ObjectiveFunctions'));
    addpath(strcat(CT_ScriptsPath, '/Util'));
    % configure the static model configuration parameters and load the
    % model into the system memory
    run(CT_ModelConfigurationFile);
    load_system(CT_ModelFile);
    % double the model simulation time set in the GUI because we need two values for the step fault model
    simulationTime = CT_ModelSimulationTime * 2;
    CT_SetSimulationTime(simulationTime);
    % retrieve the model simulation step, as it might have been changed by
    % the configuration script
    CT_ModelTimeStep = CT_GetSimulationTimeStep();
    CT_SimulationSteps=int64(simulationTime/CT_ModelTimeStep);
    
    % pre-allocate space
	ObjectiveFunctionValues = zeros(6,1);

    % start the timer to measure the running time of the model together
    % with the objective function computation
    tic;
    % generate the time for the desired value  
    assignin('base', CT_DesiredVariableName, CT_GenerateStepDesiredValue(CT_SimulationSteps, CT_ModelTimeStep, CT_InitialDesiredValue, CT_DesiredValue));
            
    % run the simulation in accelerated mode
    if (CT_AccelerationDisabled)
        simOut = sim(CT_ModelFile, 'SaveOutput','on');
    else
        simOut = sim(CT_ModelFile, 'SimulationMode', 'accelerator', 'SaveOutput','on');
    end

    actualValue = simOut.get(CT_ActualVariableName);
    
    % calculate the objective functions
    ObjectiveFunctionValues = zeros(6, 1);
    ObjectiveFunctionValues(1) = ObjectiveFunction_Stability(actualValue.signals.values, CT_ModelTimeStep, CT_TimeStable); % tStable
    ObjectiveFunctionValues(2) = ObjectiveFunction_Liveness(actualValue.signals.values, CT_DesiredValue, CT_ModelTimeStep, CT_TimeStable); % tLive
    ObjectiveFunctionValues(3) = ObjectiveFunction_Smoothness(actualValue.signals.values, CT_DesiredValue, CT_SimulationSteps/2 + 1, CT_SmoothnessStartDifference); % indexStart, startDifference
    ObjectiveFunctionValues(4) = ObjectiveFunction_Responsiveness(actualValue.signals.values, CT_DesiredValue, CT_ModelTimeStep, CT_SimulationSteps/2 + 1, CT_ResponsivenessPercentClose); % indexStart, percentClose
    [ObjectiveFunctionValues(5), ObjectiveFunctionValues(6)] = ObjectiveFunction_Oscillation(actualValue.signals.values, CT_ModelTimeStep, CT_TimeStable); % tStable
 
    % stop the timer
    duration = toc;
    % output the model running time (?)
    display('Successful execution of the model');
    display(strcat('runningTime=', num2str(duration)));
    % plot the result
    eval(strcat('plot(', CT_DesiredVariableName,'.time,', CT_DesiredVariableName, '.signals.values,', CT_DesiredVariableName, '.time, actualValue.signals.values)'));
catch e
    display('Error during model execution');
    display(getReport(e));
end
