
# Splats:
race = (winner, runners...) ->
  print winner, runners

`#= require ./jquery-1.6.1.min.js`

$(document).ready -> 
  message = "JurassicCoffee!"
  h1 = $(document.createElement 'h1')
  h1.text message
  $('body').prepend h1