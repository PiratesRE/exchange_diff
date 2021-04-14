using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Hygiene.Deployment.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.TransportDatabase
{
	public class TransportOldQueueDBCleanupProbe : ProbeWorkItem
	{
		public override void PopulateDefinition<ProbeDefinition>(ProbeDefinition pDef, Dictionary<string, string> propertyBag)
		{
			if (pDef == null)
			{
				throw new ArgumentException("Please specify a value for probeDefinition");
			}
			if (propertyBag.ContainsKey("OldMailDatabaseRetentionPeriod"))
			{
				pDef.Attributes["OldMailDatabaseRetentionPeriod"] = propertyBag["OldMailDatabaseRetentionPeriod"].Trim();
				return;
			}
			pDef.Attributes["OldMailDatabaseRetentionPeriod"] = "14.00:00:00";
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			base.Result.ExecutionContext = "TransportOldQueueDatabaseCleanupProbe started.";
			string text;
			if (!TransportOldQueueDBCleanupHelper.TryGetQueueDatabaseFolderPath(out text))
			{
				base.Result.FailureContext = string.Format("PROBE -- Invalid path for queue database. Path: {0}", text);
				return;
			}
			ProbeResult result = base.Result;
			result.ExecutionContext += "Valid path for queue database found.";
			TimeSpan oldDatabaseRetentionPeriod;
			if (!TimeSpan.TryParse(ProbeHelper.GetExtensionAttribute(new NullHygieneLogger(), this, "OldMailDatabaseRetentionPeriod"), out oldDatabaseRetentionPeriod))
			{
				base.Result.FailureContext = "PROBE -- Invalid old Database retention period.";
				return;
			}
			ProbeResult result2 = base.Result;
			result2.ExecutionContext += "Valid value for Database retention period found.";
			List<DirectoryInfo> expiredOldQueueDatabases = TransportOldQueueDBCleanupHelper.GetExpiredOldQueueDatabases(text, oldDatabaseRetentionPeriod);
			if (expiredOldQueueDatabases.Count < 1)
			{
				base.Result.FailureContext = "PROBE -- No mail.que database found which was older than retention period";
				return;
			}
			ProbeResult result3 = base.Result;
			result3.ExecutionContext += "Expired databases FOUND!, their names will be added to StateAttribute1.";
			base.Result.StateAttribute1 = string.Join(",", (from database in expiredOldQueueDatabases
			select database.Name).ToArray<string>());
			throw new Exception("Expired queue database found. Failing Probe.");
		}
	}
}
