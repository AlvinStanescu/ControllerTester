function [DesiredValue] = RandomExploration_Single_AdaptiveRandomSearchGenerateNewPoint(CurrentDesiredValues, CurrentNoPoints, RegionStart, RegionEnd)
    CandidatePoints = zeros(10, 2);
    ChosenPoint = 1;
    
    % generate 10 random candidate points and keep track of which is the
    % best choice
    for RandomPointCnt = 1: 10
        % generate point
        CandidatePoints(RandomPointCnt, 1) = RegionStart + (RegionEnd - RegionStart) * rand(1);
        
        % calculate minimum distance from previously chosen points
        CandidatePoints(RandomPointCnt, 2) = Inf(1);
        for PreviousPointCnt = 1 : CurrentNoPoints
            dist = abs(CurrentDesiredValues(PreviousPointCnt,1) - CandidatePoints(RandomPointCnt, 1));
            if (dist < CandidatePoints(RandomPointCnt, 2))
                CandidatePoints(RandomPointCnt, 2) = dist;
            end
        end
        if CandidatePoints(ChosenPoint, 2) < CandidatePoints(RandomPointCnt, 2)
            ChosenPoint = RandomPointCnt;
        end
    end
    
    DesiredValue = CandidatePoints(ChosenPoint, 1);
end
