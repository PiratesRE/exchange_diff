using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.MessagingPolicies.AttachFilter
{
	[Cmdlet("get", "attachmentfilterlistconfig")]
	public class GetAttachmentFilterListConfig : GetSingletonSystemConfigurationObjectTask<AttachmentFilteringConfig>
	{
		protected override ObjectId RootId
		{
			get
			{
				return ((IConfigurationSession)base.DataSession).GetOrgContainerId();
			}
		}
	}
}
