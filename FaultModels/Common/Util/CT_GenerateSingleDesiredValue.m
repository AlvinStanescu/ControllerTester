function [desiredValue] = CT_GenerateSingleDesiredValue(SimulationSteps, SimulationStepSize, DesiredValue)
    desiredValue.time = zeros(SimulationSteps + 1, 1);
    desiredValue.signals.dimensions = 1;
    desiredValue.signals.values = zeros(SimulationSteps + 1, 1);
    time = 0;
    
    for i = 1 : SimulationSteps + 1
        desiredValue.signals.values(i, 1) = DesiredValue;
        desiredValue.time(i, 1) = time;
        time = time + SimulationStepSize;  
    end
end
