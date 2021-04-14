using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics.Components.Entities.HolidayCalendars;
using Microsoft.Exchange.Entities.HolidayCalendars.Configuration.Exceptions;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Entities.HolidayCalendars.Configuration
{
	internal class ConfigurationCache
	{
		public ConfigurationCache(IEndpointInformationRetrieverFactory endpointRetrieverFactory = null, IDateTimeFactory dateTimeFactory = null)
		{
			this.dateTimeFactory = (dateTimeFactory ?? new DateTimeFactory());
			this.endpointRetrieverFactory = (endpointRetrieverFactory ?? new EndpointInformationRetrieverFactory());
		}

		public int CacheSize
		{
			get
			{
				int count;
				lock (this.cache)
				{
					count = this.cache.Count;
				}
				return count;
			}
		}

		public UrlResolver GetUrlResolver(VariantConfigurationSnapshot configSnapshot)
		{
			return this.GetUrlResolver(new HolidayConfigurationSnapshot(configSnapshot));
		}

		public UrlResolver GetUrlResolver(HolidayConfigurationSnapshot configSnapshot)
		{
			ConfigurationCache.CacheEntry endpointCacheEntry = this.GetEndpointCacheEntry(configSnapshot);
			if (endpointCacheEntry.UrlResolver == null)
			{
				throw endpointCacheEntry.ErrorInformation;
			}
			return endpointCacheEntry.UrlResolver;
		}

		private ConfigurationCache.CacheEntry GetEndpointCacheEntry(HolidayConfigurationSnapshot configSnapshot)
		{
			Uri calendarEndpoint = configSnapshot.CalendarEndpoint;
			int configurationFetchTimeout = configSnapshot.ConfigurationFetchTimeout;
			ConfigurationCache.CacheEntry cacheEntry;
			lock (this.cache)
			{
				if (!this.cache.TryGetValue(calendarEndpoint.AbsoluteUri, out cacheEntry) || (cacheEntry.EndpointInformation == null && cacheEntry.UtcTimeStamp.AddMinutes(5.0) < this.dateTimeFactory.GetUtcNow()))
				{
					cacheEntry = this.RequestEndpointInfo(calendarEndpoint, configurationFetchTimeout);
				}
			}
			return cacheEntry;
		}

		private ConfigurationCache.CacheEntry RequestEndpointInfo(Uri endpoint, int timeout)
		{
			IEndpointInformationRetriever endpointInformationRetriever = this.endpointRetrieverFactory.Create(endpoint, timeout);
			ConfigurationCache.CacheEntry cacheEntry;
			try
			{
				EndpointInformation endpointInformation = endpointInformationRetriever.FetchEndpointInformation();
				cacheEntry = new ConfigurationCache.CacheEntry(this.dateTimeFactory.GetUtcNow(), endpointInformation);
			}
			catch (EndpointConfigurationException ex)
			{
				ExTraceGlobals.ConfigurationCacheTracer.TraceError<string>((long)this.GetHashCode(), "Unable to fetch endpoint info. {0}", ex.Message);
				cacheEntry = new ConfigurationCache.CacheEntry(this.dateTimeFactory.GetUtcNow(), ex);
			}
			if (this.cache.ContainsKey(endpoint.AbsoluteUri))
			{
				this.cache[endpoint.AbsoluteUri] = cacheEntry;
			}
			else
			{
				this.cache.Add(endpoint.AbsoluteUri, cacheEntry);
			}
			return cacheEntry;
		}

		private const int FailedConfigurationDownloadQuarantineInMinutes = 5;

		public static readonly ConfigurationCache Instance = new ConfigurationCache(null, null);

		private readonly Dictionary<string, ConfigurationCache.CacheEntry> cache = new Dictionary<string, ConfigurationCache.CacheEntry>();

		private readonly IDateTimeFactory dateTimeFactory;

		private readonly IEndpointInformationRetrieverFactory endpointRetrieverFactory;

		private class CacheEntry
		{
			public CacheEntry(ExDateTime utcTimeStamp, EndpointInformation endpointInformation)
			{
				this.UtcTimeStamp = utcTimeStamp;
				this.EndpointInformation = endpointInformation;
			}

			public CacheEntry(ExDateTime utcTimeStamp, Exception error)
			{
				this.UtcTimeStamp = utcTimeStamp;
				this.ErrorInformation = error;
			}

			public ExDateTime UtcTimeStamp { get; private set; }

			public EndpointInformation EndpointInformation { get; private set; }

			public Exception ErrorInformation { get; private set; }

			public UrlResolver UrlResolver
			{
				get
				{
					if (this.urlResolver == null && this.EndpointInformation != null)
					{
						this.urlResolver = new UrlResolver(this.EndpointInformation);
					}
					return this.urlResolver;
				}
			}

			private UrlResolver urlResolver;
		}
	}
}
