using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Mapi;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.MailboxSpace.Probes
{
	public class DatabaseSpaceProbe : ProbeWorkItem
	{
		public override void PopulateDefinition<ProbeDefinition>(ProbeDefinition definition, Dictionary<string, string> propertyBag)
		{
			MailboxSpaceDiscovery.PopulateProbeDefinition(definition as ProbeDefinition, propertyBag["TargetResource"], base.GetType().FullName, "DatabaseSpaceProbe", TimeSpan.MaxValue, TimeSpan.FromMinutes(5.0));
			MailboxDatabase mailboxDatabaseFromName = DirectoryAccessor.Instance.GetMailboxDatabaseFromName(propertyBag["TargetResource"]);
			definition.TargetExtension = mailboxDatabaseFromName.Guid.ToString();
		}

		internal override IEnumerable<PropertyInformation> GetSubstitutePropertyInformation()
		{
			return new List<PropertyInformation>
			{
				new PropertyInformation("Identity", Strings.DatabaseSpaceHelpString, true)
			};
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			DateTime utcNow = DateTime.UtcNow;
			string targetExtension = base.Definition.TargetExtension;
			try
			{
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.MailboxSpaceTracer, base.TraceContext, "Starting database space check against database {0}", targetExtension, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\MailboxSpace\\DatabaseSpaceProbe.cs", 80);
				Guid guid = new Guid(targetExtension);
				if (DirectoryAccessor.Instance.IsDatabaseCopyActiveOnLocalServer(guid))
				{
					WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.MailboxSpaceTracer, base.TraceContext, "Running database space check against local database {0}", targetExtension, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\MailboxSpace\\DatabaseSpaceProbe.cs", 92);
					MailboxDatabase mailboxDatabaseFromGuid = DirectoryAccessor.Instance.GetMailboxDatabaseFromGuid(guid);
					if (mailboxDatabaseFromGuid == null)
					{
						throw new InvalidADObjectOperationException(new LocalizedString(Strings.DatabaseObjectNotFoundException));
					}
					base.Result.StateAttribute1 = mailboxDatabaseFromGuid.Name;
					this.GetDatabaseProperties(mailboxDatabaseFromGuid);
					this.PopulateDatabaseDiskDetails(mailboxDatabaseFromGuid.EdbFilePath.PathName);
					this.PopulateDatabaseSize(guid);
					WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.MailboxSpaceTracer, base.TraceContext, "Completed database space check against local database {0}", targetExtension, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\MailboxSpace\\DatabaseSpaceProbe.cs", 119);
				}
				else
				{
					WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.MailboxSpaceTracer, base.TraceContext, "Skipping database space check against database {0} as it is not mounted locally", targetExtension, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\MailboxSpace\\DatabaseSpaceProbe.cs", 127);
				}
			}
			finally
			{
				base.Result.SampleValue = (double)((int)(DateTime.UtcNow - utcNow).TotalMilliseconds);
			}
		}

		private void GetDatabaseProperties(MailboxDatabase mailboxDatabase)
		{
			base.Result.StateAttribute2 = mailboxDatabase.IsExcludedFromProvisioning.ToString();
			base.Result.StateAttribute3 = mailboxDatabase.IsExcludedFromInitialProvisioning.ToString();
			base.Result.StateAttribute4 = mailboxDatabase.IsExcludedFromProvisioningBySpaceMonitoring.ToString();
		}

		private void PopulateDatabaseDiskDetails(string edbFilePath)
		{
			edbFilePath = edbFilePath.Substring(0, edbFilePath.LastIndexOf('\\'));
			base.Result.StateAttribute5 = edbFilePath;
			ulong num;
			ulong num2;
			Exception freeSpace = DiskHelper.GetFreeSpace(edbFilePath, out num, out num2);
			if (freeSpace != null)
			{
				WTFDiagnostics.TraceError<string, Exception>(ExTraceGlobals.MailboxSpaceTracer, base.TraceContext, "Unable to get disk space for edb path {0}. Exception: {1}", edbFilePath, freeSpace, null, "PopulateDatabaseDiskDetails", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\MailboxSpace\\DatabaseSpaceProbe.cs", 173);
				throw freeSpace;
			}
			base.Result.StateAttribute8 = num;
			base.Result.StateAttribute9 = num2;
		}

		private void PopulateDatabaseSize(Guid databaseGuid)
		{
			using (ExRpcAdmin exRpcAdmin = ExRpcAdmin.Create("Client=StoreActiveMonitoring", Environment.MachineName, null, null, null))
			{
				ulong bytesValue;
				ulong bytesValue2;
				exRpcAdmin.GetDatabaseSize(databaseGuid, out bytesValue, out bytesValue2);
				base.Result.StateAttribute6 = (double)((long)((double)ByteQuantifiedSize.FromBytes(bytesValue)));
				base.Result.StateAttribute7 = (double)((long)((double)ByteQuantifiedSize.FromBytes(bytesValue2)));
			}
		}
	}
}
