class Vector
	constructor: (@x=0, @y=0, @z=0)->

	add: (v)->
		@x += v.x
		@y += v.y
		@z += v.z

	get: -> new Vector @x, @y, @z

	cross: (v)-> new Vector @y*v.z - v.y*@z, @z*v.x - v.z*@x, @x*v.y - v.x*@y

	div: (d)->
		v = Vector.ensure(d)
		@x /= v.x
		@y /= v.y
		@z /= v.z

	dist: (d)->
		v = Vector.ensure(d)
		Vector.dist(@, v)

	hypot: (a, b) ->
		if a is 0
			Math.abs b
		else
			Math.abs(a) * Math.sqrt 1 + Math.pow b/a, 2

	dot: (v)-> @x*v.x + @y*v.y + @z*v.z

	heading: -> -Math.atan2 -@y, @x

	limit: (high, low)-> 
		m = @mag()
		if high? and m > high 
			@normalize()
			@mult(high)
		else if low? and m < low
			@normalize()
			@mult(low)

	mag: -> Math.sqrt @x*@x + @y*@y + @z*@z

	mult: (d)->
		v = Vector.ensure(d)
		@x *= v.x
		@y *= v.y
		@z *= v.z

	normalize: ->
		m = @mag()
		@div m if m > 0

	set: (x, y, z)-> [ @x, @y, @z ] = [ x, y, z ]

	sub: (v)->
		@x -= v.z
		@y -= v.y
		@z -= v.z

	toArray: -> [@x, @y, @z]

	toString: -> "x:@x, y:@y, z:@z"

	@add = (d1, d2)-> 
		v1 = Vector.ensure(d1)
		v2 = Vector.ensure(d2)
		new Vector(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z)

	@sub = (d1, d2)-> 
		v1 = Vector.ensure(d1)
		v2 = Vector.ensure(d2)
		new Vector(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z)

	@div = (d1, d2)-> 
		v1 = Vector.ensure(d1)
		v2 = Vector.ensure(d2)
		new Vector(v1.x / v2.x, v1.y / v2.y, v1.z / v2.z)

	@ensure = (v)-> if typeof v is 'number' then new Vector(v, v, v) else v

	@dist = (v1, v2)-> 
		[a, b] = [v1?.x - v2?.x, v1?.y - v2?.y]
		@hypot a, b

	@hypot: (a, b) ->
		if a is 0
			Math.abs b
		else
			Math.abs(a) * Math.sqrt 1 + Math.pow b/a, 2
	

	@dot = (v1, v2)-> v1.dot(v2)

	@cross = (v1, v2)-> v1.cross(v2)

	@angleBetween = (v1, v2)-> Math.acos v1.dot(v2) / (v1.mag() * v2.mag())


