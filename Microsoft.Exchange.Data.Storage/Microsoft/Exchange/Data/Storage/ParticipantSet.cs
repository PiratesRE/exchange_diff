using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ParticipantSet : IEnumerable<IParticipant>, IEnumerable
	{
		public int Count
		{
			get
			{
				return this.participantPerEmailAddress.Count;
			}
		}

		public bool IsSubsetOf(ParticipantSet other)
		{
			if (this.Count > other.Count)
			{
				return false;
			}
			foreach (IParticipant participant in this)
			{
				if (!other.Contains(participant))
				{
					return false;
				}
			}
			return true;
		}

		public void UnionWith(IEnumerable<IParticipant> other)
		{
			foreach (IParticipant participant in other)
			{
				this.Add(participant);
			}
		}

		public void ExceptWith(IEnumerable<IParticipant> others)
		{
			foreach (IParticipant participant in others)
			{
				if (this.Contains(participant))
				{
					this.Remove(participant);
				}
			}
		}

		public bool Add(IParticipant participant)
		{
			if (!this.Contains(participant))
			{
				this.participantPerEmailAddress.Add(participant);
				this.participantPerSmtpEmailAddress.Add(participant);
				return true;
			}
			return false;
		}

		public bool Contains(IParticipant participant)
		{
			return this.participantPerEmailAddress.Contains(participant) || this.participantPerSmtpEmailAddress.Contains(participant);
		}

		public IEnumerator<IParticipant> GetEnumerator()
		{
			return this.participantPerEmailAddress.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new InvalidOperationException("ParticipantSet:IEnumerable.GetEnumerator: Uses the generic version");
		}

		private void Remove(IParticipant participant)
		{
			IParticipant participant2 = this.participantPerEmailAddress.FirstOrDefault((IParticipant p) => ParticipantComparer.EmailAddress.Equals(p, participant));
			if (participant2 == null)
			{
				participant2 = this.participantPerSmtpEmailAddress.FirstOrDefault((IParticipant p) => ParticipantComparer.SmtpEmailAddress.Equals(p, participant));
			}
			this.participantPerEmailAddress.Remove(participant2);
			this.participantPerSmtpEmailAddress.Remove(participant2);
		}

		private HashSet<IParticipant> participantPerEmailAddress = new HashSet<IParticipant>(ParticipantComparer.EmailAddress);

		private HashSet<IParticipant> participantPerSmtpEmailAddress = new HashSet<IParticipant>(ParticipantComparer.SmtpEmailAddress);
	}
}
