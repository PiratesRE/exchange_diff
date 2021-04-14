using System;
using System.Runtime.InteropServices;

namespace Microsoft.Office.Story.V1.GraphicsInterop.Wic
{
	[Guid("B84E2C09-78C9-4AC4-8BD3-524AE1663A2F")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IWICFastMetadataEncoder
	{
		void Commit();

		void GetMetadataQueryWriter([MarshalAs(UnmanagedType.Interface)] out IWICMetadataQueryWriter ppIMetadataQueryWriter);
	}
}
