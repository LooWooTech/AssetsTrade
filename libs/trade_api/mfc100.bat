@echo ��ʼע��
copy mfc100.dll %windir%\system32\
regsvr32 %windir%\system32\mfc100.dll /s
@echo mfc100.dllע��ɹ�
@pause