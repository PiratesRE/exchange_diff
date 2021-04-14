using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[DebuggerStepThrough]
	[DataContract(Name = "RestoreUserErrorException", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class RestoreUserErrorException : DataOperationException
	{
		[DataMember]
		public RestoreUserError[] RestoreUserErrors
		{
			get
			{
				return this.RestoreUserErrorsField;
			}
			set
			{
				this.RestoreUserErrorsField = value;
			}
		}

		private RestoreUserError[] RestoreUserErrorsField;
	}
}
