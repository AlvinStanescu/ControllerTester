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
    run(CT_ModelConfigurationFile);
    load_system(CT_ModelFile);

    % double the model simulation time set in the GUI because we need two values for the step fault model
    simulationTime = CT_ModelSimulationTime * 2;
    CT_SetSimulationTime(simulationTime);
    % retrieve the model simulation step, as it might have been changed by
    % the configuration script
    CT_ModelTimeStep = CT_GetSimulationTimeStep();
    CT_SimulationSteps=int64(CT_ModelSimulationTime/CT_ModelTimeStep);
           
    % pre-allocate space
	ObjectiveFunctionValues = zeros(7,1);

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
    ObjectiveFunctionValues(1) = ObjectiveFunction_Stability(actualValue.signals.values, CT_ModelTimeStep, CT_ModelSimulationTime + CT_TimeStable);
    ObjectiveFunctionValues(2) = ObjectiveFunction_Precision(actualValue.signals.values, CT_DesiredValue, CT_ModelTimeStep, CT_ModelSimulationTime + CT_TimeStable);
    ObjectiveFunctionValues(3) = ObjectiveFunction_Smoothness(actualValue.signals.values, CT_DesiredValue, CT_SimulationSteps/2 + 1, CT_SmoothnessStartDifference);
    ObjectiveFunctionValues(4) = ObjectiveFunction_Responsiveness(actualValue.signals.values, CT_DesiredValue, CT_ModelTimeStep, CT_SimulationSteps/2 + 1, CT_ResponsivenessClose);
    [ObjectiveFunctionValues(5), ObjectiveFunctionValues(6)] = ObjectiveFunction_Steadiness(actualValue.signals.values, CT_ModelTimeStep, CT_ModelSimulationTime + CT_TimeStable);
    ObjectiveFunctionValues(7) = ObjectiveFunction_PhysicalRange(actualValue.signals.values, CT_ActualValueRangeStart, CT_ActualValueRangeEnd);
                       
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
