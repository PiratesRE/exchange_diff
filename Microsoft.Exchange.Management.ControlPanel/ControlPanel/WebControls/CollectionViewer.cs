using System;
using System.Collections.Generic;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ClientScriptResource("CollectionViewer", "Microsoft.Exchange.Management.ControlPanel.Client.Common.js")]
	[ControlValueProperty("Value")]
	public class CollectionViewer : ScriptControlBase, INamingContainer, IScriptControl
	{
		public CollectionViewer() : base(HtmlTextWriterTag.Div)
		{
		}

		public bool WrapItems { get; set; }

		public string DisplayProperty
		{
			get
			{
				return this.displayProperty;
			}
			set
			{
				this.displayProperty = value;
			}
		}

		IEnumerable<ScriptDescriptor> IScriptControl.GetScriptDescriptors()
		{
			return new ScriptControlDescriptor[]
			{
				this.GetScriptDescriptor()
			};
		}

		IEnumerable<ScriptReference> IScriptControl.GetScriptReferences()
		{
			return ScriptObjectBuilder.GetScriptReferences(base.GetType());
		}

		private ScriptControlDescriptor GetScriptDescriptor()
		{
			ScriptControlDescriptor scriptControlDescriptor = new ScriptControlDescriptor("CollectionViewer", this.ClientID);
			if (string.IsNullOrEmpty(this.DisplayProperty))
			{
				scriptControlDescriptor.AddScriptProperty("DisplayProperty", "function($_){ return $_; }");
			}
			else
			{
				scriptControlDescriptor.AddScriptProperty("DisplayProperty", "function($_){ return $_." + this.DisplayProperty + "; }");
			}
			scriptControlDescriptor.AddProperty("WrapItems", this.WrapItems);
			return scriptControlDescriptor;
		}

		private string displayProperty = string.Empty;
	}
}
