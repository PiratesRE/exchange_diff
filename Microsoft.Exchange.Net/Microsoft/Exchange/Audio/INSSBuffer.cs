using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Audio
{
	[Guid("E1CD3524-03D7-11d2-9EED-006097D2D7CF")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	internal interface INSSBuffer
	{
		void GetLength(out uint pdwLength);

		void SetLength([In] uint dwLength);

		void GetMaxLength(out uint pdwLength);

		void GetBuffer(out IntPtr ppdwBuffer);

		void GetBufferAndLength(out IntPtr ppdwBuffer, out uint pdwLength);
	}
}
