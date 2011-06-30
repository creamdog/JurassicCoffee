#Jurassic-Coffee
####.NET compiler for .coffee files using [coffee-script.js](http://jashkenas.github.com/coffee-script/) & [Jurassic](http://jurassic.codeplex.com/)

![jurassic-coffee](http://s3.amazonaws.com/data.tumblr.com/tumblr_lnjxv04jxq1qkgjd1o1_1280.png?AWSAccessKeyId=AKIAJ6IHWSU3BX3X7X3Q&Expires=1309437620&Signature=EOfIHOdnYj%2BoDONYh10r%2FgVwRbg%3D)

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
```

### Http Handler    

*web.config*

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
