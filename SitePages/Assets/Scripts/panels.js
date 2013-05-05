var Panels = Class.create(
{
	initialize: function(inElement, inOptions)
	{
		this.element = $(inElement);
		
		if(!this.element)
		{
			return;
		}
	
		this.options = {
			selected: null,
			animate: 'swipe',
			carousel: false,
			gap: 20,
			delay: 0,
			progressive: false,
			onBeforeShow: null,
			onAfterShow: null
		};
		
		Object.extend(this.options, inOptions);
		
		this.element.next = this.next.bind(this);
		
		this.element.previous = this.previous.bind(this);
		
		this.element.show = this.show.bind(this);
		
		this.element.isFirst = this.isFirst.bind(this);
		
		this.element.isLast = this.isLast.bind(this);
		
		this.element.index = this.index.bind(this);
		
		this.element.current = this.current.bind(this);
		
		this.element.setStyle({position:"relative"});
		
		this.visited = new Hash();

		// -------
		
		var selectedID = this.element.identify() + "selected";
		
		this.selectedElement = $(selectedID);
		
		if(!this.selectedElement)
		{
			this.selectedElement = new Element("input", {type:"hidden", name:selectedID});
			
			var parent = this.element.up("form") || document.body;
			
			parent.insert({bottom: this.selectedElement});
		}
		
		// -------

		this.panels = this.element.select("> .panel");
		
		if(this.panels.length == 0)
		{
			return;
		}
		
		for(var i = 0; i < this.panels.length; i++)
		{
			this.panels[i].setStyle({display: "none", position:"absolute", width:"100%"});
		}

		this.draw($(this.options.selected) || this.panels[0]);
		
		if(this.options.delay > 0)
		{
			this.interval = window.setInterval(this.advance.bind(this), this.options.delay);
		}
	},
	
	advance: function()
	{
		this.next(true);
	},
	
	previous: function(autoAdvance)
	{
		if(!autoAdvance)
		{
			window.clearInterval(this.interval);
		}
	
		if(this.selected == this.panels[0])
		{
			if(this.options.carousel)
			{
				this.draw(this.panels[this.panels.length - 1], true, true);
			}
		}
		else
		{
			this.draw(this.selected.previous(), true);
		}
	},
	
	next: function(autoAdvance)
	{
		if(!autoAdvance)
		{
			window.clearInterval(this.interval);
		}
		
		if(this.selected == this.panels[this.panels.length - 1])
		{
			if(this.options.carousel)
			{
				this.draw(this.panels[0], true, true);
			}
		}
		else
		{
			this.draw(this.selected.next(), true);
		}
	},
	
	show: function(inID)
	{
		var panel = Object.isNumber(inID) ? this.panels[inID]: $(inID);
		
		if(panel)
		{
			window.clearInterval(this.interval);
			
			if(this.options.progressive)
			{
				if(!this.visited.get(panel.identify()))
				{
					return;
				}
			}
			
			if(panel.descendantOf(this.element))
			{
				this.draw(panel, true);
			}
		}
	},
	
	index: function(inElement)
	{
		var element = inElement || this.selected;
		
		for(var i = 0; i < this.panels.length; i++)
		{
			if(element == this.panels[i])
			{
				return i;
			}
		}
		
		return -1;
	},
	
	isFirst: function()
	{
		return(this.selected == this.panels[0]);
	},
	
	isLast: function()
	{
		return(this.selected == this.panels[this.panels.length - 1]);
	},
	
	current: function()
	{
		return this.selected;
	},
	
	draw: function(inElement, inAnimate, inCarousel)
	{
		var oldIndex = -1;
		var oldElement = null;
		var newIndex = -1;
		var newElement = null;
	
		for(var i = 0; i < this.panels.length; i++)
		{
			if(this.panels[i] == inElement)
			{
				newIndex = i;
				newElement = this.panels[i];
			}
			
			if(this.panels[i] == this.selected)
			{
				oldIndex = i;
				oldElement = this.panels[i];
			}
		}
		
		if(oldIndex == newIndex || newIndex < 0)
		{
			return;
		}
		
		if(this.start(newElement, oldElement))
		{
			if(inAnimate && this.options.animate == "swipe")
			{
				var reverse = newIndex < oldIndex;

				if(inCarousel)
				{
					if(newIndex == 0 && (oldIndex == this.panels.length - 1))
					{
						reverse = false;
					}
					
					if(newIndex == (this.panels.length - 1) && oldIndex == 0)
					{
						reverse = true;
					}
				}
				
				var width = this.element.measure("width");
				var gap = this.options.gap;
				var start = reverse ? -(width + gap) : (width + gap);
				var end = 0;
	
				newElement.setStyle({left: start + "px", display:"block"});
	
				new Effect.Tween(null, start, end,
				{
					duration:0.7,
					afterFinish: function()
					{
						oldElement.setStyle({display:"none"});
						newElement.setStyle({left: end + "px"});
						
						this.finish(newElement, oldElement);
					}.bind(this)
				},
				function(position)
				{
					newElement.setStyle({left: position + "px"});
					
					if(reverse)
					{
						oldElement.setStyle({left: (width + position + gap) + "px"});
					}
					else
					{
						oldElement.setStyle({left: (position - width - gap) + "px"});
					}
				});
			}
			else if(inAnimate && this.options.animate == "dissolve")
			{			
				newElement.setStyle({opacity: 0, display:"block"});
	
				new Effect.Tween(null, 0, 1,
				{
					duration:0.7,
					afterFinish: function()
					{
						oldElement.setStyle({display:"none"});
						newElement.setStyle({opacity:1});
						
						this.finish(newElement, oldElement);
					}.bind(this)
				},
				function(position)
				{
					newElement.setStyle({opacity: position});
					oldElement.setStyle({opacity: (1 - position)});
				});
			}
			else
			{
				if(oldElement)
				{
					oldElement.setStyle({display: "none"});
				}
				
				newElement.setStyle({display: "block"});
					
				this.finish(newElement, oldElement);
			}
		}
	},
	
	start: function(inNewPanel, inOldPanel)
	{
		if(this.options.onBeforeShow)
		{
			try
			{
				this.options.onBeforeShow(inNewPanel, inOldPanel);
			}
			catch(exception)
			{
				return false;
			}
		}
		
		return true;
	},
	
	finish: function(inNewPanel, inOldPanel)
	{
		this.selected = inNewPanel;
	
		this.visited.set(this.selected.identify(), true);

		this.selectedElement.value = this.selected.identify();
			
		if(this.options.onAfterShow)
		{
			this.options.onAfterShow(inNewPanel, inOldPanel);
		}
		
		var related = $(this.selected.readAttribute("related"));
		
		if(related)
		{
			var siblings = related.siblings();
			
			for(var i = 0; i < siblings.length; i++)
			{
				siblings[i].removeClassName("on");
			}
			
			related.addClassName("on");
		}
	}
});