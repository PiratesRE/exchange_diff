using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("C460E2B4-E199-412a-8456-84DC3E4838C3")]
	[ComVisible(true)]
	public interface IObjectHandle
	{
		object Unwrap();
	}
}
