using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.BOX.Shell
{
	[DebuggerStepThrough]
	[DataContract(Name = "GetSuiteServiceInfoRequest", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.Shell")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class GetSuiteServiceInfoRequest : ShellServiceRequest
	{
		[DataMember]
		public string UrlOfRequestingPage
		{
			get
			{
				return this.UrlOfRequestingPageField;
			}
			set
			{
				this.UrlOfRequestingPageField = value;
			}
		}

		private string UrlOfRequestingPageField;
	}
}
