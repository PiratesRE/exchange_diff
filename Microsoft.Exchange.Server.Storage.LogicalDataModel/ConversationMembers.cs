using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods.Linq;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class ConversationMembers
	{
		public ConversationMembers(Mailbox mailbox, IEnumerable<FidMid> conversationMessages, IEnumerable<FidMid> originalConversationMessages, TopMessage modifiedMessage, HashSet<StorePropTag> aggregatePropertiesToCompute)
		{
			this.mailbox = mailbox;
			this.conversationMessages = new List<FidMid>(conversationMessages);
			this.originalConversationMessages = new List<FidMid>(originalConversationMessages);
			this.modifiedMessage = modifiedMessage;
			this.aggregatePropertiesToCompute = aggregatePropertiesToCompute;
		}

		public IEnumerable<ExchangeId> FolderIds
		{
			get
			{
				ISet<ExchangeId> set = new HashSet<ExchangeId>();
				foreach (FidMid fidMid in this.conversationMessages)
				{
					set.Add(fidMid.FolderId);
				}
				return set;
			}
		}

		public IEnumerable<ExchangeId> OriginalFolderIds
		{
			get
			{
				ISet<ExchangeId> set = new HashSet<ExchangeId>();
				foreach (FidMid fidMid in this.originalConversationMessages)
				{
					set.Add(fidMid.FolderId);
				}
				return set;
			}
		}

		public IEnumerable<FidMid> ConversationMessages
		{
			get
			{
				return this.conversationMessages;
			}
		}

		public IEnumerable<FidMid> OriginalConversationMessages
		{
			get
			{
				return this.originalConversationMessages;
			}
		}

		private static void AddPropertyIfNotPresent(Mailbox mailbox, StorePropTag propTag, List<StorePropTag> propertiesToPromote)
		{
			if (!propertiesToPromote.Contains(propTag))
			{
				propertiesToPromote.Add(propTag);
			}
		}

		private static bool ShouldSkipNullAndEmptyValue(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType aggregationType)
		{
			return aggregationType != ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.DisplayNamePriorityOrder && aggregationType != ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.MinInRelevanceScore && aggregationType != ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.BooleanAnd && aggregationType != ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.UnionNotUnique_NullIfAllNotSet;
		}

		public object GetAggregateProperty(Context context, StorePropTag proptag, ExchangeId folderId, bool original)
		{
			return this.GetAggregateProperty(context, proptag, null, folderId, original);
		}

		public object GetAggregateProperty(Context context, StorePropTag proptag, ICollection<FidMid> filterList, bool original)
		{
			return this.GetAggregateProperty(context, proptag, filterList, ExchangeId.Null, original);
		}

		public static bool IsAggregateProperty(StorePropTag propTag)
		{
			ConversationMembers.ConversationPropertyAggregationInfo conversationPropertyAggregationInfo;
			return ConversationMembers.conversationPropertyMapping.TryGetValue(propTag, out conversationPropertyAggregationInfo);
		}

		public static StorePropTag? GetAggregatedOnPropTag(Context context, Mailbox mailbox, StorePropTag proptag)
		{
			ConversationMembers.ConversationPropertyAggregationInfo? aggregationInfo = ConversationMembers.GetAggregationInfo(context, mailbox, proptag);
			if (aggregationInfo != null)
			{
				return new StorePropTag?(aggregationInfo.Value.AggregatedProperty);
			}
			return null;
		}

		public object GetAggregateProperty(Context context, StorePropTag proptag, ICollection<FidMid> filterList, ExchangeId folderId, bool original)
		{
			ConversationMembers.ConversationPropertyAggregationInfo? aggregationInfo = ConversationMembers.GetAggregationInfo(context, this.mailbox, proptag);
			if (aggregationInfo == null)
			{
				return null;
			}
			this.CacheMessageProperties(context, original);
			if (ConversationMembers.PropTagIsConversationFlagCompleteTime(proptag) && !this.ConversationIsFlagged(context, PropTag.Message.ConversationFlagCompleteTime == proptag, filterList, folderId, original))
			{
				return null;
			}
			ConversationMembers.ConversationPropertyAggregationInfo value = aggregationInfo.Value;
			InitialMessageFinder initialMessageFinder = null;
			object obj = null;
			string origin = string.Empty;
			int maxValue = int.MaxValue;
			string text = null;
			DateTime creationTime = DateTime.MinValue;
			bool? flag = null;
			List<FidMid> list = original ? this.originalConversationMessages : this.conversationMessages;
			foreach (FidMid fidMid in list)
			{
				if (!value.Filtered || ((filterList != null) ? filterList.Contains(fidMid) : (fidMid.FolderId == folderId)))
				{
					if (value.UnreadOnly)
					{
						object messagePropertyValue = this.GetMessagePropertyValue(fidMid, PropTag.Message.Read, original);
						if (messagePropertyValue != null && (bool)messagePropertyValue)
						{
							continue;
						}
					}
					object obj2 = this.GetMessagePropertyValue(fidMid, value.AggregatedProperty, original);
					string text2 = obj2 as string;
					if (!ConversationMembers.ShouldSkipNullAndEmptyValue(value.Type) || (obj2 != null && (text2 == null || !(text2 == string.Empty))))
					{
						if (text2 != null && text2.Length > 255 && (value.Type != ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.Union || value.AggregatedProperty != PropTag.Message.DisplayTo))
						{
							obj2 = text2.Substring(0, 255);
						}
						if (!(PropTag.Message.IconIndex == value.AggregatedProperty) || ConversationMembers.validIconIndexes.Contains((int)obj2))
						{
							if (PropTag.Message.HasAttach == value.AggregatedProperty && (bool)obj2)
							{
								object messagePropertyValue2 = this.GetMessagePropertyValue(fidMid, ConversationMembers.GetAllAttachmentsAreHiddenPropTag(context, this.mailbox), original);
								if (messagePropertyValue2 != null && (bool)messagePropertyValue2)
								{
									obj2 = false;
								}
							}
							bool flag2 = false;
							switch (value.Type)
							{
							case ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.FromNewestMessage:
								obj = obj2;
								flag2 = true;
								break;
							case ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.MaxInOriginPreferenceOrder:
							{
								string text3 = (string)this.GetMessagePropertyValue(fidMid, PropTag.Message.PartnerNetworkId, original);
								DateTime valueOrDefault = ((DateTime?)this.GetMessagePropertyValue(fidMid, PropTag.Message.CreationTime, original)).GetValueOrDefault(DateTime.MinValue);
								if (OriginPreferenceComparer.Instance.Compare(context, this.mailbox, proptag, obj, origin, creationTime, obj2, text3, valueOrDefault) < 0)
								{
									obj = obj2;
									origin = text3;
									creationTime = valueOrDefault;
								}
								break;
							}
							case ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.Max:
								if (ValueHelper.ValuesCompare(obj, obj2, (context.Culture == null) ? null : context.Culture.CompareInfo, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth) < 0)
								{
									obj = obj2;
								}
								break;
							case ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.Sum:
								obj = (int)((obj == null) ? 0 : obj) + (int)obj2;
								break;
							case ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.Union:
							{
								if (obj == null)
								{
									obj = new Dictionary<object, DateTime>(new ValueEqualityComparer((context.Culture == null) ? null : context.Culture.CompareInfo));
								}
								IDictionary<object, DateTime> multivalues = (IDictionary<object, DateTime>)obj;
								DateTime valueOrDefault2 = ((DateTime?)this.GetMessagePropertyValue(fidMid, PropTag.Message.MessageDeliveryTime, original)).GetValueOrDefault(DateTime.MinValue);
								if (PropTag.Message.DisplayTo == value.AggregatedProperty)
								{
									foreach (string text4 in ((string)obj2).Split(ConversationMembers.DisplayToSeparators, StringSplitOptions.RemoveEmptyEntries))
									{
										string value2 = text4;
										if (text4.Length > 255)
										{
											value2 = text4.Substring(0, 255);
										}
										ConversationMembers.AddToMultiValueCollection(multivalues, value2, valueOrDefault2);
									}
								}
								else if (value.AggregatedProperty.IsMultiValued)
								{
									Array array2 = (Array)obj2;
									for (int j = 0; j < array2.Length; j++)
									{
										ConversationMembers.AddToMultiValueCollection(multivalues, array2.GetValue(j), valueOrDefault2);
									}
								}
								else
								{
									ConversationMembers.AddToMultiValueCollection(multivalues, obj2, valueOrDefault2);
								}
								break;
							}
							case ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.DisplayNamePriorityOrder:
								this.UpdateAggregatedValueBasedOnPriorityAndOriginPreference(context, proptag, ref obj, ref origin, ref maxValue, ref text, ref creationTime, fidMid, obj2, original);
								break;
							case ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.MinInRelevanceScore:
								if (obj2 == null || !(obj2 is int))
								{
									obj2 = int.MaxValue;
								}
								if (obj == null || (int)obj > (int)obj2)
								{
									obj = obj2;
								}
								break;
							case ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.BooleanAnd:
								flag = new bool?((flag == null || flag.Value) && (bool)((obj2 == null) ? false : obj2));
								if (!flag.Value)
								{
									flag2 = true;
								}
								break;
							case ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.FromInitialMessage:
							{
								byte[] conversationIndex = this.GetMessagePropertyValue(fidMid, PropTag.Message.ConversationIndex, original) as byte[];
								DateTime valueOrDefault3 = ((DateTime?)this.GetMessagePropertyValue(fidMid, PropTag.Message.MessageDeliveryTime, original)).GetValueOrDefault(DateTime.MinValue);
								if (initialMessageFinder == null)
								{
									initialMessageFinder = new InitialMessageFinder(this.conversationMessages.Count<FidMid>());
								}
								initialMessageFinder.AddMessage(obj2, valueOrDefault3, conversationIndex);
								break;
							}
							case ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.UnionNotUnique_NullIfAllNotSet:
							{
								if (value.AggregatedProperty.IsMultiValued)
								{
									continue;
								}
								if (obj == null)
								{
									obj = new List<KeyValuePair<DateTime, object>>();
								}
								if (obj2 == null)
								{
									obj2 = ConversationMembers.GetAggregatedPropertyDefaultValue(value.AggregatedProperty);
								}
								List<KeyValuePair<DateTime, object>> list2 = (List<KeyValuePair<DateTime, object>>)obj;
								DateTime valueOrDefault4 = ((DateTime?)this.GetMessagePropertyValue(fidMid, PropTag.Message.MessageDeliveryTime, original)).GetValueOrDefault(DateTime.MinValue);
								list2.Add(new KeyValuePair<DateTime, object>(valueOrDefault4, obj2));
								break;
							}
							}
							if (flag2)
							{
								break;
							}
						}
					}
				}
			}
			if (ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.Union == value.Type && obj != null)
			{
				obj = ConversationMembers.ConvertMultivalueCollectionToMvArray(proptag, (IDictionary<object, DateTime>)obj);
			}
			if (ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.BooleanAnd == value.Type && flag != null)
			{
				obj = (flag.Value ? SerializedValue.BoxedTrue : SerializedValue.BoxedFalse);
			}
			if (ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.FromInitialMessage == value.Type && initialMessageFinder != null)
			{
				obj = initialMessageFinder.GetInitialMessagePropertyValue();
			}
			if (ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.UnionNotUnique_NullIfAllNotSet == value.Type && obj != null)
			{
				object aggregatedPropertyDefaultValue = ConversationMembers.GetAggregatedPropertyDefaultValue(value.AggregatedProperty);
				obj = ConversationMembers.ConvertUnionNotUniqueMvCollectionToMvArray(proptag, (List<KeyValuePair<DateTime, object>>)obj, aggregatedPropertyDefaultValue);
			}
			return obj;
		}

		private static ConversationMembers.ConversationPropertyAggregationInfo? GetAggregationInfo(Context context, Mailbox mailbox, StorePropTag proptag)
		{
			ConversationMembers.ConversationPropertyAggregationInfo aggregationInfo;
			if (!ConversationMembers.conversationPropertyMapping.TryGetValue(proptag, out aggregationInfo))
			{
				return null;
			}
			return new ConversationMembers.ConversationPropertyAggregationInfo?(ConversationMembers.TranslateAggregationInfoIfNecessary(context, mailbox, proptag, aggregationInfo));
		}

		private static ConversationMembers.ConversationPropertyAggregationInfo TranslateAggregationInfoIfNecessary(Context context, Mailbox mailbox, StorePropTag proptag, ConversationMembers.ConversationPropertyAggregationInfo aggregationInfo)
		{
			if (PropTag.Message.ConversationCategories == proptag || PropTag.Message.ConversationCategoriesMailboxWide == proptag)
			{
				aggregationInfo = ConversationMembers.GetKeywordsAggregationInfo(context, mailbox, aggregationInfo);
			}
			else if (PropTag.Message.PersonFileAsMailboxWide == proptag)
			{
				aggregationInfo = ConversationMembers.GetFileAsAggregationInfo(context, mailbox, aggregationInfo);
			}
			else if (PropTag.Message.PersonWorkCityMailboxWide == proptag)
			{
				aggregationInfo = ConversationMembers.GetWorkCityAggregationInfo(context, mailbox, aggregationInfo);
			}
			else if (PropTag.Message.PersonDisplayNameFirstLastMailboxWide == proptag)
			{
				aggregationInfo = ConversationMembers.GetDisplayNameFirstLastAggregationInfo(context, mailbox, aggregationInfo);
			}
			else if (PropTag.Message.PersonDisplayNameLastFirstMailboxWide == proptag)
			{
				aggregationInfo = ConversationMembers.GetDisplayNameLastFirstAggregationInfo(context, mailbox, aggregationInfo);
			}
			return aggregationInfo;
		}

		private static StorePropTag GetKeywordsPropTag(Context context, Mailbox mailbox)
		{
			return mailbox.GetNamedPropStorePropTag(context, NamedPropInfo.PublicStrings.Keywords.PropName, PropertyType.MVUnicode, ObjectType.Message);
		}

		private static StorePropTag GetFileUnderPropTag(Context context, Mailbox mailbox)
		{
			return mailbox.GetNamedPropStorePropTag(context, NamedPropInfo.Address.FileUnder.PropName, PropertyType.Unicode, ObjectType.Message);
		}

		private static StorePropTag GetWorkAddressCityPropTag(Context context, Mailbox mailbox)
		{
			return mailbox.GetNamedPropStorePropTag(context, NamedPropInfo.Address.WorkAddressCity.PropName, PropertyType.Unicode, ObjectType.Message);
		}

		private static StorePropTag GetDisplayNameFirstLastPropTag(Context context, Mailbox mailbox)
		{
			return mailbox.GetNamedPropStorePropTag(context, NamedPropInfo.Address.DisplayNameFirstLast.PropName, PropertyType.Unicode, ObjectType.Message);
		}

		private static StorePropTag GetDisplayNameLastFirstPropTag(Context context, Mailbox mailbox)
		{
			return mailbox.GetNamedPropStorePropTag(context, NamedPropInfo.Address.DisplayNameLastFirst.PropName, PropertyType.Unicode, ObjectType.Message);
		}

		private static StorePropTag GetDisplayNamePriorityPropTag(Context context, Mailbox mailbox)
		{
			return mailbox.GetNamedPropStorePropTag(context, NamedPropInfo.Address.DisplayNamePriority.PropName, PropertyType.Int32, ObjectType.Message);
		}

		private static StorePropTag GetAllAttachmentsAreHiddenPropTag(Context context, Mailbox mailbox)
		{
			return mailbox.GetNamedPropStorePropTag(context, NamedPropInfo.Common.SmartNoAttach.PropName, PropertyType.Boolean, ObjectType.Message);
		}

		private static void AddToMultiValueCollection(IDictionary<object, DateTime> multivalues, object value, DateTime messageTime)
		{
			if (!multivalues.ContainsKey(value) || multivalues[value] < messageTime)
			{
				multivalues[value] = messageTime;
			}
		}

		private static bool PropTagIsConversationFlagCompleteTime(StorePropTag proptag)
		{
			return PropTag.Message.ConversationFlagCompleteTime == proptag || PropTag.Message.ConversationFlagCompleteTimeMailboxWide == proptag;
		}

		private static object ConvertMultivalueCollectionToMvArray(StorePropTag proptag, IEnumerable<KeyValuePair<object, DateTime>> objectList)
		{
			IEnumerable<object> values = (from pair in objectList
			orderby pair.Value descending
			select pair.Key).Take(100);
			return ConversationMembers.CastCollectionToMvArray(proptag, values);
		}

		private static object ConvertUnionNotUniqueMvCollectionToMvArray(StorePropTag proptag, List<KeyValuePair<DateTime, object>> objectList, object aggregatedPropertyDefaultValue)
		{
			if (objectList.All((KeyValuePair<DateTime, object> pair) => ConversationMembers.CompareMvArrayItemValues(pair.Value, aggregatedPropertyDefaultValue, proptag)))
			{
				return null;
			}
			IEnumerable<object> values = (from pair in objectList
			orderby pair.Key descending
			select pair.Value).Take(100);
			return ConversationMembers.CastCollectionToMvArray(proptag, values);
		}

		private static object CastCollectionToMvArray(StorePropTag proptag, IEnumerable<object> values)
		{
			PropertyType propType = proptag.PropType;
			if (propType <= PropertyType.MVUnicode)
			{
				switch (propType)
				{
				case PropertyType.MVInt16:
					return ConversationMembers.CastToArrayOf<short>(values);
				case PropertyType.MVInt32:
					return ConversationMembers.CastToArrayOf<int>(values);
				default:
					if (propType == PropertyType.MVUnicode)
					{
						return ConversationMembers.CastToArrayOf<string>(values);
					}
					break;
				}
			}
			else
			{
				if (propType == PropertyType.MVSysTime)
				{
					return ConversationMembers.CastToArrayOf<DateTime>(values);
				}
				if (propType == PropertyType.MVBinary)
				{
					return ConversationMembers.CastToArrayOf<byte[]>(values);
				}
			}
			return null;
		}

		private static object GetAggregatedPropertyDefaultValue(StorePropTag proptag)
		{
			if (proptag == PropTag.Message.RichContent)
			{
				short num = 0;
				return num;
			}
			return null;
		}

		private static bool CompareMvArrayItemValues(object val1, object val2, StorePropTag proptag)
		{
			PropertyType propType = proptag.PropType;
			if (propType <= PropertyType.MVUnicode)
			{
				switch (propType)
				{
				case PropertyType.MVInt16:
					return (short)val1 == (short)val2;
				case PropertyType.MVInt32:
					return (int)val1 == (int)val2;
				default:
					if (propType == PropertyType.MVUnicode)
					{
						return ((string)val1).Equals((string)val2);
					}
					break;
				}
			}
			else
			{
				if (propType == PropertyType.MVSysTime)
				{
					return ((DateTime)val1).Equals((DateTime)val2);
				}
				if (propType == PropertyType.MVBinary)
				{
					return (byte)val1 == (byte)val2;
				}
			}
			return false;
		}

		private static T[] CastToArrayOf<T>(IEnumerable<object> objectList)
		{
			return objectList.Cast<T>().ToArray<T>();
		}

		internal static byte[] BuildFidMid(ExchangeId fid, ExchangeId mid)
		{
			byte[] array = new byte[16];
			ExchangeIdHelpers.To8ByteArray(fid.Replid, fid.Counter, array, 0);
			ExchangeIdHelpers.To8ByteArray(mid.Replid, mid.Counter, array, 8);
			return array;
		}

		private static ConversationMembers.ConversationPropertyAggregationInfo GetKeywordsAggregationInfo(Context context, Mailbox mailbox, ConversationMembers.ConversationPropertyAggregationInfo aggregationInfo)
		{
			StorePropTag keywordsPropTag = ConversationMembers.GetKeywordsPropTag(context, mailbox);
			return new ConversationMembers.ConversationPropertyAggregationInfo(aggregationInfo.Type, aggregationInfo.Scope, keywordsPropTag);
		}

		private static ConversationMembers.ConversationPropertyAggregationInfo GetFileAsAggregationInfo(Context context, Mailbox mailbox, ConversationMembers.ConversationPropertyAggregationInfo aggregationInfo)
		{
			StorePropTag fileUnderPropTag = ConversationMembers.GetFileUnderPropTag(context, mailbox);
			return new ConversationMembers.ConversationPropertyAggregationInfo(aggregationInfo.Type, aggregationInfo.Scope, fileUnderPropTag);
		}

		private static ConversationMembers.ConversationPropertyAggregationInfo GetWorkCityAggregationInfo(Context context, Mailbox mailbox, ConversationMembers.ConversationPropertyAggregationInfo aggregationInfo)
		{
			StorePropTag workAddressCityPropTag = ConversationMembers.GetWorkAddressCityPropTag(context, mailbox);
			return new ConversationMembers.ConversationPropertyAggregationInfo(aggregationInfo.Type, aggregationInfo.Scope, workAddressCityPropTag);
		}

		private static ConversationMembers.ConversationPropertyAggregationInfo GetDisplayNameFirstLastAggregationInfo(Context context, Mailbox mailbox, ConversationMembers.ConversationPropertyAggregationInfo aggregationInfo)
		{
			StorePropTag displayNameFirstLastPropTag = ConversationMembers.GetDisplayNameFirstLastPropTag(context, mailbox);
			return new ConversationMembers.ConversationPropertyAggregationInfo(aggregationInfo.Type, aggregationInfo.Scope, displayNameFirstLastPropTag);
		}

		private static ConversationMembers.ConversationPropertyAggregationInfo GetDisplayNameLastFirstAggregationInfo(Context context, Mailbox mailbox, ConversationMembers.ConversationPropertyAggregationInfo aggregationInfo)
		{
			StorePropTag displayNameLastFirstPropTag = ConversationMembers.GetDisplayNameLastFirstPropTag(context, mailbox);
			return new ConversationMembers.ConversationPropertyAggregationInfo(aggregationInfo.Type, aggregationInfo.Scope, displayNameLastFirstPropTag);
		}

		private IList<StorePropTag> GetPropertiesToCache(Context context)
		{
			HashSet<StorePropTag> hashSet = new HashSet<StorePropTag>();
			hashSet.Add(PropTag.Message.CreationTime);
			hashSet.Add(PropTag.Message.DocumentId);
			foreach (KeyValuePair<StorePropTag, ConversationMembers.ConversationPropertyAggregationInfo> keyValuePair in ConversationMembers.conversationPropertyMapping)
			{
				if (this.aggregatePropertiesToCompute == null || this.aggregatePropertiesToCompute.Contains(keyValuePair.Key))
				{
					ConversationMembers.ConversationPropertyAggregationInfo conversationPropertyAggregationInfo = ConversationMembers.TranslateAggregationInfoIfNecessary(context, this.mailbox, keyValuePair.Key, keyValuePair.Value);
					if (conversationPropertyAggregationInfo.AggregatedProperty != PropTag.Message.FidMid && conversationPropertyAggregationInfo.AggregatedProperty != PropTag.Message.ContentCount)
					{
						hashSet.Add(conversationPropertyAggregationInfo.AggregatedProperty);
					}
					if (conversationPropertyAggregationInfo.UnreadOnly)
					{
						hashSet.Add(PropTag.Message.Read);
					}
					if (conversationPropertyAggregationInfo.AggregatedProperty == PropTag.Message.HasAttach)
					{
						hashSet.Add(ConversationMembers.GetAllAttachmentsAreHiddenPropTag(context, this.mailbox));
					}
					if (conversationPropertyAggregationInfo.Type == ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.FromInitialMessage)
					{
						hashSet.Add(PropTag.Message.ConversationIndex);
						hashSet.Add(PropTag.Message.MessageDeliveryTime);
					}
					else if (conversationPropertyAggregationInfo.Type == ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.DisplayNamePriorityOrder)
					{
						hashSet.Add(PropTag.Message.PartnerNetworkId);
						hashSet.Add(ConversationMembers.GetDisplayNamePriorityPropTag(context, this.mailbox));
						hashSet.Add(ConversationMembers.GetDisplayNameFirstLastPropTag(context, this.mailbox));
					}
					else if (conversationPropertyAggregationInfo.Type == ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.MaxInOriginPreferenceOrder)
					{
						hashSet.Add(PropTag.Message.PartnerNetworkId);
					}
					else if (conversationPropertyAggregationInfo.Type == ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.Union)
					{
						hashSet.Add(PropTag.Message.MessageDeliveryTime);
					}
				}
			}
			return hashSet.ToList<StorePropTag>();
		}

		private void CacheMessageProperties(Context context, bool original)
		{
			if ((original ? this.cachedOriginalMessageProperties : this.cachedMessageProperties) != null)
			{
				return;
			}
			IList<FidMid> list = original ? this.originalConversationMessages : this.conversationMessages;
			Dictionary<FidMid, Dictionary<StorePropTag, object>> dictionary = new Dictionary<FidMid, Dictionary<StorePropTag, object>>(list.Count);
			IList<StorePropTag> propertiesToCache = this.GetPropertiesToCache(context);
			ExchangeId exchangeId = ExchangeId.Null;
			ExchangeId exchangeId2 = ExchangeId.Null;
			bool flag = false;
			if (this.modifiedMessage != null)
			{
				if (!original)
				{
					exchangeId = this.modifiedMessage.GetFolderId(context);
					exchangeId2 = this.modifiedMessage.GetId(context);
				}
				else
				{
					exchangeId = this.modifiedMessage.GetOriginalFolderId(context);
					exchangeId2 = this.modifiedMessage.OriginalMessageID;
				}
				FidMid item = new FidMid(exchangeId, exchangeId2);
				if (list.Contains(item))
				{
					list = new List<FidMid>(list);
					list.Remove(item);
					flag = true;
				}
			}
			if (list.Count > 0)
			{
				Dictionary<FidMid, Dictionary<StorePropTag, object>> dictionary2 = original ? this.cachedMessageProperties : this.cachedOriginalMessageProperties;
				if (dictionary2 != null)
				{
					using (IEnumerator<FidMid> enumerator = list.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							FidMid key = enumerator.Current;
							Dictionary<StorePropTag, object> value = dictionary2[key];
							dictionary[key] = value;
						}
						goto IL_43D;
					}
				}
				MessageTable messageTable = DatabaseSchema.MessageTable(this.mailbox.Database);
				ConversationMembersBlobTableFunction conversationMembersBlobTableFunction = DatabaseSchema.ConversationMembersBlobTableFunction(this.mailbox.Database);
				List<Column> list2 = new List<Column>(propertiesToCache.Count + 2);
				for (int i = 0; i < propertiesToCache.Count; i++)
				{
					list2.Add(PropertySchema.MapToColumn(context.Database, ObjectType.Message, propertiesToCache[i]));
				}
				list2.Add(messageTable.FolderId);
				list2.Add(messageTable.MessageId);
				List<ConversationMembersBlob> list3 = new List<ConversationMembersBlob>(list.Count);
				for (int j = 0; j < list.Count; j++)
				{
					list3.Add(new ConversationMembersBlob(list[j].FolderId.To26ByteArray(), list[j].MessageId.To26ByteArray(), j));
				}
				byte[] array = ConversationMembersBlob.Serialize(list3);
				using (TableFunctionOperator tableFunctionOperator = Factory.CreateTableFunctionOperator(context.Culture, context, conversationMembersBlobTableFunction.TableFunction, new object[]
				{
					array
				}, messageTable.MessageUnique.Columns, null, new Dictionary<Column, Column>(4)
				{
					{
						messageTable.MailboxPartitionNumber,
						Factory.CreateConstantColumn(this.mailbox.MailboxPartitionNumber, messageTable.MailboxPartitionNumber)
					},
					{
						messageTable.FolderId,
						conversationMembersBlobTableFunction.FolderId
					},
					{
						messageTable.MessageId,
						conversationMembersBlobTableFunction.MessageId
					},
					{
						messageTable.IsHidden,
						Factory.CreateConstantColumn(false, messageTable.IsHidden)
					}
				}, 0, 0, KeyRange.AllRows, false, true))
				{
					using (JoinOperator joinOperator = Factory.CreateJoinOperator(context.Culture, context, messageTable.Table, list2, null, new Dictionary<Column, Column>(4)
					{
						{
							messageTable.VirtualIsRead,
							messageTable.IsRead
						}
					}, 0, 0, messageTable.MessageUnique.Columns, tableFunctionOperator, true))
					{
						using (Reader reader = joinOperator.ExecuteReader(true))
						{
							while (reader.Read())
							{
								Dictionary<StorePropTag, object> dictionary3 = new Dictionary<StorePropTag, object>(propertiesToCache.Count<StorePropTag>() + 2);
								for (int k = 0; k < propertiesToCache.Count; k++)
								{
									StorePropTag key2 = propertiesToCache[k];
									dictionary3[key2] = reader.GetValue(list2[k]);
								}
								ExchangeId exchangeId3 = ExchangeId.CreateFrom26ByteArray(context, this.mailbox.ReplidGuidMap, reader.GetBinary(messageTable.FolderId));
								ExchangeId exchangeId4 = ExchangeId.CreateFrom26ByteArray(context, this.mailbox.ReplidGuidMap, reader.GetBinary(messageTable.MessageId));
								dictionary3[PropTag.Message.ContentCount] = 1;
								dictionary3[PropTag.Message.FidMid] = ConversationMembers.BuildFidMid(exchangeId3, exchangeId4);
								dictionary[new FidMid(exchangeId3, exchangeId4)] = dictionary3;
							}
						}
					}
				}
			}
			IL_43D:
			if (flag)
			{
				Dictionary<StorePropTag, object> dictionary4 = new Dictionary<StorePropTag, object>(propertiesToCache.Count<StorePropTag>() + 2);
				foreach (StorePropTag storePropTag in propertiesToCache)
				{
					dictionary4[storePropTag] = (original ? this.modifiedMessage.GetOriginalPropertyValue(context, storePropTag) : this.modifiedMessage.GetPropertyValue(context, storePropTag));
				}
				dictionary4[PropTag.Message.ContentCount] = 1;
				dictionary4[PropTag.Message.FidMid] = ConversationMembers.BuildFidMid(exchangeId, exchangeId2);
				dictionary[new FidMid(exchangeId, exchangeId2)] = dictionary4;
			}
			List<FidMid> list4;
			if (original)
			{
				this.cachedOriginalMessageProperties = dictionary;
				list4 = this.originalConversationMessages;
			}
			else
			{
				this.cachedMessageProperties = dictionary;
				list4 = this.conversationMessages;
			}
			if (list4.Count > 1)
			{
				list4.Sort(delegate(FidMid lhs, FidMid rhs)
				{
					DateTime valueOrDefault = ((DateTime?)this.GetMessagePropertyValue(lhs, PropTag.Message.CreationTime, original)).GetValueOrDefault(DateTime.MinValue);
					int num = ((DateTime?)this.GetMessagePropertyValue(rhs, PropTag.Message.CreationTime, original)).GetValueOrDefault(DateTime.MinValue).CompareTo(valueOrDefault);
					if (num == 0)
					{
						int value2 = (int)this.GetMessagePropertyValue(lhs, PropTag.Message.DocumentId, original);
						num = ((int)this.GetMessagePropertyValue(rhs, PropTag.Message.DocumentId, original)).CompareTo(value2);
					}
					return num;
				});
			}
		}

		private void UpdateAggregatedValueBasedOnPriorityAndOriginPreference(Context context, StorePropTag propTag, ref object aggregatePropertyValue, ref string aggregatePropertyOrigin, ref int aggregatePropertyPriority, ref string aggregatePropertyDisplayName, ref DateTime aggregatePropertyCreationTime, FidMid fidMid, object propertyValue, bool original)
		{
			StorePropTag displayNamePriorityPropTag = ConversationMembers.GetDisplayNamePriorityPropTag(context, this.mailbox);
			StorePropTag displayNameFirstLastPropTag = ConversationMembers.GetDisplayNameFirstLastPropTag(context, this.mailbox);
			int valueOrDefault = ((int?)this.GetMessagePropertyValue(fidMid, displayNamePriorityPropTag, original)).GetValueOrDefault(int.MaxValue);
			string text = (string)this.GetMessagePropertyValue(fidMid, displayNameFirstLastPropTag, original);
			string text2 = (string)this.GetMessagePropertyValue(fidMid, PropTag.Message.PartnerNetworkId, original);
			DateTime valueOrDefault2 = ((DateTime?)this.GetMessagePropertyValue(fidMid, PropTag.Message.CreationTime, original)).GetValueOrDefault(DateTime.MinValue);
			if (aggregatePropertyPriority > valueOrDefault)
			{
				aggregatePropertyValue = propertyValue;
				aggregatePropertyDisplayName = text;
				aggregatePropertyPriority = valueOrDefault;
				aggregatePropertyOrigin = text2;
				aggregatePropertyCreationTime = valueOrDefault2;
				return;
			}
			if (aggregatePropertyPriority == valueOrDefault && OriginPreferenceComparer.Instance.Compare(context, this.mailbox, propTag, aggregatePropertyDisplayName, aggregatePropertyOrigin, aggregatePropertyCreationTime, text, text2, valueOrDefault2) <= 0)
			{
				aggregatePropertyValue = propertyValue;
				aggregatePropertyDisplayName = text;
				aggregatePropertyPriority = valueOrDefault;
				aggregatePropertyOrigin = text2;
				aggregatePropertyCreationTime = valueOrDefault2;
			}
		}

		private object GetMessagePropertyValue(FidMid fidMid, StorePropTag propTag, bool original)
		{
			return (original ? this.cachedOriginalMessageProperties : this.cachedMessageProperties)[fidMid][propTag];
		}

		private bool ConversationIsFlagged(Context context, bool folderScope, ICollection<FidMid> filterList, ExchangeId folderId, bool original)
		{
			object aggregateProperty = this.GetAggregateProperty(context, folderScope ? PropTag.Message.ConversationFlagStatus : PropTag.Message.ConversationFlagStatusMailboxWide, filterList, folderId, original);
			return aggregateProperty != null && 1 == (int)aggregateProperty;
		}

		internal const int MaxMultivalueInstances = 100;

		internal const int MaxStringValueLength = 255;

		private const int RelevanceScoreForIrrelevantContactEntries = 2147483647;

		internal static readonly string[] DisplayToSeparators = new string[]
		{
			"; "
		};

		private static readonly int[] validIconIndexes = new int[]
		{
			261,
			262,
			275,
			276,
			277,
			278
		};

		private static readonly Dictionary<StorePropTag, ConversationMembers.ConversationPropertyAggregationInfo> conversationPropertyMapping = new Dictionary<StorePropTag, ConversationMembers.ConversationPropertyAggregationInfo>
		{
			{
				PropTag.Message.ConversationTopic,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.FromNewestMessage, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.All, PropTag.Message.NormalizedSubject)
			},
			{
				PropTag.Message.ConversationMvFrom,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.Union, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.Filtered, PropTag.Message.SentRepresentingName)
			},
			{
				PropTag.Message.ConversationMvFromMailboxWide,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.Union, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.All, PropTag.Message.SentRepresentingName)
			},
			{
				PropTag.Message.ConversationMvTo,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.Union, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.Filtered, PropTag.Message.DisplayTo)
			},
			{
				PropTag.Message.ConversationMvToMailboxWide,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.Union, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.All, PropTag.Message.DisplayTo)
			},
			{
				PropTag.Message.ConversationMessageDeliveryTime,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.Max, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.Filtered, PropTag.Message.MessageDeliveryTime)
			},
			{
				PropTag.Message.ConversationMessageDeliveryTimeMailboxWide,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.Max, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.All, PropTag.Message.MessageDeliveryTime)
			},
			{
				PropTag.Message.ConversationCategories,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.Union, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.Filtered, StorePropTag.Invalid)
			},
			{
				PropTag.Message.ConversationCategoriesMailboxWide,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.Union, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.All, StorePropTag.Invalid)
			},
			{
				PropTag.Message.ConversationFlagStatus,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.Max, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.Filtered, PropTag.Message.FlagStatus)
			},
			{
				PropTag.Message.ConversationFlagStatusMailboxWide,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.Max, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.All, PropTag.Message.FlagStatus)
			},
			{
				PropTag.Message.ConversationFlagCompleteTime,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.Max, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.Filtered, PropTag.Message.FlagCompleteTime)
			},
			{
				PropTag.Message.ConversationFlagCompleteTimeMailboxWide,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.Max, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.All, PropTag.Message.FlagCompleteTime)
			},
			{
				PropTag.Message.ConversationHasAttach,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.Max, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.Filtered, PropTag.Message.HasAttach)
			},
			{
				PropTag.Message.ConversationHasAttachMailboxWide,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.Max, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.All, PropTag.Message.HasAttach)
			},
			{
				PropTag.Message.ConversationContentCount,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.Sum, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.Filtered, PropTag.Message.ContentCount)
			},
			{
				PropTag.Message.ConversationContentCountMailboxWide,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.Sum, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.All, PropTag.Message.ContentCount)
			},
			{
				PropTag.Message.ConversationContentUnread,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.Sum, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.Filtered | ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.UnreadOnly, PropTag.Message.ContentCount)
			},
			{
				PropTag.Message.ConversationContentUnreadMailboxWide,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.Sum, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.All | ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.UnreadOnly, PropTag.Message.ContentCount)
			},
			{
				PropTag.Message.ConversationMessageSize,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.Sum, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.Filtered, PropTag.Message.MessageSize32)
			},
			{
				PropTag.Message.ConversationMessageSizeMailboxWide,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.Sum, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.All, PropTag.Message.MessageSize32)
			},
			{
				PropTag.Message.ConversationMessageClasses,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.Union, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.Filtered, PropTag.Message.MessageClass)
			},
			{
				PropTag.Message.ConversationMessageClassesMailboxWide,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.Union, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.All, PropTag.Message.MessageClass)
			},
			{
				PropTag.Message.ConversationReplyForwardState,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.FromNewestMessage, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.Filtered, PropTag.Message.IconIndex)
			},
			{
				PropTag.Message.ConversationReplyForwardStateMailboxWide,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.FromNewestMessage, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.All, PropTag.Message.IconIndex)
			},
			{
				PropTag.Message.ConversationImportance,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.Max, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.Filtered, PropTag.Message.Importance)
			},
			{
				PropTag.Message.ConversationImportanceMailboxWide,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.Max, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.All, PropTag.Message.Importance)
			},
			{
				PropTag.Message.ConversationMvFromUnread,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.Union, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.Filtered | ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.UnreadOnly, PropTag.Message.SentRepresentingName)
			},
			{
				PropTag.Message.ConversationMvFromUnreadMailboxWide,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.Union, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.All | ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.UnreadOnly, PropTag.Message.SentRepresentingName)
			},
			{
				PropTag.Message.ConversationMvItemIds,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.Union, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.Filtered, PropTag.Message.FidMid)
			},
			{
				PropTag.Message.ConversationMvItemIdsMailboxWide,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.Union, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.All, PropTag.Message.FidMid)
			},
			{
				PropTag.Message.ConversationHasIrm,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.Max, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.Filtered, PropTag.Message.IsIRMMessage)
			},
			{
				PropTag.Message.ConversationHasIrmMailboxWide,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.Max, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.All, PropTag.Message.IsIRMMessage)
			},
			{
				PropTag.Message.PersonCompanyNameMailboxWide,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.MaxInOriginPreferenceOrder, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.All, PropTag.Message.CompanyName)
			},
			{
				PropTag.Message.PersonDisplayNameMailboxWide,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.DisplayNamePriorityOrder, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.All, PropTag.Message.DisplayName)
			},
			{
				PropTag.Message.PersonGivenNameMailboxWide,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.DisplayNamePriorityOrder, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.All, PropTag.Message.GivenName)
			},
			{
				PropTag.Message.PersonSurnameMailboxWide,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.DisplayNamePriorityOrder, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.All, PropTag.Message.SurName)
			},
			{
				PropTag.Message.PersonRelevanceScoreMailboxWide,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.MinInRelevanceScore, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.All, PropTag.Message.RelevanceScore)
			},
			{
				PropTag.Message.PersonHomeCityMailboxWide,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.MaxInOriginPreferenceOrder, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.All, PropTag.Message.HomeAddressCity)
			},
			{
				PropTag.Message.PersonCreationTimeMailboxWide,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.Max, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.All, PropTag.Message.CreationTime)
			},
			{
				PropTag.Message.PersonFileAsMailboxWide,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.DisplayNamePriorityOrder, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.All, StorePropTag.Invalid)
			},
			{
				PropTag.Message.PersonWorkCityMailboxWide,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.MaxInOriginPreferenceOrder, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.All, StorePropTag.Invalid)
			},
			{
				PropTag.Message.PersonDisplayNameFirstLastMailboxWide,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.DisplayNamePriorityOrder, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.All, StorePropTag.Invalid)
			},
			{
				PropTag.Message.PersonDisplayNameLastFirstMailboxWide,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.DisplayNamePriorityOrder, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.All, StorePropTag.Invalid)
			},
			{
				PropTag.Message.ConversationLastMemberDocumentId,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.FromNewestMessage, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.Filtered, PropTag.Message.DocumentId)
			},
			{
				PropTag.Message.ConversationLastMemberDocumentIdMailboxWide,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.FromNewestMessage, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.All, PropTag.Message.DocumentId)
			},
			{
				PropTag.Message.ConversationHasClutter,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.BooleanAnd, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.Filtered, PropTag.Message.IsClutter)
			},
			{
				PropTag.Message.ConversationHasClutterMailboxWide,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.BooleanAnd, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.All, PropTag.Message.IsClutter)
			},
			{
				PropTag.Message.ConversationInitialMemberDocumentId,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.FromInitialMessage, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.Filtered, PropTag.Message.DocumentId)
			},
			{
				PropTag.Message.ConversationMemberDocumentIds,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.Union, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.Filtered, PropTag.Message.DocumentId)
			},
			{
				PropTag.Message.ConversationMessageDeliveryOrRenewTimeMailboxWide,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.Max, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.All, PropTag.Message.DeliveryOrRenewTime)
			},
			{
				PropTag.Message.FamilyId,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.FromNewestMessage, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.All, PropTag.Message.ConversationFamilyId)
			},
			{
				PropTag.Message.ConversationMessageRichContentMailboxWide,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.UnionNotUnique_NullIfAllNotSet, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.All, PropTag.Message.RichContent)
			},
			{
				PropTag.Message.ConversationMessageDeliveryOrRenewTime,
				new ConversationMembers.ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType.Max, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.Filtered, PropTag.Message.DeliveryOrRenewTime)
			}
		};

		private readonly Mailbox mailbox;

		private readonly List<FidMid> conversationMessages;

		private readonly List<FidMid> originalConversationMessages;

		private readonly TopMessage modifiedMessage;

		private Dictionary<FidMid, Dictionary<StorePropTag, object>> cachedMessageProperties;

		private Dictionary<FidMid, Dictionary<StorePropTag, object>> cachedOriginalMessageProperties;

		private HashSet<StorePropTag> aggregatePropertiesToCompute;

		private struct ConversationPropertyAggregationInfo
		{
			public ConversationPropertyAggregationInfo(ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType type, ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope scope, StorePropTag proptag)
			{
				this.type = type;
				this.scope = scope;
				this.proptag = proptag;
			}

			public ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType Type
			{
				get
				{
					return this.type;
				}
			}

			public ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope Scope
			{
				get
				{
					return this.scope;
				}
			}

			public bool UnreadOnly
			{
				get
				{
					return (ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope)0 != (this.Scope & ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.UnreadOnly);
				}
			}

			public bool Filtered
			{
				get
				{
					return (ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope)0 != (this.Scope & ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope.Filtered);
				}
			}

			public StorePropTag AggregatedProperty
			{
				get
				{
					return this.proptag;
				}
			}

			private readonly ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationType type;

			private readonly ConversationMembers.ConversationPropertyAggregationInfo.ConversationPropertyAggregationScope scope;

			private readonly StorePropTag proptag;

			public enum ConversationPropertyAggregationType
			{
				FromNewestMessage,
				MaxInOriginPreferenceOrder,
				Max,
				Sum,
				Union,
				DisplayNamePriorityOrder,
				MinInRelevanceScore,
				BooleanAnd,
				FromInitialMessage,
				UnionNotUnique_NullIfAllNotSet
			}

			[Flags]
			public enum ConversationPropertyAggregationScope
			{
				Filtered = 1,
				All = 2,
				UnreadOnly = 4
			}
		}
	}
}
