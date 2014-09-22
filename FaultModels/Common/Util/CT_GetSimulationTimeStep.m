function [SimulationStepSize, IsVariableStep] = CT_GetSimulationTimeStep()
    hConfigurationSet = getActiveConfigSet(gcs);
    if (strcmp(get_param(hConfigurationSet, 'SolverType'),'Fixed-step'))
        IsVariableStep = false;
        SimulationTimeStepStr=get_param(hConfigurationSet, 'FixedStep');
        if (isnumeric(SimulationTimeStepStr))
            SimulationStepSize=str2double(SimulationTimeStepStr);
        else
            SimulationStepSize=evalin('base',SimulationTimeStepStr);
        end
    else
        IsVariableStep = true;
        SimulationTimeStepStr=get_param(hConfigurationSet,'MaxStep');
        if (isnumeric(SimulationTimeStepStr))
            SimulationStepSize=str2double(SimulationTimeStepStr);
        else
            if (strcmp(SimulationTimeStepStr,'auto'))
                SimulationStepSize = (str2double(get_param(hConfigurationSet, 'StopTime')) - str2double(get_param(hConfigurationSet, 'StartTime')))/50;
            else
                SimulationStepSize = evalin('base',SimulationTimeStepStr);
            end            
        end
    end
end