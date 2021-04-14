using System;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.WebService
{
	public class WebServiceConfiguration
	{
		public WebServiceConfiguration()
		{
			this.BindingType = "WSHttpBinding";
			this.CloseTimeout = TimeSpan.MaxValue;
			this.OpenTimeout = TimeSpan.MaxValue;
			this.ReceiveTimeout = TimeSpan.MaxValue;
			this.SendTimeout = TimeSpan.MaxValue;
			this.InactivityTimeout = TimeSpan.MaxValue;
			this.AllowCookies = null;
			this.BypassProxyOnLocal = null;
			this.TransactionFlow = null;
			this.Ordered = null;
			this.ReliableSessionEnabled = null;
			this.EstablishSecurityContext = null;
			this.NegotiateServiceCredential = null;
			this.MaxConnections = 0;
			this.MaxDepth = 0;
			this.MaxStringContentLength = 0;
			this.MaxArrayLength = 0;
			this.MaxBytesPerRead = 0;
			this.MaxNameTableCharCount = 0;
			this.MaxReceivedMessageSize = 0L;
			this.maxBufferPoolSize = 0;
			this.MaxBufferSize = 0;
			this.FwLinkEnabled = false;
			this.WebProxyEnabled = false;
			this.UseDefaultWebProxy = true;
			this.WebProxyPort = 80;
			this.WebProxyCredentialsRequired = false;
		}

		internal string Uri
		{
			get
			{
				return this.uri;
			}
			set
			{
				this.uri = value;
			}
		}

		internal string BindingType
		{
			get
			{
				return this.bindingType;
			}
			set
			{
				this.bindingType = value;
			}
		}

		internal string AllowCookies
		{
			get
			{
				return this.allowCookies;
			}
			set
			{
				this.allowCookies = value;
			}
		}

		internal string BypassProxyOnLocal
		{
			get
			{
				return this.bypassProxyOnLocal;
			}
			set
			{
				this.bypassProxyOnLocal = value;
			}
		}

		internal string ClientBaseAddress
		{
			get
			{
				return this.clientBaseAddress;
			}
			set
			{
				this.clientBaseAddress = value;
			}
		}

		internal TimeSpan CloseTimeout
		{
			get
			{
				return this.closeTimeout;
			}
			set
			{
				this.closeTimeout = value;
			}
		}

		internal string HostNameComparisonMode
		{
			get
			{
				return this.hostNameComparisonMode;
			}
			set
			{
				this.hostNameComparisonMode = value;
			}
		}

		internal int ListenBacklog
		{
			get
			{
				return this.listenBacklog;
			}
			set
			{
				this.listenBacklog = value;
			}
		}

		internal int MaxBufferPoolSize
		{
			get
			{
				return this.maxBufferPoolSize;
			}
			set
			{
				this.maxBufferPoolSize = value;
			}
		}

		internal int MaxBufferSize
		{
			get
			{
				return this.maxBufferSize;
			}
			set
			{
				this.maxBufferSize = value;
			}
		}

		internal int MaxConnections
		{
			get
			{
				return this.maxConnections;
			}
			set
			{
				this.maxConnections = value;
			}
		}

		internal long MaxReceivedMessageSize
		{
			get
			{
				return this.maxReceivedMessageSize;
			}
			set
			{
				this.maxReceivedMessageSize = value;
			}
		}

		internal string MessageEncoding
		{
			get
			{
				return this.messageEncoding;
			}
			set
			{
				this.messageEncoding = value;
			}
		}

		internal string BindingName
		{
			get
			{
				return this.bindingName;
			}
			set
			{
				this.bindingName = value;
			}
		}

		internal string BindingNamespace
		{
			get
			{
				return this.bindingNamespace;
			}
			set
			{
				this.bindingNamespace = value;
			}
		}

		internal TimeSpan OpenTimeout
		{
			get
			{
				return this.openTimeout;
			}
			set
			{
				this.openTimeout = value;
			}
		}

		internal TimeSpan ReceiveTimeout
		{
			get
			{
				return this.receiveTimeout;
			}
			set
			{
				this.receiveTimeout = value;
			}
		}

		internal TimeSpan SendTimeout
		{
			get
			{
				return this.sendTimeout;
			}
			set
			{
				this.sendTimeout = value;
			}
		}

		internal string TextEncoding
		{
			get
			{
				return this.textEncoding;
			}
			set
			{
				this.textEncoding = value;
			}
		}

		internal string TransactionFlow
		{
			get
			{
				return this.transactionFlow;
			}
			set
			{
				this.transactionFlow = value;
			}
		}

		internal string TransferMode
		{
			get
			{
				return this.transferMode;
			}
			set
			{
				this.transferMode = value;
			}
		}

		internal string TransactionProtocol
		{
			get
			{
				return this.transactionProtocol;
			}
			set
			{
				this.transactionProtocol = value;
			}
		}

		internal int MaxDepth
		{
			get
			{
				return this.maxDepth;
			}
			set
			{
				this.maxDepth = value;
			}
		}

		internal int MaxStringContentLength
		{
			get
			{
				return this.maxStringContentLength;
			}
			set
			{
				this.maxStringContentLength = value;
			}
		}

		internal int MaxArrayLength
		{
			get
			{
				return this.maxArrayLength;
			}
			set
			{
				this.maxArrayLength = value;
			}
		}

		internal int MaxBytesPerRead
		{
			get
			{
				return this.maxBytesPerRead;
			}
			set
			{
				this.maxBytesPerRead = value;
			}
		}

		internal int MaxNameTableCharCount
		{
			get
			{
				return this.maxNameTableCharCount;
			}
			set
			{
				this.maxNameTableCharCount = value;
			}
		}

		internal string Ordered
		{
			get
			{
				return this.ordered;
			}
			set
			{
				this.ordered = value;
			}
		}

		internal TimeSpan InactivityTimeout
		{
			get
			{
				return this.inactivityTimeout;
			}
			set
			{
				this.inactivityTimeout = value;
			}
		}

		internal bool? ReliableSessionEnabled
		{
			get
			{
				return this.reliableSessionEnabled;
			}
			set
			{
				this.reliableSessionEnabled = value;
			}
		}

		internal string SecurityMode
		{
			get
			{
				return this.securityMode;
			}
			set
			{
				this.securityMode = value;
			}
		}

		internal bool UseCertificateAuthentication
		{
			get
			{
				return (!string.IsNullOrWhiteSpace(this.MessageCredentialType) && this.MessageCredentialType.Equals("Certificate", StringComparison.OrdinalIgnoreCase)) || (!string.IsNullOrWhiteSpace(this.TransportCredentialType) && this.TransportCredentialType.Equals("Certificate", StringComparison.OrdinalIgnoreCase));
			}
		}

		internal bool UseUserNameAuthentication
		{
			get
			{
				return !string.IsNullOrWhiteSpace(this.MessageCredentialType) && this.MessageCredentialType.Equals("UserName", StringComparison.OrdinalIgnoreCase);
			}
		}

		internal string TransportCredentialType
		{
			get
			{
				return this.transportCredentialType;
			}
			set
			{
				this.transportCredentialType = value;
			}
		}

		internal string Realm
		{
			get
			{
				return this.realm;
			}
			set
			{
				this.realm = value;
			}
		}

		internal string ProtectionLevel
		{
			get
			{
				return this.protectionLevel;
			}
			set
			{
				this.protectionLevel = value;
			}
		}

		internal string MessageCredentialType
		{
			get
			{
				return this.messageCredentialType;
			}
			set
			{
				this.messageCredentialType = value;
			}
		}

		internal string AlgorithmSuite
		{
			get
			{
				return this.algorithmSuite;
			}
			set
			{
				this.algorithmSuite = value;
			}
		}

		internal string EstablishSecurityContext
		{
			get
			{
				return this.establishSecurityContext;
			}
			set
			{
				this.establishSecurityContext = value;
			}
		}

		internal string NegotiateServiceCredential
		{
			get
			{
				return this.negotiateServiceCredential;
			}
			set
			{
				this.negotiateServiceCredential = value;
			}
		}

		internal string Username
		{
			get
			{
				return this.username;
			}
			set
			{
				this.username = value;
			}
		}

		internal string Password
		{
			get
			{
				return this.password;
			}
			set
			{
				this.password = value;
			}
		}

		internal StoreLocation StoreLocation
		{
			get
			{
				return this.storeLocation;
			}
			set
			{
				this.storeLocation = value;
			}
		}

		internal StoreName StoreName
		{
			get
			{
				return this.storeName;
			}
			set
			{
				this.storeName = value;
			}
		}

		internal X509FindType X509FindType
		{
			get
			{
				return this.x509FindType;
			}
			set
			{
				this.x509FindType = value;
			}
		}

		internal string FindValue
		{
			get
			{
				return this.findValue;
			}
			set
			{
				this.findValue = value;
			}
		}

		internal string ServiceCertificateValidationMode
		{
			get
			{
				return this.serviceCertificateValidationMode;
			}
			set
			{
				this.serviceCertificateValidationMode = value;
			}
		}

		internal string ProxyClassName
		{
			get
			{
				return this.proxyClassName;
			}
			set
			{
				this.proxyClassName = value;
			}
		}

		internal string ProxyValidatorClassName
		{
			get
			{
				return this.proxyValidatorClassName;
			}
			set
			{
				this.proxyValidatorClassName = value;
			}
		}

		internal string ProxyValidatorMethodName
		{
			get
			{
				return this.proxyValidatorMethodName;
			}
			set
			{
				this.proxyValidatorMethodName = value;
			}
		}

		internal string ProxyDiagnosticsInfoMethodName
		{
			get
			{
				return this.proxyDiagnosticsInfoMethodName;
			}
			set
			{
				this.proxyDiagnosticsInfoMethodName = value;
			}
		}

		internal bool DumpDiagnosticsInfoOnSuccess
		{
			get
			{
				return this.dumpDiagnosticsInfoOnSuccess;
			}
			set
			{
				this.dumpDiagnosticsInfoOnSuccess = value;
			}
		}

		internal string ProxyAssembly
		{
			get
			{
				return this.proxyAssembly;
			}
			set
			{
				this.proxyAssembly = value;
			}
		}

		internal bool ProxyGenerated
		{
			get
			{
				return this.proxyGenerated;
			}
			set
			{
				this.proxyGenerated = value;
			}
		}

		internal bool ProxyInstanceMethodIsStatic
		{
			get
			{
				return this.proxyInstanceMethodIsStatic;
			}
			set
			{
				this.proxyInstanceMethodIsStatic = value;
			}
		}

		internal Operation ProxyInstanceMethod
		{
			get
			{
				return this.proxyInstanceMethod;
			}
			set
			{
				this.proxyInstanceMethod = value;
			}
		}

		internal bool WebProxyEnabled
		{
			get
			{
				return this.webProxyEnabled;
			}
			set
			{
				this.webProxyEnabled = value;
			}
		}

		internal bool UseDefaultWebProxy
		{
			get
			{
				return this.useDefaultWebProxy;
			}
			set
			{
				this.useDefaultWebProxy = value;
			}
		}

		internal string WebProxyServerUri
		{
			get
			{
				return this.webProxyServerUri;
			}
			set
			{
				this.webProxyServerUri = value;
			}
		}

		internal int WebProxyPort
		{
			get
			{
				return this.webProxyPort;
			}
			set
			{
				this.webProxyPort = value;
			}
		}

		internal bool WebProxyCredentialsRequired
		{
			get
			{
				return this.webProxyCredentialsRequired;
			}
			set
			{
				this.webProxyCredentialsRequired = value;
			}
		}

		internal string WebProxyUsername
		{
			get
			{
				return this.webProxyUsername;
			}
			set
			{
				this.webProxyUsername = value;
			}
		}

		internal string WebProxyPassword
		{
			get
			{
				return this.webProxyPassword;
			}
			set
			{
				this.webProxyPassword = value;
			}
		}

		internal string WebProxyDomain
		{
			get
			{
				return this.webProxyDomain;
			}
			set
			{
				this.webProxyDomain = value;
			}
		}

		internal bool FwLinkEnabled
		{
			get
			{
				return this.fwLinkEnabled;
			}
			set
			{
				this.fwLinkEnabled = value;
			}
		}

		internal string FwLinkUri
		{
			get
			{
				return this.fwLinkUri;
			}
			set
			{
				this.fwLinkUri = value;
			}
		}

		private string uri;

		private string bindingType;

		private string allowCookies;

		private string bypassProxyOnLocal;

		private string clientBaseAddress;

		private TimeSpan closeTimeout;

		private string hostNameComparisonMode;

		private int listenBacklog;

		private int maxConnections;

		private int maxBufferPoolSize;

		private int maxBufferSize;

		private long maxReceivedMessageSize;

		private string messageEncoding;

		private string bindingName;

		private string bindingNamespace;

		private TimeSpan openTimeout;

		private TimeSpan receiveTimeout;

		private TimeSpan sendTimeout;

		private string textEncoding;

		private string transactionFlow;

		private string transferMode;

		private string transactionProtocol;

		private int maxDepth;

		private int maxStringContentLength;

		private int maxArrayLength;

		private int maxBytesPerRead;

		private int maxNameTableCharCount;

		private string ordered;

		private TimeSpan inactivityTimeout;

		private bool? reliableSessionEnabled;

		private string securityMode;

		private string transportCredentialType;

		private string realm;

		private string protectionLevel;

		private string messageCredentialType;

		private string algorithmSuite;

		private string establishSecurityContext;

		private string negotiateServiceCredential;

		private string username;

		private string password;

		private StoreLocation storeLocation;

		private StoreName storeName;

		private X509FindType x509FindType;

		private string findValue;

		private string serviceCertificateValidationMode;

		private string proxyClassName;

		private string proxyValidatorClassName;

		private string proxyValidatorMethodName;

		private string proxyDiagnosticsInfoMethodName;

		private bool dumpDiagnosticsInfoOnSuccess;

		private string proxyAssembly;

		private bool proxyGenerated;

		private bool proxyInstanceMethodIsStatic;

		private Operation proxyInstanceMethod;

		private bool webProxyEnabled;

		private bool useDefaultWebProxy;

		private string webProxyServerUri;

		private int webProxyPort;

		private bool webProxyCredentialsRequired;

		private string webProxyUsername;

		private string webProxyPassword;

		private string webProxyDomain;

		private bool fwLinkEnabled;

		private string fwLinkUri;
	}
}
