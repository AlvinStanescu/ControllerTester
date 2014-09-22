function [NoChange, StabilityProblemsSmell] = ObjectiveFunction_NoActualValueChange(ActualValueSignal, DesiredValue, AllowedOscillation, indexStableStart, indexChange)
    NoChange = true;
    len = length(ActualValueSignal);
    StabilityProblemsSmell = false;

    MeanActualAfterNewDesiredValue = mean(ActualValueSignal(indexChange:len));
    MeanActualAfterInitialDesiredValueStable = mean(ActualValueSignal(indexStableStart:len));
    
    if (abs(MeanActualAfterInitialDesiredValueStable - MeanActualAfterNewDesiredValue) > (MeanActualAfterInitialDesiredValueStable/10000))
        NoChange = false;
    end
    
    % if the value at the end is greater than the desired value +- 2* the allowed
    % oscillation, the controller might exhibit stability problems
    if NoChange == false && (ActualValueSignal(length(ActualValueSignal)) > DesiredValue * (100 + 2*AllowedOscillation)/100 || ActualValueSignal(length(ActualValueSignal)) < DesiredValue * (100 - 2*AllowedOscillation)/100)
        StabilityProblemsSmell = true;
    end
end