function [InitialDesiredValue, DesiredValue] = RandomExploration_Step_RandomSearchGenerateNewPoint(~, ~, RegionXStart, RegionXEnd, RegionYStart, RegionYEnd)
    % generate a random starting point p
    InitialDesiredValue = RegionXStart + (RegionXEnd - RegionXStart) * rand(1);
    DesiredValue = RegionYStart + (RegionYEnd - RegionYStart) * rand(1);
end
