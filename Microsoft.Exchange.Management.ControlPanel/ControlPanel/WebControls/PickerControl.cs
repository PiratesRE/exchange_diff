using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ParseChildren(true, "Text")]
	[ToolboxData("<{0}:PickerControl runat=server></{0}:PickerControl>")]
	[DefaultProperty("Text")]
	[DataBindingHandler("System.Web.UI.Design.TextDataBindingHandler, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[ValidationProperty("Text")]
	[SupportsEventValidation]
	public abstract class PickerControl : PickerControlBase
	{
		[Bindable(BindableSupport.Yes)]
		[Localizable(true)]
		[DefaultValue(null)]
		[Editor("System.ComponentModel.Design.MultilineStringEditor,System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
		public string Text
		{
			get
			{
				return this.inlineSelector.Text;
			}
			set
			{
				this.inlineSelector.Text = value;
			}
		}

		[Browsable(false)]
		[Localizable(true)]
		public string BrowseButtonText
		{
			get
			{
				return this.inlineSelector.BrowseButtonText;
			}
			set
			{
				this.inlineSelector.BrowseButtonText = value;
			}
		}

		[DefaultValue(true)]
		public override bool Enabled
		{
			get
			{
				return this.inlineSelector.Enabled;
			}
			set
			{
				this.inlineSelector.Enabled = value;
			}
		}

		[DefaultValue("false")]
		public bool HideClearButton
		{
			get
			{
				return this.inlineSelector.HideClearButton;
			}
			set
			{
				this.inlineSelector.HideClearButton = value;
			}
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			this.Controls.Add(this.inlineSelector);
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddComponentProperty("InlineSelector", this.inlineSelector.ClientID, this);
		}

		private InlineSelector inlineSelector = new InlineSelector();
	}
}
