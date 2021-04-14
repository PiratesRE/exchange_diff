using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Text
{
	internal class Normalization
	{
		[SecurityCritical]
		private unsafe static void InitializeForm(NormalizationForm form, string strDataFile)
		{
			byte* ptr = null;
			if (!Environment.IsWindows8OrAbove)
			{
				if (strDataFile == null)
				{
					throw new ArgumentException(Environment.GetResourceString("Argument_InvalidNormalizationForm"));
				}
				ptr = GlobalizationAssembly.GetGlobalizationResourceBytePtr(typeof(Normalization).Assembly, strDataFile);
				if (ptr == null)
				{
					throw new ArgumentException(Environment.GetResourceString("Argument_InvalidNormalizationForm"));
				}
			}
			Normalization.nativeNormalizationInitNormalization(form, ptr);
		}

		[SecurityCritical]
		private static void EnsureInitialized(NormalizationForm form)
		{
			if (form <= (NormalizationForm)13)
			{
				switch (form)
				{
				case NormalizationForm.FormC:
					if (Normalization.NFC)
					{
						return;
					}
					Normalization.InitializeForm(form, "normnfc.nlp");
					Normalization.NFC = true;
					return;
				case NormalizationForm.FormD:
					if (Normalization.NFD)
					{
						return;
					}
					Normalization.InitializeForm(form, "normnfd.nlp");
					Normalization.NFD = true;
					return;
				case (NormalizationForm)3:
				case (NormalizationForm)4:
					break;
				case NormalizationForm.FormKC:
					if (Normalization.NFKC)
					{
						return;
					}
					Normalization.InitializeForm(form, "normnfkc.nlp");
					Normalization.NFKC = true;
					return;
				case NormalizationForm.FormKD:
					if (Normalization.NFKD)
					{
						return;
					}
					Normalization.InitializeForm(form, "normnfkd.nlp");
					Normalization.NFKD = true;
					return;
				default:
					if (form == (NormalizationForm)13)
					{
						if (Normalization.IDNA)
						{
							return;
						}
						Normalization.InitializeForm(form, "normidna.nlp");
						Normalization.IDNA = true;
						return;
					}
					break;
				}
			}
			else
			{
				switch (form)
				{
				case (NormalizationForm)257:
					if (Normalization.NFCDisallowUnassigned)
					{
						return;
					}
					Normalization.InitializeForm(form, "normnfc.nlp");
					Normalization.NFCDisallowUnassigned = true;
					return;
				case (NormalizationForm)258:
					if (Normalization.NFDDisallowUnassigned)
					{
						return;
					}
					Normalization.InitializeForm(form, "normnfd.nlp");
					Normalization.NFDDisallowUnassigned = true;
					return;
				case (NormalizationForm)259:
				case (NormalizationForm)260:
					break;
				case (NormalizationForm)261:
					if (Normalization.NFKCDisallowUnassigned)
					{
						return;
					}
					Normalization.InitializeForm(form, "normnfkc.nlp");
					Normalization.NFKCDisallowUnassigned = true;
					return;
				case (NormalizationForm)262:
					if (Normalization.NFKDDisallowUnassigned)
					{
						return;
					}
					Normalization.InitializeForm(form, "normnfkd.nlp");
					Normalization.NFKDDisallowUnassigned = true;
					return;
				default:
					if (form == (NormalizationForm)269)
					{
						if (Normalization.IDNADisallowUnassigned)
						{
							return;
						}
						Normalization.InitializeForm(form, "normidna.nlp");
						Normalization.IDNADisallowUnassigned = true;
						return;
					}
					break;
				}
			}
			if (Normalization.Other)
			{
				return;
			}
			Normalization.InitializeForm(form, null);
			Normalization.Other = true;
		}

		[SecurityCritical]
		internal static bool IsNormalized(string strInput, NormalizationForm normForm)
		{
			Normalization.EnsureInitialized(normForm);
			int num = 0;
			bool result = Normalization.nativeNormalizationIsNormalizedString(normForm, ref num, strInput, strInput.Length);
			if (num <= 8)
			{
				if (num == 0)
				{
					return result;
				}
				if (num == 8)
				{
					throw new OutOfMemoryException(Environment.GetResourceString("Arg_OutOfMemoryException"));
				}
			}
			else if (num == 87 || num == 1113)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_InvalidCharSequenceNoIndex"), "strInput");
			}
			throw new InvalidOperationException(Environment.GetResourceString("UnknownError_Num", new object[]
			{
				num
			}));
		}

		[SecurityCritical]
		internal static string Normalize(string strInput, NormalizationForm normForm)
		{
			Normalization.EnsureInitialized(normForm);
			int num = 0;
			int num2 = Normalization.nativeNormalizationNormalizeString(normForm, ref num, strInput, strInput.Length, null, 0);
			if (num != 0)
			{
				if (num == 87)
				{
					throw new ArgumentException(Environment.GetResourceString("Argument_InvalidCharSequenceNoIndex"), "strInput");
				}
				if (num == 8)
				{
					throw new OutOfMemoryException(Environment.GetResourceString("Arg_OutOfMemoryException"));
				}
				throw new InvalidOperationException(Environment.GetResourceString("UnknownError_Num", new object[]
				{
					num
				}));
			}
			else
			{
				if (num2 == 0)
				{
					return string.Empty;
				}
				char[] array;
				for (;;)
				{
					array = new char[num2];
					num2 = Normalization.nativeNormalizationNormalizeString(normForm, ref num, strInput, strInput.Length, array, array.Length);
					if (num == 0)
					{
						goto IL_103;
					}
					if (num <= 87)
					{
						break;
					}
					if (num != 122)
					{
						goto Block_9;
					}
				}
				if (num == 8)
				{
					throw new OutOfMemoryException(Environment.GetResourceString("Arg_OutOfMemoryException"));
				}
				if (num != 87)
				{
					goto IL_E4;
				}
				goto IL_B0;
				Block_9:
				if (num != 1113)
				{
					goto IL_E4;
				}
				IL_B0:
				throw new ArgumentException(Environment.GetResourceString("Argument_InvalidCharSequence", new object[]
				{
					num2
				}), "strInput");
				IL_E4:
				throw new InvalidOperationException(Environment.GetResourceString("UnknownError_Num", new object[]
				{
					num
				}));
				IL_103:
				return new string(array, 0, num2);
			}
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int nativeNormalizationNormalizeString(NormalizationForm normForm, ref int iError, string lpSrcString, int cwSrcLength, char[] lpDstString, int cwDstLength);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool nativeNormalizationIsNormalizedString(NormalizationForm normForm, ref int iError, string lpString, int cwLength);

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private unsafe static extern void nativeNormalizationInitNormalization(NormalizationForm normForm, byte* pTableData);

		private static volatile bool NFC;

		private static volatile bool NFD;

		private static volatile bool NFKC;

		private static volatile bool NFKD;

		private static volatile bool IDNA;

		private static volatile bool NFCDisallowUnassigned;

		private static volatile bool NFDDisallowUnassigned;

		private static volatile bool NFKCDisallowUnassigned;

		private static volatile bool NFKDDisallowUnassigned;

		private static volatile bool IDNADisallowUnassigned;

		private static volatile bool Other;

		private const int ERROR_SUCCESS = 0;

		private const int ERROR_NOT_ENOUGH_MEMORY = 8;

		private const int ERROR_INVALID_PARAMETER = 87;

		private const int ERROR_INSUFFICIENT_BUFFER = 122;

		private const int ERROR_NO_UNICODE_TRANSLATION = 1113;
	}
}
