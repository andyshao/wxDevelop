@echo ��ʼע��
copy Newtonsoft.Json.Net20.dll %windir%\system32\
regsvr32 %windir%\system32\Newtonsoft.Json.Net20.dll /s
@echo Newtonsoft.Json.Net20.dllע��ɹ�
@pause