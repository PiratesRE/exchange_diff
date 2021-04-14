using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class MailboxCalendarFolders : DataSourceService, IMailboxCalendarFolder, IEditObjectService<MailboxCalendarFolderRow, SetMailboxCalendarFolder>, IGetObjectService<MailboxCalendarFolderRow>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MailboxCalendarFolder?Identity@R:Self")]
		public PowerShellResults<MailboxCalendarFolderRow> GetObject(Identity identity)
		{
			identity.FaultIfNull();
			return base.GetObject<MailboxCalendarFolderRow>("Get-MailboxCalendarFolder", (Identity)identity.ToMailboxFolderIdParameter());
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MailboxCalendarFolder?Identity@R:Self+Set-MailboxCalendarFolder?Identity@W:Self")]
		public PowerShellResults<MailboxCalendarFolderRow> SetObject(Identity identity, SetMailboxCalendarFolder properties)
		{
			identity.FaultIfNull();
			return base.SetObject<MailboxCalendarFolderRow, SetMailboxCalendarFolder>("Set-MailboxCalendarFolder", (Identity)identity.ToMailboxFolderIdParameter(), properties);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MailboxCalendarFolder?Identity@R:Self+Set-MailboxCalendarFolder?Identity@W:Self")]
		public PowerShellResults<MailboxCalendarFolderRow> StartPublishing(Identity identity, SetMailboxCalendarFolder properties)
		{
			properties.FaultIfNull();
			properties.PublishEnabled = true;
			return this.SetObject(identity, properties);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MailboxCalendarFolder?Identity@R:Self+Set-MailboxCalendarFolder?Identity@W:Self")]
		public PowerShellResults<MailboxCalendarFolderRow> StopPublishing(Identity identity, SetMailboxCalendarFolder properties)
		{
			properties.FaultIfNull();
			properties.PublishEnabled = false;
			return this.SetObject(identity, properties);
		}

		internal const string GetCmdlet = "Get-MailboxCalendarFolder";

		internal const string SetCmdlet = "Set-MailboxCalendarFolder";

		internal const string ReadScope = "@R:Self";

		internal const string WriteScope = "@W:Self";

		private const string GetObjectRole = "Get-MailboxCalendarFolder?Identity@R:Self";

		private const string SetObjectRole = "Get-MailboxCalendarFolder?Identity@R:Self+Set-MailboxCalendarFolder?Identity@W:Self";
	}
}
