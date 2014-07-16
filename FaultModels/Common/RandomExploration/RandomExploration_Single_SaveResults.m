function [ ExplorationResults ] = RandomExploration_Single_SaveResults(DesiredValues, ObjectiveFunctionValues, TotalRegions, PointsPerRegion, TempPath)
    % generates a file containing all the points found and a file
    % containing a processed view of the input space
    ExplorationResults = zeros(PointsPerRegion*TotalRegions, 8);
    
    for RegionCnt = 1 : TotalRegions
        for PointCnt = 1:PointsPerRegion
            ExplorationResults(PointCnt + PointsPerRegion * (RegionCnt-1), 1) = DesiredValues(RegionCnt, PointCnt, 1);
            ExplorationResults(PointCnt + PointsPerRegion * (RegionCnt-1), 2) = ObjectiveFunctionValues(RegionCnt, PointCnt, 1);
            ExplorationResults(PointCnt + PointsPerRegion * (RegionCnt-1), 3) = ObjectiveFunctionValues(RegionCnt, PointCnt, 2);
            ExplorationResults(PointCnt + PointsPerRegion * (RegionCnt-1), 4) = ObjectiveFunctionValues(RegionCnt, PointCnt, 3);
            ExplorationResults(PointCnt + PointsPerRegion * (RegionCnt-1), 5) = ObjectiveFunctionValues(RegionCnt, PointCnt, 4);
            ExplorationResults(PointCnt + PointsPerRegion * (RegionCnt-1), 6) = ObjectiveFunctionValues(RegionCnt, PointCnt, 5);
            ExplorationResults(PointCnt + PointsPerRegion * (RegionCnt-1), 7) = ObjectiveFunctionValues(RegionCnt, PointCnt, 6);
            ExplorationResults(PointCnt + PointsPerRegion * (RegionCnt-1), 8) = ObjectiveFunctionValues(RegionCnt, PointCnt, 7);
        end
    end
    
    
    ResultsFolderPath = strcat(TempPath, '\RandomExploration');
    if (exist(ResultsFolderPath, 'dir') ~= 7)
        mkdir(ResultsFolderPath);
    end
  
    PointsFilePath = strcat(ResultsFolderPath, '\RandomExplorationPoints_Single.csv');
    RandomExplorationResultsHeader={'Desired,Stability,Precision,Smoothness,Responsiveness,Steadiness,MeanStableValue'};
    dlmwrite(PointsFilePath, RandomExplorationResultsHeader, '');
    dlmwrite(PointsFilePath, ExplorationResults,'-append', 'delimiter', ',', 'newline', 'pc');
  
    RegionWorstValues = zeros(TotalRegions, 5);
    RegionWorstIndexes = zeros(TotalRegions, 5);
    
    RegionMeanValues = zeros(TotalRegions, 5);
    RegionPhysicalRangeExceeded = zeros(TotalRegions);

    for RegionCnt = 1:TotalRegions
        % calculate mean and worst values
        for ObjectiveFncCnt = 1:5
            for PointCnt = 1:PointsPerRegion
                    if (ObjectiveFunctionValues(RegionCnt, PointCnt, 7) == 1)
                        RegionPhysicalRangeExceeded(RegionCnt) = 1;
                    end
                val = ObjectiveFunctionValues(RegionCnt, PointCnt, ObjectiveFncCnt);
                RegionMeanValues(RegionCnt, ObjectiveFncCnt) = RegionMeanValues(RegionCnt, ObjectiveFncCnt) + val;
                if (val > RegionWorstValues(RegionCnt, ObjectiveFncCnt))
                    RegionWorstValues(RegionCnt, ObjectiveFncCnt) = val;
                    RegionWorstIndexes(RegionCnt, ObjectiveFncCnt) = DesiredValues(RegionCnt, PointCnt, 1);
                end
            end

            % finish calculating the mean
            RegionMeanValues(RegionCnt, ObjectiveFncCnt) = RegionMeanValues(RegionCnt, ObjectiveFncCnt) / PointsPerRegion;
        end
    end
    
    RegionResults = zeros(TotalRegions, 17);
    
    for RegionCnt = 1 : TotalRegions        
        RegionResults(RegionCnt, 1) = RegionCnt;
        RegionResults(RegionCnt, 2) = RegionPhysicalRangeExceeded(RegionXCnt, RegionYCnt);

        for ObjectiveFncCnt = 1 : 5
            RegionResults(RegionCnt, (ObjectiveFncCnt-1)*3 + 3) = RegionMeanValues(RegionCnt, ObjectiveFncCnt);
            RegionResults(RegionCnt, (ObjectiveFncCnt-1)*3 + 4) = RegionWorstValues(RegionCnt, ObjectiveFncCnt);
            RegionResults(RegionCnt, (ObjectiveFncCnt-1)*3 + 5) = RegionWorstIndexes(RegionCnt, ObjectiveFncCnt);
        end
    end
    
    RegionsFilePath = strcat(ResultsFolderPath, '\RandomExplorationRegions_Single.csv');
    RegionResultsHeader = {'Desired,RangeExceeded,Stability,StabilityWorst,StabilityWorstDesired,Precision,PrecisionWorst,PrecisionWorstDesired,Smoothness,SmoothnessWorst,SmoothnessWorstDesired,Responsiveness,ResponsivenessWorst,ResponsivenessWorstDesired,Steadiness,SteadinessWorst,SteadinessWorstDesired'};
    dlmwrite(RegionsFilePath, RegionResultsHeader,'');
    dlmwrite(RegionsFilePath, RegionResults,'-append', 'delimiter', ',', 'newline', 'pc');

end

