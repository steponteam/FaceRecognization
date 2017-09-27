@echo off
echo 开始打包

nuget pack ../Stepon.FaceRecognization -Build -Properties Configuration=Release

echo 打包结束

echo 删除调试文件

for /r ./ %%i in (*symbols*) do (
	del %%i
)

pause