using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ClientScriptResource("CallerIdsPicker", "Microsoft.Exchange.Management.ControlPanel.Client.Rules.js")]
	[ToolboxData("<{0}:CallerIdsPicker runat=server></{0}:CallerIdsPicker>")]
	[ControlValueProperty("Value")]
	public class CallerIdsPicker : ScriptControlBase, INamingContainer
	{
		public CallerIdsPicker() : base(HtmlTextWriterTag.Div)
		{
			this.isFromNumbersChbx = new CheckBox();
			this.isInContactsFolderChbx = new CheckBox();
			this.phoneNumbersInEditor = new InlineEditor();
			this.isAmongSelectedContactsChbx = new CheckBox();
			this.openPeoplePickerLink = new HyperLink();
			this.phoneNumbersInEditor.ValidationExpression = CommonRegex.GetRegexExpressionById("UMNumberingPlanFormat").ToString();
			this.phoneNumbersInEditor.ValidationErrorMessage = Strings.CallerIdsInlineEditorErrorText;
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			this.EnsureChildControls();
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddElementProperty("IsFromNumbersChbx", this.isFromNumbersChbx.ClientID, this);
			descriptor.AddElementProperty("IsInContactsFolderChbx", this.isInContactsFolderChbx.ClientID, this);
			descriptor.AddComponentProperty("PhoneNumbersInEditor", this.phoneNumbersInEditor.ClientID, this);
			descriptor.AddElementProperty("IsAmongSelectedContactsChbx", this.isAmongSelectedContactsChbx.ClientID, this);
			descriptor.AddElementProperty("OpenPeoplePickerLink", this.openPeoplePickerLink.ClientID, this);
			descriptor.AddProperty("PeoplePickerLinkText", Strings.CallerIdsPeoplePickerLinkText, false);
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			this.isFromNumbersChbx.ID = "isFromNumbersId";
			this.isInContactsFolderChbx.ID = "isInContactsFolderId";
			this.phoneNumbersInEditor.ID = "phoneNumbersId";
			this.isAmongSelectedContactsChbx.ID = "isAmongSelectedContactsId";
			this.openPeoplePickerLink.ID = "openPeoplePickerId";
			Table table = new Table();
			table.CssClass = "RuleParametersTable";
			TableRow tableRow = new TableRow();
			TableCell tableCell = new TableCell();
			tableRow.Cells.Add(tableCell);
			TableRow tableRow2 = new TableRow();
			TableCell cell = new TableCell();
			tableRow2.Cells.Add(cell);
			TableRow tableRow3 = new TableRow();
			TableCell tableCell2 = new TableCell();
			tableRow3.Cells.Add(tableCell2);
			TableRow tableRow4 = new TableRow();
			TableCell tableCell3 = new TableCell();
			tableRow4.Cells.Add(tableCell3);
			this.phoneNumbersInEditor.InputWaterMarkText = Strings.CallerIdsInlineEditorWatermarkText;
			this.phoneNumbersInEditor.CssClass = "PhoneNumbersInEditor";
			this.isFromNumbersChbx.Text = Strings.CallerIdsCallingFromNumbersText;
			this.isInContactsFolderChbx.Text = Strings.CallerIdsIsInContactsFolderText;
			this.isAmongSelectedContactsChbx.Text = Strings.CallerIdsIsAmongContactsText;
			this.openPeoplePickerLink.Text = Strings.CallerIdsPeoplePickerLinkText;
			tableCell3.Controls.Add(this.isAmongSelectedContactsChbx);
			tableCell3.Controls.Add(this.openPeoplePickerLink);
			tableCell3.CssClass = "RuleParameterEditor";
			tableCell.Controls.Add(this.isFromNumbersChbx);
			tableCell.Controls.Add(this.phoneNumbersInEditor);
			tableCell2.Controls.Add(this.isInContactsFolderChbx);
			table.Rows.Add(tableRow);
			table.Rows.Add(tableRow2);
			table.Rows.Add(tableRow3);
			table.Rows.Add(tableRow4);
			this.Controls.Add(table);
		}

		private CheckBox isFromNumbersChbx;

		private CheckBox isInContactsFolderChbx;

		private CheckBox isAmongSelectedContactsChbx;

		private InlineEditor phoneNumbersInEditor;

		private HyperLink openPeoplePickerLink;
	}
}
