class BotBuilder

	@flock=(flockCount, botCount)->
		bots = []
		for i in [0..flockCount]
			flock = (new Bot(Rnd.next(3, 1)) for i in [0...botCount]) 
			bots = bots.concat flock

		return bots

	@graphSpokes=->
		b1 = new Bot 4
		b2 = (new Bot 4 for i in [0...10])
		b1.friends = b2
		b2.concat b1

	@graphSwarm=->
		b1 = new Bot 6
		b2 = (new Bot 4 for i in [0...16])
		b3 = (new Bot 1 for i in [0...24])
		b1.friends = b2.concat b3
		b2.concat b1, b3

	@graphMolecule=->
		c = (new Bot 5 for i in [0...6])
		h0 = (new Bot 3 for h in [0...3])
		h1 = (new Bot 3 for h in [0...2])
		h2 = (new Bot 3 for h in [0...2])
		h3 = (new Bot 3 for h in [0...2])
		h4 = (new Bot 3 for h in [0...2])
		h5 = (new Bot 3 for h in [0...3])
		c[0].friends =h0.concat [c[1]]
		c[1].friends =h1.concat [c[2]]
		c[2].friends =h2.concat [c[3]]
		c[3].friends =h3.concat [c[4]]
		c[4].friends =h4.concat [c[5]]
		c[5].friends =h5
		c.concat h0, h1, h2, h3, h4, h5

	@graphBlossom=->
		a = (new Bot 8 for i in [0...2])

		b0 = (new Bot 4 for i in [0...4])
		b1 = (new Bot 4 for i in [0...4])

		c0 = (new Bot 2 for i in [0...4])
		c1 = (new Bot 2 for i in [0...4])
		c2 = (new Bot 2 for i in [0...4])
		c3 = (new Bot 2 for i in [0...4])
		c4 = (new Bot 2 for i in [0...4])
		c5 = (new Bot 2 for i in [0...4])
		c6 = (new Bot 2 for i in [0...4])
		c7 = (new Bot 2 for i in [0...4])

		d0 = (new Bot 1 for i in [0...2])
		d1 = (new Bot 1 for i in [0...2])
		d2 = (new Bot 1 for i in [0...2])
		d3 = (new Bot 1 for i in [0...2])

		d4 = (new Bot 1 for i in [0...2])
		d5 = (new Bot 1 for i in [0...2])
		d6 = (new Bot 1 for i in [0...2])
		d7 = (new Bot 1 for i in [0...2])

		d8 = (new Bot 1 for i in [0...2])
		d9 = (new Bot 1 for i in [0...2])
		d10= (new Bot 1 for i in [0...2])
		d11= (new Bot 1 for i in [0...2])

		d12= (new Bot 1 for i in [0...2])
		d13= (new Bot 1 for i in [0...2])
		d14= (new Bot 1 for i in [0...2])
		d15= (new Bot 1 for i in [0...2])

		d16 = (new Bot 1 for i in [0...2])
		d17 = (new Bot 1 for i in [0...2])
		d18 = (new Bot 1 for i in [0...2])
		d19 = (new Bot 1 for i in [0...2])

		d20 = (new Bot 1 for i in [0...2])
		d21 = (new Bot 1 for i in [0...2])
		d22 = (new Bot 1 for i in [0...2])
		d23 = (new Bot 1 for i in [0...2])

		d24 = (new Bot 1 for i in [0...2])
		d25 = (new Bot 1 for i in [0...2])
		d26 = (new Bot 1 for i in [0...2])
		d27 = (new Bot 1 for i in [0...2])

		d28 = (new Bot 1 for i in [0...2])
		d29 = (new Bot 1 for i in [0...2])
		d30 = (new Bot 1 for i in [0...2])
		d31 = (new Bot 1 for i in [0...2])

		a[0].friends = b0.concat a[1]
		a[1].friends = b1.concat a[0]

		b0[0].friends = c0
		b0[1].friends = c1
		b0[2].friends = c2
		b0[3].friends = c3
		b1[0].friends = c4
		b1[1].friends = c5
		b1[2].friends = c6
		b1[3].friends = c7

		c0[0].friends = d0
		c0[1].friends = d1
		c0[2].friends = d2
		c0[3].friends = d3

		c1[0].friends = d4
		c1[1].friends = d5
		c1[2].friends = d6
		c1[3].friends = d7

		c2[0].friends = d8
		c2[1].friends = d9
		c2[2].friends = d10
		c2[3].friends = d11

		c3[0].friends = d12
		c3[1].friends = d13
		c3[2].friends = d14
		c3[3].friends = d15

		c4[0].friends = d16
		c4[1].friends = d17
		c4[2].friends = d18
		c4[3].friends = d19

		c5[0].friends = d20
		c5[1].friends = d21
		c5[2].friends = d22
		c5[3].friends = d23

		c6[0].friends = d24
		c6[1].friends = d25
		c6[2].friends = d26
		c6[3].friends = d27

		c7[0].friends = d28
		c7[1].friends = d29
		c7[2].friends = d30
		c7[3].friends = d31

		a.concat b0, b1, c0, c1, c2, c3, c4, c5, c6, c7, d0, d1, d2, d3, d4, d5, d6, d7, d8, d9, d10, d11, d12, d13, d14, d15, d16, d17, d18, d19, d20, d21, d22, d23, d24, d25, d26, d27, d28, d29, d30, d31