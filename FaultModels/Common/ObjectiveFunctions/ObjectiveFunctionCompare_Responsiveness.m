function [ResponsivenessDifference] = ObjectiveFunctionCompare_Responsiveness(simulationObj1, simulationObj2, indexStart, percentClose)
    ResponsivenessSimulation1 = ObjectiveFunction_Responsiveness(simulationObj1, indexStart, percentClose);
    ResponsivenessSimulation2 = ObjectiveFunction_Responsiveness(simulationObj2, indexStart, percentClose);
    
    ResponsivenessDifference = abs(ResponsivenessSimulation1 - ResponsivenessSimulation2);
end