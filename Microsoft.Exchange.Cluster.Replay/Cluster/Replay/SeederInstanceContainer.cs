using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class SeederInstanceContainer : IIdentityGuid
	{
		public SeederInstanceContainer(RpcSeederArgs rpcArgs, ConfigurationArgs configArgs)
		{
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 66, ".ctor", "f:\\15.00.1497\\sources\\dev\\cluster\\src\\Replay\\seeder\\seederinstancecontainer.cs");
			Database database = null;
			Exception ex = null;
			try
			{
				database = topologyConfigurationSession.FindDatabaseByGuid<Database>(configArgs.IdentityGuid);
			}
			catch (ADTransientException ex2)
			{
				ex = ex2;
			}
			catch (ADExternalException ex3)
			{
				ex = ex3;
			}
			catch (ADOperationException ex4)
			{
				ex = ex4;
			}
			if (ex != null)
			{
				throw new SeedPrepareException(ReplayStrings.CouldNotFindDatabase(configArgs.IdentityGuid.ToString(), ex.ToString()), ex);
			}
			this.m_seederArgs = rpcArgs;
			this.m_configArgs = configArgs;
			this.m_seedDatabase = rpcArgs.SeedDatabase;
			this.m_seedCiFiles = (rpcArgs.SeedCiFiles && !database.IsPublicFolderDatabase);
			if (this.m_seedDatabase)
			{
				if (this.m_seedCiFiles)
				{
					this.m_databaseSeeder = new DatabaseSeederInstance(rpcArgs, configArgs, new SeedCompletionCallback(this.LaunchCiFileSeeder), null);
				}
				else
				{
					this.m_databaseSeeder = new DatabaseSeederInstance(rpcArgs, configArgs, null, null);
				}
			}
			if (this.m_seedCiFiles)
			{
				this.m_ciFilesSeeder = new CiFilesSeederInstance(rpcArgs, configArgs);
			}
		}

		public string Identity
		{
			get
			{
				return SafeInstanceTable<DatabaseSeederInstance>.GetIdentityFromGuid(this.m_seederArgs.InstanceGuid);
			}
		}

		public DateTime CompletedTimeUtc
		{
			get
			{
				DateTime result;
				lock (this)
				{
					DateTime dateTime = DateTime.MaxValue;
					SeederState seederState = SeederState.Unknown;
					if (this.m_databaseSeeder != null)
					{
						seederState = this.m_databaseSeeder.SeedState;
						dateTime = this.m_databaseSeeder.CompletedTimeUtc;
					}
					if (this.m_ciFilesSeeder != null && (seederState == SeederState.SeedSuccessful || seederState == SeederState.Unknown))
					{
						seederState = this.m_ciFilesSeeder.SeedState;
						dateTime = this.m_ciFilesSeeder.CompletedTimeUtc;
					}
					result = dateTime;
				}
				return result;
			}
		}

		public SeederState SeedState
		{
			get
			{
				SeederState result;
				lock (this)
				{
					SeederState seederState = SeederState.Unknown;
					if (this.m_databaseSeeder != null)
					{
						seederState = this.m_databaseSeeder.SeedState;
					}
					if (this.m_ciFilesSeeder != null && (seederState == SeederState.SeedSuccessful || seederState == SeederState.Unknown))
					{
						seederState = this.m_ciFilesSeeder.SeedState;
					}
					result = seederState;
				}
				return result;
			}
			internal set
			{
				if (this.m_databaseSeeder != null)
				{
					this.m_databaseSeeder.SeedState = value;
				}
				if (this.m_ciFilesSeeder != null)
				{
					this.m_ciFilesSeeder.SeedState = value;
				}
			}
		}

		public string Name
		{
			get
			{
				return this.m_configArgs.Name;
			}
		}

		public string SeedingSource
		{
			get
			{
				if (!string.IsNullOrEmpty(this.m_seederArgs.SourceMachineName))
				{
					return this.m_seederArgs.SourceMachineName;
				}
				return this.m_configArgs.SourceMachine;
			}
		}

		public void PrepareDbSeed()
		{
			ExTraceGlobals.SeederServerTracer.TraceDebug((long)this.GetHashCode(), "SeederInstanceContainer.PrepareDbSeed() entered.");
			if (this.m_databaseSeeder != null)
			{
				this.m_databaseSeeder.PrepareDbSeed();
			}
			if (this.m_databaseSeeder == null && this.m_ciFilesSeeder != null)
			{
				this.m_ciFilesSeeder.PrepareCiFileSeeding();
			}
		}

		public void BeginDbSeed()
		{
			ExTraceGlobals.SeederServerTracer.TraceDebug((long)this.GetHashCode(), "SeederInstanceContainer.BeginDbSeed() entered.");
			if (this.m_databaseSeeder != null)
			{
				this.m_databaseSeeder.BeginDbSeed();
			}
			if (this.m_ciFilesSeeder != null && !this.m_seedDatabase)
			{
				this.m_ciFilesSeeder.BeginDbSeed();
			}
		}

		public void CancelDbSeed()
		{
			ExTraceGlobals.SeederServerTracer.TraceDebug((long)this.GetHashCode(), "SeederInstanceContainer.CancelDbSeed() entered.");
			if (this.m_databaseSeeder != null)
			{
				this.m_databaseSeeder.CancelDbSeed();
			}
			if (this.m_ciFilesSeeder != null)
			{
				this.m_ciFilesSeeder.CancelCiFileSeed();
			}
		}

		public RpcSeederStatus GetSeedStatus()
		{
			ExTraceGlobals.SeederServerTracer.TraceDebug((long)this.GetHashCode(), "SeederInstanceContainer.GetSeedStatus() entered.");
			RpcSeederStatus rpcSeederStatus = null;
			bool flag = true;
			bool flag2 = this.m_seedDatabase && this.m_seedCiFiles;
			if (this.m_databaseSeeder != null)
			{
				if (!flag2)
				{
					flag = false;
				}
				RpcSeederStatus seedStatus = this.m_databaseSeeder.GetSeedStatus();
				if (seedStatus.State != SeederState.SeedSuccessful)
				{
					flag = false;
				}
				rpcSeederStatus = SeederInstanceContainer.ScaleDatabaseSeedStatus(seedStatus, flag2);
			}
			if (flag)
			{
				rpcSeederStatus = this.m_ciFilesSeeder.GetSeedStatus();
				rpcSeederStatus = SeederInstanceContainer.ScaleCiSeedStatus(rpcSeederStatus, flag2);
			}
			return rpcSeederStatus;
		}

		public void WaitUntilStopped()
		{
			if (this.m_databaseSeeder != null)
			{
				this.m_databaseSeeder.WaitUntilStopped();
			}
			if (this.m_ciFilesSeeder != null)
			{
				this.m_ciFilesSeeder.WaitUntilStopped();
			}
		}

		public void LaunchCiFileSeeder(bool successfulSeed)
		{
			if (this.m_seedCiFiles && successfulSeed)
			{
				this.m_ciFilesSeeder.PrepareCiFileSeeding();
				this.m_ciFilesSeeder.BeginDbSeed();
			}
		}

		internal void TestSetCompletedTimeUtc(DateTime completedTimeUtc)
		{
			if (this.m_databaseSeeder != null)
			{
				this.m_databaseSeeder.CompletedTimeUtc = completedTimeUtc;
			}
			if (this.m_ciFilesSeeder != null)
			{
				this.m_ciFilesSeeder.CompletedTimeUtc = completedTimeUtc;
			}
		}

		private static RpcSeederStatus ScaleDatabaseSeedStatus(RpcSeederStatus databaseStatus, bool performingTwoSeeds)
		{
			RpcSeederStatus rpcSeederStatus;
			if (!performingTwoSeeds)
			{
				rpcSeederStatus = databaseStatus;
			}
			else
			{
				rpcSeederStatus = new RpcSeederStatus(databaseStatus);
				rpcSeederStatus.BytesTotalDivisor = rpcSeederStatus.BytesTotal * 5L / 4L;
			}
			return rpcSeederStatus;
		}

		private static RpcSeederStatus ScaleCiSeedStatus(RpcSeederStatus ciSeedStatus, bool performingTwoSeeds)
		{
			RpcSeederStatus result = new RpcSeederStatus(ciSeedStatus);
			if (!performingTwoSeeds)
			{
				result = ciSeedStatus;
			}
			return result;
		}

		private readonly RpcSeederArgs m_seederArgs;

		private readonly ConfigurationArgs m_configArgs;

		private readonly bool m_seedDatabase;

		private readonly bool m_seedCiFiles;

		private DatabaseSeederInstance m_databaseSeeder;

		private CiFilesSeederInstance m_ciFilesSeeder;
	}
}
