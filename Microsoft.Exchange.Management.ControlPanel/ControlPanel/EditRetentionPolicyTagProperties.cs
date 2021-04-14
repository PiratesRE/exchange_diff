using System;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public sealed class EditRetentionPolicyTagProperties : PropertyPageSheet
	{
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			this.InitializeControls();
			PowerShellResults<JsonDictionary<object>> powerShellResults = (PowerShellResults<JsonDictionary<object>>)base["PreLoadResults"];
			if (powerShellResults != null && powerShellResults.SucceededWithValue)
			{
				Dictionary<string, object> dictionary = powerShellResults.Value;
				bool includeArchive = false;
				if ((string)dictionary["Type"] == ElcFolderType.All.ToString() || (string)dictionary["Type"] == ElcFolderType.Personal.ToString())
				{
					this.divType.Visible = false;
					includeArchive = true;
				}
				RetentionUtils.PopulateRetentionActions(this.rblRetentionAction, includeArchive);
			}
		}

		private void InitializeControls()
		{
			Section section = base.Sections["GroupInformationSection"];
			this.divType = (HtmlControl)section.FindControl("divType");
			this.rblRetentionAction = (RadioButtonList)section.FindControl("rblRetentionAction");
		}

		private const string SectionID = "GroupInformationSection";

		private const string TypePanelID = "divType";

		private const string RetentionActionID = "rblRetentionAction";

		private HtmlControl divType;

		private RadioButtonList rblRetentionAction;
	}
}
