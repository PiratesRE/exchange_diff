using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class AttendeeCollection : RecipientBaseCollection<Attendee>, IAttendeeCollectionImpl, IAttendeeCollection, IRecipientBaseCollection<Attendee>, IList<Attendee>, ICollection<Attendee>, IEnumerable<Attendee>, IEnumerable, ILocationIdentifierController
	{
		internal AttendeeCollection(CoreRecipientCollection coreRecipientCollection) : base(coreRecipientCollection)
		{
			if (coreRecipientCollection.CoreItem.Session != null)
			{
				this.Cleanup();
				this.LoadIsDistributionList();
			}
		}

		public Attendee Add(Participant participant, AttendeeType attendeeType = AttendeeType.Required, ResponseType? responseType = null, ExDateTime? replyTime = null, bool checkExisting = false)
		{
			return AttendeeCollection.InternalAdd(this, delegate
			{
				Attendee attendeeToAdd = this.GetAttendeeToAdd(participant);
				attendeeToAdd.AttendeeType = attendeeType;
				if (responseType != null)
				{
					attendeeToAdd.ResponseType = responseType.Value;
				}
				if (replyTime != null)
				{
					attendeeToAdd.ReplyTime = replyTime.Value;
				}
				this.ReportRecipientChange(LastChangeAction.RecipientAdded);
				return attendeeToAdd;
			}, participant, attendeeType, responseType, checkExisting);
		}

		public override Attendee Add(Participant participant)
		{
			if (participant == null)
			{
				throw new ArgumentNullException("participant");
			}
			return this.Add(participant, AttendeeType.Required, null, null, false);
		}

		public override void Add(Attendee item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			this.Add(item.Participant, item.AttendeeType, null, null, false);
		}

		internal static Attendee InternalAdd(IAttendeeCollection attendeeCollection, Func<Attendee> performAdd, Participant participant, AttendeeType attendeeType = AttendeeType.Required, ResponseType? responseType = null, bool checkExisting = false)
		{
			if (participant == null)
			{
				throw new ArgumentNullException("participant");
			}
			EnumValidator.ThrowIfInvalid<AttendeeType>(attendeeType, "attendeeType");
			if (responseType != null)
			{
				EnumValidator.ThrowIfInvalid<ResponseType>(responseType.Value, "responseType");
			}
			if (checkExisting)
			{
				int num = AttendeeCollection.IndexOf(attendeeCollection, participant, false);
				if (num != -1)
				{
					Attendee attendee = attendeeCollection[num];
					if (attendeeType != AttendeeType.Required || attendee.AttendeeType == AttendeeType.Required)
					{
						return attendee;
					}
					attendeeCollection.RemoveAt(num);
				}
			}
			return performAdd();
		}

		internal static void Cleanup(IAttendeeCollection attendees)
		{
			HashSet<string> hashSet = new HashSet<string>();
			List<Attendee> list = new List<Attendee>();
			foreach (Attendee attendee in attendees)
			{
				if (attendee.Participant.RoutingType == "SMTP" || attendee.Participant.RoutingType == "EX")
				{
					RecipientFlags recipientFlags = attendee.RecipientFlags;
					bool flag = (recipientFlags & RecipientFlags.ExceptionalDeleted) == RecipientFlags.ExceptionalDeleted;
					string item = attendee.GetAttendeeKey() + (flag ? ":D" : ":ND");
					if (!hashSet.Contains(item))
					{
						hashSet.Add(item);
					}
					else
					{
						list.Add(attendee);
					}
				}
			}
			foreach (Attendee item2 in list)
			{
				attendees.Remove(item2);
			}
		}

		private static int IndexOf(IAttendeeCollection attendeeCollection, Participant participant, bool canLookup)
		{
			int count = attendeeCollection.Count;
			for (int i = 0; i < count; i++)
			{
				if (Participant.HasSameEmail(attendeeCollection[i].Participant, participant, canLookup))
				{
					return i;
				}
			}
			return -1;
		}

		internal void Cleanup()
		{
			((IAttendeeCollectionImpl)this).Cleanup();
		}

		internal void LoadIsDistributionList()
		{
			((IAttendeeCollectionImpl)this).LoadIsDistributionList();
		}

		void IAttendeeCollectionImpl.Cleanup()
		{
			AttendeeCollection.Cleanup(this);
		}

		void IAttendeeCollectionImpl.LoadIsDistributionList()
		{
			base.LoadAdditionalParticipantProperties(new PropertyDefinition[]
			{
				ParticipantSchema.IsDistributionList
			});
		}

		internal Attendee AddClone(Attendee item)
		{
			CoreRecipient coreRecipient = base.CoreRecipientCollection.CreateCoreRecipient(item.CoreRecipient);
			Attendee result = this.ConstructStronglyTypedRecipient(coreRecipient);
			this.ReportRecipientChange(LastChangeAction.RecipientAdded);
			return result;
		}

		protected override Attendee ConstructStronglyTypedRecipient(CoreRecipient coreRecipient)
		{
			return new Attendee(coreRecipient);
		}

		public override void Clear()
		{
			this.LocationIdentifierHelperInstance.SetLocationIdentifier(49013U);
			base.Clear();
		}

		public override void Remove(RecipientId id)
		{
			this.LocationIdentifierHelperInstance.SetLocationIdentifier(65397U);
			base.Remove(id);
		}

		public override void RemoveAt(int index)
		{
			this.LocationIdentifierHelperInstance.SetLocationIdentifier(40821U);
			base.RemoveAt(index);
		}

		protected override void OnRemoveRecipient()
		{
			this.ReportRecipientChange(LastChangeAction.RecipientRemoved);
		}

		private Attendee GetAttendeeToAdd(Participant participant)
		{
			CoreRecipient coreRecipient = base.CreateCoreRecipient(new CoreRecipient.SetDefaultPropertiesDelegate(Attendee.SetDefaultAttendeeProperties), participant);
			return this.ConstructStronglyTypedRecipient(coreRecipient);
		}

		private void ReportRecipientChange(LastChangeAction action)
		{
			this.LocationIdentifierHelperInstance.SetLocationIdentifier(59893U, action);
		}

		public LocationIdentifierHelper LocationIdentifierHelperInstance
		{
			get
			{
				return base.CoreRecipientCollection.LocationIdentifierHelperInstance;
			}
		}
	}
}
