using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource("ImageUploader", "Microsoft.Exchange.Management.ControlPanel.Client.WizardProperties.js")]
	[ToolboxData("<{0}:ImageUploader runat=server></{0}:ImageUploader>")]
	public class ImageUploader : AjaxUploader
	{
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			if (this.editFileButton != null)
			{
				this.editFileButton.Attributes["NoRoleState"] = "Hide";
			}
			if (this.deleteButton != null)
			{
				this.deleteButton.Attributes["NoRoleState"] = "Hide";
			}
		}

		public WebServiceMethod CancelWebServiceMethod { get; set; }

		public WebServiceMethod SaveWebServiceMethod { get; set; }

		public WebServiceMethod RemoveWebServiceMethod { get; set; }

		public string ImageElementId { get; set; }

		public string RemovingPreviewPhotoText { get; set; }

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			Identity dataContract = Identity.FromExecutingUserId();
			Image image = (Image)this.Parent.FindControl(this.ImageElementId);
			descriptor.AddElementProperty("Image", image.ClientID);
			descriptor.AddComponentProperty("CancelWebServiceMethod", this.CancelWebServiceMethod.ClientID);
			descriptor.AddComponentProperty("SaveWebServiceMethod", this.SaveWebServiceMethod.ClientID);
			descriptor.AddComponentProperty("RemoveWebServiceMethod", this.RemoveWebServiceMethod.ClientID);
			descriptor.AddScriptProperty("ObjectIdentity", dataContract.ToJsonString(null));
			descriptor.AddProperty("RemovingPreviewPhotoText", this.RemovingPreviewPhotoText);
		}
	}
}
