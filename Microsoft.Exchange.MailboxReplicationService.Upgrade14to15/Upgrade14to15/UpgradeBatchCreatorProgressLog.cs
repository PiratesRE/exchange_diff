using System;
using System.IO;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Win32;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	internal class UpgradeBatchCreatorProgressLog : ObjectLog<TenantUpgradeData>
	{
		public UpgradeBatchCreatorProgressLog() : base(new UpgradeBatchCreatorProgressLog.UpgradeBatchCreatorProgressLogSchema(), new UpgradeBatchCreatorProgressLog.UpgradeBatchCreatorProgressLogConfiguration())
		{
		}

		public static void Write(TenantOrganizationPresentationObjectWrapper tenant, string errorType, string errorDetails, int? upgradeE14MbxCountForCurrentStage = null, int? upgradeE14RequestCountForCurrentStage = null)
		{
			TenantUpgradeData objectToLog = default(TenantUpgradeData);
			objectToLog.Tenant = tenant;
			objectToLog.PilotUser = null;
			objectToLog.ErrorType = errorType;
			objectToLog.ErrorDetails = errorDetails;
			objectToLog.Tenant.UpgradeE14MbxCountForCurrentStage = upgradeE14MbxCountForCurrentStage;
			objectToLog.Tenant.UpgradeE14RequestCountForCurrentStage = upgradeE14RequestCountForCurrentStage;
			UpgradeBatchCreatorProgressLog.instance.LogObject(objectToLog);
		}

		public static void FlushLog()
		{
			UpgradeBatchCreatorProgressLog.instance.Flush();
		}

		private static UpgradeBatchCreatorProgressLog instance = new UpgradeBatchCreatorProgressLog();

		private class UpgradeBatchCreatorProgressLogSchema : ObjectLogSchema
		{
			public override string Software
			{
				get
				{
					return "Microsoft Exchange Upgrade Batch Creator";
				}
			}

			public override string LogType
			{
				get
				{
					return "Batch Creator Log";
				}
			}

			public static readonly ObjectLogSimplePropertyDefinition<TenantUpgradeData> TenantId = new ObjectLogSimplePropertyDefinition<TenantUpgradeData>("TenantId", (TenantUpgradeData d) => d.Tenant.ExternalDirectoryOrganizationId);

			public static readonly ObjectLogSimplePropertyDefinition<TenantUpgradeData> TenantName = new ObjectLogSimplePropertyDefinition<TenantUpgradeData>("TenantName", (TenantUpgradeData d) => d.Tenant.Name);

			public static readonly ObjectLogSimplePropertyDefinition<TenantUpgradeData> TenantVersion = new ObjectLogSimplePropertyDefinition<TenantUpgradeData>("TenantVersion", (TenantUpgradeData d) => d.Tenant.AdminDisplayVersion.ToString());

			public static readonly ObjectLogSimplePropertyDefinition<TenantUpgradeData> UserId = new ObjectLogSimplePropertyDefinition<TenantUpgradeData>("UserId", (TenantUpgradeData d) => string.Empty);

			public static readonly ObjectLogSimplePropertyDefinition<TenantUpgradeData> UpgradeRequest = new ObjectLogSimplePropertyDefinition<TenantUpgradeData>("UpgradeRequest", (TenantUpgradeData d) => d.Tenant.UpgradeRequest.ToString());

			public static readonly ObjectLogSimplePropertyDefinition<TenantUpgradeData> UpgradeStatus = new ObjectLogSimplePropertyDefinition<TenantUpgradeData>("UpgradeStatus", (TenantUpgradeData d) => d.Tenant.UpgradeStatus.ToString());

			public static readonly ObjectLogSimplePropertyDefinition<TenantUpgradeData> UpgradeStage = new ObjectLogSimplePropertyDefinition<TenantUpgradeData>("UpgradeStage", delegate(TenantUpgradeData d)
			{
				if (d.Tenant.UpgradeStage == null)
				{
					return string.Empty;
				}
				return d.Tenant.UpgradeStage.Value.ToString();
			});

			public static readonly ObjectLogSimplePropertyDefinition<TenantUpgradeData> IsUpgradingOrganization = new ObjectLogSimplePropertyDefinition<TenantUpgradeData>("IsUpgradingOrganization", (TenantUpgradeData d) => d.Tenant.IsUpgradingOrganization.ToString());

			public static readonly ObjectLogSimplePropertyDefinition<TenantUpgradeData> IsUpgradeOperationInProgress = new ObjectLogSimplePropertyDefinition<TenantUpgradeData>("IsUpgradeOperationInProgress", (TenantUpgradeData d) => d.Tenant.IsUpgradeOperationInProgress.ToString());

			public static readonly ObjectLogSimplePropertyDefinition<TenantUpgradeData> IsPilotingOrganization = new ObjectLogSimplePropertyDefinition<TenantUpgradeData>("IsPilotingOrganization", (TenantUpgradeData d) => d.Tenant.IsPilotingOrganization.ToString());

			public static readonly ObjectLogSimplePropertyDefinition<TenantUpgradeData> UpgradeE14SysMbxCount = new ObjectLogSimplePropertyDefinition<TenantUpgradeData>("UpgradeE14SysMbxCount", delegate(TenantUpgradeData d)
			{
				if (d.Tenant.UpgradeStage == null || d.Tenant.UpgradeStage.Value != Microsoft.Exchange.Data.Directory.SystemConfiguration.UpgradeStage.MoveArbitration)
				{
					return string.Empty;
				}
				return Convert.ToString(d.Tenant.UpgradeE14MbxCountForCurrentStage);
			});

			public static readonly ObjectLogSimplePropertyDefinition<TenantUpgradeData> UpgradeE14UserMbxCount = new ObjectLogSimplePropertyDefinition<TenantUpgradeData>("UpgradeE14UserMbxCount", delegate(TenantUpgradeData d)
			{
				if (d.Tenant.UpgradeStage == null || d.Tenant.UpgradeStage.Value != Microsoft.Exchange.Data.Directory.SystemConfiguration.UpgradeStage.MoveRegularUser)
				{
					return string.Empty;
				}
				return Convert.ToString(d.Tenant.UpgradeE14MbxCountForCurrentStage);
			});

			public static readonly ObjectLogSimplePropertyDefinition<TenantUpgradeData> UpgradeE14CloudOnlyMbxCount = new ObjectLogSimplePropertyDefinition<TenantUpgradeData>("UpgradeE14CloudOnlyMbxCount", delegate(TenantUpgradeData d)
			{
				if (d.Tenant.UpgradeStage == null || d.Tenant.UpgradeStage.Value != Microsoft.Exchange.Data.Directory.SystemConfiguration.UpgradeStage.MoveCloudOnlyArchive)
				{
					return string.Empty;
				}
				return Convert.ToString(d.Tenant.UpgradeE14MbxCountForCurrentStage);
			});

			public static readonly ObjectLogSimplePropertyDefinition<TenantUpgradeData> UpgradeE14PilotMbxCount = new ObjectLogSimplePropertyDefinition<TenantUpgradeData>("UpgradeE14PilotMbxCount", delegate(TenantUpgradeData d)
			{
				if (d.Tenant.UpgradeStage == null || d.Tenant.UpgradeStage.Value != Microsoft.Exchange.Data.Directory.SystemConfiguration.UpgradeStage.MoveRegularPilot)
				{
					return string.Empty;
				}
				return Convert.ToString(d.Tenant.UpgradeE14MbxCountForCurrentStage);
			});

			public static readonly ObjectLogSimplePropertyDefinition<TenantUpgradeData> UpgradeE14PilotCloudOnlyMbxCount = new ObjectLogSimplePropertyDefinition<TenantUpgradeData>("UpgradeE14CloudOnlyArchivePilotMbxCount", delegate(TenantUpgradeData d)
			{
				if (d.Tenant.UpgradeStage == null || d.Tenant.UpgradeStage.Value != Microsoft.Exchange.Data.Directory.SystemConfiguration.UpgradeStage.MoveCloudOnlyArchivePilot)
				{
					return string.Empty;
				}
				return Convert.ToString(d.Tenant.UpgradeE14MbxCountForCurrentStage);
			});

			public static readonly ObjectLogSimplePropertyDefinition<TenantUpgradeData> UpgradeE14SysMoveCount = new ObjectLogSimplePropertyDefinition<TenantUpgradeData>("UpgradeE14SysMoveCount", delegate(TenantUpgradeData d)
			{
				if (d.Tenant.UpgradeStage == null || d.Tenant.UpgradeStage.Value != Microsoft.Exchange.Data.Directory.SystemConfiguration.UpgradeStage.MoveArbitration)
				{
					return string.Empty;
				}
				return Convert.ToString(d.Tenant.UpgradeE14RequestCountForCurrentStage);
			});

			public static readonly ObjectLogSimplePropertyDefinition<TenantUpgradeData> UpgradeE14UserMoveCount = new ObjectLogSimplePropertyDefinition<TenantUpgradeData>("UpgradeE14UserMoveCount", delegate(TenantUpgradeData d)
			{
				if (d.Tenant.UpgradeStage == null || d.Tenant.UpgradeStage.Value != Microsoft.Exchange.Data.Directory.SystemConfiguration.UpgradeStage.MoveRegularUser)
				{
					return string.Empty;
				}
				return Convert.ToString(d.Tenant.UpgradeE14RequestCountForCurrentStage);
			});

			public static readonly ObjectLogSimplePropertyDefinition<TenantUpgradeData> UpgradeE14CloudOnlyMoveCount = new ObjectLogSimplePropertyDefinition<TenantUpgradeData>("UpgradeE14CloudOnlyArchiveMoveCount", delegate(TenantUpgradeData d)
			{
				if (d.Tenant.UpgradeStage == null || d.Tenant.UpgradeStage.Value != Microsoft.Exchange.Data.Directory.SystemConfiguration.UpgradeStage.MoveCloudOnlyArchive)
				{
					return string.Empty;
				}
				return Convert.ToString(d.Tenant.UpgradeE14RequestCountForCurrentStage);
			});

			public static readonly ObjectLogSimplePropertyDefinition<TenantUpgradeData> UpgradeE14PilotMoveCount = new ObjectLogSimplePropertyDefinition<TenantUpgradeData>("UpgradeE14PilotMoveCount", delegate(TenantUpgradeData d)
			{
				if (d.Tenant.UpgradeStage == null || d.Tenant.UpgradeStage.Value != Microsoft.Exchange.Data.Directory.SystemConfiguration.UpgradeStage.MoveRegularPilot)
				{
					return string.Empty;
				}
				return Convert.ToString(d.Tenant.UpgradeE14RequestCountForCurrentStage);
			});

			public static readonly ObjectLogSimplePropertyDefinition<TenantUpgradeData> UpgradeE14PilotCloudOnlyMoveCount = new ObjectLogSimplePropertyDefinition<TenantUpgradeData>("UpgradeE14CloudOnlyArchivePilotMoveCount", delegate(TenantUpgradeData d)
			{
				if (d.Tenant.UpgradeStage == null || d.Tenant.UpgradeStage.Value != Microsoft.Exchange.Data.Directory.SystemConfiguration.UpgradeStage.MoveCloudOnlyArchivePilot)
				{
					return string.Empty;
				}
				return Convert.ToString(d.Tenant.UpgradeE14RequestCountForCurrentStage);
			});

			public static readonly ObjectLogSimplePropertyDefinition<TenantUpgradeData> Errortype = new ObjectLogSimplePropertyDefinition<TenantUpgradeData>("Errortype", (TenantUpgradeData d) => d.ErrorType);

			public static readonly ObjectLogSimplePropertyDefinition<TenantUpgradeData> ErrorDetails = new ObjectLogSimplePropertyDefinition<TenantUpgradeData>("ErrorDetails", (TenantUpgradeData d) => d.ErrorDetails);
		}

		private class UpgradeBatchCreatorProgressLogConfiguration : ObjectLogConfiguration
		{
			public override bool IsEnabled
			{
				get
				{
					return true;
				}
			}

			public override TimeSpan MaxLogAge
			{
				get
				{
					return TimeSpan.FromDays(7.0);
				}
			}

			public override string LoggingFolder
			{
				get
				{
					string name = "SOFTWARE\\Microsoft\\ExchangeServer\\V15\\Setup";
					using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(name))
					{
						if (registryKey != null)
						{
							string path = registryKey.GetValue("MsiInstallPath").ToString();
							return Path.Combine(path, "Logging\\UpgradeBatchCreatorLogs");
						}
					}
					return null;
				}
			}

			public override string LogComponentName
			{
				get
				{
					return "UpgradeBatchCreator";
				}
			}

			public override string FilenamePrefix
			{
				get
				{
					return "UpgradeBatchCreatorProgress";
				}
			}

			public override long MaxLogDirSize
			{
				get
				{
					return 50000000L;
				}
			}

			public override long MaxLogFileSize
			{
				get
				{
					return 500000L;
				}
			}
		}
	}
}
