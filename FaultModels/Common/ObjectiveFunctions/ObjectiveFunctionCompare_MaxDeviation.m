function [MaxDeviation] = ObjectiveFunctionCompare_MaxDeviation(simulationObj1, simulationObj2)
    MaxDeviation = 0;
    indexFinal = length(simulationObj1.ActualValueSignal);

    for i = 1 : indexFinal
        Deviation = abs(simulationObj1.ActualValueSignal - simulationObj2.ActualValueSignal);
        if (Deviation > MaxDeviation)
            MaxDeviation = Deviation;
        end
    end
end