using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Management.FfoReporting.Common
{
	public class TrafficTask<TOutputObject> : FfoReportingDalTask<TOutputObject> where TOutputObject : new()
	{
		public TrafficTask(string dalTypeName) : base(dalTypeName)
		{
			this.Domain = new MultiValuedProperty<Fqdn>();
			this.Direction = new MultiValuedProperty<string>();
			this.Direction.Add(Schema.DirectionValues.Inbound.ToString());
			this.Direction.Add(Schema.DirectionValues.Outbound.ToString());
			this.AggregateBy = Schema.AggregateByValues.Day.ToString();
		}

		[QueryParameter("DomainListQueryDefinition", new string[]
		{

		})]
		[CmdletValidator("ValidateDomain", new object[]
		{

		}, ErrorMessage = Strings.IDs.InvalidDomain, ValidatorType = CmdletValidator.ValidatorTypes.PostprocessingWithConfigSession)]
		[Parameter(Mandatory = false, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public MultiValuedProperty<Fqdn> Domain { get; set; }

		[Parameter(Mandatory = false)]
		[QueryParameter("StartDateKeyQueryDefinition", new string[]
		{
			"StartHourKeyQueryDefinition"
		}, MethodName = "AddDateFilter")]
		public DateTime? StartDate { get; set; }

		[QueryParameter("EndDateKeyQueryDefinition", new string[]
		{
			"EndHourKeyQueryDefinition"
		}, MethodName = "AddDateFilter")]
		[Parameter(Mandatory = false)]
		public DateTime? EndDate { get; set; }

		[QueryParameter("DirectionListQueryDefinition", new string[]
		{

		})]
		[Parameter(Mandatory = false)]
		[CmdletValidator("ValidateEnum", new object[]
		{
			typeof(Schema.DirectionValues)
		}, ErrorMessage = Strings.IDs.InvalidDirection)]
		public MultiValuedProperty<string> Direction { get; set; }

		[Parameter(Mandatory = false)]
		[QueryParameter("AggregateByQueryDefinition", new string[]
		{

		})]
		[CmdletValidator("ValidateEnum", new object[]
		{
			typeof(Schema.AggregateByValues)
		}, ErrorMessage = Strings.IDs.InvalidAggregateBy)]
		public string AggregateBy { get; set; }

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

		protected override void CustomInternalValidate()
		{
			base.CustomInternalValidate();
			Schema.Utilities.CheckDates(this.StartDate, this.EndDate, new Schema.Utilities.NotifyNeedDefaultDatesDelegate(this.SetDefaultDates), new Schema.Utilities.ValidateDatesDelegate(Schema.Utilities.VerifyDateRange));
		}

		private void SetDefaultDates()
		{
			DateTime value = (DateTime)ExDateTime.UtcNow;
			this.EndDate = new DateTime?(value);
			this.StartDate = new DateTime?(value.AddDays(-14.0));
		}

		private const int DefaultDateOffset = -14;
	}
}
