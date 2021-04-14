using System;
using System.Text;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Sync.CookieManager
{
	internal abstract class MsoFullSyncCookieManager : FullSyncCookieManager
	{
		protected MsoFullSyncCookieManager(Guid contextId) : base(contextId)
		{
			this.configSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(null, false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromExternalDirectoryOrganizationId(contextId), 55, ".ctor", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\Sync\\CookieManager\\MsoFullSyncCookieManager.cs");
		}

		public override byte[] ReadCookie()
		{
			MsoTenantCookieContainer msoTenantCookieContainer = this.configSession.GetMsoTenantCookieContainer(base.ContextId);
			if (msoTenantCookieContainer == null)
			{
				return null;
			}
			MultiValuedProperty<byte[]> multiValuedProperty = this.RetrievePersistedPageTokens(msoTenantCookieContainer);
			if (multiValuedProperty.Count == 0)
			{
				return null;
			}
			base.LastCookie = MsoFullSyncCookie.FromStorageCookie(multiValuedProperty[0]);
			return base.LastCookie.RawCookie;
		}

		public override DateTime? GetMostRecentCookieTimestamp()
		{
			if (base.LastCookie == null)
			{
				this.ReadCookie();
			}
			if (base.LastCookie != null)
			{
				return new DateTime?(base.LastCookie.Timestamp);
			}
			return null;
		}

		public override void WriteCookie(byte[] cookie, DateTime timestamp)
		{
			if (cookie == null || cookie.Length == 0)
			{
				throw new ArgumentException("cookie is empty");
			}
			MsoTenantCookieContainer msoTenantCookieContainer = this.configSession.GetMsoTenantCookieContainer(base.ContextId);
			if (msoTenantCookieContainer != null)
			{
				int cookieVersion = (base.LastCookie != null) ? base.LastCookie.Version : 1;
				MsoFullSyncCookie msoFullSyncCookie = new MsoFullSyncCookie(cookie, cookieVersion);
				msoFullSyncCookie.Timestamp = timestamp;
				if (base.LastCookie != null)
				{
					msoFullSyncCookie.SyncType = base.LastCookie.SyncType;
					msoFullSyncCookie.SyncRequestor = base.LastCookie.SyncRequestor;
					msoFullSyncCookie.WhenSyncRequested = base.LastCookie.WhenSyncRequested;
					msoFullSyncCookie.WhenSyncStarted = ((base.LastCookie.WhenSyncStarted != DateTime.MinValue) ? base.LastCookie.WhenSyncStarted : timestamp);
				}
				byte[] item = msoFullSyncCookie.ToStorageCookie();
				MultiValuedProperty<byte[]> multiValuedProperty = this.RetrievePersistedPageTokens(msoTenantCookieContainer);
				multiValuedProperty.Clear();
				multiValuedProperty.Add(item);
				this.configSession.Save(msoTenantCookieContainer);
				this.LogPersistPageTokenEvent();
			}
		}

		public void WriteInitialSyncCookie(TenantSyncType type, string requestor)
		{
			string s = (type == TenantSyncType.Full) ? "start" : "start-partial-sync";
			base.LastCookie = new MsoFullSyncCookie(Encoding.UTF8.GetBytes(s), 3);
			base.LastCookie.SyncType = type;
			base.LastCookie.SyncRequestor = requestor;
			base.LastCookie.WhenSyncRequested = DateTime.UtcNow;
			this.WriteCookie(base.LastCookie.RawCookie, DateTime.MinValue);
		}

		public static bool IsInitialFullSyncCookie(byte[] cookie)
		{
			return MsoFullSyncCookieManager.CompareCookie(cookie, "start");
		}

		public static bool IsInitialPartialSyncCookie(byte[] cookie)
		{
			return MsoFullSyncCookieManager.CompareCookie(cookie, "start-partial-sync");
		}

		public override string DomainController
		{
			get
			{
				if (this.configSession != null)
				{
					return this.configSession.DomainController;
				}
				return string.Empty;
			}
		}

		private static bool CompareCookie(byte[] cookie, string syncCookieString)
		{
			if (cookie == null || cookie.Length == 0)
			{
				throw new ArgumentException("Cookie is empty.");
			}
			if (cookie.Length != syncCookieString.Length)
			{
				return false;
			}
			string @string = Encoding.UTF8.GetString(cookie);
			return string.Equals(@string, syncCookieString, StringComparison.OrdinalIgnoreCase);
		}

		public override void ClearCookie()
		{
			MsoTenantCookieContainer msoTenantCookieContainer = this.configSession.GetMsoTenantCookieContainer(base.ContextId);
			if (msoTenantCookieContainer != null)
			{
				MultiValuedProperty<byte[]> multiValuedProperty = this.RetrievePersistedPageTokens(msoTenantCookieContainer);
				multiValuedProperty.Clear();
				if (multiValuedProperty.Changed)
				{
					this.configSession.Save(msoTenantCookieContainer);
				}
				this.LogClearPageTokenEvent();
			}
		}

		protected abstract MultiValuedProperty<byte[]> RetrievePersistedPageTokens(MsoTenantCookieContainer container);

		protected abstract void LogPersistPageTokenEvent();

		protected abstract void LogClearPageTokenEvent();

		private const string StartFullSyncCookie = "start";

		private const string StartPartialSyncCookie = "start-partial-sync";

		private readonly ITenantConfigurationSession configSession;
	}
}
