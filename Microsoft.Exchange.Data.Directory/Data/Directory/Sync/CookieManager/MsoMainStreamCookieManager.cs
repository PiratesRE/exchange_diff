using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory.Sync.CookieManager
{
	internal abstract class MsoMainStreamCookieManager : DeltaSyncCookieManager
	{
		protected MsoMainStreamCookieManager(string serviceInstanceName, int maxCookieHistoryCount, TimeSpan cookieHistoryInterval) : base(serviceInstanceName, maxCookieHistoryCount, cookieHistoryInterval)
		{
			ADServer adserver = this.FindRidMasterDomainController();
			if (adserver == null)
			{
				ExTraceGlobals.ADTopologyTracer.TraceError((long)this.GetHashCode(), "RID master not found");
				throw new InvalidOperationException("RID master not found");
			}
			ExTraceGlobals.ADTopologyTracer.TraceDebug<string>((long)this.GetHashCode(), "ridMaster.DnsHostName \"{0}\"", adserver.DnsHostName);
			this.cookieSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(adserver.DnsHostName, false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 56, ".ctor", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\Sync\\CookieManager\\MsoMainStreamCookieManager.cs");
			this.cookieSession.UseConfigNC = false;
		}

		public override byte[] ReadCookie()
		{
			MsoMainStreamCookie msoMainStreamCookie = this.ReadMostRecentCookie();
			if (msoMainStreamCookie == null)
			{
				return null;
			}
			return msoMainStreamCookie.RawCookie;
		}

		public override void WriteCookie(byte[] cookie, IEnumerable<string> filteredContextIds, DateTime timestamp, bool isUpgradingCookie, ServerVersion version, bool more)
		{
			if (cookie == null || cookie.Length == 0)
			{
				throw new ArgumentNullException("cookie");
			}
			MsoMainStreamCookieContainer msoMainStreamCookieContainer = this.cookieSession.GetMsoMainStreamCookieContainer(base.ServiceInstanceName);
			MultiValuedProperty<byte[]> multiValuedProperty = this.RetrievePersistedCookies(msoMainStreamCookieContainer);
			List<MsoMainStreamCookieWithIndex> list = new List<MsoMainStreamCookieWithIndex>();
			for (int i = multiValuedProperty.Count - 1; i >= 0; i--)
			{
				MsoMainStreamCookie msoMainStreamCookie = null;
				Exception ex = null;
				if (MsoMainStreamCookie.TryFromStorageCookie(multiValuedProperty[i], out msoMainStreamCookie, out ex) && string.Equals(base.ServiceInstanceName, msoMainStreamCookie.ServiceInstanceName, StringComparison.OrdinalIgnoreCase))
				{
					list.Add(new MsoMainStreamCookieWithIndex(msoMainStreamCookie, i));
				}
			}
			List<MsoMainStreamCookieWithIndex> list2 = this.SortMainStreamCookieList(list);
			if (list2.Count >= 2)
			{
				MsoMainStreamCookieWithIndex msoMainStreamCookieWithIndex = list2[list2.Count - 1];
				MsoMainStreamCookieWithIndex msoMainStreamCookieWithIndex2 = list2[list2.Count - 2];
				if (msoMainStreamCookieWithIndex.Cookie.TimeStamp < new DateTime(msoMainStreamCookieWithIndex2.Cookie.TimeStamp.Ticks + this.cookieHistoryInterval.Ticks, DateTimeKind.Utc))
				{
					multiValuedProperty.RemoveAt(msoMainStreamCookieWithIndex.Index);
				}
				else if (list2.Count >= this.maxCookieHistoryCount)
				{
					multiValuedProperty.RemoveAt(list2[0].Index);
				}
			}
			base.UpdateSyncPropertySetVersion(isUpgradingCookie, version, more);
			MsoMainStreamCookie msoMainStreamCookie2 = new MsoMainStreamCookie(base.ServiceInstanceName, cookie, timestamp, base.SyncPropertySetVersion, base.IsSyncPropertySetUpgrading);
			multiValuedProperty.Add(msoMainStreamCookie2.ToStorageCookie());
			this.cookieSession.Save(msoMainStreamCookieContainer);
			this.LogPersistCookieEvent();
		}

		public override string DomainController
		{
			get
			{
				if (this.cookieSession != null)
				{
					return this.cookieSession.DomainController;
				}
				return string.Empty;
			}
		}

		public override DateTime? GetMostRecentCookieTimestamp()
		{
			MsoMainStreamCookie msoMainStreamCookie = this.ReadMostRecentCookie();
			if (msoMainStreamCookie != null)
			{
				return new DateTime?(msoMainStreamCookie.TimeStamp);
			}
			return null;
		}

		public MultiValuedProperty<byte[]> RetrievePersistedCookies()
		{
			MsoMainStreamCookieContainer msoMainStreamCookieContainer = this.cookieSession.GetMsoMainStreamCookieContainer(base.ServiceInstanceName);
			return this.RetrievePersistedCookies(msoMainStreamCookieContainer);
		}

		protected abstract MultiValuedProperty<byte[]> RetrievePersistedCookies(MsoMainStreamCookieContainer container);

		protected abstract void LogPersistCookieEvent();

		private List<MsoMainStreamCookieWithIndex> SortMainStreamCookieList(List<MsoMainStreamCookieWithIndex> cookieList)
		{
			List<MsoMainStreamCookieWithIndex> list = new List<MsoMainStreamCookieWithIndex>();
			IOrderedEnumerable<MsoMainStreamCookieWithIndex> orderedEnumerable = from cookieWithIndex in cookieList
			orderby cookieWithIndex.Cookie.TimeStamp
			select cookieWithIndex;
			foreach (MsoMainStreamCookieWithIndex item in orderedEnumerable)
			{
				list.Add(item);
			}
			return list;
		}

		public MsoMainStreamCookie ReadMostRecentCookie()
		{
			MsoMainStreamCookieContainer msoMainStreamCookieContainer = this.cookieSession.GetMsoMainStreamCookieContainer(base.ServiceInstanceName);
			MultiValuedProperty<byte[]> multiValuedProperty = this.RetrievePersistedCookies(msoMainStreamCookieContainer);
			if (multiValuedProperty.Count == 0)
			{
				return null;
			}
			MsoMainStreamCookie msoMainStreamCookie = null;
			foreach (byte[] storageCookie in multiValuedProperty)
			{
				MsoMainStreamCookie msoMainStreamCookie2 = null;
				Exception ex = null;
				if (MsoMainStreamCookie.TryFromStorageCookie(storageCookie, out msoMainStreamCookie2, out ex) && string.Equals(base.ServiceInstanceName, msoMainStreamCookie2.ServiceInstanceName, StringComparison.OrdinalIgnoreCase) && (msoMainStreamCookie == null || msoMainStreamCookie.TimeStamp < msoMainStreamCookie2.TimeStamp))
				{
					msoMainStreamCookie = msoMainStreamCookie2;
				}
			}
			if (msoMainStreamCookie != null)
			{
				base.SyncPropertySetVersion = msoMainStreamCookie.SyncPropertySetVersion;
				base.IsSyncPropertySetUpgrading = msoMainStreamCookie.IsSyncPropertySetUpgrading;
				return msoMainStreamCookie;
			}
			return null;
		}

		private ADServer FindRidMasterDomainController()
		{
			ExTraceGlobals.ADTopologyTracer.TraceDebug((long)this.GetHashCode(), "Find RID master domain controller ...");
			ADServer result = null;
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 273, "FindRidMasterDomainController", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\Sync\\CookieManager\\MsoMainStreamCookieManager.cs");
			tenantOrTopologyConfigurationSession.UseConfigNC = false;
			RidManagerContainer[] array = tenantOrTopologyConfigurationSession.Find<RidManagerContainer>(null, QueryScope.SubTree, null, null, 1);
			if (array != null && array.Length > 0)
			{
				ExTraceGlobals.ADTopologyTracer.TraceDebug<int>((long)this.GetHashCode(), "ridManagerContainer.Length = {0}", array.Length);
				tenantOrTopologyConfigurationSession.UseConfigNC = true;
				ADObjectId fsmoRoleOwner = array[0].FsmoRoleOwner;
				if (fsmoRoleOwner != null)
				{
					ExTraceGlobals.ADTopologyTracer.TraceDebug<string>((long)this.GetHashCode(), "ntdsId.DistinguishedName \"{0}\"", fsmoRoleOwner.DistinguishedName);
					result = tenantOrTopologyConfigurationSession.Read<ADServer>(fsmoRoleOwner.Parent);
				}
			}
			return result;
		}

		private ITopologyConfigurationSession cookieSession;
	}
}
