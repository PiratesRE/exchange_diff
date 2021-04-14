using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConversationActionItem : Item
	{
		internal ConversationActionItem(ICoreItem coreItem) : base(coreItem, false)
		{
		}

		public override Schema Schema
		{
			get
			{
				this.CheckDisposed("Schema::get");
				return ConversationActionItemSchema.Instance;
			}
		}

		public static ConversationActionItem Bind(MailboxSession session, StoreId itemId)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(itemId, "itemId");
			StoreId storeId = itemId;
			if (storeId is ConversationId)
			{
				storeId = ConversationActionItem.QueryConversationActionsFolder(session, (ConversationId)storeId);
				if (storeId == null)
				{
					throw new ObjectNotFoundException(ServerStrings.ExConversationActionItemNotFound);
				}
			}
			ConversationActionItem conversationActionItem = ItemBuilder.ItemBind<ConversationActionItem>(session, storeId, ConversationActionItemSchema.Instance, null);
			if (!conversationActionItem.IsCorrectVersion())
			{
				conversationActionItem.Dispose();
				throw new ObjectNotFoundException(new LocalizedString("Invalid Conversation Action Item Version"));
			}
			return conversationActionItem;
		}

		internal static ConversationActionItem Create(MailboxSession session, ConversationId conversationId, string conversationTopic)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(conversationId, "conversationId");
			Util.ThrowOnNullArgument(conversationTopic, "conversationTopic");
			return ConversationActionItem.InternalCreate(session, conversationId, conversationTopic);
		}

		private static ConversationActionItem InternalCreate(MailboxSession session, ConversationId conversationId, string conversationTopic)
		{
			StoreId conversationActionsFolderId = ConversationActionItem.GetConversationActionsFolderId(session, true);
			if (conversationActionsFolderId == null)
			{
				throw new NotSupportedException("Failed to get conversation actions folder");
			}
			ConversationActionItem conversationActionItem = ItemBuilder.CreateNewItem<ConversationActionItem>(session, conversationActionsFolderId, ItemCreateInfo.ConversationActionInfo, CreateMessageType.Associated);
			conversationActionItem[ItemSchema.ConversationTopic] = conversationTopic;
			conversationActionItem[ItemSchema.Subject] = conversationTopic;
			conversationActionItem[StoreObjectSchema.ItemClass] = "IPM.ConversationAction";
			conversationActionItem[ItemSchema.ConversationId] = conversationId;
			conversationActionItem.SetExpiration();
			conversationActionItem[ConversationActionItemSchema.ConversationActionVersion] = ConversationActionItem.MakeVersion();
			return conversationActionItem;
		}

		private static ConversationIndex GetConversationIndex(ConversationId conversationId)
		{
			ConversationIndex conversationIndex = ConversationIndex.Create(conversationId);
			byte[] header = new byte[5];
			return conversationIndex.UpdateHeader(header);
		}

		private static bool MergeCategories(IList<string> messageCategories, IList<string> categoriesCurrent, IList<string> categoriesPrev, out List<string> mergedCategories)
		{
			mergedCategories = null;
			categoriesCurrent = ((categoriesCurrent == null) ? new List<string>() : categoriesCurrent);
			categoriesPrev = ((categoriesPrev == null) ? new List<string>() : categoriesPrev);
			messageCategories = ((messageCategories == null) ? new List<string>() : messageCategories);
			IEnumerable<string> second = categoriesCurrent.Except(categoriesPrev, Category.NameComparer);
			IEnumerable<string> second2 = categoriesPrev.Except(categoriesCurrent, Category.NameComparer);
			IEnumerable<string> collection = messageCategories.Except(second2).Union(second);
			mergedCategories = new List<string>(collection);
			return ConversationActionItem.NeedToAddCategory(mergedCategories, messageCategories);
		}

		private static bool NeedToAddCategory(IList<string> mergedCategories, IList<string> messageCategories)
		{
			if (mergedCategories == null && messageCategories == null)
			{
				return false;
			}
			if (mergedCategories == null || messageCategories == null)
			{
				return true;
			}
			if (mergedCategories.Count != messageCategories.Count)
			{
				return true;
			}
			for (int i = 0; i < messageCategories.Count; i++)
			{
				if (!mergedCategories.Contains(messageCategories[i], Category.NameComparer))
				{
					return true;
				}
			}
			return false;
		}

		private static int GetMajorVersion(int version)
		{
			return version >> 20 & 4095;
		}

		private static int GetMinorVersion(int version)
		{
			return version & 1048575;
		}

		private static int MakeVersion()
		{
			if (ConversationActionItem.buildVersion == null)
			{
				ServerVersion serverVersion = ConversationIndexTrackingEx.GetBuildVersion();
				ConversationActionItem.buildVersion = new int?(4194304 | (serverVersion.Major * 10000 + serverVersion.Build & 1048575));
			}
			return ConversationActionItem.buildVersion.Value;
		}

		internal string ConversationTopic
		{
			get
			{
				return base.CoreItem.PropertyBag.GetValueOrDefault<string>(ItemSchema.ConversationTopic);
			}
		}

		public ConversationId ConversationId
		{
			get
			{
				this.CheckDisposed("ConversationId::get");
				byte[] valueOrDefault = base.CoreItem.PropertyBag.GetValueOrDefault<byte[]>(ItemSchema.ConversationIndex);
				if (valueOrDefault == null)
				{
					throw new CorruptDataException(ServerStrings.ExCorruptConversationActionItem);
				}
				ConversationId conversationId = ConversationId.Create(ConversationIndex.Create(valueOrDefault).Guid);
				if (conversationId == null)
				{
					throw new CorruptDataException(ServerStrings.ExCorruptConversationActionItem);
				}
				return conversationId;
			}
			set
			{
				this.CheckDisposed("ConversationId::set");
				Util.ThrowOnNullArgument(value, "value");
				this[ItemSchema.ConversationId] = value;
			}
		}

		public int ConversationActionVersionMajor
		{
			get
			{
				this.CheckDisposed("ConversationActionVersionMajor::get");
				int valueOrDefault = base.GetValueOrDefault<int>(ConversationActionItemSchema.ConversationActionVersion, -1);
				if (valueOrDefault == -1)
				{
					return valueOrDefault;
				}
				return ConversationActionItem.GetMajorVersion(valueOrDefault);
			}
		}

		public int ConversationActionVersionMinor
		{
			get
			{
				this.CheckDisposed("ConversationActionVersionMinor::get");
				int valueOrDefault = base.GetValueOrDefault<int>(ConversationActionItemSchema.ConversationActionVersion, -1);
				if (valueOrDefault == -1)
				{
					return valueOrDefault;
				}
				return ConversationActionItem.GetMinorVersion(valueOrDefault);
			}
		}

		public ExDateTime ConversationActionMaxDeliveryTime
		{
			get
			{
				this.CheckDisposed("ConversationActionMaxDeliveryTime::get");
				return base.GetValueOrDefault<ExDateTime>(ConversationActionItemSchema.ConversationActionMaxDeliveryTime, ExDateTime.MinValue);
			}
			set
			{
				this.CheckDisposed("ConversationActionMaxDeliveryTime::set");
				Util.ThrowOnMismatchType<ExDateTime>(ConversationActionItemSchema.ConversationActionMaxDeliveryTime, "value");
				this[ConversationActionItemSchema.ConversationActionMaxDeliveryTime] = value;
			}
		}

		internal byte[] ConversationActionLastMoveFolderId
		{
			get
			{
				this.CheckDisposed("ConversationActionLastMoveFolderId::get");
				return base.CoreItem.PropertyBag.GetValueOrDefault<byte[]>(ConversationActionItemSchema.ConversationActionLastMoveFolderId);
			}
			set
			{
				this.CheckDisposed("ConversationActionLastMoveFolderId::set");
				if (value == null)
				{
					base.Delete(ConversationActionItemSchema.ConversationActionLastMoveFolderId);
					return;
				}
				this[ConversationActionItemSchema.ConversationActionLastMoveFolderId] = value;
			}
		}

		internal string[] ConversationActionLastCategorySet
		{
			get
			{
				this.CheckDisposed("ConversationActionLastCategorySet::get");
				return base.CoreItem.PropertyBag.GetValueOrDefault<string[]>(ConversationActionItemSchema.ConversationActionLastCategorySet);
			}
			set
			{
				this.CheckDisposed("ConversationActionLastCategorySet::set");
				if (value == null || value.Length == 0)
				{
					base.Delete(ConversationActionItemSchema.ConversationActionLastCategorySet);
					return;
				}
				this[ConversationActionItemSchema.ConversationActionLastCategorySet] = value;
			}
		}

		public string[] AlwaysCategorizeValue
		{
			get
			{
				this.CheckDisposed("AlwaysCategorizeValue::get");
				string[] valueOrDefault = base.CoreItem.PropertyBag.GetValueOrDefault<string[]>(ItemSchema.Categories);
				if (valueOrDefault == null || valueOrDefault.Length <= 0)
				{
					return null;
				}
				return valueOrDefault;
			}
			set
			{
				this.CheckDisposed("AlwaysCategorizeValue::set");
				if (value == null)
				{
					base.CoreItem.PropertyBag.Delete(ItemSchema.Categories);
					return;
				}
				this[ItemSchema.Categories] = value;
			}
		}

		public StoreObjectId AlwaysMoveValue
		{
			get
			{
				this.CheckDisposed("AlwaysMoveValue::get");
				StoreObjectId storeObjectId = null;
				bool storedFolderId = this.GetStoredFolderId(out storeObjectId);
				if (storedFolderId && storeObjectId != null)
				{
					return storeObjectId;
				}
				return null;
			}
			set
			{
				this.CheckDisposed("AlwaysMoveValue::set");
				base.CoreItem.PropertyBag.Delete(ConversationActionItemSchema.ConversationActionMoveStoreId);
				if (value == null)
				{
					base.CoreItem.PropertyBag.Delete(ConversationActionItemSchema.ConversationActionMoveFolderId);
					return;
				}
				this.ValidateTargetFolderId(value);
				this[ConversationActionItemSchema.ConversationActionMoveFolderId] = value.ProviderLevelItemId;
			}
		}

		public bool AlwaysDeleteValue
		{
			get
			{
				this.CheckDisposed("AlwaysDeleteValue::get");
				StoreObjectId storeObjectId = null;
				bool storedFolderId = this.GetStoredFolderId(out storeObjectId);
				return storedFolderId && storeObjectId == null;
			}
			set
			{
				this.CheckDisposed("AlwaysDeleteValue::set");
				base.CoreItem.PropertyBag.Delete(ConversationActionItemSchema.ConversationActionMoveStoreId);
				if (value)
				{
					this[ConversationActionItemSchema.ConversationActionMoveFolderId] = Array<byte>.Empty;
					return;
				}
				base.CoreItem.PropertyBag.Delete(ConversationActionItemSchema.ConversationActionMoveFolderId);
			}
		}

		public bool? AlwaysClutterOrUnclutterValue
		{
			get
			{
				this.CheckDisposed("AlwaysClutterOrUnclutterValue::get");
				return base.CoreItem.PropertyBag.GetValueAsNullable<bool>(ItemSchema.IsClutter);
			}
			set
			{
				this.CheckDisposed("AlwaysClutterOrUnclutterValue::set");
				if (value != null)
				{
					this[ItemSchema.IsClutter] = value;
					return;
				}
				base.CoreItem.PropertyBag.Delete(ItemSchema.IsClutter);
			}
		}

		internal ConversationActionItem.ProcessDebugInfo ProcessStats
		{
			get
			{
				return this.processStats;
			}
		}

		protected override void OnBeforeSave()
		{
			base.OnBeforeSave();
			this[ConversationActionItemSchema.ConversationActionVersion] = ConversationActionItem.MakeVersion();
			this.SetExpiration();
		}

		public AggregateOperationResult ProcessItems(ConversationAction actionsFilter)
		{
			this.CheckDisposed("ProcessItems");
			Conversation conversation = Conversation.Load((MailboxSession)base.Session, this.ConversationId, ConversationActionItem.PropertiesToRequestInConversationLoad);
			return this.ProcessItems(actionsFilter, conversation);
		}

		internal AggregateOperationResult ProcessItems(ConversationAction actionsFilter, Conversation conversation)
		{
			Util.ThrowOnNullArgument(conversation, "conversation");
			EnumValidator.ThrowIfInvalid<ConversationAction>(actionsFilter, "actionsFilter");
			if (!conversation.ConversationId.Equals(this.ConversationId))
			{
				throw new ArgumentException("Conversation ID of passed conversation doesn't match with id of the action item");
			}
			List<GroupOperationResult> list = new List<GroupOperationResult>();
			if (!this.IsCorrectVersion())
			{
				return Folder.CreateAggregateOperationResult(list);
			}
			ConversationAction conversationAction = this.EnabledActions & actionsFilter;
			if ((conversationAction & ConversationAction.AlwaysMoveOrDelete) != ConversationAction.None)
			{
				LocalizedException ex = null;
				try
				{
					this.ValidateTargetFolderId(this.TargetFolderId);
				}
				catch (ObjectNotFoundException ex2)
				{
					ex = ex2;
				}
				catch (InvalidFolderTypeException ex3)
				{
					ex = ex3;
				}
				if (ex != null)
				{
					list.Add(new GroupOperationResult(OperationResult.Failed, new StoreObjectId[0], ex));
					conversationAction &= ~(ConversationAction.AlwaysMove | ConversationAction.AlwaysDelete);
				}
			}
			if ((conversationAction & ConversationAction.AlwaysClutterOrUnclutter) != ConversationAction.None)
			{
				MailboxSession mailboxSession = base.Session as MailboxSession;
				if (mailboxSession != null)
				{
					IConversationClutterProcessor conversationClutterProcessor = ConversationClutterProcessorFactory.Create(mailboxSession);
					if (conversationClutterProcessor != null)
					{
						this.processStats.UpdateCount = this.processStats.UpdateCount + (long)conversationClutterProcessor.Process(this.AlwaysClutterOrUnclutterValue.Value, conversation.ConversationTree, list);
					}
				}
			}
			if ((conversationAction & ConversationAction.AlwaysCategorize) != ConversationAction.None)
			{
				this.processStats.UpdateCount = this.processStats.UpdateCount + (long)this.UpdateItems(conversation.ConversationTree, list);
			}
			else if (this.ConversationActionLastCategorySet != null)
			{
				this.processStats.UpdateCount = this.processStats.UpdateCount + (long)this.UpdateItems(conversation.ConversationTree, list);
			}
			if ((conversationAction & ConversationAction.AlwaysMoveOrDelete) != ConversationAction.None)
			{
				this.processStats.MoveCount = (long)this.MoveItems(conversation.ConversationTree, list, false);
			}
			else if (this.ConversationActionLastMoveFolderId != null)
			{
				if (this.ConversationActionLastMoveFolderId.Length == 0)
				{
					this.processStats.UpdateCount = this.processStats.UpdateCount + (long)this.MoveItems(conversation.ConversationTree, list, true);
				}
				else
				{
					this.ConversationActionLastMoveFolderId = null;
				}
			}
			return Folder.CreateAggregateOperationResult(list);
		}

		public StoreId TargetFolderId
		{
			get
			{
				this.CheckDisposed("TargetFolderId::get");
				if (this.AlwaysDeleteValue)
				{
					return ((MailboxSession)base.Session).GetDefaultFolderId(DefaultFolderType.DeletedItems);
				}
				return this.AlwaysMoveValue;
			}
		}

		public bool IsCorrectVersion()
		{
			this.CheckDisposed("IsCorrectVersion");
			return this.ConversationActionVersionMajor >= 4;
		}

		public List<string> GetCategoriesForItem(IList<string> existingCategories)
		{
			this.CheckDisposed("GetCategoriesForItem");
			if (!this.IsCorrectVersion())
			{
				return existingCategories as List<string>;
			}
			if (existingCategories == null)
			{
				if (this.AlwaysCategorizeValue == null)
				{
					return null;
				}
				return new List<string>(this.AlwaysCategorizeValue);
			}
			else
			{
				if (this.AlwaysCategorizeValue != null)
				{
					return new List<string>(existingCategories.Union(this.AlwaysCategorizeValue));
				}
				return existingCategories as List<string>;
			}
		}

		public static StoreId QueryConversationActionsFolder(MailboxSession session, ConversationId conversationId)
		{
			IList<StoreId> list = ConversationActionItem.QueryConversationActionsFolder(session, conversationId, 1);
			if (list == null)
			{
				return null;
			}
			return list[0];
		}

		public static IList<StoreId> QueryConversationActionsFolder(MailboxSession session, ConversationId conversationId, int maxRowCount)
		{
			int num;
			return ConversationActionItem.QueryConversationActionsFolder(session, conversationId, maxRowCount, out num);
		}

		public static IList<StoreId> QueryConversationActionsFolder(MailboxSession session, ConversationId conversationId, int maxRowCount, out int totalActionItemCount)
		{
			totalActionItemCount = 0;
			StoreId conversationActionsFolderId = ConversationActionItem.GetConversationActionsFolderId(session, false);
			if (conversationActionsFolderId == null)
			{
				return null;
			}
			IList<StoreId> result;
			try
			{
				using (Folder folder = Folder.Bind(session, conversationActionsFolderId))
				{
					int? valueAsNullable = folder.GetValueAsNullable<int>(FolderSchema.AssociatedItemCount);
					if (valueAsNullable != null && valueAsNullable <= 0)
					{
						result = null;
					}
					else
					{
						totalActionItemCount = ((valueAsNullable != null) ? valueAsNullable.Value : 0);
						if (conversationId == null)
						{
							result = null;
						}
						else
						{
							SortBy[] sortColumns = new SortBy[]
							{
								new SortBy(ItemSchema.ConversationIndex, SortOrder.Ascending),
								new SortBy(StoreObjectSchema.LastModifiedTime, SortOrder.Descending)
							};
							ConversationIndex conversationIndex = ConversationActionItem.GetConversationIndex(conversationId);
							using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.Associated, null, sortColumns, new PropertyDefinition[]
							{
								ItemSchema.ConversationIndex,
								ItemSchema.Id,
								ConversationActionItemSchema.ConversationActionVersion
							}))
							{
								if (queryResult.SeekToCondition(SeekReference.OriginBeginning, new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.ConversationIndex, conversationIndex.ToByteArray())))
								{
									IStorePropertyBag[] propertyBags = queryResult.GetPropertyBags(maxRowCount);
									if (propertyBags.Length >= 1)
									{
										List<StoreId> list = new List<StoreId>();
										for (int i = 0; i < propertyBags.Length; i++)
										{
											byte[] array = propertyBags[i][ItemSchema.ConversationIndex] as byte[];
											if (array != null)
											{
												ConversationIndex conversationIndex2;
												bool flag = ConversationIndex.TryCreate(array, out conversationIndex2);
												if (flag)
												{
													if (!conversationIndex2.Equals(conversationIndex))
													{
														break;
													}
													if (!(propertyBags[i].TryGetProperty(ConversationActionItemSchema.ConversationActionVersion) is PropertyError))
													{
														int version = (int)propertyBags[i].TryGetProperty(ConversationActionItemSchema.ConversationActionVersion);
														if (ConversationActionItem.GetMajorVersion(version) >= 4)
														{
															list.Add(propertyBags[i][ItemSchema.Id] as VersionedId);
														}
													}
												}
											}
										}
										return (list.Count == 0) ? null : list;
									}
								}
								result = null;
							}
						}
					}
				}
			}
			catch (ObjectNotFoundException)
			{
				result = null;
			}
			return result;
		}

		public static StoreObjectId GetConversationActionsFolderId(MailboxSession session, bool createIfMissing)
		{
			Util.ThrowOnNullArgument(session, "session");
			StoreObjectId storeObjectId = session.GetDefaultFolderId(DefaultFolderType.ConversationActions);
			if (storeObjectId == null && createIfMissing)
			{
				storeObjectId = session.CreateDefaultFolder(DefaultFolderType.ConversationActions);
			}
			return storeObjectId;
		}

		private ConversationAction EnabledActions
		{
			get
			{
				ConversationAction conversationAction = ConversationAction.None;
				conversationAction |= (this.AlwaysDeleteValue ? ConversationAction.AlwaysDelete : ConversationAction.None);
				conversationAction |= ((this.AlwaysMoveValue != null) ? ConversationAction.AlwaysMove : ConversationAction.None);
				conversationAction |= ((this.AlwaysCategorizeValue != null) ? ConversationAction.AlwaysCategorize : ConversationAction.None);
				return conversationAction | ((this.AlwaysClutterOrUnclutterValue != null) ? ConversationAction.AlwaysClutterOrUnclutter : ConversationAction.None);
			}
		}

		private void SetExpiration()
		{
			PolicyTagHelper.SetRetentionProperties(this, ExDateTime.Now.AddDays(30.0), 30);
		}

		private int MoveItems(IConversationTree conversationTree, List<GroupOperationResult> results, bool isUndo)
		{
			int num = 0;
			List<StoreId> list = new List<StoreId>();
			StoreId defaultFolderId = ((MailboxSession)base.Session).GetDefaultFolderId(DefaultFolderType.Inbox);
			StoreId defaultFolderId2 = ((MailboxSession)base.Session).GetDefaultFolderId(DefaultFolderType.SentItems);
			StoreId defaultFolderId3 = ((MailboxSession)base.Session).GetDefaultFolderId(DefaultFolderType.DeletedItems);
			foreach (IConversationTreeNode conversationTreeNode in conversationTree)
			{
				for (int i = 0; i < conversationTreeNode.StorePropertyBags.Count; i++)
				{
					if (!this.CanSkipProcessing(conversationTreeNode.StorePropertyBags[i]))
					{
						StoreId item = conversationTreeNode.StorePropertyBags[i].TryGetProperty(ItemSchema.Id) as StoreId;
						StoreId storeId = conversationTreeNode.StorePropertyBags[i].TryGetProperty(StoreObjectSchema.ParentItemId) as StoreId;
						if (isUndo)
						{
							if (!storeId.Equals(defaultFolderId2) && !storeId.Equals(defaultFolderId))
							{
								num++;
								list.Add(item);
							}
						}
						else if (!storeId.Equals(defaultFolderId2) && !storeId.Equals(this.TargetFolderId) && (this.ConversationActionLastMoveFolderId == null || (this.ConversationActionLastMoveFolderId.Length == 0 && !this.TargetFolderId.Equals(defaultFolderId3)) || ArrayComparer<byte>.Comparer.Equals(storeId.GetBytes(), this.ConversationActionLastMoveFolderId)))
						{
							num++;
							list.Add(item);
						}
					}
				}
			}
			if (list.Count > 0)
			{
				AggregateOperationResult aggregateOperationResult;
				if (isUndo)
				{
					aggregateOperationResult = base.Session.Move(defaultFolderId, list.ToArray());
				}
				else
				{
					aggregateOperationResult = base.Session.Move(this.TargetFolderId, list.ToArray());
				}
				if (aggregateOperationResult != null && aggregateOperationResult.GroupOperationResults != null && aggregateOperationResult.GroupOperationResults.Length > 0)
				{
					foreach (GroupOperationResult item2 in aggregateOperationResult.GroupOperationResults)
					{
						results.Add(item2);
					}
				}
			}
			if (isUndo)
			{
				base.Delete(ConversationActionItemSchema.ConversationActionLastMoveFolderId);
			}
			else if (this.AlwaysDeleteValue)
			{
				if (this.ConversationActionLastMoveFolderId == null || this.ConversationActionLastMoveFolderId.Length != 0)
				{
					this.ConversationActionLastMoveFolderId = Array<byte>.Empty;
				}
			}
			else if (!ArrayComparer<byte>.Comparer.Equals(this.ConversationActionLastMoveFolderId, this.TargetFolderId.GetBytes()))
			{
				this.ConversationActionLastMoveFolderId = this.TargetFolderId.GetBytes();
			}
			return num;
		}

		private int UpdateItems(IConversationTree conversationTree, List<GroupOperationResult> results)
		{
			int num = 0;
			foreach (IConversationTreeNode conversationTreeNode in conversationTree)
			{
				for (int i = 0; i < conversationTreeNode.StorePropertyBags.Count; i++)
				{
					if (!this.CanSkipProcessing(conversationTreeNode.StorePropertyBags[i]))
					{
						StoreId storeId = conversationTreeNode.StorePropertyBags[i].TryGetProperty(ItemSchema.Id) as StoreId;
						List<string> list = null;
						if (ConversationActionItem.MergeCategories(conversationTreeNode.StorePropertyBags[i].TryGetProperty(ItemSchema.Categories) as string[], this.AlwaysCategorizeValue, this.ConversationActionLastCategorySet, out list))
						{
							try
							{
								using (Item item = Microsoft.Exchange.Data.Storage.Item.Bind(base.Session, storeId, null))
								{
									num++;
									item.OpenAsReadWrite();
									if (list.Count == 0)
									{
										item.Delete(ItemSchema.Categories);
									}
									else
									{
										item[ItemSchema.Categories] = list.ToArray();
									}
									item.Save(SaveMode.ResolveConflicts);
									results.Add(new GroupOperationResult(OperationResult.Succeeded, new StoreObjectId[]
									{
										StoreId.GetStoreObjectId(storeId)
									}, null));
								}
							}
							catch (LocalizedException storageException)
							{
								results.Add(new GroupOperationResult(OperationResult.Failed, new StoreObjectId[]
								{
									StoreId.GetStoreObjectId(storeId)
								}, storageException));
							}
						}
					}
				}
			}
			if (ConversationActionItem.NeedToAddCategory(this.ConversationActionLastCategorySet, this.AlwaysCategorizeValue))
			{
				this.ConversationActionLastCategorySet = this.AlwaysCategorizeValue;
			}
			return num;
		}

		private bool CanSkipProcessing(IStorePropertyBag propertyBag)
		{
			ExDateTime exDateTime = ExDateTime.MinValue;
			if (!(propertyBag.TryGetProperty(ItemSchema.ReceivedTime) is PropertyError))
			{
				exDateTime = (ExDateTime)propertyBag.TryGetProperty(ItemSchema.ReceivedTime);
			}
			return (!(exDateTime == ExDateTime.MinValue) || !(this.ConversationActionMaxDeliveryTime == ExDateTime.MinValue)) && exDateTime <= this.ConversationActionMaxDeliveryTime;
		}

		private bool GetStoredFolderId(out StoreObjectId folderId)
		{
			folderId = null;
			byte[] valueOrDefault = base.CoreItem.PropertyBag.GetValueOrDefault<byte[]>(ConversationActionItemSchema.ConversationActionMoveStoreId);
			if (valueOrDefault != null && valueOrDefault.Length > 0)
			{
				return false;
			}
			object obj = base.CoreItem.PropertyBag.TryGetProperty(ConversationActionItemSchema.ConversationActionMoveFolderId);
			if (PropertyError.IsPropertyNotFound(obj))
			{
				return false;
			}
			byte[] array = (byte[])obj;
			if (array != null && array.Length != 0)
			{
				try
				{
					folderId = StoreObjectId.FromProviderSpecificId(array, StoreObjectType.Unknown);
				}
				catch (CorruptDataException)
				{
					return false;
				}
				return true;
			}
			folderId = null;
			return true;
		}

		private void ValidateTargetFolderId(StoreId folderId)
		{
			using (Folder folder = Folder.Bind(base.Session, folderId))
			{
				if (!ObjectClass.IsMessageFolder(folder.ClassName))
				{
					throw new InvalidFolderTypeException(ServerStrings.ExConversationActionInvalidFolderType);
				}
			}
		}

		public const int MajorVersion = 4;

		private const int NumberOfDaysToRetain = 30;

		private const string CatVersionInvalid = "Invalid Conversation Action Item Version";

		private static int? buildVersion = null;

		private static readonly PropertyDefinition[] PropertiesToRequestInConversationLoad = new PropertyDefinition[]
		{
			ItemSchema.IsClutter
		};

		private ConversationActionItem.ProcessDebugInfo processStats = default(ConversationActionItem.ProcessDebugInfo);

		internal struct ProcessDebugInfo
		{
			public void Reset()
			{
				this.MoveCount = (this.UpdateCount = 0L);
			}

			public long MoveCount;

			public long UpdateCount;
		}
	}
}
