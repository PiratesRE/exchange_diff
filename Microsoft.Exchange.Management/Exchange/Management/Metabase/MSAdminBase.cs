using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Management.Metabase
{
	[ClassInterface(ClassInterfaceType.None)]
	[Guid("a9e69610-b80d-11d0-b9b9-00a0c922e750")]
	[TypeLibType(TypeLibTypeFlags.FCanCreate)]
	[ComImport]
	internal class MSAdminBase
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern MSAdminBase();
	}
}
