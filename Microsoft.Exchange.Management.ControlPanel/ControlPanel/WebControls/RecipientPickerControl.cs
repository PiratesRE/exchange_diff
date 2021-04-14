using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ToolboxData("<{0}:RecipientPickerControl runat=server></{0}:RecipientPickerControl>")]
	[ClientScriptResource("RecipientPickerControl", "Microsoft.Exchange.Management.ControlPanel.Client.WizardProperties.js")]
	public class RecipientPickerControl : PickerControl
	{
		public RecipientPickerControl()
		{
			this.AllowTyping = true;
			this.PreferOwaPicker = true;
			base.ValueProperty = "PrimarySmtpAddress";
			if (string.IsNullOrEmpty(base.PickerFormUrl))
			{
				base.PickerFormUrl = "~/Pickers/MemberPicker.aspx";
			}
		}

		public bool SupportPostback { get; set; }

		public bool SingleSelect { get; set; }

		public bool UseVoicemailPicker { get; set; }

		public bool AllowTyping { get; set; }

		public bool PreferOwaPicker { get; set; }

		private bool UseOwaPicker
		{
			get
			{
				return this.PreferOwaPicker && OwaPickerUtil.CanUseOwaPicker;
			}
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			if (this.UseOwaPicker)
			{
				this.owaPickerUtil = new OwaPickerUtil();
				this.Controls.Add(this.owaPickerUtil);
			}
			if (this.SupportPostback)
			{
				this.hiddenField = new HiddenField();
				this.hiddenField.ID = "pbValue";
				this.Controls.Add(this.hiddenField);
			}
		}

		public string ValuePostBack
		{
			get
			{
				if (this.hiddenField != null)
				{
					return this.hiddenField.Value;
				}
				return null;
			}
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddProperty("SingleSelect", this.SingleSelect, true);
			descriptor.AddProperty("AllowTyping", this.AllowTyping, true);
			descriptor.AddProperty("UseVoicemailPicker", this.UseVoicemailPicker, false);
			if (this.SupportPostback)
			{
				descriptor.AddElementProperty("HiddenField", this.hiddenField.ClientID, true);
			}
			if (this.UseOwaPicker)
			{
				descriptor.AddComponentProperty("OwaPickerUtil", this.owaPickerUtil.ClientID, this);
			}
		}

		private OwaPickerUtil owaPickerUtil;

		private HiddenField hiddenField;
	}
}
