using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.HA.DirectoryServices
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ADSessionFactory
	{
		private static IADSessionFactory Default
		{
			get
			{
				if (ADSessionFactory.defaultInstance == null)
				{
					ADSessionFactory.defaultInstance = new ADSessionWrapperFactoryImpl();
				}
				return ADSessionFactory.defaultInstance;
			}
		}

		internal static void SetTestADSessionFactory(IADSessionFactory testADSessionFactory)
		{
			ADSessionFactory.defaultInstance = testADSessionFactory;
		}

		public static IADToplogyConfigurationSession CreateIgnoreInvalidRootOrgSession(bool readOnly = true)
		{
			return ADSessionFactory.Default.CreateIgnoreInvalidRootOrgSession(readOnly);
		}

		public static IADToplogyConfigurationSession CreatePartiallyConsistentRootOrgSession(bool readOnly = true)
		{
			return ADSessionFactory.Default.CreatePartiallyConsistentRootOrgSession(readOnly);
		}

		public static IADToplogyConfigurationSession CreateFullyConsistentRootOrgSession(bool readOnly = true)
		{
			return ADSessionFactory.Default.CreateFullyConsistentRootOrgSession(readOnly);
		}

		public static IADRootOrganizationRecipientSession CreateIgnoreInvalidRootOrgRecipientSession()
		{
			return ADSessionFactory.Default.CreateIgnoreInvalidRootOrgRecipientSession();
		}

		public static IADToplogyConfigurationSession CreateWrapper(ITopologyConfigurationSession session)
		{
			return ADTopologyConfigurationSessionWrapper.CreateWrapper(session);
		}

		private static IADSessionFactory defaultInstance;
	}
}
