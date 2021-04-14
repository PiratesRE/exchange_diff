using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.ServiceModel.Security.Tokens;
using System.Xml;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.Net.XropService
{
	internal class Binding : CustomBinding
	{
		protected internal Binding()
		{
		}

		protected internal Binding(BindingElementCollection sourceBindingElementCollection) : base(sourceBindingElementCollection)
		{
			ExTraceGlobals.XropServiceClientTracer.TraceDebug((long)this.GetHashCode(), "Xrop Binding initialized");
		}

		internal static BindingElementCollection GetClientBindingElements()
		{
			return Binding.ClientBinding.CreateBindingElements("XropLiveIdFederatedBinding");
		}

		internal static BindingElementCollection GetListenerBindingElements()
		{
			return Binding.ListenerBinding.CreateBindingElements("XropLiveIdFederatedBinding");
		}

		internal void SetMaxTimeout(TimeSpan timeSpan)
		{
			base.CloseTimeout = timeSpan;
			base.OpenTimeout = timeSpan;
			base.ReceiveTimeout = timeSpan;
			base.SendTimeout = timeSpan;
		}

		internal void SetMaxTimeout()
		{
			this.SetMaxTimeout(TimeSpan.MaxValue);
		}

		protected static WSHttpBindingBase InitializeDefaultTemplateBinding(string bindingTypeConfigurationKey)
		{
			WS2007HttpBinding ws2007HttpBinding = new WS2007HttpBinding(SecurityMode.TransportWithMessageCredential, false);
			ws2007HttpBinding.Security.Message.EstablishSecurityContext = true;
			ws2007HttpBinding.Security.Message.ClientCredentialType = MessageCredentialType.IssuedToken;
			ws2007HttpBinding.Security.Message.NegotiateServiceCredential = false;
			ws2007HttpBinding.AllowCookies = false;
			ws2007HttpBinding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
			ws2007HttpBinding.Security.Transport.Realm = string.Empty;
			if (bindingTypeConfigurationKey == "XropLiveIdFederatedBinding")
			{
				ws2007HttpBinding.Security.Message.AlgorithmSuite = SecurityAlgorithmSuite.TripleDes;
			}
			return ws2007HttpBinding;
		}

		protected static BindingElementCollection InitializeBindingElements(BindingElementCollection sourceBindingElementCollection)
		{
			HttpsTransportBindingElement httpsTransportBindingElement = sourceBindingElementCollection.Find<HttpsTransportBindingElement>();
			if (httpsTransportBindingElement == null)
			{
				ExTraceGlobals.XropServiceClientTracer.TraceError(0L, "Invalid Xrop Service Binding, missing required HTTPS binding element");
				throw new InvalidOperationException();
			}
			TransportSecurityBindingElement transportSecurityBindingElement = sourceBindingElementCollection.Find<TransportSecurityBindingElement>();
			if (transportSecurityBindingElement == null)
			{
				ExTraceGlobals.XropServiceClientTracer.TraceError(0L, "Xrop Service Transport Security Binding missing");
				throw new InvalidOperationException();
			}
			Binding.ConfigureDefaultHttpTransportSettings(httpsTransportBindingElement);
			Binding.ConfigureValidateSecuritySettings(transportSecurityBindingElement);
			MessageEncodingBindingElement messageEncodingBindingElement;
			if (!Binding.UseTextMessageEncoder.Value)
			{
				messageEncodingBindingElement = sourceBindingElementCollection.Find<BinaryMessageEncodingBindingElement>();
				if (messageEncodingBindingElement == null)
				{
					messageEncodingBindingElement = new BinaryMessageEncodingBindingElement();
					Binding.ConfigureDefaultBinaryXmlSettings(messageEncodingBindingElement as BinaryMessageEncodingBindingElement);
				}
			}
			else
			{
				messageEncodingBindingElement = sourceBindingElementCollection.Find<TextMessageEncodingBindingElement>();
				if (messageEncodingBindingElement == null)
				{
					messageEncodingBindingElement = new TextMessageEncodingBindingElement();
				}
			}
			return new BindingElementCollection
			{
				transportSecurityBindingElement,
				messageEncodingBindingElement,
				httpsTransportBindingElement
			};
		}

		private static void ConfigureDefaultHttpTransportSettings(HttpsTransportBindingElement transportBindingElement)
		{
			transportBindingElement.RequireClientCertificate = false;
			transportBindingElement.AuthenticationScheme = AuthenticationSchemes.Anonymous;
			transportBindingElement.ProxyAuthenticationScheme = AuthenticationSchemes.Anonymous;
			transportBindingElement.UnsafeConnectionNtlmAuthentication = false;
			transportBindingElement.TransferMode = TransferMode.Buffered;
			transportBindingElement.KeepAliveEnabled = true;
			transportBindingElement.ManualAddressing = false;
		}

		private static void ConfigureValidateSecuritySettings(TransportSecurityBindingElement transporSecuritytBindingElement)
		{
			SecureConversationSecurityTokenParameters secureConversationSecurityTokenParameters = null;
			foreach (SecurityTokenParameters securityTokenParameters in transporSecuritytBindingElement.EndpointSupportingTokenParameters.Endorsing)
			{
				if (securityTokenParameters is SecureConversationSecurityTokenParameters)
				{
					secureConversationSecurityTokenParameters = (securityTokenParameters as SecureConversationSecurityTokenParameters);
					break;
				}
			}
			if (secureConversationSecurityTokenParameters == null)
			{
				ExTraceGlobals.XropServiceClientTracer.TraceError(0L, "Xrop Service Transport Security Binding missing Secure Conversation Parameters");
				throw new InvalidOperationException();
			}
			TransportSecurityBindingElement transportSecurityBindingElement = secureConversationSecurityTokenParameters.BootstrapSecurityBindingElement as TransportSecurityBindingElement;
			if (transportSecurityBindingElement == null)
			{
				ExTraceGlobals.XropServiceClientTracer.TraceError(0L, "Xrop Service Secure Conversation Token Parameters missing bootstrap security binding element");
				throw new InvalidOperationException();
			}
			secureConversationSecurityTokenParameters.RequireCancellation = true;
			secureConversationSecurityTokenParameters.RequireDerivedKeys = true;
			bool flag = false;
			foreach (SecurityTokenParameters securityTokenParameters2 in transporSecuritytBindingElement.EndpointSupportingTokenParameters.Signed)
			{
				if (securityTokenParameters2 is UserNameSecurityTokenParameters)
				{
					flag = true;
					break;
				}
			}
			if (!flag && !Binding.DoNotSendUserNameSecurityToken.Value)
			{
				UserNameSecurityTokenParameters item = new UserNameSecurityTokenParameters();
				transportSecurityBindingElement.EndpointSupportingTokenParameters.Signed.Add(item);
			}
		}

		private static void ConfigureDefaultBinaryXmlSettings(BinaryMessageEncodingBindingElement messageEncodingBindingElement)
		{
			messageEncodingBindingElement.MaxReadPoolSize = 64;
			messageEncodingBindingElement.MaxWritePoolSize = 64;
			messageEncodingBindingElement.ReaderQuotas.MaxDepth = 64;
			messageEncodingBindingElement.ReaderQuotas.MaxStringContentLength = 1048576;
			messageEncodingBindingElement.ReaderQuotas.MaxArrayLength = 1048576;
			messageEncodingBindingElement.ReaderQuotas.MaxBytesPerRead = 1048576;
			messageEncodingBindingElement.ReaderQuotas.MaxNameTableCharCount = 16384;
		}

		internal const string DiagInfo = "X-DiagInfo";

		private const string LiveIdFederatedBinding = "XropLiveIdFederatedBinding";

		internal static readonly BoolAppSettingsEntry UseTextMessageEncoder = new BoolAppSettingsEntry("UseTextMessageEncoder", false, null);

		internal static readonly BoolAppSettingsEntry DoNotSendUserNameSecurityToken = new BoolAppSettingsEntry("DoNotSendUserNameSecurityToken", false, null);

		internal static readonly BoolAppSettingsEntry DoNotCacheFactories = new BoolAppSettingsEntry("DoNotCacheFactories", false, null);

		internal static readonly BoolAppSettingsEntry AddSessionIdToQueryString = new BoolAppSettingsEntry("AddSessionIdToQueryString", true, null);

		internal static readonly BoolAppSettingsEntry IncludeErrorDetailsInTrace = new BoolAppSettingsEntry("IncludeErrorDetailsInTrace", true, null);

		internal static readonly BoolAppSettingsEntry IncludeDetailsInServiceFaults = new BoolAppSettingsEntry("IncludeDetailsInServiceFaults", true, null);

		internal static readonly BoolAppSettingsEntry IncludeStackInServiceFaults = new BoolAppSettingsEntry("IncludeStackInServiceFaults", true, null);

		internal static readonly BoolAppSettingsEntry Use200ForSoapFaults = new BoolAppSettingsEntry("Use200ForSoapFaults", true, null);

		internal static readonly BoolAppSettingsEntry UseHttpListenerExtendedErrorLogging = new BoolAppSettingsEntry("UseHttpListenerExtendedErrorLogging", true, null);

		internal static readonly BoolAppSettingsEntry UseWCFTransportExceptionHandler = new BoolAppSettingsEntry("UseWCFTransportExceptionHandler", true, null);

		private class ClientBinding : Binding
		{
			internal static BindingElementCollection CreateBindingElements(string bindingTypeConfigurationKey)
			{
				try
				{
					CustomBinding customBinding = new CustomBinding(bindingTypeConfigurationKey);
					return customBinding.CreateBindingElements();
				}
				catch (ConfigurationErrorsException)
				{
					ExTraceGlobals.XropServiceClientTracer.TraceDebug((long)bindingTypeConfigurationKey.GetHashCode(), "No external Xrop WCF configuration found");
				}
				catch (KeyNotFoundException)
				{
					ExTraceGlobals.XropServiceClientTracer.TraceDebug((long)bindingTypeConfigurationKey.GetHashCode(), "No external Xrop WCF configuration found");
				}
				WSHttpBindingBase wshttpBindingBase = Binding.InitializeDefaultTemplateBinding(bindingTypeConfigurationKey);
				wshttpBindingBase.ProxyAddress = null;
				wshttpBindingBase.UseDefaultWebProxy = true;
				wshttpBindingBase.BypassProxyOnLocal = false;
				Binding.ClientBinding.SetThrottling(wshttpBindingBase);
				return Binding.InitializeBindingElements(wshttpBindingBase.CreateBindingElements());
			}

			private static void SetThrottling(WSHttpBindingBase binding)
			{
				binding.MaxReceivedMessageSize = 1048576L;
				binding.ReceiveTimeout = TimeSpan.FromMinutes(10.0);
				binding.SendTimeout = TimeSpan.FromMinutes(10.0);
				Binding.ClientBinding.SetThrottling(binding.ReaderQuotas);
			}

			private static void SetThrottling(XmlDictionaryReaderQuotas readerQuotas)
			{
				readerQuotas.MaxDepth = 64;
				readerQuotas.MaxStringContentLength = 1048576;
				readerQuotas.MaxArrayLength = 1048576;
				readerQuotas.MaxBytesPerRead = 1048576;
				readerQuotas.MaxNameTableCharCount = 16384;
			}

			private static void ConfigureSystemDotNetBehaviors()
			{
			}
		}

		private class ListenerBinding : Binding
		{
			internal static BindingElementCollection CreateBindingElements(string bindingTypeConfigurationKey)
			{
				try
				{
					CustomBinding customBinding = new CustomBinding(bindingTypeConfigurationKey);
					return customBinding.CreateBindingElements();
				}
				catch (ConfigurationErrorsException)
				{
					ExTraceGlobals.XropServiceClientTracer.TraceDebug((long)bindingTypeConfigurationKey.GetHashCode(), "No external Xrop WCF configuration found");
				}
				catch (KeyNotFoundException)
				{
					ExTraceGlobals.XropServiceClientTracer.TraceDebug((long)bindingTypeConfigurationKey.GetHashCode(), "No external Xrop WCF configuration found");
				}
				WSHttpBindingBase wshttpBindingBase = Binding.InitializeDefaultTemplateBinding(bindingTypeConfigurationKey);
				Binding.ListenerBinding.SetThrottling(wshttpBindingBase);
				return Binding.InitializeBindingElements(wshttpBindingBase.CreateBindingElements());
			}

			private static void SetThrottling(WSHttpBindingBase binding)
			{
				binding.MaxReceivedMessageSize = 1048576L;
				binding.ReceiveTimeout = TimeSpan.FromMinutes(10.0);
				binding.SendTimeout = TimeSpan.FromMinutes(10.0);
				Binding.ListenerBinding.SetThrottling(binding.ReaderQuotas);
			}

			private static void SetThrottling(XmlDictionaryReaderQuotas readerQuotas)
			{
				readerQuotas.MaxDepth = 64;
				readerQuotas.MaxStringContentLength = 1048576;
				readerQuotas.MaxArrayLength = 1048576;
				readerQuotas.MaxBytesPerRead = 1048576;
				readerQuotas.MaxNameTableCharCount = 16384;
			}

			private static void ConfigureSystemDotNetBehaviors()
			{
			}
		}
	}
}
