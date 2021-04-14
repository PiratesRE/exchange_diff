using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetRetentionPolicyTagsResponse : OptionsResponseBase
	{
		public GetRetentionPolicyTagsResponse()
		{
			this.RetentionPolicyTagDisplayCollection = new RetentionPolicyTagDisplayCollection();
		}

		[DataMember(IsRequired = true)]
		public RetentionPolicyTagDisplayCollection RetentionPolicyTagDisplayCollection { get; set; }

		public override string ToString()
		{
			return string.Format("GetRetentionPolicyTagsResponse: {0}", this.RetentionPolicyTagDisplayCollection);
		}
	}
}
