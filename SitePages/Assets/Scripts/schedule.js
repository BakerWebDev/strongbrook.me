var Scheduler =
{
	initialize: function(inOptions)
	{
		this.options = {
			updateURL: null,
			scheduleURL: null
		};
		
		this.data = {
			base: 0,
			labels: [],
			invalid: []
		};
		
		this.day = 0;
		this.hour = 0;
		this.score = 0;
		
		Object.extend(this.options, inOptions);
		
		this.times = $$(".time");
	
		$('first').on('keyup', this.updateButtons.bindAsEventListener(this));
		$('last').on('keyup', this.updateButtons.bindAsEventListener(this));
		$('phone').on('keyup', this.updateButtons.bindAsEventListener(this));
		$('email').on('keyup', this.updateButtons.bindAsEventListener(this));
		
		$('first').on('change', this.updateButtons.bindAsEventListener(this));
		$('last').on('change', this.updateButtons.bindAsEventListener(this));
		$('phone').on('change', this.updateButtons.bindAsEventListener(this));
		$('email').on('change', this.updateButtons.bindAsEventListener(this));
		
		$('credit').on('change', this.updateButtons.bindAsEventListener(this));
		$('equity').on('change', this.updateButtons.bindAsEventListener(this));
		$('income').on('change', this.updateButtons.bindAsEventListener(this));
		$('savings').on('change', this.updateButtons.bindAsEventListener(this));
		$('retirement').on('change', this.updateButtons.bindAsEventListener(this));
		
		document.on('click', this.click.bindAsEventListener(this));
		document.on('dblclick', this.dblclick.bindAsEventListener(this));

		this.updateTimes();
		this.updateButtons();
		
		window.setInterval(this.updateTimes.bind(this), 10 * 60 * 1000);
	},
	
	click: function(inEvent)
	{
		var element = inEvent.element();
		
		if(!element.hasClassName("time"))
		{
			element = element.up(".time");
		}
		
		if(!element)
		{
			return;
		}
		
		if(element.hasClassName("invalid"))
		{
			return;
		}
		
		for(var i = 0; i < this.times.length; i++)
		{
			this.times[i].removeClassName("selected");
		}
		
		element.addClassName("selected");
		
		var id = element.identify();
		
		this.day = parseInt(id.substring(1, 2)) + this.data.base;
		this.hour = parseInt(id.substring(3));
	
		this.updateButtons();
	},
	
	dblclick: function(inEvent)
	{
		var element = inEvent.element();
		
		if(!element.hasClassName("time"))
		{
			element = element.up(".time");
		}
		
		if(!element)
		{
			return;
		}
		
		if(element.hasClassName("invalid"))
		{
			return;
		}
		
		$('ok').click();
	},
	
	updateButtons: function()
	{
		this.score = 2;
		this.score += parseInt($('equity').value) >= 3 ? 1 : 0;
		this.score += parseInt($('savings').value) >= 2 ? 1 : 0;
		this.score += parseInt($('retirement').value) >= 2 ? 1 : 0;
	
		var qualified = this.score >= 0;  // was 3

		$('phoneschedule').showWhen(!qualified);
		$('timeschedule').showWhen(qualified);

		$('ok').enableWhen((this.day > 0 && this.hour > 0) || !qualified);
		
		var selected = $('panels').current().identify();
		
		if(selected == "introduction")
		{
			$('next').enableWhen($('first').value.length > 0 && $('last').value.length > 0 && $('phone').value.length > 0 && $('email').value.length > 0);
		}
		else if(selected == "questions")
		{
			$('next').enableWhen($('credit').value != "0" && $('equity').value != "0" && $('income').value != "0" && $('savings').value != "0" && $('retirement').value != "0");
		}
		
		var firstname = $('first').value;
		var firstnames = document.body.select(".firstname");
		
		for(var i = 0; i < firstnames.length; i++)
		{
			firstnames[i].update(firstname);
		}
	},
	
	drawButtons: function(inNew, inOld)
	{
		var newPanel = inNew.identify();
	
		if(newPanel == 'introduction')
		{
			$('next').show().disable();
			$('previous').show().disable();
			$('cancel').show().enable();
			
			$('ok').hide().disable();
			$('done').hide().disable();
		}
		else if(newPanel == 'questions')
		{
			$('next').show().disable();
			$('previous').show().enable();
			$('cancel').show().enable();
			
			$('ok').hide().disable();
			$('done').hide().disable();
		}
		else if(newPanel == 'times')
		{
			$('previous').show().enable();
			$('ok').show().disable();
			$('cancel').show().enable();
			
			$('done').hide().disable();
			$('next').hide().disable();
		}
		else if(newPanel == 'reciept')
		{
			$('done').show().enable();
			
			$('cancel').hide().disable();
			$('ok').hide().disable();
			$('next').hide().disable();
			$('previous').hide().disable();
		}
		
		this.updateButtons();
	},
	
	updateTimes: function()
	{
		new Ajax.Request(this.options.updateURL,
		{
			onComplete: function(response)
			{
				if(response.status == 200)
				{
					this.drawTimes(response.responseText.evalJSON(true));
				}
			}.bind(this)
		});
	},
	
	drawTimes: function(inData)
	{
		Object.extend(this.data, inData);
		
		for(var i = 0; i < this.times.length; i++)
		{
			this.times[i].removeClassName("invalid");
			this.times[i].removeClassName("selected");
		}
		
		for(var i = 0; i < this.data.labels.length; i++)
		{
			var day = $('d' + i);
			
			if(day)
			{
				day.update(this.data.labels[i]);
			}
		}
	
		for(var i = 0; i < this.data.invalid.length; i++)
		{
			var time = $(this.data.invalid[i]);
	
			if(time)
			{
				time.addClassName("invalid");
			}
		}
		
		if(this.day > 0)
		{
			var time = $("d" + (this.day - this.data.base) + "h" + this.hour);
			
			if(time)
			{
				if(time.hasClassName("invalid"))
				{
					this.day = 0;
					this.hour = 0;
				}
				else
				{
					time.addClassName("selected");
				}
				
				this.updateButtons();
			}
		}
	},
	
	submit: function()
	{
		var parameters = new Hash();
			
		var form = document.body.down("form");
	
		if(form)
		{
			parameters = form.serialize(true);
		}
		
		parameters.day = this.day;
		parameters.hour = this.hour;
		parameters.score = this.score;
		
		new Ajax.Request(this.options.scheduleURL,
		{
			parameters: parameters,
			
			onComplete: function(response)
			{
				if(response.status == 200)
				{
					$('reminder').update(response.responseText);
					
					$('panels').next();
				}
			}.bind(this)
		});
	
		
	}
};