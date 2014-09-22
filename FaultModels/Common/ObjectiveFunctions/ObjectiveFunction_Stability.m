%% checks for stability after t_stable by computing the standard deviation
function [StabilityValue] = ObjectiveFunction_Stability(actualValue, indexStableStart)  
    StabilityValue = std(actualValue.signals.values(indexStableStart : length(actualValue.signals.values)));
end