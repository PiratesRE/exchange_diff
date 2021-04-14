using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Assistants;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.TimeBasedAssistants
{
	internal class TimeBasedAssistantsActiveDatabaseCriteria
	{
		public List<KeyValuePair<string, Guid[]>> FindOutOfCriteria(Dictionary<AssistantInfo, Dictionary<MailboxDatabase, WindowJob[]>> fullDiagnostics, bool isTest = false)
		{
			ArgumentValidator.ThrowIfNull("fullDiagnostics", fullDiagnostics);
			HashSet<Guid> hashSet = new HashSet<Guid>();
			if (isTest)
			{
				hashSet.Add(TimeBasedAssistantsActiveDatabaseCriteria.testDatabaseGuid);
			}
			else
			{
				KeyValuePair<Guid, CopyStatusClientCachedEntry>[] dbsCopyStatusOnLocalServer = CachedDbStatusReader.Instance.GetDbsCopyStatusOnLocalServer(new Guid[0]);
				foreach (KeyValuePair<Guid, CopyStatusClientCachedEntry> keyValuePair in from dbStatus in dbsCopyStatusOnLocalServer
				where dbStatus.Value.IsActive
				select dbStatus)
				{
					hashSet.Add(keyValuePair.Key);
				}
			}
			List<KeyValuePair<string, Guid[]>> list = new List<KeyValuePair<string, Guid[]>>();
			foreach (KeyValuePair<AssistantInfo, Dictionary<MailboxDatabase, WindowJob[]>> keyValuePair2 in fullDiagnostics)
			{
				AssistantInfo key = keyValuePair2.Key;
				IEnumerable<MailboxDatabase> keys = keyValuePair2.Value.Keys;
				VariantConfigurationSnapshot snapshot = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null);
				bool activeDatabaseProcessingMonitoringEnabled = snapshot.MailboxAssistants.GetObject<IMailboxAssistantSettings>(key.AssistantName).ActiveDatabaseProcessingMonitoringEnabled;
				if (activeDatabaseProcessingMonitoringEnabled)
				{
					HashSet<Guid> hashSet2 = new HashSet<Guid>();
					foreach (MailboxDatabase mailboxDatabase in from database in keys
					where database.IsAssistantEnabled
					select database)
					{
						hashSet2.Add(mailboxDatabase.Guid);
					}
					if (!hashSet2.SetEquals(hashSet) && hashSet.Count >= hashSet2.Count)
					{
						IEnumerable<Guid> enumerable = hashSet.Except(hashSet2);
						List<Guid> list2 = new List<Guid>();
						foreach (Guid guid in enumerable)
						{
							VariantConfigurationSnapshot snapshot2 = VariantConfiguration.GetSnapshot(DatabaseSettingsContext.Get(guid), null, null);
							bool enabled = snapshot2.MailboxAssistants.GetObject<IMailboxAssistantSettings>(key.AssistantName).Enabled;
							if (enabled)
							{
								list2.Add(guid);
							}
						}
						if (list2.Any<Guid>())
						{
							list.Add(new KeyValuePair<string, Guid[]>(key.AssistantName, list2.ToArray()));
						}
					}
				}
			}
			return list;
		}

		private static readonly Guid testDatabaseGuid = Guid.Parse("C3D078DE-9D3D-4FFF-831F-E8DB97C26EBB");
	}
}
