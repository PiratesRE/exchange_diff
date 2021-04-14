using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ClientScriptResource(null, "Microsoft.Exchange.Management.ControlPanel.Client.Extension.js")]
	public class UploadPackageStep : WizardStep
	{
		public UploadPackageStep()
		{
			base.ClientClassName = "UploadPackageStep";
			HtmlGenericControl htmlGenericControl = new HtmlGenericControl(HtmlTextWriterTag.Div.ToString());
			htmlGenericControl.Attributes["class"] = "AjaxUploaderButtonDiv";
			this.Controls.Add(new LiteralControl("<div class = \"ExtensionFileselect\">"));
			this.fileLocationLabel = new EncodingLabel();
			this.fileLocationLabel.ID = "fileLocationLbl";
			this.fileLocationLabel.Text = OwaOptionStrings.ExtensionPackageLocation;
			this.Controls.Add(this.fileLocationLabel);
			this.Controls.Add(new LiteralControl("<br /><br />"));
			this.uploaderBase = new UploaderBase();
			this.uploaderBase.ID = "uploader";
			htmlGenericControl.Controls.Add(this.uploaderBase);
			this.Controls.Add(htmlGenericControl);
			this.Controls.Add(new LiteralControl("</div>"));
			Util.RequireUpdateProgressPopUp(this);
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			foreach (Binding item in this.Parameters)
			{
				this.uploaderBase.Parameters.Add(item);
			}
		}

		public string ProgressDescription { get; set; }

		[PersistenceMode(PersistenceMode.InnerDefaultProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BindingCollection Parameters
		{
			get
			{
				return this.parameters;
			}
		}

		public UploadHandlers UploadHandlerClass
		{
			get
			{
				return this.uploaderBase.UploadHandlerClass;
			}
			set
			{
				this.uploaderBase.UploadHandlerClass = value;
			}
		}

		[TypeConverter(typeof(StringArrayConverter))]
		public string[] Extensions
		{
			get
			{
				return this.uploaderBase.Extensions;
			}
			set
			{
				this.uploaderBase.Extensions = value;
			}
		}

		protected override ScriptControlDescriptor GetScriptDescriptor()
		{
			ScriptControlDescriptor scriptDescriptor = base.GetScriptDescriptor();
			scriptDescriptor.AddComponentProperty("Properties", base.FindPropertiesParent().ClientID);
			scriptDescriptor.AddElementProperty("FileLocationLbl", this.fileLocationLabel.ClientID);
			scriptDescriptor.AddComponentProperty("UploaderImplementation", this.uploaderBase.ClientID);
			scriptDescriptor.AddProperty("ProgressDescription", this.ProgressDescription ?? OwaOptionStrings.PleaseWait);
			return scriptDescriptor;
		}

		private UploaderBase uploaderBase;

		private EncodingLabel fileLocationLabel;

		private BindingCollection parameters = new BindingCollection();
	}
}
