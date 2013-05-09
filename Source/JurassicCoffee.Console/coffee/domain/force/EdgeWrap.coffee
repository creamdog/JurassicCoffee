class EdgeWrap

	@applyForce=(bot)->
		xMax = bot.location.x - window.width
		xMin = bot.location.x
		bot.location.x = 0 if (xMax > 0) 
		bot.location.x = window.width  if (xMin < 0) 

		yMax = bot.location.y - window.height
		yMin = bot.location.y
		bot.location.y = 0 if (yMax > 0) 
		bot.location.y = window.height  if (yMin < 0) 