using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	[DataContract(Name = "GetUserByUpnRequest", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	public class GetUserByUpnRequest : Request
	{
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

		[DataMember]
		public string UserPrincipalName
		{
			get
			{
				return this.UserPrincipalNameField;
			}
			set
			{
				this.UserPrincipalNameField = value;
			}
		}

		private bool? ReturnDeletedUsersField;

		private string UserPrincipalNameField;
	}
}
