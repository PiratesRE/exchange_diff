using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriverDelivery;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxTransport.StoreDriverCommon;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Categorizer;
using Microsoft.Exchange.Transport.Logging.ConnectionLog;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal class DeliveryItem : DisposeTrackableBase, IDeliveryItem
	{
		public DeliveryItem(MailItemDeliver context)
		{
			this.context = context;
		}

		public bool HasMessage
		{
			get
			{
				return this.messageItem != null;
			}
		}

		public MessageItem Message
		{
			get
			{
				return this.messageItem;
			}
		}

		public StoreSession Session
		{
			get
			{
				return this.storeSession;
			}
		}

		public StoreId DeliverToFolder
		{
			get
			{
				return this.deliverToFolder;
			}
			set
			{
				this.deliverToFolder = value;
			}
		}

		public MailboxSession MailboxSession
		{
			get
			{
				return this.storeSession as MailboxSession;
			}
		}

		public PublicFolderSession PublicFolderSession
		{
			get
			{
				return this.storeSession as PublicFolderSession;
			}
		}

		public void Deliver(ProxyAddress recipientProxyAddress)
		{
			this.storeSession.Deliver(this.messageItem, recipientProxyAddress, RecipientItemType.Unknown);
		}

		public void SetProperty(PropertyDefinition property, object value)
		{
			this.messageItem[property] = value;
		}

		public void DeleteProperty(PropertyDefinition property)
		{
			this.messageItem.Delete(property);
		}

		public Stream OpenPropertyStream(PropertyDefinition propertyDefinition, PropertyOpenMode openMode)
		{
			return this.messageItem.OpenPropertyStream(propertyDefinition, openMode);
		}

		public void CreatePublicFolderMessage(MailRecipient recipient, DeliverableItem item)
		{
			PublicFolderSession publicFolderSession = (PublicFolderSession)this.storeSession;
			bool flag = false;
			try
			{
				this.context.BeginTrackLatency(LatencyComponent.StoreDriverDeliveryRpc);
				using (Folder folder = Folder.Bind(publicFolderSession, this.deliverToFolder, new PropertyDefinition[]
				{
					FolderSchema.SecurityDescriptor
				}))
				{
					switch (MailPublicFolderPermissionHandler.CheckAccessForEmailDelivery(this.context, folder))
					{
					case AccessCheckResult.NotAllowedAnonymous:
						DeliveryItem.Diag.TraceError(0L, "Anonymous users are not permitted to add contents to mail enabled public folder.");
						throw new SmtpResponseException(AckReason.NotAuthenticated, MessageAction.NDR);
					case AccessCheckResult.NotAllowedAuthenticated:
						DeliveryItem.Diag.TraceError<RoutingAddress>(0L, "User {0} is not permitted to add contents to mail enabled public folder.", this.context.MbxTransportMailItem.From);
						throw new SmtpResponseException(AckReason.RecipientPermissionRestricted, MessageAction.NDR);
					case AccessCheckResult.NotAllowedInternalSystemError:
						DeliveryItem.Diag.TraceError(0L, "Exception occured when determining permission for sender on public folder");
						throw new SmtpResponseException(AckReason.PublicFolderSenderValidationFailed, MessageAction.NDR);
					default:
						if (folder.IsContentAvailable())
						{
							this.messageItem = MessageItem.CreateForDelivery(publicFolderSession, folder.Id, this.context.ReplayItem.InternetMessageId, this.context.ReplayItem.GetValueAsNullable<ExDateTime>(ItemSchema.SentTime));
							if (this.messageItem != null && this.messageItem.DisposeTracker != null)
							{
								this.messageItem.DisposeTracker.AddExtraDataWithStackTrace("DeliveryItem owns messageItem at:{0}{1}");
							}
							flag = true;
						}
						else
						{
							this.ReroutePublicFolderRecipient(publicFolderSession, folder, recipient);
						}
						break;
					}
				}
			}
			finally
			{
				TimeSpan additionalLatency = this.context.EndTrackLatency(LatencyComponent.StoreDriverDeliveryRpc);
				this.context.AddRpcLatency(additionalLatency, "Open message");
			}
			if (flag)
			{
				ItemConversion.ReplayInboundContent(this.context.ReplayItem, this.messageItem);
			}
		}

		public void CreateSession(MailRecipient recipient, OpenTransportSessionFlags deliveryFlags, DeliverableItem item, ICollection<CultureInfo> recipientLanguages)
		{
			bool flag = this.IsPublicFolderRecipient(item);
			if (flag)
			{
				DeliveryItem.Diag.TracePfd(0L, "PFD ESD {0} Deliver to PF recipient {1} on MDB {2} with public folder GUID {3}", new object[]
				{
					29595,
					item.LegacyExchangeDN,
					item.HomeMdbDN,
					this.context.MbxTransportMailItem.DatabaseGuid
				});
			}
			else
			{
				DeliveryItem.Diag.TracePfd<int, string, string>(0L, "PFD ESD {0} Deliver to Mailbox recipient {1} on MDB {2}", 19611, item.LegacyExchangeDN, item.HomeMdbDN);
			}
			this.ValidateLegacyDN(recipient, item.LegacyExchangeDN);
			bool flag2 = false;
			try
			{
				this.CreateSessionInternal(recipient, deliveryFlags, item, recipientLanguages, false);
			}
			catch (MailboxUnavailableException ex)
			{
				if (-2146233088 != ex.ErrorCode)
				{
					throw;
				}
				DeliveryItem.Diag.TraceDebug<string>(0L, "Failed to open mailbox {0} with stripped principal. Will retry with complete principal", recipient.Email.ToString());
				flag2 = true;
			}
			if (flag2)
			{
				this.CreateSessionInternal(recipient, deliveryFlags, item, recipientLanguages, true);
			}
		}

		public void LoadMailboxMessage(string internetMessageId)
		{
			MailboxSession session = (MailboxSession)this.storeSession;
			try
			{
				this.context.BeginTrackLatency(LatencyComponent.StoreDriverDeliveryRpc);
				IEnumerable<IStorePropertyBag> source = AllItemsFolderHelper.FindItemsFromInternetId(session, internetMessageId, ItemQueryType.NoNotifications, new PropertyDefinition[]
				{
					ItemSchema.Id
				});
				if (source.Count<IStorePropertyBag>() != 1)
				{
					throw new UnexpectedMessageCountException(string.Format("The number of messages found was unexpected. Count: {0}", source.Count<IStorePropertyBag>()));
				}
				IStorePropertyBag storePropertyBag = source.First<IStorePropertyBag>();
				StoreObjectId objectId = ((VersionedId)storePropertyBag[ItemSchema.Id]).ObjectId;
				this.messageItem = MessageItem.Bind(session, objectId);
				if (this.messageItem == null)
				{
					throw new System.Data.ObjectNotFoundException("The requested message was not loaded.");
				}
				this.messageItem.OpenAsReadWrite();
			}
			catch (Microsoft.Exchange.Data.Storage.ObjectNotFoundException innerException)
			{
				throw new System.Data.ObjectNotFoundException("The requested message was not loaded.", innerException);
			}
			finally
			{
				TimeSpan additionalLatency = this.context.EndTrackLatency(LatencyComponent.StoreDriverDeliveryRpc);
				this.context.AddRpcLatency(additionalLatency, "Load message");
			}
		}

		public void CreateMailboxMessage(bool leaveReceivedTime)
		{
			MailboxSession mailboxSession = (MailboxSession)this.storeSession;
			try
			{
				this.context.BeginTrackLatency(LatencyComponent.StoreDriverDeliveryRpc);
				if (this.deliverToFolder == null)
				{
					this.deliverToFolder = mailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox);
				}
				ExDateTime? clientSubmitTime = this.context.ReplayItem.GetValueAsNullable<ExDateTime>(ItemSchema.SentTime);
				if (leaveReceivedTime)
				{
					this.messageItem = MessageItem.CreateAggregatedForDelivery(mailboxSession, this.deliverToFolder, this.context.ReplayItem.InternetMessageId, clientSubmitTime);
				}
				else
				{
					bool value = this.context.Recipient.ExtendedProperties.GetValue<bool>("Microsoft.Exchange.Transport.MailboxTransport.RetryOnDuplicateDelivery ", false);
					if (value)
					{
						clientSubmitTime = null;
					}
					this.messageItem = MessageItem.CreateForDelivery(mailboxSession, this.deliverToFolder, this.context.ReplayItem.InternetMessageId, clientSubmitTime);
				}
			}
			finally
			{
				TimeSpan additionalLatency = this.context.EndTrackLatency(LatencyComponent.StoreDriverDeliveryRpc);
				this.context.AddRpcLatency(additionalLatency, "Create message");
			}
			if (this.messageItem != null && this.messageItem.DisposeTracker != null)
			{
				this.messageItem.DisposeTracker.AddExtraDataWithStackTrace("DeliveryItem owns messageItem at:{0}{1}");
			}
			ItemConversion.ReplayInboundContent(this.context.ReplayItem, this.messageItem);
		}

		public void DisposeMessageAndSession()
		{
			if (this.messageItem != null)
			{
				this.messageItem.Dispose();
				this.messageItem = null;
			}
			if (this.storeSession != null)
			{
				try
				{
					this.context.BeginTrackLatency(LatencyComponent.StoreDriverDeliveryRpc);
					this.storeSession.Dispose();
					this.storeSession = null;
				}
				finally
				{
					TimeSpan additionalLatency = this.context.EndTrackLatency(LatencyComponent.StoreDriverDeliveryRpc);
					this.context.AddRpcLatency(additionalLatency, "Dispose store session");
				}
			}
		}

		internal void ReroutePublicFolderRecipient(PublicFolderSession publicFolderSession, Folder publicFolder, MailRecipient recipient)
		{
			DeliveryItem.Diag.TraceDebug<string>(0L, "No local Replica for recipient {0}.", recipient.Email.ToString());
			throw new SmtpResponseException(AckReason.PublicFolderRoute, MessageAction.Reroute);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<DeliveryItem>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (this.messageItem != null && this.messageItem.DisposeTracker != null)
			{
				this.messageItem.DisposeTracker.AddExtraDataWithStackTrace(string.Format(CultureInfo.InvariantCulture, "DeliveryItem.InternalDispose({0}) called with stack", new object[]
				{
					disposing
				}));
			}
			if (disposing)
			{
				this.DisposeMessageAndSession();
			}
		}

		private void CreateSessionInternal(MailRecipient recipient, OpenTransportSessionFlags deliveryFlags, DeliverableItem item, ICollection<CultureInfo> recipientLanguages, bool useCompletePrincipal)
		{
			bool isPublicFolderRecipient = this.IsPublicFolderRecipient(item);
			ExchangePrincipal principal = this.GetExchangePrincipalForRecipient(recipient, item, recipientLanguages, useCompletePrincipal);
			string databaseName = this.context.MbxTransportMailItem.DatabaseName;
			this.LogConnection(databaseName, "Mailbox");
			this.RunUnderOpenStoreSessionFailedLogger(databaseName, delegate
			{
				if (isPublicFolderRecipient)
				{
					this.storeSession = PublicFolderSession.OpenAsTransport(principal, deliveryFlags);
					return;
				}
				this.storeSession = MailboxSession.OpenAsTransport(principal, deliveryFlags);
			});
			if (!isPublicFolderRecipient)
			{
				this.storeSession.ExTimeZone = ExTimeZone.CurrentTimeZone;
			}
		}

		private void ValidateLegacyDN(MailRecipient recipient, string legacyDN)
		{
			if (string.IsNullOrEmpty(legacyDN))
			{
				string text = recipient.Email.ToString();
				StoreDriverDeliveryDiagnostics.LogEvent(MailboxTransportEventLogConstants.Tuple_DeliveryFailedNoLegacyDN, text, new object[]
				{
					text
				});
				throw new SmtpResponseException(AckReason.NoLegacyDN, MessageAction.Reroute);
			}
		}

		private void ClearPropertiesForMeetingMessage()
		{
			DeliveryItem.Diag.TraceDebug(0L, "Copied content from previous item");
			if (ObjectClass.IsMeetingMessage(this.context.MessageClass))
			{
				this.messageItem.Load(StoreObjectSchema.ContentConversionProperties);
				this.messageItem.DeleteProperties(new PropertyDefinition[]
				{
					MeetingMessageSchema.CalendarProcessed,
					MeetingMessageInstanceSchema.IsProcessed,
					CalendarItemBaseSchema.OldLocation,
					CalendarItemBaseSchema.ChangeHighlight,
					MeetingRequestSchema.OldStartWhole,
					MeetingRequestSchema.OldEndWhole
				});
			}
		}

		private void LogConnection(string mdb, string sessionType)
		{
			if (!this.context.WasSessionOpenedForLastRecipient)
			{
				ConnectionLog.MapiDeliveryConnectionServerConnect(this.context.SessionId, mdb, StoreDriverDelivery.MailboxServerFqdn, sessionType);
			}
		}

		private bool IsPublicFolderRecipient(DeliverableItem item)
		{
			return item.RecipientType == Microsoft.Exchange.Data.Directory.Recipient.RecipientType.PublicFolder;
		}

		private ExchangePrincipal GetExchangePrincipalForRecipient(MailRecipient recipient, DeliverableItem item, ICollection<CultureInfo> recipientLanguages, bool useCompletePrincipal)
		{
			ADSessionSettings adsessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(recipient.MailItemScopeOrganizationId);
			Guid databaseGuid = this.context.MbxTransportMailItem.DatabaseGuid;
			ExchangePrincipal exchangePrincipal;
			if (this.IsPublicFolderRecipient(item))
			{
				ADObjectId value = recipient.ExtendedProperties.GetValue<ADObjectId>("Microsoft.Exchange.Transport.DirectoryData.ContentMailbox", null);
				StoreObjectId storeObjectId = null;
				if (value == null || !StoreObjectId.TryParseFromHexEntryId(recipient.ExtendedProperties.GetValue<string>("Microsoft.Exchange.Transport.DirectoryData.EntryId", null), out storeObjectId))
				{
					throw new SmtpResponseException(AckReason.UnableToDetermineTargetPublicFolderMailbox, MessageAction.Reroute);
				}
				this.deliverToFolder = storeObjectId;
				try
				{
					exchangePrincipal = ExchangePrincipal.FromDirectoryObjectId(DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, adsessionSettings, 830, "GetExchangePrincipalForRecipient", "f:\\15.00.1497\\sources\\dev\\MailboxTransport\\src\\MailboxTransportDelivery\\StoreDriver\\DeliveryItem.cs"), value, RemotingOptions.LocalConnectionsOnly);
					goto IL_14C;
				}
				catch (Microsoft.Exchange.Data.Storage.ObjectNotFoundException)
				{
					throw new SmtpResponseException(AckReason.PublicFolderMailboxNotFound, MessageAction.Reroute);
				}
			}
			MailboxItem mailboxItem = item as MailboxItem;
			if (mailboxItem == null)
			{
				throw new InvalidOperationException("Delivery to PFDBs is not supported in E15");
			}
			if (!useCompletePrincipal)
			{
				string legacyExchangeDN;
				if (!recipient.ExtendedProperties.TryGetValue<string>("Microsoft.Exchange.Transport.MailRecipient.DisplayName", out legacyExchangeDN))
				{
					legacyExchangeDN = mailboxItem.LegacyExchangeDN;
				}
				exchangePrincipal = ExchangePrincipal.FromMailboxData(legacyExchangeDN, adsessionSettings, databaseGuid, mailboxItem.MailboxGuid, mailboxItem.LegacyExchangeDN, recipient.Email.ToString(), recipientLanguages ?? new MultiValuedProperty<CultureInfo>(), true, mailboxItem.RecipientType, mailboxItem.RecipientTypeDetails.GetValueOrDefault());
			}
			else
			{
				ProxyAddress proxyAddress = new SmtpProxyAddress((string)recipient.Email, true);
				exchangePrincipal = ExchangePrincipal.FromProxyAddress(adsessionSettings, proxyAddress.ToString());
			}
			IL_14C:
			if (exchangePrincipal.MailboxInfo.IsRemote)
			{
				throw new SmtpResponseException(AckReason.RecipientMailboxIsRemote, MessageAction.Reroute);
			}
			if (exchangePrincipal.MailboxInfo.Location == MailboxDatabaseLocation.Unknown)
			{
				throw new SmtpResponseException(AckReason.RecipientMailboxLocationInfoNotAvailable, MessageAction.Reroute);
			}
			return exchangePrincipal;
		}

		private void RunUnderOpenStoreSessionFailedLogger(string mdb, DeliveryItem.OpenStoreSession openStoreSession)
		{
			try
			{
				this.context.BeginTrackLatency(LatencyComponent.StoreDriverDeliveryRpc);
				this.context.WasSessionOpenedForLastRecipient = false;
				openStoreSession();
				this.context.WasSessionOpenedForLastRecipient = true;
			}
			finally
			{
				TimeSpan additionalLatency = this.context.EndTrackLatency(LatencyComponent.StoreDriverDeliveryRpc);
				this.context.AddRpcLatency(additionalLatency, "Open session");
				if (!this.context.WasSessionOpenedForLastRecipient)
				{
					ConnectionLog.MapiDeliveryConnectionServerConnectFailed(this.context.SessionId, mdb, StoreDriverDelivery.MailboxServerFqdn);
				}
			}
		}

		DisposeTracker IDeliveryItem.get_DisposeTracker()
		{
			return base.DisposeTracker;
		}

		private const uint UserAvailabilityErrorCode = 2148734208U;

		private static readonly Trace Diag = ExTraceGlobals.MapiDeliverTracer;

		private MailItemDeliver context;

		private StoreSession storeSession;

		private MessageItem messageItem;

		private StoreId deliverToFolder;

		private delegate void OpenStoreSession();

		private struct SessionType
		{
			internal const string Mailbox = "Mailbox";

			internal const string PublicFolder = "Public Folder";
		}
	}
}
