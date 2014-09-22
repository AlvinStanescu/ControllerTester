function [ ExplorationResults ] = RandomExploration_Step_SaveResults(DesiredValues, ObjectiveFunctionValues, LimitDesiredValues, LimitObjectiveFunctionValues, TotalRegions, PointsPerRegion, TempPath)
    % generates a file containing all the points found and a file
    % containing a processed view of the input space
    ExplorationResults = zeros(PointsPerRegion*TotalRegions+4, 9);
    
    for RegionCnt = 1 : TotalRegions
        for PointCnt = 1 : PointsPerRegion
            ExplorationResults(PointCnt + PointsPerRegion * (RegionCnt-1), 1) = DesiredValues(RegionCnt, PointCnt, 1);
            ExplorationResults(PointCnt + PointsPerRegion * (RegionCnt-1), 2) = DesiredValues(RegionCnt, PointCnt, 2);
            ExplorationResults(PointCnt + PointsPerRegion * (RegionCnt-1), 3) = ObjectiveFunctionValues(RegionCnt, PointCnt, 1);
            ExplorationResults(PointCnt + PointsPerRegion * (RegionCnt-1), 4) = ObjectiveFunctionValues(RegionCnt, PointCnt, 2);
            ExplorationResults(PointCnt + PointsPerRegion * (RegionCnt-1), 5) = ObjectiveFunctionValues(RegionCnt, PointCnt, 3);
            ExplorationResults(PointCnt + PointsPerRegion * (RegionCnt-1), 6) = ObjectiveFunctionValues(RegionCnt, PointCnt, 4);
            ExplorationResults(PointCnt + PointsPerRegion * (RegionCnt-1), 7) = ObjectiveFunctionValues(RegionCnt, PointCnt, 5);
            ExplorationResults(PointCnt + PointsPerRegion * (RegionCnt-1), 8) = ObjectiveFunctionValues(RegionCnt, PointCnt, 6);
            ExplorationResults(PointCnt + PointsPerRegion * (RegionCnt-1), 9) = ObjectiveFunctionValues(RegionCnt, PointCnt, 7);
        end
    end

    % add limit test cases
    indexStart = PointsPerRegion * TotalRegions + 1;
    for i = indexStart : indexStart + 3
        iLim = i - PointsPerRegion * TotalRegions;
        ExplorationResults(i, 1) = LimitDesiredValues(iLim, 1);
        ExplorationResults(i, 2) = LimitDesiredValues(iLim, 2);
        ExplorationResults(i, 3) = LimitObjectiveFunctionValues(iLim, 1);
        ExplorationResults(i, 4) = LimitObjectiveFunctionValues(iLim, 2);
        ExplorationResults(i, 5) = LimitObjectiveFunctionValues(iLim, 3);
        ExplorationResults(i, 6) = LimitObjectiveFunctionValues(iLim, 4);
        ExplorationResults(i, 7) = LimitObjectiveFunctionValues(iLim, 5);
        ExplorationResults(i, 8) = LimitObjectiveFunctionValues(iLim, 6);
        ExplorationResults(i, 9) = LimitObjectiveFunctionValues(iLim, 7);
    end
    
    
    ResultsFolderPath = strcat(TempPath, '\RandomExploration');
    if (exist(ResultsFolderPath, 'dir') ~= 7)
        mkdir(ResultsFolderPath);
    end
  
    PointsFilePath = strcat(ResultsFolderPath, '\RandomExplorationPoints_Step.csv');
    RandomExplorationResultsHeader={'InitialDesired,Desired,Stability,Precision,Smoothness,Responsiveness,Steadiness,MeanStableValue,PhysicalRangeExceeded'};
    dlmwrite(PointsFilePath, RandomExplorationResultsHeader, '');
    dlmwrite(PointsFilePath, ExplorationResults,'-append', 'delimiter', ',', 'newline', 'pc');
  
    RegionsPerAxis = sqrt(TotalRegions);
  
    RegionWorstValues = zeros(RegionsPerAxis, RegionsPerAxis, 5);
    RegionWorstIndexes = zeros(RegionsPerAxis, RegionsPerAxis, 5);
    
    RegionMeanValues = zeros(RegionsPerAxis, RegionsPerAxis, 5);
    RegionIndexes = zeros(RegionsPerAxis, RegionsPerAxis, 2);

    RegionPhysicalRangeExceeded = zeros(RegionsPerAxis, RegionsPerAxis);
    
    for RegionXCnt = 1:RegionsPerAxis
        for RegionYCnt = 1:RegionsPerAxis
            % save the region indexes
            RegionIndexes(RegionXCnt, RegionYCnt, 1) = RegionXCnt;
            RegionIndexes(RegionXCnt, RegionYCnt, 2) = RegionYCnt;
            
            % calculate mean and worst values
            for ObjectiveFncCnt = 1:5
                % init worst indexes
                RegionWorstIndexes(RegionXCnt, RegionYCnt, ObjectiveFncCnt, 1) = DesiredValues(RegionYCnt+(RegionXCnt-1)*RegionsPerAxis, 1, 1);
                RegionWorstIndexes(RegionXCnt, RegionYCnt, ObjectiveFncCnt, 2) = DesiredValues(RegionYCnt+(RegionXCnt-1)*RegionsPerAxis, 1, 2);
                
                for PointCnt = 1:PointsPerRegion
                    if (ObjectiveFunctionValues(RegionYCnt+(RegionXCnt-1)*RegionsPerAxis, PointCnt, 7) == 1)
                        RegionPhysicalRangeExceeded(RegionXCnt, RegionYCnt) = 1;
                    end
                    val = ObjectiveFunctionValues(RegionYCnt+(RegionXCnt-1)*RegionsPerAxis, PointCnt, ObjectiveFncCnt);
                    RegionMeanValues(RegionXCnt, RegionYCnt, ObjectiveFncCnt) = RegionMeanValues(RegionXCnt, RegionYCnt, ObjectiveFncCnt) + val;
                    if (val > RegionWorstValues(RegionXCnt, RegionYCnt, ObjectiveFncCnt))
                        RegionWorstValues(RegionXCnt, RegionYCnt, ObjectiveFncCnt) = val;
                        RegionWorstIndexes(RegionXCnt, RegionYCnt, ObjectiveFncCnt, 1) = DesiredValues(RegionYCnt+(RegionXCnt-1)*RegionsPerAxis, PointCnt, 1);
                        RegionWorstIndexes(RegionXCnt, RegionYCnt, ObjectiveFncCnt, 2) = DesiredValues(RegionYCnt+(RegionXCnt-1)*RegionsPerAxis, PointCnt, 2);
                    end
                end

                limitTestCase = 0;
                % add limit test cases
                if RegionXCnt == 1 && RegionYCnt == 1
                    % bottom left region
                    limitTestCase = 1;
                else
                    if RegionXCnt == 1 && RegionYCnt == RegionsPerAxis
                        % top left region
                        limitTestCase = 2;
                    else
                        if RegionXCnt == RegionsPerAxis && RegionYCnt == 1
                            % bottom right region
                            limitTestCase = 3;
                        else
                            if RegionXCnt == RegionsPerAxis && RegionYCnt == RegionsPerAxis
                                % top right region
                                limitTestCase = 4;
                            end
                        end
                    end
                end
                                
                if limitTestCase == 0
                    % finish calculating the mean
                    RegionMeanValues(RegionXCnt, RegionYCnt, ObjectiveFncCnt) = RegionMeanValues(RegionXCnt, RegionYCnt, ObjectiveFncCnt) / PointsPerRegion;
                else
                    % add limit test cases
                    if (LimitObjectiveFunctionValues(limitTestCase, 7) == 1)
                        RegionPhysicalRangeExceeded(RegionXCnt, RegionYCnt) = 1;
                    end
                    val = LimitObjectiveFunctionValues(limitTestCase, ObjectiveFncCnt);
                    RegionMeanValues(RegionXCnt, RegionYCnt, ObjectiveFncCnt) = RegionMeanValues(RegionXCnt, RegionYCnt, ObjectiveFncCnt) + val;
                    if (val > RegionWorstValues(RegionXCnt, RegionYCnt, ObjectiveFncCnt))
                        RegionWorstValues(RegionXCnt, RegionYCnt, ObjectiveFncCnt) = val;
                        RegionWorstIndexes(RegionXCnt, RegionYCnt, ObjectiveFncCnt, 1) = LimitDesiredValues(limitTestCase, 1);
                        RegionWorstIndexes(RegionXCnt, RegionYCnt, ObjectiveFncCnt, 2) = LimitDesiredValues(limitTestCase, 2);
                    end
                    % calculate the mean
                    RegionMeanValues(RegionXCnt, RegionYCnt, ObjectiveFncCnt) = RegionMeanValues(RegionXCnt, RegionYCnt, ObjectiveFncCnt) / (PointsPerRegion + 1);
                end

            end
        end
    end
    
    RegionResults = zeros(TotalRegions, 23);
    
    for RegionCnt = 1 : TotalRegions        
        RegionXCnt = 1 + floor((RegionCnt-1) / RegionsPerAxis);
        RegionYCnt = 1 + floor(mod(RegionCnt-1, RegionsPerAxis));
        
        RegionResults(RegionCnt, 1) = RegionIndexes(RegionXCnt, RegionYCnt, 1);
        RegionResults(RegionCnt, 2) = RegionIndexes(RegionXCnt, RegionYCnt, 2);
        RegionResults(RegionCnt, 3) = RegionPhysicalRangeExceeded(RegionXCnt, RegionYCnt);
        
        for ObjectiveFncCnt = 1 : 5
            RegionResults(RegionCnt, (ObjectiveFncCnt-1)*4 + 4) = RegionMeanValues(RegionXCnt, RegionYCnt, ObjectiveFncCnt);
            RegionResults(RegionCnt, (ObjectiveFncCnt-1)*4 + 5) = RegionWorstValues(RegionXCnt, RegionYCnt, ObjectiveFncCnt);
            RegionResults(RegionCnt, (ObjectiveFncCnt-1)*4 + 6) = RegionWorstIndexes(RegionXCnt, RegionYCnt, ObjectiveFncCnt, 1);
            RegionResults(RegionCnt, (ObjectiveFncCnt-1)*4 + 7) = RegionWorstIndexes(RegionXCnt, RegionYCnt, ObjectiveFncCnt, 2);
        end
    end
    
    RegionsFilePath = strcat(ResultsFolderPath, '\RandomExplorationRegions_Step.csv');
    RegionResultsHeader = {'RegionX,RegionY,RangeExceeded,Stability,StabilityWorst,StabilityWorstX,StabilityWorstY,Precision,PrecisionWorst,PrecisionWorstX,PrecisionWorstY,Smoothness,SmoothnessWorst,SmoothnessWorstX,SmoothnessWorstY,Responsiveness,ResponsivenessWorst,ResponsivenessWorstX,ResponsivenessWorstY,Steadiness,SteadinessWorst,SteadinessWorstX,SteadinessWorstY'};
    dlmwrite(RegionsFilePath, RegionResultsHeader,'');
    dlmwrite(RegionsFilePath, RegionResults,'-append', 'delimiter', ',', 'newline', 'pc');

end

