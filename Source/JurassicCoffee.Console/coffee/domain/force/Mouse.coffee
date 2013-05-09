class Mouse
	@isActive=false
	@k=0.7
	@applyForce=(bot)->
		return if not Mouse.isActive
		p = window.processing
		vm = new Vector p.mouseX, p.mouseY
		direction = Vector.sub vm , bot.location
		direction.normalize()
		direction.mult Mouse.k
		bot.accelerate direction