using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ControlValueProperty("Value")]
	[ClientScriptResource("PeoplePicker", "Microsoft.Exchange.Management.ControlPanel.Client.Rules.js")]
	[ToolboxData("<{0}:PeoplePicker runat=server></{0}:PeoplePicker>")]
	public class PeoplePicker : PickerControlBase
	{
		public PeoplePicker() : base(HtmlTextWriterTag.Div)
		{
			this.AllowTyping = true;
			this.PreferOwaPicker = true;
			base.ValueProperty = "PrimarySmtpAddress";
			base.PickerFormUrl = "~/Pickers/MemberPicker.aspx";
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			if (this.owaPickerUtil != null)
			{
				descriptor.AddComponentProperty("OwaPickerUtil", this.owaPickerUtil.ClientID, this);
			}
			if (this.IsStandalonePicker)
			{
				descriptor.AddElementProperty("PickerLink", this.PickerLink);
				descriptor.AddProperty("PickerType", PickerType.PickUsersAndGroups.ToString());
				descriptor.AddProperty("IsStandalonePicker", this.IsStandalonePicker);
				descriptor.AddProperty("LinkText", this.LinkText.IsNullOrBlank() ? Strings.PeoplePickerSelectOne : this.LinkText);
			}
			descriptor.AddProperty("AllowTyping", this.AllowTyping, true);
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			if (this.PreferOwaPicker && OwaPickerUtil.CanUseOwaPicker)
			{
				this.owaPickerUtil = new OwaPickerUtil();
				this.Controls.Add(this.owaPickerUtil);
			}
			if (this.IsStandalonePicker)
			{
				this.linkPicker = new HyperLink();
				this.linkPicker.ID = "lnkPicker";
				this.linkPicker.Text = this.LinkText;
				this.Controls.Add(this.linkPicker);
				this.CssClass = "peoplePickerLink";
			}
		}

		public bool IsStandalonePicker { get; set; }

		public bool AllowTyping { get; set; }

		public string LinkText { get; set; }

		public string PickerLink
		{
			get
			{
				return this.linkPicker.ClientID;
			}
		}

		public bool PreferOwaPicker { get; set; }

		private OwaPickerUtil owaPickerUtil;

		private HyperLink linkPicker;
	}
}
