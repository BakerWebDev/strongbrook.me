var Client = 
{
	initialize: function(startup)
	{
		document.observe('keydown', this.keydown.bindAsEventListener(this));
		
		document.observe('keyup', this.keyup.bindAsEventListener(this));
		
		this.menus = [];
		
		this.startup = startup;
		
		Event.observe(document, 'dom:loaded', function()
		{
			Client.ready();
		});
	},
	
	update: function(startup)
	{
		this.startup = startup;
		
		Client.checkReady();
	},
	
	when: function(event, action)
	{
		document.on(event, action);
		
		document.on("behavior:ready", action);
	},
	
	keydown: function(inEvent)
	{
		this.defaultButton = null;
		this.cancelButton = null;
	
		if(inEvent.keyCode == 18) // option key
		{
			document.body.addClassName("optionkey");
		}
		else if(inEvent.keyCode == Event.KEY_RETURN)
		{
			var element = inEvent.element();
			
			if(element.getAttribute("contenteditable"))
			{
				return;
			}
			else if(element.tagName.toLowerCase() == 'textarea')
			{
				return;
			}
			
			this.defaultButton = null;
			
			var buttons = document.body.select(".defaultbutton");

			var currentModal = Modal.current();
			
			for(var i = 0; i < buttons.length; i++)
			{
				if(currentModal && buttons[i].descendantOf(currentModal))
				{
					this.defaultButton = buttons[i];
					
					break;
				}
				else if(!buttons[i].up(".modal"))
				{
					this.defaultButton = buttons[i];
					
					break;
				}
			}

			Event.stop(inEvent);
		}
		else if(inEvent.keyCode == Event.KEY_ESC)
		{
			this.cancelButton = null;
		
			var buttons = document.body.select(".cancelbutton");

			var currentModal = Modal.current();
			
			for(var i = 0; i < buttons.length; i++)
			{
				if(currentModal && buttons[i].descendantOf(currentModal))
				{
					this.cancelButton = buttons[i];
					
					break;
				}
				else if(!buttons[i].up(".modal"))
				{
					this.cancelButton = buttons[i];
					
					break;
				}
			}
			
			Event.stop(inEvent);
		}
	},
	
	keyup: function(inEvent)
	{
		if(inEvent.keyCode == 18) // option
		{
			document.body.removeClassName("optionkey");
		}
		else if(inEvent.keyCode == Event.KEY_RETURN && this.defaultButton)
		{
			this.defaultButton.click();
			
			Event.stop(inEvent);
		}
		else if(inEvent.keyCode == Event.KEY_ESC && this.cancelButton)
		{
			this.cancelButton.click();
			
			Event.stop(inEvent);
		}
	},

	addMenu: function(inElement)
	{
		this.menus.push(inElement);
	},
	
	hideMenus: function()
	{
		for(var i = 0; i < this.menus.length; i++)
		{
			this.menus[i].hideMenu();
		}
		
		this.menus.clear();
	},
	
	setActiveElement: function(inElement, clear)
	{
		if(clear)
		{
			if(this.activeElement == inElement)
			{
				this.activeElement = null;
			}
		}
		else
		{
			this.activeElement = $(inElement);
		}
	},
	
	getActiveElement: function()
	{
		var tagName = document.activeElement.tagName.toLowerCase();
		
		if(tagName != "input" && tagName != "textarea" && tagName != "select" && document.activeElement.readAttribute("contenteditable") != "true")
		{
			return this.activeElement;
		}
		
		return document.activeElement;
	},
	
	libraries: 0,
	
	loadLibrary: function(type, url)
	{
		if(type == "js")
		{
			var scripts = $$("head script");
			
			for(var i = 0; i < scripts.length; i++)
			{
				if(scripts[i].src.indexOf(url) >= 0)
				{
					return;
				}
			}
			
			Client.libraries++;
		
			var element = document.createElement('script');
			
			element.setAttribute("type","text/javascript");
			element.setAttribute("src", url);
			element.setAttribute("onload", "Client.libraries--; Client.checkReady();");
			element.setAttribute("onerror", "Client.libraries--; Client.checkReady();");

			document.getElementsByTagName("head")[0].appendChild(element);
		 }
		 else if(type == "css")
		 {
		 	var links = $$("head link");
			
			for(var i = 0; i < links.length; i++)
			{
				if(links[i].href.indexOf(url) >= 0)
				{
					return;
				}
			}
			
			var element = document.createElement("link");
			element.setAttribute("rel", "stylesheet");
			element.setAttribute("type", "text/css");
			element.setAttribute("href", url);
			
			document.getElementsByTagName("head")[0].appendChild(element);
		 }
	},
	
	checkReady: function()
	{
		if(Client.libraries <= 0)
		{
			document.fire("dom:loaded");
			
			Client.libraries = 0;
		}
	},
	
	ready: function()
	{
		if(this.startup)
		{
			this.startup();
		
			this.startup = null;
		}
	
		var focusElements = $$("input[autofocus=true]");
	
		for(var i = 0; i < focusElements.length; i++)
		{
			if(!focusElements[i].up(".modal"))
			{
				focusElements[i].activate();	
			
				focusElements[i].writeAttribute("autofocus", "");
				
				break;
			}
		}
		
		var requiredElements = $$("input[require=true]");
	
		for(var i = 0; i < requiredElements.length; i++)
		{
			requiredElements[i].observe('keyup', this.updateRequired.bindAsEventListener(this));
		
			this.drawRequired(requiredElements[i]);
			
			requiredElements[i].writeAttribute("require", "");
		}
		
		document.fire("behavior:ready");
	},
	
	resize: function()
	{
		document.fire('behavior:resize');
	},
	
	updateRequired: function(event)
	{
		this.drawRequired(event.element());
	},
	
	drawRequired: function(element)
	{
		if(element)
		{
			if(element.value.length == 0)
			{
				element.addClassName('required');
			}
			else
			{
				element.removeClassName('required');
			}
		}
	}
};

window.onresize = Client.resize.bind(Client);

// -------------

var Modal =  
{
	modals: [],
	
	show: function(inID, inOptions)
	{
		var modalContainer = $(inID);
		
		if(modalContainer == this.current())
		{
			return;
		}
		
		this.options = $H(inOptions);
		
		var modal = modalContainer.down('.modal');

		var frame = modalContainer.down("iframe");
		
		var parent = modalContainer.up('form');
		
		var url = this.getURL();
		
		if(!parent)
		{
			parent = document.body;
		}
		
		if(url && frame)
		{
			frame.setAttribute("src", url);
		}

		parent.insert({bottom: modalContainer});

		new Effect.Tween(null, 0, 1,
		{
			duration:0.3,
			beforeStart: function()
			{
				modalContainer.setStyle({display:"block"});
				
				var position = (document.viewport.getDimensions().height - modal.measure("height")) / 3;
		
				modal.setStyle({WebkitTransform:"scale(0)", top: position + "px"});
			},
			afterFinish: function()
			{
				modal.setStyle({WebkitTransform:"scale(1.0)"});
			}
		},
		function(position)
		{
			modal.setStyle({WebkitTransform:"scale(" + position + ")"});
		});
		
		var focusElement = modal.down("input[autofocus=true]");
	
		if(focusElement)
		{
			focusElement.activate();
		}
		
		this.modals.push(modalContainer);
		
		document.fire('behavoir:update');
	},
	
	hide: function(inOptions)
	{
		var modalContainer = this.modals.pop();
	
		if(modalContainer)
		{
			this.set(inOptions);
		
			var modal = modalContainer.down('.modal');
		
			new Effect.Tween(null, 1, 0,
			{
				duration:0.3,
				beforeStart: function()
				{
					modal.setStyle({WebkitTransform:"scale(1)"});
				},
				afterFinish: function()
				{
					modalContainer.setStyle({display:"none"});
				}
			},
			function(position)
			{
				modal.setStyle({WebkitTransform:"scale(" + position + ")"});
			});
		}
		else
		{
			if(window.parent && window.parent != window)
			{
				window.parent.Modal.hide(inOptions);
			}
		}
	},
	
	current: function()
	{
		if(this.modals.length == 0)
		{
			return null;
		}
		
		var modalContainer = this.modals[this.modals.length - 1];
		
		if(modalContainer)
		{
			return modalContainer.identify();
		}
		
		return null;
	},
	
	getURL: function()
	{
		var url = this.options.get('url');
		
		if(url)
		{
			this.options.each(function(pair)
			{
				if(pair.key == "url")
				{
					return;
				}
				
				var name = "%" + pair.key + "%";
				
				var element = $(pair.value);
			
				if(element)
				{
					if(element.activate)
					{
						url = url.replace(name, element.value);
					}
					else
					{
						url = url.replace(name, element.innerHTML);
					}
				}
			});
		}
		
		return url;
	},
	
	set: function(inOptions)
	{
		var updateOptions = $H(inOptions);
		
		this.options.each(function(pair)
		{
			if(pair.key == "url")
			{
				return;
			}
			
			var element = $(pair.value);
			
			if(element)
			{
				var value = updateOptions.get(pair.key);
			
				if(value)
				{
					var tagName = element.tagName.toLowerCase();
					
					if(tagName == 'input' || tagName == 'select' || tagName == 'textarea')
					{
						element.value = value;
					}
					else
					{
						element.update(value);
					}
				}
			}
		});
	}
};


// -------------

var Notification =
{
	position: function(inPosition)
	{
		return this;
	},
	
	show: function(inData, inOptions)
	{
		if(!this.element)
		{
			this.element = new Element("div", {"class": "notification"});
			
			this.element.on("mousedown", this.hide.bindAsEventListener(this));
		}
		
		this.options = {
			autoClose: true,
			delay: 5000,
			onClose: null
		};
		
		Object.extend(this.options, inOptions);

		window.clearTimeout(this.timeout);

		this.element.setStyle({display:"none"});
		
		// ---------
		
		if(inData.startsWith("id:"))
		{
			var content = $(inData.substring(3));
			
			if(content)
			{
				this.element.update(content);
			}
		}
		else
		{
			var content = new Element("div");
			
			content.setStyle({width: "150px"});
			content.update(inData);
			
			this.element.update(content);
		}
		
		// ---------
		
		document.body.insert({bottom:this.element});
		
		new Effect.Tween(null, 0, 1,
		{
			duration:1.5,
			beforeStart: function()
			{
				this.element.setStyle({opacity:0.0, display:"block"});
			}.bind(this)
		},
		function(position)
		{
			this.element.setStyle({opacity:position});
		}.bind(this));
		
		if(this.options.autoClose)
		{
			this.timeout = window.setTimeout(this.hide.bind(this), this.options.delay);
		}
		
		return this;
	},
	
	hide: function()
	{
		window.clearTimeout(this.timeout);
		
		new Effect.Tween(null, 1, 0,
		{
			duration:0.7,
			afterFinish: function()
			{
				this.element.setStyle({display:"none"});
				
				if(this.options.onClose)
				{
					this.options.onClose();
				}
			}.bind(this)
		},
		function(position)
		{
			this.element.setStyle({opacity:position});
		}.bind(this));
		
		return this;
	},
};

// --------------

var Format = 
{
	parts: function(number, precision, thousands, decimal, currencyBefore, currencyAfter, negativeBefore, negativeAfter, trimZeros)
	{
		var x = Math.round(number * Math.pow(10, precision));
		
		if(x >= 0)
		{
			negativeBefore = negativeAfter = '';
		}
		
		if(precision <= 0)
		{
			decimal = '';
		}
		
		var y = ('' + Math.abs(x)).split('');
		
		var z = y.length - precision;

		if(z < 0)
		{
			z--;
		}

		for(var i = z; i < 0; i++)
		{
			y.unshift('0');
		}

		if(z < 0)
		{
			z = 1;
		}

		y.splice(z, 0, decimal);

		if(y[0] == decimal)
		{
			y.unshift('0');
		}

		while(z > 3)
		{
			z -= 3;
			y.splice(z, 0, thousands);
		}
	
		if(y.indexOf(decimal) > 0 && trimZeros)
		{
			var length = y.length - 1;
			
			for(var i = 0; i < precision; i++)
			{
				if(y[length - i] == '0')
				{
					y.pop();
				}
			}
			
			if(y[y.length - 1] == decimal)
			{
				y.pop();
			}
		}
		
		return currencyBefore + negativeBefore + y.join('') + negativeAfter + currencyAfter;
	},
	
	number: function(inNumber, inFormat, inAltFormat)
	{
		if(isNaN(inNumber))
		{
			return inNumber;
		}
		
		var type = undefined;
		var precision = undefined;
		var trimZeros = undefined;
		
		if(inAltFormat)
		{
			type = inAltFormat.format != undefined ? inAltFormat.format : type;
			precision = inAltFormat.precision != undefined ? inAltFormat.precision : precision;
			trimZeros = inAltFormat.trimZeros != undefined ? inAltFormat.trimZeros : trimZeros;
		}
	
		if(inFormat)
		{
			type = inFormat.format != undefined ? inFormat.format : type;
			precision = inFormat.precision != undefined ? inFormat.precision : precision;
			trimZeros = inFormat.trimZeros != undefined ? inFormat.trimZeros : trimZeros;
		}
		
		if(type == undefined)
		{
			type = "number";
		}
		
		if(precision == undefined)
		{
			precision = 0;
		}
		
		if(trimZeros == undefined)
		{
			trimZeros = true;
		}
	
		if(type == "number")
		{
			return this.parts(inNumber, precision, ',', '.', '', '', '-', '', trimZeros);
		}
		else if(type == "year")
		{
			return this.parts(inNumber, 0, '', '.', '', '', '', '', false);
		}
		else if(type == "currency")
		{
			return this.parts(inNumber, precision, ',', '.', '$', '', '(', ')', false);
		}
		else if(type == "percent")
		{
			return this.parts(inNumber * 100,  precision, ',', '.', '', '%', '-', '', trimZeros);
		}
		else if(type == "date")
		{
			var date = new Date(inNumber * 24 * 60 * 60 * 1000);

			if(precision == 1)
			{
				return this.weekdays[date.getDay()] + " " + date.getDate() + " " + this.months[date.getMonth()];
			}
			else if(precision == 2)
			{
				return date.getDate() + " " + this.months[date.getMonth()] + " " + date.getFullYear();
			}
			else if(precision == 3)
			{
				return this.weekdays[date.getDay()] + " " + date.getDate() + " " + this.months[date.getMonth()] + " " + date.getFullYear();
			}
	
			return date.getDate() + " " + this.months[date.getMonth()];
		}
		else
		{
			return "" + inNumber;
		}
	},
	
	months: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],

	weekdays: ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat']
};

//--------------

var Color =
{
	lighter: function(inColor, inAdjustment)
	{
		return this.adjust(inColor, inAdjustment, true);
	},
	
	darker: function(inColor, inAdjustment)
	{
		return this.adjust(inColor, inAdjustment, false);
	},
	
	shade: function(inColor, inAdjustment)
	{
		var hsv = this.RGBtoHSV(this.RGB(inColor));
		
		return this.adjust(inColor, inAdjustment, hsv.value <= 0.85);
	},
	
	adjust: function(inColor, inAdjustment, isLighter)
	{
		var rgb = this.RGB(inColor);
		var hsv = this.RGBtoHSV(rgb);
		
		if(rgb.red == rgb.green && rgb.red == rgb.blue)
		{
			if(isLighter)
			{
				hsv.value += (1.0 - hsv.value) * inAdjustment;
			}
			else
			{
				hsv.value -= (hsv.value) * inAdjustment;
			}
		}
		else
		{
			if(isLighter)
			{
				hsv.saturation -= (hsv.saturation) * inAdjustment;
				hsv.value += (1.0 - hsv.value) * inAdjustment;
			}
			else
			{
				hsv.value -= (hsv.value) * inAdjustment;
				hsv.saturation += (1.0 - hsv.saturation) * inAdjustment;
			}

		}

		return this.color(this.HSVtoRGB(hsv));
	},
	
	color: function(inRGBA)
	{
		var color = "#";
		
		color += this.intToHex(inRGBA.red) + this.intToHex(inRGBA.green) + this.intToHex(inRGBA.blue);
		
		if(inRGBA.alpha)
		{
			color += this.intToHex(inRGBA.alpha);
		}
		
		return color;
	},
	
	RGB: function(inColor)
	{
		var parts = {red:0, green:0, blue:0};

		if(inColor.charAt(0) == '#')
		{
			inColor = inColor.substring(1);
		}

		if(inColor.length == 3)
		{
			inColor = "" + inColor.charAt(0) + inColor.charAt(0) + inColor.charAt(1) + inColor.charAt(1) + inColor.charAt(2) + inColor.charAt(2);
		}

		if(inColor.length < 6)
		{
			return parts;
		}

		parts.red += this.hexToInt(inColor.charAt(0)) << 4;
		parts.red += this.hexToInt(inColor.charAt(1));

		parts.green += this.hexToInt(inColor.charAt(2)) << 4;
		parts.green += this.hexToInt(inColor.charAt(3));

		parts.blue += this.hexToInt(inColor.charAt(4)) << 4;
		parts.blue += this.hexToInt(inColor.charAt(5));

		if(inColor.length == 8)
		{
			parts.alpha = 0;
			parts.alpha += this.hexToInt(inColor.charAt(6)) << 4;
			parts.alpha += this.hexToInt(inColor.charAt(7));
		}

		return parts;
	},
	
	RGBtoHSV: function(inRGBA)
	{
		var h = 0.0;
		var s = 0.0;
		var v = 0.0;

		var r = inRGBA.red / 255.0;
		var g = inRGBA.green / 255.0;
		var b = inRGBA.blue / 255.0;

		var max = Math.max(r, Math.max(g, b));
		var min = Math.min(r, Math.min(g, b));

		v = max;

		if(max != 0.0)
		{
			s = (max - min) / max;
		}

		if(s != 0.0)
		{
			var delta = max - min;

			if(r == max)
			{
				h = (g - b) / delta;
			}
			else if(g == max)
			{
				h = 2.0 + (b - r) / delta;
			}
			else if(b == max)
			{
				h = 4.0 + (r - g) / delta;
			}

			h *= 60.0;

			if(h < 0.0)
			{
				h += 360.0;
			}
		}

		return { hue:h, saturation:s, value:v };
	},
	
	HSVtoRGB: function(inHSV)
	{
		var r = 0;
		var g = 0;
		var b = 0;
		var h = inHSV.hue;
		var s = inHSV.saturation;
		var v = inHSV.value;

		if(s == 0.0)
		{
			r = v;
			g = v;
			b = v;
		}
		else
		{
			h /= 60.0;

			var i = Math.floor(h);
			var f = h - i;
			var p = v * (1.0 - s);
			var q = v * (1.0 - s * f);
			var t = v * (1.0 - s * (1.0 - f));

			switch(i)
			{
				case 0:
					r = v;
					g = t;
					b = p;
					break;
				case 1:
					r = q;
					g = v;
					b = p;
					break;
				case 2:
					r = p;
					g = v;
					b = t;
					break;
				case 3:
					r = p;
					g = q;
					b = v;
					break;
				case 4:
					r = t;
					g = p;
					b = v;
					break;
				default: // case 5:
					r = v;
					g = p;
					b = q;
					break;
			}
		}

		return { red: parseInt(r *= 255), green: parseInt(g *= 255), blue: parseInt(b *= 255) };
	},
	
	hexToInt: function(inChar)
	{
		var index = this.hexValues.indexOf(inChar.toLowerCase().charAt(0));
		
		if(index < 0)
		{
			index = 0;
		}
		
		return index;
	},
	
	intToHex: function(inValue)
	{
		return ("" + this.hexValues[parseInt(inValue / 16)] + this.hexValues[inValue % 16]);
	},
	
	hexValues: ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f" ]
};

function setCookie(name, value, expirydays, path)
{
	var expiry;

	if(!name || name == "")
		return;
		
	if(expirydays && expirydays > 0)
	{
		var	expiryDate = new Date();
		
		expiryDate.setTime(expiryDate.getTime() + (expirydays * 24 * 60 * 60 * 1000));
		
		expiry = expiryDate.toGMTString();
	}
	
	var cookieString = name + "=" + escape(value) +
					   ((!expiry) ? "" : ("; expires=" + expiry)) +
					   ((!path)   ? "" : ("; path=" + path));

	document.cookie = cookieString;
}

function getCookie(name)
{
	var arg = name + "=";
	var arglength = arg.length;
	var cookielength = document.cookie.length;
	var i = 0;
	
	while(i < cookielength)
	{
		var j = i + arglength;
		
		if(document.cookie.substring(i, j) == arg)
		{
			k = document.cookie.indexOf(";", j);
			
			if(k < 0)
			{
				k = cookielength;
			}
			
			return unescape(document.cookie.substring(j, k));
		}
			
		i = document.cookie.indexOf(" ", i) + 1;
		
		if(i == 0)
		{
			break;
		}
	}
	
	return "";
}

// ----------
// Utility

function checkCapsLock(e)
{
	var myKeyCode = 0;
	var myShiftKey = false;
	var myCapsLock = false;

	if(document.all) // Internet Explorer
	{
		e = window.event;
		myKeyCode = e.keyCode;
		myShiftKey = e.shiftKey;
	}
	else if(document.layers) // Netscape 4
	{
		myKeyCode = e.which;
		myShiftKey = (myKeyCode == 16) ? true : false;
	}
	else if(document.getElementById) // Netscape 6
	{
		myKeyCode = e.which;
		myShiftKey = e.shiftKey || ( e.modifiers && ( e.modifiers & 4 ) );
	}

	if((myKeyCode >= 65 && myKeyCode <= 90) && !myShiftKey)
	{
		myCapsLock = true;
	}
	else if((myKeyCode >= 97 && myKeyCode <= 122) && myShiftKey)
	{
		myCapsLock = true;
	}

	var detector = document.getElementById("capslockdetector");

	if(myCapsLock)
	{
		detector.style.display = "";
	}
	else
	{
		detector.style.display = "none";
	}
}

function maxLength(inEvent, inElement, inLength)
{
	if(Prototype.Browser.IE)
	{
		;
	}
	else
	{
		if(inElement.selectionStart != inElement.selectionEnd)
		{
			return true;
		}
		else if(inEvent.which == 0 || inEvent.which == inEvent.DOM_VK_BACK_SPACE)
		{
			return true;
		}
	}
	
	if(inElement.value.length >= inLength)
	{
		inElement.value = inElement.value.substring(0, inLength);
		
		return false;
	}
	
	return true;
}

function trimLength(element, length)
{
	if(element.value.length >= length)
	{
		element.value = element.value.substring(0, length);
	}
}

// ------------
// Actions

Element.addMethods(
{
	parseNumber: function(element)
	{
		var value = element.getValue ? element.getValue() : element.innerHTML;
		
		return parseFloat(value.gsub(/[^-\.\d]/, "")) || 0;
	},
	
	when: function(element, event, action)
	{
		var element = $(element);
		
		element.on(event, action);
		
		element.on("behavior:" + event, action);
		
		document.on("behavior:ready", action);
		
		return element;
	},
	
	showWhen: function(element, test)
	{
		var changed = false;

		if(test)
		{
			changed = element.getStyle("display") == "none";

			element.show();
		}
		else
		{
			changed = element.getStyle("display") != "none";

			element.hide();
		}
		
		if(changed)
		{
			document.fire("behavior:resize");
		}
		
		return element;
	},
	
	enableWhen: function(element, test)
	{
		if(test)
		{
			if(element.enable)
			{
				element.enable();
			}
			else
			{
				element.disabled = false;
			}
		}
		else
		{
			if(element.disable)
			{
				element.disable();
			}
			else
			{
				element.disabled = true;
			}
		}
		
		return element;
	},

	addClassNameWhen: function(element, className, test)
	{
		if(test)
		{
			element.addClassName(className);
		}
		else
		{
			element.removeClassName(className);
		}
		
		return element;
	}
});

//----------------
// Scroll Wheel

Object.extend(Event,
{
	wheel:function (event)
	{
		var delta = 0;
		
		if(!event)
		{
			event = window.event;
		}
		
		if(event.wheelDelta)
		{
			delta = event.wheelDelta / 120;
			
			if (window.opera)
			{
				delta = -delta;
			}
			
		}
		else if(event.detail)
		{
			delta = -event.detail / 3;
		}
		
		return Math.round(delta); //Safari Round
	}
});

//----------------
// Browser Detection

Object.extend(Prototype,
{
	OS:(function()
	{
		return {
			Windows:      navigator.userAgent.indexOf('Windows') > -1,
			Mac:          navigator.userAgent.indexOf('Mac') > -1,
			Linux:        navigator.userAgent.indexOf('Linux') > -1,
			UNIX:         navigator.userAgent.indexOf('X11') > -1
		}
	})()
});

Object.extend(Prototype.Browser, 
{
	IE6: (/MSIE (\d+\.\d+);/.test(navigator.userAgent)) ? (Number(RegExp.$1) == 6 ? true : false) : false,
	IE7: (/MSIE (\d+\.\d+);/.test(navigator.userAgent)) ? (Number(RegExp.$1) == 7 ? true : false) : false,
	IE8: (/MSIE (\d+\.\d+);/.test(navigator.userAgent)) ? (Number(RegExp.$1) == 8 ? true : false) : false,
	IE9: (/MSIE (\d+\.\d+);/.test(navigator.userAgent)) ? (Number(RegExp.$1) == 9 ? true : false) : false
});

