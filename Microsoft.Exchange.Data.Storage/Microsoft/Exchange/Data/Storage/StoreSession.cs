using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Compliance.Logging;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Data.Storage.ActivityLog;
using Microsoft.Exchange.Data.Storage.MailboxRules;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Data.Storage.ResourceHealth;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Exchange.Win32;
using Microsoft.Exchange.WorkloadManagement;
using Microsoft.Mapi;
using Microsoft.Mapi.Security;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class StoreSession : IStoreSession, IDisposeTrackable, IDisposable
	{
		internal static int CurrentServerMajorVersion
		{
			get
			{
				return StoreSession.currentServerMajorVersion;
			}
		}

		internal UnifiedGroupMemberType UnifiedGroupMemberType
		{
			get
			{
				UnifiedGroupMemberType result;
				using (this.CheckDisposed("UnifiedGroupMemberType::get"))
				{
					result = this.unifiedGroupMemberType;
				}
				return result;
			}
		}

		public virtual IActivitySession ActivitySession
		{
			get
			{
				return null;
			}
		}

		public IContentIndexingSession ContentIndexingSession
		{
			get
			{
				IContentIndexingSession result;
				using (this.CheckDisposed("ContentIndexingSession::get"))
				{
					result = this.contentIndexingSession;
				}
				return result;
			}
			set
			{
				using (this.CheckDisposed("ContentIndexingSession::set"))
				{
					this.contentIndexingSession = value;
				}
			}
		}

		public static bool UseRPCContextPoolResiliency
		{
			get
			{
				return StoreSession.useRPCContextPoolResiliency;
			}
			set
			{
				StoreSession.useRPCContextPoolResiliency = value;
			}
		}

		public static bool UseRPCContextPool
		{
			get
			{
				return true;
			}
			set
			{
			}
		}

		protected StoreSession()
		{
		}

		protected StoreSession(CultureInfo cultureInfo, string clientInfoString) : this(cultureInfo, clientInfoString, null)
		{
		}

		protected StoreSession(CultureInfo cultureInfo, string clientInfoString, IBudget budget)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				StorageGlobals.TraceConstructIDisposable(this);
				this.subscriptionsManager = new SubscriptionsManager();
				this.budget = budget;
				this.disposeTracker = this.GetDisposeTracker();
				this.sessionCapabilities = SessionCapabilities.PrimarySessionCapabilities;
				if (cultureInfo == null)
				{
					throw new ArgumentNullException("cultureInfo");
				}
				if (clientInfoString == null)
				{
					throw new ArgumentNullException("clientInfoString");
				}
				if (clientInfoString.Length == 0)
				{
					throw new ArgumentException("clientInfoString has zero length", "clientInfoString");
				}
				this.idConverter = new IdConverter(this);
				this.clientInfoString = clientInfoString;
				this.SessionCultureInfo = cultureInfo;
				this.schema = MailboxSchema.Instance;
				this.clientIPAddress = IPAddress.IPv6Loopback;
				this.serverIPAddress = IPAddress.IPv6Loopback;
				this.InternalInitializeGccProtocolSession();
				this.InitializeADSessionFactory();
				disposeGuard.Success();
			}
		}

		public static bool IsPublicFolderMailbox(int type)
		{
			return type == 1 || type == 2;
		}

		public static bool IsArchiveMailbox(int type)
		{
			return (type & 32) == 32;
		}

		public static bool IsGroupMailbox(int type)
		{
			return type == 4;
		}

		public static bool IsUserMailbox(int type)
		{
			return type == 1;
		}

		public static bool IsTeamSiteMailbox(int type)
		{
			return type == 3;
		}

		public static bool IsSharedMailbox(int type)
		{
			return type == 2;
		}

		protected static CultureInfo GetCultureWithoutInvariant(CultureInfo culture)
		{
			if (culture == CultureInfo.InvariantCulture)
			{
				return null;
			}
			return culture;
		}

		public virtual bool IsRemote
		{
			get
			{
				bool result;
				using (this.CheckDisposed("IsRemote::get"))
				{
					result = false;
				}
				return result;
			}
		}

		public abstract IExchangePrincipal MailboxOwner { get; }

		public bool IsMoveUser
		{
			get
			{
				bool isMoveUser;
				using (this.CheckDisposed("IsMoveUser::get"))
				{
					isMoveUser = this.operationContext.IsMoveUser;
				}
				return isMoveUser;
			}
			protected set
			{
				using (this.CheckDisposed("IsMoveUser::set"))
				{
					this.operationContext.IsMoveUser = value;
				}
			}
		}

		public bool IsMoveSource
		{
			get
			{
				bool isMoveSource;
				using (this.CheckDisposed("IsMoveSource::get"))
				{
					isMoveSource = this.operationContext.IsMoveSource;
				}
				return isMoveSource;
			}
			set
			{
				using (this.CheckDisposed("IsMoveSource::set"))
				{
					this.operationContext.IsMoveSource = value;
				}
			}
		}

		public bool IsXForestMove
		{
			get
			{
				bool isXForestMove;
				using (this.CheckDisposed("IsXForestMove::get"))
				{
					isXForestMove = this.operationContext.IsXForestMove;
				}
				return isXForestMove;
			}
			set
			{
				using (this.CheckDisposed("IsXForestMove::set"))
				{
					this.operationContext.IsXForestMove = value;
				}
			}
		}

		public bool IsOlcMoveDestination
		{
			get
			{
				bool isOlcMoveDestination;
				using (this.CheckDisposed("IsOlcMoveDestination::get"))
				{
					isOlcMoveDestination = this.operationContext.IsOlcMoveDestination;
				}
				return isOlcMoveDestination;
			}
			set
			{
				using (this.CheckDisposed("IsOlcMoveDestination::set"))
				{
					this.operationContext.IsOlcMoveDestination = value;
				}
			}
		}

		public ExchangeOperationContext OperationContext
		{
			get
			{
				ExchangeOperationContext result;
				using (this.CheckDisposed("OperationContext::get"))
				{
					result = this.operationContext;
				}
				return result;
			}
		}

		public bool BlockFolderCreation
		{
			get
			{
				bool result;
				using (this.CheckDisposed("BlockFolderCreation::get"))
				{
					result = this.blockFolderCreation;
				}
				return result;
			}
			set
			{
				using (this.CheckDisposed("BlockFolderCreation::set"))
				{
					this.blockFolderCreation = value;
				}
			}
		}

		public MailboxMoveStage MailboxMoveStage
		{
			get
			{
				MailboxMoveStage result;
				using (this.CheckDisposed("MailboxMoveStage::get"))
				{
					if (!this.IsMoveUser)
					{
						result = MailboxMoveStage.None;
					}
					else
					{
						result = this.mailboxMoveStage;
					}
				}
				return result;
			}
			set
			{
				using (this.CheckDisposed("MailboxMoveStage::set"))
				{
					this.mailboxMoveStage = value;
				}
			}
		}

		public virtual bool IsPublicFolderSession
		{
			get
			{
				bool result;
				using (this.CheckDisposed("IsPublicFolderSession.get"))
				{
					result = false;
				}
				return result;
			}
		}

		public byte[] PersistableTenantPartitionHint
		{
			get
			{
				return this.Mailbox.TryGetProperty(MailboxSchema.PersistableTenantPartitionHint) as byte[];
			}
		}

		protected ObjectAccessGuard CheckDisposed(string methodName)
		{
			if (this.isDisposed)
			{
				StorageGlobals.TraceFailedCheckDisposed(this, methodName);
				throw new ObjectDisposedException(this.ToString());
			}
			return ObjectAccessGuard.Create(this.sessionThreadTracker, methodName);
		}

		protected virtual ObjectAccessGuard CheckObjectState(string methodName)
		{
			return this.CheckDisposed(methodName);
		}

		protected ObjectAccessGuard CreateSessionGuard(string methodName)
		{
			return ObjectAccessGuard.Create(this.sessionThreadTracker, methodName);
		}

		internal IDictionary<StoreObjectId, bool> IsContactFolder
		{
			get
			{
				IDictionary<StoreObjectId, bool> result;
				using (this.CreateSessionGuard("IsContactFolder::get"))
				{
					if (this.isContactFolder == null)
					{
						this.isContactFolder = new Dictionary<StoreObjectId, bool>();
					}
					result = this.isContactFolder;
				}
				return result;
			}
		}

		internal void CheckCapabilities(bool test, string message)
		{
			using (this.CreateSessionGuard("CheckCapabilities"))
			{
				if (!test)
				{
					throw new InvalidOperationException("Session does not have capability " + message);
				}
			}
		}

		public virtual void Dispose()
		{
			using (this.CreateSessionGuard("Dispose"))
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}
		}

		private void Dispose(bool disposing)
		{
			StorageGlobals.TraceDispose(this, this.isDisposed, disposing);
			if (!this.isDisposed)
			{
				if (disposing)
				{
					this.InternalEndGccProtocolSession();
				}
				this.isDisposed = true;
				this.InternalDispose(disposing);
			}
		}

		protected virtual void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.InternalDisposeGccProtocolSession();
				Util.DisposeIfPresent(this.subscriptionsManager);
				this.StopDeadSessionChecking();
				this.SetMailboxStoreObject(null);
				Util.DisposeIfPresent(this.disposeTracker);
			}
		}

		public abstract DisposeTracker GetDisposeTracker();

		public void SuppressDisposeTracker()
		{
			using (this.CreateSessionGuard("SuppressDisposeTracker"))
			{
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Suppress();
				}
			}
		}

		public AggregateOperationResult Delete(DeleteItemFlags deleteFlags, params StoreId[] ids)
		{
			return this.Delete(deleteFlags, false, ids);
		}

		public AggregateOperationResult Delete(DeleteItemFlags deleteFlags, bool returnNewItemIds, params StoreId[] ids)
		{
			AggregateOperationResult result;
			using (this.CheckObjectState("Delete"))
			{
				this.CheckDeleteItemFlags(deleteFlags);
				ExTraceGlobals.SessionTracer.Information((long)this.GetHashCode(), "StoreSession::DeleteItems.");
				List<OccurrenceStoreObjectId> list = new List<OccurrenceStoreObjectId>();
				List<StoreId> list2 = new List<StoreId>();
				Folder.GroupOccurrencesAndObjectIds(ids, list2, list);
				List<GroupOperationResult> list3 = new List<GroupOperationResult>();
				Dictionary<StoreObjectId, List<StoreId>> dictionary = new Dictionary<StoreObjectId, List<StoreId>>();
				this.GroupNonOccurrenceByFolder(list2, dictionary, list3);
				this.ExecuteOperationOnObjects(dictionary, list3, (Folder sourceFolder, StoreId[] sourceObjectIds) => sourceFolder.DeleteObjects(deleteFlags, returnNewItemIds, sourceObjectIds));
				using (List<OccurrenceStoreObjectId>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						OccurrenceStoreObjectId occurrenceId = enumerator.Current;
						Folder.ExecuteGroupOperationAndAggregateResults(list3, new StoreObjectId[]
						{
							occurrenceId
						}, () => this.DeleteCalendarOccurrence(deleteFlags, occurrenceId));
					}
				}
				result = Folder.CreateAggregateOperationResult(list3);
			}
			return result;
		}

		public AggregateOperationResult Move(StoreId destinationFolderId, params StoreId[] ids)
		{
			AggregateOperationResult result;
			using (this.CreateSessionGuard("Move"))
			{
				result = this.Move(this, destinationFolderId, ids);
			}
			return result;
		}

		public AggregateOperationResult Move(StoreSession destinationSession, StoreId destinationFolderId, params StoreId[] ids)
		{
			AggregateOperationResult result;
			using (this.CreateSessionGuard("Move"))
			{
				result = this.Move(destinationSession, destinationFolderId, null, ids);
			}
			return result;
		}

		public AggregateOperationResult Move(StoreSession destinationSession, StoreId destinationFolderId, DeleteItemFlags? deleteFlags, params StoreId[] ids)
		{
			AggregateOperationResult result;
			using (this.CreateSessionGuard("Move"))
			{
				result = this.Move(destinationSession, destinationFolderId, false, deleteFlags, ids);
			}
			return result;
		}

		public AggregateOperationResult Move(StoreSession destinationSession, StoreId destinationFolderId, bool returnNewIds, params StoreId[] ids)
		{
			AggregateOperationResult result;
			using (this.CreateSessionGuard("Move"))
			{
				result = this.Move(destinationSession, destinationFolderId, returnNewIds, null, ids);
			}
			return result;
		}

		public AggregateOperationResult Move(StoreSession destinationSession, StoreId destinationFolderId, bool returnNewIds, DeleteItemFlags? deleteFlags, params StoreId[] ids)
		{
			AggregateOperationResult result;
			using (this.CheckObjectState("Move"))
			{
				ExTraceGlobals.SessionTracer.Information<int>((long)this.GetHashCode(), "StoreSession::Move. HashCode = {0}", this.GetHashCode());
				List<GroupOperationResult> list = new List<GroupOperationResult>();
				Dictionary<StoreObjectId, List<StoreId>> dictionary = new Dictionary<StoreObjectId, List<StoreId>>();
				this.GroupNonOccurrenceByFolder(ids, dictionary, list);
				this.ExecuteOperationOnObjects(dictionary, list, (Folder sourceFolder, StoreId[] sourceObjectIds) => sourceFolder.MoveObjects(destinationSession, destinationFolderId, returnNewIds, deleteFlags, sourceObjectIds));
				result = Folder.CreateAggregateOperationResult(list);
			}
			return result;
		}

		public AggregateOperationResult Copy(StoreId destinationFolderId, params StoreId[] ids)
		{
			AggregateOperationResult result;
			using (this.CreateSessionGuard("Copy"))
			{
				result = this.Copy(this, destinationFolderId, ids);
			}
			return result;
		}

		public AggregateOperationResult Copy(StoreSession destinationSession, StoreId destinationFolderId, params StoreId[] ids)
		{
			AggregateOperationResult result;
			using (this.CreateSessionGuard("Copy"))
			{
				result = this.Copy(destinationSession, destinationFolderId, false, ids);
			}
			return result;
		}

		public AggregateOperationResult Copy(StoreSession destinationSession, StoreId destinationFolderId, bool returnNewIds, params StoreId[] ids)
		{
			AggregateOperationResult result;
			using (this.CheckObjectState("Copy"))
			{
				ExTraceGlobals.SessionTracer.Information<int>((long)this.GetHashCode(), "StoreSession::Copy. HashCode = {0}", this.GetHashCode());
				List<GroupOperationResult> list = new List<GroupOperationResult>();
				Dictionary<StoreObjectId, List<StoreId>> dictionary = new Dictionary<StoreObjectId, List<StoreId>>();
				this.GroupNonOccurrenceByFolder(ids, dictionary, list);
				this.ExecuteOperationOnObjects(dictionary, list, (Folder sourceFolder, StoreId[] sourceObjectIds) => sourceFolder.CopyObjects(destinationSession, destinationFolderId, returnNewIds, sourceObjectIds));
				result = Folder.CreateAggregateOperationResult(list);
			}
			return result;
		}

		public void DoneWithMessage(Item item)
		{
			using (this.CheckObjectState("DoneWithMessage"))
			{
				this.CheckCapabilities(this.Capabilities.CanSend, "CanSend");
				if (item == null)
				{
					ExTraceGlobals.SessionTracer.TraceError((long)this.GetHashCode(), "StoreSession::DoneWithMessage. DoneWithMessage cannot be called on a Null item.");
					throw new ArgumentNullException("item");
				}
				MapiMessage mapiMessage = item.MapiMessage;
				if (mapiMessage == null)
				{
					ExTraceGlobals.SessionTracer.TraceError((long)this.GetHashCode(), "StoreSession::DoneWithMessage. DoneWithMessage cannot be called on a Null MapiMessage.");
					throw new ArgumentException("item");
				}
				MailboxSession mailboxSession = this as MailboxSession;
				StoreObjectId storeObjectId = item.StoreObjectId;
				StoreObjectId sourceFolderId = null;
				StoreObjectId destinationFolderId = null;
				List<StoreObjectId> list = null;
				FolderChangeOperation operation = FolderChangeOperation.Move;
				using (CallbackContext callbackContext = new CallbackContext(this))
				{
					if (mailboxSession != null)
					{
						if (item.GetValueOrDefault<bool>(InternalSchema.DeleteAfterSubmit))
						{
							if (item.GetValueOrDefault<byte[]>(InternalSchema.TargetEntryId) == null)
							{
								operation = FolderChangeOperation.DoneWithMessageDelete;
							}
							else
							{
								operation = FolderChangeOperation.Copy;
							}
							destinationFolderId = null;
						}
						else
						{
							operation = FolderChangeOperation.Move;
							byte[] valueOrDefault = item.GetValueOrDefault<byte[]>(InternalSchema.SentMailEntryId);
							if (valueOrDefault != null)
							{
								destinationFolderId = StoreObjectId.FromProviderSpecificId(valueOrDefault, StoreObjectType.Folder);
							}
							else
							{
								destinationFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.SentItems);
							}
						}
						sourceFolderId = item.ParentId;
						list = new List<StoreObjectId>(1);
						if (storeObjectId != null)
						{
							list.Add(storeObjectId);
						}
						this.OnBeforeFolderChange(operation, FolderChangeOperationFlags.IncludeAll, this, this, sourceFolderId, destinationFolderId, list, callbackContext);
					}
					LocalizedException ex = null;
					try
					{
						bool flag = false;
						try
						{
							if (this != null)
							{
								this.BeginMapiCall();
								this.BeginServerHealthCall();
								flag = true;
							}
							if (StorageGlobals.MapiTestHookBeforeCall != null)
							{
								StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
							}
							mapiMessage.DoneWithMessage();
						}
						catch (MapiPermanentException ex2)
						{
							throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotFinishSubmit, ex2, this, this, "{0}. MapiException = {1}.", new object[]
							{
								string.Format("StoreSession::DoneWithMessage.", new object[0]),
								ex2
							});
						}
						catch (MapiRetryableException ex3)
						{
							throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotFinishSubmit, ex3, this, this, "{0}. MapiException = {1}.", new object[]
							{
								string.Format("StoreSession::DoneWithMessage.", new object[0]),
								ex3
							});
						}
						finally
						{
							try
							{
								if (this != null)
								{
									this.EndMapiCall();
									if (flag)
									{
										this.EndServerHealthCall();
									}
								}
							}
							finally
							{
								if (StorageGlobals.MapiTestHookAfterCall != null)
								{
									StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
								}
							}
						}
					}
					catch (StorageTransientException ex4)
					{
						ex = ex4;
						throw;
					}
					catch (StoragePermanentException ex5)
					{
						ex = ex5;
						throw;
					}
					finally
					{
						if (mailboxSession != null)
						{
							GroupOperationResult result = new GroupOperationResult((ex == null) ? OperationResult.Succeeded : OperationResult.Failed, list.ToArray(), ex);
							this.OnAfterFolderChange(operation, FolderChangeOperationFlags.IncludeAll, this, this, sourceFolderId, destinationFolderId, list, result, callbackContext);
						}
					}
				}
			}
		}

		public GroupOperationResult DeleteAllObjects(DeleteItemFlags flags, StoreId folderId)
		{
			GroupOperationResult result;
			using (this.CheckObjectState("DeleteAllObjects"))
			{
				this.CheckDeleteItemFlags(flags);
				ExTraceGlobals.SessionTracer.Information<int>((long)this.GetHashCode(), "StoreSession::DeleteAllObjects. HashCode = {0}", this.GetHashCode());
				this.CheckDeleteItemFlags(flags);
				if (folderId == null)
				{
					ExTraceGlobals.SessionTracer.TraceError<string>((long)this.GetHashCode(), "StoreSession::DeleteAllObjects. The folder Id cannot be null. Argument = {0}.", "folderId");
					throw new ArgumentNullException(ServerStrings.ExNullParameter("folderId", 2));
				}
				using (Folder folder = Folder.Bind(this, folderId))
				{
					result = folder.DeleteAllObjects(flags);
				}
			}
			return result;
		}

		public abstract void Connect();

		public abstract void Disconnect();

		public void MarkAsRead(bool suppressReadReceipts, params StoreId[] itemIds)
		{
			this.MarkAsRead(suppressReadReceipts, itemIds, false, false);
		}

		public void MarkAsRead(bool suppressReadReceipts, bool suppressNotReadReceipts, params StoreId[] itemIds)
		{
			this.MarkAsRead(suppressReadReceipts, itemIds, false, suppressNotReadReceipts);
		}

		public void MarkAsRead(bool suppressReadReceipts, StoreId[] itemIds, bool throwIfWarning, bool suppressNotReadReceipts = false)
		{
			using (this.CheckObjectState("MarkAsRead"))
			{
				if (itemIds == null)
				{
					ExTraceGlobals.SessionTracer.TraceError<string>((long)this.GetHashCode(), "StoreSession::MarkAsRead. The parameter cannot be null. Parameter = {0}.", "itemIds");
					throw new ArgumentNullException(ServerStrings.ExNullParameter("itemIds", 1));
				}
				ExTraceGlobals.SessionTracer.Information<int>((long)this.GetHashCode(), "StoreSession::MarkAsRead. itemIds.Length = {0}.", itemIds.Length);
				if (itemIds.Length != 0)
				{
					this.InternalMarkReadFlagsByGroup(suppressReadReceipts, true, itemIds, throwIfWarning, suppressNotReadReceipts);
				}
			}
		}

		public void MarkAsUnread(bool suppressReadReceipts, params StoreId[] ids)
		{
			this.MarkAsUnread(suppressReadReceipts, ids, false);
		}

		public void MarkAsUnread(bool suppressReadReceipts, StoreId[] ids, bool throwIfWarning)
		{
			using (this.CheckObjectState("MarkAsUnread"))
			{
				if (ids == null)
				{
					ExTraceGlobals.SessionTracer.TraceError<string>((long)this.GetHashCode(), "StoreSession::MarkAsUnRead. The parameter cannot be null. Parameter = {0}.", "itemIds");
					throw new ArgumentNullException(ServerStrings.ExNullParameter("itemIds", 1));
				}
				ExTraceGlobals.SessionTracer.Information<int>((long)this.GetHashCode(), "StoreSession::MarkAsUnRead. itemIds.Length = {0}.", ids.Length);
				if (ids.Length != 0)
				{
					this.InternalMarkReadFlagsByGroup(suppressReadReceipts, false, ids, throwIfWarning, false);
				}
			}
		}

		public Mailbox Mailbox
		{
			get
			{
				Mailbox mailbox;
				using (this.CheckObjectState("Mailbox::get"))
				{
					if (this.mailboxStoreObject == null)
					{
						throw new ConnectionFailedPermanentException(new LocalizedString(ServerStrings.ExStoreSessionDisconnected + ", mailboxStoreObject = null"));
					}
					mailbox = this.mailboxStoreObject.Mailbox;
				}
				return mailbox;
			}
		}

		IXSOMailbox IStoreSession.Mailbox
		{
			get
			{
				return this.Mailbox;
			}
		}

		public abstract ExTimeZone ExTimeZone { get; set; }

		public abstract string GccResourceIdentifier { get; }

		public virtual ContactFolders ContactFolders
		{
			get
			{
				return null;
			}
		}

		public bool IsE15Session
		{
			get
			{
				bool flag = false;
				bool isE15Store;
				try
				{
					if (this != null)
					{
						this.BeginMapiCall();
						this.BeginServerHealthCall();
						flag = true;
					}
					if (StorageGlobals.MapiTestHookBeforeCall != null)
					{
						StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
					}
					isE15Store = this.Mailbox.MapiStore.IsE15Store;
				}
				catch (MapiPermanentException ex)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.ExErrorInDetectE15Store, ex, this, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("StoreSession::IsE15Session.", new object[0]),
						ex
					});
				}
				catch (MapiRetryableException ex2)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.ExErrorInDetectE15Store, ex2, this, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("StoreSession::IsE15Session.", new object[0]),
						ex2
					});
				}
				finally
				{
					try
					{
						if (this != null)
						{
							this.EndMapiCall();
							if (flag)
							{
								this.EndServerHealthCall();
							}
						}
					}
					finally
					{
						if (StorageGlobals.MapiTestHookAfterCall != null)
						{
							StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
						}
					}
				}
				return isE15Store;
			}
		}

		internal CultureInfo InternalCulture
		{
			get
			{
				CultureInfo sessionCultureInfo;
				using (this.CheckDisposed("InternalCulture::get"))
				{
					sessionCultureInfo = this.SessionCultureInfo;
				}
				return sessionCultureInfo;
			}
		}

		public CultureInfo Culture
		{
			get
			{
				CultureInfo internalCulture;
				using (this.CheckDisposed("Culture::get"))
				{
					this.CheckCapabilities(this.Capabilities.CanHaveCulture, "CanHaveCulture");
					internalCulture = this.InternalCulture;
				}
				return internalCulture;
			}
		}

		public IBudget AccountingObject
		{
			get
			{
				IBudget result;
				using (this.CheckDisposed("AccountingObject::get"))
				{
					result = this.budget;
				}
				return result;
			}
			set
			{
				using (this.CheckDisposed("AccountingObject::set"))
				{
					this.budget = value;
				}
			}
		}

		public virtual CultureInfo PreferedCulture
		{
			get
			{
				CultureInfo internalPreferedCulture;
				using (this.CheckDisposed("PreferedCulture::get"))
				{
					this.CheckCapabilities(this.Capabilities.CanHaveCulture, "CanHaveCulture");
					internalPreferedCulture = this.InternalPreferedCulture;
				}
				return internalPreferedCulture;
			}
		}

		internal virtual CultureInfo InternalPreferedCulture
		{
			get
			{
				CultureInfo sessionCultureInfo;
				using (this.CheckDisposed("InternalPreferedCulture::get"))
				{
					sessionCultureInfo = this.SessionCultureInfo;
				}
				return sessionCultureInfo;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(base.GetType().FullName);
			stringBuilder.AppendLine();
			if (this.isDisposed)
			{
				stringBuilder.AppendLine("disposed");
			}
			else if (this.Mailbox != null)
			{
				using (this.CreateSessionGuard("ToString"))
				{
					string text = "";
					try
					{
						text = (this.Mailbox.TryGetProperty(MailboxSchema.UserName) as string);
					}
					catch (NotInBagPropertyErrorException)
					{
						text = null;
					}
					if (text != null)
					{
						stringBuilder.AppendFormat("User: {0}", text);
						stringBuilder.AppendLine();
					}
				}
			}
			return stringBuilder.ToString();
		}

		public abstract StoreObjectId GetDefaultFolderId(DefaultFolderType defaultFolderType);

		public abstract bool TryFixDefaultFolderId(DefaultFolderType defaultFolderType, out StoreObjectId id);

		internal StoreObjectId SafeGetDefaultFolderId(DefaultFolderType defaultFolderType)
		{
			StoreObjectId result;
			using (this.CreateSessionGuard("SafeGetDefaultFolderId"))
			{
				EnumValidator.ThrowIfInvalid<DefaultFolderType>(defaultFolderType, "defaultFolderType");
				StoreObjectId defaultFolderId = this.GetDefaultFolderId(defaultFolderType);
				if (defaultFolderId == null)
				{
					throw new ObjectNotFoundException(ServerStrings.ExDefaultFolderNotFound(defaultFolderType));
				}
				result = defaultFolderId;
			}
			return result;
		}

		public void SetClientIPEndpoints(IPAddress clientIPAddress, IPAddress serverIPAddress)
		{
			using (this.CreateSessionGuard("SetClientIPEndpoints"))
			{
				this.clientIPAddress = clientIPAddress;
				this.serverIPAddress = serverIPAddress;
				this.InternalLogIpEndpoints(clientIPAddress, serverIPAddress);
				if (ExTraceGlobals.SessionTracer.IsTraceEnabled(TraceType.PfdTrace))
				{
					ExTraceGlobals.SessionTracer.TracePfd((long)this.GetHashCode(), "StoreSession::SetClientInfo. ClientIP={0}; ServerIP={1}; ClientMachine={2}; ClientProcess={3}; ClientVersion={4}", new object[]
					{
						this.clientIPAddress,
						this.serverIPAddress,
						this.clientMachineName,
						this.clientProcessName,
						this.clientVersion
					});
				}
			}
		}

		public bool IsInBackoffState
		{
			get
			{
				bool result;
				using (this.CheckObjectState("IsInBackoffState::get"))
				{
					result = this.Mailbox.MapiStore.BackoffNow();
				}
				return result;
			}
		}

		public StoreSession.IItemBinder ItemBinder
		{
			get
			{
				StoreSession.IItemBinder result;
				using (this.CheckDisposed("ItemBinder::get"))
				{
					result = this.itemBinder;
				}
				return result;
			}
			set
			{
				using (this.CheckDisposed("ItemBinder::set"))
				{
					this.itemBinder = value;
				}
			}
		}

		public IdConverter IdConverter
		{
			get
			{
				IdConverter result;
				using (this.CheckObjectState("IdConverter::get"))
				{
					result = this.idConverter;
				}
				return result;
			}
		}

		public ICollection<string> SupportedRoutingTypes
		{
			get
			{
				ICollection<string> result;
				using (this.CheckDisposed("SupportedRoutingTypes::get"))
				{
					if (this.supportedRoutingTypes == null)
					{
						this.supportedRoutingTypes = this.InternalGetSupportedRoutingTypes();
					}
					result = this.supportedRoutingTypes;
				}
				return result;
			}
		}

		public SessionCapabilities Capabilities
		{
			get
			{
				SessionCapabilities result;
				using (this.CheckDisposed("Capabilities::get"))
				{
					result = this.sessionCapabilities;
				}
				return result;
			}
			internal set
			{
				using (this.CreateSessionGuard("Capabilities::set"))
				{
					this.sessionCapabilities = value;
				}
			}
		}

		public SpoolerManager SpoolerManager
		{
			get
			{
				SpoolerManager result;
				using (this.CheckObjectState("SpoolerManager::get"))
				{
					if (this.spoolerManager == null)
					{
						this.spoolerManager = new SpoolerManager(this);
					}
					result = this.spoolerManager;
				}
				return result;
			}
		}

		public virtual bool CheckSubmissionQuota(int recipientCount)
		{
			bool result;
			using (this.CheckDisposed("CheckSubmissionQuota"))
			{
				result = true;
			}
			return result;
		}

		public string UserLegacyDN
		{
			get
			{
				string result;
				using (this.CheckDisposed("UserLegacyDN::get"))
				{
					result = this.userLegacyDn;
				}
				return result;
			}
		}

		public void AbortSubmit(StoreObjectId submittedId)
		{
			using (this.CheckDisposed("AbortSubmit"))
			{
				MapiStore mapiStore = this.Mailbox.MapiStore;
				bool flag = false;
				try
				{
					if (this != null)
					{
						this.BeginMapiCall();
						this.BeginServerHealthCall();
						flag = true;
					}
					if (StorageGlobals.MapiTestHookBeforeCall != null)
					{
						StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
					}
					mapiStore.AbortSubmit(submittedId.ProviderLevelItemId);
				}
				catch (MapiPermanentException ex)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotSubmitMessage, ex, this, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("StoreSession.AbortSubmit()", new object[0]),
						ex
					});
				}
				catch (MapiRetryableException ex2)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotSubmitMessage, ex2, this, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("StoreSession.AbortSubmit()", new object[0]),
						ex2
					});
				}
				finally
				{
					try
					{
						if (this != null)
						{
							this.EndMapiCall();
							if (flag)
							{
								this.EndServerHealthCall();
							}
						}
					}
					finally
					{
						if (StorageGlobals.MapiTestHookAfterCall != null)
						{
							StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
						}
					}
				}
			}
		}

		public byte[] ReadPerUserInformation(byte[] longTermId, bool wantIfChanged, uint dataOffset, ushort maxDataSize, out bool hasFinished)
		{
			byte[] result;
			using (this.CheckObjectState("ReadPerUserInformation"))
			{
				bool flag = false;
				try
				{
					if (this != null)
					{
						this.BeginMapiCall();
						this.BeginServerHealthCall();
						flag = true;
					}
					if (StorageGlobals.MapiTestHookBeforeCall != null)
					{
						StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
					}
					this.Mailbox.MapiStore.ReadPerUserInformation(longTermId, wantIfChanged, (int)dataOffset, (int)maxDataSize, out hasFinished, out result);
				}
				catch (MapiPermanentException ex)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotReadPerUserInformation, ex, this, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("StoreSession.ReadPerUserInformation", new object[0]),
						ex
					});
				}
				catch (MapiRetryableException ex2)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotReadPerUserInformation, ex2, this, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("StoreSession.ReadPerUserInformation", new object[0]),
						ex2
					});
				}
				finally
				{
					try
					{
						if (this != null)
						{
							this.EndMapiCall();
							if (flag)
							{
								this.EndServerHealthCall();
							}
						}
					}
					finally
					{
						if (StorageGlobals.MapiTestHookAfterCall != null)
						{
							StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
						}
					}
				}
			}
			return result;
		}

		public void WritePerUserInformation(byte[] longTermId, bool hasFinished, uint dataOffset, byte[] data, Guid? replicaGuid)
		{
			using (this.CheckObjectState("WritePerUserInformation"))
			{
				bool flag = false;
				try
				{
					if (this != null)
					{
						this.BeginMapiCall();
						this.BeginServerHealthCall();
						flag = true;
					}
					if (StorageGlobals.MapiTestHookBeforeCall != null)
					{
						StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
					}
					this.Mailbox.MapiStore.WritePerUserInformation(longTermId, hasFinished, (int)dataOffset, data, (replicaGuid != null) ? replicaGuid.Value : Guid.Empty);
				}
				catch (MapiPermanentException ex)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotWritePerUserInformation, ex, this, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("StoreSession.WritePerUserInformation", new object[0]),
						ex
					});
				}
				catch (MapiRetryableException ex2)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotWritePerUserInformation, ex2, this, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("StoreSession.WritePerUserInformation", new object[0]),
						ex2
					});
				}
				finally
				{
					try
					{
						if (this != null)
						{
							this.EndMapiCall();
							if (flag)
							{
								this.EndServerHealthCall();
							}
						}
					}
					finally
					{
						if (StorageGlobals.MapiTestHookAfterCall != null)
						{
							StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
						}
					}
				}
			}
		}

		public uint GetEffectiveRights(byte[] addressBookId, StoreObjectId folderId)
		{
			uint effectiveRights;
			using (this.CheckObjectState("GetEffectiveRights"))
			{
				if (!AddressBookEntryId.IsAddressBookEntryId(addressBookId))
				{
					throw new ArgumentException("addressBookId");
				}
				bool flag = false;
				try
				{
					if (this != null)
					{
						this.BeginMapiCall();
						this.BeginServerHealthCall();
						flag = true;
					}
					if (StorageGlobals.MapiTestHookBeforeCall != null)
					{
						StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
					}
					effectiveRights = this.Mailbox.MapiStore.GetEffectiveRights(addressBookId, folderId.ProviderLevelItemId);
				}
				catch (MapiPermanentException ex)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetEffectiveRights, ex, this, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("StoreSession.GetEffectiveRights", new object[0]),
						ex
					});
				}
				catch (MapiRetryableException ex2)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetEffectiveRights, ex2, this, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("StoreSession.GetEffectiveRights", new object[0]),
						ex2
					});
				}
				finally
				{
					try
					{
						if (this != null)
						{
							this.EndMapiCall();
							if (flag)
							{
								this.EndServerHealthCall();
							}
						}
					}
					finally
					{
						if (StorageGlobals.MapiTestHookAfterCall != null)
						{
							StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
						}
					}
				}
			}
			return effectiveRights;
		}

		public void CheckForNotifications()
		{
			using (this.CheckObjectState("CheckForNotifications"))
			{
				bool flag = false;
				try
				{
					if (this != null)
					{
						this.BeginMapiCall();
						this.BeginServerHealthCall();
						flag = true;
					}
					if (StorageGlobals.MapiTestHookBeforeCall != null)
					{
						StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
					}
					this.Mailbox.MapiStore.CheckForNotifications();
				}
				catch (MapiPermanentException ex)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCheckForNotifications, ex, this, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("StoreSession.CheckForNotifications", new object[0]),
						ex
					});
				}
				catch (MapiRetryableException ex2)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCheckForNotifications, ex2, this, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("StoreSession.CheckForNotifications", new object[0]),
						ex2
					});
				}
				finally
				{
					try
					{
						if (this != null)
						{
							this.EndMapiCall();
							if (flag)
							{
								this.EndServerHealthCall();
							}
						}
					}
					finally
					{
						if (StorageGlobals.MapiTestHookAfterCall != null)
						{
							StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
						}
					}
				}
			}
		}

		public void ExecuteWithInternalAccessElevation(Action actionDelegate)
		{
			using (this.CheckObjectState("ExecuteWithInternalAccessElevation"))
			{
				StoreSession storeSession = null;
				bool flag = false;
				try
				{
					if (storeSession != null)
					{
						storeSession.BeginMapiCall();
						storeSession.BeginServerHealthCall();
						flag = true;
					}
					if (StorageGlobals.MapiTestHookBeforeCall != null)
					{
						StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
					}
					this.Mailbox.MapiStore.ExecuteWithInternalAccess(actionDelegate);
				}
				catch (MapiPermanentException ex)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotExecuteWithInternalAccess, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("StoreSession.ExecuteWithInternalAccessElevation", new object[0]),
						ex
					});
				}
				catch (MapiRetryableException ex2)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotExecuteWithInternalAccess, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("StoreSession.ExecuteWithInternalAccessElevation", new object[0]),
						ex2
					});
				}
				finally
				{
					try
					{
						if (storeSession != null)
						{
							storeSession.EndMapiCall();
							if (flag)
							{
								storeSession.EndServerHealthCall();
							}
						}
					}
					finally
					{
						if (StorageGlobals.MapiTestHookAfterCall != null)
						{
							StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
						}
					}
				}
			}
		}

		public abstract ADSessionSettings GetADSessionSettings();

		protected void StartDeadSessionChecking()
		{
			this.isDead = false;
			bool flag = false;
			try
			{
				if (this != null)
				{
					this.BeginMapiCall();
					this.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				this.tickleNotificationHandle = this.mailboxStoreObject.Mailbox.MapiStore.Advise(null, AdviseFlags.Extended, new MapiNotificationHandler(this.OnTickle), NotificationCallbackMode.Async, (MapiNotificationClientFlags)0);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.ExCannotStartDeadSessionChecking, ex, this, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("StoreSession::StartDeadSessionChecking.", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.ExCannotStartDeadSessionChecking, ex2, this, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("StoreSession::StartDeadSessionChecking.", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (this != null)
					{
						this.EndMapiCall();
						if (flag)
						{
							this.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
			bool flag2 = false;
			try
			{
				if (this != null)
				{
					this.BeginMapiCall();
					this.BeginServerHealthCall();
					flag2 = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				this.droppedNotificationHandle = this.mailboxStoreObject.Mailbox.MapiStore.Advise(null, AdviseFlags.ConnectionDropped, new MapiNotificationHandler(this.OnDroppedNotify), NotificationCallbackMode.Async, (MapiNotificationClientFlags)0);
			}
			catch (MapiPermanentException ex3)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.ExCannotStartDeadSessionChecking, ex3, this, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("StoreSession::StartDeadSessionChecking.", new object[0]),
					ex3
				});
			}
			catch (MapiRetryableException ex4)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.ExCannotStartDeadSessionChecking, ex4, this, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("StoreSession::StartDeadSessionChecking.", new object[0]),
					ex4
				});
			}
			finally
			{
				try
				{
					if (this != null)
					{
						this.EndMapiCall();
						if (flag2)
						{
							this.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
		}

		protected bool StopDeadSessionChecking()
		{
			bool result = false;
			try
			{
				if (!this.isDead && this.mailboxStoreObject != null && this.mailboxStoreObject.Mailbox != null && this.mailboxStoreObject.Mailbox.MapiStore != null && !this.mailboxStoreObject.Mailbox.MapiStore.IsDead)
				{
					if (this.droppedNotificationHandle != null)
					{
						this.mailboxStoreObject.Mailbox.MapiStore.Unadvise(this.droppedNotificationHandle);
						result = true;
					}
					if (this.tickleNotificationHandle != null)
					{
						this.mailboxStoreObject.Mailbox.MapiStore.Unadvise(this.tickleNotificationHandle);
						result = true;
					}
				}
				this.droppedNotificationHandle = null;
				this.tickleNotificationHandle = null;
			}
			catch (MapiPermanentException arg)
			{
				ExTraceGlobals.SessionTracer.Information<MapiPermanentException>((long)this.GetHashCode(), "StoreSession::StopDeadSessionChecking, MapiPermanentException {0} calling Unadvise, ignored.", arg);
			}
			catch (MapiRetryableException arg2)
			{
				ExTraceGlobals.SessionTracer.Information<MapiRetryableException>((long)this.GetHashCode(), "StoreSession::StopDeadSessionChecking, mapiRetryableException {0} calling Unadvise, ignored.", arg2);
			}
			return result;
		}

		public abstract Guid MdbGuid { get; }

		public abstract Guid MailboxGuid { get; }

		public abstract string ServerFullyQualifiedDomainName { get; }

		public abstract string DisplayAddress { get; }

		public abstract OrganizationId OrganizationId { get; }

		protected void ExecuteOperationOnObjects(Dictionary<StoreObjectId, List<StoreId>> groupByFolder, List<GroupOperationResult> groupOperationResultList, StoreSession.ActOnObjectsDelegate actOnObjectsDelegate)
		{
			foreach (KeyValuePair<StoreObjectId, List<StoreId>> keyValuePair in groupByFolder)
			{
				try
				{
					using (Folder folder = Folder.Bind(this, keyValuePair.Key))
					{
						AggregateOperationResult aggregateOperationResult = actOnObjectsDelegate(folder, keyValuePair.Value.ToArray());
						groupOperationResultList.AddRange(aggregateOperationResult.GroupOperationResults);
					}
				}
				catch (ObjectNotFoundException storageException)
				{
					ExTraceGlobals.SessionTracer.TraceError<StoreObjectId>((long)this.GetHashCode(), "StoreSession::CopyItemEx, folderNotFound = {0}", keyValuePair.Key);
					StoreObjectId[] objectIds = Folder.StoreIdsToStoreObjectIds(keyValuePair.Value.ToArray());
					GroupOperationResult item = new GroupOperationResult(OperationResult.Failed, objectIds, storageException);
					groupOperationResultList.Add(item);
				}
			}
		}

		protected void GroupNonOccurrenceByFolder(IList<StoreId> objectIds, Dictionary<StoreObjectId, List<StoreId>> groupedObjects, List<GroupOperationResult> errors)
		{
			for (int i = 0; i < objectIds.Count; i++)
			{
				StoreObjectId storeObjectId = StoreId.GetStoreObjectId(objectIds[i]);
				this.GroupNonOccurrenceByFolder(storeObjectId, groupedObjects, errors);
			}
		}

		protected abstract MapiStore ForceOpen(MapiStore linkedStore);

		internal bool IsDisposed
		{
			get
			{
				bool result;
				using (this.CreateSessionGuard("IsDisposed"))
				{
					result = this.isDisposed;
				}
				return result;
			}
		}

		internal Schema Schema
		{
			get
			{
				Schema result;
				using (this.CheckObjectState("Schema::get"))
				{
					result = this.schema;
				}
				return result;
			}
		}

		public string ClientInfoString
		{
			get
			{
				string result;
				using (this.CreateSessionGuard("ClientInfoString::get"))
				{
					result = this.clientInfoString;
				}
				return result;
			}
		}

		public IPAddress ClientIPAddress
		{
			get
			{
				IPAddress result;
				using (this.CreateSessionGuard("ClientIPAddress::get"))
				{
					result = this.clientIPAddress;
				}
				return result;
			}
		}

		public IPAddress ServerIPAddress
		{
			get
			{
				IPAddress result;
				using (this.CreateSessionGuard("ServerIPAddress::get"))
				{
					result = this.serverIPAddress;
				}
				return result;
			}
		}

		public long ClientVersion
		{
			get
			{
				long result;
				using (this.CreateSessionGuard("ClientVersion::get"))
				{
					result = this.clientVersion;
				}
				return result;
			}
			set
			{
				using (this.CreateSessionGuard("ClientVersion::set"))
				{
					this.clientVersion = value;
				}
			}
		}

		public string ClientProcessName
		{
			get
			{
				string result;
				using (this.CreateSessionGuard("ClientProcessName::get"))
				{
					result = this.clientProcessName;
				}
				return result;
			}
			set
			{
				using (this.CreateSessionGuard("ClientProcessName::set"))
				{
					this.clientProcessName = value;
				}
			}
		}

		public string ClientMachineName
		{
			get
			{
				string result;
				using (this.CreateSessionGuard("ClientMachineName::get"))
				{
					result = this.clientMachineName;
				}
				return result;
			}
			set
			{
				using (this.CreateSessionGuard("ClientMachineName::set"))
				{
					this.clientMachineName = value;
				}
			}
		}

		public LogonType LogonType
		{
			get
			{
				LogonType result;
				using (this.CheckDisposed("LogonType::get"))
				{
					result = this.logonType;
				}
				return result;
			}
			internal set
			{
				using (this.CheckDisposed("LogonType::set"))
				{
					this.logonType = value;
				}
			}
		}

		public string MappingSignature
		{
			get
			{
				string result;
				using (this.CheckDisposed("MappingSignature::get"))
				{
					result = this.mappingSignature;
				}
				return result;
			}
			internal set
			{
				using (this.CheckDisposed("MappingSignature::set"))
				{
					this.mappingSignature = value;
				}
			}
		}

		internal NamedPropMap NamedPropertyResolutionCache
		{
			get
			{
				NamedPropMap result;
				using (this.CheckDisposed("NamedPropertyResolutionCache::get"))
				{
					if (this.propResolutionCache == null || !this.propResolutionCache.IsAlive)
					{
						NamedPropMap namedPropMap = NamedPropMapCache.Default.GetMapping(this.mappingSignature);
						this.propResolutionCache = new WeakReference(namedPropMap);
						result = namedPropMap;
					}
					else
					{
						NamedPropMap namedPropMap = this.propResolutionCache.Target as NamedPropMap;
						if (namedPropMap == null)
						{
							this.propResolutionCache = null;
							result = this.NamedPropertyResolutionCache;
						}
						else
						{
							result = namedPropMap;
						}
					}
				}
				return result;
			}
		}

		public bool IsConnected
		{
			get
			{
				bool result;
				using (this.CheckDisposed("IsConnected::get"))
				{
					result = this.isConnected;
				}
				return result;
			}
			protected set
			{
				this.isConnected = value;
			}
		}

		public bool IsDead
		{
			get
			{
				bool isConnectionDead;
				using (this.CheckDisposed("IsDead::get"))
				{
					isConnectionDead = this.IsConnectionDead;
				}
				return isConnectionDead;
			}
			protected set
			{
				using (this.CheckDisposed("IsDead::set"))
				{
					this.isDead = value;
				}
			}
		}

		internal void PlayDead()
		{
			using (this.CreateSessionGuard("PlayDead"))
			{
				this.IsDead = true;
			}
		}

		public object Identity
		{
			get
			{
				object result;
				using (this.CreateSessionGuard("Identity::get"))
				{
					result = this.identity;
				}
				return result;
			}
		}

		internal OpenStoreFlag StoreFlag
		{
			get
			{
				OpenStoreFlag result;
				using (this.CreateSessionGuard("StoreFlag::get"))
				{
					result = this.storeFlag;
				}
				return result;
			}
			set
			{
				using (this.CreateSessionGuard("StoreFlag::set"))
				{
					this.storeFlag = value;
				}
			}
		}

		public int PreferredInternetCodePageForShiftJis
		{
			get
			{
				int result;
				using (this.CreateSessionGuard("PreferredInternetCodePageForShiftJis::get"))
				{
					result = this.preferredInternetCodePageForShiftJis;
				}
				return result;
			}
			protected set
			{
				using (this.CreateSessionGuard("PreferredInternetCodePageForShiftJis::set"))
				{
					this.preferredInternetCodePageForShiftJis = value;
				}
			}
		}

		public int RequiredCoverage
		{
			get
			{
				int result;
				using (this.CreateSessionGuard("RequiredCoverage::get"))
				{
					result = this.requiredCoverage;
				}
				return result;
			}
			protected set
			{
				using (this.CreateSessionGuard("RequiredCoverage::set"))
				{
					this.requiredCoverage = value;
				}
			}
		}

		internal GenericIdentity AuxiliaryIdentity
		{
			get
			{
				GenericIdentity result;
				using (this.CreateSessionGuard("AuxiliaryIdentity::get"))
				{
					result = this.auxiliaryIdentity;
				}
				return result;
			}
		}

		internal virtual void CheckDeleteItemFlags(DeleteItemFlags flags)
		{
			using (this.CreateSessionGuard("CheckDeleteItemFlags"))
			{
				EnumValidator.ThrowIfInvalid<DeleteItemFlags>(flags, "flags");
			}
		}

		internal virtual void CheckSystemFolderAccess(StoreObjectId id)
		{
			using (this.CreateSessionGuard("CheckSystemFolderAccess"))
			{
			}
		}

		internal GroupOperationResult DeleteCalendarOccurrence(DeleteItemFlags flags, OccurrenceStoreObjectId itemId)
		{
			GroupOperationResult result;
			using (this.CreateSessionGuard("DeleteCalendarOccurrence"))
			{
				ExTraceGlobals.SessionTracer.Information<DeleteItemFlags, OccurrenceStoreObjectId>((long)this.GetHashCode(), "StoreSession::DeleteCalendarOccurrence. flags={0}; itemId = {1}", flags, itemId);
				using (CalendarItem calendarItem = CalendarItem.Bind(this, itemId.GetMasterStoreObjectId()))
				{
					calendarItem.OpenAsReadWrite();
					try
					{
						calendarItem.DeleteOccurrence(itemId, flags);
					}
					catch (LastOccurrenceDeletionException)
					{
						ExTraceGlobals.StorageTracer.TraceDebug((long)this.GetHashCode(), "StoreSession::DeleteCalendarOccurrence, Delete the master because the recurrence is Empty.");
						using (Folder folder = Folder.Bind(this, calendarItem.ParentId))
						{
							AggregateOperationResult aggregateOperationResult = folder.DeleteObjects(flags, new StoreId[]
							{
								calendarItem.Id.ObjectId
							});
							if (aggregateOperationResult.GroupOperationResults.Length > 0)
							{
								return aggregateOperationResult.GroupOperationResults[0];
							}
							return new GroupOperationResult(OperationResult.Succeeded, new StoreObjectId[]
							{
								itemId
							}, null);
						}
					}
					catch (RecurrenceException storageException)
					{
						return new GroupOperationResult(OperationResult.Failed, new StoreObjectId[]
						{
							itemId
						}, storageException);
					}
					ConflictResolutionResult conflictResolutionResult = calendarItem.Save(SaveMode.ResolveConflicts);
					if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
					{
						ExTraceGlobals.StorageTracer.TraceError<OccurrenceStoreObjectId, SaveResult>((long)this.GetHashCode(), "StoreSession::DeleteCalendarOccurrence, Cannot save the master. ItemId = {0}, conflictResolutionResult = {1}", itemId, conflictResolutionResult.SaveStatus);
						result = new GroupOperationResult(OperationResult.Failed, new StoreObjectId[]
						{
							itemId
						}, new SaveConflictException(ServerStrings.ExSaveFailedBecauseOfConflicts(null), conflictResolutionResult));
					}
					else
					{
						result = new GroupOperationResult(OperationResult.Succeeded, new StoreObjectId[]
						{
							itemId
						}, null);
					}
				}
			}
			return result;
		}

		internal MapiProp GetMapiProp(StoreObjectId id)
		{
			MapiProp mapiProp;
			using (this.CreateSessionGuard("GetMapiProp"))
			{
				mapiProp = this.GetMapiProp(id, OpenEntryFlags.BestAccess | OpenEntryFlags.DeferredErrors);
			}
			return mapiProp;
		}

		internal virtual MapiProp GetMapiProp(StoreObjectId id, OpenEntryFlags flags)
		{
			MapiProp result;
			using (this.CheckObjectState("GetMapiProp"))
			{
				bool flag = false;
				try
				{
					if (this != null)
					{
						this.BeginMapiCall();
						this.BeginServerHealthCall();
						flag = true;
					}
					if (StorageGlobals.MapiTestHookBeforeCall != null)
					{
						StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
					}
					result = (MapiProp)this.Mailbox.MapiStore.OpenEntry(id.ProviderLevelItemId, flags);
				}
				catch (MapiPermanentException ex)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.ExInvalidItemId, ex, this, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("StoreSession::GetMapiProp. id = {0}.", id),
						ex
					});
				}
				catch (MapiRetryableException ex2)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.ExInvalidItemId, ex2, this, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("StoreSession::GetMapiProp. id = {0}.", id),
						ex2
					});
				}
				finally
				{
					try
					{
						if (this != null)
						{
							this.EndMapiCall();
							if (flag)
							{
								this.EndServerHealthCall();
							}
						}
					}
					finally
					{
						if (StorageGlobals.MapiTestHookAfterCall != null)
						{
							StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
						}
					}
				}
			}
			return result;
		}

		protected void SetMailboxStoreObject(MailboxStoreObject mailboxStoreObject)
		{
			using (this.CreateSessionGuard("SetMailboxStoreObject"))
			{
				if (this.mailboxStoreObject != mailboxStoreObject)
				{
					if (this.mailboxStoreObject != null)
					{
						if (this.droppedNotificationHandle != null)
						{
							return;
						}
						ExTraceGlobals.SessionTracer.TraceDebug<MailboxStoreObject, MailboxStoreObject>((long)this.GetHashCode(), "StoreSession SetMailboxStoreObject. We are disposing existing mailboxObject and set the new one. Existing = {0}, New = {1}.", this.mailboxStoreObject, mailboxStoreObject);
						this.mailboxStoreObject.Dispose();
						this.mailboxStoreObject = null;
					}
					this.mailboxStoreObject = mailboxStoreObject;
				}
			}
		}

		public StoreObjectId GetParentFolderId(StoreObjectId objectId)
		{
			StoreObjectId result;
			using (this.CreateSessionGuard("GetParentFolderId"))
			{
				byte[] entryId = null;
				bool flag = false;
				try
				{
					if (this != null)
					{
						this.BeginMapiCall();
						this.BeginServerHealthCall();
						flag = true;
					}
					if (StorageGlobals.MapiTestHookBeforeCall != null)
					{
						StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
					}
					entryId = this.Mailbox.MapiStore.GetParentEntryId(objectId.ProviderLevelItemId);
				}
				catch (MapiPermanentException ex)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetParentId, ex, this, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("StoreSession::GetParentFolderId. Object id = {0}.", objectId),
						ex
					});
				}
				catch (MapiRetryableException ex2)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetParentId, ex2, this, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("StoreSession::GetParentFolderId. Object id = {0}.", objectId),
						ex2
					});
				}
				finally
				{
					try
					{
						if (this != null)
						{
							this.EndMapiCall();
							if (flag)
							{
								this.EndServerHealthCall();
							}
						}
					}
					finally
					{
						if (StorageGlobals.MapiTestHookAfterCall != null)
						{
							StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
						}
					}
				}
				result = StoreObjectId.FromProviderSpecificId(entryId);
			}
			return result;
		}

		internal static bool TestRequestExchangeRpcServer
		{
			get
			{
				return StoreSession.testRequestExchangeRpcServer;
			}
			set
			{
				StoreSession.testRequestExchangeRpcServer = value;
			}
		}

		internal void TestSetLogCallback(ILogChanges callback)
		{
			using (this.CreateSessionGuard("TestSetLogCallback"))
			{
				this.testLogCallback = callback;
			}
		}

		internal virtual bool OnBeforeItemChange(ItemChangeOperation operation, StoreSession session, StoreId itemId, CoreItem item, CallbackContext callbackContext)
		{
			bool result;
			using (this.CreateSessionGuard("OnBeforeItemChange"))
			{
				if (this.contentIndexingSession != null)
				{
					this.contentIndexingSession.OnBeforeItemChange(operation, item);
				}
				if (this.testLogCallback != null)
				{
					result = this.testLogCallback.OnBeforeItemChange(operation, session, itemId, item);
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		internal virtual void OnAfterItemChange(ItemChangeOperation operation, StoreSession session, StoreId itemId, CoreItem item, ConflictResolutionResult result, CallbackContext callbackContext)
		{
			using (this.CreateSessionGuard("OnAfterItemChange"))
			{
				if (this.testLogCallback != null)
				{
					this.testLogCallback.OnAfterItemChange(operation, session, itemId, item, result);
				}
			}
		}

		internal virtual bool OnBeforeItemSave(ItemChangeOperation operation, StoreSession session, StoreId itemId, CoreItem item, CallbackContext callbackContext)
		{
			bool result;
			using (this.CreateSessionGuard("OnBeforeItemSave"))
			{
				if (this.testLogCallback != null)
				{
					result = this.testLogCallback.OnBeforeItemSave(operation, session, itemId, item);
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		internal virtual void OnAfterItemSave(ItemChangeOperation operation, StoreSession session, StoreId itemId, CoreItem item, ConflictResolutionResult result, CallbackContext callbackContext)
		{
			using (this.CreateSessionGuard("OnAfterItemSave"))
			{
				if (this.testLogCallback != null)
				{
					this.testLogCallback.OnAfterItemSave(operation, session, itemId, item, result);
				}
			}
		}

		internal virtual bool OnBeforeFolderChange(FolderChangeOperation operation, FolderChangeOperationFlags flags, StoreSession sourceSession, StoreSession destinationSession, StoreObjectId sourceFolderId, StoreObjectId destinationFolderId, ICollection<StoreObjectId> itemIds, CallbackContext callbackContext)
		{
			bool result;
			using (this.CreateSessionGuard("OnBeforeFolderChange"))
			{
				if (this.testLogCallback != null)
				{
					result = this.testLogCallback.OnBeforeFolderChange(operation, flags, sourceSession, destinationSession, sourceFolderId, destinationFolderId, itemIds);
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		internal virtual void OnAfterFolderChange(FolderChangeOperation operation, FolderChangeOperationFlags flags, StoreSession sourceSession, StoreSession destinationSession, StoreObjectId sourceFolderId, StoreObjectId destinationFolderId, ICollection<StoreObjectId> itemIds, GroupOperationResult result, CallbackContext callbackContext)
		{
			using (this.CreateSessionGuard("OnAfterFolderChange"))
			{
				if (this.testLogCallback != null)
				{
					this.testLogCallback.OnAfterFolderChange(operation, flags, sourceSession, destinationSession, sourceFolderId, destinationFolderId, itemIds, result);
				}
			}
		}

		internal virtual void OnBeforeFolderBind(StoreObjectId folderId, CallbackContext callbackContext)
		{
			using (this.CreateSessionGuard("OnBeforeFolderBind"))
			{
				if (this.testLogCallback != null)
				{
					this.testLogCallback.OnBeforeFolderBind(this, folderId);
				}
			}
		}

		internal virtual void OnAfterFolderBind(StoreObjectId folderId, CoreFolder folder, bool success, CallbackContext callbackContext)
		{
			using (this.CreateSessionGuard("OnAfterFolderBind"))
			{
				if (this.testLogCallback != null)
				{
					this.testLogCallback.OnAfterFolderBind(this, folderId, folder, success);
				}
			}
		}

		internal virtual GroupOperationResult GetCallbackResults()
		{
			GroupOperationResult result;
			using (this.CreateSessionGuard("GetCallbackResults"))
			{
				result = null;
			}
			return result;
		}

		internal SubscriptionsManager SubscriptionsManager
		{
			get
			{
				SubscriptionsManager result;
				using (this.CreateSessionGuard("SubscriptionsManager::get"))
				{
					result = this.subscriptionsManager;
				}
				return result;
			}
		}

		internal virtual void ValidateOperation(FolderChangeOperation folderOperation, StoreObjectId folderId)
		{
			using (this.CheckObjectState("ValidateOperation"))
			{
			}
		}

		internal virtual bool IsValidOperation(ICoreObject coreObject, PropertyDefinition property, out PropertyErrorCode? error)
		{
			bool result;
			using (this.CheckDisposed("IsValidOperation"))
			{
				error = null;
				result = true;
			}
			return result;
		}

		private void InternalMarkReadFlagsByGroup(bool suppressReadReceipts, bool isMarkAsRead, StoreId[] itemIds, bool throwIfWarning, bool suppressNotReadReceipts = false)
		{
			Dictionary<StoreObjectId, List<StoreId>> dictionary = new Dictionary<StoreObjectId, List<StoreId>>();
			List<GroupOperationResult> errors = new List<GroupOperationResult>();
			List<StoreId> list = new List<StoreId>();
			List<OccurrenceStoreObjectId> sourceOccurrenceIdList = new List<OccurrenceStoreObjectId>();
			Folder.GroupOccurrencesAndObjectIds(itemIds, list, sourceOccurrenceIdList);
			this.GroupNonOccurrenceByFolder(list, dictionary, errors);
			foreach (KeyValuePair<StoreObjectId, List<StoreId>> keyValuePair in dictionary)
			{
				using (Folder folder = Folder.Bind(this, keyValuePair.Key))
				{
					folder.InternalSetReadFlags(suppressReadReceipts, isMarkAsRead, keyValuePair.Value.ToArray(), throwIfWarning, suppressNotReadReceipts);
				}
			}
		}

		private void GroupNonOccurrenceByFolder(StoreId itemId, Dictionary<StoreObjectId, List<StoreId>> groupedObjects, List<GroupOperationResult> errors)
		{
			if (itemId == null)
			{
				ExTraceGlobals.SessionTracer.TraceError((long)this.GetHashCode(), "StoreSession::GroupItemsByFolderHelper. The in itemId cannot be null.");
				throw new ArgumentException(ServerStrings.ExNullItemIdParameter(0));
			}
			ExTraceGlobals.SessionTracer.Information<StoreId>((long)this.GetHashCode(), "Folder::GroupItemsByFolder - ItemId:{0}", itemId);
			StoreObjectId storeObjectId = null;
			StoreObjectId storeObjectId2 = StoreId.GetStoreObjectId(itemId);
			if (storeObjectId2 is OccurrenceStoreObjectId)
			{
				throw new ArgumentException(ServerStrings.ExCannotMoveOrCopyOccurrenceItem(itemId));
			}
			try
			{
				storeObjectId = this.GetParentFolderId(storeObjectId2);
			}
			catch (ObjectNotFoundException ex)
			{
				ExTraceGlobals.StorageTracer.TraceError<StoreObjectId, ObjectNotFoundException>((long)this.GetHashCode(), "StoreSession::GroupNonOccurrenceByFolder, Cannot find the object's parent folder. id = {0}, Exception = {1}", storeObjectId2, ex);
				GroupOperationResult item = new GroupOperationResult(OperationResult.Failed, new StoreObjectId[]
				{
					storeObjectId2
				}, ex);
				errors.Add(item);
				return;
			}
			if (!groupedObjects.ContainsKey(StoreId.GetStoreObjectId(storeObjectId)))
			{
				groupedObjects[storeObjectId] = new List<StoreId>();
			}
			groupedObjects[storeObjectId].Add(itemId);
		}

		private void OnDroppedNotify(MapiNotification notification)
		{
			MapiConnectionDroppedNotification mapiConnectionDroppedNotification = notification as MapiConnectionDroppedNotification;
			if (mapiConnectionDroppedNotification != null)
			{
				ExTraceGlobals.SessionTracer.Information<string, string, int>((long)this.GetHashCode(), "StoreSession::OnDroppedNotify. serverDn = {0}, userDn = {1}, tickDeath {2}.", mapiConnectionDroppedNotification.ServerDN, mapiConnectionDroppedNotification.UserDN, mapiConnectionDroppedNotification.TickDeath);
				this.isDead = true;
			}
		}

		private void OnTickle(MapiNotification notification)
		{
		}

		private bool IsConnectionDead
		{
			get
			{
				if (this.isDead)
				{
					return true;
				}
				if (this.mailboxStoreObject != null && this.mailboxStoreObject.Mailbox != null && this.mailboxStoreObject.Mailbox.MapiStore != null)
				{
					try
					{
						if (this.mailboxStoreObject.Mailbox.MapiStore.IsDead)
						{
							this.isDead = true;
						}
					}
					catch (MapiPermanentException arg)
					{
						ExTraceGlobals.SessionTracer.Information<MapiPermanentException>((long)this.GetHashCode(), "StoreSession::IsConnectionDead, MapiPermanentException calling IsDead, ignored. {0}", arg);
					}
					catch (MapiRetryableException arg2)
					{
						ExTraceGlobals.SessionTracer.Information<MapiRetryableException>((long)this.GetHashCode(), "StoreSession::IsConnectionDead, MapiRetryableException calling IsDead, ignored. {0}", arg2);
					}
					return this.isDead;
				}
				return true;
			}
		}

		public static CultureInfo CreateMapiCultureInfo(int stringLCID, int sortLCID, int codePage)
		{
			return MapiCultureInfo.CreateCultureInfo(stringLCID, sortLCID, codePage);
		}

		public static void AbandonNotificationsDuringShutdown(bool abandon)
		{
			MapiNotification.AbandonNotificationsDuringShutdown(abandon);
		}

		private void AccumulatePerRPCStatistics(PerRPCPerformanceStatistics newStats)
		{
			uint validVersion = this.cumulativeRPCStats.validVersion;
			this.cumulativeRPCStats.timeInServer = this.cumulativeRPCStats.timeInServer + newStats.timeInServer;
			this.cumulativeRPCStats.timeInCPU = this.cumulativeRPCStats.timeInCPU + newStats.timeInCPU;
			this.cumulativeRPCStats.pagesRead = this.cumulativeRPCStats.pagesRead + newStats.pagesRead;
			this.cumulativeRPCStats.pagesPreread = this.cumulativeRPCStats.pagesPreread + newStats.pagesPreread;
			this.cumulativeRPCStats.logRecords = this.cumulativeRPCStats.logRecords + newStats.logRecords;
			this.cumulativeRPCStats.logBytes = this.cumulativeRPCStats.logBytes + newStats.logBytes;
			this.cumulativeRPCStats.ldapReads = this.cumulativeRPCStats.ldapReads + newStats.ldapReads;
			this.cumulativeRPCStats.ldapSearches = this.cumulativeRPCStats.ldapSearches + newStats.ldapSearches;
			this.cumulativeRPCStats.avgDbLatency = newStats.avgDbLatency;
			this.cumulativeRPCStats.avgServerLatency = newStats.avgServerLatency;
			this.cumulativeRPCStats.totalDbOperations = newStats.totalDbOperations;
			this.cumulativeRPCStats.currentThreads = newStats.currentThreads;
			this.cumulativeRPCStats.currentDbThreads = newStats.currentDbThreads;
			this.cumulativeRPCStats.currentSCTThreads = newStats.currentSCTThreads;
			this.cumulativeRPCStats.currentSCTSessions = newStats.currentSCTSessions;
			this.cumulativeRPCStats.dataProtectionHealth = newStats.dataProtectionHealth;
			this.cumulativeRPCStats.dataAvailabilityHealth = newStats.dataAvailabilityHealth;
			this.cumulativeRPCStats.currentCpuUsage = newStats.currentCpuUsage;
			this.cumulativeRPCStats.validVersion = newStats.validVersion;
			this.cumulativeRPCStats.processID = newStats.processID;
		}

		public CumulativeRPCPerformanceStatistics GetStoreCumulativeRPCStats()
		{
			CumulativeRPCPerformanceStatistics result;
			using (this.CreateSessionGuard("GetStoreCumulativeRPCStats"))
			{
				result = this.cumulativeRPCStats;
			}
			return result;
		}

		public static CumulativeRPCPerformanceStatistics SubtractRPCPerformanceStatistics(CumulativeRPCPerformanceStatistics later, CumulativeRPCPerformanceStatistics earlier)
		{
			CumulativeRPCPerformanceStatistics result;
			result.timeInServer = later.timeInServer - earlier.timeInServer;
			result.timeInCPU = later.timeInCPU - earlier.timeInCPU;
			result.pagesRead = later.pagesRead - earlier.pagesRead;
			result.pagesPreread = later.pagesPreread - earlier.pagesPreread;
			result.logRecords = later.logRecords - earlier.logRecords;
			result.logBytes = later.logBytes - earlier.logBytes;
			result.ldapReads = later.ldapReads - earlier.ldapReads;
			result.ldapSearches = later.ldapSearches - earlier.ldapSearches;
			result.totalDbOperations = later.totalDbOperations - earlier.totalDbOperations;
			result.avgDbLatency = later.avgDbLatency;
			result.avgServerLatency = later.avgServerLatency;
			result.currentThreads = later.currentThreads;
			result.currentDbThreads = later.currentDbThreads;
			result.currentSCTThreads = later.currentSCTThreads;
			result.currentSCTSessions = later.currentSCTSessions;
			result.dataProtectionHealth = later.dataProtectionHealth;
			result.dataAvailabilityHealth = later.dataAvailabilityHealth;
			result.currentCpuUsage = later.currentCpuUsage;
			result.validVersion = later.validVersion;
			result.processID = later.processID;
			return result;
		}

		protected static IDatabaseLocationProvider DatabaseLocationProvider
		{
			get
			{
				if (StoreSession.databaseLocationProvider == null)
				{
					StoreSession.databaseLocationProvider = new DatabaseLocationProvider();
				}
				return StoreSession.databaseLocationProvider;
			}
		}

		protected RemoteMailboxProperties RemoteMailboxProperties
		{
			get
			{
				if (this.remoteMailboxProperties == null)
				{
					this.remoteMailboxProperties = new RemoteMailboxProperties(this, StoreSession.directoryAccessor);
				}
				return this.remoteMailboxProperties;
			}
		}

		public static void SetDatabaseLocationProviderForTest(IDatabaseLocationProvider databaseLocationProvider)
		{
			StoreSession.databaseLocationProvider = databaseLocationProvider;
		}

		public static ClientIdentityInfo FromAuthZContext(ADSessionSettings adSettings, AuthzContextHandle authenticatedUserHandle)
		{
			List<string> list;
			StoreSession.GetSidFromClientContext(authenticatedUserHandle, AuthzContextInformation.UserSid, out list);
			SecurityIdentifier securityIdentifier = new SecurityIdentifier(list[0]);
			StoreSession.GetSidFromClientContext(authenticatedUserHandle, AuthzContextInformation.GroupSids, out list);
			int? num = null;
			try
			{
				num = StoreSession.directoryAccessor.GetPrimaryGroupId(adSettings.CreateRecipientSession(null), securityIdentifier);
			}
			catch (ObjectNotFoundException arg)
			{
				ExTraceGlobals.StorageTracer.TraceError<ObjectNotFoundException>(0L, "FromAuthZContext. More than one recipients with same sid was found. Exception = {0}.", arg);
			}
			SecurityIdentifier _sidGroup;
			if (num == null)
			{
				_sidGroup = StoreSession.PickAnyGroup(list);
			}
			else
			{
				_sidGroup = StoreSession.PickOneApproxGroup(securityIdentifier, list, num.Value);
			}
			return new ClientIdentityInfo(authenticatedUserHandle.DangerousGetHandle(), securityIdentifier, _sidGroup);
		}

		private static SecurityIdentifier PickOneApproxGroup(SecurityIdentifier sidUser, List<string> groupSidStrings, int primaryGroupId)
		{
			if (groupSidStrings == null || groupSidStrings.Count == 0)
			{
				return null;
			}
			string arg = sidUser.Value.Substring(0, sidUser.Value.LastIndexOf('-'));
			int index = 0;
			for (int i = 0; i < groupSidStrings.Count; i++)
			{
				if (string.Compare(arg + "-" + primaryGroupId, groupSidStrings[i]) == 0)
				{
					return new SecurityIdentifier(groupSidStrings[i]);
				}
				if (groupSidStrings[i].EndsWith("-" + primaryGroupId))
				{
					index = i;
				}
			}
			return new SecurityIdentifier(groupSidStrings[index]);
		}

		private static SecurityIdentifier PickAnyGroup(List<string> sidStrings)
		{
			if (sidStrings == null || sidStrings.Count == 0)
			{
				return null;
			}
			return new SecurityIdentifier(sidStrings[0]);
		}

		private static void GetSidFromClientContext(AuthzContextHandle contextHandle, AuthzContextInformation contextClass, out List<string> sidStrings)
		{
			sidStrings = null;
			switch (contextClass)
			{
			case AuthzContextInformation.UserSid:
			{
				NativeMethods.SecurityIdentifierAndAttributes securityIdentifierAndAttributes = NativeMethods.AuthzGetInformationFromContextTokenUser(contextHandle);
				sidStrings = new List<string>(1);
				sidStrings.Add(securityIdentifierAndAttributes.sid.ToString());
				return;
			}
			case AuthzContextInformation.GroupSids:
			{
				NativeMethods.SecurityIdentifierAndAttributes[] array = NativeMethods.AuthzGetInformationFromContextTokenGroup(contextHandle);
				if (array != null)
				{
					sidStrings = new List<string>(array.Length);
					foreach (NativeMethods.SecurityIdentifierAndAttributes securityIdentifierAndAttributes2 in array)
					{
						sidStrings.Add(securityIdentifierAndAttributes2.sid.ToString());
					}
					return;
				}
				return;
			}
			default:
				throw new NotSupportedException();
			}
		}

		private void InitializeADSessionFactory()
		{
			this.adRecipientSessionFactory = ((bool isReadonly, ConsistencyMode consistencyMode) => DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, null, this.InternalPreferedCulture.LCID, isReadonly, consistencyMode, null, this.GetADSessionSettings(), 3509, "InitializeADSessionFactory", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\obj\\amd64\\StoreSession.cs"));
			this.adConfigurationSessionFactory = ((bool isReadonly, ConsistencyMode consistencyMode) => DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(null, isReadonly, consistencyMode, null, this.GetADSessionSettings(), 3519, "InitializeADSessionFactory", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\obj\\amd64\\StoreSession.cs"));
		}

		public void SetADRecipientSessionFactory(Func<bool, ConsistencyMode, IRecipientSession> adRecipientSessionFactory)
		{
			using (this.CheckDisposed("SetADRecipientSessionFactory"))
			{
				ArgumentValidator.ThrowIfNull("adRecipientSessionFactory", adRecipientSessionFactory);
				this.adRecipientSessionFactory = adRecipientSessionFactory;
			}
		}

		public void SetADConfigurationSessionFactory(Func<bool, ConsistencyMode, IConfigurationSession> adConfigurationSessionFactory)
		{
			using (this.CheckDisposed("SetADConfigurationSessionFactory"))
			{
				ArgumentValidator.ThrowIfNull("adConfigurationSessionFactory", adConfigurationSessionFactory);
				this.adConfigurationSessionFactory = adConfigurationSessionFactory;
			}
		}

		public void Deliver(Item item, ProxyAddress recipientProxyAddress, RecipientItemType recipientType)
		{
			using (this.CheckObjectState("Deliver"))
			{
				this.CheckCapabilities(this.Capabilities.CanDeliver, "CanDeliver");
				if (item == null)
				{
					throw new ArgumentNullException("item");
				}
				MapiMessage mapiMessage = item.MapiMessage;
				if (mapiMessage == null)
				{
					ExTraceGlobals.SessionTracer.TraceError((long)this.GetHashCode(), "StoreSession::Deliver. The item.MapiMessage is Null.");
					throw new ArgumentException("item");
				}
				if (recipientType != RecipientItemType.Unknown && recipientType != RecipientItemType.To && recipientType != RecipientItemType.Cc && recipientType != RecipientItemType.Bcc)
				{
					ExTraceGlobals.SessionTracer.TraceError<RecipientItemType>((long)this.GetHashCode(), "StoreSession::Deliver. Unknown recipient type. recipientType = {0}", recipientType);
					throw new EnumOutOfRangeException("recipientType");
				}
				using (CallbackContext callbackContext = new CallbackContext(this))
				{
					using (MailboxEvaluationResult mailboxEvaluationResult = this.EvaluateFolderRules(item.CoreItem, recipientProxyAddress))
					{
						callbackContext.ItemOperationAuditInfo = new ItemAuditInfo((item.Id == null) ? null : item.Id.ObjectId, null, null, item.PropertyBag.TryGetProperty(CoreItemSchema.Subject) as string, item.PropertyBag.TryGetProperty(ItemSchema.From) as Participant);
						item.SaveFlags |= (PropertyBagSaveFlags.IgnoreMapiComputedErrors | PropertyBagSaveFlags.IgnoreUnresolvedHeaders);
						item.SaveInternal(SaveMode.NoConflictResolution, false, callbackContext, CoreItemOperation.Save);
						byte[] entryId = null;
						IDictionary<string, string> deliveryActivityInfo = null;
						if (this.ActivitySession != null)
						{
							deliveryActivityInfo = ActivityLogHelper.ExtractDeliveryDetails(this, item);
						}
						try
						{
							if (this.ExecuteFolderRulesOnBefore(mailboxEvaluationResult) != FolderRuleEvaluationStatus.Continue)
							{
								return;
							}
							ExTraceGlobals.SessionTracer.TraceDebug((long)this.GetHashCode(), "StoreSession::Deliver. Invoke MapiMessage.Deliver.");
							bool flag = false;
							try
							{
								if (this != null)
								{
									this.BeginMapiCall();
									this.BeginServerHealthCall();
									flag = true;
								}
								if (StorageGlobals.MapiTestHookBeforeCall != null)
								{
									StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
								}
								mapiMessage.Deliver((RecipientType)recipientType);
								if (this.ActivitySession != null)
								{
									PropValue prop = mapiMessage.GetProp(PropTag.EntryId);
									if (!prop.IsError())
									{
										entryId = prop.GetBytes();
									}
								}
							}
							catch (MapiPermanentException ex)
							{
								throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotDeliverItem, ex, this, this, "{0}. MapiException = {1}.", new object[]
								{
									string.Format("StoreSession::Deliver.", new object[0]),
									ex
								});
							}
							catch (MapiRetryableException ex2)
							{
								throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotDeliverItem, ex2, this, this, "{0}. MapiException = {1}.", new object[]
								{
									string.Format("StoreSession::Deliver.", new object[0]),
									ex2
								});
							}
							finally
							{
								try
								{
									if (this != null)
									{
										this.EndMapiCall();
										if (flag)
										{
											this.EndServerHealthCall();
										}
									}
								}
								finally
								{
									if (StorageGlobals.MapiTestHookAfterCall != null)
									{
										StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
									}
								}
							}
						}
						finally
						{
							this.ExecuteFolderRulesOnAfter(mailboxEvaluationResult);
						}
						item.CoreItem.PropertyBag.Clear();
						if (this.ActivitySession != null)
						{
							this.ActivitySession.CaptureDelivery(StoreObjectId.FromProviderSpecificIdOrNull(entryId), deliveryActivityInfo);
						}
						this.OnAfterItemChange(ItemChangeOperation.Create, this, null, item.CoreItem as CoreItem, ConflictResolutionResult.Success, callbackContext);
					}
				}
			}
		}

		public bool MessagesWereDownloaded
		{
			get
			{
				bool result;
				using (this.CreateSessionGuard("MessagesWereDownloaded::get"))
				{
					result = (this.gccProtocolLoggerSession != null && this.gccProtocolLoggerSession.MessagesWereDownloaded);
				}
				return result;
			}
			set
			{
				using (this.CreateSessionGuard("MessagesWereDownloaded::set"))
				{
					if (this.gccProtocolLoggerSession != null)
					{
						this.gccProtocolLoggerSession.MessagesWereDownloaded = true;
						this.gccProtocolLoggerSession.TagCurrentIntervalAsLogworthy();
					}
				}
			}
		}

		internal ClientSessionInfo RemoteClientSessionInfo
		{
			get
			{
				ClientSessionInfo result;
				using (this.CreateSessionGuard("RemoteClientSessionInfo::get"))
				{
					result = this.remoteClientSessionInfo;
				}
				return result;
			}
		}

		public void SetRemoteClientSessionInfo(byte[] infoBlob)
		{
			using (this.CheckDisposed("SetRemoteClientSessionInfo"))
			{
				this.remoteClientSessionInfo = ClientSessionInfo.FromBytes(infoBlob);
			}
		}

		internal static int GetConfigFromRegistry(string registryKeyName, string registryValueName, int defaultValue, Predicate<int> validator)
		{
			int result = defaultValue;
			Exception ex = null;
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(registryKeyName, RegistryKeyPermissionCheck.ReadSubTree))
				{
					if (registryKey != null)
					{
						object value = registryKey.GetValue(registryValueName, defaultValue);
						if (value is int && (validator == null || validator((int)value)))
						{
							result = (int)value;
						}
						else
						{
							ExTraceGlobals.SessionTracer.TraceDebug<string, string, int>(0L, "Invalid value provided for registry entry - {0}\\{1}. Default Value of {2} will be used.", registryKeyName, registryValueName, defaultValue);
						}
					}
				}
			}
			catch (UnauthorizedAccessException ex2)
			{
				ex = ex2;
			}
			catch (SecurityException ex3)
			{
				ex = ex3;
			}
			catch (IOException ex4)
			{
				ex = ex4;
			}
			finally
			{
				if (ex != null)
				{
					result = defaultValue;
					ExTraceGlobals.SessionTracer.TraceError(0L, "Failed to read registry entry - {0}\\{1}. Default Value of {2} will be used. Exception - {3}", new object[]
					{
						registryKeyName,
						registryValueName,
						defaultValue,
						ex
					});
				}
			}
			return result;
		}

		protected static byte[] GetTenantHint(IExchangePrincipal principal)
		{
			byte[] tenantHint = null;
			if (principal.MailboxInfo.OrganizationId != null && principal.MailboxInfo.OrganizationId != OrganizationId.ForestWideOrgId)
			{
				DirectoryHelper.DoAdCallAndTranslateExceptions(delegate
				{
					tenantHint = TenantPartitionHint.Serialize(TenantPartitionHint.FromOrganizationId(principal.MailboxInfo.OrganizationId));
				}, "GetTenantHint");
			}
			return tenantHint;
		}

		public IRecipientSession GetADRecipientSession(bool isReadOnly, ConsistencyMode consistencyMode)
		{
			IRecipientSession result;
			using (this.CheckDisposed("GetADRecipientSession"))
			{
				result = this.adRecipientSessionFactory(isReadOnly, consistencyMode);
			}
			return result;
		}

		public IConfigurationSession GetADConfigurationSession(bool isReadOnly, ConsistencyMode consistencyMode)
		{
			IConfigurationSession result;
			using (this.CheckDisposed("GetADConfigurationSession"))
			{
				result = this.adConfigurationSessionFactory(isReadOnly, consistencyMode);
			}
			return result;
		}

		internal string WlmOperationInstance
		{
			get
			{
				string result;
				using (this.CreateSessionGuard("WlmOperationInstance::get"))
				{
					if (this.wlmOperationInstance == null)
					{
						string str = "NA";
						MailboxSession mailboxSession = this as MailboxSession;
						IExchangePrincipal exchangePrincipal = (mailboxSession != null) ? mailboxSession.MailboxOwner : null;
						string str2;
						if (exchangePrincipal != null && exchangePrincipal.MailboxInfo.IsRemote)
						{
							Uri remoteEndpoint = this.RemoteMailboxProperties.GetRemoteEndpoint(this.MailboxOwner.MailboxInfo.RemoteIdentity);
							if (remoteEndpoint.HostNameType != UriHostNameType.IPv4 && remoteEndpoint.HostNameType != UriHostNameType.IPv6)
							{
								str2 = MachineName.GetNodeNameFromFqdn(remoteEndpoint.Host);
							}
							else
							{
								str2 = remoteEndpoint.Host;
							}
							if (!exchangePrincipal.MailboxInfo.MailboxDatabase.IsNullOrEmpty())
							{
								str = exchangePrincipal.MailboxInfo.GetDatabaseGuid().ToString("D");
							}
						}
						else
						{
							str2 = MachineName.GetNodeNameFromFqdn(this.ServerFullyQualifiedDomainName);
							if (this.MdbGuid != Guid.Empty)
							{
								str = this.MdbGuid.ToString("D");
							}
						}
						this.wlmOperationInstance = str2 + "." + str;
					}
					result = this.wlmOperationInstance;
				}
				return result;
			}
		}

		public void PreFinalSyncDataProcessing(int? sourceMailboxVersion)
		{
			using (this.CheckObjectState("PreFinalSyncDataProcessing"))
			{
				try
				{
					GrayException.MapAndReportGrayExceptions(delegate()
					{
						if (sourceMailboxVersion == null)
						{
							ExTraceGlobals.SessionTracer.TraceDebug<Guid>((long)this.GetHashCode(), "StoreSession::PreFinalSyncDataProcessing. Skipping mailbox as version number is null. GUID: {0}", this.MailboxGuid);
							return;
						}
						ServerVersion serverVersion = new ServerVersion(sourceMailboxVersion.Value);
						if (serverVersion.Major <= Server.Exchange2009MajorVersion)
						{
							AutomaticLink.PerformContactLinkingAfterMigration(this);
							return;
						}
						ExTraceGlobals.SessionTracer.TraceDebug<Guid, ServerVersion>((long)this.GetHashCode(), "StoreSession::PreFinalSyncDataProcessing. Skipping mailbox due to version number. GUID: {0}. Version: {1}", this.MailboxGuid, serverVersion);
					});
				}
				catch (GrayException ex)
				{
					ExTraceGlobals.SessionTracer.TraceError<Guid, GrayException>((long)this.GetHashCode(), "StoreSession::PreFinalSyncDataProcessing. Failed while processing mailbox {1}. Error: {2}", this.MailboxGuid, ex);
					ExWatson.SendReport(ex);
				}
			}
		}

		protected string[] InternalGetSupportedRoutingTypes()
		{
			string[] routingTypes = RoutingTypeBuilder.Instance.GetRoutingTypes();
			for (int i = 0; i < routingTypes.Length; i++)
			{
				routingTypes[i] = string.Intern(Participant.NormalizeRoutingType(routingTypes[i]));
			}
			return routingTypes;
		}

		protected IRPCLatencyProvider GetMdbHealthMonitor()
		{
			IRPCLatencyProvider result;
			using (this.CheckDisposed("HealthMonitor::get"))
			{
				result = (this.mdbHealthMonitor ?? this.GetHealthMonitor<IRPCLatencyProvider>(new MdbResourceHealthMonitorKey(this.MdbGuid), ref this.mdbHealthMonitor));
			}
			return result;
		}

		protected IDatabaseReplicationProvider GetReplicationHealthMonitor()
		{
			IDatabaseReplicationProvider result;
			using (this.CheckDisposed("ReplicationHealthMonitor::get"))
			{
				result = (this.replicationHealthMonitor ?? this.GetHealthMonitor<IDatabaseReplicationProvider>(new MdbReplicationResourceHealthMonitorKey(this.MdbGuid), ref this.replicationHealthMonitor));
			}
			return result;
		}

		protected IDatabaseAvailabilityProvider GetAvailabilityHealthMonitor()
		{
			IDatabaseAvailabilityProvider result;
			using (this.CheckDisposed("AvailabilityHealthMonitor::get"))
			{
				result = (this.availabilityHealthMonitor ?? this.GetHealthMonitor<IDatabaseAvailabilityProvider>(new MdbAvailabilityResourceHealthMonitorKey(this.MdbGuid), ref this.availabilityHealthMonitor));
			}
			return result;
		}

		internal virtual MailboxEvaluationResult EvaluateFolderRules(ICoreItem item, ProxyAddress recipientProxyAddress)
		{
			MailboxEvaluationResult result;
			using (this.CheckObjectState("EvaluateFolderRules"))
			{
				result = null;
			}
			return result;
		}

		internal virtual void ExecuteFolderRulesOnAfter(MailboxEvaluationResult evaluationResult)
		{
			using (this.CheckObjectState("ExecuteFolderRulesOnAfter"))
			{
			}
		}

		internal virtual FolderRuleEvaluationStatus ExecuteFolderRulesOnBefore(MailboxEvaluationResult evaluationResult)
		{
			FolderRuleEvaluationStatus result;
			using (this.CheckObjectState("ExecuteFolderRulesOnBefore"))
			{
				result = FolderRuleEvaluationStatus.Continue;
			}
			return result;
		}

		private T GetHealthMonitor<T>(ResourceKey key, ref T member) where T : IResourceLoadMonitor
		{
			if (this.IsRemote)
			{
				throw new InvalidOperationException();
			}
			try
			{
				member = (T)((object)ResourceHealthMonitorManager.Singleton.Get(key));
			}
			catch (DataSourceTransientException ex)
			{
				throw StorageGlobals.TranslateDirectoryException(ServerStrings.ADException, ex, null, "StoreSession.GetHealthMonitor encountered transient directory exception: {0}.", new object[]
				{
					ex
				});
			}
			catch (DataSourceOperationException ex2)
			{
				throw StorageGlobals.TranslateDirectoryException(ServerStrings.ADException, ex2, null, "StoreSession.GetHealthMonitor encountered operation directory exception: {0}.", new object[]
				{
					ex2
				});
			}
			if (member == null)
			{
				throw new InvalidOperationException("ResourceHealthMonitorManager.Get should never return a null monitor.  ResourceKey: " + key);
			}
			return member;
		}

		private void AddOperations(PerRPCPerformanceStatistics stats, RpcStatistics rpcStats)
		{
			IActivityScope currentActivityScope = ActivityContext.GetCurrentActivityScope();
			if (stats.timeInServer.TotalMilliseconds > 0.0)
			{
				ActivityContext.AddOperation(currentActivityScope, ActivityOperationType.StoreCall, this.WlmOperationInstance, (float)stats.timeInServer.TotalMilliseconds, 1);
			}
			ActivityContext.AddOperation(currentActivityScope, ActivityOperationType.MailboxCall, this.WlmOperationInstance, (float)this.stopwatch.ElapsedMilliseconds, (int)rpcStats.rpcCount);
			ActivityContext.AddOperation(currentActivityScope, ActivityOperationType.MapiLatency, this.WlmOperationInstance, (float)this.stopwatch.ElapsedMilliseconds, 1);
			ActivityContext.AddOperation(currentActivityScope, ActivityOperationType.MapiCount, this.WlmOperationInstance, 0f, 1);
			if (rpcStats.rpcCount > 0U)
			{
				ActivityContext.AddOperation(currentActivityScope, ActivityOperationType.RpcCount, this.WlmOperationInstance, 0f, (int)rpcStats.rpcCount);
				ActivityContext.AddOperation(currentActivityScope, ActivityOperationType.RpcLatency, this.WlmOperationInstance, (float)this.stopwatch.ElapsedMilliseconds, 1);
			}
			ActivityContext.AddOperation(currentActivityScope, ActivityOperationType.Rop, this.WlmOperationInstance, 0f, (int)stats.totalDbOperations);
			if (stats.timeInCPU > TimeSpan.Zero)
			{
				ActivityContext.AddOperation(currentActivityScope, ActivityOperationType.StoreCpu, this.WlmOperationInstance, (float)stats.timeInCPU.TotalMilliseconds, 1);
			}
			if (stats.logBytes > 0U)
			{
				ActivityContext.AddOperation(currentActivityScope, ActivityOperationType.MailboxLogBytes, this.WlmOperationInstance, stats.logBytes, 1);
			}
			if (rpcStats.messagesCreated > 0U)
			{
				ActivityContext.AddOperation(currentActivityScope, ActivityOperationType.MailboxMessagesCreated, this.WlmOperationInstance, rpcStats.messagesCreated, 1);
			}
		}

		internal void BeginMapiCall()
		{
			using (this.CreateSessionGuard("BeginMapiCall"))
			{
				if (this.mailboxStoreObject != null && !this.IsRemote)
				{
					this.mailboxStoreObject.MapiStore.ClearStorePerRPCStats();
					this.mailboxStoreObject.MapiStore.ClearRpcStatistics();
					IActivityScope currentActivityScope = ActivityContext.GetCurrentActivityScope();
					if (currentActivityScope != null)
					{
						this.mailboxStoreObject.MapiStore.SetCurrentActivityInfo(currentActivityScope.ActivityId, currentActivityScope.Component, currentActivityScope.Protocol, currentActivityScope.Action);
					}
				}
			}
			this.stopwatch.Restart();
		}

		internal void EndMapiCall()
		{
			this.stopwatch.Stop();
			using (this.CreateSessionGuard("EndMapiCall"))
			{
				if (this.mailboxStoreObject != null && !this.IsRemote)
				{
					PerRPCPerformanceStatistics storePerRPCStats = this.mailboxStoreObject.MapiStore.GetStorePerRPCStats();
					RpcStatistics rpcStatistics = this.mailboxStoreObject.MapiStore.GetRpcStatistics();
					this.AddOperations(storePerRPCStats, rpcStatistics);
					if (storePerRPCStats.validVersion > 0U)
					{
						this.AccumulatePerRPCStatistics(storePerRPCStats);
						CumulativeRPCPerformanceStatistics storeCumulativeRPCStats = this.GetStoreCumulativeRPCStats();
						try
						{
							this.GetMdbHealthMonitor().Update((int)storeCumulativeRPCStats.avgDbLatency, (storePerRPCStats.validVersion >= 2U) ? storeCumulativeRPCStats.totalDbOperations : 100U);
							if (storePerRPCStats.validVersion >= 4U)
							{
								this.GetReplicationHealthMonitor().Update(storeCumulativeRPCStats.dataProtectionHealth);
								this.GetAvailabilityHealthMonitor().Update(storeCumulativeRPCStats.dataAvailabilityHealth);
							}
						}
						catch (DataSourceTransientException ex)
						{
							throw StorageGlobals.TranslateDirectoryException(ServerStrings.ADException, ex, null, "StoreSession.EndMapiCall encountered transient directory exception: {0}.", new object[]
							{
								ex
							});
						}
						catch (DataSourceOperationException ex2)
						{
							throw StorageGlobals.TranslateDirectoryException(ServerStrings.ADException, ex2, null, "StoreSession.EndMapiCall encountered operation directory exception: {0}.", new object[]
							{
								ex2
							});
						}
					}
				}
			}
		}

		internal DatabaseLocationInfo GetDatabaseLocationInfoOnOperationFailure()
		{
			bool flag = false;
			DatabaseLocationInfo databaseLocationInfo = null;
			if (!this.MailboxOwner.MailboxInfo.MailboxDatabase.IsNullOrEmpty())
			{
				try
				{
					databaseLocationInfo = StoreSession.DatabaseLocationProvider.GetLocationInfo(this.MailboxOwner.MailboxInfo.MailboxDatabase.ObjectGuid, false, this.MailboxOwner.IsCrossSiteAccessAllowed);
				}
				catch (DatabaseLocationUnavailableException)
				{
					flag = true;
				}
				if (databaseLocationInfo != null)
				{
					flag |= (this.MailboxOwner.MailboxInfo.Location.ServerLegacyDn == null || !string.Equals(this.MailboxOwner.MailboxInfo.Location.ServerLegacyDn, databaseLocationInfo.ServerLegacyDN, StringComparison.OrdinalIgnoreCase));
					if (databaseLocationInfo.RequestResult == DatabaseLocationInfoResult.Success && !flag)
					{
						databaseLocationInfo = StoreSession.DatabaseLocationProvider.GetLocationInfo(this.MailboxOwner.MailboxInfo.GetDatabaseGuid(), true, this.MailboxOwner.IsCrossSiteAccessAllowed);
					}
				}
			}
			return databaseLocationInfo;
		}

		internal virtual void BeginServerHealthCall()
		{
			using (this.CreateSessionGuard("BeginServerHealthCall"))
			{
			}
		}

		internal virtual void EndServerHealthCall()
		{
			using (this.CreateSessionGuard("EndServerHealthCall"))
			{
			}
		}

		private void InternalInitializeGccProtocolSession()
		{
			if (string.IsNullOrEmpty(MapiStore.GetLocalServerFqdn()))
			{
				MapiStore.SetLocalServerFqdn(LocalServer.GetServer().Fqdn);
			}
			this.gccProtocolLoggerSession = new GccProtocolActivityLoggerSession(this);
			this.gccProtocolLoggerSession.StartSession();
		}

		private void InternalEndGccProtocolSession()
		{
			if (this.gccProtocolLoggerSession != null)
			{
				this.gccProtocolLoggerSession.EndSession();
			}
		}

		private void InternalDisposeGccProtocolSession()
		{
			Util.DisposeIfPresent(this.gccProtocolLoggerSession);
		}

		private void InternalLogIpEndpoints(IPAddress clientIPAddress, IPAddress serverIPAddress)
		{
			if (VariantConfiguration.InvariantNoFlightingSnapshot.DataStorage.LogIpEndpoints.Enabled && this.gccProtocolLoggerSession != null)
			{
				string gccResourceIdentifier = "Unknown";
				try
				{
					gccResourceIdentifier = this.GccResourceIdentifier;
				}
				catch (InvalidOperationException)
				{
				}
				this.gccProtocolLoggerSession.SetStoreSessionInfo(gccResourceIdentifier, clientIPAddress, serverIPAddress);
			}
		}

		private readonly Schema schema;

		private readonly SubscriptionsManager subscriptionsManager;

		private readonly IdConverter idConverter;

		private static readonly int currentServerMajorVersion = 15;

		private readonly Stopwatch stopwatch = new Stopwatch();

		protected readonly CultureInfo SessionCultureInfo;

		protected string clientInfoString;

		private static bool useRPCContextPoolResiliency = false;

		[ThreadStatic]
		private static bool testRequestExchangeRpcServer;

		private IContentIndexingSession contentIndexingSession;

		protected bool isConnected;

		protected LogonType logonType;

		protected object identity;

		protected ConnectFlag connectFlag;

		protected string userLegacyDn;

		protected GenericIdentity auxiliaryIdentity;

		protected UnifiedGroupMemberType unifiedGroupMemberType;

		private string[] supportedRoutingTypes;

		private long clientVersion;

		private string clientProcessName;

		private string clientMachineName;

		private readonly ObjectThreadTracker sessionThreadTracker = new ObjectThreadTracker();

		private ExchangeOperationContext operationContext = new ExchangeOperationContext();

		private bool blockFolderCreation;

		private MailboxMoveStage mailboxMoveStage;

		private bool isDisposed;

		private OpenStoreFlag storeFlag;

		private bool isDead;

		private int preferredInternetCodePageForShiftJis;

		private int requiredCoverage = 100;

		private StoreSession.IItemBinder itemBinder;

		private string mappingSignature;

		private WeakReference propResolutionCache;

		private IBudget budget;

		private MailboxStoreObject mailboxStoreObject;

		private DisposeTracker disposeTracker;

		private ILogChanges testLogCallback;

		private SessionCapabilities sessionCapabilities;

		private SpoolerManager spoolerManager;

		private IPAddress clientIPAddress;

		private IPAddress serverIPAddress;

		private MapiNotificationHandle tickleNotificationHandle;

		private MapiNotificationHandle droppedNotificationHandle;

		private IDictionary<StoreObjectId, bool> isContactFolder;

		private CumulativeRPCPerformanceStatistics cumulativeRPCStats;

		private static IDatabaseLocationProvider databaseLocationProvider;

		private IRPCLatencyProvider mdbHealthMonitor;

		private IDatabaseReplicationProvider replicationHealthMonitor;

		private IDatabaseAvailabilityProvider availabilityHealthMonitor;

		private string wlmOperationInstance;

		private GccProtocolActivityLoggerSession gccProtocolLoggerSession;

		private ClientSessionInfo remoteClientSessionInfo;

		private Func<bool, ConsistencyMode, IRecipientSession> adRecipientSessionFactory;

		private Func<bool, ConsistencyMode, IConfigurationSession> adConfigurationSessionFactory;

		private RemoteMailboxProperties remoteMailboxProperties;

		protected static readonly IDirectoryAccessor directoryAccessor = new DirectoryAccessor();

		protected delegate AggregateOperationResult ActOnObjectsDelegate(Folder parentFolder, StoreId[] sourceObjectIds);

		public interface IItemBinder
		{
			Item BindItem(StoreObjectId itemId, bool isPublic, StoreObjectId folderId);
		}
	}
}
