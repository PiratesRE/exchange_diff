using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Set", "PublicFolderMailboxMigrationRequest", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetPublicFolderMailboxMigrationRequest : SetRequest<PublicFolderMailboxMigrationRequestIdParameter>
	{
		[Parameter(Mandatory = false, ParameterSetName = "MailboxMigrationOutlookAnywherePublicFolder")]
		[ValidateNotNullOrEmpty]
		public string RemoteMailboxLegacyDN
		{
			get
			{
				return (string)base.Fields["RemoteMailboxLegacyDN"];
			}
			set
			{
				base.Fields["RemoteMailboxLegacyDN"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ParameterSetName = "MailboxMigrationOutlookAnywherePublicFolder")]
		public string RemoteMailboxServerLegacyDN
		{
			get
			{
				return (string)base.Fields["RemoteMailboxServerLegacyDN"];
			}
			set
			{
				base.Fields["RemoteMailboxServerLegacyDN"] = value;
			}
		}

		[ValidateNotNull]
		[Parameter(Mandatory = false, ParameterSetName = "MailboxMigrationOutlookAnywherePublicFolder")]
		public Fqdn OutlookAnywhereHostName
		{
			get
			{
				return (Fqdn)base.Fields["OutlookAnywhereHostName"];
			}
			set
			{
				base.Fields["OutlookAnywhereHostName"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MailboxMigrationOutlookAnywherePublicFolder")]
		public AuthenticationMethod AuthenticationMethod
		{
			get
			{
				return (AuthenticationMethod)(base.Fields["AuthenticationMethod"] ?? AuthenticationMethod.Basic);
			}
			set
			{
				base.Fields["AuthenticationMethod"] = value;
			}
		}

		[ValidateNotNull]
		[Parameter(Mandatory = false, ParameterSetName = "MailboxMigrationLocalPublicFolder")]
		public DatabaseIdParameter SourceDatabase
		{
			get
			{
				return (DatabaseIdParameter)base.Fields["SourceDatabase"];
			}
			set
			{
				base.Fields["SourceDatabase"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MailboxMigrationOutlookAnywherePublicFolder")]
		[ValidateNotNull]
		public new PSCredential RemoteCredential
		{
			get
			{
				return base.RemoteCredential;
			}
			set
			{
				base.RemoteCredential = value;
			}
		}

		[Parameter(Mandatory = false)]
		public new Unlimited<int> BadItemLimit
		{
			get
			{
				return base.BadItemLimit;
			}
			set
			{
				base.BadItemLimit = value;
			}
		}

		[Parameter(Mandatory = false)]
		public new Unlimited<int> LargeItemLimit
		{
			get
			{
				return base.LargeItemLimit;
			}
			set
			{
				base.LargeItemLimit = value;
			}
		}

		[Parameter(Mandatory = false)]
		public new string BatchName
		{
			get
			{
				return base.BatchName;
			}
			set
			{
				base.BatchName = value;
			}
		}

		[Parameter(Mandatory = false)]
		public new SwitchParameter AcceptLargeDataLoss
		{
			get
			{
				return base.AcceptLargeDataLoss;
			}
			set
			{
				base.AcceptLargeDataLoss = value;
			}
		}

		protected override void ValidateRequest(TransactionalRequestJob requestJob)
		{
			base.ValidateRequest(requestJob);
			if (base.ParameterSetName.Equals("MailboxMigrationLocalPublicFolder") && requestJob.Flags.HasFlag(RequestFlags.CrossOrg))
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorInvalidParametersForOutlookAnywherePublicFolderMailboxMigration), ExchangeErrorCategory.Client, null);
				return;
			}
			if (base.ParameterSetName.Equals("MailboxMigrationOutlookAnywherePublicFolder") && requestJob.Flags.HasFlag(RequestFlags.IntraOrg))
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorInvalidParametersForLocalPublicFolderMailboxMigration), ExchangeErrorCategory.Client, null);
			}
		}

		protected override void ModifyRequest(TransactionalRequestJob requestJob)
		{
			base.ModifyRequest(requestJob);
			if (base.IsFieldSet("SourceDatabase"))
			{
				PublicFolderDatabase publicFolderDatabase = (PublicFolderDatabase)base.GetDataObject<PublicFolderDatabase>(this.SourceDatabase, base.ConfigSession, null, new LocalizedString?(Strings.ErrorDatabaseNotFound(this.SourceDatabase.ToString())), new LocalizedString?(Strings.ErrorDatabaseNotUnique(this.SourceDatabase.ToString())));
				DatabaseInformation databaseInformation = MapiUtils.FindServerForMdb(publicFolderDatabase.Id.ObjectGuid, null, null, FindServerFlags.None);
				if (!this.IsSupportedDatabaseVersion(databaseInformation.ServerVersion))
				{
					base.WriteError(new DatabaseVersionUnsupportedPermanentException(publicFolderDatabase.Identity.ToString(), databaseInformation.ServerFqdn, new ServerVersion(databaseInformation.ServerVersion).ToString()), ErrorCategory.InvalidArgument, null);
				}
				requestJob.SourceDatabase = publicFolderDatabase.Id;
			}
			if (base.IsFieldSet("RemoteMailboxLegacyDN"))
			{
				requestJob.RemoteMailboxLegacyDN = this.RemoteMailboxLegacyDN;
			}
			if (base.IsFieldSet("RemoteMailboxServerLegacyDN"))
			{
				requestJob.RemoteMailboxServerLegacyDN = this.RemoteMailboxServerLegacyDN;
			}
			if (base.IsFieldSet("OutlookAnywhereHostName"))
			{
				requestJob.OutlookAnywhereHostName = this.OutlookAnywhereHostName;
			}
			if (base.IsFieldSet("AuthenticationMethod"))
			{
				requestJob.AuthenticationMethod = new AuthenticationMethod?(this.AuthenticationMethod);
			}
			if (base.IsFieldSet("RemoteCredential"))
			{
				requestJob.RemoteCredential = RequestTaskHelper.GetNetworkCredential(this.RemoteCredential, requestJob.AuthenticationMethod);
			}
		}

		protected override void CheckIndexEntry()
		{
		}

		private bool IsSupportedDatabaseVersion(int serverVersion)
		{
			return serverVersion >= Server.E15MinVersion || (serverVersion >= Server.E14MinVersion && serverVersion < Server.E15MinVersion) || (serverVersion >= Server.E2007SP2MinVersion && serverVersion < Server.E14MinVersion);
		}

		private const string TaskNoun = "PublicFolderMailboxMigrationRequest";

		private const string ParameterSetOutlookAnywherePublicFolderMailboxMigration = "MailboxMigrationOutlookAnywherePublicFolder";

		private const string ParameterRemoteMailboxLegacyDN = "RemoteMailboxLegacyDN";

		private const string ParameterRemoteMailboxServerLegacyDN = "RemoteMailboxServerLegacyDN";

		private const string ParameterOutlookAnywhereHostName = "OutlookAnywhereHostName";

		private const string ParameterAuthenticationMethod = "AuthenticationMethod";

		private const string ParameterSetLocalPublicFolderMailboxMigration = "MailboxMigrationLocalPublicFolder";

		private const string ParameterSourceDatabase = "SourceDatabase";
	}
}
