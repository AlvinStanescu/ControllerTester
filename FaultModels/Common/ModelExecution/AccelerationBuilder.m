% this script executes the model with a single uniform desired value
% there is no inherent fault model behind it, its main purpose being to
% verify that simulating the system works as intended
try
    % add all the paths containing the model and the functions we use
    temp_directory = tempdir;
    load(strcat(temp_directory,'settings.mat'));
    addpath(CT_ModelPath);
    addpath(strcat(CT_ScriptsPath, '/ObjectiveFunctions'));
    addpath(strcat(CT_ScriptsPath, '/Util'));
    % configure the static model configuration parameters and load the
    % model into the system memory
    run(CT_ModelConfigurationFile);
    load_system(CT_ModelFile);

    % double the model simulation time set in the GUI because we need two values for the step fault model
    simulationTime = CT_ModelSimulationTime * 2;
    CT_SetSimulationTime(simulationTime);
    % retrieve the model simulation step, as it might have been changed by
    % the configuration script
    CT_ModelTimeStep = CT_GetSimulationTimeStep();
    CT_SimulationSteps=int64(CT_ModelSimulationTime/CT_ModelTimeStep);
           
    % start the timer to measure the running time of the model together
    % with the objective function computation
    tic;
    % generate the time for the desired value  
    assignin('base', CT_DesiredVariableName, CT_GenerateStepDesiredValue(CT_SimulationSteps, CT_ModelTimeStep, CT_InitialDesiredValue, CT_DesiredValue));
            
    accelbuild(gcs)
    fName = 'compile.done';
    fid = fopen(fName,'w');
    if fid>=0
        fprintf(fid, 'done\n');
        fclose(fid);
    end

    % stop the timer
    duration = toc;
    % output the model running time (?)
    display('Successful compilation of the accelerated model');
    display(strcat('runningTime=', num2str(duration)));
    close_system(gcs, 0);
catch e
    display('Error during model compilation');
    display(getReport(e));
end
exit;