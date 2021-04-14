using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.Net
{
	internal class Dns
	{
		public static int MaxCnameRecords
		{
			get
			{
				return Dns.maxCnameRecords;
			}
			set
			{
				Dns.maxCnameRecords = value;
			}
		}

		public static int MaxSubQueries
		{
			get
			{
				return Dns.maxSubQueries;
			}
			set
			{
				Dns.maxSubQueries = value;
			}
		}

		public int MaxDataPerRequest
		{
			get
			{
				return this.maxDataPerRequest;
			}
			set
			{
				this.maxDataPerRequest = value;
			}
		}

		public DnsServerList ServerList
		{
			get
			{
				return this.serverList;
			}
			set
			{
				this.serverList = value;
			}
		}

		public IEnumerable<IPAddress> LocalIPAddresses
		{
			get
			{
				return this.localAddresses;
			}
			set
			{
				this.localAddresses = value;
			}
		}

		public TimeSpan Timeout
		{
			get
			{
				return this.timeout;
			}
			set
			{
				if (value < TimeSpan.Zero)
				{
					throw new ArgumentOutOfRangeException();
				}
				this.timeout = value;
			}
		}

		public TimeSpan QueryRetryInterval
		{
			get
			{
				return this.queryRetryInterval;
			}
			set
			{
				if (value < TimeSpan.Zero)
				{
					throw new ArgumentOutOfRangeException();
				}
				this.queryRetryInterval = value;
			}
		}

		public AddressFamily DefaultAddressFamily
		{
			get
			{
				return this.defaultAddressFamily;
			}
			set
			{
				if ((value == AddressFamily.InterNetwork || value == AddressFamily.Unspecified) && !Socket.OSSupportsIPv4)
				{
					throw new NotSupportedException("IPv4 not supported on this machine");
				}
				if ((value == AddressFamily.InterNetworkV6 || value == AddressFamily.Unspecified) && !Socket.OSSupportsIPv6)
				{
					throw new NotSupportedException("IPv6 not supported on this machine");
				}
				this.defaultAddressFamily = value;
			}
		}

		public DnsQueryOptions Options
		{
			get
			{
				return this.queryOptions;
			}
			set
			{
				if ((value & ~(DnsQueryOptions.AcceptTruncatedResponse | DnsQueryOptions.UseTcpOnly | DnsQueryOptions.NoRecursion)) != DnsQueryOptions.None)
				{
					throw new ArgumentException(NetException.InvalidFlags((int)value), "value");
				}
				this.queryOptions = value;
			}
		}

		public static ExEventLog EventLogger
		{
			get
			{
				if (Dns.eventLogger == null)
				{
					Dns.eventLogger = new ExEventLog(ExTraceGlobals.DNSTracer.Category, "MSExchange Common");
				}
				return Dns.eventLogger;
			}
		}

		public static DnsStatus EndResolveToAddresses(IAsyncResult asyncResult, out IPAddress[] addresses)
		{
			IPAddress ipaddress;
			return Dns.EndResolveToAddresses(asyncResult, out addresses, out ipaddress);
		}

		public static DnsStatus EndResolveToAddresses(IAsyncResult asyncResult, out IPAddress[] addresses, out IPAddress server)
		{
			LazyAsyncResult lazyAsyncResult = asyncResult as LazyAsyncResult;
			if (lazyAsyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			DnsStatus status;
			using (Request request = lazyAsyncResult.AsyncObject as Request)
			{
				if (request == null)
				{
					throw new ArgumentException(NetException.WrongType, "asyncResult");
				}
				Request.Result result = (Request.Result)lazyAsyncResult.Result;
				addresses = (IPAddress[])result.Data;
				server = result.Server;
				status = result.Status;
			}
			return status;
		}

		public static DnsStatus EndResolveToMailServers(IAsyncResult asyncResult, out TargetHost[] hosts)
		{
			IPAddress ipaddress;
			return Dns.EndResolveToMailServers(asyncResult, out hosts, out ipaddress);
		}

		public static DnsStatus EndResolveToMailServers(IAsyncResult asyncResult, out TargetHost[] hosts, out IPAddress server)
		{
			LazyAsyncResult lazyAsyncResult = asyncResult as LazyAsyncResult;
			if (lazyAsyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			DnsStatus status;
			using (MxRequest mxRequest = lazyAsyncResult.AsyncObject as MxRequest)
			{
				if (mxRequest == null)
				{
					throw new ArgumentException(NetException.WrongType, "asyncResult");
				}
				Request.Result result = (Request.Result)lazyAsyncResult.Result;
				hosts = (TargetHost[])result.Data;
				server = result.Server;
				status = result.Status;
			}
			return status;
		}

		public static DnsStatus EndRetrieveTextRecords(IAsyncResult asyncResult, out string[] text)
		{
			IPAddress ipaddress;
			return Dns.EndRetrieveTextRecords(asyncResult, out text, out ipaddress);
		}

		public static DnsStatus EndRetrieveTextRecords(IAsyncResult asyncResult, out string[] text, out IPAddress server)
		{
			LazyAsyncResult lazyAsyncResult = asyncResult as LazyAsyncResult;
			if (lazyAsyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			DnsStatus status;
			using (TxtRequest txtRequest = lazyAsyncResult.AsyncObject as TxtRequest)
			{
				if (txtRequest == null)
				{
					throw new ArgumentException(NetException.WrongType, "asyncResult");
				}
				Request.Result result = (Request.Result)lazyAsyncResult.Result;
				text = (string[])result.Data;
				server = result.Server;
				status = result.Status;
			}
			return status;
		}

		public static DnsStatus EndRetrieveCNameRecords(IAsyncResult asyncResult, out DnsCNameRecord[] records)
		{
			IPAddress ipaddress;
			return Dns.EndRetrieveCNameRecords(asyncResult, out records, out ipaddress);
		}

		public static DnsStatus EndRetrieveCNameRecords(IAsyncResult asyncResult, out DnsCNameRecord[] records, out IPAddress server)
		{
			LazyAsyncResult lazyAsyncResult = asyncResult as LazyAsyncResult;
			if (lazyAsyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			DnsStatus status;
			using (CNameRequest cnameRequest = lazyAsyncResult.AsyncObject as CNameRequest)
			{
				if (cnameRequest == null)
				{
					throw new ArgumentException(NetException.WrongType, "asyncResult");
				}
				Request.Result result = (Request.Result)lazyAsyncResult.Result;
				records = (DnsCNameRecord[])result.Data;
				server = result.Server;
				status = result.Status;
			}
			return status;
		}

		public static DnsStatus EndResolveToNames(IAsyncResult asyncResult, out string[] domains)
		{
			IPAddress ipaddress;
			return Dns.EndResolveToNames(asyncResult, out domains, out ipaddress);
		}

		public static DnsStatus EndResolveToNames(IAsyncResult asyncResult, out string[] domains, out IPAddress server)
		{
			LazyAsyncResult lazyAsyncResult = asyncResult as LazyAsyncResult;
			if (lazyAsyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			DnsStatus status;
			using (PtrRequest ptrRequest = lazyAsyncResult.AsyncObject as PtrRequest)
			{
				if (ptrRequest == null)
				{
					throw new ArgumentException(NetException.WrongType, "asyncResult");
				}
				Request.Result result = (Request.Result)lazyAsyncResult.Result;
				domains = (string[])result.Data;
				server = result.Server;
				status = result.Status;
			}
			return status;
		}

		public static DnsStatus EndResolveToSrvRecords(IAsyncResult asyncResult, out SrvRecord[] srvRecords)
		{
			LazyAsyncResult lazyAsyncResult = asyncResult as LazyAsyncResult;
			if (lazyAsyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			DnsStatus result2;
			using (SrvRequest srvRequest = lazyAsyncResult.AsyncObject as SrvRequest)
			{
				if (srvRequest == null)
				{
					throw new ArgumentException(NetException.WrongType, "asyncResult");
				}
				Request.Result result = (Request.Result)lazyAsyncResult.Result;
				if (result != null)
				{
					srvRecords = (SrvRecord[])result.Data;
					result2 = result.Status;
				}
				else
				{
					srvRecords = null;
					result2 = DnsStatus.InfoNoRecords;
				}
			}
			return result2;
		}

		public static DnsStatus EndRetrieveSoaRecords(IAsyncResult asyncResult, out SoaRecord[] records)
		{
			LazyAsyncResult lazyAsyncResult = asyncResult as LazyAsyncResult;
			if (lazyAsyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			DnsStatus status;
			using (SoaRequest soaRequest = lazyAsyncResult.AsyncObject as SoaRequest)
			{
				if (soaRequest == null)
				{
					throw new ArgumentException(NetException.WrongType, "asyncResult");
				}
				Request.Result result = (Request.Result)lazyAsyncResult.Result;
				records = (SoaRecord[])result.Data;
				status = result.Status;
			}
			return status;
		}

		public static DnsStatus EndRetrieveNsRecords(IAsyncResult asyncResult, out DnsNsRecord[] records)
		{
			IPAddress ipaddress;
			return Dns.EndRetrieveNsRecords(asyncResult, out records, out ipaddress);
		}

		public static DnsStatus EndRetrieveNsRecords(IAsyncResult asyncResult, out DnsNsRecord[] records, out IPAddress server)
		{
			LazyAsyncResult lazyAsyncResult = asyncResult as LazyAsyncResult;
			if (lazyAsyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			DnsStatus status;
			using (NsRequest nsRequest = lazyAsyncResult.AsyncObject as NsRequest)
			{
				if (nsRequest == null)
				{
					throw new ArgumentException(NetException.WrongType, "asyncResult");
				}
				Request.Result result = (Request.Result)lazyAsyncResult.Result;
				records = (DnsNsRecord[])result.Data;
				server = result.Server;
				status = result.Status;
			}
			return status;
		}

		public void AdapterServerList(Guid adapterGuid)
		{
			this.AdapterServerList(adapterGuid, false, false);
		}

		public void AdapterServerList(Guid adapterGuid, bool excludeServersFromLoopbackAdapters, bool excludeIPv6SiteLocalAddresses)
		{
			IPAddress[] adapterDnsServerList = DnsServerList.GetAdapterDnsServerList(adapterGuid, excludeServersFromLoopbackAdapters, excludeIPv6SiteLocalAddresses);
			if (adapterDnsServerList == null || adapterDnsServerList.Length == 0)
			{
				Dns.EventLogger.LogEvent(CommonEventLogConstants.Tuple_NoAdapterDnsServerList, null, new object[]
				{
					adapterGuid
				});
				return;
			}
			this.InitializeServerList(adapterDnsServerList);
		}

		public void InitializeFromMachineServerList()
		{
			IPAddress[] machineDnsServerList = DnsServerList.GetMachineDnsServerList();
			if (machineDnsServerList == null || machineDnsServerList.Length == 0)
			{
				Dns.EventLogger.LogEvent(CommonEventLogConstants.Tuple_NoMachineDnsServerList, null, new object[0]);
				return;
			}
			this.InitializeServerList(machineDnsServerList);
		}

		public void InitializeServerList(IPAddress[] addresses)
		{
			DnsServerList dnsServerList = this.serverList;
			if (dnsServerList == null || !dnsServerList.IsAddressListIdentical(addresses))
			{
				DnsServerList dnsServerList2 = new DnsServerList();
				dnsServerList2.Initialize(addresses);
				DnsServerList dnsServerList3 = Interlocked.CompareExchange<DnsServerList>(ref this.serverList, dnsServerList2, dnsServerList);
				if (dnsServerList3 == dnsServerList)
				{
					if (dnsServerList3 != null)
					{
						dnsServerList3.Dispose();
						return;
					}
				}
				else
				{
					dnsServerList2.Dispose();
				}
			}
		}

		public IAsyncResult BeginResolveToAddresses(string domainName, AddressFamily type, AsyncCallback requestCallback, object state)
		{
			return this.BeginResolveToAddresses(domainName, type, this.ServerList, this.Options, requestCallback, state);
		}

		public IAsyncResult BeginResolveToAddresses(string domainName, AddressFamily type, DnsQueryOptions options, AsyncCallback requestCallback, object state)
		{
			return this.BeginResolveToAddresses(domainName, type, this.ServerList, options, requestCallback, state);
		}

		public IAsyncResult BeginResolveToAddresses(string domainName, AddressFamily type, DnsServerList list, DnsQueryOptions options, AsyncCallback requestCallback, object state)
		{
			if (string.IsNullOrEmpty(domainName))
			{
				throw new ArgumentNullException("domainName");
			}
			domainName = Dns.CleanseDomainName(domainName);
			if (!Dns.IsValidQuestion(domainName))
			{
				throw new ArgumentException(NetException.InvalidFQDN(domainName), "domainName");
			}
			if (type != AddressFamily.InterNetwork && type != AddressFamily.InterNetworkV6 && type != AddressFamily.Unspecified)
			{
				throw new ArgumentException(NetException.InvalidIPType, "type");
			}
			AddressRequest addressRequest = new AddressRequest(list, options, type, this, requestCallback, state);
			addressRequest.MaxWireDataSize = this.MaxDataPerRequest;
			addressRequest.RequestTimeout = DateTime.UtcNow + this.timeout;
			IAsyncResult result;
			try
			{
				result = addressRequest.Resolve(domainName);
			}
			catch (Exception)
			{
				addressRequest.Dispose();
				throw;
			}
			return result;
		}

		public IAsyncResult BeginResolveToAddresses(string domainName, DnsServerList list, DnsQueryOptions options, AsyncCallback requestCallback, object state)
		{
			if (string.IsNullOrEmpty(domainName))
			{
				throw new ArgumentNullException("domainName");
			}
			domainName = Dns.CleanseDomainName(domainName);
			if (!Dns.IsValidQuestion(domainName))
			{
				throw new ArgumentException(NetException.InvalidFQDN(domainName), "domainName");
			}
			AddressRequest addressRequest = new AddressRequest(list, options, this, requestCallback, state);
			addressRequest.MaxWireDataSize = this.MaxDataPerRequest;
			addressRequest.RequestTimeout = DateTime.UtcNow + this.timeout;
			IAsyncResult result;
			try
			{
				result = addressRequest.Resolve(domainName);
			}
			catch (Exception)
			{
				addressRequest.Dispose();
				throw;
			}
			return result;
		}

		public IAsyncResult BeginResolveToMailServers(string domainName, AsyncCallback requestCallback, object state)
		{
			return this.BeginResolveToMailServers(domainName, this.ServerList, this.Options, requestCallback, state);
		}

		public IAsyncResult BeginResolveToMailServers(string domainName, DnsQueryOptions options, AsyncCallback requestCallback, object state)
		{
			return this.BeginResolveToMailServers(domainName, this.ServerList, options, requestCallback, state);
		}

		public IAsyncResult BeginResolveToMailServers(string domainName, DnsServerList list, DnsQueryOptions options, AsyncCallback requestCallback, object state)
		{
			if (string.IsNullOrEmpty(domainName))
			{
				throw new ArgumentNullException("domainName");
			}
			domainName = Dns.CleanseDomainName(domainName);
			if (!Dns.IsValidQuestion(domainName))
			{
				throw new ArgumentException(NetException.InvalidFQDN(domainName), "domainName");
			}
			MxRequest mxRequest = new MxRequest(list, options, this.LocalIPAddresses, 0, this, requestCallback, state);
			DnsLog.Log(mxRequest, "ResolveMailServer for domain {0}. Servers={1}; options={2}; TimeOut={3}", new object[]
			{
				domainName,
				list,
				options,
				mxRequest.RequestTimeout
			});
			mxRequest.MaxWireDataSize = this.MaxDataPerRequest;
			mxRequest.RequestTimeout = DateTime.UtcNow + this.timeout;
			IAsyncResult result;
			try
			{
				result = mxRequest.Resolve(domainName);
			}
			catch (Exception)
			{
				mxRequest.Dispose();
				throw;
			}
			return result;
		}

		public IAsyncResult BeginResolveToMailServers(string domainName, bool implicitSearch, AddressFamily type, AsyncCallback requestCallback, object state)
		{
			if (string.IsNullOrEmpty(domainName))
			{
				throw new ArgumentNullException("domainName");
			}
			domainName = Dns.CleanseDomainName(domainName);
			if (!Dns.IsValidQuestion(domainName))
			{
				throw new ArgumentException(NetException.InvalidFQDN(domainName), "domainName");
			}
			MxRequest mxRequest = new MxRequest(this.serverList, this.Options, this.LocalIPAddresses, 0, type, implicitSearch, this, requestCallback, state);
			mxRequest.MaxWireDataSize = this.MaxDataPerRequest;
			mxRequest.RequestTimeout = DateTime.UtcNow + this.timeout;
			IAsyncResult result;
			try
			{
				result = mxRequest.Resolve(domainName);
			}
			catch (Exception)
			{
				mxRequest.Dispose();
				throw;
			}
			return result;
		}

		public IAsyncResult BeginRetrieveTextRecords(string domainName, AsyncCallback requestCallback, object state)
		{
			return this.BeginRetrieveTextRecords(domainName, this.ServerList, this.Options, requestCallback, state);
		}

		public IAsyncResult BeginRetrieveTextRecords(string domainName, DnsQueryOptions options, AsyncCallback requestCallback, object state)
		{
			return this.BeginRetrieveTextRecords(domainName, this.ServerList, options, requestCallback, state);
		}

		public IAsyncResult BeginRetrieveTextRecords(string domainName, DnsServerList list, DnsQueryOptions options, AsyncCallback requestCallback, object state)
		{
			if (string.IsNullOrEmpty(domainName))
			{
				throw new ArgumentNullException("domainName");
			}
			domainName = Dns.CleanseDomainName(domainName);
			if (!Dns.IsValidQuestion(domainName))
			{
				throw new ArgumentException(NetException.InvalidFQDN(domainName), "domainName");
			}
			TxtRequest txtRequest = new TxtRequest(list, options, this, requestCallback, state);
			txtRequest.MaxWireDataSize = this.MaxDataPerRequest;
			txtRequest.RequestTimeout = DateTime.UtcNow + this.timeout;
			IAsyncResult result;
			try
			{
				result = txtRequest.Resolve(domainName);
			}
			catch (Exception)
			{
				txtRequest.Dispose();
				throw;
			}
			return result;
		}

		public IAsyncResult BeginRetrieveSoaRecords(string domainName, AsyncCallback requestCallback, object state)
		{
			return this.BeginRetrieveSoaRecords(domainName, this.ServerList, this.Options, requestCallback, state);
		}

		public IAsyncResult BeginRetrieveSoaRecords(string domainName, DnsQueryOptions options, AsyncCallback requestCallback, object state)
		{
			return this.BeginRetrieveSoaRecords(domainName, this.ServerList, options, requestCallback, state);
		}

		public IAsyncResult BeginRetrieveSoaRecords(string domainName, DnsServerList list, DnsQueryOptions options, AsyncCallback requestCallback, object state)
		{
			if (string.IsNullOrEmpty(domainName))
			{
				throw new ArgumentNullException("domainName");
			}
			domainName = Dns.CleanseDomainName(domainName);
			if (!Dns.IsValidQuestion(domainName))
			{
				throw new ArgumentException(NetException.InvalidFQDN(domainName), "domainName");
			}
			SoaRequest soaRequest = new SoaRequest(list, options, this, requestCallback, state);
			soaRequest.MaxWireDataSize = this.MaxDataPerRequest;
			soaRequest.RequestTimeout = DateTime.UtcNow + this.timeout;
			IAsyncResult result;
			try
			{
				result = soaRequest.Resolve(domainName);
			}
			catch (Exception)
			{
				soaRequest.Dispose();
				throw;
			}
			return result;
		}

		public IAsyncResult BeginRetrieveCNameRecords(string domainName, AsyncCallback requestCallback, object state)
		{
			return this.BeginRetrieveCNameRecords(domainName, this.ServerList, this.Options, requestCallback, state);
		}

		public IAsyncResult BeginRetrieveCNameRecords(string domainName, DnsQueryOptions options, AsyncCallback requestCallback, object state)
		{
			return this.BeginRetrieveCNameRecords(domainName, this.ServerList, options, requestCallback, state);
		}

		public IAsyncResult BeginRetrieveCNameRecords(string domainName, DnsServerList list, DnsQueryOptions options, AsyncCallback requestCallback, object state)
		{
			if (string.IsNullOrEmpty(domainName))
			{
				throw new ArgumentNullException("domainName");
			}
			domainName = Dns.CleanseDomainName(domainName);
			if (!Dns.IsValidQuestion(domainName))
			{
				throw new ArgumentException(NetException.InvalidFQDN(domainName), "domainName");
			}
			CNameRequest cnameRequest = new CNameRequest(list, options, this, requestCallback, state);
			cnameRequest.MaxWireDataSize = this.MaxDataPerRequest;
			cnameRequest.RequestTimeout = DateTime.UtcNow + this.timeout;
			IAsyncResult result;
			try
			{
				result = cnameRequest.Resolve(domainName);
			}
			catch (Exception)
			{
				cnameRequest.Dispose();
				throw;
			}
			return result;
		}

		public IAsyncResult BeginResolveToNames(IPAddress address, AsyncCallback requestCallback, object state)
		{
			return this.BeginResolveToNames(address, this.ServerList, this.Options, requestCallback, state);
		}

		public IAsyncResult BeginResolveToNames(IPAddress address, DnsQueryOptions options, AsyncCallback requestCallback, object state)
		{
			return this.BeginResolveToNames(address, this.ServerList, options, requestCallback, state);
		}

		public IAsyncResult BeginResolveToNames(IPAddress address, DnsServerList list, DnsQueryOptions options, AsyncCallback requestCallback, object state)
		{
			PtrRequest ptrRequest = new PtrRequest(list, options, this, requestCallback, state);
			ptrRequest.MaxWireDataSize = this.MaxDataPerRequest;
			ptrRequest.RequestTimeout = DateTime.UtcNow + this.timeout;
			IAsyncResult result;
			try
			{
				result = ptrRequest.Resolve(address);
			}
			catch (Exception)
			{
				ptrRequest.Dispose();
				throw;
			}
			return result;
		}

		public IAsyncResult BeginRetrieveSrvRecords(string name, DnsQueryOptions options, AsyncCallback requestCallback, object state)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			SrvRequest srvRequest = new SrvRequest(this.ServerList, options, this, requestCallback, state);
			srvRequest.MaxWireDataSize = this.MaxDataPerRequest;
			srvRequest.RequestTimeout = DateTime.UtcNow + this.timeout;
			IAsyncResult result;
			try
			{
				result = srvRequest.Resolve(name);
			}
			catch (Exception)
			{
				srvRequest.Dispose();
				throw;
			}
			return result;
		}

		public IAsyncResult BeginRetrieveNsRecords(string domainName, AsyncCallback requestCallback, object state)
		{
			return this.BeginRetrieveNsRecords(domainName, this.ServerList, this.Options, requestCallback, state);
		}

		public IAsyncResult BeginRetrieveNsRecords(string domainName, DnsQueryOptions options, AsyncCallback requestCallback, object state)
		{
			return this.BeginRetrieveNsRecords(domainName, this.ServerList, options, requestCallback, state);
		}

		public IAsyncResult BeginRetrieveNsRecords(string domainName, DnsServerList list, DnsQueryOptions options, AsyncCallback requestCallback, object state)
		{
			if (string.IsNullOrEmpty(domainName))
			{
				throw new ArgumentNullException("domainName");
			}
			domainName = Dns.CleanseDomainName(domainName);
			if (!Dns.IsValidQuestion(domainName))
			{
				throw new ArgumentException(NetException.InvalidFQDN(domainName), "domainName");
			}
			NsRequest nsRequest = new NsRequest(list, options, this, requestCallback, state);
			nsRequest.MaxWireDataSize = this.MaxDataPerRequest;
			nsRequest.RequestTimeout = DateTime.UtcNow + this.timeout;
			IAsyncResult result;
			try
			{
				result = nsRequest.Resolve(domainName);
			}
			catch (Exception)
			{
				nsRequest.Dispose();
				throw;
			}
			return result;
		}

		internal static bool IsValidQuestion(string name)
		{
			return !string.IsNullOrEmpty(name) && name[0] != '*';
		}

		internal static bool IsValidName(string name)
		{
			return Dns.ValidateName(DNSNameFormat.Domain, name) == DNSNameStatus.Valid;
		}

		internal static bool IsValidWildcardName(string name)
		{
			return Dns.ValidateName(DNSNameFormat.Wildcard, name) == DNSNameStatus.Valid;
		}

		internal static DNSNameStatus ValidateName(DNSNameFormat format, string name)
		{
			if (!string.IsNullOrEmpty(name))
			{
				return (DNSNameStatus)DnsNativeMethods.ValidateName(name, (int)format);
			}
			return DNSNameStatus.InvalidName;
		}

		internal static string DnsStatusToString(DnsStatus key)
		{
			int num = (int)key;
			if (Dns.enumStatusString == null)
			{
				string[] names = Enum.GetNames(typeof(DnsStatus));
				Interlocked.Exchange<string[]>(ref Dns.enumStatusString, names);
			}
			if (num < 0 || num >= Dns.enumStatusString.Length)
			{
				return num.ToString("X8", NumberFormatInfo.InvariantInfo);
			}
			return Dns.enumStatusString[num];
		}

		internal static string TrimTrailingDot(string name)
		{
			if (string.IsNullOrEmpty(name) || name[name.Length - 1] != '.')
			{
				return name;
			}
			return name.Substring(0, name.Length - 1);
		}

		internal static string CleanseDomainName(string domainName)
		{
			domainName = Dns.TrimTrailingDot(domainName);
			return new IdnMapping().GetAscii(domainName);
		}

		private static AddressFamily GetDefaultAddressFamily()
		{
			AddressFamily result = AddressFamily.Unspecified;
			if (Socket.OSSupportsIPv4)
			{
				if (!Socket.OSSupportsIPv6)
				{
					result = AddressFamily.InterNetwork;
				}
			}
			else
			{
				if (!Socket.OSSupportsIPv6)
				{
					throw new NotSupportedException("Neither IPv4 nor IPv6 supported on this machine!");
				}
				result = AddressFamily.InterNetworkV6;
			}
			return result;
		}

		internal const int MaxTargetHosts = 100;

		private static int maxCnameRecords = 10;

		private static int maxSubQueries = 50;

		private int maxDataPerRequest = 8192;

		private DnsServerList serverList = new DnsServerList();

		private IEnumerable<IPAddress> localAddresses;

		private DnsQueryOptions queryOptions;

		private static string[] enumStatusString;

		private static ExEventLog eventLogger;

		private TimeSpan timeout = TimeSpan.FromSeconds(60.0);

		private TimeSpan queryRetryInterval = TimeSpan.FromSeconds(5.0);

		private AddressFamily defaultAddressFamily = Dns.GetDefaultAddressFamily();
	}
}
