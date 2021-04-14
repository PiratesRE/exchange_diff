using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public abstract class GetDatabaseTask<TDataObject> : GetSystemConfigurationObjectTask<DatabaseIdParameter, TDataObject> where TDataObject : Database, new()
	{
		[Parameter(Mandatory = true, ParameterSetName = "Server", ValueFromPipeline = true)]
		public ServerIdParameter Server
		{
			get
			{
				return (ServerIdParameter)base.Fields["Server"];
			}
			set
			{
				base.Fields["Server"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public virtual SwitchParameter IncludePreExchange2013
		{
			get
			{
				return (SwitchParameter)(base.Fields["IncludePreExchange2013"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["IncludePreExchange2013"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Status
		{
			get
			{
				return (SwitchParameter)(base.Fields["Status"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Status"] = value;
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				return this.rootId;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			this.rootId = null;
			this.AllowLegacy = this.IncludePreExchange2013;
			if ("StorageGroup" == base.ParameterSetName || "Server" == base.ParameterSetName || (this.Identity != null && (this.Identity.RawIdentity == null || (!this.Identity.RawIdentity.StartsWith("*") && !this.Identity.RawIdentity.EndsWith("*")))))
			{
				this.AllowLegacy = true;
				if (this.Identity != null)
				{
					this.Identity.AllowLegacy = this.AllowLegacy;
				}
			}
			if (this.Server != null)
			{
				this.m_ServerObject = (Server)base.GetDataObject<Server>(this.Server, base.DataSession, null, new LocalizedString?(Strings.ErrorServerNotFound(this.Server.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(this.Server.ToString())));
				if (!this.m_ServerObject.IsMailboxServer && (this.m_ServerObject.IsExchange2007OrLater || (!this.m_ServerObject.IsExchange2007OrLater && !this.AllowLegacy)))
				{
					base.WriteError(this.m_ServerObject.GetServerRoleError(ServerRole.Mailbox), ErrorCategory.InvalidOperation, this.Server);
				}
				this.rootId = (ADObjectId)this.m_ServerObject.Identity;
			}
			if (this.Identity != null)
			{
				this.Identity.AllowInvalid = true;
			}
			base.InternalValidate();
			TaskLogger.LogExit();
		}

		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (this.RootId != null)
			{
				try
				{
					base.WriteVerbose(TaskVerboseStringHelper.GetFindDataObjectsVerboseString(base.DataSession, typeof(TDataObject), this.InternalFilter, this.RootId, this.DeepSearch));
					LocalizedString? localizedString;
					IEnumerable<TDataObject> objects = new DatabaseIdParameter
					{
						AllowInvalid = true
					}.GetObjects<TDataObject>(this.RootId, base.DataSession, base.OptionalIdentityData, out localizedString);
					this.WriteResult<TDataObject>(objects);
					goto IL_7E;
				}
				catch (DataSourceTransientException exception)
				{
					base.WriteError(exception, ErrorCategory.ReadError, null);
					goto IL_7E;
				}
			}
			base.InternalProcessRecord();
			IL_7E:
			TaskLogger.LogExit();
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			TaskLogger.LogEnter(new object[]
			{
				dataObject.Identity,
				dataObject
			});
			try
			{
				Database database = (Database)dataObject;
				if (database.IsExchange2009OrLater || (this.AllowLegacy && (ServerIdParameter.HasRole(((Database)dataObject).Server, ServerRole.Mailbox, base.DataSession) || !ServerIdParameter.HasRole(((Database)dataObject).Server, ServerRole.All, base.DataSession))))
				{
					string text;
					if (this.serverLegacyDNToFqdnCache.TryGetValue(database.RpcClientAccessServerLegacyDN, out text))
					{
						database.RpcClientAccessServer = text;
					}
					database.CompleteAllCalculatedProperties();
					if (text == null)
					{
						this.serverLegacyDNToFqdnCache.Add(database.RpcClientAccessServerLegacyDN, database.RpcClientAccessServer);
					}
					base.WriteResult(dataObject);
				}
			}
			catch (InvalidOperationException)
			{
				base.WriteError(new InvalidADObjectOperationException(Strings.ErrorFoundInvalidADObject(((ADObjectId)dataObject.Identity).ToDNString())), ErrorCategory.InvalidOperation, this.Identity);
			}
			TaskLogger.LogExit();
		}

		protected override void WriteResult<T>(IEnumerable<T> dataObjects)
		{
			TaskLogger.LogEnter(new object[]
			{
				dataObjects
			});
			if (!this.Status)
			{
				using (IEnumerator<T> enumerator = dataObjects.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						T t = enumerator.Current;
						IConfigurable configurable = t;
						Database database = (Database)configurable;
						bool flag = false;
						Server server = database.GetServer();
						if (server == null)
						{
							base.WriteError(new InvalidOperationException(Strings.ErrorDBOwningServerNotFound(database.Identity.ToString())), ErrorCategory.InvalidOperation, database.Identity);
						}
						else
						{
							flag = server.IsE15OrLater;
						}
						if (this.AllowLegacy || flag)
						{
							this.WriteResult(database);
						}
					}
					goto IL_66D;
				}
			}
			Dictionary<string, Collection<Database>> dictionary = new Dictionary<string, Collection<Database>>();
			foreach (T t2 in dataObjects)
			{
				IConfigurable configurable2 = t2;
				Database database2 = (Database)configurable2;
				string text = null;
				bool flag2 = false;
				Server server2 = database2.GetServer();
				if (server2 == null)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorDBOwningServerNotFound(database2.Identity.ToString())), ErrorCategory.InvalidOperation, database2.Identity);
				}
				else
				{
					flag2 = server2.IsE15OrLater;
				}
				bool flag3;
				if (flag2)
				{
					ActiveManager activeManager = null;
					Exception ex = null;
					try
					{
						activeManager = ActiveManager.CreateCustomActiveManager(false, null, null, null, null, null, null, (ITopologyConfigurationSession)this.ConfigurationSession, false);
						DatabaseLocationInfo serverForDatabase = activeManager.GetServerForDatabase(database2.Guid, GetServerForDatabaseFlags.IgnoreAdSiteBoundary);
						text = serverForDatabase.ServerFqdn;
					}
					catch (DatabaseNotFoundException ex2)
					{
						ex = ex2;
					}
					catch (ObjectNotFoundException ex3)
					{
						ex = ex3;
					}
					catch (ServerForDatabaseNotFoundException ex4)
					{
						ex = ex4;
					}
					finally
					{
						if (activeManager != null)
						{
							activeManager.Dispose();
							activeManager = null;
						}
					}
					if (ex != null)
					{
						TaskLogger.Trace("GetDatabase.WriteResult() raised exception from AM.GetServerForDatabase() for database '{0}': {1}", new object[]
						{
							database2.Name,
							ex.ToString()
						});
						this.WriteResult(database2);
						this.WriteWarning(Strings.ErrorFailedToQueryActiveServerForDatabase(database2.Name, ex.Message));
						continue;
					}
					database2.MountedOnServer = text;
					flag3 = true;
				}
				else
				{
					text = server2.Fqdn;
					database2.MountedOnServer = text;
					flag3 = this.AllowLegacy;
				}
				if (flag3)
				{
					Collection<Database> collection;
					if (!dictionary.TryGetValue(text, out collection))
					{
						collection = new Collection<Database>();
						dictionary.Add(text, collection);
					}
					collection.Add(database2);
				}
			}
			foreach (string text2 in dictionary.Keys)
			{
				Collection<Database> collection2 = dictionary[text2];
				try
				{
					base.WriteVerbose(Strings.VerboseConnectionAdminRpcInterface(text2));
					using (ExRpcAdmin exRpcAdmin = ExRpcAdmin.Create("Client=Management", text2, null, null, null))
					{
						Guid[] array = new Guid[collection2.Count];
						for (int i = 0; i < collection2.Count; i++)
						{
							array[i] = collection2[i].Guid;
							base.WriteVerbose(Strings.VerboseCheckDatabaseStatus(array[i].ToString()));
						}
						MdbStatus[] array2 = exRpcAdmin.ListMdbStatus(array);
						for (int j = 0; j < collection2.Count; j++)
						{
							collection2[j].Mounted = new bool?((array2[j].Status & MdbStatusFlags.Online) != MdbStatusFlags.Offline);
							collection2[j].OnlineMaintenanceInProgress = new bool?((array2[j].Status & MdbStatusFlags.Isinteg) == MdbStatusFlags.Offline);
							collection2[j].BackupInProgress = new bool?((array2[j].Status & MdbStatusFlags.Backup) != MdbStatusFlags.Offline);
							if (collection2[j].Mounted.Value)
							{
								this.FillLastBackupTimes(exRpcAdmin, collection2[j]);
								if (collection2[j].LastFullBackup != null)
								{
									collection2[j].LastFullBackup = new DateTime?(collection2[j].LastFullBackup.Value.ToLocalTime());
								}
								if (collection2[j].LastIncrementalBackup != null)
								{
									collection2[j].LastIncrementalBackup = new DateTime?(collection2[j].LastIncrementalBackup.Value.ToLocalTime());
								}
								if (collection2[j].LastDifferentialBackup != null)
								{
									collection2[j].LastDifferentialBackup = new DateTime?(collection2[j].LastDifferentialBackup.Value.ToLocalTime());
								}
								if (collection2[j].LastCopyBackup != null)
								{
									collection2[j].LastCopyBackup = new DateTime?(collection2[j].LastCopyBackup.Value.ToLocalTime());
								}
								this.FillDatabaseSize(exRpcAdmin, collection2[j]);
								if (collection2[j].GetServer().IsE15OrLater)
								{
									this.FillDatabaseProcessInfo(exRpcAdmin, collection2[j]);
								}
							}
							else
							{
								TaskLogger.Trace("Database {0} is not mounted, not getting the last backup times of it", new object[]
								{
									collection2[j].Name
								});
							}
							this.WriteResult(collection2[j]);
						}
					}
				}
				catch (MapiPermanentException ex5)
				{
					TaskLogger.Trace(string.Format(CultureInfo.InvariantCulture, "GetDatabase.WriteResult() raises exception while connecting to server '{0}': {1}", new object[]
					{
						text2,
						ex5.Message
					}), new object[0]);
					foreach (Database dataObject in collection2)
					{
						this.WriteResult(dataObject);
						this.WriteWarning(Strings.ErrorFailedToConnectToStore(text2));
					}
				}
				catch (MapiRetryableException ex6)
				{
					TaskLogger.Trace(string.Format(CultureInfo.InvariantCulture, "GetDatabase.WriteResult() raises exception while connecting to server '{0}': {1}", new object[]
					{
						text2,
						ex6.Message
					}), new object[0]);
					foreach (Database dataObject2 in collection2)
					{
						this.WriteResult(dataObject2);
						this.WriteWarning(Strings.ErrorFailedToConnectToStore(text2));
					}
				}
			}
			IL_66D:
			TaskLogger.LogExit();
		}

		private void FillLastBackupTimes(ExRpcAdmin rpcAdmin, Database database)
		{
			DateTime value = default(DateTime);
			DateTime value2 = default(DateTime);
			DateTime value3 = default(DateTime);
			DateTime value4 = default(DateTime);
			bool? snapshotLastFullBackup = null;
			bool? snapshotLastIncrementalBackup = null;
			bool? snapshotLastDifferentialBackup = null;
			bool? snapshotLastCopyBackup = null;
			try
			{
				FaultInjectionUtils.FaultInjectionTracer.TraceTest(2321952061U);
				rpcAdmin.GetLastBackupInfo(database.Guid, out value, out value2, out value3, out value4, out snapshotLastFullBackup, out snapshotLastIncrementalBackup, out snapshotLastDifferentialBackup, out snapshotLastCopyBackup);
				if (value.ToFileTimeUtc() != 0L)
				{
					database.LastFullBackup = new DateTime?(value);
				}
				if (value2.ToFileTimeUtc() != 0L)
				{
					database.LastIncrementalBackup = new DateTime?(value2);
				}
				if (value3.ToFileTimeUtc() != 0L)
				{
					database.LastDifferentialBackup = new DateTime?(value3);
				}
				if (value4.ToFileTimeUtc() != 0L)
				{
					database.LastCopyBackup = new DateTime?(value4);
				}
				database.SnapshotLastFullBackup = snapshotLastFullBackup;
				database.SnapshotLastIncrementalBackup = snapshotLastIncrementalBackup;
				database.SnapshotLastDifferentialBackup = snapshotLastDifferentialBackup;
				database.SnapshotLastCopyBackup = snapshotLastCopyBackup;
			}
			catch (MapiPermanentException ex)
			{
				TaskLogger.Trace(string.Format(CultureInfo.InvariantCulture, "GetDatabase.FillLastBackupTimes(Database '{0}') raises exception: {1}", new object[]
				{
					database.Identity.ToString(),
					ex.Message
				}), new object[0]);
				this.WriteWarning(Strings.ErrorFailedToGetDatabaseLastBackupTimes(database.Identity.ToString()));
			}
			catch (MapiRetryableException ex2)
			{
				TaskLogger.Trace(string.Format(CultureInfo.InvariantCulture, "GetDatabase.FillLastBackupTimes(Database '{0}') raises exception: {1}", new object[]
				{
					database.Identity.ToString(),
					ex2.Message
				}), new object[0]);
				this.WriteWarning(Strings.ErrorFailedToGetDatabaseLastBackupTimes(database.Identity.ToString()));
			}
		}

		private void FillDatabaseSize(ExRpcAdmin rpcAdmin, Database database)
		{
			try
			{
				FaultInjectionUtils.FaultInjectionTracer.TraceTest(3395693885U);
				ulong bytesValue;
				ulong bytesValue2;
				rpcAdmin.GetDatabaseSize(database.Guid, out bytesValue, out bytesValue2);
				database.DatabaseSize = new ByteQuantifiedSize?(ByteQuantifiedSize.FromBytes(bytesValue));
				database.AvailableNewMailboxSpace = new ByteQuantifiedSize?(ByteQuantifiedSize.FromBytes(bytesValue2));
			}
			catch (MapiExceptionNoSupport)
			{
			}
			catch (MapiPermanentException ex)
			{
				TaskLogger.Trace(string.Format(CultureInfo.InvariantCulture, "GetDatabase.FillDatabaseSize(Database '{0}') raises exception: {1}", new object[]
				{
					database.Identity.ToString(),
					ex.Message
				}), new object[0]);
				this.WriteWarning(Strings.ErrorFailedToGetDatabaseSize(database.Identity.ToString()));
			}
			catch (MapiRetryableException ex2)
			{
				TaskLogger.Trace(string.Format(CultureInfo.InvariantCulture, "GetDatabase.FillDatabaseSize(Database '{0}') raises exception: {1}", new object[]
				{
					database.Identity.ToString(),
					ex2.Message
				}), new object[0]);
				this.WriteWarning(Strings.ErrorFailedToGetDatabaseSize(database.Identity.ToString()));
			}
		}

		private void FillDatabaseProcessInfo(ExRpcAdmin rpcAdmin, Database database)
		{
			try
			{
				PropValue[][] databaseProcessInfo = rpcAdmin.GetDatabaseProcessInfo(database.Guid, new PropTag[]
				{
					PropTag.WorkerProcessId,
					PropTag.MailboxDatabaseVersion,
					PropTag.RequestedDatabaseSchemaVersion
				});
				if (databaseProcessInfo.Length > 0)
				{
					PropValue[] array = databaseProcessInfo[0];
					foreach (PropValue propValue in array)
					{
						if (propValue.PropTag == PropTag.WorkerProcessId && propValue.Value is int)
						{
							database.WorkerProcessId = new int?((int)propValue.Value);
						}
						if (propValue.PropTag == PropTag.MailboxDatabaseVersion && propValue.Value is int)
						{
							int num = (int)propValue.Value;
							database.CurrentSchemaVersion = string.Format("{0}.{1}", num >> 16, num & 65535);
						}
						if (propValue.PropTag == PropTag.RequestedDatabaseSchemaVersion && propValue.Value is int)
						{
							int num2 = (int)propValue.Value;
							database.RequestedSchemaVersion = string.Format("{0}.{1}", num2 >> 16, num2 & 65535);
						}
					}
				}
			}
			catch (MapiPermanentException ex)
			{
				TaskLogger.Trace(string.Format(CultureInfo.InvariantCulture, "GetDatabase.FillDatabaseProcessInfo(Database '{0}') raises exception: {1}", new object[]
				{
					database.Identity.ToString(),
					ex.Message
				}), new object[0]);
				this.WriteWarning(Strings.ErrorFailedToGetDatabaseProcessInfo(database.Identity.ToString()));
			}
			catch (MapiRetryableException ex2)
			{
				TaskLogger.Trace(string.Format(CultureInfo.InvariantCulture, "GetDatabase.FillDatabaseProcessInfo(Database '{0}') raises exception: {1}", new object[]
				{
					database.Identity.ToString(),
					ex2.Message
				}), new object[0]);
				this.WriteWarning(Strings.ErrorFailedToGetDatabaseProcessInfo(database.Identity.ToString()));
			}
		}

		protected Server m_ServerObject;

		protected bool AllowLegacy;

		private readonly Dictionary<string, string> serverLegacyDNToFqdnCache = new Dictionary<string, string>();

		private ADObjectId rootId;
	}
}
