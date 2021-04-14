using System;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace Microsoft.Online.BOX.Shell
{
	[Flags]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "Filters", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.Shell")]
	public enum Filters
	{
		[EnumMember]
		None = 0,
		[EnumMember]
		Images = 1,
		[EnumMember]
		StaticText = 2,
		[EnumMember]
		UserSpecificText = 4,
		[EnumMember]
		Text = 6,
		[EnumMember]
		StaticLinkUrls = 8,
		[EnumMember]
		UserSpecificLinkUrls = 16,
		[EnumMember]
		LinkUrls = 24,
		[EnumMember]
		Css = 32,
		[EnumMember]
		BecContextToken = 64,
		[EnumMember]
		UserSpecificAll = 84,
		[EnumMember]
		NonUserSpecificAll = 43,
		[EnumMember]
		All = -1
	}
}
