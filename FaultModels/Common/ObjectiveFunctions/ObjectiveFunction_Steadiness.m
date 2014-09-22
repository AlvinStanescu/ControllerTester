% calculates the max. oscillation in the actual value after tStable
function [Steadiness, MeanStableValue] = ObjectiveFunction_Steadiness(actualValue, indexStart)   
    valuesCnt = length(actualValue.signals.values);
    MinValue = actualValue.signals.values(valuesCnt);
    MaxValue = actualValue.signals.values(valuesCnt);
    MeanValue = 0;
    
    indexEnd = valuesCnt;
    
    for i = indexStart : indexEnd
        MeanValue = MeanValue + actualValue.signals.values(i);
        if (actualValue.signals.values(i) > MaxValue)
            MaxValue = actualValue.signals.values(i);
        end
        if (actualValue.signals.values(i) < MinValue)
            MinValue = actualValue.signals.values(i);
        end
    end
    MeanValue = MeanValue / (indexEnd - indexStart + 1);
    
    MeanStableValue = MeanValue;
    Steadiness = MaxValue - MinValue;
end