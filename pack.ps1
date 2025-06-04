# 设置路径
$solutionDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
$commonProject = "$solutionDir\IBatisNetSelf.Common\IBatisNetSelf.Common.csproj"
$dataMapperProject = "$solutionDir\IBatisNetSelf.DataMapper\IBatisNetSelf.DataMapper.csproj"
$commonOutput = "$solutionDir\IBatisNetSelf.Common\bin\Release\net6.0\IBatisNetSelf.Common.dll"

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