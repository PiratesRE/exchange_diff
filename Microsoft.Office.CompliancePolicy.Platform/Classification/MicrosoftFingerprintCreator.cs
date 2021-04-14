using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Office.CompliancePolicy.Classification
{
	[Guid("7D3549FB-E229-4AE0-A0C6-C381EF9F5858")]
	[TypeLibType(TypeLibTypeFlags.FCanCreate)]
	[ClassInterface(ClassInterfaceType.None)]
	[ComImport]
	public class MicrosoftFingerprintCreator : IMicrosoftFingerprintCreator, IFingerprintCreator
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public virtual extern uint GetBase64EncodedFingerprint([MarshalAs(UnmanagedType.BStr)] [In] string strText, [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)] out byte[] fingerprint);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern MicrosoftFingerprintCreator();
	}
}
