using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.MAPI
{
	internal static class RcaTypeHelpers
	{
		public static StoreId ExchangeIdToStoreId(ExchangeId exchangeId)
		{
			return new StoreId(exchangeId.ToLong());
		}

		public static ExchangeId StoreIdToExchangeId(StoreId storeId, Mailbox mailbox)
		{
			return ExchangeId.CreateFromInt64(mailbox.CurrentOperationContext, mailbox.ReplidGuidMap, storeId);
		}

		public static StoreId[] ExchangeIdsToStoreIds(ICollection<ExchangeId> exchangeIds)
		{
			StoreId[] array = Array<StoreId>.Empty;
			if (exchangeIds != null && exchangeIds.Count != 0)
			{
				array = new StoreId[exchangeIds.Count];
				int num = 0;
				foreach (ExchangeId exchangeId in exchangeIds)
				{
					array[num] = RcaTypeHelpers.ExchangeIdToStoreId(exchangeId);
					num++;
				}
			}
			return array;
		}

		public static ulong[] ExchangeIdsToArrayOfUInt64(ICollection<ExchangeId> exchangeIds)
		{
			ulong[] array = Array<ulong>.Empty;
			if (exchangeIds != null && exchangeIds.Count != 0)
			{
				array = new ulong[exchangeIds.Count];
				int num = 0;
				foreach (ExchangeId exchangeId in exchangeIds)
				{
					array[num] = (ulong)exchangeId.ToLong();
					num++;
				}
			}
			return array;
		}

		public static ExchangeId[] StoreIdsToExchangeIds(ICollection<StoreId> storeIds, Mailbox mailbox)
		{
			ExchangeId[] array = Array<ExchangeId>.Empty;
			if (storeIds != null && storeIds.Count != 0)
			{
				array = new ExchangeId[storeIds.Count];
				int num = 0;
				foreach (StoreId storeId in storeIds)
				{
					array[num] = RcaTypeHelpers.StoreIdToExchangeId(storeId, mailbox);
					num++;
				}
			}
			return array;
		}

		public static List<PropertyTag> PropertyTagsFromStorePropTags(IList<StorePropTag> propTags)
		{
			List<PropertyTag> list = new List<PropertyTag>(propTags.Count);
			foreach (StorePropTag storePropTag in propTags)
			{
				list.Add(new PropertyTag(storePropTag.PropTag));
			}
			return list;
		}

		public static Restriction RestrictionToRestriction(MapiContext context, Restriction rcaRestriction, MapiLogon logon, MapiObjectType objectType)
		{
			Restriction result = null;
			if (rcaRestriction != null)
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (Writer writer = new StreamWriter(memoryStream))
					{
						rcaRestriction.Serialize(writer, CTSGlobals.AsciiEncoding, WireFormatStyle.Rop);
						int posMax = (int)memoryStream.Position;
						int num = 0;
						result = Restriction.Deserialize(context, MapiProtocolsHelpers.GetUnderlyingBytesFromMemoryStream(memoryStream), ref num, posMax, logon.StoreMailbox, Helper.GetPropTagObjectType(objectType));
					}
				}
			}
			return result;
		}

		public static void SerializeRuleActions(Writer writer, RuleAction[] actions)
		{
			if (actions != null)
			{
				writer.WriteUInt16((ushort)actions.Length);
				for (int i = 0; i < actions.Length; i++)
				{
					if (actions[0] == null)
					{
						throw new ArgumentNullException("action");
					}
					long position = writer.Position;
					writer.WriteUInt16(0);
					actions[i].Serialize(writer, CTSGlobals.AsciiEncoding);
					writer.UpdateSize(position, writer.Position);
				}
			}
		}

		public static void MassageIncomingPropertyValue(PropertyTag tag, ref object value)
		{
			PropertyType propertyType = tag.PropertyType;
			if (propertyType != PropertyType.SysTime)
			{
				switch (propertyType)
				{
				case PropertyType.Restriction:
				case PropertyType.Actions:
					using (MemoryStream memoryStream = new MemoryStream())
					{
						using (Writer writer = new StreamWriter(memoryStream))
						{
							if (tag.PropertyType == PropertyType.Restriction)
							{
								Restriction restriction = (Restriction)value;
								restriction.Serialize(writer, CTSGlobals.AsciiEncoding, WireFormatStyle.Rop);
							}
							else
							{
								RcaTypeHelpers.SerializeRuleActions(writer, (RuleAction[])value);
							}
						}
						value = memoryStream.ToArray();
						break;
					}
					goto IL_8D;
				default:
				{
					if (propertyType != PropertyType.MultiValueSysTime)
					{
						return;
					}
					DateTime[] array = ExDateTime.ToDateTimeArray((ExDateTime[])value);
					value = array;
					break;
				}
				}
				return;
			}
			IL_8D:
			DateTime dateTime = (DateTime)((ExDateTime)value);
			value = dateTime;
		}

		public static PropertyValue[] MassageOutgoingProperties(Properties props, out bool hasErrors)
		{
			hasErrors = false;
			PropertyValue[] array = Array<PropertyValue>.Empty;
			if (props.Count > 0)
			{
				array = new PropertyValue[props.Count];
				for (int i = 0; i < props.Count; i++)
				{
					array[i] = RcaTypeHelpers.MassageOutgoingProperty(props[i], false);
					if (array[i].IsError)
					{
						hasErrors = true;
					}
				}
			}
			return array;
		}

		public static PropertyValue MassageOutgoingProperty(Property prop, bool fixupOutlookDateTime)
		{
			prop = LegacyHelper.MassageOutgoingProperty(prop, fixupOutlookDateTime);
			PropertyTag propertyTag = (PropertyType.Error == prop.Tag.PropType || prop.Tag.ExternalType == PropertyType.Unspecified) ? new PropertyTag(prop.Tag.PropTag) : new PropertyTag(prop.Tag.ExternalTag);
			propertyTag = PropertyTag.RemoveMviWithMvIfNeeded(propertyTag);
			object obj = prop.Value;
			PropertyType propType = prop.Tag.PropType;
			if (propType <= PropertyType.MVUnicode)
			{
				if (propType <= PropertyType.Guid)
				{
					if (propType <= PropertyType.Unicode)
					{
						switch (propType)
						{
						case PropertyType.Null:
						case PropertyType.Int16:
						case PropertyType.Int32:
						case PropertyType.Real32:
						case PropertyType.Real64:
						case PropertyType.Currency:
						case PropertyType.AppTime:
						case PropertyType.Boolean:
						case PropertyType.Int64:
							goto IL_3C6;
						case (PropertyType)8:
						case (PropertyType)9:
						case (PropertyType)12:
						case (PropertyType)14:
						case (PropertyType)15:
						case (PropertyType)16:
						case (PropertyType)17:
						case (PropertyType)18:
						case (PropertyType)19:
							goto IL_392;
						case PropertyType.Error:
						{
							ErrorCode errorCode = (ErrorCode)((ErrorCodeValue)obj);
							ErrorCode errorCode2 = errorCode;
							if (errorCode2 == (ErrorCode)2147746063U)
							{
								obj = RcaTypeHelpers.BoxedErrorCodeNotFound;
								goto IL_3C6;
							}
							if (errorCode2 != (ErrorCode)2147942414U)
							{
								obj = errorCode;
								goto IL_3C6;
							}
							obj = RcaTypeHelpers.BoxedErrorCodeNotEnoughMemory;
							goto IL_3C6;
						}
						case PropertyType.Object:
							propertyTag = RcaTypeHelpers.ChangePropertyType(prop.Tag.PropTag, PropertyType.Error);
							obj = RcaTypeHelpers.BoxedErrorCodeNotEnoughMemory;
							goto IL_3C6;
						default:
						{
							if (propType != PropertyType.Unicode)
							{
								goto IL_392;
							}
							LargeValue largeValue = obj as LargeValue;
							if (largeValue != null)
							{
								propertyTag = RcaTypeHelpers.ChangePropertyType(prop.Tag.PropTag, PropertyType.Error);
								obj = RcaTypeHelpers.BoxedErrorCodeNotEnoughMemory;
								goto IL_3C6;
							}
							break;
						}
						}
					}
					else
					{
						if (propType == PropertyType.SysTime)
						{
							goto IL_351;
						}
						if (propType != PropertyType.Guid)
						{
							goto IL_392;
						}
						goto IL_3C6;
					}
				}
				else if (propType <= PropertyType.Binary)
				{
					switch (propType)
					{
					case PropertyType.SvrEid:
						goto IL_31B;
					case (PropertyType)252:
					case PropertyType.SRestriction:
					case PropertyType.Actions:
						goto IL_392;
					default:
					{
						if (propType != PropertyType.Binary)
						{
							goto IL_392;
						}
						LargeValue largeValue2 = obj as LargeValue;
						if (largeValue2 != null)
						{
							propertyTag = RcaTypeHelpers.ChangePropertyType(prop.Tag.PropTag, PropertyType.Error);
							obj = RcaTypeHelpers.BoxedErrorCodeNotEnoughMemory;
							goto IL_3C6;
						}
						goto IL_31B;
					}
					}
				}
				else
				{
					switch (propType)
					{
					case PropertyType.MVInt16:
					case PropertyType.MVInt32:
					case PropertyType.MVReal32:
					case PropertyType.MVReal64:
					case PropertyType.MVCurrency:
					case PropertyType.MVAppTime:
						goto IL_3C6;
					default:
					{
						if (propType == PropertyType.MVInt64)
						{
							goto IL_3C6;
						}
						if (propType != PropertyType.MVUnicode)
						{
							goto IL_392;
						}
						string[] array = obj as string[];
						for (int i = 0; i < array.Length; i++)
						{
							array[i] = RcaTypeHelpers.TruncateAtEmbeddedNull(array[i]);
						}
						goto IL_3C6;
					}
					}
				}
			}
			else if (propType <= PropertyType.MVIAppTime)
			{
				if (propType <= PropertyType.MVGuid)
				{
					if (propType == PropertyType.MVSysTime)
					{
						obj = RcaTypeHelpers.ToExDateTimeArray(obj);
						goto IL_3C6;
					}
					if (propType != PropertyType.MVGuid)
					{
						goto IL_392;
					}
					goto IL_3C6;
				}
				else
				{
					if (propType == PropertyType.MVBinary)
					{
						goto IL_3C6;
					}
					switch (propType)
					{
					case PropertyType.MVIInt16:
					case PropertyType.MVIInt32:
					case PropertyType.MVIReal32:
					case PropertyType.MVIReal64:
					case PropertyType.MVICurrency:
					case PropertyType.MVIAppTime:
						goto IL_3C6;
					default:
						goto IL_392;
					}
				}
			}
			else if (propType <= PropertyType.MVIUnicode)
			{
				if (propType == PropertyType.MVIInt64)
				{
					goto IL_3C6;
				}
				if (propType != PropertyType.MVIUnicode)
				{
					goto IL_392;
				}
			}
			else
			{
				if (propType == PropertyType.MVISysTime)
				{
					goto IL_351;
				}
				if (propType == PropertyType.MVIGuid)
				{
					goto IL_3C6;
				}
				if (propType != PropertyType.MVIBinary)
				{
					goto IL_392;
				}
				goto IL_31B;
			}
			obj = RcaTypeHelpers.TruncateAtEmbeddedNull((string)obj);
			goto IL_3C6;
			IL_31B:
			byte[] array2 = obj as byte[];
			if (array2.Length >= 65535)
			{
				propertyTag = RcaTypeHelpers.ChangePropertyType(prop.Tag.PropTag, PropertyType.Error);
				obj = RcaTypeHelpers.BoxedErrorCodeNotEnoughMemory;
				goto IL_3C6;
			}
			goto IL_3C6;
			IL_351:
			obj = new ExDateTime(ExTimeZone.UtcTimeZone, (DateTime)obj);
			goto IL_3C6;
			IL_392:
			throw new ExExceptionNoSupport((LID)45880U, "We do not support this property type (" + prop.Tag.PropType + ") yet!");
			IL_3C6:
			return new PropertyValue(propertyTag, obj);
		}

		public static PropertyProblem[] PropertyProblemFromMapiPropertyPropblem(IList<MapiPropertyProblem> problems, PropertyTag[] tags)
		{
			PropertyProblem[] array = Array<PropertyProblem>.Empty;
			if (problems != null && problems.Count > 0 && tags != null && tags.Length > 0)
			{
				array = new PropertyProblem[problems.Count];
				for (int i = 0; i < problems.Count; i++)
				{
					MapiPropertyProblem mapiPropertyProblem = problems[i];
					ushort index = ushort.MaxValue;
					for (int j = 0; j < tags.Length; j++)
					{
						if (mapiPropertyProblem.MapiPropTag.ExternalTag == tags[j])
						{
							index = (ushort)j;
							break;
						}
					}
					array[i] = new PropertyProblem(index, new PropertyTag(mapiPropertyProblem.MapiPropTag.PropTag), (ErrorCode)mapiPropertyProblem.ErrorCode);
				}
			}
			return array;
		}

		public static PropertyProblem[] PropertyProblemFromMapiPropertyPropblemAndValues(IList<MapiPropertyProblem> problems, PropertyValue[] values)
		{
			PropertyProblem[] array = Array<PropertyProblem>.Empty;
			if (problems != null && problems.Count > 0 && values != null && values.Length > 0)
			{
				array = new PropertyProblem[problems.Count];
				for (int i = 0; i < problems.Count; i++)
				{
					MapiPropertyProblem mapiPropertyProblem = problems[i];
					ushort index = ushort.MaxValue;
					for (int j = 0; j < values.Length; j++)
					{
						if (mapiPropertyProblem.MapiPropTag.ExternalTag == values[j].PropertyTag)
						{
							index = (ushort)j;
							break;
						}
					}
					array[i] = new PropertyProblem(index, new PropertyTag(mapiPropertyProblem.MapiPropTag.PropTag), (ErrorCode)mapiPropertyProblem.ErrorCode);
				}
			}
			return array;
		}

		public static NamedProperty[] GetNamesFromPropertyIDs(MapiContext context, IList<PropertyId> propertyIds, MapiMailbox mapiMailbox)
		{
			NamedProperty[] array = Array<NamedProperty>.Empty;
			if (propertyIds != null && propertyIds.Count > 0)
			{
				array = new NamedProperty[propertyIds.Count];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = RcaTypeHelpers.NamedPropertyFromStorePropName(LegacyHelper.GetNameFromNumber(context, (ushort)propertyIds[i], mapiMailbox));
				}
			}
			return array;
		}

		private static NamedProperty NamedPropertyFromStorePropName(StorePropName propertyName)
		{
			if (propertyName == StorePropName.Invalid)
			{
				return new NamedProperty();
			}
			if (propertyName.Name != null)
			{
				return new NamedProperty(propertyName.Guid, propertyName.Name);
			}
			return new NamedProperty(propertyName.Guid, propertyName.DispId);
		}

		private static PropertyTag ChangePropertyType(uint propertyTag, PropertyType newPropertyType)
		{
			return new PropertyTag((propertyTag & 4294901760U) | (uint)newPropertyType);
		}

		private static string TruncateAtEmbeddedNull(string value)
		{
			if (value != null)
			{
				int num = value.IndexOf('\0');
				if (num >= 0)
				{
					value = value.Substring(0, num);
				}
			}
			return value;
		}

		private static ExDateTime[] ToExDateTimeArray(object value)
		{
			if (value == null)
			{
				return null;
			}
			if (value is ExDateTime[])
			{
				return (ExDateTime[])value;
			}
			if (value is DateTime[])
			{
				DateTime[] array = (DateTime[])value;
				ExDateTime[] array2 = new ExDateTime[array.Length];
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i] = new ExDateTime(ExTimeZone.UtcTimeZone, array[i]);
				}
				return array2;
			}
			throw new ArgumentException("Value has invalid type: " + value.GetType().Name, "value");
		}

		private static readonly object BoxedErrorCodeNotEnoughMemory = (ErrorCode)2147942414U;

		private static readonly object BoxedErrorCodeNotFound = (ErrorCode)2147746063U;
	}
}
