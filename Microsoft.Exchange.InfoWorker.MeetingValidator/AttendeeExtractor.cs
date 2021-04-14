using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class AttendeeExtractor
	{
		public AttendeeExtractor(IRecipientSession recipientSession)
		{
			if (recipientSession == null)
			{
				throw new ArgumentNullException("recipientSession");
			}
			this.recipientSession = recipientSession;
			this.expansionManager = new ADRecipientExpansion(recipientSession, true, AttendeeExtractor.DLExpansionHandler.RecipientAdditionalRequiredProperties);
		}

		public IEnumerable<UserObject> ExtractAttendees(CalendarItemBase calendarItem, bool dlParticipantsOnly = false)
		{
			return this.ExtractAttendees(calendarItem.AttendeeCollection, calendarItem.Organizer, dlParticipantsOnly);
		}

		private static void RevisitAttendee(Dictionary<ADObjectId, UserObject> extractedAttendeeTable, Attendee attendee, ADRecipient attendeeRecipient)
		{
			UserObject userObject;
			if (!(attendeeRecipient is ADGroup) && extractedAttendeeTable.TryGetValue(attendeeRecipient.Id, out userObject) && (userObject.Attendee == null || userObject.Attendee.ReplyTime < attendee.ReplyTime))
			{
				userObject.Attendee = attendee;
			}
		}

		private IEnumerable<UserObject> ExtractAttendees(IAttendeeCollection attendeeCollection, Participant organizerParticipant, bool dlParticipantsOnly)
		{
			Dictionary<ADObjectId, UserObject> dictionary = new Dictionary<ADObjectId, UserObject>(attendeeCollection.Count);
			HashSet<ADObjectId> hashSet = new HashSet<ADObjectId>();
			Dictionary<ProxyAddress, UserObject> dictionary2 = new Dictionary<ProxyAddress, UserObject>();
			int expandedDLCount = 0;
			foreach (Attendee attendee in attendeeCollection)
			{
				if (CalendarValidator.IsValidParticipant(attendee.Participant))
				{
					ProxyAddress attendeeProxyAddress = ProxyAddress.Parse(attendee.Participant.RoutingType, attendee.Participant.EmailAddress);
					ADRecipient attendeeRecipient = null;
					ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
					{
						attendeeRecipient = this.recipientSession.FindByProxyAddress(attendeeProxyAddress);
					});
					if (!adoperationResult.Succeeded || attendeeRecipient == null)
					{
						this.ExtractUnaccessibleAttendee(organizerParticipant, dictionary2, attendee, attendeeProxyAddress);
					}
					else if (hashSet.Contains(attendeeRecipient.Id))
					{
						AttendeeExtractor.RevisitAttendee(dictionary, attendee, attendeeRecipient);
					}
					else if (attendeeRecipient is ADGroup)
					{
						AttendeeExtractor.DLExpansionHandler dlexpansionHandler = new AttendeeExtractor.DLExpansionHandler(organizerParticipant, dictionary, hashSet, expandedDLCount, attendee, attendeeRecipient, this.recipientSession, this.expansionManager);
						expandedDLCount = dlexpansionHandler.ExpandDL();
					}
					else if (!dlParticipantsOnly)
					{
						hashSet.Add(attendeeRecipient.Id);
						AttendeeExtractor.DLExpansionHandler.AddOrganizerFilteredAttendee<ADObjectId>(dictionary, attendeeRecipient.Id, new UserObject(attendee, attendeeRecipient, this.recipientSession), organizerParticipant, this.recipientSession);
					}
				}
			}
			return dictionary.Values.Concat(dictionary2.Values);
		}

		private void ExtractUnaccessibleAttendee(Participant organizerParticipant, Dictionary<ProxyAddress, UserObject> unaccessibleUsers, Attendee attendee, ProxyAddress attendeeProxyAddress)
		{
			if (!unaccessibleUsers.ContainsKey(attendeeProxyAddress))
			{
				AttendeeExtractor.DLExpansionHandler.AddOrganizerFilteredAttendee<ProxyAddress>(unaccessibleUsers, attendeeProxyAddress, new UserObject(attendee, null, this.recipientSession), organizerParticipant, this.recipientSession);
			}
		}

		private IRecipientSession recipientSession;

		private ADRecipientExpansion expansionManager;

		private sealed class DLExpansionHandler
		{
			public Participant OrganizerParticipant { get; private set; }

			public Dictionary<ADObjectId, UserObject> IndividualAttendees { get; private set; }

			public HashSet<ADObjectId> AllVisitedAttendees { get; private set; }

			public int ExpandedDLCount { get; private set; }

			public Attendee Attendee { get; private set; }

			public ADRawEntry AttendeeRawEntry { get; private set; }

			public IRecipientSession Session { get; private set; }

			public ADRecipientExpansion ExpansionManager { get; private set; }

			public DLExpansionHandler(Participant organizerParticipant, Dictionary<ADObjectId, UserObject> individualAttendees, HashSet<ADObjectId> allVisitedAttendees, int expandedDLCount, Attendee attendee, ADRawEntry attendeeRawEntry, IRecipientSession session, ADRecipientExpansion expansionManager)
			{
				this.OrganizerParticipant = organizerParticipant;
				this.IndividualAttendees = individualAttendees;
				this.AllVisitedAttendees = allVisitedAttendees;
				this.ExpandedDLCount = expandedDLCount;
				this.Attendee = attendee;
				this.AttendeeRawEntry = attendeeRawEntry;
				this.Session = session;
				this.ExpansionManager = expansionManager;
			}

			private bool ExpandAnotherDL()
			{
				if (this.ExpandedDLCount < Configuration.DLExpansionLimit)
				{
					this.ExpandedDLCount++;
					return true;
				}
				return false;
			}

			private ExpansionControl OnRecipientExtracted(ADRawEntry recipient, ExpansionType recipientExpansionType, ADRawEntry parent, ExpansionType parentExpansionType)
			{
				if (!this.AllVisitedAttendees.Add(recipient.Id))
				{
					return ExpansionControl.Skip;
				}
				if (recipientExpansionType != ExpansionType.GroupMembership)
				{
					AttendeeExtractor.DLExpansionHandler.AddOrganizerFilteredAttendee<ADObjectId>(this.IndividualAttendees, recipient.Id, new UserObject(recipient, this.Session), this.OrganizerParticipant, this.Session);
					return ExpansionControl.Continue;
				}
				if (this.ExpandAnotherDL())
				{
					return ExpansionControl.Continue;
				}
				AttendeeExtractor.DLExpansionHandler.AddOrganizerFilteredAttendee<ADObjectId>(this.IndividualAttendees, recipient.Id, new UserObject(recipient, this.Session), this.OrganizerParticipant, this.Session);
				return ExpansionControl.Skip;
			}

			private ExpansionControl OnFailureEncountered(ExpansionFailure failure, ADRawEntry recipient, ExpansionType recipientExpansionType, ADRawEntry parent, ExpansionType parentExpansionType)
			{
				this.AllVisitedAttendees.Add(recipient.Id);
				return ExpansionControl.Skip;
			}

			public static void AddOrganizerFilteredAttendee<KeyType>(Dictionary<KeyType, UserObject> attendeeTable, KeyType key, UserObject user, Participant organizer, IRecipientSession session)
			{
				if (!Participant.HasSameEmail(user.Participant, organizer, session))
				{
					attendeeTable.Add(key, user);
				}
			}

			public int ExpandDL()
			{
				this.ExpansionManager.Expand(this.AttendeeRawEntry, new ADRecipientExpansion.HandleRecipientDelegate(this.OnRecipientExtracted), new ADRecipientExpansion.HandleFailureDelegate(this.OnFailureEncountered));
				return this.ExpandedDLCount;
			}

			public static readonly PropertyDefinition[] RecipientAdditionalRequiredProperties = new PropertyDefinition[]
			{
				ADRecipientSchema.DisplayName,
				ADRecipientSchema.LegacyExchangeDN,
				ADRecipientSchema.RecipientTypeDetails,
				ADRecipientSchema.Alias,
				ADMailboxRecipientSchema.ExchangeGuid,
				ADMailboxRecipientSchema.Database,
				ADRecipientSchema.MasterAccountSid,
				ADObjectSchema.OrganizationId,
				ADUserSchema.CalendarRepairDisabled
			};
		}
	}
}
