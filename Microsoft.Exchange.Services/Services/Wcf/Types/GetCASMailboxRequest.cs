using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetCASMailboxRequest : BaseJsonRequest
	{
		[DataMember(IsRequired = true)]
		public GetCASMailboxOptions Options { get; set; }

		public override string ToString()
		{
			return string.Format("GetCASMailboxOptions: {0}", this.Options);
		}
	}
}
