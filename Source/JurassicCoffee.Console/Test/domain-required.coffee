
#= require `./js/domain.min.js`

bots = []
attractors = []

$ ->
	canvas = $('<canvas id="processing-canvas">').appendTo($('#screen'))[0]