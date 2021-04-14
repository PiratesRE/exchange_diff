using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "GetUserRequest", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	public class GetUserRequest : Request
	{
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
		public bool? ReturnDeletedUsers
		{
			get
			{
				return this.ReturnDeletedUsersField;
			}
			set
			{
				this.ReturnDeletedUsersField = value;
			}
		}

		private Guid ObjectIdField;

		private bool? ReturnDeletedUsersField;
	}
}
