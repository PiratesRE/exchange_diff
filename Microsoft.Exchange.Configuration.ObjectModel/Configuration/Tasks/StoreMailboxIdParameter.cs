using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Mapi;
using Microsoft.Exchange.Data.Mapi.Common;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class StoreMailboxIdParameter : MapiIdParameter
	{
		public StoreMailboxIdParameter()
		{
		}

		public StoreMailboxIdParameter(MailboxId mailboxId) : base(mailboxId)
		{
		}

		public StoreMailboxIdParameter(Guid mailboxGuid) : this(new MailboxId(null, mailboxGuid))
		{
		}

		public StoreMailboxIdParameter(MailboxEntry mailbox) : this(mailbox.Identity)
		{
		}

		public StoreMailboxIdParameter(INamedIdentity namedIdentity) : this(namedIdentity.Identity)
		{
			this.rawIdentity = namedIdentity.DisplayName;
		}

		private StoreMailboxIdParameter(string identity)
		{
			this.rawIdentity = identity;
			if (string.IsNullOrEmpty(identity))
			{
				throw new ArgumentNullException("identity");
			}
			MailboxId mailboxId = null;
			try
			{
				mailboxId = new MailboxId(null, new Guid(identity));
			}
			catch (FormatException)
			{
			}
			catch (ArgumentOutOfRangeException)
			{
			}
			if (null == mailboxId && CultureInfo.InvariantCulture.CompareInfo.IsPrefix(identity, "/o=", CompareOptions.IgnoreCase))
			{
				mailboxId = new MailboxId(identity);
			}
			if (null != mailboxId)
			{
				this.Initialize(mailboxId);
			}
			this.displayName = identity;
		}

		internal ulong Flags
		{
			get
			{
				return this.flags;
			}
			set
			{
				this.flags = value;
			}
		}

		internal override string RawIdentity
		{
			get
			{
				return this.rawIdentity ?? base.RawIdentity;
			}
		}

		public static StoreMailboxIdParameter Parse(string identity)
		{
			return new StoreMailboxIdParameter(identity);
		}

		public override string ToString()
		{
			MailboxId mailboxId = base.MapiObjectId as MailboxId;
			if (null != mailboxId)
			{
				return mailboxId.ToString();
			}
			if (!string.IsNullOrEmpty(this.displayName))
			{
				return this.displayName;
			}
			return string.Empty;
		}

		internal override void Initialize(ObjectId objectId)
		{
			if (objectId == null)
			{
				throw new ArgumentNullException("objectId");
			}
			MailboxId mailboxId = objectId as MailboxId;
			if (null == mailboxId)
			{
				throw new ArgumentException(Strings.ErrorInvalidParameterType("objectId", typeof(MailboxId).Name), "objectId");
			}
			if (!string.IsNullOrEmpty(this.displayName))
			{
				throw new InvalidOperationException(Strings.ErrorChangeImmutableType);
			}
			if (Guid.Empty == mailboxId.MailboxGuid && string.IsNullOrEmpty(mailboxId.MailboxExchangeLegacyDn))
			{
				throw new ArgumentException(Strings.ErrorInvalidParameterFormat("objectId"), "objectId");
			}
			base.Initialize(objectId);
		}

		internal override IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			if (!typeof(MailboxEntry).IsAssignableFrom(typeof(T)))
			{
				throw new ArgumentException(Strings.ErrorInvalidType(typeof(T).Name), "type");
			}
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (rootId == null)
			{
				throw new ArgumentNullException("rootId");
			}
			if (!(session is MapiAdministrationSession))
			{
				throw new ArgumentException(Strings.ErrorInvalidType(typeof(MapiAdministrationSession).Name), "session");
			}
			if (string.IsNullOrEmpty(this.displayName) && null == base.MapiObjectId)
			{
				throw new InvalidOperationException(Strings.ErrorOperationOnInvalidObject);
			}
			notFoundReason = null;
			List<T> list = new List<T>();
			Guid mailboxGuid = Guid.Empty;
			if (base.MapiObjectId != null)
			{
				mailboxGuid = ((MailboxId)base.MapiObjectId).MailboxGuid;
			}
			QueryFilter filter = new MailboxContextFilter(mailboxGuid, this.flags);
			try
			{
				IEnumerable<T> enumerable = session.FindPaged<T>(filter, rootId, true, null, 0);
				if (null != base.MapiObjectId)
				{
					bool flag = Guid.Empty != ((MailboxId)base.MapiObjectId).MailboxGuid;
					bool flag2 = !string.IsNullOrEmpty(((MailboxId)base.MapiObjectId).MailboxExchangeLegacyDn);
					if (flag || flag2)
					{
						foreach (T t in enumerable)
						{
							IConfigurable configurable = t;
							MailboxEntry mailboxEntry = (MailboxEntry)configurable;
							if (flag && mailboxEntry.Identity.MailboxGuid == ((MailboxId)base.MapiObjectId).MailboxGuid)
							{
								list.Add((T)((object)mailboxEntry));
							}
							else if (flag2 && string.Equals(mailboxEntry.Identity.MailboxExchangeLegacyDn, ((MailboxId)base.MapiObjectId).MailboxExchangeLegacyDn, StringComparison.OrdinalIgnoreCase))
							{
								list.Add((T)((object)mailboxEntry));
							}
						}
					}
				}
				if (list.Count == 0 && typeof(MailboxStatistics).IsAssignableFrom(typeof(T)) && !string.IsNullOrEmpty(this.displayName))
				{
					foreach (T t2 in enumerable)
					{
						IConfigurable configurable2 = t2;
						MailboxStatistics mailboxStatistics = (MailboxStatistics)configurable2;
						if (string.Equals(this.displayName, mailboxStatistics.DisplayName, StringComparison.OrdinalIgnoreCase))
						{
							list.Add((T)((object)mailboxStatistics));
						}
					}
				}
			}
			catch (MapiObjectNotFoundException)
			{
			}
			return list;
		}

		private string displayName;

		private string rawIdentity;

		private ulong flags;
	}
}
