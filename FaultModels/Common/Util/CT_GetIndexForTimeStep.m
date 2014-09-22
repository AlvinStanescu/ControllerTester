function [index] = CT_GetIndexForTimeStep(timeArray, searchedTime)
    % get the starting index for stability, precision and steadiness
    signalLength = length(timeArray);
    for index = 1 : signalLength
        if timeArray(index) >= searchedTime
            break;
        end
    end
end