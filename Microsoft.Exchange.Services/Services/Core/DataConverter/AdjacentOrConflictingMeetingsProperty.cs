using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class AdjacentOrConflictingMeetingsProperty : ComplexPropertyBase, IToXmlCommand, IToServiceObjectCommand, IPropertyCommand
	{
		private AdjacentOrConflictingMeetingsProperty(CommandContext commandContext, bool propertyIsAdjacentMeetings, bool propertyIsMeetingCount) : base(commandContext)
		{
			this.propertyIsAdjacentMeetings = propertyIsAdjacentMeetings;
			this.propertyIsMeetingCount = propertyIsMeetingCount;
		}

		public static AdjacentOrConflictingMeetingsProperty CreateCommandForAdjacentMeetings(CommandContext commandContext)
		{
			return new AdjacentOrConflictingMeetingsProperty(commandContext, true, false);
		}

		public static AdjacentOrConflictingMeetingsProperty CreateCommandForConflictingMeetings(CommandContext commandContext)
		{
			return new AdjacentOrConflictingMeetingsProperty(commandContext, false, false);
		}

		public static AdjacentOrConflictingMeetingsProperty CreateCommandForAdjacentMeetingCount(CommandContext commandContext)
		{
			return new AdjacentOrConflictingMeetingsProperty(commandContext, true, true);
		}

		public static AdjacentOrConflictingMeetingsProperty CreateCommandForConflictingMeetingCount(CommandContext commandContext)
		{
			return new AdjacentOrConflictingMeetingsProperty(commandContext, false, true);
		}

		public void ToXml()
		{
			throw new InvalidOperationException("AdjacentOrConflictingMeetingsProperty.ToXml should not be called.");
		}

		public override bool ToServiceObjectRequiresMailboxAccess
		{
			get
			{
				return true;
			}
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			ServiceObject serviceObject = commandSettings.ServiceObject;
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			MailboxSession mailboxSession = commandSettings.StoreObject.Session as MailboxSession;
			if (mailboxSession == null)
			{
				return;
			}
			if (commandSettings.StoreObject.Id == null || commandSettings.StoreObject.Id.ObjectId.ProviderLevelItemId.Length == 0)
			{
				return;
			}
			MailboxId calendarItemMailboxOwnerMailboxId = null;
			CalendarItemBase calendarItemBase = commandSettings.StoreObject as CalendarItemBase;
			AdjacencyOrConflictInfo[] adjacentOrConflictingItems;
			if (calendarItemBase == null)
			{
				MeetingRequest meetingRequest = (MeetingRequest)commandSettings.StoreObject;
				adjacentOrConflictingItems = AdjacentOrConflictingMeetingsProperty.GetAdjacentOrConflictingItems(mailboxSession, meetingRequest, out calendarItemMailboxOwnerMailboxId);
			}
			else
			{
				adjacentOrConflictingItems = AdjacentOrConflictingMeetingsProperty.GetAdjacentOrConflictingItems(mailboxSession, calendarItemBase, out calendarItemMailboxOwnerMailboxId);
			}
			int num = 0;
			if (adjacentOrConflictingItems != null)
			{
				foreach (AdjacencyOrConflictInfo adjacencyOrConflictInfo in adjacentOrConflictingItems)
				{
					if (this.propertyIsAdjacentMeetings == (adjacencyOrConflictInfo.AdjacencyOrConflictType == AdjacencyOrConflictType.Precedes || adjacencyOrConflictInfo.AdjacencyOrConflictType == AdjacencyOrConflictType.Follows))
					{
						if (this.propertyIsMeetingCount)
						{
							num++;
						}
						else
						{
							EwsCalendarItemType item = AdjacentOrConflictingMeetingsProperty.CreateCalendarItemType(adjacencyOrConflictInfo, calendarItemMailboxOwnerMailboxId);
							if (!serviceObject.PropertyBag.Contains(propertyInformation))
							{
								serviceObject[propertyInformation] = new NonEmptyArrayOfAllItemsType();
							}
							((NonEmptyArrayOfAllItemsType)serviceObject[propertyInformation]).Add(item);
						}
					}
				}
				if (this.propertyIsMeetingCount)
				{
					serviceObject[propertyInformation] = num;
				}
			}
		}

		private static AdjacencyOrConflictInfo[] GetAdjacentOrConflictingItems(MailboxSession mailboxSession, MeetingRequest meetingRequest, out MailboxId calendarItemMailboxOwnerMailboxId)
		{
			AdjacencyOrConflictInfo[] result = null;
			calendarItemMailboxOwnerMailboxId = null;
			CalendarItemBase cachedEmbeddedItem = meetingRequest.GetCachedEmbeddedItem();
			if (meetingRequest.IsDelegated())
			{
				using (DelegateSessionHandleWrapper delegateSessionHandle = AdjacentOrConflictingMeetingsProperty.GetDelegateSessionHandle(meetingRequest))
				{
					if (delegateSessionHandle != null)
					{
						result = AdjacentOrConflictingMeetingsProperty.GetAdjacentOrConflictingItemsFromDefaultCalendarFolder(delegateSessionHandle.Handle.MailboxSession, cachedEmbeddedItem);
						calendarItemMailboxOwnerMailboxId = new MailboxId(delegateSessionHandle.Handle.MailboxSession);
					}
					return result;
				}
			}
			result = AdjacentOrConflictingMeetingsProperty.GetAdjacentOrConflictingItemsFromDefaultCalendarFolder(mailboxSession, cachedEmbeddedItem);
			calendarItemMailboxOwnerMailboxId = new MailboxId(mailboxSession);
			return result;
		}

		private static AdjacencyOrConflictInfo[] GetAdjacentOrConflictingItems(MailboxSession mailboxSession, CalendarItemBase calendarItemBase, out MailboxId calendarItemMailboxOwnerMailboxId)
		{
			AdjacencyOrConflictInfo[] result = null;
			using (Folder folder = Folder.Bind(mailboxSession, calendarItemBase.ParentId, null))
			{
				CalendarFolder calendarFolder = folder as CalendarFolder;
				if (calendarFolder != null)
				{
					result = calendarFolder.GetAdjacentOrConflictingItems(calendarItemBase);
				}
				else
				{
					result = AdjacentOrConflictingMeetingsProperty.GetAdjacentOrConflictingItemsFromDefaultCalendarFolder(mailboxSession, calendarItemBase);
				}
			}
			calendarItemMailboxOwnerMailboxId = new MailboxId(mailboxSession);
			return result;
		}

		private static EwsCalendarItemType CreateCalendarItemType(AdjacencyOrConflictInfo itemInfo, MailboxId calendarItemMailboxOwnerMailboxId)
		{
			ConcatenatedIdAndChangeKey concatenatedId = IdConverter.GetConcatenatedId(itemInfo.OccurrenceInfo.VersionedId, calendarItemMailboxOwnerMailboxId, null);
			return new EwsCalendarItemType
			{
				ItemId = new ItemId(concatenatedId.Id, concatenatedId.ChangeKey),
				Subject = itemInfo.Subject,
				Start = ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime(itemInfo.OccurrenceInfo.StartTime),
				End = ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime(itemInfo.OccurrenceInfo.EndTime),
				LegacyFreeBusyStatusString = BusyTypeConverter.ToString(itemInfo.FreeBusyStatus),
				Location = itemInfo.Location,
				IsAllDayEvent = new bool?(itemInfo.IsAllDayEvent)
			};
		}

		private static AdjacencyOrConflictInfo[] GetAdjacentOrConflictingItemsFromDefaultCalendarFolder(MailboxSession mailboxSession, CalendarItemBase calendarItemBase)
		{
			AdjacencyOrConflictInfo[] result;
			try
			{
				using (CalendarFolder calendarFolder = CalendarFolder.Bind(mailboxSession, DefaultFolderType.Calendar, null))
				{
					result = calendarFolder.GetAdjacentOrConflictingItems(calendarItemBase);
				}
			}
			catch (ObjectNotFoundException)
			{
				result = null;
			}
			return result;
		}

		private static DelegateSessionHandleWrapper GetDelegateSessionHandle(MeetingMessage meetingMessage)
		{
			MailboxSession mailboxSession = meetingMessage.Session as MailboxSession;
			if (mailboxSession.LogonType != LogonType.Owner)
			{
				return null;
			}
			Participant participant = MailboxHelper.TryConvertTo(meetingMessage.ReceivedRepresenting, "EX");
			if (participant != null && participant.RoutingType == "EX")
			{
				try
				{
					ExchangePrincipal exchangePrincipal = ExchangePrincipal.FromLegacyDN(mailboxSession.MailboxOwner.MailboxInfo.OrganizationId.ToADSessionSettings(), participant.EmailAddress, RemotingOptions.AllowCrossSite);
					return new DelegateSessionHandleWrapper(mailboxSession.GetDelegateSessionHandleForEWS(exchangePrincipal));
				}
				catch (ObjectNotFoundException arg)
				{
					ExTraceGlobals.CalendarDataTracer.TraceWarning<string, ObjectNotFoundException>((long)meetingMessage.GetHashCode(), "Unable to get exchange principal for receivedRepresenting '{0}' . Exception '{1}'.", participant.EmailAddress, arg);
				}
			}
			return null;
		}

		private const bool IsAdjacentMeetingProperty = true;

		private const bool IsMeetingCountProperty = true;

		private bool propertyIsAdjacentMeetings;

		private bool propertyIsMeetingCount;
	}
}
