using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AggregationByItemClassReferencesSubjectProcessor : IAggregationByItemClassReferencesSubjectProcessor
	{
		public AggregationByItemClassReferencesSubjectProcessor(IXSOFactory xsoFactory, IMailboxSession session, PropertyDefinition[] basicSearchPropertyDefinitions, ConversationIndexTrackingEx indexTrackingEx)
		{
			this.session = session;
			this.indexTrackingEx = indexTrackingEx;
			this.xsoFactory = xsoFactory;
			this.basicSearchPropertyDefinitions = basicSearchPropertyDefinitions;
		}

		private PropertyDefinition[] BasicSearchPropertyDefinitions
		{
			get
			{
				return this.basicSearchPropertyDefinitions;
			}
		}

		public static IAggregationByItemClassReferencesSubjectProcessor CreateInstance(IXSOFactory xsoFactory, IMailboxSession session, bool requestExtraPropertiesWhenSearching, ConversationIndexTrackingEx indexTrackingEx)
		{
			PropertyDefinition[] array;
			if (requestExtraPropertiesWhenSearching)
			{
				List<PropertyDefinition> list = new List<PropertyDefinition>();
				list.AddRange(AggregationByItemClassReferencesSubjectProcessor.MinimumRequiredPropertyDefinitionsList);
				list.AddRange(AggregationByItemClassReferencesSubjectProcessor.SideConversationProcessingEnabledRequiredPropertyDefinitionsList);
				array = list.ToArray();
			}
			else
			{
				array = AggregationByItemClassReferencesSubjectProcessor.MinimumRequiredPropertyDefinitionsList;
			}
			return new AggregationByItemClassReferencesSubjectProcessor(xsoFactory, session, array, indexTrackingEx);
		}

		public void Aggregate(ICorePropertyBag item, bool shouldSearchForDuplicatedMessage, out IStorePropertyBag parentItem, out ConversationIndex newIndex, out ConversationIndex.FixupStage stage)
		{
			newIndex = ConversationIndex.Empty;
			stage = ConversationIndex.FixupStage.Unknown;
			string valueOrDefault = item.GetValueOrDefault<string>(InternalSchema.ItemClass, string.Empty);
			parentItem = null;
			if (!string.IsNullOrEmpty(valueOrDefault))
			{
				if (ObjectClass.IsMeetingMessage(valueOrDefault))
				{
					if (AggregationByItemClassReferencesSubjectProcessor.FixupMeetingMessage(this.xsoFactory, this.session, new AggregationByItemClassReferencesSubjectProcessor.PropertyDefinitionListConstructorDelegate(this.GetSearchPropertyDefinitions), item, ref newIndex, ref stage, out parentItem))
					{
						return;
					}
				}
				else if (ObjectClass.IsSmsMessage(valueOrDefault))
				{
					AggregationBySmsItemClassProcessor aggregationBySmsItemClassProcessor = new AggregationBySmsItemClassProcessor(this.xsoFactory, this.session, this.indexTrackingEx);
					aggregationBySmsItemClassProcessor.Aggregate(item, out newIndex, out stage);
					return;
				}
			}
			ConversationIndex conversationIndex;
			bool flag = ConversationIndex.TryCreate(item.TryGetProperty(ItemSchema.ConversationIndex) as byte[], out conversationIndex);
			if (this.indexTrackingEx != null)
			{
				if (flag)
				{
					this.indexTrackingEx.Trace(conversationIndex);
				}
				else
				{
					this.indexTrackingEx.Trace("II", "<invalid>");
				}
			}
			if (AggregationByItemClassReferencesSubjectProcessor.SearchByReferences(this.xsoFactory, this.session, item, this.indexTrackingEx, shouldSearchForDuplicatedMessage, out parentItem, this.GetSearchPropertyDefinitions(null)))
			{
				bool flag2 = false;
				if (shouldSearchForDuplicatedMessage)
				{
					string a = item.TryGetProperty(ItemSchema.InternetMessageId) as string;
					string text = parentItem.TryGetProperty(ItemSchema.InternetMessageId) as string;
					flag2 = (!string.IsNullOrEmpty(text) && string.Equals(a, text, StringComparison.OrdinalIgnoreCase));
				}
				if (flag2)
				{
					this.CalculateBasedOnMessageWithSameInternetMessageId(item, parentItem, out stage, out newIndex);
				}
				else
				{
					this.CalculateBasedOnReferenceMessage(item, parentItem, out stage, out newIndex);
				}
			}
			string text2 = item.TryGetProperty(ItemSchema.NormalizedSubject) as string;
			if (stage == ConversationIndex.FixupStage.Unknown)
			{
				if (string.IsNullOrEmpty(item.GetValueOrDefault<string>(ItemSchema.SubjectPrefix)) && flag)
				{
					if (conversationIndex.Components.Count > 1)
					{
						byte[] incomingConversationIdBytes = ConversationId.Create(conversationIndex.Guid).GetBytes();
						int conversationIdHash = (int)AllItemsFolderHelper.GetHashValue(incomingConversationIdBytes);
						Stopwatch stopwatch = Stopwatch.StartNew();
						parentItem = this.xsoFactory.RunQueryOnAllItemsFolder<IStorePropertyBag>(this.session, AllItemsFolderHelper.SupportedSortBy.ConversationIdHash, conversationIdHash, null, delegate(QueryResult queryResult)
						{
							IStorePropertyBag[] propertyBags;
							byte[] array;
							do
							{
								propertyBags = queryResult.GetPropertyBags(1);
								if (propertyBags == null || propertyBags.Length != 1)
								{
									goto IL_69;
								}
								int? num = propertyBags[0].TryGetProperty(ItemSchema.ConversationIdHash) as int?;
								if (num == null || num.Value != conversationIdHash)
								{
									goto IL_69;
								}
								array = (propertyBags[0].TryGetProperty(InternalSchema.MapiConversationId) as byte[]);
							}
							while (array == null || !Util.CompareByteArray(incomingConversationIdBytes, array));
							return propertyBags[0];
							IL_69:
							return null;
						}, this.GetSearchPropertyDefinitions(new StorePropertyDefinition[]
						{
							InternalSchema.MapiConversationId,
							ItemSchema.ConversationIdHash
						}));
						stopwatch.Stop();
						if (this.indexTrackingEx != null)
						{
							this.indexTrackingEx.Trace("SBCID", stopwatch.ElapsedMilliseconds.ToString());
						}
						if (parentItem != null && !ConversationIndex.CompareTopics(parentItem.TryGetProperty(ItemSchema.ConversationTopic) as string, text2))
						{
							newIndex = ConversationIndex.CreateNew();
							stage = ConversationIndex.FixupStage.H11;
							if (this.indexTrackingEx != null)
							{
								string text3 = parentItem.TryGetProperty(ItemSchema.InternetMessageId) as string;
								if (text3 != null)
								{
									this.indexTrackingEx.Trace("S3", text3);
								}
							}
						}
					}
					if (stage == ConversationIndex.FixupStage.Unknown)
					{
						newIndex = conversationIndex;
						bool flag3 = parentItem != null;
						if (flag3)
						{
							bool? flag4 = parentItem.TryGetProperty(ItemSchema.ConversationIndexTracking) as bool?;
							stage = ((flag4 == null || !flag4.Value) ? ConversationIndex.FixupStage.L1 : ConversationIndex.FixupStage.H14);
						}
						else
						{
							stage = ConversationIndex.FixupStage.H4;
						}
					}
				}
				else
				{
					if (string.IsNullOrEmpty(item.GetValueOrDefault<string>(ItemSchema.InReplyTo, string.Empty)) && string.IsNullOrEmpty(item.GetValueOrDefault<string>(ItemSchema.InternetReferences, string.Empty)) && string.IsNullOrEmpty(item.GetValueOrDefault<string>(ItemSchema.SubjectPrefix)) && !flag)
					{
						TopicHashCache topicHashCache = TopicHashCache.Load(this.xsoFactory, this.session, 50);
						if (string.IsNullOrEmpty(text2) || !topicHashCache.Contains(AllItemsFolderHelper.GetHashValue(text2)))
						{
							newIndex = ConversationIndex.CreateNew();
							stage = ConversationIndex.FixupStage.H12;
						}
					}
					bool flag5;
					bool flag6;
					if (stage == ConversationIndex.FixupStage.Unknown && AggregationByItemClassReferencesSubjectProcessor.SearchByTopic(this.xsoFactory, this.session, item, this.indexTrackingEx, out parentItem, out flag5, out flag6, this.GetSearchPropertyDefinitions(new StorePropertyDefinition[]
					{
						ItemSchema.ReceivedTime,
						ItemSchema.InReplyTo,
						ItemSchema.InternetReferences
					})))
					{
						if (this.indexTrackingEx != null)
						{
							string text4 = parentItem.TryGetProperty(ItemSchema.InternetMessageId) as string;
							if (text4 != null)
							{
								this.indexTrackingEx.Trace("S2", text4);
							}
						}
						bool? flag7 = parentItem.TryGetProperty(ItemSchema.ConversationIndexTracking) as bool?;
						bool flag8 = flag7 == null || !flag7.Value;
						if (flag5)
						{
							newIndex = conversationIndex;
							stage = (flag8 ? ConversationIndex.FixupStage.L1 : ConversationIndex.FixupStage.H5);
						}
						else
						{
							ConversationIndex conversationIndex2;
							bool flag9 = ConversationIndex.TryCreate(parentItem.TryGetProperty(ItemSchema.ConversationIndex) as byte[], out conversationIndex2);
							if (flag6)
							{
								ExDateTime? valueAsNullable = item.GetValueAsNullable<ExDateTime>(ItemSchema.SentTime);
								newIndex = ((valueAsNullable != null) ? ConversationIndex.Create(conversationIndex2.Guid, valueAsNullable.Value) : ConversationIndex.Create(conversationIndex2.Guid));
								stage = (flag8 ? ConversationIndex.FixupStage.L1 : ConversationIndex.FixupStage.H6);
							}
							else
							{
								object obj = parentItem.TryGetProperty(ItemSchema.ReceivedTime);
								string valueOrDefault2 = item.GetValueOrDefault<string>(ItemSchema.SubjectPrefix);
								if (obj != null && obj is ExDateTime)
								{
									ExDateTime dt = (ExDateTime)obj;
									TimeSpan timeSpan = ExDateTime.Now - dt;
									if (timeSpan.TotalHours >= -72.0 && timeSpan.TotalHours <= 72.0 && flag9)
									{
										if (flag)
										{
											newIndex = conversationIndex.UpdateGuid(conversationIndex2.Guid);
											stage = (flag8 ? ConversationIndex.FixupStage.L1 : ConversationIndex.FixupStage.H9);
										}
										else if (!string.IsNullOrEmpty(valueOrDefault2))
										{
											ExDateTime? valueAsNullable2 = item.GetValueAsNullable<ExDateTime>(ItemSchema.SentTime);
											if (valueAsNullable2 != null)
											{
												newIndex = ConversationIndex.Create(conversationIndex2.Guid, valueAsNullable2.Value);
											}
											else
											{
												newIndex = ConversationIndex.Create(conversationIndex2.Guid);
											}
											stage = (flag8 ? ConversationIndex.FixupStage.L1 : ConversationIndex.FixupStage.H6);
										}
									}
								}
							}
						}
					}
				}
			}
			if (stage == ConversationIndex.FixupStage.Unknown)
			{
				if (!string.IsNullOrEmpty(item.GetValueOrDefault<string>(ItemSchema.SubjectPrefix)))
				{
					TopicHashCache topicHashCache2 = TopicHashCache.Load(this.xsoFactory, this.session, 50);
					uint hashValue = AllItemsFolderHelper.GetHashValue(text2);
					topicHashCache2.Add(hashValue);
					TopicHashCache.Save(topicHashCache2, this.xsoFactory, this.session);
					if (this.indexTrackingEx != null)
					{
						this.indexTrackingEx.Trace("THA", hashValue.ToString());
					}
				}
				if (flag)
				{
					newIndex = conversationIndex;
					stage = ConversationIndex.FixupStage.H7;
					return;
				}
				newIndex = ConversationIndex.CreateNew();
				stage = ConversationIndex.FixupStage.H8;
			}
		}

		private void CalculateBasedOnMessageWithSameInternetMessageId(ICorePropertyBag message, IStorePropertyBag referenceItem, out ConversationIndex.FixupStage stage, out ConversationIndex newIndex)
		{
			string text = message.TryGetProperty(ItemSchema.NormalizedSubject) as string;
			newIndex = ConversationIndex.Empty;
			stage = ConversationIndex.FixupStage.Unknown;
			ConversationIndex conversationIndex;
			bool flag = ConversationIndex.TryCreate(referenceItem.TryGetProperty(ItemSchema.ConversationIndex) as byte[], out conversationIndex);
			bool valueOrDefault = referenceItem.GetValueOrDefault<bool>(InternalSchema.IsDraft, false);
			if (flag && !valueOrDefault)
			{
				string value = referenceItem.TryGetProperty(ItemSchema.ConversationTopic) as string;
				if (string.IsNullOrEmpty(value) || (!string.IsNullOrEmpty(text) && text.Equals(value, StringComparison.OrdinalIgnoreCase)))
				{
					bool? flag2 = referenceItem.TryGetProperty(ItemSchema.ConversationIndexTracking) as bool?;
					stage = ((flag2 == null || !flag2.Value) ? ConversationIndex.FixupStage.L1 : ConversationIndex.FixupStage.H13);
					newIndex = conversationIndex;
				}
			}
		}

		private bool CalculateBasedOnReferenceMessage(ICorePropertyBag message, IStorePropertyBag referenceItem, out ConversationIndex.FixupStage stage, out ConversationIndex newIndex)
		{
			bool result = true;
			newIndex = ConversationIndex.Empty;
			stage = ConversationIndex.FixupStage.Unknown;
			if (this.indexTrackingEx != null)
			{
				string text = referenceItem.TryGetProperty(ItemSchema.InternetMessageId) as string;
				if (text != null)
				{
					this.indexTrackingEx.Trace("S1", text);
				}
			}
			string text2 = message.TryGetProperty(ItemSchema.NormalizedSubject) as string;
			ConversationIndex conversationIndex;
			bool flag = ConversationIndex.TryCreate(referenceItem.TryGetProperty(ItemSchema.ConversationIndex) as byte[], out conversationIndex);
			ConversationIndex conversationIndex2;
			bool flag2 = ConversationIndex.TryCreate(message.TryGetProperty(ItemSchema.ConversationIndex) as byte[], out conversationIndex2);
			bool? flag3 = referenceItem.TryGetProperty(ItemSchema.ConversationIndexTracking) as bool?;
			bool flag4 = flag3 == null || !flag3.Value;
			if (flag)
			{
				string value = referenceItem.TryGetProperty(ItemSchema.ConversationTopic) as string;
				if (string.IsNullOrEmpty(value) || (!string.IsNullOrEmpty(text2) && text2.EndsWith(value, StringComparison.OrdinalIgnoreCase)))
				{
					string text3 = message.TryGetProperty(ItemSchema.InReplyTo) as string;
					string text4 = referenceItem.TryGetProperty(ItemSchema.InternetMessageId) as string;
					if (!string.IsNullOrEmpty(text3) && !string.IsNullOrEmpty(text4) && string.Compare(text3, text4, StringComparison.OrdinalIgnoreCase) == 0)
					{
						if (conversationIndex.IsParentOf(conversationIndex2))
						{
							newIndex = conversationIndex2;
							stage = (flag4 ? ConversationIndex.FixupStage.L1 : ConversationIndex.FixupStage.H1);
							result = false;
						}
						else
						{
							ExDateTime? valueAsNullable = message.GetValueAsNullable<ExDateTime>(ItemSchema.SentTime);
							if (valueAsNullable != null)
							{
								newIndex = ConversationIndex.CreateFromParent(conversationIndex.Bytes, valueAsNullable.Value);
							}
							else
							{
								newIndex = ConversationIndex.CreateFromParent(conversationIndex.Bytes);
							}
							stage = (flag4 ? ConversationIndex.FixupStage.L1 : ConversationIndex.FixupStage.H2);
						}
					}
					else if (flag2)
					{
						if (conversationIndex.IsAncestorOf(conversationIndex2))
						{
							newIndex = conversationIndex2;
							stage = (flag4 ? ConversationIndex.FixupStage.L1 : ConversationIndex.FixupStage.H1);
							result = false;
						}
						else
						{
							newIndex = conversationIndex2.UpdateGuid(conversationIndex.Guid);
							newIndex = newIndex.UpdateHeader(conversationIndex.Header);
							stage = (flag4 ? ConversationIndex.FixupStage.L1 : ConversationIndex.FixupStage.H10);
						}
					}
					else
					{
						newIndex = ConversationIndex.CreateFromParent(conversationIndex.Bytes);
						stage = (flag4 ? ConversationIndex.FixupStage.L1 : ConversationIndex.FixupStage.H2);
					}
				}
				else
				{
					newIndex = ConversationIndex.CreateNew();
					stage = ConversationIndex.FixupStage.H3;
				}
			}
			return result;
		}

		private static bool FixupMeetingMessage(IXSOFactory xsoFactory, IMailboxSession session, AggregationByItemClassReferencesSubjectProcessor.PropertyDefinitionListConstructorDelegate propertyDefinitionListConstructorDelegate, ICorePropertyBag corePropertyBag, ref ConversationIndex newIndex, ref ConversationIndex.FixupStage stage, out IStorePropertyBag parentItemPropertyBag)
		{
			corePropertyBag.Load(propertyDefinitionListConstructorDelegate(new PropertyDefinition[]
			{
				CalendarItemBaseSchema.GlobalObjectId,
				CalendarItemBaseSchema.CleanGlobalObjectId,
				CalendarItemBaseSchema.OwnerCriticalChangeTime,
				InternalSchema.AppointmentSequenceNumber
			}));
			byte[] array = corePropertyBag.TryGetProperty(CalendarItemBaseSchema.GlobalObjectId) as byte[];
			if (array == null)
			{
				parentItemPropertyBag = null;
				return false;
			}
			GlobalObjectId globalObjectId;
			GlobalObjectId globalObjectId3;
			try
			{
				globalObjectId = new GlobalObjectId(array);
				globalObjectId3 = new GlobalObjectId(array);
			}
			catch (CorruptDataException)
			{
				parentItemPropertyBag = null;
				return false;
			}
			globalObjectId3.Date = ExDateTime.MinValue;
			string valueOrDefault = corePropertyBag.GetValueOrDefault<string>(InternalSchema.ItemClass, string.Empty);
			bool isRequest = ObjectClass.IsMeetingRequest(valueOrDefault);
			object fixupStage = null;
			ExDateTime ownerCriticalChangeTime = corePropertyBag.GetValueOrDefault<ExDateTime>(CalendarItemBaseSchema.OwnerCriticalChangeTime, ExDateTime.MaxValue);
			int sequenceNumber = corePropertyBag.GetValueOrDefault<int>(InternalSchema.AppointmentSequenceNumber, -1);
			if (sequenceNumber == -1)
			{
				parentItemPropertyBag = null;
				return false;
			}
			parentItemPropertyBag = xsoFactory.RunQueryOnAllItemsFolder<IStorePropertyBag>(session, AllItemsFolderHelper.SupportedSortBy.CleanGlobalObjectId, globalObjectId3.Bytes, null, delegate(QueryResult queryResult)
			{
				IStorePropertyBag result = null;
				bool flag2 = false;
				while (!flag2)
				{
					IStorePropertyBag[] propertyBags = queryResult.GetPropertyBags(50);
					flag2 = (propertyBags.Length == 0);
					IStorePropertyBag[] array2 = propertyBags;
					int i = 0;
					while (i < array2.Length)
					{
						IStorePropertyBag storePropertyBag = array2[i];
						VersionedId versionedId = (VersionedId)storePropertyBag.TryGetProperty(ItemSchema.Id);
						byte[] array3 = (storePropertyBag.TryGetProperty(CalendarItemBaseSchema.GlobalObjectId) as byte[]) ?? (storePropertyBag.TryGetProperty(CalendarItemBaseSchema.CleanGlobalObjectId) as byte[]);
						if (array3 != null && GlobalObjectId.CompareCleanGlobalObjectIds(array3, globalObjectId.Bytes))
						{
							GlobalObjectId globalObjectId2 = new GlobalObjectId(array3);
							if (versionedId.ObjectId.ObjectType == StoreObjectType.MeetingRequest)
							{
								ConversationId conversationId = storePropertyBag.TryGetProperty(ConversationItemSchema.ConversationId) as ConversationId;
								if (conversationId != null)
								{
									if (isRequest)
									{
										fixupStage = ConversationIndex.FixupStage.M1;
										return storePropertyBag;
									}
									if (AggregationByItemClassReferencesSubjectProcessor.IsMatchForMeetingResponse(globalObjectId, ownerCriticalChangeTime, sequenceNumber, storePropertyBag))
									{
										if (globalObjectId.Date != ExDateTime.MinValue && globalObjectId.Date == globalObjectId2.Date)
										{
											fixupStage = ConversationIndex.FixupStage.M3;
											return storePropertyBag;
										}
										if (globalObjectId.Date == ExDateTime.MinValue)
										{
											fixupStage = ConversationIndex.FixupStage.M2;
											return storePropertyBag;
										}
										fixupStage = ConversationIndex.FixupStage.M4;
										result = storePropertyBag;
									}
								}
							}
							i++;
							continue;
						}
						return result;
					}
				}
				return result;
			}, propertyDefinitionListConstructorDelegate(new PropertyDefinition[]
			{
				CalendarItemBaseSchema.GlobalObjectId,
				CalendarItemBaseSchema.CleanGlobalObjectId,
				InternalSchema.AppointmentSequenceNumber,
				CalendarItemBaseSchema.OwnerCriticalChangeTime,
				ConversationItemSchema.ConversationId
			}));
			if (parentItemPropertyBag == null)
			{
				return false;
			}
			ConversationIndex conversationIndex;
			if (!ConversationIndex.TryCreate(parentItemPropertyBag.TryGetProperty(ItemSchema.ConversationIndex) as byte[], out conversationIndex))
			{
				return false;
			}
			bool? flag = parentItemPropertyBag.TryGetProperty(ItemSchema.ConversationIndexTracking) as bool?;
			stage = ((flag == null || !flag.Value) ? ConversationIndex.FixupStage.L1 : ((ConversationIndex.FixupStage)fixupStage));
			if (isRequest)
			{
				byte[] valueOrDefault2 = corePropertyBag.GetValueOrDefault<byte[]>(ItemSchema.ConversationIndex);
				ConversationIndex conversationIndex2;
				if (valueOrDefault2 != null && ConversationIndex.TryCreate(valueOrDefault2, out conversationIndex2))
				{
					newIndex = conversationIndex.UpdateHeader(conversationIndex2.Components[0]);
				}
				else
				{
					newIndex = ConversationIndex.Create(conversationIndex.Guid);
				}
			}
			else
			{
				newIndex = ConversationIndex.CreateFromParent(conversationIndex.Bytes);
			}
			return true;
		}

		private static bool IsMatchForMeetingResponse(GlobalObjectId globalObjectId, ExDateTime ownerCriticalTime, int sequenceNumber, IStorePropertyBag potentialSelection)
		{
			object obj = potentialSelection.TryGetProperty(InternalSchema.AppointmentSequenceNumber);
			byte[] array = potentialSelection.TryGetProperty(CalendarItemBaseSchema.GlobalObjectId) as byte[];
			if (!(obj is int) || array == null)
			{
				return false;
			}
			int num = (int)obj;
			if (num != sequenceNumber)
			{
				return false;
			}
			GlobalObjectId globalObjectId2 = new GlobalObjectId(array);
			if (globalObjectId2.Date != ExDateTime.MinValue && globalObjectId2.Date != globalObjectId.Date)
			{
				return false;
			}
			object obj2 = potentialSelection.TryGetProperty(CalendarItemBaseSchema.OwnerCriticalChangeTime);
			return !(globalObjectId2.Date != ExDateTime.MinValue) || (obj2 is ExDateTime && ExDateTime.Compare(ownerCriticalTime, (ExDateTime)obj2, AggregationByItemClassReferencesSubjectProcessor.OwnerCriticalTimeTreshold) == 0);
		}

		private PropertyDefinition[] GetSearchPropertyDefinitions(PropertyDefinition[] additionalProperties = null)
		{
			if (additionalProperties == null)
			{
				return this.BasicSearchPropertyDefinitions;
			}
			List<PropertyDefinition> list = new List<PropertyDefinition>(this.BasicSearchPropertyDefinitions);
			list.AddRange(additionalProperties);
			return list.ToArray();
		}

		private static bool SearchByReferences(IXSOFactory xsoFactory, IMailboxSession session, ICorePropertyBag persistPropertyBag, ConversationIndexTrackingEx indexTrackingEx, bool searchByDupedMessage, out IStorePropertyBag foundPropertyBag, params PropertyDefinition[] propsToReturn)
		{
			foundPropertyBag = null;
			Util.ThrowOnNullArgument(propsToReturn, "propsToReturn");
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(persistPropertyBag, "persistPropertyBag");
			List<string> internetIds = new List<string>();
			if (searchByDupedMessage)
			{
				string valueOrDefault = persistPropertyBag.GetValueOrDefault<string>(ItemSchema.InternetMessageId, string.Empty);
				if (!string.IsNullOrEmpty(valueOrDefault))
				{
					internetIds.Add(valueOrDefault);
				}
			}
			string valueOrDefault2 = persistPropertyBag.GetValueOrDefault<string>(ItemSchema.InReplyTo, string.Empty);
			if (!string.IsNullOrEmpty(valueOrDefault2))
			{
				internetIds.Add(valueOrDefault2);
			}
			string valueOrDefault3 = persistPropertyBag.GetValueOrDefault<string>(ItemSchema.InternetReferences, string.Empty);
			if (!string.IsNullOrEmpty(valueOrDefault3))
			{
				string[] array = valueOrDefault3.Split(AggregationByItemClassReferencesSubjectProcessor.ReferencesSeparators, StringSplitOptions.RemoveEmptyEntries);
				if (array != null && array.Length > 0)
				{
					int num = array.Length - 1;
					for (int i = num; i >= 0; i--)
					{
						if (!string.IsNullOrEmpty(array[i]) && !internetIds.Contains(array[i]))
						{
							internetIds.Add(array[i]);
							if (internetIds.Count >= 50)
							{
								break;
							}
						}
					}
				}
			}
			if (internetIds.Count > 0)
			{
				ICollection<PropertyDefinition> properties = InternalSchema.Combine<PropertyDefinition>(propsToReturn, new PropertyDefinition[]
				{
					ItemSchema.InternetMessageIdHash
				});
				Stopwatch stopwatch = Stopwatch.StartNew();
				IStorePropertyBag storePropertyBag = xsoFactory.RunQueryOnAllItemsFolder<IStorePropertyBag>(session, AllItemsFolderHelper.SupportedSortBy.InternetMessageIdHash, delegate(QueryResult queryResult)
				{
					using (List<string>.Enumerator enumerator = internetIds.GetEnumerator())
					{
						IL_A6:
						while (enumerator.MoveNext())
						{
							string text = enumerator.Current;
							int hashValue = (int)AllItemsFolderHelper.GetHashValue(text);
							if (queryResult.SeekToCondition(SeekReference.OriginBeginning, new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.InternetMessageIdHash, hashValue)))
							{
								IStorePropertyBag[] propertyBags;
								string text2;
								do
								{
									propertyBags = queryResult.GetPropertyBags(1);
									if (propertyBags == null || propertyBags.Length != 1)
									{
										goto IL_A6;
									}
									int? num2 = propertyBags[0].TryGetProperty(ItemSchema.InternetMessageIdHash) as int?;
									if (num2 == null || num2.Value != hashValue)
									{
										goto IL_A6;
									}
									text2 = (propertyBags[0].TryGetProperty(ItemSchema.InternetMessageId) as string);
								}
								while (string.IsNullOrEmpty(text2) || !string.Equals(text, text2, StringComparison.OrdinalIgnoreCase));
								return propertyBags[0];
							}
						}
					}
					return null;
				}, properties);
				stopwatch.Stop();
				if (indexTrackingEx != null)
				{
					indexTrackingEx.Trace("SBMID", stopwatch.ElapsedMilliseconds.ToString());
				}
				if (storePropertyBag != null)
				{
					foundPropertyBag = storePropertyBag;
					return true;
				}
			}
			return false;
		}

		private static bool SearchByTopic(IXSOFactory xsoFactory, IMailboxSession session, ICorePropertyBag persistPropertyBag, ConversationIndexTrackingEx indexTrackingEx, out IStorePropertyBag foundPropertyBag, out bool didConversationIdMatch, out bool didReferencesMatch, params PropertyDefinition[] propsToReturn)
		{
			foundPropertyBag = null;
			didConversationIdMatch = false;
			didReferencesMatch = false;
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(persistPropertyBag, "persistPropertyBag");
			Util.ThrowOnNullArgument(propsToReturn, "propsToReturn");
			ICollection<PropertyDefinition> properties = InternalSchema.Combine<PropertyDefinition>(propsToReturn, new PropertyDefinition[]
			{
				ItemSchema.ConversationTopicHash
			});
			string incomingConversationTopic = persistPropertyBag.GetValueOrDefault<string>(ItemSchema.ConversationTopic);
			ConversationIndex conversationIndex;
			bool isValidIncomingIndex = ConversationIndex.TryCreate(persistPropertyBag.TryGetProperty(ItemSchema.ConversationIndex) as byte[], out conversationIndex);
			ConversationId incomingConversationId = isValidIncomingIndex ? ConversationId.Create(conversationIndex.Guid) : null;
			if (incomingConversationTopic == null)
			{
				return false;
			}
			bool didConversationIdMatchLocal = false;
			bool didReferencesMatchLocal = false;
			int incomingConversationTopicHash = (int)AllItemsFolderHelper.GetHashValue(incomingConversationTopic);
			Stopwatch stopwatch = Stopwatch.StartNew();
			IStorePropertyBag storePropertyBag = xsoFactory.RunQueryOnAllItemsFolder<IStorePropertyBag>(session, AllItemsFolderHelper.SupportedSortBy.ConversationTopicHash, incomingConversationTopicHash, null, delegate(QueryResult queryResult)
			{
				bool flag = queryResult.SeekToCondition(SeekReference.OriginBeginning, new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.ConversationTopicHash, incomingConversationTopicHash));
				if (flag)
				{
					IStorePropertyBag storePropertyBag2 = null;
					for (int i = 0; i < 5; i++)
					{
						IStorePropertyBag[] propertyBags = queryResult.GetPropertyBags(1);
						if (propertyBags.Length != 1)
						{
							break;
						}
						int? num = propertyBags[0].TryGetProperty(ItemSchema.ConversationTopicHash) as int?;
						if (num == null || num.Value != incomingConversationTopicHash)
						{
							break;
						}
						string foundTopic = propertyBags[0].TryGetProperty(ItemSchema.ConversationTopic) as string;
						if (ConversationIndex.CompareTopics(incomingConversationTopic, foundTopic))
						{
							if (storePropertyBag2 == null)
							{
								storePropertyBag2 = propertyBags[0];
							}
							ConversationIndex conversationIndex2;
							bool flag2 = ConversationIndex.TryCreate(propertyBags[0].TryGetProperty(ItemSchema.ConversationIndex) as byte[], out conversationIndex2);
							if (flag2)
							{
								if (isValidIncomingIndex)
								{
									ConversationId conversationId = ConversationId.Create(conversationIndex2.Guid);
									if (conversationId.Equals(incomingConversationId))
									{
										didConversationIdMatchLocal = true;
										return propertyBags[0];
									}
								}
								else if (AggregationByItemClassReferencesSubjectProcessor.MatchMessageIdWithReferences(persistPropertyBag, propertyBags[0]))
								{
									didReferencesMatchLocal = true;
									return propertyBags[0];
								}
							}
						}
						if (!queryResult.SeekToCondition(SeekReference.OriginCurrent, new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.ConversationTopicHash, incomingConversationTopicHash)))
						{
							break;
						}
					}
					if (storePropertyBag2 != null)
					{
						return storePropertyBag2;
					}
				}
				return null;
			}, properties);
			stopwatch.Stop();
			if (indexTrackingEx != null)
			{
				indexTrackingEx.Trace("SBT", stopwatch.ElapsedMilliseconds.ToString());
			}
			if (storePropertyBag != null)
			{
				foundPropertyBag = storePropertyBag;
				didConversationIdMatch = didConversationIdMatchLocal;
				didReferencesMatch = didReferencesMatchLocal;
				return true;
			}
			return false;
		}

		private static bool MatchMessageIdWithReferences(ICorePropertyBag persistPropertyBag, IStorePropertyBag foundPropertyBag)
		{
			string valueOrDefault = persistPropertyBag.GetValueOrDefault<string>(ItemSchema.InternetMessageId, null);
			if (string.IsNullOrEmpty(valueOrDefault))
			{
				return false;
			}
			List<string> list = new List<string>(0);
			string text = foundPropertyBag.TryGetProperty(ItemSchema.InReplyTo) as string;
			if (!string.IsNullOrEmpty(text))
			{
				list.Add(text);
			}
			string text2 = foundPropertyBag.TryGetProperty(ItemSchema.InternetReferences) as string;
			if (!string.IsNullOrEmpty(text2))
			{
				string[] array = text2.Split(AggregationByItemClassReferencesSubjectProcessor.ReferencesSeparators, StringSplitOptions.RemoveEmptyEntries);
				if (array != null)
				{
					list.AddRange(array);
				}
			}
			int num = 0;
			while (num < list.Count && num < 50)
			{
				if (string.Compare(valueOrDefault, list[num], StringComparison.OrdinalIgnoreCase) == 0)
				{
					return true;
				}
				num++;
			}
			return false;
		}

		private const int TopicHashCacheSize = 50;

		private const int SubjectMatchMaxRowCount = 5;

		private const int MaxReferencesToSearch = 50;

		private static readonly PropertyDefinition[] MinimumRequiredPropertyDefinitionsList = new PropertyDefinition[]
		{
			ItemSchema.Id,
			ItemSchema.ConversationIndex,
			ItemSchema.ConversationId,
			ItemSchema.ConversationTopic,
			ItemSchema.InternetMessageId,
			ItemSchema.ConversationIndexTracking
		};

		private static readonly PropertyDefinition[] SideConversationProcessingEnabledRequiredPropertyDefinitionsList = new PropertyDefinition[]
		{
			ItemSchema.ConversationFamilyId,
			StoreObjectSchema.ItemClass,
			ItemSchema.SupportsSideConversation,
			InternalSchema.ReplyAllDisplayNames,
			InternalSchema.IsDraft
		};

		private static readonly TimeSpan OwnerCriticalTimeTreshold = TimeSpan.FromSeconds(60.0);

		private static readonly char[] ReferencesSeparators = new char[]
		{
			' ',
			','
		};

		private readonly ConversationIndexTrackingEx indexTrackingEx;

		private readonly IMailboxSession session;

		private readonly IXSOFactory xsoFactory;

		private readonly PropertyDefinition[] basicSearchPropertyDefinitions;

		private delegate PropertyDefinition[] PropertyDefinitionListConstructorDelegate(PropertyDefinition[] additionalProperties = null);
	}
}
