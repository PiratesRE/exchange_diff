using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.Exchange.Data.Transport.Interop
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("0597316F-7D23-3B38-9E6B-7B26F0867152")]
	[ComVisible(true)]
	public interface IProxyCallback
	{
		void AsyncCompletion();

		void SetWriteStream([MarshalAs(UnmanagedType.Interface)] IStream writeStream);

		void PutProperty(int id, byte[] value);

		void GetProperty(int id, out byte[] value);
	}
}
