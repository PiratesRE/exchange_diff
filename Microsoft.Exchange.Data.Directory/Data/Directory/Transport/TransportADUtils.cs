using System;
using System.DirectoryServices;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Transport
{
	internal static class TransportADUtils
	{
		public static ExchangeConfigurationContainer GetForestInformation(out Guid forestGuid, out string forestName)
		{
			forestGuid = Guid.Empty;
			forestName = null;
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(true, ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 40, "GetForestInformation", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\Transport\\TransportADUtils.cs");
			ExchangeConfigurationContainer exchangeConfigurationContainer = topologyConfigurationSession.GetExchangeConfigurationContainer();
			if (exchangeConfigurationContainer.Id != null && exchangeConfigurationContainer.Id.Parent != null && exchangeConfigurationContainer.Id.Parent.Parent != null)
			{
				forestName = exchangeConfigurationContainer.Id.Parent.Parent.DistinguishedName;
			}
			forestGuid = exchangeConfigurationContainer.Guid;
			return exchangeConfigurationContainer;
		}

		public static ActiveDirectorySecurity SetupActiveDirectorySecurity(RawSecurityDescriptor rawSecurityDescriptor)
		{
			ActiveDirectorySecurity activeDirectorySecurity = new ActiveDirectorySecurity();
			byte[] array = new byte[rawSecurityDescriptor.BinaryLength];
			rawSecurityDescriptor.GetBinaryForm(array, 0);
			activeDirectorySecurity.SetSecurityDescriptorBinaryForm(array);
			SecurityIdentifier identity = new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null);
			ActiveDirectoryAccessRule rule = new ActiveDirectoryAccessRule(identity, ActiveDirectoryRights.ReadControl | ActiveDirectoryRights.ListChildren | ActiveDirectoryRights.Self | ActiveDirectoryRights.ReadProperty | ActiveDirectoryRights.WriteProperty | ActiveDirectoryRights.ListObject, AccessControlType.Allow);
			activeDirectorySecurity.AddAccessRule(rule);
			SecurityIdentifier identity2 = new SecurityIdentifier(WellKnownSidType.NetworkServiceSid, null);
			rule = new ActiveDirectoryAccessRule(identity2, ActiveDirectoryRights.ReadControl | ActiveDirectoryRights.ListChildren | ActiveDirectoryRights.Self | ActiveDirectoryRights.ReadProperty | ActiveDirectoryRights.WriteProperty | ActiveDirectoryRights.ListObject, AccessControlType.Allow);
			activeDirectorySecurity.AddAccessRule(rule);
			return activeDirectorySecurity;
		}
	}
}
