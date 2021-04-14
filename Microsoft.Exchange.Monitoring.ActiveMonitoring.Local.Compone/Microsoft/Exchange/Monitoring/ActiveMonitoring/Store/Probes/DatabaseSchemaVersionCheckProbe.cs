using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Mapi;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Store.Probes
{
	public class DatabaseSchemaVersionCheckProbe : ProbeWorkItem
	{
		public override void PopulateDefinition<ProbeDefinition>(ProbeDefinition definition, Dictionary<string, string> propertyBag)
		{
			StoreDiscovery.PopulateProbeDefinition(definition as ProbeDefinition, propertyBag["TargetResource"], base.GetType(), "DatabaseSchemaVersionCheckProbe", TimeSpan.MaxValue, TimeSpan.FromMinutes(2.0));
			MailboxDatabase mailboxDatabaseFromName = DirectoryAccessor.Instance.GetMailboxDatabaseFromName(propertyBag["TargetResource"]);
			definition.TargetExtension = mailboxDatabaseFromName.Guid.ToString();
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			DateTime utcNow = DateTime.UtcNow;
			string targetExtension = base.Definition.TargetExtension;
			try
			{
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.StoreTracer, base.TraceContext, "Starting database schema version check probe against database {0}", targetExtension, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Store\\DatabaseSchemaVersionCheckProbe.cs", 61);
				Guid guid = new Guid(targetExtension);
				if (DirectoryAccessor.Instance.IsDatabaseCopyActiveOnLocalServer(guid))
				{
					WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.StoreTracer, base.TraceContext, "Running database schema version check probe against local database {0}", targetExtension, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Store\\DatabaseSchemaVersionCheckProbe.cs", 73);
					base.Result.StateAttribute1 = base.Definition.TargetResource;
					MailboxDatabase mailboxDatabaseFromGuid = DirectoryAccessor.Instance.GetMailboxDatabaseFromGuid(guid);
					if (mailboxDatabaseFromGuid == null)
					{
						throw new InvalidADObjectOperationException(new LocalizedString(Strings.DatabaseObjectNotFoundException));
					}
					this.PopulateDatabaseProperties(mailboxDatabaseFromGuid);
					int currentDatabaseSchemaVersions = this.GetCurrentDatabaseSchemaVersions(guid);
					base.Result.StateAttribute6 = (double)currentDatabaseSchemaVersions;
					WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.StoreTracer, base.TraceContext, "Completed database schema version check probe execution against local database {0}", targetExtension, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Store\\DatabaseSchemaVersionCheckProbe.cs", 98);
				}
				else
				{
					WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.StoreTracer, base.TraceContext, "Skipping database schema version check probe against database {0} as it is not mounted locally", targetExtension, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Store\\DatabaseSchemaVersionCheckProbe.cs", 106);
				}
			}
			finally
			{
				base.Result.SampleValue = (double)((int)(DateTime.UtcNow - utcNow).TotalMilliseconds);
			}
		}

		private void PopulateDatabaseProperties(MailboxDatabase mailboxDatabase)
		{
			base.Result.StateAttribute2 = mailboxDatabase.IsExcludedFromProvisioning.ToString();
			base.Result.StateAttribute3 = mailboxDatabase.IsExcludedFromProvisioningBySchemaVersionMonitoring.ToString();
		}

		private int GetCurrentDatabaseSchemaVersions(Guid databaseGuid)
		{
			int result = -1;
			using (ExRpcAdmin exRpcAdmin = ExRpcAdmin.Create("Client=StoreActiveMonitoring", Environment.MachineName, null, null, null))
			{
				PropValue[][] databaseProcessInfo = exRpcAdmin.GetDatabaseProcessInfo(databaseGuid, new PropTag[]
				{
					PropTag.MailboxDatabaseVersion
				});
				if (databaseProcessInfo == null || databaseProcessInfo.Length != 1 || databaseProcessInfo[0].Length != 1)
				{
					throw new UnableToGetDatabaseSchemaVersionException(databaseGuid.ToString());
				}
				if (databaseProcessInfo[0][0].PropTag == PropTag.MailboxDatabaseVersion && databaseProcessInfo[0][0].Value is int)
				{
					result = (int)databaseProcessInfo[0][0].Value;
				}
			}
			return result;
		}
	}
}
