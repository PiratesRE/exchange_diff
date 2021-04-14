using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Restore", "DetailsTemplate", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High, DefaultParameterSetName = "Identity")]
	public sealed class RestoreDetailsTemplate : SystemConfigurationObjectActionTask<DetailsTemplateIdParameter, DetailsTemplate>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRestoreDetailsTemplate(this.Identity.ToString());
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			DetailsTemplate detailsTemplate = (DetailsTemplate)base.PrepareDataObject();
			detailsTemplate[DetailsTemplateSchema.TemplateBlob] = detailsTemplate[DetailsTemplateSchema.TemplateBlobOriginal];
			detailsTemplate.MAPIPropertiesDictionary = MAPIPropertiesDictionaryFactory.GetPropertiesDictionary();
			detailsTemplate.BlobToPages();
			return detailsTemplate;
		}
	}
}
