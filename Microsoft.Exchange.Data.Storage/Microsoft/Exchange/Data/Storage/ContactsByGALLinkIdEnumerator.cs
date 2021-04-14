using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ContactsByGALLinkIdEnumerator : IEnumerable<IStorePropertyBag>, IEnumerable
	{
		public ContactsByGALLinkIdEnumerator(MailboxSession session, DefaultFolderType defaultFolderType, Guid galLinkId, ICollection<PropertyDefinition> requestedProperties)
		{
			ArgumentValidator.ThrowIfNull("session", session);
			ArgumentValidator.ThrowIfEmpty("galLinkId", galLinkId);
			ArgumentValidator.ThrowIfNull("columns", requestedProperties);
			this.session = session;
			this.defaultFolderType = defaultFolderType;
			this.galLinkId = galLinkId;
			this.requestedProperties = requestedProperties;
		}

		public IEnumerator<IStorePropertyBag> GetEnumerator()
		{
			ContactsByGALLinkIdEnumerator.Tracer.TraceDebug<Guid>((long)this.galLinkId.GetHashCode(), "ContactsByGALLinkIdEnumerator.GetEnumerator: this.galLinkId = {0}", this.galLinkId);
			ComparisonFilter filter = new ComparisonFilter(ComparisonOperator.Equal, InternalSchema.GALLinkID, this.galLinkId);
			PropertyDefinition[] allProperties = PropertyDefinitionCollection.Merge<PropertyDefinition>(new IEnumerable<PropertyDefinition>[]
			{
				ContactsByGALLinkIdEnumerator.RequiredProperties,
				this.requestedProperties
			});
			Folder folder = Folder.Bind(this.session, this.defaultFolderType, Array<PropertyDefinition>.Empty);
			QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, null, ContactsByGALLinkIdEnumerator.SortByGalLinkId, allProperties);
			if (!queryResult.SeekToCondition(SeekReference.OriginBeginning, filter))
			{
				ContactsByGALLinkIdEnumerator.Tracer.TraceDebug<Guid>((long)this.galLinkId.GetHashCode(), "ContactsByGALLinkIdEnumerator.GetEnumerator: SeekToCondition = false.  No contacts with GALLinkID={0}.", this.galLinkId);
				yield break;
			}
			for (;;)
			{
				ContactsByGALLinkIdEnumerator.Tracer.TraceDebug<int>((long)this.galLinkId.GetHashCode(), "ContactsByGALLinkIdEnumerator.GetEnumerator: querying for {0} more property bags", 100);
				IStorePropertyBag[] contacts = queryResult.GetPropertyBags(100);
				if (contacts == null || contacts.Length == 0)
				{
					break;
				}
				foreach (IStorePropertyBag contact in contacts)
				{
					Guid contactGalLinkId = contact.GetValueOrDefault<Guid>(InternalSchema.GALLinkID, Guid.Empty);
					if (contactGalLinkId != this.galLinkId)
					{
						goto Block_3;
					}
					ContactsByGALLinkIdEnumerator.Tracer.TraceDebug((long)this.galLinkId.GetHashCode(), "ContactsByGALLinkIdEnumerator.GetEnumerator: found property bag");
					yield return contact;
				}
			}
			ContactsByGALLinkIdEnumerator.Tracer.TraceDebug((long)this.galLinkId.GetHashCode(), "ContactsByGALLinkIdEnumerator.GetEnumerator: no more property bags");
			yield break;
			Block_3:
			ContactsByGALLinkIdEnumerator.Tracer.TraceDebug((long)this.galLinkId.GetHashCode(), "ContactsByGALLinkIdEnumerator.GetEnumerator: no more property bags");
			yield break;
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotSupportedException("Must use the generics interface of GetEnumerator.");
		}

		private const int ContactBatchSize = 100;

		private static readonly Trace Tracer = ExTraceGlobals.PersonTracer;

		private static readonly SortBy[] SortByGalLinkId = new SortBy[]
		{
			new SortBy(InternalSchema.GALLinkID, SortOrder.Ascending)
		};

		private static readonly PropertyDefinition[] RequiredProperties = new PropertyDefinition[]
		{
			InternalSchema.GALLinkID
		};

		private readonly MailboxSession session;

		private readonly ICollection<PropertyDefinition> requestedProperties;

		private readonly DefaultFolderType defaultFolderType;

		private readonly Guid galLinkId;
	}
}
