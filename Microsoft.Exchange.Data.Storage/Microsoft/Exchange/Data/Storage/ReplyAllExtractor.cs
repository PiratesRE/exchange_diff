using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensions;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ReplyAllExtractor : IReplyAllExtractor
	{
		public ReplyAllExtractor(IMailboxSession session, IXSOFactory xsoFactory)
		{
			this.session = session;
			this.xsoFactory = xsoFactory;
		}

		public bool TryRetrieveReplyAllDisplayNames(IStorePropertyBag propertyBag, out HashSet<string> displayNames)
		{
			ReplyAllParticipantsRepresentationProperty<string> replyAllDisplayNames = InternalSchema.ReplyAllDisplayNames;
			object obj = propertyBag.TryGetProperty(replyAllDisplayNames);
			displayNames = null;
			if (PropertyError.IsPropertyError(obj))
			{
				return false;
			}
			displayNames = this.RetrieveReplyAllData((IDictionary<RecipientItemType, HashSet<string>>)obj, replyAllDisplayNames.ParticipantRepresentationComparer);
			return true;
		}

		public HashSet<string> RetrieveReplyAllDisplayNames(ICorePropertyBag propertyBag)
		{
			ReplyAllDisplayNamesProperty replyAllDisplayNames = InternalSchema.ReplyAllDisplayNames;
			object obj = propertyBag.TryGetProperty(replyAllDisplayNames);
			return this.RetrieveReplyAllData((IDictionary<RecipientItemType, HashSet<string>>)obj, replyAllDisplayNames.ParticipantRepresentationComparer);
		}

		public ParticipantSet RetrieveReplyAllParticipants(ICorePropertyBag propertyBag)
		{
			ReplyAllParticipantsProperty replyAllParticipants = InternalSchema.ReplyAllParticipants;
			object obj = propertyBag.TryGetProperty(replyAllParticipants);
			return this.RetrieveReplyAllData((IDictionary<RecipientItemType, HashSet<IParticipant>>)obj);
		}

		public ParticipantSet RetrieveReplyAllParticipants(IStorePropertyBag propertyBag)
		{
			ParticipantSet result;
			using (IItem item = this.xsoFactory.BindToItem(this.session, (VersionedId)propertyBag.TryGetProperty(ItemSchema.Id), new PropertyDefinition[0]))
			{
				ReplyAllParticipantsProperty replyAllParticipants = InternalSchema.ReplyAllParticipants;
				object obj = item.TryGetProperty(replyAllParticipants);
				result = this.RetrieveReplyAllData((IDictionary<RecipientItemType, HashSet<IParticipant>>)obj);
			}
			return result;
		}

		private ParticipantSet RetrieveReplyAllData(IDictionary<RecipientItemType, HashSet<IParticipant>> replyAllTable)
		{
			ParticipantSet participantSet = new ParticipantSet();
			foreach (KeyValuePair<RecipientItemType, HashSet<IParticipant>> keyValuePair in replyAllTable)
			{
				participantSet.UnionWith(keyValuePair.Value);
			}
			return participantSet;
		}

		private HashSet<string> RetrieveReplyAllData(IDictionary<RecipientItemType, HashSet<string>> replyAllTable, IEqualityComparer<string> comparer)
		{
			HashSet<string> hashSet = new HashSet<string>(comparer);
			foreach (KeyValuePair<RecipientItemType, HashSet<string>> keyValuePair in replyAllTable)
			{
				hashSet.AddRange(keyValuePair.Value);
			}
			return hashSet;
		}

		private readonly IMailboxSession session;

		private readonly IXSOFactory xsoFactory;
	}
}
