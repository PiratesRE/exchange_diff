using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class UpdateInboxRuleRequestBase : BaseJsonRequest
	{
		[DataMember]
		public bool AlwaysDeleteOutlookRulesBlob { get; set; }

		[DataMember]
		public bool Force { get; set; }

		public override string ToString()
		{
			return string.Format(base.GetType().Name + ": AlwaysDeleteOutlookRulesBlob = {0}, Force = {1}", this.AlwaysDeleteOutlookRulesBlob, this.Force);
		}
	}
}
