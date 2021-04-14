using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[DataContract(Name = "GetSubscriptionRequest", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class GetSubscriptionRequest : Request
	{
		[DataMember]
		public Guid SubscriptionId
		{
			get
			{
				return this.SubscriptionIdField;
			}
			set
			{
				this.SubscriptionIdField = value;
			}
		}

		private Guid SubscriptionIdField;
	}
}
