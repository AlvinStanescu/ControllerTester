function [stop,options,optchanged] = SingleStateSearch_LogIteration(options,optimvalues,flag,displayCount,header)
%   STOP = SingleStateSearch_PlotIteration(OPTIONS,OPTIMVALUES,FLAG,EACHITERATION) where OPTIMVALUES is a
%   structure with the following fields:
%              x: current point
%           fval: function value at x
%          bestx: best point found so far
%       bestfval: function value at bestx
%    temperature: current temperature
%      iteration: current iteration
%      funccount: number of function evaluations
%             t0: start time
%              k: annealing parameter
%
    stop = false;
    optchanged = false;
    if (strcmp(flag,'done'))
        CT_DiaryLog(strcat(header,':100%'));
    else
        if (mod(optimvalues.iteration, displayCount) == 0)
            it = optimvalues.iteration;
            maxIt = saoptimget(options, 'MaxIter');
            if (uint8(it/maxIt*100) ~= 100)
                CT_DiaryLog(strcat(header,':',num2str(double(uint8(it/maxIt*100))),'%'));
            end
        end
    end
end