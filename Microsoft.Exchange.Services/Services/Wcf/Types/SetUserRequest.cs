using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SetUserRequest : BaseJsonRequest
	{
		[DataMember(IsRequired = true)]
		public SetUserData User { get; set; }

		public override string ToString()
		{
			return string.Format("SetUserRequest: {0}", this.User);
		}
	}
}
