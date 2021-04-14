using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.MessageSecurity.EdgeSync;

namespace Microsoft.Exchange.EdgeSync.Validation
{
	internal class CredentialRecordsLoader
	{
		public static MultiValuedProperty<CredentialRecord> Load(Server edgeServer)
		{
			ITopologyConfigurationSession configurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 103, "Load", "f:\\15.00.1497\\sources\\dev\\EdgeSync\\src\\Common\\Validation\\CredentialRecord.cs");
			MultiValuedProperty<CredentialRecord> credentialRecords = new MultiValuedProperty<CredentialRecord>();
			ADNotificationAdapter.TryReadConfigurationPaged<Server>(() => configurationSession.FindAllServersWithVersionNumber(Server.E2007MinVersion), delegate(Server server)
			{
				if (server.IsHubTransportServer && server.EdgeSyncCredentials != null && server.EdgeSyncCredentials.Count != 0)
				{
					foreach (byte[] data in server.EdgeSyncCredentials)
					{
						EdgeSyncCredential edgeSyncCredential = EdgeSyncCredential.DeserializeEdgeSyncCredential(data);
						if (edgeSyncCredential.EdgeServerFQDN.Equals(edgeServer.Fqdn, StringComparison.OrdinalIgnoreCase))
						{
							CredentialRecord credentialRecord = new CredentialRecord();
							credentialRecord.TargetEdgeServerFQDN = edgeSyncCredential.EdgeServerFQDN;
							credentialRecord.ESRAUsername = edgeSyncCredential.ESRAUsername;
							credentialRecord.EffectiveDate = new DateTime(edgeSyncCredential.EffectiveDate).ToLocalTime();
							credentialRecord.Duration = new TimeSpan(edgeSyncCredential.Duration);
							credentialRecord.IsBootStrapAccount = edgeSyncCredential.IsBootStrapAccount;
							credentialRecords.Add(credentialRecord);
						}
					}
				}
			});
			return credentialRecords;
		}
	}
}
