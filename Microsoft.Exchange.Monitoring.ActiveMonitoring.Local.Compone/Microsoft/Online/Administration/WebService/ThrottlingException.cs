using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "ThrottlingException", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	public class ThrottlingException : MsolAdministrationException
	{
		[DataMember]
		public int RetryWaitPeriod
		{
			get
			{
				return this.RetryWaitPeriodField;
			}
			set
			{
				this.RetryWaitPeriodField = value;
			}
		}

		private int RetryWaitPeriodField;
	}
}
