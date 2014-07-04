function CT_DiaryInit(filePath)
    delete(filePath);
    fid = fopen(filePath,'w');
    fclose(fid);
    diary(filePath);
    diary off;
end

