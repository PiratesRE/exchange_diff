using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Threading;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Directory.Probes
{
	public class RemoteDomainControllerStateProbe : ProbeWorkItem
	{
		public override void PopulateDefinition<ProbeDefinition>(ProbeDefinition pDef, Dictionary<string, string> propertyBag)
		{
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			DirectoryUtils.Logger(this, StxLogType.RemoteDomainControllerStateProbe, delegate
			{
				if (!DirectoryUtils.IsRidMaster())
				{
					this.Result.StateAttribute5 = "This DC is not a RID master.  Probe will be skipped.";
					return;
				}
				this.CheckRemoteDomainControllersState(cancellationToken);
			});
		}

		private void CheckRemoteDomainControllersState(CancellationToken cancellationToken)
		{
			string empty = string.Empty;
			string localDomain = DirectoryGeneralUtils.GetLocalDomain();
			string item = string.Empty;
			string text = string.Empty;
			List<string> list = new List<string>();
			using (DirectoryEntry directoryEntry = new DirectoryEntry())
			{
				using (DirectoryEntry directoryEntry2 = new DirectoryEntry("LDAP://localhost/OU=Domain Controllers," + directoryEntry.Properties["distinguishedName"].Value.ToString()))
				{
					foreach (object obj in directoryEntry2.Children)
					{
						DirectoryEntry directoryEntry3 = (DirectoryEntry)obj;
						text = directoryEntry3.Properties["cn"].Value.ToString();
						if (!(text.ToUpper() == Environment.MachineName.ToUpper()))
						{
							int num = 0;
							if (directoryEntry3.Properties.Contains("msExchProvisioningFlags"))
							{
								num = (int)directoryEntry3.Properties["msExchProvisioningFlags"].Value;
							}
							if (num == 0 && !DirectoryUtils.VerifyDomainControllerLDAPRead(text, directoryEntry.Properties["distinguishedName"].Value.ToString()))
							{
								item = string.Format("{0}.{1}", text, localDomain);
								list.Add(item);
							}
						}
					}
					if (list.Count > 0)
					{
						base.Result.StateAttribute1 = DirectoryUtils.ListToString(list);
						List<string> dcsForRecovery = this.GetDCsForRecovery(cancellationToken, list);
						if (dcsForRecovery.Count > 0)
						{
							base.Result.StateAttribute2 = DirectoryUtils.ListToString(dcsForRecovery);
							throw new Exception(string.Format("Following DCs have msExchProvisioningFlag = 0, but LDAP Read from these DCs is failing:   {0}", base.Result.StateAttribute2));
						}
					}
					else
					{
						base.Result.StateAttribute3 = "All provisioned Domain Controllers are healthy.";
					}
				}
			}
		}

		private List<string> GetDCsForRecovery(CancellationToken cancellationToken, List<string> downDCList)
		{
			List<string> list = new List<string>();
			DateTime monitoringWindowStartTime = base.Result.ExecutionStartTime.AddSeconds((double)(-1 * base.Definition.RecurrenceIntervalSeconds * 4));
			List<ProbeResult> allProbeResults = WorkItemResultHelper.GetAllProbeResults(base.Broker, base.Result.ExecutionStartTime, base.Definition.Name, monitoringWindowStartTime, cancellationToken);
			if (allProbeResults.Count >= 3)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>();
				string text = string.Empty;
				using (List<string>.Enumerator enumerator = downDCList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string text2 = enumerator.Current;
						dictionary.Add(text2, 0);
						for (int i = 0; i < 3; i++)
						{
							text = allProbeResults[i].StateAttribute1;
							if (!string.IsNullOrEmpty(text) && text.Contains(text2))
							{
								dictionary[text2]++;
							}
						}
						if (dictionary[text2] == 3)
						{
							list.Add(text2);
						}
					}
					return list;
				}
			}
			base.Result.StateAttribute3 = string.Format("Not enough probe results to work with. Expected atleast 3, but found {0} probe results.", allProbeResults.Count);
			return list;
		}
	}
}
