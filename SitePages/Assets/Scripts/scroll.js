var Scroll = Class.create(
{
	initialize: function(inElement, inOptions)
	{
		this.element = $(inElement);
		
		if(!this.element)
		{
			return;
		}
		
		this.options = {
			scrollTo: null,
			parent: null,
			handle: null,
			position: 0,
			height: 0,
			distributeClass: null,
			onBottom: null
		};
			
		Object.extend(this.options, inOptions);
		
		if(this.options.parent)
		{
			this.parent = $(this.options.parent);
			
			if(this.parent)
			{
				this.parent.setStyle({overflowY:"hidden"});
			}
		}
		
		if(!this.parent)
		{
			this.parent = this.element;
		
			if(this.options.handle)
			{
				this.handle = $(this.options.handle);
				
				if(this.handle)
				{
					this.handle.observe('mousedown', this.mousedown.bindAsEventListener(this));
				}
			}
			
			if(this.handle)
			{
				this.minimum = parseInt(this.parent.getStyle("minHeight"));
				
				this.maximum = parseInt(this.parent.getStyle("maxHeight"));
				
				if(this.minimum > 0 && this.options.height < this.minimum)
				{
					this.options.height = this.minimum;
				}
				
				if(this.maximum > 0 && this.options.height > this.maximum)
				{
					this.options.height = this.maximum;
				}
			}
			
			if(this.options.height > 0)
			{
				this.parent.setStyle({height: this.options.height + "px"});
			}
		}

		// ------------

		document.on('behavior:ready', this.ready.bindAsEventListener(this));
		
		document.on('behavior:resize', this.resize.bindAsEventListener(this));
	
		document.on("behavior:submit", this.save.bindAsEventListener(this));
		
		this.reset.bind(this).defer();
	},
	
	reset: function()
	{
		if(this.options.scrollTo)
		{
			var scrollTo = this.element.down('.' + this.options.scrollTo);
		
			if(scrollTo)
			{
				this.element.scrollTop = scrollTo.positionedOffset().top;
			}
		} 
		else if(this.options.position > 0)
		{
			this.element.scrollTop = this.options.position;
		}
	},
	
	ready: function(inEvent)
	{
		this.distributeAdjustment = 0;
		
		if(this.parent != this.element)
		{
			if(this.options.distributeClass)
			{
				children = this.element.select("." + this.options.distributeClass);
				
				for(var i = 0; i < children.length; i++)
				{
					var layout = children[i].getLayout();
	
					this.distributeAdjustment += layout.get("margin-top");
					this.distributeAdjustment += layout.get("border-top");
					this.distributeAdjustment += layout.get("padding-top");
					this.distributeAdjustment += layout.get("padding-bottom");
					this.distributeAdjustment += layout.get("border-bottom");
					this.distributeAdjustment += layout.get("margin-bottom");
				}
			}
		}
		
		this.resize();
	},
	
	resize: function()
	{
		if(this.parent != this.element)
		{
			var adjustment = this.parent.measure("height");
		
			var children = this.parent.childElements();
		
			for(var i = 0; i < children.length; i++)
			{
				adjustment -= children[i].measure("margin-box-height");
			}
			
			var height = this.element.measure("height") + adjustment;
			
			if(height < 0)
			{
				height = 0;
			}
			
			this.element.setStyle({ height: height + "px" });
			
			if(this.options.distributeClass)
			{
				children = this.element.select("." + this.options.distributeClass);
				
				var distributeHeight = (height - this.distributeAdjustment) / children.length;

				for(var i = 0; i < children.length; i++)
				{
					var height = Math.round(distributeHeight * (i + 1)) - Math.round(distributeHeight * i);
					
					children[i].setStyle({ height: height + "px" });
				}
			}
		}
	},
	
	save: function(inEvent)
	{
		var visible = true;
		
		var ancestors = this.element.ancestors();
		
		for(var i = 0; i < ancestors.length; i++)
		{
			if(!ancestors[i].visible())
			{
				visible = false;
				
				break;
			}
		}
		
		if(visible)
		{
			var positionID = this.element.identify() + "-scroll";
		
			var positionElement = $(positionID);
			
			if(!positionElement)
			{
				positionElement = new Element("input", {name:positionID, type:"hidden"});
				
				this.element.insert({before: positionElement});
			}
			
			positionElement.value = this.element.scrollTop + "," + this.parent.getHeight();
		}
	},
	
	mousedown: function(inEvent)
	{
		if(this.handle)
		{
			var element = inEvent.element();
			
			if(element.hasClassName("input") || element.up(".input"))
			{
				return;
			}
			
			this.mousemoveFunction = this.mousemove.bindAsEventListener(this);
			this.mouseupFunction = this.mouseup.bindAsEventListener(this);

			document.observe('mousemove', this.mousemoveFunction);
			document.observe('mouseup', this.mouseupFunction);
		
			this.sourceY = inEvent.pointerY();
			this.sourceHeight = this.parent.getHeight();
			this.resizing = true;

			Event.stop(inEvent);
		}
	},
	
	mousemove: function(inEvent)
	{
		if(this.handle && this.resizing)
		{
			var height = this.sourceHeight + inEvent.pointerY() - this.sourceY;
	
			if(this.minimum > 0 && height < this.minimum)
			{
				height = this.minimum;
			}
	
			if(this.maximum > 0 && height > this.maximum)
			{
				height = this.maximum;
			}

			this.parent.setStyle({height: height + "px"});
			
			document.fire('behavior:resize');
		}
	},
	
	mouseup: function(inEvent)
	{
		if(this.handle && this.resizing)
		{
			document.stopObserving('mousemove', this.mousemoveFunction);
			document.stopObserving('mouseup', this.mouseupFunction);
			
			this.resizing = false;
		}
	}
});