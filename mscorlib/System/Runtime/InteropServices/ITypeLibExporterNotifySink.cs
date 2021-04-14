using System;
using System.Reflection;

namespace System.Runtime.InteropServices
{
	[Guid("F1C3BF77-C3E4-11d3-88E7-00902754C43A")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComVisible(true)]
	public interface ITypeLibExporterNotifySink
	{
		void ReportEvent(ExporterEventKind eventKind, int eventCode, string eventMsg);

		[return: MarshalAs(UnmanagedType.Interface)]
		object ResolveRef(Assembly assembly);
	}
}
