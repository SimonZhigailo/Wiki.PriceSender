if %computername%==VSAVINOV (
echo on
echo %2 - 2
echo create nuget
echo %2..\..\.nuget\nuget.exe pack  %2Wiki.PriceSender.Service.nuspec -BasePath %1 -OutputDirectory %1
%2..\..\.nuget\nuget.exe pack  %2Wiki.PriceSender.Service.nuspec -BasePath %1 -OutputDirectory D:\Projects\nuget


)

if %computername%==MISHENKO (
echo on
echo %2 - 2
echo create nuget
echo %2..\..\.nuget\nuget.exe pack  %2Wiki.PriceSender.Service.nuspec -BasePath %1 -OutputDirectory %1
%2..\..\.nuget\nuget.exe pack  %2Wiki.PriceSender.Service.nuspec -BasePath %1 -OutputDirectory D:\Work\nuget


)


if %computername%==DESINER (
echo on
echo create nuget
%2..\.nuget\nuget.exe pack  %2Wiki.PriceSender.Service.nuspec -BasePath %1 -OutputDirectory c:\wiki\nuget


)

if %computername%==VNEDVIGIN (
echo on
echo create nuget
%2..\.nuget\nuget.exe pack  %2Wiki.PriceSender.Service.nuspec -BasePath %1 -OutputDirectory D:\Infrastucture\Nuget

)