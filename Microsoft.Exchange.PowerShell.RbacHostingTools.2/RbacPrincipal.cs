using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Principal;
using System.ServiceModel;
using System.Threading;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.Management.ControlPanel;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.PowerShell.RbacHostingTools
{
	internal class RbacPrincipal : IPrincipal, IIdentity, IExtension<OperationContext>, IIsInRole
	{
		public RbacPrincipal(ExchangeRunspaceConfiguration roles, string cacheKey)
		{
			if (roles == null)
			{
				throw new ArgumentNullException("roles");
			}
			if (string.IsNullOrEmpty(cacheKey))
			{
				throw new ArgumentNullException("cacheKey");
			}
			this.RbacConfiguration = roles;
			ADObjectId executingUserId;
			roles.TryGetExecutingUserId(out executingUserId);
			this.ExecutingUserId = executingUserId;
			MultiValuedProperty<CultureInfo> executingUserLanguages = this.RbacConfiguration.ExecutingUserLanguages;
			if (executingUserLanguages.Count > 0)
			{
				this.UserCulture = executingUserLanguages[0];
			}
			this.cacheKeys = new string[]
			{
				cacheKey
			};
			this.Name = this.RbacConfiguration.ExecutingUserDisplayName;
			if (string.IsNullOrEmpty(this.Name))
			{
				this.Name = this.ExecutingUserId.Name;
			}
		}

		public static RbacPrincipal Current
		{
			get
			{
				return RbacPrincipal.GetCurrent(true);
			}
		}

		public static RbacPrincipal GetCurrent(bool throwOnInvalidValue)
		{
			OperationContext operationContext = OperationContext.Current;
			if (operationContext != null)
			{
				return operationContext.GetRbacPrincipal();
			}
			RbacPrincipal.PrincipalWrapper principalWrapper = Thread.CurrentPrincipal as RbacPrincipal.PrincipalWrapper;
			if (throwOnInvalidValue)
			{
				return (Thread.CurrentPrincipal as RbacPrincipal) ?? ((RbacPrincipal)((RbacPrincipal.PrincipalWrapper)Thread.CurrentPrincipal).Principal);
			}
			if (principalWrapper != null)
			{
				return principalWrapper.Principal as RbacPrincipal;
			}
			return Thread.CurrentPrincipal as RbacPrincipal;
		}

		public static string[] SplitRoles(string roleString)
		{
			return roleString.Split(RbacPrincipal.andSeparatorList, StringSplitOptions.RemoveEmptyEntries);
		}

		public IIdentity Identity
		{
			get
			{
				return this;
			}
		}

		public string Name { get; internal set; }

		public string NameForEventLog
		{
			get
			{
				return this.RbacConfiguration.IdentityName;
			}
		}

		public string UniqueName
		{
			get
			{
				return this.RbacConfiguration.IdentityName;
			}
		}

		public ExchangeRunspaceConfiguration RbacConfiguration { get; private set; }

		public ADObjectId ExecutingUserId { get; private set; }

		public virtual string DateFormat { get; protected set; }

		public virtual string TimeFormat { get; protected set; }

		public virtual ExTimeZone UserTimeZone { get; protected set; }

		protected CultureInfo UserCulture { get; set; }

		public object OwaOptionsLock
		{
			get
			{
				return this.owaOptionsLock;
			}
		}

		public string[] CacheKeys
		{
			get
			{
				return this.cacheKeys;
			}
		}

		public bool IsAdmin
		{
			get
			{
				return this.RbacConfiguration.HasAdminRoles;
			}
		}

		string IIdentity.AuthenticationType
		{
			get
			{
				return null;
			}
		}

		bool IIdentity.IsAuthenticated
		{
			get
			{
				return true;
			}
		}

		public bool IsInRole(string role)
		{
			return this.IsInRole(role, null);
		}

		public bool IsInRole(string role, ADRawEntry adRawEntry)
		{
			if (string.IsNullOrEmpty(role))
			{
				throw new ArgumentNullException("role");
			}
			bool flag = false;
			string[] array = RbacPrincipal.SplitRoles(role);
			foreach (string text in array)
			{
				this.roleCacheLock.AcquireReaderLock(-1);
				try
				{
					if (adRawEntry != null || RbacQuery.LegacyIsScoped || !this.roleCache.TryGetValue(text, out flag))
					{
						bool flag2;
						flag = this.IsInRole(text, out flag2, adRawEntry);
						if (flag2)
						{
							LockCookie lockCookie = this.roleCacheLock.UpgradeToWriterLock(-1);
							try
							{
								this.roleCache[text] = flag;
							}
							finally
							{
								this.roleCacheLock.DowngradeFromWriterLock(ref lockCookie);
							}
						}
					}
				}
				finally
				{
					this.roleCacheLock.ReleaseReaderLock();
				}
				if (!flag)
				{
					ExTraceGlobals.RBACTracer.TraceInformation<string, string, string>(this.GetHashCode(), 0L, "RbacPrincipal({0}).IsInRole({1}) returning false because {2} is not granted.", this.NameForEventLog, role, text);
					break;
				}
			}
			return flag;
		}

		public virtual void SetCurrentThreadPrincipal()
		{
			Thread.CurrentPrincipal = this.GetWrapperPrincipal();
		}

		internal IPrincipal GetWrapperPrincipal()
		{
			if (this.wrapper == null)
			{
				Interlocked.CompareExchange<RbacPrincipal.PrincipalWrapper>(ref this.wrapper, new RbacPrincipal.PrincipalWrapper(this), null);
			}
			return this.wrapper;
		}

		void IExtension<OperationContext>.Attach(OperationContext owner)
		{
		}

		void IExtension<OperationContext>.Detach(OperationContext owner)
		{
		}

		protected virtual bool IsInRole(string rbacQuery, out bool canCache, ADRawEntry adRawEntry)
		{
			RbacQuery rbacQuery2 = new RbacQuery(rbacQuery, adRawEntry);
			bool result = rbacQuery2.IsInRole(this.RbacConfiguration);
			canCache = rbacQuery2.CanCache;
			return result;
		}

		public const string EnterpriseRole = "Enterprise";

		public const string LiveIDRole = "LiveID";

		public const string MultiTenantRole = "MultiTenant";

		private const char AndSeparator = '+';

		private static char[] andSeparatorList = new char[]
		{
			'+'
		};

		private object owaOptionsLock = new object();

		private Dictionary<string, bool> roleCache = new Dictionary<string, bool>(StringComparer.InvariantCultureIgnoreCase);

		private ReaderWriterLock roleCacheLock = new ReaderWriterLock();

		private string[] cacheKeys;

		private RbacPrincipal.PrincipalWrapper wrapper;

		private class PrincipalWrapper : IPrincipal
		{
			public PrincipalWrapper(IPrincipal principal)
			{
				this.refPricipal = new WeakReference(principal);
			}

			IIdentity IPrincipal.Identity
			{
				get
				{
					IPrincipal principal = this.Principal;
					if (principal != null)
					{
						return principal.Identity;
					}
					return null;
				}
			}

			bool IPrincipal.IsInRole(string role)
			{
				IPrincipal principal = this.Principal;
				return principal != null && principal.IsInRole(role);
			}

			public IPrincipal Principal
			{
				get
				{
					return (IPrincipal)this.refPricipal.Target;
				}
			}

			private readonly WeakReference refPricipal;
		}
	}
}
