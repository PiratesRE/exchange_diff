using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[XmlRoot(ElementName = "Configuration", IsNullable = false)]
	[Serializable]
	public sealed class ServicePlan
	{
		internal ServicePlan()
		{
			this.mailboxPlans = new List<ServicePlan.MailboxPlan>();
			this.organization = new ServicePlan.OrganizationSettings();
		}

		internal bool IsValid
		{
			get
			{
				return this.Validate().Count == 0;
			}
		}

		public List<ValidationError> Validate()
		{
			List<ValidationError> list = new List<ValidationError>();
			list.AddRange(this.ValidateFeaturesAllowedForSKU());
			list.AddRange(this.ValidateDependencies());
			return list;
		}

		internal static List<ValidationError> ValidateFileSchema(string fileName)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				throw new ArgumentNullException("FileName");
			}
			List<ValidationError> list = new List<ValidationError>();
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			xmlReaderSettings.ConformanceLevel = ConformanceLevel.Document;
			xmlReaderSettings.ValidationType = ValidationType.Schema;
			List<ValidationError> result;
			using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ServicePlans.xsd"))
			{
				xmlReaderSettings.Schemas.Add(null, XmlReader.Create(manifestResourceStream));
				xmlReaderSettings.ValidationEventHandler += delegate(object _, ValidationEventArgs e)
				{
					throw e.Exception;
				};
				string path = Path.Combine(ConfigurationContext.Setup.InstallPath, "ClientAccess\\ServicePlans");
				string path2 = Path.Combine(path, fileName + ".servicePlan");
				using (Stream stream = File.Open(path2, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					using (XmlReader xmlReader = XmlReader.Create(stream, xmlReaderSettings))
					{
						try
						{
							while (xmlReader.Read())
							{
							}
						}
						catch (XmlSchemaException ex)
						{
							list.Add(new ServicePlanSchemaValidationError(ex.Message));
						}
					}
				}
				result = list;
			}
			return result;
		}

		internal List<ValidationError> ValidateFeaturesAllowedForSKU()
		{
			List<ValidationError> list = new List<ValidationError>();
			list.AddRange(this.Organization.ValidateFeaturesAllowedForSKU());
			foreach (ServicePlan.MailboxPlan mailboxPlan in this.MailboxPlans)
			{
				list.AddRange(mailboxPlan.ValidateFeaturesAllowedForSKU());
			}
			return list;
		}

		internal List<ValidationError> ValidateDependencies()
		{
			List<ValidationError> list = new List<ValidationError>();
			list.AddRange(this.Organization.ValidateDependencies(this));
			foreach (ServicePlan.MailboxPlan mailboxPlan in this.MailboxPlans)
			{
				list.AddRange(mailboxPlan.ValidateDependencies(this));
			}
			return list;
		}

		internal void FixDependencies()
		{
			int num = 0;
			while (!this.IsValid && num++ < 10)
			{
				this.Organization.FixDependencies(this);
				foreach (ServicePlan.MailboxPlan mailboxPlan in this.MailboxPlans)
				{
					mailboxPlan.FixDependencies(this);
				}
			}
		}

		[XmlAttribute]
		public string Version
		{
			get
			{
				return this.version;
			}
			set
			{
				this.version = value;
			}
		}

		public ServicePlan.OrganizationSettings Organization
		{
			get
			{
				return this.organization;
			}
			set
			{
				this.organization = value;
			}
		}

		[XmlAttribute]
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		public List<ServicePlan.MailboxPlan> MailboxPlans
		{
			get
			{
				return this.mailboxPlans;
			}
			set
			{
				this.mailboxPlans = value;
			}
		}

		public ServicePlan.MailboxPlan GetMailboxPlanByName(string name)
		{
			foreach (ServicePlan.MailboxPlan mailboxPlan in this.MailboxPlans)
			{
				if (mailboxPlan.Name == name)
				{
					return mailboxPlan;
				}
			}
			return null;
		}

		public static ServicePlan LoadFromFile(string filePath)
		{
			ServicePlan result;
			try
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(ServicePlan));
				using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					result = (ServicePlan)xmlSerializer.Deserialize(fileStream);
				}
			}
			catch (InvalidOperationException ex)
			{
				throw new XmlDeserializationException(filePath, ex.Message, (ex.InnerException == null) ? string.Empty : ex.InnerException.Message);
			}
			return result;
		}

		internal static bool CompareAndCalculateDelta(ServicePlan servicePlanFrom, ServicePlan servicePlanTo, bool isCrossSKUMigration, out ServicePlan servicePlanDelta, out List<string> featuresToApply)
		{
			bool result = true;
			bool flag = false;
			servicePlanDelta = new ServicePlan();
			servicePlanDelta.Name = "delta";
			featuresToApply = new List<string>();
			bool flag2 = servicePlanFrom.Organization.CommonHydrateableObjectsSharedEnabled && !servicePlanTo.Organization.CommonHydrateableObjectsSharedEnabled;
			bool flag3 = servicePlanFrom.Organization.AdvancedHydrateableObjectsSharedEnabled && !servicePlanTo.Organization.AdvancedHydrateableObjectsSharedEnabled;
			if (!servicePlanFrom.Organization.PilotEnabled)
			{
				bool pilotEnabled = servicePlanTo.Organization.PilotEnabled;
			}
			bool flag4 = servicePlanFrom.Organization.PilotEnabled && !servicePlanTo.Organization.PilotEnabled;
			foreach (object obj in ((IEnumerable)servicePlanFrom.Organization.Schema))
			{
				FeatureDefinition featureDefinition = (FeatureDefinition)obj;
				bool flag5 = false;
				if (isCrossSKUMigration)
				{
					flag5 = (featureDefinition == OrganizationSettingsSchema.RecipientMailSubmissionRateQuota);
					flag5 |= (featureDefinition == OrganizationSettingsSchema.ReducedOutOfTheBoxMrmTagsEnabled);
				}
				if (flag2)
				{
					flag5 |= (featureDefinition == OrganizationSettingsSchema.ReducedOutOfTheBoxMrmTagsEnabled);
					flag5 |= (featureDefinition == OrganizationSettingsSchema.OwaInstantMessagingType);
					flag5 |= (featureDefinition == OrganizationSettingsSchema.PublicFoldersEnabled);
					flag5 |= (featureDefinition == OrganizationSettingsSchema.MalwareFilteringPolicyCustomizationEnabled);
					flag5 |= (featureDefinition == OrganizationSettingsSchema.HostedSpamFilteringPolicyCustomizationEnabled);
				}
				if (flag3)
				{
					flag5 |= (featureDefinition == OrganizationSettingsSchema.DistributionListCountQuota);
					flag5 |= (featureDefinition == OrganizationSettingsSchema.MailboxCountQuota);
					flag5 |= (featureDefinition == OrganizationSettingsSchema.MailUserCountQuota);
					flag5 |= (featureDefinition == OrganizationSettingsSchema.ContactCountQuota);
					flag5 |= (featureDefinition == OrganizationSettingsSchema.TeamMailboxCountQuota);
					flag5 |= (featureDefinition == OrganizationSettingsSchema.PublicFolderMailboxCountQuota);
					flag5 |= (featureDefinition == OrganizationSettingsSchema.MailPublicFolderCountQuota);
					flag5 |= (featureDefinition == OrganizationSettingsSchema.RecipientMailSubmissionRateQuota);
				}
				if (!featureDefinition.IsValueEqual(servicePlanFrom.Organization[featureDefinition], servicePlanTo.Organization[featureDefinition]) || flag5)
				{
					result = false;
					servicePlanDelta.Organization[featureDefinition] = servicePlanTo.Organization[featureDefinition];
					featuresToApply.Add(featureDefinition.Name);
				}
			}
			foreach (ServicePlan.MailboxPlan mailboxPlan in servicePlanTo.MailboxPlans)
			{
				if (servicePlanFrom.GetMailboxPlanByName(mailboxPlan.Name) == null)
				{
					servicePlanDelta.MailboxPlans.Add(mailboxPlan);
					flag = true;
				}
			}
			if (!isCrossSKUMigration)
			{
				for (int i = 0; i < servicePlanFrom.MailboxPlans.Count; i++)
				{
					ServicePlan.MailboxPlan mailboxPlan2 = servicePlanFrom.MailboxPlans[i];
					ServicePlan.MailboxPlan mailboxPlanByName = servicePlanTo.GetMailboxPlanByName(mailboxPlan2.Name);
					if (!flag4 || mailboxPlanByName != null || !mailboxPlan2.IsPilotMailboxPlan)
					{
						foreach (object obj2 in ((IEnumerable)mailboxPlan2.Schema))
						{
							FeatureDefinition featureDefinition2 = (FeatureDefinition)obj2;
							if (flag || !featureDefinition2.IsValueEqual(mailboxPlan2[featureDefinition2], mailboxPlanByName[featureDefinition2]))
							{
								result = false;
								ServicePlan.MailboxPlan mailboxPlan3 = servicePlanDelta.GetMailboxPlanByName(mailboxPlanByName.Name);
								if (mailboxPlan3 == null)
								{
									mailboxPlan3 = new ServicePlan.MailboxPlan();
									mailboxPlan3.Name = mailboxPlanByName.Name;
									mailboxPlan3.MailboxPlanIndex = mailboxPlanByName.MailboxPlanIndex;
									servicePlanDelta.MailboxPlans.Add(mailboxPlan3);
								}
								mailboxPlan3[featureDefinition2] = mailboxPlanByName[featureDefinition2];
								if (!featuresToApply.Contains(featureDefinition2.Name))
								{
									featuresToApply.Add(featureDefinition2.Name);
								}
							}
						}
					}
				}
			}
			else
			{
				foreach (object obj3 in ((IEnumerable)new MailboxPlanSchema()))
				{
					FeatureDefinition featureDefinition3 = (FeatureDefinition)obj3;
					if (!featuresToApply.Contains(featureDefinition3.Name))
					{
						featuresToApply.Add(featureDefinition3.Name);
					}
				}
			}
			return result;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(string.Format("\r\n\tName={0}, IsValid={1}", this.Name, this.IsValid));
			stringBuilder.Append("\r\n\tOrganization Settings:");
			foreach (object obj in ((IEnumerable)this.Organization.Schema))
			{
				FeatureDefinition featureDefinition = (FeatureDefinition)obj;
				stringBuilder.Append(string.Format("\r\n\t\t{0}={1}", featureDefinition.Name, this.Organization[featureDefinition]));
			}
			foreach (ServicePlan.MailboxPlan mailboxPlan in this.MailboxPlans)
			{
				stringBuilder.Append(string.Format("\r\n\tMailboxPlan {0}:", mailboxPlan.Name));
				foreach (object obj2 in ((IEnumerable)mailboxPlan.Schema))
				{
					FeatureDefinition featureDefinition2 = (FeatureDefinition)obj2;
					stringBuilder.Append(string.Format("\r\n\t\t{0}={1}", featureDefinition2.Name, mailboxPlan[featureDefinition2]));
				}
				Microsoft.Exchange.Data.Directory.Management.MailboxPlan mailboxPlan2 = mailboxPlan.Instance as Microsoft.Exchange.Data.Directory.Management.MailboxPlan;
				if (mailboxPlan2 != null)
				{
					stringBuilder.Append(string.Format("\r\n\t\tInstance={0}", mailboxPlan2.Identity));
				}
				else
				{
					stringBuilder.Append(string.Format("\r\n\t\tInstance={0}", mailboxPlan.Instance));
				}
			}
			return stringBuilder.ToString();
		}

		internal List<string> GetAggregatedMailboxPlanPermissions()
		{
			List<string> list = new List<string>();
			foreach (ServicePlan.MailboxPlan mailboxPlan in this.MailboxPlans)
			{
				list.AddRange(mailboxPlan.GetEnabledPermissionFeatures());
			}
			return list;
		}

		internal List<string> GetAggregatedMailboxPlanRoleAssignmentFeatures()
		{
			List<string> list = new List<string>();
			foreach (ServicePlan.MailboxPlan mailboxPlan in this.MailboxPlans)
			{
				list.AddRange(mailboxPlan.GetEnabledMailboxPlanRoleAssignmentFeatures());
			}
			return list;
		}

		private string name;

		private List<ServicePlan.MailboxPlan> mailboxPlans;

		private ServicePlan.OrganizationSettings organization;

		private string version;

		public sealed class OrganizationSettings : BooleanFeatureBag
		{
			internal override ServicePlanElementSchema Schema
			{
				get
				{
					return ServicePlan.OrganizationSettings.schema;
				}
			}

			internal OrganizationSettings()
			{
			}

			private bool GetUnifiedRoleAssignmentPolicyModeValue(ServicePlan sp, FeatureDefinition feature)
			{
				foreach (ServicePlan.MailboxPlan mailboxPlan in sp.MailboxPlans)
				{
					if ((bool)mailboxPlan[feature])
					{
						return true;
					}
				}
				return false;
			}

			private void SetUnifiedRoleAssignmentPolicyModeValue(ServicePlan sp, FeatureDefinition feature, bool value)
			{
				sp.MailboxPlans[0][feature] = value;
				for (int i = 1; i < sp.MailboxPlans.Count; i++)
				{
					sp.MailboxPlans[i][feature] = !value;
				}
			}

			protected override void InitializeDependencies()
			{
				base.Dependencies.Add(new DependencyEntry("CommonConfiguration", "MailboxPlans", () => true, (ServicePlan sp) => sp.MailboxPlans.Count > 0, delegate(ServicePlan sp, bool value)
				{
					if (sp.MailboxPlans.Count == 0)
					{
						sp.MailboxPlans.Add(new ServicePlan.MailboxPlan());
						sp.MailboxPlans[0].Name = "DefaultMailboxPlan";
						sp.MailboxPlans[0].MailboxPlanIndex = "0";
					}
				}));
				base.Dependencies.Add(new DependencyEntry("CommonConfiguration", "ProvisionAsDefault", () => true, delegate(ServicePlan sp)
				{
					int num = 0;
					foreach (ServicePlan.MailboxPlan mailboxPlan in sp.MailboxPlans)
					{
						if (mailboxPlan.ProvisionAsDefault)
						{
							num++;
						}
					}
					return num == 1;
				}, delegate(ServicePlan sp, bool value)
				{
					for (int i = 0; i < sp.MailboxPlans.Count; i++)
					{
						sp.MailboxPlans[i].ProvisionAsDefault = (i == 0);
					}
				}));
				base.Dependencies.Add(new DependencyEntry("SkuCapability", "MailboxPlans", () => true, delegate(ServicePlan sp)
				{
					int mbxPlanWithCapability = 0;
					sp.MailboxPlans.ForEach(delegate(ServicePlan.MailboxPlan x)
					{
						mbxPlanWithCapability = ((x.SkuCapability != Capability.None) ? (mbxPlanWithCapability + 1) : mbxPlanWithCapability);
					});
					return mbxPlanWithCapability == 0 || (mbxPlanWithCapability != 0 && mbxPlanWithCapability == sp.MailboxPlans.Count);
				}, delegate(ServicePlan sp, bool value)
				{
					sp.MailboxPlans.ForEach(delegate(ServicePlan.MailboxPlan x)
					{
						x.SkuCapability = Capability.None;
					});
				}));
				base.Dependencies.Add(new DependencyEntry("ApplicationImpersonationEnabled", "PrivacyFeaturesAllowed", () => this.ApplicationImpersonationEnabled, (ServicePlan sp) => sp.organization.ApplicationImpersonationEnabled == this.PrivacyFeaturesAllowed, delegate(ServicePlan sp, bool value)
				{
					sp.organization.PrivacyFeaturesAllowed = sp.organization.ApplicationImpersonationEnabled;
				}));
				base.Dependencies.Add(new DependencyEntry("ResetUserPasswordManagementPermissions", "PrivacyFeaturesAllowed", () => this.ResetUserPasswordManagementPermissions, (ServicePlan sp) => sp.organization.ResetUserPasswordManagementPermissions == sp.organization.PrivacyFeaturesAllowed, delegate(ServicePlan sp, bool value)
				{
					sp.organization.PrivacyFeaturesAllowed = sp.organization.ResetUserPasswordManagementPermissions;
				}));
				base.Dependencies.Add(new DependencyEntry("ResetUserPasswordManagementPermissions", "SkipResetPasswordOnFirstLogonEnabled", () => this.ResetUserPasswordManagementPermissions, delegate(ServicePlan sp)
				{
					foreach (ServicePlan.MailboxPlan mailboxPlan in sp.MailboxPlans)
					{
						if (!mailboxPlan.SkipResetPasswordOnFirstLogonEnabled)
						{
							return false;
						}
					}
					return true;
				}, delegate(ServicePlan sp, bool value)
				{
					foreach (ServicePlan.MailboxPlan mailboxPlan in sp.MailboxPlans)
					{
						mailboxPlan.SkipResetPasswordOnFirstLogonEnabled = value;
					}
				}));
				base.Dependencies.Add(new DependencyEntry("UMPBXPermissions", "UMPermissions", () => this.UMPBXPermissions, (ServicePlan sp) => sp.Organization.UMPermissions, delegate(ServicePlan sp, bool value)
				{
					sp.Organization.UMPermissions = value;
				}));
				base.Dependencies.Add(new DependencyEntry("UMAutoAttendantPermissions", "UMPermissions", () => this.UMAutoAttendantPermissions, (ServicePlan sp) => sp.Organization.UMPermissions, delegate(ServicePlan sp, bool value)
				{
					sp.Organization.UMPermissions = value;
				}));
				base.Dependencies.Add(new DependencyEntry("UMSMSMsgWaitingPermissions", "UMPermissions", () => this.UMSMSMsgWaitingPermissions, (ServicePlan sp) => sp.Organization.UMPermissions, delegate(ServicePlan sp, bool value)
				{
					sp.Organization.UMPermissions = value;
				}));
				base.Dependencies.Add(new DependencyEntry("UMCloudServicePermissions", "UMPermissions", () => this.UMCloudServicePermissions, (ServicePlan sp) => sp.Organization.UMPermissions, delegate(ServicePlan sp, bool value)
				{
					sp.Organization.UMPermissions = value;
				}));
				base.Dependencies.Add(new DependencyEntry("OpenDomainProfileUpdatePermissions", "!ProfileUpdatePermissions", () => this.OpenDomainProfileUpdatePermissions, (ServicePlan sp) => sp.Organization.OpenDomainProfileUpdatePermissions != sp.Organization.ProfileUpdatePermissions, delegate(ServicePlan sp, bool value)
				{
					sp.Organization.ProfileUpdatePermissions = !sp.Organization.OpenDomainProfileUpdatePermissions;
				}));
				base.Dependencies.Add(new DependencyEntry("!PerMBXPlanOWAPolicyEnabled", "OWAPermissions", () => !this.PerMBXPlanOWAPolicyEnabled, (ServicePlan sp) => sp.Organization.OWAPermissions, delegate(ServicePlan sp, bool value)
				{
					sp.Organization.OWAPermissions = value;
				}));
				base.Dependencies.Add(new DependencyEntry("OWAMailboxPolicyPermissions", "!PerMBXPlanOWAPolicyEnabled", () => this.OWAMailboxPolicyPermissions, (ServicePlan sp) => !sp.Organization.PerMBXPlanOWAPolicyEnabled, delegate(ServicePlan sp, bool value)
				{
					sp.Organization.PerMBXPlanOWAPolicyEnabled = !value;
				}));
				base.Dependencies.Add(new DependencyEntry("OfflineAddressBookEnabled", "OutlookAnywhereEnabled", () => this.OfflineAddressBookEnabled, delegate(ServicePlan sp)
				{
					foreach (ServicePlan.MailboxPlan mailboxPlan in sp.MailboxPlans)
					{
						if (mailboxPlan.OutlookAnywhereEnabled)
						{
							return true;
						}
					}
					return false;
				}, delegate(ServicePlan sp, bool value)
				{
					foreach (ServicePlan.MailboxPlan mailboxPlan in sp.MailboxPlans)
					{
						mailboxPlan.OutlookAnywhereEnabled = value;
					}
				}));
				base.Dependencies.Add(new DependencyEntry("SearchMessagePermissions", "PrivacyFeaturesAllowed", () => this.SearchMessagePermissions, (ServicePlan sp) => this.PrivacyFeaturesAllowed, delegate(ServicePlan sp, bool value)
				{
					sp.Organization.PrivacyFeaturesAllowed = sp.Organization.SearchMessagePermissions;
				}));
				base.Dependencies.Add(new DependencyEntry("SearchMessageEnabled", "SearchMessagePermissions", () => this.SearchMessageEnabled, (ServicePlan sp) => sp.organization.SearchMessagePermissions, delegate(ServicePlan sp, bool value)
				{
					sp.organization.SearchMessagePermissions = value;
				}));
				base.Dependencies.Add(new DependencyEntry("TransportRulesPermissions", "PrivacyFeaturesAllowed", () => this.TransportRulesPermissions, (ServicePlan sp) => this.PrivacyFeaturesAllowed, delegate(ServicePlan sp, bool value)
				{
					sp.Organization.PrivacyFeaturesAllowed = sp.Organization.TransportRulesPermissions;
				}));
				base.Dependencies.Add(new DependencyEntry("UserMailboxAccessPermissions", "PrivacyFeaturesAllowed", () => this.UserMailboxAccessPermissions, (ServicePlan sp) => this.PrivacyFeaturesAllowed, delegate(ServicePlan sp, bool value)
				{
					sp.Organization.PrivacyFeaturesAllowed = sp.Organization.UserMailboxAccessPermissions;
				}));
				base.Dependencies.Add(new DependencyEntry("MailboxRecoveryPermissions", "PrivacyFeaturesAllowed", () => this.MailboxRecoveryPermissions, (ServicePlan sp) => this.PrivacyFeaturesAllowed, delegate(ServicePlan sp, bool value)
				{
					sp.Organization.PrivacyFeaturesAllowed = sp.Organization.MailboxRecoveryPermissions;
				}));
				base.Dependencies.Add(new DependencyEntry("JournalingRulesPermissions", "PrivacyFeaturesAllowed", () => this.JournalingRulesPermissions, (ServicePlan sp) => this.PrivacyFeaturesAllowed, delegate(ServicePlan sp, bool value)
				{
					sp.Organization.PrivacyFeaturesAllowed = sp.Organization.JournalingRulesPermissions;
				}));
				base.Dependencies.Add(new DependencyEntry("MessageTrackingPermissions", "PrivacyFeaturesAllowed", () => this.MessageTrackingPermissions, (ServicePlan sp) => this.PrivacyFeaturesAllowed, delegate(ServicePlan sp, bool value)
				{
					sp.Organization.PrivacyFeaturesAllowed = sp.Organization.MessageTrackingPermissions;
				}));
				base.Dependencies.Add(new DependencyEntry("ActiveSyncDeviceDataAccessPermissions", "PrivacyFeaturesAllowed", () => this.ActiveSyncDeviceDataAccessPermissions, (ServicePlan sp) => this.PrivacyFeaturesAllowed, delegate(ServicePlan sp, bool value)
				{
					sp.Organization.PrivacyFeaturesAllowed = sp.Organization.ActiveSyncDeviceDataAccessPermissions;
				}));
				base.Dependencies.Add(new DependencyEntry("OutlookAnywherePermissions", "OutlookAnywhereEnabled", () => this.OutlookAnywherePermissions, delegate(ServicePlan sp)
				{
					foreach (ServicePlan.MailboxPlan mailboxPlan in sp.MailboxPlans)
					{
						if (mailboxPlan.OutlookAnywhereEnabled)
						{
							return true;
						}
					}
					return false;
				}, delegate(ServicePlan sp, bool value)
				{
					foreach (ServicePlan.MailboxPlan mailboxPlan in sp.MailboxPlans)
					{
						mailboxPlan.OutlookAnywhereEnabled = value;
					}
				}));
				base.Dependencies.Add(new DependencyEntry("UMOutDialingPermissions", "UMPermissions", () => this.UMOutDialingPermissions, (ServicePlan sp) => sp.organization.UMPermissions, delegate(ServicePlan sp, bool value)
				{
					sp.organization.UMPermissions = value;
				}));
				base.Dependencies.Add(new DependencyEntry("UMPersonalAutoAttendantPermissions", "UMPermissions", () => this.UMPersonalAutoAttendantPermissions, (ServicePlan sp) => sp.organization.UMPermissions, delegate(ServicePlan sp, bool value)
				{
					sp.organization.UMPermissions = value;
				}));
				base.Dependencies.Add(new DependencyEntry("UMFaxPermissions", "UMPermissions", () => this.UMFaxPermissions, (ServicePlan sp) => sp.organization.UMPermissions, delegate(ServicePlan sp, bool value)
				{
					sp.organization.UMPermissions = value;
				}));
				base.Dependencies.Add(new DependencyEntry("UMSMSMsgWaitingPermissions", "UMPermissions", () => this.UMSMSMsgWaitingPermissions, (ServicePlan sp) => sp.organization.UMPermissions, delegate(ServicePlan sp, bool value)
				{
					sp.organization.UMPermissions = value;
				}));
				base.Dependencies.Add(new DependencyEntry("SupervisionPermissions", "SupervisionEnabled", () => this.SupervisionPermissions, (ServicePlan sp) => sp.organization.SupervisionEnabled, delegate(ServicePlan sp, bool value)
				{
					sp.organization.SupervisionEnabled = value;
				}));
				base.Dependencies.Add(new DependencyEntry("SupervisionEnabled", "SupervisionPermissions", () => this.SupervisionEnabled, (ServicePlan sp) => sp.Organization.SupervisionPermissions, delegate(ServicePlan sp, bool value)
				{
					sp.Organization.SupervisionPermissions = value;
				}));
				base.Dependencies.Add(new DependencyEntry("SupervisionEnabled", "ViewSupervisionListPermissions", () => this.SupervisionEnabled, delegate(ServicePlan sp)
				{
					if (this.PerMBXPlanRoleAssignmentPolicyEnabled)
					{
						foreach (ServicePlan.MailboxPlan mailboxPlan in sp.MailboxPlans)
						{
							if (!mailboxPlan.ViewSupervisionListPermissions)
							{
								return false;
							}
						}
						return true;
					}
					return this.GetUnifiedRoleAssignmentPolicyModeValue(sp, MailboxPlanSchema.ViewSupervisionListPermissions);
				}, delegate(ServicePlan sp, bool value)
				{
					if (this.PerMBXPlanRoleAssignmentPolicyEnabled)
					{
						using (List<ServicePlan.MailboxPlan>.Enumerator enumerator = sp.MailboxPlans.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								ServicePlan.MailboxPlan mailboxPlan = enumerator.Current;
								mailboxPlan.ViewSupervisionListPermissions = value;
							}
							return;
						}
					}
					this.SetUnifiedRoleAssignmentPolicyModeValue(sp, MailboxPlanSchema.ViewSupervisionListPermissions, value);
				}));
				base.Dependencies.Add(new DependencyEntry("RecipientMailSubmissionRateQuota", "MailboxPlans", () => true, (ServicePlan sp) => this.RecipientMailSubmissionRateQuota != null, delegate(ServicePlan sp, bool value)
				{
					this.RecipientMailSubmissionRateQuota = Unlimited<int>.UnlimitedString;
				}));
				base.Dependencies.Add(new DependencyEntry("MailTipsPermissions", "MailTipsEnabled", () => this.MailTipsPermissions, (ServicePlan sp) => sp.organization.MailTipsEnabled, delegate(ServicePlan sp, bool value)
				{
					sp.organization.MailTipsEnabled = value;
				}));
				base.Dependencies.Add(new DependencyEntry("DistributionListCountQuota", "MailboxPlans", () => true, (ServicePlan sp) => sp.organization.DistributionListCountQuota != null, delegate(ServicePlan sp, bool value)
				{
					sp.organization.DistributionListCountQuota = "30";
				}));
				base.Dependencies.Add(new DependencyEntry("MailboxCountQuota", "MailboxPlans", () => true, (ServicePlan sp) => sp.organization.MailboxCountQuota != null, delegate(ServicePlan sp, bool value)
				{
					sp.organization.MailboxCountQuota = "60";
				}));
				base.Dependencies.Add(new DependencyEntry("MailUserCountQuota", "MailboxPlans", () => true, (ServicePlan sp) => sp.organization.MailUserCountQuota != null, delegate(ServicePlan sp, bool value)
				{
					sp.organization.MailUserCountQuota = "30";
				}));
				base.Dependencies.Add(new DependencyEntry("ContactCountQuota", "MailboxPlans", () => true, (ServicePlan sp) => sp.organization.ContactCountQuota != null, delegate(ServicePlan sp, bool value)
				{
					sp.organization.ContactCountQuota = "60";
				}));
				base.Dependencies.Add(new DependencyEntry("ImapMigrationPermissions", "ImapSyncPermissions", () => this.ImapMigrationPermissions, (ServicePlan sp) => sp.Organization.ImapSyncPermissions, delegate(ServicePlan sp, bool value)
				{
					sp.Organization.ImapSyncPermissions = value;
				}));
				base.Dependencies.Add(new DependencyEntry("HotmailMigrationPermissions", "HotmailSyncPermissions", () => this.HotmailMigrationPermissions, (ServicePlan sp) => sp.Organization.HotmailSyncPermissions, delegate(ServicePlan sp, bool value)
				{
					sp.Organization.HotmailSyncPermissions = value;
				}));
				base.Dependencies.Add(new DependencyEntry("GroupAsGroupSyncPermissions", "GALSyncEnabled", () => this.GroupAsGroupSyncPermissions, (ServicePlan sp) => sp.Organization.GALSyncEnabled, delegate(ServicePlan sp, bool value)
				{
					sp.Organization.GALSyncEnabled = value;
				}));
				base.Dependencies.Add(new DependencyEntry("GALSyncEnabled", "!ShareableConfigurationEnabled", () => this.GALSyncEnabled, (ServicePlan sp) => !sp.Organization.ShareableConfigurationEnabled, delegate(ServicePlan sp, bool value)
				{
					sp.Organization.ShareableConfigurationEnabled = !value;
				}));
				base.Dependencies.Add(new DependencyEntry("GALSyncEnabled", "!AdvancedHydrateableObjectsSharedEnabled", () => this.GALSyncEnabled, (ServicePlan sp) => !sp.Organization.AdvancedHydrateableObjectsSharedEnabled, delegate(ServicePlan sp, bool value)
				{
					sp.Organization.AdvancedHydrateableObjectsSharedEnabled = false;
				}));
				base.Dependencies.Add(new DependencyEntry("AdvancedHydrateableObjectsSharedEnabled", "ShareableConfigurationEnabled", () => this.AdvancedHydrateableObjectsSharedEnabled, (ServicePlan sp) => sp.Organization.ShareableConfigurationEnabled, delegate(ServicePlan sp, bool value)
				{
					sp.Organization.ShareableConfigurationEnabled = value;
				}));
				base.Dependencies.Add(new DependencyEntry("CommonHydrateableObjectsSharedEnabled", "ShareableConfigurationEnabled", () => this.CommonHydrateableObjectsSharedEnabled, (ServicePlan sp) => sp.Organization.ShareableConfigurationEnabled, delegate(ServicePlan sp, bool value)
				{
					sp.Organization.ShareableConfigurationEnabled = value;
				}));
				base.Dependencies.Add(new DependencyEntry("CommonHydrateableObjectsSharedEnabled", "AdvancedHydrateableObjectsSharedEnabled", () => this.CommonHydrateableObjectsSharedEnabled, (ServicePlan sp) => sp.Organization.AdvancedHydrateableObjectsSharedEnabled, delegate(ServicePlan sp, bool value)
				{
					sp.Organization.AdvancedHydrateableObjectsSharedEnabled = value;
				}));
				base.Dependencies.Add(new DependencyEntry("PerMBXPlanOWAPolicyEnabled", "!CommonHydrateableObjectsSharedEnabled", () => this.PerMBXPlanOWAPolicyEnabled, (ServicePlan sp) => !sp.Organization.CommonHydrateableObjectsSharedEnabled, delegate(ServicePlan sp, bool value)
				{
					sp.Organization.CommonHydrateableObjectsSharedEnabled = false;
				}));
				base.Dependencies.Add(new DependencyEntry("RoleAssignmentPolicyPermissions", "!PerMBXPlanRoleAssignmentPolicyEnabled", () => this.RoleAssignmentPolicyPermissions, (ServicePlan sp) => !sp.Organization.PerMBXPlanRoleAssignmentPolicyEnabled, delegate(ServicePlan sp, bool value)
				{
					sp.Organization.PerMBXPlanRoleAssignmentPolicyEnabled = !value;
				}));
				base.Dependencies.Add(new DependencyEntry("!PerMBXPlanRoleAssignmentPolicyEnabled", "MailboxPlans", () => !this.PerMBXPlanRoleAssignmentPolicyEnabled, delegate(ServicePlan sp)
				{
					if (sp.MailboxPlans.Count == 0)
					{
						return true;
					}
					int num = 0;
					foreach (ServicePlan.MailboxPlan mailboxPlan in sp.MailboxPlans)
					{
						List<string> enabledMailboxPlanPermissionsFeatures = mailboxPlan.GetEnabledMailboxPlanPermissionsFeatures();
						enabledMailboxPlanPermissionsFeatures.Remove("*");
						if (enabledMailboxPlanPermissionsFeatures.Count > 0)
						{
							num++;
						}
					}
					return num <= 1;
				}, delegate(ServicePlan sp, bool value)
				{
					if (sp.MailboxPlans.Count == 0)
					{
						return;
					}
					List<string> aggregatedMailboxPlanPermissions = sp.GetAggregatedMailboxPlanPermissions();
					ServicePlan.MailboxPlan mailboxPlan = sp.MailboxPlans[0];
					foreach (object obj in ((IEnumerable)mailboxPlan.Schema))
					{
						FeatureDefinition featureDefinition = (FeatureDefinition)obj;
						if (aggregatedMailboxPlanPermissions.Contains(featureDefinition.Name, StringComparer.OrdinalIgnoreCase))
						{
							mailboxPlan[featureDefinition] = value;
							for (int i = 1; i < sp.MailboxPlans.Count; i++)
							{
								sp.MailboxPlans[i][featureDefinition] = !value;
							}
						}
					}
				}));
				base.Dependencies.Add(new DependencyEntry("PermissionManagementEnabled", "RBACManagementPermissions", () => this.PermissionManagementEnabled, (ServicePlan sp) => sp.Organization.RBACManagementPermissions == sp.Organization.PermissionManagementEnabled, delegate(ServicePlan sp, bool value)
				{
					sp.Organization.RBACManagementPermissions = sp.Organization.PermissionManagementEnabled;
				}));
				base.Dependencies.Add(new DependencyEntry("SetHiddenFromAddressListPermissions", "ShowInAddressListsEnabled", () => this.SetHiddenFromAddressListPermissions, delegate(ServicePlan sp)
				{
					foreach (ServicePlan.MailboxPlan mailboxPlan in sp.MailboxPlans)
					{
						if (!mailboxPlan.ShowInAddressListsEnabled)
						{
							return false;
						}
					}
					return true;
				}, delegate(ServicePlan sp, bool value)
				{
					foreach (ServicePlan.MailboxPlan mailboxPlan in sp.MailboxPlans)
					{
						mailboxPlan.ShowInAddressListsEnabled = value;
					}
				}));
				base.Dependencies.Add(new DependencyEntry("PerimeterSafelistingUIMode", "ExchangeHostedFilteringAdminCenterAvailabilityEnabled", () => this.PerimeterSafelistingUIMode == "EhfAC", (ServicePlan ac) => ac.Organization.ExchangeHostedFilteringAdminCenterAvailabilityEnabled, delegate(ServicePlan ac, bool value)
				{
					ac.Organization.ExchangeHostedFilteringAdminCenterAvailabilityEnabled = value;
				}));
				base.Dependencies.Add(new DependencyEntry("ContactCountQuota", "!FastRecipientCountingDisabled", delegate()
				{
					Unlimited<int> contactCountQuota = this.GetContactCountQuota();
					return contactCountQuota.IsUnlimited || contactCountQuota.Value >= 1001;
				}, (ServicePlan ac) => !ac.Organization.FastRecipientCountingDisabled, delegate(ServicePlan ac, bool value)
				{
					ac.Organization.FastRecipientCountingDisabled = false;
				}));
				base.Dependencies.Add(new DependencyEntry("DistributionListCountQuota", "!FastRecipientCountingDisabled", delegate()
				{
					Unlimited<int> distributionListCountQuota = this.GetDistributionListCountQuota();
					return distributionListCountQuota.IsUnlimited || distributionListCountQuota.Value >= 1001;
				}, (ServicePlan ac) => !ac.Organization.FastRecipientCountingDisabled, delegate(ServicePlan ac, bool value)
				{
					ac.Organization.FastRecipientCountingDisabled = false;
				}));
				base.Dependencies.Add(new DependencyEntry("MailboxCountQuota", "!FastRecipientCountingDisabled", delegate()
				{
					Unlimited<int> mailboxCountQuota = this.GetMailboxCountQuota();
					return mailboxCountQuota.IsUnlimited || mailboxCountQuota.Value >= 1001;
				}, (ServicePlan ac) => !ac.Organization.FastRecipientCountingDisabled, delegate(ServicePlan ac, bool value)
				{
					ac.Organization.FastRecipientCountingDisabled = false;
				}));
				base.Dependencies.Add(new DependencyEntry("MailUserCountQuota", "!FastRecipientCountingDisabled", delegate()
				{
					Unlimited<int> mailUserCountQuota = this.GetMailUserCountQuota();
					return mailUserCountQuota.IsUnlimited || mailUserCountQuota.Value >= 1001;
				}, (ServicePlan ac) => !ac.Organization.FastRecipientCountingDisabled, delegate(ServicePlan ac, bool value)
				{
					ac.Organization.FastRecipientCountingDisabled = false;
				}));
				base.Dependencies.Add(new DependencyEntry("TeamMailboxCountQuota", "TeamMailboxPermissions", () => 0 != this.GetTeamMailboxCountQuota(), (ServicePlan ac) => ac.Organization.TeamMailboxPermissions, delegate(ServicePlan ac, bool value)
				{
					ac.Organization.TeamMailboxPermissions = true;
				}));
				base.Dependencies.Add(new DependencyEntry("TeamMailboxCountQuota", "!FastRecipientCountingDisabled", delegate()
				{
					Unlimited<int> teamMailboxCountQuota = this.GetTeamMailboxCountQuota();
					return teamMailboxCountQuota.IsUnlimited || teamMailboxCountQuota.Value >= 1001;
				}, (ServicePlan ac) => !ac.Organization.FastRecipientCountingDisabled, delegate(ServicePlan ac, bool value)
				{
					ac.Organization.FastRecipientCountingDisabled = false;
				}));
				base.Dependencies.Add(new DependencyEntry("PublicFolderMailboxCountQuota", "!FastRecipientCountingDisabled", delegate()
				{
					Unlimited<int> publicFolderMailboxCountQuota = this.GetPublicFolderMailboxCountQuota();
					return publicFolderMailboxCountQuota.IsUnlimited || publicFolderMailboxCountQuota.Value >= 1001;
				}, (ServicePlan ac) => !ac.Organization.FastRecipientCountingDisabled, delegate(ServicePlan ac, bool value)
				{
					ac.Organization.FastRecipientCountingDisabled = false;
				}));
				base.Dependencies.Add(new DependencyEntry("MailPublicFolderCountQuota", "!FastRecipientCountingDisabled", delegate()
				{
					Unlimited<int> mailPublicFolderCountQuota = this.GetMailPublicFolderCountQuota();
					return mailPublicFolderCountQuota.IsUnlimited || mailPublicFolderCountQuota.Value >= 1001;
				}, (ServicePlan ac) => !ac.Organization.FastRecipientCountingDisabled, delegate(ServicePlan ac, bool value)
				{
					ac.Organization.FastRecipientCountingDisabled = false;
				}));
				base.Dependencies.Add(new DependencyEntry("SupervisionEnabled", "TransportRulesCollectionsEnabled", () => Datacenter.IsMicrosoftHostedOnly(true) && this.SupervisionEnabled, (ServicePlan ac) => ac.Organization.TransportRulesCollectionsEnabled, delegate(ServicePlan ac, bool value)
				{
					ac.Organization.TransportRulesCollectionsEnabled = value;
				}));
				base.Dependencies.Add(new DependencyEntry("TransportRulesPermissions", "TransportRulesCollectionsEnabled", () => Datacenter.IsMicrosoftHostedOnly(true) && this.TransportRulesPermissions, (ServicePlan ac) => ac.Organization.TransportRulesCollectionsEnabled, delegate(ServicePlan ac, bool value)
				{
					ac.Organization.TransportRulesCollectionsEnabled = value;
				}));
			}

			public bool AcceptedDomains
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.AcceptedDomains];
				}
				set
				{
					base[OrganizationSettingsSchema.AcceptedDomains] = value;
				}
			}

			public bool AddressListsEnabled
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.AddressListsEnabled];
				}
				set
				{
					base[OrganizationSettingsSchema.AddressListsEnabled] = value;
				}
			}

			public bool AdvancedHydrateableObjectsSharedEnabled
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.AdvancedHydrateableObjectsSharedEnabled];
				}
				set
				{
					base[OrganizationSettingsSchema.AdvancedHydrateableObjectsSharedEnabled] = value;
				}
			}

			public bool AllowDeleteOfExternalIdentityUponRemove
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.AllowDeleteOfExternalIdentityUponRemove];
				}
				set
				{
					base[OrganizationSettingsSchema.AllowDeleteOfExternalIdentityUponRemove] = value;
				}
			}

			public bool ApplicationImpersonationEnabled
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.ApplicationImpersonationEnabled];
				}
				set
				{
					base[OrganizationSettingsSchema.ApplicationImpersonationEnabled] = value;
				}
			}

			public bool ApplicationImpersonationRegularRoleAssignmentEnabled
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.ApplicationImpersonationRegularRoleAssignmentEnabled];
				}
				set
				{
					base[OrganizationSettingsSchema.ApplicationImpersonationRegularRoleAssignmentEnabled] = value;
				}
			}

			public bool AutoForwardEnabled
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.AutoForwardEnabled];
				}
				set
				{
					base[OrganizationSettingsSchema.AutoForwardEnabled] = value;
				}
			}

			public bool AutoReplyEnabled
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.AutoReplyEnabled];
				}
				set
				{
					base[OrganizationSettingsSchema.AutoReplyEnabled] = value;
				}
			}

			public bool CalendarVersionStoreEnabled
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.CalendarVersionStoreEnabled];
				}
				set
				{
					base[OrganizationSettingsSchema.CalendarVersionStoreEnabled] = value;
				}
			}

			public bool CommonConfiguration
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.CommonConfiguration];
				}
				set
				{
					base[OrganizationSettingsSchema.CommonConfiguration] = value;
				}
			}

			public bool CommonHydrateableObjectsSharedEnabled
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.CommonHydrateableObjectsSharedEnabled];
				}
				set
				{
					base[OrganizationSettingsSchema.CommonHydrateableObjectsSharedEnabled] = value;
				}
			}

			public bool DataLossPreventionEnabled
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.DataLossPreventionEnabled];
				}
				set
				{
					base[OrganizationSettingsSchema.DataLossPreventionEnabled] = value;
				}
			}

			public bool DeviceFiltersSetupEnabled
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.DeviceFiltersSetupEnabled];
				}
				set
				{
					base[OrganizationSettingsSchema.DeviceFiltersSetupEnabled] = value;
				}
			}

			public bool ExchangeHostedFilteringAdminCenterAvailabilityEnabled
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.ExchangeHostedFilteringAdminCenterAvailabilityEnabled];
				}
				set
				{
					base[OrganizationSettingsSchema.ExchangeHostedFilteringAdminCenterAvailabilityEnabled] = value;
				}
			}

			public bool ExchangeHostedFilteringPerimeterEnabled
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.ExchangeHostedFilteringPerimeterEnabled];
				}
				set
				{
					base[OrganizationSettingsSchema.ExchangeHostedFilteringPerimeterEnabled] = value;
				}
			}

			public bool EXOCoreFeatures
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.EXOCoreFeatures];
				}
				set
				{
					base[OrganizationSettingsSchema.EXOCoreFeatures] = value;
				}
			}

			public bool FastRecipientCountingDisabled
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.FastRecipientCountingDisabled];
				}
				set
				{
					base[OrganizationSettingsSchema.FastRecipientCountingDisabled] = value;
				}
			}

			public bool GALSyncEnabled
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.GALSyncEnabled];
				}
				set
				{
					base[OrganizationSettingsSchema.GALSyncEnabled] = value;
				}
			}

			public bool HideAdminAccessWarningEnabled
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.HideAdminAccessWarningEnabled];
				}
				set
				{
					base[OrganizationSettingsSchema.HideAdminAccessWarningEnabled] = value;
				}
			}

			public bool HostedSpamFilteringPolicyCustomizationEnabled
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.HostedSpamFilteringPolicyCustomizationEnabled];
				}
				set
				{
					base[OrganizationSettingsSchema.HostedSpamFilteringPolicyCustomizationEnabled] = value;
				}
			}

			public bool MSOSyncEnabled
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.MSOSyncEnabled];
				}
				set
				{
					base[OrganizationSettingsSchema.MSOSyncEnabled] = value;
				}
			}

			public bool LicenseEnforcementEnabled
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.LicenseEnforcementEnabled];
				}
				set
				{
					base[OrganizationSettingsSchema.LicenseEnforcementEnabled] = value;
				}
			}

			public bool MailboxImportExportRegularRoleAssignmentEnabled
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.MailboxImportExportRegularRoleAssignmentEnabled];
				}
				set
				{
					base[OrganizationSettingsSchema.MailboxImportExportRegularRoleAssignmentEnabled] = value;
				}
			}

			public bool MailTipsEnabled
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.MailTipsEnabled];
				}
				set
				{
					base[OrganizationSettingsSchema.MailTipsEnabled] = value;
				}
			}

			public bool MalwareFilteringPolicyCustomizationEnabled
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.MalwareFilteringPolicyCustomizationEnabled];
				}
				set
				{
					base[OrganizationSettingsSchema.MalwareFilteringPolicyCustomizationEnabled] = value;
				}
			}

			public bool MessageTrace
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.MessageTrace];
				}
				set
				{
					base[OrganizationSettingsSchema.MessageTrace] = value;
				}
			}

			public bool MultiEngineAVEnabled
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.MultiEngineAVEnabled];
				}
				set
				{
					base[OrganizationSettingsSchema.MultiEngineAVEnabled] = value;
				}
			}

			public bool OfflineAddressBookEnabled
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.OfflineAddressBookEnabled];
				}
				set
				{
					base[OrganizationSettingsSchema.OfflineAddressBookEnabled] = value;
				}
			}

			public bool OpenDomainRoutingEnabled
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.OpenDomainRoutingEnabled];
				}
				set
				{
					base[OrganizationSettingsSchema.OpenDomainRoutingEnabled] = value;
				}
			}

			public bool AddOutlookAcceptedDomains
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.AddOutlookAcceptedDomains];
				}
				set
				{
					base[OrganizationSettingsSchema.AddOutlookAcceptedDomains] = value;
				}
			}

			public string OwaInstantMessagingType
			{
				get
				{
					return (string)base[OrganizationSettingsSchema.OwaInstantMessagingType];
				}
				set
				{
					base[OrganizationSettingsSchema.OwaInstantMessagingType] = value;
				}
			}

			public string PerimeterSafelistingUIMode
			{
				get
				{
					object obj = base[OrganizationSettingsSchema.PerimeterSafelistingUIMode];
					if (obj != null)
					{
						return obj.ToString();
					}
					return null;
				}
				set
				{
					base[OrganizationSettingsSchema.PerimeterSafelistingUIMode] = value;
				}
			}

			public bool PerMBXPlanOWAPolicyEnabled
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.PerMBXPlanOWAPolicyEnabled];
				}
				set
				{
					base[OrganizationSettingsSchema.PerMBXPlanOWAPolicyEnabled] = value;
				}
			}

			public bool PerMBXPlanRetentionPolicyEnabled
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.PerMBXPlanRetentionPolicyEnabled];
				}
				set
				{
					base[OrganizationSettingsSchema.PerMBXPlanRetentionPolicyEnabled] = value;
				}
			}

			public bool PerMBXPlanRoleAssignmentPolicyEnabled
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.PerMBXPlanRoleAssignmentPolicyEnabled];
				}
				set
				{
					base[OrganizationSettingsSchema.PerMBXPlanRoleAssignmentPolicyEnabled] = value;
				}
			}

			public bool PermissionManagementEnabled
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.PermissionManagementEnabled];
				}
				set
				{
					base[OrganizationSettingsSchema.PermissionManagementEnabled] = value;
				}
			}

			public bool PilotEnabled
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.PilotEnabled];
				}
				set
				{
					base[OrganizationSettingsSchema.PilotEnabled] = value;
				}
			}

			public string PrivacyLink
			{
				get
				{
					return (string)base[OrganizationSettingsSchema.PrivacyLink];
				}
				set
				{
					base[OrganizationSettingsSchema.PrivacyLink] = value;
				}
			}

			public bool PublicFoldersEnabled
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.PublicFoldersEnabled];
				}
				set
				{
					base[OrganizationSettingsSchema.PublicFoldersEnabled] = value;
				}
			}

			public bool QuarantineEnabled
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.QuarantineEnabled];
				}
				set
				{
					base[OrganizationSettingsSchema.QuarantineEnabled] = value;
				}
			}

			public bool ReducedOutOfTheBoxMrmTagsEnabled
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.ReducedOutOfTheBoxMrmTagsEnabled];
				}
				set
				{
					base[OrganizationSettingsSchema.ReducedOutOfTheBoxMrmTagsEnabled] = value;
				}
			}

			public bool SearchMessageEnabled
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.SearchMessageEnabled];
				}
				set
				{
					base[OrganizationSettingsSchema.SearchMessageEnabled] = value;
				}
			}

			public bool ServiceConnectors
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.ServiceConnectors];
				}
				set
				{
					base[OrganizationSettingsSchema.ServiceConnectors] = value;
				}
			}

			public bool ShareableConfigurationEnabled
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.ShareableConfigurationEnabled];
				}
				set
				{
					base[OrganizationSettingsSchema.ShareableConfigurationEnabled] = value;
				}
			}

			public bool SkipToUAndParentalControlCheckEnabled
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.SkipToUAndParentalControlCheckEnabled];
				}
				set
				{
					base[OrganizationSettingsSchema.SkipToUAndParentalControlCheckEnabled] = value;
				}
			}

			public bool SMTPAddressCheckWithAcceptedDomainEnabled
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.SMTPAddressCheckWithAcceptedDomainEnabled];
				}
				set
				{
					base[OrganizationSettingsSchema.SMTPAddressCheckWithAcceptedDomainEnabled] = value;
				}
			}

			public bool SupervisionEnabled
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.SupervisionEnabled];
				}
				set
				{
					base[OrganizationSettingsSchema.SupervisionEnabled] = value;
				}
			}

			public bool SyncAccountsEnabled
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.SyncAccountsEnabled];
				}
				set
				{
					base[OrganizationSettingsSchema.SyncAccountsEnabled] = value;
				}
			}

			public bool TemplateTenant
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.TemplateTenant];
				}
				set
				{
					base[OrganizationSettingsSchema.TemplateTenant] = value;
				}
			}

			public bool TransportRulesCollectionsEnabled
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.TransportRulesCollectionsEnabled];
				}
				set
				{
					base[OrganizationSettingsSchema.TransportRulesCollectionsEnabled] = value;
				}
			}

			public bool UseServicePlanAsCounterInstanceName
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.UseServicePlanAsCounterInstanceName];
				}
				set
				{
					base[OrganizationSettingsSchema.UseServicePlanAsCounterInstanceName] = value;
				}
			}

			public bool RIMRoleGroupEnabled
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.RIMRoleGroupEnabled];
				}
				set
				{
					base[OrganizationSettingsSchema.RIMRoleGroupEnabled] = value;
				}
			}

			public bool ActiveSyncDeviceDataAccessPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.ActiveSyncDeviceDataAccessPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.ActiveSyncDeviceDataAccessPermissions] = value;
				}
			}

			public bool ActiveSyncPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.ActiveSyncPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.ActiveSyncPermissions] = value;
				}
			}

			public bool AddressBookPolicyPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.AddressBookPolicyPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.AddressBookPolicyPermissions] = value;
				}
			}

			public bool AddSecondaryDomainPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.AddSecondaryDomainPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.AddSecondaryDomainPermissions] = value;
				}
			}

			public bool ArchivePermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.ArchivePermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.ArchivePermissions] = value;
				}
			}

			public bool ChangeMailboxPlanAssignmentPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.ChangeMailboxPlanAssignmentPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.ChangeMailboxPlanAssignmentPermissions] = value;
				}
			}

			public bool ConfigCustomizationsPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.ConfigCustomizationsPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.ConfigCustomizationsPermissions] = value;
				}
			}

			public bool EwsPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.EwsPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.EwsPermissions] = value;
				}
			}

			public bool ExchangeMigrationPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.ExchangeMigrationPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.ExchangeMigrationPermissions] = value;
				}
			}

			public bool GroupAsGroupSyncPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.GroupAsGroupSyncPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.GroupAsGroupSyncPermissions] = value;
				}
			}

			public bool HotmailMigrationPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.HotmailMigrationPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.HotmailMigrationPermissions] = value;
				}
			}

			public bool HotmailSyncPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.HotmailSyncPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.HotmailSyncPermissions] = value;
				}
			}

			public bool ImapMigrationPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.ImapMigrationPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.ImapMigrationPermissions] = value;
				}
			}

			public bool ImapPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.ImapPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.ImapPermissions] = value;
				}
			}

			public bool ImapSyncPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.ImapSyncPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.ImapSyncPermissions] = value;
				}
			}

			public bool IRMPremiumFeaturesPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.IRMPremiumFeaturesPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.IRMPremiumFeaturesPermissions] = value;
				}
			}

			public bool JournalingRulesPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.JournalingRulesPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.JournalingRulesPermissions] = value;
				}
			}

			public bool LitigationHoldPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.LitigationHoldPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.LitigationHoldPermissions] = value;
				}
			}

			public bool MOWADeviceDataAccessPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.MOWADeviceDataAccessPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.MOWADeviceDataAccessPermissions] = value;
				}
			}

			public bool MSOIdPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.MSOIdPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.MSOIdPermissions] = value;
				}
			}

			public bool MailboxQuotaPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.MailboxQuotaPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.MailboxQuotaPermissions] = value;
				}
			}

			public bool MailboxSIRPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.MailboxSIRPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.MailboxSIRPermissions] = value;
				}
			}

			public bool MailboxRecoveryPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.MailboxRecoveryPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.MailboxRecoveryPermissions] = value;
				}
			}

			public bool MailTipsPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.MailTipsPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.MailTipsPermissions] = value;
				}
			}

			public bool ManagedFoldersPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.ManagedFoldersPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.ManagedFoldersPermissions] = value;
				}
			}

			public bool MessageTrackingPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.MessageTrackingPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.MessageTrackingPermissions] = value;
				}
			}

			public bool ModeratedRecipientsPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.ModeratedRecipientsPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.ModeratedRecipientsPermissions] = value;
				}
			}

			public bool NewUserPasswordManagementPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.NewUserPasswordManagementPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.NewUserPasswordManagementPermissions] = value;
				}
			}

			public bool NewUserResetPasswordOnNextLogonPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.NewUserResetPasswordOnNextLogonPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.NewUserResetPasswordOnNextLogonPermissions] = value;
				}
			}

			public bool OpenDomainProfileUpdatePermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.OpenDomainProfileUpdatePermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.OpenDomainProfileUpdatePermissions] = value;
				}
			}

			public bool OrganizationalAffinityPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.OrganizationalAffinityPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.OrganizationalAffinityPermissions] = value;
				}
			}

			public bool OutlookAnywherePermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.OutlookAnywherePermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.OutlookAnywherePermissions] = value;
				}
			}

			public bool OWAMailboxPolicyPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.OWAMailboxPolicyPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.OWAMailboxPolicyPermissions] = value;
				}
			}

			public bool OWAPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.OWAPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.OWAPermissions] = value;
				}
			}

			public bool PopPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.PopPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.PopPermissions] = value;
				}
			}

			public bool PopSyncPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.PopSyncPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.PopSyncPermissions] = value;
				}
			}

			public bool ProfileUpdatePermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.ProfileUpdatePermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.ProfileUpdatePermissions] = value;
				}
			}

			public bool RBACManagementPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.RBACManagementPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.RBACManagementPermissions] = value;
				}
			}

			public bool RecipientManagementPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.RecipientManagementPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.RecipientManagementPermissions] = value;
				}
			}

			public bool ResetUserPasswordManagementPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.ResetUserPasswordManagementPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.ResetUserPasswordManagementPermissions] = value;
				}
			}

			public bool RoleAssignmentPolicyPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.RoleAssignmentPolicyPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.RoleAssignmentPolicyPermissions] = value;
				}
			}

			public bool SearchMessagePermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.SearchMessagePermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.SearchMessagePermissions] = value;
				}
			}

			public bool SetHiddenFromAddressListPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.SetHiddenFromAddressListPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.SetHiddenFromAddressListPermissions] = value;
				}
			}

			public bool TeamMailboxPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.TeamMailboxPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.TeamMailboxPermissions] = value;
				}
			}

			public bool SMSPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.SMSPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.SMSPermissions] = value;
				}
			}

			public bool SupervisionPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.SupervisionPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.SupervisionPermissions] = value;
				}
			}

			public bool TransportRulesPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.TransportRulesPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.TransportRulesPermissions] = value;
				}
			}

			public bool UMAutoAttendantPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.UMAutoAttendantPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.UMAutoAttendantPermissions] = value;
				}
			}

			public bool UMCloudServicePermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.UMCloudServicePermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.UMCloudServicePermissions] = value;
				}
			}

			public bool UMFaxPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.UMFaxPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.UMFaxPermissions] = value;
				}
			}

			public bool UMOutDialingPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.UMOutDialingPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.UMOutDialingPermissions] = value;
				}
			}

			public bool UMPBXPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.UMPBXPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.UMPBXPermissions] = value;
				}
			}

			public bool UMPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.UMPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.UMPermissions] = value;
				}
			}

			public bool UMPersonalAutoAttendantPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.UMPersonalAutoAttendantPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.UMPersonalAutoAttendantPermissions] = value;
				}
			}

			public bool UMSMSMsgWaitingPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.UMSMSMsgWaitingPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.UMSMSMsgWaitingPermissions] = value;
				}
			}

			public bool UserLiveIdManagementPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.UserLiveIdManagementPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.UserLiveIdManagementPermissions] = value;
				}
			}

			public bool UserMailboxAccessPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.UserMailboxAccessPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.UserMailboxAccessPermissions] = value;
				}
			}

			public bool SoftDeletedFeatureManagementPermissions
			{
				get
				{
					return (bool)base[OrganizationSettingsSchema.SoftDeletedFeatureManagementPermissions];
				}
				set
				{
					base[OrganizationSettingsSchema.SoftDeletedFeatureManagementPermissions] = value;
				}
			}

			public string ContactCountQuota
			{
				get
				{
					object obj = base[OrganizationSettingsSchema.ContactCountQuota];
					if (obj != null)
					{
						return obj.ToString();
					}
					return null;
				}
				set
				{
					base[OrganizationSettingsSchema.ContactCountQuota] = Unlimited<int>.Parse(value);
				}
			}

			public string DistributionListCountQuota
			{
				get
				{
					object obj = base[OrganizationSettingsSchema.DistributionListCountQuota];
					if (obj != null)
					{
						return obj.ToString();
					}
					return null;
				}
				set
				{
					base[OrganizationSettingsSchema.DistributionListCountQuota] = Unlimited<int>.Parse(value);
				}
			}

			public string MailboxCountQuota
			{
				get
				{
					object obj = base[OrganizationSettingsSchema.MailboxCountQuota];
					if (obj != null)
					{
						return obj.ToString();
					}
					return null;
				}
				set
				{
					base[OrganizationSettingsSchema.MailboxCountQuota] = Unlimited<int>.Parse(value);
				}
			}

			public string MailPublicFolderCountQuota
			{
				get
				{
					object obj = base[OrganizationSettingsSchema.MailPublicFolderCountQuota];
					if (obj != null)
					{
						return obj.ToString();
					}
					return null;
				}
				set
				{
					base[OrganizationSettingsSchema.MailPublicFolderCountQuota] = Unlimited<int>.Parse(value);
				}
			}

			public string MailUserCountQuota
			{
				get
				{
					object obj = base[OrganizationSettingsSchema.MailUserCountQuota];
					if (obj != null)
					{
						return obj.ToString();
					}
					return null;
				}
				set
				{
					base[OrganizationSettingsSchema.MailUserCountQuota] = Unlimited<int>.Parse(value);
				}
			}

			public string PublicFolderMailboxCountQuota
			{
				get
				{
					object obj = base[OrganizationSettingsSchema.PublicFolderMailboxCountQuota];
					if (obj != null)
					{
						return obj.ToString();
					}
					return null;
				}
				set
				{
					base[OrganizationSettingsSchema.PublicFolderMailboxCountQuota] = Unlimited<int>.Parse(value);
				}
			}

			public string RecipientMailSubmissionRateQuota
			{
				get
				{
					object obj = base[OrganizationSettingsSchema.RecipientMailSubmissionRateQuota];
					if (obj != null)
					{
						return obj.ToString();
					}
					return null;
				}
				set
				{
					base[OrganizationSettingsSchema.RecipientMailSubmissionRateQuota] = Unlimited<int>.Parse(value);
				}
			}

			public string TeamMailboxCountQuota
			{
				get
				{
					object obj = base[OrganizationSettingsSchema.TeamMailboxCountQuota];
					if (obj != null)
					{
						return obj.ToString();
					}
					return null;
				}
				set
				{
					base[OrganizationSettingsSchema.TeamMailboxCountQuota] = Unlimited<int>.Parse(value);
				}
			}

			internal bool PrivacyFeaturesAllowed
			{
				get
				{
					return this.SkipToUAndParentalControlCheckEnabled || !this.HideAdminAccessWarningEnabled;
				}
				set
				{
					if (!value)
					{
						throw new NotImplementedException();
					}
					this.SkipToUAndParentalControlCheckEnabled = value;
				}
			}

			private Unlimited<int> GetCountQuota(FeatureDefinition countQuotaFeature)
			{
				object obj = base[countQuotaFeature];
				if (obj != null)
				{
					return (Unlimited<int>)obj;
				}
				return new Unlimited<int>(0);
			}

			public Unlimited<int> GetDistributionListCountQuota()
			{
				return this.GetCountQuota(OrganizationSettingsSchema.DistributionListCountQuota);
			}

			public Unlimited<int> GetMailboxCountQuota()
			{
				return this.GetCountQuota(OrganizationSettingsSchema.MailboxCountQuota);
			}

			public Unlimited<int> GetMailUserCountQuota()
			{
				return this.GetCountQuota(OrganizationSettingsSchema.MailUserCountQuota);
			}

			public Unlimited<int> GetContactCountQuota()
			{
				return this.GetCountQuota(OrganizationSettingsSchema.ContactCountQuota);
			}

			public Unlimited<int> GetTeamMailboxCountQuota()
			{
				return this.GetCountQuota(OrganizationSettingsSchema.TeamMailboxCountQuota);
			}

			public Unlimited<int> GetPublicFolderMailboxCountQuota()
			{
				return this.GetCountQuota(OrganizationSettingsSchema.PublicFolderMailboxCountQuota);
			}

			public Unlimited<int> GetMailPublicFolderCountQuota()
			{
				return this.GetCountQuota(OrganizationSettingsSchema.MailPublicFolderCountQuota);
			}

			public Unlimited<int> GetRecipientMailSubmissionRateQuota()
			{
				return this.GetCountQuota(OrganizationSettingsSchema.RecipientMailSubmissionRateQuota);
			}

			private static OrganizationSettingsSchema schema = new OrganizationSettingsSchema();
		}

		public sealed class MailboxPlan : BooleanFeatureBag
		{
			internal override ServicePlanElementSchema Schema
			{
				get
				{
					return ServicePlan.MailboxPlan.schema;
				}
			}

			internal MailboxPlan()
			{
			}

			protected override void InitializeDependencies()
			{
				base.Dependencies.Add(new DependencyEntry("OpenDomainProfileUpdatePermissions", "!ProfileUpdatePermissions", () => this.OpenDomainProfileUpdatePermissions, (ServicePlan sp) => this.OpenDomainProfileUpdatePermissions != this.ProfileUpdatePermissions, delegate(ServicePlan sp, bool value)
				{
					this.ProfileUpdatePermissions = !this.OpenDomainProfileUpdatePermissions;
				}));
				base.Dependencies.Add(new DependencyEntry("OutlookAnywhereEnabled", "ShowInAddressListsEnabled", () => this.OutlookAnywhereEnabled, (ServicePlan sp) => this.ShowInAddressListsEnabled, delegate(ServicePlan sp, bool value)
				{
					this.ShowInAddressListsEnabled = value;
				}));
				base.Dependencies.Add(new DependencyEntry("SyncAccountsEnabled", "SyncAccountsMaxAccountsQuota", () => this.SyncAccountsEnabled, (ServicePlan sp) => this.SyncAccountsMaxAccountsQuota != null, delegate(ServicePlan sp, bool value)
				{
					string syncAccountsMaxAccountsQuota = ((ADPropertyDefinition)RemoteAccountPolicySchema.MaxSyncAccounts).DefaultValue.ToString();
					this.SyncAccountsMaxAccountsQuota = syncAccountsMaxAccountsQuota;
				}));
				base.Dependencies.Add(new DependencyEntry("SyncAccountsEnabled", "SyncAccountsPollingInterval", () => this.SyncAccountsEnabled, (ServicePlan sp) => this.SyncAccountsPollingInterval != null, delegate(ServicePlan sp, bool value)
				{
					string syncAccountsPollingInterval = ((ADPropertyDefinition)RemoteAccountPolicySchema.PollingInterval).DefaultValue.ToString();
					this.SyncAccountsPollingInterval = syncAccountsPollingInterval;
				}));
				base.Dependencies.Add(new DependencyEntry("SyncAccountsEnabled", "SyncAccountsTimeBeforeInactive", () => this.SyncAccountsEnabled, (ServicePlan sp) => this.SyncAccountsTimeBeforeInactive != null, delegate(ServicePlan sp, bool value)
				{
					string syncAccountsTimeBeforeInactive = ((ADPropertyDefinition)RemoteAccountPolicySchema.TimeBeforeInactive).DefaultValue.ToString();
					this.SyncAccountsTimeBeforeInactive = syncAccountsTimeBeforeInactive;
				}));
				base.Dependencies.Add(new DependencyEntry("SyncAccountsEnabled", "SyncAccountsTimeBeforeDormant", () => this.SyncAccountsEnabled, (ServicePlan sp) => this.SyncAccountsTimeBeforeDormant != null, delegate(ServicePlan sp, bool value)
				{
					string syncAccountsTimeBeforeDormant = ((ADPropertyDefinition)RemoteAccountPolicySchema.TimeBeforeDormant).DefaultValue.ToString();
					this.SyncAccountsTimeBeforeDormant = syncAccountsTimeBeforeDormant;
				}));
				base.Dependencies.Add(new DependencyEntry("SyncAccountsSyncNowEnabled", "SyncAccountsEnabled", () => this.SyncAccountsSyncNowEnabled, (ServicePlan sp) => this.SyncAccountsEnabled, delegate(ServicePlan sp, bool value)
				{
					this.SyncAccountsEnabled = value;
				}));
				base.Dependencies.Add(new DependencyEntry("SyncAccountsMaxAccountsQuota", "SyncAccountsEnabled", () => this.SyncAccountsMaxAccountsQuota != null, (ServicePlan sp) => this.SyncAccountsEnabled, delegate(ServicePlan sp, bool value)
				{
					this.SyncAccountsEnabled = value;
				}));
				base.Dependencies.Add(new DependencyEntry("SyncAccountsPollingInterval", "SyncAccountsEnabled", () => this.SyncAccountsPollingInterval != null, (ServicePlan sp) => this.SyncAccountsEnabled, delegate(ServicePlan sp, bool value)
				{
					this.SyncAccountsEnabled = value;
				}));
				base.Dependencies.Add(new DependencyEntry("SyncAccountsTimeBeforeInactive", "SyncAccountsEnabled", () => this.SyncAccountsTimeBeforeInactive != null, (ServicePlan sp) => this.SyncAccountsEnabled, delegate(ServicePlan sp, bool value)
				{
					this.SyncAccountsEnabled = value;
				}));
				base.Dependencies.Add(new DependencyEntry("SyncAccountsTimeBeforeDormant", "SyncAccountsEnabled", () => this.SyncAccountsTimeBeforeDormant != null, (ServicePlan sp) => this.SyncAccountsEnabled, delegate(ServicePlan sp, bool value)
				{
					this.SyncAccountsEnabled = value;
				}));
				base.Dependencies.Add(new DependencyEntry("SyncAccountsEnabled", "SyncAccountsEnabled", () => this.SyncAccountsEnabled, (ServicePlan sp) => sp.organization.SyncAccountsEnabled, delegate(ServicePlan sp, bool value)
				{
					sp.organization.SyncAccountsEnabled = value;
				}));
				base.Dependencies.Add(new DependencyEntry("MailboxPlanIndex", "ProhibitSendReceiveMaiboxQuota", () => true, (ServicePlan sp) => !string.IsNullOrEmpty(this.ProhibitSendReceiveMaiboxQuota), delegate(ServicePlan sp, bool value)
				{
					this.ProhibitSendReceiveMaiboxQuota = "10GB";
				}));
				base.Dependencies.Add(new DependencyEntry("UniqueMailboxPlanIndex", "MailboxPlanIndex", () => true, (ServicePlan sp) => this.MailboxPlanIndexSetAndUnique(sp), delegate(ServicePlan sp, bool value)
				{
					Random random = new Random();
					this.MailboxPlanIndex = random.Next(0, 65535).ToString();
				}));
				base.Dependencies.Add(new DependencyEntry("ModeratedRecipientsPermissions", "AutoGroupPermissions", () => this.ModeratedRecipientsPermissions, (ServicePlan sp) => this.AutoGroupPermissions, delegate(ServicePlan sp, bool value)
				{
					this.AutoGroupPermissions = value;
				}));
				base.Dependencies.Add(new DependencyEntry("ActiveSyncDeviceDataAccessPermissions", "ActiveSyncEnabled", () => this.ActiveSyncDeviceDataAccessPermissions, (ServicePlan sp) => this.ActiveSyncEnabled, delegate(ServicePlan sp, bool value)
				{
					this.ActiveSyncEnabled = value;
				}));
				base.Dependencies.Add(new DependencyEntry("MOWADeviceDataAccessPermissions", "MOWAEnabled", () => this.MOWADeviceDataAccessPermissions, (ServicePlan sp) => this.MOWAEnabled, delegate(ServicePlan sp, bool value)
				{
					this.MOWAEnabled = value;
				}));
				base.Dependencies.Add(new DependencyEntry("UMSMSMsgWaitingPermissions", "UMPermissions", () => this.UMSMSMsgWaitingPermissions, (ServicePlan sp) => this.UMPermissions, delegate(ServicePlan sp, bool value)
				{
					this.UMPermissions = value;
				}));
				base.Dependencies.Add(new DependencyEntry("UMCloudServicePermissions", "UMPermissions", () => this.UMCloudServicePermissions, (ServicePlan sp) => this.UMPermissions, delegate(ServicePlan sp, bool value)
				{
					this.UMPermissions = value;
				}));
				base.Dependencies.Add(new DependencyEntry("MailTipsPermissions", "MailTipsEnabled", () => this.MailTipsPermissions, (ServicePlan sp) => sp.organization.MailTipsEnabled, delegate(ServicePlan sp, bool value)
				{
					sp.organization.MailTipsEnabled = value;
				}));
				base.Dependencies.Add(new DependencyEntry("ViewSupervisionListPermissions", "SupervisionEnabled", () => this.ViewSupervisionListPermissions, (ServicePlan sp) => sp.organization.SupervisionEnabled, delegate(ServicePlan sp, bool value)
				{
					sp.organization.SupervisionEnabled = value;
				}));
				base.Dependencies.Add(new DependencyEntry("SkipResetPasswordOnFirstLogonEnabled", "PrivacyFeaturesAllowed", () => this.SkipResetPasswordOnFirstLogonEnabled, (ServicePlan sp) => sp.organization.PrivacyFeaturesAllowed, delegate(ServicePlan sp, bool value)
				{
					sp.organization.PrivacyFeaturesAllowed = value;
				}));
				base.Dependencies.Add(new DependencyEntry("ArchiveQuota", "ArchivePermissions", () => this.ArchiveQuota != null, (ServicePlan sp) => sp.organization.ArchivePermissions, delegate(ServicePlan sp, bool value)
				{
					sp.organization.ArchivePermissions = value;
				}));
			}

			private bool MailboxPlanIndexSetAndUnique(ServicePlan sp)
			{
				if (string.IsNullOrEmpty(this.MailboxPlanIndex))
				{
					return false;
				}
				foreach (ServicePlan.MailboxPlan mailboxPlan in sp.MailboxPlans)
				{
					if (this != mailboxPlan && this.MailboxPlanIndex.Equals(mailboxPlan.MailboxPlanIndex, StringComparison.InvariantCultureIgnoreCase))
					{
						return false;
					}
				}
				return true;
			}

			public object Instance { get; set; }

			[XmlAttribute]
			public string Name
			{
				get
				{
					return this.name;
				}
				set
				{
					this.name = value;
				}
			}

			[XmlAttribute]
			public string MailboxPlanIndex
			{
				get
				{
					return this.mailboxPlanIndex;
				}
				set
				{
					this.mailboxPlanIndex = value;
				}
			}

			[XmlAttribute]
			public bool ProvisionAsDefault { get; set; }

			[XmlAttribute]
			public bool IsPilotMailboxPlan { get; set; }

			public bool ActiveSyncEnabled
			{
				get
				{
					return (bool)base[MailboxPlanSchema.ActiveSyncEnabled];
				}
				set
				{
					base[MailboxPlanSchema.ActiveSyncEnabled] = value;
				}
			}

			public bool EwsEnabled
			{
				get
				{
					return (bool)base[MailboxPlanSchema.EwsEnabled];
				}
				set
				{
					base[MailboxPlanSchema.EwsEnabled] = value;
				}
			}

			public bool ImapEnabled
			{
				get
				{
					return (bool)base[MailboxPlanSchema.ImapEnabled];
				}
				set
				{
					base[MailboxPlanSchema.ImapEnabled] = value;
				}
			}

			public bool MOWAEnabled
			{
				get
				{
					return (bool)base[MailboxPlanSchema.MOWAEnabled];
				}
				set
				{
					base[MailboxPlanSchema.MOWAEnabled] = value;
				}
			}

			public bool OrganizationalQueryBaseDNEnabled
			{
				get
				{
					return (bool)base[MailboxPlanSchema.OrganizationalQueryBaseDNEnabled];
				}
				set
				{
					base[MailboxPlanSchema.OrganizationalQueryBaseDNEnabled] = value;
				}
			}

			public bool OutlookAnywhereEnabled
			{
				get
				{
					return (bool)base[MailboxPlanSchema.OutlookAnywhereEnabled];
				}
				set
				{
					base[MailboxPlanSchema.OutlookAnywhereEnabled] = value;
				}
			}

			public bool PopEnabled
			{
				get
				{
					return (bool)base[MailboxPlanSchema.PopEnabled];
				}
				set
				{
					base[MailboxPlanSchema.PopEnabled] = value;
				}
			}

			public bool ShowInAddressListsEnabled
			{
				get
				{
					return (bool)base[MailboxPlanSchema.ShowInAddressListsEnabled];
				}
				set
				{
					base[MailboxPlanSchema.ShowInAddressListsEnabled] = value;
				}
			}

			public bool SingleItemRecoveryEnabled
			{
				get
				{
					return (bool)base[MailboxPlanSchema.SingleItemRecoveryEnabled];
				}
				set
				{
					base[MailboxPlanSchema.SingleItemRecoveryEnabled] = value;
				}
			}

			public bool SkipResetPasswordOnFirstLogonEnabled
			{
				get
				{
					return (bool)base[MailboxPlanSchema.SkipResetPasswordOnFirstLogonEnabled];
				}
				set
				{
					base[MailboxPlanSchema.SkipResetPasswordOnFirstLogonEnabled] = value;
				}
			}

			public bool SyncAccountsEnabled
			{
				get
				{
					return (bool)base[MailboxPlanSchema.SyncAccountsEnabled];
				}
				set
				{
					base[MailboxPlanSchema.SyncAccountsEnabled] = value;
				}
			}

			public bool SyncAccountsSyncNowEnabled
			{
				get
				{
					return (bool)base[MailboxPlanSchema.SyncAccountsSyncNowEnabled];
				}
				set
				{
					base[MailboxPlanSchema.SyncAccountsSyncNowEnabled] = value;
				}
			}

			public Capability SkuCapability
			{
				get
				{
					return (Capability)base[MailboxPlanSchema.SkuCapability];
				}
				set
				{
					if (value != Capability.None && !CapabilityHelper.AllowedSKUCapabilities.Contains(value))
					{
						throw new XmlException(Strings.ErrorServicePlanMailboxPlanInvalidSkuCapability(value.ToString(), MultiValuedPropertyBase.FormatMultiValuedProperty(CapabilityHelper.AllowedSKUCapabilities)));
					}
					base[MailboxPlanSchema.SkuCapability] = value;
				}
			}

			public UMDeploymentModeOptions UMEnabled
			{
				get
				{
					return (UMDeploymentModeOptions)base[MailboxPlanSchema.UMEnabled];
				}
				set
				{
					base[MailboxPlanSchema.UMEnabled] = value;
				}
			}

			public bool ActiveSyncDeviceDataAccessPermissions
			{
				get
				{
					return (bool)base[MailboxPlanSchema.ActiveSyncDeviceDataAccessPermissions];
				}
				set
				{
					base[MailboxPlanSchema.ActiveSyncDeviceDataAccessPermissions] = value;
				}
			}

			public bool ActiveSyncPermissions
			{
				get
				{
					return (bool)base[MailboxPlanSchema.ActiveSyncPermissions];
				}
				set
				{
					base[MailboxPlanSchema.ActiveSyncPermissions] = value;
				}
			}

			public bool AutoGroupPermissions
			{
				get
				{
					return (bool)base[MailboxPlanSchema.AutoGroupPermissions];
				}
				set
				{
					base[MailboxPlanSchema.AutoGroupPermissions] = value;
				}
			}

			public bool EXOCoreFeatures
			{
				get
				{
					return (bool)base[MailboxPlanSchema.EXOCoreFeatures];
				}
				set
				{
					base[MailboxPlanSchema.EXOCoreFeatures] = value;
				}
			}

			public bool HotmailSyncPermissions
			{
				get
				{
					return (bool)base[MailboxPlanSchema.HotmailSyncPermissions];
				}
				set
				{
					base[MailboxPlanSchema.HotmailSyncPermissions] = value;
				}
			}

			public bool ImapPermissions
			{
				get
				{
					return (bool)base[MailboxPlanSchema.ImapPermissions];
				}
				set
				{
					base[MailboxPlanSchema.ImapPermissions] = value;
				}
			}

			public bool ImapSyncPermissions
			{
				get
				{
					return (bool)base[MailboxPlanSchema.ImapSyncPermissions];
				}
				set
				{
					base[MailboxPlanSchema.ImapSyncPermissions] = value;
				}
			}

			public bool MailTipsPermissions
			{
				get
				{
					return (bool)base[MailboxPlanSchema.MailTipsPermissions];
				}
				set
				{
					base[MailboxPlanSchema.MailTipsPermissions] = value;
				}
			}

			public bool MessageTrackingPermissions
			{
				get
				{
					return (bool)base[MailboxPlanSchema.MessageTrackingPermissions];
				}
				set
				{
					base[MailboxPlanSchema.MessageTrackingPermissions] = value;
				}
			}

			public bool ModeratedRecipientsPermissions
			{
				get
				{
					return (bool)base[MailboxPlanSchema.ModeratedRecipientsPermissions];
				}
				set
				{
					base[MailboxPlanSchema.ModeratedRecipientsPermissions] = value;
				}
			}

			public bool MOWADeviceDataAccessPermissions
			{
				get
				{
					return (bool)base[MailboxPlanSchema.MOWADeviceDataAccessPermissions];
				}
				set
				{
					base[MailboxPlanSchema.MOWADeviceDataAccessPermissions] = value;
				}
			}

			public bool OpenDomainProfileUpdatePermissions
			{
				get
				{
					return (bool)base[MailboxPlanSchema.OpenDomainProfileUpdatePermissions];
				}
				set
				{
					base[MailboxPlanSchema.OpenDomainProfileUpdatePermissions] = value;
				}
			}

			public bool OrganizationalAffinityPermissions
			{
				get
				{
					return (bool)base[MailboxPlanSchema.OrganizationalAffinityPermissions];
				}
				set
				{
					base[MailboxPlanSchema.OrganizationalAffinityPermissions] = value;
				}
			}

			public bool PopPermissions
			{
				get
				{
					return (bool)base[MailboxPlanSchema.PopPermissions];
				}
				set
				{
					base[MailboxPlanSchema.PopPermissions] = value;
				}
			}

			public bool PopSyncPermissions
			{
				get
				{
					return (bool)base[MailboxPlanSchema.PopSyncPermissions];
				}
				set
				{
					base[MailboxPlanSchema.PopSyncPermissions] = value;
				}
			}

			public bool ProfileUpdatePermissions
			{
				get
				{
					return (bool)base[MailboxPlanSchema.ProfileUpdatePermissions];
				}
				set
				{
					base[MailboxPlanSchema.ProfileUpdatePermissions] = value;
				}
			}

			public bool ResetUserPasswordManagementPermissions
			{
				get
				{
					return (bool)base[MailboxPlanSchema.ResetUserPasswordManagementPermissions];
				}
				set
				{
					base[MailboxPlanSchema.ResetUserPasswordManagementPermissions] = value;
				}
			}

			public bool TeamMailboxPermissions
			{
				get
				{
					return (bool)base[MailboxPlanSchema.TeamMailboxPermissions];
				}
				set
				{
					base[MailboxPlanSchema.TeamMailboxPermissions] = value;
				}
			}

			public bool SMSPermissions
			{
				get
				{
					return (bool)base[MailboxPlanSchema.SMSPermissions];
				}
				set
				{
					base[MailboxPlanSchema.SMSPermissions] = value;
				}
			}

			public bool UMCloudServicePermissions
			{
				get
				{
					return (bool)base[MailboxPlanSchema.UMCloudServicePermissions];
				}
				set
				{
					base[MailboxPlanSchema.UMCloudServicePermissions] = value;
				}
			}

			public bool UMPermissions
			{
				get
				{
					return (bool)base[MailboxPlanSchema.UMPermissions];
				}
				set
				{
					base[MailboxPlanSchema.UMPermissions] = value;
				}
			}

			public bool UMSMSMsgWaitingPermissions
			{
				get
				{
					return (bool)base[MailboxPlanSchema.UMSMSMsgWaitingPermissions];
				}
				set
				{
					base[MailboxPlanSchema.UMSMSMsgWaitingPermissions] = value;
				}
			}

			public bool UserMailboxAccessPermissions
			{
				get
				{
					return (bool)base[MailboxPlanSchema.UserMailboxAccessPermissions];
				}
				set
				{
					base[MailboxPlanSchema.UserMailboxAccessPermissions] = value;
				}
			}

			public bool ViewSupervisionListPermissions
			{
				get
				{
					return (bool)base[MailboxPlanSchema.ViewSupervisionListPermissions];
				}
				set
				{
					base[MailboxPlanSchema.ViewSupervisionListPermissions] = value;
				}
			}

			public string MaxReceiveTransportQuota
			{
				get
				{
					return (string)base[MailboxPlanSchema.MaxReceiveTransportQuota];
				}
				set
				{
					base[MailboxPlanSchema.MaxReceiveTransportQuota] = value;
				}
			}

			public string MaxRecipientsTransportQuota
			{
				get
				{
					return (string)base[MailboxPlanSchema.MaxRecipientsTransportQuota];
				}
				set
				{
					base[MailboxPlanSchema.MaxRecipientsTransportQuota] = value;
				}
			}

			public string MaxSendTransportQuota
			{
				get
				{
					return (string)base[MailboxPlanSchema.MaxSendTransportQuota];
				}
				set
				{
					base[MailboxPlanSchema.MaxSendTransportQuota] = value;
				}
			}

			public string ProhibitSendReceiveMaiboxQuota
			{
				get
				{
					return (string)base[MailboxPlanSchema.ProhibitSendReceiveMaiboxQuota];
				}
				set
				{
					base[MailboxPlanSchema.ProhibitSendReceiveMaiboxQuota] = value;
				}
			}

			public string SyncAccountsMaxAccountsQuota
			{
				get
				{
					return (string)base[MailboxPlanSchema.SyncAccountsMaxAccountsQuota];
				}
				set
				{
					base[MailboxPlanSchema.SyncAccountsMaxAccountsQuota] = value;
				}
			}

			public string ArchiveQuota
			{
				get
				{
					return (string)base[MailboxPlanSchema.ArchiveQuota];
				}
				set
				{
					base[MailboxPlanSchema.ArchiveQuota] = value;
				}
			}

			public string SyncAccountsPollingInterval
			{
				get
				{
					return (string)base[MailboxPlanSchema.SyncAccountsPollingInterval];
				}
				set
				{
					base[MailboxPlanSchema.SyncAccountsPollingInterval] = value;
				}
			}

			public string SyncAccountsTimeBeforeDormant
			{
				get
				{
					return (string)base[MailboxPlanSchema.SyncAccountsTimeBeforeDormant];
				}
				set
				{
					base[MailboxPlanSchema.SyncAccountsTimeBeforeDormant] = value;
				}
			}

			public string SyncAccountsTimeBeforeInactive
			{
				get
				{
					return (string)base[MailboxPlanSchema.SyncAccountsTimeBeforeInactive];
				}
				set
				{
					base[MailboxPlanSchema.SyncAccountsTimeBeforeInactive] = value;
				}
			}

			private static MailboxPlanSchema schema = new MailboxPlanSchema();

			private string name;

			private string mailboxPlanIndex;
		}
	}
}
