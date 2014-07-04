function SingleStateSearch_Step_SaveResults(worstPoint, worstObjectiveFunctionValue, TempPath)
    % generates a file containing the point found
    SearchResults = zeros(1, 3);
    SearchResults(1,1) = worstPoint(1);
    SearchResults(1,2) = worstPoint(2);
    SearchResults(1,3) = worstObjectiveFunctionValue;
    
    ResultsFolderPath = strcat(TempPath, '\SingleStateSearch');
    if (exist(ResultsFolderPath, 'dir') ~= 7)
        mkdir(ResultsFolderPath);
    end
  
    PointsFilePath = strcat(ResultsFolderPath, '\SingleStateSearch_WorstCase.csv');
    ResultsHeader={'InitialDesired,Desired,ObjectiveFunctionValue'};
    dlmwrite(PointsFilePath, ResultsHeader, '');
    dlmwrite(PointsFilePath, SearchResults,'-append', 'delimiter', ',', 'newline', 'pc');
  
end

