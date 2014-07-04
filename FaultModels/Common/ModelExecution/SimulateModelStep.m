function [ObjectiveFunctionValues] = SimulateModelStep(ModelFile, InitialDesiredValue, DesiredValue, ObjectiveFunction, SimulationSteps, ModelTimeStep, DesiredVariableName, ActualVariableName, tStable, tLive, smoothnessStartDifference, responsivenessPercentClose, AccelerationDisabled, ModelConfigurationFile)
    % generate the time for the desired value  
    if (ModelConfigurationFile)
        evalin('base', strcat('run(''',ModelConfigurationFile,''')'));
    end
    assignin('base', DesiredVariableName, CT_GenerateStepDesiredValue(SimulationSteps, ModelTimeStep, InitialDesiredValue, DesiredValue));
            
    % run the simulation in accelerated mode
    if (AccelerationDisabled)
        simOut = sim(ModelFile, 'SaveOutput', 'on');
    else
        simOut = sim(ModelFile, 'SimulationMode', 'accelerator', 'SaveOutput', 'on');
    end

    actualValue = simOut.get(ActualVariableName);
            
    % calculate the objective functions
    if ObjectiveFunction == 0
        ObjectiveFunctionValues = zeros(6, 1);
        ObjectiveFunctionValues(1) = ObjectiveFunction_Stability(actualValue.signals.values, ModelTimeStep, tStable); % tStable
        ObjectiveFunctionValues(2) = ObjectiveFunction_Liveness(actualValue.signals.values, DesiredValue, ModelTimeStep, tLive); % tLive
        ObjectiveFunctionValues(3) = ObjectiveFunction_Smoothness(actualValue.signals.values, DesiredValue, SimulationSteps/2 + 1, smoothnessStartDifference); % indexStart, startDifference
        ObjectiveFunctionValues(4) = ObjectiveFunction_Responsiveness(actualValue.signals.values, DesiredValue, ModelTimeStep, SimulationSteps/2 + 1, responsivenessPercentClose); % indexStart, percentClose
        [ObjectiveFunctionValues(5), ObjectiveFunctionValues(6)] = ObjectiveFunction_Oscillation(actualValue.signals.values, ModelTimeStep, tStable); % tStable
    else
        switch ObjectiveFunction
            case 1
                ObjectiveFunctionValues = ObjectiveFunction_Stability(actualValue.signals.values, ModelTimeStep, tStable); % tStable
            case 2
                ObjectiveFunctionValues = ObjectiveFunction_Liveness(actualValue.signals.values, DesiredValue, ModelTimeStep, tLive); % tLive
            case 3
                ObjectiveFunctionValues = ObjectiveFunction_Smoothness(actualValue.signals.values, DesiredValue, SimulationSteps/2 + 1, smoothnessStartDifference); % indexStart, startDifference
            case 4
                ObjectiveFunctionValues = ObjectiveFunction_Responsiveness(actualValue.signals.values, DesiredValue, ModelTimeStep, SimulationSteps/2 + 1, responsivenessPercentClose); % indexStart, percentClose
            case 5
                ObjectiveFunctionValues = ObjectiveFunction_Oscillation(actualValue.signals.values, ModelTimeStep, tStable); % tStable
        end
    end
    
end