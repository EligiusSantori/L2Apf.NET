set exe=%~n1.exe
csc /nologo /r:core.dll /r:script.dll /out:.\%exe% /o %1
.\%exe%
