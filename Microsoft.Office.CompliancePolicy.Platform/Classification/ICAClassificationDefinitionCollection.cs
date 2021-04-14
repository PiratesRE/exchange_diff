using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Office.CompliancePolicy.Classification
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("F3E2330F-E898-46B6-BE7A-F61457B90A6F")]
	[TypeLibType(TypeLibTypeFlags.FNonExtensible)]
	[ComImport]
	public interface ICAClassificationDefinitionCollection
	{
		int Count { [MethodImpl(MethodImplOptions.InternalCall)] get; }

		ICAClassificationDefinition this[object index]
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[return: MarshalAs(UnmanagedType.Interface)]
			get;
		}

		object _NewEnum { [MethodImpl(MethodImplOptions.InternalCall)] [return: MarshalAs(UnmanagedType.IUnknown)] get; }
	}
}
