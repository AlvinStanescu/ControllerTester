%% checks whether the physical range of the plant was exceeded
function [IsInPhysicalRange] = ObjectiveFunction_PhysicalRange(ActualValues, rangeStart, rangeEnd)
    IsInPhysicalRange = 1;    
    indexFinal = length(ActualValues);

    for i = 1 : indexFinal
        if (ActualValues(i) > rangeEnd || ActualValues(i) < rangeStart)
            IsInPhysicalRange = 0;
        end
    end
end