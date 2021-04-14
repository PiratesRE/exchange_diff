using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Get", "DetailsTemplate", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class GetDetailsTemplate : GetSystemConfigurationObjectTask<DetailsTemplateIdParameter, DetailsTemplate>
	{
		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			DetailsTemplate detailsTemplate = dataObject as DetailsTemplate;
			if (detailsTemplate.Language != null)
			{
				detailsTemplate.MAPIPropertiesDictionary = MAPIPropertiesDictionaryFactory.GetPropertiesDictionary();
				detailsTemplate.BlobToPages();
				if (this.Identity == null || !detailsTemplate.Identity.Equals(this.Identity.RawIdentity))
				{
					detailsTemplate.MAPIPropertiesDictionary = null;
				}
				base.WriteResult(dataObject);
			}
		}
	}
}
