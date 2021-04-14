using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public class MoveRequestIdParameter : MailboxOrMailUserIdParameter
	{
		public MoveRequestIdParameter()
		{
		}

		public MoveRequestIdParameter(MoveRequestStatistics moveRequest) : this(moveRequest.MailboxIdentity)
		{
		}

		public MoveRequestIdParameter(MoveRequest moveRequest) : this(moveRequest.Id)
		{
		}

		public MoveRequestIdParameter(ADObjectId userId) : base(userId)
		{
		}

		public MoveRequestIdParameter(Mailbox user) : base(user)
		{
		}

		public MoveRequestIdParameter(MailUser user) : base(user)
		{
		}

		public MoveRequestIdParameter(User user) : base(user.Id)
		{
		}

		public MoveRequestIdParameter(Guid guid) : base(guid.ToString())
		{
		}

		public MoveRequestIdParameter(string user) : base(user)
		{
		}

		protected override QueryFilter AdditionalQueryFilter
		{
			get
			{
				QueryFilter queryFilter = new AndFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.NotEqual, ADUserSchema.MailboxMoveStatus, RequestStatus.None),
					new ExistsFilter(ADUserSchema.MailboxMoveStatus)
				});
				if (base.AdditionalQueryFilter != null)
				{
					return new AndFilter(new QueryFilter[]
					{
						queryFilter,
						base.AdditionalQueryFilter
					});
				}
				return queryFilter;
			}
		}

		public new static MoveRequestIdParameter Parse(string identity)
		{
			return new MoveRequestIdParameter(identity);
		}

		internal override IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			IEnumerable<T> objects = base.GetObjects<T>(rootId, session, subTreeSession, optionalData, out notFoundReason);
			if (notFoundReason == null)
			{
				notFoundReason = new LocalizedString?(MrsStrings.ErrorCouldNotFindMoveRequest(this.ToString()));
			}
			return objects;
		}
	}
}
