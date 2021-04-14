using System;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DatabaseSettingsContext : SimpleDatabaseSettingsContext, IConstraintProvider
	{
		public DatabaseSettingsContext(Guid mdbGuid, SettingsContextBase nextContext = null) : base(mdbGuid, nextContext)
		{
			DatabaseLocationInfo databaseLocationInfo = null;
			try
			{
				databaseLocationInfo = ActiveManager.GetCachingActiveManagerInstance().GetServerForDatabase(mdbGuid, GetServerForDatabaseFlags.IgnoreAdSiteBoundary | GetServerForDatabaseFlags.BasicQuery);
			}
			catch (ObjectNotFoundException)
			{
			}
			if (databaseLocationInfo != null)
			{
				this.databaseName = databaseLocationInfo.DatabaseName;
				this.databaseVersion = databaseLocationInfo.AdminDisplayVersion;
				this.serverName = databaseLocationInfo.ServerFqdn;
				this.serverVersion = new ServerVersion(databaseLocationInfo.ServerVersion);
				this.serverGuid = new Guid?(databaseLocationInfo.ServerGuid);
				return;
			}
			this.databaseName = string.Empty;
			this.databaseVersion = new ServerVersion(0);
			this.serverName = string.Empty;
			this.serverVersion = new ServerVersion(0);
			this.serverGuid = new Guid?(Guid.Empty);
		}

		public static IConstraintProvider Get(Guid mdbGuid)
		{
			if (DatabaseSettingsContext.TestProvider != null)
			{
				return DatabaseSettingsContext.TestProvider;
			}
			return new DatabaseSettingsContext(mdbGuid, null);
		}

		public override string DatabaseName
		{
			get
			{
				return this.databaseName;
			}
		}

		public override ServerVersion DatabaseVersion
		{
			get
			{
				return this.databaseVersion;
			}
		}

		public override string ServerName
		{
			get
			{
				return this.serverName;
			}
		}

		public override ServerVersion ServerVersion
		{
			get
			{
				return this.serverVersion;
			}
		}

		public override Guid? ServerGuid
		{
			get
			{
				return this.serverGuid;
			}
		}

		internal static IConstraintProvider TestProvider { get; set; }

		ConstraintCollection IConstraintProvider.Constraints
		{
			get
			{
				ConstraintCollection constraintCollection = ConstraintCollection.CreateGlobal();
				if (!string.IsNullOrEmpty(this.DatabaseName))
				{
					constraintCollection.Add(VariantType.MdbName, this.DatabaseName);
				}
				if (this.DatabaseGuid != null)
				{
					constraintCollection.Add(VariantType.MdbGuid, this.DatabaseGuid.Value);
				}
				if (this.DatabaseVersion != null)
				{
					constraintCollection.Add(VariantType.MdbVersion, this.DatabaseVersion);
				}
				return constraintCollection;
			}
		}

		string IConstraintProvider.RampId
		{
			get
			{
				return ((this.DatabaseGuid != null) ? this.DatabaseGuid.Value : Guid.Empty).ToString();
			}
		}

		string IConstraintProvider.RotationId
		{
			get
			{
				return ((this.DatabaseGuid != null) ? this.DatabaseGuid.Value : Guid.Empty).ToString();
			}
		}

		private readonly string databaseName;

		private readonly ServerVersion databaseVersion;

		private readonly string serverName;

		private readonly Guid? serverGuid;

		private readonly ServerVersion serverVersion;
	}
}
