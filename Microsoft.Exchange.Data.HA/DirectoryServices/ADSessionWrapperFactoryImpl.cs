using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.HA.DirectoryServices
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ADSessionWrapperFactoryImpl : IADSessionFactory
	{
		public IADToplogyConfigurationSession CreateIgnoreInvalidRootOrgSession(bool readOnly)
		{
			return this.CreateSession(readOnly, ConsistencyMode.IgnoreInvalid);
		}

		public IADToplogyConfigurationSession CreatePartiallyConsistentRootOrgSession(bool readOnly)
		{
			return this.CreateSession(readOnly, ConsistencyMode.PartiallyConsistent);
		}

		public IADToplogyConfigurationSession CreateFullyConsistentRootOrgSession(bool readOnly)
		{
			return this.CreateSession(readOnly, ConsistencyMode.FullyConsistent);
		}

		public IADRootOrganizationRecipientSession CreateIgnoreInvalidRootOrgRecipientSession()
		{
			IRootOrganizationRecipientSession session = DirectorySessionFactory.Default.CreateRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 226, "CreateIgnoreInvalidRootOrgRecipientSession", "f:\\15.00.1497\\sources\\dev\\data\\src\\HA\\DirectoryServices\\ADSessionFactory.cs");
			return ADRootOrganizationRecipientSessionWrapper.CreateWrapper(session);
		}

		private IADToplogyConfigurationSession CreateSession(bool readOnly, ConsistencyMode consistencyMode)
		{
			ITopologyConfigurationSession session = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(readOnly, consistencyMode, ADSessionSettings.FromRootOrgScopeSet(), 239, "CreateSession", "f:\\15.00.1497\\sources\\dev\\data\\src\\HA\\DirectoryServices\\ADSessionFactory.cs");
			return ADSessionFactory.CreateWrapper(session);
		}
	}
}
