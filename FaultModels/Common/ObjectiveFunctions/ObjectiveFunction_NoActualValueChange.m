function [NoChange] = ObjectiveFunction_NoActualValueChange(simulationObj, tStableInitial, tChange)
    NoChange = 1;
    
    MeanActualAfterInitialDesiredValueStable = mean(simulationObj.ActualValueSignal(tChange*simulationObj.SimulationStepSize):length(simulationObj.ActualValueSignal));
    MeanActualAfterNewDesiredValue = mean(simulationObj.ActualValueSignal(tStableInitial*simulationObj.SimulationStepSize):length(simulationObj.ActualValueSignal));
    
    if (abs(MeanActualAfterInitialDesiredValueStable - MeanActualAfterNewDesiredValue) > 0.01)
        NoChange = 0;
    end
end