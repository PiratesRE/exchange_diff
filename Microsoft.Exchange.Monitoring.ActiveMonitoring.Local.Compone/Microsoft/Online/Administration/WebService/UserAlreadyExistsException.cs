using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[DataContract(Name = "UserAlreadyExistsException", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class UserAlreadyExistsException : ObjectAlreadyExistsException
	{
		[DataMember]
		public bool UserCollisionInLive
		{
			get
			{
				return this.UserCollisionInLiveField;
			}
			set
			{
				this.UserCollisionInLiveField = value;
			}
		}

		private bool UserCollisionInLiveField;
	}
}
