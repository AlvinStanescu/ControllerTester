function [InitialDesiredValue, DesiredValue] = RandomExploration_GenerateNew2DPoint(~, ~, RegionXStart, RegionXEnd, RegionYStart, RegionYEnd)
    % generate a random starting point p
    InitialDesiredValue = RegionXStart + (RegionXEnd - RegionXStart) * rand(1);
    DesiredValue = RegionYStart + (RegionYEnd - RegionYStart) * rand(1);
end
