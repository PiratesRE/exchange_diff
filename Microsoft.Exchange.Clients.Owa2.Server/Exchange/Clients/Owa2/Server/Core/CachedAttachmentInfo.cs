using System;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Caching;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class CachedAttachmentInfo
	{
		private CachedAttachmentInfo(string mailboxSmtpAddress, string logonSmtpAddress, string logonDisplayName, string logonPuid, SecurityIdentifier logonSid, string cultureName, string ewsAttachmentId, string sessionId)
		{
			this.MailboxSmtpAddress = mailboxSmtpAddress;
			this.LogonSmtpAddress = logonSmtpAddress;
			this.LogonDisplayName = logonDisplayName;
			this.LogonPuid = logonPuid;
			this.LogonSid = logonSid;
			this.CultureName = cultureName;
			this.EwsAttachmentId = ewsAttachmentId;
			this.SessionId = sessionId;
			this.instanceId = Interlocked.Increment(ref CachedAttachmentInfo.nextInstanceId);
		}

		public string MailboxSmtpAddress { get; private set; }

		public string LogonSmtpAddress { get; private set; }

		public string LogonDisplayName { get; private set; }

		public string LogonPuid { get; private set; }

		public SecurityIdentifier LogonSid { get; private set; }

		public string CultureName { get; private set; }

		public string EwsAttachmentId { get; private set; }

		public string SessionId { get; private set; }

		public CobaltStore CobaltStore { get; set; }

		public int LockCount
		{
			get
			{
				return this.lockCount;
			}
		}

		public int InstanceId
		{
			get
			{
				return this.instanceId;
			}
		}

		public static CachedAttachmentInfo GetInstance(string mailboxSmtpAddress, string ewsAttachmentId, string sessionId, SecurityIdentifier logonSid, string cultureName)
		{
			string cacheKey = CachedAttachmentInfo.GetCacheKey(mailboxSmtpAddress, ewsAttachmentId);
			CachedAttachmentInfo cachedAttachmentInfo = CachedAttachmentInfo.GetFromCache(cacheKey);
			if (cachedAttachmentInfo != null)
			{
				OwaApplication.GetRequestDetailsLogger.Set(WacRequestHandlerMetadata.CacheHit, true);
				return cachedAttachmentInfo;
			}
			CachedAttachmentInfo result;
			lock (CachedAttachmentInfo.factorySynchronizer)
			{
				cachedAttachmentInfo = CachedAttachmentInfo.GetFromCache(cacheKey);
				if (cachedAttachmentInfo != null)
				{
					OwaApplication.GetRequestDetailsLogger.Set(WacRequestHandlerMetadata.CacheHit, true);
					result = cachedAttachmentInfo;
				}
				else
				{
					OwaApplication.GetRequestDetailsLogger.Set(WacRequestHandlerMetadata.CacheHit, false);
					string domain = ((SmtpAddress)mailboxSmtpAddress).Domain;
					string logonSmtpAddress;
					string logonDisplayName;
					string logonPuid;
					CachedAttachmentInfo.GetLogonUserInfo(logonSid, domain, out logonSmtpAddress, out logonDisplayName, out logonPuid);
					cachedAttachmentInfo = new CachedAttachmentInfo(mailboxSmtpAddress, logonSmtpAddress, logonDisplayName, logonPuid, logonSid, cultureName, ewsAttachmentId, sessionId);
					cachedAttachmentInfo.InsertIntoCache(cacheKey);
					result = cachedAttachmentInfo;
				}
			}
			return result;
		}

		public static string GetCacheKey(string primarySmtpAddress, string ewsAttachmentId)
		{
			return primarySmtpAddress + " " + ewsAttachmentId;
		}

		public static CachedAttachmentInfo GetFromCache(string key)
		{
			return (CachedAttachmentInfo)HttpRuntime.Cache.Get(key);
		}

		public void IncrementLockCount()
		{
			Interlocked.Increment(ref this.lockCount);
		}

		public void DecrementLockCount()
		{
			Interlocked.Decrement(ref this.lockCount);
		}

		public void InsertIntoCache(string key)
		{
			if (WacConfiguration.Instance.AccessTokenCacheTime > TimeSpan.FromSeconds(0.0))
			{
				HttpRuntime.Cache.Insert(key, this, null, Cache.NoAbsoluteExpiration, WacConfiguration.Instance.AccessTokenCacheTime, new CacheItemUpdateCallback(CachedAttachmentInfo.OnCacheEntryExpired));
			}
		}

		private static void OnCacheEntryExpired(string key, CacheItemUpdateReason reason, out object expensiveObject, out CacheDependency dependency, out DateTime absoluteExpiration, out TimeSpan slidingExpiration)
		{
			expensiveObject = null;
			dependency = null;
			absoluteExpiration = Cache.NoAbsoluteExpiration;
			slidingExpiration = Cache.NoSlidingExpiration;
			CachedAttachmentInfo cachedAttachmentInfo = (CachedAttachmentInfo)HttpRuntime.Cache.Get(key);
			if (cachedAttachmentInfo.CobaltStore == null)
			{
				return;
			}
			WacRequestHandler.OnCacheEntryExpired(cachedAttachmentInfo);
		}

		private static void GetLogonUserInfo(SecurityIdentifier sid, string smtpDomain, out string smtpAddress, out string displayName, out string puid)
		{
			string smtpAddressTemporary = sid.ToString();
			string displayNameTemporary = sid.ToString();
			string puidTemporary = string.Empty;
			ADNotificationAdapter.RunADOperation(delegate()
			{
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.RootOrgOrSingleTenantFromAcceptedDomainAutoDetect(smtpDomain), 339, "GetLogonUserInfo", "f:\\15.00.1497\\sources\\dev\\clients\\src\\Owa2\\Server\\Core\\attachment\\CachedAttachmentInfo.cs");
				PropertyDefinition[] properties = new PropertyDefinition[]
				{
					ADRecipientSchema.PrimarySmtpAddress,
					ADRecipientSchema.LegacyExchangeDN,
					ADRecipientSchema.DisplayName,
					ADUserSchema.NetID
				};
				ADRawEntry entry = tenantOrRootOrgRecipientSession.FindADRawEntryBySid(sid, properties);
				try
				{
					CachedAttachmentInfo.SafeGetValue(entry, ADRecipientSchema.PrimarySmtpAddress, ref smtpAddressTemporary);
					CachedAttachmentInfo.SafeGetValue(entry, ADRecipientSchema.DisplayName, ref displayNameTemporary);
					CachedAttachmentInfo.SafeGetValue(entry, ADUserSchema.NetID, ref puidTemporary);
				}
				catch (NotInBagPropertyErrorException)
				{
				}
			});
			smtpAddress = smtpAddressTemporary;
			displayName = displayNameTemporary;
			puid = puidTemporary;
		}

		private static void SafeGetValue(ADRawEntry entry, PropertyDefinition property, ref string result)
		{
			object obj = entry[property];
			if (obj == null)
			{
				return;
			}
			result = obj.ToString();
		}

		private static readonly object factorySynchronizer = new object();

		private static int nextInstanceId;

		private readonly int instanceId;

		private int lockCount;
	}
}
