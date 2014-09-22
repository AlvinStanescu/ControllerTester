function CT_CheckCorrectOutput(outputVariableName)
    BlockPaths = find_system(gcs,'BlockType','ToWorkspace');
    
    for i = 1 : length(BlockPaths)
        if strcmp(get_param(BlockPaths,'VariableName'),outputVariableName)
            if ~strcmp(get_param(BlockPaths,'SaveFormat'),'Structure With Time')
                err = MException('ModelChk:InvalidOutput', ['Output variable ',outputVariableName,' needs to be saved as a Structure with Time.']);
                throw(err);
            end
        end
    end
end

