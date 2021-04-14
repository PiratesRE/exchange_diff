using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ClientScriptResource("WellKnownRecipientTypeControl", "Microsoft.Exchange.Management.ControlPanel.Client.WizardProperties.js")]
	[ToolboxData("<{0}:WellKnownRecipientTypeControl runat=server></{0}:WellKnownRecipientTypeControl>")]
	public class WellKnownRecipientTypeControl : ScriptControlBase
	{
		public WellKnownRecipientTypeControl() : base(HtmlTextWriterTag.Div)
		{
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			this.includeOnlySpecifiedRecipientTypesRadioButtonList = new EcpRadioButtonList();
			this.includeOnlySpecifiedRecipientTypesRadioButtonList.CssClass = "RadioInputTable";
			this.includeOnlySpecifiedRecipientTypesRadioButtonList.RepeatDirection = RepeatDirection.Vertical;
			this.includeOnlySpecifiedRecipientTypesRadioButtonList.Attributes.Add("uid", "includeOnlySpecifiedRecipientTypesRadioButtonList");
			this.includeOnlySpecifiedRecipientTypesRadioButtonList.Attributes.Add("data-control", "RadioButtonList");
			this.includeOnlySpecifiedRecipientTypesRadioButtonList.Items.Add(new ListItem(Strings.IncludeAllRecipientTypes, "false"));
			this.includeOnlySpecifiedRecipientTypesRadioButtonList.Items.Add(new ListItem(Strings.IncludeSpecifiedRecipientTypes, "true"));
			this.includeOnlySpecifiedRecipientTypesRadioButtonList.SelectedValue = "false";
			this.Controls.Add(this.includeOnlySpecifiedRecipientTypesRadioButtonList);
			HtmlGenericControl htmlGenericControl = new HtmlGenericControl("div");
			htmlGenericControl.Attributes.Add("class", "DependenceControl");
			this.specifiedRecipientTypesCheckBoxList = new EnumCheckBoxList();
			this.specifiedRecipientTypesCheckBoxList.CssClass = "CheckboxInputTable";
			this.specifiedRecipientTypesCheckBoxList.RepeatDirection = RepeatDirection.Vertical;
			this.specifiedRecipientTypesCheckBoxList.Attributes.Add("data-control", "EnumCheckBoxList");
			this.specifiedRecipientTypesCheckBoxList.Attributes.Add("data-enabled", "{value, Mode=OneWay, ElementId=includeOnlySpecifiedRecipientTypesRadioButtonList, ConvertTo=ValueConverter.StringToBool}");
			this.specifiedRecipientTypesCheckBoxList.Items.Add(new ListItem(Strings.MailboxUsers, "MailboxUsers"));
			this.specifiedRecipientTypesCheckBoxList.Items.Add(new ListItem(Strings.MailUsers, "MailUsers"));
			this.specifiedRecipientTypesCheckBoxList.Items.Add(new ListItem(Strings.ResourceMailboxes, "Resources"));
			this.specifiedRecipientTypesCheckBoxList.Items.Add(new ListItem(Strings.MailContacts, "MailContacts"));
			this.specifiedRecipientTypesCheckBoxList.Items.Add(new ListItem(Strings.MailGroups, "MailGroups"));
			this.specifiedRecipientTypesCheckBoxList.SelectedValue = string.Empty;
			htmlGenericControl.Controls.Add(this.specifiedRecipientTypesCheckBoxList);
			this.Controls.Add(htmlGenericControl);
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddComponentProperty("IncludeOnlySpecifiedRecipientTypesRadioButtonList", this.includeOnlySpecifiedRecipientTypesRadioButtonList.ClientID, this);
			descriptor.AddComponentProperty("SpecifiedRecipientTypesCheckBoxList", this.specifiedRecipientTypesCheckBoxList.ClientID, this);
		}

		private EcpRadioButtonList includeOnlySpecifiedRecipientTypesRadioButtonList;

		private EnumCheckBoxList specifiedRecipientTypesCheckBoxList;
	}
}
