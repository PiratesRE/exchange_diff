using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Win32;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	internal class TenantSizeLog : DisposableObjectLog<TenantSize>
	{
		public TenantSizeLog(string logFilePrefix) : base(new TenantSizeLog.TenantSizeLogSchema(), new TenantSizeLog.TenantSizeLogConfiguration(logFilePrefix))
		{
		}

		public void Write(TenantData tenantData, string error)
		{
			TenantSize objectToLog = new TenantSize
			{
				ExternalDirectoryOrganizationId = tenantData.TenantId,
				Name = tenantData.TenantName,
				Constraints = tenantData.Constraints,
				UpgradeConstraintsDisabled = tenantData.UpgradeConstraintsDisabled,
				UpgradeUnitsOverride = tenantData.UpgradeUnitsOverride,
				ServicePlan = tenantData.ServicePlan,
				ProgramId = tenantData.ProgramId,
				OfferId = tenantData.OfferId,
				AdminDisplayVersion = tenantData.Version,
				IsUpgradingOrganization = tenantData.IsUpgradingOrganization,
				IsPilotingOrganization = tenantData.IsPilotingOrganization,
				E14PrimaryMbxCount = tenantData.E14MbxData.PrimaryData.Count,
				E14PrimaryMbxSize = tenantData.E14MbxData.PrimaryData.Size,
				E14ArchiveMbxCount = tenantData.E14MbxData.ArchiveData.Count,
				E14ArchiveMbxSize = tenantData.E14MbxData.ArchiveData.Size,
				E15PrimaryMbxCount = tenantData.E15MbxData.PrimaryData.Count,
				E15PrimaryMbxSize = tenantData.E15MbxData.PrimaryData.Size,
				E15ArchiveMbxCount = tenantData.E15MbxData.ArchiveData.Count,
				E15ArchiveMbxSize = tenantData.E15MbxData.ArchiveData.Size,
				TotalPrimaryMbxCount = tenantData.TotalPrimaryMbxCount,
				TotalPrimaryMbxSize = tenantData.TotalPrimaryMbxSize,
				TotalArchiveMbxCount = tenantData.TotalArchiveMbxCount,
				TotalArchiveMbxSize = tenantData.TotalArchiveMbxSize,
				UploadedSize = tenantData.TenantSize,
				ValidationError = error
			};
			base.LogObject(objectToLog);
		}

		private class TenantSizeLogSchema : ObjectLogSchema
		{
			public override string Software
			{
				get
				{
					return "Microsoft Exchange Tenant Data Collector";
				}
			}

			public override string LogType
			{
				get
				{
					return "Tenant Size Log";
				}
			}

			public static readonly ObjectLogSimplePropertyDefinition<TenantSize> ExternalDirectoryOrganizationId = new ObjectLogSimplePropertyDefinition<TenantSize>("TenantId", (TenantSize d) => d.ExternalDirectoryOrganizationId.ToString());

			public static readonly ObjectLogSimplePropertyDefinition<TenantSize> Name = new ObjectLogSimplePropertyDefinition<TenantSize>("Name", (TenantSize d) => d.Name ?? string.Empty);

			public static readonly ObjectLogSimplePropertyDefinition<TenantSize> Constraints = new ObjectLogSimplePropertyDefinition<TenantSize>("Constraints", delegate(TenantSize d)
			{
				if (d.Constraints != null)
				{
					return string.Join(";", d.Constraints);
				}
				return string.Empty;
			});

			public static readonly ObjectLogSimplePropertyDefinition<TenantSize> UpgradeConstraintsDisabled = new ObjectLogSimplePropertyDefinition<TenantSize>("UpgradeConstraintsDisabled", (TenantSize d) => d.UpgradeConstraintsDisabled);

			public static readonly ObjectLogSimplePropertyDefinition<TenantSize> UpgradeUnitsOverride = new ObjectLogSimplePropertyDefinition<TenantSize>("UpgradeUnitsOverride", (TenantSize d) => d.UpgradeUnitsOverride);

			public static readonly ObjectLogSimplePropertyDefinition<TenantSize> ServicePlan = new ObjectLogSimplePropertyDefinition<TenantSize>("ServicePlan", (TenantSize d) => d.ServicePlan ?? string.Empty);

			public static readonly ObjectLogSimplePropertyDefinition<TenantSize> ProgramId = new ObjectLogSimplePropertyDefinition<TenantSize>("ProgramId", (TenantSize d) => d.ProgramId ?? string.Empty);

			public static readonly ObjectLogSimplePropertyDefinition<TenantSize> OfferId = new ObjectLogSimplePropertyDefinition<TenantSize>("OfferId", (TenantSize d) => d.OfferId ?? string.Empty);

			public static readonly ObjectLogSimplePropertyDefinition<TenantSize> AdminDisplayVersion = new ObjectLogSimplePropertyDefinition<TenantSize>("AdminDisplayVersion", delegate(TenantSize d)
			{
				if (!(d.AdminDisplayVersion == null))
				{
					return d.AdminDisplayVersion.ToString();
				}
				return string.Empty;
			});

			public static readonly ObjectLogSimplePropertyDefinition<TenantSize> IsUpgradingOrganization = new ObjectLogSimplePropertyDefinition<TenantSize>("IsUpgradingOrganization", (TenantSize d) => d.IsUpgradingOrganization);

			public static readonly ObjectLogSimplePropertyDefinition<TenantSize> IsPilotingOrganization = new ObjectLogSimplePropertyDefinition<TenantSize>("IsPilotingOrganization", (TenantSize d) => d.IsPilotingOrganization);

			public static readonly ObjectLogSimplePropertyDefinition<TenantSize> E14PrimaryMbxCount = new ObjectLogSimplePropertyDefinition<TenantSize>("E14PrimaryMbxCount", (TenantSize d) => d.E14PrimaryMbxCount);

			public static readonly ObjectLogSimplePropertyDefinition<TenantSize> E14PrimaryMbxSize = new ObjectLogSimplePropertyDefinition<TenantSize>("E14PrimaryMbxSize", (TenantSize d) => d.E14PrimaryMbxSize);

			public static readonly ObjectLogSimplePropertyDefinition<TenantSize> E14ArchiveMbxCount = new ObjectLogSimplePropertyDefinition<TenantSize>("E14ArchiveMbxCount", (TenantSize d) => d.E14ArchiveMbxCount);

			public static readonly ObjectLogSimplePropertyDefinition<TenantSize> E14ArchiveMbxSize = new ObjectLogSimplePropertyDefinition<TenantSize>("E14ArchiveMbxSize", (TenantSize d) => d.E14ArchiveMbxSize);

			public static readonly ObjectLogSimplePropertyDefinition<TenantSize> E15PrimaryMbxCount = new ObjectLogSimplePropertyDefinition<TenantSize>("E15PrimaryMbxCount", (TenantSize d) => d.E15PrimaryMbxCount);

			public static readonly ObjectLogSimplePropertyDefinition<TenantSize> E15PrimaryMbxSize = new ObjectLogSimplePropertyDefinition<TenantSize>("E15PrimaryMbxSize", (TenantSize d) => d.E15PrimaryMbxSize);

			public static readonly ObjectLogSimplePropertyDefinition<TenantSize> E15ArchiveMbxCount = new ObjectLogSimplePropertyDefinition<TenantSize>("E15ArchiveMbxCount", (TenantSize d) => d.E15ArchiveMbxCount);

			public static readonly ObjectLogSimplePropertyDefinition<TenantSize> E15ArchiveMbxSize = new ObjectLogSimplePropertyDefinition<TenantSize>("E15ArchiveMbxSize", (TenantSize d) => d.E15ArchiveMbxSize);

			public static readonly ObjectLogSimplePropertyDefinition<TenantSize> TotalPrimaryMbxCount = new ObjectLogSimplePropertyDefinition<TenantSize>("TotalPrimaryMbxCount", (TenantSize d) => d.TotalPrimaryMbxCount);

			public static readonly ObjectLogSimplePropertyDefinition<TenantSize> TotalPrimaryMbxSize = new ObjectLogSimplePropertyDefinition<TenantSize>("TotalPrimaryMbxSize", (TenantSize d) => d.TotalPrimaryMbxSize);

			public static readonly ObjectLogSimplePropertyDefinition<TenantSize> TotalArchiveMbxCount = new ObjectLogSimplePropertyDefinition<TenantSize>("TotalArchiveMbxCount", (TenantSize d) => d.TotalArchiveMbxCount);

			public static readonly ObjectLogSimplePropertyDefinition<TenantSize> TotalArchiveMbxSize = new ObjectLogSimplePropertyDefinition<TenantSize>("TotalArchiveMbxSize", (TenantSize d) => d.TotalArchiveMbxSize);

			public static readonly ObjectLogSimplePropertyDefinition<TenantSize> UploadedSize = new ObjectLogSimplePropertyDefinition<TenantSize>("UploadedSize", (TenantSize d) => d.UploadedSize);

			public static readonly ObjectLogSimplePropertyDefinition<TenantSize> ValidationError = new ObjectLogSimplePropertyDefinition<TenantSize>("ValidationError", (TenantSize d) => d.ValidationError);
		}

		private class TenantSizeLogConfiguration : ObjectLogConfiguration
		{
			public TenantSizeLogConfiguration(string logFilePrefix)
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
							return Path.Combine(path, "Logging\\TenantDataCollectorLogs");
						}
					}
					return null;
				}
			}

			public override string LogComponentName
			{
				get
				{
					return "TenantDataCollector";
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
					return 10000000L;
				}
			}

			private readonly string logFilePrefix;
		}
	}
}
