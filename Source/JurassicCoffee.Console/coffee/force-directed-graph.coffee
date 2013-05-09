#= require common\domain.min.js

bots = []
edge = EdgeBounce

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
		applyForces bot for bot in bots
		bot.move() for bot in bots
		bot.drawLines() for bot in bots if Spring.isActive
		bot.drawShape() for bot in bots

setupProcessing=(p)->
	p.width = 855
	p.height = 500
	p.fill 128, 255, 128
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
	Spring.restLength = 40
	Spring.k = 0.01


refresh=->	
	switch $('#edge').val()
		when 'bounce' then edge = EdgeBounce
		when 'wrap' then edge = EdgeWrap

	Noise.scale = $('#noise').val()
	Magnet.isActive = $('#magnets').is(':checked')
	Spring.isActive = $('#springs').is(':checked')

restart=->
	refresh()
	setupBots()

setupBots=->
	bots=[]
	count = parseInt $('#count').val()
	switch $('#type').val()
		when 'spokes' then bots = bots.concat BotBuilder.graphSpokes() for i in [1..count]
		when 'swarm' then bots = bots.concat BotBuilder.graphSwarm() for i in [1..count]
		when 'molecule' then bots = bots.concat BotBuilder.graphMolecule() for i in [1..count]
		when 'blossom' then bots = bots.concat BotBuilder.graphBlossom() for i in [1..count]
	bots

applyForces=(bot)->
	Spring.applyForce bot, friend for friend in bot.friends if Spring.isActive
	Magnet.applyForce bot, other for other in bots when other!=bot if Magnet.isActive
	Mouse.applyForce bot if Mouse.isActive
	Drag.applyForce bot 
	Noise.applyForce bot
	edge.applyForce bot