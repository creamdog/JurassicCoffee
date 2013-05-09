class Rnd
	@gate=(chance) ->
		passed = (Math.random() * 100) < chance
		return passed

	@next=(max=1, min=0) ->
		(Math.random() * max) + min

	@location=->
		new Vector(Rnd.next(window.width), Rnd.next(window.height))

	@velocity=->
		new Vector(Rnd.next(20)*Rnd.sign(), Rnd.next(20)*Rnd.sign())

	@sign=->
		if Rnd.gate(50) then 1 else -1