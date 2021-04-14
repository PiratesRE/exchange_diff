using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ToolboxData("<{0}:UploaderBase runat=server></{0}:UploaderBase>")]
	[ClientScriptResource("UploaderBase", "Microsoft.Exchange.Management.ControlPanel.Client.WizardProperties.js")]
	public class UploaderBase : ScriptControlBase
	{
		public UploaderBase() : base(HtmlTextWriterTag.Div)
		{
			this.iframeElement = new HtmlGenericControl(HtmlTextWriterTag.Iframe.ToString());
			this.iframeElement.ID = "iframe";
			this.iframeElement.Attributes["class"] = "UploaderIframe" + (Util.IsSafari() ? " UploaderIframe-Safari" : string.Empty);
			if (Util.IsIE())
			{
				this.iframeElement.Attributes["src"] = ThemeResource.BlankHtmlPath;
			}
			this.Controls.Add(this.iframeElement);
			this.Controls.Add(new LiteralControl("<br />"));
			this.errorLabel = new EncodingLabel();
			this.errorLabel.ID = "errorLbl";
			this.Controls.Add(this.errorLabel);
			this.Bindings = new DataContractBinding();
		}

		[PersistenceMode(PersistenceMode.InnerDefaultProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BindingCollection Parameters
		{
			get
			{
				return this.parameters;
			}
		}

		[TypeConverter(typeof(StringArrayConverter))]
		public string[] ParameterNames { get; set; }

		public DataContractBinding Bindings { get; private set; }

		public UploadHandlers UploadHandlerClass { get; set; }

		[TypeConverter(typeof(StringArrayConverter))]
		public string[] Extensions { get; set; }

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			if (this.UploadHandlerClass == UploadHandlers.None)
			{
				throw new ArgumentNullException("UploadHandlerClass", "UploadHandlerClass must be set.");
			}
			descriptor.AddProperty("UploadHandlerClass", this.UploadHandlerClass.ToString());
			if (!this.Extensions.IsNullOrEmpty())
			{
				descriptor.AddProperty("Extensions", this.Extensions);
			}
			if (!this.ParameterNames.IsNullOrEmpty())
			{
				descriptor.AddProperty("ParameterNames", this.ParameterNames);
			}
			descriptor.AddElementProperty("Iframe", this.iframeElement.ClientID);
			descriptor.AddElementProperty("ErrorLbl", this.errorLabel.ClientID);
			foreach (Binding binding in this.Parameters)
			{
				this.Bindings.Bindings.Add(binding.Name, binding);
			}
			descriptor.AddScriptProperty("Parameters", this.Bindings.ToJavaScript(this));
		}

		private HtmlGenericControl iframeElement;

		private EncodingLabel errorLabel;

		private BindingCollection parameters = new BindingCollection();
	}
}
