function [ StabilityDifference ] = ObjectiveFunctionCompare_Stability(simulationObj1, simulationObj2, tStable)
    StabilitySimulation1 = ObjectiveFunction_Stability(simulationObj1, tStable);
    StabilitySimulation2 = ObjectiveFunction_Stability(simulationObj2, tStable);
    
    StabilityDifference = abs(StabilitySimulation1 - StabilitySimulation2);
end

