using System;
using System.Web;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.HttpProxy.Common;
using Microsoft.Exchange.HttpProxy.Routing;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.HttpProxy
{
	internal abstract class AnchorMailbox
	{
		protected AnchorMailbox(AnchorSource anchorSource, object sourceObject, IRequestContext requestContext)
		{
			if (sourceObject == null)
			{
				throw new ArgumentNullException("sourceObject");
			}
			if (requestContext == null)
			{
				throw new ArgumentNullException("requestContext");
			}
			this.AnchorSource = anchorSource;
			this.SourceObject = sourceObject;
			this.RequestContext = requestContext;
			ExTraceGlobals.VerboseTracer.TraceDebug<int, AnchorSource, object>((long)this.GetHashCode(), "[AnchorMailbox::ctor]: context {0}; AnchorSource {1}; SourceObject {2}", this.RequestContext.TraceContext, anchorSource, sourceObject);
		}

		public AnchorSource AnchorSource { get; private set; }

		public object SourceObject { get; private set; }

		public IRequestContext RequestContext { get; private set; }

		public Func<Exception> NotFoundExceptionCreator { get; set; }

		public AnchorMailbox OriginalAnchorMailbox { get; set; }

		public bool CacheEntryCacheHit { get; private set; }

		private protected BackEndCookieEntryBase IncomingCookieEntry { protected get; private set; }

		public virtual string GetOrganizationNameForLogging()
		{
			if (this.loadedCachedEntry != null)
			{
				return this.loadedCachedEntry.DomainName;
			}
			return null;
		}

		public virtual BackEndServer TryDirectBackEndCalculation()
		{
			return null;
		}

		public virtual BackEndServer AcceptBackEndCookie(HttpCookie backEndCookie)
		{
			if (backEndCookie == null)
			{
				throw new ArgumentNullException("backEndCookie");
			}
			string name = this.ToCookieKey();
			string text = backEndCookie.Values[name];
			BackEndServer backEndServer = null;
			if (!string.IsNullOrEmpty(text))
			{
				try
				{
					BackEndCookieEntryBase backEndCookieEntryBase;
					string text2;
					if (!BackEndCookieEntryParser.TryParse(text, out backEndCookieEntryBase, out text2))
					{
						throw new InvalidBackEndCookieException();
					}
					if (backEndCookieEntryBase.Expired)
					{
						ExTraceGlobals.VerboseTracer.TraceDebug<BackEndCookieEntryBase>((long)this.GetHashCode(), "[AnchorMailbox::ProcessBackEndCookie]: Back end cookie entry {0} has expired.", backEndCookieEntryBase);
						this.RequestContext.Logger.SafeSet(HttpProxyMetadata.BackEndCookie, string.Format("Expired~{0}", text2));
						throw new InvalidBackEndCookieException();
					}
					this.RequestContext.Logger.SafeSet(HttpProxyMetadata.BackEndCookie, text2);
					this.IncomingCookieEntry = backEndCookieEntryBase;
					this.CacheEntryCacheHit = true;
					PerfCounters.HttpProxyCacheCountersInstance.CookieUseRate.Increment();
					PerfCounters.UpdateMovingPercentagePerformanceCounter(PerfCounters.HttpProxyCacheCountersInstance.MovingPercentageCookieUseRate);
					backEndServer = this.TryGetBackEndFromCookie(this.IncomingCookieEntry);
					ExTraceGlobals.VerboseTracer.TraceDebug<BackEndServer, BackEndCookieEntryBase>((long)this.GetHashCode(), "[AnchorMailbox::ProcessBackEndCookie]: Back end server {0} resolved from cookie {1}.", backEndServer, this.IncomingCookieEntry);
				}
				catch (InvalidBackEndCookieException)
				{
					ExTraceGlobals.VerboseTracer.TraceDebug((long)this.GetHashCode(), "[AnchorMailbox::ProcessBackEndCookie]: Invalid back end cookie entry.");
					backEndCookie.Values.Remove(name);
				}
			}
			return backEndServer;
		}

		public virtual BackEndCookieEntryBase BuildCookieEntryForTarget(BackEndServer routingTarget, bool proxyToDownLevel, bool useResourceForest)
		{
			if (routingTarget == null)
			{
				throw new ArgumentNullException("routingTarget");
			}
			return new BackEndServerCookieEntry(routingTarget.Fqdn, routingTarget.Version);
		}

		public virtual string ToCookieKey()
		{
			if (this.OriginalAnchorMailbox != null)
			{
				return this.OriginalAnchorMailbox.ToCookieKey();
			}
			return this.SourceObject.ToString().Replace(" ", "_").Replace("=", "+");
		}

		public virtual IRoutingEntry GetRoutingEntry()
		{
			return null;
		}

		public override string ToString()
		{
			return string.Format("{0}~{1}", this.AnchorSource, this.SourceObject);
		}

		public void UpdateCache(AnchorMailboxCacheEntry cacheEntry)
		{
			string key = this.ToCacheKey();
			AnchorMailboxCache.Instance.Add(key, cacheEntry, this.RequestContext);
			if (HttpProxySettings.NegativeAnchorMailboxCacheEnabled.Value)
			{
				NegativeAnchorMailboxCache.Instance.Remove(key);
			}
		}

		public void InvalidateCache()
		{
			string key = this.ToCacheKey();
			AnchorMailboxCache.Instance.Remove(key, this.RequestContext);
			if (HttpProxySettings.NegativeAnchorMailboxCacheEnabled.Value)
			{
				NegativeAnchorMailboxCache.Instance.Remove(key);
			}
		}

		public void UpdateNegativeCache(NegativeAnchorMailboxCacheEntry cacheEntry)
		{
			if (!HttpProxySettings.NegativeAnchorMailboxCacheEnabled.Value)
			{
				return;
			}
			string key = this.ToCacheKey();
			NegativeAnchorMailboxCache.Instance.Add(key, cacheEntry);
			AnchorMailboxCache.Instance.Remove(key, this.RequestContext);
		}

		public NegativeAnchorMailboxCacheEntry GetNegativeCacheEntry()
		{
			if (!HttpProxySettings.NegativeAnchorMailboxCacheEnabled.Value)
			{
				return null;
			}
			string key = this.ToCacheKey();
			NegativeAnchorMailboxCacheEntry result;
			if (NegativeAnchorMailboxCache.Instance.TryGet(key, out result))
			{
				return result;
			}
			return null;
		}

		protected virtual BackEndServer TryGetBackEndFromCookie(BackEndCookieEntryBase cookieEntry)
		{
			BackEndServerCookieEntry backEndServerCookieEntry = cookieEntry as BackEndServerCookieEntry;
			if (backEndServerCookieEntry != null)
			{
				ExTraceGlobals.VerboseTracer.TraceDebug<string>((long)this.GetHashCode(), "[AnchorMailbox::TryGetBackEndFromCookie]: BackEndServerCookier {0}", backEndServerCookieEntry.ToString());
				return new BackEndServer(backEndServerCookieEntry.Fqdn, backEndServerCookieEntry.Version);
			}
			ExTraceGlobals.VerboseTracer.TraceDebug((long)this.GetHashCode(), "[AnchorMailbox::TryGetBackEndFromCookie]: No BackEndServerCookie");
			return null;
		}

		protected AnchorMailboxCacheEntry GetCacheEntry()
		{
			if (this.loadedCachedEntry == null)
			{
				this.loadedCachedEntry = this.LoadCacheEntryFromIncomingCookie();
				string key = this.ToCacheKey();
				if (this.loadedCachedEntry == null)
				{
					if (AnchorMailboxCache.Instance.TryGet(key, this.RequestContext, out this.loadedCachedEntry))
					{
						ExTraceGlobals.VerboseTracer.TraceDebug<AnchorMailboxCacheEntry, AnchorMailbox>((long)this.GetHashCode(), "[AnchorMailbox::GetCacheEntry]: Using cached entry {0} for anchor mailbox {1}.", this.loadedCachedEntry, this);
						this.CacheEntryCacheHit = true;
					}
					else
					{
						this.loadedCachedEntry = this.RefreshCacheEntry();
						ExTraceGlobals.VerboseTracer.TraceDebug<AnchorMailboxCacheEntry, AnchorMailbox>((long)this.GetHashCode(), "[AnchorMailbox::GetCacheEntry]: RefreshCacheEntry() returns {0} for anchor mailbox {1}.", this.loadedCachedEntry, this);
						if (this.ShouldAddEntryToAnchorMailboxCache(this.loadedCachedEntry))
						{
							this.UpdateCache(this.loadedCachedEntry);
						}
						else
						{
							ExTraceGlobals.VerboseTracer.TraceDebug<AnchorMailboxCacheEntry, AnchorMailbox>((long)this.GetHashCode(), "[AnchorMailbox::GetCacheEntry]: Will not add cache entry {0} for anchor mailbox {1}.", this.loadedCachedEntry, this);
						}
					}
				}
				else
				{
					this.CacheEntryCacheHit = true;
				}
				this.OnPopulateCacheEntry(this.loadedCachedEntry);
			}
			return this.loadedCachedEntry;
		}

		protected virtual bool ShouldAddEntryToAnchorMailboxCache(AnchorMailboxCacheEntry cacheEntry)
		{
			return true;
		}

		protected virtual void OnPopulateCacheEntry(AnchorMailboxCacheEntry cacheEntry)
		{
		}

		protected virtual AnchorMailboxCacheEntry LoadCacheEntryFromIncomingCookie()
		{
			return null;
		}

		protected virtual AnchorMailboxCacheEntry RefreshCacheEntry()
		{
			return null;
		}

		protected virtual string ToCacheKey()
		{
			return this.ToString().Replace(" ", "_");
		}

		protected T CheckForNullAndThrowIfApplicable<T>(T ret)
		{
			if (ret == null && this.NotFoundExceptionCreator != null)
			{
				throw this.NotFoundExceptionCreator();
			}
			return ret;
		}

		public static readonly BoolAppSettingsEntry AllowMissingTenant = new BoolAppSettingsEntry("AnchorMailbox.AllowMissingTenant", false, ExTraceGlobals.VerboseTracer);

		private AnchorMailboxCacheEntry loadedCachedEntry;
	}
}
