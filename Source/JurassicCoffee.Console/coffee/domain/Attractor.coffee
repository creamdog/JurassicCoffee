class Attractor
	@isActive = true
	constructor:->
		@p = window.processing
		@mass = 2
		@acceleration = new Vector
		@velocity = Rnd.velocity()
		@location = Rnd.location()
		@accelerationChance = 0

	move:->
		@changeVector()
		@velocity.add @acceleration 
		@velocity.limit 20
		@location.add @velocity
		@acceleration.mult 0

	changeVector:->
		@accelerationChance++
		return if !Rnd.gate(@accelerationChance/10)
		@accelerationChance = 0
		x= Rnd.next(100) * Rnd.sign()
		y= Rnd.next(100) * Rnd.sign()
		@accelerate new Vector x, y

	drawShape:->
		@p.pushMatrix()
		@p.translate @location.x, @location.y
		@p.rotate @velocity.heading();
		@p.scale 0.5 + @mass/8
		@p.fill 255, 128, 128
		@p.stroke 255, 0, 0
		@p.triangle -4, 4, 8, 0, -4, -4
		@p.popMatrix()

	applyForce:(bot)->
		direction = Vector.sub @location, bot.location
		direction.normalize()
		bot.accelerate direction

	accelerate:(force)->
		@acceleration.add Vector.div(force, @mass)