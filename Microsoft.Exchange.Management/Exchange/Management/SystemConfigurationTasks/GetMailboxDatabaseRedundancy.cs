using System;
using System.Management.Automation;
using Microsoft.Exchange.Cluster.Replay.Monitoring;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "MailboxDatabaseRedundancy", DefaultParameterSetName = "Identity")]
	[ClassAccessLevel(AccessLevel.Consumer)]
	[OutputType(new Type[]
	{
		typeof(DatabaseRedundancy)
	})]
	public sealed class GetMailboxDatabaseRedundancy : GetRedundancyTaskBase<DatabaseIdParameter, Database>
	{
		[Parameter(Mandatory = false, ParameterSetName = "Identity", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		[ValidateNotNullOrEmpty]
		[Alias(new string[]
		{
			"Database"
		})]
		public override DatabaseIdParameter Identity
		{
			get
			{
				return base.Identity;
			}
			set
			{
				base.Identity = value;
			}
		}

		protected override ADObjectId LookupIdentityObjectAndGetDagId()
		{
			Database database = this.LookupDatabase(this.Identity);
			if (database.MasterType != MasterType.DatabaseAvailabilityGroup)
			{
				base.WriteError(new DatabaseMustBeInDagException(database.Name), ErrorCategory.InvalidOperation, this.Identity);
				return null;
			}
			ADObjectId masterServerOrAvailabilityGroup = database.MasterServerOrAvailabilityGroup;
			if (masterServerOrAvailabilityGroup == null)
			{
				base.WriteError(new DatabaseMustBeInDagException(database.Name), ErrorCategory.InvalidOperation, this.Identity);
				return null;
			}
			return masterServerOrAvailabilityGroup;
		}

		protected override void WriteResultsFromHealthInfo(HealthInfoPersisted hip, string serverContactedFqdn)
		{
			bool flag = false;
			foreach (DbHealthInfoPersisted dbHealthInfoPersisted in hip.Databases)
			{
				if (this.Identity == null || dbHealthInfoPersisted.DbName.Equals(this.Identity.RawIdentity, StringComparison.InvariantCultureIgnoreCase))
				{
					flag = true;
					DatabaseRedundancy dataObject = new DatabaseRedundancy(hip, dbHealthInfoPersisted, serverContactedFqdn);
					this.WriteResult(dataObject);
				}
			}
			if (!flag)
			{
				this.WriteWarning(Strings.GetDagHealthInfoNoResultsWarning);
			}
		}

		private Database LookupDatabase(DatabaseIdParameter dbParam)
		{
			ADObjectId id = new DatabasesContainer().Id;
			Database database = (Database)base.GetDataObject<Database>(dbParam, base.GlobalConfigSession, id, new LocalizedString?(Strings.ErrorDatabaseNotFound(dbParam.ToString())), new LocalizedString?(Strings.ErrorDatabaseNotUnique(dbParam.ToString())));
			if (!database.IsExchange2009OrLater)
			{
				base.WriteError(new ErrorDatabaseWrongVersion(database.Name), ErrorCategory.InvalidOperation, dbParam);
				return null;
			}
			return database;
		}
	}
}
