using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Rpc.Cluster;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability.Probes
{
	internal class FailedAndSuspendedCopyProbe : CopyStatusProbeBase
	{
		public static ProbeDefinition CreateDefinition(string name, string serviceName, MailboxDatabaseInfo targetDatabase, int thresholdInSecs, int recurrenceInterval, int timeout, int maxRetry)
		{
			ProbeDefinition probeDefinition = CopyStatusProbeBase.CreateDefinition(name, typeof(FailedAndSuspendedCopyProbe).FullName, serviceName, targetDatabase, recurrenceInterval, timeout, maxRetry);
			probeDefinition.Attributes[FailedAndSuspendedCopyProbe.ThresholdInSecsAttrName] = thresholdInSecs.ToString();
			return probeDefinition;
		}

		public static ProbeDefinition CreateDefinition(string name, string serviceName, MailboxDatabaseInfo targetDatabase, int thresholdInSecs, int recurrenceInterval)
		{
			return FailedAndSuspendedCopyProbe.CreateDefinition(name, serviceName, targetDatabase, thresholdInSecs, recurrenceInterval, recurrenceInterval / 2, 3);
		}

		public override void PopulateDefinition<ProbeDefinition>(ProbeDefinition pDef, Dictionary<string, string> propertyBag)
		{
			base.PopulateDefinition<ProbeDefinition>(pDef, propertyBag);
			if (pDef == null)
			{
				throw new ArgumentException("Please specify a value for probeDefinition");
			}
			if (propertyBag.ContainsKey(FailedAndSuspendedCopyProbe.ThresholdInSecsAttrName))
			{
				pDef.Attributes[FailedAndSuspendedCopyProbe.ThresholdInSecsAttrName] = propertyBag[FailedAndSuspendedCopyProbe.ThresholdInSecsAttrName].ToString().Trim();
				return;
			}
			throw new ArgumentException("Please specify value for" + FailedAndSuspendedCopyProbe.ThresholdInSecsAttrName);
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			base.DoWork(cancellationToken);
			int num;
			if (!base.Definition.Attributes.ContainsKey("ThresholdInSecs") || string.IsNullOrWhiteSpace(base.Definition.Attributes["ThresholdInSecs"]) || !int.TryParse(base.Definition.Attributes["ThresholdInSecs"], out num))
			{
				throw new HighAvailabilityMAProbeException("Probe Attribute 'ThresholdInSecs' is either Empty, Undefined or not in right format. It should be an integer.");
			}
			this.threshold = TimeSpan.FromSeconds((double)num);
			DateTime utcNow = DateTime.UtcNow;
			if ((base.CopyStatus.CopyStatus == CopyStatusEnum.FailedAndSuspended || base.CopyStatus.CopyStatus == CopyStatusEnum.Failed) && utcNow - base.CopyStatus.CopyStatusTimestamp >= this.threshold)
			{
				base.Result.StateAttribute11 = "Failed";
				throw new HighAvailabilityMAProbeRedResultException(string.Format("Database Copy {0} on server {1} is {2} for over {3} mins. Copy in this status since {4} (UTC).", new object[]
				{
					base.CopyStatus.DatabaseName,
					Environment.MachineName,
					base.CopyStatus.CopyStatus,
					this.threshold.TotalMinutes,
					base.CopyStatus.CopyStatusTimestamp.ToString()
				}));
			}
			base.Result.StateAttribute11 = string.Format("Passed. CurrentTime={0}; TimeCompared={1}; Threshold={2} mins", utcNow.ToString(), base.CopyStatus.CopyStatusTimestamp.ToString(), this.threshold.TotalMinutes);
		}

		private TimeSpan threshold;

		public static readonly string ThresholdInSecsAttrName = "ThresholdInSecs";
	}
}
