class Bot
	constructor:(@mass = (Math.random() * 10)+1)->
		@p = window.processing
		@acceleration = new Vector
		@velocity = Rnd.velocity()
		@location = Rnd.location()
		@friends = []
		
	drawShape:->
		@p.pushMatrix()
		@p.translate @location.x, @location.y
		@p.rotate @velocity.heading();
		@p.scale 0.5 + @mass/8
		@p.fill 128, 255, 128
		@p.stroke 0, 255, 0
		@p.triangle -4, 4, 8, 0, -4, -4
		@p.popMatrix()

	drawLines:->
		@p.pushMatrix()
		@p.stroke 75
		@p.line(@location.x, @location.y, f.location.x, f.location.y) for f in @friends
		@p.popMatrix()

	move:->
		@velocity.add @acceleration 
		@velocity.limit 10
		@location.add @velocity
		@acceleration.mult 0

	accelerate:(force)->
		@acceleration.add Vector.div(force, @mass)