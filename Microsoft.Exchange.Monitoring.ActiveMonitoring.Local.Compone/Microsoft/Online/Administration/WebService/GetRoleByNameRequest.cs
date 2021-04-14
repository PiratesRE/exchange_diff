using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "GetRoleByNameRequest", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	[DebuggerStepThrough]
	public class GetRoleByNameRequest : Request
	{
		[DataMember]
		public string RoleName
		{
			get
			{
				return this.RoleNameField;
			}
			set
			{
				this.RoleNameField = value;
			}
		}

		private string RoleNameField;
	}
}
