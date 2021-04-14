using System;
using System.Management.Automation;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.ReportingTask;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Management.FfoReporting.Common
{
	public class MtrtTask<TOutputObject> : FfoReportingDalTask<TOutputObject> where TOutputObject : new()
	{
		public MtrtTask(string dalTypeName) : base(dalTypeName)
		{
		}

		[Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
		[QueryParameter("StartDateQueryDefinition", new string[]
		{

		})]
		public DateTime? StartDate { get; set; }

		[Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
		[QueryParameter("EndDateQueryDefinition", new string[]
		{

		})]
		public DateTime? EndDate { get; set; }

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
			Schema.Utilities.CheckDates(this.StartDate, this.EndDate, new Schema.Utilities.NotifyNeedDefaultDatesDelegate(this.SetDefaultDates), new Schema.Utilities.ValidateDatesDelegate(this.ValidateDateRange));
		}

		private void SetDefaultDates()
		{
			DateTime value = (DateTime)ExDateTime.UtcNow;
			this.EndDate = new DateTime?(value);
			this.StartDate = new DateTime?(value.AddHours(-48.0));
		}

		private void ValidateDateRange(DateTime startTime, DateTime endTime)
		{
			Schema.Utilities.VerifyDateRange(startTime, endTime);
			int days = ((DateTime)ExDateTime.UtcNow).Subtract(startTime).Days;
			if (days > 30)
			{
				throw new InvalidExpressionException(Strings.InvalidStartDateOffset);
			}
		}

		private const int DefaultHourOffset = -48;

		private const int MaxDateOffset = 30;
	}
}
