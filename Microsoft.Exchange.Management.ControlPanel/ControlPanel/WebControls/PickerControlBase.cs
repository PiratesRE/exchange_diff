using System;
using System.ComponentModel;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ClientScriptResource("PickerControlBase", "Microsoft.Exchange.Management.ControlPanel.Client.WizardProperties.js")]
	public abstract class PickerControlBase : ScriptControlBase, INamingContainer
	{
		public PickerControlBase() : this(HtmlTextWriterTag.Div)
		{
		}

		protected PickerControlBase(HtmlTextWriterTag tag) : base(tag)
		{
			this.DisplayProperty = "DisplayName";
			this.ValueProperty = "Identity";
		}

		public string PickerFormUrl
		{
			get
			{
				return this.pickerFormUrl;
			}
			set
			{
				this.pickerFormUrl = value;
			}
		}

		[DefaultValue("DisplayName")]
		public string DisplayProperty { get; set; }

		[DefaultValue("Identity")]
		public string ValueProperty { get; set; }

		public int? DialogHeight { get; set; }

		public int? DialogWidth { get; set; }

		[Description("Picker Content to build query parameters")]
		[TemplateInstance(TemplateInstance.Single)]
		[Browsable(false)]
		[DefaultValue(null)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[TemplateContainer(typeof(PropertiesContentPanel))]
		public virtual ITemplate Content { get; set; }

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			if (this.Content != null)
			{
				PropertiesContentPanel propertiesContentPanel = new PropertiesContentPanel();
				propertiesContentPanel.ID = "contentContainer";
				this.Controls.Add(propertiesContentPanel);
				this.Content.InstantiateIn(propertiesContentPanel);
			}
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			if (!string.IsNullOrEmpty(this.PickerFormUrl))
			{
				descriptor.AddProperty("PickerFormUrl", base.ResolveClientUrl(this.PickerFormUrl));
			}
			descriptor.AddProperty("DisplayProperty", this.DisplayProperty);
			descriptor.AddProperty("ValueProperty", this.ValueProperty);
			if (this.DialogHeight != null)
			{
				descriptor.AddProperty("DialogHeight", this.DialogHeight);
			}
			if (this.DialogWidth != null)
			{
				descriptor.AddProperty("DialogWidth", this.DialogWidth);
			}
		}

		private string pickerFormUrl;
	}
}
