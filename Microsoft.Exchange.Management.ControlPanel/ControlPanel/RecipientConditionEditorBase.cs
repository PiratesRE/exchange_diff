using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[RequiredScript(typeof(CommonToolkitScripts))]
	[ClientScriptResource("RecipientConditionEditor", "Microsoft.Exchange.Management.ControlPanel.Client.Rules.js")]
	public abstract class RecipientConditionEditorBase : ScriptControlBase, INamingContainer
	{
		[DefaultValue(false)]
		public bool HasDefaultItem { get; set; }

		protected abstract RulePhrase[] SupportedConditions { get; }

		public RecipientConditionEditorBase() : base(HtmlTextWriterTag.Div)
		{
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddScriptProperty("AllConditions", this.SupportedConditions.ToJsonString(null));
			descriptor.AddProperty("HasDefaultItem", this.HasDefaultItem);
		}

		protected override void CreateChildControls()
		{
			List<Type> list = new List<Type>();
			RuleEditor.GetRequiredFormlets(this.SupportedConditions, list);
			Panel panel = new Panel();
			panel.Style.Add(HtmlTextWriterStyle.Display, "none");
			foreach (Type type in list)
			{
				if (string.Equals(type.Name, "PeoplePicker", StringComparison.Ordinal))
				{
					PeoplePicker peoplePicker = (PeoplePicker)Activator.CreateInstance(type);
					peoplePicker.PreferOwaPicker = false;
					peoplePicker.AllowTyping = true;
					peoplePicker.IsStandalonePicker = false;
					panel.Controls.Add(peoplePicker);
				}
				else
				{
					panel.Controls.Add((Control)Activator.CreateInstance(type));
				}
			}
			this.Controls.Add(panel);
			base.CreateChildControls();
		}
	}
}
