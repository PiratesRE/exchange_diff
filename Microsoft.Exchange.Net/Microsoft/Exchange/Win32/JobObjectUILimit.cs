using System;

namespace Microsoft.Exchange.Win32
{
	[Flags]
	internal enum JobObjectUILimit : uint
	{
		Default = 0U,
		Handles = 1U,
		ReadClipboard = 2U,
		SystemParameters = 8U,
		WriteClipboard = 4U,
		Desktop = 64U,
		DisplaySettings = 16U,
		ExitWindows = 128U,
		GlobalAtoms = 32U
	}
}
