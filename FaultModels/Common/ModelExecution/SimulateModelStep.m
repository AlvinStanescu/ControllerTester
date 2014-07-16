function [ObjectiveFunctionValues] = SimulateModelStep(ModelFile, InitialDesiredValue, DesiredValue, ActualValueRangeStart, ActualValueRangeEnd, ObjectiveFunction, SimulationSteps, ModelTimeStep, DesiredVariableName, ActualVariableName, tStable, tLive, smoothnessStartDifference, responsivenessClose, AccelerationDisabled, ModelConfigurationFile)
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
        ObjectiveFunctionValues = zeros(7, 1);
        ObjectiveFunctionValues(1) = ObjectiveFunction_Stability(actualValue.signals.values, ModelTimeStep, double((SimulationSteps/2 + 1)*ModelTimeStep) + tStable);
        ObjectiveFunctionValues(2) = ObjectiveFunction_Precision(actualValue.signals.values, DesiredValue, ModelTimeStep, double((SimulationSteps/2 + 1)*ModelTimeStep) + tLive);
        ObjectiveFunctionValues(3) = ObjectiveFunction_Smoothness(actualValue.signals.values, DesiredValue, SimulationSteps/2 + 1, smoothnessStartDifference);
        ObjectiveFunctionValues(4) = ObjectiveFunction_Responsiveness(actualValue.signals.values, DesiredValue, ModelTimeStep, SimulationSteps/2 + 1, responsivenessClose);
        [ObjectiveFunctionValues(5), ObjectiveFunctionValues(6)] = ObjectiveFunction_Steadiness(actualValue.signals.values, ModelTimeStep, double((SimulationSteps/2 + 1)*ModelTimeStep) + tStable);
        ObjectiveFunctionValues(7) = ObjectiveFunction_PhysicalRange(actualValue.signals.values, ActualValueRangeStart, ActualValueRangeEnd);

    else
        switch ObjectiveFunction
            case 1
                ObjectiveFunctionValues = ObjectiveFunction_Stability(actualValue.signals.values, ModelTimeStep, double((SimulationSteps/2 + 1)*ModelTimeStep) + tStable);
            case 2
                ObjectiveFunctionValues = ObjectiveFunction_Precision(actualValue.signals.values, DesiredValue, ModelTimeStep, double((SimulationSteps/2 + 1)*ModelTimeStep) + tLive);
            case 3
                ObjectiveFunctionValues = ObjectiveFunction_Smoothness(actualValue.signals.values, DesiredValue, SimulationSteps/2 + 1, smoothnessStartDifference);
            case 4
                ObjectiveFunctionValues = ObjectiveFunction_Responsiveness(actualValue.signals.values, DesiredValue, ModelTimeStep, SimulationSteps/2 + 1, responsivenessClose);
            case 5
                ObjectiveFunctionValues = ObjectiveFunction_Steadiness(actualValue.signals.values, ModelTimeStep, double((SimulationSteps/2 + 1)*ModelTimeStep) + tStable);
        end
    end
    
end