using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "UserAuthenticationUnchangedException", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	[DebuggerStepThrough]
	public class UserAuthenticationUnchangedException : MsolAdministrationException
	{
		[DataMember]
		public string ObjectKey
		{
			get
			{
				return this.ObjectKeyField;
			}
			set
			{
				this.ObjectKeyField = value;
			}
		}

		private string ObjectKeyField;
	}
}
