function [SmoothnessDifference] = ObjectiveFunctionCompare_Smoothness(simulationObj1, simulationObj2, indexStart, startDifference)
    SmoothnessSimulation1 = ObjectiveFunction_Smoothness(simulationObj1, indexStart, startDifference);
    SmoothnessSimulation2 = ObjectiveFunction_Smoothness(simulationObj2, indexStart, startDifference);
    
    SmoothnessDifference = abs(SmoothnessSimulation1 - SmoothnessSimulation2);
end