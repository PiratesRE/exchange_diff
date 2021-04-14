using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Win32;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	internal class UpgradeHandlerSyncLog : DisposableObjectLog<TenantUpgradeData>
	{
		public UpgradeHandlerSyncLog(string logFilePrefix) : base(new UpgradeHandlerSyncLog.UpgradeHandlerSyncLogSchema(), new UpgradeHandlerSyncLog.UpgradeHandlerSyncLogConfiguration(logFilePrefix))
		{
		}

		public void Write(TenantOrganizationPresentationObjectWrapper tenant, RecipientWrapper user, string errorType, string errorDetails)
		{
			base.LogObject(new TenantUpgradeData
			{
				Tenant = tenant,
				PilotUser = user,
				ErrorType = errorType,
				ErrorDetails = errorDetails
			});
		}

		private class UpgradeHandlerSyncLogSchema : ObjectLogSchema
		{
			public override string Software
			{
				get
				{
					return "Microsoft Exchange Upgrade Hanlder";
				}
			}

			public override string LogType
			{
				get
				{
					return "Handler Log";
				}
			}

			public static readonly ObjectLogSimplePropertyDefinition<TenantUpgradeData> TenantId = new ObjectLogSimplePropertyDefinition<TenantUpgradeData>("TenantId", delegate(TenantUpgradeData d)
			{
				if (d.Tenant == null)
				{
					return string.Empty;
				}
				return d.Tenant.ExternalDirectoryOrganizationId;
			});

			public static readonly ObjectLogSimplePropertyDefinition<TenantUpgradeData> TenantName = new ObjectLogSimplePropertyDefinition<TenantUpgradeData>("TenantName", delegate(TenantUpgradeData d)
			{
				if (d.Tenant == null)
				{
					return string.Empty;
				}
				return d.Tenant.Name;
			});

			public static readonly ObjectLogSimplePropertyDefinition<TenantUpgradeData> UserId = new ObjectLogSimplePropertyDefinition<TenantUpgradeData>("UserId", delegate(TenantUpgradeData d)
			{
				if (d.PilotUser == null)
				{
					return string.Empty;
				}
				return d.PilotUser.Id.ObjectGuid.ToString();
			});

			public static readonly ObjectLogSimplePropertyDefinition<TenantUpgradeData> UpgradeRequest = new ObjectLogSimplePropertyDefinition<TenantUpgradeData>("UpgradeRequest", delegate(TenantUpgradeData d)
			{
				if (d.PilotUser != null)
				{
					return d.PilotUser.UpgradeRequest.ToString();
				}
				if (d.Tenant == null)
				{
					return string.Empty;
				}
				return d.Tenant.UpgradeRequest.ToString();
			});

			public static readonly ObjectLogSimplePropertyDefinition<TenantUpgradeData> UpgradeStatus = new ObjectLogSimplePropertyDefinition<TenantUpgradeData>("UpgradeStatus", delegate(TenantUpgradeData d)
			{
				if (d.PilotUser != null)
				{
					return d.PilotUser.UpgradeStatus.ToString();
				}
				if (d.Tenant == null)
				{
					return string.Empty;
				}
				return d.Tenant.UpgradeStatus.ToString();
			});

			public static readonly ObjectLogSimplePropertyDefinition<TenantUpgradeData> UpgradeStageChangeTo = new ObjectLogSimplePropertyDefinition<TenantUpgradeData>("UpgradeStageChangeTo", delegate(TenantUpgradeData d)
			{
				if (d.PilotUser != null)
				{
					return Convert.ToString(d.PilotUser.UpgradeStage);
				}
				if (d.Tenant == null)
				{
					return string.Empty;
				}
				return Convert.ToString(d.Tenant.UpgradeStage);
			});

			public static readonly ObjectLogSimplePropertyDefinition<TenantUpgradeData> Errortype = new ObjectLogSimplePropertyDefinition<TenantUpgradeData>("Errortype", (TenantUpgradeData d) => d.ErrorType);

			public static readonly ObjectLogSimplePropertyDefinition<TenantUpgradeData> ErrorDetails = new ObjectLogSimplePropertyDefinition<TenantUpgradeData>("ErrorDetails", (TenantUpgradeData d) => d.ErrorDetails);
		}

		private class UpgradeHandlerSyncLogConfiguration : ObjectLogConfiguration
		{
			public UpgradeHandlerSyncLogConfiguration(string logFilePrefix)
			{
				this.logFilePrefix = logFilePrefix;
			}

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
							return Path.Combine(path, "Logging\\UpgradeHandlerLogs");
						}
					}
					return null;
				}
			}

			public override string LogComponentName
			{
				get
				{
					return "UpgradeHandler";
				}
			}

			public override string FilenamePrefix
			{
				get
				{
					return this.logFilePrefix;
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

			private readonly string logFilePrefix;
		}
	}
}
