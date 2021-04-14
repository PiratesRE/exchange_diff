using System;
using System.Net.Security;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Xml;

namespace Microsoft.Exchange.DxStore.Common
{
	public class EndpointBuilder
	{
		public static string ConstructEndpointAddress(string interfaceName, string componentName, string self, string target, string targetAddress, string padding, bool isZeroboxMode, int portNumber, string protocolName)
		{
			if (string.IsNullOrEmpty(target))
			{
				target = self;
			}
			if (string.IsNullOrEmpty(targetAddress))
			{
				targetAddress = target;
			}
			if (isZeroboxMode || Utils.IsEqual(self, target, StringComparison.OrdinalIgnoreCase))
			{
				targetAddress = "localhost";
			}
			string text = string.Empty;
			if (!Utils.IsEqual(protocolName, "net.pipe", StringComparison.OrdinalIgnoreCase))
			{
				text = string.Format("{0}://{1}:{2}/DxStore/{3}/{4}", new object[]
				{
					protocolName,
					targetAddress,
					portNumber,
					interfaceName,
					componentName
				});
			}
			else
			{
				text = string.Format("{0}://{1}/DxStore/{2}/{3}", new object[]
				{
					protocolName,
					targetAddress,
					interfaceName,
					componentName
				});
			}
			if (!string.IsNullOrEmpty(padding))
			{
				text = text + "/" + padding;
			}
			if (isZeroboxMode)
			{
				text = text + "/" + target;
			}
			return text;
		}

		public static Binding CreateBindingAndInitialize(string address, string bindingName, WcfTimeout timeout, bool isServerBinding)
		{
			string a = address.Substring(0, address.IndexOf(':'));
			Binding result;
			if (Utils.IsEqual(a, "http", StringComparison.OrdinalIgnoreCase))
			{
				result = EndpointBuilder.CreateHttpBindingAndInitialize(bindingName, timeout, isServerBinding);
			}
			else if (Utils.IsEqual(a, "net.tcp", StringComparison.OrdinalIgnoreCase))
			{
				result = EndpointBuilder.CreateNettcpBindingAndInitialize(bindingName, timeout, isServerBinding);
			}
			else
			{
				if (!Utils.IsEqual(a, "net.pipe", StringComparison.OrdinalIgnoreCase))
				{
					throw new DxStoreBindingNotSupportedException(address);
				}
				result = EndpointBuilder.CreateNamedPipeBindingAndInitialize(bindingName, timeout, isServerBinding);
			}
			return result;
		}

		public static int MebiBytes(int mb)
		{
			return mb * 1024 * 1024;
		}

		public static void IfHasValue<T>(T? v, Action<T> action) where T : struct
		{
			if (v != null)
			{
				action(v.Value);
			}
		}

		public static BasicHttpBinding CreateHttpBindingAndInitialize(string bindingName, WcfTimeout timeout, bool isServerBinding)
		{
			BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
			basicHttpBinding.Name = bindingName;
			basicHttpBinding.MaxBufferPoolSize = (long)EndpointBuilder.MebiBytes(64);
			basicHttpBinding.MaxBufferSize = EndpointBuilder.MebiBytes(64);
			basicHttpBinding.MaxReceivedMessageSize = (long)EndpointBuilder.MebiBytes(64);
			if (isServerBinding)
			{
				EndpointBuilder.SetReaderQuotas(basicHttpBinding.ReaderQuotas);
			}
			else
			{
				EndpointBuilder.SetTimeout(basicHttpBinding, timeout);
			}
			return basicHttpBinding;
		}

		public static NetNamedPipeBinding CreateNamedPipeBindingAndInitialize(string bindingName, WcfTimeout timeout, bool isServerBinding)
		{
			NetNamedPipeBinding netNamedPipeBinding = new NetNamedPipeBinding();
			netNamedPipeBinding.Name = bindingName;
			netNamedPipeBinding.MaxBufferPoolSize = (long)EndpointBuilder.MebiBytes(64);
			netNamedPipeBinding.MaxBufferSize = EndpointBuilder.MebiBytes(64);
			netNamedPipeBinding.MaxReceivedMessageSize = (long)EndpointBuilder.MebiBytes(64);
			netNamedPipeBinding.MaxConnections = 200;
			if (isServerBinding)
			{
				EndpointBuilder.SetReaderQuotas(netNamedPipeBinding.ReaderQuotas);
			}
			else
			{
				EndpointBuilder.SetTimeout(netNamedPipeBinding, timeout);
			}
			return netNamedPipeBinding;
		}

		public static NetTcpBinding CreateNettcpBindingAndInitialize(string bindingName, WcfTimeout timeout, bool isServerBinding)
		{
			NetTcpBinding netTcpBinding = new NetTcpBinding();
			netTcpBinding.Name = bindingName;
			netTcpBinding.MaxBufferPoolSize = (long)EndpointBuilder.MebiBytes(64);
			netTcpBinding.MaxBufferSize = EndpointBuilder.MebiBytes(64);
			netTcpBinding.MaxReceivedMessageSize = (long)EndpointBuilder.MebiBytes(64);
			netTcpBinding.MaxConnections = 200;
			if (isServerBinding)
			{
				netTcpBinding.PortSharingEnabled = true;
				EndpointBuilder.SetReaderQuotas(netTcpBinding.ReaderQuotas);
			}
			else
			{
				EndpointBuilder.SetTimeout(netTcpBinding, timeout);
				EndpointBuilder.SetBindingSecurity(netTcpBinding.Security);
			}
			return netTcpBinding;
		}

		public static void SetTimeout(Binding binding, WcfTimeout timeout)
		{
			EndpointBuilder.IfHasValue<TimeSpan>(timeout.Open, delegate(TimeSpan t)
			{
				binding.OpenTimeout = t;
			});
			EndpointBuilder.IfHasValue<TimeSpan>(timeout.Close, delegate(TimeSpan t)
			{
				binding.CloseTimeout = t;
			});
			EndpointBuilder.IfHasValue<TimeSpan>(timeout.Send, delegate(TimeSpan t)
			{
				binding.SendTimeout = t;
			});
			EndpointBuilder.IfHasValue<TimeSpan>(timeout.Receive, delegate(TimeSpan t)
			{
				binding.ReceiveTimeout = t;
			});
		}

		public static void SetReaderQuotas(XmlDictionaryReaderQuotas readerQuotas)
		{
			readerQuotas.MaxDepth = 256;
			readerQuotas.MaxArrayLength = int.MaxValue;
			readerQuotas.MaxBytesPerRead = int.MaxValue;
			readerQuotas.MaxNameTableCharCount = int.MaxValue;
			readerQuotas.MaxStringContentLength = int.MaxValue;
		}

		public static void SetBindingSecurity(NetTcpSecurity security)
		{
			security.Mode = SecurityMode.Transport;
			security.Message.ClientCredentialType = MessageCredentialType.Windows;
			security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
			security.Transport.ProtectionLevel = ProtectionLevel.EncryptAndSign;
		}

		public static ServiceEndpoint ConstructAndInitializeEndpoint(Type interfaceType, string address, string bindingName, WcfTimeout timeout, bool isServerBinding)
		{
			Binding binding = EndpointBuilder.CreateBindingAndInitialize(address, bindingName, timeout, isServerBinding);
			return new ServiceEndpoint(ContractDescription.GetContract(interfaceType), binding, new EndpointAddress(address));
		}

		public static string ConstructUniqueBindingName(string target, string componentName, string protocolName, string interfaceName, string padding, bool isZeroboxMode)
		{
			string text = string.Format("{0}_{1}_{2}", protocolName, interfaceName, componentName);
			if (!string.IsNullOrEmpty(padding))
			{
				text = text + "_" + padding;
			}
			if (isZeroboxMode)
			{
				text = text + "_" + target;
			}
			return text;
		}

		public static ServiceEndpoint GetStoreAccessEndpoint(InstanceGroupConfig cfg, string target, bool isUseDefaultGroup, bool isServerBinding, WcfTimeout timeout = null)
		{
			string storeAccessEndPointAddress = cfg.GetStoreAccessEndPointAddress(target, isUseDefaultGroup);
			string bindingName = cfg.ConstructUniqueAccessBindingName(target, isUseDefaultGroup);
			timeout = (timeout ?? cfg.Settings.StoreAccessWcfTimeout);
			return EndpointBuilder.ConstructAndInitializeEndpoint(typeof(IDxStoreAccess), storeAccessEndPointAddress, bindingName, timeout, isServerBinding);
		}

		public static ServiceEndpoint GetStoreManagerEndpoint(InstanceManagerConfig cfg, string target, bool isServerBinding, WcfTimeout timeout = null)
		{
			string endPointAddress = cfg.GetEndPointAddress(target);
			string bindingName = cfg.ConstructUniqueBindingName(target);
			timeout = (timeout ?? cfg.DefaultTimeout);
			return EndpointBuilder.ConstructAndInitializeEndpoint(typeof(IDxStoreManager), endPointAddress, bindingName, timeout, isServerBinding);
		}

		public static ServiceEndpoint GetStoreInstanceEndpoint(InstanceGroupConfig cfg, string target, bool isUseDefaultGroup, bool isServerBinding, WcfTimeout timeout = null)
		{
			string storeInstanceEndPointAddress = cfg.GetStoreInstanceEndPointAddress(target, isUseDefaultGroup);
			string bindingName = cfg.ConstructUniqueInstanceBindingName(target, isUseDefaultGroup);
			timeout = (timeout ?? cfg.Settings.StoreInstanceWcfTimeout);
			return EndpointBuilder.ConstructAndInitializeEndpoint(typeof(IDxStoreInstance), storeInstanceEndPointAddress, bindingName, timeout, isServerBinding);
		}

		public static string GetNetworkAddress(string self, string targetShortName, string targetNetworkAddress, IServerNameResolver nameResolver, bool isDefaultToShortName = false)
		{
			if (string.IsNullOrEmpty(targetShortName))
			{
				targetShortName = self;
			}
			if (!string.IsNullOrEmpty(targetNetworkAddress))
			{
				return targetNetworkAddress;
			}
			if (nameResolver != null)
			{
				targetNetworkAddress = nameResolver.ResolveNameBestEffort(targetShortName);
			}
			if (string.IsNullOrEmpty(targetNetworkAddress) && isDefaultToShortName)
			{
				targetNetworkAddress = targetShortName;
			}
			return targetNetworkAddress;
		}
	}
}
