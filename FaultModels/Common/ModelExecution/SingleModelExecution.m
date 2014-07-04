% this script executes the model with a single uniform desired value
% there is no inherent fault model behind it, its main purpose being to
% verify that simulating the system works as intended
diary(strcat(CT_ScriptsPath,'/Temp/output.log'));
try
    % add all the paths containing the model and the functions we use
    addpath(CT_ModelPath);
    addpath(strcat(CT_ScriptsPath, '/ObjectiveFunctions'));
    addpath(strcat(CT_ScriptsPath, '/Util'));
    % configure the static model configuration parameters and load the
    % model into the system memory
    run(CT_ModelConfigurationFile);
    load_system(CT_ModelFile);
    % change the model simulation time to conform to the one set in the GUI
    CT_SetSimulationTime(CT_ModelSimulationTime);
    % retrieve the model simulation step, as it might have been changed by
    % the configuration script
    CT_ModelTimeStep = CT_GetSimulationTimeStep();
    CT_SimulationSteps=int64(CT_ModelSimulationTime/CT_ModelTimeStep);
        
    % to be removed
    CT_DesiredVariableName = 'desiredPosition';
    CT_ActualVariableName = 'actualPosition';
    
    % pre-allocate space
	ObjectiveFunctionValues = zeros(5,1);

    % start the timer to measure the running time of the model together
    % with the objective function computation
    tic;
    % generate the time for the desired value  
    assignin('base', CT_DesiredVariableName, CT_GenerateSingleDesiredValue(CT_SimulationSteps, CT_ModelTimeStep, CT_DesiredValue));
            
    % run the simulation in accelerated mode
    if (CT_AccelerationDisabled)
        simOut = sim(CT_ModelFile, 'SaveOutput','on');
    else
        simOut = sim(CT_ModelFile, 'SimulationMode', 'accelerator', 'SaveOutput','on');
    end

    actualValue = simOut.get(CT_ActualVariableName);
            
    % calculate the objective functions
    ObjectiveFunctionValues = zeros(6, 1);
    ObjectiveFunctionValues(1) = ObjectiveFunction_Stability(actualValue.signals.values, CT_ModelTimeStep, 11); % tStable
    ObjectiveFunctionValues(2) = ObjectiveFunction_Liveness(actualValue.signals.values, CT_DesiredValue, CT_ModelTimeStep, 11); % tLive
    ObjectiveFunctionValues(3) = ObjectiveFunction_Smoothness(actualValue.signals.values, CT_DesiredValue, 1, 0.02); % indexStart, startDifference
    ObjectiveFunctionValues(4) = ObjectiveFunction_Responsiveness(actualValue.signals.values, CT_DesiredValue, CT_ModelTimeStep, 1, 0.05); % indexStart, percentClose
    [ObjectiveFunctionValues(5), ObjectiveFunctionValues(6)] = ObjectiveFunction_Oscillation(actualValue.signals.values, CT_ModelTimeStep, 11); % tStable
                       
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
diary off;