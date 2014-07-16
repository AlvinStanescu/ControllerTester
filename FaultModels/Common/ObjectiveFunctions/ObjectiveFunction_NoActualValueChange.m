function [NoChange, StabilityProblemsSmell] = ObjectiveFunction_NoActualValueChange(ActualValueSignal, SimulationStepSize, DesiredValue, AllowedOscillation, tStableInitial, tChange)
    NoChange = true;
    len = length(ActualValueSignal);
    StabilityProblemsSmell = false;

    MeanActualAfterNewDesiredValue = mean(ActualValueSignal(tChange/SimulationStepSize:len));
    MeanActualAfterInitialDesiredValueStable = mean(ActualValueSignal(tStableInitial/SimulationStepSize:len));
    
    if (abs(MeanActualAfterInitialDesiredValueStable - MeanActualAfterNewDesiredValue) > (MeanActualAfterInitialDesiredValueStable/10000))
        NoChange = false;
    end
    
    % if the value at the end is greater than the desired value +- 2* the allowed
    % oscillation, the controller might exhibit stability problems
    if NoChange == false && (ActualValueSignal(length(ActualValueSignal)) > DesiredValue * (100 + 2*AllowedOscillation)/100 || ActualValueSignal(length(ActualValueSignal)) < DesiredValue * (100 - 2*AllowedOscillation)/100)
        StabilityProblemsSmell = true;
    end
end