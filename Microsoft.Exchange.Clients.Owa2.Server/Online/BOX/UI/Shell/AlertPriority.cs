using System;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace Microsoft.Online.BOX.UI.Shell
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "AlertPriority", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.UI.Shell")]
	public enum AlertPriority
	{
		[EnumMember]
		Unknown,
		[EnumMember]
		Low,
		[EnumMember]
		Medium,
		[EnumMember]
		High
	}
}
