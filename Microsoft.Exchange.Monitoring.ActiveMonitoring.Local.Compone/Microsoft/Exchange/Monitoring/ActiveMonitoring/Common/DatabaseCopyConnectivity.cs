using System;
using System.Diagnostics;
using System.Globalization;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Rpc.Cluster;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	public class DatabaseCopyConnectivity
	{
		public DatabaseCopyConnectivity(Guid databaseGuid)
		{
			if (databaseGuid == Guid.Empty)
			{
				throw new ArgumentException("Guid of the database to be validated cannot be null or empty", "DatabaseGuid");
			}
			this.databaseGuid = databaseGuid;
		}

		public Guid DatabaseGuid
		{
			get
			{
				return this.databaseGuid;
			}
		}

		public bool? IsActive
		{
			get
			{
				return this.isActive;
			}
		}

		public LocalizedException Exception
		{
			get
			{
				return this.exception;
			}
		}

		public TimeSpan Latency
		{
			get
			{
				return this.latency;
			}
		}

		internal string DiagnosticContext
		{
			get
			{
				return this.diagnosticContext;
			}
		}

		public DatabaseCopyConnectivity.DatabaseAvailable Validate(Guid systemMailboxGuid, bool activeDatabase)
		{
			if (systemMailboxGuid == Guid.Empty)
			{
				throw new ArgumentException("Guid of the system mailbox to use for database availability validation cannot be null or empty", "systemMailboxGuid");
			}
			if (DirectoryAccessor.Instance == null)
			{
				throw new ArgumentException("DirectoryAccessor instance cannot be null", "DirectoryAccessor.Instance");
			}
			this.isActive = new bool?(DirectoryAccessor.Instance.IsDatabaseCopyActiveOnLocalServer(this.DatabaseGuid));
			if (this.isActive == null)
			{
				throw new UnableToGetDatabaseStateException(this.databaseGuid.ToString());
			}
			if (activeDatabase != this.isActive.Value)
			{
				return DatabaseCopyConnectivity.DatabaseAvailable.NoOp;
			}
			DatabaseLocationInfo databaseLocationInfo = new DatabaseLocationInfo(LocalServer.GetServer(), false);
			ExchangePrincipal exPrincipal = ExchangePrincipal.FromMailboxData(systemMailboxGuid, this.DatabaseGuid, null, new CultureInfo[]
			{
				CultureInfo.CurrentCulture
			}, RemotingOptions.LocalConnectionsOnly, databaseLocationInfo);
			if (!this.isActive.Value && !this.IsDatabaseHealthyHAPerspective())
			{
				return DatabaseCopyConnectivity.DatabaseAvailable.PassiveUnhealthyHA;
			}
			Stopwatch stopwatch = Stopwatch.StartNew();
			DatabaseCopyConnectivity.DatabaseAvailable databaseAvailable = this.ValidateDatabaseCopy(exPrincipal);
			stopwatch.Stop();
			this.latency = stopwatch.Elapsed;
			if (!this.isActive.Value && databaseAvailable == DatabaseCopyConnectivity.DatabaseAvailable.Failure && !this.IsDatabaseAttachedReadOnly())
			{
				return DatabaseCopyConnectivity.DatabaseAvailable.PassiveDatabaseNotAttached;
			}
			if (this.exception != null)
			{
				this.diagnosticContext = this.GetDiagnosticContext(this.exception);
			}
			return databaseAvailable;
		}

		private DatabaseCopyConnectivity.DatabaseAvailable ValidateDatabaseCopy(ExchangePrincipal exPrincipal)
		{
			try
			{
				using (MailboxSession mailboxSession = MailboxSession.OpenAsAdmin(exPrincipal, CultureInfo.InvariantCulture, "Client=StoreActiveMonitoring", true, false, !this.isActive.Value))
				{
					using (Folder.Bind(mailboxSession, DefaultFolderType.Inbox, new PropertyDefinition[]
					{
						FolderSchema.ItemCount
					}))
					{
					}
				}
			}
			catch (MailboxOfflineException ex)
			{
				this.exception = ex;
				return DatabaseCopyConnectivity.DatabaseAvailable.Failure;
			}
			catch (MailboxInSiteFailoverException ex2)
			{
				this.exception = ex2;
				return DatabaseCopyConnectivity.DatabaseAvailable.Failure;
			}
			catch (StorageTransientException ex3)
			{
				this.exception = ex3;
				return DatabaseCopyConnectivity.DatabaseAvailable.Failure;
			}
			catch (StoragePermanentException ex4)
			{
				this.exception = ex4;
				return DatabaseCopyConnectivity.DatabaseAvailable.Failure;
			}
			return DatabaseCopyConnectivity.DatabaseAvailable.Success;
		}

		private string GetDiagnosticContext(Exception exception)
		{
			if (exception == null)
			{
				throw new ArgumentNullException("exception");
			}
			string result = string.Empty;
			Exception innerException = exception.InnerException;
			MapiPermanentException ex = innerException as MapiPermanentException;
			MapiRetryableException ex2 = innerException as MapiRetryableException;
			if (ex != null)
			{
				result = ex.DiagCtx.ToCompactString();
			}
			else if (ex2 != null)
			{
				result = ex2.DiagCtx.ToCompactString();
			}
			return result;
		}

		private bool IsDatabaseHealthyHAPerspective()
		{
			Exception ex;
			CopyStatusClientCachedEntry[] copyStatus = CopyStatusHelper.GetCopyStatus(AmServerName.LocalComputerName, RpcGetDatabaseCopyStatusFlags2.None, new Guid[]
			{
				this.databaseGuid
			}, 5000, null, out ex);
			if (ex != null)
			{
				this.exception = (LocalizedException)ex;
				return false;
			}
			if (copyStatus == null || copyStatus.Length != 1 || copyStatus[0].CopyStatus.CopyStatus != CopyStatusEnum.Healthy)
			{
				this.exception = new HAPassiveCopyUnhealthyException(copyStatus[0].CopyStatus.CopyStatus.ToString());
				return false;
			}
			return true;
		}

		private bool IsDatabaseAttachedReadOnly()
		{
			using (ExRpcAdmin exRpcAdmin = ExRpcAdmin.Create("Client=StoreActiveMonitoring", Environment.MachineName, null, null, null))
			{
				MdbStatus[] array = exRpcAdmin.ListMdbStatus(new Guid[]
				{
					this.databaseGuid
				});
				if (array != null && array.Length > 0 && (array[0].Status & MdbStatusFlags.AttachedReadOnly) != MdbStatusFlags.AttachedReadOnly)
				{
					this.exception = new DatabaseNotAttachedReadOnlyException();
					return false;
				}
			}
			return true;
		}

		private readonly Guid databaseGuid;

		private bool? isActive = null;

		private LocalizedException exception;

		private TimeSpan latency = TimeSpan.Zero;

		private string diagnosticContext;

		public enum DatabaseAvailable
		{
			Success,
			Failure,
			PassiveUnhealthyHA,
			PassiveDatabaseNotAttached,
			NoOp
		}
	}
}
