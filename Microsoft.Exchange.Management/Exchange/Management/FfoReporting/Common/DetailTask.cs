using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Management.FfoReporting.Common
{
	public abstract class DetailTask<TOutputObject> : FfoReportingDalTask<TOutputObject> where TOutputObject : new()
	{
		public DetailTask(string dalTypeName) : base(dalTypeName)
		{
			this.MessageTraceId = new MultiValuedProperty<Guid>();
			this.Domain = new MultiValuedProperty<Fqdn>();
			this.Direction = new MultiValuedProperty<string>();
			this.Direction.Add(Schema.DirectionValues.Inbound.ToString());
			this.Direction.Add(Schema.DirectionValues.Outbound.ToString());
			this.MessageId = new MultiValuedProperty<string>();
			this.SenderAddress = new MultiValuedProperty<string>();
			this.RecipientAddress = new MultiValuedProperty<string>();
			this.Action = new MultiValuedProperty<string>();
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<Guid> MessageTraceId { get; set; }

		[CmdletValidator("ValidateDomain", new object[]
		{

		}, ErrorMessage = Strings.IDs.InvalidDomain, ValidatorType = CmdletValidator.ValidatorTypes.PostprocessingWithConfigSession)]
		[QueryParameter("DomainListQueryDefinition", new string[]
		{

		})]
		[Parameter(Mandatory = false, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public MultiValuedProperty<Fqdn> Domain { get; set; }

		[QueryParameter("StartDateKeyQueryDefinition", new string[]
		{
			"StartHourKeyQueryDefinition"
		}, MethodName = "AddDateFilter")]
		[Parameter(Mandatory = false)]
		public DateTime? StartDate { get; set; }

		[Parameter(Mandatory = false)]
		[QueryParameter("EndDateKeyQueryDefinition", new string[]
		{
			"EndHourKeyQueryDefinition"
		}, MethodName = "AddDateFilter")]
		public DateTime? EndDate { get; set; }

		[QueryParameter("DirectionListQueryDefinition", new string[]
		{

		})]
		[CmdletValidator("ValidateEnum", new object[]
		{
			typeof(Schema.DirectionValues)
		}, ErrorMessage = Strings.IDs.InvalidDirection)]
		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> Direction { get; set; }

		[Parameter(Mandatory = false)]
		[QueryParameter("MessageIdListQueryDefinition", new string[]
		{

		})]
		public MultiValuedProperty<string> MessageId { get; set; }

		[Parameter(Mandatory = false)]
		[QueryParameter("SenderAddressListQueryDefinition", new string[]
		{

		})]
		public MultiValuedProperty<string> SenderAddress { get; set; }

		[Parameter(Mandatory = false)]
		[QueryParameter("RecipientAddressListQueryDefinition", new string[]
		{

		})]
		public MultiValuedProperty<string> RecipientAddress { get; set; }

		[QueryParameter("ActionListQueryDefinition", new string[]
		{

		})]
		[Parameter(Mandatory = false)]
		[CmdletValidator("ValidateEnum", new object[]
		{
			typeof(Schema.Actions)
		}, ErrorMessage = Strings.IDs.InvalidActionParameter)]
		public MultiValuedProperty<string> Action { get; set; }

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
