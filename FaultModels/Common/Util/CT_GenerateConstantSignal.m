function [desiredValue] = CT_GenerateConstantSignal(SimulationSteps, SimulationStepSize, DesiredValue)
    SimSteps = int64(SimulationSteps);
    desiredValue.time = zeros(SimSteps + 1, 1);
    desiredValue.signals.dimensions = 1;
    desiredValue.signals.values = zeros(SimSteps + 1, 1);
    time = 0;
    
    for i = 1 : SimSteps + 1
        desiredValue.signals.values(i, 1) = DesiredValue;
        desiredValue.time(i, 1) = time;
        time = time + SimulationStepSize;  
    end
end
