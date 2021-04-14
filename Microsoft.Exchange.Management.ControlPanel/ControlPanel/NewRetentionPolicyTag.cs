using System;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class NewRetentionPolicyTag : BaseForm
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			bool includeArchive = true;
			this.InitializeControls();
			NameValueCollection queryString = this.Context.Request.QueryString;
			string text = queryString["typeGroup"];
			if (!string.IsNullOrEmpty(text))
			{
				this.divType.Visible = false;
				string text2;
				if (string.Compare(text, "SystemFolder", true) == 0)
				{
					text2 = Strings.NewRetentionTagForSystemFolder;
					this.tbxTypeHidden.Text = string.Empty;
					this.divType.Visible = true;
					this.tbxTypeHidden.Visible = false;
					RetentionUtils.PopulateRetentionTypes(this.ddlType);
					includeArchive = false;
				}
				else if (string.Compare(text, "All", true) == 0)
				{
					text2 = Strings.NewRetentionTagForAll;
					this.tbxTypeHidden.Text = "All";
				}
				else
				{
					if (string.Compare(text, "Personal", true) != 0)
					{
						throw new BadQueryParameterException("typeGroup", new ArgumentException(string.Format("Retention tag type group [{0}] is not supported.", text)));
					}
					text2 = Strings.NewRetentionTagForPersonal;
					this.tbxTypeHidden.Text = "Personal";
				}
				base.Title = text2;
				base.Caption = text2;
				RetentionUtils.PopulateRetentionActions(this.rblRetentionAction, includeArchive);
				return;
			}
			throw new BadQueryParameterException("typeGroup", new ArgumentException("Retention tag type group is required."));
		}

		private void InitializeControls()
		{
			PropertyPageSheet propertyPageSheet = (PropertyPageSheet)base.ContentPanel.FindControl("RetentionPolicyTagProperties");
			Section section = (Section)propertyPageSheet.FindControl("GroupInformationSection");
			this.divType = (HtmlControl)section.FindControl("divType");
			this.ddlType = (DropDownList)section.FindControl("ddlType");
			this.tbxTypeHidden = (TextBox)section.FindControl("tbxTypeHidden");
			this.rblRetentionAction = (RadioButtonList)section.FindControl("rblRetentionAction");
		}

		private const string TypeGroupAll = "All";

		private const string TypeGroupSystemFolder = "SystemFolder";

		private const string TypeGroupPersonal = "Personal";

		private const string PropertyID = "RetentionPolicyTagProperties";

		private const string SectionID = "GroupInformationSection";

		private const string TypePanelID = "divType";

		private const string TypeID = "ddlType";

		private const string TypeGroupID = "tbxTypeHidden";

		private const string RetentionActionID = "rblRetentionAction";

		private HtmlControl divType;

		private DropDownList ddlType;

		private TextBox tbxTypeHidden;

		private RadioButtonList rblRetentionAction;
	}
}
