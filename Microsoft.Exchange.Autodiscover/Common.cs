using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Xml;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Autodiscover;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Autodiscover;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Win32;

namespace Microsoft.Exchange.Autodiscover
{
	internal static class Common
	{
		private static ServiceEndpointElement TryFindEndpointByNameAndContract(ServiceEndpointElementCollection endpoints, string name, string contract)
		{
			if (endpoints != null)
			{
				foreach (object obj in endpoints)
				{
					ServiceEndpointElement serviceEndpointElement = (ServiceEndpointElement)obj;
					if (serviceEndpointElement.Name == name && serviceEndpointElement.Contract == contract)
					{
						return serviceEndpointElement;
					}
				}
			}
			return null;
		}

		public static void GenerateErrorResponseDontLog(XmlWriter xmlFragment, string ns, string errorCode, string message, string debugData, RequestData requestData, string assemblyQualifiedName)
		{
			if (string.IsNullOrEmpty(ns))
			{
				ns = "http://schemas.microsoft.com/exchange/autodiscover/responseschema/2006";
			}
			Common.StartEnvelope(xmlFragment);
			xmlFragment.WriteStartElement("Response", ns);
			xmlFragment.WriteStartElement("Error", ns);
			xmlFragment.WriteAttributeString("Time", requestData.Timestamp);
			xmlFragment.WriteAttributeString("Id", requestData.ComputerNameHash);
			xmlFragment.WriteElementString("ErrorCode", ns, errorCode);
			xmlFragment.WriteElementString("Message", ns, message);
			xmlFragment.WriteElementString("DebugData", debugData);
			xmlFragment.WriteEndElement();
			xmlFragment.WriteEndElement();
			Common.EndEnvelope(xmlFragment);
			ExTraceGlobals.FrameworkTracer.TraceDebug((long)requestData.GetHashCode(), "[GenerateErrorResponse()] Time=\"{0}\",Id=\"{1}\",ErrorCode=\"{2}\",Message=\"{3}\",DebugData=\"{4}\"", new object[]
			{
				requestData.Timestamp,
				requestData.ComputerNameHash,
				errorCode,
				message,
				debugData
			});
			RequestDetailsLoggerBase<RequestDetailsLogger>.Current.Set(ServiceCommonMetadata.ErrorCode, errorCode);
			RequestDetailsLoggerBase<RequestDetailsLogger>.Current.AppendGenericError("message", message);
			RequestDetailsLoggerBase<RequestDetailsLogger>.Current.AppendGenericError("debugData", debugData);
		}

		public static void GenerateErrorResponse(XmlWriter xmlFragment, string ns, string errorCode, string message, string debugData, RequestData requestData, string assemblyQualifiedName)
		{
			Common.GenerateErrorResponseDontLog(xmlFragment, ns, errorCode, message, debugData, requestData, assemblyQualifiedName);
			Common.EventLog.LogEvent(AutodiscoverEventLogConstants.Tuple_WarnProvErrorResponse, Common.PeriodicKey, new object[]
			{
				requestData.Timestamp,
				requestData.ComputerNameHash,
				errorCode,
				message,
				debugData,
				requestData.EMailAddress,
				requestData.LegacyDN,
				assemblyQualifiedName
			});
		}

		public static bool DoesEmailAddressReferenceArchive(ADRecipient recipient, string emailAddress)
		{
			bool result = false;
			Guid g;
			if (AutodiscoverCommonUserSettings.TryGetExchangeGuidFromEmailAddress(emailAddress, out g) && (recipient.RecipientType == RecipientType.MailUser || recipient.RecipientType == RecipientType.UserMailbox))
			{
				ADUser aduser = (ADUser)recipient;
				if (aduser.ArchiveGuid.Equals(g))
				{
					result = true;
				}
			}
			return result;
		}

		internal static void ThrowIfNullOrEmpty(string parameterValue, string parameterName)
		{
			if (parameterValue == null)
			{
				throw new ArgumentNullException(parameterName);
			}
			if (parameterValue.Length == 0)
			{
				throw new ArgumentException(parameterName);
			}
		}

		internal static void SendWatsonReportOnUnhandledException(ExWatson.MethodDelegate methodDelegate)
		{
			ExWatson.SendReportOnUnhandledException(methodDelegate, delegate(object exception)
			{
				bool flag = true;
				Exception ex = exception as Exception;
				if (ex != null)
				{
					ExTraceGlobals.FrameworkTracer.TraceError<Exception>(0L, "Encountered unhandled exception: {0}", ex);
					flag = Common.IsSendReportValid(ex);
				}
				ExTraceGlobals.FrameworkTracer.TraceError<bool>(0L, "SendWatsonReportOnUnhandledException isSendReportValid: {0}", flag);
				if (flag)
				{
					Common.EventLog.LogEvent(AutodiscoverEventLogConstants.Tuple_ErrWebException, Common.PeriodicKey, new object[]
					{
						ex.Message,
						ex.StackTrace
					});
				}
				RequestDetailsLoggerBase<RequestDetailsLogger>.Current.AppendGenericError("WatsonExceptionMessage", ex.ToString());
				return flag;
			}, ReportOptions.None);
		}

		internal static bool IsSendReportValid(Exception exception)
		{
			bool flag = true;
			if (exception.Data["FilterExceptionFromWatson"] is bool && (bool)exception.Data["FilterExceptionFromWatson"])
			{
				ExTraceGlobals.FrameworkTracer.TraceDebug<Exception>(0L, "[IsSendReportValid] Received {0} - skipping Watson reporting", exception);
				flag = false;
			}
			else if (exception is HttpException)
			{
				ExTraceGlobals.FrameworkTracer.TraceDebug<Exception>(0L, "[IsSendReportValid()] Received HttpException {0} - skipping Watson reporting", exception);
				flag = false;
			}
			else if (exception is ThreadAbortException)
			{
				flag = false;
			}
			else if (exception is COMException && ((COMException)exception).ErrorCode == -2147024832)
			{
				ExTraceGlobals.FrameworkTracer.TraceDebug<Exception>(0L, "[IsSendReportValid] Received COMException (0x80070040) {0} - skipping Watson reporting", exception);
				flag = false;
			}
			else if (exception is CommunicationException)
			{
				ExTraceGlobals.FrameworkTracer.TraceDebug<Exception>(0L, "[IsSendReportValid()] Received CommunicationException {0} - skipping Watson reporting", exception);
				flag = false;
			}
			else if (exception is DataValidationException)
			{
				flag = false;
			}
			else if (exception is DataSourceOperationException)
			{
				flag = false;
			}
			else if (exception is StoragePermanentException || exception is StorageTransientException)
			{
				flag = false;
			}
			else if (exception is ServiceDiscoveryTransientException)
			{
				flag = false;
			}
			else if (exception is IOException)
			{
				flag = false;
			}
			else if (exception is OutOfMemoryException)
			{
				flag = false;
			}
			else if (exception is ADTransientException)
			{
				flag = false;
			}
			else if (exception is DataSourceTransientException)
			{
				flag = false;
			}
			ExTraceGlobals.FrameworkTracer.TraceDebug<bool>(0L, "IsSendReportValid isSendReportValid: {0}", flag);
			return flag;
		}

		internal static void ReportException(Exception exception, object responsibleObject, HttpContext httpContext)
		{
			if (!Common.IsSendReportValid(exception))
			{
				return;
			}
			if (ExWatson.IsWatsonReportAlreadySent(exception))
			{
				return;
			}
			ExTraceGlobals.FrameworkTracer.TraceDebug<Type>((long)responsibleObject.GetHashCode(), "[ReportException()] exception.GetType()=\"{0}\"", exception.GetType());
			bool flag = exception is AccessViolationException;
			if (Common.EventLog != null && exception != null && exception.Message.Length != 0)
			{
				ExTraceGlobals.FrameworkTracer.TraceError<Exception>((long)responsibleObject.GetHashCode(), "[ReportException()] exception=\"{0}\"", exception);
				Common.EventLog.LogEvent(AutodiscoverEventLogConstants.Tuple_ErrWebException, Common.PeriodicKey, new object[]
				{
					exception.Message,
					exception.StackTrace
				});
				string text;
				string text2;
				string text3;
				if (httpContext != null && httpContext.Request != null && httpContext.User != null && httpContext.User.Identity.IsAuthenticated)
				{
					text = httpContext.User.Identity.GetSecurityIdentifier().Value;
					text2 = (httpContext.Request.UserHostAddress ?? string.Empty);
					text3 = (httpContext.Request.UserHostName ?? string.Empty);
				}
				else
				{
					text = string.Empty;
					text2 = string.Empty;
					text3 = string.Empty;
				}
				ExTraceGlobals.FrameworkTracer.TraceDebug<string, string, string>((long)responsibleObject.GetHashCode(), "[ReportException()] userSid=\"{0}\";userHostAddress=\"{1}\";userHostName=\"{2}\"", text, text2, text3);
				Common.EventLog.LogEvent(AutodiscoverEventLogConstants.Tuple_InfoWebSessionFailure, Common.PeriodicKey, new object[]
				{
					text,
					text2,
					text3
				});
			}
			ExWatson.HandleException(new UnhandledExceptionEventArgs(exception, flag), ReportOptions.None);
			ExWatson.SetWatsonReportAlreadySent(exception);
			if (flag)
			{
				ExTraceGlobals.FrameworkTracer.TraceError(0L, "[ReportException()] 'Terminating the process'");
				Environment.Exit(1);
			}
		}

		public static FileVersionInfo ServerVersion
		{
			get
			{
				return Common.serverVersion.Member;
			}
		}

		internal static bool IsMultiTenancyEnabled
		{
			get
			{
				return Common.isMultiTenancyEnabled.Member;
			}
		}

		public static bool IsPartnerHostedOnly
		{
			get
			{
				return Common.isPartnerHostedOnly.Member;
			}
		}

		internal static AuthenticationSchemes AutodiscoverBindingAuthenticationScheme
		{
			get
			{
				return Common.autodiscoverBindingAuthenticationScheme.Member;
			}
		}

		internal static void StartEnvelope(XmlWriter writer)
		{
			writer.WriteStartDocument();
			writer.WriteStartElement("Autodiscover", "http://schemas.microsoft.com/exchange/autodiscover/responseschema/2006");
		}

		internal static void EndEnvelope(XmlWriter writer)
		{
			writer.WriteEndElement();
			writer.WriteEndDocument();
			writer.Flush();
		}

		internal static string SafeGetUserAgent(HttpRequest request)
		{
			if (request == null)
			{
				return null;
			}
			return request.UserAgent ?? request.Headers.Get("UserAgent");
		}

		internal static bool CheckClientCertificate(HttpRequest request)
		{
			return FaultInjection.TraceTest<bool>((FaultInjection.LIDs)4213583165U) || (request != null && request.ClientCertificate != null && request.ClientCertificate.IsValid && string.Compare(request.ServerVariables["AUTH_TYPE"], "SSL/PCT", StringComparison.OrdinalIgnoreCase) == 0);
		}

		internal static string GetIdentityNameForTrace(IIdentity identity)
		{
			string result;
			try
			{
				result = identity.Name;
			}
			catch (SystemException)
			{
				result = "<Unknown>";
			}
			return result;
		}

		internal static ExchangePrincipal GetExchangePrincipal(Guid mailboxGuid, OrganizationId organizationId)
		{
			return RequestDetailsLoggerBase<RequestDetailsLogger>.Current.TrackLatency<ExchangePrincipal>(ServiceLatencyMetadata.ExchangePrincipalLatency, () => ExchangePrincipal.FromMailboxGuid(ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId), mailboxGuid, null));
		}

		internal static ExchangePrincipal GetExchangePrincipal(ADUser adUser)
		{
			return RequestDetailsLoggerBase<RequestDetailsLogger>.Current.TrackLatency<ExchangePrincipal>(ServiceLatencyMetadata.ExchangePrincipalLatency, () => ExchangePrincipal.FromADUser(ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(adUser.OrganizationId), adUser));
		}

		internal static void LoadAuthenticatingUserInfo(ADRecipient callerRecipient)
		{
			if (callerRecipient != null && HttpContext.Current != null && HttpContext.Current.Items["AuthenticatedUser"] == null)
			{
				HttpContext.Current.Items["AuthenticatedUser"] = callerRecipient.PrimarySmtpAddress;
				if (callerRecipient.OrganizationId != null && callerRecipient.OrganizationId.OrganizationalUnit != null)
				{
					HttpContext.Current.Items["AuthenticatedUserOrganization"] = callerRecipient.OrganizationId.OrganizationalUnit.Name;
				}
			}
		}

		public static bool IsWsSecurityAddress(Uri uri)
		{
			return uri.LocalPath.EndsWith("wssecurity", StringComparison.OrdinalIgnoreCase);
		}

		public static bool IsWsSecuritySymmetricKeyAddress(Uri uri)
		{
			return uri.LocalPath.EndsWith("wssecurity/symmetrickey", StringComparison.OrdinalIgnoreCase);
		}

		public static bool IsWsSecurityX509CertAddress(Uri uri)
		{
			return uri.LocalPath.EndsWith("wssecurity/x509cert", StringComparison.OrdinalIgnoreCase);
		}

		public static ADSessionSettings SessionSettingsFromAddress(string address)
		{
			if (Common.IsMultiTenancyEnabled && !string.IsNullOrEmpty(address) && SmtpAddress.IsValidSmtpAddress(address))
			{
				try
				{
					return ADSessionSettings.FromTenantAcceptedDomain(new SmtpAddress(address).Domain);
				}
				catch (CannotResolveTenantNameException)
				{
					ExTraceGlobals.FrameworkTracer.TraceDebug<string>(0L, "CreateSessionSettingsByAddresss -- Cannot locate organization for address {0}.", address);
				}
			}
			return ADSessionSettings.FromRootOrgScopeSet();
		}

		public static void ResolveCaller()
		{
			IIdentity identity = HttpContext.Current.User.Identity;
			if (!(identity is WindowsIdentity) && !(identity is ClientSecurityContextIdentity))
			{
				return;
			}
			try
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.Current.TrackLatency(ServiceLatencyMetadata.CallerADLatency, delegate()
				{
					DateTime utcNow = DateTime.UtcNow;
					OrganizationId organizationId = (OrganizationId)HttpContext.Current.Items["UserOrganizationId"];
					ADSessionSettings adsessionSettings;
					if (organizationId != null)
					{
						adsessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId);
						RequestDetailsLoggerBase<RequestDetailsLogger>.Current.AppendGenericInfo("ADSessionSettingsFromOrgId", (DateTime.UtcNow - utcNow).TotalMilliseconds);
					}
					else
					{
						string memberName = HttpContext.Current.GetMemberName();
						if (string.IsNullOrEmpty(memberName) && identity is SidBasedIdentity)
						{
							memberName = (identity as SidBasedIdentity).MemberName;
						}
						adsessionSettings = Common.SessionSettingsFromAddress(memberName);
						RequestDetailsLoggerBase<RequestDetailsLogger>.Current.AppendGenericInfo("ADSessionSettingsFromAddress", (DateTime.UtcNow - utcNow).TotalMilliseconds);
					}
					HttpContext.Current.Items["ADSessionSettings"] = adsessionSettings;
					utcNow = DateTime.UtcNow;
					IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, adsessionSettings, 989, "ResolveCaller", "f:\\15.00.1497\\sources\\dev\\autodisc\\src\\Common\\Common.cs");
					tenantOrRootOrgRecipientSession.ServerTimeout = new TimeSpan?(Common.RecipientLookupTimeout);
					ADRecipient adrecipient = tenantOrRootOrgRecipientSession.FindBySid(identity.GetSecurityIdentifier());
					RequestDetailsLoggerBase<RequestDetailsLogger>.Current.AppendGenericInfo("ADRecipientSessionFindBySid", (DateTime.UtcNow - utcNow).TotalMilliseconds);
					if (adrecipient != null)
					{
						ExTraceGlobals.FrameworkTracer.TraceDebug<ObjectId>(0L, "ResolveCaller -- Resolved caller is {0}.", adrecipient.Identity);
					}
					HttpContext.Current.Items["CallerRecipient"] = adrecipient;
					ADUser aduser = adrecipient as ADUser;
					if (aduser != null && aduser.NetID != null)
					{
						HttpContext.Current.Items["PassportUniqueId"] = aduser.NetID.ToString();
					}
				});
			}
			catch (NonUniqueRecipientException)
			{
				ExTraceGlobals.FrameworkTracer.TraceError<string>(0L, "ResolveCaller -- InternalResolveCaller returned non-unique user for {0}.", identity.Name);
			}
		}

		public static ADSessionSettings GetSessionSettingsForCallerScope()
		{
			ADSessionSettings adsessionSettings = HttpContext.Current.Items["ADSessionSettings"] as ADSessionSettings;
			if (adsessionSettings == null)
			{
				adsessionSettings = ADSessionSettings.FromRootOrgScopeSet();
			}
			return adsessionSettings;
		}

		public static string AddUserHintToUrl(Uri uri, string userHint)
		{
			userHint = userHint.Replace("@", "..");
			UriBuilder uriBuilder = new UriBuilder(uri);
			string[] segments = uri.Segments;
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < segments.Length - 1; i++)
			{
				stringBuilder.Append(segments[i]);
			}
			stringBuilder.Append(userHint);
			stringBuilder.Append('/');
			stringBuilder.Append(segments[segments.Length - 1]);
			uriBuilder.Path = stringBuilder.ToString();
			return uriBuilder.ToString();
		}

		public static bool SkipServiceTopologyInDatacenter(VariantConfigurationSnapshot variantConfigurationSnapshot)
		{
			return Common.IsMultiTenancyEnabled && variantConfigurationSnapshot.Autodiscover.SkipServiceTopologyDiscovery.Enabled;
		}

		public static bool SkipServiceTopologyInDatacenter()
		{
			return Common.IsMultiTenancyEnabled && VariantConfiguration.InvariantNoFlightingSnapshot.Autodiscover.SkipServiceTopologyDiscovery.Enabled;
		}

		internal static bool IsCustomEmailRedirectEnabled()
		{
			try
			{
				object value = Registry.GetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\MSExchange Autodiscover", "CustomEmailRedirect", 0);
				if (value is int)
				{
					return (int)value != 0;
				}
			}
			catch (Exception ex)
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.Current.AppendGenericError("IsCustomEmailRedirectEnabled", ex.ToString());
			}
			return false;
		}

		public const string XmlDefaultResponseSchema = "http://schemas.microsoft.com/exchange/autodiscover/responseschema/2006";

		public const string XmlElemAutodiscover = "Autodiscover";

		public const string XmlAtrXmlns = "xmlns";

		public const string XmlElemRequest = "Request";

		public const string XmlElemEMailAddress = "EMailAddress";

		public const string XmlElemLegacyDN = "LegacyDN";

		public const string XmlElemResponseSchema = "AcceptableResponseSchema";

		public const string RequestHeaderMapiHttp = "X-MapiHttpCapability";

		public const string FilterExceptionFromWatson = "FilterExceptionFromWatson";

		public const string AutodiscoverLocalPath = "/autodiscover/autodiscover.xml";

		public const string WsSecurityAddress = "wssecurity";

		public const string WsSecuritySymmetricKeyAddress = "wssecurity/symmetrickey";

		public const string WsSecurityX509CertAddress = "wssecurity/x509cert";

		public const string OAuthAddress = "oauth";

		public const string CertificateString = "Certificate";

		public const string AuthenticationMethodString = "AuthenticationMethod";

		public const string Anonymous = "Anonymous";

		public const string CallerRecipientItemKey = "CallerRecipient";

		public const string ADSessionSettingsItemKey = "ADSessionSettings";

		public const string UserOrganizationIdItemKey = "UserOrganizationId";

		public const string UserPUIDItemKey = "PassportUniqueId";

		internal const int Ews2010SP2SchemaMinVersion = 1937866977;

		internal static readonly TimeSpan RecipientLookupTimeout = TimeSpan.FromSeconds(30.0);

		public static readonly ExEventLog EventLog = new ExEventLog(new Guid("A5FB0E69-BDB3-429d-B927-01F7A2E0B258"), "MSExchange Autodiscover");

		public static readonly string PeriodicKey = string.Empty;

		public static readonly string EndpointContract = "Microsoft.Exchange.Autodiscover.WCF.IAutodiscover";

		private static readonly string EndpointNameHttps = "Https";

		private static readonly string ServiceName = "Microsoft.Exchange.Autodiscover.WCF.AutodiscoverService";

		private static LazyMember<FileVersionInfo> serverVersion = new LazyMember<FileVersionInfo>(() => FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location));

		private static LazyMember<bool> isMultiTenancyEnabled = new LazyMember<bool>(() => VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled);

		private static LazyMember<bool> isPartnerHostedOnly = new LazyMember<bool>(delegate()
		{
			try
			{
				if (Datacenter.IsPartnerHostedOnly(true))
				{
					return true;
				}
			}
			catch (CannotDetermineExchangeModeException)
			{
			}
			return false;
		});

		private static LazyMember<AuthenticationSchemes> autodiscoverBindingAuthenticationScheme = new LazyMember<AuthenticationSchemes>(delegate()
		{
			Configuration configuration = WebConfigurationManager.OpenWebConfiguration("~/web.config");
			ServicesSection servicesSection = (ServicesSection)configuration.GetSection("system.serviceModel/services");
			if (!servicesSection.Services.ContainsKey(Common.ServiceName))
			{
				ExTraceGlobals.FrameworkTracer.TraceDebug<string>(0L, "Service {0} was not found in web.config file.", Common.ServiceName);
				return AuthenticationSchemes.None;
			}
			ServiceElement serviceElement = servicesSection.Services[Common.ServiceName];
			ServiceEndpointElement serviceEndpointElement = Common.TryFindEndpointByNameAndContract(serviceElement.Endpoints, Common.EndpointNameHttps, Common.EndpointContract);
			if (serviceEndpointElement == null)
			{
				ExTraceGlobals.FrameworkTracer.TraceDebug<string, string>(0L, "Endpoint name='{0}' contract='{1}' was not found in web.config file.", Common.EndpointNameHttps, Common.EndpointContract);
				return AuthenticationSchemes.None;
			}
			string bindingConfiguration = serviceEndpointElement.BindingConfiguration;
			BindingsSection bindingsSection = (BindingsSection)configuration.GetSection("system.serviceModel/bindings");
			if (!bindingsSection.CustomBinding.Bindings.ContainsKey(bindingConfiguration))
			{
				ExTraceGlobals.FrameworkTracer.TraceDebug<string>(0L, "Binding {0} was not found in web.config file.", bindingConfiguration);
				return AuthenticationSchemes.None;
			}
			CustomBindingElement customBindingElement = bindingsSection.CustomBinding.Bindings[bindingConfiguration];
			HttpsTransportElement httpsTransportElement = (HttpsTransportElement)customBindingElement[typeof(HttpsTransportElement)];
			if (httpsTransportElement == null)
			{
				ExTraceGlobals.FrameworkTracer.TraceDebug<string>(0L, "https/http transport element not found on binding {0} in web.config file.", bindingConfiguration);
				return AuthenticationSchemes.None;
			}
			return httpsTransportElement.AuthenticationScheme;
		});
	}
}
