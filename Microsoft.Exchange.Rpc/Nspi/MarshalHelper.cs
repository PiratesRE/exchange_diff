using System;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.RpcClientAccess;

namespace Microsoft.Exchange.Rpc.Nspi
{
	internal class MarshalHelper
	{
		public static int GetString8CodePage(NspiState state)
		{
			int result = 0;
			if (state != null)
			{
				result = state.CodePage;
			}
			return result;
		}

		public static Encoding GetString8Encoding(NspiState state)
		{
			int codePage = 0;
			if (state != null)
			{
				codePage = state.CodePage;
			}
			return MarshalHelper.GetString8Encoding(codePage);
		}

		public static Encoding GetString8Encoding(int codePage)
		{
			Encoding result = null;
			codePage = ((codePage == 0) ? 1252 : codePage);
			if (!String8Encodings.TryGetEncoding(codePage, out result))
			{
				throw new FailRpcException(string.Format("Invalid code page; codePage={0}", codePage), -2147221218);
			}
			return result;
		}

		public unsafe static SafeRpcMemoryHandle ConvertPropertyTagArrayToSPropTagArray(PropertyTag[] propertyTags, [MarshalAs(UnmanagedType.U1)] bool allowEmptyArray)
		{
			SafeRpcMemoryHandle safeRpcMemoryHandle = null;
			bool flag = false;
			SafeRpcMemoryHandle result;
			try
			{
				safeRpcMemoryHandle = new SafeRpcMemoryHandle();
				if (propertyTags != null)
				{
					int num = propertyTags.Length;
					if (num > 0 || allowEmptyArray)
					{
						safeRpcMemoryHandle.Allocate((ulong)(((long)num + 2L) * 4L));
						_SPropTagArray_r* ptr = (_SPropTagArray_r*)safeRpcMemoryHandle.DangerousGetHandle().ToPointer();
						*(int*)ptr = num;
						for (int i = 0; i < num; i++)
						{
							*(int*)(((long)i + 1L / 4L) * 4L + ptr / 4) = (int)propertyTags[i];
						}
					}
				}
				flag = true;
				result = safeRpcMemoryHandle;
			}
			finally
			{
				if (!flag && safeRpcMemoryHandle != null)
				{
					((IDisposable)safeRpcMemoryHandle).Dispose();
				}
			}
			return result;
		}

		public static SafeRpcMemoryHandle ConvertPropertyTagArrayToSPropTagArray(PropertyTag[] propertyTags)
		{
			return MarshalHelper.ConvertPropertyTagArrayToSPropTagArray(propertyTags, false);
		}

		public unsafe static PropertyTag[] ConvertSPropTagArrayToPropertyTagArray(IntPtr pPropTags)
		{
			PropertyTag[] array = null;
			if (pPropTags != IntPtr.Zero)
			{
				_SPropTagArray_r* ptr = (_SPropTagArray_r*)pPropTags.ToPointer();
				int num = *(int*)ptr;
				if (num < 0)
				{
					throw new FailRpcException(string.Format("SPropTagArray length cannot be negative; length={0}", num), -2147467259);
				}
				array = new PropertyTag[num];
				int num2 = 0;
				long num3 = (long)num;
				if (0L < num3)
				{
					_SPropTagArray_r* ptr2 = ptr + 4L / (long)sizeof(_SPropTagArray_r);
					ulong num4 = (ulong)num3;
					do
					{
						PropertyTag propertyTag = new PropertyTag((uint)(*(int*)ptr2));
						array[num2] = propertyTag;
						num2++;
						ptr2 += 4L / (long)sizeof(_SPropTagArray_r);
						num4 -= 1UL;
					}
					while (num4 > 0UL);
				}
			}
			return array;
		}

		public unsafe static int[] ConvertSPropTagArrayToIntArray(IntPtr pPropTagArray)
		{
			int[] array = null;
			if (pPropTagArray != IntPtr.Zero)
			{
				_SPropTagArray_r* ptr = (_SPropTagArray_r*)pPropTagArray.ToPointer();
				int num = *(int*)ptr;
				if (num < 0)
				{
					throw new FailRpcException(string.Format("SPropTagArray length cannot be negative; length={0}", num), -2147467259);
				}
				array = new int[num];
				if (num > 0)
				{
					IntPtr source = new IntPtr((void*)(ptr + 4L / (long)sizeof(_SPropTagArray_r)));
					Marshal.Copy(source, array, 0, num);
				}
			}
			return array;
		}

		public unsafe static String8 ConvertPtString8ToString8(IntPtr pString8, int codePage)
		{
			if (pString8 == IntPtr.Zero)
			{
				return null;
			}
			sbyte* ptr = (sbyte*)pString8.ToPointer();
			sbyte* ptr2 = ptr;
			if (*(sbyte*)ptr != 0)
			{
				do
				{
					ptr2 += 1L / (long)sizeof(sbyte);
				}
				while (*(sbyte*)ptr2 != 0);
			}
			int num = (int)(ptr2 - ptr);
			byte[] array = new byte[num];
			if (array.Length > 0)
			{
				Marshal.Copy(pString8, array, 0, num);
			}
			ArraySegment<byte> encodedBytes = new ArraySegment<byte>(array);
			String8 @string = new String8(encodedBytes);
			@string.ResolveString8Values(MarshalHelper.GetString8Encoding(codePage));
			return @string;
		}

		public unsafe static PropertyValue ConvertSPropValueToPropertyValue(IntPtr pPropValue, int codePage)
		{
			ExDateTime[] array = null;
			PropertyValue result;
			try
			{
				_SPropValue_r* ptr = (_SPropValue_r*)pPropValue.ToPointer();
				PropertyTag propertyTag = new PropertyTag((uint)(*(int*)ptr));
				PropertyTag propertyTag2 = propertyTag;
				object obj = null;
				int propertyType = (int)propertyTag2.PropertyType;
				if (propertyType <= 258)
				{
					if (propertyType != 258)
					{
						switch (propertyType + -1)
						{
						case 0:
						case 12:
							goto IL_79A;
						case 1:
							obj = *(short*)(ptr + 8L / (long)sizeof(_SPropValue_r));
							goto IL_79A;
						case 2:
							obj = *(int*)(ptr + 8L / (long)sizeof(_SPropValue_r));
							goto IL_79A;
						case 3:
						case 4:
						case 5:
						case 6:
						case 7:
						case 8:
						case 11:
						case 13:
						case 14:
						case 15:
						case 16:
						case 17:
						case 18:
						case 19:
						case 20:
						case 21:
						case 22:
						case 23:
						case 24:
						case 25:
						case 26:
						case 27:
						case 28:
						case 31:
						case 32:
						case 33:
						case 34:
						case 35:
						case 36:
						case 37:
						case 38:
						case 39:
						case 40:
						case 41:
						case 42:
						case 43:
						case 44:
						case 45:
						case 46:
						case 47:
						case 48:
						case 49:
						case 50:
						case 51:
						case 52:
						case 53:
						case 54:
						case 55:
						case 56:
						case 57:
						case 58:
						case 59:
						case 60:
						case 61:
						case 62:
						case 64:
						case 65:
						case 66:
						case 67:
						case 68:
						case 69:
						case 70:
							goto IL_553;
						case 9:
							obj = (ErrorCode)(*(int*)(ptr + 8L / (long)sizeof(_SPropValue_r)));
							goto IL_79A;
						case 10:
							break;
						case 29:
						{
							long num = *(long*)(ptr + 8L / (long)sizeof(_SPropValue_r));
							if (num != 0L)
							{
								IntPtr pString = new IntPtr(num);
								obj = MarshalHelper.ConvertPtString8ToString8(pString, codePage);
								goto IL_79A;
							}
							goto IL_79A;
						}
						case 30:
						{
							long num2 = *(long*)(ptr + 8L / (long)sizeof(_SPropValue_r));
							if (num2 != 0L)
							{
								IntPtr ptr2 = new IntPtr(num2);
								obj = Marshal.PtrToStringUni(ptr2);
								goto IL_79A;
							}
							goto IL_79A;
						}
						case 63:
							try
							{
								obj = ExDateTime.FromFileTimeUtc(*(long*)(ptr + 8L / (long)sizeof(_SPropValue_r)));
								goto IL_79A;
							}
							catch (ArgumentOutOfRangeException)
							{
								obj = ExDateTime.MaxValue;
								goto IL_79A;
							}
							break;
						case 71:
						{
							long num3 = *(long*)(ptr + 8L / (long)sizeof(_SPropValue_r));
							if (num3 != 0L)
							{
								IntPtr ptr3 = new IntPtr(num3);
								obj = (Guid)Marshal.PtrToStructure(ptr3, typeof(Guid));
								goto IL_79A;
							}
							goto IL_79A;
						}
						default:
							goto IL_553;
						}
						byte b = (*(ushort*)(ptr + 8L / (long)sizeof(_SPropValue_r)) != 0) ? 1 : 0;
						obj = (b != 0);
						goto IL_79A;
					}
					int num4 = *(int*)(ptr + 8L / (long)sizeof(_SPropValue_r));
					if (num4 < 0)
					{
						throw new FailRpcException(string.Format("Property {0} array length cannot be negative; length={1}", propertyTag2, num4), -2147467259);
					}
					if (num4 == 0)
					{
						obj = new byte[0];
						goto IL_79A;
					}
					ulong num5 = (ulong)(*(long*)(ptr + 16L / (long)sizeof(_SPropValue_r)));
					if (num5 != 0UL)
					{
						obj = new byte[num4];
						IntPtr source = new IntPtr(num5);
						Marshal.Copy(source, (byte[])obj, 0, num4);
						goto IL_79A;
					}
					goto IL_79A;
				}
				else if (propertyType <= 4127)
				{
					if (propertyType != 4127)
					{
						if (propertyType != 4098)
						{
							if (propertyType != 4099)
							{
								if (propertyType == 4126)
								{
									int num6 = *(int*)(ptr + 8L / (long)sizeof(_SPropValue_r));
									if (num6 < 0)
									{
										throw new FailRpcException(string.Format("Property {0} array length cannot be negative; len={1}", propertyTag2, num6), -2147467259);
									}
									if (num6 == 0)
									{
										obj = new String8[0];
										goto IL_79A;
									}
									ulong num5 = (ulong)(*(long*)(ptr + 16L / (long)sizeof(_SPropValue_r)));
									if (num5 != 0UL)
									{
										String8[] array2 = new String8[num6];
										void** ptr4 = num5;
										for (int i = 0; i < num6; i++)
										{
											ulong num7 = (ulong)(*(long*)((long)i * 8L + ptr4 / 8));
											if (num7 != 0UL)
											{
												IntPtr pString2 = new IntPtr(num7);
												array2[i] = MarshalHelper.ConvertPtString8ToString8(pString2, codePage);
											}
											else
											{
												array2[i] = null;
											}
										}
										obj = array2;
										goto IL_79A;
									}
									goto IL_79A;
								}
							}
							else
							{
								int num8 = *(int*)(ptr + 8L / (long)sizeof(_SPropValue_r));
								if (num8 < 0)
								{
									throw new FailRpcException(string.Format("Property {0} array length cannot be negative; len={1}", propertyTag2, num8), -2147467259);
								}
								if (num8 == 0)
								{
									obj = new int[0];
									goto IL_79A;
								}
								ulong num5 = (ulong)(*(long*)(ptr + 16L / (long)sizeof(_SPropValue_r)));
								if (num5 != 0UL)
								{
									obj = new int[num8];
									IntPtr source2 = new IntPtr(num5);
									Marshal.Copy(source2, (int[])obj, 0, num8);
									goto IL_79A;
								}
								goto IL_79A;
							}
						}
						else
						{
							int num9 = *(int*)(ptr + 8L / (long)sizeof(_SPropValue_r));
							if (num9 < 0)
							{
								throw new FailRpcException(string.Format("Property {0} array length cannot be negative; length={1}", propertyTag2, num9), -2147467259);
							}
							if (num9 == 0)
							{
								obj = new short[0];
								goto IL_79A;
							}
							ulong num5 = (ulong)(*(long*)(ptr + 16L / (long)sizeof(_SPropValue_r)));
							if (num5 != 0UL)
							{
								obj = new short[num9];
								IntPtr source3 = new IntPtr(num5);
								Marshal.Copy(source3, (short[])obj, 0, num9);
								goto IL_79A;
							}
							goto IL_79A;
						}
					}
					else
					{
						int num10 = *(int*)(ptr + 8L / (long)sizeof(_SPropValue_r));
						if (num10 < 0)
						{
							throw new FailRpcException(string.Format("Property {0} array length cannot be negative; len={1}", propertyTag2, num10), -2147467259);
						}
						if (num10 == 0)
						{
							obj = new string[0];
							goto IL_79A;
						}
						ulong num5 = (ulong)(*(long*)(ptr + 16L / (long)sizeof(_SPropValue_r)));
						if (num5 != 0UL)
						{
							string[] array3 = new string[num10];
							void** ptr5 = num5;
							for (int j = 0; j < num10; j++)
							{
								ulong num11 = (ulong)(*(long*)((long)j * 8L + ptr5 / 8));
								if (num11 != 0UL)
								{
									IntPtr ptr6 = new IntPtr(num11);
									array3[j] = Marshal.PtrToStringUni(ptr6);
								}
								else
								{
									array3[j] = null;
								}
							}
							obj = array3;
							goto IL_79A;
						}
						goto IL_79A;
					}
				}
				else if (propertyType != 4160)
				{
					if (propertyType != 4168)
					{
						if (propertyType == 4354)
						{
							int num12 = *(int*)(ptr + 8L / (long)sizeof(_SPropValue_r));
							if (num12 < 0)
							{
								throw new FailRpcException(string.Format("Property {0} array length cannot be negative; len={1}", propertyTag2, num12), -2147467259);
							}
							if (num12 == 0)
							{
								obj = new byte[0][];
								goto IL_79A;
							}
							ulong num5 = (ulong)(*(long*)(ptr + 16L / (long)sizeof(_SPropValue_r)));
							if (num5 != 0UL)
							{
								byte[][] array4 = new byte[num12][];
								_SBinary_r* ptr7 = num5;
								for (int k = 0; k < num12; k++)
								{
									_SBinary_r* ptr8 = (long)k * 16L + ptr7 / sizeof(_SBinary_r);
									if (*(long*)(ptr8 + 8L / (long)sizeof(_SBinary_r)) != 0L)
									{
										array4[k] = new byte[*(int*)ptr8];
										if (*(int*)ptr8 != 0)
										{
											IntPtr source4 = new IntPtr(*(long*)(ptr8 + 8L / (long)sizeof(_SBinary_r)));
											Marshal.Copy(source4, array4[k], 0, *(int*)ptr8);
										}
									}
									else
									{
										array4[k] = null;
									}
								}
								obj = array4;
								goto IL_79A;
							}
							goto IL_79A;
						}
					}
					else
					{
						int num13 = *(int*)(ptr + 8L / (long)sizeof(_SPropValue_r));
						if (num13 < 0)
						{
							throw new FailRpcException(string.Format("Property {0} array length cannot be negative; len={1}", propertyTag2, num13), -2147467259);
						}
						if (num13 == 0)
						{
							obj = new Guid[0];
							goto IL_79A;
						}
						ulong num5 = (ulong)(*(long*)(ptr + 16L / (long)sizeof(_SPropValue_r)));
						if (num5 != 0UL)
						{
							Guid[] array5 = new Guid[num13];
							void** ptr9 = num5;
							for (int l = 0; l < num13; l++)
							{
								ulong num14 = (ulong)(*(long*)((long)l * 8L + ptr9 / 8));
								if (num14 != 0UL)
								{
									IntPtr ptr10 = new IntPtr(num14);
									array5[l] = (Guid)Marshal.PtrToStructure(ptr10, typeof(Guid));
								}
								else
								{
									array5[l] = Guid.Empty;
								}
							}
							obj = array5;
							goto IL_79A;
						}
						goto IL_79A;
					}
				}
				else
				{
					int num15 = *(int*)(ptr + 8L / (long)sizeof(_SPropValue_r));
					if (num15 < 0)
					{
						throw new FailRpcException(string.Format("Property {0} array length cannot be negative; len={1}", propertyTag2, num15), -2147467259);
					}
					if (num15 == 0)
					{
						obj = new ExDateTime[0];
						goto IL_79A;
					}
					ulong num5 = (ulong)(*(long*)(ptr + 16L / (long)sizeof(_SPropValue_r)));
					if (num5 != 0UL)
					{
						array = new ExDateTime[num15];
						long* ptr11 = num5;
						for (int m = 0; m < num15; m++)
						{
							try
							{
								ExDateTime exDateTime = ExDateTime.FromFileTimeUtc(((long)m * 8L)[ptr11 / 8]);
								array[m] = exDateTime;
							}
							catch (ArgumentOutOfRangeException)
							{
								array[m] = ExDateTime.MaxValue;
							}
						}
						obj = array;
						goto IL_79A;
					}
					goto IL_79A;
				}
				IL_553:
				throw new FailRpcException(string.Format("Unsupported property type {0} on property {1}.", propertyTag2.PropertyType, propertyTag2), -2147467259);
				IL_79A:
				if (obj == null)
				{
					result = PropertyValue.NullValue(propertyTag2);
				}
				else
				{
					PropertyValue propertyValue = new PropertyValue(propertyTag2, obj);
					result = propertyValue;
				}
			}
			catch (InvalidPropertyValueTypeException innerException)
			{
				throw new FailRpcException("Invalid PropertyValue detected from client.", -2147467259, innerException);
			}
			return result;
		}

		public unsafe static SafeRpcMemoryHandle ConvertPropertyValueToSPropValue(PropertyValue propertyValue, int codePage)
		{
			SafeRpcMemoryHandle safeRpcMemoryHandle = null;
			bool flag = false;
			SafeRpcMemoryHandle result;
			try
			{
				safeRpcMemoryHandle = new SafeRpcMemoryHandle();
				int paddedSizeOf_SPropValue_r = MarshalHelper.PaddedSizeOf_SPropValue_r;
				int num = MarshalHelper.GetBytesToMarshal(propertyValue) + paddedSizeOf_SPropValue_r;
				int num2 = num + 7 & -8;
				safeRpcMemoryHandle.Allocate(num2);
				byte* ptr = (byte*)safeRpcMemoryHandle.DangerousGetHandle().ToPointer();
				initblk(ptr, 0, (long)num2);
				IntPtr pData = new IntPtr((void*)((byte*)((long)paddedSizeOf_SPropValue_r) + ptr));
				IntPtr intPtr = MarshalHelper.MarshalToNative(propertyValue, (_SPropValue_r*)ptr, pData, codePage);
				flag = true;
				result = safeRpcMemoryHandle;
			}
			finally
			{
				if (!flag && safeRpcMemoryHandle != null)
				{
					((IDisposable)safeRpcMemoryHandle).Dispose();
				}
			}
			return result;
		}

		public unsafe static PropertyValue[] ConvertSRowToPropertyValueArray(IntPtr pRow, int codePage)
		{
			PropertyValue[] array = null;
			_SRow_r* ptr = (_SRow_r*)pRow.ToPointer();
			if (ptr != null)
			{
				ulong num = (ulong)(*(long*)(ptr + 8L / (long)sizeof(_SRow_r)));
				if (num != 0UL)
				{
					int num2 = *(int*)(ptr + 4L / (long)sizeof(_SRow_r));
					if (num2 < 0)
					{
						throw new FailRpcException(string.Format("SRow property count cannot be negative; length={0}", num2), -2147467259);
					}
					_SPropValue_r* ptr2 = num;
					array = new PropertyValue[num2];
					int num3 = 0;
					if (0 < num2)
					{
						_SPropValue_r* ptr3 = ptr2;
						do
						{
							PropertyValue propertyValue = MarshalHelper.ConvertSPropValueToPropertyValue((IntPtr)((void*)ptr3), codePage);
							array[num3] = propertyValue;
							num3++;
							ptr3 += 24L / (long)sizeof(_SPropValue_r);
						}
						while (num3 < num2);
					}
				}
			}
			return array;
		}

		public unsafe static SafeRpcMemoryHandle ConvertPropertyValueArrayToSRow(PropertyValue[] propertyValues, int codePage)
		{
			SafeRpcMemoryHandle safeRpcMemoryHandle = null;
			bool flag = false;
			SafeRpcMemoryHandle result;
			try
			{
				safeRpcMemoryHandle = new SafeRpcMemoryHandle();
				int paddedSizeOf_SRow_r = MarshalHelper.PaddedSizeOf_SRow_r;
				safeRpcMemoryHandle.Allocate(paddedSizeOf_SRow_r);
				byte* ptr = (byte*)safeRpcMemoryHandle.DangerousGetHandle().ToPointer();
				initblk(ptr, 0, (long)paddedSizeOf_SRow_r);
				MarshalHelper.MarshalToNative(safeRpcMemoryHandle, propertyValues, (_SRow_r*)ptr, codePage);
				flag = true;
				result = safeRpcMemoryHandle;
			}
			finally
			{
				if (!flag && safeRpcMemoryHandle != null)
				{
					((IDisposable)safeRpcMemoryHandle).Dispose();
				}
			}
			return result;
		}

		public unsafe static PropertyValue[][] ConvertSRowSetToPropertyValueArrays(IntPtr pRowSet, int codePage)
		{
			PropertyValue[][] array = null;
			_SRowSet_r* ptr = (_SRowSet_r*)pRowSet.ToPointer();
			if (ptr != null)
			{
				int num = *(int*)ptr;
				if (num < 0)
				{
					throw new FailRpcException(string.Format("SRowSet row count cannot be negative; length={0}", num), -2147467259);
				}
				_SRow_r* ptr2 = (_SRow_r*)(ptr + 8L / (long)sizeof(_SRowSet_r));
				array = new PropertyValue[num][];
				int num2 = 0;
				if (0 < num)
				{
					do
					{
						IntPtr pRow = new IntPtr((void*)ptr2);
						array[num2] = MarshalHelper.ConvertSRowToPropertyValueArray(pRow, codePage);
						ptr2 += 16L / (long)sizeof(_SRow_r);
						num2++;
					}
					while (num2 < num);
				}
			}
			return array;
		}

		public unsafe static SafeRpcMemoryHandle ConvertPropertyValueArraysToSRowSet(PropertyValue[][] propertyRows, int codePage)
		{
			SafeRpcMemoryHandle safeRpcMemoryHandle = null;
			bool flag = false;
			SafeRpcMemoryHandle result;
			try
			{
				safeRpcMemoryHandle = new SafeRpcMemoryHandle();
				int num;
				if (propertyRows != null)
				{
					num = propertyRows.Length;
				}
				else
				{
					num = 0;
				}
				int num2 = (int)((long)num * 16L + 24L) + 7 & -8;
				safeRpcMemoryHandle.Allocate(num2);
				byte* ptr = (byte*)safeRpcMemoryHandle.DangerousGetHandle().ToPointer();
				initblk(ptr, 0, (long)num2);
				MarshalHelper.MarshalToNative(safeRpcMemoryHandle, propertyRows, (_SRowSet_r*)ptr, codePage);
				flag = true;
				result = safeRpcMemoryHandle;
			}
			finally
			{
				if (!flag && safeRpcMemoryHandle != null)
				{
					((IDisposable)safeRpcMemoryHandle).Dispose();
				}
			}
			return result;
		}

		public unsafe static Restriction ConvertSRestrictionToRestriction(IntPtr pRest, int codePage)
		{
			if (pRest == IntPtr.Zero)
			{
				return null;
			}
			_SRestriction_r* ptr = (_SRestriction_r*)pRest.ToPointer();
			RestrictionType restrictionType = (RestrictionType)(*(int*)ptr);
			switch (restrictionType)
			{
			case RestrictionType.And:
				return new AndRestriction(MarshalHelper.ConvertSRestrictionArray(*(int*)(ptr + 8L / (long)sizeof(_SRestriction_r)), *(long*)(ptr + 8L / (long)sizeof(_SRestriction_r) + 8L / (long)sizeof(_SRestriction_r)), codePage));
			case RestrictionType.Or:
				return new OrRestriction(MarshalHelper.ConvertSRestrictionArray(*(int*)(ptr + 8L / (long)sizeof(_SRestriction_r)), *(long*)(ptr + 8L / (long)sizeof(_SRestriction_r) + 8L / (long)sizeof(_SRestriction_r)), codePage));
			case RestrictionType.Not:
				return MarshalHelper.ConvertSNotRestrictionToNotRestriction((_SNotRestriction_r*)(ptr + 8L / (long)sizeof(_SRestriction_r)), codePage);
			case RestrictionType.Content:
				return MarshalHelper.ConvertSContentRestrictionToContentRestriction((_SContentRestriction_r*)(ptr + 8L / (long)sizeof(_SRestriction_r)), codePage);
			case RestrictionType.Property:
				return MarshalHelper.ConvertSPropertyRestrictionToPropertyRestriction((_SPropertyRestriction_r*)(ptr + 8L / (long)sizeof(_SRestriction_r)), codePage);
			case RestrictionType.CompareProps:
				return MarshalHelper.ConvertSComparePropsRestrictionToComparePropsRestriction((_SComparePropsRestriction_r*)(ptr + 8L / (long)sizeof(_SRestriction_r)));
			case RestrictionType.BitMask:
			{
				PropertyTag propertyTag = new PropertyTag((uint)(*(int*)(ptr + 8L / (long)sizeof(_SRestriction_r) + 4L / (long)sizeof(_SRestriction_r))));
				return new BitMaskRestriction((BitMaskOperator)(*(int*)(ptr + 8L / (long)sizeof(_SRestriction_r))), propertyTag, (uint)(*(int*)(ptr + 8L / (long)sizeof(_SRestriction_r) + 8L / (long)sizeof(_SRestriction_r))));
			}
			case RestrictionType.Size:
			{
				PropertyTag propertyTag2 = new PropertyTag((uint)(*(int*)(ptr + 8L / (long)sizeof(_SRestriction_r) + 4L / (long)sizeof(_SRestriction_r))));
				return new SizeRestriction((RelationOperator)(*(int*)(ptr + 8L / (long)sizeof(_SRestriction_r))), propertyTag2, (uint)(*(int*)(ptr + 8L / (long)sizeof(_SRestriction_r) + 8L / (long)sizeof(_SRestriction_r))));
			}
			case RestrictionType.Exists:
			{
				PropertyTag propertyTag3 = new PropertyTag((uint)(*(int*)(ptr + 12L / (long)sizeof(_SRestriction_r))));
				return new ExistsRestriction(propertyTag3);
			}
			case RestrictionType.SubRestriction:
				return MarshalHelper.ConvertSSubRestrictionToSubRestriction((_SSubRestriction_r*)(ptr + 8L / (long)sizeof(_SRestriction_r)), codePage);
			default:
				throw new FailRpcException(string.Format("Unable to convert invalid restriction type: {0}", restrictionType), -2147467259);
			}
		}

		public unsafe static SafeRpcMemoryHandle ConvertRestrictionToSRestriction(Restriction restriction, int codePage)
		{
			SafeRpcMemoryHandle safeRpcMemoryHandle = null;
			bool flag = false;
			SafeRpcMemoryHandle result;
			try
			{
				safeRpcMemoryHandle = new SafeRpcMemoryHandle();
				int paddedSizeOf_SRestriction_r = MarshalHelper.PaddedSizeOf_SRestriction_r;
				int num = MarshalHelper.GetBytesToMarshal(restriction) + paddedSizeOf_SRestriction_r;
				int num2 = num + 7 & -8;
				safeRpcMemoryHandle.Allocate(num2);
				byte* ptr = (byte*)safeRpcMemoryHandle.DangerousGetHandle().ToPointer();
				initblk(ptr, 0, (long)num2);
				IntPtr pData = new IntPtr((void*)((byte*)((long)paddedSizeOf_SRestriction_r) + ptr));
				IntPtr intPtr = MarshalHelper.MarshalToNative(restriction, (_SRestriction_r*)ptr, pData, codePage);
				flag = true;
				result = safeRpcMemoryHandle;
			}
			finally
			{
				if (!flag && safeRpcMemoryHandle != null)
				{
					((IDisposable)safeRpcMemoryHandle).Dispose();
				}
			}
			return result;
		}

		public static string GetString8PropertyValue(PropertyValue propertyValue)
		{
			string result = null;
			if (propertyValue.CanGetValue<String8>())
			{
				String8 value = propertyValue.GetValue<String8>();
				if (value != null)
				{
					result = value.StringValue;
				}
			}
			else if (propertyValue.CanGetValue<string>())
			{
				result = propertyValue.GetValue<string>();
			}
			return result;
		}

		public static string[] GetMultiValuedString8PropertyValue(PropertyValue propertyValue)
		{
			string[] array = null;
			if (propertyValue.CanGetValue<String8[]>())
			{
				String8[] value = propertyValue.GetValue<String8[]>();
				if (value != null)
				{
					int num = value.Length;
					array = new string[num];
					int num2 = 0;
					if (0 < num)
					{
						do
						{
							String8 @string = value[num2];
							if (@string != null)
							{
								array[num2] = @string.StringValue;
							}
							else
							{
								array[num2] = null;
							}
							num2++;
						}
						while (num2 < value.Length);
					}
				}
			}
			else if (propertyValue.CanGetValue<string[]>())
			{
				array = propertyValue.GetValue<string[]>();
			}
			return array;
		}

		private static int PadSizeToQuadWordAlignment(int size)
		{
			return size + 7 & -8;
		}

		private static int GetBytesToMarshal(Restriction restriction)
		{
			if (restriction == null)
			{
				return 0;
			}
			switch (restriction.RestrictionType)
			{
			case RestrictionType.And:
				return MarshalHelper.GetBytesToMarshal((CompositeRestriction)restriction);
			case RestrictionType.Or:
				return MarshalHelper.GetBytesToMarshal((CompositeRestriction)restriction);
			case RestrictionType.Not:
				return MarshalHelper.GetBytesToMarshal(((SingleRestriction)restriction).ChildRestriction) + MarshalHelper.PaddedSizeOf_SRestriction_r;
			case RestrictionType.Content:
				return MarshalHelper.GetBytesToMarshal((ContentRestriction)restriction);
			case RestrictionType.Property:
				return MarshalHelper.GetBytesToMarshal((PropertyRestriction)restriction);
			case RestrictionType.CompareProps:
			case RestrictionType.BitMask:
			case RestrictionType.Size:
			case RestrictionType.Exists:
				return 0;
			case RestrictionType.SubRestriction:
				return MarshalHelper.GetBytesToMarshal(((SingleRestriction)restriction).ChildRestriction) + MarshalHelper.PaddedSizeOf_SRestriction_r;
			default:
				throw new FailRpcException(string.Format("Unable to convert invalid restriction type: {0}", restriction.RestrictionType), -2147467259);
			}
		}

		private static int GetBytesToMarshal(PropertyRestriction restriction)
		{
			if (restriction.PropertyValue != null)
			{
				return MarshalHelper.GetBytesToMarshal(restriction.PropertyValue.Value) + MarshalHelper.PaddedSizeOf_SPropValue_r;
			}
			return 0;
		}

		private static int GetBytesToMarshal(ContentRestriction restriction)
		{
			if (restriction.PropertyValue != null)
			{
				return MarshalHelper.GetBytesToMarshal(restriction.PropertyValue.Value) + MarshalHelper.PaddedSizeOf_SPropValue_r;
			}
			return 0;
		}

		private static int GetBytesToMarshal(SingleRestriction restriction)
		{
			return MarshalHelper.GetBytesToMarshal(restriction.ChildRestriction) + MarshalHelper.PaddedSizeOf_SRestriction_r;
		}

		private static int GetBytesToMarshal(CompositeRestriction restriction)
		{
			Restriction[] childRestrictions = restriction.ChildRestrictions;
			int num = childRestrictions.Length;
			int num2 = (int)((long)num * 24L) + 7 & -8;
			int num3 = 0;
			if (0 < num)
			{
				do
				{
					num2 += MarshalHelper.GetBytesToMarshal(childRestrictions[num3]);
					num3++;
				}
				while (num3 < num);
			}
			return num2;
		}

		private static int GetBytesToMarshal(PropertyValue[] propertyValues)
		{
			int num = 0;
			if (propertyValues != null)
			{
				int num2 = propertyValues.Length;
				int num3 = 0;
				if (0 < num2)
				{
					do
					{
						num += MarshalHelper.GetBytesToMarshal(propertyValues[num3]);
						num3++;
					}
					while (num3 < num2);
				}
			}
			return num;
		}

		private static int GetBytesToMarshal(PropertyValue propertyValue)
		{
			int result;
			try
			{
				int num = 0;
				PropertyTag propertyTag = propertyValue.PropertyTag;
				PropertyTag propertyTag2 = propertyTag;
				if (!propertyValue.IsNullValue)
				{
					int propertyType = (int)propertyTag2.PropertyType;
					if (propertyType <= 258)
					{
						if (propertyType != 258)
						{
							switch (propertyType + -1)
							{
							case 0:
							case 1:
							case 2:
							case 9:
							case 10:
							case 12:
							case 63:
								goto IL_437;
							case 29:
							{
								string string8PropertyValue = MarshalHelper.GetString8PropertyValue(propertyValue);
								if (string8PropertyValue != null)
								{
									int num2 = (int)((long)(string8PropertyValue.Length + 1) * 2L);
									num = (num2 + 7 & -8);
									goto IL_437;
								}
								goto IL_437;
							}
							case 30:
							{
								string value = propertyValue.GetValue<string>();
								if (value != null)
								{
									int num3 = (int)((long)(value.Length + 1) * 2L);
									num = (num3 + 7 & -8);
									goto IL_437;
								}
								goto IL_437;
							}
							case 71:
								num = MarshalHelper.PaddedSizeOf_Guid;
								goto IL_437;
							}
						}
						else
						{
							byte[] value2 = propertyValue.GetValue<byte[]>();
							if (value2 != null)
							{
								num = (value2.Length + 7 & -8);
								goto IL_437;
							}
							goto IL_437;
						}
					}
					else if (propertyType <= 4127)
					{
						if (propertyType != 4127)
						{
							if (propertyType != 4098)
							{
								if (propertyType != 4099)
								{
									if (propertyType == 4126)
									{
										string[] multiValuedString8PropertyValue = MarshalHelper.GetMultiValuedString8PropertyValue(propertyValue);
										if (multiValuedString8PropertyValue != null)
										{
											int num4 = multiValuedString8PropertyValue.Length;
											num = ((int)((long)num4 * 8L) + 7 & -8);
											for (int i = 0; i < num4; i++)
											{
												if (multiValuedString8PropertyValue[i] != null)
												{
													int num5 = (int)((long)(multiValuedString8PropertyValue[i].Length + 1) * 2L);
													num += (num5 + 7 & -8);
												}
											}
											goto IL_437;
										}
										goto IL_437;
									}
								}
								else
								{
									int[] value3 = propertyValue.GetValue<int[]>();
									if (value3 != null)
									{
										num = ((int)((long)value3.Length * 4L) + 7 & -8);
										goto IL_437;
									}
									goto IL_437;
								}
							}
							else
							{
								short[] value4 = propertyValue.GetValue<short[]>();
								if (value4 != null)
								{
									num = ((int)((long)value4.Length * 2L) + 7 & -8);
									goto IL_437;
								}
								goto IL_437;
							}
						}
						else
						{
							string[] value5 = propertyValue.GetValue<string[]>();
							if (value5 != null)
							{
								int num6 = value5.Length;
								num = ((int)((long)num6 * 8L) + 7 & -8);
								for (int j = 0; j < num6; j++)
								{
									string text = value5[j];
									if (text != null)
									{
										int num7 = (int)((long)(text.Length + 1) * 2L);
										num += (num7 + 7 & -8);
									}
								}
								goto IL_437;
							}
							goto IL_437;
						}
					}
					else if (propertyType != 4160)
					{
						if (propertyType != 4168)
						{
							if (propertyType == 4354)
							{
								byte[][] value6 = propertyValue.GetValue<byte[][]>();
								if (value6 != null)
								{
									int num8 = value6.Length;
									num = ((int)((long)num8 * 16L) + 7 & -8);
									for (int k = 0; k < num8; k++)
									{
										byte[] array = value6[k];
										if (array != null)
										{
											num += (array.Length + 7 & -8);
										}
									}
									goto IL_437;
								}
								goto IL_437;
							}
						}
						else
						{
							Guid[] value7 = propertyValue.GetValue<Guid[]>();
							if (value7 != null)
							{
								int num9 = value7.Length;
								num = MarshalHelper.PaddedSizeOf_Guid * num9 + ((int)((long)num9 * 8L) + 7 & -8);
								goto IL_437;
							}
							goto IL_437;
						}
					}
					else
					{
						ExDateTime[] value8 = propertyValue.GetValue<ExDateTime[]>();
						if (value8 != null)
						{
							num = ((int)((long)value8.Length * 8L) + 7 & -8);
							goto IL_437;
						}
						goto IL_437;
					}
					throw new FailRpcException(string.Format("Unable to marshal unsupported property type {0} on property {1}.", propertyTag2.PropertyType, propertyTag2), -2147467259);
				}
				IL_437:
				result = num;
			}
			catch (UnexpectedPropertyTypeException innerException)
			{
				throw new FailRpcException("Invalid PropertyValue detected from client.", -2147467259, innerException);
			}
			catch (NotSupportedException innerException2)
			{
				throw new FailRpcException("Unsupported PropertyType detected from client.", -2147467259, innerException2);
			}
			return result;
		}

		private unsafe static IntPtr MarshalStringToNativeMultiByteString(string stringValue, IntPtr pData, int codePage)
		{
			byte* ptr = (byte*)pData.ToPointer();
			sbyte* ptr2 = (sbyte*)ptr;
			int num = 0;
			if (stringValue != null && stringValue.Length > 0)
			{
				byte[] bytes = MarshalHelper.GetString8Encoding(codePage).GetBytes(stringValue);
				num = bytes.Length;
				int num2 = 0;
				if (0 < num)
				{
					do
					{
						*(byte*)ptr2 = (byte)((sbyte)bytes[num2]);
						ptr2 += 1L / (long)sizeof(sbyte);
						num2++;
					}
					while (num2 < num);
				}
			}
			*(byte*)ptr2 = 0;
			IntPtr result = new IntPtr((void*)(((long)(num + 8) & -8L) + ptr));
			return result;
		}

		private unsafe static IntPtr MarshalString8ToNative(PropertyValue propertyValue, _SPropValue_r* pSPropValue, IntPtr pData, int codePage)
		{
			byte* ptr = (byte*)pData.ToPointer();
			*(long*)(pSPropValue + 8L / (long)sizeof(_SPropValue_r)) = 0L;
			if (!propertyValue.IsNullValue)
			{
				string string8PropertyValue = MarshalHelper.GetString8PropertyValue(propertyValue);
				if (string8PropertyValue != null)
				{
					*(long*)(pSPropValue + 8L / (long)sizeof(_SPropValue_r)) = ptr;
					IntPtr pData2 = new IntPtr((void*)ptr);
					ptr = (byte*)MarshalHelper.MarshalStringToNativeMultiByteString(string8PropertyValue, pData2, codePage).ToPointer();
				}
			}
			IntPtr result = new IntPtr((void*)ptr);
			return result;
		}

		private unsafe static IntPtr MarshalMultiValuedString8ToNative(PropertyValue propertyValue, _SPropValue_r* pSPropValue, IntPtr pData, int codePage)
		{
			byte* ptr = (byte*)pData.ToPointer();
			*(int*)(pSPropValue + 8L / (long)sizeof(_SPropValue_r)) = 0;
			*(long*)(pSPropValue + 16L / (long)sizeof(_SPropValue_r)) = 0L;
			if (!propertyValue.IsNullValue)
			{
				string[] multiValuedString8PropertyValue = MarshalHelper.GetMultiValuedString8PropertyValue(propertyValue);
				if (multiValuedString8PropertyValue != null)
				{
					int num = multiValuedString8PropertyValue.Length;
					*(int*)(pSPropValue + 8L / (long)sizeof(_SPropValue_r)) = num;
					*(long*)(pSPropValue + 16L / (long)sizeof(_SPropValue_r)) = ptr;
					ptr = ((long)((int)((long)num * 8L) + 7) & -8L) + ptr;
					sbyte** ptr2 = *(long*)(pSPropValue + 16L / (long)sizeof(_SPropValue_r));
					int num2 = 0;
					long num3 = 0L;
					long num4 = (long)num;
					if (0L < num4)
					{
						do
						{
							if (multiValuedString8PropertyValue[num2] != null)
							{
								*(long*)(num3 * 8L / (long)sizeof(sbyte*) + ptr2) = ptr;
								IntPtr pData2 = new IntPtr((void*)ptr);
								ptr = (byte*)MarshalHelper.MarshalStringToNativeMultiByteString(multiValuedString8PropertyValue[num2], pData2, codePage).ToPointer();
							}
							else
							{
								*(long*)(num3 * 8L / (long)sizeof(sbyte*) + ptr2) = 0L;
							}
							num2++;
							num3 += 1L;
						}
						while (num3 < num4);
					}
				}
			}
			IntPtr result = new IntPtr((void*)ptr);
			return result;
		}

		private unsafe static IntPtr MarshalToNative(Restriction restriction, _SRestriction_r* pSRestriction, IntPtr pData, int codePage)
		{
			*(int*)pSRestriction = (int)restriction.RestrictionType;
			switch (restriction.RestrictionType)
			{
			case RestrictionType.And:
			{
				_SRestriction_r* pSRestriction2 = pSRestriction + 8L / (long)sizeof(_SRestriction_r);
				return MarshalHelper.MarshalToNative((CompositeRestriction)restriction, (_SAndOrRestriction_r*)pSRestriction2, pData, codePage);
			}
			case RestrictionType.Or:
			{
				_SRestriction_r* pSRestriction3 = pSRestriction + 8L / (long)sizeof(_SRestriction_r);
				return MarshalHelper.MarshalToNative((CompositeRestriction)restriction, (_SAndOrRestriction_r*)pSRestriction3, pData, codePage);
			}
			case RestrictionType.Not:
			{
				IntPtr pointer = pData;
				SingleRestriction singleRestriction = (NotRestriction)restriction;
				*(long*)(pSRestriction + 8L / (long)sizeof(_SRestriction_r)) = pointer.ToPointer();
				IntPtr pData2 = IntPtr.Add(pointer, MarshalHelper.PaddedSizeOf_SRestriction_r);
				return MarshalHelper.MarshalToNative(singleRestriction.ChildRestriction, *(long*)(pSRestriction + 8L / (long)sizeof(_SRestriction_r)), pData2, codePage);
			}
			case RestrictionType.Content:
			{
				_SRestriction_r* pSRestriction4 = pSRestriction + 8L / (long)sizeof(_SRestriction_r);
				return MarshalHelper.MarshalToNative((ContentRestriction)restriction, (_SContentRestriction_r*)pSRestriction4, pData, codePage);
			}
			case RestrictionType.Property:
			{
				_SRestriction_r* pSRestriction5 = pSRestriction + 8L / (long)sizeof(_SRestriction_r);
				return MarshalHelper.MarshalToNative((PropertyRestriction)restriction, (_SPropertyRestriction_r*)pSRestriction5, pData, codePage);
			}
			case RestrictionType.CompareProps:
			{
				ComparePropsRestriction comparePropsRestriction = (ComparePropsRestriction)restriction;
				*(int*)(pSRestriction + 8L / (long)sizeof(_SRestriction_r)) = (int)comparePropsRestriction.RelationOperator;
				PropertyTag property = comparePropsRestriction.Property1;
				*(int*)(pSRestriction + 8L / (long)sizeof(_SRestriction_r) + 4L / (long)sizeof(_SRestriction_r)) = (int)property;
				PropertyTag property2 = comparePropsRestriction.Property2;
				*(int*)(pSRestriction + 8L / (long)sizeof(_SRestriction_r) + 8L / (long)sizeof(_SRestriction_r)) = (int)property2;
				return pData;
			}
			case RestrictionType.BitMask:
			{
				BitMaskRestriction bitMaskRestriction = (BitMaskRestriction)restriction;
				*(int*)(pSRestriction + 8L / (long)sizeof(_SRestriction_r)) = (int)bitMaskRestriction.BitMaskOperator;
				PropertyTag propertyTag = bitMaskRestriction.PropertyTag;
				*(int*)(pSRestriction + 8L / (long)sizeof(_SRestriction_r) + 4L / (long)sizeof(_SRestriction_r)) = (int)propertyTag;
				*(int*)(pSRestriction + 8L / (long)sizeof(_SRestriction_r) + 8L / (long)sizeof(_SRestriction_r)) = (int)bitMaskRestriction.BitMask;
				return pData;
			}
			case RestrictionType.Size:
			{
				SizeRestriction sizeRestriction = (SizeRestriction)restriction;
				*(int*)(pSRestriction + 8L / (long)sizeof(_SRestriction_r)) = (int)sizeRestriction.RelationOperator;
				PropertyTag propertyTag2 = sizeRestriction.PropertyTag;
				*(int*)(pSRestriction + 8L / (long)sizeof(_SRestriction_r) + 4L / (long)sizeof(_SRestriction_r)) = (int)propertyTag2;
				*(int*)(pSRestriction + 8L / (long)sizeof(_SRestriction_r) + 8L / (long)sizeof(_SRestriction_r)) = (int)sizeRestriction.Size;
				return pData;
			}
			case RestrictionType.Exists:
			{
				ExistsRestriction existsRestriction = (ExistsRestriction)restriction;
				*(int*)(pSRestriction + 8L / (long)sizeof(_SRestriction_r)) = 0;
				PropertyTag propertyTag3 = existsRestriction.PropertyTag;
				*(int*)(pSRestriction + 8L / (long)sizeof(_SRestriction_r) + 4L / (long)sizeof(_SRestriction_r)) = (int)propertyTag3;
				*(int*)(pSRestriction + 8L / (long)sizeof(_SRestriction_r) + 8L / (long)sizeof(_SRestriction_r)) = 0;
				return pData;
			}
			case RestrictionType.SubRestriction:
			{
				_SRestriction_r* pSRestriction6 = pSRestriction + 8L / (long)sizeof(_SRestriction_r);
				return MarshalHelper.MarshalToNative((SubRestriction)restriction, (_SSubRestriction_r*)pSRestriction6, pData, codePage);
			}
			default:
				throw new FailRpcException(string.Format("Unable to convert invalid restriction type: {0}", restriction.RestrictionType), -2147467259);
			}
		}

		private unsafe static IntPtr MarshalToNative(ExistsRestriction restriction, _SExistRestriction_r* pSRestriction, IntPtr pData)
		{
			*(int*)pSRestriction = 0;
			PropertyTag propertyTag = restriction.PropertyTag;
			*(int*)(pSRestriction + 4L / (long)sizeof(_SExistRestriction_r)) = (int)propertyTag;
			*(int*)(pSRestriction + 8L / (long)sizeof(_SExistRestriction_r)) = 0;
			return pData;
		}

		private unsafe static IntPtr MarshalToNative(SizeRestriction restriction, _SSizeRestriction_r* pSRestriction, IntPtr pData)
		{
			*(int*)pSRestriction = (int)restriction.RelationOperator;
			PropertyTag propertyTag = restriction.PropertyTag;
			*(int*)(pSRestriction + 4L / (long)sizeof(_SSizeRestriction_r)) = (int)propertyTag;
			*(int*)(pSRestriction + 8L / (long)sizeof(_SSizeRestriction_r)) = (int)restriction.Size;
			return pData;
		}

		private unsafe static IntPtr MarshalToNative(BitMaskRestriction restriction, _SBitMaskRestriction_r* pSRestriction, IntPtr pData)
		{
			*(int*)pSRestriction = (int)restriction.BitMaskOperator;
			PropertyTag propertyTag = restriction.PropertyTag;
			*(int*)(pSRestriction + 4L / (long)sizeof(_SBitMaskRestriction_r)) = (int)propertyTag;
			*(int*)(pSRestriction + 8L / (long)sizeof(_SBitMaskRestriction_r)) = (int)restriction.BitMask;
			return pData;
		}

		private unsafe static IntPtr MarshalToNative(ComparePropsRestriction restriction, _SComparePropsRestriction_r* pSRestriction, IntPtr pData)
		{
			*(int*)pSRestriction = (int)restriction.RelationOperator;
			PropertyTag property = restriction.Property1;
			*(int*)(pSRestriction + 4L / (long)sizeof(_SComparePropsRestriction_r)) = (int)property;
			PropertyTag property2 = restriction.Property2;
			*(int*)(pSRestriction + 8L / (long)sizeof(_SComparePropsRestriction_r)) = (int)property2;
			return pData;
		}

		private unsafe static IntPtr MarshalToNative(SubRestriction restriction, _SSubRestriction_r* pSRestriction, IntPtr pData, int codePage)
		{
			*(int*)pSRestriction = (int)restriction.SubRestrictionType;
			*(long*)(pSRestriction + 8L / (long)sizeof(_SSubRestriction_r)) = pData.ToPointer();
			IntPtr pData2 = IntPtr.Add(pData, MarshalHelper.PaddedSizeOf_SRestriction_r);
			return MarshalHelper.MarshalToNative(restriction.ChildRestriction, *(long*)(pSRestriction + 8L / (long)sizeof(_SSubRestriction_r)), pData2, codePage);
		}

		private unsafe static IntPtr MarshalToNative(PropertyRestriction restriction, _SPropertyRestriction_r* pSRestriction, IntPtr pData, int codePage)
		{
			*(int*)pSRestriction = (int)restriction.RelationOperator;
			PropertyTag propertyTag = restriction.PropertyTag;
			*(int*)(pSRestriction + 4L / (long)sizeof(_SPropertyRestriction_r)) = (int)propertyTag;
			if (restriction.PropertyValue != null)
			{
				*(long*)(pSRestriction + 8L / (long)sizeof(_SPropertyRestriction_r)) = pData.ToPointer();
				IntPtr pData2 = IntPtr.Add(pData, MarshalHelper.PaddedSizeOf_SPropValue_r);
				return MarshalHelper.MarshalToNative(restriction.PropertyValue.Value, *(long*)(pSRestriction + 8L / (long)sizeof(_SPropertyRestriction_r)), pData2, codePage);
			}
			*(long*)(pSRestriction + 8L / (long)sizeof(_SPropertyRestriction_r)) = 0L;
			return pData;
		}

		private unsafe static IntPtr MarshalToNative(ContentRestriction restriction, _SContentRestriction_r* pSRestriction, IntPtr pData, int codePage)
		{
			*(int*)pSRestriction = (int)restriction.FuzzyLevel;
			PropertyTag propertyTag = restriction.PropertyTag;
			*(int*)(pSRestriction + 4L / (long)sizeof(_SContentRestriction_r)) = (int)propertyTag;
			*(long*)(pSRestriction + 8L / (long)sizeof(_SContentRestriction_r)) = pData.ToPointer();
			if (restriction.PropertyValue != null)
			{
				*(long*)(pSRestriction + 8L / (long)sizeof(_SContentRestriction_r)) = pData.ToPointer();
				IntPtr pData2 = IntPtr.Add(pData, MarshalHelper.PaddedSizeOf_SPropValue_r);
				return MarshalHelper.MarshalToNative(restriction.PropertyValue.Value, *(long*)(pSRestriction + 8L / (long)sizeof(_SContentRestriction_r)), pData2, codePage);
			}
			*(long*)(pSRestriction + 8L / (long)sizeof(_SContentRestriction_r)) = 0L;
			return pData;
		}

		private unsafe static IntPtr MarshalToNative(NotRestriction restriction, _SNotRestriction_r* pSRestriction, IntPtr pData, int codePage)
		{
			*(long*)pSRestriction = pData.ToPointer();
			IntPtr pData2 = IntPtr.Add(pData, MarshalHelper.PaddedSizeOf_SRestriction_r);
			return MarshalHelper.MarshalToNative(restriction.ChildRestriction, *(long*)pSRestriction, pData2, codePage);
		}

		private unsafe static IntPtr MarshalToNative(CompositeRestriction restriction, _SAndOrRestriction_r* pSRestriction, IntPtr pData, int codePage)
		{
			Restriction[] childRestrictions = restriction.ChildRestrictions;
			int num = childRestrictions.Length;
			_SRestriction_r* ptr = (_SRestriction_r*)pData.ToPointer();
			*(int*)pSRestriction = num;
			*(long*)(pSRestriction + 8L / (long)sizeof(_SAndOrRestriction_r)) = ptr;
			IntPtr intPtr = IntPtr.Add(pData, (int)((long)num * 24L) + 7 & -8);
			int num2 = 0;
			if (0 < num)
			{
				_SRestriction_r* ptr2 = ptr;
				do
				{
					intPtr = MarshalHelper.MarshalToNative(childRestrictions[num2], ptr2, intPtr, codePage);
					num2++;
					ptr2 += 24L / (long)sizeof(_SRestriction_r);
				}
				while (num2 < num);
			}
			return intPtr;
		}

		private unsafe static void MarshalToNative(SafeRpcMemoryHandle rpcMemoryHandle, PropertyValue[][] propertyRows, _SRowSet_r* pSRowSet, int codePage)
		{
			if (propertyRows != null)
			{
				int num = propertyRows.Length;
				*(int*)pSRowSet = num;
				_SRow_r* ptr = (_SRow_r*)(pSRowSet + 8L / (long)sizeof(_SRowSet_r));
				int num2 = 0;
				if (0 < num)
				{
					do
					{
						MarshalHelper.MarshalToNative(rpcMemoryHandle, propertyRows[num2], ptr, codePage);
						ptr += 16L / (long)sizeof(_SRow_r);
						num2++;
					}
					while (num2 < num);
				}
			}
			else
			{
				*(int*)pSRowSet = 0;
			}
		}

		private unsafe static void MarshalToNative(SafeRpcMemoryHandle rpcMemoryHandle, PropertyValue[] propertyValues, _SRow_r* pSRow, int codePage)
		{
			if (propertyValues != null)
			{
				SafeRpcMemoryHandle safeRpcMemoryHandle = null;
				bool flag = false;
				try
				{
					int num = propertyValues.Length;
					int num2 = (int)((long)num * 24L) + 7 & -8;
					int num3 = MarshalHelper.GetBytesToMarshal(propertyValues) + num2;
					int num4 = num3 + 7 & -8;
					safeRpcMemoryHandle = new SafeRpcMemoryHandle();
					safeRpcMemoryHandle.Allocate(num4);
					byte* ptr = (byte*)safeRpcMemoryHandle.DangerousGetHandle().ToPointer();
					initblk(ptr, 0, (long)num4);
					_SPropValue_r* ptr2 = (_SPropValue_r*)ptr;
					ptr = num2 + ptr;
					rpcMemoryHandle.AddAssociatedHandle(safeRpcMemoryHandle);
					safeRpcMemoryHandle = null;
					*(int*)(pSRow + 4L / (long)sizeof(_SRow_r)) = num;
					*(long*)(pSRow + 8L / (long)sizeof(_SRow_r)) = ptr2;
					for (int i = 0; i < num; i++)
					{
						IntPtr pData = new IntPtr((void*)ptr);
						ptr = (byte*)MarshalHelper.MarshalToNative(propertyValues[i], ptr2, pData, codePage).ToPointer();
						ptr2 += 24L / (long)sizeof(_SPropValue_r);
					}
					flag = true;
					return;
				}
				finally
				{
					if (!flag && safeRpcMemoryHandle != null)
					{
						((IDisposable)safeRpcMemoryHandle).Dispose();
					}
				}
			}
			*(int*)(pSRow + 4L / (long)sizeof(_SRow_r)) = 0;
			*(long*)(pSRow + 8L / (long)sizeof(_SRow_r)) = 0L;
		}

		private unsafe static IntPtr MarshalToNative(PropertyValue propertyValue, _SPropValue_r* pSPropValue, IntPtr pData, int codePage)
		{
			IntPtr result;
			try
			{
				byte* ptr = (byte*)pData.ToPointer();
				PropertyTag propertyTag = propertyValue.PropertyTag;
				PropertyTag propertyTag2 = propertyTag;
				initblk(pSPropValue, 0, 24L);
				*(int*)pSPropValue = (int)propertyTag2;
				if (!propertyValue.IsNullValue)
				{
					int propertyType = (int)propertyTag2.PropertyType;
					if (propertyType <= 258)
					{
						if (propertyType != 258)
						{
							switch (propertyType + -1)
							{
							case 0:
							case 12:
								goto IL_726;
							case 1:
							{
								short value = propertyValue.GetValue<short>();
								*(short*)(pSPropValue + 8L / (long)sizeof(_SPropValue_r)) = value;
								goto IL_726;
							}
							case 2:
							{
								int value2 = propertyValue.GetValue<int>();
								*(int*)(pSPropValue + 8L / (long)sizeof(_SPropValue_r)) = value2;
								goto IL_726;
							}
							case 9:
							{
								ErrorCode errorCode = (ErrorCode)propertyValue.Value;
								*(int*)(pSPropValue + 8L / (long)sizeof(_SPropValue_r)) = (int)errorCode;
								goto IL_726;
							}
							case 10:
							{
								bool value3 = propertyValue.GetValue<bool>();
								int num = value3 ? 1 : 0;
								*(short*)(pSPropValue + 8L / (long)sizeof(_SPropValue_r)) = (short)num;
								goto IL_726;
							}
							case 29:
							{
								IntPtr pData2 = new IntPtr((void*)ptr);
								ptr = (byte*)MarshalHelper.MarshalString8ToNative(propertyValue, pSPropValue, pData2, codePage).ToPointer();
								goto IL_726;
							}
							case 30:
							{
								string value4 = propertyValue.GetValue<string>();
								if (value4 != null)
								{
									int length = value4.Length;
									*(long*)(pSPropValue + 8L / (long)sizeof(_SPropValue_r)) = ptr;
									ptr = ((long)((int)((long)(length + 1) * 2L) + 7) & -8L) + ptr;
									ushort* ptr2 = *(long*)(pSPropValue + 8L / (long)sizeof(_SPropValue_r));
									for (int i = 0; i < length; i++)
									{
										*ptr2 = (ushort)value4[i];
										ptr2 += 2L / 2L;
									}
									*ptr2 = 0;
									goto IL_726;
								}
								goto IL_726;
							}
							case 63:
							{
								long num2 = propertyValue.GetValue<ExDateTime>().ToFileTimeUtc();
								*(long*)(pSPropValue + 8L / (long)sizeof(_SPropValue_r)) = num2;
								goto IL_726;
							}
							case 71:
							{
								Guid value5 = propertyValue.GetValue<Guid>();
								ref Guid ptr3 = ref *(Guid*)ptr;
								*(long*)(pSPropValue + 8L / (long)sizeof(_SPropValue_r)) = ptr;
								ptr = MarshalHelper.PaddedSizeOf_Guid + ptr;
								ptr3 = value5;
								goto IL_726;
							}
							}
						}
						else
						{
							byte[] value6 = propertyValue.GetValue<byte[]>();
							if (value6 == null)
							{
								goto IL_726;
							}
							int num3 = value6.Length;
							*(int*)(pSPropValue + 8L / (long)sizeof(_SPropValue_r)) = num3;
							*(long*)(pSPropValue + 16L / (long)sizeof(_SPropValue_r)) = ptr;
							ptr = ((long)(num3 + 7) & -8L) + ptr;
							byte* value7 = *(long*)(pSPropValue + 16L / (long)sizeof(_SPropValue_r));
							if (num3 > 0)
							{
								IntPtr destination = (IntPtr)((void*)value7);
								Marshal.Copy(value6, 0, destination, num3);
								goto IL_726;
							}
							goto IL_726;
						}
					}
					else if (propertyType <= 4127)
					{
						if (propertyType != 4127)
						{
							if (propertyType != 4098)
							{
								if (propertyType != 4099)
								{
									if (propertyType == 4126)
									{
										IntPtr pData3 = new IntPtr((void*)ptr);
										ptr = (byte*)MarshalHelper.MarshalMultiValuedString8ToNative(propertyValue, pSPropValue, pData3, codePage).ToPointer();
										goto IL_726;
									}
								}
								else
								{
									int[] value8 = propertyValue.GetValue<int[]>();
									if (value8 == null)
									{
										goto IL_726;
									}
									int num4 = value8.Length;
									*(int*)(pSPropValue + 8L / (long)sizeof(_SPropValue_r)) = num4;
									*(long*)(pSPropValue + 16L / (long)sizeof(_SPropValue_r)) = ptr;
									ptr = ((long)((int)((long)num4 * 4L) + 7) & -8L) + ptr;
									uint* value9 = *(long*)(pSPropValue + 16L / (long)sizeof(_SPropValue_r));
									if (num4 > 0)
									{
										IntPtr destination2 = (IntPtr)((void*)value9);
										Marshal.Copy(value8, 0, destination2, num4);
										goto IL_726;
									}
									goto IL_726;
								}
							}
							else
							{
								short[] value10 = propertyValue.GetValue<short[]>();
								if (value10 == null)
								{
									goto IL_726;
								}
								int num5 = value10.Length;
								*(int*)(pSPropValue + 8L / (long)sizeof(_SPropValue_r)) = num5;
								*(long*)(pSPropValue + 16L / (long)sizeof(_SPropValue_r)) = ptr;
								ptr = ((long)((int)((long)num5 * 2L) + 7) & -8L) + ptr;
								byte* value11 = *(long*)(pSPropValue + 16L / (long)sizeof(_SPropValue_r));
								if (num5 > 0)
								{
									IntPtr destination3 = (IntPtr)((void*)value11);
									Marshal.Copy(value10, 0, destination3, num5);
									goto IL_726;
								}
								goto IL_726;
							}
						}
						else
						{
							string[] value12 = propertyValue.GetValue<string[]>();
							if (value12 != null)
							{
								int num6 = value12.Length;
								*(int*)(pSPropValue + 8L / (long)sizeof(_SPropValue_r)) = num6;
								*(long*)(pSPropValue + 16L / (long)sizeof(_SPropValue_r)) = ptr;
								ptr = ((long)((int)((long)num6 * 8L) + 7) & -8L) + ptr;
								ushort** ptr4 = *(long*)(pSPropValue + 16L / (long)sizeof(_SPropValue_r));
								for (int j = 0; j < num6; j++)
								{
									string text = value12[j];
									if (text != null)
									{
										int length2 = text.Length;
										ushort** ptr5 = (long)j * 8L + ptr4 / sizeof(ushort*);
										*(long*)ptr5 = ptr;
										ptr = ((long)((int)((long)(length2 + 1) * 2L) + 7) & -8L) + ptr;
										ushort* ptr6 = *(long*)ptr5;
										for (int k = 0; k < length2; k++)
										{
											*ptr6 = (ushort)text[k];
											ptr6 += 2L / 2L;
										}
										*ptr6 = 0;
									}
									else
									{
										*(long*)((long)j * 8L + ptr4 / 8) = 0L;
									}
								}
								goto IL_726;
							}
							goto IL_726;
						}
					}
					else if (propertyType != 4160)
					{
						if (propertyType != 4168)
						{
							if (propertyType == 4354)
							{
								byte[][] value13 = propertyValue.GetValue<byte[][]>();
								if (value13 != null)
								{
									int num7 = value13.Length;
									*(int*)(pSPropValue + 8L / (long)sizeof(_SPropValue_r)) = num7;
									*(long*)(pSPropValue + 16L / (long)sizeof(_SPropValue_r)) = ptr;
									ptr = ((long)((int)((long)num7 * 16L) + 7) & -8L) + ptr;
									_SBinary_r* ptr7 = *(long*)(pSPropValue + 16L / (long)sizeof(_SPropValue_r));
									for (int l = 0; l < num7; l++)
									{
										byte[] array = value13[l];
										if (array != null)
										{
											int num8 = array.Length;
											_SBinary_r* ptr8 = (long)l * 16L + ptr7 / sizeof(_SBinary_r);
											*(int*)ptr8 = num8;
											*(long*)(ptr8 + 8L / (long)sizeof(_SBinary_r)) = ptr;
											ptr = ((long)(num8 + 7) & -8L) + ptr;
											if (num8 > 0)
											{
												IntPtr destination4 = (IntPtr)(*(long*)(ptr8 + 8L / (long)sizeof(_SBinary_r)));
												Marshal.Copy(value13[l], 0, destination4, num8);
											}
										}
										else
										{
											_SBinary_r* ptr8 = (long)l * 16L + ptr7 / sizeof(_SBinary_r);
											*(int*)ptr8 = 0;
											*(long*)(ptr8 + 8L / (long)sizeof(_SBinary_r)) = 0L;
										}
									}
									goto IL_726;
								}
								goto IL_726;
							}
						}
						else
						{
							Guid[] value14 = propertyValue.GetValue<Guid[]>();
							if (value14 != null)
							{
								int num9 = value14.Length;
								*(int*)(pSPropValue + 8L / (long)sizeof(_SPropValue_r)) = num9;
								*(long*)(pSPropValue + 16L / (long)sizeof(_SPropValue_r)) = ptr;
								ptr = ((long)((int)((long)num9 * 8L) + 7) & -8L) + ptr;
								__MIDL_nspi_0001** ptr9 = *(long*)(pSPropValue + 16L / (long)sizeof(_SPropValue_r));
								for (int m = 0; m < num9; m++)
								{
									__MIDL_nspi_0001** ptr10 = (long)m * 8L + ptr9 / sizeof(__MIDL_nspi_0001*);
									*(long*)ptr10 = ptr;
									ptr = MarshalHelper.PaddedSizeOf_Guid + ptr;
									*(*(long*)ptr10) = value14[m];
								}
								goto IL_726;
							}
							goto IL_726;
						}
					}
					else
					{
						ExDateTime[] value15 = propertyValue.GetValue<ExDateTime[]>();
						if (value15 != null)
						{
							int num10 = value15.Length;
							*(int*)(pSPropValue + 8L / (long)sizeof(_SPropValue_r)) = num10;
							*(long*)(pSPropValue + 16L / (long)sizeof(_SPropValue_r)) = ptr;
							ptr = ((long)((int)((long)num10 * 8L) + 7) & -8L) + ptr;
							_FILETIME_r* ptr11 = *(long*)(pSPropValue + 16L / (long)sizeof(_SPropValue_r));
							for (int n = 0; n < num10; n++)
							{
								ExDateTime exDateTime = value15[n];
								long num11 = exDateTime.ToFileTimeUtc();
								*(long*)ptr11 = num11;
								ptr11 += 8L / (long)sizeof(_FILETIME_r);
							}
							goto IL_726;
						}
						goto IL_726;
					}
					throw new FailRpcException(string.Format("Unable to marshal unsupported property type {0} on property {1}.", propertyTag2.PropertyType, propertyTag2), -2147467259);
				}
				IL_726:
				result = (IntPtr)((void*)ptr);
			}
			catch (UnexpectedPropertyTypeException innerException)
			{
				throw new FailRpcException("Invalid PropertyValue detected from client.", -2147467259, innerException);
			}
			return result;
		}

		private unsafe static Restriction[] ConvertSRestrictionArray(int count, _SRestriction_r* pSRestriction, int codePage)
		{
			if (pSRestriction == null)
			{
				throw new FailRpcException("Null restriction array in SAndOrRestriction.", -2147467259);
			}
			Restriction[] array = new Restriction[count];
			int num = 0;
			if (0 < count)
			{
				_SRestriction_r* ptr = pSRestriction;
				do
				{
					IntPtr pRest = (IntPtr)((void*)ptr);
					array[num] = MarshalHelper.ConvertSRestrictionToRestriction(pRest, codePage);
					num++;
					ptr += 24L / (long)sizeof(_SRestriction_r);
				}
				while (num < count);
			}
			return array;
		}

		private unsafe static AndRestriction ConvertSAndRestrictionToAndRestriction(_SAndOrRestriction_r* pSAndRestriction, int codePage)
		{
			return new AndRestriction(MarshalHelper.ConvertSRestrictionArray(*(int*)pSAndRestriction, *(long*)(pSAndRestriction + 8L / (long)sizeof(_SAndOrRestriction_r)), codePage));
		}

		private unsafe static OrRestriction ConvertSOrRestrictionToOrRestriction(_SAndOrRestriction_r* pSOrRestriction, int codePage)
		{
			return new OrRestriction(MarshalHelper.ConvertSRestrictionArray(*(int*)pSOrRestriction, *(long*)(pSOrRestriction + 8L / (long)sizeof(_SAndOrRestriction_r)), codePage));
		}

		private unsafe static NotRestriction ConvertSNotRestrictionToNotRestriction(_SNotRestriction_r* pSNotRestriction, int codePage)
		{
			ulong num = (ulong)(*(long*)pSNotRestriction);
			if (num == 0UL)
			{
				throw new FailRpcException("Null restriction in SNotRestriction.", -2147467259);
			}
			return new NotRestriction(MarshalHelper.ConvertSRestrictionToRestriction((IntPtr)num, codePage));
		}

		private unsafe static ContentRestriction ConvertSContentRestrictionToContentRestriction(_SContentRestriction_r* pSContentRestriction, int codePage)
		{
			ulong num = (ulong)(*(long*)(pSContentRestriction + 8L / (long)sizeof(_SContentRestriction_r)));
			if (num == 0UL)
			{
				throw new FailRpcException("Null property value in SContentRestriction.", -2147467259);
			}
			PropertyValue? propertyValue = MarshalHelper.ConvertSPropValueToPropertyValue((IntPtr)num, codePage);
			PropertyTag propertyTag = new PropertyTag((uint)(*(int*)(pSContentRestriction + 4L / (long)sizeof(_SContentRestriction_r))));
			return new ContentRestriction((FuzzyLevel)(*(int*)pSContentRestriction), propertyTag, propertyValue);
		}

		private unsafe static PropertyRestriction ConvertSPropertyRestrictionToPropertyRestriction(_SPropertyRestriction_r* pSPropertyRestriction, int codePage)
		{
			ulong num = (ulong)(*(long*)(pSPropertyRestriction + 8L / (long)sizeof(_SPropertyRestriction_r)));
			if (num == 0UL)
			{
				throw new FailRpcException("Null property value in SPropertyRestriction.", -2147467259);
			}
			PropertyValue? propertyValue = MarshalHelper.ConvertSPropValueToPropertyValue((IntPtr)num, codePage);
			PropertyTag propertyTag = new PropertyTag((uint)(*(int*)(pSPropertyRestriction + 4L / (long)sizeof(_SPropertyRestriction_r))));
			return new PropertyRestriction((RelationOperator)(*(int*)pSPropertyRestriction), propertyTag, propertyValue);
		}

		private unsafe static ComparePropsRestriction ConvertSComparePropsRestrictionToComparePropsRestriction(_SComparePropsRestriction_r* pSComparePropsRestriction)
		{
			PropertyTag property = new PropertyTag((uint)(*(int*)(pSComparePropsRestriction + 8L / (long)sizeof(_SComparePropsRestriction_r))));
			PropertyTag property2 = new PropertyTag((uint)(*(int*)(pSComparePropsRestriction + 4L / (long)sizeof(_SComparePropsRestriction_r))));
			return new ComparePropsRestriction((RelationOperator)(*(int*)pSComparePropsRestriction), property2, property);
		}

		private unsafe static BitMaskRestriction ConvertSBitMaskRestrictionToBitMaskRestriction(_SBitMaskRestriction_r* pSBitMaskRestriction)
		{
			PropertyTag propertyTag = new PropertyTag((uint)(*(int*)(pSBitMaskRestriction + 4L / (long)sizeof(_SBitMaskRestriction_r))));
			return new BitMaskRestriction((BitMaskOperator)(*(int*)pSBitMaskRestriction), propertyTag, (uint)(*(int*)(pSBitMaskRestriction + 8L / (long)sizeof(_SBitMaskRestriction_r))));
		}

		private unsafe static SubRestriction ConvertSSubRestrictionToSubRestriction(_SSubRestriction_r* pSSubRestriction, int codePage)
		{
			ulong num = (ulong)(*(long*)(pSSubRestriction + 8L / (long)sizeof(_SSubRestriction_r)));
			if (num == 0UL)
			{
				throw new FailRpcException("Null restriction in SSubRestriction.", -2147467259);
			}
			IntPtr pRest = (IntPtr)num;
			return new SubRestriction((SubRestrictionType)(*(int*)pSSubRestriction), MarshalHelper.ConvertSRestrictionToRestriction(pRest, codePage));
		}

		private unsafe static SizeRestriction ConvertSSizeRestrictionToSizeRestriction(_SSizeRestriction_r* pSSizeRestriction)
		{
			PropertyTag propertyTag = new PropertyTag((uint)(*(int*)(pSSizeRestriction + 4L / (long)sizeof(_SSizeRestriction_r))));
			return new SizeRestriction((RelationOperator)(*(int*)pSSizeRestriction), propertyTag, (uint)(*(int*)(pSSizeRestriction + 8L / (long)sizeof(_SSizeRestriction_r))));
		}

		private unsafe static ExistsRestriction ConvertSExistRestrictionToExistsRestriction(_SExistRestriction_r* pSExistRestriction)
		{
			PropertyTag propertyTag = new PropertyTag((uint)(*(int*)(pSExistRestriction + 4L / (long)sizeof(_SExistRestriction_r))));
			return new ExistsRestriction(propertyTag);
		}

		private static int DefaultANSICodePage = 1252;

		private static readonly int PaddedSizeOf_SPropValue_r = 24;

		private static readonly int PaddedSizeOf_SRow_r = 16;

		private static readonly int PaddedSizeOf_SRestriction_r = 24;

		private static readonly int PaddedSizeOf_Guid = 16;
	}
}
