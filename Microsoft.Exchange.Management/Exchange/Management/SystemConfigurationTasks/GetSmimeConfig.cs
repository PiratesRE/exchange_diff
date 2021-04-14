using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "SmimeConfig")]
	public sealed class GetSmimeConfig : GetMultitenancySingletonSystemConfigurationObjectTask<SmimeConfigurationContainer>
	{
		protected override ObjectId RootId
		{
			get
			{
				return SmimeConfigurationContainer.GetWellKnownParentLocation(base.CurrentOrgContainerId);
			}
		}

		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override void WriteResult<T>(IEnumerable<T> dataObjects)
		{
			TaskLogger.LogEnter(new object[]
			{
				dataObjects
			});
			if (dataObjects != null)
			{
				SmimeConfigurationContainer smimeConfigurationContainer = null;
				using (IEnumerator<T> enumerator = dataObjects.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						smimeConfigurationContainer = (enumerator.Current as SmimeConfigurationContainer);
					}
					else
					{
						smimeConfigurationContainer = new SmimeConfigurationContainer();
						smimeConfigurationContainer.OrganizationId = base.CurrentOrganizationId;
						smimeConfigurationContainer.SetId(this.RootId as ADObjectId);
					}
				}
				base.WriteResult(smimeConfigurationContainer);
			}
			TaskLogger.LogExit();
		}
	}
}
