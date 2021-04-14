using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Audio
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("96406BDA-2B2B-11d3-B36B-00C04F6108FF")]
	[ComImport]
	internal interface IWMHeaderInfo
	{
		void GetAttributeCount();

		void GetAttributeByIndex();

		void GetAttributeByName([In] [Out] ref ushort pwStreamNum, [MarshalAs(UnmanagedType.LPWStr)] [In] string pszName, out WindowsMediaNativeMethods.WMT_ATTR_DATATYPE pType, ref ulong pValue, [In] [Out] ref ushort pcbLength);

		void SetAttribute();

		void GetMarkerCount();

		void GetMarker();

		void AddMarker();

		void RemoveMarker();

		void GetScriptCount();

		void GetScript();

		void AddScript();

		void RemoveScript();
	}
}
