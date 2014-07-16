%% checks for liveness by computing the maximum absolute difference between desired and actual value after t_live
function [LivenessDifference] = ObjectiveFunctionCompare_Liveness(simulationObj1, simulationObj2, tLive)
    LivenessSimulation1 = ObjectiveFunction_Liveness(simulationObj1, tLive);
    LivenessSimulation2 = ObjectiveFunction_Liveness(simulationObj2, tLive);
    
    LivenessDifference = abs(LivenessSimulation1 - LivenessSimulation2);
end