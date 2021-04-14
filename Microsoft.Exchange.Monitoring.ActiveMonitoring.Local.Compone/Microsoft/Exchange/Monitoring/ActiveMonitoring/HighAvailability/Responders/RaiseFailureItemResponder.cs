using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Common.HA;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability.Responders
{
	public abstract class RaiseFailureItemResponder : ResponderWorkItem
	{
		protected abstract string GetFailureItemMessage();

		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			RecoveryActionRunner recoveryActionRunner = new RecoveryActionRunner(RecoveryActionId.RaiseFailureItem, Environment.MachineName, this, true, cancellationToken, null);
			recoveryActionRunner.Execute(new Action(this.RaiseFailureItems));
		}

		private void RaiseFailureItems()
		{
			IADDatabase[] allLocalDatabases = CachedAdReader.Instance.AllLocalDatabases;
			IADDatabase targetDatabase = allLocalDatabases.FirstOrDefault((IADDatabase d) => d.Name.Equals(base.Definition.TargetResource, StringComparison.CurrentCultureIgnoreCase));
			if (targetDatabase == null)
			{
				string message = string.Format("Failed to find the database by '{0}' in AD", base.Definition.TargetResource);
				WTFDiagnostics.TraceError(ExTraceGlobals.HighAvailabilityTracer, base.TraceContext, message, null, "RaiseFailureItems", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HighAvailability\\Responders\\RaiseFailureItemResponder.cs", 78);
				throw new RaiseFailureItemResponder.HARaiseFailureItemResponderPrereqFailedException(message);
			}
			string groupOfTargetDatabase = targetDatabase.DatabaseGroup;
			Tuple<string, Guid>[] array;
			if (!string.IsNullOrWhiteSpace(groupOfTargetDatabase))
			{
				IEnumerable<IADDatabase> source = from d in allLocalDatabases
				where d.DatabaseGroup == groupOfTargetDatabase
				select d;
				array = (from d in source
				select new Tuple<string, Guid>(d.Name, d.Guid)).ToArray<Tuple<string, Guid>>();
			}
			else
			{
				KeyValuePair<Guid, CopyStatusClientCachedEntry>[] dbsCopyStatusOnLocalServer = CachedDbStatusReader.Instance.GetDbsCopyStatusOnLocalServer((from d in allLocalDatabases
				select d.Guid).ToArray<Guid>());
				IEnumerable<CopyStatusClientCachedEntry> source2 = from keyvalue in dbsCopyStatusOnLocalServer
				select keyvalue.Value;
				CopyStatusClientCachedEntry copyStatusClientCachedEntry = source2.FirstOrDefault((CopyStatusClientCachedEntry cs) => cs.CopyStatus.DBName.Equals(targetDatabase.Name, StringComparison.CurrentCultureIgnoreCase));
				if (copyStatusClientCachedEntry == null)
				{
					string message2 = string.Format("Failed to find database copy status for database '{0}'", targetDatabase.Name);
					WTFDiagnostics.TraceError(ExTraceGlobals.HighAvailabilityTracer, base.TraceContext, message2, null, "RaiseFailureItems", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HighAvailability\\Responders\\RaiseFailureItemResponder.cs", 105);
					throw new RaiseFailureItemResponder.HARaiseFailureItemResponderPrereqFailedException(message2);
				}
				string volumeNameOfTargetDatabase = copyStatusClientCachedEntry.CopyStatus.DatabaseVolumeName;
				if (volumeNameOfTargetDatabase == null)
				{
					string message3 = string.Format("Failed to find DatabaseVolumeNames for database '{0}'", targetDatabase.Name);
					WTFDiagnostics.TraceError(ExTraceGlobals.HighAvailabilityTracer, base.TraceContext, message3, null, "RaiseFailureItems", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HighAvailability\\Responders\\RaiseFailureItemResponder.cs", 119);
					throw new RaiseFailureItemResponder.HARaiseFailureItemResponderPrereqFailedException(message3);
				}
				IEnumerable<CopyStatusClientCachedEntry> source3 = from cs in source2
				where cs.CopyStatus.DatabaseVolumeName.Equals(volumeNameOfTargetDatabase, StringComparison.OrdinalIgnoreCase)
				select cs;
				array = (from cs in source3
				select new Tuple<string, Guid>(cs.CopyStatus.DBName, cs.CopyStatus.DBGuid)).ToArray<Tuple<string, Guid>>();
			}
			bool flag = true;
			string arg = "";
			foreach (Tuple<string, Guid> tuple in array)
			{
				DatabaseHealthValidationRunner databaseHealthValidationRunner = new DatabaseHealthValidationRunner(AmServerName.LocalComputerName.NetbiosName);
				databaseHealthValidationRunner.Initialize();
				IHealthValidationResultMinimal healthValidationResultMinimal = databaseHealthValidationRunner.RunDatabaseRedundancyChecksForCopyRemoval(false, new Guid?(tuple.Item2), false, false, -1).FirstOrDefault<IHealthValidationResultMinimal>();
				if (healthValidationResultMinimal != null && !healthValidationResultMinimal.IsValidationSuccessful)
				{
					flag = false;
					arg = healthValidationResultMinimal.ErrorMessageWithoutFullStatus;
					WTFDiagnostics.TraceWarning<string, string, string>(ExTraceGlobals.HighAvailabilityTracer, base.TraceContext, "{0} will skip raising failure item for database {1} as it may reduce one viable copy of the database and put the database at data loss risk. Details:{2}", base.Definition.Name, tuple.Item1, arg, null, "RaiseFailureItems", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HighAvailability\\Responders\\RaiseFailureItemResponder.cs", 147);
					break;
				}
			}
			if (!flag)
			{
				throw new RaiseFailureItemResponder.HARaiseFailureItemResponderPrereqFailedException(string.Format("{0} will skip raising failure items for the databases ({1}) as it will put the databases at data loss risk. Reasons:{2}", base.Definition.Name, string.Join(",", from d in array
				select d.Item1), arg));
			}
			foreach (Tuple<string, Guid> tuple2 in array)
			{
				new DatabaseFailureItem(FailureNameSpace.MA, FailureTag.FileSystemCorruption, tuple2.Item2, this.GetFailureItemMessage())
				{
					InstanceName = tuple2.Item2.ToString()
				}.Publish();
			}
		}

		public class HARaiseFailureItemResponderPrereqFailedException : HighAvailabilityMAResponderException
		{
			public HARaiseFailureItemResponderPrereqFailedException(string message) : base(message)
			{
			}
		}
	}
}
