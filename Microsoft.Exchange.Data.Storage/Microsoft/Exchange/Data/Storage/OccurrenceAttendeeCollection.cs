using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class OccurrenceAttendeeCollection : IAttendeeCollectionImpl, IAttendeeCollection, IRecipientBaseCollection<Attendee>, IList<Attendee>, ICollection<Attendee>, IEnumerable<Attendee>, IEnumerable
	{
		internal OccurrenceAttendeeCollection(CalendarItemOccurrence occurrence)
		{
			this.occurrence = occurrence;
			this.LoadMasterAttendeeCollection();
			CoreRecipientCollection recipients;
			if (occurrence.OccurrencePropertyBag.ExceptionMessage != null)
			{
				recipients = occurrence.OccurrencePropertyBag.ExceptionMessage.CoreItem.Recipients;
			}
			else
			{
				recipients = occurrence.OccurrencePropertyBag.MasterCalendarItem.CoreItem.Recipients;
				CalendarItem masterCalendarItem = occurrence.OccurrencePropertyBag.MasterCalendarItem;
			}
			this.exceptionAttendeeCollection = new AttendeeCollection(recipients);
			this.BuildOccurrenceAttendeeCollection();
		}

		public Attendee Add(Participant participant, AttendeeType attendeeType = AttendeeType.Required, ResponseType? responseType = null, ExDateTime? replyTime = null, bool checkExisting = false)
		{
			return AttendeeCollection.InternalAdd(this, delegate
			{
				int num;
				Attendee attendee = OccurrenceAttendeeCollection.FindAttendee(this.exceptionAttendeeCollection, participant, this.occurrence.Session as MailboxSession, out num);
				if (attendee != null && attendee.HasFlags(RecipientFlags.ExceptionalDeleted))
				{
					attendee.RecipientFlags &= ~RecipientFlags.ExceptionalDeleted;
					this.exceptionAttendeeCollection.Remove(attendee);
					this.exceptionAttendeeCollection.LocationIdentifierHelperInstance.SetLocationIdentifier(46965U);
					attendee = this.exceptionAttendeeCollection.AddClone(attendee);
					attendee.AttendeeType = attendeeType;
					if (responseType != null)
					{
						attendee.ResponseType = responseType.Value;
					}
					if (replyTime != null)
					{
						attendee.ReplyTime = replyTime.Value;
					}
				}
				else
				{
					attendee = this.exceptionAttendeeCollection.Add(participant, attendeeType, responseType, replyTime, checkExisting);
				}
				this.attendeeCollection.Add(attendee);
				return attendee;
			}, participant, attendeeType, responseType, checkExisting);
		}

		void IAttendeeCollectionImpl.Cleanup()
		{
			this.Cleanup();
		}

		void IAttendeeCollectionImpl.LoadIsDistributionList()
		{
			this.LoadIsDistributionList();
		}

		public Attendee Add(Participant participant)
		{
			if (participant == null)
			{
				throw new ArgumentNullException("participant");
			}
			return this.Add(participant, AttendeeType.Required, null, null, false);
		}

		public Attendee this[RecipientId id]
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public void Remove(RecipientId id)
		{
			throw new NotImplementedException();
		}

		public int IndexOf(Attendee item)
		{
			return this.attendeeCollection.IndexOf(item);
		}

		public void Insert(int index, Attendee item)
		{
			throw new NotSupportedException();
		}

		public void RemoveAt(int index)
		{
			Attendee match = this.attendeeCollection[index];
			this.attendeeCollection.RemoveAt(index);
			MailboxSession session = this.occurrence.Session as MailboxSession;
			int num;
			Attendee attendee = OccurrenceAttendeeCollection.FindAttendee(this.masterAttendeeCollection, match, session, out num);
			int index2;
			Attendee attendee2 = OccurrenceAttendeeCollection.FindAttendee(this.exceptionAttendeeCollection, match, session, out index2);
			if (attendee == null && attendee2 != null)
			{
				this.exceptionAttendeeCollection.LocationIdentifierHelperInstance.SetLocationIdentifier(50677U);
				this.exceptionAttendeeCollection.RemoveAt(index2);
			}
			else if (attendee != null)
			{
				if (attendee2 == null)
				{
					this.exceptionAttendeeCollection.LocationIdentifierHelperInstance.SetLocationIdentifier(47605U);
					attendee2 = this.exceptionAttendeeCollection.AddClone(attendee);
				}
				attendee2.RecipientFlags |= RecipientFlags.ExceptionalDeleted;
			}
			if (index < this.masterAttendeeCount)
			{
				this.masterAttendeeCount--;
			}
		}

		public Attendee this[int index]
		{
			get
			{
				return this.attendeeCollection[index];
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public void Add(Attendee attendee)
		{
			if (attendee == null)
			{
				throw new ArgumentNullException("attendee");
			}
			this.Add(attendee.Participant, attendee.AttendeeType, null, null, false);
		}

		public bool Remove(Attendee attendee)
		{
			if (attendee == null)
			{
				throw new ArgumentNullException("attendee");
			}
			int num = this.IndexOf(attendee);
			if (num != -1)
			{
				this.RemoveAt(num);
			}
			return num != -1;
		}

		public void Clear()
		{
			for (int i = this.attendeeCollection.Count - 1; i >= 0; i--)
			{
				this.RemoveAt(i);
			}
		}

		public bool Contains(Attendee item)
		{
			return this.attendeeCollection.Contains(item);
		}

		public void CopyTo(Attendee[] array, int arrayIndex)
		{
			this.attendeeCollection.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get
			{
				return this.attendeeCollection.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public IEnumerator<Attendee> GetEnumerator()
		{
			return this.attendeeCollection.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.attendeeCollection.GetEnumerator();
		}

		internal bool IsDirty
		{
			get
			{
				return this.masterAttendeeCollection.IsDirty || this.exceptionAttendeeCollection.IsDirty;
			}
		}

		internal void ApplyChangesToExceptionAttendeeCollection(MapiMessage exceptionMessage)
		{
			if (this.IsDirty)
			{
				this.Cleanup();
				for (int i = 0; i < this.masterAttendeeCount; i++)
				{
					Attendee attendee = this.attendeeCollection[i];
					if (attendee.IsDirty && !this.exceptionAttendeeCollection.Contains(attendee))
					{
						this.exceptionAttendeeCollection.AddClone(attendee);
					}
				}
				if (this.exceptionAttendeeCollection.CoreItem.MapiMessage != exceptionMessage)
				{
					CoreRecipientCollection recipients = this.occurrence.OccurrencePropertyBag.ExceptionMessage.CoreItem.Recipients;
					AttendeeCollection attendeeCollection = new AttendeeCollection(recipients);
					foreach (Attendee attendee2 in this.exceptionAttendeeCollection)
					{
						if (attendee2.IsDirty)
						{
							attendeeCollection.AddClone(attendee2);
						}
					}
					this.exceptionAttendeeCollection = attendeeCollection;
					this.occurrence.OccurrencePropertyBag.MasterCalendarItem.AbandonRecipientChanges();
					this.LoadMasterAttendeeCollection();
					this.BuildOccurrenceAttendeeCollection();
				}
			}
		}

		private static Attendee FindAttendee(IList<Attendee> list, Participant participant, MailboxSession session, out int index)
		{
			index = -1;
			Attendee result = null;
			for (int i = 0; i < list.Count; i++)
			{
				Attendee attendee = list[i];
				if (Participant.HasSameEmail(attendee.Participant, participant, session, false))
				{
					result = attendee;
					index = i;
					break;
				}
			}
			return result;
		}

		private static Attendee FindAttendee(IList<Attendee> list, Attendee match, MailboxSession session, out int index)
		{
			index = -1;
			foreach (Attendee attendee in list)
			{
				index++;
				if (Participant.HasSameEmail(attendee.Participant, match.Participant, session, false) && attendee.AttendeeType == match.AttendeeType)
				{
					return attendee;
				}
			}
			index = -1;
			return null;
		}

		private void Cleanup()
		{
			AttendeeCollection.Cleanup(this.masterAttendeeCollection);
			AttendeeCollection.Cleanup(this.exceptionAttendeeCollection);
		}

		private void LoadIsDistributionList()
		{
			this.masterAttendeeCollection.LoadAdditionalParticipantProperties(new PropertyDefinition[]
			{
				ParticipantSchema.IsDistributionList
			});
			this.exceptionAttendeeCollection.LoadAdditionalParticipantProperties(new PropertyDefinition[]
			{
				ParticipantSchema.IsDistributionList
			});
		}

		private void LoadMasterAttendeeCollection()
		{
			CoreRecipientCollection recipients = this.occurrence.OccurrencePropertyBag.MasterCalendarItem.CoreItem.Recipients;
			this.masterAttendeeCollection = new AttendeeCollection(recipients);
		}

		private void MergeCollections()
		{
			Dictionary<string, Attendee> uniqueExceptionAttendees = this.GetUniqueExceptionAttendees();
			this.JoinAttendees(uniqueExceptionAttendees);
			this.RemoveExceptionalDeletedAttendees(uniqueExceptionAttendees);
		}

		private Dictionary<string, Attendee> GetUniqueExceptionAttendees()
		{
			Dictionary<string, Attendee> dictionary = new Dictionary<string, Attendee>();
			foreach (Attendee attendee in this.exceptionAttendeeCollection)
			{
				string attendeeKey = attendee.GetAttendeeKey();
				if (!dictionary.ContainsKey(attendeeKey))
				{
					dictionary.Add(attendeeKey, attendee);
				}
				else
				{
					Attendee attendee2;
					dictionary.TryGetValue(attendeeKey, out attendee2);
					RecipientFlags recipientFlags = attendee2.RecipientFlags;
					if ((recipientFlags & RecipientFlags.ExceptionalDeleted) == RecipientFlags.ExceptionalDeleted)
					{
						dictionary.Remove(attendeeKey);
						dictionary.Add(attendeeKey, attendee);
					}
				}
			}
			return dictionary;
		}

		private void JoinAttendees(Dictionary<string, Attendee> uniqueExceptionAttendees)
		{
			foreach (Attendee item in this.masterAttendeeCollection)
			{
				this.attendeeCollection.Add(item);
			}
			this.masterAttendeeCount = this.masterAttendeeCollection.Count;
			if (this.masterAttendeeCollection.CoreItem != this.exceptionAttendeeCollection.CoreItem)
			{
				foreach (KeyValuePair<string, Attendee> keyValuePair in uniqueExceptionAttendees)
				{
					RecipientFlags recipientFlags = keyValuePair.Value.RecipientFlags;
					if ((recipientFlags & RecipientFlags.ExceptionalDeleted) != RecipientFlags.ExceptionalDeleted)
					{
						this.AddExceptionalAttendee(keyValuePair.Value);
					}
				}
			}
		}

		private void RemoveExceptionalDeletedAttendees(Dictionary<string, Attendee> uniqueExceptionAttendees)
		{
			int num = 0;
			List<Attendee> list = new List<Attendee>();
			foreach (KeyValuePair<string, Attendee> keyValuePair in uniqueExceptionAttendees)
			{
				Attendee value = keyValuePair.Value;
				if (value.Participant.RoutingType != null && value.Participant.EmailAddress != null)
				{
					string key = keyValuePair.Key;
					RecipientFlags recipientFlags = value.RecipientFlags;
					if ((recipientFlags & RecipientFlags.ExceptionalDeleted) == RecipientFlags.ExceptionalDeleted)
					{
						int i = 0;
						while (i < this.attendeeCollection.Count)
						{
							Attendee attendee = this.attendeeCollection[i];
							string attendeeKey = attendee.GetAttendeeKey();
							if (key == attendeeKey)
							{
								list.Add(attendee);
								if (i < this.masterAttendeeCount)
								{
									num++;
									break;
								}
								break;
							}
							else
							{
								i++;
							}
						}
					}
				}
			}
			this.masterAttendeeCount -= num;
			foreach (Attendee item in list)
			{
				this.attendeeCollection.Remove(item);
			}
		}

		private void AddExceptionalAttendee(Attendee attendee)
		{
			int num;
			Attendee attendee2 = OccurrenceAttendeeCollection.FindAttendee(this.attendeeCollection, attendee, this.occurrence.Session as MailboxSession, out num);
			if (attendee2 != null && num < this.masterAttendeeCount)
			{
				this.attendeeCollection.RemoveAt(num);
			}
			this.attendeeCollection.Add(attendee);
		}

		private void BuildOccurrenceAttendeeCollection()
		{
			this.attendeeCollection.Clear();
			this.Cleanup();
			this.LoadIsDistributionList();
			this.MergeCollections();
		}

		private readonly CalendarItemOccurrence occurrence;

		private readonly List<Attendee> attendeeCollection = new List<Attendee>();

		private int masterAttendeeCount;

		private AttendeeCollection masterAttendeeCollection;

		private AttendeeCollection exceptionAttendeeCollection;
	}
}
