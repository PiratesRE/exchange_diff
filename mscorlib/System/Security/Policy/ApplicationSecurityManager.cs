using System;
using System.Deployment.Internal.Isolation.Manifest;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Util;

namespace System.Security.Policy
{
	[ComVisible(true)]
	public static class ApplicationSecurityManager
	{
		[SecurityCritical]
		[SecurityPermission(SecurityAction.Assert, Unrestricted = true)]
		public static bool DetermineApplicationTrust(ActivationContext activationContext, TrustManagerContext context)
		{
			if (activationContext == null)
			{
				throw new ArgumentNullException("activationContext");
			}
			AppDomainManager domainManager = AppDomain.CurrentDomain.DomainManager;
			ApplicationTrust applicationTrust;
			if (domainManager != null)
			{
				HostSecurityManager hostSecurityManager = domainManager.HostSecurityManager;
				if (hostSecurityManager != null && (hostSecurityManager.Flags & HostSecurityManagerOptions.HostDetermineApplicationTrust) == HostSecurityManagerOptions.HostDetermineApplicationTrust)
				{
					applicationTrust = hostSecurityManager.DetermineApplicationTrust(CmsUtils.MergeApplicationEvidence(null, activationContext.Identity, activationContext, null), null, context);
					return applicationTrust != null && applicationTrust.IsApplicationTrustedToRun;
				}
			}
			applicationTrust = ApplicationSecurityManager.DetermineApplicationTrustInternal(activationContext, context);
			return applicationTrust != null && applicationTrust.IsApplicationTrustedToRun;
		}

		public static ApplicationTrustCollection UserApplicationTrusts
		{
			[SecuritySafeCritical]
			[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlPolicy)]
			get
			{
				return new ApplicationTrustCollection(true);
			}
		}

		public static IApplicationTrustManager ApplicationTrustManager
		{
			[SecuritySafeCritical]
			[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlPolicy)]
			get
			{
				if (ApplicationSecurityManager.m_appTrustManager == null)
				{
					ApplicationSecurityManager.m_appTrustManager = ApplicationSecurityManager.DecodeAppTrustManager();
					if (ApplicationSecurityManager.m_appTrustManager == null)
					{
						throw new PolicyException(Environment.GetResourceString("Policy_NoTrustManager"));
					}
				}
				return ApplicationSecurityManager.m_appTrustManager;
			}
		}

		[SecurityCritical]
		internal static ApplicationTrust DetermineApplicationTrustInternal(ActivationContext activationContext, TrustManagerContext context)
		{
			ApplicationTrustCollection applicationTrustCollection = new ApplicationTrustCollection(true);
			ApplicationTrust applicationTrust;
			if (context == null || !context.IgnorePersistedDecision)
			{
				applicationTrust = applicationTrustCollection[activationContext.Identity.FullName];
				if (applicationTrust != null)
				{
					return applicationTrust;
				}
			}
			applicationTrust = ApplicationSecurityManager.ApplicationTrustManager.DetermineApplicationTrust(activationContext, context);
			if (applicationTrust == null)
			{
				applicationTrust = new ApplicationTrust(activationContext.Identity);
			}
			applicationTrust.ApplicationIdentity = activationContext.Identity;
			if (applicationTrust.Persist)
			{
				applicationTrustCollection.Add(applicationTrust);
			}
			return applicationTrust;
		}

		[SecurityCritical]
		private static IApplicationTrustManager DecodeAppTrustManager()
		{
			if (File.InternalExists(ApplicationSecurityManager.s_machineConfigFile))
			{
				string xml;
				using (FileStream fileStream = new FileStream(ApplicationSecurityManager.s_machineConfigFile, FileMode.Open, FileAccess.Read))
				{
					xml = new StreamReader(fileStream).ReadToEnd();
				}
				SecurityElement securityElement = SecurityElement.FromString(xml);
				SecurityElement securityElement2 = securityElement.SearchForChildByTag("mscorlib");
				if (securityElement2 != null)
				{
					SecurityElement securityElement3 = securityElement2.SearchForChildByTag("security");
					if (securityElement3 != null)
					{
						SecurityElement securityElement4 = securityElement3.SearchForChildByTag("policy");
						if (securityElement4 != null)
						{
							SecurityElement securityElement5 = securityElement4.SearchForChildByTag("ApplicationSecurityManager");
							if (securityElement5 != null)
							{
								SecurityElement securityElement6 = securityElement5.SearchForChildByTag("IApplicationTrustManager");
								if (securityElement6 != null)
								{
									IApplicationTrustManager applicationTrustManager = ApplicationSecurityManager.DecodeAppTrustManagerFromElement(securityElement6);
									if (applicationTrustManager != null)
									{
										return applicationTrustManager;
									}
								}
							}
						}
					}
				}
			}
			return ApplicationSecurityManager.DecodeAppTrustManagerFromElement(ApplicationSecurityManager.CreateDefaultApplicationTrustManagerElement());
		}

		[SecurityCritical]
		private static SecurityElement CreateDefaultApplicationTrustManagerElement()
		{
			SecurityElement securityElement = new SecurityElement("IApplicationTrustManager");
			securityElement.AddAttribute("class", "System.Security.Policy.TrustManager, System.Windows.Forms, Version=" + ((RuntimeAssembly)Assembly.GetExecutingAssembly()).GetVersion() + ", Culture=neutral, PublicKeyToken=b77a5c561934e089");
			securityElement.AddAttribute("version", "1");
			return securityElement;
		}

		[SecurityCritical]
		private static IApplicationTrustManager DecodeAppTrustManagerFromElement(SecurityElement elTrustManager)
		{
			new ReflectionPermission(ReflectionPermissionFlag.MemberAccess).Assert();
			string typeName = elTrustManager.Attribute("class");
			Type type = Type.GetType(typeName, false, false);
			if (type == null)
			{
				return null;
			}
			IApplicationTrustManager applicationTrustManager = Activator.CreateInstance(type) as IApplicationTrustManager;
			if (applicationTrustManager != null)
			{
				applicationTrustManager.FromXml(elTrustManager);
			}
			return applicationTrustManager;
		}

		private static volatile IApplicationTrustManager m_appTrustManager = null;

		private static string s_machineConfigFile = Config.MachineDirectory + "applicationtrust.config";
	}
}
