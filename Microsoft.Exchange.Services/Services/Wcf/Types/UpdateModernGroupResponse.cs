using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Name = "UpdateModernGroupResponse", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class UpdateModernGroupResponse : BaseJsonResponse
	{
		internal UpdateModernGroupResponse()
		{
			this.Error = null;
		}

		internal UpdateModernGroupResponse(string error)
		{
			this.Error = error;
		}

		[DataMember(Name = "ErrorState", IsRequired = false)]
		public UnifiedGroupResponseErrorState ErrorState { get; set; }

		[DataMember(Name = "Error", IsRequired = false)]
		public string Error { get; set; }
	}
}
