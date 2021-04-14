using System;

namespace Microsoft.Exchange.Security.Authorization
{
	[Flags]
	public enum AccessMask
	{
		Open = 0,
		CreateChild = 1,
		DeleteChild = 2,
		List = 4,
		Self = 8,
		ReadProp = 16,
		WriteProp = 32,
		DeleteTree = 64,
		ListObject = 128,
		ControlAccess = 256,
		MaximumAllowed = 33554432,
		GenericRead = -2147483648
	}
}
