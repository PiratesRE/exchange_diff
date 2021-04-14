using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Common.Interop
{
	[Guid("275C23E2-3747-11D0-9FEA-00AA003F8646")]
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[ComImport]
	internal class CMultiLanguage
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern CMultiLanguage();
	}
}
