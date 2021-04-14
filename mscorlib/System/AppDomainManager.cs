using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Hosting;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Threading;

namespace System
{
	[SecurityCritical]
	[ComVisible(true)]
	[SecurityPermission(SecurityAction.InheritanceDemand, Flags = SecurityPermissionFlag.Infrastructure)]
	public class AppDomainManager : MarshalByRefObject
	{
		[SecurityCritical]
		public virtual AppDomain CreateDomain(string friendlyName, Evidence securityInfo, AppDomainSetup appDomainInfo)
		{
			return AppDomainManager.CreateDomainHelper(friendlyName, securityInfo, appDomainInfo);
		}

		[SecurityCritical]
		[SecurityPermission(SecurityAction.Demand, ControlAppDomain = true)]
		protected static AppDomain CreateDomainHelper(string friendlyName, Evidence securityInfo, AppDomainSetup appDomainInfo)
		{
			if (friendlyName == null)
			{
				throw new ArgumentNullException(Environment.GetResourceString("ArgumentNull_String"));
			}
			if (securityInfo != null)
			{
				new SecurityPermission(SecurityPermissionFlag.ControlEvidence).Demand();
				AppDomain.CheckDomainCreationEvidence(appDomainInfo, securityInfo);
			}
			if (appDomainInfo == null)
			{
				appDomainInfo = new AppDomainSetup();
			}
			if (appDomainInfo.AppDomainManagerAssembly == null || appDomainInfo.AppDomainManagerType == null)
			{
				string appDomainManagerAssembly;
				string appDomainManagerType;
				AppDomain.CurrentDomain.GetAppDomainManagerType(out appDomainManagerAssembly, out appDomainManagerType);
				if (appDomainInfo.AppDomainManagerAssembly == null)
				{
					appDomainInfo.AppDomainManagerAssembly = appDomainManagerAssembly;
				}
				if (appDomainInfo.AppDomainManagerType == null)
				{
					appDomainInfo.AppDomainManagerType = appDomainManagerType;
				}
			}
			if (appDomainInfo.TargetFrameworkName == null)
			{
				appDomainInfo.TargetFrameworkName = AppDomain.CurrentDomain.GetTargetFrameworkName();
			}
			return AppDomain.nCreateDomain(friendlyName, appDomainInfo, securityInfo, (securityInfo == null) ? AppDomain.CurrentDomain.InternalEvidence : null, AppDomain.CurrentDomain.GetSecurityDescriptor());
		}

		[SecurityCritical]
		public virtual void InitializeNewDomain(AppDomainSetup appDomainInfo)
		{
		}

		public AppDomainManagerInitializationOptions InitializationFlags
		{
			get
			{
				return this.m_flags;
			}
			set
			{
				this.m_flags = value;
			}
		}

		public virtual ApplicationActivator ApplicationActivator
		{
			get
			{
				if (this.m_appActivator == null)
				{
					this.m_appActivator = new ApplicationActivator();
				}
				return this.m_appActivator;
			}
		}

		public virtual HostSecurityManager HostSecurityManager
		{
			get
			{
				return null;
			}
		}

		public virtual HostExecutionContextManager HostExecutionContextManager
		{
			get
			{
				return HostExecutionContextManager.GetInternalHostExecutionContextManager();
			}
		}

		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern void GetEntryAssembly(ObjectHandleOnStack retAssembly);

		public virtual Assembly EntryAssembly
		{
			[SecurityCritical]
			get
			{
				if (this.m_entryAssembly == null)
				{
					AppDomain currentDomain = AppDomain.CurrentDomain;
					if (currentDomain.IsDefaultAppDomain() && currentDomain.ActivationContext != null)
					{
						ManifestRunner manifestRunner = new ManifestRunner(currentDomain, currentDomain.ActivationContext);
						this.m_entryAssembly = manifestRunner.EntryAssembly;
					}
					else
					{
						RuntimeAssembly entryAssembly = null;
						AppDomainManager.GetEntryAssembly(JitHelpers.GetObjectHandleOnStack<RuntimeAssembly>(ref entryAssembly));
						this.m_entryAssembly = entryAssembly;
					}
				}
				return this.m_entryAssembly;
			}
		}

		internal static AppDomainManager CurrentAppDomainManager
		{
			[SecurityCritical]
			get
			{
				return AppDomain.CurrentDomain.DomainManager;
			}
		}

		public virtual bool CheckSecuritySettings(SecurityState state)
		{
			return false;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasHost();

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern void RegisterWithHost(IntPtr appDomainManager);

		internal void RegisterWithHost()
		{
			if (AppDomainManager.HasHost())
			{
				IntPtr intPtr = IntPtr.Zero;
				RuntimeHelpers.PrepareConstrainedRegions();
				try
				{
					intPtr = Marshal.GetIUnknownForObject(this);
					AppDomainManager.RegisterWithHost(intPtr);
				}
				finally
				{
					if (!intPtr.IsNull())
					{
						Marshal.Release(intPtr);
					}
				}
			}
		}

		private AppDomainManagerInitializationOptions m_flags;

		private ApplicationActivator m_appActivator;

		private Assembly m_entryAssembly;
	}
}
