using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource("AdhocValues", "Microsoft.Exchange.Management.ControlPanel.Client.Common.js")]
	public class AdhocValues : ScriptControlBase
	{
		public AdhocValues()
		{
			this.Values = new List<ValuePair>();
			this.Name = "AdhocValues";
		}

		[DefaultValue("AdhocValues")]
		public string Name { get; set; }

		[MergableProperty(false)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DefaultValue("")]
		public List<ValuePair> Values { get; private set; }

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			this.CssClass = "hidden";
			base.Attributes["data-control"] = "AdhocValues";
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			foreach (ValuePair child in this.Values)
			{
				this.Controls.Add(child);
			}
		}

		protected override string TagName
		{
			get
			{
				return "var";
			}
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			this.DataBind();
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			foreach (ValuePair valuePair in this.Values)
			{
				dictionary.Add(valuePair.Name, valuePair.Value);
			}
			descriptor.AddScriptProperty("Values", dictionary.ToJsonString(null));
			descriptor.AddProperty("Name", this.Name);
		}
	}
}
