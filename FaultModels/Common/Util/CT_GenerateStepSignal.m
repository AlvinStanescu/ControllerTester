function [desiredValue] = CT_GenerateStepSignal(SimulationSteps, SimulationStepSize, InitialDesiredValue, DesiredValue, tChange)
    SimSteps = int64(SimulationSteps);
    desiredValue.time = zeros(SimSteps + 1, 1);
    desiredValue.signals.dimensions = 1;
    desiredValue.signals.values = zeros(SimSteps + 1, 1);
    indexFinalInitial = double(int64(tChange/SimulationStepSize));
    time = 0;
    
    for i = 1 : indexFinalInitial
        desiredValue.signals.values(i, 1) = InitialDesiredValue;
        desiredValue.time(i, 1) = time;
        time = time + SimulationStepSize;  
    end
    
    for i = indexFinalInitial + 1 : SimSteps + 1
        desiredValue.signals.values(i, 1) = DesiredValue;
        desiredValue.time(i, 1) = time;
        time = time + SimulationStepSize;  
    end
end