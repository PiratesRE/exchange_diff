using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Mapi.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Mapi
{
	[Serializable]
	public class MailboxResourceMonitorEntry : MapiObject
	{
		protected sealed override ObjectId GetIdentity()
		{
			object obj;
			if ((obj = this[MapiObjectSchema.Identity]) == null)
			{
				obj = (this[MapiObjectSchema.Identity] = new MailboxId());
			}
			return (ObjectId)obj;
		}

		protected sealed override MapiObject.RetrievePropertiesScope RetrievePropertiesScopeForFinding
		{
			get
			{
				return MapiObject.RetrievePropertiesScope.Database;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return MailboxResourceMonitorEntry.schema;
			}
		}

		internal sealed override void Save(bool keepUnmanagedResources)
		{
			throw new NotImplementedException("The method Save is not implemented.");
		}

		internal sealed override void Read(bool keepUnmanagedResources)
		{
			throw new NotImplementedException("The method Read is not implemented.");
		}

		internal sealed override void Delete()
		{
			throw new NotImplementedException("The method Delete is not implemented.");
		}

		internal sealed override MapiProp RawMapiEntry
		{
			get
			{
				throw new NotImplementedException("The property RawMapiEntry is not implemented.");
			}
		}

		protected sealed override void UpdateIdentity(MapiObject.UpdateIdentityFlags flags)
		{
			MapiEntryId mapiEntryId = this.Identity.MapiEntryId;
			DatabaseId mailboxDatabaseId = this.Identity.MailboxDatabaseId;
			Guid mailboxGuid = this.Identity.MailboxGuid;
			string mailboxExchangeLegacyDn = this.Identity.MailboxExchangeLegacyDn;
			this.Identity = new MailboxId(mapiEntryId, mailboxDatabaseId, mailboxGuid, mailboxExchangeLegacyDn);
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<MailboxResourceMonitorEntry>(this);
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

		internal override T[] Find<T>(QueryFilter filter, MapiObjectId root, QueryScope scope, SortBy sort, int maximumResultsSize)
		{
			return new List<T>(this.FindPaged<T>(filter, root, scope, sort, 0, maximumResultsSize)).ToArray();
		}

		internal override IEnumerable<T> FindPaged<T>(QueryFilter filter, MapiObjectId root, QueryScope scope, SortBy sort, int pageSize, int maximumResultsSize)
		{
			MailboxResourceMonitorEntry.<>c__DisplayClass1<T> CS$<>8__locals1 = new MailboxResourceMonitorEntry.<>c__DisplayClass1<T>();
			CS$<>8__locals1.<>4__this = this;
			if (!base.GetType().IsAssignableFrom(typeof(T)))
			{
				throw new ArgumentException("T");
			}
			CS$<>8__locals1.databaseId = (root as DatabaseId);
			if (null == CS$<>8__locals1.databaseId)
			{
				throw new NotSupportedException(Strings.ExceptionIdentityTypeInvalid);
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
				throw new MapiInvalidOperationException(Strings.ExceptionSessionNull);
			}
			base.EnableDisposeTracking();
			PropTag[] tagsToRead;
			using (MailboxResourceMonitorEntry mailboxResourceMonitorEntry2 = (MailboxResourceMonitorEntry)((object)((default(T) == null) ? Activator.CreateInstance<T>() : default(T))))
			{
				tagsToRead = mailboxResourceMonitorEntry2.GetPropertyTagsToRead();
			}
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
				entries = CS$<>8__locals1.<>4__this.MapiSession.Administration.GetResourceMonitorDigest(CS$<>8__locals1.databaseId.Guid, tagsToRead);
			}, Strings.ExceptionFindObject(typeof(T).Name, (null == root) ? Strings.ConstantNull : root.ToString()), CS$<>8__locals1.databaseId);
			int resultSize = entries.Length;
			if (0 < maximumResultsSize && maximumResultsSize < resultSize)
			{
				resultSize = maximumResultsSize;
			}
			foreach (PropValue[] entry in entries)
			{
				MailboxResourceMonitorEntry mailboxResourceMonitorEntry = (MailboxResourceMonitorEntry)((object)((default(T) == null) ? Activator.CreateInstance<T>() : default(T)));
				try
				{
					mailboxResourceMonitorEntry.Instantiate(entry);
					mailboxResourceMonitorEntry.MapiSession = base.MapiSession;
					if (mailboxResourceMonitorEntry[MapiPropertyDefinitions.MailboxGuid] != null)
					{
						mailboxResourceMonitorEntry.Identity = new MailboxId(CS$<>8__locals1.databaseId, (Guid)mailboxResourceMonitorEntry[MapiPropertyDefinitions.MailboxGuid]);
					}
					mailboxResourceMonitorEntry.UpdateIdentity(mailboxResourceMonitorEntry.UpdateIdentityFlagsForFinding);
					mailboxResourceMonitorEntry.OriginatingServer = Fqdn.Parse(mailboxResourceMonitorEntry.MapiSession.ServerName);
					mailboxResourceMonitorEntry.ResetChangeTracking(true);
				}
				finally
				{
					mailboxResourceMonitorEntry.Dispose();
				}
				yield return (T)((object)mailboxResourceMonitorEntry);
			}
			yield break;
			yield break;
		}

		private static MapiObjectSchema schema = ObjectSchema.GetInstance<MailboxResourceMonitorEntry.MailboxResourceMonitorEntrySchema>();

		private class MailboxResourceMonitorEntrySchema : MapiObjectSchema
		{
		}
	}
}
