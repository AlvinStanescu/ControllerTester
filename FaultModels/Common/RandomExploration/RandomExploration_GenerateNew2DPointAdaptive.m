function [InitialDesiredValue, DesiredValue] = RandomExploration_GenerateNew2DPointAdaptive(CurrentDesiredValues, CurrentNoPoints, RegionXStart, RegionXEnd, RegionYStart, RegionYEnd)
    CandidatePoints = zeros(10, 3);
    ChosenPoint = 1;
    
    % generate 10 random candidate points and keep track of which is the
    % best choice
    for RandomPointCnt = 1: 10
        % generate point
        CandidatePoints(RandomPointCnt, 1) = RegionXStart + (RegionXEnd - RegionXStart) * rand(1);
        CandidatePoints(RandomPointCnt, 2) = RegionYStart + (RegionYEnd - RegionYStart) * rand(1);
        
        % calculate minimum distance from previously chosen points
        CandidatePoints(RandomPointCnt, 3) = Inf(1);
        for PreviousPointCnt = 1 : CurrentNoPoints
            dist = sqrt(power(CurrentDesiredValues(PreviousPointCnt,1) - CandidatePoints(RandomPointCnt, 1),2) + power(CurrentDesiredValues(PreviousPointCnt, 2) - CandidatePoints(RandomPointCnt, 2),2));
            if (dist < CandidatePoints(RandomPointCnt, 3))
                CandidatePoints(RandomPointCnt, 3) = dist;
            end
        end
        if CandidatePoints(ChosenPoint, 3) < CandidatePoints(RandomPointCnt, 3)
            ChosenPoint = RandomPointCnt;
        end
    end
    
    InitialDesiredValue = CandidatePoints(ChosenPoint, 1);
    DesiredValue = CandidatePoints(ChosenPoint, 2);
end
