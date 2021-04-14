using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.MailSubmission;
using Microsoft.Exchange.Transport.MessageRepository;

namespace Microsoft.Exchange.Management.ResubmitRequest
{
	[Cmdlet("Add", "ResubmitRequest", SupportsShouldProcess = true, DefaultParameterSetName = "MDBResubmitParams", ConfirmImpact = ConfirmImpact.Medium)]
	public sealed class NewResubmitRequest : NewTaskBase<ResubmitRequest>
	{
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ValueFromPipeline = true)]
		public ServerIdParameter Server
		{
			get
			{
				return (ServerIdParameter)base.Fields["Server"];
			}
			set
			{
				base.Fields["Server"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public DateTime StartTime { get; set; }

		[Parameter(Mandatory = true)]
		public DateTime EndTime { get; set; }

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> UnresponsivePrimaryServers { get; set; }

		[Parameter(Mandatory = false, ParameterSetName = "MDBResubmitParams")]
		public Guid Destination { get; set; }

		[Parameter(Mandatory = false)]
		public Guid CorrelationId { get; set; }

		[Parameter(Mandatory = false)]
		public bool TestOnly { get; set; }

		[Parameter(Mandatory = false, ParameterSetName = "ConditionalResubmitParams")]
		public string Recipient { get; set; }

		[Parameter(Mandatory = false, ParameterSetName = "ConditionalResubmitParams")]
		public string Sender { get; set; }

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.AddResubmitRequestConfirmation(this.StartTime.ToString(), this.EndTime.ToString(), this.Destination.ToString());
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return new ResubmitRequestDataProvider(this.Server, new ResubmitRequestId(0L));
		}

		protected override IConfigurable PrepareDataObject()
		{
			return ResubmitRequest.Create(0L, string.Empty, this.StartTime, this.Destination.ToString(), null, this.EndTime, DateTime.UtcNow, 0);
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			if (this.Server == null)
			{
				this.Server = new ServerIdParameter();
			}
			if (this.StartTime >= this.EndTime)
			{
				base.WriteError(new LocalizedException(Strings.InvalidStartAndEndTime), ErrorCategory.InvalidArgument, null);
			}
			if (this.Destination.Equals(Guid.Empty) && !this.TryCreateConditionalString(out this.conditionalString))
			{
				base.WriteError(new LocalizedException(Strings.InvalidParameterSet), ErrorCategory.InvalidArgument, null);
			}
		}

		protected override void InternalProcessRecord()
		{
			try
			{
				MessageResubmissionRpcClientImpl messageResubmissionRpcClientImpl = new MessageResubmissionRpcClientImpl(this.Server.Fqdn);
				string[] unresponsivePrimaryServers = (this.UnresponsivePrimaryServers != null) ? this.UnresponsivePrimaryServers.ToArray() : null;
				byte[] reservedBytes = null;
				if (this.TestOnly)
				{
					MdbefPropertyCollection mdbefPropertyCollection = new MdbefPropertyCollection();
					mdbefPropertyCollection[65547U] = true;
					reservedBytes = mdbefPropertyCollection.GetBytes();
				}
				AddResubmitRequestStatus addResubmitRequestStatus;
				if (string.IsNullOrEmpty(this.conditionalString))
				{
					addResubmitRequestStatus = messageResubmissionRpcClientImpl.AddMdbResubmitRequest((this.CorrelationId == Guid.Empty) ? Guid.NewGuid() : this.CorrelationId, new Guid(this.DataObject.Destination), this.StartTime.ToUniversalTime().Ticks, this.EndTime.ToUniversalTime().Ticks, unresponsivePrimaryServers, reservedBytes);
				}
				else
				{
					addResubmitRequestStatus = messageResubmissionRpcClientImpl.AddConditionalResubmitRequest((this.CorrelationId == Guid.Empty) ? Guid.NewGuid() : this.CorrelationId, this.StartTime.ToUniversalTime().Ticks, this.EndTime.ToUniversalTime().Ticks, this.conditionalString, unresponsivePrimaryServers, reservedBytes);
				}
				if (addResubmitRequestStatus != AddResubmitRequestStatus.Success)
				{
					base.WriteError(new LocalizedException(Strings.AddResubmitRequestFailed(addResubmitRequestStatus)), ErrorCategory.NotSpecified, null);
				}
			}
			catch (RpcException rpcException)
			{
				GetResubmitRequest.ProcessRpcError(rpcException, this.Server.Fqdn, this);
			}
			catch (ResubmitRequestException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidArgument, null);
			}
			catch (LocalizedException exception2)
			{
				base.WriteError(exception2, ExchangeErrorCategory.Client, null);
			}
		}

		private bool TryCreateConditionalString(out string conditionalString)
		{
			bool result = false;
			StringBuilder stringBuilder = new StringBuilder();
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (!string.IsNullOrEmpty(this.Recipient) && SmtpAddress.IsValidSmtpAddress(this.Recipient))
			{
				result = true;
			}
			dictionary.Add("toAddress", this.Recipient);
			if (!string.IsNullOrEmpty(this.Sender) && SmtpAddress.IsValidSmtpAddress(this.Sender))
			{
				result = true;
			}
			dictionary.Add("fromAddress", this.Sender);
			foreach (string text in dictionary.Keys)
			{
				if (!string.IsNullOrEmpty(dictionary[text]))
				{
					stringBuilder.AppendFormat("{0}={1};", text, dictionary[text]);
				}
			}
			conditionalString = stringBuilder.ToString();
			return result;
		}

		private string conditionalString;
	}
}
