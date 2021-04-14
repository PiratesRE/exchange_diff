using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Availability;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal static class RemoteServiceUriCache
	{
		internal static WebServiceUri Get(EmailAddress email, int versionBucket)
		{
			string text = email.Address.ToLower();
			VersionedWebServiceUri versionedWebServiceUri;
			lock (RemoteServiceUriCache.cacheLocker)
			{
				versionedWebServiceUri = (VersionedWebServiceUri)RemoteServiceUriCache.cache[text];
			}
			if (versionedWebServiceUri != null)
			{
				RemoteServiceUriCache.AutoDiscoverTracer.TraceDebug<object, VersionedWebServiceUri, string>(0L, "{0}: Found service URI {1} for email {2} in the cache.", TraceContext.Get(), versionedWebServiceUri, text);
				WebServiceUri webServiceUri = versionedWebServiceUri.Get(versionBucket);
				if (webServiceUri == null)
				{
					RemoteServiceUriCache.AutoDiscoverTracer.TraceDebug<object, string, int>(0L, "{0}: Service URI for email {1} was not found in version bucket {2}.", TraceContext.Get(), text, versionBucket);
				}
				else if (webServiceUri.Expired(DateTime.UtcNow) && (LocalizedString.Empty.Equals(webServiceUri.AutodiscoverFailedExceptionString) || RemoteServiceUriCache.IsBadUri(webServiceUri)))
				{
					RemoteServiceUriCache.AutoDiscoverTracer.TraceDebug<object, WebServiceUri>(0L, "{0}: URI {1} was found to be a bad URL, resetting and returning NULL.", TraceContext.Get(), webServiceUri);
					lock (RemoteServiceUriCache.cacheLocker)
					{
						if (webServiceUri == versionedWebServiceUri.Get(versionBucket))
						{
							versionedWebServiceUri.Clear(versionBucket);
						}
					}
					return null;
				}
				return webServiceUri;
			}
			RemoteServiceUriCache.AutoDiscoverTracer.TraceDebug<object, string>(0L, "{0}: Service URI for email {1} was not found in the cache.", TraceContext.Get(), text);
			return null;
		}

		internal static void Add(EmailAddress email, WebServiceUri uri, int versionBucket)
		{
			RemoteServiceUriCache.AutoDiscoverTracer.TraceDebug<object, WebServiceUri, EmailAddress>(0L, "{0}: Adding/Updating AS URI {1} for email {2} in cache.", TraceContext.Get(), uri, email);
			string key = email.Address.ToLower();
			WebServiceUri webServiceUri = new WebServiceUri(uri, DateTime.UtcNow);
			lock (RemoteServiceUriCache.cacheLocker)
			{
				VersionedWebServiceUri versionedWebServiceUri = (VersionedWebServiceUri)RemoteServiceUriCache.cache[key];
				if (versionedWebServiceUri == null)
				{
					if (500000 == RemoteServiceUriCache.cache.Count)
					{
						RemoteServiceUriCache.cache.RemoveAt(0);
					}
					versionedWebServiceUri = VersionedWebServiceUri.Create(webServiceUri, versionBucket);
					RemoteServiceUriCache.cache[key] = versionedWebServiceUri;
				}
				else
				{
					versionedWebServiceUri.Set(webServiceUri, versionBucket);
				}
			}
		}

		internal static void Invalidate(string url)
		{
			RemoteServiceUriCache.AutoDiscoverTracer.TraceDebug<object, string>(0L, "{0}: Invalidating url {1} in RemoteServiceUriCache.", TraceContext.Get(), (url == null) ? "<NULL>" : url);
			if (url != null)
			{
				url = url.ToLower();
				lock (RemoteServiceUriCache.badUrlListLocker)
				{
					if (!RemoteServiceUriCache.badUrlList.Contains(url))
					{
						RemoteServiceUriCache.badUrlList.Add(url);
					}
				}
			}
		}

		internal static void Validate(string url)
		{
			RemoteServiceUriCache.AutoDiscoverTracer.TraceDebug<object, string>(0L, "{0}: Validating url {1} in ServiceUriCache.", TraceContext.Get(), (url == null) ? "<NULL>" : url);
			if (url != null)
			{
				url = url.ToLower();
				lock (RemoteServiceUriCache.badUrlListLocker)
				{
					RemoteServiceUriCache.badUrlList.Remove(url);
				}
			}
		}

		internal static bool IsBadUri(WebServiceUri uri)
		{
			bool result = false;
			if (uri.Uri != null)
			{
				string text = uri.Uri.ToString();
				if (text != null)
				{
					text = text.ToLower();
					lock (RemoteServiceUriCache.badUrlListLocker)
					{
						if (RemoteServiceUriCache.badUrlList.Contains(text))
						{
							result = true;
						}
					}
				}
			}
			return result;
		}

		private const int MaxEntries = 500000;

		private static readonly Trace AutoDiscoverTracer = ExTraceGlobals.AutoDiscoverTracer;

		private static OrderedDictionary cache = new OrderedDictionary();

		private static object cacheLocker = new object();

		private static HashSet<string> badUrlList = new HashSet<string>();

		private static object badUrlListLocker = new object();
	}
}
