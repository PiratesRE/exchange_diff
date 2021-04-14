using System;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Services.OnlineMeetings.Autodiscover
{
	internal class AutodiscoverCacheEntry
	{
		internal AutodiscoverCacheEntry(string sipUri, string ucwaUrl, ExDateTime? expirationDate)
		{
			this.sipUri = sipUri;
			this.ucwaUrl = ucwaUrl;
			this.Expiration = expirationDate;
		}

		public string SipUri
		{
			get
			{
				return this.sipUri;
			}
		}

		public string UcwaUrl
		{
			get
			{
				return this.ucwaUrl;
			}
		}

		public bool IsUcwaEnabled
		{
			get
			{
				return !string.IsNullOrEmpty(this.UcwaUrl);
			}
		}

		public int MaxFailureThreshold
		{
			get
			{
				if (this.maxFailureThreshold < 0)
				{
					this.maxFailureThreshold = AppConfigLoader.GetConfigIntValue("OnlineMeetingAutodiscoverRetryThreshold", 0, int.MaxValue, 5);
				}
				return this.maxFailureThreshold;
			}
		}

		public ExDateTime? Expiration { get; set; }

		public bool IsValid
		{
			get
			{
				return (this.Expiration == null || ExDateTime.Now.CompareTo(this.Expiration.Value) < 0) && this.FailureCount < this.MaxFailureThreshold;
			}
		}

		internal int FailureCount
		{
			get
			{
				return this.failureCount;
			}
		}

		public void IncrementFailureCount()
		{
			ExTraceGlobals.OnlineMeetingTracer.TraceInformation<string, int>(0, 0L, "[AutodiscoverCacheEntry.IncrementFailureCount] Pre-incremented failure count of entry for user '{0}': {1}", this.SipUri, this.failureCount);
			this.failureCount++;
		}

		internal const int MaxFailureThresholdDefault = 5;

		private readonly string sipUri;

		private readonly string ucwaUrl;

		private int failureCount;

		private int maxFailureThreshold = -1;
	}
}
