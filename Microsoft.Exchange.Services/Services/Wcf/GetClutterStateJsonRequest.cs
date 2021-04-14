using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetClutterStateJsonRequest : BaseJsonRequest
	{
		[DataMember(IsRequired = true, Order = 0)]
		public GetClutterStateRequest Body;
	}
}
