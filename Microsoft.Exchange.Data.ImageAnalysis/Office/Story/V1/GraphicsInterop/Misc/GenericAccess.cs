using System;

namespace Microsoft.Office.Story.V1.GraphicsInterop.Misc
{
	[Flags]
	internal enum GenericAccess
	{
		GENERIC_ALL = 268435456,
		GENERIC_EXECUTE = 536870912,
		GENERIC_WRITE = 1073741824,
		GENERIC_READ = -2147483648
	}
}
