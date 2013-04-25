$(document).ready -> 
  message = "JurassicCoffee!!"
  h1 = $(document.createElement 'h1')
  h1.text message
  $('body').prepend h1