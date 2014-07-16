% calculates the oscillation in actual value after tStable
function [OscillationDifference] = ObjectiveFunctionCompare_Oscillation(simulationObj1, simulationObj2, tStable)
    Oscillation1 = ObjectiveFunction_Oscillation(simulationObj1, tStable);
    Oscillation2 = ObjectiveFunction_Oscillation(simulationObj2, tStable);
    
    OscillationDifference = abs(Oscillation1 - Oscillation2);
end