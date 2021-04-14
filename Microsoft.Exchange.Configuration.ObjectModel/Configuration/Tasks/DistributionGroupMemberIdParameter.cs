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
	public class DistributionGroupMemberIdParameter : DistributionGroupIdParameter
	{
		public DistributionGroupMemberIdParameter(string identity) : base(identity)
		{
		}

		public DistributionGroupMemberIdParameter()
		{
		}

		public DistributionGroupMemberIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public DistributionGroupMemberIdParameter(ReducedRecipient recipient) : base(recipient.Id)
		{
		}

		public DistributionGroupMemberIdParameter(DistributionGroup group) : base(group.Id)
		{
		}

		public DistributionGroupMemberIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		internal override RecipientType[] RecipientTypes
		{
			get
			{
				return GeneralRecipientIdParameter.AllowedRecipientTypes;
			}
		}

		public new static DistributionGroupMemberIdParameter Parse(string identity)
		{
			return new DistributionGroupMemberIdParameter(identity);
		}

		internal override IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			DistributionGroupIdParameter @object = new DistributionGroupIdParameter(base.RawIdentity);
			ReducedRecipient object2 = base.GetObject<ReducedRecipient>(rootId, session, subTreeSession, optionalData, new RecipientIdParameter.GetObjectsDelegate<ReducedRecipient>(@object.GetObjects<ReducedRecipient>), out notFoundReason);
			if (object2 == null)
			{
				return EnumerableWrapper<T>.Empty;
			}
			if (object2.RecipientTypeDetails == RecipientTypeDetails.GroupMailbox || object2.RecipientTypeDetails == RecipientTypeDetails.RemoteGroupMailbox)
			{
				throw new ArgumentException(Strings.WrongTypeMailboxRecipient(object2.Id.ToString()));
			}
			IDirectorySession directorySession = TaskHelper.UnderscopeSessionToOrganization(session, object2.OrganizationId, true);
			IDirectorySession reducedRecipientSession = DirectorySessionFactory.Default.GetReducedRecipientSession((IRecipientSession)directorySession, 155, "GetObjects", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\IdentityParameter\\RecipientParameters\\DistributionGroupMemberIdParameter.cs");
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.MemberOfGroup, object2.Id);
			return base.PerformPrimarySearch<T>(filter, null, reducedRecipientSession, true, optionalData);
		}
	}
}
