# 设置路径
# 获取当前脚本文件所在的解决方案（Solution）根目录路径，常用于 .NET 项目的自动化脚本（如 .ps1 脚本）中
# $MyInvocation 是 PowerShell 中的自动变量，用于存储当前脚本 / 命令的执行信息。
# $MyInvocation.MyCommand 表示当前正在执行的命令（即脚本文件本身）。
# .Definition 属性返回当前脚本文件的完整路径（包含文件名），例如 C:\Projects\MySolution\scripts\build.ps1
# Split-Path 是 PowerShell 的路径处理 cmdlet，用于分割路径字符串。 -Parent 参数表示取路径的父目录（即去掉文件名后的文件夹路径）。
$solutionDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
$commonProject = "$solutionDir\IBatisNetSelf.Common\IBatisNetSelf.Common.csproj"
$commonOutput = "$solutionDir\IBatisNetSelf.Common\bin\Release\net6.0\IBatisNetSelf.Common.dll"
$dataMapperProject = "$solutionDir\IBatisNetSelf.DataMapper\IBatisNetSelf.DataMapper.csproj"

# 1. 生成 Common 项目
Write-Host "Building IBatisNetSelf.Common..."
dotnet build $commonProject -c Release

if (!(Test-Path $commonOutput)) {
    Write-Error "IBatisNetSelf.Common 编译失败或 DLL 未生成。"
    exit 1
}

# 2. 构建并打包 DataMapper 项目
Write-Host "Packing IBatisNetSelf.DataMapper..."
dotnet pack $dataMapperProject -c Release /p:BuildingPackage=true

Write-Host "打包完成！"

# 等待用户输入（按回车后才会退出）
Read-Host -Prompt "按回车键关闭窗口"