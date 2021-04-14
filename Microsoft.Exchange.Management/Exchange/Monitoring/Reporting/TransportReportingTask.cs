using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring.Reporting
{
	public abstract class TransportReportingTask : ReportingTask<OrganizationIdParameter>
	{
		[Parameter(Mandatory = false)]
		public AdSiteIdParameter AdSite
		{
			get
			{
				return (AdSiteIdParameter)base.Fields["AdSite"];
			}
			set
			{
				base.Fields["AdSite"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter DailyStatistics
		{
			get
			{
				return (SwitchParameter)(base.Fields["DailyStatistics"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["DailyStatistics"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "StartEndDateTime")]
		public ExDateTime EndDate
		{
			get
			{
				return (ExDateTime)base.Fields["EndDate"];
			}
			set
			{
				base.Fields["EndDate"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "StartEndDateTime")]
		public ExDateTime StartDate
		{
			get
			{
				return (ExDateTime)base.Fields["StartDate"];
			}
			set
			{
				base.Fields["StartDate"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "ReportingPeriod")]
		public ReportingPeriod ReportingPeriod
		{
			get
			{
				return (ReportingPeriod)base.Fields["ReportingPeriod"];
			}
			set
			{
				base.Fields["ReportingPeriod"] = value;
			}
		}

		[Parameter(Mandatory = false, Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public override OrganizationIdParameter Identity
		{
			get
			{
				return (OrganizationIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			if (base.ParameterSetName == "StartEndDateTime" && this.StartDate >= this.EndDate)
			{
				base.WriteError(new ArgumentException(Strings.InvalidTimeRange, "StartDate"), ErrorCategory.InvalidArgument, null);
			}
			TaskLogger.LogExit();
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			TaskLogger.LogEnter();
			if (dataObject != null)
			{
				ADOrganizationalUnit adorganizationalUnit = dataObject as ADOrganizationalUnit;
				if (adorganizationalUnit != null)
				{
					if (!adorganizationalUnit.OrganizationId.Equals(OrganizationId.ForestWideOrgId))
					{
						this.WriteStatistics(adorganizationalUnit.OrganizationId.OrganizationalUnit);
					}
				}
				else
				{
					base.WriteResult(dataObject);
				}
			}
			TaskLogger.LogExit();
		}

		protected override void WriteResult<T>(IEnumerable<T> dataObjects)
		{
			TaskLogger.LogEnter();
			if (dataObjects != null)
			{
				if (this.Identity != null)
				{
					base.WriteResult<T>(dataObjects);
				}
				else if (base.CurrentOrganizationId.Equals(OrganizationId.ForestWideOrgId))
				{
					this.WriteStatistics(null);
				}
				else
				{
					this.WriteStatistics(base.CurrentOrganizationId.OrganizationalUnit);
					base.WriteResult<T>(dataObjects);
				}
			}
			TaskLogger.LogExit();
		}

		protected abstract void WriteStatistics(ADObjectId tenantId);

		protected const int SqlNetworkError = 53;

		internal abstract class ParameterSet
		{
			internal const string ReportingPeriod = "ReportingPeriod";

			internal const string StartEndDateTime = "StartEndDateTime";
		}
	}
}
