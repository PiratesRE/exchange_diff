using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", TypeName = "ApprovalRequestDataType")]
	[Serializable]
	public class ApprovalRequestDataType
	{
		public ApprovalRequestDataType()
		{
		}

		internal ApprovalRequestDataType(MessageItem approvalRequest)
		{
			this.IsUndecidedApprovalRequest = approvalRequest.IsValidUndecidedApprovalRequest();
			if (approvalRequest.GetValueAsNullable<int>(MessageItemSchema.ApprovalDecision) != null)
			{
				this.ApprovalDecision = approvalRequest.GetValueAsNullable<int>(MessageItemSchema.ApprovalDecision).Value;
			}
			this.ApprovalDecisionMaker = approvalRequest.GetValueOrDefault<string>(MessageItemSchema.ApprovalDecisionMaker);
			ExDateTime? valueAsNullable = approvalRequest.GetValueAsNullable<ExDateTime>(MessageItemSchema.ApprovalDecisionTime);
			if (valueAsNullable != null)
			{
				this.ApprovalDecisionTime = ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime(valueAsNullable.Value);
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 1)]
		public bool IsUndecidedApprovalRequest { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 2)]
		public int ApprovalDecision { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 3)]
		public string ApprovalDecisionMaker { get; set; }

		[DateTimeString]
		[DataMember(EmitDefaultValue = false, Order = 4)]
		public string ApprovalDecisionTime { get; set; }
	}
}
