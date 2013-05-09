class Spring
	@isActive=true
	@k=0.01
	@restLength=50

	@applyForce=(bot1, bot2)->
		return if not Magnet.isActive
		force = Vector.sub bot1.location, bot2.location
		currentLength = force.mag()
		distance = Spring.restLength - currentLength
		force.normalize()
		force.mult Spring.k * distance
		bot1.accelerate force
		force.mult -1
		bot2.accelerate force
