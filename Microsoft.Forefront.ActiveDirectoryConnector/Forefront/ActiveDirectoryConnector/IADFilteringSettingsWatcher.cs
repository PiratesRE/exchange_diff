using System;
using System.Runtime.InteropServices;

namespace Microsoft.Forefront.ActiveDirectoryConnector
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[TypeLibType(TypeLibTypeFlags.FNonExtensible | TypeLibTypeFlags.FOleAutomation)]
	[Guid("2307AD21-F105-4DA4-B699-355E4F3C9D4A")]
	[ComImport]
	public interface IADFilteringSettingsWatcher
	{
		void Start();

		void Stop();

		int GetProcessId();
	}
}
