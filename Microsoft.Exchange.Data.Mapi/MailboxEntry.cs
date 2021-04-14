using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Mapi.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Mapi
{
	[Serializable]
	public class MailboxEntry : MessageStore
	{
		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<MailboxEntry>(this);
		}

		protected override ObjectId GetIdentity()
		{
			object obj;
			if ((obj = this[MapiObjectSchema.Identity]) == null)
			{
				obj = (this[MapiObjectSchema.Identity] = new MailboxId());
			}
			return (ObjectId)obj;
		}

		public new MailboxId Identity
		{
			get
			{
				return (MailboxId)base.MapiIdentity;
			}
			internal set
			{
				base.MapiIdentity = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return MailboxEntry.schema;
			}
		}

		internal override void Delete()
		{
			if (ObjectState.Deleted == base.ObjectState)
			{
				throw new MapiInvalidOperationException(Strings.ExceptionObjectStateInvalid(base.ObjectState.ToString()));
			}
			if (base.ObjectState == ObjectState.New)
			{
				base.MarkAsDeleted();
				return;
			}
			if (null == this.Identity)
			{
				throw new MapiInvalidOperationException(Strings.ExceptionIdentityNull);
			}
			if ((null == this.Identity.MailboxDatabaseId || Guid.Empty == this.Identity.MailboxDatabaseId.Guid || Guid.Empty == this.Identity.MailboxGuid) && this.Identity.MailboxExchangeLegacyDn == null)
			{
				throw new MapiInvalidOperationException(Strings.ExceptionIdentityInvalid);
			}
			if (!(base.MapiSession is MapiAdministrationSession))
			{
				throw new MapiInvalidOperationException(Strings.ExceptionSessionInvalid);
			}
			base.EnableDisposeTracking();
			try
			{
				((MapiAdministrationSession)base.MapiSession).DeleteMailbox(this.Identity);
				if (!base.IsOriginatingServerRetrieved)
				{
					base.OriginatingServer = Fqdn.Parse(base.MapiSession.ServerName);
				}
			}
			finally
			{
				base.Dispose();
			}
			base.ResetChangeTracking();
			base.MarkAsDeleted();
		}

		internal override T[] Find<T>(QueryFilter filter, MapiObjectId root, QueryScope scope, SortBy sort, int maximumResultsSize)
		{
			return new List<T>(this.FindPaged<T>(filter, root, scope, sort, 0, maximumResultsSize)).ToArray();
		}

		internal override IEnumerable<T> FindPaged<T>(QueryFilter filter, MapiObjectId root, QueryScope scope, SortBy sort, int pageSize, int maximumResultsSize)
		{
			MailboxEntry.<>c__DisplayClass1<T> CS$<>8__locals1 = new MailboxEntry.<>c__DisplayClass1<T>();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.mailboxGuid = Guid.Empty;
			CS$<>8__locals1.mailboxTableFlags = MailboxTableFlags.MailboxTableFlagsNone;
			if (!base.GetType().IsAssignableFrom(typeof(T)))
			{
				throw new ArgumentException("T");
			}
			CS$<>8__locals1.databaseId = (root as DatabaseId);
			if (null == CS$<>8__locals1.databaseId)
			{
				throw new NotSupportedException(Strings.ExceptionIdentityTypeInvalid);
			}
			if (filter != null)
			{
				MailboxContextFilter mailboxContextFilter = filter as MailboxContextFilter;
				if (mailboxContextFilter == null)
				{
					throw new NotSupportedException(Strings.ExceptionIdentityTypeInvalid);
				}
				CS$<>8__locals1.mailboxGuid = mailboxContextFilter.MailboxGuid;
				CS$<>8__locals1.mailboxTableFlags |= (MailboxTableFlags)mailboxContextFilter.MailboxFlags;
				this.noADLookup = mailboxContextFilter.NoADLookup;
			}
			else
			{
				this.noADLookup = false;
			}
			if (MapiObject.RetrievePropertiesScope.Database != this.RetrievePropertiesScopeForFinding)
			{
				yield break;
			}
			if (QueryScope.SubTree != scope)
			{
				throw new ArgumentException("scope");
			}
			if (sort != null)
			{
				throw new ArgumentException("sort");
			}
			if (0 > maximumResultsSize)
			{
				throw new ArgumentException("maximumResultsSize");
			}
			if (base.MapiSession == null)
			{
				throw new MapiOperationException(Strings.ExceptionSessionNull);
			}
			base.EnableDisposeTracking();
			PropTag[] tagsToRead = base.GetPropertyTagsToRead();
			if (tagsToRead == null || tagsToRead.Length == 0)
			{
				tagsToRead = new PropTag[]
				{
					PropTag.UserGuid,
					PropTag.EmailAddress
				};
			}
			PropValue[][] entries = null;
			base.MapiSession.InvokeWithWrappedException(delegate()
			{
				entries = CS$<>8__locals1.<>4__this.MapiSession.Administration.GetMailboxTableInfo(CS$<>8__locals1.databaseId.Guid, CS$<>8__locals1.mailboxGuid, CS$<>8__locals1.mailboxTableFlags, tagsToRead);
			}, Strings.ExceptionFindObject(typeof(T).Name, (null == root) ? Strings.ConstantNull : root.ToString()), CS$<>8__locals1.databaseId);
			int resultSize = entries.Length;
			if (0 < maximumResultsSize && maximumResultsSize < resultSize)
			{
				resultSize = maximumResultsSize;
			}
			int entryIndex = 0;
			while (resultSize > entryIndex)
			{
				MailboxEntry mailbox = (MailboxEntry)((object)((default(T) == null) ? Activator.CreateInstance<T>() : default(T)));
				try
				{
					mailbox.Instantiate(entries[entryIndex]);
					mailbox.MapiSession = base.MapiSession;
					if (mailbox[MapiPropertyDefinitions.MailboxGuid] != null)
					{
						mailbox.Identity = new MailboxId(CS$<>8__locals1.databaseId, (Guid)mailbox[MapiPropertyDefinitions.MailboxGuid]);
					}
					mailbox.UpdateIdentity(mailbox.UpdateIdentityFlagsForFinding);
					mailbox.OriginatingServer = Fqdn.Parse(mailbox.MapiSession.ServerName);
					mailbox.ResetChangeTrackingAndObjectState();
				}
				finally
				{
					mailbox.Dispose();
				}
				yield return (T)((object)mailbox);
				entryIndex++;
			}
			yield break;
		}

		internal override void AdjustPropertyTagsToRead(List<PropTag> propertyTags)
		{
			base.AdjustPropertyTagsToRead(propertyTags);
			if (this.noADLookup)
			{
				propertyTags.RemoveAll((PropTag pt) => PropTag.StorageLimitInformation == pt);
			}
		}

		protected override MapiObject.RetrievePropertiesScope RetrievePropertiesScopeForFinding
		{
			get
			{
				return MapiObject.RetrievePropertiesScope.Database;
			}
		}

		protected override void UpdateIdentity(MapiObject.UpdateIdentityFlags flags)
		{
			bool flag = MapiObject.UpdateIdentityFlags.Nop == (MapiObject.UpdateIdentityFlags.SkipIfExists & flags);
			MapiEntryId entryId = this.Identity.MapiEntryId;
			DatabaseId mailboxDatabaseId = this.Identity.MailboxDatabaseId;
			Guid? guid = new Guid?(this.Identity.MailboxGuid);
			string mailboxExchangeLegacyDn = this.Identity.MailboxExchangeLegacyDn;
			if ((MapiObject.UpdateIdentityFlags.EntryIdentity & flags) != MapiObject.UpdateIdentityFlags.Nop && (null == this.Identity.MapiEntryId || flag))
			{
				entryId = (MapiEntryId)this[MapiPropertyDefinitions.EntryId];
			}
			if ((MapiObject.UpdateIdentityFlags.MailboxGuid & flags) != MapiObject.UpdateIdentityFlags.Nop && (Guid.Empty == this.Identity.MailboxGuid || flag))
			{
				guid = (Guid?)this[MapiPropertyDefinitions.MailboxGuid];
			}
			if ((MapiObject.UpdateIdentityFlags.LegacyDistinguishedName & flags) != MapiObject.UpdateIdentityFlags.Nop && (string.IsNullOrEmpty(this.Identity.MailboxExchangeLegacyDn) || flag))
			{
				mailboxExchangeLegacyDn = (string)this[MapiPropertyDefinitions.LegacyDN];
			}
			this.Identity = new MailboxId(entryId, mailboxDatabaseId, guid ?? Guid.Empty, mailboxExchangeLegacyDn);
		}

		public MailboxEntry()
		{
		}

		internal MailboxEntry(MailboxId mapiObjectId, MapiSession mapiSession) : base(mapiObjectId, mapiSession)
		{
		}

		private static MapiObjectSchema schema = ObjectSchema.GetInstance<MailboxEntry.MailboxEntrySchema>();

		private bool noADLookup;

		private class MailboxEntrySchema : MapiObjectSchema
		{
		}
	}
}
