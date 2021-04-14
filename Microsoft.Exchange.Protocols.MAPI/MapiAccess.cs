using System;

namespace Microsoft.Exchange.Protocols.MAPI
{
	[Flags]
	public enum MapiAccess
	{
		None = 0,
		Modify = 1,
		Read = 2,
		Delete = 4,
		CreateHierarchy = 8,
		CreateContent = 16,
		CreateAssociated = 32
	}
}
