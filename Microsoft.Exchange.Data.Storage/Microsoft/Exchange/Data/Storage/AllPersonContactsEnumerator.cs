using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class AllPersonContactsEnumerator : IEnumerable<IStorePropertyBag>, IEnumerable
	{
		protected AllPersonContactsEnumerator(MailboxSession session, ICollection<PropertyDefinition> columns)
		{
			this.session = session;
			this.columns = columns;
		}

		protected MailboxSession Session
		{
			get
			{
				return this.session;
			}
		}

		protected ICollection<PropertyDefinition> Columns
		{
			get
			{
				return this.columns;
			}
		}

		public static AllPersonContactsEnumerator Create(MailboxSession session, PersonId personId, ICollection<PropertyDefinition> columns)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(personId, "personId");
			Util.ThrowOnNullArgument(columns, "columns");
			return new AllPersonContactsEnumerator.ByPersonId(session, personId, columns);
		}

		public abstract IEnumerator<IStorePropertyBag> GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotSupportedException("Must use the generics interface of GetEnumerator.");
		}

		private const int ContactBatchSize = 100;

		private static readonly Trace Tracer = ExTraceGlobals.PersonTracer;

		private readonly MailboxSession session;

		private readonly ICollection<PropertyDefinition> columns;

		private sealed class ByPersonId : AllPersonContactsEnumerator
		{
			public ByPersonId(MailboxSession session, PersonId personId, ICollection<PropertyDefinition> columns) : base(session, columns)
			{
				this.personId = personId;
			}

			public override IEnumerator<IStorePropertyBag> GetEnumerator()
			{
				AllPersonContactsEnumerator.Tracer.TraceDebug<PersonId>(PersonId.TraceId(this.personId), "AllPersonContactsEnumerator.GetEnumerator: this.personId = {0}", this.personId);
				ComparisonFilter filterByPersonId = new ComparisonFilter(ComparisonOperator.Equal, InternalSchema.MapiConversationId, this.personId.GetBytes());
				Folder rootFolder = Folder.Bind(base.Session, DefaultFolderType.Configuration, Array<PropertyDefinition>.Empty);
				QueryResult queryResult = ((CoreFolder)rootFolder.CoreObject).QueryExecutor.InternalItemQuery(ContentsTableFlags.ShowConversationMembers, filterByPersonId, null, QueryExclusionType.Row, this.columns, null);
				for (;;)
				{
					AllPersonContactsEnumerator.Tracer.TraceDebug<int>(PersonId.TraceId(this.personId), "AllPersonContactsEnumerator.GetEnumerator: querying for {0} more property bags", 100);
					IStorePropertyBag[] contacts = queryResult.GetPropertyBags(100);
					if (contacts == null || contacts.Length == 0)
					{
						break;
					}
					foreach (IStorePropertyBag contact in contacts)
					{
						AllPersonContactsEnumerator.Tracer.TraceDebug(PersonId.TraceId(this.personId), "AllPersonContactsEnumerator.GetEnumerator: found property bag");
						yield return contact;
					}
				}
				AllPersonContactsEnumerator.Tracer.TraceDebug(PersonId.TraceId(this.personId), "AllPersonContactsEnumerator.GetEnumerator: no more property bags");
				yield break;
				yield break;
			}

			private readonly PersonId personId;
		}
	}
}
