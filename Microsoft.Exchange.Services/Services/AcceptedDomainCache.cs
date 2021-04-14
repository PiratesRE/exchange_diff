using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Services;

namespace Microsoft.Exchange.Services
{
	internal class AcceptedDomainCache : IDisposable
	{
		internal AcceptedDomainCache()
		{
			this.session = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 45, ".ctor", "f:\\15.00.1497\\sources\\dev\\services\\src\\Services\\Common\\AcceptedDomainCache.cs");
		}

		private bool IsInitialized
		{
			get
			{
				return this.cookie != null;
			}
		}

		public AcceptedDomain DefaultDomain
		{
			get
			{
				AcceptedDomain result = null;
				lock (this)
				{
					this.CheckDisposed();
					if (!this.IsInitialized)
					{
						this.RegisterWithAD();
						this.Reload(null);
					}
					result = this.defaultDomain;
				}
				return result;
			}
		}

		public void Dispose()
		{
			if (!this.isDisposed)
			{
				lock (this)
				{
					if (this.cookie != null)
					{
						this.ReleaseCookie();
						this.cookie = null;
					}
					this.isDisposed = true;
				}
			}
		}

		private void CheckDisposed()
		{
			if (this.isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		private void Reload(ADNotificationEventArgs args)
		{
			lock (this)
			{
				try
				{
					if (this.defaultDomain != null)
					{
						QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Guid, this.defaultDomain.Guid);
						AcceptedDomain[] array = this.session.Find<AcceptedDomain>(null, QueryScope.SubTree, filter, null, 1);
						if (array != null && array.Length == 1 && array[0].Default)
						{
							this.defaultDomain = array[0];
							ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string>((long)this.GetHashCode(), "Default accepted domain still set to {0}", this.defaultDomain.DomainName.Address);
							return;
						}
						ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string>((long)this.GetHashCode(), "Default accepted domain changed. It was previously set to {0}", this.defaultDomain.DomainName.Address);
					}
					ExTraceGlobals.CommonAlgorithmTracer.TraceDebug((long)this.GetHashCode(), "Attempting to find the current default accepted domain");
					BitMaskAndFilter filter2 = new BitMaskAndFilter(AcceptedDomainSchema.AcceptedDomainFlags, 4UL);
					AcceptedDomain[] array2 = this.session.Find<AcceptedDomain>(null, QueryScope.SubTree, filter2, null, 1);
					if (array2 != null && array2.Length == 1)
					{
						this.defaultDomain = array2[0];
						ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string>((long)this.GetHashCode(), "Default accepted domain has been changed to {0}", this.defaultDomain.DomainName.Address);
					}
				}
				catch (ADTransientException arg)
				{
					ExTraceGlobals.ExceptionTracer.TraceError<ADTransientException>((long)this.GetHashCode(), "Failed to find default Accepted Domain: {0}", arg);
					if (args == null)
					{
						throw;
					}
				}
			}
		}

		private void RegisterWithAD()
		{
			try
			{
				this.cookie = ADNotificationAdapter.RegisterChangeNotification<AcceptedDomain>(this.session.GetOrgContainerId().GetDescendantId(AcceptedDomain.AcceptedDomainContainer), new ADNotificationCallback(this.Reload), null);
			}
			catch (ADTransientException arg)
			{
				this.cookie = null;
				ExTraceGlobals.ExceptionTracer.TraceError<ADTransientException>((long)this.GetHashCode(), "Failed to register Accepted Domain Change notification: {0}", arg);
			}
		}

		private void ReleaseCookie()
		{
			ADNotificationAdapter.UnregisterChangeNotification(this.cookie);
		}

		private AcceptedDomain defaultDomain;

		private IConfigurationSession session;

		private bool isDisposed;

		private ADNotificationRequestCookie cookie;
	}
}
