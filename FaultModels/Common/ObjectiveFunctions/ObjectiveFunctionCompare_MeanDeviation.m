function [MeanDeviation] = ObjectiveFunctionCompare_MeanDeviation(simulationObj1, simulationObj2)
    MeanDeviation = 0;
    lengthSignal = length(simulationObj1.ActualValueSignal);

    for i = 1 : lengthSignal
        Deviation = abs(simulationObj1.ActualValueSignal - simulationObj2.ActualValueSignal);
        MeanDeviation = MeanDeviation + Deviation;
    end

    MeanDeviation = MeanDeviation / lengthSignal;
end