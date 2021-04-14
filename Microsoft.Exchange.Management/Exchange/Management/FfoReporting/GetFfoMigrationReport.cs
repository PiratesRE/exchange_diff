using System;
using System.Management.Automation;
using Microsoft.Exchange.Management.FfoReporting.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Management.FfoReporting
{
	[OutputType(new Type[]
	{
		typeof(FfoMigrationReport)
	})]
	[Cmdlet("Get", "FfoMigrationReport")]
	public sealed class GetFfoMigrationReport : FfoReportingDalTask<FfoMigrationReport>
	{
		public GetFfoMigrationReport() : base("Microsoft.Exchange.Hygiene.Data.AsyncQueue.AsyncQueueReport, Microsoft.Exchange.Hygiene.Data")
		{
		}

		public override string DataSessionTypeName
		{
			get
			{
				return "Microsoft.Exchange.Hygiene.Data.AsyncQueue.AsyncQueueSession";
			}
		}

		public override string DataSessionMethodName
		{
			get
			{
				return "FindMigrationReport";
			}
		}

		public override string ComponentName
		{
			get
			{
				return ExchangeComponent.FfoRws.Name;
			}
		}

		public override string MonitorEventName
		{
			get
			{
				return "FFO Reporting Task Status Monitor";
			}
		}

		public override string DalMonitorEventName
		{
			get
			{
				return "FFO DAL Retrieval Status Monitor";
			}
		}
	}
}
