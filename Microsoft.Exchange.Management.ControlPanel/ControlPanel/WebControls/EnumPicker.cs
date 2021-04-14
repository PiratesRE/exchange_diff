using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ClientScriptResource("EnumPicker", "Microsoft.Exchange.Management.ControlPanel.Client.Rules.js")]
	[ControlValueProperty("Value")]
	[ToolboxData("<{0}:EnumPicker runat=server></{0}:EnumPicker>")]
	public class EnumPicker : ScriptControlBase, INamingContainer
	{
		public EnumPicker() : base(HtmlTextWriterTag.Div)
		{
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddElementProperty("DropDownList", this.DropDownListID, this);
			descriptor.AddElementProperty("DescriptionLabel", this.DescriptionLabelID, this);
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			this.CssClass = "EnumPicker";
			DropDownList dropDownList = new DropDownList();
			dropDownList.ID = "bugfix_dropdownhiddenlist";
			dropDownList.Attributes["aria-label"] = "none";
			dropDownList.Style.Add(HtmlTextWriterStyle.Display, "none");
			dropDownList.Items.Add("hidden item");
			this.Controls.Add(dropDownList);
			this.dropDownList = new DropDownList();
			this.dropDownList.ID = "dropDownList";
			this.dropDownList.CssClass = "EnumPickerSelect";
			this.descriptionLabel = new Label();
			this.descriptionLabel.ID = "dropDownList_Description_label";
			this.descriptionLabel.CssClass = "EnumPickerDescription";
			this.Controls.Add(this.descriptionLabel);
			this.Controls.Add(this.dropDownList);
		}

		public string DropDownListID
		{
			get
			{
				this.EnsureChildControls();
				return this.dropDownList.ClientID;
			}
		}

		public string DescriptionLabelID
		{
			get
			{
				this.EnsureChildControls();
				return this.descriptionLabel.ClientID;
			}
		}

		private DropDownList dropDownList;

		private Label descriptionLabel;
	}
}
