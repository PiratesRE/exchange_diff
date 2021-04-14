using System;

namespace System.Runtime.InteropServices.ComTypes
{
	[Guid("00000010-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[__DynamicallyInvokable]
	[ComImport]
	public interface IRunningObjectTable
	{
		[__DynamicallyInvokable]
		int Register(int grfFlags, [MarshalAs(UnmanagedType.Interface)] object punkObject, IMoniker pmkObjectName);

		[__DynamicallyInvokable]
		void Revoke(int dwRegister);

		[__DynamicallyInvokable]
		[PreserveSig]
		int IsRunning(IMoniker pmkObjectName);

		[__DynamicallyInvokable]
		[PreserveSig]
		int GetObject(IMoniker pmkObjectName, [MarshalAs(UnmanagedType.Interface)] out object ppunkObject);

		[__DynamicallyInvokable]
		void NoteChangeTime(int dwRegister, ref FILETIME pfiletime);

		[__DynamicallyInvokable]
		[PreserveSig]
		int GetTimeOfLastChange(IMoniker pmkObjectName, out FILETIME pfiletime);

		[__DynamicallyInvokable]
		void EnumRunning(out IEnumMoniker ppenumMoniker);
	}
}
