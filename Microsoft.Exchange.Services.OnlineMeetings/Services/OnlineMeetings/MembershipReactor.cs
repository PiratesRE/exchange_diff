using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.Services.OnlineMeetings
{
	internal class MembershipReactor<T>
	{
		internal MembershipReactor(IEqualityComparer<T> comparer)
		{
			this.comparer = comparer;
		}

		internal IEnumerable<T> Added
		{
			get
			{
				return this.addedMembers;
			}
		}

		internal IEnumerable<T> Removed
		{
			get
			{
				return this.removedMembers;
			}
		}

		internal IEnumerable<T> Modified
		{
			get
			{
				return this.modifiedMembers;
			}
		}

		internal void Fusion(IEnumerable<T> existingParticipants, IEnumerable<T> updatedParticipants)
		{
			this.addedMembers = updatedParticipants.Except(existingParticipants, this.comparer);
			this.removedMembers = existingParticipants.Except(updatedParticipants, this.comparer);
			this.modifiedMembers = updatedParticipants.Except(this.addedMembers, this.comparer).Except(this.removedMembers, this.comparer);
		}

		private readonly IEqualityComparer<T> comparer;

		private IEnumerable<T> addedMembers;

		private IEnumerable<T> modifiedMembers;

		private IEnumerable<T> removedMembers;
	}
}
