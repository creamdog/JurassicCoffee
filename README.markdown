#Jurassic-Coffee
####.NET compiler for .coffee files using [coffee-script.js](http://jashkenas.github.com/coffee-script/) & [Jurassic](http://jurassic.codeplex.com/)
####Embedding of local or external .js or .coffee files

![jurassic-coffee](http://creamdog.se/jurassic-coffee.small.png)
## Installation

###1. install as a Nuget package:
*Download and install [Nuget](http://nuget.org/) if you do not allready have it.*

- Installs as a single JurassicCoffee.dll
- Adds a *.coffee HttpHandler to you web.config if you have one

```
>> Install-Package jurassic-coffee
```

_make sure to add the following if you are installing into a MVC application_ : [Anujb](https://github.com/anujb)

```c#
routes.IgnoreRoute("{resource}.coffee/{*pathInfo}");
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
#= require sayhello.coffee
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

### including external files via http(s)

files can be loaded via http(s)

_additional require statements in files loaded in this manner will not be evaluated_

```coffeescript
#= require `https://ajax.googleapis.com/ajax/libs/swfobject/2.2/swfobject.js`  
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

- Compiles .coffee files into .js files
- Keeps track of .coffee and compiled .js files and only re-compiles when files are added, deleted or changed
- Keeps track of included files from "#= require" sprockets and re-compiles on changes

#### web.config

_configuration section_

```xml
<configSections>
    <section name="jurassic.coffee" type="JurassicCoffee.Web.Configuration.ConfigurationHandler, JurassicCoffee.Web"/>
</configSections>

<jurassic.coffee>
	<!--enable/disable YUI compression-->
    <EnableCompression>true</EnableCompression>
	<!--compiled .js output directory-->
    <CompiledDirectory>compiled-coffee</CompiledDirectory>
	<!--disabled compression and adds debug information to compiled files-->
    <DebugMode>false</DebugMode>
</jurassic.coffee>
```

_http handler section_

```xml
<httpHandlers> 
    <add type="JurassicCoffee.Web.JurassicCoffeeHttpHandler,JurassicCoffee.Web" validate="false" path="*.coffee" verb="*" />
</httpHandlers>
```

_make sure to add the following if you have a MVC application_ : [Anujb](https://github.com/anujb)

```c#
routes.IgnoreRoute("{resource}.coffee/{*pathInfo}");
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
