using System;
using System.Runtime.InteropServices;

namespace System.Deployment.Internal.Isolation
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("285a8860-c84a-11d7-850f-005cd062464f")]
	[ComImport]
	internal interface ICDF
	{
		ISection GetRootSection(uint SectionId);

		ISectionEntry GetRootSectionEntry(uint SectionId);

		object _NewEnum { [return: MarshalAs(UnmanagedType.Interface)] get; }

		uint Count { get; }

		object GetItem(uint SectionId);
	}
}
