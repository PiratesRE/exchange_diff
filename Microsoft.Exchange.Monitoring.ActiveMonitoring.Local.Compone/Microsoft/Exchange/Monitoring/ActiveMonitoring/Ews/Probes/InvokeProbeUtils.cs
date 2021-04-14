using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Ews.Probes
{
	internal class InvokeProbeUtils
	{
		internal static void PopulateDefinition(ProbeDefinition probe, Dictionary<string, string> propertyBag, string assemblyPath, string typeName, string endpoint, bool isMbxProbe)
		{
			ICollection<MailboxDatabaseInfo> collection;
			ProbeAuthNType probeAuthNType;
			int num;
			if (isMbxProbe)
			{
				collection = LocalEndpointManager.Instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend;
				probeAuthNType = ProbeAuthNType.Cafe;
				num = 444;
			}
			else
			{
				collection = LocalEndpointManager.Instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForCafe;
				probeAuthNType = ProbeAuthNType.LiveIdOrNegotiate;
				num = 443;
			}
			if (collection == null || collection.Count == 0)
			{
				throw new InvalidOperationException("Mailbox collection is empty");
			}
			probe.Attributes["PrimaryAuthN"] = probeAuthNType.ToString();
			probe.Attributes["TrustAnySslCertificate"] = true.ToString();
			probe.Attributes["Verbose"] = true.ToString();
			probe.Attributes["IsOutsideInMonitoring"] = false.ToString();
			probe.Name = propertyBag["Name"];
			probe.ServiceName = propertyBag["ServiceName"];
			probe.TargetResource = propertyBag["TargetResource"];
			probe.RecurrenceIntervalSeconds = 0;
			probe.TimeoutSeconds = 60;
			probe.MaxRetryAttempts = 0;
			MailboxDatabaseInfo mailboxDatabaseInfo = collection.FirstOrDefault((MailboxDatabaseInfo db) => !string.IsNullOrWhiteSpace(db.MonitoringAccountPassword));
			probe.Account = mailboxDatabaseInfo.MonitoringAccount + "@" + mailboxDatabaseInfo.MonitoringAccountDomain;
			probe.AccountPassword = mailboxDatabaseInfo.MonitoringAccountPassword;
			probe.AccountDisplayName = mailboxDatabaseInfo.MonitoringAccount;
			probe.Attributes["TargetPort"] = num.ToString();
			probe.AssemblyPath = assemblyPath;
			probe.TypeName = typeName;
			probe.Endpoint = endpoint;
			if (propertyBag.ContainsKey("Account"))
			{
				probe.Account = propertyBag["Account"];
				probe.AccountDisplayName = probe.Account;
			}
			if (propertyBag.ContainsKey("Password"))
			{
				probe.AccountPassword = propertyBag["Password"];
			}
			if (propertyBag.ContainsKey("Endpoint"))
			{
				probe.Endpoint = propertyBag["Endpoint"];
			}
			int timeoutSeconds;
			if (propertyBag.ContainsKey("TimeoutSeconds") && int.TryParse(propertyBag["TimeoutSeconds"], out timeoutSeconds))
			{
				probe.TimeoutSeconds = timeoutSeconds;
			}
		}
	}
}
