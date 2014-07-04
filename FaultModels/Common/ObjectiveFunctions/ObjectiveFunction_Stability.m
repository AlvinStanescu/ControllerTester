%% checks for stability after t_stable by computing the standard deviation
function [StabilityValue] = ObjectiveFunction_Stability(ActualValues, SimulationStepSize, tStable)
    indexStableStart = length(ActualValues) - round(length(ActualValues)-tStable/SimulationStepSize);
    
    StabilityValue = std(ActualValues(indexStableStart : length(ActualValues)));
end