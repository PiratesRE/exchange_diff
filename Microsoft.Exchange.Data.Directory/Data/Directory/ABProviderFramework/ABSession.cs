using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.ABProviderFramework
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class ABSession : IDisposeTrackable, IDisposable
	{
		protected ABSession(Trace tracer) : this(tracer, CultureInfo.CurrentCulture.LCID)
		{
		}

		protected ABSession(Trace tracer, int lcid)
		{
			if (tracer == null)
			{
				throw new ArgumentNullException("tracer");
			}
			this.disposeTracker = ((IDisposeTrackable)this).GetDisposeTracker();
			this.tracer = tracer;
		}

		public abstract ABProviderCapabilities ProviderCapabilities { get; }

		public TimeSpan? Timeout
		{
			get
			{
				this.ThrowIfDisposed();
				return this.timeout;
			}
			set
			{
				this.ThrowIfDisposed();
				this.timeout = value;
			}
		}

		public string ProviderName
		{
			get
			{
				this.ThrowIfDisposed();
				return this.InternalProviderName;
			}
		}

		protected Trace Tracer
		{
			get
			{
				return this.tracer;
			}
		}

		protected abstract string InternalProviderName { get; }

		public ABObject FindById(ABObjectId id)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			this.ThrowIfDisposed();
			this.Tracer.TraceDebug<ABObjectId>(0L, "FindById ({0})", id);
			ABObject abobject = this.InternalFindById(id);
			this.TraceForObjectAfterFind(abobject);
			return abobject;
		}

		public ABRawEntry FindById(ABObjectId id, ABPropertyDefinitionCollection properties)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			ABSession.ValidateProperties(properties);
			this.ThrowIfDisposed();
			this.Tracer.TraceDebug<ABObjectId, ABPropertyDefinitionCollection>(0L, "FindById(id={0}, properties={1})", id, properties);
			ABObjectId[] ids = new ABObjectId[]
			{
				id
			};
			IList<ABRawEntry> list = this.InternalFindByIds(ids, properties);
			this.Tracer.TraceDebug<int>(0L, "Provider returned '{0}' results.", list.Count);
			if (list.Count > 1)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Provider returned {0} results for FindById({1}).", new object[]
				{
					list.Count,
					id
				}));
			}
			if (list.Count != 0)
			{
				return list[0];
			}
			return null;
		}

		public IList<ABRawEntry> FindByIds(ICollection<ABObjectId> ids, ABPropertyDefinitionCollection properties)
		{
			if (ids == null)
			{
				throw new ArgumentNullException("ids");
			}
			if (ids.Count == 0)
			{
				throw new ArgumentException("ids collection can't be empty.");
			}
			ABSession.ValidateProperties(properties);
			this.ThrowIfDisposed();
			this.Tracer.TraceDebug<int, ABPropertyDefinitionCollection>(0L, "FindByIds({0} ids, properties={1})", ids.Count, properties);
			IList<ABRawEntry> list = this.InternalFindByIds(ids, properties);
			this.Tracer.TraceDebug<int>(0L, "Provider returned '{0}' results.", list.Count);
			return list;
		}

		public IList<ABRawEntry> FindByANR(string anrMatch, int maxResults, ABPropertyDefinitionCollection properties)
		{
			if (string.IsNullOrEmpty(anrMatch))
			{
				throw new ArgumentNullException("anrMatch");
			}
			if (maxResults < 1)
			{
				throw new ArgumentOutOfRangeException("maxResults", "maxResults must be greater than 0 and smaller than or equal to " + 1000.ToString());
			}
			if (maxResults > 1000)
			{
				maxResults = 1000;
			}
			ABSession.ValidateProperties(properties);
			this.ThrowIfDisposed();
			this.Tracer.TraceDebug<string, int, ABPropertyDefinitionCollection>(0L, "FindByAnr({0}, {1}, {2})", anrMatch, maxResults, properties);
			IList<ABRawEntry> list = this.InternalFindByANR(anrMatch, maxResults, properties);
			if (list == null)
			{
				list = ABSession.emptyRawEntriesReadOnlyList;
			}
			else if (list.Count > maxResults)
			{
				string message = string.Format(CultureInfo.InvariantCulture, "Provider '{0}' returned '{1}' rather than '{2}' maximum results.", new object[]
				{
					this.ProviderName,
					list.Count,
					maxResults
				});
				this.Tracer.TraceError(0L, message);
				throw new InvalidOperationException(message);
			}
			this.Tracer.TraceDebug<int>(0L, "Provider returned '{0}' results.", list.Count);
			return list;
		}

		public IList<ABObject> FindByANR(string anrMatch, int maxResults)
		{
			if (string.IsNullOrEmpty(anrMatch))
			{
				throw new ArgumentNullException("anrMatch");
			}
			if (maxResults < 1)
			{
				throw new ArgumentOutOfRangeException("maxResults", "maxResults must be greater than 0 and smaller than or equal to " + 1000.ToString());
			}
			if (maxResults > 1000)
			{
				maxResults = 1000;
			}
			this.ThrowIfDisposed();
			this.Tracer.TraceDebug<string, int>(0L, "FindByAnr({0}, {1})", anrMatch, maxResults);
			IList<ABObject> list = this.InternalFindByANR(anrMatch, maxResults);
			if (list == null)
			{
				list = ABSession.emptyReadOnlyList;
			}
			else if (list.Count > maxResults)
			{
				string message = string.Format(CultureInfo.InvariantCulture, "Provider '{0}' returned '{1}' rather than '{2}' maximum results.", new object[]
				{
					this.ProviderName,
					list.Count,
					maxResults
				});
				this.Tracer.TraceError(0L, message);
				throw new InvalidOperationException(message);
			}
			this.Tracer.TraceDebug<int>(0L, "Provider returned '{0}' results.", list.Count);
			return list;
		}

		public ABObject FindByProxyAddress(ProxyAddress proxyAddress)
		{
			if (proxyAddress == null)
			{
				throw new ArgumentNullException("proxyAddress");
			}
			this.ThrowIfDisposed();
			this.Tracer.TraceDebug<ProxyAddress>(0L, "FindByProxyAddress('{0}')", proxyAddress);
			ABObject abobject = this.InternalFindByProxyAddress(proxyAddress);
			this.TraceForObjectAfterFind(abobject);
			return abobject;
		}

		public ABObject FindByLegacyExchangeDN(string legacyExchangeDN)
		{
			if (string.IsNullOrEmpty(legacyExchangeDN))
			{
				throw new ArgumentNullException("legacyExchangeDN");
			}
			this.ThrowIfDisposed();
			this.Tracer.TraceDebug<string>(0L, "FindByLegacyExchangeDN('{0}')", legacyExchangeDN);
			ABObject abobject = this.InternalFindByLegacyExchangeDN(legacyExchangeDN);
			this.TraceForObjectAfterFind(abobject);
			return abobject;
		}

		public ABRawEntry FindByLegacyExchangeDN(string legacyDN, ABPropertyDefinitionCollection properties)
		{
			if (string.IsNullOrEmpty(legacyDN))
			{
				throw new ArgumentNullException("legacyDN");
			}
			ABSession.ValidateProperties(properties);
			this.ThrowIfDisposed();
			this.Tracer.TraceDebug<string, ABPropertyDefinitionCollection>(0L, "FindByLegacyExchangeDN(id={0}, properties={1})", legacyDN, properties);
			string[] legacyDNs = new string[]
			{
				legacyDN
			};
			IList<ABRawEntry> list = this.InternalFindByLegacyExchangeDNs(legacyDNs, properties);
			this.Tracer.TraceDebug<int>(0L, "Provider returned '{0}' results.", list.Count);
			if (list.Count > 1)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Provider returned {0} results for FindByLegacyDN({1}).", new object[]
				{
					list.Count,
					legacyDN
				}));
			}
			if (list.Count != 0)
			{
				return list[0];
			}
			return null;
		}

		public IList<ABRawEntry> FindByLegacyExchangeDNs(ICollection<string> legacyDNs, ABPropertyDefinitionCollection properties)
		{
			if (legacyDNs == null)
			{
				throw new ArgumentNullException("legacyDNs");
			}
			if (legacyDNs.Count == 0)
			{
				throw new ArgumentException("LegacyDNs collection can't be empty.");
			}
			ABSession.ValidateProperties(properties);
			this.ThrowIfDisposed();
			this.Tracer.TraceDebug<int, ABPropertyDefinitionCollection>(0L, "FindByLegacyExchangeDNs({0} legacyDNs, properties={1})", legacyDNs.Count, properties);
			IList<ABRawEntry> list = this.InternalFindByLegacyExchangeDNs(legacyDNs, properties);
			this.Tracer.TraceDebug<int>(0L, "Provider returned '{0}' results.", list.Count);
			return list;
		}

		public void Dispose()
		{
			if (!this.disposed)
			{
				this.disposed = true;
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
					this.disposeTracker = null;
				}
				this.InternalDispose();
			}
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<ABSession>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		protected abstract ABObject InternalFindById(ABObjectId id);

		protected virtual ABRawEntry InternalFindById(ABObjectId id, ABPropertyDefinitionCollection properties)
		{
			ABObject abobject = this.InternalFindById(id);
			if (abobject != null)
			{
				abobject.PropertyDefinitionCollection = properties;
			}
			return abobject;
		}

		protected abstract List<ABObject> InternalFindByANR(string anrMatch, int maxResults);

		protected virtual List<ABRawEntry> InternalFindByANR(string anrMatch, int maxResults, ABPropertyDefinitionCollection properties)
		{
			List<ABObject> list = this.InternalFindByANR(anrMatch, maxResults);
			List<ABRawEntry> list2 = new List<ABRawEntry>(list.Count);
			foreach (ABObject abobject in list)
			{
				if (abobject != null)
				{
					abobject.PropertyDefinitionCollection = properties;
					list2.Add(abobject);
				}
			}
			return list2;
		}

		protected abstract ABObject InternalFindByProxyAddress(ProxyAddress proxyAddress);

		protected abstract ABObject InternalFindByLegacyExchangeDN(string legacyExchangeDN);

		protected virtual ABRawEntry InternalFindByLegacyExchangeDN(string legacyExchangeDN, ABPropertyDefinitionCollection properties)
		{
			ABObject abobject = this.InternalFindByLegacyExchangeDN(legacyExchangeDN);
			if (abobject != null)
			{
				abobject.PropertyDefinitionCollection = properties;
			}
			return abobject;
		}

		protected virtual void InternalDispose()
		{
		}

		protected virtual IList<ABRawEntry> InternalFindByLegacyExchangeDNs(ICollection<string> legacyDNs, ABPropertyDefinitionCollection properties)
		{
			IList<ABRawEntry> list = new List<ABRawEntry>(legacyDNs.Count);
			foreach (string legacyExchangeDN in legacyDNs)
			{
				list.Add(this.InternalFindByLegacyExchangeDN(legacyExchangeDN, properties));
			}
			return list;
		}

		protected virtual IList<ABRawEntry> InternalFindByIds(ICollection<ABObjectId> ids, ABPropertyDefinitionCollection properties)
		{
			IList<ABRawEntry> list = new List<ABRawEntry>(ids.Count);
			foreach (ABObjectId id in ids)
			{
				list.Add(this.InternalFindById(id, properties));
			}
			return list;
		}

		private static void ValidateProperties(ABPropertyDefinitionCollection properties)
		{
			if (properties == null)
			{
				throw new ArgumentNullException("properties");
			}
		}

		private void ThrowIfDisposed()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException("ABSession");
			}
		}

		private void TraceForObjectAfterFind(ABObject objectFound)
		{
			if (objectFound == null)
			{
				this.Tracer.TraceDebug(0L, "Provider didn't find object.");
				return;
			}
			this.Tracer.TraceDebug<string>(0L, "Provider found object with display name '{0}'.", objectFound.DisplayName);
		}

		public const int MaxSupportedAnrResult = 1000;

		private static IList<ABObject> emptyReadOnlyList = new ReadOnlyCollection<ABObject>(new List<ABObject>(0));

		private static IList<ABRawEntry> emptyRawEntriesReadOnlyList = new ReadOnlyCollection<ABRawEntry>(new List<ABRawEntry>(0));

		private DisposeTracker disposeTracker;

		private Trace tracer;

		private TimeSpan? timeout;

		private bool disposed;
	}
}
