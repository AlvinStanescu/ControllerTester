function [SimulationStepSize] = CT_GetSimulationTimeStep()
    hConfigurationSet = getActiveConfigSet(gcs);
    SimulationTimeStepStr=get_param(hConfigurationSet, 'FixedStep');
    if (isnumeric(SimulationTimeStepStr))
        SimulationStepSize=str2double(SimulationTimeStepStr);
    else
        SimulationStepSize=evalin('base',SimulationTimeStepStr);
    end
end