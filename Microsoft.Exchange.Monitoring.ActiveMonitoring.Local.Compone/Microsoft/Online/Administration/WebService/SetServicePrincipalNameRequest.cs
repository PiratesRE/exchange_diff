using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[DataContract(Name = "SetServicePrincipalNameRequest", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	public class SetServicePrincipalNameRequest : Request
	{
		[DataMember]
		public string[] AddSpn
		{
			get
			{
				return this.AddSpnField;
			}
			set
			{
				this.AddSpnField = value;
			}
		}

		[DataMember]
		public Guid ObjectId
		{
			get
			{
				return this.ObjectIdField;
			}
			set
			{
				this.ObjectIdField = value;
			}
		}

		[DataMember]
		public string[] RemoveSpn
		{
			get
			{
				return this.RemoveSpnField;
			}
			set
			{
				this.RemoveSpnField = value;
			}
		}

		private string[] AddSpnField;

		private Guid ObjectIdField;

		private string[] RemoveSpnField;
	}
}
