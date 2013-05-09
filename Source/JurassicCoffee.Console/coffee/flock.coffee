#= require common\domain.min.js

bots = []
attractors = []

$ ->
	$('.restart').change restart
	$('.refresh').change refresh
	canvas = $('<canvas id="processing-canvas">').appendTo($('#screen'))[0]
	x = new Processing canvas, main

main=(processing)->
	p = processing
	p.setup=setupProcessing p
	setupForceConstants()
	restart()
	p.draw=->
		p.background 51
		processAttractors()
		processBots()
		
processAttractors=->
	applyUniversalForces attractor for attractor in attractors
	attractor.move() for attractor in attractors
	attractor.drawShape() for attractor in attractors if Attractor.isActive

processBots=->
	applyUniversalForces bot for bot in bots
	attractor.applyForce bot for bot in bots for attractor in attractors
	bot.move() for bot in bots
	bot.drawShape() for bot in bots

setupProcessing=(p)->
	p.width = 855
	p.height = 500
	p.background 51
	window.width = p.width
	window.height = p.height
	window.midX = (p.width/2)>>0
	window.midY = (p.height/2)>>0
	window.processing = p
	p.mousePressed = -> Mouse.isActive = !Mouse.isActive
	null

setupForceConstants=->
	Drag.k = 0.05
	Magnet.k = 10

applyUniversalForces=(bot)->
	Magnet.applyForce bot, other for other in bots when other!=bot if Magnet.isActive
	Drag.applyForce bot 
	Noise.applyForce bot
	EdgeWrap.applyForce bot

refresh=->	
	Attractor.isActive = $('#attractor-show').is(':checked')
	setupAttractors()

restart=->
	refresh()
	setupBots()
	setupAttractors()

setupBots=->
	flockCount = parseInt $('#flock-count').val()
	botCount = parseInt $('#bot-count').val()
	bots = BotBuilder.flock flockCount, botCount
	bots

setupAttractors=->
	count = parseInt $('#attractor-count').val()
	attractors = (new Attractor() for i in [1..count])
	attractors