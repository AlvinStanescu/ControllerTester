function CT_SetSimulationTime(tSim)
    hConfigurationSet = getActiveConfigSet(gcs);
    set_param(hConfigurationSet, 'StopTime', num2str(tSim));
end