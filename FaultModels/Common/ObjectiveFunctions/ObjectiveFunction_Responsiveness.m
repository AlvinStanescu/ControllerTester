%% checks for responsiveness by computing the time it takes for the actual value to w at x% of the desired value
function [ResponsivenessValue] = ObjectiveFunction_Responsiveness(actualValue, DesiredValue, indexStart, responsivenessClose)
    indexFinal = length(actualValue.signals.values);

    for ResponsivenessIndex = indexStart : indexFinal
        if (abs(DesiredValue - actualValue.signals.values(ResponsivenessIndex)) <= responsivenessClose)
            break            
        end
    end
    ResponsivenessValue = actualValue.time(ResponsivenessIndex) - actualValue.time(indexStart);

end