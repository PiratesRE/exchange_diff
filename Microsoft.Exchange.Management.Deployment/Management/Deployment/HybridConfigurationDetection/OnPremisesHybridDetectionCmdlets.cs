using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Deployment.HybridConfigurationDetection
{
	internal class OnPremisesHybridDetectionCmdlets : IOnPremisesHybridDetectionCmdlets
	{
		public OnPremisesHybridDetectionCmdlets()
		{
			this.monadProvider = new MonadProvider();
		}

		public IEnumerable<AcceptedDomain> GetAcceptedDomain()
		{
			IEnumerable<AcceptedDomain> result;
			try
			{
				object[] array = this.monadProvider.ExecuteCommand("Get-AcceptedDomain");
				if (array != null && array.Length > 0)
				{
					AcceptedDomain[] array2 = new AcceptedDomain[array.Length];
					for (int i = 0; i < array.Length; i++)
					{
						array2[i] = (AcceptedDomain)array[i];
					}
					result = array2;
				}
				else
				{
					result = null;
				}
			}
			catch
			{
				result = null;
			}
			return result;
		}

		public IEnumerable<OrganizationRelationship> GetOrganizationRelationship()
		{
			IEnumerable<OrganizationRelationship> result;
			try
			{
				object[] array = this.monadProvider.ExecuteCommand("Get-OrganizationRelationship");
				if (array != null && array.Length > 0)
				{
					OrganizationRelationship[] array2 = new OrganizationRelationship[array.Length];
					for (int i = 0; i < array.Length; i++)
					{
						array2[i] = (OrganizationRelationship)array[i];
					}
					result = array2;
				}
				else
				{
					result = null;
				}
			}
			catch
			{
				result = null;
			}
			return result;
		}

		private const string GetAcceptedDomainCmdlet = "Get-AcceptedDomain";

		private const string GetOrganizationRelationshipCmdlet = "Get-OrganizationRelationship";

		private MonadProvider monadProvider;
	}
}
