using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Win32;

namespace Microsoft.Exchange.Configuration.Authorization
{
	internal struct RbacQuery
	{
		static RbacQuery()
		{
			foreach (object obj in Enum.GetValues(typeof(RoleType)))
			{
				RoleType roleType = (RoleType)obj;
				RbacQuery.WellKnownQueryProcessors.Add(roleType.ToString(), new RbacQuery.RoleTypeQueryProcessor(roleType));
			}
			List<string> list = new List<string>(RbacQuery.WellKnownQueryProcessors.Keys);
			foreach (string text in list)
			{
				RbacQuery.RbacQueryProcessor predicate = RbacQuery.WellKnownQueryProcessors[text];
				RbacQuery.WellKnownQueryProcessors.Add("!" + text, new RbacQuery.NotQueryProcessor(predicate));
			}
		}

		public RbacQuery(string rbacQuery)
		{
			this = new RbacQuery(rbacQuery, null);
		}

		public RbacQuery(string rbacQuery, ADRawEntry adRawEntry)
		{
			if (string.IsNullOrEmpty(rbacQuery))
			{
				throw new ArgumentNullException("rbacQuery");
			}
			if (!RbacQuery.WellKnownQueryProcessors.TryGetValue(rbacQuery, out this.queryProcessor) && !RbacQuery.ConditionalQueryProcessors.TryParse(rbacQuery, out this.queryProcessor) && !RbacQuery.CmdletQueryProcessor.TryParse(rbacQuery, out this.queryProcessor))
			{
				throw new ArgumentException(string.Format("'{0}' is not a valid RBAC query.", rbacQuery));
			}
			if (adRawEntry != null)
			{
				RbacQuery.CmdletQueryProcessor cmdletQueryProcessor = this.queryProcessor as RbacQuery.CmdletQueryProcessor;
				if (cmdletQueryProcessor != null)
				{
					cmdletQueryProcessor.TargetObject = adRawEntry;
				}
			}
		}

		public static void RegisterQueryProcessor(string role, RbacQuery.RbacQueryProcessor processor)
		{
			RbacQuery.WellKnownQueryProcessors.Add(role, processor);
			RbacQuery.WellKnownQueryProcessors.Add("!" + role, new RbacQuery.NotQueryProcessor(processor));
		}

		public bool IsInRole(ExchangeRunspaceConfiguration rbacConfiguration)
		{
			return this.queryProcessor.IsInRole(rbacConfiguration);
		}

		public bool CanCache
		{
			get
			{
				return this.queryProcessor.CanCache;
			}
		}

		public bool IsCmdletQuery
		{
			get
			{
				return this.queryProcessor is RbacQuery.CmdletQueryProcessor;
			}
		}

		public string SnapInName
		{
			get
			{
				if (!this.IsCmdletQuery)
				{
					return null;
				}
				return ((RbacQuery.CmdletQueryProcessor)this.queryProcessor).SnapInName;
			}
		}

		public string CmdletName
		{
			get
			{
				if (!this.IsCmdletQuery)
				{
					return null;
				}
				return ((RbacQuery.CmdletQueryProcessor)this.queryProcessor).CmdletName;
			}
		}

		public string QualifiedCmdletName
		{
			get
			{
				if (!this.IsCmdletQuery)
				{
					return null;
				}
				return ((RbacQuery.CmdletQueryProcessor)this.queryProcessor).QualifiedCmdletName;
			}
		}

		public string[] ParameterNames
		{
			get
			{
				if (!this.IsCmdletQuery)
				{
					return null;
				}
				return ((RbacQuery.CmdletQueryProcessor)this.queryProcessor).ParameterNames;
			}
		}

		public ScopeSet ScopeSet
		{
			get
			{
				if (!this.IsCmdletQuery)
				{
					return null;
				}
				return ((RbacQuery.CmdletQueryProcessor)this.queryProcessor).ScopeSet;
			}
		}

		public RoleType RoleType
		{
			get
			{
				if (!this.IsRoleTypeQuery)
				{
					return (RoleType)0;
				}
				return ((RbacQuery.RoleTypeQueryProcessor)this.queryProcessor).RoleType;
			}
		}

		public bool IsRoleTypeQuery
		{
			get
			{
				return this.queryProcessor is RbacQuery.RoleTypeQueryProcessor;
			}
		}

		public bool IsExchangeSkuQuery
		{
			get
			{
				return this.queryProcessor is RbacQuery.ExchangeSkuQueryProcessor;
			}
		}

		public Datacenter.ExchangeSku ExchangeSku
		{
			get
			{
				return ((RbacQuery.ExchangeSkuQueryProcessor)this.queryProcessor).ExchangeSku;
			}
		}

		public bool IsBrowserCheckQuery
		{
			get
			{
				return this.queryProcessor is RbacQuery.BrowserQueryProcessor;
			}
		}

		public static bool LegacyIsScoped
		{
			get
			{
				return RbacQuery.LegacyTargetObject is ADRawEntry;
			}
		}

		public static IConfigurable LegacyTargetObject
		{
			get
			{
				return (IConfigurable)HttpContext.Current.Items[RbacQuery.rbacQueryRawEntryKey];
			}
			set
			{
				HttpContext.Current.Items[RbacQuery.rbacQueryRawEntryKey] = value;
			}
		}

		internal static Dictionary<string, RbacQuery.RbacQueryProcessor> WellKnownQueryProcessors = new Dictionary<string, RbacQuery.RbacQueryProcessor>(127)
		{
			{
				"*",
				RbacQuery.AccountFeaturesQueryProcessor.Any
			},
			{
				"LiveID",
				RbacQuery.ExchangeSkuQueryProcessor.LiveId
			},
			{
				"Enterprise",
				RbacQuery.ExchangeSkuQueryProcessor.Enterprise
			},
			{
				"Hosted",
				RbacQuery.ExchangeSkuQueryProcessor.PartnerHosted
			},
			{
				"MultiTenant",
				new RbacQuery.AnyQueryProcessor(new List<RbacQuery.RbacQueryProcessor>
				{
					RbacQuery.ExchangeSkuQueryProcessor.LiveId,
					RbacQuery.ExchangeSkuQueryProcessor.ForeFrontForOffice,
					RbacQuery.ExchangeSkuQueryProcessor.PartnerHosted
				})
			},
			{
				"Dedicated",
				RbacQuery.ExchangeSkuQueryProcessor.Dedicated
			},
			{
				"IE",
				RbacQuery.BrowserQueryProcessor.IE
			},
			{
				"SkipToUAndParentalControlCheck",
				RbacQuery.OrganizationPropertyQueryProcessor.SkipToUAndParentalControlCheck
			},
			{
				"OWA",
				RbacQuery.AccountFeaturesQueryProcessor.OWA
			},
			{
				"Admin",
				RbacQuery.AccountFeaturesQueryProcessor.Admin
			},
			{
				"MailboxFullAccess",
				RbacQuery.AccountFeaturesQueryProcessor.MailboxFullAccess
			},
			{
				"UM",
				RbacQuery.AccountFeaturesQueryProcessor.UM
			},
			{
				"ExternalReplies",
				RbacQuery.AccountFeaturesQueryProcessor.ExternalReplies
			},
			{
				"Resource",
				RbacQuery.AccountFeaturesQueryProcessor.Resource
			},
			{
				"UMConfigured",
				RbacQuery.AccountFeaturesQueryProcessor.UMConfigured
			},
			{
				"ActiveSync",
				RbacQuery.AccountFeaturesQueryProcessor.ActiveSync
			},
			{
				"Outlook",
				RbacQuery.AccountFeaturesQueryProcessor.Outlook
			},
			{
				"Impersonated",
				RbacQuery.AccountFeaturesQueryProcessor.Impersonated
			},
			{
				"Partner",
				RbacQuery.AccountFeaturesQueryProcessor.Partner
			},
			{
				"RetentionPolicy",
				RbacQuery.AccountFeaturesQueryProcessor.RetentionPolicy
			},
			{
				"DelegatedAdmin",
				RbacQuery.AccountFeaturesQueryProcessor.DelegatedAdmin
			},
			{
				"ByoidAdmin",
				RbacQuery.ByoidAdminQueryProcessor.ByoidAdmin
			},
			{
				"MachineToPersonTextingOnly",
				RbacQuery.AccountFeaturesQueryProcessor.MachineToPersonTextMessagingOnlyEnabled
			},
			{
				"Mailbox",
				RbacQuery.RecipientTypeQueryProcessor.Mailbox
			},
			{
				"LogonUserMailbox",
				RbacQuery.RecipientTypeQueryProcessor.LogonUserMailbox
			},
			{
				"LogonMailUser",
				RbacQuery.RecipientTypeQueryProcessor.LogonMailUser
			},
			{
				"Rules",
				RbacQuery.OwaSegmentationQueryProcessor.Rules
			},
			{
				"Signatures",
				RbacQuery.OwaSegmentationQueryProcessor.Signatures
			},
			{
				"SpellChecker",
				RbacQuery.OwaSegmentationQueryProcessor.SpellChecker
			},
			{
				"Calendar",
				RbacQuery.OwaSegmentationQueryProcessor.Calendar
			},
			{
				"RemindersAndNotifications",
				RbacQuery.OwaSegmentationQueryProcessor.RemindersAndNotifications
			},
			{
				"GlobalAddressList",
				RbacQuery.OwaSegmentationQueryProcessor.GlobalAddressList
			},
			{
				"Contacts",
				RbacQuery.OwaSegmentationQueryProcessor.Contacts
			},
			{
				"OWALight",
				RbacQuery.OwaSegmentationQueryProcessor.OWALight
			},
			{
				"ChangePassword",
				RbacQuery.OwaSegmentationQueryProcessor.ChangePassword
			},
			{
				"SMime",
				RbacQuery.OwaSegmentationQueryProcessor.SMime
			},
			{
				"UMIntegration",
				RbacQuery.OwaSegmentationQueryProcessor.UMIntegration
			},
			{
				"TextMessaging",
				RbacQuery.OwaSegmentationQueryProcessor.TextMessaging
			},
			{
				"JunkEmail",
				RbacQuery.OwaSegmentationQueryProcessor.JunkEmail
			},
			{
				"SetPhotoEnabled",
				RbacQuery.OwaSegmentationQueryProcessor.SetPhotoEnabled
			},
			{
				"MobileDevices",
				RbacQuery.OwaSegmentationQueryProcessor.MobileDevices
			},
			{
				"OrgMgmControlPanel",
				RbacQuery.ExchangeFeaturesEnabledProcessor.OrgMgmECPEnabled
			},
			{
				"IsLicensingEnforced",
				RbacQuery.OrganizationPropertyQueryProcessor.IsLicensingEnforced
			},
			{
				"BposUser",
				RbacQuery.AccountFeaturesQueryProcessor.BposUser
			},
			{
				"FFO",
				RbacQuery.FFOQueryProcessor.FFO
			},
			{
				"EOPStandard",
				RbacQuery.EOPStandardQueryProcessor.EOPStandard
			},
			{
				"EOPPremium",
				RbacQuery.OrganizationConfigurationQueryProcessor.IsEOPPremiumCapability
			},
			{
				"FacebookEnabled",
				RbacQuery.OwaSegmentationQueryProcessor.FacebookEnabled
			},
			{
				"LinkedInEnabled",
				RbacQuery.OwaSegmentationQueryProcessor.LinkedInEnabled
			},
			{
				"WacExternalServicesEnabled",
				RbacQuery.OwaSegmentationQueryProcessor.WacExternalServicesEnabled
			},
			{
				"WacOMEXEnabled",
				RbacQuery.OwaSegmentationQueryProcessor.WacOMEXEnabled
			},
			{
				"FFOMigrationInProgress",
				RbacQuery.OrganizationConfigurationQueryProcessor.FFOMigrationInProgressQueryProcessor
			},
			{
				"IsPilotMode",
				RbacQuery.OrganizationConfigurationQueryProcessor.IsPilotModeQueryProcessor
			},
			{
				"IsGallatin",
				RbacQuery.GallatinQueryProcessor.IsGallatin
			},
			{
				"ReportJunkEmailEnabled",
				RbacQuery.OwaSegmentationQueryProcessor.ReportJunkEmailEnabled
			},
			{
				"GroupCreationEnabled",
				RbacQuery.OwaSegmentationQueryProcessor.GroupCreationEnabled
			},
			{
				"EndUserExperience",
				new RbacQuery.EndUserExperienceQueryProcessor()
			}
		};

		private RbacQuery.RbacQueryProcessor queryProcessor;

		private static object rbacQueryRawEntryKey = new object();

		internal abstract class RbacQueryProcessor
		{
			public virtual bool CanCache
			{
				get
				{
					return true;
				}
			}

			public bool IsInRole(ExchangeRunspaceConfiguration rbacConfiguration)
			{
				return this.TryIsInRole(rbacConfiguration) ?? false;
			}

			public abstract bool? TryIsInRole(ExchangeRunspaceConfiguration rbacConfiguration);
		}

		private sealed class NotQueryProcessor : RbacQuery.RbacQueryProcessor
		{
			public NotQueryProcessor(RbacQuery.RbacQueryProcessor predicate)
			{
				this.Predicate = predicate;
			}

			public RbacQuery.RbacQueryProcessor Predicate { get; private set; }

			public override bool CanCache
			{
				get
				{
					return this.Predicate.CanCache;
				}
			}

			public override bool? TryIsInRole(ExchangeRunspaceConfiguration rbacConfiguration)
			{
				bool? flag = this.Predicate.TryIsInRole(rbacConfiguration);
				if (flag == null)
				{
					return flag;
				}
				bool? flag2 = flag;
				if (flag2 == null)
				{
					return null;
				}
				return new bool?(!flag2.GetValueOrDefault());
			}
		}

		private sealed class AnyQueryProcessor : RbacQuery.RbacQueryProcessor
		{
			public AnyQueryProcessor(List<RbacQuery.RbacQueryProcessor> predicates)
			{
				this.Predicates = predicates;
			}

			public List<RbacQuery.RbacQueryProcessor> Predicates { get; private set; }

			public override bool CanCache
			{
				get
				{
					foreach (RbacQuery.RbacQueryProcessor rbacQueryProcessor in this.Predicates)
					{
						if (rbacQueryProcessor.CanCache)
						{
							return true;
						}
					}
					return false;
				}
			}

			public override bool? TryIsInRole(ExchangeRunspaceConfiguration rbacConfiguration)
			{
				bool? result = null;
				foreach (RbacQuery.RbacQueryProcessor rbacQueryProcessor in this.Predicates)
				{
					bool? flag = rbacQueryProcessor.TryIsInRole(rbacConfiguration);
					if (flag == true)
					{
						return new bool?(true);
					}
					if (flag == false)
					{
						result = new bool?(false);
					}
				}
				return result;
			}
		}

		private sealed class ExchangeSkuQueryProcessor : RbacQuery.RbacQueryProcessor
		{
			public Datacenter.ExchangeSku ExchangeSku { get; private set; }

			private ExchangeSkuQueryProcessor(Datacenter.ExchangeSku exchangeSku)
			{
				this.ExchangeSku = exchangeSku;
			}

			public override bool? TryIsInRole(ExchangeRunspaceConfiguration rbacConfiguration)
			{
				return new bool?(this.ExchangeSku == Datacenter.GetExchangeSku());
			}

			public static readonly RbacQuery.RbacQueryProcessor LiveId = new RbacQuery.ExchangeSkuQueryProcessor(Datacenter.ExchangeSku.ExchangeDatacenter);

			public static readonly RbacQuery.RbacQueryProcessor Enterprise = new RbacQuery.ExchangeSkuQueryProcessor(Datacenter.ExchangeSku.Enterprise);

			public static readonly RbacQuery.RbacQueryProcessor PartnerHosted = new RbacQuery.ExchangeSkuQueryProcessor(Datacenter.ExchangeSku.PartnerHosted);

			public static readonly RbacQuery.RbacQueryProcessor ForeFrontForOffice = new RbacQuery.ExchangeSkuQueryProcessor(Datacenter.ExchangeSku.ForefrontForOfficeDatacenter);

			public static readonly RbacQuery.RbacQueryProcessor Dedicated = new RbacQuery.ExchangeSkuQueryProcessor(Datacenter.ExchangeSku.DatacenterDedicated);
		}

		private sealed class RoleTypeQueryProcessor : RbacQuery.RbacQueryProcessor
		{
			public RoleType RoleType { get; private set; }

			public RoleTypeQueryProcessor(RoleType roleType)
			{
				this.RoleType = roleType;
			}

			public override bool? TryIsInRole(ExchangeRunspaceConfiguration rbacConfiguration)
			{
				return new bool?(rbacConfiguration.HasRoleOfType(this.RoleType));
			}
		}

		private sealed class RecipientTypeQueryProcessor : RbacQuery.RbacQueryProcessor
		{
			public RecipientType RecipientType { get; private set; }

			public bool ForLogonUser { get; private set; }

			public RecipientTypeQueryProcessor(RecipientType recipientType, bool forLogonUser)
			{
				this.RecipientType = recipientType;
				this.ForLogonUser = forLogonUser;
			}

			public override bool? TryIsInRole(ExchangeRunspaceConfiguration rbacConfiguration)
			{
				return new bool?(this.RecipientType == (this.ForLogonUser ? rbacConfiguration.LogonUserRecipientType : rbacConfiguration.ExecutingUserRecipientType));
			}

			public static readonly RbacQuery.RecipientTypeQueryProcessor Mailbox = new RbacQuery.RecipientTypeQueryProcessor(RecipientType.UserMailbox, false);

			public static readonly RbacQuery.RecipientTypeQueryProcessor LogonUserMailbox = new RbacQuery.RecipientTypeQueryProcessor(RecipientType.UserMailbox, true);

			public static readonly RbacQuery.RecipientTypeQueryProcessor LogonMailUser = new RbacQuery.RecipientTypeQueryProcessor(RecipientType.MailUser, true);
		}

		private sealed class OrganizationPropertyQueryProcessor : RbacQuery.RbacQueryProcessor
		{
			public OrganizationPropertyQueryProcessor(Func<OrganizationProperties, bool> predicate)
			{
				this.predicate = predicate;
			}

			public override bool CanCache
			{
				get
				{
					return false;
				}
			}

			public override bool? TryIsInRole(ExchangeRunspaceConfiguration rbacConfiguration)
			{
				OrganizationProperties arg;
				if (!OrganizationPropertyCache.TryGetOrganizationProperties(rbacConfiguration.OrganizationId, out arg))
				{
					throw new ArgumentException("The organization does not exist in AD. OrgId:" + rbacConfiguration.OrganizationId);
				}
				return new bool?(this.predicate(arg));
			}

			public static readonly RbacQuery.RbacQueryProcessor SkipToUAndParentalControlCheck = new RbacQuery.OrganizationPropertyQueryProcessor((OrganizationProperties x) => x.SkipToUAndParentalControlCheck);

			public static readonly RbacQuery.RbacQueryProcessor IsLicensingEnforced = new RbacQuery.OrganizationPropertyQueryProcessor((OrganizationProperties x) => x.IsLicensingEnforced);

			private Func<OrganizationProperties, bool> predicate;
		}

		private sealed class OrganizationConfigurationQueryProcessor : RbacQuery.RbacQueryProcessor
		{
			public OrganizationConfigurationQueryProcessor(Func<ExchangeConfigurationUnit, bool> predicate, bool canCache)
			{
				this.predicate = predicate;
				this.canCache = canCache;
			}

			public override bool CanCache
			{
				get
				{
					return this.canCache;
				}
			}

			public override bool? TryIsInRole(ExchangeRunspaceConfiguration rbacConfiguration)
			{
				bool? result;
				try
				{
					ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(rbacConfiguration.OrganizationId, false);
					ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(true, ConsistencyMode.PartiallyConsistent, sessionSettings, 265, "TryIsInRole", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\RbacQuery\\RbacQuery.cs");
					ExchangeConfigurationUnit arg = tenantConfigurationSession.Read<ExchangeConfigurationUnit>(rbacConfiguration.OrganizationId.ConfigurationUnit);
					result = new bool?(this.predicate(arg));
				}
				catch
				{
					result = new bool?(false);
				}
				return result;
			}

			private readonly bool canCache = true;

			private Func<ExchangeConfigurationUnit, bool> predicate;

			public static RbacQuery.OrganizationConfigurationQueryProcessor FFOMigrationInProgressQueryProcessor = new RbacQuery.OrganizationConfigurationQueryProcessor((ExchangeConfigurationUnit v) => v.IsFfoMigrationInProgress, false);

			public static RbacQuery.OrganizationConfigurationQueryProcessor IsPilotModeQueryProcessor = new RbacQuery.OrganizationConfigurationQueryProcessor((ExchangeConfigurationUnit v) => v.IsPilotingOrganization, false);

			public static RbacQuery.OrganizationConfigurationQueryProcessor IsEOPPremiumCapability = new RbacQuery.OrganizationConfigurationQueryProcessor((ExchangeConfigurationUnit v) => v.PersistedCapabilities.Contains(Capability.BPOS_S_EopPremiumAddOn) && !v.PersistedCapabilities.Contains(Capability.BPOS_S_Standard) && !v.PersistedCapabilities.Contains(Capability.BPOS_S_Enterprise), false);

			public static RbacQuery.OrganizationConfigurationQueryProcessor IsEOPStandardCapability = new RbacQuery.OrganizationConfigurationQueryProcessor((ExchangeConfigurationUnit v) => v.PersistedCapabilities.Contains(Capability.BPOS_S_EopStandardAddOn), false);
		}

		private sealed class ExchangeFeaturesEnabledProcessor : RbacQuery.RbacQueryProcessor
		{
			private ExchangeFeaturesEnabledProcessor(bool featureEnabled)
			{
				this.featureEnabled = featureEnabled;
			}

			public override bool? TryIsInRole(ExchangeRunspaceConfiguration rbacConfiguration)
			{
				return new bool?(this.featureEnabled);
			}

			private static bool OMECPEnabled
			{
				get
				{
					return Datacenter.GetExchangeSku() != Datacenter.ExchangeSku.PartnerHosted || RbacQuery.ExchangeFeaturesEnabledProcessor.ReadRegistryKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15", "OMECPDisabled") == null;
				}
			}

			private static object ReadRegistryKey(string keyPath, string valueName)
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(keyPath))
				{
					if (registryKey != null)
					{
						return registryKey.GetValue(valueName, null);
					}
				}
				return null;
			}

			private readonly bool featureEnabled;

			public static readonly RbacQuery.RbacQueryProcessor OrgMgmECPEnabled = new RbacQuery.ExchangeFeaturesEnabledProcessor(RbacQuery.ExchangeFeaturesEnabledProcessor.OMECPEnabled);
		}

		private sealed class AccountFeaturesQueryProcessor : RbacQuery.RbacQueryProcessor
		{
			private AccountFeaturesQueryProcessor(Func<ExchangeRunspaceConfiguration, bool> predicate, bool canCache)
			{
				this.predicate = predicate;
				this.canCache = canCache;
			}

			public override bool? TryIsInRole(ExchangeRunspaceConfiguration rbacConfiguration)
			{
				return new bool?(this.predicate(rbacConfiguration));
			}

			public override bool CanCache
			{
				get
				{
					return this.canCache;
				}
			}

			private readonly bool canCache = true;

			public static readonly RbacQuery.RbacQueryProcessor OWA = new RbacQuery.AccountFeaturesQueryProcessor((ExchangeRunspaceConfiguration x) => x.ExecutingUserIsAllowedOWA, true);

			public static readonly RbacQuery.RbacQueryProcessor MailboxFullAccess = new RbacQuery.AccountFeaturesQueryProcessor((ExchangeRunspaceConfiguration x) => !x.Impersonated || !x.OpenMailboxAsAdmin, true);

			public static readonly RbacQuery.RbacQueryProcessor UM = new RbacQuery.AccountFeaturesQueryProcessor((ExchangeRunspaceConfiguration x) => x.ExecutingUserIsUmEnabled, true);

			public static readonly RbacQuery.RbacQueryProcessor ExternalReplies = new RbacQuery.AccountFeaturesQueryProcessor((ExchangeRunspaceConfiguration x) => x.ExecutingUserHasExternalOofOptions, true);

			public static readonly RbacQuery.RbacQueryProcessor Resource = new RbacQuery.AccountFeaturesQueryProcessor((ExchangeRunspaceConfiguration x) => x.ExecutingUserIsResource, true);

			public static readonly RbacQuery.RbacQueryProcessor ActiveSync = new RbacQuery.AccountFeaturesQueryProcessor((ExchangeRunspaceConfiguration x) => x.ExecutingUserIsActiveSyncEnabled, true);

			public static readonly RbacQuery.RbacQueryProcessor Outlook = new RbacQuery.AccountFeaturesQueryProcessor((ExchangeRunspaceConfiguration x) => x.ExecutingUserIsMAPIEnabled, true);

			public static readonly RbacQuery.RbacQueryProcessor UMConfigured = new RbacQuery.AccountFeaturesQueryProcessor((ExchangeRunspaceConfiguration x) => x.ExecutingUserIsUmConfigured, false);

			public static readonly RbacQuery.RbacQueryProcessor Impersonated = new RbacQuery.AccountFeaturesQueryProcessor((ExchangeRunspaceConfiguration x) => x.Impersonated, true);

			public static readonly RbacQuery.RbacQueryProcessor Partner = new RbacQuery.AccountFeaturesQueryProcessor((ExchangeRunspaceConfiguration x) => ExchangeRunspaceConfiguration.IsAllowedOrganizationForPartnerAccounts(x.OrganizationId), true);

			public static readonly RbacQuery.RbacQueryProcessor RetentionPolicy = new RbacQuery.AccountFeaturesQueryProcessor((ExchangeRunspaceConfiguration x) => x.ExecutingUserHasRetentionPolicy, true);

			public static readonly RbacQuery.RbacQueryProcessor DelegatedAdmin = new RbacQuery.AccountFeaturesQueryProcessor((ExchangeRunspaceConfiguration x) => x.DelegatedPrincipal != null, true);

			public static readonly RbacQuery.RbacQueryProcessor Any = new RbacQuery.AccountFeaturesQueryProcessor((ExchangeRunspaceConfiguration x) => true, true);

			public static readonly RbacQuery.RbacQueryProcessor MachineToPersonTextMessagingOnlyEnabled = new RbacQuery.AccountFeaturesQueryProcessor((ExchangeRunspaceConfiguration x) => !x.ExecutingUserIsPersonToPersonTextMessagingEnabled && x.ExecutingUserIsMachineToPersonTextMessagingEnabled, false);

			public static readonly RbacQuery.RbacQueryProcessor Admin = new RbacQuery.AccountFeaturesQueryProcessor((ExchangeRunspaceConfiguration x) => x.HasAdminRoles, true);

			public static readonly RbacQuery.RbacQueryProcessor BposUser = new RbacQuery.AccountFeaturesQueryProcessor((ExchangeRunspaceConfiguration x) => x.ExecutingUserIsBposUser, true);

			private Func<ExchangeRunspaceConfiguration, bool> predicate;
		}

		private sealed class OwaSegmentationQueryProcessor : RbacQuery.RbacQueryProcessor
		{
			public ADPropertyDefinition PropertyDefinition { get; private set; }

			private OwaSegmentationQueryProcessor(ADPropertyDefinition propertyDefinition)
			{
				this.PropertyDefinition = propertyDefinition;
			}

			public override bool? TryIsInRole(ExchangeRunspaceConfiguration rbacConfiguration)
			{
				return new bool?(rbacConfiguration.OwaSegmentationSettings[this.PropertyDefinition]);
			}

			public override bool CanCache
			{
				get
				{
					return true;
				}
			}

			public static readonly RbacQuery.RbacQueryProcessor Rules = new RbacQuery.OwaSegmentationQueryProcessor(OwaMailboxPolicySchema.RulesEnabled);

			public static readonly RbacQuery.RbacQueryProcessor Signatures = new RbacQuery.OwaSegmentationQueryProcessor(OwaMailboxPolicySchema.SignaturesEnabled);

			public static readonly RbacQuery.RbacQueryProcessor SpellChecker = new RbacQuery.OwaSegmentationQueryProcessor(OwaMailboxPolicySchema.SpellCheckerEnabled);

			public static readonly RbacQuery.RbacQueryProcessor Calendar = new RbacQuery.OwaSegmentationQueryProcessor(OwaMailboxPolicySchema.CalendarEnabled);

			public static readonly RbacQuery.RbacQueryProcessor RemindersAndNotifications = new RbacQuery.OwaSegmentationQueryProcessor(OwaMailboxPolicySchema.RemindersAndNotificationsEnabled);

			public static readonly RbacQuery.RbacQueryProcessor GlobalAddressList = new RbacQuery.OwaSegmentationQueryProcessor(OwaMailboxPolicySchema.GlobalAddressListEnabled);

			public static readonly RbacQuery.RbacQueryProcessor Contacts = new RbacQuery.OwaSegmentationQueryProcessor(OwaMailboxPolicySchema.ContactsEnabled);

			public static readonly RbacQuery.RbacQueryProcessor OWALight = new RbacQuery.OwaSegmentationQueryProcessor(OwaMailboxPolicySchema.OWALightEnabled);

			public static readonly RbacQuery.RbacQueryProcessor ChangePassword = new RbacQuery.OwaSegmentationQueryProcessor(OwaMailboxPolicySchema.ChangePasswordEnabled);

			public static readonly RbacQuery.RbacQueryProcessor SMime = new RbacQuery.OwaSegmentationQueryProcessor(OwaMailboxPolicySchema.SMimeEnabled);

			public static readonly RbacQuery.RbacQueryProcessor UMIntegration = new RbacQuery.OwaSegmentationQueryProcessor(OwaMailboxPolicySchema.UMIntegrationEnabled);

			public static readonly RbacQuery.RbacQueryProcessor TextMessaging = new RbacQuery.OwaSegmentationQueryProcessor(OwaMailboxPolicySchema.TextMessagingEnabled);

			public static readonly RbacQuery.RbacQueryProcessor JunkEmail = new RbacQuery.OwaSegmentationQueryProcessor(OwaMailboxPolicySchema.JunkEmailEnabled);

			public static readonly RbacQuery.RbacQueryProcessor SetPhotoEnabled = new RbacQuery.OwaSegmentationQueryProcessor(OwaMailboxPolicySchema.SetPhotoEnabled);

			public static readonly RbacQuery.RbacQueryProcessor MobileDevices = new RbacQuery.OwaSegmentationQueryProcessor(OwaMailboxPolicySchema.ActiveSyncIntegrationEnabled);

			public static readonly RbacQuery.RbacQueryProcessor FacebookEnabled = new RbacQuery.OwaSegmentationQueryProcessor(OwaMailboxPolicySchema.FacebookEnabled);

			public static readonly RbacQuery.RbacQueryProcessor LinkedInEnabled = new RbacQuery.OwaSegmentationQueryProcessor(OwaMailboxPolicySchema.LinkedInEnabled);

			public static readonly RbacQuery.RbacQueryProcessor WacExternalServicesEnabled = new RbacQuery.OwaSegmentationQueryProcessor(OwaMailboxPolicySchema.WacExternalServicesEnabled);

			public static readonly RbacQuery.RbacQueryProcessor WacOMEXEnabled = new RbacQuery.OwaSegmentationQueryProcessor(OwaMailboxPolicySchema.WacOMEXEnabled);

			public static readonly RbacQuery.RbacQueryProcessor ReportJunkEmailEnabled = new RbacQuery.OwaSegmentationQueryProcessor(OwaMailboxPolicySchema.ReportJunkEmailEnabled);

			public static readonly RbacQuery.RbacQueryProcessor GroupCreationEnabled = new RbacQuery.OwaSegmentationQueryProcessor(OwaMailboxPolicySchema.GroupCreationEnabled);

			public static readonly RbacQuery.RbacQueryProcessor SkipCreateUnifiedGroupCustomSharepointClassification = new RbacQuery.OwaSegmentationQueryProcessor(OwaMailboxPolicySchema.SkipCreateUnifiedGroupCustomSharepointClassification);
		}

		private sealed class BrowserQueryProcessor : RbacQuery.RbacQueryProcessor
		{
			public string BrowserSignature { get; private set; }

			public int MajorVersion { get; private set; }

			private BrowserQueryProcessor(string browserSignature, int majorVersion)
			{
				this.BrowserSignature = browserSignature;
				this.MajorVersion = majorVersion;
			}

			public override bool CanCache
			{
				get
				{
					return false;
				}
			}

			public override bool? TryIsInRole(ExchangeRunspaceConfiguration rbacConfiguration)
			{
				HttpBrowserCapabilities browser = HttpContext.Current.Request.Browser;
				return new bool?(browser.MajorVersion >= this.MajorVersion && browser.Browser.IndexOf(this.BrowserSignature, StringComparison.OrdinalIgnoreCase) >= 0);
			}

			public static readonly RbacQuery.RbacQueryProcessor IE = new RbacQuery.BrowserQueryProcessor("IE", 6);
		}

		private sealed class ByoidAdminQueryProcessor : RbacQuery.RbacQueryProcessor
		{
			private ByoidAdminQueryProcessor()
			{
			}

			public override bool CanCache
			{
				get
				{
					return true;
				}
			}

			public override bool? TryIsInRole(ExchangeRunspaceConfiguration rbacConfiguration)
			{
				return new bool?(!string.IsNullOrEmpty(HttpContext.Current.Request.Headers["msExchOrganizationContext"]));
			}

			public static readonly RbacQuery.ByoidAdminQueryProcessor ByoidAdmin = new RbacQuery.ByoidAdminQueryProcessor();
		}

		public sealed class ConditionalQueryProcessors
		{
			public static bool TryParse(string rbacQuery, out RbacQuery.RbacQueryProcessor queryProcessor)
			{
				queryProcessor = null;
				bool flag = false;
				if (rbacQuery == null || rbacQuery.Length == 0)
				{
					return false;
				}
				if (rbacQuery[0] == '!')
				{
					flag = true;
					rbacQuery = rbacQuery.Substring(1);
				}
				if (RbacQuery.ConditionalQueryProcessors.creators != null)
				{
					foreach (RbacQuery.ConditionalQueryProcessors.QueryProcesorCreator queryProcesorCreator in RbacQuery.ConditionalQueryProcessors.creators)
					{
						if (queryProcesorCreator(rbacQuery, out queryProcessor))
						{
							if (flag)
							{
								queryProcessor = new RbacQuery.NotQueryProcessor(queryProcessor);
							}
							return true;
						}
					}
					return false;
				}
				return false;
			}

			public static void Regist(RbacQuery.ConditionalQueryProcessors.QueryProcesorCreator creator)
			{
				if (RbacQuery.ConditionalQueryProcessors.creators == null)
				{
					RbacQuery.ConditionalQueryProcessors.creators = new List<RbacQuery.ConditionalQueryProcessors.QueryProcesorCreator>();
				}
				RbacQuery.ConditionalQueryProcessors.creators.Add(creator);
			}

			private static List<RbacQuery.ConditionalQueryProcessors.QueryProcesorCreator> creators;

			public delegate bool QueryProcesorCreator(string rbacQuery, out RbacQuery.RbacQueryProcessor queryProcessor);
		}

		private sealed class CmdletQueryProcessor : RbacQuery.RbacQueryProcessor
		{
			public string SnapInName { get; private set; }

			public string CmdletName { get; private set; }

			public string QualifiedCmdletName { get; private set; }

			public string[] ParameterNames { get; private set; }

			public ScopeSet ScopeSet { get; private set; }

			public ADRawEntry TargetObject { get; set; }

			static CmdletQueryProcessor()
			{
				using (Stream manifestResourceStream = typeof(RbacQuery).Assembly.GetManifestResourceStream(typeof(RbacQuery), "RbacQuery.txt"))
				{
					using (StreamReader streamReader = new StreamReader(manifestResourceStream))
					{
						RbacQuery.CmdletQueryProcessor.regex = new Regex(streamReader.ReadToEnd(), RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
					}
				}
			}

			public static bool TryParse(string rbacQuery, out RbacQuery.RbacQueryProcessor queryProcessor)
			{
				Match match = RbacQuery.CmdletQueryProcessor.regex.Match(rbacQuery);
				if (match.Success)
				{
					queryProcessor = new RbacQuery.CmdletQueryProcessor(match);
					return true;
				}
				queryProcessor = null;
				return false;
			}

			private CmdletQueryProcessor(Match m)
			{
				this.SnapInName = m.Groups["snapinName"].Value;
				if (string.IsNullOrEmpty(this.SnapInName))
				{
					this.SnapInName = "Microsoft.Exchange.Management.PowerShell.E2010";
				}
				else if (this.SnapInName.IndexOf('.') == -1)
				{
					this.SnapInName = SnapInAliasMap.GetSnapInName(this.SnapInName);
				}
				this.CmdletName = m.Groups["cmdletName"].Value;
				this.QualifiedCmdletName = this.SnapInName + "\\" + this.CmdletName;
				CaptureCollection captures = m.Groups["parameterName"].Captures;
				if (captures.Count > 0)
				{
					this.ParameterNames = new string[captures.Count];
					for (int i = 0; i < this.ParameterNames.Length; i++)
					{
						this.ParameterNames[i] = captures[i].Value;
					}
				}
				ADScope recipientReadScope = RbacQuery.CmdletQueryProcessor.CreateRbacScope(m.Groups["domainReadScope"].Value, false);
				ADScope configReadScope = RbacQuery.CmdletQueryProcessor.CreateRbacScope(m.Groups["configScope"].Value, false);
				ADScopeCollection[] recipientWriteScopes = null;
				CaptureCollection captures2 = m.Groups["domainWriteScope"].Captures;
				if (captures2.Count > 0)
				{
					List<ADScope> list = new List<ADScope>(captures2.Count);
					for (int j = 0; j < captures2.Count; j++)
					{
						ADScope adscope = RbacQuery.CmdletQueryProcessor.CreateRbacScope(captures2[j].Value, true);
						if (adscope != null)
						{
							list.Add(adscope);
						}
					}
					recipientWriteScopes = new ADScopeCollection[]
					{
						new ADScopeCollection(list)
					};
				}
				this.ScopeSet = new ScopeSet(recipientReadScope, recipientWriteScopes, configReadScope, null);
			}

			private static ScopeType GetScopeType(string scopeTypeName)
			{
				switch (scopeTypeName)
				{
				case "Organization":
					return ScopeType.Organization;
				case "MyGAL":
					return ScopeType.MyGAL;
				case "Self":
					return ScopeType.Self;
				case "MyDirectReports":
					return ScopeType.MyDirectReports;
				case "OU":
					return ScopeType.OU;
				case "MyDistributionGroups":
					return ScopeType.MyDistributionGroups;
				case "MyExecutive":
					return ScopeType.MyExecutive;
				case "OrganizationConfig":
					return ScopeType.OrganizationConfig;
				case "ServerScope":
					return ScopeType.CustomConfigScope;
				}
				return ScopeType.None;
			}

			private static ADScope CreateRbacScope(string scopeTypeName, bool ignoreOrganizationScope)
			{
				ScopeType scopeType = RbacQuery.CmdletQueryProcessor.GetScopeType(scopeTypeName);
				if (ignoreOrganizationScope && scopeType == ScopeType.Organization)
				{
					scopeType = ScopeType.None;
				}
				return new RbacScope(scopeType);
			}

			public override bool CanCache
			{
				get
				{
					return !RbacQuery.LegacyIsScoped && !this.IsScoped;
				}
			}

			public bool IsScoped
			{
				get
				{
					return this.TargetObject != null;
				}
			}

			public override bool? TryIsInRole(ExchangeRunspaceConfiguration rbacConfiguration)
			{
				ADRawEntry adrawEntry = this.TargetObject ?? (RbacQuery.LegacyTargetObject as ADRawEntry);
				if (adrawEntry != null && !(adrawEntry is ADConfigurationObject) && !(adrawEntry is OrganizationConfig))
				{
					ScopeLocation scopeLocation = this.CmdletName.StartsWith("get-", StringComparison.OrdinalIgnoreCase) ? ScopeLocation.RecipientRead : ScopeLocation.RecipientWrite;
					return new bool?(rbacConfiguration.IsCmdletAllowedInScope(this.CmdletName, this.ParameterNames ?? new string[0], adrawEntry, scopeLocation));
				}
				return new bool?(rbacConfiguration.IsCmdletAllowedInScope(this.QualifiedCmdletName, this.ParameterNames, this.ScopeSet));
			}

			private static Regex regex;
		}

		private sealed class FFOQueryProcessor : RbacQuery.RbacQueryProcessor
		{
			public override bool? TryIsInRole(ExchangeRunspaceConfiguration rbacConfiguration)
			{
				return new bool?(DatacenterRegistry.IsForefrontForOffice() || RbacQuery.OrganizationConfigurationQueryProcessor.IsEOPPremiumCapability.TryIsInRole(rbacConfiguration).Value);
			}

			public static readonly RbacQuery.FFOQueryProcessor FFO = new RbacQuery.FFOQueryProcessor();
		}

		private sealed class EOPStandardQueryProcessor : RbacQuery.RbacQueryProcessor
		{
			public override bool? TryIsInRole(ExchangeRunspaceConfiguration rbacConfiguration)
			{
				return new bool?(DatacenterRegistry.IsForefrontForOffice() || RbacQuery.OrganizationConfigurationQueryProcessor.IsEOPStandardCapability.TryIsInRole(rbacConfiguration).Value);
			}

			public static readonly RbacQuery.EOPStandardQueryProcessor EOPStandard = new RbacQuery.EOPStandardQueryProcessor();
		}

		private sealed class GallatinQueryProcessor : RbacQuery.RbacQueryProcessor
		{
			public override bool? TryIsInRole(ExchangeRunspaceConfiguration rbacConfiguration)
			{
				return new bool?(DatacenterRegistry.IsForefrontForOffice() ? DatacenterRegistry.IsFFOGallatinDatacenter() : DatacenterRegistry.IsGallatinDatacenter());
			}

			public static readonly RbacQuery.GallatinQueryProcessor IsGallatin = new RbacQuery.GallatinQueryProcessor();
		}

		private sealed class EndUserExperienceQueryProcessor : RbacQuery.RbacQueryProcessor
		{
			public override bool CanCache
			{
				get
				{
					return false;
				}
			}

			public override bool? TryIsInRole(ExchangeRunspaceConfiguration rbacConfiguration)
			{
				return new bool?(RbacQuery.EndUserExperienceQueryProcessor.IsEndUserExperience.Value);
			}

			private static bool GetEndUserExperienceSetting()
			{
				bool result;
				if (!bool.TryParse(ConfigurationManager.AppSettings["EndUserExperience"], out result))
				{
					result = false;
				}
				return result;
			}

			private static readonly Lazy<bool> IsEndUserExperience = new Lazy<bool>(new Func<bool>(RbacQuery.EndUserExperienceQueryProcessor.GetEndUserExperienceSetting));
		}
	}
}
