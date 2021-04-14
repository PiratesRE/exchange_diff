using System;
using System.Diagnostics;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Security.Authentication.FederatedAuthService
{
	internal class ADHomeRealmDiscovery : IHomeRealmDiscovery
	{
		private static LiveIdBasicAuthenticationCountersInstance counters
		{
			get
			{
				return AuthServiceHelper.PerformanceCounters;
			}
		}

		public ADHomeRealmDiscovery(LiveIdInstanceType instance)
		{
			this.instance = instance;
		}

		public LiveIdInstanceType Instance
		{
			get
			{
				return this.instance;
			}
		}

		public string RealmDiscoveryUri
		{
			get
			{
				return "Offline HomeRealmDiscovery";
			}
		}

		public string StsTag
		{
			get
			{
				return "OfflineHRD";
			}
		}

		public string ErrorString { get; private set; }

		public string LiveServer
		{
			get
			{
				return "Offline HomeRealmDiscovery";
			}
		}

		public long Latency
		{
			get
			{
				if (this.stopwatch != null)
				{
					return this.stopwatch.ElapsedMilliseconds;
				}
				return -1L;
			}
		}

		public long SSLConnectionLatency
		{
			get
			{
				return 0L;
			}
		}

		public IAsyncResult StartRequestChain(object userDomain, AsyncCallback callback, object state)
		{
			this.stopwatch = Stopwatch.StartNew();
			this.authService = (state as AuthService);
			this.userDomain = (string)userDomain;
			if (this.authService == null)
			{
				this.ErrorString = "Internal Service Error: authService is null ";
				throw new ArgumentNullException("authService");
			}
			if (string.IsNullOrEmpty(this.userDomain))
			{
				this.ErrorString = "Internal Service Erro: userDomain is null or empty";
				throw new ArgumentException("userDomain");
			}
			LazyAsyncResult ar = new LazyAsyncResult(null, state, null);
			return callback.BeginInvoke(ar, null, null);
		}

		public IAsyncResult ProcessRequest(IAsyncResult asyncResult, AsyncCallback callback, object state)
		{
			LazyAsyncResult ar = new LazyAsyncResult(null, state, null);
			return callback.BeginInvoke(ar, null, null);
		}

		public DomainConfig ProcessResponse(IAsyncResult asyncResult)
		{
			DomainConfig domainConfig = null;
			DomainConfig result;
			try
			{
				if (this.authService == null)
				{
					this.ErrorString = "Internal Service Error: authService is null ";
					throw new ArgumentNullException("authService");
				}
				if (string.IsNullOrEmpty(this.userDomain))
				{
					this.ErrorString = "Internal Service Erro: userDomain is null or empty";
					throw new ArgumentException("userDomain");
				}
				ITenantConfigurationSession session = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromTenantAcceptedDomain(this.userDomain), 144, "ProcessResponse", "f:\\15.00.1497\\sources\\dev\\Security\\src\\Authentication\\FederatedAuthService\\ADHomeRealmDiscovery.cs");
				string errorString;
				domainConfig = OfflineOrgIdAuth.GetHRDEntryFromAD(session, null, this.userDomain, out errorString);
				this.ErrorString = errorString;
				if (domainConfig == null)
				{
					throw new ADHrdException(this.ErrorString);
				}
				result = domainConfig;
			}
			catch (CannotResolveTenantNameException ex)
			{
				throw new ADHrdException(ex.Message);
			}
			finally
			{
				this.stopwatch.Stop();
				if (domainConfig == null)
				{
					ADHomeRealmDiscovery.counters.NumberOfFailedADHrdRequests.Increment();
				}
			}
			return result;
		}

		public void Abort()
		{
		}

		public string GetLatency()
		{
			return string.Format("<{0}-{1}-{2}ms>", "Offline HomeRealmDiscovery", this.instance.ToString(), this.Latency);
		}

		private string userDomain;

		private AuthService authService;

		private LiveIdInstanceType instance;

		private Stopwatch stopwatch;
	}
}
