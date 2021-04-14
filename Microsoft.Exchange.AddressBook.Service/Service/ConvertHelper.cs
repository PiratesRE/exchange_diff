using System;
using System.Linq;
using Microsoft.Exchange.AddressBook.Nspi;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.Rpc.Nspi;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Mapi;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal static class ConvertHelper
	{
		internal static PropTag ConvertToMapiPropTag(PropertyTag propertyTag)
		{
			return (PropTag)propertyTag;
		}

		internal static PropTag ConvertToMapiPropTag(SubRestrictionType subRestrictionType)
		{
			return (PropTag)subRestrictionType;
		}

		internal static PropertyTag ConvertFromMapiPropTag(PropTag propTag)
		{
			return new PropertyTag((uint)propTag);
		}

		internal static Restriction.RelOp ConvertToMapiRelOp(RelationOperator relationOperator)
		{
			return (Restriction.RelOp)relationOperator;
		}

		internal static RelationOperator ConvertFromMapiRelOp(Restriction.RelOp relOp)
		{
			return (RelationOperator)relOp;
		}

		internal static BitMaskOperator ConvertFromMapiRelBmr(Restriction.RelBmr relBmr)
		{
			return (BitMaskOperator)relBmr;
		}

		internal static Restriction.RelBmr ConvertToMapiRelBmr(BitMaskOperator bitMaskOperator)
		{
			return (Restriction.RelBmr)bitMaskOperator;
		}

		internal static PropTag[] ConvertToMapiPropTags(PropertyTag[] propertyTags)
		{
			if (propertyTags == null)
			{
				return null;
			}
			PropTag[] array = new PropTag[propertyTags.Length];
			for (int i = 0; i < propertyTags.Length; i++)
			{
				array[i] = ConvertHelper.ConvertToMapiPropTag(propertyTags[i]);
			}
			return array;
		}

		internal static PropertyTag[] ConvertFromMapiPropTags(PropTag[] mapiPropTags)
		{
			if (mapiPropTags == null)
			{
				return null;
			}
			PropertyTag[] array = new PropertyTag[mapiPropTags.Length];
			for (int i = 0; i < mapiPropTags.Length; i++)
			{
				array[i] = ConvertHelper.ConvertFromMapiPropTag(mapiPropTags[i]);
			}
			return array;
		}

		internal static PropValue ConvertToMapiPropValue(PropertyValue propertyValue)
		{
			PropValue result;
			try
			{
				PropertyTag propertyTag = propertyValue.PropertyTag;
				object value = null;
				if (!propertyValue.IsNullValue)
				{
					PropertyType propertyType = propertyTag.PropertyType;
					if (propertyType <= PropertyType.Guid)
					{
						if (propertyType <= PropertyType.Object)
						{
							switch (propertyType)
							{
							case PropertyType.Null:
								goto IL_244;
							case PropertyType.Int16:
								value = propertyValue.GetValue<short>();
								goto IL_244;
							case PropertyType.Int32:
								value = propertyValue.GetValue<int>();
								goto IL_244;
							default:
								switch (propertyType)
								{
								case PropertyType.Error:
									value = (uint)propertyValue.Value;
									goto IL_244;
								case PropertyType.Bool:
									value = propertyValue.GetValue<bool>();
									goto IL_244;
								case PropertyType.Object:
									goto IL_244;
								}
								break;
							}
						}
						else
						{
							switch (propertyType)
							{
							case PropertyType.String8:
								value = MarshalHelper.GetString8PropertyValue(propertyValue);
								goto IL_244;
							case PropertyType.Unicode:
								value = propertyValue.GetValue<string>();
								goto IL_244;
							default:
								if (propertyType == PropertyType.SysTime)
								{
									value = (DateTime)propertyValue.GetValue<ExDateTime>();
									goto IL_244;
								}
								if (propertyType == PropertyType.Guid)
								{
									value = propertyValue.GetValue<Guid>();
									goto IL_244;
								}
								break;
							}
						}
					}
					else if (propertyType <= PropertyType.MultiValueUnicode)
					{
						if (propertyType == PropertyType.Binary)
						{
							value = propertyValue.GetValue<byte[]>();
							goto IL_244;
						}
						switch (propertyType)
						{
						case PropertyType.MultiValueInt16:
							value = propertyValue.GetValue<short[]>();
							goto IL_244;
						case PropertyType.MultiValueInt32:
							value = propertyValue.GetValue<int[]>();
							goto IL_244;
						default:
							switch (propertyType)
							{
							case PropertyType.MultiValueString8:
								value = MarshalHelper.GetMultiValuedString8PropertyValue(propertyValue);
								goto IL_244;
							case PropertyType.MultiValueUnicode:
								value = propertyValue.GetValue<string[]>();
								goto IL_244;
							}
							break;
						}
					}
					else
					{
						if (propertyType == PropertyType.MultiValueSysTime)
						{
							ExDateTime[] value2 = propertyValue.GetValue<ExDateTime[]>();
							DateTime[] array = null;
							if (value2 != null)
							{
								array = new DateTime[value2.Length];
								for (int i = 0; i < value2.Length; i++)
								{
									array[i] = (DateTime)value2[i];
								}
							}
							value = array;
							goto IL_244;
						}
						if (propertyType == PropertyType.MultiValueGuid)
						{
							value = propertyValue.GetValue<Guid[]>();
							goto IL_244;
						}
						if (propertyType == PropertyType.MultiValueBinary)
						{
							value = propertyValue.GetValue<byte[][]>();
							goto IL_244;
						}
					}
					throw new NspiException(NspiStatus.InvalidParameter, string.Format("Unable to convert unsupported property type {0} on property {1}.", propertyTag.PropertyType, propertyTag));
				}
				IL_244:
				result = new PropValue(ConvertHelper.ConvertToMapiPropTag(propertyTag), value);
			}
			catch (UnexpectedPropertyTypeException inner)
			{
				throw new NspiException(NspiStatus.InvalidParameter, string.Format("Unable to convert invalid PropValue as it contains unsupported PropType on property {0}.", propertyValue), inner);
			}
			return result;
		}

		internal static String8 ConvertMapiPtString8ToString8(object stringData, int codePage)
		{
			String8 @string = null;
			string text = stringData as string;
			if (text != null)
			{
				@string = new String8(text);
			}
			else
			{
				byte[] array = stringData as byte[];
				if (array != null)
				{
					@string = new String8(new ArraySegment<byte>(array));
				}
			}
			if (@string != null)
			{
				@string.ResolveString8Values(MarshalHelper.GetString8Encoding(codePage));
			}
			return @string;
		}

		internal static PropertyValue ConvertFromMapiPropValue(PropValue mapiPropValue, int codePage)
		{
			PropertyValue result;
			try
			{
				PropertyTag propertyTag = new PropertyTag((uint)mapiPropValue.PropTag);
				object obj = null;
				PropertyType propertyType = propertyTag.PropertyType;
				if (propertyType <= PropertyType.Guid)
				{
					if (propertyType <= PropertyType.Object)
					{
						switch (propertyType)
						{
						case PropertyType.Null:
							goto IL_2E7;
						case PropertyType.Int16:
							obj = mapiPropValue.GetShort();
							goto IL_2E7;
						case PropertyType.Int32:
							obj = mapiPropValue.GetInt();
							goto IL_2E7;
						default:
							switch (propertyType)
							{
							case PropertyType.Error:
								obj = (ErrorCode)mapiPropValue.GetErrorValue();
								goto IL_2E7;
							case PropertyType.Bool:
								obj = mapiPropValue.GetBoolean();
								goto IL_2E7;
							case PropertyType.Object:
								goto IL_2E7;
							}
							break;
						}
					}
					else
					{
						switch (propertyType)
						{
						case PropertyType.String8:
							obj = ConvertHelper.ConvertMapiPtString8ToString8(mapiPropValue.Value, codePage);
							goto IL_2E7;
						case PropertyType.Unicode:
							obj = mapiPropValue.GetString();
							goto IL_2E7;
						default:
							if (propertyType == PropertyType.SysTime)
							{
								DateTime dateTime = mapiPropValue.GetDateTime();
								obj = (ExDateTime)dateTime;
								goto IL_2E7;
							}
							if (propertyType == PropertyType.Guid)
							{
								obj = mapiPropValue.GetGuid();
								goto IL_2E7;
							}
							break;
						}
					}
				}
				else if (propertyType <= PropertyType.MultiValueUnicode)
				{
					if (propertyType == PropertyType.Binary)
					{
						obj = mapiPropValue.GetBytes();
						goto IL_2E7;
					}
					switch (propertyType)
					{
					case PropertyType.MultiValueInt16:
						obj = mapiPropValue.GetShortArray();
						goto IL_2E7;
					case PropertyType.MultiValueInt32:
						obj = mapiPropValue.GetIntArray();
						goto IL_2E7;
					default:
						switch (propertyType)
						{
						case PropertyType.MultiValueString8:
							if (mapiPropValue.Value is string[])
							{
								string[] array = (string[])mapiPropValue.Value;
								String8[] array2 = new String8[array.Length];
								for (int i = 0; i < array.Length; i++)
								{
									array2[i] = ConvertHelper.ConvertMapiPtString8ToString8(array[i], codePage);
								}
								obj = array2;
								goto IL_2E7;
							}
							if (mapiPropValue.Value is byte[][])
							{
								byte[][] array3 = (byte[][])mapiPropValue.Value;
								String8[] array4 = new String8[array3.Length];
								for (int j = 0; j < array3.Length; j++)
								{
									array4[j] = ConvertHelper.ConvertMapiPtString8ToString8(array3[j], codePage);
								}
								obj = array4;
								goto IL_2E7;
							}
							goto IL_2E7;
						case PropertyType.MultiValueUnicode:
							obj = mapiPropValue.GetStringArray();
							goto IL_2E7;
						}
						break;
					}
				}
				else if (propertyType != PropertyType.MultiValueSysTime)
				{
					if (propertyType == PropertyType.MultiValueGuid)
					{
						obj = mapiPropValue.GetGuidArray();
						goto IL_2E7;
					}
					if (propertyType == PropertyType.MultiValueBinary)
					{
						obj = mapiPropValue.GetBytesArray();
						goto IL_2E7;
					}
				}
				else
				{
					DateTime[] dateTimeArray = mapiPropValue.GetDateTimeArray();
					if (dateTimeArray != null)
					{
						ExDateTime[] array5 = new ExDateTime[dateTimeArray.Length];
						for (int k = 0; k < dateTimeArray.Length; k++)
						{
							array5[k] = (ExDateTime)dateTimeArray[k];
						}
						obj = array5;
						goto IL_2E7;
					}
					goto IL_2E7;
				}
				throw new NspiException(NspiStatus.InvalidParameter, string.Format("Unable to convert unsupported property type {0} on property {1}.", propertyTag.PropertyType, propertyTag));
				IL_2E7:
				if (obj == null)
				{
					result = PropertyValue.NullValue(propertyTag);
				}
				else
				{
					result = new PropertyValue(propertyTag, obj);
				}
			}
			catch (InvalidPropertyValueTypeException inner)
			{
				throw new NspiException(NspiStatus.InvalidParameter, string.Format("Unable to convert invalid PropValue on property {0}.", mapiPropValue), inner);
			}
			catch (NotSupportedException inner2)
			{
				throw new NspiException(NspiStatus.InvalidParameter, string.Format("Unable to convert invalid PropValue as it contains unsupported PropType on property {0}.", mapiPropValue), inner2);
			}
			return result;
		}

		internal static PropValue[] ConvertToMapiPropValues(PropertyValue[] propertyValues)
		{
			PropValue[] array = null;
			if (propertyValues != null)
			{
				array = new PropValue[propertyValues.Length];
				for (int i = 0; i < propertyValues.Length; i++)
				{
					array[i] = ConvertHelper.ConvertToMapiPropValue(propertyValues[i]);
				}
			}
			return array;
		}

		internal static PropertyValue[] ConvertFromMapiPropValues(PropValue[] mapiPropValues, int codePage)
		{
			PropertyValue[] array = null;
			if (mapiPropValues != null)
			{
				array = new PropertyValue[mapiPropValues.Length];
				for (int i = 0; i < mapiPropValues.Length; i++)
				{
					array[i] = ConvertHelper.ConvertFromMapiPropValue(mapiPropValues[i], codePage);
				}
			}
			return array;
		}

		internal static PropRow ConvertToMapiPropRow(PropertyValue[] propertyValues)
		{
			PropValue[] properties = ConvertHelper.ConvertToMapiPropValues(propertyValues);
			return new PropRow(properties);
		}

		internal static PropertyValue[] ConvertFromMapiPropRow(PropRow mapiPropRow, int codePage)
		{
			PropertyValue[] result = null;
			if (mapiPropRow != null)
			{
				PropValue[] mapiPropValues = mapiPropRow.Properties.ToArray<PropValue>();
				result = ConvertHelper.ConvertFromMapiPropValues(mapiPropValues, codePage);
			}
			return result;
		}

		internal static PropRowSet ConvertToMapiPropRow(PropertyValue[][] propertyRows)
		{
			PropRowSet propRowSet = null;
			if (propertyRows != null)
			{
				propRowSet = new PropRowSet(propertyRows.Length);
				for (int i = 0; i < propertyRows.Length; i++)
				{
					PropRow row = ConvertHelper.ConvertToMapiPropRow(propertyRows[i]);
					propRowSet.Add(row);
				}
			}
			return propRowSet;
		}

		internal static PropertyValue[][] ConvertFromMapiPropRowSet(PropRowSet mapiPropRowSet, int codePage)
		{
			PropertyValue[][] array = null;
			if (mapiPropRowSet != null)
			{
				array = new PropertyValue[mapiPropRowSet.Rows.Count][];
				int num = 0;
				foreach (PropRow mapiPropRow in mapiPropRowSet.Rows)
				{
					array[num++] = ConvertHelper.ConvertFromMapiPropRow(mapiPropRow, codePage);
				}
			}
			return array;
		}

		internal static Restriction ConvertFromMapiRestriction(Restriction restriction, int codePage)
		{
			if (restriction == null)
			{
				return null;
			}
			switch (restriction.Type)
			{
			case Restriction.ResType.And:
				return new AndRestriction((from r in ((Restriction.AndRestriction)restriction).Restrictions
				select ConvertHelper.ConvertFromMapiRestriction(r, codePage)).ToArray<Restriction>());
			case Restriction.ResType.Or:
				return new OrRestriction((from r in ((Restriction.OrRestriction)restriction).Restrictions
				select ConvertHelper.ConvertFromMapiRestriction(r, codePage)).ToArray<Restriction>());
			case Restriction.ResType.Not:
				return new NotRestriction(ConvertHelper.ConvertFromMapiRestriction((Restriction.NotRestriction)restriction, codePage));
			case Restriction.ResType.Content:
			{
				Restriction.ContentRestriction contentRestriction = (Restriction.ContentRestriction)restriction;
				return new ContentRestriction((FuzzyLevel)contentRestriction.Flags, ConvertHelper.ConvertFromMapiPropTag(contentRestriction.PropTag), new PropertyValue?(ConvertHelper.ConvertFromMapiPropValue(contentRestriction.PropValue, codePage)));
			}
			case Restriction.ResType.Property:
			{
				Restriction.PropertyRestriction propertyRestriction = (Restriction.PropertyRestriction)restriction;
				RelationOperator relop = ConvertHelper.ConvertFromMapiRelOp(propertyRestriction.Op);
				PropertyTag propertyTag = ConvertHelper.ConvertFromMapiPropTag(propertyRestriction.PropTag);
				PropertyValue value = ConvertHelper.ConvertFromMapiPropValue(propertyRestriction.PropValue, codePage);
				return new PropertyRestriction(relop, propertyTag, new PropertyValue?(value));
			}
			case Restriction.ResType.CompareProps:
			{
				Restriction.ComparePropertyRestriction comparePropertyRestriction = (Restriction.ComparePropertyRestriction)restriction;
				return new ComparePropsRestriction(ConvertHelper.ConvertFromMapiRelOp(comparePropertyRestriction.Op), ConvertHelper.ConvertFromMapiPropTag(comparePropertyRestriction.TagLeft), ConvertHelper.ConvertFromMapiPropTag(comparePropertyRestriction.TagRight));
			}
			case Restriction.ResType.BitMask:
			{
				Restriction.BitMaskRestriction bitMaskRestriction = (Restriction.BitMaskRestriction)restriction;
				return new BitMaskRestriction(ConvertHelper.ConvertFromMapiRelBmr(bitMaskRestriction.Bmr), ConvertHelper.ConvertFromMapiPropTag(bitMaskRestriction.Tag), (uint)bitMaskRestriction.Mask);
			}
			case Restriction.ResType.Size:
			{
				Restriction.SizeRestriction sizeRestriction = (Restriction.SizeRestriction)restriction;
				return new SizeRestriction(ConvertHelper.ConvertFromMapiRelOp(sizeRestriction.Op), ConvertHelper.ConvertFromMapiPropTag(sizeRestriction.Tag), (uint)sizeRestriction.Size);
			}
			case Restriction.ResType.Exist:
			{
				Restriction.ExistRestriction existRestriction = (Restriction.ExistRestriction)restriction;
				return new ExistsRestriction(ConvertHelper.ConvertFromMapiPropTag(existRestriction.Tag));
			}
			case Restriction.ResType.SubRestriction:
			{
				Restriction.SubRestriction subRestriction = (Restriction.SubRestriction)restriction;
				return new SubRestriction((SubRestrictionType)subRestriction.Type, ConvertHelper.ConvertFromMapiRestriction(subRestriction.Restriction, codePage));
			}
			default:
				throw new NspiException(NspiStatus.InvalidParameter, string.Format("Invalid MAPI restriction type: {0}", restriction));
			}
		}

		internal static Restriction ConvertToMapiRestriction(Restriction restriction)
		{
			if (restriction == null)
			{
				return null;
			}
			switch (restriction.RestrictionType)
			{
			case RestrictionType.And:
				return Restriction.And(ConvertHelper.ConvertToMapiRestrictions(((CompositeRestriction)restriction).ChildRestrictions));
			case RestrictionType.Or:
				return Restriction.Or(ConvertHelper.ConvertToMapiRestrictions(((CompositeRestriction)restriction).ChildRestrictions));
			case RestrictionType.Not:
				return Restriction.Not(ConvertHelper.ConvertToMapiRestriction(((NotRestriction)restriction).ChildRestriction));
			case RestrictionType.Content:
			{
				ContentRestriction contentRestriction = restriction as ContentRestriction;
				if (contentRestriction.PropertyValue == null)
				{
					throw new NspiException(NspiStatus.InvalidParameter, "Null PropertyValue is not valid for ContentRestriction.");
				}
				return Restriction.Content(ConvertHelper.ConvertToMapiPropTag(contentRestriction.PropertyTag), contentRestriction.PropertyTag.IsMultiValuedProperty, ConvertHelper.ConvertToMapiPropValue(contentRestriction.PropertyValue.Value), (ContentFlags)contentRestriction.FuzzyLevel);
			}
			case RestrictionType.Property:
			{
				PropertyRestriction propertyRestriction = restriction as PropertyRestriction;
				if (propertyRestriction.PropertyValue == null)
				{
					throw new NspiException(NspiStatus.InvalidParameter, "Null PropertyValue is not valid for PropertyRestriction.");
				}
				return Restriction.Property(ConvertHelper.ConvertToMapiRelOp(propertyRestriction.RelationOperator), ConvertHelper.ConvertToMapiPropTag(propertyRestriction.PropertyTag), propertyRestriction.PropertyTag.IsMultiValuedProperty, ConvertHelper.ConvertToMapiPropValue(propertyRestriction.PropertyValue.Value));
			}
			case RestrictionType.CompareProps:
			{
				ComparePropsRestriction comparePropsRestriction = restriction as ComparePropsRestriction;
				return Restriction.CompareProps(ConvertHelper.ConvertToMapiRelOp(comparePropsRestriction.RelationOperator), ConvertHelper.ConvertToMapiPropTag(comparePropsRestriction.Property1), ConvertHelper.ConvertToMapiPropTag(comparePropsRestriction.Property2));
			}
			case RestrictionType.BitMask:
			{
				BitMaskRestriction bitMaskRestriction = restriction as BitMaskRestriction;
				return Restriction.BitMask(ConvertHelper.ConvertToMapiRelBmr(bitMaskRestriction.BitMaskOperator), ConvertHelper.ConvertToMapiPropTag(bitMaskRestriction.PropertyTag), (int)bitMaskRestriction.BitMask);
			}
			case RestrictionType.Size:
			{
				SizeRestriction sizeRestriction = restriction as SizeRestriction;
				return Restriction.PropertySize(ConvertHelper.ConvertToMapiRelOp(sizeRestriction.RelationOperator), ConvertHelper.ConvertToMapiPropTag(sizeRestriction.PropertyTag), (int)sizeRestriction.Size);
			}
			case RestrictionType.Exists:
			{
				ExistsRestriction existsRestriction = restriction as ExistsRestriction;
				return Restriction.Exist(ConvertHelper.ConvertToMapiPropTag(existsRestriction.PropertyTag));
			}
			case RestrictionType.SubRestriction:
			{
				SubRestriction subRestriction = restriction as SubRestriction;
				return Restriction.Sub(ConvertHelper.ConvertToMapiPropTag(subRestriction.SubRestrictionType), ConvertHelper.ConvertToMapiRestriction(subRestriction.ChildRestriction));
			}
			default:
				throw new NspiException(NspiStatus.InvalidParameter, string.Format("Invalid restriction type: {0}", restriction));
			}
		}

		internal static Restriction[] ConvertToMapiRestrictions(Restriction[] restrictions)
		{
			if (restrictions == null)
			{
				return null;
			}
			Restriction[] array = new Restriction[restrictions.Length];
			for (int i = 0; i < restrictions.Length; i++)
			{
				array[i] = ConvertHelper.ConvertToMapiRestriction(restrictions[i]);
			}
			return array;
		}

		internal static EntryId[] ConvertToMapiEntryIds(byte[][] rawEntryIds)
		{
			if (rawEntryIds.Length > 100000)
			{
				throw new NspiException(NspiStatus.InvalidParameter, "Too many EntryId values");
			}
			EntryId[] array = new EntryId[rawEntryIds.Length];
			for (int i = 0; i < rawEntryIds.Length; i++)
			{
				if (rawEntryIds[i] == null)
				{
					throw new NspiException(NspiStatus.InvalidParameter, "EntryId is null");
				}
				if (rawEntryIds[i].Length < 28 || rawEntryIds[i].Length > 2048)
				{
					throw new NspiException(NspiStatus.InvalidParameter, string.Format("EntryId is an invalid length; size={0}", rawEntryIds[i].Length));
				}
				if (!EntryId.TryParse(rawEntryIds[i], out array[i]))
				{
					throw new NspiException(NspiStatus.InvalidParameter, "Could not parse EntryId");
				}
			}
			return array;
		}

		private const int MinEntryIdSize = 28;

		private const int MaxEntryIdSize = 2048;

		private const int MaxRows = 100000;
	}
}
