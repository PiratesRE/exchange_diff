using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "MsolConnectResponse", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	public class MsolConnectResponse : Response
	{
		[DataMember]
		public bool UpdateAvailable
		{
			get
			{
				return this.UpdateAvailableField;
			}
			set
			{
				this.UpdateAvailableField = value;
			}
		}

		private bool UpdateAvailableField;
	}
}
