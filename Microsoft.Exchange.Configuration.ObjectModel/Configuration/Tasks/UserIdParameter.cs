using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class UserIdParameter : RecipientIdParameter
	{
		public UserIdParameter(string identity) : base(identity)
		{
		}

		public UserIdParameter()
		{
		}

		public UserIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public UserIdParameter(MailUser user) : base(user.Id)
		{
		}

		public UserIdParameter(User user) : base(user.Id)
		{
		}

		public UserIdParameter(Mailbox user) : base(user.Id)
		{
		}

		public UserIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		internal override RecipientType[] RecipientTypes
		{
			get
			{
				return UserIdParameter.AllowedRecipientTypes;
			}
		}

		public new static UserIdParameter Parse(string identity)
		{
			return new UserIdParameter(identity);
		}

		internal override IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			EnumerableWrapper<T> wrapper = EnumerableWrapper<T>.GetWrapper(base.GetObjects<T>(rootId, session, subTreeSession, optionalData, out notFoundReason));
			if (wrapper.HasElements())
			{
				return wrapper;
			}
			SecurityPrincipalIdParameter securityPrincipalIdParameter = new SecurityPrincipalIdParameter(base.RawIdentity);
			OptionalIdentityData optionalIdentityData = (optionalData == null) ? new OptionalIdentityData() : optionalData.Clone();
			optionalIdentityData.AdditionalFilter = QueryFilter.AndTogether(new QueryFilter[]
			{
				optionalIdentityData.AdditionalFilter,
				RecipientIdParameter.GetRecipientTypeFilter(this.RecipientTypes)
			});
			return securityPrincipalIdParameter.GetObjects<T>(rootId, session, subTreeSession, optionalIdentityData, out notFoundReason);
		}

		protected override LocalizedString GetErrorMessageForWrongType(string id)
		{
			return Strings.WrongTypeUser(id);
		}

		internal new static readonly RecipientType[] AllowedRecipientTypes = new RecipientType[]
		{
			RecipientType.User,
			RecipientType.UserMailbox,
			RecipientType.MailUser
		};
	}
}
