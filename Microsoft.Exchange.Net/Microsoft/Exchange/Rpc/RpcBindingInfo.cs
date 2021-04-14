using System;
using System.Net;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Rpc
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RpcBindingInfo : ICloneable
	{
		public RpcBindingInfo()
		{
			this.RpcAuthentication = AuthenticationService.Negotiate;
			this.Timeout = TimeSpan.FromMinutes(5.0);
			this.WebProxyServer = null;
			this.RpcProxyPort = RpcProxyPort.Default;
			this.UseSsl = true;
			this.RpcHttpCookies = new CookieCollection();
			this.RpcHttpHeaders = new WebHeaderCollection();
			this.AllowImpersonation = false;
			this.AllowRpcRetry = true;
			this.IgnoreInvalidRpcProxyServerCertificateSubject = false;
			this.UseUniqueBinding = true;
		}

		public bool AllowRpcRetry { get; set; }

		public string ClientCertificateSubjectName { get; set; }

		public Guid ClientObjectGuid { get; set; }

		public NetworkCredential Credential { get; set; }

		public string ProtocolSequence
		{
			get
			{
				return this.protocolSequence;
			}
			set
			{
				this.protocolSequence = ((value != null) ? string.Intern(value.ToLowerInvariant()) : null);
			}
		}

		public string RpcServer { get; set; }

		public int? RpcPort { get; set; }

		public string ServicePrincipalName { get; set; }

		public AuthenticationService RpcAuthentication { get; set; }

		public bool UseRpcEncryption
		{
			get
			{
				bool? flag = this.useRpcEncryption;
				if (flag == null)
				{
					return this.RpcAuthentication != AuthenticationService.None;
				}
				return flag.GetValueOrDefault();
			}
			set
			{
				this.useRpcEncryption = new bool?(value);
			}
		}

		public TimeSpan Timeout { get; set; }

		public Uri Uri
		{
			get
			{
				RpcBindingInfo rpcBindingInfo = this.Clone();
				rpcBindingInfo.DefaultOmittedProperties();
				return rpcBindingInfo.CreateUri();
			}
		}

		public bool IsRpcServerLocalMachine
		{
			get
			{
				return string.Equals(this.RpcServer, "localhost", StringComparison.OrdinalIgnoreCase) || string.Equals(this.RpcServer, Environment.MachineName, StringComparison.OrdinalIgnoreCase) || string.Equals(this.RpcServer, ComputerInformation.DnsHostName, StringComparison.OrdinalIgnoreCase) || string.Equals(this.RpcServer, ComputerInformation.DnsFullyQualifiedDomainName, StringComparison.OrdinalIgnoreCase);
			}
		}

		public string RpcProxyServer
		{
			get
			{
				return this.rpcProxyServer;
			}
			set
			{
				string text = value;
				if (!string.IsNullOrEmpty(value))
				{
					int num = value.IndexOf(':');
					if (num >= 0)
					{
						text = value.Substring(0, num);
						this.RpcProxyPort = (RpcProxyPort)int.Parse(value.Substring(num + 1));
					}
				}
				this.rpcProxyServer = text;
			}
		}

		public string WebProxyServer { get; set; }

		public RpcProxyPort RpcProxyPort { get; set; }

		public HttpAuthenticationScheme RpcProxyAuthentication { get; set; }

		public bool UseSsl { get; set; }

		public WebHeaderCollection RpcHttpHeaders { get; private set; }

		public CookieCollection RpcHttpCookies { get; private set; }

		public bool AllowImpersonation { get; set; }

		public bool UseUniqueBinding { get; set; }

		public bool UseExplicitEndpointLookup { get; set; }

		public bool IgnoreInvalidRpcProxyServerCertificateSubject { get; set; }

		public string ExpectedRpcProxyServerCertificateSubject { get; set; }

		public RpcBindingInfo Clone()
		{
			return (RpcBindingInfo)base.MemberwiseClone();
		}

		object ICloneable.Clone()
		{
			return this.Clone();
		}

		public bool PackHeadersAndCookiesIntoRpcCookie(out string cookieName, out string cookieValue)
		{
			if (this.RpcHttpCookies.Count == 0 && this.RpcHttpHeaders.Count == 0)
			{
				cookieName = null;
				cookieValue = null;
				return false;
			}
			StringBuilder stringBuilder = new StringBuilder();
			cookieName = string.Empty;
			for (int i = 0; i < this.RpcHttpCookies.Count; i++)
			{
				if (i == 0)
				{
					cookieName = this.RpcHttpCookies[0].Name;
					stringBuilder.Append(this.RpcHttpCookies[0].Value);
				}
				else
				{
					stringBuilder.AppendFormat("; {0}={1}", this.RpcHttpCookies[i].Name, this.RpcHttpCookies[i].Value);
				}
			}
			if (this.RpcHttpHeaders.Count > 0)
			{
				stringBuilder.AppendLine();
				string text = this.RpcHttpHeaders.ToString();
				if (text.EndsWith("\r\n\r\n"))
				{
					text = text.Substring(0, text.Length - "\r\n\r\n".Length);
				}
				stringBuilder.Append(text);
			}
			cookieValue = stringBuilder.ToString();
			return true;
		}

		public RpcBindingInfo DefaultOmittedProperties()
		{
			if (string.IsNullOrEmpty(this.RpcServer))
			{
				this.RpcServer = ComputerInformation.DnsFullyQualifiedDomainName;
			}
			this.DefaultProtocolSequence();
			this.DefaultRpcHttpSettings();
			this.ForceLRpcSettings();
			this.UseExplicitEndpointLookup &= (this.ClientObjectGuid != Guid.Empty);
			return this;
		}

		public static string BuildKerberosSpn(string serviceClass, string hostName)
		{
			return string.Format("{0}/{1}", serviceClass ?? "host", hostName ?? ComputerInformation.DnsFullyQualifiedDomainName);
		}

		public RpcBindingInfo UseProtocolSequenceWithOptionalRpcPortSpecification(string protocolSequence)
		{
			int num;
			if (protocolSequence != null && (num = protocolSequence.IndexOf(':')) != -1)
			{
				this.ProtocolSequence = protocolSequence.Substring(0, num);
				this.RpcPort = new int?(int.Parse(protocolSequence.Substring(num + 1)));
			}
			else
			{
				this.ProtocolSequence = protocolSequence;
			}
			return this;
		}

		public RpcBindingInfo UseTcp()
		{
			this.ProtocolSequence = "ncacn_ip_tcp";
			return this;
		}

		public RpcBindingInfo UseRpcProxy(int rpcPort, string rpcProxyServer, RpcProxyPort rpcProxyPort)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("rpcProxyServer", rpcProxyServer);
			this.ProtocolSequence = "ncacn_http";
			this.RpcPort = new int?(rpcPort);
			this.RpcProxyServer = rpcProxyServer;
			this.RpcProxyPort = rpcProxyPort;
			return this;
		}

		public RpcBindingInfo UseKerberosSpn(string serviceClass, string hostName)
		{
			if (string.IsNullOrEmpty(hostName) && !string.IsNullOrEmpty(this.RpcServer) && this.IsRealServerName(this.RpcServer))
			{
				hostName = this.RpcServer;
			}
			this.ServicePrincipalName = RpcBindingInfo.BuildKerberosSpn(serviceClass, hostName);
			return this;
		}

		private Uri CreateUri()
		{
			UriBuilder uriBuilder = new UriBuilder();
			uriBuilder.Scheme = this.ProtocolSequence.Substring(this.ProtocolSequence.LastIndexOf('_') + 1);
			uriBuilder.Host = this.RpcServer;
			if (this.RpcPort != null)
			{
				uriBuilder.Port = this.RpcPort.Value;
			}
			string a;
			if ((a = this.ProtocolSequence) != null)
			{
				if (!(a == "ncacn_http"))
				{
					if (a == "ncacn_ip_tcp")
					{
						uriBuilder.Scheme = Uri.UriSchemeNetTcp;
					}
				}
				else
				{
					uriBuilder.Scheme = (this.UseSsl ? Uri.UriSchemeHttps : Uri.UriSchemeHttp);
					uriBuilder.Host = this.RpcProxyServer;
					uriBuilder.Port = (int)this.RpcProxyPort;
					uriBuilder.Path = "rpc/rpcproxy.dll";
					uriBuilder.Query = ((this.RpcPort != null) ? string.Format("{0}:{1}", this.RpcServer, this.RpcPort) : this.RpcServer);
				}
			}
			return uriBuilder.Uri;
		}

		private void DefaultProtocolSequence()
		{
			if (string.IsNullOrEmpty(this.ProtocolSequence))
			{
				if (this.Credential != null)
				{
					this.UseTcp();
					return;
				}
				if (this.IsRpcServerLocalMachine)
				{
					this.UseProtocolSequenceWithOptionalRpcPortSpecification("ncalrpc");
					return;
				}
				this.UseTcp();
			}
		}

		private void DefaultRpcHttpSettings()
		{
			if (this.ProtocolSequence == "ncacn_http")
			{
				if (string.IsNullOrEmpty(this.RpcProxyServer))
				{
					this.RpcProxyServer = this.RpcServer;
					if (this.IsRpcServerLocalMachine)
					{
						this.RpcServer = Environment.MachineName;
					}
				}
				if (this.RpcPort == null)
				{
					this.RpcPort = new int?(6001);
				}
				if (this.RpcProxyAuthentication == HttpAuthenticationScheme.Undefined)
				{
					this.RpcProxyAuthentication = ((!this.UseSsl || this.RpcProxyPort == RpcProxyPort.Backend) ? HttpAuthenticationScheme.Negotiate : HttpAuthenticationScheme.Basic);
				}
			}
		}

		private void ForceLRpcSettings()
		{
			if (this.ProtocolSequence == "ncalrpc")
			{
				this.RpcServer = Environment.MachineName;
				this.RpcAuthentication = AuthenticationService.Ntlm;
			}
		}

		private bool IsRealServerName(string serverName)
		{
			return !serverName.Contains("@");
		}

		public const int OutlookConsolidatedEndpoint = 6001;

		public const string OutlookSessionCookieName = "OutlookSession";

		public const string RpcProxyPath = "rpc/rpcproxy.dll";

		public const string LocalHost = "localhost";

		public const string WebProxyNone = "<none>";

		public const string WebProxyAuto = null;

		private string protocolSequence;

		private bool? useRpcEncryption;

		private string rpcProxyServer;
	}
}
