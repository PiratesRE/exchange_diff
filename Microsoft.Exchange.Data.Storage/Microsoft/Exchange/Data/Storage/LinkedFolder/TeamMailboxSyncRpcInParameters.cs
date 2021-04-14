using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class TeamMailboxSyncRpcInParameters : RpcParameters
	{
		public Guid MailboxGuid { get; private set; }

		public OrganizationId OrgId { get; private set; }

		public string ClientString { get; private set; }

		public SyncOption SyncOption { get; private set; }

		public string DomainController { get; private set; }

		public TeamMailboxSyncRpcInParameters(byte[] data) : base(data)
		{
			this.MailboxGuid = (Guid)base.GetParameterValue("MailboxGuid");
			this.OrgId = (OrganizationId)base.GetParameterValue("OrgId");
			this.ClientString = (string)base.GetParameterValue("ClientString");
			this.SyncOption = (SyncOption)base.GetParameterValue("SyncOption");
			this.DomainController = (string)base.GetParameterValue("DomainController");
		}

		public TeamMailboxSyncRpcInParameters(Guid mailboxGuid, OrganizationId orgId, string clientString, SyncOption syncOption, string domainController)
		{
			this.MailboxGuid = mailboxGuid;
			this.OrgId = orgId;
			this.ClientString = clientString;
			this.SyncOption = syncOption;
			this.DomainController = domainController;
			base.SetParameterValue("MailboxGuid", this.MailboxGuid);
			base.SetParameterValue("OrgId", this.OrgId);
			base.SetParameterValue("ClientString", this.ClientString);
			base.SetParameterValue("SyncOption", this.SyncOption);
			base.SetParameterValue("DomainController", this.DomainController);
		}

		private const string MailboxGuidParameterName = "MailboxGuid";

		private const string OrgIdParameterName = "OrgId";

		private const string ClientStringParameterName = "ClientString";

		private const string SyncOptionParameterName = "SyncOption";

		private const string DomainControllerParameterName = "DomainController";
	}
}
