using System;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.RpcClientAccess
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct PropertyValue : IFormattable
	{
		public bool IsError
		{
			get
			{
				return this.propertyTag.PropertyType == PropertyType.Error;
			}
		}

		public bool IsNullValue
		{
			get
			{
				return !this.IsError && this.unionVal.Value == null;
			}
		}

		public bool IsNotFound
		{
			get
			{
				return this.IsError && this.unionVal.ErrorCode == (ErrorCode)2147746063U;
			}
		}

		public PropertyTag PropertyTag
		{
			get
			{
				return this.propertyTag;
			}
		}

		internal ErrorCode ErrorCodeValue
		{
			get
			{
				if (!this.IsError)
				{
					throw new InvalidOperationException("Property is not an error type");
				}
				return this.unionVal.ErrorCode;
			}
		}

		public object Value
		{
			get
			{
				if (this.IsError)
				{
					return this.unionVal.ErrorCode;
				}
				String8 @string = this.unionVal.Value as String8;
				if (@string != null)
				{
					return @string.StringValue;
				}
				String8[] array = this.unionVal.Value as String8[];
				if (array != null)
				{
					return array.Select(delegate(String8 x)
					{
						if (x == null)
						{
							return null;
						}
						return x.StringValue;
					}).ToArray<string>();
				}
				return this.unionVal.Value;
			}
		}

		public bool CanGetValue<T>()
		{
			return !this.IsError && this.Value is T;
		}

		public T GetServerValue<T>()
		{
			if (this.CanGetValue<T>())
			{
				return (T)((object)this.Value);
			}
			throw new RopExecutionException(string.Format("RpcClientAccess expects a value convertible to {0}, but the Store returned {1} instead", typeof(T), this), (ErrorCode)2147746075U);
		}

		public T GetValue<T>()
		{
			if (this.CanGetValue<T>())
			{
				return (T)((object)this.Value);
			}
			throw new UnexpectedPropertyTypeException(typeof(T), this);
		}

		public T GetValueAssert<T>()
		{
			if (!this.CanGetValue<T>())
			{
				ExAssert.RetailAssert(false, "Unable to cast value to {0}", new object[]
				{
					typeof(T)
				});
			}
			return (T)((object)this.Value);
		}

		public PropertyValue(PropertyTag propertyTag, object value)
		{
			this = new PropertyValue(propertyTag, value, false);
		}

		private PropertyValue(PropertyTag propertyTag, object value, bool allowNullValue)
		{
			if (!allowNullValue && propertyTag.PropertyType != PropertyType.Null && value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.propertyTag = propertyTag;
			this.unionVal.ErrorCode = ErrorCode.None;
			this.unionVal.Value = value;
			PropertyType propertyType = propertyTag.PropertyType;
			if (propertyType <= PropertyType.Binary)
			{
				if (propertyType <= PropertyType.SysTime)
				{
					switch (propertyType)
					{
					case PropertyType.Unspecified:
					case (PropertyType)8:
					case (PropertyType)9:
					case (PropertyType)12:
					case (PropertyType)14:
					case (PropertyType)15:
					case (PropertyType)16:
					case (PropertyType)17:
					case (PropertyType)18:
					case (PropertyType)19:
						break;
					case PropertyType.Null:
						this.CheckValueTypeOnConstruction(value == null);
						return;
					case PropertyType.Int16:
						this.CheckValueTypeOnConstruction(value is short);
						return;
					case PropertyType.Int32:
						this.CheckValueTypeOnConstruction(value is int);
						return;
					case PropertyType.Float:
						this.CheckValueTypeOnConstruction(value is float);
						return;
					case PropertyType.Double:
					case PropertyType.AppTime:
						this.CheckValueTypeOnConstruction(value is double);
						return;
					case PropertyType.Currency:
					case PropertyType.Int64:
						this.CheckValueTypeOnConstruction(value is long);
						return;
					case PropertyType.Error:
						this.CheckValueTypeOnConstruction(value is ErrorCode || value is uint);
						this.unionVal.Value = null;
						this.unionVal.ErrorCode = (ErrorCode)value;
						return;
					case PropertyType.Bool:
						this.CheckValueTypeOnConstruction(value is bool);
						return;
					case PropertyType.Object:
						return;
					default:
						switch (propertyType)
						{
						case PropertyType.String8:
						{
							this.CheckValueTypeOnConstruction(value is String8 || value is string || (allowNullValue && value == null));
							string text = value as string;
							if (text != null)
							{
								this.unionVal.Value = new String8(text);
								return;
							}
							return;
						}
						case PropertyType.Unicode:
							this.CheckValueTypeOnConstruction(value is string || (allowNullValue && value == null));
							return;
						default:
							if (propertyType == PropertyType.SysTime)
							{
								this.CheckValueTypeOnConstruction(value is ExDateTime);
								return;
							}
							break;
						}
						break;
					}
				}
				else
				{
					if (propertyType != PropertyType.Guid)
					{
						switch (propertyType)
						{
						case PropertyType.ServerId:
							break;
						case (PropertyType)252:
							goto IL_455;
						case PropertyType.Restriction:
							this.CheckValueTypeOnConstruction(value is Restriction || (allowNullValue && value == null));
							return;
						case PropertyType.Actions:
							this.CheckValueTypeOnConstruction(value is RuleAction[] || (allowNullValue && value == null));
							return;
						default:
							if (propertyType != PropertyType.Binary)
							{
								goto IL_455;
							}
							break;
						}
						this.CheckValueTypeOnConstruction(value is byte[] || (allowNullValue && value == null));
						return;
					}
					this.CheckValueTypeOnConstruction(value is Guid || (allowNullValue && value == null));
					return;
				}
			}
			else
			{
				if (propertyType <= PropertyType.MultiValueUnicode)
				{
					switch (propertyType)
					{
					case PropertyType.MultiValueInt16:
						this.CheckValueTypeOnConstruction(value is short[] || (allowNullValue && value == null));
						return;
					case PropertyType.MultiValueInt32:
						this.CheckValueTypeOnConstruction(value is int[] || (allowNullValue && value == null));
						return;
					case PropertyType.MultiValueFloat:
						this.CheckValueTypeOnConstruction(value is float[] || (allowNullValue && value == null));
						return;
					case PropertyType.MultiValueDouble:
					case PropertyType.MultiValueAppTime:
						this.CheckValueTypeOnConstruction(value is double[] || (allowNullValue && value == null));
						return;
					case PropertyType.MultiValueCurrency:
						break;
					default:
						if (propertyType != PropertyType.MultiValueInt64)
						{
							switch (propertyType)
							{
							case PropertyType.MultiValueString8:
							{
								this.CheckValueTypeOnConstruction(value is String8[] || value is string[] || (allowNullValue && value == null));
								string[] array = value as string[];
								if (array != null)
								{
									this.unionVal.Value = array.Select(delegate(string x)
									{
										if (x == null)
										{
											return null;
										}
										return new String8(x);
									}).ToArray<String8>();
									return;
								}
								return;
							}
							case PropertyType.MultiValueUnicode:
								this.CheckValueTypeOnConstruction(value is string[] || (allowNullValue && value == null));
								return;
							default:
								goto IL_455;
							}
						}
						break;
					}
					this.CheckValueTypeOnConstruction(value is long[] || (allowNullValue && value == null));
					return;
				}
				if (propertyType == PropertyType.MultiValueSysTime)
				{
					this.CheckValueTypeOnConstruction(value is ExDateTime[] || (allowNullValue && value == null));
					return;
				}
				if (propertyType == PropertyType.MultiValueGuid)
				{
					this.CheckValueTypeOnConstruction(value is Guid[] || (allowNullValue && value == null));
					return;
				}
				if (propertyType == PropertyType.MultiValueBinary)
				{
					this.CheckValueTypeOnConstruction(value is byte[][] || (allowNullValue && value == null));
					return;
				}
			}
			IL_455:
			throw new NotSupportedException(string.Format("Property type not supported: {0}.", propertyTag.PropertyType));
		}

		private PropertyValue(PropertyId propertyId, ErrorCode value)
		{
			this.propertyTag = new PropertyTag(propertyId, PropertyType.Error);
			this.unionVal.Value = null;
			this.unionVal.ErrorCode = value;
		}

		public PropertyValue CloneAs(PropertyTag propertyTag)
		{
			if (this.IsNullValue)
			{
				return PropertyValue.NullValue(propertyTag);
			}
			return new PropertyValue(propertyTag, this.Value);
		}

		public PropertyValue CloneAs(PropertyId propertyId)
		{
			return this.CloneAs(new PropertyTag(propertyId, this.PropertyTag.PropertyType));
		}

		public static PropertyValue Error(PropertyId propertyId, ErrorCode errorCode)
		{
			return new PropertyValue(propertyId, errorCode);
		}

		public static PropertyValue NullValue(PropertyTag propertyTag)
		{
			return new PropertyValue(propertyTag, null, true);
		}

		internal static PropertyValue CreateNotEnoughMemory(PropertyId propertyId)
		{
			return new PropertyValue(PropertyTag.CreateError(propertyId), (ErrorCode)2147942414U);
		}

		internal static PropertyValue CreateNotFound(PropertyId propertyId)
		{
			return new PropertyValue(PropertyTag.CreateError(propertyId), (ErrorCode)2147746063U);
		}

		internal static bool IsSupportedPropertyType(PropertyTag propertyTag)
		{
			return EnumValidator.IsValidValue<PropertyType>(propertyTag.PropertyType);
		}

		public override string ToString()
		{
			return this.ToString("B", null);
		}

		internal void AppendToString(StringBuilder stringBuilder)
		{
			this.AppendToString(stringBuilder, "G", null);
		}

		internal void AppendToString(StringBuilder stringBuilder, string format, IFormatProvider fp)
		{
			if (format != null)
			{
				if (!(format == "G"))
				{
					if (!(format == "B"))
					{
					}
				}
				else
				{
					stringBuilder.Append("Tag=").Append(this.propertyTag.ToString());
					if (this.IsError)
					{
						stringBuilder.Append(" Error=");
						stringBuilder.Append(this.unionVal.ErrorCode.ToString());
						return;
					}
					stringBuilder.Append(" Value=");
					Util.AppendToString(stringBuilder, this.unionVal.Value);
					return;
				}
			}
			stringBuilder.AppendFormat("[{0} : ", this.PropertyTag);
			if (this.IsError)
			{
				stringBuilder.Append(this.unionVal.ErrorCode.ToString());
			}
			else
			{
				Util.AppendToString(stringBuilder, this.unionVal.Value);
			}
			stringBuilder.Append("]");
		}

		internal void ResolveString8Values(Encoding string8Encoding)
		{
			if (!this.IsNullValue)
			{
				if (this.propertyTag.PropertyType == PropertyType.String8)
				{
					((String8)this.unionVal.Value).ResolveString8Values(string8Encoding);
					return;
				}
				if (this.propertyTag.PropertyType == PropertyType.MultiValueString8)
				{
					foreach (String8 @string in (String8[])this.unionVal.Value)
					{
						if (@string != null)
						{
							@string.ResolveString8Values(string8Encoding);
						}
					}
					return;
				}
				if (this.propertyTag.PropertyType == PropertyType.Restriction)
				{
					((Restriction)this.unionVal.Value).ResolveString8Values(string8Encoding);
					return;
				}
				if (this.propertyTag.PropertyType == PropertyType.Actions)
				{
					foreach (RuleAction ruleAction in (RuleAction[])this.unionVal.Value)
					{
						ruleAction.ResolveString8Values(string8Encoding);
					}
				}
			}
		}

		public string ToString(string format, IFormatProvider fp)
		{
			StringBuilder stringBuilder = new StringBuilder();
			this.AppendToString(stringBuilder, format, fp);
			return stringBuilder.ToString();
		}

		internal static ExDateTime ExDateTimeFromFileTimeUtc(long fileTimeAsInt64)
		{
			if (fileTimeAsInt64 == 0L)
			{
				return ExDateTime.MinValue;
			}
			if (fileTimeAsInt64 < 0L || fileTimeAsInt64 >= PropertyValue.ExDateTimeUtcMaxValueAsFileTime)
			{
				return ExDateTime.MaxValue;
			}
			ExDateTime result;
			try
			{
				result = ExDateTime.FromFileTimeUtc(fileTimeAsInt64);
			}
			catch (ArgumentOutOfRangeException)
			{
				result = ExDateTime.MaxValue;
			}
			return result;
		}

		internal static long ExDateTimeToFileTimeUtc(ExDateTime dateTime)
		{
			if (dateTime == ExDateTime.MaxValue)
			{
				return long.MaxValue;
			}
			if (dateTime <= PropertyValue.FileTimeMinValueAsExDateTimeUtc)
			{
				return 0L;
			}
			long result;
			try
			{
				result = dateTime.ToFileTimeUtc();
			}
			catch (ArgumentOutOfRangeException)
			{
				result = 0L;
			}
			return result;
		}

		private void CheckValueTypeOnConstruction(bool isTypeAcceptable)
		{
			if (!isTypeAcceptable)
			{
				throw new InvalidPropertyValueTypeException(string.Format("Value for property {0} of type {1} is of incorrect CLR type {2}", this.propertyTag, this.propertyTag.PropertyType, (this.unionVal.Value != null) ? this.unionVal.Value.GetType().Name : "(null)"), "value");
			}
		}

		private readonly PropertyTag propertyTag;

		private readonly PropertyValue.Union unionVal;

		internal static readonly long ExDateTimeUtcMaxValueAsFileTime = ExDateTime.MaxValue.ToFileTimeUtc();

		internal static readonly ExDateTime FileTimeMinValueAsExDateTimeUtc = ExDateTime.FromFileTimeUtc(0L);

		private struct Union
		{
			public object Value;

			public ErrorCode ErrorCode;
		}
	}
}
