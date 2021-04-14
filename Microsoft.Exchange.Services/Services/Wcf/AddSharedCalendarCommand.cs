using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class AddSharedCalendarCommand : ServiceCommand<AddSharedCalendarResponse>
	{
		public AddSharedCalendarCommand(CallContext callContext, AddSharedCalendarRequest request) : base(callContext)
		{
			this.TraceDebug("Constructor", new object[0]);
			this.Request = request;
			this.TraceDebug("Validating Request", new object[0]);
			this.Request.ValidateRequest();
			this.mbSession = base.CallContext.SessionCache.GetMailboxIdentityMailboxSession();
		}

		public AddSharedCalendarRequest Request { get; set; }

		protected override AddSharedCalendarResponse InternalExecute()
		{
			this.TraceDebug("Binding to the sharing message item", new object[0]);
			AddSharedCalendarResponse result;
			using (SharingMessageItem sharingMessageItem = SharingMessageItem.Bind(this.mbSession, this.Request.MessageStoreId))
			{
				this.TraceDebug("MessageType:{0} FolderType={1}", new object[]
				{
					sharingMessageItem.SharingMessageType.IsInvitationOrAcceptOfRequest,
					sharingMessageItem.SharedFolderType
				});
				if (!sharingMessageItem.SharingMessageType.IsInvitationOrAcceptOfRequest || sharingMessageItem.SharedFolderType != DefaultFolderType.Calendar)
				{
					this.TraceDebug("Request not supported", new object[0]);
					throw FaultExceptionUtilities.CreateFault(new InvalidRequestException(), FaultParty.Sender);
				}
				SubscribeResults subscribeResults = sharingMessageItem.SubscribeAndOpen();
				this.TraceDebug("Subsription Result:{0}", new object[]
				{
					subscribeResults
				});
				VersionedId versionedId = (subscribeResults is SubscribeResultsInternal) ? this.CreateLinkedGroupEntry((SubscribeResultsInternal)subscribeResults, sharingMessageItem.IsSharedFolderPrimary) : this.CreateLocalGroupEntry(subscribeResults);
				if (versionedId == null)
				{
					this.TraceError("Could not create the entry", new object[0]);
					result = new AddSharedCalendarResponse(CalendarActionError.AddSharedCalendarFailed);
				}
				else
				{
					this.TraceDebug("Suceessfully created the entry", new object[0]);
					result = new AddSharedCalendarResponse(IdConverter.ConvertStoreItemIdToItemId(versionedId, this.mbSession));
				}
			}
			return result;
		}

		private VersionedId CreateLinkedGroupEntry(SubscribeResultsInternal subResult, bool isSharedFolderPrimary)
		{
			this.TraceDebug("Bind to the Other Calendars Group", new object[0]);
			VersionedId result;
			using (CalendarGroup calendarGroup = CalendarGroup.Bind(this.mbSession, CalendarGroupType.OtherCalendars))
			{
				if (isSharedFolderPrimary)
				{
					ADRecipient adrecipient = this.RetrieveADRecipient(subResult.InitiatorSmtpAddress);
					if (adrecipient == null)
					{
						this.TraceError("Not able to retrive the ADRecipient for " + subResult.InitiatorSmtpAddress, new object[0]);
						return null;
					}
					this.TraceDebug("Find existing GS Calendar", new object[0]);
					CalendarGroupEntryInfo calendarGroupEntryInfo = calendarGroup.FindSharedGSCalendaryEntry(adrecipient.LegacyExchangeDN);
					if (calendarGroupEntryInfo != null)
					{
						this.TraceDebug("An entry already exists for " + adrecipient.LegacyExchangeDN, new object[0]);
						return calendarGroupEntryInfo.Id;
					}
					this.TraceDebug("Invoking CalendarGroupEntry.Create to create a LinkedGSCalendarEntry", new object[0]);
					using (CalendarGroupEntry calendarGroupEntry = CalendarGroupEntry.Create(this.mbSession, adrecipient.LegacyExchangeDN, calendarGroup))
					{
						return this.CreateAndLoad(subResult.InitiatorName, calendarGroupEntry);
					}
				}
				this.TraceDebug("Get remote mailbox session for user:{0}", new object[]
				{
					subResult.InitiatorSmtpAddress
				});
				MailboxSession mailboxSessionBySmtpAddress = base.CallContext.SessionCache.GetMailboxSessionBySmtpAddress(subResult.InitiatorSmtpAddress);
				this.TraceDebug("Bind to the remote folder", new object[0]);
				using (CalendarFolder calendarFolder = CalendarFolder.Bind(mailboxSessionBySmtpAddress, subResult.RemoteFolderId))
				{
					this.TraceDebug("Find existing shared calendar based on Folder Id", new object[0]);
					CalendarGroupEntryInfo calendarGroupEntryInfo2 = calendarGroup.FindSharedCalendaryEntry(subResult.RemoteFolderId);
					if (calendarGroupEntryInfo2 != null)
					{
						this.TraceDebug("An entry already exists", new object[0]);
						result = calendarGroupEntryInfo2.Id;
					}
					else
					{
						this.TraceDebug("Invoking CalendarGroupEntry.Create to create a LinkedCalendarEntry", new object[0]);
						using (CalendarGroupEntry calendarGroupEntry2 = CalendarGroupEntry.Create(this.mbSession, calendarFolder, calendarGroup))
						{
							result = this.CreateAndLoad(ServerStrings.SharingFolderName(subResult.RemoteFolderName, subResult.InitiatorName), calendarGroupEntry2);
						}
					}
				}
			}
			return result;
		}

		private ADRecipient RetrieveADRecipient(string smtpAddress)
		{
			IRecipientSession adrecipientSession = base.CallContext.ADRecipientSessionContext.GetADRecipientSession();
			return adrecipientSession.FindByProxyAddress(ProxyAddress.Parse(smtpAddress));
		}

		private VersionedId CreateLocalGroupEntry(SubscribeResults subResult)
		{
			this.TraceDebug("Bind to the local folder", new object[0]);
			VersionedId result;
			using (CalendarFolder calendarFolder = CalendarFolder.Bind(this.mbSession, subResult.LocalFolderId))
			{
				this.TraceDebug("Bind to the Other Calendars Group", new object[0]);
				using (CalendarGroup calendarGroup = CalendarGroup.Bind(this.mbSession, CalendarGroupType.OtherCalendars))
				{
					this.TraceDebug("Find existing shared calendar based on Folder Id", new object[0]);
					CalendarGroupEntryInfo calendarGroupEntryInfo = calendarGroup.FindSharedCalendaryEntry(subResult.LocalFolderId);
					if (calendarGroupEntryInfo != null)
					{
						this.TraceDebug("An entry already exists", new object[0]);
						result = calendarGroupEntryInfo.Id;
					}
					else
					{
						this.TraceDebug("Invoking CalendarGroupEntry.Create", new object[0]);
						using (CalendarGroupEntry calendarGroupEntry = CalendarGroupEntry.Create(this.mbSession, calendarFolder, calendarGroup))
						{
							result = this.CreateAndLoad(subResult.LocalFolderName, calendarGroupEntry);
						}
					}
				}
			}
			return result;
		}

		private VersionedId CreateAndLoad(string calendarName, CalendarGroupEntry newCalendarGroupEntry)
		{
			newCalendarGroupEntry.CalendarName = calendarName;
			ConflictResolutionResult conflictResolutionResult = newCalendarGroupEntry.Save(SaveMode.NoConflictResolution);
			if (conflictResolutionResult.SaveStatus == SaveResult.Success)
			{
				newCalendarGroupEntry.Load();
				return newCalendarGroupEntry.CalendarGroupEntryId;
			}
			this.TraceError("Unable to save shared group entry to common view", new object[0]);
			return null;
		}

		private void TraceError(string messageFormat, params object[] args)
		{
			ExTraceGlobals.AddSharedCalendarCallTracer.TraceError((long)this.GetHashCode(), messageFormat, args);
		}

		private void TraceDebug(string messageFormat, params object[] args)
		{
			ExTraceGlobals.AddSharedCalendarCallTracer.TraceDebug((long)this.GetHashCode(), messageFormat, args);
		}

		private readonly MailboxSession mbSession;
	}
}
