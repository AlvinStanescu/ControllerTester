% calculates the oscillation in actual value after tStable
function [Oscillation, MeanStableValue] = ObjectiveFunction_Oscillation(ActualValues, SimulationStepSize, tStable)
    valuesCnt = length(ActualValues);
    MinValue = ActualValues(valuesCnt);
    MaxValue = ActualValues(valuesCnt);
    MeanValue = 0;
    
    indexStart = valuesCnt - round(valuesCnt-tStable/SimulationStepSize);
    indexEnd = valuesCnt;
    
    for i = indexStart : indexEnd
        MeanValue = MeanValue + ActualValues(i);
        if (ActualValues(i) > MaxValue)
            MaxValue = ActualValues(i);
        end
        if (ActualValues(i) < MinValue)
            MinValue = ActualValues(i);
        end
    end
    MeanValue = MeanValue / (indexEnd - indexStart + 1);
    
    MeanStableValue = MeanValue;
    Oscillation = max(abs(MeanValue-MinValue)/MeanValue, abs(MeanValue-MaxValue)/MeanValue);
end