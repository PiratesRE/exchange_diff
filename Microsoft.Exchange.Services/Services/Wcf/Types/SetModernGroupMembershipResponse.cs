using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SetModernGroupMembershipResponse
	{
		public SetModernGroupMembershipResponse()
		{
			this.ErrorCode = ModernGroupActionError.None;
		}

		public SetModernGroupMembershipResponse(ModernGroupActionError errorCode)
		{
			this.ErrorCode = errorCode;
		}

		[DataMember]
		public UnifiedGroupResponseErrorState ErrorState { get; set; }

		[DataMember]
		public string Error { get; set; }

		[DataMember]
		public ModernGroupActionError ErrorCode { get; set; }

		[DataMember]
		public JoinResponse JoinInfo { get; set; }
	}
}
