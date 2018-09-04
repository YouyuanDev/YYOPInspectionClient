1：OP在线播放问题,WebBroswer默认使用ie7内核，需要修改注册表内容,修改方法:
   打开注册表依次打开HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION然后新建DWORD32位的数据项(名称为程序名,比如YYInspectionClient.exe,修改十进制数据为:11000)
2：config.txt文件配置的是读码器的ip地址(100.100.0.101)
3:  录像机的ip地址默认设置为100.100.0.90
4：server.txt文件配置的是服务器的ip和端口地址(192.168.0.200:8080)
5：bin目录下的unsubmit目录中存在的都是提交失败的表单信息
