%% checks whether the physical range of the plant was exceeded
function [PhysicalRangeExceeded] = ObjectiveFunction_PhysicalRange(actualValue, rangeStart, rangeEnd)
    PhysicalRangeExceeded = 0;    
    indexFinal = length(actualValue.signals.values);

    for i = 1 : indexFinal
        if (actualValue.signals.values(i) > rangeEnd || actualValue.signals.values(i) < rangeStart)
            PhysicalRangeExceeded = 1;
        end
    end
end