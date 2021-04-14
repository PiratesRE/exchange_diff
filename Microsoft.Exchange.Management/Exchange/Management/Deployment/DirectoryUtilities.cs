using System;
using System.DirectoryServices;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Security.AccessControl;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DirectoryUtilities
	{
		private DirectoryUtilities()
		{
		}

		private static ITopologyConfigurationSession ConfigurationSession
		{
			get
			{
				if (DirectoryUtilities.configurationSession == null)
				{
					DirectoryUtilities.configurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 86, "ConfigurationSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Deployment\\DirectoryUtilities.cs");
				}
				return DirectoryUtilities.configurationSession;
			}
		}

		private static bool IsGlobalCatalog(ADServer server)
		{
			ADForest localForest = ADForest.GetLocalForest();
			ReadOnlyCollection<ADServer> readOnlyCollection = localForest.FindAllGlobalCatalogs();
			foreach (ADServer adserver in readOnlyCollection)
			{
				if (adserver.ValidateRead().Length == 0 && adserver.Id.Equals(server.Id))
				{
					return true;
				}
			}
			return false;
		}

		public static ADServer GetSchemaMasterDomainController()
		{
			string schemaMasterDC = DirectoryUtilities.ConfigurationSession.GetSchemaMasterDC();
			ADServer adserver = DirectoryUtilities.DomainControllerFromName(schemaMasterDC);
			if (adserver == null)
			{
				throw new SchemaMasterDCNotFoundException(Strings.DCWithGivenNameCouldNotBeFound(schemaMasterDC));
			}
			return adserver;
		}

		public static ADServer DomainControllerFromName(string domainControllerName)
		{
			string dnsHostName = null;
			try
			{
				dnsHostName = Dns.GetHostEntry(domainControllerName).HostName;
			}
			catch (SocketException)
			{
				return null;
			}
			ADServer adserver = DirectoryUtilities.ConfigurationSession.FindDCByFqdn(dnsHostName);
			if (adserver != null && adserver.ValidateRead().Length == 0)
			{
				return adserver;
			}
			return null;
		}

		public static ADServer PickLocalDomainController()
		{
			ADServer adserver = DirectoryUtilities.DomainControllerFromName("localhost");
			if (adserver != null && DirectoryUtilities.IsGlobalCatalog(adserver) && adserver.IsAvailable())
			{
				return adserver;
			}
			ADSite localSite = DirectoryUtilities.ConfigurationSession.GetLocalSite();
			if (localSite == null)
			{
				throw new ADInitializationException(Strings.LocalSiteNotFound);
			}
			ADDomain addomain = ADForest.GetLocalForest().FindLocalDomain();
			if (addomain == null)
			{
				throw new ADInitializationException(Strings.LocalDomainNotFoundException);
			}
			ReadOnlyCollection<ADServer> readOnlyCollection = addomain.FindAllDomainControllers();
			foreach (ADServer adserver2 in readOnlyCollection)
			{
				if (adserver2.ValidateRead().Length == 0 && adserver2.Site.Equals(localSite.Id) && adserver2.IsAvailable() && DirectoryUtilities.IsGlobalCatalog(adserver2))
				{
					return adserver2;
				}
			}
			if (adserver != null)
			{
				return adserver;
			}
			foreach (ADServer adserver3 in readOnlyCollection)
			{
				if (adserver3.ValidateRead().Length == 0 && adserver3.Site.Equals(localSite.Id) && adserver3.IsAvailable())
				{
					return adserver3;
				}
			}
			throw new ADInitializationException(Strings.NoDCsAvailableException(addomain.Name, localSite.Name));
		}

		public static ADServer PickGlobalCatalog(string configDCName)
		{
			ADServer adserver = DirectoryUtilities.DomainControllerFromName(configDCName);
			if (adserver != null && DirectoryUtilities.IsGlobalCatalog(adserver) && adserver.IsAvailable())
			{
				return adserver;
			}
			ADForest localForest = ADForest.GetLocalForest();
			ADSite localSite = DirectoryUtilities.ConfigurationSession.GetLocalSite();
			if (localSite == null)
			{
				throw new ADInitializationException(Strings.LocalSiteNotFound);
			}
			ReadOnlyCollection<ADServer> readOnlyCollection = localForest.FindAllGlobalCatalogs();
			foreach (ADServer adserver2 in readOnlyCollection)
			{
				if (adserver2.ValidateRead().Length == 0 && adserver2.Site.Equals(localSite.Id) && adserver2.IsAvailable())
				{
					return adserver2;
				}
			}
			foreach (ADServer adserver3 in readOnlyCollection)
			{
				if (adserver3.ValidateRead().Length == 0 && adserver3.IsAvailable())
				{
					return adserver3;
				}
			}
			throw new ADInitializationException(Strings.NoGCsAvailableException(localForest.Fqdn));
		}

		public static bool TryGetSchemaVersionRangeUpper(string domainControllerName, out int rangeUpper)
		{
			rangeUpper = 0;
			IConfigurationSession configurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(domainControllerName, true, ConsistencyMode.FullyConsistent, null, ADSessionSettings.FromRootOrgScopeSet(), 272, "TryGetSchemaVersionRangeUpper", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Deployment\\DirectoryUtilities.cs");
			try
			{
				ADSchemaAttributeObject[] array = configurationSession.Find<ADSchemaAttributeObject>(DirectoryUtilities.ConfigurationSession.SchemaNamingContext, QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, "ms-Exch-Schema-Version-Pt"), null, 1);
				if (array.Length > 0)
				{
					rangeUpper = (array[0].RangeUpper ?? 0);
					return true;
				}
			}
			catch (DataValidationException ex)
			{
				throw new ADInitializationException(Strings.SchemaVersionDataValidationException(ex.Message), ex);
			}
			return false;
		}

		public static bool IsSchemaUpToDate(string domainControllerName)
		{
			int num;
			return DirectoryUtilities.TryGetSchemaVersionRangeUpper(domainControllerName, out num) && num >= 15312;
		}

		public static ADSchemaVersion GetSchemaVersion(string domainControllerName)
		{
			IConfigurationSession configurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(domainControllerName, true, ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 339, "GetSchemaVersion", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Deployment\\DirectoryUtilities.cs");
			ADSchemaVersion result = ADSchemaVersion.Unrecognized;
			try
			{
				ADSchemaAttributeObject[] array = configurationSession.Find<ADSchemaAttributeObject>(DirectoryUtilities.ConfigurationSession.SchemaNamingContext, QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, "ms-Exch-Schema-Version-Pt"), null, 1);
				ADSchemaAttributeObject[] array2 = configurationSession.Find<ADSchemaAttributeObject>(DirectoryUtilities.ConfigurationSession.SchemaNamingContext, QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, "ms-Exch-Schema-Version-ADC"), null, 1);
				if (array.Length == 0 && array2.Length == 0)
				{
					result = ADSchemaVersion.Windows;
				}
				else if (array.Length != 0)
				{
					int num = array[0].RangeUpper ?? 0;
					if (num >= 10637)
					{
						result = ADSchemaVersion.Exchange2007Rtm;
					}
					else if (num >= 6870)
					{
						result = ADSchemaVersion.Exchange2003;
					}
					else if (num >= 4397)
					{
						result = ADSchemaVersion.Exchange2000;
					}
				}
			}
			catch (DataValidationException ex)
			{
				throw new ADInitializationException(Strings.SchemaVersionDataValidationException(ex.Message), ex);
			}
			return result;
		}

		public static bool TryGetOrgConfigVersion(string domainControllerName, out int orgConfigVersion)
		{
			orgConfigVersion = 0;
			IConfigurationSession configurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(domainControllerName, true, ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 406, "TryGetOrgConfigVersion", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Deployment\\DirectoryUtilities.cs");
			try
			{
				configurationSession.GetExchangeConfigurationContainer();
			}
			catch (ExchangeConfigurationContainerNotFoundException)
			{
				return false;
			}
			catch (DataValidationException ex)
			{
				throw new ADInitializationException(Strings.ExchangeConfigContainerDataValidationException(ex.Message), ex);
			}
			try
			{
				Organization orgContainer = configurationSession.GetOrgContainer();
				object value = null;
				if (orgContainer.propertyBag.TryGetField(OrganizationSchema.ObjectVersion, ref value))
				{
					orgConfigVersion = Convert.ToInt32(value);
					return true;
				}
			}
			catch (OrgContainerNotFoundException)
			{
			}
			catch (DataValidationException ex2)
			{
				throw new ADInitializationException(Strings.OrganizationDataValidationException(ex2.Message), ex2);
			}
			return false;
		}

		public static bool IsOrgConfigUpToDate(string domainControllerName)
		{
			int num = 0;
			return DirectoryUtilities.TryGetOrgConfigVersion(domainControllerName, out num) && num >= Organization.OrgConfigurationVersion;
		}

		public static bool TryGetLocalDomainConfigVersion(out int domainConfigVersion)
		{
			domainConfigVersion = 0;
			bool result = false;
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 484, "TryGetLocalDomainConfigVersion", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Deployment\\DirectoryUtilities.cs");
			topologyConfigurationSession.UseConfigNC = false;
			ADDomain addomain = ADForest.GetLocalForest().FindLocalDomain();
			if (addomain == null)
			{
				throw new ADInitializationException(Strings.LocalDomainNotFoundException);
			}
			MesoContainer mesoContainer = topologyConfigurationSession.FindMesoContainer(addomain);
			if (mesoContainer != null)
			{
				ValidationError validationError;
				if (!DirectoryUtilities.IsPropertyValid(mesoContainer, MesoContainerSchema.ObjectVersion, out validationError))
				{
					throw new ADInitializationException(Strings.MesoVersionInvalidException(validationError.Description));
				}
				domainConfigVersion = mesoContainer.ObjectVersion;
				result = true;
			}
			return result;
		}

		public static bool IsLocalDomainConfigUpToDate()
		{
			bool result = false;
			bool flag = false;
			int num = 0;
			ADDomain addomain = ADForest.GetLocalForest().FindLocalDomain();
			if (addomain == null)
			{
				throw new ADInitializationException(Strings.LocalDomainNotFoundException);
			}
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 531, "IsLocalDomainConfigUpToDate", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Deployment\\DirectoryUtilities.cs");
			topologyConfigurationSession.UseConfigNC = false;
			MesoContainer mesoContainer = topologyConfigurationSession.FindMesoContainer(addomain);
			if (mesoContainer != null)
			{
				flag = true;
				ValidationError validationError;
				if (!DirectoryUtilities.IsPropertyValid(mesoContainer, MesoContainerSchema.ObjectVersion, out validationError))
				{
					throw new ADInitializationException(Strings.MesoVersionInvalidException(validationError.Description));
				}
				num = mesoContainer.ObjectVersion;
			}
			if (flag && num >= MesoContainer.DomainPrepVersion)
			{
				topologyConfigurationSession.UseGlobalCatalog = true;
				ADGroup adgroup = topologyConfigurationSession.ResolveWellKnownGuid<ADGroup>(WellKnownGuid.ExSWkGuid, topologyConfigurationSession.ConfigurationNamingContext);
				topologyConfigurationSession.UseGlobalCatalog = false;
				if (adgroup != null)
				{
					ActiveDirectoryAccessRule activeDirectoryAccessRule = new ActiveDirectoryAccessRule(adgroup.Sid, ActiveDirectoryRights.DeleteTree, AccessControlType.Deny, ActiveDirectorySecurityInheritance.All);
					result = DirectoryCommon.FindAces(mesoContainer, new ActiveDirectoryAccessRule[]
					{
						activeDirectoryAccessRule
					});
				}
			}
			return result;
		}

		public static bool InLocalDomain(ADServer server)
		{
			ADDomain addomain = ADForest.GetLocalForest().FindLocalDomain();
			if (addomain == null || addomain.Id == null)
			{
				throw new ADInitializationException(Strings.LocalDomainNotFoundException);
			}
			if (server.DomainId == null)
			{
				throw new ADInitializationException(Strings.ServerDoesNotHaveADomain(server.Name));
			}
			return server.DomainId.Equals(addomain.Id);
		}

		public static bool InSameDomain(ADServer serverOne, ADServer serverTwo)
		{
			return serverOne.DomainId.Equals(serverTwo.DomainId);
		}

		public static bool InSameSite(ADServer serverOne, ADServer serverTwo)
		{
			return serverOne.Site.Equals(serverTwo.Site);
		}

		public static bool IsPropertyValid(ADObject o, PropertyDefinition property, out ValidationError validationError)
		{
			bool result = true;
			validationError = null;
			ValidationError[] array = o.ValidateRead();
			foreach (ValidationError validationError2 in array)
			{
				if (validationError2 is PropertyValidationError)
				{
					PropertyValidationError propertyValidationError = validationError2 as PropertyValidationError;
					if (string.Compare(property.Name, propertyValidationError.PropertyDefinition.Name, true, CultureInfo.InvariantCulture) == 0 && property.Type == propertyValidationError.PropertyDefinition.Type)
					{
						result = false;
						validationError = validationError2;
						break;
					}
				}
			}
			return result;
		}

		public const int Exchange2000SchemaVersionNumber = 4397;

		public const int Exchange2003SchemaVersionNumber = 6870;

		public const int Exchange2007RtmSchemaVersionNumber = 10637;

		private static ITopologyConfigurationSession configurationSession;
	}
}
