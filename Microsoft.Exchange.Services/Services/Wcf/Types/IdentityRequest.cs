using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class IdentityRequest : BaseJsonRequest
	{
		[DataMember(IsRequired = true)]
		public Identity Identity { get; set; }

		public override string ToString()
		{
			return string.Format("IdentityRequest: {0}", this.Identity);
		}
	}
}
