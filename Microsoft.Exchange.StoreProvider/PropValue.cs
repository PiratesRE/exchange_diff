using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct PropValue : IEquatable<PropValue>
	{
		public PropTag PropTag
		{
			get
			{
				return this.propTag;
			}
		}

		public PropType PropType
		{
			get
			{
				return (PropType)(this.propTag & (PropTag)65535U);
			}
		}

		public object Value
		{
			get
			{
				if (this.PropType == PropType.Error)
				{
					return null;
				}
				return this.RawValue;
			}
		}

		public object RawValue
		{
			get
			{
				if (this.value == null && this.nativeValueType != PropValue.NativeType.Unused)
				{
					PropType propType = this.PropType;
					switch (propType)
					{
					case PropType.Short:
						this.value = this.nativeValue.s;
						goto IL_15D;
					case PropType.Int:
					case PropType.Error:
					case PropType.Object:
						this.value = this.nativeValue.i;
						goto IL_15D;
					case PropType.Float:
						this.value = this.nativeValue.f;
						goto IL_15D;
					case PropType.Double:
					case PropType.AppTime:
						this.value = this.nativeValue.d;
						goto IL_15D;
					case PropType.Currency:
					case PropType.Long:
						this.value = this.nativeValue.l;
						goto IL_15D;
					case (PropType)8:
					case (PropType)9:
					case (PropType)12:
					case (PropType)14:
					case (PropType)15:
					case (PropType)16:
					case (PropType)17:
					case (PropType)18:
					case (PropType)19:
						break;
					case PropType.Boolean:
						this.value = ((this.nativeValue.s != 0) ? PropValue.True : PropValue.False);
						goto IL_15D;
					default:
						if (propType == PropType.SysTime)
						{
							this.value = PropValue.LongAsDateTime(this.nativeValue.l);
							goto IL_15D;
						}
						break;
					}
					throw MapiExceptionHelper.InvalidTypeException("Unknown property native type " + this.PropType + " returned from server.");
				}
				IL_15D:
				return this.value;
			}
		}

		public PropValue(PropTag propTag, object value)
		{
			this.nativeValue = default(_PV);
			this.nativeValueType = PropValue.NativeType.Unused;
			this.propTag = propTag;
			if (value is DateTime)
			{
				DateTime dateTime = (DateTime)value;
				if (dateTime != DateTime.MinValue && dateTime.Kind != DateTimeKind.Utc)
				{
					dateTime = dateTime.ToUniversalTime();
				}
				this.value = dateTime;
				return;
			}
			DateTime[] array = value as DateTime[];
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					DateTime dateTime2 = array[i];
					if (array[i] != DateTime.MinValue && array[i].Kind != DateTimeKind.Utc)
					{
						array[i] = array[i].ToUniversalTime();
					}
				}
				this.value = array;
				return;
			}
			this.value = value;
		}

		internal unsafe PropValue(SPropValue* buffer)
		{
			this = new PropValue(buffer, false);
		}

		internal unsafe PropValue(SPropValue* buffer, bool retainAnsiStrings)
		{
			this.value = null;
			this.nativeValue = default(_PV);
			this.nativeValueType = PropValue.NativeType.Unused;
			this.propTag = (PropTag)buffer->ulPropTag;
			PropType propType = (PropType)(this.propTag & (PropTag)65535U);
			if (propType <= PropType.Binary)
			{
				if (propType <= PropType.SysTime)
				{
					switch (propType)
					{
					case PropType.Null:
						goto IL_183;
					case PropType.Short:
					case PropType.Int:
					case PropType.Float:
					case PropType.Double:
					case PropType.Currency:
					case PropType.AppTime:
					case PropType.Error:
					case PropType.Boolean:
					case PropType.Object:
					case PropType.Long:
						break;
					case (PropType)8:
					case (PropType)9:
					case (PropType)12:
					case (PropType)14:
					case (PropType)15:
					case (PropType)16:
					case (PropType)17:
					case (PropType)18:
					case (PropType)19:
						goto IL_711;
					default:
						switch (propType)
						{
						case PropType.AnsiString:
							if (retainAnsiStrings)
							{
								this.value = PropValue.GetAnsiStringAsByteArray(buffer->value.intPtr);
								return;
							}
							this.value = Marshal.PtrToStringAnsi(buffer->value.intPtr);
							return;
						case PropType.String:
							this.value = Marshal.PtrToStringUni(buffer->value.intPtr);
							return;
						default:
							if (propType != PropType.SysTime)
							{
								goto IL_711;
							}
							break;
						}
						break;
					}
					this.InitNativeValue(buffer);
					return;
				}
				if (propType == PropType.Guid)
				{
					this.value = Marshal.PtrToStructure(buffer->value.intPtr, typeof(Guid));
					return;
				}
				switch (propType)
				{
				case PropType.Restriction:
					this.value = Restriction.Unmarshal((SRestriction*)((void*)buffer->value.intPtr));
					return;
				case PropType.Actions:
					this.value = RuleActions.Unmarshal(buffer->value.intPtr);
					return;
				default:
					if (propType != PropType.Binary)
					{
						goto IL_711;
					}
					this.value = new byte[buffer->value.cp.count];
					if (buffer->value.cp.count > 0)
					{
						Marshal.Copy(buffer->value.cp.intPtr, (byte[])this.value, 0, buffer->value.cp.count);
						return;
					}
					return;
				}
			}
			else if (propType <= PropType.StringArray)
			{
				switch (propType)
				{
				case PropType.ShortArray:
					this.value = new short[buffer->value.cp.count];
					if (buffer->value.cp.count > 0)
					{
						Marshal.Copy(buffer->value.cp.intPtr, (short[])this.value, 0, buffer->value.cp.count);
						return;
					}
					return;
				case PropType.IntArray:
					this.value = new int[buffer->value.cp.count];
					if (buffer->value.cp.count > 0)
					{
						Marshal.Copy(buffer->value.cp.intPtr, (int[])this.value, 0, buffer->value.cp.count);
						return;
					}
					return;
				case PropType.FloatArray:
					this.value = new float[buffer->value.cp.count];
					if (buffer->value.cp.count > 0)
					{
						Marshal.Copy(buffer->value.cp.intPtr, (float[])this.value, 0, buffer->value.cp.count);
						return;
					}
					return;
				case PropType.DoubleArray:
				case PropType.AppTimeArray:
					this.value = new double[buffer->value.cp.count];
					if (buffer->value.cp.count > 0)
					{
						Marshal.Copy(buffer->value.cp.intPtr, (double[])this.value, 0, buffer->value.cp.count);
						return;
					}
					return;
				case PropType.CurrencyArray:
					break;
				case (PropType)4104:
				case (PropType)4105:
				case (PropType)4106:
				case (PropType)4107:
				case (PropType)4108:
					goto IL_711;
				case PropType.ObjectArray:
					goto IL_183;
				default:
					if (propType != PropType.LongArray)
					{
						switch (propType)
						{
						case PropType.AnsiStringArray:
						{
							int count = buffer->value.cp.count;
							IntPtr* ptr = (IntPtr*)((void*)buffer->value.cp.intPtr);
							object[] array;
							if (retainAnsiStrings)
							{
								array = new byte[count][];
								for (int i = 0; i < count; i++)
								{
									array[i] = PropValue.GetAnsiStringAsByteArray(ptr[(IntPtr)i * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)]);
								}
							}
							else
							{
								array = new string[count];
								for (int j = 0; j < count; j++)
								{
									array[j] = Marshal.PtrToStringAnsi(ptr[(IntPtr)j * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)]);
								}
							}
							this.value = array;
							return;
						}
						case PropType.StringArray:
						{
							int count2 = buffer->value.cp.count;
							string[] array2 = new string[count2];
							IntPtr* ptr2 = (IntPtr*)((void*)buffer->value.cp.intPtr);
							for (int k = 0; k < count2; k++)
							{
								array2[k] = Marshal.PtrToStringUni(ptr2[(IntPtr)k * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)]);
							}
							this.value = array2;
							return;
						}
						default:
							goto IL_711;
						}
					}
					break;
				}
				this.value = new long[buffer->value.cp.count];
				if (buffer->value.cp.count > 0)
				{
					Marshal.Copy(buffer->value.cp.intPtr, (long[])this.value, 0, buffer->value.cp.count);
					return;
				}
				return;
			}
			else
			{
				if (propType == PropType.SysTimeArray)
				{
					int count3 = buffer->value.cp.count;
					DateTime[] array3 = new DateTime[count3];
					if (buffer->value.cp.count > 0)
					{
						long* ptr3 = (long*)((void*)buffer->value.cp.intPtr);
						for (int l = 0; l < count3; l++)
						{
							array3[l] = PropValue.LongAsDateTime(ptr3[l]);
						}
					}
					this.value = array3;
					return;
				}
				if (propType == PropType.GuidArray)
				{
					int count4 = buffer->value.cp.count;
					Guid[] array4 = new Guid[count4];
					Guid* ptr4 = (Guid*)((void*)buffer->value.cp.intPtr);
					for (int m = 0; m < count4; m++)
					{
						array4[m] = ptr4[m];
					}
					this.value = array4;
					return;
				}
				if (propType != PropType.BinaryArray)
				{
					goto IL_711;
				}
				int count5 = buffer->value.cp.count;
				byte[][] array5 = new byte[count5][];
				CountAndPtr* ptr5 = (CountAndPtr*)((void*)buffer->value.cp.intPtr);
				for (int n = 0; n < count5; n++)
				{
					array5[n] = new byte[ptr5[n].count];
					if (ptr5[n].count > 0)
					{
						Marshal.Copy(ptr5[n].intPtr, array5[n], 0, ptr5[n].count);
					}
				}
				this.value = array5;
				return;
			}
			IL_183:
			this.value = null;
			return;
			IL_711:
			throw MapiExceptionHelper.InvalidTypeException("Unsupported property type " + (int)(this.propTag & (PropTag)65535U) + " returned from server.");
		}

		public static PropValue Unmarshal(IntPtr propValueNative)
		{
			return PropValue.Unmarshal(propValueNative, false);
		}

		public unsafe static PropValue Unmarshal(IntPtr propValueNative, bool retainAnsiStrings)
		{
			return new PropValue((SPropValue*)propValueNative.ToPointer(), retainAnsiStrings);
		}

		internal unsafe void InitNativeValue(SPropValue* buffer)
		{
			PropType propType = this.PropType;
			switch (propType)
			{
			case PropType.Short:
				this.nativeValue.s = buffer->value.s;
				this.nativeValueType = PropValue.NativeType.Short;
				return;
			case PropType.Int:
			case PropType.Error:
			case PropType.Object:
				this.nativeValue.i = buffer->value.i;
				this.nativeValueType = PropValue.NativeType.Int;
				return;
			case PropType.Float:
				this.nativeValue.f = buffer->value.f;
				this.nativeValueType = PropValue.NativeType.Float;
				return;
			case PropType.Double:
			case PropType.AppTime:
				this.nativeValue.d = buffer->value.d;
				this.nativeValueType = PropValue.NativeType.Double;
				return;
			case PropType.Currency:
			case PropType.Long:
				this.nativeValue.l = buffer->value.l;
				this.nativeValueType = PropValue.NativeType.Long;
				return;
			case (PropType)8:
			case (PropType)9:
			case (PropType)12:
			case (PropType)14:
			case (PropType)15:
			case (PropType)16:
			case (PropType)17:
			case (PropType)18:
			case (PropType)19:
				break;
			case PropType.Boolean:
				this.nativeValue.s = buffer->value.s;
				this.nativeValueType = PropValue.NativeType.Short;
				return;
			default:
				if (propType == PropType.SysTime)
				{
					this.nativeValue.l = buffer->value.l;
					this.nativeValueType = PropValue.NativeType.Long;
					return;
				}
				break;
			}
			throw MapiExceptionHelper.InvalidTypeException("Unsupported property type " + (int)(this.propTag & (PropTag)65535U) + " returned from server.");
		}

		public override string ToString()
		{
			string arg;
			if (this.IsNull())
			{
				arg = "(null)";
			}
			else if (this.IsError())
			{
				arg = this.GetErrorValue().ToString("X8");
			}
			else
			{
				arg = this.Value.ToString();
			}
			return string.Format("{0} ({1}): {2}", this.PropTag.ToString(), ((int)this.PropTag).ToString("X8"), arg);
		}

		public bool IsNull()
		{
			return this.nativeValueType == PropValue.NativeType.Unused && this.value == null;
		}

		public bool IsError()
		{
			return this.PropType == PropType.Error;
		}

		public int GetErrorValue()
		{
			return this.GetInt();
		}

		public short GetShort()
		{
			if (this.nativeValueType == PropValue.NativeType.Unused)
			{
				return (short)this.value;
			}
			if (this.nativeValueType == PropValue.NativeType.Short)
			{
				return this.nativeValue.s;
			}
			if (this.nativeValueType == PropValue.NativeType.Int)
			{
				return (short)this.nativeValue.i;
			}
			if (this.nativeValueType == PropValue.NativeType.Float)
			{
				return (short)this.nativeValue.f;
			}
			if (this.nativeValueType == PropValue.NativeType.Double)
			{
				return (short)this.nativeValue.d;
			}
			if (this.nativeValueType == PropValue.NativeType.Long)
			{
				return (short)this.nativeValue.l;
			}
			throw MapiExceptionHelper.InvalidTypeException("Can't convert an incompatible type. Proptype:" + this.PropType.ToString() + " Native type: " + this.nativeValueType.ToString());
		}

		public int GetInt()
		{
			if (this.IsNull() && this.PropType == PropType.Object)
			{
				return 0;
			}
			if (this.nativeValueType == PropValue.NativeType.Unused)
			{
				return (int)this.value;
			}
			if (this.nativeValueType == PropValue.NativeType.Int)
			{
				return this.nativeValue.i;
			}
			if (this.nativeValueType == PropValue.NativeType.Short)
			{
				return (int)this.nativeValue.s;
			}
			if (this.nativeValueType == PropValue.NativeType.Float)
			{
				return (int)this.nativeValue.f;
			}
			if (this.nativeValueType == PropValue.NativeType.Double)
			{
				return (int)this.nativeValue.d;
			}
			if (this.nativeValueType == PropValue.NativeType.Long)
			{
				return (int)this.nativeValue.l;
			}
			throw MapiExceptionHelper.InvalidTypeException("Can't convert an incompatible type. Proptype:" + this.PropType.ToString() + " Native type: " + this.nativeValueType.ToString());
		}

		public int GetInt(int defaultValue)
		{
			if (this.IsError() || this.IsNull())
			{
				return defaultValue;
			}
			return this.GetInt();
		}

		public long GetLong()
		{
			if (this.nativeValueType == PropValue.NativeType.Unused)
			{
				if (this.value is DateTime)
				{
					return PropValue.DateTimeAsLong((DateTime)this.value);
				}
				return (long)this.value;
			}
			else
			{
				if (this.nativeValueType == PropValue.NativeType.Long)
				{
					return this.nativeValue.l;
				}
				if (this.nativeValueType == PropValue.NativeType.Short)
				{
					return (long)this.nativeValue.s;
				}
				if (this.nativeValueType == PropValue.NativeType.Int)
				{
					return (long)this.nativeValue.i;
				}
				if (this.nativeValueType == PropValue.NativeType.Float)
				{
					return (long)this.nativeValue.f;
				}
				if (this.nativeValueType == PropValue.NativeType.Double)
				{
					return (long)this.nativeValue.d;
				}
				throw MapiExceptionHelper.InvalidTypeException("Can't convert an incompatible type. Proptype:" + this.PropType.ToString() + " Native type: " + this.nativeValueType.ToString());
			}
		}

		public float GetFloat()
		{
			if (this.nativeValueType == PropValue.NativeType.Unused)
			{
				return (float)this.value;
			}
			if (this.nativeValueType == PropValue.NativeType.Float)
			{
				return this.nativeValue.f;
			}
			if (this.nativeValueType == PropValue.NativeType.Double)
			{
				return (float)this.nativeValue.d;
			}
			if (this.nativeValueType == PropValue.NativeType.Short)
			{
				return (float)this.nativeValue.s;
			}
			if (this.nativeValueType == PropValue.NativeType.Long)
			{
				return (float)this.nativeValue.l;
			}
			if (this.nativeValueType == PropValue.NativeType.Int)
			{
				return (float)this.nativeValue.i;
			}
			throw MapiExceptionHelper.InvalidTypeException("Can't convert an incompatible type. Proptype:" + this.PropType.ToString() + " Native type: " + this.nativeValueType.ToString());
		}

		public double GetDouble()
		{
			if (this.nativeValueType == PropValue.NativeType.Unused)
			{
				return (double)this.value;
			}
			if (this.nativeValueType == PropValue.NativeType.Double)
			{
				return this.nativeValue.d;
			}
			if (this.nativeValueType == PropValue.NativeType.Float)
			{
				return (double)this.nativeValue.f;
			}
			if (this.nativeValueType == PropValue.NativeType.Short)
			{
				return (double)this.nativeValue.s;
			}
			if (this.nativeValueType == PropValue.NativeType.Long)
			{
				return (double)this.nativeValue.l;
			}
			if (this.nativeValueType == PropValue.NativeType.Int)
			{
				return (double)this.nativeValue.i;
			}
			throw MapiExceptionHelper.InvalidTypeException("Can't convert an incompatible type. Proptype:" + this.PropType.ToString() + " Native type: " + this.nativeValueType.ToString());
		}

		public DateTime GetDateTime()
		{
			if (this.nativeValueType == PropValue.NativeType.Unused)
			{
				if (this.value is long)
				{
					return PropValue.LongAsDateTime((long)this.value);
				}
				return (DateTime)this.value;
			}
			else
			{
				if (this.nativeValueType == PropValue.NativeType.Long)
				{
					return PropValue.LongAsDateTime(this.nativeValue.l);
				}
				throw MapiExceptionHelper.InvalidCastException("To GetDateTime(), PropType can only be of DateTime or long format.");
			}
		}

		public bool GetBoolean()
		{
			if (this.nativeValueType == PropValue.NativeType.Unused)
			{
				return (bool)this.value;
			}
			return 0 != this.GetShort();
		}

		public string GetString()
		{
			return (string)this.Value;
		}

		public Guid GetGuid()
		{
			return (Guid)this.Value;
		}

		public short[] GetShortArray()
		{
			return (short[])this.Value;
		}

		public int[] GetIntArray()
		{
			return (int[])this.Value;
		}

		public float[] GetFloatArray()
		{
			return (float[])this.Value;
		}

		public double[] GetDoubleArray()
		{
			return (double[])this.Value;
		}

		public long[] GetLongArray()
		{
			long[] array = null;
			DateTime[] array2 = this.value as DateTime[];
			if (array2 != null)
			{
				array = new long[array2.Length];
				for (int i = 0; i < array.Length; i++)
				{
					try
					{
						array[i] = array2[i].ToFileTimeUtc();
					}
					catch (ArgumentOutOfRangeException)
					{
						array[i] = 0L;
					}
				}
			}
			else
			{
				array = (long[])this.value;
			}
			return array;
		}

		public DateTime[] GetDateTimeArray()
		{
			long[] array = this.value as long[];
			DateTime[] array2;
			if (array != null)
			{
				array2 = new DateTime[array.Length];
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i] = PropValue.LongAsDateTime(array[i]);
				}
			}
			else
			{
				array2 = (DateTime[])this.value;
			}
			return array2;
		}

		public bool[] GetBoolArray()
		{
			return (bool[])this.Value;
		}

		public byte[] GetBytes()
		{
			return (byte[])this.Value;
		}

		public string[] GetStringArray()
		{
			return (string[])this.Value;
		}

		public Guid[] GetGuidArray()
		{
			return (Guid[])this.Value;
		}

		public byte[][] GetBytesArray()
		{
			return (byte[][])this.Value;
		}

		public override bool Equals(object comparand)
		{
			return comparand is PropValue && this.Equals((PropValue)comparand);
		}

		public bool Equals(PropValue comparand)
		{
			return this.IsEqualTo(comparand);
		}

		public static bool Equals(PropValue v1, PropValue v2)
		{
			return v1.Equals(v2);
		}

		public override int GetHashCode()
		{
			return this.propTag.GetHashCode();
		}

		public bool IsEqualTo(PropValue propOther)
		{
			if (this.propTag != propOther.propTag)
			{
				return false;
			}
			PropType propType = this.PropType;
			if (propType <= PropType.Actions)
			{
				if (propType <= PropType.String)
				{
					switch (propType)
					{
					case PropType.Null:
						return true;
					case PropType.Short:
						return this.GetShort() == propOther.GetShort();
					case PropType.Int:
						return this.GetInt() == propOther.GetInt();
					case PropType.Float:
						return this.GetFloat() == propOther.GetFloat();
					case PropType.Double:
					case PropType.AppTime:
						return this.GetDouble() == propOther.GetDouble();
					case PropType.Currency:
						break;
					case (PropType)8:
					case (PropType)9:
						goto IL_55C;
					case PropType.Error:
						return this.GetErrorValue() == propOther.GetErrorValue();
					case PropType.Boolean:
						return this.GetBoolean() == propOther.GetBoolean();
					default:
						if (propType != PropType.Long)
						{
							switch (propType)
							{
							case PropType.AnsiString:
							case PropType.String:
								return this.GetString().CompareTo(propOther.GetString()) == 0;
							default:
								goto IL_55C;
							}
						}
						break;
					}
					return this.GetLong() == propOther.GetLong();
				}
				if (propType == PropType.SysTime)
				{
					return this.GetDateTime().CompareTo(propOther.GetDateTime()) == 0;
				}
				if (propType == PropType.Guid)
				{
					return this.GetGuid().CompareTo(propOther.GetGuid()) == 0;
				}
				switch (propType)
				{
				case PropType.Restriction:
					return Restriction.Equals((Restriction)this.value, (Restriction)propOther.value);
				case PropType.Actions:
				{
					if (this.value == propOther.value)
					{
						return true;
					}
					if (this.value == null || propOther.value == null)
					{
						return false;
					}
					RuleAction[] array = (RuleAction[])this.value;
					RuleAction[] array2 = (RuleAction[])propOther.Value;
					if (array.Length != array2.Length)
					{
						return false;
					}
					for (int i = 0; i < array.Length; i++)
					{
						if (!RuleAction.Equals(array[i], array2[i]))
						{
							return false;
						}
					}
					return true;
				}
				default:
					goto IL_55C;
				}
			}
			else
			{
				if (propType <= PropType.LongArray)
				{
					if (propType != PropType.Binary)
					{
						switch (propType)
						{
						case PropType.ShortArray:
						{
							short[] shortArray = this.GetShortArray();
							short[] shortArray2 = propOther.GetShortArray();
							int num = shortArray.Length;
							int num2 = shortArray2.Length;
							if (num != num2)
							{
								return false;
							}
							for (int j = 0; j < num; j++)
							{
								if (shortArray[j] != shortArray2[j])
								{
									return false;
								}
							}
							return true;
						}
						case PropType.IntArray:
						{
							int[] intArray = this.GetIntArray();
							int[] intArray2 = propOther.GetIntArray();
							int num3 = intArray.Length;
							int num4 = intArray2.Length;
							if (num3 != num4)
							{
								return false;
							}
							for (int k = 0; k < num3; k++)
							{
								if (intArray[k] != intArray2[k])
								{
									return false;
								}
							}
							return true;
						}
						case PropType.FloatArray:
						{
							float[] floatArray = this.GetFloatArray();
							float[] floatArray2 = propOther.GetFloatArray();
							int num5 = floatArray.Length;
							int num6 = floatArray2.Length;
							if (num5 != num6)
							{
								return false;
							}
							for (int l = 0; l < num5; l++)
							{
								if (floatArray[l] != floatArray2[l])
								{
									return false;
								}
							}
							return true;
						}
						case PropType.DoubleArray:
						case PropType.AppTimeArray:
						{
							double[] doubleArray = this.GetDoubleArray();
							double[] doubleArray2 = propOther.GetDoubleArray();
							int num7 = doubleArray.Length;
							int num8 = doubleArray2.Length;
							if (num7 != num8)
							{
								return false;
							}
							for (int m = 0; m < num7; m++)
							{
								if (doubleArray[m] != doubleArray2[m])
								{
									return false;
								}
							}
							return true;
						}
						case PropType.CurrencyArray:
							break;
						case (PropType)4104:
						case (PropType)4105:
						case (PropType)4106:
						case (PropType)4107:
						case (PropType)4108:
							goto IL_55C;
						case PropType.ObjectArray:
							return true;
						default:
							if (propType != PropType.LongArray)
							{
								goto IL_55C;
							}
							break;
						}
					}
					else
					{
						byte[] bytes = this.GetBytes();
						byte[] bytes2 = propOther.GetBytes();
						int num9 = bytes.Length;
						int num10 = bytes2.Length;
						if (num9 != num10)
						{
							return false;
						}
						for (int n = 0; n < num9; n++)
						{
							if (bytes[n] != bytes2[n])
							{
								return false;
							}
						}
						return true;
					}
				}
				else if (propType <= PropType.SysTimeArray)
				{
					switch (propType)
					{
					case PropType.AnsiStringArray:
					case PropType.StringArray:
					{
						string[] stringArray = this.GetStringArray();
						string[] stringArray2 = propOther.GetStringArray();
						int num11 = stringArray.Length;
						int num12 = stringArray2.Length;
						if (num11 != num12)
						{
							return false;
						}
						for (int num13 = 0; num13 < num11; num13++)
						{
							if (stringArray[num13].CompareTo(stringArray2[num13]) != 0)
							{
								return false;
							}
						}
						return true;
					}
					default:
						if (propType != PropType.SysTimeArray)
						{
							goto IL_55C;
						}
						break;
					}
				}
				else if (propType != PropType.GuidArray)
				{
					if (propType != PropType.BinaryArray)
					{
						goto IL_55C;
					}
					byte[][] bytesArray = this.GetBytesArray();
					byte[][] bytesArray2 = propOther.GetBytesArray();
					int num14 = bytesArray.Length;
					int num15 = bytesArray2.Length;
					if (num14 != num15)
					{
						return false;
					}
					for (int num16 = 0; num16 < num14; num16++)
					{
						byte[] array3 = bytesArray[num16];
						byte[] array4 = bytesArray2[num16];
						int num17 = array3.Length;
						int num18 = array4.Length;
						if (num17 != num18)
						{
							return false;
						}
						for (int num19 = 0; num19 < num17; num19++)
						{
							if (array3[num19] != array4[num19])
							{
								return false;
							}
						}
					}
					return true;
				}
				else
				{
					Guid[] guidArray = this.GetGuidArray();
					Guid[] guidArray2 = propOther.GetGuidArray();
					int num20 = guidArray.Length;
					int num21 = guidArray2.Length;
					if (num20 != num21)
					{
						return false;
					}
					for (int num22 = 0; num22 < num20; num22++)
					{
						if (guidArray[num22].CompareTo(guidArray2[num22]) != 0)
						{
							return false;
						}
					}
					return true;
				}
				long[] longArray = this.GetLongArray();
				long[] longArray2 = propOther.GetLongArray();
				int num23 = longArray.Length;
				int num24 = longArray2.Length;
				if (num23 != num24)
				{
					return false;
				}
				for (int num25 = 0; num25 < num23; num25++)
				{
					if (longArray[num25] != longArray2[num25])
					{
						return false;
					}
				}
				return true;
			}
			return true;
			IL_55C:
			throw MapiExceptionHelper.InvalidTypeException("Unable to compare unsupported property type " + (int)(this.propTag & (PropTag)65535U) + ".");
		}

		public int GetBytesToMarshal()
		{
			int num = SPropValue.SizeOf + 7 & -8;
			PropType propType = this.PropType;
			if (propType <= PropType.Binary)
			{
				if (propType <= PropType.SysTime)
				{
					switch (propType)
					{
					case PropType.Null:
					case PropType.Short:
					case PropType.Int:
					case PropType.Float:
					case PropType.Double:
					case PropType.Currency:
					case PropType.AppTime:
					case PropType.Error:
					case PropType.Boolean:
					case PropType.Object:
					case PropType.Long:
						return num;
					case (PropType)8:
					case (PropType)9:
					case (PropType)12:
					case (PropType)14:
					case (PropType)15:
					case (PropType)16:
					case (PropType)17:
					case (PropType)18:
					case (PropType)19:
						break;
					default:
						switch (propType)
						{
						case PropType.AnsiString:
							if (this.value == null)
							{
								return num;
							}
							if (this.value is string)
							{
								return num + (((string)this.value).Length + 1 + 7 & -8);
							}
							return num + (((byte[])this.value).Length + 1 + 7 & -8);
						case PropType.String:
							if (this.value != null)
							{
								return num + ((((string)this.value).Length + 1) * 2 + 7 & -8);
							}
							return num;
						default:
							if (propType == PropType.SysTime)
							{
								return num;
							}
							break;
						}
						break;
					}
				}
				else
				{
					if (propType == PropType.Guid)
					{
						return num + (Marshal.SizeOf(typeof(Guid)) + 7 & -8);
					}
					switch (propType)
					{
					case PropType.Restriction:
						if (this.value != null)
						{
							return num + ((Restriction)this.value).GetBytesToMarshal();
						}
						return num;
					case PropType.Actions:
						return num + RuleActions.GetBytesToMarshal((RuleAction[])this.value);
					default:
						if (propType == PropType.Binary)
						{
							if (this.value != null)
							{
								return num + (((byte[])this.value).Length + 7 & -8);
							}
							return num;
						}
						break;
					}
				}
			}
			else
			{
				if (propType <= PropType.StringArray)
				{
					switch (propType)
					{
					case PropType.ShortArray:
						return num + (((short[])this.value).Length * 2 + 7 & -8);
					case PropType.IntArray:
						return num + (((int[])this.value).Length * 4 + 7 & -8);
					case PropType.FloatArray:
						return num + (((float[])this.value).Length * 4 + 7 & -8);
					case PropType.DoubleArray:
					case PropType.AppTimeArray:
						return num + (((double[])this.value).Length * 8 + 7 & -8);
					case PropType.CurrencyArray:
						break;
					case (PropType)4104:
					case (PropType)4105:
					case (PropType)4106:
					case (PropType)4107:
					case (PropType)4108:
						goto IL_4ED;
					case PropType.ObjectArray:
						return num;
					default:
						if (propType != PropType.LongArray)
						{
							switch (propType)
							{
							case PropType.AnsiStringArray:
								if (this.value is string[])
								{
									num += (((string[])this.value).Length * Marshal.SizeOf(typeof(IntPtr)) + 7 & -8);
									foreach (string text in (string[])this.value)
									{
										num += (text.Length + 1 + 7 & -8);
									}
									return num;
								}
								num += (((byte[][])this.value).Length * Marshal.SizeOf(typeof(IntPtr)) + 7 & -8);
								foreach (byte[] array3 in (byte[][])this.value)
								{
									num += (array3.Length + 1 + 7 & -8);
								}
								return num;
							case PropType.StringArray:
								num += (((string[])this.value).Length * Marshal.SizeOf(typeof(IntPtr)) + 7 & -8);
								foreach (string text2 in (string[])this.value)
								{
									num += ((text2.Length + 1) * 2 + 7 & -8);
								}
								return num;
							default:
								goto IL_4ED;
							}
						}
						break;
					}
					return num + (((long[])this.value).Length * 8 + 7 & -8);
				}
				if (propType != PropType.SysTimeArray)
				{
					if (propType == PropType.GuidArray)
					{
						return num + (((Guid[])this.value).Length * Marshal.SizeOf(typeof(Guid)) + 7 & -8);
					}
					if (propType == PropType.BinaryArray)
					{
						num += (((byte[][])this.value).Length * Marshal.SizeOf(typeof(CountAndPtr)) + 7 & -8);
						foreach (byte[] array6 in (byte[][])this.value)
						{
							num += (array6.Length + 7 & -8);
						}
						return num;
					}
				}
				else
				{
					if (this.value is DateTime[])
					{
						return num + (((DateTime[])this.value).Length * 8 + 7 & -8);
					}
					if (this.value is long[])
					{
						return num + (((long[])this.value).Length * 8 + 7 & -8);
					}
					throw MapiExceptionHelper.IncompatiblePropTypeException("Only DateTime[] or long[] can stored as PropType.SysTimeArray.");
				}
			}
			IL_4ED:
			throw MapiExceptionHelper.IncompatiblePropTypeException("Unable to marshal unsupported property type " + (int)(this.propTag & (PropTag)65535U) + ".");
		}

		internal unsafe void MarshalToNative(SPropValue* pspv, ref byte* pExtra)
		{
			pspv->ulPropTag = (int)this.propTag;
			PropType propType = this.PropType;
			if (propType <= PropType.Binary)
			{
				if (propType <= PropType.SysTime)
				{
					switch (propType)
					{
					case PropType.Null:
					case PropType.Object:
						goto IL_163;
					case PropType.Short:
						pspv->value.s = this.GetShort();
						return;
					case PropType.Int:
					case PropType.Error:
						pspv->value.i = this.GetInt();
						return;
					case PropType.Float:
						pspv->value.f = this.GetFloat();
						return;
					case PropType.Double:
					case PropType.AppTime:
						pspv->value.d = this.GetDouble();
						return;
					case PropType.Currency:
					case PropType.Long:
						break;
					case (PropType)8:
					case (PropType)9:
					case (PropType)12:
					case (PropType)14:
					case (PropType)15:
					case (PropType)16:
					case (PropType)17:
					case (PropType)18:
					case (PropType)19:
						goto IL_C6C;
					case PropType.Boolean:
						pspv->value.s = (this.GetBoolean() ? 1 : 0);
						return;
					default:
						switch (propType)
						{
						case PropType.AnsiString:
						{
							if (this.value == null)
							{
								pspv->value.intPtr = (IntPtr)null;
								return;
							}
							byte* ptr = pExtra;
							pspv->value.intPtr = (IntPtr)((void*)ptr);
							if (this.value is string)
							{
								pExtra += (IntPtr)(((string)this.value).Length + 1 + 7 & -8);
								foreach (char c in (string)this.value)
								{
									*(ptr++) = (byte)c;
								}
								*ptr = 0;
								return;
							}
							pExtra += (IntPtr)(((byte[])this.value).Length + 1 + 7 & -8);
							foreach (byte b in (byte[])this.value)
							{
								*(ptr++) = b;
							}
							*ptr = 0;
							return;
						}
						case PropType.String:
							if (this.value != null)
							{
								char* ptr2 = pExtra;
								pspv->value.intPtr = (IntPtr)((void*)ptr2);
								pExtra += (IntPtr)((((string)this.value).Length + 1) * 2 + 7 & -8);
								foreach (char c2 in (string)this.value)
								{
									*(ptr2++) = c2;
								}
								*ptr2 = '\0';
								return;
							}
							pspv->value.intPtr = (IntPtr)null;
							return;
						default:
							if (propType != PropType.SysTime)
							{
								goto IL_C6C;
							}
							break;
						}
						break;
					}
					pspv->value.l = this.GetLong();
					return;
				}
				if (propType == PropType.Guid)
				{
					Guid* ptr3 = pExtra;
					pspv->value.intPtr = (IntPtr)((void*)ptr3);
					pExtra += (IntPtr)(Marshal.SizeOf(typeof(Guid)) + 7 & -8);
					*ptr3 = (Guid)this.value;
					return;
				}
				switch (propType)
				{
				case PropType.Restriction:
					if (this.value != null)
					{
						SRestriction* psr = pExtra;
						pExtra += (IntPtr)(SRestriction.SizeOf + 7 & -8);
						((Restriction)this.value).MarshalToNative(psr, ref pExtra);
						pspv->value.intPtr = (IntPtr)((void*)psr);
						return;
					}
					pspv->value.intPtr = IntPtr.Zero;
					return;
				case PropType.Actions:
					pspv->value.intPtr = (IntPtr)pExtra;
					RuleActions.MarshalToNative(ref pExtra, (RuleAction[])this.value);
					return;
				default:
					if (propType != PropType.Binary)
					{
						goto IL_C6C;
					}
					if (this.value != null)
					{
						byte* ptr4 = pExtra;
						pExtra += (IntPtr)(((byte[])this.value).Length + 7 & -8);
						pspv->value.cp.count = ((byte[])this.value).Length;
						pspv->value.cp.intPtr = (IntPtr)((void*)ptr4);
						Marshal.Copy((byte[])this.value, 0, (IntPtr)((void*)ptr4), ((byte[])this.value).Length);
						return;
					}
					pspv->value.cp.count = 0;
					pspv->value.cp.intPtr = (IntPtr)null;
					return;
				}
			}
			else
			{
				if (propType <= PropType.StringArray)
				{
					switch (propType)
					{
					case PropType.ShortArray:
					{
						short* ptr5 = pExtra;
						pExtra += (IntPtr)(((short[])this.value).Length * 2 + 7 & -8);
						pspv->value.cp.count = ((short[])this.value).Length;
						pspv->value.cp.intPtr = (IntPtr)((void*)ptr5);
						Marshal.Copy((short[])this.value, 0, (IntPtr)((void*)ptr5), ((short[])this.value).Length);
						return;
					}
					case PropType.IntArray:
					{
						int* ptr6 = pExtra;
						pExtra += (IntPtr)(((int[])this.value).Length * 4 + 7 & -8);
						pspv->value.cp.count = ((int[])this.value).Length;
						pspv->value.cp.intPtr = (IntPtr)((void*)ptr6);
						Marshal.Copy((int[])this.value, 0, (IntPtr)((void*)ptr6), ((int[])this.value).Length);
						return;
					}
					case PropType.FloatArray:
					{
						float* ptr7 = pExtra;
						pExtra += (IntPtr)(((float[])this.value).Length * 4 + 7 & -8);
						pspv->value.cp.count = ((float[])this.value).Length;
						pspv->value.cp.intPtr = (IntPtr)((void*)ptr7);
						Marshal.Copy((float[])this.value, 0, (IntPtr)((void*)ptr7), ((float[])this.value).Length);
						return;
					}
					case PropType.DoubleArray:
					case PropType.AppTimeArray:
					{
						double* ptr8 = pExtra;
						pExtra += (IntPtr)(((double[])this.value).Length * 8 + 7 & -8);
						pspv->value.cp.count = ((double[])this.value).Length;
						pspv->value.cp.intPtr = (IntPtr)((void*)ptr8);
						Marshal.Copy((double[])this.value, 0, (IntPtr)((void*)ptr8), ((double[])this.value).Length);
						return;
					}
					case PropType.CurrencyArray:
						break;
					case (PropType)4104:
					case (PropType)4105:
					case (PropType)4106:
					case (PropType)4107:
					case (PropType)4108:
						goto IL_C6C;
					case PropType.ObjectArray:
						goto IL_163;
					default:
						if (propType != PropType.LongArray)
						{
							switch (propType)
							{
							case PropType.AnsiStringArray:
							{
								byte** ptr9 = pExtra;
								if (this.value is string[])
								{
									pExtra += (IntPtr)(((string[])this.value).Length * Marshal.SizeOf(typeof(IntPtr)) + 7 & -8);
									pspv->value.cp.count = ((string[])this.value).Length;
									pspv->value.cp.intPtr = (IntPtr)((void*)ptr9);
									foreach (string text3 in (string[])this.value)
									{
										byte* ptr10 = pExtra;
										pExtra += (IntPtr)(text3.Length + 1 + 7 & -8);
										byte** ptr11 = ptr9;
										ptr9 = ptr11 + sizeof(byte*) / sizeof(byte*);
										*(IntPtr*)ptr11 = ptr10;
										foreach (char c3 in text3)
										{
											*(ptr10++) = (byte)c3;
										}
										*ptr10 = 0;
									}
									return;
								}
								pExtra += (IntPtr)(((byte[][])this.value).Length * Marshal.SizeOf(typeof(IntPtr)) + 7 & -8);
								pspv->value.cp.count = ((byte[][])this.value).Length;
								pspv->value.cp.intPtr = (IntPtr)((void*)ptr9);
								foreach (byte[] array4 in (byte[][])this.value)
								{
									byte* ptr12 = pExtra;
									pExtra += (IntPtr)(array4.Length + 1 + 7 & -8);
									byte** ptr13 = ptr9;
									ptr9 = ptr13 + sizeof(byte*) / sizeof(byte*);
									*(IntPtr*)ptr13 = ptr12;
									foreach (byte b2 in array4)
									{
										*(ptr12++) = b2;
									}
									*ptr12 = 0;
								}
								return;
							}
							case PropType.StringArray:
							{
								char** ptr14 = pExtra;
								pExtra += (IntPtr)(((string[])this.value).Length * Marshal.SizeOf(typeof(IntPtr)) + 7 & -8);
								pspv->value.cp.count = ((string[])this.value).Length;
								pspv->value.cp.intPtr = (IntPtr)((void*)ptr14);
								foreach (string text5 in (string[])this.value)
								{
									char* ptr15 = pExtra;
									pExtra += (IntPtr)((text5.Length + 1) * 2 + 7 & -8);
									char** ptr16 = ptr14;
									ptr14 = ptr16 + sizeof(char*) / sizeof(char*);
									*(IntPtr*)ptr16 = ptr15;
									foreach (char c4 in text5)
									{
										*(ptr15++) = c4;
									}
									*ptr15 = '\0';
								}
								return;
							}
							default:
								goto IL_C6C;
							}
						}
						break;
					}
					long* ptr17 = pExtra;
					pExtra += (IntPtr)(((long[])this.value).Length * 8 + 7 & -8);
					pspv->value.cp.count = ((long[])this.value).Length;
					pspv->value.cp.intPtr = (IntPtr)((void*)ptr17);
					Marshal.Copy((long[])this.value, 0, (IntPtr)((void*)ptr17), ((long[])this.value).Length);
					return;
				}
				if (propType != PropType.SysTimeArray)
				{
					if (propType == PropType.GuidArray)
					{
						Guid* ptr18 = pExtra;
						pExtra += (IntPtr)(((Guid[])this.value).Length * Marshal.SizeOf(typeof(Guid)) + 7 & -8);
						pspv->value.cp.count = ((Guid[])this.value).Length;
						pspv->value.cp.intPtr = (IntPtr)((void*)ptr18);
						foreach (Guid guid in (Guid[])this.value)
						{
							*(ptr18++) = guid;
						}
						return;
					}
					if (propType != PropType.BinaryArray)
					{
						goto IL_C6C;
					}
					CountAndPtr* ptr19 = pExtra;
					pExtra += (IntPtr)(((byte[][])this.value).Length * Marshal.SizeOf(typeof(CountAndPtr)) + 7 & -8);
					pspv->value.cp.count = ((byte[][])this.value).Length;
					pspv->value.cp.intPtr = (IntPtr)((void*)ptr19);
					foreach (byte[] array9 in (byte[][])this.value)
					{
						byte* ptr20 = pExtra;
						pExtra += (IntPtr)(array9.Length + 7 & -8);
						ptr19->count = array9.Length;
						ptr19->intPtr = (IntPtr)((void*)ptr20);
						ptr19++;
						foreach (byte b3 in array9)
						{
							*(ptr20++) = b3;
						}
					}
					return;
				}
				else
				{
					if (this.value is DateTime[])
					{
						long* ptr21 = pExtra;
						pExtra += (IntPtr)(((DateTime[])this.value).Length * 8 + 7 & -8);
						pspv->value.cp.count = ((DateTime[])this.value).Length;
						pspv->value.cp.intPtr = (IntPtr)((void*)ptr21);
						int num7 = 0;
						while (num7 < pspv->value.cp.count)
						{
							try
							{
								*ptr21 = ((DateTime[])this.value)[num7].ToFileTimeUtc();
							}
							catch (ArgumentOutOfRangeException)
							{
								*ptr21 = 0L;
							}
							num7++;
							ptr21++;
						}
						return;
					}
					if (this.value is long[])
					{
						long* ptr22 = pExtra;
						pExtra += (IntPtr)(((long[])this.value).Length * 8 + 7 & -8);
						pspv->value.cp.count = ((long[])this.value).Length;
						pspv->value.cp.intPtr = (IntPtr)((void*)ptr22);
						Marshal.Copy((long[])this.value, 0, (IntPtr)((void*)ptr22), ((long[])this.value).Length);
						return;
					}
					throw MapiExceptionHelper.InvalidCastException("PropType.SysTimeArray can only be of DateTime[] or long[] format.");
				}
			}
			IL_163:
			pspv->value.intPtr = IntPtr.Zero;
			return;
			IL_C6C:
			throw MapiExceptionHelper.InvalidTypeException("Unable to marshal unsupported property type " + (int)(this.propTag & (PropTag)65535U) + ".");
		}

		public static int GetBytesToMarshal(ICollection<PropValue> pva)
		{
			int num = 0;
			foreach (PropValue propValue in pva)
			{
				num += propValue.GetBytesToMarshal();
			}
			return num;
		}

		public unsafe static void MarshalToNative(ICollection<PropValue> pva, SafeHandle handle)
		{
			PropValue.MarshalToNative(pva, (byte*)handle.DangerousGetHandle().ToPointer());
		}

		internal unsafe static void MarshalToNative(ICollection<PropValue> pva, byte* pb)
		{
			byte* ptr = pb + (SPropValue.SizeOf * pva.Count + 7 & -8);
			SPropValue* ptr2 = (SPropValue*)pb;
			foreach (PropValue propValue in pva)
			{
				propValue.MarshalToNative(ptr2, ref ptr);
				ptr2++;
			}
		}

		internal static long DateTimeAsLong(DateTime dateTime)
		{
			long result = 0L;
			if (dateTime != DateTime.MinValue)
			{
				try
				{
					result = dateTime.ToFileTimeUtc();
				}
				catch (ArgumentOutOfRangeException)
				{
					result = 0L;
				}
			}
			return result;
		}

		internal static DateTime LongAsDateTime(long longValue)
		{
			DateTime result;
			try
			{
				if (longValue < 0L || longValue > 2650467743999999999L)
				{
					result = DateTime.MaxValue;
				}
				else
				{
					long ticks = longValue + 504911232000000000L;
					result = new DateTime(ticks, DateTimeKind.Utc);
				}
			}
			catch (ArgumentOutOfRangeException)
			{
				result = DateTime.MaxValue;
			}
			return result;
		}

		internal unsafe static byte[] GetAnsiStringAsByteArray(IntPtr str)
		{
			if (str == IntPtr.Zero)
			{
				return null;
			}
			int num = 0;
			byte* ptr = (byte*)str.ToPointer();
			while (*(ptr++) != 0)
			{
				num++;
			}
			byte[] array = new byte[num];
			Marshal.Copy(str, array, 0, num);
			return array;
		}

		private static readonly object True = true;

		private static readonly object False = false;

		private readonly PropTag propTag;

		private object value;

		private _PV nativeValue;

		private PropValue.NativeType nativeValueType;

		private enum NativeType
		{
			Unused,
			Short,
			Int,
			Float,
			Double,
			Long
		}
	}
}
