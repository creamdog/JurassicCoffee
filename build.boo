solution_file = "JurassicCoffee.sln"
configuration = "release"

target default, (compile, package):
  pass

target init:
  rmdir("build")
  mkdir("build")

desc "Compiles the solution"
target compile:
  msbuild(file: solution_file, configuration: configuration)
  
target package:
  rm("./build")
  mkdir("./build")
  mkdir("./build/core")
  exec("./Dependencies/ILMerge/ILMerge.exe","/v4 /wildcards /target:dll /out:build/core/JurassicCoffee.dll ./Source/JurassicCoffee.Core/bin/${configuration}/*.dll")
  mkdir("./build/web")
  exec("./Dependencies/ILMerge/ILMerge.exe","/v4 /wildcards /target:dll /out:build/web/JurassicCoffee.dll ./Source/JurassicCoffee.Web/bin/${configuration}/*.dll")
  mkdir("./build/console")  
  exec("./Dependencies/ILMerge/ILMerge.exe","/v4 /wildcards /target:exe /out:build/console/JurassicCoffee.exe ./Source/JurassicCoffee.Console/bin/${configuration}/*.exe ./Source/JurassicCoffee.Console/bin/Release/*.dll")
  with FileList("build/web/"):
    .Include("*.dll")
    .ForEach def(file):
      file.CopyToDirectory("Nuget/Lib/net40/")