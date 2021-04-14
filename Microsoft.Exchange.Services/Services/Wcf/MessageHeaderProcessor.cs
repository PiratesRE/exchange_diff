using System;
using System.Globalization;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Web;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Availability;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Win32;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class MessageHeaderProcessor
	{
		static MessageHeaderProcessor()
		{
			MessageHeaderProcessor.installedClientCultures = LanguagePackInfo.GetInstalledLanguagePackCultures(LanguagePackType.Client);
			MessageHeaderProcessor.installedServerCultures = LanguagePackInfo.GetInstalledLanguagePackCultures(LanguagePackType.Server);
			MessageHeaderProcessor.InitializeExchangeServerCulture();
		}

		public static MessageHeaderProcessor GetInstance()
		{
			if (MessageHeaderProcessor.singletonInstance == null)
			{
				MessageHeaderProcessor.singletonInstance = new MessageHeaderProcessor();
			}
			return MessageHeaderProcessor.singletonInstance;
		}

		private static void InitializeExchangeServerCulture()
		{
			MessageHeaderProcessor.exchangeServerCulture = CultureInfo.CurrentUICulture;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Exchange\\Language"))
			{
				if (registryKey != null)
				{
					string[] valueNames = registryKey.GetValueNames();
					if (valueNames != null)
					{
						foreach (string name in valueNames)
						{
							int num;
							if (int.TryParse(registryKey.GetValue(name, 0).ToString(), out num))
							{
								try
								{
									MessageHeaderProcessor.exchangeServerCulture = new CultureInfo(num);
									break;
								}
								catch (ArgumentException)
								{
									Microsoft.Exchange.Diagnostics.Components.Services.ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<int>(0L, "Invalid lcid found in registry (SOFTWARE\\Microsoft\\Exchange\\Language) : {0}", num);
								}
							}
						}
					}
				}
			}
			if (!MessageHeaderProcessor.IsCultureSupported(MessageHeaderProcessor.exchangeServerCulture, MessageHeaderProcessor.installedServerCultures))
			{
				Microsoft.Exchange.Diagnostics.Components.Services.ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string>(0L, "[CallContext::InitializeExchangeServerCulture] OS culture '{0}' does not have a corresponding language pack installed.  Defaulting to en-US.", MessageHeaderProcessor.exchangeServerCulture.Name);
				MessageHeaderProcessor.exchangeServerCulture = new CultureInfo("en-US");
			}
		}

		public static CultureInfo GetExchangeServerCulture()
		{
			return MessageHeaderProcessor.exchangeServerCulture;
		}

		internal virtual void ValidateRights(CallContext callContext, AuthZClientInfo callerClientInfo, Message request)
		{
			if (callContext.MailboxAccessType == MailboxAccessType.ServerToServer || callContext.MailboxAccessType == MailboxAccessType.ExchangeImpersonation)
			{
				OriginatedFrom originatedFrom = callContext.EffectiveCaller.OriginatedFrom;
				if (originatedFrom == OriginatedFrom.AccessToken && !S2SRightsWrapper.AllowsTokenSerializationBy(callerClientInfo.ClientSecurityContext))
				{
					throw new TokenSerializationDeniedException();
				}
			}
			bool flag = false;
			Microsoft.Exchange.Diagnostics.Components.Services.ExTraceGlobals.FaultInjectionTracer.TraceTest<bool>(3167104317U, ref flag);
			if (flag)
			{
				Microsoft.Exchange.Diagnostics.Components.Services.ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<LogonType, LogonTypeSource, bool>(0L, "[MessageHeaderProcesor::ValidateRights] Skip token serialziation for OpenAsAdminOrSystemServiceHeader or IsPublic flag.LogonType: {0}, Source: {1}, IsPublic: {2}", callContext.LogonType, callContext.LogonTypeSource, callContext.WebMethodEntry.IsPublic);
				return;
			}
			if (callContext.RequirePrivilegedLogon && callContext.LogonTypeSource == LogonTypeSource.OpenAsAdminOrSystemServiceHeader)
			{
				bool flag2 = false;
				AuthZClientInfo.ApplicationAttachedAuthZClientInfo applicationAttachedAuthZClientInfo = callerClientInfo as AuthZClientInfo.ApplicationAttachedAuthZClientInfo;
				if (applicationAttachedAuthZClientInfo != null)
				{
					flag2 = applicationAttachedAuthZClientInfo.OAuthIdentity.IsKnownFromSameOrgExchange;
				}
				if (!S2SRightsWrapper.AllowsTokenSerializationBy(callerClientInfo.ClientSecurityContext) && !flag2)
				{
					throw new TokenSerializationDeniedException();
				}
			}
			if (!callContext.WebMethodEntry.IsPublic && !S2SRightsWrapper.AllowsTokenSerializationBy(callerClientInfo.ClientSecurityContext))
			{
				throw new TokenSerializationDeniedException();
			}
		}

		internal static void GetCulture(string mailboxCulture, out CultureInfo serverCulture, out CultureInfo clientCulture)
		{
			serverCulture = MessageHeaderProcessor.GetExchangeServerCulture();
			clientCulture = serverCulture;
			if (!string.IsNullOrEmpty(mailboxCulture))
			{
				try
				{
					CultureInfo cultureInfo = new CultureInfo(mailboxCulture);
					if (MessageHeaderProcessor.IsCultureSupported(cultureInfo, MessageHeaderProcessor.installedServerCultures))
					{
						serverCulture = cultureInfo;
					}
					if (MessageHeaderProcessor.IsCultureSupported(cultureInfo, MessageHeaderProcessor.installedClientCultures))
					{
						clientCulture = cultureInfo;
					}
				}
				catch (ArgumentException)
				{
					Microsoft.Exchange.Diagnostics.Components.Services.ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string>(0L, "Invalid MailboxCulture : {0}", mailboxCulture);
				}
			}
		}

		internal virtual void ProcessMailboxCultureHeader(Message request)
		{
			string text = null;
			string mailboxCulture = MessageHeaderProcessor.GetMessageHeader<string>(request.Headers, "MailboxCulture", "http://schemas.microsoft.com/exchange/services/2006/types", out text) ? text : null;
			CultureInfo serverCulture;
			CultureInfo clientCulture;
			MessageHeaderProcessor.GetCulture(mailboxCulture, out serverCulture, out clientCulture);
			EWSSettings.ClientCulture = clientCulture;
			EWSSettings.ServerCulture = serverCulture;
		}

		protected static bool IsCultureSupported(CultureInfo culture, CultureInfo[] installedCultures)
		{
			while (!culture.Equals(CultureInfo.InvariantCulture))
			{
				foreach (CultureInfo cultureInfo in installedCultures)
				{
					if (culture.LCID == cultureInfo.LCID)
					{
						return true;
					}
				}
				culture = culture.Parent;
			}
			return false;
		}

		internal virtual void ProcessTimeZoneContextHeader(Message request)
		{
			EWSSettings.RequestTimeZone = null;
			TimeZoneContextType timeZoneContextType;
			if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2007SP1) && MessageHeaderProcessor.GetMessageHeader<TimeZoneContextType>(request.Headers, "TimeZoneContext", "http://schemas.microsoft.com/exchange/services/2006/types", out timeZoneContextType) && timeZoneContextType.TimeZoneDefinition != null)
			{
				EWSSettings.RequestTimeZone = timeZoneContextType.TimeZoneDefinition.ExTimeZone;
			}
		}

		internal virtual void ProcessDateTimePrecisionHeader(Message request)
		{
			EWSSettings.DateTimePrecision = DateTimePrecision.Seconds;
			DateTimePrecision dateTimePrecision;
			if (MessageHeaderProcessor.GetMessageHeader<DateTimePrecision>(request.Headers, "DateTimePrecision", "http://schemas.microsoft.com/exchange/services/2006/types", out dateTimePrecision))
			{
				EWSSettings.DateTimePrecision = dateTimePrecision;
			}
		}

		internal virtual bool ProcessBackgroundLoadHeader(Message request)
		{
			return false;
		}

		internal virtual bool ProcessServiceUnavailableOnTransientErrorHeader(Message request)
		{
			return false;
		}

		internal virtual ManagementRoleType ProcessManagementRoleHeader(Message request)
		{
			ManagementRoleType managementRoleType = null;
			if (MessageHeaderProcessor.GetMessageHeader<ManagementRoleType>(request.Headers, "ManagementRole", "http://schemas.microsoft.com/exchange/services/2006/types", out managementRoleType) && managementRoleType != null)
			{
				if (this.ContainsMessageHeader(request, "ExchangeImpersonation", "http://schemas.microsoft.com/exchange/services/2006/types") || this.ContainsMessageHeader(request, "OpenAsAdminOrSystemService", "http://schemas.microsoft.com/exchange/services/2006/types") || this.ContainsMessageHeader(request, "SerializedSecurityContext", "http://schemas.microsoft.com/exchange/services/2006/types"))
				{
					throw new InvalidManagementRoleHeaderException(CoreResources.IDs.MessageManagementRoleHeaderCannotUseWithOtherHeaders);
				}
				managementRoleType.ValidateAndConvert();
			}
			return managementRoleType;
		}

		internal virtual ProxyRequestType? ProcessRequestTypeHeader(Message request)
		{
			int num = 0;
			foreach (MessageHeaderInfo messageHeaderInfo in request.Headers)
			{
				if (messageHeaderInfo.Name == "RequestTypeHeader")
				{
					using (XmlDictionaryReader readerAtHeader = request.Headers.GetReaderAtHeader(num))
					{
						if (readerAtHeader.ReadToDescendant("RequestType", "http://schemas.microsoft.com/exchange/services/2006/types"))
						{
							readerAtHeader.Read();
							return this.ParseProxyRequestType(readerAtHeader.Value);
						}
						break;
					}
				}
				num++;
			}
			return null;
		}

		internal virtual AuthZClientInfo ProcessProxyHeaders(Message incomingMessage, AuthZClientInfo callerClientInfo)
		{
			byte[] proxyHeaderBytes = null;
			byte[] proxyHeaderBytes2 = null;
			byte[] proxyHeaderBytes3 = null;
			bool messageHeader = MessageHeaderProcessor.GetMessageHeader<byte[]>(incomingMessage.Headers, "ProxySecurityContext", "http://schemas.microsoft.com/exchange/services/2006/types", out proxyHeaderBytes);
			bool messageHeader2 = MessageHeaderProcessor.GetMessageHeader<byte[]>(incomingMessage.Headers, "ProxySuggesterSid", "http://schemas.microsoft.com/exchange/services/2006/types", out proxyHeaderBytes2);
			bool messageHeader3 = MessageHeaderProcessor.GetMessageHeader<byte[]>(incomingMessage.Headers, "ProxyPartnerToken", "http://schemas.microsoft.com/exchange/services/2006/types", out proxyHeaderBytes3);
			AuthZClientInfo authZClientInfo = null;
			if (messageHeader)
			{
				if (messageHeader2 || messageHeader3)
				{
					MessageHeaderProcessor.GenerateFaultForDuplicateProxy(messageHeader, messageHeader2, messageHeader3);
				}
				Microsoft.Exchange.Diagnostics.Components.Services.ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug(0L, "[MessageHeaderProcessor::ProcessProxyHeaders] Request contained a full proxy token header");
				authZClientInfo = ProxyHeaderConverter.ToAuthZClientInfo(callerClientInfo, ProxyHeaderType.FullToken, proxyHeaderBytes);
			}
			else if (messageHeader2)
			{
				if (messageHeader3)
				{
					MessageHeaderProcessor.GenerateFaultForDuplicateProxy(messageHeader, messageHeader2, messageHeader3);
				}
				Microsoft.Exchange.Diagnostics.Components.Services.ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug(0L, "[MessageHeaderProcessor::ProcessProxyHeaders] Request contained a proxy suggester sid header");
				authZClientInfo = ProxyHeaderConverter.ToAuthZClientInfo(callerClientInfo, ProxyHeaderType.SuggesterSid, proxyHeaderBytes2);
			}
			else if (messageHeader3)
			{
				Microsoft.Exchange.Diagnostics.Components.Services.ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug(0L, "[MessageHeaderProcessor::ProcessProxyHeaders] Request contained a proxy partner identity header");
				authZClientInfo = ProxyHeaderConverter.ToPartnerAuthZClientInfo(callerClientInfo, ProxyHeaderType.PartnerToken, proxyHeaderBytes3);
			}
			if (authZClientInfo != null && authZClientInfo.PrimarySmtpAddress != null)
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(RequestDetailsLogger.Current, EwsMetadata.ProxyAsUser, authZClientInfo.PrimarySmtpAddress);
			}
			return authZClientInfo;
		}

		private static void GenerateFaultForDuplicateProxy(bool proxySecurityContextPresent, bool proxySuggesterSidPresent, bool proxyPartnerTokenPresent)
		{
			Microsoft.Exchange.Diagnostics.Components.Services.ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug<bool, bool, bool>(0L, "[MessageHeaderProcessor::GenerateFaultForDuplicateProxy] Found more than one proxy soap header where proxySecurityContextPresent {0}, proxySuggesterSidPresent {1} and ProxyPartnerTokenPresent {2}.  Failing request", proxySecurityContextPresent, proxySuggesterSidPresent, proxyPartnerTokenPresent);
			throw FaultExceptionUtilities.CreateFault(new TokenSerializationDeniedException(), FaultParty.Sender);
		}

		private bool? CheckIfImpersonationMustPresent(AuthZClientInfo impersonatingClientInfo)
		{
			AuthZClientInfo.ApplicationAttachedAuthZClientInfo applicationAttachedAuthZClientInfo = impersonatingClientInfo as AuthZClientInfo.ApplicationAttachedAuthZClientInfo;
			if (applicationAttachedAuthZClientInfo == null)
			{
				return null;
			}
			OAuthIdentity oauthIdentity = applicationAttachedAuthZClientInfo.OAuthIdentity;
			switch (oauthIdentity.OAuthApplication.ApplicationType)
			{
			case OAuthApplicationType.S2SApp:
			case OAuthApplicationType.CallbackApp:
			case OAuthApplicationType.V1ExchangeSelfIssuedApp:
				return new bool?(false);
			case OAuthApplicationType.V1App:
				return new bool?(oauthIdentity.IsAppOnly);
			default:
				throw new InvalidOperationException();
			}
		}

		internal virtual AuthZClientInfo ProcessImpersonationHeaders(Message request, AuthZClientInfo proxyClientInfo, AuthZClientInfo impersonatingClientInfo)
		{
			bool? flag = this.CheckIfImpersonationMustPresent(impersonatingClientInfo);
			ExchangeImpersonationType exchangeImpersonationType = null;
			AuthZClientInfo result = null;
			try
			{
				if (MessageHeaderProcessor.GetMessageHeader<ExchangeImpersonationType>(request.Headers, "ExchangeImpersonation", "http://schemas.microsoft.com/exchange/services/2006/types", out exchangeImpersonationType) && exchangeImpersonationType != null)
				{
					if (flag != null && !flag.Value)
					{
						RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericError(RequestDetailsLogger.Current, "EIHeader", "ShouldNotPresent");
						throw FaultExceptionUtilities.CreateFault(new InvalidExchangeImpersonationHeaderException(), FaultParty.Sender);
					}
					IIdentity impersonatingIdentity;
					if (proxyClientInfo != null)
					{
						impersonatingIdentity = null;
					}
					else
					{
						impersonatingIdentity = HttpContext.Current.User.Identity;
					}
					result = MessageHeaderProcessor.CreateAuthZClientInfoFromConnectingSID(exchangeImpersonationType.ConnectingSID, impersonatingClientInfo, impersonatingIdentity);
					return result;
				}
				else if (flag != null && flag.Value)
				{
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericError(RequestDetailsLogger.Current, "EIHeader", "MustPresent");
					throw FaultExceptionUtilities.CreateFault(new InvalidExchangeImpersonationHeaderException(), FaultParty.Sender);
				}
			}
			catch (AuthzException ex)
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeLogRequestException(RequestDetailsLogger.Current, ex, "MessageHeaderProcessor.ProcessImpersonationHeaders_AuthzException");
				throw FaultExceptionUtilities.CreateFault(new AuthZFailureException(ex), FaultParty.Sender);
			}
			catch (LocalizedException ex2)
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeLogRequestException(RequestDetailsLogger.Current, ex2, "MessageHeaderProcessor.ProcessImpersonationHeaders_LocalizedException");
				throw FaultExceptionUtilities.CreateFault(ex2, FaultParty.Sender);
			}
			return result;
		}

		internal virtual AuthZClientInfo ProcessSerializedSecurityContextHeaders(Message request)
		{
			AuthZClientInfo result;
			try
			{
				SerializedSecurityContextType serializedSecurityContextType = null;
				SerializedSecurityContextTypeForAS serializedSecurityContextTypeForAS = null;
				ExchangeImpersonationType exchangeImpersonationType = null;
				OpenAsAdminOrSystemServiceType openAsAdminOrSystemServiceType = null;
				AuthZClientInfo authZClientInfo = null;
				if (MessageHeaderProcessor.GetMessageHeader<SerializedSecurityContextType>(request.Headers, "SerializedSecurityContext", "http://schemas.microsoft.com/exchange/services/2006/types", out serializedSecurityContextType) && serializedSecurityContextType != null)
				{
					if (MessageHeaderProcessor.GetMessageHeader<ExchangeImpersonationType>(request.Headers, "ExchangeImpersonation", "http://schemas.microsoft.com/exchange/services/2006/types", out exchangeImpersonationType) && exchangeImpersonationType != null)
					{
						Microsoft.Exchange.Diagnostics.Components.Services.ExTraceGlobals.ServerToServerAuthZTracer.TraceDebug(0L, "[MessageHeaderProcessor::ProcessS2SHeaders] Found more than one S2S SOAP headers (SerializedSecurityContext + ExchangeImpersonation).  Failing request.");
						throw new MoreThanOneAccessModeSpecifiedException();
					}
					if (MessageHeaderProcessor.GetMessageHeader<OpenAsAdminOrSystemServiceType>(request.Headers, "OpenAsAdminOrSystemService", "http://schemas.microsoft.com/exchange/services/2006/types", out openAsAdminOrSystemServiceType) && openAsAdminOrSystemServiceType != null)
					{
						Microsoft.Exchange.Diagnostics.Components.Services.ExTraceGlobals.ServerToServerAuthZTracer.TraceDebug(0L, "[MessageHeaderProcessor::ProcessS2SHeaders] Found more than one S2S SOAP headers (SerializedSecurityContext + OpenAsAdminOrSystemService).  Failing request.");
						throw new MoreThanOneAccessModeSpecifiedException();
					}
					authZClientInfo = serializedSecurityContextType.ToAuthZClientInfo();
				}
				else if (MessageHeaderProcessor.GetMessageHeader<SerializedSecurityContextTypeForAS>(request.Headers, "SerializedSecurityContext", "http://schemas.microsoft.com/exchange/services/2006/messages", out serializedSecurityContextTypeForAS) && serializedSecurityContextTypeForAS != null)
				{
					string text = HttpContext.Current.Request.Headers[WellKnownHeader.AnchorMailbox];
					if (!string.IsNullOrEmpty(text) && SmtpAddress.IsValidSmtpAddress(text))
					{
						serializedSecurityContextTypeForAS.PrimarySmtpAddress = text;
					}
					authZClientInfo = serializedSecurityContextTypeForAS.ToAuthZClientInfo();
				}
				if (authZClientInfo != null && authZClientInfo.PrimarySmtpAddress != null)
				{
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(RequestDetailsLogger.Current, EwsMetadata.ActAsUser, authZClientInfo.PrimarySmtpAddress);
				}
				result = authZClientInfo;
			}
			catch (AuthzException innerException)
			{
				throw FaultExceptionUtilities.CreateFault(new AuthZFailureException(innerException), FaultParty.Sender);
			}
			catch (LocalizedException exception)
			{
				throw FaultExceptionUtilities.CreateFault(exception, FaultParty.Sender);
			}
			return result;
		}

		internal virtual AuthZClientInfo ProcessOpenAsAdminOrSystemServiceHeader(Message request, AuthZClientInfo impersonatingClientInfo, out SpecialLogonType? specialLogonType, out int? budgetType)
		{
			OpenAsAdminOrSystemServiceType openAsAdminOrSystemServiceType = null;
			AuthZClientInfo result = null;
			specialLogonType = null;
			budgetType = null;
			try
			{
				if (MessageHeaderProcessor.GetMessageHeader<OpenAsAdminOrSystemServiceType>(request.Headers, "OpenAsAdminOrSystemService", "http://schemas.microsoft.com/exchange/services/2006/types", out openAsAdminOrSystemServiceType) && openAsAdminOrSystemServiceType != null)
				{
					if (!ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2010SP1))
					{
						Microsoft.Exchange.Diagnostics.Components.Services.ExTraceGlobals.ServerToServerAuthZTracer.TraceDebug<ExchangeVersion>(0L, "[MessageHeaderProcessor::ProcessSpecialMailboxOpenHeaders] OpenAsAdminOrSystemServiceType header is only supported after E14SP1. The request was from {0}. Failing request.", ExchangeVersion.Current);
						throw new InvalidRequestException();
					}
					if (openAsAdminOrSystemServiceType.LogonType != SpecialLogonType.SystemService && openAsAdminOrSystemServiceType.LogonType != SpecialLogonType.Admin)
					{
						Microsoft.Exchange.Diagnostics.Components.Services.ExTraceGlobals.ServerToServerAuthZTracer.TraceDebug<SpecialLogonType>(0L, "[MessageHeaderProcessor::ProcessSpecialMailboxOpenHeaders] Found unexpected logon type {0} in the OpenAsAdminOrSystemService header. Failing request.", openAsAdminOrSystemServiceType.LogonType);
						throw new InvalidLogonTypeException();
					}
					ExchangeImpersonationType exchangeImpersonationType;
					if (MessageHeaderProcessor.GetMessageHeader<ExchangeImpersonationType>(request.Headers, "ExchangeImpersonation", "http://schemas.microsoft.com/exchange/services/2006/types", out exchangeImpersonationType) && exchangeImpersonationType != null)
					{
						Microsoft.Exchange.Diagnostics.Components.Services.ExTraceGlobals.ServerToServerAuthZTracer.TraceDebug(0L, "[MessageHeaderProcessor::ProcessSpecialMailboxOpenHeaders] Found the ExchangeImpersonationType header along with the OpenAsAdminOrSystemService header. Failing request.");
						throw new MoreThanOneAccessModeSpecifiedException();
					}
					result = MessageHeaderProcessor.CreateAuthZClientInfoFromConnectingSID(openAsAdminOrSystemServiceType.ConnectingSID, impersonatingClientInfo, null);
					Microsoft.Exchange.Diagnostics.Components.Services.ExTraceGlobals.ServerToServerAuthZTracer.TraceDebug<SpecialLogonType, bool, int>(0L, "[MessageHeaderProcessor::ProcessSpecialMailboxOpenHeaders] Found the ExchangeImpersonationType header with logonType {0}, budgetTypeSpecified {1}, budgetType {2}", openAsAdminOrSystemServiceType.LogonType, openAsAdminOrSystemServiceType.BudgetTypeSpecified, openAsAdminOrSystemServiceType.BudgetType);
					specialLogonType = new SpecialLogonType?(openAsAdminOrSystemServiceType.LogonType);
					budgetType = new int?(openAsAdminOrSystemServiceType.BudgetTypeSpecified ? openAsAdminOrSystemServiceType.BudgetType : 0);
					RequestDetailsLogger.Current.AppendGenericInfo("BudgetType", budgetType.ToString());
				}
			}
			catch (AuthzException innerException)
			{
				throw FaultExceptionUtilities.CreateFault(new AuthZFailureException(innerException), FaultParty.Sender);
			}
			catch (LocalizedException exception)
			{
				throw FaultExceptionUtilities.CreateFault(exception, FaultParty.Sender);
			}
			return result;
		}

		internal bool IsAvailabilityServiceS2S(Message request)
		{
			SerializedSecurityContextTypeForAS serializedSecurityContextTypeForAS = null;
			return MessageHeaderProcessor.GetMessageHeader<SerializedSecurityContextTypeForAS>(request.Headers, "SerializedSecurityContext", "http://schemas.microsoft.com/exchange/services/2006/messages", out serializedSecurityContextTypeForAS) && serializedSecurityContextTypeForAS != null;
		}

		internal bool SeeksProxyingOrS2S(Message request)
		{
			return this.ContainsMessageHeader(request, "ExchangeImpersonation", "http://schemas.microsoft.com/exchange/services/2006/types") || this.ContainsMessageHeader(request, "ProxySecurityContext", "http://schemas.microsoft.com/exchange/services/2006/types") || this.ContainsMessageHeader(request, "SerializedSecurityContext", "http://schemas.microsoft.com/exchange/services/2006/types");
		}

		internal static XmlElement GetMessageHeaderAsXmlElement(MessageHeaders messageHeaders, string elementName, string xmlNamespace)
		{
			XmlElement result;
			bool messageHeader = MessageHeaderProcessor.GetMessageHeader<XmlElement>(messageHeaders, elementName, xmlNamespace, out result);
			if (messageHeader)
			{
				return result;
			}
			return null;
		}

		private static bool GetMessageHeader<T>(MessageHeaders messageHeaders, string elementName, string xmlNamespace, out T header)
		{
			int num = 0;
			try
			{
				num = messageHeaders.FindHeader(elementName, xmlNamespace);
			}
			catch (MessageHeaderException innerException)
			{
				throw FaultExceptionUtilities.CreateFault(new DuplicateSOAPHeaderException(innerException), FaultParty.Sender);
			}
			if (num < 0)
			{
				header = default(T);
				return false;
			}
			Type typeFromHandle = typeof(T);
			if (typeFromHandle.IsPrimitive || typeFromHandle.IsArray || typeFromHandle == typeof(string))
			{
				header = messageHeaders.GetHeader<T>(num);
			}
			else
			{
				XmlDictionaryReader readerAtHeader = messageHeaders.GetReaderAtHeader(num);
				SafeXmlSerializer safeXmlSerializer = new SafeXmlSerializer(typeFromHandle);
				try
				{
					header = (T)((object)safeXmlSerializer.Deserialize(readerAtHeader));
				}
				catch (InvalidOperationException)
				{
					throw FaultExceptionUtilities.CreateFault(new ServiceInvalidOperationException(CoreResources.IDs.MessageMalformedSoapHeader)
					{
						ConstantValues = 
						{
							{
								"HeaderName",
								elementName
							}
						}
					}, FaultParty.Sender);
				}
			}
			if (!messageHeaders.UnderstoodHeaders.Contains(messageHeaders[num]))
			{
				messageHeaders.UnderstoodHeaders.Add(messageHeaders[num]);
			}
			return true;
		}

		private static AuthZClientInfo CreateAuthZClientInfoFromConnectingSID(ConnectingSIDType connectingSID, AuthZClientInfo impersonatingClientInfo, IIdentity impersonatingIdentity)
		{
			ImpersonationProcessorBase impersonationProcessorBase;
			if (!string.IsNullOrEmpty(connectingSID.PrincipalName))
			{
				impersonationProcessorBase = new UpnImpersonationProcessor(connectingSID.PrincipalName, impersonatingClientInfo, impersonatingIdentity);
			}
			else if (!string.IsNullOrEmpty(connectingSID.SID))
			{
				impersonationProcessorBase = new SidImpersonationProcessor(connectingSID.SID, impersonatingClientInfo, impersonatingIdentity);
			}
			else if (!string.IsNullOrEmpty(connectingSID.PrimarySmtpAddress))
			{
				impersonationProcessorBase = new SmtpAddressImpersonationProcessor(connectingSID.PrimarySmtpAddress, true, impersonatingClientInfo, impersonatingIdentity);
			}
			else
			{
				if (string.IsNullOrEmpty(connectingSID.SmtpAddress))
				{
					Microsoft.Exchange.Diagnostics.Components.Services.ExTraceGlobals.ServerToServerAuthZTracer.TraceDebug(0L, "[MessageHeaderProcessor::CreateAuthZClientInfoFromConnectingSID] Header did not contain any data.");
					throw new InvalidExchangeImpersonationHeaderException();
				}
				impersonationProcessorBase = new SmtpAddressImpersonationProcessor(connectingSID.SmtpAddress, false, impersonatingClientInfo, impersonatingIdentity);
			}
			return impersonationProcessorBase.CreateAuthZClientInfo();
		}

		private bool ContainsMessageHeader(Message message, string elementName, string xmlNamespace)
		{
			int num = 0;
			try
			{
				num = message.Headers.FindHeader(elementName, xmlNamespace);
			}
			catch (MessageHeaderException innerException)
			{
				throw FaultExceptionUtilities.CreateFault(new DuplicateSOAPHeaderException(innerException), FaultParty.Sender);
			}
			return num >= 0;
		}

		internal void MarkMessageHeaderAsUnderstoodIfExists(Message message, string elementName, string xmlNamespace)
		{
			MessageHeaders headers = message.Headers;
			int num = 0;
			try
			{
				num = headers.FindHeader(elementName, xmlNamespace);
			}
			catch (MessageHeaderException innerException)
			{
				throw FaultExceptionUtilities.CreateFault(new DuplicateSOAPHeaderException(innerException), FaultParty.Sender);
			}
			if (num >= 0 && !headers.UnderstoodHeaders.Contains(headers[num]))
			{
				headers.UnderstoodHeaders.Add(headers[num]);
			}
		}

		protected ProxyRequestType? ParseProxyRequestType(string requestType)
		{
			if (requestType != null)
			{
				if (requestType == "CrossSite")
				{
					return new ProxyRequestType?(ProxyRequestType.CrossSite);
				}
				if (requestType == "CrossForest")
				{
					return new ProxyRequestType?(ProxyRequestType.CrossForest);
				}
			}
			return null;
		}

		private static CultureInfo[] installedServerCultures;

		private static CultureInfo[] installedClientCultures;

		private static CultureInfo exchangeServerCulture;

		private static MessageHeaderProcessor singletonInstance;

		protected static readonly Trace SecurityTracer = Microsoft.Exchange.Diagnostics.Components.InfoWorker.Availability.ExTraceGlobals.SecurityTracer;

		protected static readonly Trace CalendarViewTracer = Microsoft.Exchange.Diagnostics.Components.InfoWorker.Availability.ExTraceGlobals.CalendarViewTracer;

		protected static readonly Trace ConfigurationTracer = Microsoft.Exchange.Diagnostics.Components.InfoWorker.Availability.ExTraceGlobals.ConfigurationTracer;
	}
}
