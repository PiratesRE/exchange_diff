using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "GetUserByLiveIdRequest", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	public class GetUserByLiveIdRequest : Request
	{
		[DataMember]
		public string LiveId
		{
			get
			{
				return this.LiveIdField;
			}
			set
			{
				this.LiveIdField = value;
			}
		}

		private string LiveIdField;
	}
}
