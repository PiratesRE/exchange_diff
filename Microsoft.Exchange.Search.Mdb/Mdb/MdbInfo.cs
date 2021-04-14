using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.Search.Mdb
{
	internal class MdbInfo : IEquatable<MdbInfo>
	{
		internal MdbInfo(MailboxDatabase mailboxDatabase) : this(mailboxDatabase.Guid, mailboxDatabase.Name, mailboxDatabase.Server.Name, Path.GetDirectoryName(mailboxDatabase.EdbFilePath.ToString()), mailboxDatabase.DatabaseCopies.Length)
		{
			this.EnableOwningServerUpdate = true;
		}

		internal MdbInfo(Guid mdbGuid) : this(mdbGuid, string.Empty, string.Empty, string.Empty, 1)
		{
		}

		internal MdbInfo(MdbInfo mdbInfo) : this(mdbInfo.Guid, mdbInfo.Name, mdbInfo.OwningServer, mdbInfo.DatabasePath, mdbInfo.NumberOfCopies)
		{
			this.ActivationPreference = mdbInfo.ActivationPreference;
			this.MountedOnLocalServer = mdbInfo.MountedOnLocalServer;
			this.IsSuspended = mdbInfo.IsSuspended;
			this.NumberOfItems = mdbInfo.NumberOfItems;
			this.IsInstantSearchEnabled = mdbInfo.IsInstantSearchEnabled;
			this.IsRefinersEnabled = mdbInfo.IsRefinersEnabled;
			this.IsCatalogSuspended = mdbInfo.IsCatalogSuspended;
		}

		internal MdbInfo(Guid mdbGuid, string mdbName, string owningServer, string databasePath, int numberOfCopies = 1)
		{
			this.guid = mdbGuid;
			this.name = mdbName;
			this.OwningServer = owningServer;
			this.DatabasePath = databasePath;
			this.NumberOfCopies = numberOfCopies;
			this.cachedToStringValue = string.Format("{0} ({1})", this.Guid, this.Name);
		}

		internal Guid Guid
		{
			[DebuggerStepThrough]
			get
			{
				return this.guid;
			}
		}

		internal string Name
		{
			[DebuggerStepThrough]
			get
			{
				return this.name;
			}
		}

		internal string OwningServer { get; private set; }

		internal string DatabasePath { get; private set; }

		internal int NumberOfCopies { get; private set; }

		internal string IndexSystemName
		{
			get
			{
				return FastIndexVersion.GetIndexSystemName(this.Guid);
			}
		}

		internal virtual Guid? SystemAttendantGuid
		{
			get
			{
				this.InitializeSystemMailboxAndSystemAttendantGuids(false);
				return this.systemAttendantGuid;
			}
		}

		internal virtual Guid SystemMailboxGuid
		{
			get
			{
				this.InitializeSystemMailboxAndSystemAttendantGuids(false);
				return this.systemMailboxGuid;
			}
		}

		internal ADSystemMailbox SystemMailbox
		{
			get
			{
				this.InitializeSystemMailboxAndSystemAttendantGuids(false);
				return this.systemMailbox;
			}
		}

		internal VersionInfo CatalogVersion { get; set; }

		internal bool PreferredActiveCopy { get; set; }

		internal int ActivationPreference { get; set; }

		internal ICollection<MdbCopy> DatabaseCopies { get; set; }

		internal int MaxSupportedVersion { get; set; }

		internal IndexStatusErrorCode NotIndexed { get; set; }

		internal bool EnableOwningServerUpdate { get; set; }

		internal bool MountedOnLocalServer { get; set; }

		internal string DesiredCatalogFolder
		{
			get
			{
				string text = this.DatabasePath;
				if (ExEnvironment.IsTest && !string.IsNullOrEmpty(text))
				{
					text += "_Catalog";
					if (Directory.Exists(this.DatabasePath) && !Directory.Exists(text))
					{
						Directory.CreateDirectory(text);
					}
				}
				if (string.IsNullOrEmpty(text))
				{
					return text;
				}
				return Path.GetFullPath(text);
			}
		}

		internal bool IsSuspended { get; set; }

		internal bool ShouldAutomaticallySuspendCatalog { get; set; }

		internal bool IsLagCopy { get; set; }

		internal long NumberOfItems { get; set; }

		internal DateTime NumberOfItemsTimeStamp { get; set; }

		internal bool IsInstantSearchEnabled { get; set; }

		internal bool IsRefinersEnabled { get; set; }

		internal bool IsCatalogSuspended { get; set; }

		public override string ToString()
		{
			return this.cachedToStringValue;
		}

		public override int GetHashCode()
		{
			return this.Guid.GetHashCode();
		}

		public bool Equals(MdbInfo other)
		{
			return this.Guid.Equals(other.Guid);
		}

		public void ResetSystemMailboxGuidCache()
		{
			this.InitializeSystemMailboxAndSystemAttendantGuids(true);
		}

		public void UpdateDatabaseLocationInfo()
		{
			if (!this.EnableOwningServerUpdate)
			{
				return;
			}
			LocalizedString message = Strings.FailedToGetActiveServer(this.Guid);
			try
			{
				ActiveManager noncachingActiveManagerInstance = ActiveManager.GetNoncachingActiveManagerInstance();
				DatabaseLocationInfo serverForDatabase = noncachingActiveManagerInstance.GetServerForDatabase(this.Guid, GetServerForDatabaseFlags.IgnoreAdSiteBoundary | GetServerForDatabaseFlags.BasicQuery);
				if (serverForDatabase.RequestResult != DatabaseLocationInfoResult.Success)
				{
					throw new ComponentFailedTransientException(message);
				}
				this.OwningServer = serverForDatabase.ServerFqdn;
				this.MountedOnLocalServer = (serverForDatabase.ServerGuid == LocalServer.GetServer().Guid);
			}
			catch (ServerForDatabaseNotFoundException innerException)
			{
				throw new ComponentFailedTransientException(message, innerException);
			}
			catch (StorageTransientException innerException2)
			{
				throw new ComponentFailedTransientException(message, innerException2);
			}
			catch (ADTransientException innerException3)
			{
				throw new ComponentFailedTransientException(message, innerException3);
			}
			catch (StoragePermanentException innerException4)
			{
				throw new ComponentFailedPermanentException(message, innerException4);
			}
			catch (ADExternalException innerException5)
			{
				throw new ComponentFailedPermanentException(message, innerException5);
			}
			catch (ADOperationException innerException6)
			{
				throw new ComponentFailedPermanentException(message, innerException6);
			}
		}

		private void InitializeSystemMailboxAndSystemAttendantGuids(bool forceInitialize)
		{
			if (!this.guidInitializationAttempted || forceInitialize)
			{
				this.guidInitializationAttempted = true;
				ADNotificationAdapter.RunADOperation(delegate()
				{
					string legacyExchangeDN = LocalServer.GetServer().ExchangeLegacyDN + "/cn=Microsoft System Attendant";
					IRootOrganizationRecipientSession rootOrganizationRecipientSession = DirectorySessionFactory.Default.CreateRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 509, "InitializeSystemMailboxAndSystemAttendantGuids", "f:\\15.00.1497\\sources\\dev\\Search\\src\\Mdb\\MdbInfo.cs");
					rootOrganizationRecipientSession.ServerTimeout = new TimeSpan?(MdbInfo.ADTimeout);
					try
					{
						ADRecipient adrecipient = rootOrganizationRecipientSession.FindByLegacyExchangeDN(legacyExchangeDN);
						ADSystemAttendantMailbox adsystemAttendantMailbox = adrecipient as ADSystemAttendantMailbox;
						if (adsystemAttendantMailbox != null && adsystemAttendantMailbox.Database != null && adsystemAttendantMailbox.Database.ObjectGuid == this.Guid)
						{
							this.systemAttendantGuid = new Guid?((adsystemAttendantMailbox.ExchangeGuid == Guid.Empty) ? adsystemAttendantMailbox.Guid : adsystemAttendantMailbox.ExchangeGuid);
						}
					}
					catch (DataValidationException)
					{
					}
					ADRecipient[] array = rootOrganizationRecipientSession.Find(null, QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, "SystemMailbox{" + this.Guid + "}"), null, 2);
					if (array.Length != 1)
					{
						throw new ComponentFailedPermanentException(Strings.FailedToFindSystemMailbox(this.Guid));
					}
					this.systemMailbox = (array[0] as ADSystemMailbox);
					if (this.systemMailbox != null)
					{
						this.systemMailboxGuid = this.systemMailbox.ExchangeGuid;
						return;
					}
					throw new ComponentFailedPermanentException(Strings.FailedToFindSystemMailbox(this.Guid));
				});
			}
		}

		private const string SystemMailboxNamePrefix = "SystemMailbox";

		private const string SystemAttendantLegacyDNSuffix = "/cn=Microsoft System Attendant";

		private static readonly TimeSpan ADTimeout = TimeSpan.FromSeconds(30.0);

		private readonly Guid guid;

		private readonly string name;

		private readonly string cachedToStringValue;

		private Guid? systemAttendantGuid;

		private Guid systemMailboxGuid;

		private ADSystemMailbox systemMailbox;

		private bool guidInitializationAttempted;
	}
}
