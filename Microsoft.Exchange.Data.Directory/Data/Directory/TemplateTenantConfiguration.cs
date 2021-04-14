using System;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Data.Directory
{
	internal static class TemplateTenantConfiguration
	{
		internal static string CreateSharedConfigurationName(string programId, string offerId)
		{
			SharedConfigurationInfo sharedConfigurationInfo = SharedConfigurationInfo.FromInstalledVersion(programId, offerId);
			string text = string.Format("{0}{1}{2}{3}{4}", new object[]
			{
				SharedConfiguration.SCTNamePrefix,
				TemplateTenantConfiguration.Separator,
				sharedConfigurationInfo.ToString().ToLower().Replace("_", "-"),
				TemplateTenantConfiguration.Separator,
				Guid.NewGuid().ToString().Substring(0, 5)
			});
			if (text.Length > TemplateTenantConfiguration.maxStubLength)
			{
				text = text.Substring(0, TemplateTenantConfiguration.maxStubLength);
			}
			return text + TemplateTenantConfiguration.TopLevelDomain;
		}

		internal static SmtpDomain CreateSharedConfigurationDomainName(string name)
		{
			return new SmtpDomain(name);
		}

		internal static ITenantRecipientSession GetTempateTenantRecipientSession()
		{
			if (TemplateTenantConfiguration.cachedTemplateUser == null)
			{
				TemplateTenantConfiguration.RetrieveLocalTempateUserContext();
			}
			return TemplateTenantConfiguration.cachedRecipientSession;
		}

		internal static ADUser GetLocalTempateUser()
		{
			if (TemplateTenantConfiguration.cachedTemplateUser == null)
			{
				TemplateTenantConfiguration.RetrieveLocalTempateUserContext();
			}
			return TemplateTenantConfiguration.cachedTemplateUser;
		}

		internal static SecurityDescriptor GetTemplateUserSecurityDescriptorBlob()
		{
			if (TemplateTenantConfiguration.cachedTemplateUser == null)
			{
				TemplateTenantConfiguration.RetrieveLocalTempateUserContext();
			}
			return TemplateTenantConfiguration.cachedTemplateUserSd;
		}

		private static void RetrieveLocalTempateUserContext()
		{
			if (TemplateTenantConfiguration.cachedTemplateUser == null)
			{
				ExchangeConfigurationUnit localTemplateTenant = TemplateTenantConfiguration.GetLocalTemplateTenant();
				if (localTemplateTenant == null)
				{
					throw new ADTransientException(DirectoryStrings.CannotFindTemplateTenant);
				}
				ADSessionSettings adsessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), localTemplateTenant.OrganizationId, null, false);
				adsessionSettings.ForceADInTemplateScope = true;
				ITenantRecipientSession tenantRecipientSession = DirectorySessionFactory.Default.CreateTenantRecipientSession(false, ConsistencyMode.IgnoreInvalid, adsessionSettings, 121, "RetrieveLocalTempateUserContext", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\TemplateTenantConfiguration.cs");
				QueryFilter filter = new AndFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectClass, ADUser.MostDerivedClass),
					new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientTypeDetailsValue, RecipientTypeDetails.MailboxPlan)
				});
				ADUser[] array = tenantRecipientSession.FindADUser(null, QueryScope.OneLevel, filter, null, 1);
				if (array == null || array.Length == 0)
				{
					new ADTransientException(DirectoryStrings.CannotFindTemplateUser(localTemplateTenant.OrganizationalUnitRoot.DistinguishedName));
				}
				array[0].RecipientTypeDetails = RecipientTypeDetails.UserMailbox;
				array[0].RecipientDisplayType = new RecipientDisplayType?(RecipientDisplayType.MailboxUser);
				array[0].MasterAccountSid = null;
				array[0].AcceptMessagesOnlyFrom = null;
				array[0].UseDatabaseQuotaDefaults = new bool?(false);
				array[0].ExchangeUserAccountControl = UserAccountControlFlags.None;
				TemplateTenantConfiguration.cachedTemplateUser = array[0];
				TemplateTenantConfiguration.cachedTemplateUserSd = tenantRecipientSession.ReadSecurityDescriptorBlob(array[0].Id);
				TemplateTenantConfiguration.cachedRecipientSession = tenantRecipientSession;
			}
		}

		private static ExchangeConfigurationUnit RetrieveLocalTempateTenant()
		{
			ADPagedReader<ExchangeConfigurationUnit> adpagedReader = TemplateTenantConfiguration.FindAllTempateTenants(TemplateTenantConfiguration.ProgramId, TemplateTenantConfiguration.OfferId, PartitionId.LocalForest);
			ExchangeConfigurationUnit[] array = adpagedReader.ReadAllPages();
			switch (array.Length)
			{
			case 0:
				return null;
			case 1:
				return array[0];
			default:
			{
				Array.Sort<ExchangeConfigurationUnit>(array, new Comparison<ExchangeConfigurationUnit>(SharedConfiguration.CompareBySharedConfigurationInfo));
				ExchangeConfigurationUnit result = array[0];
				foreach (ExchangeConfigurationUnit exchangeConfigurationUnit in array)
				{
					if (!(exchangeConfigurationUnit.SharedConfigurationInfo != null) || ((IComparable)ServerVersion.InstalledVersion).CompareTo(exchangeConfigurationUnit.SharedConfigurationInfo.CurrentVersion) < 0)
					{
						break;
					}
					result = exchangeConfigurationUnit;
				}
				return result;
			}
			}
		}

		internal static ExchangeConfigurationUnit GetLocalTemplateTenant()
		{
			if (TemplateTenantConfiguration.cachedTemplateTenant == null)
			{
				TemplateTenantConfiguration.cachedTemplateTenant = TemplateTenantConfiguration.RetrieveLocalTempateTenant();
			}
			return TemplateTenantConfiguration.cachedTemplateTenant;
		}

		internal static ADPagedReader<ExchangeConfigurationUnit> FindAllTempateTenants(string programId, string offerId, PartitionId partitionId)
		{
			ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromAllTenantsPartitionId(partitionId), 236, "FindAllTempateTenants", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\TemplateTenantConfiguration.cs");
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new ExistsFilter(OrganizationSchema.SharedConfigurationInfo),
				new ComparisonFilter(ComparisonOperator.Equal, ExchangeConfigurationUnitSchema.ResellerId, programId + "." + offerId),
				new ComparisonFilter(ComparisonOperator.Equal, ExchangeConfigurationUnitSchema.OrganizationStatus, OrganizationStatus.Active),
				new ComparisonFilter(ComparisonOperator.Equal, OrganizationSchema.EnableAsSharedConfiguration, true)
			});
			return tenantConfigurationSession.FindPaged<ExchangeConfigurationUnit>(null, QueryScope.SubTree, filter, null, 0);
		}

		internal static bool IsTemplateTenant(OrganizationId orgId)
		{
			if (orgId == null)
			{
				throw new ArgumentNullException("orgId");
			}
			ADObjectId organizationalUnit = orgId.OrganizationalUnit;
			return organizationalUnit != null && organizationalUnit.DistinguishedName != null && TemplateTenantConfiguration.IsTemplateTenantName(organizationalUnit.Name);
		}

		internal static bool IsTemplateTenantName(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			return TemplateTenantConfiguration.templateTenantRegex.Match(name).Success;
		}

		private const int maxCNLength = 64;

		internal static readonly string DefaultDomain = "outlook.com";

		internal static readonly string TopLevelDomain = ".templateTenant";

		private static readonly Regex templateTenantRegex = new Regex("\\.templateTenant$", RegexOptions.Compiled, TimeSpan.FromSeconds(1.0));

		private static readonly int maxStubLength = 64 - TemplateTenantConfiguration.TopLevelDomain.Length;

		public static readonly string Separator = "-";

		public static readonly string TemplateTenantExternalDirectoryOrganizationId = "84df9e7f-e9f6-40af-b435-aaaaaaaaaaaa";

		public static readonly Guid TemplateTenantExternalDirectoryOrganizationIdGuid = new Guid(TemplateTenantConfiguration.TemplateTenantExternalDirectoryOrganizationId);

		public static readonly string ProgramId = "MSOnline";

		public static readonly string OfferId = "Outlook";

		public static readonly string TestProgramId = "ExchangeTest";

		public static readonly string TestOfferId = "29";

		public static readonly ServerVersion RequiredTemplateTenantVersion = new ServerVersion(15, 0, 1037, 0);

		private static ADUser cachedTemplateUser;

		private static SecurityDescriptor cachedTemplateUserSd;

		private static ExchangeConfigurationUnit cachedTemplateTenant;

		private static ITenantRecipientSession cachedRecipientSession;
	}
}
