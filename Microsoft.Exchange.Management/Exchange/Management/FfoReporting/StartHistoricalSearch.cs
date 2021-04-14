using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.FfoReporting.Common;
using Microsoft.Exchange.Management.ReportingTask;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.FfoReporting
{
	[OutputType(new Type[]
	{
		typeof(HistoricalSearch)
	})]
	[Cmdlet("Start", "HistoricalSearch")]
	public sealed class StartHistoricalSearch : HistoricalSearchBaseTask
	{
		public StartHistoricalSearch() : base("StartHistoricalSearch", "Microsoft.Exchange.Hygiene.ManagementHelper.HistoricalSearch.StartHistoricalSearchHelper")
		{
			this.Locale = Thread.CurrentThread.CurrentCulture;
		}

		[Parameter(Mandatory = true, ValueFromPipelineByPropertyName = false)]
		public HistoricalSearchReportType ReportType { get; set; }

		[Parameter(Mandatory = true, ValueFromPipelineByPropertyName = false)]
		public string ReportTitle { get; set; }

		[CmdletValidator("ValidateEmailAddressWithDomain", new object[]
		{
			CmdletValidator.EmailAddress.Recipient,
			CmdletValidator.WildcardValidationOptions.Disallow,
			CmdletValidator.EmailAcceptedDomainOptions.Verify
		}, ErrorMessage = Strings.IDs.InvalidNotifyAddress)]
		[Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
		public MultiValuedProperty<string> NotifyAddress { get; set; }

		[Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
		public DateTime StartDate { get; set; }

		[Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
		public DateTime EndDate { get; set; }

		[CmdletValidator("ValidateEnum", new object[]
		{
			typeof(MessageDeliveryStatus)
		}, ErrorMessage = Strings.IDs.InvalidDeliveryStatus)]
		[Parameter(Mandatory = false, ValueFromPipelineByPropertyName = false)]
		public string DeliveryStatus { get; set; }

		[Parameter(Mandatory = false, ValueFromPipelineByPropertyName = false)]
		[CmdletValidator("ValidateEmailAddress", new object[]
		{
			CmdletValidator.EmailAddress.Sender,
			CmdletValidator.WildcardValidationOptions.Allow
		})]
		public MultiValuedProperty<string> SenderAddress { get; set; }

		[Parameter(Mandatory = false, ValueFromPipelineByPropertyName = false)]
		[CmdletValidator("ValidateEmailAddress", new object[]
		{
			CmdletValidator.EmailAddress.Recipient,
			CmdletValidator.WildcardValidationOptions.Allow
		})]
		public MultiValuedProperty<string> RecipientAddress { get; set; }

		[Parameter(Mandatory = false, ValueFromPipelineByPropertyName = false)]
		public string OriginalClientIP { get; set; }

		[Parameter(Mandatory = false, ValueFromPipelineByPropertyName = false)]
		public MultiValuedProperty<string> MessageID { get; set; }

		[Parameter(Mandatory = false, ValueFromPipelineByPropertyName = false)]
		public MultiValuedProperty<Guid> DLPPolicy { get; set; }

		[Parameter(Mandatory = false, ValueFromPipelineByPropertyName = false)]
		public MultiValuedProperty<Guid> TransportRule { get; set; }

		[Parameter(Mandatory = false, ValueFromPipelineByPropertyName = false)]
		public CultureInfo Locale { get; set; }

		[Parameter(Mandatory = false, ValueFromPipelineByPropertyName = false)]
		public MessageDirection Direction { get; set; }

		protected override void InternalValidate()
		{
			try
			{
				base.InternalValidate();
				Schema.Utilities.ValidateParameters(this, () => base.ConfigSession, new HashSet<CmdletValidator.ValidatorTypes>
				{
					CmdletValidator.ValidatorTypes.Preprocessing
				});
				Schema.Utilities.VerifyDateRange(this.StartDate, this.EndDate);
				DateTime dateTime = (DateTime)ExDateTime.UtcNow;
				int days = dateTime.Subtract(this.StartDate).Days;
				if (days > 90)
				{
					base.WriteError(new ArgumentException(Strings.InvalidStartDate(90)), ErrorCategory.InvalidArgument, null);
				}
				if (this.EndDate > dateTime)
				{
					this.EndDate = dateTime;
				}
				if ((this.ReportType == HistoricalSearchReportType.MessageTrace || this.ReportType == HistoricalSearchReportType.MessageTraceDetail) && this.MessageID == null && this.RecipientAddress == null && this.SenderAddress == null)
				{
					base.WriteError(new ArgumentException(Strings.MessageTraceMinimumCriteriaFieldsInErrorDeliveryStatus), ErrorCategory.InvalidArgument, null);
				}
				if (!string.IsNullOrEmpty(this.OriginalClientIP))
				{
					IPvxAddress none = IPvxAddress.None;
					if (!IPvxAddress.TryParse(this.OriginalClientIP, out none))
					{
						base.WriteError(new ArgumentException(Strings.InvalidIPAddress(this.OriginalClientIP)), ErrorCategory.InvalidArgument, null);
					}
				}
			}
			catch (InvalidExpressionException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidArgument, null);
			}
			catch (Exception exception2)
			{
				base.WriteError(exception2, ErrorCategory.InvalidOperation, null);
			}
		}

		private const int MaxReportDays = 90;

		private const string ComponentName = "StartHistoricalSearch";
	}
}
