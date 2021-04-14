using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Office.CompliancePolicy.Classification
{
	[TypeLibType(TypeLibTypeFlags.FNonExtensible)]
	[Guid("C8012AE8-E7A7-4158-ABFC-14F5582CE22B")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	public interface ICAAttributeDefinitionCollection
	{
		int Count { [MethodImpl(MethodImplOptions.InternalCall)] get; }

		ICAAttributeDefinition this[int nIndex]
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[return: MarshalAs(UnmanagedType.Interface)]
			get;
		}

		object _NewEnum { [MethodImpl(MethodImplOptions.InternalCall)] [return: MarshalAs(UnmanagedType.IUnknown)] get; }
	}
}
