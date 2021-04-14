using System;
using System.Management.Automation.Runspaces;
using System.Web;
using System.Web.Caching;
using Microsoft.PowerShell.HostingTools;

namespace Microsoft.Exchange.PowerShell.RbacHostingTools.Asp.Net
{
	internal class RbacRunspaceCache : RunspaceCache
	{
		protected virtual string GetSessionKey()
		{
			HttpContext.Current.Session["RunspaceWebCache"] = true;
			return "Exchange" + HttpContext.Current.Session.SessionID;
		}

		protected virtual DateTime AbsoluteExpiration
		{
			get
			{
				return Cache.NoAbsoluteExpiration;
			}
		}

		protected override bool AddRunspace(Runspace runspace)
		{
			CacheDependency dependencies = null;
			string[] dependencyCacheKeys = this.GetDependencyCacheKeys();
			if (dependencyCacheKeys != null)
			{
				dependencies = new CacheDependency(null, dependencyCacheKeys);
			}
			return null == HttpRuntime.Cache.Add(this.GetSessionKey(), runspace, dependencies, this.AbsoluteExpiration, this.SlidingExpiration, CacheItemPriority.High, new CacheItemRemovedCallback(this.Cache_ItemRemoved));
		}

		protected override Runspace RemoveRunspace()
		{
			return (Runspace)HttpRuntime.Cache.Remove(this.GetSessionKey());
		}

		private void Cache_ItemRemoved(string key, object value, CacheItemRemovedReason reason)
		{
			if (reason != CacheItemRemovedReason.Removed)
			{
				Runspace runspace = (Runspace)value;
				this.DisposeRunspace(runspace);
			}
		}

		protected virtual TimeSpan SlidingExpiration
		{
			get
			{
				return RbacSection.Instance.RbacRunspaceSlidingExpiration;
			}
		}

		protected virtual string[] GetDependencyCacheKeys()
		{
			return RbacPrincipal.Current.CacheKeys;
		}

		protected const string RunspaceName = "Exchange";
	}
}
