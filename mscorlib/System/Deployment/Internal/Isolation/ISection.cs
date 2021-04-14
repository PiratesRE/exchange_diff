using System;
using System.Runtime.InteropServices;

namespace System.Deployment.Internal.Isolation
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("285a8862-c84a-11d7-850f-005cd062464f")]
	[ComImport]
	internal interface ISection
	{
		object _NewEnum { [return: MarshalAs(UnmanagedType.Interface)] get; }

		uint Count { get; }

		uint SectionID { get; }

		string SectionName { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
	}
}
