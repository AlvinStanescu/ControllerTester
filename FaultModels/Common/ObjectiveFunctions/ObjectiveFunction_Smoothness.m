%% checks for smoothness by computing the maximum absolute difference between desired and actual value after t_smooth
%% t_smooth is calculated as a percentage of the desired value
function [SmoothnessValue] = ObjectiveFunction_Smoothness(actualValue, DesiredValue, indexStart, startDifference)
    wasCloseToDesiredValue = false;
    maxDifference = Inf;
    indexFinal = length(actualValue.signals.values);

    for i = indexStart : indexFinal
        difference = abs(DesiredValue-actualValue.signals.values(i));
        if (wasCloseToDesiredValue)
            if (difference > maxDifference)
                maxDifference = difference;
            end
        else
            if (difference <= startDifference)
                wasCloseToDesiredValue = true;
                maxDifference = difference;
            end
        end
    end
    
    SmoothnessValue = maxDifference;
end