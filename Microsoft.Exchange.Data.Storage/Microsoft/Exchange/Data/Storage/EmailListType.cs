using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum EmailListType
	{
		None = 0,
		Email1 = 1,
		Email2 = 2,
		Email3 = 4,
		BusinessFax = 8,
		HomeFax = 16,
		OtherFax = 32
	}
}
