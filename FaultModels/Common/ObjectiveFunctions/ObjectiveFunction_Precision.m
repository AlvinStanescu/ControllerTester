%% checks for liveness by computing the maximum absolute difference between desired and actual value (the steady-state error) after t_live
function [LivenessValue] = ObjectiveFunction_Precision(actualValue, DesiredValue, indexLivenessStart)
    indexLivenessEnd = length(actualValue.signals.values);
    
    LivenessValue = 0;
    for i = indexLivenessStart : indexLivenessEnd
        NewLivenessValue = abs(DesiredValue-actualValue.signals.values(i));
        if (NewLivenessValue > LivenessValue)
            LivenessValue = NewLivenessValue;
        end
    end
end