using System;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal abstract class DatabaseValidationCheck
	{
		protected static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.MonitoringTracer;
			}
		}

		protected DatabaseValidationCheck(DatabaseValidationCheck.ID checkId)
		{
			this.m_checkId = checkId;
			this.m_checkName = checkId.ToString();
		}

		public DatabaseValidationCheck.ID CheckId
		{
			get
			{
				return this.m_checkId;
			}
		}

		public string CheckName
		{
			get
			{
				return this.m_checkName;
			}
		}

		public DatabaseValidationCheck.Result Validate(DatabaseValidationCheck.Arguments args, ref LocalizedString error)
		{
			DiagCore.RetailAssert(args.Status != null, "args.Status cannot be null!", new object[0]);
			DatabaseValidationCheck.Tracer.TraceDebug((long)this.GetHashCode(), "Check '{0}' is starting against database copy '{1}'. [ActiveServer: {2}, TargetServer: {3}, IsActive: {4}]", new object[]
			{
				this.CheckName,
				args.DatabaseCopyName,
				args.ActiveServer.NetbiosName,
				args.TargetServer.NetbiosName,
				args.Status.IsActive
			});
			if (!this.IsPrerequisiteMetForCheck(args))
			{
				DatabaseValidationCheck.Tracer.TraceError<string, string>((long)this.GetHashCode(), "Check '{0}' is skipping for database copy '{1}' because prereqs were not met. Returning Passed result.", this.CheckName, args.DatabaseCopyName);
				return DatabaseValidationCheck.Result.Passed;
			}
			DatabaseValidationCheck.Result result = this.ValidateInternal(args, ref error);
			if (result == DatabaseValidationCheck.Result.Passed)
			{
				DatabaseValidationCheck.Tracer.TraceDebug<string, string>((long)this.GetHashCode(), "Check '{0}' for database copy '{1}' Passed.", this.CheckName, args.DatabaseCopyName);
			}
			else
			{
				DatabaseValidationCheck.Tracer.TraceError((long)this.GetHashCode(), "Check '{0}' for database copy '{1}' returned result '{2}'. Error: {3}", new object[]
				{
					this.CheckName,
					args.DatabaseCopyName,
					result,
					error
				});
			}
			return result;
		}

		protected abstract bool IsPrerequisiteMetForCheck(DatabaseValidationCheck.Arguments args);

		protected abstract DatabaseValidationCheck.Result ValidateInternal(DatabaseValidationCheck.Arguments args, ref LocalizedString error);

		private readonly string m_checkName;

		private readonly DatabaseValidationCheck.ID m_checkId;

		internal enum Result
		{
			Passed,
			Warning,
			Failed
		}

		internal enum ID
		{
			DatabaseCheckCopyStatusRpcSuccessful,
			DatabaseCheckClusterNodeUp,
			DatabaseCheckServerInMaintenanceMode,
			DatabaseCheckServerHasTooManyActives,
			DatabaseCheckServerAllowedForActivation,
			DatabaseCheckActivationDisfavored,
			DatabaseCheckActiveMountState,
			DatabaseCheckActiveCopyNotActivationSuspended,
			DatabaseCheckPassiveCopyStatusIsOkForAvailability,
			DatabaseCheckPassiveCopyStatusIsOkForRedundancy,
			DatabaseCheckPassiveCopyTotalQueueLength,
			DatabaseCheckPassiveCopyRealCopyQueueLength,
			DatabaseCheckPassiveCopyInspectorQueueLength,
			DatabaseCheckReplayServiceUpOnActiveCopy,
			DatabaseCheckDatabaseIsReplicated,
			DatabaseCheckCopyStatusNotStale,
			DatabaseCheckActiveConnected,
			DatabaseCheckPassiveConnected
		}

		internal class Arguments
		{
			public Arguments(AmServerName targetServer, AmServerName activeServer, IADDatabase database, CopyStatusClientCachedEntry copyStatus, CopyStatusClientCachedEntry activeCopyStatus, ICopyStatusClientLookup statusLookup, IMonitoringADConfig adConfig, PropertyUpdateTracker propertyUpdateTracker = null, bool ignoreActivationDisfavored = true, bool isCopyRemoval = false, bool ignoreMaintenanceChecks = true, bool ignoreTooManyActivesCheck = true)
			{
				this.m_targetServer = targetServer;
				this.m_activeServer = activeServer;
				this.m_database = database;
				this.m_copyStatus = copyStatus;
				this.m_activeCopyStatus = activeCopyStatus;
				this.m_statusLookup = statusLookup;
				this.m_adConfig = adConfig;
				this.m_propertyUpdateTracker = propertyUpdateTracker;
				this.m_ignoreActivationDisfavored = ignoreActivationDisfavored;
				this.m_isCopyRemoval = isCopyRemoval;
				this.m_ignoreMaintenanceChecks = ignoreMaintenanceChecks;
				this.m_ignoreTooManyActivesCheck = ignoreTooManyActivesCheck;
				if (this.m_activeServer == null)
				{
					this.m_activeServer = AmServerName.Empty;
				}
				this.m_dbName = database.Name;
				this.m_dbCopyName = string.Format("{0}\\{1}", database.Name, targetServer.NetbiosName);
			}

			public IMonitoringADConfig ADConfig
			{
				get
				{
					return this.m_adConfig;
				}
			}

			public AmServerName TargetServer
			{
				get
				{
					return this.m_targetServer;
				}
			}

			public AmServerName ActiveServer
			{
				get
				{
					return this.m_activeServer;
				}
			}

			public IADDatabase Database
			{
				get
				{
					return this.m_database;
				}
			}

			public CopyStatusClientCachedEntry Status
			{
				get
				{
					return this.m_copyStatus;
				}
			}

			public CopyStatusClientCachedEntry ActiveStatus
			{
				get
				{
					return this.m_activeCopyStatus;
				}
			}

			public ICopyStatusClientLookup StatusLookup
			{
				get
				{
					return this.m_statusLookup;
				}
			}

			public PropertyUpdateTracker PropertyUpdateTracker
			{
				get
				{
					return this.m_propertyUpdateTracker;
				}
			}

			public bool IgnoreActivationDisfavored
			{
				get
				{
					return this.m_ignoreActivationDisfavored;
				}
			}

			public bool IsCopyRemoval
			{
				get
				{
					return this.m_isCopyRemoval;
				}
			}

			public bool IgnoreMaintenanceChecks
			{
				get
				{
					return this.m_ignoreMaintenanceChecks;
				}
			}

			public bool IgnoreTooManyActivesCheck
			{
				get
				{
					return this.m_ignoreTooManyActivesCheck;
				}
			}

			public string DatabaseName
			{
				get
				{
					return this.m_dbName;
				}
			}

			public string DatabaseCopyName
			{
				get
				{
					return this.m_dbCopyName;
				}
			}

			private readonly IMonitoringADConfig m_adConfig;

			private readonly AmServerName m_targetServer;

			private readonly AmServerName m_activeServer;

			private readonly IADDatabase m_database;

			private readonly CopyStatusClientCachedEntry m_copyStatus;

			private readonly CopyStatusClientCachedEntry m_activeCopyStatus;

			private readonly ICopyStatusClientLookup m_statusLookup;

			private readonly PropertyUpdateTracker m_propertyUpdateTracker;

			private readonly string m_dbName;

			private readonly string m_dbCopyName;

			private readonly bool m_ignoreActivationDisfavored;

			private readonly bool m_isCopyRemoval;

			private readonly bool m_ignoreMaintenanceChecks;

			private readonly bool m_ignoreTooManyActivesCheck;
		}
	}
}
