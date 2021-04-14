using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Name = "RemoveModernGroupResponse", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class RemoveModernGroupResponse : BaseJsonResponse
	{
		internal RemoveModernGroupResponse()
		{
			this.Error = null;
		}

		internal RemoveModernGroupResponse(string error)
		{
			this.Error = error;
		}

		[DataMember(Name = "ErrorState", IsRequired = false)]
		public UnifiedGroupResponseErrorState ErrorState { get; set; }

		[DataMember(Name = "Error", IsRequired = false)]
		public string Error { get; set; }
	}
}
