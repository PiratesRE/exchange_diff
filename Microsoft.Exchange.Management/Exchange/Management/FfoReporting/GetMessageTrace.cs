using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.FfoReporting.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.FfoReporting
{
	[Cmdlet("Get", "MessageTrace")]
	[OutputType(new Type[]
	{
		typeof(MessageTrace)
	})]
	public sealed class GetMessageTrace : MtrtTask<MessageTrace>
	{
		public GetMessageTrace() : base("Microsoft.Exchange.Hygiene.Data.MessageTrace.Reports.MessageTrace, Microsoft.Exchange.Hygiene.Data")
		{
			this.MessageId = new MultiValuedProperty<string>();
			this.SenderAddress = new MultiValuedProperty<string>();
			this.RecipientAddress = new MultiValuedProperty<string>();
			this.Status = new MultiValuedProperty<string>();
		}

		[QueryParameter("MessageIdListQueryDefinition", new string[]
		{

		})]
		[Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
		public MultiValuedProperty<string> MessageId { get; set; }

		[Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
		[QueryParameter("SenderAddressListQueryDefinition", new string[]
		{

		})]
		[CmdletValidator("ValidateEmailAddress", new object[]
		{
			CmdletValidator.EmailAddress.Sender,
			CmdletValidator.WildcardValidationOptions.Allow
		})]
		public MultiValuedProperty<string> SenderAddress { get; set; }

		[CmdletValidator("ValidateEmailAddress", new object[]
		{
			CmdletValidator.EmailAddress.Recipient,
			CmdletValidator.WildcardValidationOptions.Allow
		})]
		[Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
		[QueryParameter("RecipientAddressListQueryDefinition", new string[]
		{

		})]
		public MultiValuedProperty<string> RecipientAddress { get; set; }

		[QueryParameter("MailDeliveryStatusListDefinition", new string[]
		{

		})]
		[CmdletValidator("ValidateEnum", new object[]
		{
			typeof(Schema.DeliveryStatusValues)
		}, ErrorMessage = Strings.IDs.InvalidDeliveryStatus)]
		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> Status { get; set; }

		[QueryParameter("InternalMessageIdQueryDefinition", new string[]
		{

		})]
		[Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
		public Guid? MessageTraceId { get; set; }

		[Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
		[QueryParameter("ToIPAddressQueryDefinition", new string[]
		{

		})]
		public string ToIP { get; set; }

		[QueryParameter("FromIPAddressQueryDefinition", new string[]
		{

		})]
		[Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
		public string FromIP { get; set; }

		protected override void CustomInternalValidate()
		{
			base.CustomInternalValidate();
			bool flag = false;
			MultiValuedProperty<string> multiValuedProperty = new MultiValuedProperty<string>();
			foreach (string text in this.MessageId)
			{
				if (!string.IsNullOrEmpty(text))
				{
					string item = text;
					bool flag2 = text[0] != '<' && text[text.Length - 1] != '>';
					if (flag2)
					{
						item = '<' + text + '>';
						flag = true;
					}
					multiValuedProperty.Add(item);
				}
			}
			if (flag)
			{
				this.MessageId = multiValuedProperty;
			}
		}
	}
}
