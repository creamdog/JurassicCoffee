class Noise
	@chance = 0.5
	@scale = 2
	@applyForce=(bot)->
		return if Noise.scale==0 or Rnd.gate(100-Noise.chance)
		x= Rnd.next(Noise.scale * bot.mass) * Rnd.sign()
		y= Rnd.next(Noise.scale * bot.mass) * Rnd.sign()
		bot.accelerate new Vector x, y