using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Text;
using System.Xml;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.WebService
{
	internal class WebServiceClient
	{
		public WebServiceClient(WebServiceConfiguration config)
		{
			if (config == null)
			{
				throw new ArgumentException("config");
			}
			this.configuration = config;
			string text = this.Configuration.ProxyAssembly;
			if (!File.Exists(text))
			{
				string fileName = Path.GetFileName(this.Configuration.ProxyAssembly);
				text = Path.Combine(".", fileName);
				if (!File.Exists(text))
				{
					text = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), fileName);
				}
			}
			try
			{
				this.assembly = Assembly.LoadFrom(text);
			}
			catch
			{
				Type type = Type.GetType(this.Configuration.ProxyClassName, false);
				if (!(type != null))
				{
					throw;
				}
				this.assembly = type.Assembly;
			}
			ServicePointManager.EnableDnsRoundRobin = true;
			ServicePointManager.DnsRefreshTimeout = 100000;
			ServicePointManager.MaxServicePointIdleTime = 90000;
			if (this.Configuration.ProxyGenerated)
			{
				this.proxy = this.CreateProxy();
				return;
			}
			this.proxy = this.CreateNonGeneratedProxy();
		}

		internal object Proxy
		{
			get
			{
				return this.proxy;
			}
		}

		internal WebServiceConfiguration Configuration
		{
			get
			{
				return this.configuration;
			}
		}

		internal Assembly Assembly
		{
			get
			{
				return this.assembly;
			}
		}

		internal Binding Binding
		{
			get
			{
				Binding binding;
				if (this.Configuration.BindingType.Equals("WSHttpBinding", StringComparison.OrdinalIgnoreCase))
				{
					binding = this.WSHttpBinding;
				}
				else if (this.Configuration.BindingType.Equals("NetTcpBinding", StringComparison.OrdinalIgnoreCase))
				{
					binding = this.NetTcpBinding;
				}
				else if (this.Configuration.BindingType.Equals("BasicHttpBinding", StringComparison.OrdinalIgnoreCase))
				{
					binding = this.BasicHttpBinding;
				}
				else if (this.Configuration.BindingType.Equals("WSDualHttpBinding", StringComparison.OrdinalIgnoreCase))
				{
					binding = this.WSDualHttpBinding;
				}
				else
				{
					if (!this.Configuration.BindingType.Equals("NetNamedPipeBinding", StringComparison.OrdinalIgnoreCase))
					{
						throw new ArgumentException(string.Format("Binding {0} not supported.", this.Configuration.BindingType));
					}
					binding = this.NetNamedPipeBinding;
				}
				if (this.Configuration.SendTimeout != TimeSpan.MaxValue)
				{
					binding.SendTimeout = this.Configuration.SendTimeout;
				}
				if (this.Configuration.ReceiveTimeout != TimeSpan.MaxValue)
				{
					binding.ReceiveTimeout = this.Configuration.ReceiveTimeout;
				}
				if (this.Configuration.OpenTimeout != TimeSpan.MaxValue)
				{
					binding.OpenTimeout = this.Configuration.OpenTimeout;
				}
				if (this.Configuration.CloseTimeout != TimeSpan.MaxValue)
				{
					binding.CloseTimeout = this.Configuration.CloseTimeout;
				}
				return binding;
			}
		}

		internal WSHttpBinding WSHttpBinding
		{
			get
			{
				WSHttpBinding wshttpBinding = new WSHttpBinding();
				if (!string.IsNullOrWhiteSpace(this.Configuration.AllowCookies))
				{
					wshttpBinding.AllowCookies = Utils.GetBoolean(this.Configuration.AllowCookies, "AllowCookies");
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.BypassProxyOnLocal))
				{
					wshttpBinding.BypassProxyOnLocal = Utils.GetBoolean(this.Configuration.BypassProxyOnLocal, "BypassProxyOnLocal");
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.HostNameComparisonMode))
				{
					wshttpBinding.HostNameComparisonMode = Utils.GetEnumValue<HostNameComparisonMode>(this.Configuration.HostNameComparisonMode, "HostNameComparisonMode");
				}
				if (this.Configuration.MaxBufferPoolSize > 0)
				{
					wshttpBinding.MaxBufferPoolSize = (long)this.Configuration.MaxBufferPoolSize;
				}
				if (this.Configuration.MaxReceivedMessageSize > 0L)
				{
					wshttpBinding.MaxReceivedMessageSize = this.Configuration.MaxReceivedMessageSize;
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.MessageEncoding))
				{
					wshttpBinding.MessageEncoding = Utils.GetEnumValue<WSMessageEncoding>(this.Configuration.MessageEncoding, "MessageEncoding");
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.BindingName))
				{
					wshttpBinding.Name = this.Configuration.BindingName;
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.BindingNamespace))
				{
					wshttpBinding.Namespace = this.Configuration.BindingNamespace;
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.TextEncoding))
				{
					wshttpBinding.TextEncoding = this.TextEncoding;
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.TransactionFlow))
				{
					wshttpBinding.TransactionFlow = Utils.GetBoolean(this.Configuration.TransactionFlow, "TransactionFlow");
				}
				this.SetReaderQuotas(wshttpBinding.ReaderQuotas);
				this.SetReliableSession(wshttpBinding.ReliableSession);
				if (!string.IsNullOrWhiteSpace(this.Configuration.SecurityMode))
				{
					wshttpBinding.Security.Mode = Utils.GetEnumValue<SecurityMode>(this.Configuration.SecurityMode, "Mode");
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.TransportCredentialType))
				{
					wshttpBinding.Security.Transport.ClientCredentialType = Utils.GetEnumValue<HttpClientCredentialType>(this.Configuration.TransportCredentialType, "ClientCredentialType");
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.Realm))
				{
					wshttpBinding.Security.Transport.Realm = this.Configuration.Realm;
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.AlgorithmSuite))
				{
					wshttpBinding.Security.Message.AlgorithmSuite = Utils.GetProperty<SecurityAlgorithmSuite>(this.Configuration.AlgorithmSuite, "AlgorithmSuite");
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.MessageCredentialType))
				{
					wshttpBinding.Security.Message.ClientCredentialType = Utils.GetEnumValue<MessageCredentialType>(this.Configuration.MessageCredentialType, "ClientCredentialType");
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.EstablishSecurityContext))
				{
					wshttpBinding.Security.Message.EstablishSecurityContext = Utils.GetBoolean(this.Configuration.EstablishSecurityContext, "EstablishSecurityContext");
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.NegotiateServiceCredential))
				{
					wshttpBinding.Security.Message.NegotiateServiceCredential = Utils.GetBoolean(this.Configuration.NegotiateServiceCredential, "NegotiateServiceCredential");
				}
				wshttpBinding.UseDefaultWebProxy = this.SetWebProxy();
				return wshttpBinding;
			}
		}

		internal NetTcpBinding NetTcpBinding
		{
			get
			{
				NetTcpBinding netTcpBinding = new NetTcpBinding();
				if (!string.IsNullOrWhiteSpace(this.Configuration.HostNameComparisonMode))
				{
					netTcpBinding.HostNameComparisonMode = Utils.GetEnumValue<HostNameComparisonMode>(this.Configuration.HostNameComparisonMode, "HostNameComparisonMode");
				}
				if (this.Configuration.ListenBacklog > 0)
				{
					netTcpBinding.ListenBacklog = this.Configuration.ListenBacklog;
				}
				if (this.Configuration.MaxBufferPoolSize > 0)
				{
					netTcpBinding.MaxBufferPoolSize = (long)this.Configuration.MaxBufferPoolSize;
				}
				if (this.Configuration.MaxBufferSize > 0)
				{
					netTcpBinding.MaxBufferSize = this.Configuration.MaxBufferSize;
				}
				if (this.Configuration.MaxConnections > 0)
				{
					netTcpBinding.MaxConnections = this.Configuration.MaxConnections;
				}
				if (this.Configuration.MaxReceivedMessageSize > 0L)
				{
					netTcpBinding.MaxReceivedMessageSize = this.Configuration.MaxReceivedMessageSize;
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.BindingName))
				{
					netTcpBinding.Name = this.Configuration.BindingName;
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.BindingNamespace))
				{
					netTcpBinding.Namespace = this.Configuration.BindingNamespace;
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.TransactionFlow))
				{
					netTcpBinding.TransactionFlow = Utils.GetBoolean(this.Configuration.TransactionFlow, "TransactionFlow");
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.TransferMode))
				{
					netTcpBinding.TransferMode = Utils.GetEnumValue<TransferMode>(this.Configuration.TransferMode, "TransferMode");
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.TransactionProtocol))
				{
					netTcpBinding.TransactionProtocol = Utils.GetProperty<TransactionProtocol>(this.Configuration.TransactionProtocol, "TransactionProtocol");
				}
				this.SetReaderQuotas(netTcpBinding.ReaderQuotas);
				this.SetReliableSession(netTcpBinding.ReliableSession);
				if (!string.IsNullOrWhiteSpace(this.Configuration.SecurityMode))
				{
					netTcpBinding.Security.Mode = Utils.GetEnumValue<SecurityMode>(this.Configuration.SecurityMode, "Mode");
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.TransportCredentialType))
				{
					netTcpBinding.Security.Transport.ClientCredentialType = Utils.GetEnumValue<TcpClientCredentialType>(this.Configuration.TransportCredentialType, "ClientCredentialType");
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.ProtectionLevel))
				{
					netTcpBinding.Security.Transport.ProtectionLevel = Utils.GetEnumValue<ProtectionLevel>(this.Configuration.ProtectionLevel, "ProtectionLevel");
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.AlgorithmSuite))
				{
					netTcpBinding.Security.Message.AlgorithmSuite = Utils.GetProperty<SecurityAlgorithmSuite>(this.Configuration.AlgorithmSuite, "AlgorithmSuite");
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.MessageCredentialType))
				{
					netTcpBinding.Security.Message.ClientCredentialType = Utils.GetEnumValue<MessageCredentialType>(this.Configuration.MessageCredentialType, "ClientCredentialType");
				}
				return netTcpBinding;
			}
		}

		internal BasicHttpBinding BasicHttpBinding
		{
			get
			{
				BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
				if (!string.IsNullOrWhiteSpace(this.Configuration.AllowCookies))
				{
					basicHttpBinding.AllowCookies = Utils.GetBoolean(this.Configuration.AllowCookies, "AllowCookies");
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.BypassProxyOnLocal))
				{
					basicHttpBinding.BypassProxyOnLocal = Utils.GetBoolean(this.Configuration.BypassProxyOnLocal, "BypassProxyOnLocal");
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.HostNameComparisonMode))
				{
					basicHttpBinding.HostNameComparisonMode = Utils.GetEnumValue<HostNameComparisonMode>(this.Configuration.HostNameComparisonMode, "HostNameComparisonMode");
				}
				if (this.Configuration.MaxBufferPoolSize > 0)
				{
					basicHttpBinding.MaxBufferPoolSize = (long)this.Configuration.MaxBufferPoolSize;
				}
				if (this.Configuration.MaxBufferSize > 0)
				{
					basicHttpBinding.MaxBufferSize = this.Configuration.MaxBufferSize;
				}
				if (this.Configuration.MaxReceivedMessageSize > 0L)
				{
					basicHttpBinding.MaxReceivedMessageSize = this.Configuration.MaxReceivedMessageSize;
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.MessageEncoding))
				{
					basicHttpBinding.MessageEncoding = Utils.GetEnumValue<WSMessageEncoding>(this.Configuration.MessageEncoding, "MessageEncoding");
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.BindingName))
				{
					basicHttpBinding.Name = this.Configuration.BindingName;
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.BindingNamespace))
				{
					basicHttpBinding.Namespace = this.Configuration.BindingName;
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.TextEncoding))
				{
					basicHttpBinding.TextEncoding = this.TextEncoding;
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.TransferMode))
				{
					basicHttpBinding.TransferMode = Utils.GetEnumValue<TransferMode>(this.Configuration.TransferMode, "TransferMode");
				}
				this.SetReaderQuotas(basicHttpBinding.ReaderQuotas);
				if (!string.IsNullOrWhiteSpace(this.Configuration.SecurityMode))
				{
					basicHttpBinding.Security.Mode = Utils.GetEnumValue<BasicHttpSecurityMode>(this.Configuration.SecurityMode, "Mode");
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.TransportCredentialType))
				{
					basicHttpBinding.Security.Transport.ClientCredentialType = Utils.GetEnumValue<HttpClientCredentialType>(this.Configuration.TransportCredentialType, "ClientCredentialType");
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.Realm))
				{
					basicHttpBinding.Security.Transport.Realm = this.Configuration.Realm;
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.AlgorithmSuite))
				{
					basicHttpBinding.Security.Message.AlgorithmSuite = Utils.GetProperty<SecurityAlgorithmSuite>(this.Configuration.AlgorithmSuite, "AlgorithmSuite");
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.MessageCredentialType))
				{
					basicHttpBinding.Security.Message.ClientCredentialType = Utils.GetEnumValue<BasicHttpMessageCredentialType>(this.Configuration.MessageCredentialType, "ClientCredentialType");
				}
				basicHttpBinding.UseDefaultWebProxy = this.SetWebProxy();
				return basicHttpBinding;
			}
		}

		internal WSDualHttpBinding WSDualHttpBinding
		{
			get
			{
				WSDualHttpBinding wsdualHttpBinding = new WSDualHttpBinding();
				if (!string.IsNullOrWhiteSpace(this.Configuration.BypassProxyOnLocal))
				{
					wsdualHttpBinding.BypassProxyOnLocal = Utils.GetBoolean(this.Configuration.BypassProxyOnLocal, "BypassProxyOnLocal");
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.ClientBaseAddress))
				{
					wsdualHttpBinding.ClientBaseAddress = new Uri(this.Configuration.ClientBaseAddress);
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.HostNameComparisonMode))
				{
					wsdualHttpBinding.HostNameComparisonMode = Utils.GetEnumValue<HostNameComparisonMode>(this.Configuration.HostNameComparisonMode, "HostNameComparisonMode");
				}
				if (this.Configuration.MaxBufferPoolSize > 0)
				{
					wsdualHttpBinding.MaxBufferPoolSize = (long)this.Configuration.MaxBufferPoolSize;
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.MessageEncoding))
				{
					wsdualHttpBinding.MessageEncoding = Utils.GetEnumValue<WSMessageEncoding>(this.Configuration.MessageEncoding, "MessageEncoding");
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.BindingName))
				{
					wsdualHttpBinding.Name = this.Configuration.BindingName;
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.BindingNamespace))
				{
					wsdualHttpBinding.Namespace = this.Configuration.BindingNamespace;
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.TextEncoding))
				{
					wsdualHttpBinding.TextEncoding = this.TextEncoding;
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.TransactionFlow))
				{
					wsdualHttpBinding.TransactionFlow = Utils.GetBoolean(this.Configuration.TransactionFlow, "TransactionFlow");
				}
				this.SetReaderQuotas(wsdualHttpBinding.ReaderQuotas);
				this.SetReliableSession(wsdualHttpBinding.ReliableSession);
				if (!string.IsNullOrWhiteSpace(this.Configuration.SecurityMode))
				{
					wsdualHttpBinding.Security.Mode = Utils.GetEnumValue<WSDualHttpSecurityMode>(this.Configuration.SecurityMode, "Mode");
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.MessageCredentialType))
				{
					wsdualHttpBinding.Security.Message.ClientCredentialType = Utils.GetEnumValue<MessageCredentialType>(this.Configuration.MessageCredentialType, "ClientCredentialType");
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.AlgorithmSuite))
				{
					wsdualHttpBinding.Security.Message.AlgorithmSuite = Utils.GetProperty<SecurityAlgorithmSuite>(this.Configuration.AlgorithmSuite, "AlgorithmSuite");
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.NegotiateServiceCredential))
				{
					wsdualHttpBinding.Security.Message.NegotiateServiceCredential = Utils.GetBoolean(this.Configuration.NegotiateServiceCredential, "NegotiateServiceCredential");
				}
				wsdualHttpBinding.UseDefaultWebProxy = this.SetWebProxy();
				return wsdualHttpBinding;
			}
		}

		internal NetNamedPipeBinding NetNamedPipeBinding
		{
			get
			{
				NetNamedPipeBinding netNamedPipeBinding = new NetNamedPipeBinding();
				if (!string.IsNullOrWhiteSpace(this.Configuration.HostNameComparisonMode))
				{
					netNamedPipeBinding.HostNameComparisonMode = Utils.GetEnumValue<HostNameComparisonMode>(this.Configuration.HostNameComparisonMode, "HostNameComparisonMode");
				}
				if (this.Configuration.MaxBufferPoolSize > 0)
				{
					netNamedPipeBinding.MaxBufferPoolSize = (long)this.Configuration.MaxBufferPoolSize;
				}
				if (this.Configuration.MaxBufferSize > 0)
				{
					netNamedPipeBinding.MaxBufferSize = this.Configuration.MaxBufferSize;
				}
				if (this.Configuration.MaxConnections > 0)
				{
					netNamedPipeBinding.MaxConnections = this.Configuration.MaxConnections;
				}
				if (this.Configuration.MaxReceivedMessageSize > 0L)
				{
					netNamedPipeBinding.MaxReceivedMessageSize = this.Configuration.MaxReceivedMessageSize;
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.BindingName))
				{
					netNamedPipeBinding.Name = this.Configuration.BindingName;
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.BindingNamespace))
				{
					netNamedPipeBinding.Namespace = this.Configuration.BindingNamespace;
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.TransactionFlow))
				{
					netNamedPipeBinding.TransactionFlow = Utils.GetBoolean(this.Configuration.TransactionFlow, "TransactionFlow");
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.TransferMode))
				{
					netNamedPipeBinding.TransferMode = Utils.GetEnumValue<TransferMode>(this.Configuration.TransferMode, "TransferMode");
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.TransactionProtocol))
				{
					netNamedPipeBinding.TransactionProtocol = Utils.GetProperty<TransactionProtocol>(this.Configuration.TransactionProtocol, "TransactionProtocol");
				}
				this.SetReaderQuotas(netNamedPipeBinding.ReaderQuotas);
				if (!string.IsNullOrWhiteSpace(this.Configuration.SecurityMode))
				{
					netNamedPipeBinding.Security.Mode = Utils.GetEnumValue<NetNamedPipeSecurityMode>(this.Configuration.SecurityMode, "Mode");
				}
				if (!string.IsNullOrWhiteSpace(this.Configuration.ProtectionLevel))
				{
					netNamedPipeBinding.Security.Transport.ProtectionLevel = Utils.GetEnumValue<ProtectionLevel>(this.Configuration.ProtectionLevel, "ProtectionLevel");
				}
				return netNamedPipeBinding;
			}
		}

		internal EndpointAddress EndpointAddress
		{
			get
			{
				string text = this.Configuration.Uri;
				if (this.Configuration.FwLinkEnabled)
				{
					HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(this.Configuration.FwLinkUri);
					using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
					{
						if (httpWebResponse.StatusCode == HttpStatusCode.OK)
						{
							text = httpWebResponse.ResponseUri.AbsoluteUri;
						}
					}
				}
				if (this.Configuration.UseCertificateAuthentication)
				{
					return new EndpointAddress(new Uri(text), EndpointIdentity.CreateDnsIdentity(this.Configuration.FindValue), new AddressHeader[0]);
				}
				return new EndpointAddress(text);
			}
		}

		internal Encoding TextEncoding
		{
			get
			{
				string item = this.Configuration.TextEncoding.ToLower();
				List<string> list = new List<string>
				{
					"utf-8",
					"Utf8TextEncoding".ToLower()
				};
				if (list.Contains(item))
				{
					return Encoding.UTF8;
				}
				list = new List<string>
				{
					"utf-16",
					"Utf16TextEncoding".ToLower()
				};
				if (list.Contains(item))
				{
					return Encoding.Unicode;
				}
				list = new List<string>
				{
					"unicodeFFFE",
					"utf-16BE".ToLower(),
					"UnicodeFffeTextEncoding".ToLower()
				};
				if (list.Contains(item))
				{
					return Encoding.BigEndianUnicode;
				}
				throw new ArgumentException(string.Format("Work definition error - attribute 'TextEncoding' has invalid value '{0}'.", this.Configuration.TextEncoding));
			}
		}

		private bool Aborted { get; set; }

		public MethodInfo GetValidatorMethod(Type paramType)
		{
			if (string.IsNullOrWhiteSpace(this.Configuration.ProxyValidatorClassName))
			{
				throw new InvalidOperationException("Work definition error - ValidatorClassName is missing in Proxy node");
			}
			if (string.IsNullOrWhiteSpace(this.Configuration.ProxyValidatorMethodName))
			{
				throw new InvalidOperationException("Work definition error - ValidatorMethodName is missing in Proxy node");
			}
			Type type = this.Assembly.GetType(this.Configuration.ProxyValidatorClassName, true, true);
			MethodInfo method = type.GetMethod(this.Configuration.ProxyValidatorMethodName, new Type[]
			{
				paramType,
				paramType,
				typeof(string).MakeByRefType()
			});
			if (method == null)
			{
				throw new InvalidOperationException(string.Format("Work definition error - ValidatorMethodName={0}.{1} which accepts parameters of type={2} doesnot exist in Assembly={3}", new object[]
				{
					this.Configuration.ProxyValidatorClassName,
					this.Configuration.ProxyValidatorMethodName,
					paramType,
					this.Configuration.ProxyAssembly
				}));
			}
			return method;
		}

		public MethodInfo GetDiagnosticsInfoMethod()
		{
			if (string.IsNullOrWhiteSpace(this.Configuration.ProxyValidatorClassName))
			{
				throw new InvalidOperationException("Work definition error - ValidatorClassName is missing in Proxy node");
			}
			if (string.IsNullOrWhiteSpace(this.Configuration.ProxyDiagnosticsInfoMethodName))
			{
				throw new InvalidOperationException("Work definition error - DiagnosticsInfoMethodName is missing in Proxy node");
			}
			Type type = this.Assembly.GetType(this.Configuration.ProxyValidatorClassName, true, true);
			MethodInfo method = type.GetMethod(this.Configuration.ProxyDiagnosticsInfoMethodName);
			if (method == null)
			{
				throw new InvalidOperationException(string.Format("Work definition error - DiagnosticsInfoMethodName={0}.{1} doesnot exist in Assembly={3}", this.Configuration.ProxyValidatorClassName, this.Configuration.ProxyDiagnosticsInfoMethodName, this.Configuration.ProxyAssembly));
			}
			return method;
		}

		internal void Abort()
		{
			if (this.Proxy != null)
			{
				MethodInfo method = this.Proxy.GetType().GetMethod("Abort");
				if (method != null)
				{
					method.Invoke(this.Proxy, null);
					this.Aborted = true;
				}
			}
		}

		internal void Close()
		{
			if (this.Proxy != null && !this.Aborted)
			{
				MethodInfo method = this.Proxy.GetType().GetMethod("Close");
				if (method != null)
				{
					try
					{
						method.Invoke(this.Proxy, null);
					}
					catch (CommunicationException)
					{
						this.Abort();
						throw;
					}
					catch (TimeoutException)
					{
						this.Abort();
						throw;
					}
				}
			}
		}

		private static X509Certificate2 FindFirstCertificate(StoreLocation storeLocation, StoreName storeName, X509FindType findType, string findValue)
		{
			X509Store x509Store = new X509Store(storeName, storeLocation);
			X509Certificate2 result;
			try
			{
				x509Store.Open(OpenFlags.OpenExistingOnly);
				X509Certificate2Collection x509Certificate2Collection = x509Store.Certificates.Find(findType, findValue, true);
				if (x509Certificate2Collection.Count == 0)
				{
					throw new Exception(string.Format("Certificate not found. StoreName {0}; StoreLocation {1}; FindType {2}; FindValue {3}", new object[]
					{
						storeName,
						storeLocation,
						findType,
						findValue
					}));
				}
				result = x509Certificate2Collection[0];
			}
			finally
			{
				x509Store.Close();
			}
			return result;
		}

		private void CheckConstructor()
		{
			string proxyClassName = this.Configuration.ProxyClassName;
			Type type = this.Assembly.GetType(proxyClassName, true, true);
			ConstructorInfo constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.HasThis, new Type[]
			{
				typeof(Binding),
				typeof(EndpointAddress)
			}, null);
			if (constructor == null)
			{
				throw new Exception(string.Format("The constructor of {0} that takes Binding and EndpointAddress parameters is not available.", proxyClassName));
			}
		}

		private object CreateProxy()
		{
			this.CheckConstructor();
			object[] args = new object[]
			{
				this.Binding,
				this.EndpointAddress
			};
			string proxyClassName = this.Configuration.ProxyClassName;
			object obj = this.Assembly.CreateInstance(proxyClassName, false, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public, null, args, null, null);
			if (obj == null)
			{
				throw new Exception(string.Format("Failure creating an instance of {0}.", proxyClassName));
			}
			PropertyInfo property = obj.GetType().GetProperty("ClientCredentials");
			if (property == null)
			{
				throw new Exception(string.Format("The type {0} does not contain the property 'ClientCredentials'.", proxyClassName));
			}
			ClientCredentials clientCredentials = (ClientCredentials)property.GetValue(obj, null);
			if (clientCredentials == null)
			{
				throw new Exception(string.Format("The value of property 'ClientCredentials' of type {0} is null.", proxyClassName));
			}
			if (this.Configuration.UseCertificateAuthentication)
			{
				X509CertificateInitiatorClientCredential clientCertificate = clientCredentials.ClientCertificate;
				clientCertificate.Certificate = WebServiceClient.FindFirstCertificate(this.configuration.StoreLocation, this.configuration.StoreName, this.Configuration.X509FindType, this.Configuration.FindValue);
				if (!string.IsNullOrWhiteSpace(this.Configuration.ServiceCertificateValidationMode))
				{
					X509CertificateRecipientClientCredential serviceCertificate = clientCredentials.ServiceCertificate;
					serviceCertificate.Authentication.CertificateValidationMode = Utils.GetEnumValue<X509CertificateValidationMode>(this.Configuration.ServiceCertificateValidationMode, "ServiceCertificateValidationMode");
				}
			}
			else if (this.Configuration.UseUserNameAuthentication)
			{
				UserNamePasswordClientCredential userName = clientCredentials.UserName;
				userName.UserName = this.Configuration.Username;
				userName.Password = this.Configuration.Password;
			}
			return obj;
		}

		private object CreateNonGeneratedProxy()
		{
			string proxyClassName = this.Configuration.ProxyClassName;
			object[] array = null;
			if (this.Configuration.ProxyInstanceMethod.Parameters.Count != 0)
			{
				List<object> list = new List<object>();
				foreach (Parameter parameter in this.Configuration.ProxyInstanceMethod.Parameters)
				{
					object item = null;
					if (!parameter.IsNull)
					{
						if (string.IsNullOrWhiteSpace(parameter.Type))
						{
							item = parameter.Value.InnerText;
						}
						else
						{
							Type type;
							try
							{
								type = Type.GetType(parameter.Type, true, true);
							}
							catch
							{
								type = this.Assembly.GetType(parameter.Type, true, true);
							}
							item = Utils.DeserializeFromXml(parameter.Value.InnerXml, type);
						}
					}
					list.Add(item);
				}
				array = list.ToArray();
			}
			object obj;
			if (!this.Configuration.ProxyInstanceMethodIsStatic)
			{
				obj = this.Assembly.CreateInstance(proxyClassName, false, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public, null, array, null, null);
			}
			else
			{
				string name = this.Configuration.ProxyInstanceMethod.Name;
				Type type2 = this.Assembly.GetType(proxyClassName, true, true);
				MethodInfo method = type2.GetMethod(name, BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.Public);
				if (method == null)
				{
					throw new ArgumentException(string.Format("ProxyConstructor '{0}' not found in proxy class type '{1}'.", name, proxyClassName));
				}
				obj = method.Invoke(null, array);
			}
			if (obj == null)
			{
				throw new Exception(string.Format("Failure creating an instance of {0}.", proxyClassName));
			}
			return obj;
		}

		private void SetReaderQuotas(XmlDictionaryReaderQuotas readerQuotas)
		{
			if (this.Configuration.MaxDepth > 0)
			{
				readerQuotas.MaxDepth = this.Configuration.MaxDepth;
			}
			if (this.Configuration.MaxNameTableCharCount > 0)
			{
				readerQuotas.MaxNameTableCharCount = this.Configuration.MaxNameTableCharCount;
			}
			if (this.Configuration.MaxArrayLength > 0)
			{
				readerQuotas.MaxArrayLength = this.Configuration.MaxArrayLength;
			}
			if (this.Configuration.MaxBytesPerRead > 0)
			{
				readerQuotas.MaxBytesPerRead = this.Configuration.MaxBytesPerRead;
			}
			if (this.Configuration.MaxStringContentLength > 0)
			{
				readerQuotas.MaxStringContentLength = this.Configuration.MaxStringContentLength;
			}
		}

		private void SetReliableSession(OptionalReliableSession reliableSession)
		{
			if (!string.IsNullOrWhiteSpace(this.Configuration.Ordered))
			{
				reliableSession.Ordered = Utils.GetBoolean(this.Configuration.Ordered, "Ordered");
			}
			if (this.Configuration.InactivityTimeout != TimeSpan.MaxValue)
			{
				reliableSession.InactivityTimeout = this.Configuration.InactivityTimeout;
			}
			if (this.Configuration.ReliableSessionEnabled != null)
			{
				reliableSession.Enabled = (this.Configuration.ReliableSessionEnabled ?? false);
			}
		}

		private void SetReliableSession(ReliableSession reliableSession)
		{
			if (!string.IsNullOrWhiteSpace(this.Configuration.Ordered))
			{
				reliableSession.Ordered = Utils.GetBoolean(this.Configuration.Ordered, "Ordered");
			}
			if (this.Configuration.InactivityTimeout != TimeSpan.MaxValue)
			{
				reliableSession.InactivityTimeout = this.Configuration.InactivityTimeout;
			}
		}

		private bool SetWebProxy()
		{
			if (this.Configuration.WebProxyEnabled && !this.Configuration.UseDefaultWebProxy)
			{
				Uri uri = new Uri(this.Configuration.WebProxyServerUri);
				WebProxy webProxy = new WebProxy(uri.Host, this.Configuration.WebProxyPort);
				if (this.Configuration.WebProxyCredentialsRequired)
				{
					if (string.IsNullOrEmpty(this.Configuration.WebProxyDomain))
					{
						webProxy.Credentials = new NetworkCredential(this.Configuration.WebProxyUsername, this.Configuration.WebProxyPassword);
					}
					else
					{
						webProxy.Credentials = new NetworkCredential(this.Configuration.WebProxyUsername, this.Configuration.WebProxyPassword, this.Configuration.WebProxyDomain);
					}
				}
				WebRequest.DefaultWebProxy = webProxy;
			}
			return this.Configuration.UseDefaultWebProxy;
		}

		private object proxy;

		private WebServiceConfiguration configuration;

		private Assembly assembly;
	}
}
