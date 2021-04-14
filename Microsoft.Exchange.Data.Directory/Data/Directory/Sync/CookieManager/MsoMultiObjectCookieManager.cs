using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory.EventLog;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Sync.CookieManager
{
	internal class MsoMultiObjectCookieManager : DeltaSyncCookieManager
	{
		internal MsoMultiObjectCookieManager(string serviceInstanceName, int maxCookieHistoryCount, TimeSpan cookieHistoryInterval, ForwardSyncCookieType cookieType) : base(serviceInstanceName, maxCookieHistoryCount, cookieHistoryInterval)
		{
			this.cookieSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 71, ".ctor", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\Sync\\CookieManager\\MsoMultiObjectCookieManager.cs");
			this.cookieSession.UseConfigNC = false;
			this.cookieType = cookieType;
			this.cookieRootContainer = MsoMultiObjectCookieManager.CreateOrGetRootContainer(serviceInstanceName, this.cookieSession);
			ForwardSyncCookieHeader[] source = MsoMultiObjectCookieManager.LoadCookieHeaders(this.cookieSession, serviceInstanceName, this.cookieType);
			this.cookieHeaderList = source.ToList<ForwardSyncCookieHeader>();
		}

		public static ForwardSyncCookieHeader[] LoadCookieHeaders(ITopologyConfigurationSession cookieSession, string serviceInstanceName, ForwardSyncCookieType cookieType)
		{
			ADObjectId serviceInstanceObjectId = SyncServiceInstance.GetServiceInstanceObjectId(serviceInstanceName);
			Container container = cookieSession.Read<Container>(serviceInstanceObjectId.GetChildId("Cookies"));
			if (container != null)
			{
				ComparisonFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ForwardSyncCookieHeaderSchema.Type, cookieType);
				ForwardSyncCookieHeader[] array = cookieSession.Find<ForwardSyncCookieHeader>(container.Id, QueryScope.OneLevel, filter, null, 0);
				Array.Sort<ForwardSyncCookieHeader>(array, (ForwardSyncCookieHeader x, ForwardSyncCookieHeader y) => DateTime.Compare(x.Timestamp, y.Timestamp));
				return array;
			}
			return new ForwardSyncCookieHeader[0];
		}

		private static ADObjectId CreateOrGetRootContainer(string serviceInstanceName, ITopologyConfigurationSession cookieSession)
		{
			ADObjectId serviceInstanceObjectId = SyncServiceInstance.GetServiceInstanceObjectId(serviceInstanceName);
			Container container = cookieSession.Read<Container>(serviceInstanceObjectId.GetChildId("Cookies"));
			if (container == null)
			{
				container = new Container();
				container.SetId(serviceInstanceObjectId.GetChildId("Cookies"));
				try
				{
					cookieSession.Save(container);
				}
				catch (ADObjectAlreadyExistsException)
				{
				}
			}
			return container.Id;
		}

		public ForwardSyncCookie ReadMostRecentCookie()
		{
			if (this.mostRecentCookie == null && this.cookieHeaderList.Count > 0)
			{
				ForwardSyncCookieHeader forwardSyncCookieHeader = this.cookieHeaderList.Last<ForwardSyncCookieHeader>();
				this.mostRecentCookie = this.cookieSession.Read<ForwardSyncCookie>(forwardSyncCookieHeader.Id);
				this.lastNewCookieTimestamp = this.mostRecentCookie.Timestamp;
				base.SyncPropertySetVersion = new ServerVersion(this.mostRecentCookie.SyncPropertySetVersion);
				base.IsSyncPropertySetUpgrading = this.mostRecentCookie.IsUpgradingSyncPropertySet;
			}
			return this.mostRecentCookie;
		}

		public override byte[] ReadCookie()
		{
			ForwardSyncCookie forwardSyncCookie = this.ReadMostRecentCookie();
			if (forwardSyncCookie == null)
			{
				return null;
			}
			return forwardSyncCookie.Data;
		}

		public override DateTime? GetMostRecentCookieTimestamp()
		{
			ForwardSyncCookie forwardSyncCookie = this.ReadMostRecentCookie();
			if (forwardSyncCookie != null)
			{
				return new DateTime?(forwardSyncCookie.Timestamp);
			}
			return null;
		}

		public override void WriteCookie(byte[] cookie, IEnumerable<string> filteredContextIds, DateTime timestamp, bool isUpgradingCookie, ServerVersion version, bool more)
		{
			if (cookie == null || cookie.Length == 0)
			{
				throw new ArgumentNullException("cookie");
			}
			base.UpdateSyncPropertySetVersion(isUpgradingCookie, version, more);
			ForwardSyncCookie forwardSyncCookie = this.ReadMostRecentCookie();
			bool flag = forwardSyncCookie == null || timestamp.Subtract(this.lastNewCookieTimestamp) >= this.cookieHistoryInterval;
			bool flag2 = false;
			if (flag)
			{
				if (this.cookieHeaderList.Count > this.maxCookieHistoryCount)
				{
					this.CleanupOldCookies();
				}
				else
				{
					flag2 = (this.cookieHeaderList.Count < this.maxCookieHistoryCount);
				}
				forwardSyncCookie = new ForwardSyncCookie();
				if (flag2)
				{
					string unescapedCommonName = string.Format("{0}-{1}", Enum.GetName(typeof(ForwardSyncCookieType), this.cookieType), timestamp.Ticks);
					forwardSyncCookie.SetId(this.cookieRootContainer.GetChildId(unescapedCommonName));
				}
				else
				{
					forwardSyncCookie.SetId(this.cookieHeaderList.First<ForwardSyncCookieHeader>().Id);
					forwardSyncCookie.Name = forwardSyncCookie.Id.Name;
					forwardSyncCookie.m_Session = this.cookieSession;
					forwardSyncCookie.ResetChangeTracking(true);
				}
				forwardSyncCookie.Type = this.cookieType;
				forwardSyncCookie.Version = 1;
			}
			forwardSyncCookie.Timestamp = timestamp;
			forwardSyncCookie.Data = cookie;
			if (filteredContextIds != null)
			{
				forwardSyncCookie.FilteredContextIds = new MultiValuedProperty<string>(filteredContextIds);
			}
			forwardSyncCookie.IsUpgradingSyncPropertySet = base.IsSyncPropertySetUpgrading;
			forwardSyncCookie.SyncPropertySetVersion = base.SyncPropertySetVersion.ToInt();
			this.cookieSession.Save(forwardSyncCookie);
			this.mostRecentCookie = forwardSyncCookie;
			if (flag)
			{
				ForwardSyncCookieHeader forwardSyncCookieHeader = new ForwardSyncCookieHeader
				{
					Name = forwardSyncCookie.Name,
					Timestamp = forwardSyncCookie.Timestamp
				};
				forwardSyncCookieHeader.SetId(forwardSyncCookie.Id);
				this.cookieHeaderList.Add(forwardSyncCookieHeader);
				this.lastNewCookieTimestamp = timestamp;
				if (!flag2)
				{
					this.cookieHeaderList.RemoveAt(0);
				}
			}
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

		private void CleanupOldCookies()
		{
			while (this.cookieHeaderList.Count > this.maxCookieHistoryCount)
			{
				try
				{
					this.cookieSession.Delete(this.cookieHeaderList.First<ForwardSyncCookieHeader>());
					this.cookieHeaderList.RemoveAt(0);
				}
				catch (Exception ex)
				{
					Globals.LogEvent(DirectoryEventLogConstants.Tuple_FailedToCleanupCookies, base.ServiceInstanceName, new object[]
					{
						ex
					});
					break;
				}
			}
		}

		private const int CookieVersion = 1;

		public const string CookieContainerName = "Cookies";

		private readonly ITopologyConfigurationSession cookieSession;

		private readonly List<ForwardSyncCookieHeader> cookieHeaderList;

		private readonly ADObjectId cookieRootContainer;

		private readonly ForwardSyncCookieType cookieType;

		private ForwardSyncCookie mostRecentCookie;

		private DateTime lastNewCookieTimestamp;
	}
}
