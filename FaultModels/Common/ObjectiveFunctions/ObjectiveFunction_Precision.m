%% checks for liveness by computing the maximum absolute difference between desired and actual value (the steady-state error) after t_live
function [LivenessValue] = ObjectiveFunction_Precision(ActualValues, DesiredValue, SimulationStepSize, tLive)
    indexLivenessStart = length(ActualValues) - round(length(ActualValues)-tLive/SimulationStepSize);
    indexLivenessEnd = length(ActualValues);

    LivenessValue = 0;
    for i = indexLivenessStart : indexLivenessEnd
        NewLivenessValue = abs(DesiredValue-ActualValues(i));
        if (NewLivenessValue > LivenessValue)
            LivenessValue = NewLivenessValue;
        end
    end
end