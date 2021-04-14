using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Audio
{
	[Guid("d16679f2-6ca0-472d-8d31-2f5d55aee155")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	internal interface IWMProfileManager
	{
		void CreateEmptyProfile();

		void LoadProfileByID();

		void LoadProfileByData([MarshalAs(UnmanagedType.LPWStr)] [In] string pwszProfile, [MarshalAs(UnmanagedType.Interface)] out IWMProfile ppProfile);

		void SaveProfile();

		void GetSystemProfileCount();

		void LoadSystemProfile();
	}
}
