function [desiredValue] = CT_GenerateStepDesiredValue(SimulationSteps, SimulationStepSize, InitialDesiredValue, DesiredValue)
    desiredValue.time = zeros(SimulationSteps + 1, 1);
    desiredValue.signals.dimensions = 1;
    desiredValue.signals.values = zeros(SimulationSteps + 1, 1);
    indexFinalInitial = SimulationSteps/2;
    time = 0;
    
    for i = 1 : indexFinalInitial
        desiredValue.signals.values(i, 1) = InitialDesiredValue;
        desiredValue.time(i, 1) = time;
        time = time + SimulationStepSize;  
    end
    
    for i = indexFinalInitial + 1 : SimulationSteps + 1
        desiredValue.signals.values(i, 1) = DesiredValue;
        desiredValue.time(i, 1) = time;
        time = time + SimulationStepSize;  
    end
end