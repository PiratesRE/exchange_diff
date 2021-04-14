using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ParticipantTable
	{
		public ParticipantTable()
		{
			this.data = new Dictionary<RecipientItemType, ParticipantSet>();
		}

		public ParticipantTable(IDictionary<RecipientItemType, IEnumerable<IParticipant>> otherRecipientTable) : this()
		{
			foreach (KeyValuePair<RecipientItemType, IEnumerable<IParticipant>> keyValuePair in otherRecipientTable)
			{
				this.Add(keyValuePair.Key, keyValuePair.Value);
			}
		}

		public ParticipantSet this[RecipientItemType type]
		{
			get
			{
				return this.GetOrCreateParticipantHash(type);
			}
			set
			{
				this.Add(type, value);
			}
		}

		public Dictionary<RecipientItemType, HashSet<IParticipant>> ToDictionary()
		{
			Dictionary<RecipientItemType, HashSet<IParticipant>> dictionary = new Dictionary<RecipientItemType, HashSet<IParticipant>>();
			foreach (KeyValuePair<RecipientItemType, ParticipantSet> keyValuePair in this.data)
			{
				dictionary[keyValuePair.Key] = new HashSet<IParticipant>(keyValuePair.Value);
			}
			return dictionary;
		}

		public Dictionary<RecipientItemType, ParticipantSet> ToParticipantSet()
		{
			return this.data;
		}

		public List<IParticipant> ToList()
		{
			return this.data.SelectMany((KeyValuePair<RecipientItemType, ParticipantSet> d) => d.Value).ToList<IParticipant>();
		}

		public void Add(RecipientItemType type, params IParticipant[] participants)
		{
			this.Add(type, participants.AsEnumerable<IParticipant>());
		}

		public void Add(RecipientItemType type, IEnumerable<IParticipant> participants)
		{
			ParticipantSet orCreateParticipantHash = this.GetOrCreateParticipantHash(type);
			orCreateParticipantHash.UnionWith(participants);
		}

		public bool Any
		{
			get
			{
				foreach (KeyValuePair<RecipientItemType, ParticipantSet> keyValuePair in this.data)
				{
					if (keyValuePair.Value.Any<IParticipant>())
					{
						return true;
					}
				}
				return false;
			}
		}

		private ParticipantSet GetOrCreateParticipantHash(RecipientItemType type)
		{
			ParticipantSet participantSet;
			if (!this.data.TryGetValue(type, out participantSet))
			{
				participantSet = new ParticipantSet();
				this.data.Add(type, participantSet);
			}
			return participantSet;
		}

		private Dictionary<RecipientItemType, ParticipantSet> data;
	}
}
