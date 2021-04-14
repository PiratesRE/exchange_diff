using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Common.Cache;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;

namespace Microsoft.Exchange.Transport
{
	internal class CertificateValidationResultCache : DisposeTrackableBase
	{
		public CertificateValidationResultCache(ProcessTransportRole transportRole, string name, TransportAppConfig.SecureMailConfig config, ITracer tracer)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("name", name);
			ArgumentValidator.ThrowIfNull("config", config);
			ArgumentValidator.ThrowIfNull("tracer", tracer);
			this.transientFailureExpiryInterval = config.CertificateValidationCacheTransientFailureExpiryInterval;
			this.cache = new Cache<string, CertificateValidationResultCache.ValidationResultItem>((long)config.CertificateValidationCacheMaxSize.ToBytes(), config.CertificateValidationCacheExpiryInterval, TimeSpan.Zero, new DefaultCacheTracer<string>(tracer, name), new CertificateValidationResultCache.ValidationResultPerformanceCounters(transportRole, name));
			tracer.TraceDebug((long)this.GetHashCode(), "Created Certificate Validation Result Cache '{0}' with the following configuration: MaxSize={1}, ExpiryInterval={2}, TransientFailureExpiryInterval={3}", new object[]
			{
				name,
				config.CertificateValidationCacheMaxSize,
				config.CertificateValidationCacheExpiryInterval,
				config.CertificateValidationCacheTransientFailureExpiryInterval
			});
		}

		public bool TryAdd(X509Certificate2 certificate, ChainValidityStatus result)
		{
			string thumbprint = certificate.Thumbprint;
			if (string.IsNullOrEmpty(thumbprint))
			{
				return false;
			}
			DateTime expiryTime = this.CalculateItemExpiryTime(certificate, result);
			return this.cache.TryAdd(thumbprint, new CertificateValidationResultCache.ValidationResultItem(result, expiryTime));
		}

		public bool TryGetValue(X509Certificate2 certificate, out ChainValidityStatus result)
		{
			result = ChainValidityStatus.EmptyCertificate;
			if (certificate == null)
			{
				throw new ArgumentNullException("certificate");
			}
			string thumbprint = certificate.Thumbprint;
			CertificateValidationResultCache.ValidationResultItem validationResultItem;
			if (!string.IsNullOrEmpty(thumbprint) && this.cache.TryGetValue(thumbprint, out validationResultItem))
			{
				result = validationResultItem.Result;
				return true;
			}
			return false;
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (isDisposing)
			{
				this.cache.Dispose();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<CertificateValidationResultCache>(this);
		}

		protected virtual DateTime GetCertificateExpiryTime(X509Certificate2 certificate)
		{
			return certificate.NotAfter;
		}

		private DateTime CalculateItemExpiryTime(X509Certificate2 certificate, ChainValidityStatus result)
		{
			if (result <= (ChainValidityStatus)2148081683U)
			{
				switch (result)
				{
				case ChainValidityStatus.Valid:
				case ChainValidityStatus.ValidSelfSigned:
					return this.GetCertificateExpiryTime(certificate);
				default:
					switch (result)
					{
					case (ChainValidityStatus)2148081682U:
					case (ChainValidityStatus)2148081683U:
						break;
					default:
						goto IL_59;
					}
					break;
				}
			}
			else if (result != (ChainValidityStatus)2148204810U && result != (ChainValidityStatus)2148204814U)
			{
				goto IL_59;
			}
			return DateTime.UtcNow + this.transientFailureExpiryInterval;
			IL_59:
			return DateTime.MaxValue;
		}

		private readonly Cache<string, CertificateValidationResultCache.ValidationResultItem> cache;

		private readonly TimeSpan transientFailureExpiryInterval;

		private class ValidationResultItem : CachableItem
		{
			public ValidationResultItem(ChainValidityStatus result, DateTime expiryTime)
			{
				this.Result = result;
				this.expiryTime = expiryTime;
			}

			public override long ItemSize
			{
				get
				{
					return 12L;
				}
			}

			public override bool IsExpired(DateTime currentTime)
			{
				return currentTime > this.expiryTime;
			}

			public readonly ChainValidityStatus Result;

			private readonly DateTime expiryTime;
		}

		private class ValidationResultPerformanceCounters : DefaultCachePerformanceCounters
		{
			public ValidationResultPerformanceCounters(ProcessTransportRole transportRole, string instanceName)
			{
				if (instanceName == null)
				{
					throw new ArgumentNullException("instanceName");
				}
				CertificateValidationResultCachePerfCountersInstance certificateValidationResultCachePerfCountersInstance;
				try
				{
					CertificateValidationResultCachePerfCounters.SetCategoryName(CertificateValidationResultCache.ValidationResultPerformanceCounters.perfCounterCategoryMap[transportRole]);
					certificateValidationResultCachePerfCountersInstance = CertificateValidationResultCachePerfCounters.GetInstance(instanceName);
				}
				catch (InvalidOperationException ex)
				{
					ExTraceGlobals.GeneralTracer.TraceError<string, InvalidOperationException>(0L, "Failed to initialize performance counters for component '{0}': {1}", instanceName, ex);
					ExEventLog exEventLog = new ExEventLog(ExTraceGlobals.GeneralTracer.Category, TransportEventLog.GetEventSource());
					exEventLog.LogEvent(TransportEventLogConstants.Tuple_PerfCountersLoadFailure, null, new object[]
					{
						"Certificate Validation Result Cache",
						instanceName,
						ex.ToString()
					});
					certificateValidationResultCachePerfCountersInstance = null;
				}
				if (certificateValidationResultCachePerfCountersInstance != null)
				{
					base.InitializeCounters(certificateValidationResultCachePerfCountersInstance.Requests, certificateValidationResultCachePerfCountersInstance.HitRatio, certificateValidationResultCachePerfCountersInstance.HitRatio_Base, certificateValidationResultCachePerfCountersInstance.CacheSize);
				}
			}

			private static readonly IDictionary<ProcessTransportRole, string> perfCounterCategoryMap = new Dictionary<ProcessTransportRole, string>
			{
				{
					ProcessTransportRole.Edge,
					"MSExchangeTransport Certificate Validation Cache"
				},
				{
					ProcessTransportRole.Hub,
					"MSExchangeTransport Certificate Validation Cache"
				},
				{
					ProcessTransportRole.FrontEnd,
					"MSExchangeFrontEndTransport Certificate Validation Cache"
				},
				{
					ProcessTransportRole.MailboxDelivery,
					"MSExchange Delivery Certificate Validation Cache"
				},
				{
					ProcessTransportRole.MailboxSubmission,
					"MSExchange Submission Certificate Validation Cache"
				}
			};
		}
	}
}
