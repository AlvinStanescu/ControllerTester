function [desiredValue] = CT_GeneratePulseSignal(SimulationSteps, SimulationStepSize, InitialDesiredValue, DesiredValue, tChange, tHigh)
    SimSteps = int64(SimulationSteps);
    desiredValue.time = zeros(SimSteps + 1, 1);
    desiredValue.signals.dimensions = 1;
    desiredValue.signals.values = zeros(SimSteps + 1, 1);
    indexFinalLow = int64(tChange/SimulationStepSize);
    indexFinalHigh = int64((tChange+tHigh)/SimulationStepSize);
    time = 0;
    
    for i = 1 : indexFinalLow
        desiredValue.signals.values(i, 1) = InitialDesiredValue;
        desiredValue.time(i, 1) = time;
        time = time + SimulationStepSize;  
    end
    
    for i = indexFinalLow + 1 : indexFinalHigh
        desiredValue.signals.values(i, 1) = DesiredValue;
        desiredValue.time(i, 1) = time;
        time = time + SimulationStepSize;  
    end
    
    for i = indexFinalHigh + 1: SimSteps + 1
        desiredValue.signals.values(i, 1) = InitialDesiredValue;
        desiredValue.time(i, 1) = time;
        time = time + SimulationStepSize;  
    end
end