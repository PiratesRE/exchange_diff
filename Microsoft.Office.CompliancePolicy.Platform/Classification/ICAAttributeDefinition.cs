using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Office.CompliancePolicy.Classification
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[TypeLibType(TypeLibTypeFlags.FNonExtensible)]
	[Guid("5F3FC246-F5B7-4997-B1AB-8125F923679C")]
	[ComImport]
	public interface ICAAttributeDefinition
	{
		string ID { [MethodImpl(MethodImplOptions.InternalCall)] [return: MarshalAs(UnmanagedType.BStr)] get; }

		ushort Type { [MethodImpl(MethodImplOptions.InternalCall)] get; }

		string Name { [MethodImpl(MethodImplOptions.InternalCall)] [return: MarshalAs(UnmanagedType.BStr)] get; }
	}
}
