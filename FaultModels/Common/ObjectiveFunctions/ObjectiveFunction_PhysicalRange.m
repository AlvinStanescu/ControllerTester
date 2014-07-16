%% checks whether the physical range of the plant was exceeded
function [PhysicalRangeExceeded] = ObjectiveFunction_PhysicalRange(ActualValues, rangeStart, rangeEnd)
    PhysicalRangeExceeded = 0;    
    indexFinal = length(ActualValues);

    for i = 1 : indexFinal
        if (ActualValues(i) > rangeEnd || ActualValues(i) < rangeStart)
            PhysicalRangeExceeded = 1;
        end
    end
end