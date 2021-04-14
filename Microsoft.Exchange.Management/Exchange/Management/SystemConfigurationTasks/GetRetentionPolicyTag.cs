using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.InfoWorker.Common.ELC;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "RetentionPolicyTag", DefaultParameterSetName = "Identity")]
	public sealed class GetRetentionPolicyTag : GetMultitenancySystemConfigurationObjectTask<RetentionPolicyTagIdParameter, RetentionPolicyTag>
	{
		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IgnoreDehydratedFlag { get; set; }

		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				if (!this.IgnoreDehydratedFlag)
				{
					return SharedTenantConfigurationMode.Dehydrateable;
				}
				return SharedTenantConfigurationMode.NotShared;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IncludeSystemTags
		{
			get
			{
				return (SwitchParameter)(base.Fields["IncludeSystemTags"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["IncludeSystemTags"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "ParameterSetMailboxTask")]
		public MailboxIdParameter Mailbox
		{
			get
			{
				return (MailboxIdParameter)base.Fields["Mailbox"];
			}
			set
			{
				base.Fields["Mailbox"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "ParameterSetMailboxTask")]
		public SwitchParameter OptionalInMailbox
		{
			get
			{
				return (SwitchParameter)(base.Fields["OptionalInMailbox"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["OptionalInMailbox"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ElcFolderType[] Types
		{
			get
			{
				return (ElcFolderType[])base.Fields["Types"];
			}
			set
			{
				base.Fields["Types"] = value;
			}
		}

		protected override ITaskModuleFactory CreateTaskModuleFactory()
		{
			return new GetTaskBaseModuleFactory();
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			RetentionPolicyTag retentionPolicyTag = (RetentionPolicyTag)dataObject;
			if (base.Fields.Contains("Types") && this.Types.Length > 0 && !this.Types.Contains(retentionPolicyTag.Type))
			{
				return;
			}
			if (this.Identity != null || !retentionPolicyTag.SystemTag || this.IncludeSystemTags)
			{
				ElcContentSettings[] array = retentionPolicyTag.GetELCContentSettings().ToArray<ElcContentSettings>();
				if (array == null || array.Length > 1)
				{
					this.WriteWarning(Strings.WarningRetentionPolicyTagCorrupted(retentionPolicyTag.Name));
				}
				PresentationRetentionPolicyTag dataObject2 = new PresentationRetentionPolicyTag(retentionPolicyTag, array.FirstOrDefault<ElcContentSettings>());
				base.WriteResult(dataObject2);
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (this.Mailbox != null)
			{
				try
				{
					using (StoreRetentionPolicyTagHelper storeRetentionPolicyTagHelper = StoreRetentionPolicyTagHelper.FromMailboxId(base.DomainController, this.Mailbox, base.CurrentOrganizationId))
					{
						if (storeRetentionPolicyTagHelper.Mailbox.ExchangeVersion.IsOlderThan(ExchangeObjectVersion.Exchange2010))
						{
							base.WriteError(new InvalidOperationException(Strings.OptInNotSupportedForPre14Mailbox(ExchangeObjectVersion.Exchange2010.ToString(), storeRetentionPolicyTagHelper.Mailbox.Identity.ToString(), storeRetentionPolicyTagHelper.Mailbox.ExchangeVersion.ToString())), ErrorCategory.InvalidOperation, storeRetentionPolicyTagHelper.Mailbox.Identity);
						}
						IConfigurationSession configurationSession = base.DataSession as IConfigurationSession;
						configurationSession.SessionSettings.IsSharedConfigChecked = true;
						if (storeRetentionPolicyTagHelper.TagData != null && storeRetentionPolicyTagHelper.TagData.Count > 0)
						{
							foreach (Guid guid in storeRetentionPolicyTagHelper.TagData.Keys)
							{
								RetentionPolicyTag retentionTagFromGuid = this.GetRetentionTagFromGuid(guid, configurationSession);
								StoreTagData storeTagData = storeRetentionPolicyTagHelper.TagData[guid];
								if ((storeTagData.IsVisible || storeTagData.Tag.Type == ElcFolderType.All) && ((this.OptionalInMailbox && storeTagData.OptedInto) || !this.OptionalInMailbox) && retentionTagFromGuid != null)
								{
									this.WriteResult(retentionTagFromGuid);
								}
							}
						}
						if (!this.OptionalInMailbox && storeRetentionPolicyTagHelper.DefaultArchiveTagData != null && storeRetentionPolicyTagHelper.DefaultArchiveTagData.Count > 0)
						{
							foreach (Guid guid2 in storeRetentionPolicyTagHelper.DefaultArchiveTagData.Keys)
							{
								RetentionPolicyTag retentionTagFromGuid2 = this.GetRetentionTagFromGuid(guid2, configurationSession);
								if (retentionTagFromGuid2 != null)
								{
									StoreTagData storeTagData2 = storeRetentionPolicyTagHelper.DefaultArchiveTagData[guid2];
									if (storeTagData2.Tag.Type == ElcFolderType.All)
									{
										this.WriteResult(retentionTagFromGuid2);
									}
								}
							}
						}
					}
					goto IL_20A;
				}
				catch (ElcUserConfigurationException exception)
				{
					base.WriteError(exception, ErrorCategory.ResourceUnavailable, null);
					goto IL_20A;
				}
			}
			base.InternalProcessRecord();
			IL_20A:
			TaskLogger.LogExit();
		}

		protected override IConfigDataProvider CreateSession()
		{
			IConfigurationSession configurationSession;
			if (!this.IgnoreDehydratedFlag && SharedConfiguration.IsDehydratedConfiguration(base.CurrentOrganizationId))
			{
				configurationSession = SharedConfiguration.CreateScopedToSharedConfigADSession(base.CurrentOrganizationId);
				return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(false, ConsistencyMode.IgnoreInvalid, configurationSession.SessionSettings, 248, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Elc\\GetRetentionPolicyTag.cs");
			}
			if (!MobileDeviceTaskHelper.IsRunningUnderMyOptionsRole(this, base.TenantGlobalCatalogSession, base.SessionSettings))
			{
				configurationSession = (IConfigurationSession)base.CreateSession();
			}
			else
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
				configurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, sessionSettings, 267, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Elc\\GetRetentionPolicyTag.cs");
			}
			return configurationSession;
		}

		private RetentionPolicyTag GetRetentionTagFromGuid(Guid tagGuid, IConfigurationSession session)
		{
			OrFilter filter = new OrFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, RetentionPolicyTagSchema.RetentionId, tagGuid),
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Guid, tagGuid)
			});
			IList<RetentionPolicyTag> list = session.Find<RetentionPolicyTag>(null, QueryScope.SubTree, filter, null, 1);
			if (list != null && list.Count > 0)
			{
				return list[0];
			}
			return null;
		}

		public const string ParameterSetMailboxTask = "ParameterSetMailboxTask";
	}
}
