function [DesiredValue] = RandomExploration_Single_RandomSearchGenerateNewPoint(~, ~, RegionStart, RegionEnd)
    % generate a random starting point p
    DesiredValue = RegionStart + (RegionEnd - RegionStart) * rand(1);
end
