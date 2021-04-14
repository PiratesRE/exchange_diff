using System;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource("MoreOptionRecipientConditionEditor", "Microsoft.Exchange.Management.ControlPanel.Client.Rules.js")]
	[RequiredScript(typeof(CommonToolkitScripts))]
	public abstract class MoreOptionRecipientConditionEditor : RecipientConditionEditorBase
	{
		protected override void Render(HtmlTextWriter writer)
		{
			this.moreOptionId = this.ClientID + "_moreOption";
			writer.Write(string.Format("<a id=\"{0}\">{1}</a>", this.moreOptionId, Strings.MoreOptionsLabel));
			base.Render(writer);
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			descriptor.AddElementProperty("MoreOptionButton", this.moreOptionId);
			base.BuildScriptDescriptor(descriptor);
		}

		private string moreOptionId;
	}
}
