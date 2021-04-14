using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public abstract class ControlBinding : Binding
	{
		public ControlBinding(Control control)
		{
			this.Control = control;
		}

		private protected Control Control { protected get; private set; }

		protected virtual string ClientID
		{
			get
			{
				if (!(this.Control is RadioButtonList))
				{
					return this.Control.ClientID;
				}
				return this.Control.UniqueID;
			}
		}

		public sealed override string ToJavaScript(IControlResolver resolver)
		{
			if (this.Control.Visible)
			{
				return this.ToJavaScriptWhenVisible(resolver);
			}
			return "null";
		}

		protected virtual string ToJavaScriptWhenVisible(IControlResolver resolver)
		{
			return string.Format("new {0}('{1}')", base.GetType().Name, this.ClientID);
		}
	}
}
