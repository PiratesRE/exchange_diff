using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[Flags]
	[DataContract(Name = "RelayLocation")]
	internal enum RelayLocation
	{
		[EnumMember]
		None = 0,
		[EnumMember]
		Internet = 1,
		[EnumMember]
		Intranet = 2,
		[EnumMember]
		All = 3
	}
}
