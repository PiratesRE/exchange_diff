using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal class AttendeeData
	{
		public AttendeeType AttendeeType
		{
			get
			{
				return this.attendeeType;
			}
			set
			{
				this.attendeeType = value;
			}
		}

		public Participant Participant
		{
			get
			{
				return this.participant;
			}
			set
			{
				this.participant = value;
			}
		}

		public byte[] RecipientIdBytes
		{
			get
			{
				return this.recipientIdBytes;
			}
			set
			{
				this.recipientIdBytes = value;
			}
		}

		public AttendeeData()
		{
		}

		public AttendeeData(AttendeeData other)
		{
			this.attendeeType = other.attendeeType;
			this.participant = AttendeeData.CloneParticipant(other.participant);
			this.recipientIdBytes = (byte[])other.recipientIdBytes.Clone();
		}

		public AttendeeData(Participant participant, AttendeeType attendeeType)
		{
			this.attendeeType = attendeeType;
			this.participant = AttendeeData.CloneParticipant(participant);
		}

		public AttendeeData(Attendee attendee)
		{
			this.attendeeType = attendee.AttendeeType;
			this.participant = AttendeeData.CloneParticipant(attendee.Participant);
			this.recipientIdBytes = attendee.Id.GetBytes();
		}

		public static Participant CloneParticipant(Participant participant)
		{
			Participant result = null;
			if (participant != null)
			{
				result = new Participant(participant.DisplayName, participant.EmailAddress, participant.RoutingType, participant.Origin, new KeyValuePair<PropertyDefinition, object>[0]);
			}
			return result;
		}

		public static bool AreListsEqual(List<AttendeeData> list1, List<AttendeeData> list2)
		{
			if (list1 == null != (list2 == null))
			{
				return false;
			}
			if (list1 == null)
			{
				return true;
			}
			if (list1.Count != list2.Count)
			{
				return false;
			}
			for (int i = 0; i < list1.Count; i++)
			{
				AttendeeData attendeeData = list1[i];
				AttendeeData obj = list2[i];
				if (!attendeeData.Equals(obj))
				{
					return false;
				}
			}
			return true;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			AttendeeData attendeeData = obj as AttendeeData;
			if (attendeeData == null)
			{
				return false;
			}
			if (this.attendeeType != attendeeData.attendeeType)
			{
				return false;
			}
			if (this.participant == null != (attendeeData.participant == null))
			{
				return false;
			}
			if (this.participant != null && !this.participant.Equals(attendeeData.participant))
			{
				return false;
			}
			if (this.recipientIdBytes == null != (attendeeData.recipientIdBytes == null))
			{
				return false;
			}
			if (this.recipientIdBytes != null)
			{
				if (this.recipientIdBytes.Length != attendeeData.recipientIdBytes.Length)
				{
					return false;
				}
				for (int i = 0; i < this.recipientIdBytes.Length; i++)
				{
					if (this.recipientIdBytes[i] != attendeeData.recipientIdBytes[i])
					{
						return false;
					}
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int num = (int)this.attendeeType;
			if (this.participant != null)
			{
				num += this.participant.GetHashCode();
			}
			if (this.recipientIdBytes != null)
			{
				num += this.recipientIdBytes.GetHashCode();
			}
			return num;
		}

		private AttendeeType attendeeType;

		private Participant participant;

		private byte[] recipientIdBytes;
	}
}
