%% checks for responsiveness by computing the time it takes for the actual value to w at x% of the desired value
function [ResponsivenessValue] = ObjectiveFunction_Responsiveness(ActualValues, DesiredValue, SimulationStepSize, indexStart, percentClose)
    ResponsivenessValue = 0;
    indexFinal = length(ActualValues);
   if (DesiredValue == 0)
        DesiredValue = 0.0001;
    end
    for i = indexStart : indexFinal
        if (abs(DesiredValue - ActualValues(i))/DesiredValue <= percentClose)
            break            
        end
        ResponsivenessValue = ResponsivenessValue + SimulationStepSize;
    end
    if (abs(DesiredValue - ActualValues(indexStart)) < 0.01)
        ResponsivenessValue = 0;
    end

end