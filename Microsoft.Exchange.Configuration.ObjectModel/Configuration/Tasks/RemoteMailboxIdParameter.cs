using System;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class RemoteMailboxIdParameter : MailUserIdParameterBase
	{
		public RemoteMailboxIdParameter(string identity) : base(identity)
		{
		}

		public RemoteMailboxIdParameter()
		{
		}

		public RemoteMailboxIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public RemoteMailboxIdParameter(RemoteMailbox remoteMailbox) : base(remoteMailbox.Id)
		{
		}

		public RemoteMailboxIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		protected override QueryFilter AdditionalQueryFilter
		{
			get
			{
				return QueryFilter.AndTogether(new QueryFilter[]
				{
					base.AdditionalQueryFilter,
					RemoteMailboxIdParameter.GetRemoteMailboxFilter()
				});
			}
		}

		public new static RemoteMailboxIdParameter Parse(string identity)
		{
			return new RemoteMailboxIdParameter(identity);
		}

		internal static QueryFilter GetRemoteMailboxFilter()
		{
			return RecipientIdParameter.GetRecipientTypeDetailsFilter(new RecipientTypeDetails[]
			{
				(RecipientTypeDetails)((ulong)int.MinValue),
				RecipientTypeDetails.RemoteRoomMailbox,
				RecipientTypeDetails.RemoteEquipmentMailbox,
				RecipientTypeDetails.RemoteSharedMailbox,
				RecipientTypeDetails.RemoteTeamMailbox
			});
		}

		protected override LocalizedString GetErrorMessageForWrongType(string id)
		{
			return Strings.WrongTypeRemoteMailbox(id);
		}
	}
}
