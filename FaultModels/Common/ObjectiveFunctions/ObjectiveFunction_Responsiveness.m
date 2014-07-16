%% checks for responsiveness by computing the time it takes for the actual value to w at x% of the desired value
function [ResponsivenessValue] = ObjectiveFunction_Responsiveness(ActualValues, DesiredValue, SimulationStepSize, indexStart, responsivenessClose)
    ResponsivenessValue = 0;
    indexFinal = length(ActualValues);

    for i = indexStart : indexFinal
        if (abs(DesiredValue - ActualValues(i)) <= responsivenessClose)
            break            
        end
        ResponsivenessValue = ResponsivenessValue + SimulationStepSize;
    end

end