using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Exchange.Data.Directory.Cache;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory
{
	internal abstract class DirectorySessionFactory
	{
		public static DirectorySessionFactory MockDefault
		{
			set
			{
				DirectorySessionFactory.mockDefault = value;
			}
		}

		public static DirectorySessionFactory Default
		{
			get
			{
				if (DirectorySessionFactory.mockDefault != null)
				{
					return DirectorySessionFactory.mockDefault;
				}
				if (!Configuration.IsCacheEnableForCurrentProcess())
				{
					return DirectorySessionFactory.NonCacheSessionFactory;
				}
				return DirectorySessionFactory.CacheFallbackSessionFactory;
			}
		}

		internal static DirectorySessionFactory NonCacheSessionFactory
		{
			get
			{
				if (DirectorySessionFactory.nonCacheSessionFactory == null)
				{
					DirectorySessionFactory.nonCacheSessionFactory = (DirectorySessionFactory)DirectorySessionFactory.InstantiateFfoOrExoClass("Microsoft.Exchange.Hygiene.Data.Directory.FfoDirectorySesssionFactory", typeof(ADSessionFactory));
				}
				return DirectorySessionFactory.nonCacheSessionFactory;
			}
		}

		internal static DirectorySessionFactory CacheSessionFactory
		{
			get
			{
				if (DirectorySessionFactory.cacheSessionFactory == null)
				{
					DirectorySessionFactory.cacheSessionFactory = (DirectorySessionFactory)DirectorySessionFactory.InstantiateFfoOrExoClass("Microsoft.Exchange.Hygiene.Data.Directory.FfoDirectorySesssionFactory", typeof(CacheDirectorySessionFactory));
				}
				return DirectorySessionFactory.cacheSessionFactory;
			}
		}

		protected static DirectorySessionFactory CacheFallbackSessionFactory
		{
			get
			{
				if (DirectorySessionFactory.cacheFallbackSessionFactory == null)
				{
					DirectorySessionFactory.cacheFallbackSessionFactory = (DirectorySessionFactory)DirectorySessionFactory.InstantiateFfoOrExoClass("Microsoft.Exchange.Hygiene.Data.Directory.FfoDirectorySesssionFactory", typeof(CompositeDirectorySessionFactory));
				}
				return DirectorySessionFactory.cacheFallbackSessionFactory;
			}
		}

		internal static DirectorySessionFactory GetInstance(DirectorySessionFactoryType directorySessionFactoryType)
		{
			if (directorySessionFactoryType == DirectorySessionFactoryType.Cached)
			{
				return DirectorySessionFactory.CacheFallbackSessionFactory;
			}
			return DirectorySessionFactory.Default;
		}

		public static IGlobalDirectorySession GetGlobalSession(string redirectFormatForMServ = null)
		{
			return new GlsMServDirectorySession(redirectFormatForMServ);
		}

		public abstract ITenantConfigurationSession CreateTenantConfigurationSession(ConsistencyMode consistencyMode, ADSessionSettings sessionSettings, [CallerLineNumber] int callerFileLine = 0, [CallerMemberName] string memberName = null, [CallerFilePath] string callerFilePath = null);

		public abstract ITenantConfigurationSession CreateTenantConfigurationSession(bool readOnly, ConsistencyMode consistencyMode, ADSessionSettings sessionSettings, [CallerLineNumber] int callerFileLine = 0, [CallerMemberName] string memberName = null, [CallerFilePath] string callerFilePath = null);

		public ITenantConfigurationSession CreateTenantConfigurationSession(string domainController, bool readOnly, ConsistencyMode consistencyMode, ADSessionSettings sessionSettings, [CallerLineNumber] int callerFileLine = 0, [CallerMemberName] string memberName = null, [CallerFilePath] string callerFilePath = null)
		{
			return this.CreateTenantConfigurationSession(domainController, readOnly, consistencyMode, null, sessionSettings, callerFileLine, memberName, callerFilePath);
		}

		public abstract ITenantConfigurationSession CreateTenantConfigurationSession(string domainController, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, [CallerLineNumber] int callerFileLine = 0, [CallerMemberName] string memberName = null, [CallerFilePath] string callerFilePath = null);

		public abstract ITenantConfigurationSession CreateTenantConfigurationSession(string domainController, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, ConfigScopes configScope, [CallerLineNumber] int callerFileLine = 0, [CallerMemberName] string memberName = null, [CallerFilePath] string callerFilePath = null);

		public abstract ITenantConfigurationSession CreateTenantConfigurationSession(ConsistencyMode consistencyMode, Guid externalDirectoryOrganizationId, [CallerLineNumber] int callerFileLine = 0, [CallerMemberName] string memberName = null, [CallerFilePath] string callerFilePath = null);

		public abstract ITopologyConfigurationSession CreateTopologyConfigurationSession(ConsistencyMode consistencyMode, ADSessionSettings sessionSettings, [CallerLineNumber] int callerFileLine = 0, [CallerMemberName] string memberName = null, [CallerFilePath] string callerFilePath = null);

		public abstract ITopologyConfigurationSession CreateTopologyConfigurationSession(bool readOnly, ConsistencyMode consistencyMode, ADSessionSettings sessionSettings, [CallerLineNumber] int callerFileLine = 0, [CallerMemberName] string memberName = null, [CallerFilePath] string callerFilePath = null);

		public ITopologyConfigurationSession CreateTopologyConfigurationSession(string domainController, bool readOnly, ConsistencyMode consistencyMode, ADSessionSettings sessionSettings, [CallerLineNumber] int callerFileLine = 0, [CallerMemberName] string memberName = null, [CallerFilePath] string callerFilePath = null)
		{
			return this.CreateTopologyConfigurationSession(domainController, readOnly, consistencyMode, null, sessionSettings, callerFileLine, memberName, callerFilePath);
		}

		public abstract ITopologyConfigurationSession CreateTopologyConfigurationSession(string domainController, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, [CallerLineNumber] int callerFileLine = 0, [CallerMemberName] string memberName = null, [CallerFilePath] string callerFilePath = null);

		public abstract ITopologyConfigurationSession CreateTopologyConfigurationSession(string domainController, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, ConfigScopes configScope, [CallerLineNumber] int callerFileLine = 0, [CallerMemberName] string memberName = null, [CallerFilePath] string callerFilePath = null);

		public abstract ITenantRecipientSession CreateTenantRecipientSession(string domainController, ADObjectId searchRoot, int lcid, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, [CallerLineNumber] int callerFileLine = 0, [CallerMemberName] string memberName = null, [CallerFilePath] string callerFilePath = null);

		public abstract ITenantRecipientSession CreateTenantRecipientSession(string domainController, ADObjectId searchRoot, int lcid, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, ConfigScopes configScopes, [CallerLineNumber] int callerFileLine = 0, [CallerMemberName] string memberName = null, [CallerFilePath] string callerFilePath = null);

		public ITenantRecipientSession CreateTenantRecipientSession(bool readOnly, ConsistencyMode consistencyMode, ADSessionSettings sessionSettings, [CallerLineNumber] int callerFileLine = 0, [CallerMemberName] string memberName = null, [CallerFilePath] string callerFilePath = null)
		{
			return this.CreateTenantRecipientSession(null, null, CultureInfo.CurrentCulture.LCID, readOnly, consistencyMode, null, sessionSettings, callerFileLine, memberName, callerFilePath);
		}

		public abstract IRootOrganizationRecipientSession CreateRootOrgRecipientSession(string domainController, ADObjectId searchRoot, int lcid, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, ConfigScopes configScopes, [CallerLineNumber] int callerFileLine = 0, [CallerMemberName] string memberName = null, [CallerFilePath] string callerFilePath = null);

		public abstract IRootOrganizationRecipientSession CreateRootOrgRecipientSession(string domainController, ADObjectId searchRoot, int lcid, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, [CallerLineNumber] int callerFileLine = 0, [CallerMemberName] string memberName = null, [CallerFilePath] string callerFilePath = null);

		public IRootOrganizationRecipientSession CreateRootOrgRecipientSession(ConsistencyMode consistencyMode, ADSessionSettings sessionSettings, [CallerLineNumber] int callerFileLine = 0, [CallerMemberName] string memberName = null, [CallerFilePath] string callerFilePath = null)
		{
			return this.CreateRootOrgRecipientSession(true, consistencyMode, sessionSettings, callerFileLine, memberName, callerFilePath);
		}

		public IRootOrganizationRecipientSession CreateRootOrgRecipientSession(bool readOnly, ConsistencyMode consistencyMode, ADSessionSettings sessionSettings, [CallerLineNumber] int callerFileLine = 0, [CallerMemberName] string memberName = null, [CallerFilePath] string callerFilePath = null)
		{
			return this.CreateRootOrgRecipientSession(null, null, CultureInfo.CurrentCulture.LCID, readOnly, consistencyMode, null, sessionSettings, callerFileLine, memberName, callerFilePath);
		}

		public IConfigurationSession GetTenantOrTopologyConfigurationSession(ConsistencyMode consistencyMode, ADSessionSettings sessionSettings, [CallerLineNumber] int callerFileLine = 0, [CallerMemberName] string memberName = null, [CallerFilePath] string callerFilePath = null)
		{
			return this.GetTenantOrTopologyConfigurationSession(true, consistencyMode, sessionSettings, callerFileLine, memberName, callerFilePath);
		}

		public IConfigurationSession GetTenantOrTopologyConfigurationSession(bool readOnly, ConsistencyMode consistencyMode, ADSessionSettings sessionSettings, [CallerLineNumber] int callerFileLine = 0, [CallerMemberName] string memberName = null, [CallerFilePath] string callerFilePath = null)
		{
			return this.GetTenantOrTopologyConfigurationSession(null, readOnly, consistencyMode, null, sessionSettings, callerFileLine, memberName, callerFilePath);
		}

		public IConfigurationSession GetTenantOrTopologyConfigurationSession(string domainController, bool readOnly, ConsistencyMode consistencyMode, ADSessionSettings sessionSettings, [CallerLineNumber] int callerFileLine = 0, [CallerMemberName] string memberName = null, [CallerFilePath] string callerFilePath = null)
		{
			return this.GetTenantOrTopologyConfigurationSession(domainController, readOnly, consistencyMode, null, sessionSettings, callerFileLine, memberName, callerFilePath);
		}

		public IConfigurationSession GetTenantOrTopologyConfigurationSession(string domainController, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, [CallerLineNumber] int callerFileLine = 0, [CallerMemberName] string memberName = null, [CallerFilePath] string callerFilePath = null)
		{
			if (sessionSettings.CurrentOrganizationId == OrganizationId.ForestWideOrgId && sessionSettings.ConfigScopes != ConfigScopes.AllTenants)
			{
				return this.CreateTopologyConfigurationSession(domainController, readOnly, consistencyMode, networkCredential, sessionSettings, callerFileLine, memberName, callerFilePath);
			}
			return this.CreateTenantConfigurationSession(domainController, readOnly, consistencyMode, networkCredential, sessionSettings, callerFileLine, memberName, callerFilePath);
		}

		public IConfigurationSession GetTenantOrTopologyConfigurationSession(string domainController, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, ConfigScopes configScope, [CallerLineNumber] int callerFileLine = 0, [CallerMemberName] string memberName = null, [CallerFilePath] string callerFilePath = null)
		{
			if (sessionSettings.CurrentOrganizationId == OrganizationId.ForestWideOrgId && sessionSettings.ConfigScopes != ConfigScopes.AllTenants)
			{
				return this.CreateTopologyConfigurationSession(domainController, readOnly, consistencyMode, networkCredential, sessionSettings, configScope, callerFileLine, memberName, callerFilePath);
			}
			return this.CreateTenantConfigurationSession(domainController, readOnly, consistencyMode, networkCredential, sessionSettings, configScope, callerFileLine, memberName, callerFilePath);
		}

		public IRecipientSession GetTenantOrRootOrgRecipientSession(ConsistencyMode consistencyMode, ADSessionSettings sessionSettings, [CallerLineNumber] int callerFileLine = 0, [CallerMemberName] string memberName = null, [CallerFilePath] string callerFilePath = null)
		{
			return this.GetTenantOrRootOrgRecipientSession(true, consistencyMode, sessionSettings, callerFileLine, memberName, callerFilePath);
		}

		public IRecipientSession GetTenantOrRootOrgRecipientSession(bool readOnly, ConsistencyMode consistencyMode, ADSessionSettings sessionSettings, [CallerLineNumber] int callerFileLine = 0, [CallerMemberName] string memberName = null, [CallerFilePath] string callerFilePath = null)
		{
			return this.GetTenantOrRootOrgRecipientSession(null, null, CultureInfo.CurrentCulture.LCID, readOnly, consistencyMode, null, sessionSettings, callerFileLine, memberName, callerFilePath);
		}

		public IRecipientSession GetTenantOrRootOrgRecipientSession(string domainController, bool readOnly, ConsistencyMode consistencyMode, ADSessionSettings sessionSettings, [CallerLineNumber] int callerFileLine = 0, [CallerMemberName] string memberName = null, [CallerFilePath] string callerFilePath = null)
		{
			return this.GetTenantOrRootOrgRecipientSession(domainController, null, CultureInfo.CurrentCulture.LCID, readOnly, consistencyMode, null, sessionSettings, callerFileLine, memberName, callerFilePath);
		}

		public IRecipientSession GetTenantOrRootOrgRecipientSession(string domainController, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, [CallerLineNumber] int callerFileLine = 0, [CallerMemberName] string memberName = null, [CallerFilePath] string callerFilePath = null)
		{
			return this.GetTenantOrRootOrgRecipientSession(domainController, null, CultureInfo.CurrentCulture.LCID, readOnly, consistencyMode, networkCredential, sessionSettings, callerFileLine, memberName, callerFilePath);
		}

		public IRecipientSession GetTenantOrRootOrgRecipientSession(string domainController, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, ConfigScopes configScope, [CallerLineNumber] int callerFileLine = 0, [CallerMemberName] string memberName = null, [CallerFilePath] string callerFilePath = null)
		{
			return this.GetTenantOrRootOrgRecipientSession(domainController, null, CultureInfo.CurrentCulture.LCID, readOnly, consistencyMode, networkCredential, sessionSettings, configScope, callerFileLine, memberName, callerFilePath);
		}

		public IRecipientSession GetTenantOrRootOrgRecipientSession(string domainController, ADObjectId searchRoot, int lcid, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, [CallerLineNumber] int callerFileLine = 0, [CallerMemberName] string memberName = null, [CallerFilePath] string callerFilePath = null)
		{
			if (sessionSettings.CurrentOrganizationId == OrganizationId.ForestWideOrgId && sessionSettings.ConfigScopes != ConfigScopes.AllTenants)
			{
				return this.CreateRootOrgRecipientSession(domainController, searchRoot, lcid, readOnly, consistencyMode, networkCredential, sessionSettings, callerFileLine, memberName, callerFilePath);
			}
			return this.CreateTenantRecipientSession(domainController, searchRoot, lcid, readOnly, consistencyMode, networkCredential, sessionSettings, callerFileLine, memberName, callerFilePath);
		}

		public IRecipientSession GetTenantOrRootOrgRecipientSession(string domainController, ADObjectId searchRoot, int lcid, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, ConfigScopes configScope, [CallerLineNumber] int callerFileLine = 0, [CallerMemberName] string memberName = null, [CallerFilePath] string callerFilePath = null)
		{
			if (sessionSettings.CurrentOrganizationId == OrganizationId.ForestWideOrgId && sessionSettings.ConfigScopes != ConfigScopes.AllTenants)
			{
				return this.CreateRootOrgRecipientSession(domainController, searchRoot, lcid, readOnly, consistencyMode, networkCredential, sessionSettings, configScope, callerFileLine, memberName, callerFilePath);
			}
			return this.CreateTenantRecipientSession(domainController, searchRoot, lcid, readOnly, consistencyMode, networkCredential, sessionSettings, configScope, callerFileLine, memberName, callerFilePath);
		}

		public IRecipientSession GetTenantOrRootRecipientReadOnlySession(IRecipientSession recipientSession, string domainController = null, [CallerLineNumber] int callerFileLine = 0, [CallerMemberName] string memberName = null, [CallerFilePath] string callerFilePath = null)
		{
			IRecipientSession tenantOrRootOrgRecipientSession = this.GetTenantOrRootOrgRecipientSession(null, recipientSession.SearchRoot, recipientSession.Lcid, true, recipientSession.ConsistencyMode, recipientSession.NetworkCredential, recipientSession.SessionSettings, callerFileLine, memberName, callerFilePath);
			tenantOrRootOrgRecipientSession.DomainController = domainController;
			return tenantOrRootOrgRecipientSession;
		}

		public abstract IRecipientSession GetReducedRecipientSession(IRecipientSession baseSession, [CallerLineNumber] int callerFileLine = 0, [CallerMemberName] string memberName = null, [CallerFilePath] string callerFilePath = null);

		internal static object InstantiateFfoOrExoClass(string ffoTypeName, Type exoType)
		{
			if (DatacenterRegistry.IsForefrontForOffice() && !DatacenterRegistry.IsForefrontForOfficeDeployment())
			{
				Assembly assembly = Assembly.Load("Microsoft.Exchange.Hygiene.Data");
				Type type = assembly.GetType(ffoTypeName);
				return Activator.CreateInstance(type);
			}
			return Activator.CreateInstance(exoType);
		}

		[Conditional("DEBUG")]
		private static void Dbg_CheckCallStackForNonDefaultSession()
		{
		}

		private static DirectorySessionFactory nonCacheSessionFactory;

		private static DirectorySessionFactory cacheSessionFactory;

		private static DirectorySessionFactory cacheFallbackSessionFactory;

		private static DirectorySessionFactory mockDefault;
	}
}
