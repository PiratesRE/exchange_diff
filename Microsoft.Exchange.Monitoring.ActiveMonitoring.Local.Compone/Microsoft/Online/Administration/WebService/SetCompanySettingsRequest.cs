using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "SetCompanySettingsRequest", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	public class SetCompanySettingsRequest : Request
	{
		[DataMember]
		public CompanySettings Settings
		{
			get
			{
				return this.SettingsField;
			}
			set
			{
				this.SettingsField = value;
			}
		}

		private CompanySettings SettingsField;
	}
}
