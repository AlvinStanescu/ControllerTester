function [desiredValue] = CT_GenerateSineSignal(SimulationSteps, SimulationStepSize, DesiredValue, Amplitude, Frequency)
    SimSteps = int64(SimulationSteps);
    desiredValue.time = zeros(SimSteps + 1, 1);
    desiredValue.signals.dimensions = 1;
    desiredValue.signals.values = zeros(SimSteps + 1, 1);
    time = 0;
   
    for i = 1 : SimSteps + 1
        desiredValue.time(i, 1) = time;
        time = time + SimulationStepSize;  
    end
    desiredValue.signals.values = DesiredValue + Amplitude*sin(2*pi*Frequency*desiredValue.time);

end