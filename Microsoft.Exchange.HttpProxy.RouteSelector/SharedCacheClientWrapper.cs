using System;
using System.Diagnostics;
using Microsoft.Exchange.HttpProxy.Routing;
using Microsoft.Exchange.HttpProxy.Routing.Providers;
using Microsoft.Exchange.HttpProxy.Routing.RoutingKeys;
using Microsoft.Exchange.SharedCache.Client;

namespace Microsoft.Exchange.HttpProxy.RouteSelector
{
	public class SharedCacheClientWrapper : ISharedCacheClient, ISharedCache
	{
		public SharedCacheClientWrapper(SharedCacheClient client)
		{
			if (client == null)
			{
				throw new ArgumentNullException("client");
			}
			this.client = client;
		}

		bool ISharedCache.TryGet(string key, out byte[] returnBytes, IRoutingDiagnostics diagnostics)
		{
			bool result;
			try
			{
				Stopwatch stopwatch = new Stopwatch();
				string text;
				bool flag;
				try
				{
					stopwatch.Start();
					flag = this.client.TryGet(key, out returnBytes, out text);
				}
				finally
				{
					stopwatch.Stop();
					diagnostics.AddSharedCacheLatency(TimeSpan.FromMilliseconds((double)stopwatch.ElapsedMilliseconds));
				}
				diagnostics.AddDiagnosticText(text);
				result = flag;
			}
			catch (CacheClientException innerException)
			{
				throw new SharedCacheException("SharedCacheClient.TryGet(byte[]) failed", innerException);
			}
			return result;
		}

		bool ISharedCache.TryGet<T>(string key, out T value, IRoutingDiagnostics diagnostics)
		{
			bool result;
			try
			{
				Stopwatch stopwatch = new Stopwatch();
				string text;
				bool flag;
				try
				{
					stopwatch.Start();
					flag = this.client.TryGet<T>(key, out value, out text);
				}
				finally
				{
					stopwatch.Stop();
					diagnostics.AddSharedCacheLatency(TimeSpan.FromMilliseconds((double)stopwatch.ElapsedMilliseconds));
				}
				diagnostics.AddDiagnosticText(text);
				result = flag;
			}
			catch (CacheClientException innerException)
			{
				throw new SharedCacheException("SharedCacheClient.TryGet(T) failed", innerException);
			}
			return result;
		}

		string ISharedCache.GetSharedCacheKeyFromRoutingKey(IRoutingKey key)
		{
			string text = string.Empty;
			switch (key.RoutingItemType)
			{
			case RoutingItemType.ArchiveSmtp:
			{
				ArchiveSmtpRoutingKey archiveSmtpRoutingKey = key as ArchiveSmtpRoutingKey;
				text = SharedCacheClientWrapper.MakeCacheKey(SharedCacheClientWrapper.AnchorSource.Smtp, archiveSmtpRoutingKey.SmtpAddress);
				text += "_Archive";
				break;
			}
			case RoutingItemType.DatabaseGuid:
			{
				DatabaseGuidRoutingKey databaseGuidRoutingKey = key as DatabaseGuidRoutingKey;
				text = SharedCacheClientWrapper.MakeCacheKey(SharedCacheClientWrapper.AnchorSource.DatabaseGuid, databaseGuidRoutingKey.DatabaseGuid);
				break;
			}
			case RoutingItemType.MailboxGuid:
			{
				MailboxGuidRoutingKey mailboxGuidRoutingKey = key as MailboxGuidRoutingKey;
				text = SharedCacheClientWrapper.MakeCacheKey(SharedCacheClientWrapper.AnchorSource.MailboxGuid, mailboxGuidRoutingKey.MailboxGuid);
				break;
			}
			case RoutingItemType.Smtp:
			{
				SmtpRoutingKey smtpRoutingKey = key as SmtpRoutingKey;
				text = SharedCacheClientWrapper.MakeCacheKey(SharedCacheClientWrapper.AnchorSource.Smtp, smtpRoutingKey.SmtpAddress);
				break;
			}
			case RoutingItemType.ExternalDirectoryObjectId:
			{
				ExternalDirectoryObjectIdRoutingKey externalDirectoryObjectIdRoutingKey = key as ExternalDirectoryObjectIdRoutingKey;
				text = SharedCacheClientWrapper.MakeCacheKey(SharedCacheClientWrapper.AnchorSource.ExternalDirectoryObjectId, externalDirectoryObjectIdRoutingKey.UserGuid);
				break;
			}
			case RoutingItemType.LiveIdMemberName:
			{
				LiveIdMemberNameRoutingKey liveIdMemberNameRoutingKey = key as LiveIdMemberNameRoutingKey;
				text = SharedCacheClientWrapper.MakeCacheKey(SharedCacheClientWrapper.AnchorSource.LiveIdMemberName, liveIdMemberNameRoutingKey.LiveIdMemberName);
				break;
			}
			}
			return text;
		}

		bool ISharedCacheClient.TryInsert(string key, byte[] dataBytes, DateTime cacheExpiry, out string diagInfo)
		{
			return this.client.TryInsert(key, dataBytes, cacheExpiry, out diagInfo);
		}

		bool ISharedCacheClient.TryInsert(string key, ISharedCacheEntry value, DateTime valueTimeStamp, out string diagInfo)
		{
			return this.client.TryInsert(key, value, valueTimeStamp, out diagInfo);
		}

		private static string MakeCacheKey(SharedCacheClientWrapper.AnchorSource source, object obj)
		{
			string text = source.ToString() + "~" + obj.ToString();
			return text.Replace(" ", "_");
		}

		private readonly SharedCacheClient client;

		private enum AnchorSource
		{
			Smtp,
			Sid,
			Domain,
			DomainAndVersion,
			OrganizationId,
			MailboxGuid,
			DatabaseName,
			DatabaseGuid,
			UserADRawEntry,
			ServerInfo,
			ServerVersion,
			Url,
			Anonymous,
			GenericAnchorHint,
			Puid,
			ExternalDirectoryObjectId,
			OAuthActAsUser,
			LiveIdMemberName
		}
	}
}
