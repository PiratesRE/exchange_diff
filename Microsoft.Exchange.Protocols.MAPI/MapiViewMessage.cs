using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.Mapi;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods.Linq;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public class MapiViewMessage : MapiViewTableBase
	{
		public MapiViewMessage() : base(MapiObjectType.MessageView)
		{
		}

		internal ExchangeId FolderId
		{
			get
			{
				return this.folderId;
			}
		}

		internal ExchangeId RealFolderId
		{
			get
			{
				if (!this.materializedRestrictionFolderId.IsValid)
				{
					return this.folderId;
				}
				return this.materializedRestrictionFolderId;
			}
		}

		public override bool IsCategorizedViewSupported
		{
			get
			{
				return base.StoreViewTable is MessageViewTable;
			}
		}

		public override bool IsMultiValueExplosionSupported
		{
			get
			{
				return base.StoreViewTable is MessageViewTable || base.StoreViewTable is ConversationViewTable;
			}
		}

		public override bool SupportSortOperation
		{
			get
			{
				return !this.mailboxScope;
			}
		}

		protected Folder Folder
		{
			get
			{
				MessageViewTable messageViewTable = base.StoreViewTable as MessageViewTable;
				if (messageViewTable != null)
				{
					return messageViewTable.Folder;
				}
				ConversationViewTable conversationViewTable = base.StoreViewTable as ConversationViewTable;
				if (conversationViewTable != null)
				{
					return conversationViewTable.Folder;
				}
				return null;
			}
		}

		protected SearchFolder SearchFolder
		{
			get
			{
				MessageViewTable messageViewTable = base.StoreViewTable as MessageViewTable;
				if (messageViewTable != null)
				{
					return messageViewTable.SearchFolder;
				}
				ConversationViewTable conversationViewTable = base.StoreViewTable as ConversationViewTable;
				if (conversationViewTable != null)
				{
					return conversationViewTable.SearchFolder;
				}
				return null;
			}
		}

		protected bool IsSearchFolder
		{
			get
			{
				return this.SearchFolder != null;
			}
		}

		public override bool IsOptimizedInstantSearch
		{
			get
			{
				ConversationViewTable conversationViewTable = base.StoreViewTable as ConversationViewTable;
				return conversationViewTable != null && conversationViewTable.IsOptimizedInstantSearch;
			}
		}

		public static IDisposable SetMaterializationTestHook(Action action)
		{
			return MapiViewMessage.materializationTestHook.SetTestHook(action);
		}

		public void Configure(MapiContext context, MapiLogon mapiLogon, MapiFolder mapiFolder, ViewMessageConfigureFlags flags)
		{
			this.Configure(context, mapiLogon, mapiFolder, flags, uint.MaxValue);
		}

		public void Configure(MapiContext context, MapiLogon mapiLogon, MapiFolder mapiFolder, ViewMessageConfigureFlags flags, uint hsot)
		{
			if (base.IsDisposed)
			{
				ExTraceGlobals.GeneralTracer.TraceError(0L, "Configure called on a Dispose'd MapiViewMessage!  Throwing ExExceptionInvalidObject!");
				throw new ExExceptionInvalidObject((LID)41976U, "Configure cannot be invoked after Dispose.");
			}
			if (base.IsValid)
			{
				ExTraceGlobals.GeneralTracer.TraceError(0L, "Configure called on already configured MapiViewMessage!  Throwing ExExceptionInvalidObject!");
				throw new ExExceptionInvalidObject((LID)58360U, "Object has already been Configured");
			}
			if (!context.HasMailboxFullRights && mapiLogon.MapiMailbox.SharedState.MailboxTypeDetail != MailboxInfo.MailboxTypeDetail.GroupMailbox && (flags & (ViewMessageConfigureFlags.Conversation | ViewMessageConfigureFlags.ConversationMembers)) != ViewMessageConfigureFlags.None)
			{
				throw new ExExceptionAccessDenied((LID)42592U, "Conversation is disallowed on non owner logon.");
			}
			mapiFolder.CheckRights(context, FolderSecurity.ExchangeSecurityDescriptorFolderRights.ReadBody | FolderSecurity.ExchangeSecurityDescriptorFolderRights.ViewItem, AccessCheckOperation.FolderViewMessage, (LID)58215U);
			mapiFolder.CheckMessageRights(context, FolderSecurity.ExchangeSecurityDescriptorFolderRights.ViewItem, null, AccessCheckOperation.FolderViewMessage, (LID)33639U);
			mapiFolder.GhostedFolderCheck(context, (LID)32696U);
			this.parentFolderAccessCheckState = mapiFolder.AccessCheckState;
			this.configureFlags = flags;
			this.folderId = mapiFolder.Fid;
			if (mapiFolder.IsInstantSearch)
			{
				base.CorrelationId = new Guid?(CorrelationIdHelper.GetCorrelationId(context.Diagnostics.MailboxNumber, mapiFolder.Fid.ToLong()));
			}
			this.ConfigureMapiViewMessage(context, mapiLogon, hsot);
		}

		public override int GetRowCount(MapiContext context)
		{
			if (this.mailboxScope)
			{
				return 0;
			}
			return base.GetRowCount(context);
		}

		public override bool FindRow(MapiContext context, ViewSeekOrigin origin, byte[] bookmark, bool backwards, Restriction restriction, out bool bookmarkPositionChanged, out Properties row)
		{
			if (this.mailboxScope)
			{
				if ((origin != ViewSeekOrigin.Beginning && origin != ViewSeekOrigin.Current) || backwards)
				{
					throw new StoreException((LID)57040U, ErrorCodeValue.TooComplex);
				}
				this.ValidateMailboxScopeRestriction(context, restriction, true);
			}
			return base.FindRow(context, origin, bookmark, backwards, restriction, out bookmarkPositionChanged, out row);
		}

		public override void Restrict(MapiContext context, int flags, Restriction restriction)
		{
			if (restriction is RestrictionTrue)
			{
				restriction = null;
			}
			this.restriction = restriction;
			this.restrictFlags = flags;
			bool flag;
			if (restriction != null)
			{
				flag = restriction.HasClauseMeetingPredicate((Restriction clause) => MapiViewMessage.PropsThatMustBeServicedByFullTextIndex.Any(new Func<StorePropTag, bool>(clause.RefersToProperty)));
			}
			else
			{
				flag = false;
			}
			this.restrictionMustbeHandledByFullTextIndex = flag;
			this.useMaterializedRestriction = this.ShouldUseMaterializedRestriction(context, restriction);
			if (ExTraceGlobals.RestrictTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.RestrictTracer.TraceDebug(0L, "Restrict(): folder=={0}, flags=={1}, restrictionMustbeHandledByFullTextIndex=={2}, mailboxScope=={3}, shouldMaterialize={4}", new object[]
				{
					this.FolderId,
					this.restrictFlags,
					this.restrictionMustbeHandledByFullTextIndex,
					this.mailboxScope,
					this.useMaterializedRestriction
				});
				ExTraceGlobals.RestrictTracer.TraceDebug<bool, uint>(0L, "    Sorted: {0} (categoryCount=={1})", this.sortOrder != null && this.sortOrder.Length != 0, this.categoryCount);
				ExTraceGlobals.RestrictTracer.TraceDebug<string>(0L, "    Restriction: {0}", (this.restriction != null) ? this.restriction.ToString() : "<none>");
			}
			if (this.mailboxScope)
			{
				this.ValidateMailboxScopeRestriction(context, restriction, false);
			}
			if (this.useMaterializedRestriction)
			{
				this.MaterializeRestriction(context);
				this.ConfigureMapiViewMessage(context, base.Logon, base.Hsot);
				if (this.sortOrder != null)
				{
					this.Sort(context, this.sortOrder, this.sortFlags, this.categoryCount, this.expandedCount);
					return;
				}
			}
			else
			{
				if (this.materializedRestrictionFolderId.IsValid)
				{
					this.CleanupMaterializedRestrictionSearchFolder();
					this.ConfigureMapiViewMessage(context, base.Logon, base.Hsot);
					if (this.sortOrder != null)
					{
						this.Sort(context, this.sortOrder, this.sortFlags, this.categoryCount, this.expandedCount);
					}
				}
				base.Restrict(context, flags, restriction);
			}
		}

		internal static bool RestrictionQualifiedForInstantSearch(Restriction restriction)
		{
			return restriction.HasClauseMeetingPredicate((Restriction clause) => clause is RestrictionCount);
		}

		internal override void Sort(MapiContext context, SortOrder[] legacySortOrder, SortTableFlags flags, uint categoryCount, uint expandedCount)
		{
			this.sortOrder = legacySortOrder;
			this.sortFlags = flags;
			this.categoryCount = categoryCount;
			this.expandedCount = expandedCount;
			bool flag = this.isCategorizedView;
			this.isCategorizedView = (categoryCount != 0U || expandedCount != 0U);
			if (this.isCategorizedView && !this.useMaterializedRestriction && this.restriction != null && this.CanMaterializedRestrictionBeUsedForView())
			{
				this.useMaterializedRestriction = true;
				this.MaterializeRestriction(context);
				this.ConfigureMapiViewMessage(context, base.Logon, base.Hsot);
			}
			else if (flag != this.isCategorizedView)
			{
				this.ConfigureMapiViewMessage(context, base.Logon, base.Hsot);
			}
			base.Sort(context, legacySortOrder, flags, categoryCount, expandedCount);
		}

		protected override bool CanSortOnProperty(StorePropTag propTag)
		{
			ushort propId = propTag.PropId;
			return (propId != 3591 && propId != 3689) || !base.Logon.StoreMailbox.SharedState.SupportsPerUserFeatures;
		}

		protected override IList<StorePropTag> AdjustColumnsToQuery(IList<StorePropTag> columns)
		{
			if (columns.All((StorePropTag property) => !MapiMessage.IsMessageSecurityRelatedProperty(property.PropTag) && property.PropTag != 1761673247U && property.PropTag != 1762131999U && property.PropTag != 1728512072U && property.PropTag != 1735131208U))
			{
				return columns;
			}
			List<StorePropTag> list = new List<StorePropTag>(columns.Count);
			foreach (StorePropTag item in columns)
			{
				if (MapiMessage.IsMessageSecurityRelatedProperty(item.PropTag))
				{
					list.Add(PropTag.Message.CreatorSID);
					list.Add(PropTag.Message.ConversationCreatorSID);
				}
				else if (item.PropTag == 1761673247U)
				{
					list.Add(PropTag.Message.ConversationLastMemberDocumentId);
				}
				else if (item.PropTag == 1762131999U)
				{
					list.Add(PropTag.Message.ConversationLastMemberDocumentIdMailboxWide);
				}
				else if (item.PropTag == 1728512072U || item.PropTag == 1735131208U)
				{
					if (base.Logon.UnifiedLogon)
					{
						list.Add(PropTag.Message.MailboxNum);
					}
				}
				else
				{
					list.Add(item);
				}
			}
			return list;
		}

		protected override object GetPropertyValue(MapiContext context, Reader reader, StorePropTag propertyTag, Column column)
		{
			if (propertyTag.IsCategory(8))
			{
				ClientType clientType = context.ClientType;
				if (clientType != ClientType.OWA)
				{
					switch (clientType)
					{
					case ClientType.Migration:
					case ClientType.TransportSync:
						break;
					default:
						if (clientType != ClientType.EDiscoverySearch)
						{
							DiagnosticContext.TraceLocation((LID)48864U);
							return null;
						}
						break;
					}
				}
			}
			if (MapiMessage.IsMessageSecurityRelatedProperty(propertyTag.PropTag))
			{
				byte[] binary = reader.GetBinary(PropertySchema.MapToColumn(context.Database, Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message, PropTag.Message.CreatorSID));
				byte[] binary2 = reader.GetBinary(PropertySchema.MapToColumn(context.Database, Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message, PropTag.Message.ConversationCreatorSID));
				return MapiMessage.CalculateSecurityRelatedProperty(context, propertyTag, this.parentFolderAccessCheckState, (binary != null) ? SecurityHelper.CreateSecurityIdentifier(context, binary) : null, (binary2 != null) ? SecurityHelper.CreateSecurityIdentifier(context, binary2) : null);
			}
			uint propTag = propertyTag.PropTag;
			object obj;
			if (propTag <= 1728512072U)
			{
				if (propTag != 236191747U)
				{
					if (propTag != 267714563U)
					{
						if (propTag == 1728512072U)
						{
							if (!base.Logon.UnifiedLogon)
							{
								return base.Logon.MailboxGuid;
							}
							int mailboxNumber = (int)base.GetPropertyValue(context, reader, PropTag.Message.MailboxNum, null);
							MailboxState mailboxState = MailboxStateCache.Get(context, mailboxNumber);
							return mailboxState.MailboxGuid;
						}
					}
					else if (!this.isCategorizedView)
					{
						return 1;
					}
				}
				else
				{
					obj = base.GetPropertyValue(context, reader, PropTag.Message.SubmitFlags, null);
					if (obj == null)
					{
						return null;
					}
					int num = (int)obj;
					long legacyId = (long)base.GetPropertyValue(context, reader, PropTag.Message.Mid, null);
					ExchangeId item = ExchangeId.CreateFromInt64(context, base.Logon.StoreMailbox.ReplidGuidMap, legacyId);
					HashSet<ExchangeId> spoolerLockList = Microsoft.Exchange.Protocols.MAPI.Globals.GetSpoolerLockList(base.Logon.StoreMailbox);
					bool flag = spoolerLockList != null && spoolerLockList.Contains(item);
					if (flag)
					{
						num |= 1;
					}
					return num;
				}
			}
			else if (propTag != 1735131208U)
			{
				if (propTag == 1761673247U)
				{
					return this.GetConversationPreview(context, reader, PropTag.Message.ConversationLastMemberDocumentId);
				}
				if (propTag == 1762131999U)
				{
					return this.GetConversationPreview(context, reader, PropTag.Message.ConversationLastMemberDocumentIdMailboxWide);
				}
			}
			else
			{
				if (!base.Logon.UnifiedLogon)
				{
					return base.Logon.MailboxInstanceGuid;
				}
				int mailboxNumber = (int)base.GetPropertyValue(context, reader, PropTag.Message.MailboxNum, null);
				MailboxState mailboxState = MailboxStateCache.Get(context, mailboxNumber);
				return mailboxState.MailboxInstanceGuid;
			}
			obj = base.GetPropertyValue(context, reader, propertyTag, column);
			if (propertyTag.PropTag == 1732771860U && obj == null)
			{
				obj = ExchangeIdHelpers.Convert26ByteToLong(this.folderId.To26ByteArray());
			}
			return obj;
		}

		public override void OnRelease(MapiContext context)
		{
			this.CleanupMaterializedRestrictionSearchFolder();
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MapiViewMessage>(this);
		}

		private static SearchFolder CreateRestrictionSearchFolder(MailboxTaskContext mailboxTaskContext, Guid userIdentity, ExchangeId parentFolderId, string displayName, Restriction restriction, ExchangeId searchedFolderId, SetSearchCriteriaFlags searchCriteriaFlags, ref bool instantSearch, out bool openedExisting)
		{
			FaultInjection.InjectFault(MapiViewMessage.materializationTestHook);
			openedExisting = false;
			SearchFolder searchFolder = null;
			byte[] array = restriction.Serialize();
			ExchangeId exchangeId = Folder.FindFolderIdByName(mailboxTaskContext, parentFolderId, displayName, mailboxTaskContext.Mailbox);
			if (exchangeId.IsValid)
			{
				if (!instantSearch)
				{
					searchFolder = (Folder.OpenFolder(mailboxTaskContext, mailboxTaskContext.Mailbox, exchangeId) as SearchFolder);
					if (searchFolder != null)
					{
						byte[] x = (byte[])searchFolder.GetColumnValue(mailboxTaskContext, searchFolder.FolderTable.QueryCriteria);
						if (ValueHelper.ArraysEqual<byte>(x, array))
						{
							if (ExTraceGlobals.RestrictTracer.IsTraceEnabled(TraceType.DebugTrace))
							{
								ExTraceGlobals.RestrictTracer.TraceDebug<string, ExchangeId>(0L, "recycling existing search folder {0}, {1}", displayName, exchangeId);
							}
							openedExisting = true;
							return searchFolder;
						}
					}
				}
				instantSearch = true;
				displayName = Folder.FolderNameForMaterializedRestriction(mailboxTaskContext, mailboxTaskContext.Mailbox, exchangeId, null, instantSearch, restriction);
			}
			Folder folder = Folder.OpenFolder(mailboxTaskContext, mailboxTaskContext.Mailbox, parentFolderId);
			if (folder == null)
			{
				DiagnosticContext.TraceLocation((LID)37228U);
				return null;
			}
			exchangeId = mailboxTaskContext.Mailbox.GetNextObjectId(mailboxTaskContext);
			if (ExTraceGlobals.RestrictTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.RestrictTracer.TraceDebug<string, ExchangeId>(0L, "Creating new MatRet folder {0}, {1}.", displayName, exchangeId);
			}
			searchFolder = SearchFolder.CreateSearchFolder(mailboxTaskContext, folder, exchangeId, instantSearch, null);
			searchFolder.SetProperty(mailboxTaskContext, PropTag.Folder.IPMFolder, false);
			searchFolder.SetName(mailboxTaskContext, displayName);
			try
			{
				using (mailboxTaskContext.CreateUserIdentityFrame(userIdentity))
				{
					searchFolder.SetSearchCriteria(mailboxTaskContext, array, new ExchangeId[]
					{
						searchedFolderId
					}, searchCriteriaFlags, instantSearch, false, true);
				}
			}
			catch (SearchEvaluationInProgressException exception)
			{
				mailboxTaskContext.OnExceptionCatch(exception);
				if (ExTraceGlobals.RestrictTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.RestrictTracer.TraceDebug(0L, "search eval in progress. Dumping this folder.");
				}
				searchFolder.Delete(mailboxTaskContext);
				searchFolder = null;
				DiagnosticContext.TraceLocation((LID)58208U);
			}
			return searchFolder;
		}

		private void CleanupMaterializedRestrictionSearchFolder()
		{
			if (ExTraceGlobals.RestrictTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.RestrictTracer.TraceDebug<ExchangeId, uint>(0L, "Cleaning up any stale materialized restriction search folder over folder {0} (hsot:[{1}]).", this.folderId, base.Hsot);
			}
			if (this.Folder != null)
			{
				this.Folder.RemoveDoNotDeleteReference(this);
				if (ExTraceGlobals.RestrictTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.RestrictTracer.TraceDebug<bool>(0L, "Removed do-not-delete reference. Folder still has do-not-delete references: {0}", this.Folder.HasDoNotDeleteReferences);
				}
			}
			if (this.materializedRestrictionFolderId.IsValid && this.materializedRestrictionIsInstantSearchFolder)
			{
				if (ExTraceGlobals.RestrictTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.RestrictTracer.TraceDebug<ExchangeId>(0L, "Queuing folder {0} for deletion", this.materializedRestrictionFolderId);
				}
				MapiFolder.QueueInstantSearchDeletion(base.Logon.MapiMailbox.StoreMailbox, this.materializedRestrictionFolderId);
			}
			this.materializedRestrictionFolderId = ExchangeId.Zero;
			this.materializedRestrictionIsInstantSearchFolder = false;
		}

		private object GetConversationPreview(MapiContext context, Reader reader, StorePropTag propertyTag)
		{
			object propertyValue = base.GetPropertyValue(context, reader, propertyTag, null);
			if (propertyValue == null)
			{
				return null;
			}
			int num = (int)propertyValue;
			MessageTable messageTable = Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.MessageTable(context.Database);
			StartStopKey startStopKey = new StartStopKey(true, new object[]
			{
				base.Logon.MapiMailbox.SharedState.MailboxPartitionNumber,
				num
			});
			Column column = PropertySchema.MapToColumn(context.Database, Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message, PropTag.Message.Preview);
			using (TableOperator tableOperator = Factory.CreateTableOperator(context.Culture, context, messageTable.Table, messageTable.MessagePK, new Column[]
			{
				column
			}, null, null, 0, 0, new KeyRange(startStopKey, startStopKey), false, true))
			{
				using (Reader reader2 = tableOperator.ExecuteReader(true))
				{
					if (reader2.Read())
					{
						return reader2.GetString(column);
					}
				}
			}
			return null;
		}

		private void OnNotification(NotificationPublishPhase phase, Context transactionContext, NotificationEvent nev)
		{
			MapiContext mapiContext = transactionContext as MapiContext;
			using (mapiContext.SetMapiLogonForNotificationContext(base.Logon))
			{
				if (ExTraceGlobals.NotificationTracer.IsTraceEnabled(TraceType.FunctionTrace))
				{
					ExTraceGlobals.NotificationTracer.TraceFunction<uint, ExchangeId>(0L, "ENTER MapiViewMessage.OnNotification: hsot:[{0}], RealFolderId:[{1}]", base.Hsot, this.RealFolderId);
				}
				try
				{
					if (base.ConfigurationError.HasConfigurationError)
					{
						base.TraceNotificationIgnored(nev, "not configured");
					}
					else
					{
						ObjectNotificationEvent objectNotificationEvent = nev as ObjectNotificationEvent;
						if (objectNotificationEvent == null)
						{
							base.TraceNotificationIgnored(nev, "not an object notification");
						}
						else
						{
							if (ExTraceGlobals.NotificationTracer.IsTraceEnabled(TraceType.DebugTrace))
							{
								ExTraceGlobals.NotificationTracer.TraceDebug<EventType>(0L, "Evaluating notification event type {0}.", objectNotificationEvent.EventType);
							}
							if (!objectNotificationEvent.IsMessageEvent && objectNotificationEvent.EventType != EventType.MessagesLinked)
							{
								base.TraceNotificationIgnored(nev, "out of scope");
							}
							else if (EventFlags.None != (objectNotificationEvent.EventFlags & EventFlags.Conversation) != base.StoreViewTable is ConversationViewTable)
							{
								base.TraceNotificationIgnored(nev, "does not match a view type");
							}
							else if (base.Logon.IsThereTableChangedNotification(base.Hsot))
							{
								Statistics.MiscelaneousNotifications.SkippedMessageTableNotifications.Bump();
								base.TraceNotificationIgnored(nev, "detected TABLE_CHANGED notification already queued");
							}
							else
							{
								TableEventType tableEventType = TableEventType.Error;
								ExchangeId fid = ExchangeId.Null;
								ExchangeId mid = ExchangeId.Null;
								int? num = null;
								EventType eventType = objectNotificationEvent.EventType;
								if (eventType <= EventType.ObjectModified)
								{
									switch (eventType)
									{
									case EventType.NewMail:
									case EventType.ObjectCreated:
										break;
									case EventType.CriticalError | EventType.NewMail:
										goto IL_2DD;
									default:
										if (eventType == EventType.ObjectDeleted)
										{
											goto IL_265;
										}
										if (eventType != EventType.ObjectModified)
										{
											goto IL_2DD;
										}
										if (objectNotificationEvent.ParentFid == this.RealFolderId)
										{
											tableEventType = TableEventType.RowModified;
											fid = objectNotificationEvent.Fid;
											mid = objectNotificationEvent.Mid;
											num = objectNotificationEvent.DocumentId;
											goto IL_2DD;
										}
										goto IL_2DD;
									}
								}
								else if (eventType <= EventType.ObjectCopied)
								{
									if (eventType != EventType.ObjectMoved)
									{
										if (eventType != EventType.ObjectCopied)
										{
											goto IL_2DD;
										}
									}
									else
									{
										ObjectMovedCopiedNotificationEvent objectMovedCopiedNotificationEvent = objectNotificationEvent as ObjectMovedCopiedNotificationEvent;
										if (objectMovedCopiedNotificationEvent == null)
										{
											base.TraceNotificationIgnored(nev, "invalid ObjectMoved notification");
											return;
										}
										if (objectMovedCopiedNotificationEvent.OldParentFid == this.RealFolderId)
										{
											tableEventType = TableEventType.RowDeleted;
											fid = objectMovedCopiedNotificationEvent.OldFid;
											mid = objectMovedCopiedNotificationEvent.OldMid;
											num = objectMovedCopiedNotificationEvent.DocumentId;
											goto IL_2DD;
										}
										if (objectMovedCopiedNotificationEvent.ParentFid == this.RealFolderId)
										{
											tableEventType = TableEventType.RowAdded;
											fid = objectMovedCopiedNotificationEvent.Fid;
											mid = objectMovedCopiedNotificationEvent.Mid;
											num = objectMovedCopiedNotificationEvent.DocumentId;
											goto IL_2DD;
										}
										goto IL_2DD;
									}
								}
								else
								{
									if (eventType == EventType.MessageUnlinked)
									{
										goto IL_265;
									}
									if (eventType != EventType.MessagesLinked)
									{
										goto IL_2DD;
									}
									if (objectNotificationEvent.Fid == this.RealFolderId)
									{
										tableEventType = TableEventType.Changed;
										fid = objectNotificationEvent.Fid;
										goto IL_2DD;
									}
									goto IL_2DD;
								}
								if (objectNotificationEvent.ParentFid == this.RealFolderId)
								{
									tableEventType = TableEventType.RowAdded;
									fid = objectNotificationEvent.Fid;
									mid = objectNotificationEvent.Mid;
									num = objectNotificationEvent.DocumentId;
									goto IL_2DD;
								}
								goto IL_2DD;
								IL_265:
								if (objectNotificationEvent.ParentFid == this.RealFolderId)
								{
									tableEventType = TableEventType.RowDeleted;
									fid = objectNotificationEvent.Fid;
									mid = objectNotificationEvent.Mid;
									num = objectNotificationEvent.DocumentId;
								}
								IL_2DD:
								if (tableEventType != TableEventType.Error)
								{
									ExchangeId previousFid = ExchangeId.Zero;
									ExchangeId previousMid = ExchangeId.Zero;
									Properties row = Properties.Empty;
									bool flag = false;
									if (base.IsExplodingMultiValue)
									{
										if (ExTraceGlobals.NotificationTracer.IsTraceEnabled(TraceType.DebugTrace))
										{
											ExTraceGlobals.NotificationTracer.TraceDebug<TableEventType>(0L, "Converting {0} to TableChanged because the view has an exploded multi-value.", tableEventType);
										}
										tableEventType = TableEventType.Changed;
									}
									else if (base.StoreViewTable != null && base.StoreViewTable.IsCategorizedView)
									{
										flag = true;
										switch (tableEventType)
										{
										case TableEventType.RowAdded:
											if (base.StoreViewTable.CategoryCount != 1)
											{
												if (ExTraceGlobals.NotificationTracer.IsTraceEnabled(TraceType.DebugTrace))
												{
													ExTraceGlobals.NotificationTracer.TraceDebug(0L, "Converting RowAdded to TableChanged because the categorized views has more than one header.");
												}
												tableEventType = TableEventType.Changed;
											}
											else if (base.StoreViewTable.CategoryHeaderSortOverrides[0] != null)
											{
												if (ExTraceGlobals.NotificationTracer.IsTraceEnabled(TraceType.DebugTrace))
												{
													ExTraceGlobals.NotificationTracer.TraceDebug(0L, "Converting RowAdded to TableChanged because the category header has a sort override.");
												}
												tableEventType = TableEventType.Changed;
											}
											break;
										case TableEventType.RowDeleted:
											if (base.StoreViewTable.CategoryCount != 1)
											{
												if (ExTraceGlobals.NotificationTracer.IsTraceEnabled(TraceType.DebugTrace))
												{
													ExTraceGlobals.NotificationTracer.TraceDebug(0L, "Converting RowDeleted to TableChanged because the categorized views has more than one header.");
												}
												tableEventType = TableEventType.Changed;
											}
											else if (EventType.ObjectMoved != objectNotificationEvent.EventType && !this.IsSearchFolder)
											{
												if (ExTraceGlobals.NotificationTracer.IsTraceEnabled(TraceType.DebugTrace))
												{
													ExTraceGlobals.NotificationTracer.TraceDebug<EventType>(0L, "Converting RowDeleted to TableChanged because the event type was {0} and granular notifications on categorized views of normal folders are only supported for ObjectMoved.", objectNotificationEvent.EventType);
												}
												tableEventType = TableEventType.Changed;
											}
											break;
										case TableEventType.RowModified:
											if (!base.IsCommonCategorization)
											{
												tableEventType = TableEventType.Changed;
												if (ExTraceGlobals.NotificationTracer.IsTraceEnabled(TraceType.DebugTrace))
												{
													ExTraceGlobals.NotificationTracer.TraceDebug(0L, "Converting RowModified to TableChanged because the categorized views has more than one header or the header is not a common categorized prop.");
												}
											}
											else if ((objectNotificationEvent.EventFlags & EventFlags.CommonCategorizationPropertyChanged) != EventFlags.None)
											{
												tableEventType = TableEventType.Changed;
												if (ExTraceGlobals.NotificationTracer.IsTraceEnabled(TraceType.DebugTrace))
												{
													ExTraceGlobals.NotificationTracer.TraceDebug(0L, "Converting RowModified to TableChanged because the categorization property may have been modified.");
												}
											}
											break;
										default:
											if (ExTraceGlobals.NotificationTracer.IsTraceEnabled(TraceType.DebugTrace))
											{
												ExTraceGlobals.NotificationTracer.TraceDebug<TableEventType>(0L, "Converting {0} to TableChanged because the table event type is unsupported for categorized views.", tableEventType);
											}
											tableEventType = TableEventType.Changed;
											break;
										}
									}
									base.StoreViewTable.ForceReload(transactionContext, tableEventType != TableEventType.RowModified);
									if (TableEventType.RowAdded == tableEventType || TableEventType.RowModified == tableEventType || (flag && TableEventType.Changed != tableEventType))
									{
										if (this.ViewColumns != null)
										{
											MessageTable messageTable = Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.MessageTable(base.Logon.StoreMailbox.Database);
											SearchCriteria searchCriteria;
											if (this.IsSearchFolder && !(base.StoreViewTable is ConversationViewTable))
											{
												searchCriteria = Factory.CreateSearchCriteriaAnd(new SearchCriteria[]
												{
													Factory.CreateSearchCriteriaCompare(messageTable.IsHidden, SearchCriteriaCompare.SearchRelOp.Equal, Factory.CreateConstantColumn((this.configureFlags & ViewMessageConfigureFlags.ViewFAI) != ViewMessageConfigureFlags.None, messageTable.IsHidden)),
													Factory.CreateSearchCriteriaCompare(messageTable.MessageId, SearchCriteriaCompare.SearchRelOp.Equal, Factory.CreateConstantColumn(mid.To26ByteArray(), messageTable.MessageId))
												});
											}
											else
											{
												searchCriteria = Factory.CreateSearchCriteriaCompare(messageTable.MessageDocumentId, SearchCriteriaCompare.SearchRelOp.Equal, Factory.CreateConstantColumn(num.Value, messageTable.MessageDocumentId));
											}
											if (flag)
											{
												if (!this.GenerateCategoryHeaderRowNotification(transactionContext, objectNotificationEvent, tableEventType, num.Value, searchCriteria))
												{
													return;
												}
												if (ExTraceGlobals.NotificationTracer.IsTraceEnabled(TraceType.DebugTrace))
												{
													ExTraceGlobals.NotificationTracer.TraceDebug<TableEventType>(0L, "Generating leaf row notification for event type {0}.", tableEventType);
												}
											}
											object[] array = null;
											if (TableEventType.RowDeleted != tableEventType && !this.GetTableNotificationInfo(transactionContext, searchCriteria, out row, out array))
											{
												base.TraceNotificationIgnored(nev, "row not found");
												return;
											}
											if (array != null)
											{
												previousFid = ExchangeId.CreateFromInt64(transactionContext, base.Logon.StoreMailbox.ReplidGuidMap, (long)array[0]);
												if (array[1] != null)
												{
													previousMid = ExchangeId.CreateFromInt64(transactionContext, base.Logon.StoreMailbox.ReplidGuidMap, (long)array[1]);
												}
												else if (array[2] != null)
												{
													previousMid = ExchangeId.CreateFromInt64(transactionContext, base.Logon.StoreMailbox.ReplidGuidMap, (long)array[2]);
												}
											}
										}
										else
										{
											if (ExTraceGlobals.NotificationTracer.IsTraceEnabled(TraceType.DebugTrace))
											{
												ExTraceGlobals.NotificationTracer.TraceDebug<TableEventType>(0L, "Converting {0} to TableChanged because no view columns have been defined.", tableEventType);
											}
											tableEventType = TableEventType.Changed;
										}
									}
									else if (TableEventType.RowDeleted == tableEventType && base.StoreViewTable is ConversationViewTable)
									{
										MessageDeletedNotificationEvent messageDeletedNotificationEvent = nev as MessageDeletedNotificationEvent;
										if (messageDeletedNotificationEvent != null)
										{
											row = new Properties(1);
											row.Add(PropTag.Message.ConversationId, messageDeletedNotificationEvent.ConversationId);
										}
									}
									int inst = ((this.configureFlags & ViewMessageConfigureFlags.RetrieveFromIndexOnly) == ViewMessageConfigureFlags.None) ? 0 : num.Value;
									TableModifiedNotificationEvent nev2 = MapiViewTableBase.CreateTableModifiedEvent(base.Logon.MapiMailbox.Database, base.Logon.MapiMailbox.StoreMailbox.MailboxNumber, transactionContext.ClientType, objectNotificationEvent.EventFlags, tableEventType, fid, mid, inst, previousFid, previousMid, 0, row);
									base.Logon.AddPendingNotification(nev2, this, base.Hsot);
								}
								else
								{
									base.TraceNotificationIgnored(nev, "Error event type");
								}
							}
						}
					}
				}
				finally
				{
					if (ExTraceGlobals.NotificationTracer.IsTraceEnabled(TraceType.FunctionTrace))
					{
						ExTraceGlobals.NotificationTracer.TraceFunction<uint, ExchangeId>(0L, "EXIT MapiViewMessage.OnNotification: hsot:[{0}], folderId:[{1}]", base.Hsot, this.folderId);
					}
				}
			}
		}

		private void OnConversationMessageNotification(NotificationPublishPhase phase, Context context, NotificationEvent nev)
		{
			if (ExTraceGlobals.NotificationTracer.IsTraceEnabled(TraceType.FunctionTrace))
			{
				ExTraceGlobals.NotificationTracer.TraceFunction<uint, ExchangeId>(0L, "ENTER MapiViewMessage.OnConversationMessageNotification: hsot:[{0}], RealFolderId:[{1}]", base.Hsot, this.RealFolderId);
			}
			try
			{
				if (base.ConfigurationError.HasConfigurationError)
				{
					base.TraceNotificationIgnored(nev, "not configured");
				}
				else
				{
					ObjectNotificationEvent objectNotificationEvent = nev as ObjectNotificationEvent;
					if (objectNotificationEvent == null)
					{
						base.TraceNotificationIgnored(nev, "not an object notification");
					}
					else
					{
						if (ExTraceGlobals.NotificationTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							ExTraceGlobals.NotificationTracer.TraceDebug<EventType>(0L, "Evaluating notification event type {0}.", objectNotificationEvent.EventType);
						}
						if (base.Logon.IsThereTableChangedNotification(base.Hsot))
						{
							Statistics.MiscelaneousNotifications.SkippedMessageTableNotifications.Bump();
							base.TraceNotificationIgnored(nev, "detected TABLE_CHANGED notification already queued");
						}
						else
						{
							ConversationMessageViewTable conversationMessageViewTable = base.StoreViewTable as ConversationMessageViewTable;
							if (conversationMessageViewTable == null)
							{
								base.TraceNotificationIgnored(nev, "mismatched view type");
							}
							else if (conversationMessageViewTable.ConversationId == null)
							{
								base.TraceNotificationIgnored(nev, "ConversationMessage view has not yet been restricted to a specific conversation id");
							}
							else
							{
								int? conversationDocumentId = conversationMessageViewTable.GetConversationDocumentId(context);
								if (conversationDocumentId == null)
								{
									base.TraceNotificationIgnored(nev, "ConversationMessage view is on a conversation that does not exist.");
								}
								else
								{
									bool flag;
									if (objectNotificationEvent.ConversationDocumentId == conversationDocumentId)
									{
										flag = true;
									}
									else if (objectNotificationEvent is ObjectMovedCopiedNotificationEvent)
									{
										flag = (objectNotificationEvent.EventType == EventType.ObjectMoved && ((ObjectMovedCopiedNotificationEvent)objectNotificationEvent).OldConversationDocumentId == conversationDocumentId);
									}
									else
									{
										flag = (objectNotificationEvent is MessageModifiedNotificationEvent && ((MessageModifiedNotificationEvent)objectNotificationEvent).OldConversationDocumentId == conversationDocumentId);
									}
									if (!flag)
									{
										base.TraceNotificationIgnored(nev, "message is not a member of the conversation");
									}
									else
									{
										Properties empty = Properties.Empty;
										EventType eventType = objectNotificationEvent.EventType;
										TableEventType tableEventType;
										if (eventType <= EventType.ObjectDeleted)
										{
											switch (eventType)
											{
											case EventType.NewMail:
											case EventType.ObjectCreated:
												break;
											case EventType.CriticalError | EventType.NewMail:
												goto IL_284;
											default:
												if (eventType != EventType.ObjectDeleted)
												{
													goto IL_284;
												}
												tableEventType = TableEventType.RowDeleted;
												goto IL_295;
											}
										}
										else if (eventType != EventType.ObjectModified)
										{
											if (eventType == EventType.ObjectMoved)
											{
												if (ExTraceGlobals.NotificationTracer.IsTraceEnabled(TraceType.DebugTrace))
												{
													ExTraceGlobals.NotificationTracer.TraceDebug(0L, "Converting ObjectMoved to TableChanged because granular notifications are not currently supported for move-message in ComversationMessage views.");
												}
												tableEventType = TableEventType.Changed;
												goto IL_295;
											}
											if (eventType != EventType.ObjectCopied)
											{
												goto IL_284;
											}
										}
										if (this.GetConversationMessageViewTableNotificationInfo(context, objectNotificationEvent, out empty))
										{
											tableEventType = ((objectNotificationEvent.EventType == EventType.ObjectModified) ? TableEventType.RowModified : TableEventType.RowAdded);
											goto IL_295;
										}
										base.TraceNotificationIgnored(nev, "row not found");
										return;
										IL_284:
										base.TraceNotificationIgnored(nev, "invalid ConversationMessage notification event");
										return;
										IL_295:
										TableModifiedNotificationEvent nev2 = MapiViewTableBase.CreateTableModifiedEvent(base.Logon.MapiMailbox.Database, base.Logon.MapiMailbox.StoreMailbox.MailboxNumber, context.ClientType, objectNotificationEvent.EventFlags, tableEventType, objectNotificationEvent.Fid, objectNotificationEvent.Mid, 0, ExchangeId.Zero, ExchangeId.Zero, 0, empty);
										base.Logon.AddPendingNotification(nev2, this, base.Hsot);
									}
								}
							}
						}
					}
				}
			}
			finally
			{
				if (ExTraceGlobals.NotificationTracer.IsTraceEnabled(TraceType.FunctionTrace))
				{
					ExTraceGlobals.NotificationTracer.TraceFunction<uint, ExchangeId>(0L, "EXIT MapiViewMessage.OnConversationMessageNotification: hsot:[{0}], folderId:[{1}]", base.Hsot, this.folderId);
				}
			}
		}

		private bool ShouldUseMaterializedRestriction(MapiContext context, Restriction restriction)
		{
			if (restriction == null || !this.CanMaterializedRestrictionBeUsedForView())
			{
				return false;
			}
			if (this.restrictionMustbeHandledByFullTextIndex)
			{
				return true;
			}
			if (this.isCategorizedView)
			{
				return true;
			}
			if (base.Logon.IsMoveDestination)
			{
				return (this.configureFlags & ViewMessageConfigureFlags.ViewFAI) == ViewMessageConfigureFlags.None;
			}
			if (context.ClientType == ClientType.MoMT || context.ClientType == ClientType.OWA)
			{
				if (restriction.HasClauseMeetingPredicate((Restriction clause) => clause.RefersToProperty(PropTag.Message.FlagCompleteTime)))
				{
					ushort propId;
					StoreNamedPropInfo storeNamedPropInfo;
					bool numberFromName = base.Logon.StoreMailbox.NamedPropertyMap.GetNumberFromName(context, NamedPropInfo.Task.TaskDateCompleted.PropName, false, base.Logon.StoreMailbox.QuotaInfo, out propId, out storeNamedPropInfo);
					Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(numberFromName, "unexpected scenario for a named props");
					Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(object.ReferenceEquals(NamedPropInfo.Task.TaskDateCompleted, storeNamedPropInfo), "unexpected scenario for a named props");
					StorePropTag propTag = new StorePropTag(propId, PropertyType.SysTime, storeNamedPropInfo, PropertyType.SysTime, WellKnownProperties.BaseObjectType[3]);
					if (restriction.HasClauseMeetingPredicate((Restriction clause) => clause.RefersToProperty(propTag)))
					{
						return true;
					}
				}
			}
			RestrictionProperty restrictionProperty = restriction as RestrictionProperty;
			if (restrictionProperty != null && restrictionProperty.PropertyTag.PropInfo == NamedPropInfo.Appointment.Recurring && restrictionProperty.Operator == RelationOperator.Equal)
			{
				return true;
			}
			if (ConfigurationSchema.ForceRimQueryMaterialization.Value)
			{
				if (restrictionProperty != null && restrictionProperty.PropertyTag == PropTag.Message.Subject && restrictionProperty.Operator == RelationOperator.Equal && restrictionProperty.Value is string && ((string)restrictionProperty.Value).StartsWith("RIM_"))
				{
					return true;
				}
				if (restriction.HasClauseMeetingPredicate((Restriction c) => c is RestrictionCompareProps && ((RestrictionCompareProps)c).PropertyTag2.IsNamedProperty && ((RestrictionCompareProps)c).PropertyTag2.PropName.Name.StartsWith("PR_RIM_")))
				{
					if (!restriction.HasClauseMeetingPredicate((Restriction c) => c is RestrictionProperty && ((RestrictionProperty)c).PropertyTag.ExternalType == PropertyType.SysTime))
					{
						return true;
					}
				}
			}
			return false;
		}

		private void MaterializeRestriction(MapiContext context)
		{
			bool flag = false;
			if (this.materializedRestrictionFolderId.IsValid)
			{
				SearchFolder searchFolder = Folder.OpenFolder(context, base.Logon.MapiMailbox.StoreMailbox, this.materializedRestrictionFolderId) as SearchFolder;
				if (searchFolder != null)
				{
					byte[] y = this.restriction.Serialize();
					byte[] x = (byte[])searchFolder.GetColumnValue(context, searchFolder.FolderTable.QueryCriteria);
					flag = ValueHelper.ArraysEqual<byte>(x, y);
				}
			}
			if (!flag)
			{
				MapiViewMessage.<>c__DisplayClass11 CS$<>8__locals1 = new MapiViewMessage.<>c__DisplayClass11();
				this.CleanupMaterializedRestrictionSearchFolder();
				CS$<>8__locals1.instantSearch = MapiViewMessage.RestrictionQualifiedForInstantSearch(this.restriction);
				bool? hiddenItemView = ((this.configureFlags & ViewMessageConfigureFlags.ViewAll) != ViewMessageConfigureFlags.None) ? null : new bool?(ViewMessageConfigureFlags.None != (this.configureFlags & ViewMessageConfigureFlags.ViewFAI));
				CS$<>8__locals1.displayName = Folder.FolderNameForMaterializedRestriction(context, base.Logon.MapiMailbox.StoreMailbox, this.folderId, hiddenItemView, CS$<>8__locals1.instantSearch, this.restriction);
				using (EventWaitHandleEx finishedEvent = new EventWaitHandleEx(true, false))
				{
					bool flag2 = false;
					this.materizationError = ErrorCode.NoError;
					try
					{
						Guid userIdentity = context.UserIdentity;
						MailboxTaskQueue.LaunchMailboxTask<MailboxTaskContext>(context, MailboxTaskQueue.Priority.High, TaskTypeId.CategorizedViewSearchFolderRestriction, base.Logon.MapiMailbox.StoreMailbox.SharedState, context.SecurityContext.UserSid, context.ClientType, context.Culture, (MailboxTaskContext mailboxTaskContext, Func<bool> shouldTaskContinue) => this.CreateMaterializedRestrictionAndPopulate(mailboxTaskContext, userIdentity, CS$<>8__locals1.displayName, this.restriction, this.folderId, CS$<>8__locals1.instantSearch, finishedEvent, shouldTaskContinue));
						if (ExTraceGlobals.RestrictTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							ExTraceGlobals.RestrictTracer.TraceDebug(0L, "Obtaining a materialized restriction folder, which may require creation and population. Waiting for completion...");
						}
						while (!finishedEvent.WaitOne(TimeSpan.Zero))
						{
							ErrorCode errorCode = context.PulseMailboxOperation(delegate()
							{
								finishedEvent.WaitOne(TimeSpan.FromMilliseconds(100.0));
							});
							if (errorCode != ErrorCode.NoError)
							{
								throw new StoreException((LID)57472U, errorCode);
							}
						}
						flag2 = true;
					}
					finally
					{
						if (this.materizationError != ErrorCode.NoError)
						{
							throw new StoreException((LID)61804U, this.materizationError, "Restriciton materializating task failed");
						}
						if (flag2 && this.materializedRestrictionFolderId.IsNullOrZero)
						{
							throw new StoreException((LID)32896U, ErrorCodeValue.InvalidObject, "Invalid underlying search folder");
						}
					}
				}
			}
			if (ExTraceGlobals.RestrictTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.RestrictTracer.TraceDebug<ExchangeId, bool, ExchangeId>(0L, "Materialized restriction configured for message view: folderId=={0}, reuseExistingMaterializedRestriction=={1}, materializedRestrictionFolderId=={2}", this.folderId, flag, this.materializedRestrictionFolderId);
			}
		}

		private void ConfigureMapiViewMessage(MapiContext context, MapiLogon mapiLogon, uint hsot)
		{
			ViewTable storeViewTable = null;
			NotificationSubscription subscription = null;
			if ((this.configureFlags & ViewMessageConfigureFlags.EmptyTable) == ViewMessageConfigureFlags.None)
			{
				if (ClientTypeHelper.IsContentIndexing(context.ClientType) && (this.configureFlags & ViewMessageConfigureFlags.MailboxScopeView) == ViewMessageConfigureFlags.None && PropertyBagHelpers.TestPropertyFlags(context, mapiLogon.StoreMailbox, PropTag.Mailbox.MailboxFlags, 16, 16))
				{
					throw new StoreException((LID)45664U, ErrorCodeValue.NotSupported);
				}
				if ((this.configureFlags & ViewMessageConfigureFlags.Conversation) != ViewMessageConfigureFlags.None)
				{
					this.expandedConversationView = ((this.configureFlags & ViewMessageConfigureFlags.ExpandedConversations) != ViewMessageConfigureFlags.None);
					if (this.expandedConversationView)
					{
						this.configureFlags |= ViewMessageConfigureFlags.NoNotifications;
					}
					bool useIndexForEmptyFolder = mapiLogon.IsMoveDestination || ViewMessageConfigureFlags.None == (this.configureFlags & ViewMessageConfigureFlags.NoNotifications);
					storeViewTable = new ConversationViewTable(context, mapiLogon.StoreMailbox, this.RealFolderId, useIndexForEmptyFolder, this.expandedConversationView);
				}
				else if ((this.configureFlags & ViewMessageConfigureFlags.ConversationMembers) != ViewMessageConfigureFlags.None)
				{
					storeViewTable = new ConversationMessageViewTable(context, mapiLogon.StoreMailbox, this.RealFolderId);
				}
				else
				{
					if ((this.configureFlags & ViewMessageConfigureFlags.MailboxScopeView) != ViewMessageConfigureFlags.None)
					{
						if (this.folderId != mapiLogon.FidC.FidRoot)
						{
							throw new StoreException((LID)58576U, ErrorCodeValue.NotSupported);
						}
						this.mailboxScope = true;
						if ((this.configureFlags & ViewMessageConfigureFlags.PrereadExtendedProperties) != ViewMessageConfigureFlags.None)
						{
							this.prereadExtendedProperties = true;
						}
					}
					bool? hiddenItemView = ((this.configureFlags & ViewMessageConfigureFlags.ViewAll) != ViewMessageConfigureFlags.None) ? null : new bool?(ViewMessageConfigureFlags.None != (this.configureFlags & ViewMessageConfigureFlags.ViewFAI));
					storeViewTable = new MessageViewTable(context, mapiLogon.StoreMailbox, this.mailboxScope ? ExchangeId.Zero : this.RealFolderId, hiddenItemView, ViewMessageConfigureFlags.None == (this.configureFlags & ViewMessageConfigureFlags.DoNotUseLazyIndex), ViewMessageConfigureFlags.None != (this.configureFlags & ViewMessageConfigureFlags.UseCoveringIndex), mapiLogon.IsMoveDestination);
				}
				if (hsot != 4294967295U && (this.configureFlags & ViewMessageConfigureFlags.NoNotifications) == ViewMessageConfigureFlags.None)
				{
					if ((this.configureFlags & ViewMessageConfigureFlags.ConversationMembers) == ViewMessageConfigureFlags.None)
					{
						subscription = new FolderChildrenSubscription(SubscriptionKind.PostCommit, mapiLogon.Session.NotificationContext, mapiLogon.MapiMailbox.Database, mapiLogon.MapiMailbox.MailboxNumber, EventType.ObjectCreated | EventType.ObjectDeleted | EventType.ObjectModified | EventType.ObjectMoved | EventType.ObjectCopied | EventType.BeginLongOperation | EventType.EndLongOperation | EventType.MessageUnlinked | EventType.MessagesLinked, new NotificationCallback(this.OnNotification), this.RealFolderId);
					}
					else
					{
						subscription = new ConversationMessageSubscription(SubscriptionKind.PostCommit, mapiLogon.Session.NotificationContext, mapiLogon.MapiMailbox.Database, mapiLogon.MapiMailbox.MailboxNumber, EventType.NewMail | EventType.ObjectCreated | EventType.ObjectDeleted | EventType.ObjectModified | EventType.ObjectMoved | EventType.ObjectCopied, new NotificationCallback(this.OnConversationMessageNotification));
					}
				}
			}
			base.Configure(context, mapiLogon, this.isCategorizedView ? MapiViewMessage.categorizedViewNotificationColumns : MapiViewMessage.notificationColumns, storeViewTable, hsot, subscription);
			if (this.columns != null)
			{
				this.SetColumns(context, this.columns, this.columnsFlags);
			}
		}

		public override void SetColumns(MapiContext context, StorePropTag[] columns, MapiViewSetColumnsFlag flags)
		{
			this.columns = columns;
			this.columnsFlags = flags;
			base.SetColumns(context, columns, flags);
		}

		private bool GetConversationMessageViewTableNotificationInfo(Context context, ObjectNotificationEvent onev, out Properties row)
		{
			bool result = false;
			MessageTable messageTable = Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.MessageTable(context.Database);
			SearchCriteria criteria = Factory.CreateSearchCriteriaAnd(new SearchCriteria[]
			{
				Factory.CreateSearchCriteriaCompare(messageTable.FolderId, SearchCriteriaCompare.SearchRelOp.Equal, Factory.CreateConstantColumn(onev.Fid.To26ByteArray(), messageTable.FolderId)),
				Factory.CreateSearchCriteriaCompare(messageTable.MessageId, SearchCriteriaCompare.SearchRelOp.Equal, Factory.CreateConstantColumn(onev.Mid.To26ByteArray(), messageTable.MessageId))
			});
			ConversationMessageViewTable conversationMessageViewTable = base.StoreViewTable as ConversationMessageViewTable;
			conversationMessageViewTable.SaveLastBookmark();
			try
			{
				bool flag;
				using (Reader reader = conversationMessageViewTable.FindRow(context, criteria, ViewSeekOrigin.Beginning, null, false, out flag))
				{
					if (reader != null)
					{
						row = base.FillRow((MapiContext)context, reader);
						result = true;
					}
				}
			}
			finally
			{
				conversationMessageViewTable.RevertToLastBookmark();
			}
			return result;
		}

		private bool GenerateCategoryHeaderRowNotification(Context transactionContext, ObjectNotificationEvent onev, TableEventType tableEventType, int documentId, SearchCriteria findLeafRowCriteria)
		{
			Properties empty = Properties.Empty;
			object[] array = null;
			ExchangeId previousFid = ExchangeId.Zero;
			ExchangeId previousMid = ExchangeId.Zero;
			MessageViewTable messageViewTable = base.StoreViewTable as MessageViewTable;
			if (ExTraceGlobals.NotificationTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.NotificationTracer.TraceDebug<TableEventType>(0L, "Generating header row notification for leaf row event type {0}.", tableEventType);
			}
			ExchangeId mid;
			TableEventType tableEventType2;
			if (TableEventType.RowDeleted == tableEventType)
			{
				tableEventType2 = this.GetCategoryHeaderRowNotificationInfoForDelete(transactionContext, documentId, out mid, out empty, out array);
			}
			else
			{
				if (!this.GetCategoryHeaderRowNotificationInfoForAddOrModify(transactionContext, findLeafRowCriteria, out mid, out empty, out array))
				{
					base.TraceNotificationIgnored(onev, "category header row not found");
					return false;
				}
				tableEventType2 = TableEventType.RowModified;
			}
			if (array != null)
			{
				previousFid = ExchangeId.CreateFromInt64(transactionContext, base.Logon.StoreMailbox.ReplidGuidMap, (long)array[0]);
				if (array[1] != null)
				{
					previousMid = ExchangeId.CreateFromInt64(transactionContext, base.Logon.StoreMailbox.ReplidGuidMap, (long)array[1]);
				}
				else if (array[2] != null)
				{
					previousMid = ExchangeId.CreateFromInt64(transactionContext, base.Logon.StoreMailbox.ReplidGuidMap, (long)array[2]);
				}
			}
			TableModifiedNotificationEvent nev = MapiViewTableBase.CreateTableModifiedEvent(base.Logon.MapiMailbox.Database, base.Logon.MapiMailbox.StoreMailbox.MailboxNumber, transactionContext.ClientType, onev.EventFlags, tableEventType2, this.FolderId, mid, 0, previousFid, previousMid, 0, empty);
			base.Logon.AddPendingNotification(nev, this, base.Hsot);
			if (base.Logon.IsThereTableChangedNotification(base.Hsot))
			{
				Statistics.MiscelaneousNotifications.SkippedMessageTableNotifications.Bump();
				base.TraceNotificationIgnored(onev, "sending TABLE_CHANGED notification");
				return false;
			}
			bool flag = messageViewTable.CollapseState.IsHeaderExpanded(0, mid.ToLong());
			if (!flag)
			{
				base.TraceNotificationIgnored(onev, "header collapsed");
			}
			return flag;
		}

		private bool GetCategoryHeaderRowNotificationInfoForAddOrModify(Context context, SearchCriteria findRowCriteria, out ExchangeId categId, out Properties rowProps, out object[] prevRowProps)
		{
			rowProps = Properties.Empty;
			prevRowProps = null;
			categId = ExchangeId.Null;
			bool result = false;
			MessageViewTable messageViewTable = base.StoreViewTable as MessageViewTable;
			messageViewTable.SaveLastBookmark();
			try
			{
				using (Reader reader = messageViewTable.FindCategoryHeaderRowForMessage(context, findRowCriteria))
				{
					if (reader != null)
					{
						long @int = reader.GetInt64(PropertySchema.MapToColumn(context.Database, Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message, PropTag.Message.CategID));
						categId = ExchangeId.CreateFromInt64(context, base.Logon.StoreMailbox.ReplidGuidMap, @int);
						rowProps = base.FillRow((MapiContext)context, reader);
						prevRowProps = this.GetPrevRowPropsForCategoryHeaderRowNotification(context);
						result = true;
					}
				}
			}
			finally
			{
				messageViewTable.RevertToLastBookmark();
			}
			return result;
		}

		private TableEventType GetCategoryHeaderRowNotificationInfoForDelete(Context context, int documentId, out ExchangeId categId, out Properties rowProps, out object[] prevRowProps)
		{
			TableEventType tableEventType = TableEventType.RowModified;
			bool flag = false;
			MessageViewTable messageViewTable = base.StoreViewTable as MessageViewTable;
			categId = ExchangeId.Null;
			rowProps = Properties.Empty;
			prevRowProps = null;
			Column column = messageViewTable.SortOrder.Columns[0];
			bool flag2 = messageViewTable.CategoryHeaderSortOverrides[0] != null;
			List<Column> list = new List<Column>(flag2 ? 2 : 1);
			list.Add(column);
			if (flag2)
			{
				list.Add(messageViewTable.CategoryHeaderSortOverrides[0].Column);
			}
			using (Reader reader = messageViewTable.FindMessageDocumentId(context, documentId, list))
			{
				if (reader != null)
				{
					SearchCriteria findRowCriteria = Factory.CreateSearchCriteriaCompare(column, SearchCriteriaCompare.SearchRelOp.Equal, Factory.CreateConstantColumn(reader.GetValue(column), column));
					object x = null;
					if (flag2)
					{
						x = reader.GetValue(messageViewTable.CategoryHeaderSortOverrides[0].Column);
					}
					messageViewTable.SaveLastBookmark();
					try
					{
						using (Reader reader2 = messageViewTable.FindCategoryHeaderRowForMessage(context, findRowCriteria))
						{
							if (reader2 != null)
							{
								long @int = reader2.GetInt64(PropertySchema.MapToColumn(context.Database, Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message, PropTag.Message.CategID));
								categId = ExchangeId.CreateFromInt64(context, base.Logon.StoreMailbox.ReplidGuidMap, @int);
								if (flag2 && messageViewTable.CollapseState.IsHeaderExpanded(0, @int))
								{
									object value = reader2.GetValue(messageViewTable.CategoryHeaderSortOverrides[0].Column);
									int num = ValueHelper.ValuesCompare(x, value, context.Culture.CompareInfo, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth);
									bool aggregateByMaxValue = messageViewTable.CategoryHeaderSortOverrides[0].AggregateByMaxValue;
									if ((aggregateByMaxValue && num > 0) || (!aggregateByMaxValue && num < 0))
									{
										tableEventType = TableEventType.Changed;
										if (ExTraceGlobals.NotificationTracer.IsTraceEnabled(TraceType.DebugTrace))
										{
											ExTraceGlobals.NotificationTracer.TraceDebug(0L, "Generating TableChanged for category header row because the aggregation winner changed after a leaf row was deleted.");
										}
									}
								}
								if (tableEventType == TableEventType.RowModified)
								{
									rowProps = base.FillRow((MapiContext)context, reader2);
									prevRowProps = this.GetPrevRowPropsForCategoryHeaderRowNotification(context);
								}
								flag = true;
							}
						}
					}
					finally
					{
						messageViewTable.RevertToLastBookmark();
					}
				}
			}
			if (!flag)
			{
				tableEventType = TableEventType.Changed;
				if (ExTraceGlobals.NotificationTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.NotificationTracer.TraceDebug(0L, "Generating TableChanged for category header row because the header row could not be found.");
				}
			}
			return tableEventType;
		}

		private object[] GetPrevRowPropsForCategoryHeaderRowNotification(Context context)
		{
			object[] array = null;
			MessageViewTable messageViewTable = base.StoreViewTable as MessageViewTable;
			if (this.mappedNotificationColumns.Length != 0)
			{
				using (Reader reader = messageViewTable.QueryRows(context, 1, true))
				{
					if (reader != null && reader.Read())
					{
						array = new object[this.mappedNotificationColumns.Length];
						for (int i = 0; i < this.mappedNotificationColumns.Length; i++)
						{
							array[i] = reader.GetValue(this.mappedNotificationColumns[i]);
						}
					}
				}
			}
			return array;
		}

		private void ValidateMailboxScopeRestriction(MapiContext context, Restriction restriction, bool forFindRow)
		{
			List<int> list = null;
			if (restriction is RestrictionOR)
			{
				RestrictionOR restrictionOR = restriction as RestrictionOR;
				if (restrictionOR.NestedRestrictions == null)
				{
					throw new StoreException((LID)61136U, ErrorCodeValue.TooComplex);
				}
				for (int i = 0; i < restrictionOR.NestedRestrictions.Length; i++)
				{
					this.ValidateMailboxScopePropertyRestriction(restrictionOR.NestedRestrictions[i], forFindRow, ref list);
				}
			}
			else
			{
				this.ValidateMailboxScopePropertyRestriction(restriction, forFindRow, ref list);
			}
			if (list != null && list.Count > 1)
			{
				MessageTable messageTable = Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.MessageTable(base.StoreViewTable.Mailbox.Database);
				List<KeyRange> list2 = new List<KeyRange>(list.Count);
				for (int j = 0; j < list.Count; j++)
				{
					StartStopKey startStopKey = new StartStopKey(true, new object[]
					{
						base.StoreViewTable.Mailbox.MailboxPartitionNumber,
						list[j]
					});
					list2.Add(new KeyRange(startStopKey, startStopKey));
				}
				if (this.prereadExtendedProperties)
				{
					using (PreReadOperator preReadOperator = Factory.CreatePreReadOperator(context.Culture, context, messageTable.Table, messageTable.MessagePK, list2, new List<Column>
					{
						messageTable.OffPagePropertyBlob
					}, true))
					{
						preReadOperator.ExecuteScalar();
					}
				}
			}
		}

		private void ValidateMailboxScopePropertyRestriction(Restriction restriction, bool forFindRow, ref List<int> documentIdList)
		{
			RestrictionProperty restrictionProperty = restriction as RestrictionProperty;
			if (restrictionProperty == null)
			{
				throw new StoreException((LID)47888U, ErrorCodeValue.InvalidParameter);
			}
			if (restrictionProperty.PropertyTag == PropTag.Message.DocumentId)
			{
				if (restrictionProperty.Operator == RelationOperator.Equal || (forFindRow && (restrictionProperty.Operator == RelationOperator.GreaterThan || restrictionProperty.Operator == RelationOperator.GreaterThanEqual)))
				{
					if (documentIdList == null)
					{
						documentIdList = new List<int>();
					}
					documentIdList.Add((int)restrictionProperty.Value);
					return;
				}
			}
			else if (restrictionProperty.PropertyTag == PropTag.Message.EntryIdSvrEid && restrictionProperty.Operator == RelationOperator.Equal)
			{
				return;
			}
			throw new StoreException((LID)44752U, ErrorCodeValue.TooComplex);
		}

		private bool CanMaterializedRestrictionBeUsedForView()
		{
			bool flag = (this.configureFlags & (ViewMessageConfigureFlags.Conversation | ViewMessageConfigureFlags.ConversationMembers | ViewMessageConfigureFlags.MailboxScopeView | ViewMessageConfigureFlags.ExpandedConversations)) == ViewMessageConfigureFlags.None;
			return ConfigurationSchema.EnableMaterializedRestriction.Value && flag;
		}

		private IEnumerator<MailboxTaskQueue.TaskStepResult> CreateMaterializedRestrictionAndPopulate(MailboxTaskContext context, Guid userIdentity, string displayName, Restriction restriction, ExchangeId searchedFolderId, bool instantSearch, EventWaitHandleEx finishedEvent, Func<bool> shouldTaskContinue)
		{
			SearchFolder searchFolder = null;
			bool folderAlreadyExists = false;
			SetSearchCriteriaFlags flags = SetSearchCriteriaFlags.None;
			try
			{
				ExchangeId parentFolderId = ((LogicalMailbox)context.Mailbox).GetMaterializedRestrictionRootForFolder(context, searchedFolderId);
				flags = SetSearchCriteriaFlags.Shallow;
				if (!instantSearch && !this.restrictionMustbeHandledByFullTextIndex)
				{
					flags |= SetSearchCriteriaFlags.NonContentIndexed;
				}
				try
				{
					searchFolder = MapiViewMessage.CreateRestrictionSearchFolder(context, userIdentity, parentFolderId, displayName, restriction, searchedFolderId, flags, ref instantSearch, out folderAlreadyExists);
				}
				catch (StoreException ex)
				{
					DiagnosticContext.TraceLocation((LID)53612U);
					this.materizationError = ex.Error;
					yield break;
				}
				if (searchFolder != null)
				{
					searchFolder.AddDoNotDeleteReference(this);
					this.materializedRestrictionFolderId = searchFolder.GetId(context);
					this.materializedRestrictionIsInstantSearchFolder = instantSearch;
					if (!folderAlreadyExists)
					{
						searchFolder.SetProperty(context, PropTag.Folder.AllowAgeOut, true);
						if (!instantSearch)
						{
							searchFolder.SetProperty(context, PropTag.Folder.SearchFolderAgeOutTimeout, MapiViewMessage.AssociatedSearchFolderAgeOutTime);
						}
						Folder parentFolder = Folder.OpenFolder(context, context.Mailbox, parentFolderId);
						byte[] aclTableAndSDBuffer = (byte[])parentFolder.GetPropertyValue(context, PropTag.Folder.AclTableAndSecurityDescriptor);
						aclTableAndSDBuffer = FolderSecurity.AclTableAndSecurityDescriptorProperty.CreateForChildFolder(aclTableAndSDBuffer);
						FolderTable folderTable = Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.FolderTable(context.Database);
						searchFolder.SetColumn(context, folderTable.AclTableAndSecurityDescriptor, aclTableAndSDBuffer);
						searchFolder.Save(context);
						if (ExTraceGlobals.RestrictTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							ExTraceGlobals.RestrictTracer.TraceDebug(0L, "Populating the new search folder");
						}
						SearchQueue.InsertIntoSearchQueue(context, context.Mailbox, searchFolder.GetId(context));
						using (IEnumerator<MailboxTaskQueue.TaskStepResult> stepResults = searchFolder.DoSearchPopulation(context, null, shouldTaskContinue))
						{
							while (stepResults.MoveNext())
							{
								MailboxTaskQueue.TaskStepResult taskStepResult = stepResults.Current;
								yield return taskStepResult;
							}
						}
						SearchQueue.RemoveFromSearchQueue(context, context.Mailbox, searchFolder.GetId(context));
					}
				}
			}
			finally
			{
				if (ExTraceGlobals.RestrictTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.RestrictTracer.TraceDebug(0L, "{0} creating and populating materialized restriction search folder {1}: searchFolderId=={2}, flags=={3}, folderAlreadyExists=={4}, instantSearch=={5}", new object[]
					{
						(searchFolder != null) ? "Succeeded" : "Failed",
						displayName,
						(searchFolder != null) ? searchFolder.GetId(context).ToString() : "<null>",
						flags,
						folderAlreadyExists,
						instantSearch
					});
				}
				finishedEvent.Set();
			}
			yield break;
		}

		internal static readonly int AssociatedSearchFolderAgeOutTime = (int)TimeSpan.FromDays(7.0).TotalSeconds;

		private static StorePropTag[] notificationColumns = new StorePropTag[]
		{
			PropTag.Message.Fid,
			PropTag.Message.Mid
		};

		private static StorePropTag[] categorizedViewNotificationColumns = new StorePropTag[]
		{
			PropTag.Message.Fid,
			PropTag.Message.Mid,
			PropTag.Message.CategID,
			PropTag.Message.ContentCount
		};

		internal static StorePropTag[] PropsThatMustBeServicedByFullTextIndex = new StorePropTag[]
		{
			PropTag.Message.BodyUnicode,
			PropTag.Message.RtfCompressed,
			PropTag.Message.BodyHtml,
			PropTag.Message.SearchFullTextBody,
			PropTag.Message.SearchAllIndexedProps,
			PropTag.Message.DisplayName,
			PropTag.Message.SearchSender,
			PropTag.Message.SearchRecipients,
			PropTag.Message.SearchRecipientsTo,
			PropTag.Message.SearchRecipientsCc
		};

		private static Hookable<Action> materializationTestHook = Hookable<Action>.Create(true, null);

		private ExchangeId folderId;

		private ExchangeId materializedRestrictionFolderId;

		private ErrorCodeValue materizationError;

		private bool materializedRestrictionIsInstantSearchFolder;

		private AccessCheckState parentFolderAccessCheckState;

		private bool mailboxScope;

		private bool expandedConversationView;

		private bool prereadExtendedProperties;

		private ViewMessageConfigureFlags configureFlags;

		private bool isCategorizedView;

		private Restriction restriction;

		private int restrictFlags;

		private bool useMaterializedRestriction;

		private bool restrictionMustbeHandledByFullTextIndex;

		private StorePropTag[] columns;

		private MapiViewSetColumnsFlag columnsFlags;

		private SortOrder[] sortOrder;

		private SortTableFlags sortFlags;

		private uint categoryCount;

		private uint expandedCount;
	}
}
