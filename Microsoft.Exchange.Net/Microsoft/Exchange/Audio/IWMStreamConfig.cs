using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Audio
{
	[Guid("96406BDC-2B2B-11d3-B36B-00C04F6108FF")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	internal interface IWMStreamConfig
	{
		void GetStreamType();

		void GetStreamNumber();

		void SetStreamNumber();

		void GetStreamName();

		void SetStreamName();

		void GetConnectionName();

		void SetConnectionName();

		void GetBitrate();

		void SetBitrate([In] uint pdwBitrate);

		void GetBufferWindow();

		void SetBufferWindow();
	}
}
