using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum EffectiveRights
	{
		None = 0,
		Modify = 1,
		Read = 2,
		Delete = 4,
		CreateHierarchy = 8,
		CreateContents = 16,
		CreateAssociated = 32
	}
}
