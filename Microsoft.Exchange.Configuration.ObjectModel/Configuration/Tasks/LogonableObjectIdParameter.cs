using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mapi;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class LogonableObjectIdParameter : GeneralMailboxIdParameter
	{
		public LogonableObjectIdParameter(string identity) : base(identity)
		{
		}

		public LogonableObjectIdParameter()
		{
		}

		public LogonableObjectIdParameter(MailboxEntry storeMailboxEntry) : base(storeMailboxEntry)
		{
		}

		public LogonableObjectIdParameter(MailboxId storeMailboxId) : base(storeMailboxId)
		{
		}

		public LogonableObjectIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public LogonableObjectIdParameter(ReducedRecipient reducedRecipient) : base(reducedRecipient)
		{
		}

		public LogonableObjectIdParameter(ADUser user) : base(user)
		{
		}

		public LogonableObjectIdParameter(ADSystemAttendantMailbox systemAttendant) : base(systemAttendant)
		{
		}

		public LogonableObjectIdParameter(Mailbox mailbox) : base(mailbox)
		{
		}

		public LogonableObjectIdParameter(LogonStatisticsEntry logonStatisticsEntry) : this((logonStatisticsEntry == null) ? null : logonStatisticsEntry.Identity)
		{
		}

		public LogonableObjectIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		internal override RecipientType[] RecipientTypes
		{
			get
			{
				return LogonableObjectIdParameter.AllowedRecipientTypes;
			}
		}

		public new static LogonableObjectIdParameter Parse(string identity)
		{
			return new LogonableObjectIdParameter(identity);
		}

		internal override IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			TaskLogger.LogEnter();
			notFoundReason = null;
			IEnumerable<T> result = new List<T>();
			if (typeof(Database).IsAssignableFrom(typeof(T)) && !string.IsNullOrEmpty(base.RawIdentity))
			{
				LegacyDN legacyDN = null;
				if (LegacyDN.TryParse(base.RawIdentity, out legacyDN))
				{
					QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, DatabaseSchema.ExchangeLegacyDN, base.RawIdentity);
					result = base.PerformPrimarySearch<T>(filter, rootId, session, true, optionalData);
				}
			}
			else
			{
				result = base.GetObjects<T>(rootId, session, subTreeSession, optionalData, out notFoundReason);
			}
			TaskLogger.LogExit();
			return result;
		}

		protected override LocalizedString GetErrorMessageForWrongType(string id)
		{
			return Strings.WrongTypeLogonableObjectIdParameter(id);
		}

		internal new static readonly RecipientType[] AllowedRecipientTypes = new RecipientType[]
		{
			RecipientType.UserMailbox,
			RecipientType.SystemAttendantMailbox,
			RecipientType.SystemMailbox,
			RecipientType.PublicDatabase
		};
	}
}
