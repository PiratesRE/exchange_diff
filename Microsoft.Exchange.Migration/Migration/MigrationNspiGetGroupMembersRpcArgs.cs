using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Migration.DataAccessLayer;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MigrationNspiGetGroupMembersRpcArgs : MigrationNspiRpcArgs
	{
		public MigrationNspiGetGroupMembersRpcArgs(ExchangeOutlookAnywhereEndpoint endpoint, string groupSmtpAddress) : base(endpoint, MigrationProxyRpcType.GetGroupMembers)
		{
			this.GroupSmtpAddress = groupSmtpAddress;
		}

		public MigrationNspiGetGroupMembersRpcArgs(byte[] requestBlob) : base(requestBlob, MigrationProxyRpcType.GetGroupMembers)
		{
		}

		public string GroupSmtpAddress
		{
			get
			{
				return base.GetProperty<string>(2416508959U);
			}
			set
			{
				base.SetPropertyAsString(2416508959U, value);
			}
		}

		public override bool Validate(out string errorMsg)
		{
			if (!base.Validate(out errorMsg))
			{
				return false;
			}
			if (string.IsNullOrEmpty(this.GroupSmtpAddress))
			{
				errorMsg = "Group Smtp Address cannot be null or empty.";
				return false;
			}
			errorMsg = null;
			return true;
		}
	}
}
