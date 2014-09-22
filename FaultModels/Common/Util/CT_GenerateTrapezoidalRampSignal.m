function [desiredValue] = CT_GenerateTrapezoidalRampSignal(SimulationSteps, SimulationStepSize, InitialDesiredValue, DesiredValue, tChange, tDuration, tUp)
    SimSteps = int64(SimulationSteps);
    desiredValue.time = zeros(SimSteps + 1, 1);
    desiredValue.signals.dimensions = 1;
    desiredValue.signals.values = zeros(SimSteps + 1, 1);
    indexStartChange = int64(tChange/SimulationStepSize);
    indexStartMax = int64((tChange + (tDuration - tUp)/2)/SimulationStepSize);
    indexStopMax = int64((tChange + (tDuration - tUp)/2 + tUp)/SimulationStepSize);
    indexStopChange = int64((tChange + tDuration)/SimulationStepSize);
    
    time = 0;
    
    for i = 1 : indexStartChange
        desiredValue.signals.values(i, 1) = InitialDesiredValue;
        desiredValue.time(i, 1) = time;
        time = time + SimulationStepSize;  
    end
    
    for i = indexStartChange + 1 : indexStartMax
        desiredValue.signals.values(i, 1) = double(i - indexStartChange + 1)/double(indexStartMax - indexStartChange + 1) * DesiredValue;
        desiredValue.time(i, 1) = time;
        time = time + SimulationStepSize;  
    end
    
    for i = indexStartMax + 1 : indexStopMax
        desiredValue.signals.values(i, 1) = DesiredValue;
        desiredValue.time(i, 1) = time;
        time = time + SimulationStepSize;  
    end

    for i = indexStopMax + 1 : indexStopChange
        desiredValue.signals.values(i, 1) = double(indexStopChange - i)/double(indexStopChange - indexStopMax + 1) * DesiredValue;
        desiredValue.time(i, 1) = time;
        time = time + SimulationStepSize;  
    end

    
    for i = indexStopChange + 1: SimSteps + 1
        desiredValue.signals.values(i, 1) = InitialDesiredValue;
        desiredValue.time(i, 1) = time;
        time = time + SimulationStepSize;  
    end
end