using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Data
{
	[DataContract]
	public enum ItemIsRuleFormType
	{
		[EnumMember]
		Edit = 1,
		[EnumMember]
		Read,
		[EnumMember]
		ReadOrEdit
	}
}
