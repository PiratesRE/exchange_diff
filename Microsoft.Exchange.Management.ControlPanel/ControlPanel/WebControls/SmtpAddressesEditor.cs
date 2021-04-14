using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ParseChildren(true, "Value")]
	[ToolboxData("<{0}:SmtpAddressesEditor runat=server></{0}:SmtpAddressesEditor>")]
	[DefaultProperty("Value")]
	[ClientScriptResource("SmtpAddressesEditor", "Microsoft.Exchange.Management.ControlPanel.Client.WizardProperties.js")]
	public class SmtpAddressesEditor : ScriptControlBase
	{
		public SmtpAddressesEditor() : base(HtmlTextWriterTag.Span)
		{
			this.ValueProperty = "PrimarySmtpAddress";
			this.pickerButton = new HtmlButton();
			this.pickerButton.ID = "pickerButton";
			this.textBox = new TextBox();
			this.textBox.ID = "textBox";
			this.ButtonText = Strings.AddUsers;
		}

		[Browsable(false)]
		public SmtpAddressesEditorType EditorType { get; set; }

		[Localizable(true)]
		[DefaultValue(null)]
		public string Value
		{
			get
			{
				return this.textBox.Text;
			}
			set
			{
				this.textBox.Text = value;
			}
		}

		[Browsable(false)]
		public int MaxLength
		{
			get
			{
				return this.textBox.MaxLength;
			}
			set
			{
				this.textBox.MaxLength = value;
			}
		}

		[Localizable(true)]
		[Browsable(false)]
		public string ButtonText
		{
			get
			{
				return this.pickerButton.InnerText;
			}
			set
			{
				this.pickerButton.InnerText = value;
			}
		}

		[Browsable(false)]
		public string PickerFormUrl { get; set; }

		[TypeConverter(typeof(StringArrayConverter))]
		public string[] PickerRoles { get; set; }

		[Browsable(true)]
		[DefaultValue("PrimarySmtpAddress")]
		public string ValueProperty { get; set; }

		public bool SingleSelect
		{
			get
			{
				return this.EditorType == SmtpAddressesEditorType.SingleSmtpAddress;
			}
		}

		[DefaultValue("false")]
		public bool IsRequiredField { get; set; }

		protected override void CreateChildControls()
		{
			this.hasButton = (this.PickerRoles == null || LoginUtil.IsInRoles(this.Context.User, this.PickerRoles));
			if (this.hasButton)
			{
				this.CreateChildControlsWithButton();
				return;
			}
			this.CreateChildControlsWithoutButton();
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			EcpRegularExpressionValidator ecpRegularExpressionValidator = null;
			switch (this.EditorType)
			{
			case SmtpAddressesEditorType.SingleSmtpAddress:
				ecpRegularExpressionValidator = new EcpRegularExpressionValidator();
				ecpRegularExpressionValidator.ValidationExpression = "^\\s*\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*\\s*$";
				break;
			case SmtpAddressesEditorType.MultipleSmtpAddresses:
				ecpRegularExpressionValidator = new EcpRegularExpressionValidator();
				ecpRegularExpressionValidator.ValidationExpression = "^\\s*([\\w-+.]+@[\\w-+.]+(\\s*,\\s*[\\w-+.]+@[\\w-+.]+)*)*\\s*$";
				break;
			case SmtpAddressesEditorType.RecipientIds:
				ecpRegularExpressionValidator = new EcpAliasListValidator();
				break;
			case SmtpAddressesEditorType.FreeForm:
				ecpRegularExpressionValidator = new EcpRegularExpressionValidator();
				ecpRegularExpressionValidator.ValidationExpression = "^(.)+$";
				break;
			}
			this.AddValidator(ecpRegularExpressionValidator, "_Validator1");
			if (this.IsRequiredField)
			{
				this.AddValidator(new EcpRequiredFieldValidator(), "_Validator2");
			}
		}

		protected override void Render(HtmlTextWriter writer)
		{
			this.textBox.Enabled = this.Enabled;
			this.pickerButton.Disabled = !this.Enabled;
			base.Render(writer);
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			if (this.hasButton)
			{
				descriptor.AddElementProperty("PickerButton", this.pickerButton.ClientID);
			}
			descriptor.AddElementProperty("TextBox", this.textBox.ClientID);
			descriptor.AddProperty("PickerFormUrl", base.ResolveClientUrl(this.PickerFormUrl));
			descriptor.AddProperty("ValueProperty", this.ValueProperty);
			descriptor.AddProperty("SingleSelect", this.SingleSelect);
		}

		private void CreateChildControlsWithoutButton()
		{
			Panel panel = new Panel();
			panel.Controls.Add(this.textBox);
			panel.CssClass = "saeTextboxPadding";
			this.Controls.Add(panel);
		}

		private void CreateChildControlsWithButton()
		{
			Table table = new Table();
			table.CellPadding = 0;
			table.CellSpacing = 0;
			table.CssClass = "saeTable";
			TableRow tableRow = new TableRow();
			TableCell tableCell = new TableCell();
			tableCell.Controls.Add(this.textBox);
			tableCell.CssClass = "saePaddingCol";
			this.textBox.CssClass = "saeTextbox";
			tableRow.Cells.Add(tableCell);
			table.Rows.Add(tableRow);
			Table table2 = new Table();
			table2.CellPadding = 0;
			table2.CellSpacing = 1;
			table2.CssClass = "singleSelect";
			tableRow = new TableRow();
			TableCell tableCell2 = new TableCell();
			tableCell2.Controls.Add(table);
			tableRow.Cells.Add(tableCell2);
			TableCell tableCell3 = new TableCell();
			this.pickerButton.CausesValidation = false;
			this.pickerButton.Attributes["onClick"] = "javascript:return false;";
			this.pickerButton.Attributes.Add("class", "pickerBrowseButton" + (Util.IsIE() ? " pickerBrowseButton-IE" : string.Empty));
			tableCell3.Controls.Add(this.pickerButton);
			tableRow.Cells.Add(tableCell3);
			table2.Rows.Add(tableRow);
			Panel panel = new Panel();
			panel.Controls.Add(table2);
			this.Controls.Add(panel);
		}

		private void AddValidator(BaseValidator validator, string name)
		{
			validator.ID = this.textBox.ID + name;
			validator.ControlToValidate = this.textBox.ID;
			this.Controls.Add(validator);
		}

		private TextBox textBox;

		private HtmlButton pickerButton;

		private bool hasButton;
	}
}
