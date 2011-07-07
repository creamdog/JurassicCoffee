#Jurassic-Coffee
####.NET compiler for .coffee files using [coffee-script.js](http://jashkenas.github.com/coffee-script/) & [Jurassic](http://jurassic.codeplex.com/)

![jurassic-coffee](http://creamdog.se/jurassic-coffee.small.png)
## Installation

###1. install as a Nuget package:
*Download and install [Nuget](http://nuget.org/) if you do not allready have it.*

- Installs as a single JurassicCoffee.dll
- Adds a *.coffee HttpHandler to you web.config if you have one
```
>> Install-Package jurassic-coffee
```

###2. download and build it from source
*Always nice to have the latest version*

###3. Download a pre-built binary 
*note that it may be be an older version than that from source*

## Features

### Configurable post & pre-compilation actions

```c#
var compiler = new CoffeeCompiler();
compiler.PreScriptLoadActions.Add(FileRouting.RouteToCorrectVersion);
compiler.PreScriptOutputActions.Add(FileNaming.AddBuildDate);
compiler.PreScriptOutputActions.Add(FileNaming.AddMinified);
compiler.PostcompilationActions.Add(YahooYuiCompressor.Compress);
```

### REQUIRE

#### # require file.coffee
includes desired coffee file into the compilation

*sayhello.coffee*

```coffeescript
sayhello = (name)-> alert name
```

*main.coffee*

```coffeescript
# require sayhello.coffee
sayhello 'charles'
```

#### # require \`file.js\`

embeds desired javascript file

*default.coffee*

```coffeescript
#= require `./jquery-1.6.1.min.js`
$(document).ready -> 
  message = "JurassicCoffee!"
  h1 = $(document.createElement 'h1')
  h1.text message
  $('body').prepend h1    
```

## Usage

### command line tool
```
JurassicCoffee.exe script.coffee //compiles into script.js
JurassicCoffee.exe script.coffee -o out.js //compiles into out.js
JurassicCoffee.exe script.coffee -c //YUI Compression enabled
JurassicCoffee.exe script.coffee -e coffee-script.nighlty.version.js //compile using custom version of coffee-script
```

### Http Handler    

- Compiles *.coffee files into *.js files
- Watches *.coffee files for changes and only re-compiles when files have changed

#### web.config

##### Set compiled output directory
relative to the server root
```xml
<appSettings>
    <add key="JurassicCoffee.CompiledDirectory" value="compiled-coffee"/>
</appSettings>
```

##### Add http handler section
```xml
<httpHandlers> 
    <add type="JurassicCoffee.Web.JurassicCoffeeHttpHandler,JurassicCoffee.Web" validate="false" path="*.coffee" verb="*" />
</httpHandlers>
```

### code
```c#
    var compiler = new JurassicCoffee.Core.Compiler();

    //Compile test.coffee into test.js
    compiler.Compile("test.coffee");

    //Compiles coffeescript string into a javascript string
    var javascriptString = compiler.CompileString("helloworld -> 'hello world'");

    //Compiles inputstream into outputstream
    compiler.Compile(inputstream,outputstream);
```
