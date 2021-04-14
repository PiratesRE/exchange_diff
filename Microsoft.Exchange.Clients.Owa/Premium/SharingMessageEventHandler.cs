using System;
using System.Globalization;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventNamespace("SharingMessage")]
	[OwaEventSegmentation(Feature.Calendar)]
	internal sealed class SharingMessageEventHandler : MessageEventHandler
	{
		public static void Register()
		{
			OwaEventRegistry.RegisterHandler(typeof(SharingMessageEventHandler));
		}

		[OwaEventParameter("iCalUrl", typeof(string))]
		[OwaEvent("SubscribeToInternetCalendar")]
		public void SubscribeToInternetCalendar()
		{
			string iCalUrlString = (string)base.GetParameter("iCalUrl");
			try
			{
				SubscribeResultsWebCal subscribeResultsWebCal = WebCalendar.Subscribe(base.UserContext.MailboxSession, iCalUrlString, null);
				OwaStoreObjectId newFolderId = OwaStoreObjectId.CreateFromMailboxFolderId(subscribeResultsWebCal.LocalFolderId);
				NavigationHost.RenderFavoritesAndNavigationTrees(this.Writer, base.UserContext, newFolderId, new NavigationNodeGroupSection[]
				{
					NavigationNodeGroupSection.Calendar
				});
			}
			catch (InvalidSharingDataException)
			{
				this.SanitizingWriter.Write("<div id=\"err\" _msg=\"badUri\"></div>");
			}
			catch (NotSupportedWithMailboxVersionException)
			{
				throw new OwaInvalidRequestException("Your account isn't set up to allow calendars to be added from the Internet.");
			}
		}

		[OwaEventParameter("CK", typeof(string))]
		[OwaEventParameter("Id", typeof(OwaStoreObjectId))]
		[OwaEvent("AddSharedCalendar")]
		public void AddSharedCalendar()
		{
			using (SharingMessageItem requestItem = base.GetRequestItem<SharingMessageItem>(new PropertyDefinition[0]))
			{
				if (requestItem == null || !requestItem.SharingMessageType.IsInvitationOrAcceptOfRequest || requestItem.SharedFolderType != DefaultFolderType.Calendar)
				{
					throw new OwaInvalidRequestException("Not a calendar sharing message");
				}
				this.ValidateSharingAction(SharingAction.AddCalendar, requestItem);
				SubscribeResults subscribeResults = null;
				try
				{
					subscribeResults = requestItem.SubscribeAndOpen();
				}
				catch (NotSupportedWithMailboxVersionException)
				{
					throw new OwaInvalidRequestException("Your account isn't set up to allow calendars to be added from the Internet.");
				}
				catch (StoragePermanentException ex)
				{
					string message = string.Format("Unable to subscribe shared calendar folder. Exception {0}", ex.Message);
					ExTraceGlobals.CalendarCallTracer.TraceDebug(0L, message);
					this.RenderSubscriptionError(ex);
					return;
				}
				if (subscribeResults is SubscribeResultsInternal)
				{
					SubscribeResultsInternal subscribeResultsInternal = (SubscribeResultsInternal)subscribeResults;
					string text = StoreEntryId.TryParseMailboxLegacyDN(subscribeResultsInternal.RemoteMailboxId);
					ExchangePrincipal exchangePrincipal;
					if (base.UserContext.DelegateSessionManager.TryGetExchangePrincipal(text, out exchangePrincipal))
					{
						try
						{
							if (requestItem.IsSharedFolderPrimary)
							{
								if (!string.Equals(text, base.UserContext.MailboxOwnerLegacyDN, StringComparison.OrdinalIgnoreCase))
								{
									NavigationNodeCollection.AddGSCalendarToSharedFoldersGroup(base.UserContext, exchangePrincipal);
								}
							}
							else
							{
								using (OwaStoreObjectIdSessionHandle owaStoreObjectIdSessionHandle = new OwaStoreObjectIdSessionHandle(exchangePrincipal, base.UserContext))
								{
									OwaStoreObjectId folderId = OwaStoreObjectId.CreateFromSessionFolderId(base.UserContext, owaStoreObjectIdSessionHandle.Session, subscribeResultsInternal.RemoteFolderId);
									using (Folder folder = Utilities.GetFolder<Folder>(base.UserContext, folderId, FolderList.FolderTreeQueryProperties))
									{
										if (!Utilities.CanReadItemInFolder(folder))
										{
											this.RenderNotFoundResponse();
											return;
										}
										NavigationNodeCollection.AddNonMailFolderToSharedFoldersGroup(base.UserContext, folder, NavigationNodeGroupSection.Calendar);
									}
								}
							}
							goto IL_1A2;
						}
						catch (OwaSharedFromOlderVersionException)
						{
							this.RenderSharedFromE12Response();
							return;
						}
						catch (ObjectNotFoundException)
						{
							this.RenderNotFoundResponse();
							return;
						}
					}
					this.RenderNotFoundResponse();
					return;
				}
				if (subscribeResults is SubscribeResultsExternal)
				{
					OwaStoreObjectId folderId = OwaStoreObjectId.CreateFromMailboxFolderId(((SubscribeResultsExternal)subscribeResults).LocalFolderId);
				}
				IL_1A2:
				this.RenderChangeKey(requestItem);
				ExDateTime now = ExDateTime.GetNow(base.UserContext.TimeZone);
				this.Writer.Write("<div id=\"subInfo\">");
				this.Writer.Write(SanitizedHtmlString.Format(LocalizedStrings.GetHtmlEncoded(388258761), new object[]
				{
					now.ToString(base.UserContext.UserOptions.DateFormat),
					now.ToString(base.UserContext.UserOptions.TimeFormat)
				}));
				this.Writer.Write("</div>");
				RenderingUtilities.RenderNavigationTreeDirtyFlag(this.Writer, base.UserContext, NavigationTreeDirtyFlag.Calendar, new NavigationModule[]
				{
					NavigationModule.Calendar
				});
			}
		}

		[OwaEvent("AllowSharingRequest")]
		[OwaEventParameter("Id", typeof(OwaStoreObjectId))]
		[OwaEventParameter("CK", typeof(string))]
		public void AllowSharingRequest()
		{
			using (SharingMessageItem requestItem = base.GetRequestItem<SharingMessageItem>(new PropertyDefinition[0]))
			{
				this.ValidateSharingAction(SharingAction.AllowRequest, requestItem);
				try
				{
					using (SharingMessageItem sharingMessageItem = requestItem.AcceptRequest(base.UserContext.DraftsFolderId))
					{
						sharingMessageItem.Save(SaveMode.NoConflictResolution);
						sharingMessageItem.Load();
						this.RenderSharingResponseMessageId(sharingMessageItem);
					}
				}
				catch (InvalidSharingTargetRecipientException ex)
				{
					string message = string.Format("Unable to allow sharing request. Exception {0}", ex.Message);
					ExTraceGlobals.CalendarCallTracer.TraceDebug(0L, message);
					this.RenderSubscriptionError(ex);
				}
			}
		}

		private void CheckIsCalendarSharingRequest(SharingMessageItem item)
		{
			if (item == null || !item.SharingMessageType.IsRequest || item.SharedFolderType != DefaultFolderType.Calendar)
			{
				throw new OwaInvalidRequestException("Not a calendar sharing request");
			}
		}

		private void RenderSharingResponseMessageId(SharingMessageItem responseItem)
		{
			this.SanitizingWriter.Write("<div id=divOp _sOp=mr>");
			this.SanitizingWriter.Write(OwaStoreObjectId.CreateFromStoreObject(responseItem).ToBase64String());
			this.SanitizingWriter.Write("</div>");
		}

		private void ValidateSharingAction(SharingAction action, SharingMessageItem item)
		{
			if (action == SharingAction.AllowRequest && (item == null || !item.SharingMessageType.IsRequest || item.SharedFolderType != DefaultFolderType.Calendar))
			{
				throw new OwaInvalidRequestException("Not a calendar sharing request");
			}
			string text = string.Empty;
			string text2 = string.Empty;
			switch (action)
			{
			case SharingAction.AllowRequest:
				text = "allow a sharing request";
				break;
			case SharingAction.AddCalendar:
				text = "add a shared calendar";
				break;
			}
			if (base.UserContext.IsInOtherMailbox(item))
			{
				text2 = "other user's mailbox";
			}
			else if (item.IsDraft)
			{
				text2 = "draft folder";
			}
			else if (Utilities.IsPublic(item))
			{
				text2 = "public folder";
			}
			else
			{
				if (!Utilities.IsItemInDefaultFolder(item, DefaultFolderType.SentItems))
				{
					return;
				}
				text2 = "sent folder";
			}
			throw new OwaInvalidRequestException(string.Format(CultureInfo.InvariantCulture, "Cannot {0} from {1}", new object[]
			{
				text,
				text2
			}));
		}

		[OwaEventParameter("SharingLevel", typeof(SharingLevel), false, true)]
		[OwaEventParameter("CK", typeof(string), false, true)]
		[OwaEventParameter("Subj", typeof(string), false, true)]
		[OwaEventParameter("Imp", typeof(Importance), false, true)]
		[OwaEventParameter("Sensitivity", typeof(Sensitivity), false, true)]
		[OwaEventParameter("Body", typeof(string), false, true)]
		[OwaEventParameter("To", typeof(RecipientInfo), true, true)]
		[OwaEventParameter("Recips", typeof(RecipientInfoAC), true, true)]
		[OwaEventParameter("Text", typeof(bool), false, true)]
		[OwaEventParameter("DeliveryRcpt", typeof(bool), false, true)]
		[OwaEventParameter("ReadRcpt", typeof(bool), false, true)]
		[OwaEventParameter("Id", typeof(OwaStoreObjectId), false, false)]
		[OwaEventParameter("RequestPermission", typeof(bool), false, true)]
		[OwaEventParameter("UpdRcpAs", typeof(bool))]
		[OwaEvent("AutoSave")]
		public void AutoSave()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "SharingMessageEventHandler.AutoSave");
			this.TryProcessMessageRequestForAutoSave();
		}

		[OwaEventParameter("Id", typeof(OwaStoreObjectId), false, false)]
		[OwaEventParameter("CK", typeof(string), false, true)]
		[OwaEventParameter("Subj", typeof(string), false, true)]
		[OwaEventParameter("Imp", typeof(Importance), false, true)]
		[OwaEventParameter("Sensitivity", typeof(Sensitivity), false, true)]
		[OwaEventParameter("Body", typeof(string), false, true)]
		[OwaEventParameter("To", typeof(RecipientInfo), true, true)]
		[OwaEventParameter("Recips", typeof(RecipientInfoAC), true, true)]
		[OwaEventParameter("Text", typeof(bool), false, true)]
		[OwaEventParameter("DeliveryRcpt", typeof(bool), false, true)]
		[OwaEventParameter("ReadRcpt", typeof(bool), false, true)]
		[OwaEventParameter("SharingLevel", typeof(SharingLevel), false, true)]
		[OwaEventParameter("RequestPermission", typeof(bool), false, true)]
		[OwaEvent("Save")]
		public void Save()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "SharingMessageEventHandler.Save");
			this.ProcessMessageRequest(MessageAction.Save);
		}

		[OwaEventParameter("Text", typeof(bool), false, true)]
		[OwaEventParameter("ForceSend", typeof(bool), false, true)]
		[OwaEventParameter("Subj", typeof(string), false, true)]
		[OwaEventParameter("Imp", typeof(Importance), false, true)]
		[OwaEventParameter("Id", typeof(OwaStoreObjectId), false, true)]
		[OwaEventParameter("Body", typeof(string), false, true)]
		[OwaEventParameter("To", typeof(RecipientInfo), true, true)]
		[OwaEventParameter("Recips", typeof(RecipientInfoAC), true, true)]
		[OwaEventParameter("RequestPermission", typeof(bool), false, true)]
		[OwaEventParameter("CK", typeof(string), false, true)]
		[OwaEventParameter("ReadRcpt", typeof(bool), false, true)]
		[OwaEventParameter("Sensitivity", typeof(Sensitivity), false, true)]
		[OwaEventParameter("DeliveryRcpt", typeof(bool), false, true)]
		[OwaEventParameter("SharingLevel", typeof(SharingLevel), false, true)]
		[OwaEvent("Send")]
		public void Send()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "SharingMessageEventHandler.Send");
			this.ProcessMessageRequest(MessageAction.Send);
		}

		private void TryProcessMessageRequestForAutoSave()
		{
			SharingMessageItem sharingMessageItem = null;
			try
			{
				sharingMessageItem = base.GetRequestItem<SharingMessageItem>(new PropertyDefinition[0]);
				if (!sharingMessageItem.IsPublishing)
				{
					SharingLevel level = this.UpdateInviteOrRequestProperties(sharingMessageItem);
					this.TrySetSharingLevel(sharingMessageItem, level);
				}
				base.UpdateMessageForAutoSave(sharingMessageItem, StoreObjectType.Message);
				Utilities.SaveItem(sharingMessageItem, true);
				this.RenderChangeKey(sharingMessageItem);
			}
			catch (Exception ex)
			{
				ExTraceGlobals.MailTracer.TraceError<string>((long)this.GetHashCode(), "SharingMessageEventHandler.TryProcessMessageRequestForAutoSave - Exception {0} thrown during autosave", ex.Message);
				if (Utilities.ShouldSendChangeKeyForException(ex))
				{
					ExTraceGlobals.MailDataTracer.TraceDebug<string>((long)this.GetHashCode(), "SharingMessageEventHandler.TryProcessMessageRequestForAutoSave attempt to send change key on exception: {0}", ex.Message);
					base.SaveIdAndChangeKeyInCustomErrorInfo(sharingMessageItem);
				}
				base.RenderErrorForAutoSave(ex);
			}
			finally
			{
				if (sharingMessageItem != null)
				{
					sharingMessageItem.Dispose();
					sharingMessageItem = null;
				}
			}
		}

		private void ProcessMessageRequest(MessageAction action)
		{
			SharingMessageItem sharingMessageItem = null;
			bool flag = false;
			RecipientInfoAC[] array = (RecipientInfoAC[])base.GetParameter("Recips");
			if (array != null && array.Length != 0)
			{
				AutoCompleteCache.UpdateAutoCompleteCacheFromRecipientInfoList(array, base.OwaContext.UserContext);
			}
			SharingMessageItem requestItem;
			sharingMessageItem = (requestItem = base.GetRequestItem<SharingMessageItem>(new PropertyDefinition[0]));
			try
			{
				if (!sharingMessageItem.IsPublishing && (sharingMessageItem.SharingMessageType.IsInvitationOrAcceptOfRequest || sharingMessageItem.SharingMessageType.IsRequest))
				{
					SharingLevel level = this.UpdateInviteOrRequestProperties(sharingMessageItem);
					this.SetSharingLevel(sharingMessageItem, level);
				}
				bool flag2 = base.UpdateMessage(sharingMessageItem, StoreObjectType.Message);
				if (action == MessageAction.Save)
				{
					ExTraceGlobals.MailTracer.TraceDebug((long)this.GetHashCode(), "Saving Sharing message");
					Utilities.SaveItem(sharingMessageItem, true);
					this.RenderChangeKey(sharingMessageItem);
				}
				else if (action == MessageAction.Send)
				{
					if (flag2)
					{
						throw new OwaEventHandlerException("Unresolved recipients", LocalizedStrings.GetNonEncoded(2063734279));
					}
					if (sharingMessageItem.Recipients.Count == 0 && action == MessageAction.Send)
					{
						throw new OwaEventHandlerException("No recipients", LocalizedStrings.GetNonEncoded(1878192149));
					}
					if (Utilities.RecipientsOnlyHaveEmptyPDL<Recipient>(base.UserContext, sharingMessageItem.Recipients))
					{
						base.RenderPartialFailure(1389137820);
						return;
					}
					ExTraceGlobals.MailTracer.TraceDebug((long)this.GetHashCode(), "Sending sharing message");
					bool flag3 = false;
					if (base.IsParameterSet("ForceSend"))
					{
						flag3 = (bool)base.GetParameter("ForceSend");
					}
					sharingMessageItem.SetSubmitFlags(flag3 ? SharingSubmitFlags.Auto : SharingSubmitFlags.None);
					try
					{
						if (flag)
						{
							sharingMessageItem.SendWithoutSavingMessage();
						}
						else
						{
							sharingMessageItem.Send();
						}
					}
					catch (ObjectNotFoundException e)
					{
						this.SendIdAndChangeKeyToClientOnError(sharingMessageItem, e);
						this.SanitizingWriter.Write("<p id=\"err\" _msg=\"");
						this.SanitizingWriter.Write(LocalizedStrings.GetNonEncoded(-1608187286), sharingMessageItem.SharedFolderName);
						this.SanitizingWriter.Write("\"");
						this.SanitizingWriter.Write(" _shareErr=\"");
						this.SanitizingWriter.Write(1);
						this.SanitizingWriter.Write("\"></p>");
					}
					catch (InvalidSharingRecipientsException ex)
					{
						this.SendIdAndChangeKeyToClientOnError(sharingMessageItem, ex);
						this.SanitizingWriter.Write("<div id=\"err\" _msg=\"\" _shareErr=\"");
						SharingPolicy sharingPolicy = DirectoryHelper.ReadSharingPolicy(base.UserContext.MailboxSession.MailboxOwner.MailboxInfo.MailboxGuid, base.UserContext.MailboxSession.MailboxOwner.MailboxInfo.IsArchive, base.UserContext.MailboxSession.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid));
						if (sharingPolicy != null && sharingPolicy.IsAllowedForAnonymousCalendarSharing() && ex.InvalidRecipientsResolution != null)
						{
							switch (ex.InvalidRecipientsResolution.Resolution)
							{
							case InvalidSharingRecipientsResolutionType.PublishAndTryAgain:
								this.SanitizingWriter.Write(3);
								this.SanitizingWriter.Write("\" _folderId=\"");
								this.SanitizingWriter.Write(ex.InvalidRecipientsResolution.FolderId);
								this.SanitizingWriter.Write("\">");
								this.RenderPublishAndTryAgainDialog(ex);
								break;
							case InvalidSharingRecipientsResolutionType.SendPublishLinks:
								this.SanitizingWriter.Write(4);
								this.SanitizingWriter.Write("\">");
								this.RenderSendPublishLinksDialog(ex);
								break;
							default:
								throw;
							}
						}
						else
						{
							this.SanitizingWriter.Write(2);
							this.SanitizingWriter.Write("\">");
							this.RenderInvalidRecipientsDialog(ex);
						}
						this.SanitizingWriter.Write("</div>");
					}
					catch (Exception e2)
					{
						this.SendIdAndChangeKeyToClientOnError(sharingMessageItem, e2);
						throw;
					}
					if (Globals.ArePerfCountersEnabled)
					{
						OwaSingleCounters.MessagesSent.Increment();
					}
				}
				RenderingUtilities.RenderNavigationTreeDirtyFlag(this.Writer, base.UserContext, NavigationTreeDirtyFlag.Calendar, new NavigationModule[]
				{
					NavigationModule.Calendar
				});
			}
			finally
			{
				if (requestItem != null)
				{
					((IDisposable)requestItem).Dispose();
				}
			}
		}

		private void SendIdAndChangeKeyToClientOnError(MessageItem message, Exception e)
		{
			if (Utilities.ShouldSendChangeKeyForException(e))
			{
				base.SaveIdAndChangeKeyInCustomErrorInfo(message);
			}
		}

		private void SetSharingLevel(SharingMessageItem message, SharingLevel level)
		{
			if (message.IsSharedFolderPrimary && level == SharingLevel.FullDetailsEditor)
			{
				throw new OwaInvalidRequestException("Only support read-only on primary folder");
			}
			this.SetSharingLevelForMessage(message, level);
		}

		private void TrySetSharingLevel(SharingMessageItem message, SharingLevel level)
		{
			if (message.IsSharedFolderPrimary && level == SharingLevel.FullDetailsEditor)
			{
				return;
			}
			this.SetSharingLevelForMessage(message, level);
		}

		private void SetSharingLevelForMessage(SharingMessageItem message, SharingLevel level)
		{
			switch (level)
			{
			case SharingLevel.FreeBusy:
				message.SharingDetail = SharingContextDetailLevel.AvailabilityOnly;
				message.SharingPermissions = SharingContextPermissions.Reviewer;
				return;
			case SharingLevel.Limited:
				message.SharingDetail = SharingContextDetailLevel.Limited;
				message.SharingPermissions = SharingContextPermissions.Reviewer;
				return;
			case SharingLevel.FullDetailsReviewer:
				message.SharingDetail = SharingContextDetailLevel.FullDetails;
				message.SharingPermissions = SharingContextPermissions.Reviewer;
				return;
			case SharingLevel.FullDetailsEditor:
				message.SharingDetail = SharingContextDetailLevel.FullDetails;
				message.SharingPermissions = SharingContextPermissions.Editor;
				return;
			default:
				message.SharingDetail = SharingContextDetailLevel.None;
				message.SharingPermissions = SharingContextPermissions.Reviewer;
				return;
			}
		}

		private SharingLevel UpdateInviteOrRequestProperties(SharingMessageItem message)
		{
			bool flag = false;
			if (base.IsParameterSet("RequestPermission"))
			{
				flag = (bool)base.GetParameter("RequestPermission");
			}
			SharingLevel sharingLevel = SharingLevel.None;
			if (base.IsParameterSet("SharingLevel"))
			{
				sharingLevel = (SharingLevel)base.GetParameter("SharingLevel");
			}
			if (message.SharingMessageType.IsInvitationOrRequest)
			{
				if (sharingLevel == SharingLevel.None)
				{
					message.SharingMessageType = SharingMessageType.Request;
				}
				else if (!flag)
				{
					message.SharingMessageType = SharingMessageType.Invitation;
				}
				else
				{
					message.SharingMessageType = SharingMessageType.InvitationAndRequest;
				}
			}
			return sharingLevel;
		}

		private void RenderInvalidRecipientsDialog(InvalidSharingRecipientsException e)
		{
			this.SanitizingWriter.Write("<div id=\"divInvalidRecipient\">");
			this.SanitizingWriter.Write("<p>");
			this.SanitizingWriter.Write(LocalizedStrings.GetNonEncoded(-1758248740));
			this.SanitizingWriter.Write("</p>");
			this.SanitizingWriter.Write("<div id=\"divInvalidRecipientList\">");
			foreach (InvalidRecipient invalidRecipient in e.InvalidRecipients)
			{
				this.SanitizingWriter.Write("<div>");
				this.SanitizingWriter.Write(invalidRecipient.SmtpAddress);
				this.SanitizingWriter.Write("</div>");
			}
			this.SanitizingWriter.Write("</div>");
			this.SanitizingWriter.Write("<p>");
			this.SanitizingWriter.Write(LocalizedStrings.GetNonEncoded(-1873334603));
			this.SanitizingWriter.Write("</p>");
			this.SanitizingWriter.Write("</div>");
		}

		private void RenderPublishAndTryAgainDialog(InvalidSharingRecipientsException e)
		{
			this.SanitizingWriter.Write("<div id=\"divInvalidRecipient\">");
			this.SanitizingWriter.Write("<p>");
			this.SanitizingWriter.Write(LocalizedStrings.GetNonEncoded(-1001325451));
			this.SanitizingWriter.Write("</p>");
			this.SanitizingWriter.Write("<div id=\"divInvalidRecipientList\">");
			foreach (InvalidRecipient invalidRecipient in e.InvalidRecipients)
			{
				this.SanitizingWriter.Write("<div>");
				this.SanitizingWriter.Write(invalidRecipient.SmtpAddress);
				this.SanitizingWriter.Write("</div>");
			}
			this.SanitizingWriter.Write("</div>");
			this.SanitizingWriter.Write("<p>");
			this.SanitizingWriter.Write(LocalizedStrings.GetNonEncoded(-1335433459));
			this.SanitizingWriter.Write("</p>");
			this.SanitizingWriter.Write("</div>");
		}

		private void RenderSendPublishLinksDialog(InvalidSharingRecipientsException e)
		{
			this.SanitizingWriter.Write("<div id=\"divInvalidRecipient\">");
			this.SanitizingWriter.Write("<p>");
			this.SanitizingWriter.Write(SanitizedHtmlString.Format(LocalizedStrings.GetNonEncoded(-1620770268), new object[]
			{
				"<a class=\"publishUrl\" target=\"_blank\" href=\"",
				e.InvalidRecipientsResolution.BrowseUrl,
				"\">",
				"</a>"
			}));
			this.SanitizingWriter.Write("</p>");
			this.SanitizingWriter.Write("<div id=\"divInvalidRecipientList\">");
			foreach (InvalidRecipient invalidRecipient in e.InvalidRecipients)
			{
				this.SanitizingWriter.Write("<div>");
				this.SanitizingWriter.Write(invalidRecipient.SmtpAddress);
				this.SanitizingWriter.Write("</div>");
			}
			this.SanitizingWriter.Write("</div>");
			this.SanitizingWriter.Write("<p>");
			this.SanitizingWriter.Write(LocalizedStrings.GetNonEncoded(-1455565968));
			this.SanitizingWriter.Write("</p>");
			this.SanitizingWriter.Write("</div>");
		}

		private void RenderSubscriptionError(StoragePermanentException ex)
		{
			if (ex is InvalidExternalSharingSubscriberException)
			{
				this.RenderShareToExternalADDLResponse();
				return;
			}
			if (ex is InvalidSharingDataException || ex is InvalidExternalSharingInitiatorException)
			{
				this.RenderDataCorruptedResponse();
				return;
			}
			if (ex is InvalidSharingTargetRecipientException)
			{
				this.RenderArchiveNotSupportedResponse();
				return;
			}
			this.RenderNotFoundResponse();
		}

		private void RenderDataCorruptedResponse()
		{
			this.SanitizingWriter.Write("<div id=\"err\" _msg=\"msgCorrupted\"></div>");
		}

		private void RenderArchiveNotSupportedResponse()
		{
			this.SanitizingWriter.Write("<div id=\"err\" _msg=\"msgInArchive\"></div>");
		}

		private void RenderNotFoundResponse()
		{
			this.SanitizingWriter.Write("<div id=\"err\" _msg=\"notfound\"></div>");
		}

		private void RenderSharedFromE12Response()
		{
			this.SanitizingWriter.Write("<div id=\"err\" _msg=\"fromOlderVersion\"></div>");
		}

		private void RenderShareToExternalADDLResponse()
		{
			this.SanitizingWriter.Write("<div id=\"err\" _msg=\"toExternalADDL\"></div>");
		}

		private void RenderChangeKey(SharingMessageItem messageItem)
		{
			messageItem.Load();
			base.WriteChangeKey(messageItem);
		}

		public const string EventNamespace = "SharingMessage";

		public const string MethodAddSharedCalendar = "AddSharedCalendar";

		public const string MethodSend = "Send";

		public const string MethodAllowSharingRequest = "AllowSharingRequest";

		public const string MethodSubscribeToInternetCalendar = "SubscribeToInternetCalendar";

		public const string SharingLevelParameter = "SharingLevel";

		public const string RequestPermission = "RequestPermission";

		public const string ICalUrl = "iCalUrl";

		public const string ForceSend = "ForceSend";
	}
}
