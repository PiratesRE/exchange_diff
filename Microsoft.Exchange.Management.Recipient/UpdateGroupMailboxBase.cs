using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class UpdateGroupMailboxBase
	{
		protected UpdateGroupMailboxBase(ADUser group, ADUser executingUser, GroupMailboxConfigurationActionType forceActionMask, int? permissionsVersion)
		{
			ArgumentValidator.ThrowIfNull("group", group);
			ArgumentValidator.ThrowIfInvalidValue<ADUser>("group", group, (ADUser adUser) => adUser.RecipientTypeDetails == RecipientTypeDetails.GroupMailbox);
			this.group = group;
			this.executingUser = executingUser;
			this.forceActionMask = forceActionMask;
			this.permissionsVersion = permissionsVersion;
		}

		public string Error { get; protected set; }

		public ResponseCodeType? ResponseCode { get; protected set; }

		public abstract void Execute();

		protected readonly ADUser group;

		protected readonly ADUser executingUser;

		protected readonly GroupMailboxConfigurationActionType forceActionMask;

		protected readonly int? permissionsVersion;
	}
}
