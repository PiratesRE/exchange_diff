using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	[DataContract(Name = "SetCompanyDirSyncEnabledRequest", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	public class SetCompanyDirSyncEnabledRequest : Request
	{
		[DataMember]
		public bool EnableDirSync
		{
			get
			{
				return this.EnableDirSyncField;
			}
			set
			{
				this.EnableDirSyncField = value;
			}
		}

		private bool EnableDirSyncField;
	}
}
