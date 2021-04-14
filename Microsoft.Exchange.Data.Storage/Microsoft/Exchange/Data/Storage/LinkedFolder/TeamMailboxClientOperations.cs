using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.SharePoint.Client;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class TeamMailboxClientOperations
	{
		protected TeamMailboxClientOperations(ICredentials credentials, bool isOAuthCredentials, MiniRecipient actAsUser, TimeSpan requestTimeout, bool httpDebugEnabled)
		{
			this.credentials = credentials;
			this.requestTimeout = requestTimeout;
			this.isOAuthCredential = isOAuthCredentials;
			this.actAsUser = actAsUser;
			this.httpDebugEnabled = httpDebugEnabled;
			if (!string.IsNullOrEmpty(this.actAsUser.LegacyExchangeDN))
			{
				this.actAsUserParticipant = new Participant(this.actAsUser.DisplayName, this.actAsUser.LegacyExchangeDN, "EX");
				return;
			}
			if (this.actAsUser.PrimarySmtpAddress != SmtpAddress.Empty)
			{
				this.actAsUserParticipant = new Participant(this.actAsUser.DisplayName, this.actAsUser.PrimarySmtpAddress.ToString(), "SMTP");
				return;
			}
			throw new StoragePermanentException(new LocalizedString("TeamMailboxClientOperations: ActAsUser have neither legacyExchangeDN or PrimarySmtpAddress set for " + this.actAsUser.Id));
		}

		public static TeamMailboxClientOperations CreateInstance(ICredentials credentials, bool isOAuthCredentials, MiniRecipient actAsUser, TimeSpan requestTimeout, bool httpDebugEnabled, bool isMock)
		{
			if (!isMock)
			{
				return new TeamMailboxClientOperations(credentials, isOAuthCredentials, actAsUser, requestTimeout, httpDebugEnabled);
			}
			return new MockTeamMailboxClientOperations(actAsUser);
		}

		public static void ThrowIfInvalidItemOperation(StoreSession session, IEnumerable<StoreObjectId> ids)
		{
			if (TeamMailboxClientOperations.ShouldSkipClientSessionCheck(session))
			{
				return;
			}
			HashSet<StoreObjectId> hashSet = new HashSet<StoreObjectId>();
			foreach (StoreObjectId objectId in ids)
			{
				StoreObjectId parentFolderId = session.GetParentFolderId(objectId);
				if (!hashSet.Contains(parentFolderId))
				{
					TeamMailboxClientOperations.ThrowIfFolderIsShorcutFolder(session, parentFolderId);
					hashSet.Add(parentFolderId);
				}
			}
		}

		public static void ThrowIfInvalidItemOperation(StoreSession session, CoreItem item)
		{
			if (TeamMailboxClientOperations.ShouldSkipClientSessionCheck(session))
			{
				return;
			}
			byte[] valueOrDefault = item.PropertyBag.GetValueOrDefault<byte[]>(CoreObjectSchema.ParentEntryId, null);
			if (valueOrDefault != null)
			{
				StoreObjectId folderId = StoreObjectId.FromProviderSpecificId(valueOrDefault, StoreObjectType.Folder);
				TeamMailboxClientOperations.ThrowIfFolderIsShorcutFolder(session, folderId);
			}
		}

		public static void ThrowIfInvalidFolderOperation(StoreSession session, IEnumerable<StoreObjectId> ids)
		{
			if (TeamMailboxClientOperations.ShouldSkipClientSessionCheck(session))
			{
				return;
			}
			foreach (StoreObjectId folderId in ids)
			{
				TeamMailboxClientOperations.ThrowIfFolderIsShorcutFolder(session, folderId);
			}
		}

		public static void ThrowIfInvalidFolderOperation(StoreSession session, CoreFolder folder)
		{
			if (TeamMailboxClientOperations.ShouldSkipClientSessionCheck(session))
			{
				return;
			}
			folder.PropertyBag.Load(TeamMailboxClientOperations.shorcutFolderProperties);
			TeamMailboxClientOperations.ThrowIfFolderIsShorcutFolder(session, folder);
		}

		private static void ThrowIfFolderIsShorcutFolder(StoreSession session, StoreObjectId folderId)
		{
			if (TeamMailboxClientOperations.IsLinkedFolder((MailboxSession)session, folderId, true))
			{
				throw new StoragePermanentException(ServerStrings.UnsupportedClientOperation(session.ClientInfoString));
			}
		}

		private static void ThrowIfFolderIsShorcutFolder(StoreSession session, CoreFolder folder)
		{
			if (TeamMailboxClientOperations.IsLinkedFolder(folder, true))
			{
				throw new StoragePermanentException(ServerStrings.UnsupportedClientOperation(session.ClientInfoString));
			}
		}

		private static bool ShouldSkipClientSessionCheck(StoreSession session)
		{
			return !Utils.IsTeamMailbox(session) || !Utils.IsBlockedTeamMailboxSession(session);
		}

		public static bool IsLinkedFolder(MailboxSession session, StoreObjectId folderId, bool matchAny = false)
		{
			bool result;
			using (CoreFolder coreFolder = CoreFolder.Bind(session, folderId))
			{
				result = TeamMailboxClientOperations.IsLinkedFolder(coreFolder, matchAny);
			}
			return result;
		}

		public static bool IsLinkedFolder(Folder folder)
		{
			return TeamMailboxClientOperations.IsLinkedFolder((CoreFolder)folder.CoreObject, false);
		}

		public static bool IsLinkedFolder(CoreFolder folder, bool matchAny = false)
		{
			if (!Utils.IsTeamMailbox(folder.Session))
			{
				return false;
			}
			string valueOrDefault = folder.PropertyBag.GetValueOrDefault<string>(StoreObjectSchema.ContainerClass, string.Empty);
			string valueOrDefault2 = folder.PropertyBag.GetValueOrDefault<string>(CoreFolderSchema.LinkedUrl, null);
			string valueOrDefault3 = folder.PropertyBag.GetValueOrDefault<string>(CoreFolderSchema.LinkedSiteUrl, string.Empty);
			if (matchAny)
			{
				return !string.IsNullOrEmpty(valueOrDefault2) || !string.IsNullOrEmpty(valueOrDefault3) || valueOrDefault.Equals("IPF.ShortcutFolder");
			}
			return !string.IsNullOrEmpty(valueOrDefault2) && !string.IsNullOrEmpty(valueOrDefault3) && valueOrDefault.Equals("IPF.ShortcutFolder");
		}

		public static bool IsLinkedItem(CoreItem item)
		{
			return Utils.IsTeamMailbox(item.Session) && TeamMailboxClientOperations.IsLinked(item.PropertyBag);
		}

		public static bool IsLinked(CoreAttachment attachment)
		{
			return attachment.ParentCollection != null && attachment.ParentCollection.ContainerItem != null && TeamMailboxClientOperations.IsLinked(attachment.ParentCollection.ContainerItem.PropertyBag);
		}

		public static bool IsLinked(ICorePropertyBag propertyBag)
		{
			string valueOrDefault = propertyBag.GetValueOrDefault<string>(CoreItemSchema.LinkedUrl, string.Empty);
			Guid? valueAsNullable = propertyBag.GetValueAsNullable<Guid>(CoreItemSchema.LinkedId);
			return !string.IsNullOrEmpty(valueOrDefault) && valueAsNullable != null;
		}

		public static bool IsMoveCopyMessageAllowed(CoreFolder source, CoreFolder destination, out bool requireSharePointOperation)
		{
			requireSharePointOperation = false;
			if (!TeamMailboxClientOperations.IsLinkedFolder(source, false) && !TeamMailboxClientOperations.IsLinkedFolder(destination, false))
			{
				return true;
			}
			if (TeamMailboxClientOperations.IsLinkedFolder(source, false) && !TeamMailboxClientOperations.IsLinkedFolder(destination, false))
			{
				return false;
			}
			if (!TeamMailboxClientOperations.IsLinkedFolder(source, false) && TeamMailboxClientOperations.IsLinkedFolder(destination, false))
			{
				requireSharePointOperation = true;
				return true;
			}
			requireSharePointOperation = true;
			string valueOrDefault = source.PropertyBag.GetValueOrDefault<string>(CoreFolderSchema.LinkedSiteUrl, string.Empty);
			string valueOrDefault2 = destination.PropertyBag.GetValueOrDefault<string>(CoreFolderSchema.LinkedSiteUrl, string.Empty);
			if (string.IsNullOrEmpty(valueOrDefault))
			{
				throw new StoragePermanentException(new LocalizedString("sourceSiteUrl is null"));
			}
			if (string.IsNullOrEmpty(valueOrDefault2))
			{
				throw new StoragePermanentException(new LocalizedString("destinationSiteUrl is null"));
			}
			bool result;
			try
			{
				Uri x = new Uri(valueOrDefault);
				Uri y = new Uri(valueOrDefault2);
				result = UriComparer.IsEqual(x, y);
			}
			catch (UriFormatException)
			{
				result = false;
			}
			return result;
		}

		public static void ConfigureClientContext(ClientContext clientContext, ICredentials credentials, bool isOAuthCredential, bool httpDebugEnabled, int requestTimeoutInMilliSeconds)
		{
			clientContext.Credentials = credentials;
			clientContext.RequestTimeout = requestTimeoutInMilliSeconds;
			if (isOAuthCredential)
			{
				clientContext.FormDigestHandlingEnabled = false;
				clientContext.ExecutingWebRequest += delegate(object sender, WebRequestEventArgs args)
				{
					args.WebRequestExecutor.RequestHeaders.Add(HttpRequestHeader.Authorization, "Bearer");
					args.WebRequestExecutor.RequestHeaders.Add("client-request-id", Guid.NewGuid().ToString());
					args.WebRequestExecutor.RequestHeaders.Add("return-client-request-id", "true");
					args.WebRequestExecutor.WebRequest.PreAuthenticate = true;
					args.WebRequestExecutor.WebRequest.UserAgent = Utils.GetUserAgentStringForSiteMailboxRequests();
				};
			}
			if (httpDebugEnabled)
			{
				clientContext.ExecutingWebRequest += delegate(object sender, WebRequestEventArgs args)
				{
					args.WebRequestExecutor.WebRequest.Proxy = new WebProxy("127.0.0.1", 8888);
				};
			}
		}

		public void HandleSharePointPublishingError(SharePointException e, CoreItem originalItem)
		{
			MailboxSession mailboxSession = (MailboxSession)originalItem.Session;
			string str = string.Empty;
			using (MessageItem messageItem = new MessageItem(originalItem, true))
			{
				try
				{
					str = messageItem.Subject;
					messageItem.Save(SaveMode.NoConflictResolutionForceSave);
				}
				catch (MessageSubmissionExceededException)
				{
					using (MailboxSession mailboxSession2 = MailboxSession.OpenAsAdmin(mailboxSession.MailboxOwner, mailboxSession.InternalCulture, mailboxSession.ClientInfoString))
					{
						using (MessageItem messageItem2 = MessageItem.Create(mailboxSession2, mailboxSession2.GetDefaultFolderId(DefaultFolderType.Drafts)))
						{
							messageItem2.Subject = str + " - " + ClientStrings.FailedToUploadToSharePointTitle;
							TeamMailboxClientOperations.SetErrorBody(messageItem2, e, true);
							messageItem2.Recipients.Add(this.actAsUserParticipant);
							messageItem2.From = new Participant(mailboxSession2.MailboxOwner);
							messageItem2.Sender = new Participant(mailboxSession2.MailboxOwner);
							messageItem2.AutoResponseSuppress = AutoResponseSuppress.All;
							messageItem2.SendWithoutSavingMessage();
						}
					}
					throw;
				}
				messageItem.Load();
				byte[] array = messageItem.TryGetProperty(StoreObjectSchema.ParentEntryId) as byte[];
				if (array == null)
				{
					throw new StoragePermanentException(new LocalizedString("HandleSharePointPublishingError: parentFolderId is null"));
				}
				StoreObjectId folderId = StoreObjectId.FromProviderSpecificId(array, StoreObjectType.Message);
				StoreObjectId folderId2 = Utils.EnsureSyncIssueFolder(mailboxSession);
				using (Folder folder = Folder.Bind(mailboxSession, folderId))
				{
					using (Folder folder2 = Folder.Bind(mailboxSession, folderId2))
					{
						GroupOperationResult groupOperationResult = folder.MoveItems(folder2, new StoreObjectId[]
						{
							messageItem.Id.ObjectId
						}, null, null, true);
						if (groupOperationResult.ResultObjectIds == null || groupOperationResult.ResultObjectIds.Count != 1)
						{
							throw new StoragePermanentException(new LocalizedString("HandleSharePointPublishingError: moveResult.ResultObjectIds doesn't contain new StoreObjectId"));
						}
						using (MessageItem messageItem3 = Item.BindAsMessage(mailboxSession, groupOperationResult.ResultObjectIds[0]))
						{
							messageItem3.OpenAsReadWrite();
							messageItem3.From = this.actAsUserParticipant;
							messageItem3.Sender = new Participant(mailboxSession.MailboxOwner);
							messageItem3.Subject = str + " - " + ClientStrings.FailedToUploadToSharePointTitle;
							if (messageItem3.ClassName.StartsWith("IPM.Document", StringComparison.OrdinalIgnoreCase))
							{
								Attachment attachment = messageItem.AttachmentCollection.TryOpenFirstAttachment(AttachmentType.Stream);
								if (attachment == null)
								{
									goto IL_288;
								}
								using (attachment)
								{
									messageItem3.AttachmentCollection.RemoveAll();
									using (Attachment attachment3 = attachment.CreateCopy(messageItem3.AttachmentCollection, new BodyFormat?(messageItem3.Body.Format)))
									{
										attachment3.Save();
									}
									goto IL_288;
								}
							}
							messageItem3.AttachmentCollection.RemoveAll();
							using (Attachment attachment4 = messageItem3.AttachmentCollection.AddExistingItem(messageItem))
							{
								attachment4.Save();
							}
							IL_288:
							TeamMailboxClientOperations.SetErrorBody(messageItem3, e, true);
							messageItem3.ClassName = "IPM.Note";
							messageItem3.PropertyBag[MessageItemSchema.LinkedPendingState] = 0;
							messageItem3[ItemSchema.ReceivedTime] = ExDateTime.UtcNow;
							messageItem3.MarkAsUnread(true);
							messageItem3.Save(SaveMode.NoConflictResolutionForceSave);
							messageItem3.Load();
							this.GenerateNDRForPublishingError(mailboxSession, messageItem3);
						}
					}
				}
			}
		}

		public void OnCreateFolder(CoreFolder parentFolder, CoreFolder newFolder, string folderName)
		{
			Guid? guid = new Guid?(Guid.Empty);
			string value = null;
			string valueOrDefault = parentFolder.PropertyBag.GetValueOrDefault<string>(FolderSchema.LinkedSiteUrl, string.Empty);
			string valueOrDefault2 = parentFolder.PropertyBag.GetValueOrDefault<string>(FolderSchema.LinkedUrl, string.Empty);
			Guid valueOrDefault3 = parentFolder.PropertyBag.GetValueOrDefault<Guid>(FolderSchema.LinkedListId, Guid.Empty);
			this.SharePointCreateFolder(valueOrDefault, valueOrDefault2, valueOrDefault3, folderName, out guid, out value);
			newFolder.PropertyBag[FolderSchema.LinkedId] = guid;
			newFolder.PropertyBag[FolderSchema.LinkedUrl] = value;
			newFolder.PropertyBag[FolderSchema.LinkedSiteUrl] = valueOrDefault;
			newFolder.PropertyBag[FolderSchema.LinkedListId] = valueOrDefault3;
			newFolder.PropertyBag[StoreObjectSchema.ContainerClass] = "IPF.ShortcutFolder";
			Uri uri = new Uri(valueOrDefault);
			newFolder.PropertyBag[FolderSchema.LinkedSiteAuthorityUrl] = uri.AbsoluteUri;
		}

		public void OnDeleteFolder(Folder folder)
		{
			string valueOrDefault = folder.PropertyBag.GetValueOrDefault<string>(FolderSchema.LinkedUrl, string.Empty);
			string valueOrDefault2 = folder.PropertyBag.GetValueOrDefault<string>(FolderSchema.LinkedSiteUrl, string.Empty);
			if (!string.IsNullOrEmpty(valueOrDefault) && !string.IsNullOrEmpty(valueOrDefault2))
			{
				this.SharePointlDeleteFolder(valueOrDefault2, valueOrDefault);
			}
		}

		public void OnSaveMessage(CoreItem coreItem, Folder folder, out string fileUrl)
		{
			fileUrl = null;
			Guid uniqueId = Guid.Empty;
			int fileSize = 0;
			string fileName = null;
			using (MessageItem item = new MessageItem(coreItem, true))
			{
				fileUrl = item.PropertyBag.GetValueOrDefault<string>(MessageItemSchema.LinkedUrl, null);
				Guid valueOrDefault = item.PropertyBag.GetValueOrDefault<Guid>(MessageItemSchema.LinkedId, Guid.Empty);
				string valueOrDefault2 = folder.PropertyBag.GetValueOrDefault<string>(FolderSchema.LinkedUrl, null);
				string valueOrDefault3 = folder.PropertyBag.GetValueOrDefault<string>(FolderSchema.LinkedSiteUrl, string.Empty);
				if (!string.IsNullOrEmpty(fileUrl))
				{
					throw new StoragePermanentException(new LocalizedString(string.Format("linkedUrl:{0} cannot be set already", fileUrl)));
				}
				if (valueOrDefault != Guid.Empty)
				{
					throw new StoragePermanentException(new LocalizedString(string.Format("linkedId:{0} cannot be set already", valueOrDefault)));
				}
				if (!item.ClassName.StartsWith("IPM.Document", StringComparison.OrdinalIgnoreCase) && !item.ClassName.Equals("IPM.Note", StringComparison.OrdinalIgnoreCase))
				{
					throw new StoragePermanentException(new LocalizedString(string.Format("Doesn't support SharePoint publishing for message type {0}", item.ClassName)));
				}
				this.PublishToSharePoint(item, valueOrDefault3, valueOrDefault2, out fileUrl, out uniqueId, out fileSize, out fileName);
				string fileUrl2 = fileUrl;
				Utils.CommitMailboxOperationUnderNamedSemaphore(item.Session.MailboxGuid, delegate
				{
					LinkedItemProps linkedItemProps = Utils.FindItemBySharePointId((MailboxSession)item.Session, uniqueId, null);
					if (linkedItemProps != null && linkedItemProps.ParentId != null && linkedItemProps.ParentId.Equals(folder.Id.ObjectId))
					{
						throw new StoragePermanentException(new LocalizedString(string.Format("Ignore duplicate save on race condition for SharePoint document with Id {0}", uniqueId)));
					}
					this.PostSharePointOperation(item, fileUrl2, uniqueId, fileSize, fileName);
				});
			}
		}

		public GroupOperationResult OnImportDeleteMessages(CoreFolder folder, ContentsSynchronizationUploadContext contentsSynchronizationUploadContext, StoreObjectId[] messageIds)
		{
			Dictionary<StoreObjectId, SharePointException> dictionary = null;
			StoreObjectId[] array = this.InternalOnDeleteMessages(folder, messageIds, false, out dictionary);
			OperationResult operationResult = OperationResult.Succeeded;
			if (array != null)
			{
				operationResult = CoreItem.Delete(contentsSynchronizationUploadContext, DeleteItemFlags.HardDelete, array);
			}
			if (dictionary != null && dictionary.Count > 0)
			{
				foreach (KeyValuePair<StoreObjectId, SharePointException> keyValuePair in dictionary)
				{
					try
					{
						using (CoreItem coreItem = CoreItem.Bind(folder.Session, keyValuePair.Key))
						{
							coreItem.OpenAsReadWrite();
							ExDateTime valueOrDefault = coreItem.PropertyBag.GetValueOrDefault<ExDateTime>(ItemSchema.ReceivedTime, ExDateTime.UtcNow);
							coreItem.PropertyBag[ItemSchema.ReceivedTime] = valueOrDefault.AddTicks(1L);
							coreItem.SaveFlags |= PropertyBagSaveFlags.ForceNotificationPublish;
							Utils.AddUpdateHistoryEntry(coreItem, "Client_UnDelete", this.actAsUser.PrimarySmtpAddress.ToString(), ExDateTime.UtcNow);
							coreItem.Save(SaveMode.ResolveConflicts);
							coreItem.PropertyBag.Load(null);
							this.HandleSharePointImportDeleteError(keyValuePair.Value, coreItem);
						}
					}
					catch (ObjectNotFoundException)
					{
					}
				}
			}
			return new GroupOperationResult(operationResult, messageIds, null);
		}

		public GroupOperationResult OnDeleteMessages(CoreFolder folder, StoreObjectId[] messageIds)
		{
			Dictionary<StoreObjectId, SharePointException> dictionary = null;
			LocalizedException ex = null;
			StoreObjectId[] array = this.InternalOnDeleteMessages(folder, messageIds, true, out dictionary);
			if (dictionary != null && dictionary.Count > 0)
			{
				using (Dictionary<StoreObjectId, SharePointException>.ValueCollection.Enumerator enumerator = dictionary.Values.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						LocalizedException ex2 = enumerator.Current;
						ex = ex2;
					}
				}
			}
			GroupOperationResult groupOperationResult = null;
			if (array != null)
			{
				groupOperationResult = folder.Session.Delete(DeleteItemFlags.HardDelete, array).GroupOperationResults[0];
			}
			if (ex == null && groupOperationResult != null && groupOperationResult.Exception != null)
			{
				ex = groupOperationResult.Exception;
			}
			return new GroupOperationResult((ex == null) ? OperationResult.Succeeded : OperationResult.Failed, messageIds, ex);
		}

		public GroupOperationResult OnMoveCopyMessages(CoreFolder source, CoreFolder destination, StoreObjectId[] messageIds, bool isCopy)
		{
			List<StoreObjectId> list = new List<StoreObjectId>();
			string valueOrDefault = source.PropertyBag.GetValueOrDefault<string>(CoreFolderSchema.LinkedSiteUrl, string.Empty);
			string valueOrDefault2 = destination.PropertyBag.GetValueOrDefault<string>(CoreFolderSchema.LinkedSiteUrl, string.Empty);
			string valueOrDefault3 = destination.PropertyBag.GetValueOrDefault<string>(CoreFolderSchema.LinkedUrl, string.Empty);
			LocalizedException ex = null;
			try
			{
				foreach (StoreObjectId messageId in messageIds)
				{
					using (MessageItem messageItem = MessageItem.Bind(source.Session, messageId))
					{
						messageItem.OpenAsReadWrite();
						string valueOrDefault4 = messageItem.PropertyBag.GetValueOrDefault<string>(MessageItemSchema.LinkedUrl, null);
						string url = null;
						Guid empty = Guid.Empty;
						int fileSize = 0;
						string fileName = null;
						if (string.IsNullOrEmpty(valueOrDefault))
						{
							if (!string.IsNullOrEmpty(valueOrDefault4))
							{
								throw new StoragePermanentException(new LocalizedString("sourceFileUrl should be null for items in normal folder"));
							}
							this.PublishToSharePoint(messageItem, valueOrDefault2, valueOrDefault3, out url, out empty, out fileSize, out fileName);
						}
						else
						{
							if (string.IsNullOrEmpty(valueOrDefault4))
							{
								throw new StoragePermanentException(new LocalizedString("sourceFileUrl should NOT be null for items in Shortcut folder"));
							}
							this.SharePointMoveOrCopyFile(isCopy, valueOrDefault, valueOrDefault4, valueOrDefault3, out empty, out url);
						}
						MessageItem messageItem2;
						if (isCopy)
						{
							GroupOperationResult groupOperationResult = source.CopyItems(destination, new StoreObjectId[]
							{
								messageItem.Id.ObjectId
							}, null, null, null, true);
							if (groupOperationResult.OperationResult != OperationResult.Succeeded)
							{
								throw groupOperationResult.Exception;
							}
							messageItem2 = MessageItem.Bind(destination.Session, groupOperationResult.ResultObjectIds[0]);
							messageItem2.OpenAsReadWrite();
						}
						else
						{
							messageItem2 = messageItem;
						}
						this.PostSharePointOperation(messageItem2, url, empty, fileSize, fileName);
						if (isCopy)
						{
							messageItem2.Dispose();
						}
						else
						{
							list.Add(messageItem2.Id.ObjectId);
						}
					}
				}
			}
			catch (SharePointException ex2)
			{
				ex = ex2;
			}
			catch (StorageTransientException innerException)
			{
				ex = new LocalizedException(new LocalizedString("OnMoveCopyMessages: failed to save changes to Exchange with StorageTransientException"), innerException);
			}
			catch (StoragePermanentException innerException2)
			{
				ex = new LocalizedException(new LocalizedString("OnMoveCopyMessages: failed to save changes to Exchange with StoragePermanentException"), innerException2);
			}
			GroupOperationResult groupOperationResult2 = null;
			if (!isCopy && list.Count > 0)
			{
				groupOperationResult2 = source.MoveItems(destination, list.ToArray(), null, null, null);
			}
			if (ex == null && groupOperationResult2 != null && groupOperationResult2.Exception != null)
			{
				ex = groupOperationResult2.Exception;
			}
			return new GroupOperationResult((ex == null) ? OperationResult.Succeeded : OperationResult.Failed, messageIds, ex);
		}

		protected virtual void SharePointCreateFolder(string siteUrl, string parentFolderUrl, Guid listId, string folderName, out Guid? uniqueId, out string folderUrl)
		{
			uniqueId = new Guid?(Guid.Empty);
			folderUrl = null;
			string absolutePath = new Uri(parentFolderUrl).AbsolutePath;
			try
			{
				using (ClientContext clientContext = new ClientContext(siteUrl))
				{
					TeamMailboxClientOperations.ConfigureClientContext(clientContext, this.credentials, this.isOAuthCredential, this.httpDebugEnabled, (int)this.requestTimeout.TotalMilliseconds);
					Folder folderByServerRelativeUrl = clientContext.Web.GetFolderByServerRelativeUrl(absolutePath);
					Uri uri = new Uri(parentFolderUrl + "/" + folderName);
					Folder folder = folderByServerRelativeUrl.Folders.Add(uri.AbsoluteUri);
					clientContext.Load<Folder>(folderByServerRelativeUrl, new Expression<Func<Folder, object>>[0]);
					clientContext.Load<Folder>(folder, new Expression<Func<Folder, object>>[0]);
					clientContext.ExecuteQuery();
					List byId = clientContext.Web.Lists.GetById(listId);
					if (byId == null)
					{
						throw new SharePointException(parentFolderUrl, new LocalizedString(string.Format("SharePointCreateFolder: GetById return null for Id {0}, site {1}", listId, siteUrl)));
					}
					CamlQuery camlQuery = new CamlQuery();
					camlQuery.ViewXml = "<View Scope='RecursiveAll'><Query><Where><Eq><FieldRef Name='FileRef' /><Value Type='Text'>" + Uri.UnescapeDataString(uri.AbsolutePath) + "</Value></Eq></Where></Query></View>";
					byId.GetItems(camlQuery);
					ListItemCollection items = byId.GetItems(camlQuery);
					clientContext.Load<ListItemCollection>(items, new Expression<Func<ListItemCollection, object>>[0]);
					clientContext.ExecuteQuery();
					using (IEnumerator<ListItem> enumerator = items.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							ListItem listItem = enumerator.Current;
							uniqueId = (listItem["UniqueId"] as Guid?);
						}
					}
					if (uniqueId == null || uniqueId == Guid.Empty)
					{
						throw new SharePointException(parentFolderUrl, new LocalizedString(string.Format("SharePointCreateFolder:UniqueId returned null for {0}, siteUrl {1}", uri.AbsoluteUri, siteUrl)));
					}
					folderUrl = uri.AbsoluteUri;
				}
			}
			catch (ClientRequestException e)
			{
				throw new SharePointException(parentFolderUrl, e);
			}
			catch (ServerException e2)
			{
				throw new SharePointException(parentFolderUrl, e2);
			}
			catch (WebException e3)
			{
				throw new SharePointException(parentFolderUrl, e3, true);
			}
		}

		protected virtual void SharePointlDeleteFolder(string siteUrl, string folderUrl)
		{
			Uri uri = new Uri(folderUrl);
			try
			{
				using (ClientContext clientContext = new ClientContext(siteUrl))
				{
					TeamMailboxClientOperations.ConfigureClientContext(clientContext, this.credentials, this.isOAuthCredential, this.httpDebugEnabled, (int)this.requestTimeout.TotalMilliseconds);
					bool flag = this.SharePointIsRecycleBinEnabled(siteUrl, clientContext);
					Folder folderByServerRelativeUrl = clientContext.Web.GetFolderByServerRelativeUrl(uri.AbsolutePath);
					if (flag)
					{
						folderByServerRelativeUrl.Recycle();
					}
					else
					{
						folderByServerRelativeUrl.DeleteObject();
					}
					clientContext.ExecuteQuery();
				}
			}
			catch (ClientRequestException e)
			{
				throw new SharePointException(folderUrl, e);
			}
			catch (ServerException e2)
			{
				throw new SharePointException(folderUrl, e2);
			}
			catch (WebException e3)
			{
				throw new SharePointException(folderUrl, e3, true);
			}
		}

		protected virtual void SharePointCreateFile(string siteUrl, string folderUrl, string fileName, Stream contentStream, out Guid uniqueId, out string fileUrl, out int fileSize)
		{
			try
			{
				using (ClientContext clientContext = new ClientContext(siteUrl))
				{
					TeamMailboxClientOperations.ConfigureClientContext(clientContext, this.credentials, this.isOAuthCredential, this.httpDebugEnabled, (int)this.requestTimeout.TotalMilliseconds);
					Uri uri = new Uri(folderUrl);
					string leftPart = uri.GetLeftPart(UriPartial.Authority);
					string absolutePath = uri.AbsolutePath;
					Folder folderByServerRelativeUrl = clientContext.Web.GetFolderByServerRelativeUrl(absolutePath);
					clientContext.Load<FileCollection>(folderByServerRelativeUrl.Files, new Expression<Func<FileCollection, object>>[0]);
					clientContext.ExecuteQuery();
					FileCreationInformation fileCreationInformation = new FileCreationInformation();
					fileCreationInformation.Url = absolutePath + "/" + fileName;
					fileCreationInformation.ContentStream = contentStream;
					fileCreationInformation.Overwrite = false;
					File file = folderByServerRelativeUrl.Files.Add(fileCreationInformation);
					clientContext.Load<File>(file, new Expression<Func<File, object>>[]
					{
						(File item) => item.ListItemAllFields,
						(File item) => item.ETag,
						(File item) => item.ServerRelativeUrl
					});
					clientContext.ExecuteQuery();
					this.RetrieveFileInfoFromAllListItemProperty(file, fileName, out uniqueId, out fileSize);
					fileUrl = new Uri(new Uri(leftPart), file.ServerRelativeUrl).AbsoluteUri;
				}
			}
			catch (ClientRequestException e)
			{
				throw new SharePointException(folderUrl, e);
			}
			catch (ServerException e2)
			{
				throw new SharePointException(folderUrl, e2);
			}
			catch (WebException e3)
			{
				throw new SharePointException(folderUrl, e3, true);
			}
		}

		protected virtual void SharePointDeleteFile(string siteUrl, string fileUrl, bool isSharePointRecycleBinEnabled)
		{
			try
			{
				using (ClientContext clientContext = new ClientContext(siteUrl))
				{
					TeamMailboxClientOperations.ConfigureClientContext(clientContext, this.credentials, this.isOAuthCredential, this.httpDebugEnabled, (int)this.requestTimeout.TotalMilliseconds);
					string absolutePath = new Uri(fileUrl).AbsolutePath;
					File fileByServerRelativeUrl = clientContext.Web.GetFileByServerRelativeUrl(absolutePath);
					if (isSharePointRecycleBinEnabled)
					{
						fileByServerRelativeUrl.Recycle();
					}
					else
					{
						fileByServerRelativeUrl.DeleteObject();
					}
					clientContext.ExecuteQuery();
				}
			}
			catch (ClientRequestException e)
			{
				throw new SharePointException(fileUrl, e);
			}
			catch (ServerException e2)
			{
				throw new SharePointException(fileUrl, e2);
			}
			catch (WebException e3)
			{
				throw new SharePointException(fileUrl, e3, true);
			}
		}

		protected virtual void SharePointMoveOrCopyFile(bool isCopy, string siteUrl, string sourceFileUrl, string destinationFolderUrl, out Guid uniqueId, out string destinationFileUrl)
		{
			try
			{
				using (ClientContext clientContext = new ClientContext(siteUrl))
				{
					TeamMailboxClientOperations.ConfigureClientContext(clientContext, this.credentials, this.isOAuthCredential, this.httpDebugEnabled, (int)this.requestTimeout.TotalMilliseconds);
					Uri uri = new Uri(sourceFileUrl);
					string fileNameFromUri = Utils.GetFileNameFromUri(uri);
					File fileByServerRelativeUrl = clientContext.Web.GetFileByServerRelativeUrl(uri.AbsolutePath);
					Uri uri2 = new Uri(destinationFolderUrl + "/" + fileNameFromUri);
					File fileByServerRelativeUrl2 = clientContext.Web.GetFileByServerRelativeUrl(uri2.AbsolutePath);
					if (isCopy)
					{
						fileByServerRelativeUrl.CopyTo(uri2.AbsoluteUri, false);
					}
					else
					{
						fileByServerRelativeUrl.MoveTo(uri2.AbsoluteUri, 0);
					}
					clientContext.Load<File>(fileByServerRelativeUrl, new Expression<Func<File, object>>[0]);
					clientContext.Load<File>(fileByServerRelativeUrl2, new Expression<Func<File, object>>[]
					{
						(File item) => item.ListItemAllFields
					});
					clientContext.ExecuteQuery();
					int num = 0;
					this.RetrieveFileInfoFromAllListItemProperty(fileByServerRelativeUrl2, fileNameFromUri, out uniqueId, out num);
					destinationFileUrl = uri2.AbsoluteUri;
				}
			}
			catch (ClientRequestException e)
			{
				throw new SharePointException(sourceFileUrl, e);
			}
			catch (ServerException e2)
			{
				throw new SharePointException(sourceFileUrl, e2);
			}
			catch (WebException e3)
			{
				throw new SharePointException(sourceFileUrl, e3, true);
			}
		}

		protected virtual bool SharePointIsRecycleBinEnabled(string siteUrl)
		{
			bool result;
			using (ClientContext clientContext = new ClientContext(siteUrl))
			{
				TeamMailboxClientOperations.ConfigureClientContext(clientContext, this.credentials, this.isOAuthCredential, this.httpDebugEnabled, (int)this.requestTimeout.TotalMilliseconds);
				result = this.SharePointIsRecycleBinEnabled(siteUrl, clientContext);
			}
			return result;
		}

		private static string GenerateMsgFileName(MessageItem messageItem)
		{
			string text = null;
			if (messageItem.From != null && !string.IsNullOrEmpty(messageItem.From.DisplayName))
			{
				text = messageItem.From.DisplayName;
			}
			else if (messageItem.Sender != null && !string.IsNullOrEmpty(messageItem.Sender.DisplayName))
			{
				text = messageItem.Sender.DisplayName;
			}
			text = Utils.FilterInvalidSharePointFileNameChars(text, int.MaxValue);
			string text2 = messageItem.ReceivedTime.ToString("HHmmss");
			int num = 128 - text.Length - text2.Length - 6;
			if (num <= 0)
			{
				throw new StoragePermanentException(new LocalizedString("GenerateMsgFileName: Unable to generate Msg FileName because length is greater than SharePoint limit."));
			}
			string text3 = messageItem.Subject;
			text3 = Utils.FilterInvalidSharePointFileNameChars(text3, num);
			return string.Format("{0}-{1}-{2}.msg", text3, text, text2);
		}

		private bool SharePointIsRecycleBinEnabled(string siteUrl, ClientContext clientContext)
		{
			bool recycleBinEnabled;
			try
			{
				clientContext.Load<Web>(clientContext.Web, new Expression<Func<Web, object>>[0]);
				clientContext.ExecuteQuery();
				recycleBinEnabled = clientContext.Web.RecycleBinEnabled;
			}
			catch (ClientRequestException e)
			{
				throw new SharePointException(siteUrl, e);
			}
			catch (ServerException e2)
			{
				throw new SharePointException(siteUrl, e2);
			}
			catch (WebException e3)
			{
				throw new SharePointException(siteUrl, e3, true);
			}
			return recycleBinEnabled;
		}

		private void PublishToSharePoint(MessageItem messageItem, string siteUrl, string folderUrl, out string fileUrl, out Guid uniqueId, out int fileSize, out string fileName)
		{
			fileUrl = null;
			uniqueId = Guid.Empty;
			fileSize = 0;
			fileName = null;
			Attachment attachment = null;
			Stream stream = null;
			try
			{
				if (messageItem.ClassName.StartsWith("IPM.Document", StringComparison.OrdinalIgnoreCase))
				{
					attachment = messageItem.AttachmentCollection.TryOpenFirstAttachment(AttachmentType.Stream);
					if (attachment == null)
					{
						throw new StoragePermanentException(new LocalizedString("PublishToSharePoint: Free-doc message doesn't have attachment"));
					}
					fileName = (attachment.PropertyBag.TryGetProperty(AttachmentSchema.AttachLongFileName) as string);
					if (string.IsNullOrEmpty(fileName))
					{
						throw new StoragePermanentException(new LocalizedString("PublishToSharePoint: attachment doesn't have file name"));
					}
					stream = attachment.PropertyBag.OpenPropertyStream(AttachmentSchema.AttachDataBin, PropertyOpenMode.ReadOnly);
				}
				else
				{
					if (!messageItem.ClassName.Equals("IPM.Note", StringComparison.OrdinalIgnoreCase))
					{
						throw new StoragePermanentException(new LocalizedString("PublishToSharePoint: messageItem must be IPM.Document* or IPM.Note"));
					}
					if (messageItem.PropertyBag.GetValueOrDefault<string>(InternalSchema.MapiSubject, null) == null)
					{
						messageItem.PropertyBag[InternalSchema.MapiSubject] = messageItem.Subject;
					}
					fileName = TeamMailboxClientOperations.GenerateMsgFileName(messageItem);
					using (DisposeGuard disposeGuard = default(DisposeGuard))
					{
						stream = Streams.CreateTemporaryStorageStream();
						disposeGuard.Add<Stream>(stream);
						messageItem.Load(StoreObjectSchema.ContentConversionProperties);
						ItemConversion.ConvertItemToMsgStorage(messageItem, stream, new OutboundConversionOptions(messageItem.Session.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid), string.Empty));
						stream.Position = 0L;
						disposeGuard.Success();
					}
				}
				using (stream)
				{
					this.SharePointCreateFile(siteUrl, folderUrl, fileName, stream, out uniqueId, out fileUrl, out fileSize);
				}
			}
			finally
			{
				if (attachment != null)
				{
					attachment.Dispose();
				}
			}
		}

		private void RetrieveFileInfoFromAllListItemProperty(File file, string fileName, out Guid uniqueId, out int fileSize)
		{
			uniqueId = Guid.Empty;
			fileSize = 0;
			if (file.ListItemAllFields != null && file.ListItemAllFields.FieldValues != null)
			{
				object obj = null;
				if (file.ListItemAllFields.FieldValues.TryGetValue("UniqueId", out obj))
				{
					uniqueId = (Guid)obj;
				}
				if (file.ListItemAllFields.FieldValues.TryGetValue("File_x0020_Size", out obj))
				{
					int.TryParse(obj as string, out fileSize);
				}
			}
			if (uniqueId == Guid.Empty)
			{
				throw new SharePointException(string.Empty, new LocalizedString(string.Format("RetrieveFileInfoFromAllListItemProperty:Unique id is missing for {0}", fileName)));
			}
		}

		private void PostSharePointOperation(MessageItem messageItem, string url, Guid uniqueId, int fileSize, string fileName)
		{
			messageItem.AttachmentCollection.RemoveAll();
			using (Attachment attachment = messageItem.AttachmentCollection.Create(AttachmentType.Reference))
			{
				attachment.PropertyBag[AttachmentSchema.AttachLongPathName] = url;
				attachment.PropertyBag.SetOrDeleteProperty(AttachmentSchema.AttachDataBin, null);
				attachment.PropertyBag[AttachmentSchema.AttachMethod] = 2;
				attachment.Save();
			}
			messageItem.Save(SaveMode.NoConflictResolutionForceSave);
			messageItem.Load();
			using (MailboxSession mailboxSession = MailboxSession.OpenAsAdmin(((MailboxSession)messageItem.Session).MailboxOwner, messageItem.Session.InternalCulture, messageItem.Session.ClientInfoString))
			{
				using (MessageItem messageItem2 = MessageItem.Bind(mailboxSession, messageItem.Id.ObjectId))
				{
					messageItem2.OpenAsReadWrite();
					if (messageItem2.ClassName.Equals("IPM.Note", StringComparison.OrdinalIgnoreCase))
					{
						messageItem2.ClassName = "IPM.Document.msgfile";
						messageItem2.CoreItem.PropertyBag.SetOrDeleteProperty(ItemSchema.HtmlBody, null);
						messageItem2.CoreItem.PropertyBag.SetOrDeleteProperty(ItemSchema.RtfBody, null);
						messageItem2.CoreItem.PropertyBag.SetOrDeleteProperty(ItemSchema.TextBody, null);
						if (!string.IsNullOrEmpty(fileName))
						{
							messageItem2.Subject = fileName;
						}
					}
					using (Attachment attachment2 = messageItem2.AttachmentCollection.TryOpenFirstAttachment(AttachmentType.Reference))
					{
						attachment2.PropertyBag[AttachmentSchema.AttachLongPathName] = url;
						attachment2.PropertyBag.SetOrDeleteProperty(AttachmentSchema.AttachDataBin, null);
						attachment2.PropertyBag[AttachmentSchema.AttachMethod] = 2;
						attachment2.Save();
					}
					messageItem2.PropertyBag[MessageItemSchema.LinkedId] = uniqueId;
					messageItem2.PropertyBag[MessageItemSchema.LinkedUrl] = url;
					messageItem2.PropertyBag[MessageItemSchema.LinkedDocumentSize] = fileSize;
					messageItem2.PropertyBag[MessageItemSchema.LinkedPendingState] = 0;
					messageItem2.From = this.actAsUserParticipant;
					messageItem2.Sender = messageItem2.From;
					messageItem2[ItemSchema.LastModifiedBy] = this.actAsUser.DisplayName;
					messageItem2.MarkAsUnread(true);
					messageItem2.SaveFlags |= PropertyBagSaveFlags.ForceNotificationPublish;
					Utils.AddUpdateHistoryEntry((CoreItem)messageItem2.CoreItem, "Client_Publish", this.actAsUser.PrimarySmtpAddress.ToString(), ExDateTime.UtcNow);
					messageItem2.Save(SaveMode.NoConflictResolutionForceSave);
					messageItem.PropertyBag[MessageItemSchema.LinkedId] = uniqueId;
					messageItem.PropertyBag[MessageItemSchema.LinkedUrl] = url;
				}
			}
		}

		private StoreObjectId[] InternalOnDeleteMessages(CoreFolder folder, StoreObjectId[] messageIds, bool exitOnFirstFailure, out Dictionary<StoreObjectId, SharePointException> failedEntries)
		{
			List<StoreObjectId> list = new List<StoreObjectId>();
			failedEntries = new Dictionary<StoreObjectId, SharePointException>();
			string valueOrDefault = folder.PropertyBag.GetValueOrDefault<string>(FolderSchema.LinkedSiteUrl, string.Empty);
			bool isSharePointRecycleBinEnabled = true;
			try
			{
				isSharePointRecycleBinEnabled = this.SharePointIsRecycleBinEnabled(valueOrDefault);
			}
			catch (SharePointException value)
			{
				foreach (StoreObjectId key in messageIds)
				{
					failedEntries.Add(key, value);
				}
				return null;
			}
			foreach (StoreObjectId storeObjectId in messageIds)
			{
				try
				{
					using (CoreItem coreItem = CoreItem.Bind(folder.Session, storeObjectId))
					{
						string valueOrDefault2 = coreItem.PropertyBag.GetValueOrDefault<string>(CoreItemSchema.LinkedUrl, null);
						Guid? valueAsNullable = coreItem.PropertyBag.GetValueAsNullable<Guid>(CoreItemSchema.LinkedId);
						if (!string.IsNullOrEmpty(valueOrDefault2) && valueAsNullable != null)
						{
							this.SharePointDeleteFile(valueOrDefault, valueOrDefault2, isSharePointRecycleBinEnabled);
						}
						list.Add(coreItem.Id.ObjectId);
					}
				}
				catch (SharePointException value2)
				{
					failedEntries.Add(storeObjectId, value2);
					if (exitOnFirstFailure)
					{
						break;
					}
				}
				catch (ObjectNotFoundException)
				{
				}
			}
			if (list.Count <= 0)
			{
				return null;
			}
			return list.ToArray();
		}

		private void HandleSharePointImportDeleteError(SharePointException e, CoreItem originalItem)
		{
			MailboxSession mailboxSession = (MailboxSession)originalItem.Session;
			StoreObjectId parentFolderId = Utils.EnsureSyncIssueFolder(mailboxSession);
			using (MessageItem messageItem = new MessageItem(originalItem, true))
			{
				using (MessageItem messageItem2 = MessageItem.CloneMessage(mailboxSession, parentFolderId, messageItem))
				{
					messageItem2.OpenAsReadWrite();
					messageItem2.From = this.actAsUserParticipant;
					messageItem2.Sender = new Participant(mailboxSession.MailboxOwner);
					messageItem2.Subject = messageItem.Subject + " - " + ClientStrings.FailedToDeleteFromSharePointTitle;
					TeamMailboxClientOperations.SetErrorBody(messageItem2, e, false);
					messageItem2.ClassName = "IPM.Note";
					messageItem2.AttachmentCollection.RemoveAll();
					messageItem2[ItemSchema.ReceivedTime] = ExDateTime.UtcNow;
					messageItem2.MarkAsUnread(true);
					messageItem2.Save(SaveMode.NoConflictResolutionForceSave);
					messageItem2.Load();
					this.GenerateNDRForPublishingError(mailboxSession, messageItem2);
				}
			}
		}

		private void GenerateNDRForPublishingError(MailboxSession session, MessageItem syncIssueMessage)
		{
			using (MailboxSession mailboxSession = MailboxSession.OpenAsAdmin(((MailboxSession)syncIssueMessage.Session).MailboxOwner, syncIssueMessage.Session.InternalCulture, syncIssueMessage.Session.ClientInfoString))
			{
				using (MessageItem messageItem = MessageItem.CloneMessage(mailboxSession, syncIssueMessage.ParentId, syncIssueMessage))
				{
					messageItem.Recipients.Clear();
					messageItem.Recipients.Add(this.actAsUserParticipant);
					messageItem.From = new Participant(mailboxSession.MailboxOwner);
					messageItem.Sender = new Participant(mailboxSession.MailboxOwner);
					messageItem.AutoResponseSuppress = AutoResponseSuppress.All;
					messageItem.SendWithoutSavingMessage();
				}
			}
		}

		private static void SetErrorBody(MessageItem message, SharePointException e, bool isPublishingFailure)
		{
			using (TextWriter textWriter = message.Body.OpenTextWriter(BodyFormat.TextHtml))
			{
				message.Body.Reset();
				using (HtmlWriter htmlWriter = new HtmlWriter(textWriter))
				{
					htmlWriter.WriteStartTag(HtmlTagId.Html);
					htmlWriter.WriteStartTag(HtmlTagId.Body);
					htmlWriter.WriteStartTag(HtmlTagId.P);
					if (isPublishingFailure)
					{
						htmlWriter.WriteText(ClientStrings.FailedToUploadToSharePointErrorText(message.Subject, e.RequestUrl));
					}
					else
					{
						htmlWriter.WriteText(ClientStrings.FailedToDeleteFromSharePointErrorText(message.Subject, e.RequestUrl));
					}
					htmlWriter.WriteEndTag(HtmlTagId.P);
					htmlWriter.WriteStartTag(HtmlTagId.P);
					htmlWriter.WriteText(e.DiagnosticInfo);
					htmlWriter.WriteEndTag(HtmlTagId.P);
					htmlWriter.WriteEndTag(HtmlTagId.Body);
					htmlWriter.WriteEndTag(HtmlTagId.Html);
				}
			}
		}

		private readonly ICredentials credentials;

		private readonly TimeSpan requestTimeout;

		private readonly bool isOAuthCredential;

		private readonly bool httpDebugEnabled;

		private readonly MiniRecipient actAsUser;

		private readonly Participant actAsUserParticipant;

		private static readonly PropertyDefinition[] shorcutFolderProperties = new PropertyDefinition[]
		{
			StoreObjectSchema.ContainerClass,
			CoreFolderSchema.LinkedUrl,
			CoreFolderSchema.LinkedSiteUrl
		};
	}
}
