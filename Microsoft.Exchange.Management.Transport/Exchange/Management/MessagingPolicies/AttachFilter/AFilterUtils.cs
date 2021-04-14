using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.SystemConfigurationTasks;

namespace Microsoft.Exchange.Management.MessagingPolicies.AttachFilter
{
	internal class AFilterUtils
	{
		internal static AttachmentFilteringConfig GetAFilterConfig(IConfigDataProvider session)
		{
			ObjectId rootId = null;
			try
			{
				rootId = ((IConfigurationSession)session).GetOrgContainerId();
			}
			catch (OrgContainerNotFoundException)
			{
				throw new AttachmentFilterADEntryNotFoundException();
			}
			catch (TenantOrgContainerNotFoundException)
			{
				throw new AttachmentFilterADEntryNotFoundException();
			}
			IConfigurable[] array = session.Find<AttachmentFilteringConfig>(null, rootId, false, null);
			if (array.Length != 1)
			{
				throw new AttachmentFilterADEntryNotFoundException();
			}
			return array[0] as AttachmentFilteringConfig;
		}
	}
}
