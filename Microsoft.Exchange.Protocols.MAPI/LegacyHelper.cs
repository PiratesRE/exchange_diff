using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.Mapi;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public static class LegacyHelper
	{
		public static StorePropName GetNameFromNumber(MapiContext context, ushort propNumber, MapiMailbox mapiMailbox)
		{
			StorePropName result;
			if (propNumber < 32768)
			{
				result = new StorePropName(StorePropName.UnnamedPropertyNamespaceGuid, (uint)propNumber);
			}
			else
			{
				StorePropInfo nameFromNumber = mapiMailbox.GetNameFromNumber(context, propNumber);
				if (nameFromNumber != null)
				{
					result = nameFromNumber.PropName;
				}
				else
				{
					result = StorePropName.Invalid;
				}
			}
			return result;
		}

		public static Property MassageOutgoingProperty(Property prop, bool fixupOutlookDateTime)
		{
			StorePropTag tag = prop.Tag;
			object obj = prop.Value;
			if (prop.Tag.IsMultiValued)
			{
				Array array = obj as Array;
				if (array != null && array.Length == 0)
				{
					tag = tag.ConvertToError();
					obj = LegacyHelper.BoxedErrorCodeNotFound;
					prop = new Property(tag, obj);
				}
			}
			PropertyType propType = prop.Tag.PropType;
			if (propType <= PropertyType.MVUnicode)
			{
				if (propType <= PropertyType.Guid)
				{
					if (propType <= PropertyType.Unicode)
					{
						switch (propType)
						{
						case PropertyType.Int16:
						case PropertyType.Int32:
						case PropertyType.Real32:
						case PropertyType.Real64:
						case PropertyType.Currency:
						case PropertyType.AppTime:
						case PropertyType.Error:
						case PropertyType.Boolean:
							goto IL_38E;
						case (PropertyType)8:
						case (PropertyType)9:
						case (PropertyType)12:
						case (PropertyType)14:
						case (PropertyType)15:
						case (PropertyType)16:
						case (PropertyType)17:
						case (PropertyType)18:
						case (PropertyType)19:
							goto IL_380;
						case PropertyType.Object:
							break;
						case PropertyType.Int64:
							goto IL_219;
						default:
							if (propType != PropertyType.Unicode)
							{
								goto IL_380;
							}
							if (!(obj is LargeValue))
							{
								goto IL_38E;
							}
							goto IL_38E;
						}
					}
					else
					{
						if (propType == PropertyType.SysTime)
						{
							goto IL_23E;
						}
						if (propType != PropertyType.Guid)
						{
							goto IL_380;
						}
						goto IL_38E;
					}
				}
				else if (propType <= PropertyType.Binary)
				{
					switch (propType)
					{
					case PropertyType.SvrEid:
					case PropertyType.SRestriction:
					case PropertyType.Actions:
						goto IL_38E;
					case (PropertyType)252:
						goto IL_380;
					default:
						if (propType != PropertyType.Binary)
						{
							goto IL_380;
						}
						break;
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
						goto IL_38E;
					default:
						if (propType != PropertyType.MVInt64)
						{
							if (propType != PropertyType.MVUnicode)
							{
								goto IL_380;
							}
							goto IL_38E;
						}
						else
						{
							if (obj is ExchangeId[])
							{
								long[] array2 = new long[((ExchangeId[])obj).Length];
								for (int i = 0; i < array2.Length; i++)
								{
									array2[i] = ((ExchangeId[])obj)[i].ToLong();
								}
								obj = array2;
								goto IL_38E;
							}
							goto IL_38E;
						}
						break;
					}
				}
				if (!(obj is LargeValue))
				{
					goto IL_38E;
				}
				goto IL_38E;
			}
			else if (propType <= PropertyType.MVIAppTime)
			{
				if (propType <= PropertyType.MVGuid)
				{
					if (propType != PropertyType.MVSysTime)
					{
						if (propType != PropertyType.MVGuid)
						{
							goto IL_380;
						}
						goto IL_38E;
					}
					else
					{
						if (fixupOutlookDateTime)
						{
							DateTime[] array3 = (DateTime[])obj;
							for (int j = 0; j < array3.Length; j++)
							{
								if (DateTime.Compare(array3[j], ExDateTime.OutlookDateTimeMin) <= 0)
								{
									array3[j] = ExDateTime.OutlookDateTimeMin;
								}
								else if (DateTime.Compare(array3[j], ExDateTime.OutlookDateTimeMax) >= 0)
								{
									array3[j] = ExDateTime.OutlookDateTimeMax;
								}
							}
							goto IL_38E;
						}
						goto IL_38E;
					}
				}
				else
				{
					if (propType == PropertyType.MVBinary)
					{
						goto IL_38E;
					}
					switch (propType)
					{
					case PropertyType.MVIInt16:
					case PropertyType.MVIInt32:
					case PropertyType.MVIReal32:
					case PropertyType.MVIReal64:
					case PropertyType.MVICurrency:
					case PropertyType.MVIAppTime:
						goto IL_38E;
					default:
						goto IL_380;
					}
				}
			}
			else if (propType <= PropertyType.MVIUnicode)
			{
				if (propType != PropertyType.MVIInt64)
				{
					if (propType != PropertyType.MVIUnicode)
					{
						goto IL_380;
					}
					goto IL_38E;
				}
			}
			else
			{
				if (propType == PropertyType.MVISysTime)
				{
					goto IL_23E;
				}
				if (propType != PropertyType.MVIGuid && propType != PropertyType.MVIBinary)
				{
					goto IL_380;
				}
				goto IL_38E;
			}
			IL_219:
			if (obj is ExchangeId)
			{
				obj = ((ExchangeId)obj).ToLong();
				goto IL_38E;
			}
			goto IL_38E;
			IL_23E:
			if (!fixupOutlookDateTime)
			{
				goto IL_38E;
			}
			if (DateTime.Compare((DateTime)obj, ExDateTime.OutlookDateTimeMin) <= 0)
			{
				obj = ExDateTime.OutlookDateTimeMin;
				goto IL_38E;
			}
			if (DateTime.Compare((DateTime)obj, ExDateTime.OutlookDateTimeMax) >= 0)
			{
				obj = ExDateTime.OutlookDateTimeMax;
				goto IL_38E;
			}
			goto IL_38E;
			IL_380:
			tag = tag.ConvertToError();
			obj = LegacyHelper.BoxedErrorCodeInvalidType;
			IL_38E:
			return new Property(tag, obj);
		}

		public static void MassageOutgoingProperties(Properties properties)
		{
			for (int i = 0; i < properties.Count; i++)
			{
				properties[i] = LegacyHelper.MassageOutgoingProperty(properties[i], true);
			}
		}

		internal static PropertyId[] GetNumbersFromNames(MapiContext context, bool create, NamedProperty[] names, MapiLogon logon)
		{
			MapiMailbox mapiMailbox = logon.MapiMailbox;
			PropertyId[] array = new PropertyId[names.Length];
			for (int i = 0; i < names.Length; i++)
			{
				switch (names[i].Kind)
				{
				case NamedPropertyKind.Id:
					if (names[i].IsMapiNamespace)
					{
						array[i] = (PropertyId)names[i].Id;
					}
					else
					{
						ushort numberFromName = mapiMailbox.GetNumberFromName(context, create, new StorePropName(names[i].Guid, names[i].Id), logon);
						array[i] = (PropertyId)numberFromName;
					}
					break;
				case NamedPropertyKind.String:
				{
					if (names[i].IsMapiNamespace)
					{
						throw new InvalidParameterException((LID)59192U, "String named property in the Mapi namespace is not allowed.");
					}
					ushort numberFromName2 = mapiMailbox.GetNumberFromName(context, create, new StorePropName(names[i].Guid, names[i].Name), logon);
					array[i] = (PropertyId)numberFromName2;
					break;
				}
				default:
					array[i] = PropertyId.Invalid;
					break;
				}
			}
			return array;
		}

		public static void GetNamesFromNumbers(MapiContext context, uint flags, ushort[] propNumbers, MapiMailbox mapiMailbox, out StorePropName[] names)
		{
			if (flags != 0U)
			{
				ExTraceGlobals.GeneralTracer.TraceError(0L, "Invalid value for unused parameter 'flags' in 'GetNamesFromNumbers' method!  Throwing ExExceptionNoSupport!");
				throw new ExExceptionNoSupport((LID)34616U, "Invalid value for unused parameter 'flags'.");
			}
			if (propNumbers == null)
			{
				ExTraceGlobals.GeneralTracer.TraceError(0L, "Invalid value 'null'  for 'propNumbers' in 'GetNamesFromNumbers' method!  Throwing ExExceptionInvalidParameter!");
				throw new ExExceptionInvalidParameter((LID)64312U, "Invalid value 'null' for parameter 'propNumbers'.");
			}
			int num = propNumbers.Length;
			names = new StorePropName[num];
			for (int i = 0; i < num; i++)
			{
				names[i] = LegacyHelper.GetNameFromNumber(context, propNumbers[i], mapiMailbox);
			}
		}

		public static IList<ushort> QueryNamedProps(MapiContext context, MapiQryNamedPropsFlags flags, Guid? guid, MapiMailbox mapiMailbox)
		{
			bool includeIdKind = (flags & MapiQryNamedPropsFlags.NoIds) != MapiQryNamedPropsFlags.NoIds;
			bool includeNameKind = (flags & MapiQryNamedPropsFlags.NoStrings) != MapiQryNamedPropsFlags.NoStrings;
			List<ushort> result = new List<ushort>(mapiMailbox.StoreMailbox.NamedPropertyMap.GetNamedPropertyCount(context));
			mapiMailbox.StoreMailbox.NamedPropertyMap.ForEachElement(context, delegate(KeyValuePair<ushort, StoreNamedPropInfo> propidNamePair)
			{
				if ((guid == null || propidNamePair.Value.PropName.Guid == guid.Value) && ((includeIdKind && propidNamePair.Value.PropName.Name == null) || (includeNameKind && propidNamePair.Value.PropName.Name != null)))
				{
					result.Add(propidNamePair.Key);
				}
			});
			return result;
		}

		public static void ConvertToLegacyPropTags(IList<StorePropTag> propTags, out uint[] legacyPropTags)
		{
			int count = propTags.Count;
			legacyPropTags = new uint[count];
			for (int i = 0; i < count; i++)
			{
				legacyPropTags[i] = propTags[i].ExternalTag;
			}
		}

		internal static void ConvertToLegacyPropTags(IList<StorePropTag> propTags, out PropertyTag[] legacyPropTags)
		{
			int num = (propTags != null) ? propTags.Count : 0;
			legacyPropTags = Array<PropertyTag>.Empty;
			if (num > 0)
			{
				legacyPropTags = new PropertyTag[num];
				for (int i = 0; i < num; i++)
				{
					legacyPropTags[i] = new PropertyTag(propTags[i].ExternalTag);
				}
			}
		}

		public static void MapiProblemsToLegacyProblems(StorePropTag[] propTags, List<MapiPropertyProblem> alProblems, out LegacyPropProblem[] legacyPropProblems)
		{
			int num = (alProblems == null) ? 0 : alProblems.Count;
			legacyPropProblems = new LegacyPropProblem[num];
			for (int i = 0; i < num; i++)
			{
				MapiPropertyProblem mapiPropertyProblem = alProblems[i];
				legacyPropProblems[i] = default(LegacyPropProblem);
				legacyPropProblems[i].Idx = -1;
				for (int j = 0; j < propTags.Length; j++)
				{
					if (propTags[j] == mapiPropertyProblem.MapiPropTag)
					{
						legacyPropProblems[i].Idx = j;
						break;
					}
				}
				legacyPropProblems[i].PropTag = mapiPropertyProblem.MapiPropTag.PropTag;
				legacyPropProblems[i].ErrorCode = mapiPropertyProblem.ErrorCode;
			}
		}

		public static void MapiProblemsToLegacyProblems(Properties props, List<MapiPropertyProblem> alProblems, out LegacyPropProblem[] legacyPropProblems)
		{
			int num = (alProblems == null) ? 0 : alProblems.Count;
			legacyPropProblems = new LegacyPropProblem[num];
			for (int i = 0; i < num; i++)
			{
				MapiPropertyProblem mapiPropertyProblem = alProblems[i];
				legacyPropProblems[i] = default(LegacyPropProblem);
				legacyPropProblems[i].Idx = -1;
				for (int j = 0; j < props.Count; j++)
				{
					if (props[j].Tag == mapiPropertyProblem.MapiPropTag)
					{
						legacyPropProblems[i].Idx = j;
						break;
					}
				}
				legacyPropProblems[i].PropTag = mapiPropertyProblem.MapiPropTag.PropTag;
				legacyPropProblems[i].ErrorCode = mapiPropertyProblem.ErrorCode;
			}
		}

		public static StorePropTag[] ConvertFromLegacyPropTags(uint[] legacyPropTags, Microsoft.Exchange.Server.Storage.PropTags.ObjectType objectType, MapiMailbox mapiMailbox, bool suppressMVI)
		{
			int num = legacyPropTags.Length;
			StorePropTag[] array = new StorePropTag[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = LegacyHelper.ConvertFromLegacyPropTag(legacyPropTags[i], objectType, mapiMailbox, suppressMVI);
			}
			return array;
		}

		internal static StorePropTag[] ConvertFromLegacyPropTags(IList<PropertyTag> legacyPropTags, Microsoft.Exchange.Server.Storage.PropTags.ObjectType objectType, MapiMailbox mapiMailbox, bool suppressMVI)
		{
			int count = legacyPropTags.Count;
			StorePropTag[] array = new StorePropTag[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = LegacyHelper.ConvertFromLegacyPropTag(legacyPropTags[i], objectType, mapiMailbox, suppressMVI);
			}
			return array;
		}

		public static StorePropTag ConvertFromLegacyPropTag(uint legacyPropTag, Microsoft.Exchange.Server.Storage.PropTags.ObjectType objectType, MapiMailbox mapiMailbox, bool suppressMVI)
		{
			if (suppressMVI)
			{
				legacyPropTag &= 4294959103U;
			}
			Mailbox mailbox = (mapiMailbox != null) ? mapiMailbox.StoreMailbox : null;
			Context context = (mailbox != null) ? mailbox.CurrentOperationContext : null;
			return Mailbox.GetStorePropTag(context, mailbox, legacyPropTag, objectType);
		}

		internal static StorePropTag ConvertFromLegacyPropTag(PropertyTag legacyPropTag, Microsoft.Exchange.Server.Storage.PropTags.ObjectType objectType, MapiMailbox mapiMailbox, bool suppressMVI)
		{
			return LegacyHelper.ConvertFromLegacyPropTag(legacyPropTag, objectType, mapiMailbox, suppressMVI);
		}

		internal static readonly object BoxedErrorCodeNotFound = ErrorCodeValue.NotFound;

		internal static readonly object BoxedErrorCodeNotEnoughMemory = ErrorCodeValue.NotEnoughMemory;

		internal static readonly object BoxedErrorCodeInvalidType = ErrorCodeValue.InvalidType;

		internal static readonly object BoxedErrorCodeInvalidParameter = ErrorCodeValue.InvalidParameter;

		internal static readonly object BoxedErrorCodeNoAccess = ErrorCodeValue.NoAccess;

		internal static readonly object BoxedTrue = true;

		internal static readonly object BoxedFalse = false;
	}
}
