using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Common.Interop
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[InterfaceType(1)]
	[CoClass(typeof(CMultiLanguage))]
	[Guid("DCCFC164-2B38-11D2-B7EC-00C04F8F5D9A")]
	[ComImport]
	internal interface IMultiLanguage2
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		void GetNumberOfCodePageInfo(out uint pcCodePage);

		[MethodImpl(MethodImplOptions.InternalCall)]
		void GetCodePageInfo();

		[MethodImpl(MethodImplOptions.InternalCall)]
		void GetFamilyCodePage([In] uint uiCodePage, out uint puiFamilyCodePage);

		[MethodImpl(MethodImplOptions.InternalCall)]
		void EnumCodePages();

		[MethodImpl(MethodImplOptions.InternalCall)]
		void GetCharsetInfo();

		[MethodImpl(MethodImplOptions.InternalCall)]
		void IsConvertible([In] uint dwSrcEncoding, [In] uint dwDstEncoding);

		[MethodImpl(MethodImplOptions.InternalCall)]
		void ConvertString([In] [Out] ref uint pdwMode, [In] uint dwSrcEncoding, [In] uint dwDstEncoding, [In] ref byte pSrcStr, [In] [Out] ref uint pcSrcSize, [In] ref byte pDstStr, [In] [Out] ref uint pcDstSize);

		[MethodImpl(MethodImplOptions.InternalCall)]
		void ConvertStringToUnicode([In] [Out] ref uint pdwMode, [In] uint dwEncoding, [In] ref sbyte pSrcStr, [In] [Out] ref uint pcSrcSize, [In] ref ushort pDstStr, [In] [Out] ref uint pcDstSize);

		[MethodImpl(MethodImplOptions.InternalCall)]
		void ConvertStringFromUnicode([In] [Out] ref uint pdwMode, [In] uint dwEncoding, [In] ref ushort pSrcStr, [In] [Out] ref uint pcSrcSize, [In] ref sbyte pDstStr, [In] [Out] ref uint pcDstSize);

		[MethodImpl(MethodImplOptions.InternalCall)]
		void ConvertStringReset();

		[MethodImpl(MethodImplOptions.InternalCall)]
		void GetRfc1766FromLcid([In] uint locale, [MarshalAs(UnmanagedType.BStr)] out string pbstrRfc1766);

		[MethodImpl(MethodImplOptions.InternalCall)]
		void GetLcidFromRfc1766(out uint plocale, [MarshalAs(UnmanagedType.BStr)] [In] string bstrRfc1766);

		[MethodImpl(MethodImplOptions.InternalCall)]
		void EnumRfc1766();

		[MethodImpl(MethodImplOptions.InternalCall)]
		void GetRfc1766Info();

		[MethodImpl(MethodImplOptions.InternalCall)]
		void CreateConvertCharset();

		[MethodImpl(MethodImplOptions.InternalCall)]
		void ConvertStringInIStream();

		[MethodImpl(MethodImplOptions.InternalCall)]
		void ConvertStringToUnicodeEx([In] [Out] ref uint pdwMode, [In] uint dwEncoding, [In] ref sbyte pSrcStr, [In] [Out] ref uint pcSrcSize, [In] ref ushort pDstStr, [In] [Out] ref uint pcDstSize, [In] uint dwFlag, [In] ref ushort lpFallBack);

		[MethodImpl(MethodImplOptions.InternalCall)]
		void ConvertStringFromUnicodeEx([In] [Out] ref uint pdwMode, [In] uint dwEncoding, [In] ref ushort pSrcStr, [In] [Out] ref uint pcSrcSize, [In] ref sbyte pDstStr, [In] [Out] ref uint pcDstSize, [In] uint dwFlag, [In] ref ushort lpFallBack);

		[MethodImpl(MethodImplOptions.InternalCall)]
		void DetectCodepageInIStream();

		[MethodImpl(MethodImplOptions.InternalCall)]
		void DetectInputCodepage([In] MLDETECTCP flags, [In] uint dwPrefWinCodePage, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr, SizeParamIndex = 1)] [In] byte[] pSrcStr, [In] [Out] ref int pcSrcSize, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr, SizeParamIndex = 1)] [In] [Out] DetectEncodingInfo[] lpEncoding, [In] [Out] ref int pnScores);

		[MethodImpl(MethodImplOptions.InternalCall)]
		void ValidateCodePage();

		[MethodImpl(MethodImplOptions.InternalCall)]
		void GetCodePageDescription([In] uint uiCodePage, [In] uint lcid, [MarshalAs(UnmanagedType.LPWStr)] [In] [Out] string lpWideCharStr, [In] int cchWideChar);

		[MethodImpl(MethodImplOptions.InternalCall)]
		void IsCodePageInstallable();

		[MethodImpl(MethodImplOptions.InternalCall)]
		void SetMimeDBSource();

		[MethodImpl(MethodImplOptions.InternalCall)]
		void GetNumberOfScripts(out uint pnScripts);

		[MethodImpl(MethodImplOptions.InternalCall)]
		void EnumScripts();

		[MethodImpl(MethodImplOptions.InternalCall)]
		void ValidateCodePageEx();
	}
}
