class Drag
	@k=0.05
	@applyForce=(bot)->
		speed = bot.velocity.mag()
		force = bot.velocity.get()
		force.mult -1
		force.normalize()
		force.mult Drag.k * speed * speed
		bot.accelerate force
