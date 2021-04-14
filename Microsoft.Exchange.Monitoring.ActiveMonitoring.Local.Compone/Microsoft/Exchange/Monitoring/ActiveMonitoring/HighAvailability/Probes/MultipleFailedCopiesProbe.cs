using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Rpc.Cluster;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability.Probes
{
	internal class MultipleFailedCopiesProbe : ProbeWorkItem
	{
		public static ProbeDefinition CreateDefinition(string name, string serviceName, double failurePercentage, int recurrenceInterval, int timeout, int maxRetry)
		{
			ProbeDefinition probeDefinition = new ProbeDefinition();
			probeDefinition.AssemblyPath = Assembly.GetExecutingAssembly().Location;
			probeDefinition.ServiceName = serviceName;
			probeDefinition.TypeName = typeof(MultipleFailedCopiesProbe).FullName;
			probeDefinition.Name = name;
			probeDefinition.RecurrenceIntervalSeconds = recurrenceInterval;
			probeDefinition.TimeoutSeconds = timeout;
			probeDefinition.MaxRetryAttempts = maxRetry;
			probeDefinition.TargetResource = Environment.MachineName;
			probeDefinition.Attributes[MultipleFailedCopiesProbe.FailurePercentageAttrName] = failurePercentage.ToString();
			return probeDefinition;
		}

		public static ProbeDefinition CreateDefinition(string name, double failurePercentage, int recurrenceInterval)
		{
			return MultipleFailedCopiesProbe.CreateDefinition(name, HighAvailabilityConstants.ServiceName, failurePercentage, recurrenceInterval, recurrenceInterval / 2, 3);
		}

		public override void PopulateDefinition<ProbeDefinition>(ProbeDefinition pDef, Dictionary<string, string> propertyBag)
		{
			if (pDef == null)
			{
				throw new ArgumentException("Please specify a value for probeDefinition");
			}
			if (propertyBag.ContainsKey(MultipleFailedCopiesProbe.FailurePercentageAttrName))
			{
				pDef.Attributes[MultipleFailedCopiesProbe.FailurePercentageAttrName] = propertyBag[MultipleFailedCopiesProbe.FailurePercentageAttrName].ToString().Trim();
				return;
			}
			throw new ArgumentException("Please specify value for" + MultipleFailedCopiesProbe.FailurePercentageAttrName);
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			if (HighAvailabilityUtility.CheckCancellationRequested(cancellationToken))
			{
				base.Result.StateAttribute1 = "Cancellation Requested!";
				return;
			}
			double num = double.Parse(base.Definition.Attributes["FailurePercentage"]);
			base.Result.StateAttribute1 = base.Definition.Name;
			base.Result.StateAttribute2 = Environment.MachineName;
			base.Result.StateAttribute3 = string.Format("Failure Threshold: {0}", num);
			IADDatabase[] allLocalDatabases = CachedAdReader.Instance.AllLocalDatabases;
			KeyValuePair<Guid, CopyStatusClientCachedEntry>[] array = null;
			if (allLocalDatabases != null && allLocalDatabases.Length > 0)
			{
				array = CachedDbStatusReader.Instance.GetDbsCopyStatusOnLocalServer((from o in allLocalDatabases
				select o.Guid).ToArray<Guid>());
			}
			if (allLocalDatabases != null && allLocalDatabases.Length > 0 && array != null && array.Length > 0)
			{
				List<CopyStatusClientCachedEntry> list = new List<CopyStatusClientCachedEntry>();
				List<CopyStatusClientCachedEntry> list2 = new List<CopyStatusClientCachedEntry>();
				KeyValuePair<Guid, CopyStatusClientCachedEntry>[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					KeyValuePair<Guid, CopyStatusClientCachedEntry> keyValuePair = array2[i];
					CopyStatusClientCachedEntry entry = keyValuePair.Value;
					if (!entry.IsActive && entry.Result == CopyStatusRpcResult.Success)
					{
						if (MultipleFailedCopiesProbe.isStorageClassError.Any((Predicate<CopyStatusClientCachedEntry> pred) => pred(entry)) && entry.CopyStatus.CopyStatus == CopyStatusEnum.FailedAndSuspended)
						{
							list.Add(entry);
						}
						list2.Add(entry);
					}
				}
				double num2;
				if (list2.Count < 1 && list.Count > 0)
				{
					num2 = 1.0;
				}
				else if (list2.Count < 0)
				{
					num2 = 0.0;
				}
				else
				{
					num2 = (double)list.Count / (double)list2.Count;
				}
				base.Result.StateAttribute4 = string.Format("Total passive copies = {0}, StorageClass failed copies = {1}, failurePercentage = {2}", list2.Count, list.Count, num2);
				base.Result.StateAttribute5 = string.Join("; ", from o in list
				select string.Format("{0}\\{1} - {2} - CQ:{3} - RQ:{4} -ErrEvtId: {5}", new object[]
				{
					o.CopyStatus.DBName,
					Environment.MachineName,
					o.CopyStatus.CopyStatus,
					o.CopyStatus.GetCopyQueueLength(),
					o.CopyStatus.GetReplayQueueLength(),
					o.CopyStatus.ErrorEventId
				}));
				base.Result.StateAttribute11 = "Scanned Copies: " + string.Join("; ", from o in list2
				select string.Format("{0}\\{1} [{2}]", o.CopyStatus.DBName, Environment.MachineName, o.CopyStatus.CopyStatus));
				if (num2 >= num)
				{
					throw new HighAvailabilityMAProbeRedResultException(string.Format("DB Copy Failure Rate = {0} on Server {1} with Total DB copies {2} and Failed DB Copies {3}. Include Actives. Threshold={4}", new object[]
					{
						num2,
						Environment.MachineName,
						array.Length,
						list.Count,
						num
					}));
				}
			}
			else
			{
				base.Result.StateAttribute4 = "No database copies on local server.";
			}
		}

		// Note: this type is marked as 'beforefieldinit'.
		static MultipleFailedCopiesProbe()
		{
			Predicate<CopyStatusClientCachedEntry>[] array = new Predicate<CopyStatusClientCachedEntry>[3];
			array[0] = ((CopyStatusClientCachedEntry entry) => entry.CopyStatus.ErrorEventId == 2073U);
			array[1] = ((CopyStatusClientCachedEntry entry) => entry.CopyStatus.ErrorEventId == 102U);
			array[2] = ((CopyStatusClientCachedEntry entry) => entry.CopyStatus.ErrorEventId == 4117U && entry.CopyStatus.ErrorMessage.Contains("-1021"));
			MultipleFailedCopiesProbe.isStorageClassError = array;
		}

		public static readonly string FailurePercentageAttrName = "FailurePercentage";

		private static Predicate<CopyStatusClientCachedEntry>[] isStorageClassError;
	}
}
