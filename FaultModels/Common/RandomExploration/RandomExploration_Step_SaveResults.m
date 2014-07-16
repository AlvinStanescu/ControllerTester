function [ ExplorationResults ] = RandomExploration_Step_SaveResults(DesiredValues, ObjectiveFunctionValues, TotalRegions, PointsPerRegion, TempPath)
    % generates a file containing all the points found and a file
    % containing a processed view of the input space
    ExplorationResults = zeros(PointsPerRegion*TotalRegions, 9);
    
    for RegionCnt = 1 : TotalRegions
        for PointCnt = 1:PointsPerRegion
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
    
    
    ResultsFolderPath = strcat(TempPath, '\RandomExploration');
    if (exist(ResultsFolderPath, 'dir') ~= 7)
        mkdir(ResultsFolderPath);
    end
  
    PointsFilePath = strcat(ResultsFolderPath, '\RandomExplorationPoints_Step.csv');
    RandomExplorationResultsHeader={'InitialDesired,Desired,Stability,Liveness,Smoothness,Responsiveness,Oscillation,MeanStableValue,PhysicalRangeExceeded'};
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
                
                % finish calculating the mean
                RegionMeanValues(RegionXCnt, RegionYCnt, ObjectiveFncCnt) = RegionMeanValues(RegionXCnt, RegionYCnt, ObjectiveFncCnt) / PointsPerRegion;
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
    RegionResultsHeader = {'RegionX,RegionY,RangeExceeded,Stability,StabilityWorst,StabilityWorstX,StabilityWorstY,Liveness,LivenessWorst,LivenessWorstX,LivenessWorstY,Smoothness,SmoothnessWorst,SmoothnessWorstX,SmoothnessWorstY,Responsiveness,ResponsivenessWorst,ResponsivenessWorstX,ResponsivenessWorstY,Oscillation,OscillationWorst,OscillationWorstX,OscillationWorstY'};
    dlmwrite(RegionsFilePath, RegionResultsHeader,'');
    dlmwrite(RegionsFilePath, RegionResults,'-append', 'delimiter', ',', 'newline', 'pc');

end

