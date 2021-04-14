using System;
using System.Web.UI;
using AjaxControlToolkit;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[RequiredScript(typeof(CommonToolkitScripts))]
	[RequiredScript(typeof(WizardForm))]
	[ClientScriptResource("EditDataClassification", "Microsoft.Exchange.Management.ControlPanel.Client.DLPPolicy.js")]
	public class EditDataClassification : BaseForm
	{
		public EditDataClassification()
		{
			Util.RequireUpdateProgressPopUp(this);
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			descriptor.AddComponentProperty("CollectionEditor", this.lvFingerprints.ClientID, true);
			descriptor.AddComponentProperty("AjaxUploader", this.fingerprintUploadHandler.ClientID, true);
			base.BuildScriptDescriptor(descriptor);
		}

		protected FingerprintCollectionEditor lvFingerprints;

		protected FingerprintUploader fingerprintUploadHandler;
	}
}
