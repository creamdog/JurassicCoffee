class EdgeBounce

	@applyForce=(bot)->
		xMax = bot.location.x - window.width
		xMin = bot.location.x
		bot.accelerate new Vector -xMax, 0 if (xMax > 0)
		bot.accelerate new Vector -xMin, 0 if (xMin < 0)

		yMax = bot.location.y - window.height
		yMin = bot.location.y
		bot.accelerate new Vector 0, -yMax if (yMax > 0)
		bot.accelerate new Vector 0, -yMin if (yMin < 0)