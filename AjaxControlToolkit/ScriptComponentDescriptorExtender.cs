using System;
using System.Web.UI;

namespace AjaxControlToolkit
{
	public static class ScriptComponentDescriptorExtender
	{
		public static void AddClientIdProperty(this ScriptComponentDescriptor descriptor, string name, Control control)
		{
			if (control == null || !control.Visible)
			{
				return;
			}
			descriptor.AddProperty(name, control.ClientID);
		}

		public static void AddComponentProperty(this ScriptComponentDescriptor descriptor, string name, string value, IControlResolver controlResolver)
		{
			if (!string.IsNullOrEmpty(value))
			{
				Control control = null;
				if (controlResolver != null)
				{
					control = controlResolver.ResolveControl(value);
				}
				if (control != null)
				{
					ExtenderControlBase extenderControlBase = control as ExtenderControlBase;
					if (extenderControlBase != null && extenderControlBase.BehaviorID.Length > 0)
					{
						value = extenderControlBase.BehaviorID;
					}
					else
					{
						value = control.ClientID;
					}
				}
				descriptor.AddComponentProperty(name, value);
			}
		}

		public static void AddComponentProperty(this ScriptComponentDescriptor descriptor, string name, string value, bool skipWithEmptyOrNullValue)
		{
			if (skipWithEmptyOrNullValue && string.IsNullOrEmpty(value))
			{
				return;
			}
			descriptor.AddComponentProperty(name, value);
		}

		public static void AddComponentProperty(this ScriptComponentDescriptor descriptor, string name, Control control)
		{
			if (control == null || !control.Visible)
			{
				return;
			}
			descriptor.AddComponentProperty(name, control.ClientID);
		}

		public static void AddElementProperty(this ScriptComponentDescriptor descriptor, string name, string value, IControlResolver controlResolver)
		{
			if (!string.IsNullOrEmpty(value))
			{
				Control control = null;
				if (controlResolver != null)
				{
					control = controlResolver.ResolveControl(value);
				}
				if (control != null)
				{
					value = control.ClientID;
				}
				descriptor.AddElementProperty(name, value);
			}
		}

		public static void AddElementProperty(this ScriptComponentDescriptor descriptor, string name, string value, bool skipWithEmptyOrNullValue)
		{
			if (skipWithEmptyOrNullValue && string.IsNullOrEmpty(value))
			{
				return;
			}
			descriptor.AddElementProperty(name, value);
		}

		public static void AddProperty(this ScriptComponentDescriptor descriptor, string name, string value, bool skipWithEmptyOrNullValue)
		{
			if (skipWithEmptyOrNullValue && string.IsNullOrEmpty(value))
			{
				return;
			}
			descriptor.AddProperty(name, value);
		}

		public static void AddProperty(this ScriptComponentDescriptor descriptor, string name, bool value, bool skipFalseValue)
		{
			if (value || !skipFalseValue)
			{
				descriptor.AddProperty(name, value);
			}
		}

		public static void AddProperty(this ScriptComponentDescriptor descriptor, string name, int value, int defaultValue)
		{
			if (value != defaultValue)
			{
				descriptor.AddProperty(name, value);
			}
		}

		public static void AddIDReferenceProperty(this ScriptComponentDescriptor descriptor, string name, string value, IControlResolver controlResolver)
		{
			if (!string.IsNullOrEmpty(value))
			{
				Control control = null;
				if (controlResolver != null)
				{
					control = controlResolver.ResolveControl(value);
				}
				if (control != null)
				{
					value = control.ClientID;
				}
				descriptor.AddProperty(name, value);
			}
		}

		public static void AddUrlProperty(this ScriptComponentDescriptor descriptor, string name, string value, IUrlResolutionService urlResolver)
		{
			if (!string.IsNullOrEmpty(value))
			{
				if (urlResolver != null)
				{
					value = urlResolver.ResolveClientUrl(value);
				}
				descriptor.AddProperty(name, value);
			}
		}

		public static void AddEvent(this ScriptComponentDescriptor descriptor, string name, string value, bool skipWithEmptyOrNullValue)
		{
			if (skipWithEmptyOrNullValue && string.IsNullOrEmpty(value))
			{
				return;
			}
			descriptor.AddEvent(name, value);
		}
	}
}
