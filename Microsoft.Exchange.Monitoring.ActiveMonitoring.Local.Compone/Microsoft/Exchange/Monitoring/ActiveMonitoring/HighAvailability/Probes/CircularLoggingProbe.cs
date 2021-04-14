using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability.Probes
{
	public class CircularLoggingProbe : ProbeWorkItem
	{
		public static ProbeDefinition CreateDefinition(string name, string serviceName, MailboxDatabaseInfo targetDatabase, int recurrenceInterval, int timeout, int maxRetry)
		{
			ProbeDefinition probeDefinition = new ProbeDefinition();
			probeDefinition.AssemblyPath = Assembly.GetExecutingAssembly().Location;
			probeDefinition.ServiceName = serviceName;
			probeDefinition.TypeName = typeof(CircularLoggingProbe).FullName;
			probeDefinition.Name = name;
			probeDefinition.RecurrenceIntervalSeconds = recurrenceInterval;
			probeDefinition.TimeoutSeconds = timeout;
			probeDefinition.MaxRetryAttempts = maxRetry;
			probeDefinition.TargetResource = targetDatabase.MailboxDatabaseName;
			probeDefinition.Attributes[CircularLoggingProbe.DBGuidAttrName] = targetDatabase.MailboxDatabaseGuid.ToString();
			return probeDefinition;
		}

		public static ProbeDefinition CreateDefinition(string name, string serviceName, MailboxDatabaseInfo targetDatabase, int recurrenceInterval)
		{
			return CircularLoggingProbe.CreateDefinition(name, serviceName, targetDatabase, recurrenceInterval, recurrenceInterval / 2, 3);
		}

		public override void PopulateDefinition<ProbeDefinition>(ProbeDefinition pDef, Dictionary<string, string> propertyBag)
		{
			if (pDef == null)
			{
				throw new ArgumentException("Please specify a value for probeDefinition");
			}
			if (propertyBag.ContainsKey(CircularLoggingProbe.DBGuidAttrName))
			{
				pDef.Attributes[CircularLoggingProbe.DBGuidAttrName] = propertyBag[CircularLoggingProbe.DBGuidAttrName].ToString().Trim();
				return;
			}
			throw new ArgumentException("Please specify value for" + CircularLoggingProbe.DBGuidAttrName);
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			if (HighAvailabilityUtility.CheckCancellationRequested(cancellationToken))
			{
				base.Result.StateAttribute1 = "Cancellation Requested!";
				return;
			}
			this.databaseGuid = new Guid(base.Definition.Attributes["dbGuid"]);
			IADDatabase databaseOnLocalServer = CachedAdReader.Instance.GetDatabaseOnLocalServer(this.databaseGuid);
			base.Result.StateAttribute1 = base.Definition.Name;
			base.Result.StateAttribute2 = string.Format("{0}", string.IsNullOrEmpty(base.Definition.TargetResource) ? "NULL" : base.Definition.TargetResource);
			if (databaseOnLocalServer == null)
			{
				throw new HighAvailabilityMAProbeException(string.Format("Unable to find Database AdObject for database {0} (Guid={1})!", string.IsNullOrEmpty(base.Definition.TargetResource) ? "NULL" : base.Definition.TargetResource, string.IsNullOrEmpty(base.Definition.Attributes["dbGuid"]) ? "NULL" : base.Definition.Attributes["dbGuid"]));
			}
			base.Result.StateAttribute3 = string.Format("CircularLogging Enabled - {0}", databaseOnLocalServer.CircularLoggingEnabled);
			if (!databaseOnLocalServer.CircularLoggingEnabled)
			{
				base.Result.StateAttribute11 = "Failed";
				throw new HighAvailabilityMAProbeRedResultException(string.Format("Database {0} has CircularLogging {1}", base.Definition.TargetResource, databaseOnLocalServer.CircularLoggingEnabled ? "Enabled" : "Disabled"));
			}
			base.Result.StateAttribute11 = "Passed";
		}

		private Guid databaseGuid;

		public static readonly string DBGuidAttrName = "dbGuid";
	}
}
