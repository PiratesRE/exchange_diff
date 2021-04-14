using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.Web;
using System.Web.Configuration;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.DispatchPipe.Ews;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Win32;

namespace Microsoft.Exchange.Services
{
	internal static class EWSSettings
	{
		private static T SafeGetMessageProperty<T>(string keyName, T defaultValue)
		{
			EwsOperationContextBase operationContext = EWSSettings.GetOperationContext();
			if (operationContext == null || operationContext.RequestMessage == null || operationContext.RequestMessage.State == MessageState.Closed)
			{
				EWSSettings.TraceSafeGetSetMessagePropertyFailure("EWSSettings::SafeGetMessageProperty", operationContext);
				return defaultValue;
			}
			object obj;
			if (operationContext.RequestMessage.Properties.TryGetValue(keyName, out obj) && obj is T)
			{
				return (T)((object)obj);
			}
			return defaultValue;
		}

		private static void SafeSetMessageProperty<T>(string keyName, T value)
		{
			EwsOperationContextBase operationContext = EWSSettings.GetOperationContext();
			if (operationContext == null || operationContext.RequestMessage == null || operationContext.RequestMessage.State == MessageState.Closed)
			{
				EWSSettings.TraceSafeGetSetMessagePropertyFailure("EWSSettings::SafeSetMessageProperty", operationContext);
				return;
			}
			operationContext.RequestMessage.Properties[keyName] = value;
		}

		private static void TraceSafeGetSetMessagePropertyFailure(string functionName, EwsOperationContextBase operationContext)
		{
			string text;
			string text2;
			string text3;
			if (operationContext == null)
			{
				text = "null";
				text2 = "n/a";
				text3 = "n/a";
			}
			else if (operationContext.RequestMessage == null)
			{
				text = "non-null";
				if (operationContext is WrappedWcfOperationContext)
				{
					text2 = ((operationContext.BackingOperationContext.RequestContext != null) ? "non-null" : "null");
				}
				else
				{
					text2 = "n/a";
				}
				text3 = "null";
			}
			else
			{
				text = "non-null";
				text2 = "non-null";
				text3 = "null";
			}
			ExTraceGlobals.CommonAlgorithmTracer.TraceDebug(0L, "[{0}] Could not get/set message property because one of the following was null: OperationContext.Current({1}); OperationContext.Current.RequestContext({2}); OperationContext.Current.RequestContext.RequestMessage({3})", new object[]
			{
				functionName,
				text,
				text2,
				text3
			});
		}

		private static T SafeGet<T>(string keyName, T defaultValue)
		{
			HttpContext httpContext = EWSSettings.GetHttpContext();
			if (httpContext == null)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug(0L, "[EWSSettings::SafeGet] Could not get HttpContext.Current property because HttpContext.Current was null");
				return defaultValue;
			}
			object obj = httpContext.Items[keyName];
			if (obj != null)
			{
				return (T)((object)obj);
			}
			return defaultValue;
		}

		private static void SafeSet<T>(string keyName, T value)
		{
			HttpContext httpContext = EWSSettings.GetHttpContext();
			if (httpContext == null)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug(0L, "[EWSSettings::SafeSet] Could not get HttpContext.Current property because HttpContext.Current was null");
				return;
			}
			httpContext.Items[keyName] = value;
		}

		internal static HttpWebResponse ProxyResponse
		{
			get
			{
				return EWSSettings.SafeGet<HttpWebResponse>("WS_ProxyResponse", null);
			}
			set
			{
				EWSSettings.SafeSet<HttpWebResponse>("WS_ProxyResponse", value);
			}
		}

		internal static Dictionary<string, string> ProxyHopHeaders
		{
			get
			{
				return EWSSettings.SafeGet<Dictionary<string, string>>("WS_ProxyHopHeaders", null);
			}
			set
			{
				EWSSettings.SafeSet<Dictionary<string, string>>("WS_ProxyHopHeaders", value);
			}
		}

		internal static bool InWCFChannelLayer
		{
			get
			{
				return EWSSettings.SafeGet<bool>("WS_InWCFChannelLayer", false);
			}
			set
			{
				EWSSettings.SafeSet<bool>("WS_InWCFChannelLayer", value);
			}
		}

		internal static string FailoverType
		{
			get
			{
				return EWSSettings.SafeGet<string>("WS_FailoverType", null);
			}
			set
			{
				EWSSettings.SafeSet<string>("WS_FailoverType", value);
			}
		}

		internal static string ExceptionType
		{
			get
			{
				return EWSSettings.SafeGet<string>("WS_ExceptionType", null);
			}
			set
			{
				EWSSettings.SafeSet<string>("WS_ExceptionType", value);
			}
		}

		internal static bool WritingToWire
		{
			get
			{
				return EWSSettings.SafeGetMessageProperty<bool>(EWSSettings.WritingToWireKey, false);
			}
			set
			{
				EWSSettings.SafeSetMessageProperty<bool>(EWSSettings.WritingToWireKey, value);
			}
		}

		internal static Message MessageCopyForProxyOnly
		{
			get
			{
				return EWSSettings.SafeGet<Message>("WS_ProxyMessageCopy", null);
			}
			set
			{
				EWSSettings.SafeSet<Message>("WS_ProxyMessageCopy", value);
			}
		}

		internal static string UpnFromClaimSets
		{
			get
			{
				return EWSSettings.SafeGet<string>("WS_UpnFromClaimSets", null);
			}
			set
			{
				EWSSettings.SafeSet<string>("WS_UpnFromClaimSets", value);
			}
		}

		internal static bool FaultExceptionDueToAuthorizationManager
		{
			get
			{
				return EWSSettings.SafeGet<bool>("WS_FaultExceptionAuthZMgr", false);
			}
			set
			{
				EWSSettings.SafeSet<bool>("WS_FaultExceptionAuthZMgr", value);
			}
		}

		public static Guid RequestCorrelation
		{
			get
			{
				return EWSSettings.SafeGetMessageProperty<Guid>("WS_RequestCorrelationKey", Guid.Empty);
			}
		}

		public static int RequestThreadId
		{
			get
			{
				return EWSSettings.SafeGetMessageProperty<int>("WS_RequestThreadIdKey", -1);
			}
		}

		public static Exception WcfDelayedException
		{
			get
			{
				return EWSSettings.SafeGetMessageProperty<Exception>("WS_WcfDelayedExceptionKey", null);
			}
			set
			{
				EWSSettings.SafeSetMessageProperty<Exception>("WS_WcfDelayedExceptionKey", value);
			}
		}

		internal static bool? ItemHasBlockedImages
		{
			get
			{
				return EWSSettings.SafeGet<bool?>("WS_ItemHasBlockedImages", null);
			}
			set
			{
				EWSSettings.SafeSet<bool?>("WS_ItemHasBlockedImages", value);
			}
		}

		internal static JunkEmailRule JunkEmailRule
		{
			get
			{
				return EWSSettings.SafeGet<JunkEmailRule>("WS_JunkEmailRuleKey", null);
			}
			set
			{
				EWSSettings.SafeSet<JunkEmailRule>("WS_JunkEmailRuleKey", value);
			}
		}

		internal static IDictionary<string, bool> InlineImagesInUniqueBody
		{
			get
			{
				IDictionary<string, bool> dictionary = EWSSettings.SafeGet<IDictionary<string, bool>>("WS_InlineImageIdsToUniqueBody", null);
				if (dictionary == null)
				{
					dictionary = new Dictionary<string, bool>();
					EWSSettings.SafeSet<IDictionary<string, bool>>("WS_InlineImageIdsToUniqueBody", dictionary);
				}
				return dictionary;
			}
		}

		internal static IDictionary<string, bool> InlineImagesInNormalizedBody
		{
			get
			{
				IDictionary<string, bool> dictionary = EWSSettings.SafeGet<IDictionary<string, bool>>("WS_InlineImageIdsToNormalizedBody", null);
				if (dictionary == null)
				{
					dictionary = new Dictionary<string, bool>();
					EWSSettings.SafeSet<IDictionary<string, bool>>("WS_InlineImageIdsToNormalizedBody", dictionary);
				}
				return dictionary;
			}
		}

		internal static void SetInlineAttachmentFlags(ItemType item)
		{
			if (item != null && item.Attachments != null)
			{
				foreach (Microsoft.Exchange.Services.Core.Types.AttachmentType attachmentType in item.Attachments)
				{
					attachmentType.IsInlineToUniqueBody = EWSSettings.InlineImagesInUniqueBody.ContainsKey(attachmentType.AttachmentId.Id);
					attachmentType.IsInlineToNormalBody = EWSSettings.InlineImagesInNormalizedBody.ContainsKey(attachmentType.AttachmentId.Id);
					if ((!string.IsNullOrEmpty(attachmentType.ContentType) && "audio/wav".Contains(attachmentType.ContentType.ToLowerInvariant())) || attachmentType is ReferenceAttachmentType)
					{
						attachmentType.IsInline = false;
					}
					else
					{
						attachmentType.IsInline = (attachmentType.IsInlineToUniqueBody || attachmentType.IsInlineToNormalBody || attachmentType.IsInline);
					}
					item.HasAttachments |= !attachmentType.IsInline;
				}
			}
		}

		private static bool GetOWARegistryValue(string valueName, bool defaultValue)
		{
			bool result;
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(EWSSettings.MSExchangeOWARegistryPath, false))
				{
					object value = registryKey.GetValue(valueName);
					if (value == null || !(value is int))
					{
						result = defaultValue;
					}
					else
					{
						result = ((int)value != 0);
					}
				}
			}
			catch (SecurityException)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError<string, bool>(0L, "[EWSSettings::GetOWARegistryValue] Security exception encountered while retrieving {0} registry value.  Defaulting to {1}", valueName, defaultValue);
				result = defaultValue;
			}
			catch (UnauthorizedAccessException)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError<string, bool>(0L, "[EWSSettings::allowInternalUntrustedCerts delegate] Security exception encountered while retrieving {0} registry value.  Defaulting to {1}", valueName, defaultValue);
				result = defaultValue;
			}
			return result;
		}

		internal static EwsOperationContextBase GetOperationContext()
		{
			EwsOperationContextBase ewsOperationContextBase = null;
			if (EwsOperationContextBase.Current != null)
			{
				ewsOperationContextBase = EwsOperationContextBase.Current;
			}
			else
			{
				CallContext callContext = CallContext.Current;
				if (callContext != null)
				{
					ewsOperationContextBase = callContext.OperationContext;
				}
			}
			if (ewsOperationContextBase == null)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug(0L, "EWSSettings.GetOperationContext() OperationContext is null.");
			}
			return ewsOperationContextBase;
		}

		internal static HttpContext GetHttpContext()
		{
			HttpContext httpContext = null;
			if (HttpContext.Current != null)
			{
				httpContext = HttpContext.Current;
			}
			else
			{
				CallContext callContext = CallContext.Current;
				if (callContext != null)
				{
					httpContext = callContext.HttpContext;
				}
			}
			if (httpContext == null)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug(0L, "EWSSettings.GetHttpContext() HttpContext is null.");
			}
			return httpContext;
		}

		public static bool AllowInternalUntrustedCerts
		{
			get
			{
				return EWSSettings.allowInternalUntrustedCerts.Member;
			}
		}

		public static bool AllowProxyingWithoutSSL
		{
			get
			{
				return EWSSettings.allowProxyingWithoutSSL.Member;
			}
		}

		public static CultureInfo ClientCulture
		{
			get
			{
				return EWSSettings.SafeGet<CultureInfo>("WS_ClientCultureKey", null);
			}
			set
			{
				EWSSettings.SafeSet<CultureInfo>("WS_ClientCultureKey", value);
			}
		}

		public static CultureInfo ServerCulture
		{
			get
			{
				return EWSSettings.SafeGet<CultureInfo>("WS_ServerCultureKey", null);
			}
			set
			{
				EWSSettings.SafeSet<CultureInfo>("WS_ServerCultureKey", value);
			}
		}

		internal static ParticipantInformationDictionary ParticipantInformation
		{
			get
			{
				ParticipantInformationDictionary participantInformationDictionary = EWSSettings.SafeGet<ParticipantInformationDictionary>("WS_ParticipantInformation", null);
				if (participantInformationDictionary == null)
				{
					participantInformationDictionary = new ParticipantInformationDictionary();
					EWSSettings.SafeSet<ParticipantInformationDictionary>("WS_ParticipantInformation", participantInformationDictionary);
				}
				return participantInformationDictionary;
			}
		}

		internal static DistinguishedFolderIdNameDictionary DistinguishedFolderIdNameDictionary
		{
			get
			{
				return EWSSettings.SafeGet<DistinguishedFolderIdNameDictionary>("WS_DistinguishedFolderIdNameDictionary", null);
			}
			set
			{
				EWSSettings.SafeSet<DistinguishedFolderIdNameDictionary>("WS_DistinguishedFolderIdNameDictionary", value);
			}
		}

		internal static Dictionary<AttachmentId, Microsoft.Exchange.Services.Core.Types.AttachmentType> AttachmentInformation
		{
			get
			{
				return EWSSettings.SafeGet<Dictionary<AttachmentId, Microsoft.Exchange.Services.Core.Types.AttachmentType>>("WS_AttachmentInformation", null);
			}
			set
			{
				EWSSettings.SafeSet<Dictionary<AttachmentId, Microsoft.Exchange.Services.Core.Types.AttachmentType>>("WS_AttachmentInformation", value);
			}
		}

		internal static ICoreConversation CurrentConversation
		{
			get
			{
				return EWSSettings.SafeGet<ICoreConversation>("WS_CurrentConversationKey", null);
			}
			set
			{
				EWSSettings.SafeSet<ICoreConversation>("WS_CurrentConversationKey", value);
			}
		}

		internal static bool CreateItemWithAttachments
		{
			get
			{
				return EWSSettings.SafeGet<bool>("WS_CreateItemWithAttachments", false);
			}
			set
			{
				EWSSettings.SafeSet<bool>("WS_CreateItemWithAttachments", value);
			}
		}

		internal static int AttachmentNestLevel
		{
			get
			{
				return EWSSettings.SafeGet<int>("WS_AttachmentNestLevelKey", 0);
			}
			set
			{
				EWSSettings.SafeSet<int>("WS_AttachmentNestLevelKey", value);
			}
		}

		public static PostSavePropertyCollection PostSavePropertyCommands
		{
			get
			{
				PostSavePropertyCollection postSavePropertyCollection = EWSSettings.SafeGet<PostSavePropertyCollection>("WS_PostSaveProperties", null);
				if (postSavePropertyCollection == null)
				{
					postSavePropertyCollection = new PostSavePropertyCollection();
					EWSSettings.SafeSet<PostSavePropertyCollection>("WS_PostSaveProperties", postSavePropertyCollection);
				}
				return postSavePropertyCollection;
			}
		}

		public static ExTimeZone RequestTimeZone
		{
			get
			{
				return EWSSettings.SafeGet<ExTimeZone>("WS_RequestTimeZoneKey", ExTimeZone.UtcTimeZone);
			}
			set
			{
				EWSSettings.SafeSet<ExTimeZone>("WS_RequestTimeZoneKey", value);
			}
		}

		public static bool UpdateSessionTimeZoneFromRequestSoapHeader(StoreSession session)
		{
			ExTimeZone exTimeZone = EWSSettings.SafeGet<ExTimeZone>("WS_RequestTimeZoneKey", null);
			if (exTimeZone != null)
			{
				session.ExTimeZone = exTimeZone;
				return true;
			}
			return false;
		}

		public static ExTimeZone DefaultGmtTimeZone
		{
			get
			{
				if (EWSSettings.gmtTimeZone == null && !ExTimeZoneEnumerator.Instance.TryGetTimeZoneByName("Greenwich Standard Time", out EWSSettings.gmtTimeZone))
				{
					ExTimeZoneInformation exTimeZoneInformation = new ExTimeZoneInformation("Greenwich Standard Time", "Greenwich Standard Time");
					ExTimeZoneRuleGroup exTimeZoneRuleGroup = new ExTimeZoneRuleGroup(null);
					ExTimeZoneRule ruleInfo = new ExTimeZoneRule("Standard", "Standard", new TimeSpan(0L), null);
					exTimeZoneRuleGroup.AddRule(ruleInfo);
					exTimeZoneInformation.AddGroup(exTimeZoneRuleGroup);
					EWSSettings.gmtTimeZone = new ExTimeZone(exTimeZoneInformation);
				}
				return EWSSettings.gmtTimeZone;
			}
		}

		public static DateTimePrecision DateTimePrecision
		{
			get
			{
				return EWSSettings.SafeGet<DateTimePrecision>("WS_DateTimePrecisionKey", DateTimePrecision.Seconds);
			}
			set
			{
				EWSSettings.SafeSet<DateTimePrecision>("WS_DateTimePrecisionKey", value);
			}
		}

		internal static string SimpleAssemblyName
		{
			get
			{
				return EWSSettings.simpleAssemblyName.Member;
			}
		}

		internal static bool IsPartnerHostedOnly
		{
			get
			{
				return EWSSettings.isPartnerHostedOnly.Member;
			}
		}

		internal static List<string> OtherSimpleAssemblyNames { get; set; }

		internal static bool IsFromEwsAssemblies(string source)
		{
			return source == EWSSettings.SimpleAssemblyName || (EWSSettings.OtherSimpleAssemblyNames != null && EWSSettings.OtherSimpleAssemblyNames.Contains(source));
		}

		internal static bool IsWsSecurityAddress(Uri uri)
		{
			return uri.LocalPath.EndsWith("wssecurity", StringComparison.OrdinalIgnoreCase);
		}

		internal static bool IsWsSecuritySymmetricKeyAddress(Uri uri)
		{
			return uri.LocalPath.EndsWith("wssecurity/symmetrickey", StringComparison.OrdinalIgnoreCase);
		}

		internal static bool IsWsSecurityX509CertAddress(Uri uri)
		{
			return uri.LocalPath.EndsWith("wssecurity/x509cert", StringComparison.OrdinalIgnoreCase);
		}

		internal static double WcfDispatchLatency
		{
			get
			{
				return EWSSettings.SafeGetMessageProperty<double>("WcfLatency", 0.0);
			}
		}

		private static void CheckWsSecurityEndpointsStatus()
		{
			EWSSettings.isWsSecurityEndpointEnabled = new bool?(false);
			EWSSettings.isWsSecuritySymmetricKeyEndpointEnabled = new bool?(false);
			EWSSettings.isWsSecurityX509CertEndpointEnabled = new bool?(false);
			EWSSettings.isOAuthEndpointEnabled = new bool?(OAuthHttpModule.IsModuleLoaded.Value);
			Configuration config = WebConfigurationManager.OpenWebConfiguration("~/web.config");
			ServiceElementCollection services = ServiceModelSectionGroup.GetSectionGroup(config).Services.Services;
			foreach (object obj in services)
			{
				ServiceElement serviceElement = (ServiceElement)obj;
				foreach (object obj2 in serviceElement.Endpoints)
				{
					ServiceEndpointElement serviceEndpointElement = (ServiceEndpointElement)obj2;
					if (!string.IsNullOrEmpty(serviceEndpointElement.BindingConfiguration) && (serviceEndpointElement.Contract.Equals("Microsoft.Exchange.Services.Wcf.IEWSContract", StringComparison.OrdinalIgnoreCase) || serviceEndpointElement.Contract.Equals("Microsoft.Exchange.Services.Wcf.IEWSStreamingContract", StringComparison.OrdinalIgnoreCase)))
					{
						if (serviceEndpointElement.Address.OriginalString.Equals("wssecurity", StringComparison.OrdinalIgnoreCase))
						{
							EWSSettings.isWsSecurityEndpointEnabled = new bool?(true);
						}
						else if (serviceEndpointElement.Address.OriginalString.Equals("wssecurity/symmetrickey", StringComparison.OrdinalIgnoreCase))
						{
							EWSSettings.isWsSecuritySymmetricKeyEndpointEnabled = new bool?(true);
						}
						else if (serviceEndpointElement.Address.OriginalString.Equals("wssecurity/x509cert", StringComparison.OrdinalIgnoreCase))
						{
							EWSSettings.isWsSecurityX509CertEndpointEnabled = new bool?(true);
						}
					}
				}
			}
		}

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

		internal static BaseResponseRenderer ResponseRenderer
		{
			get
			{
				return EWSSettings.SafeGetMessageProperty<BaseResponseRenderer>("WS_ResponseRenderer", SoapWcfResponseRenderer.Singleton);
			}
			set
			{
				EWSSettings.SafeSetMessageProperty<BaseResponseRenderer>("WS_ResponseRenderer", value ?? SoapWcfResponseRenderer.Singleton);
			}
		}

		internal static Stream MessageStream
		{
			get
			{
				return EWSSettings.SafeGetMessageProperty<Stream>("MessageStream", null);
			}
			set
			{
				EWSSettings.SafeSetMessageProperty<Stream>("MessageStream", value);
			}
		}

		internal static bool IsMultiTenancyEnabled
		{
			get
			{
				return EWSSettings.isMultiTenancyEnabled.Member;
			}
		}

		internal static bool IsLinkedAccountTokenMungingEnabled
		{
			get
			{
				return EWSSettings.isLinkedAccountTokenMungingEnabled.Member;
			}
		}

		internal static bool IsWsPerformanceCountersEnabled
		{
			get
			{
				return EWSSettings.isWsPerformanceCountersEnabled.Member;
			}
		}

		public static Guid SelfSiteGuid
		{
			get
			{
				return EWSSettings.selfSiteGuid.Member;
			}
		}

		internal static bool IsWsSecurityEndpointEnabled
		{
			get
			{
				if (EWSSettings.isWsSecurityEndpointEnabled == null)
				{
					EWSSettings.CheckWsSecurityEndpointsStatus();
				}
				return EWSSettings.isWsSecurityEndpointEnabled.Value;
			}
		}

		internal static bool IsWsSecuritySymmetricKeyEndpointEnabled
		{
			get
			{
				if (EWSSettings.isWsSecuritySymmetricKeyEndpointEnabled == null)
				{
					EWSSettings.CheckWsSecurityEndpointsStatus();
				}
				return EWSSettings.isWsSecuritySymmetricKeyEndpointEnabled.Value;
			}
		}

		internal static bool IsWsSecurityX509CertEndpointEnabled
		{
			get
			{
				if (EWSSettings.isWsSecurityX509CertEndpointEnabled == null)
				{
					EWSSettings.CheckWsSecurityEndpointsStatus();
				}
				return EWSSettings.isWsSecurityX509CertEndpointEnabled.Value;
			}
		}

		internal static bool IsOAuthEndpointEnabled
		{
			get
			{
				if (EWSSettings.isOAuthEndpointEnabled == null)
				{
					EWSSettings.CheckWsSecurityEndpointsStatus();
				}
				return EWSSettings.isOAuthEndpointEnabled.Value;
			}
		}

		internal static bool AreGccStoredSecretKeysValid
		{
			get
			{
				return EWSSettings.areGccStoredSecretKeysValid.Member;
			}
		}

		internal static bool DisableReferenceAttachment
		{
			get
			{
				if (EWSSettings.disableReferenceAttachment == null)
				{
					EWSSettings.disableReferenceAttachment = new bool?(Global.GetAppSettingAsBool("DisableReferenceAttachment", false));
				}
				return EWSSettings.disableReferenceAttachment.Value;
			}
		}

		internal static void SetOutgoingHttpStatusCode(HttpStatusCode statusCode)
		{
			EWSSettings.ResponseRenderer = SoapWcfResponseRenderer.Create(statusCode);
		}

		private const string ClientCultureKey = "WS_ClientCultureKey";

		private const string ServerCultureKey = "WS_ServerCultureKey";

		private const string RequestTimeZoneKey = "WS_RequestTimeZoneKey";

		private const string DateTimePrecisionKey = "WS_DateTimePrecisionKey";

		private const string ProxyMessageCopyKey = "WS_ProxyMessageCopy";

		private const string UpnFromClaimSetsKey = "WS_UpnFromClaimSets";

		private const string FaultExceptionDueToAuthorizationManagerKey = "WS_FaultExceptionAuthZMgr";

		private const string ProxyHopHeadersKey = "WS_ProxyHopHeaders";

		private const string ProxyResponseKey = "WS_ProxyResponse";

		private const string FailoverTypeKey = "WS_FailoverType";

		private const string ExceptionTypeKey = "WS_ExceptionType";

		private const string ParticipantInformationKey = "WS_ParticipantInformation";

		private const string DistinguishedFolderIdNameDictionaryKey = "WS_DistinguishedFolderIdNameDictionary";

		private const string AttachmentInformationKey = "WS_AttachmentInformation";

		private const string CurrentConversationKey = "WS_CurrentConversationKey";

		private const string CreateItemWithAttachmentsKey = "WS_CreateItemWithAttachments";

		private const string AttachmentNestLevelKey = "WS_AttachmentNestLevelKey";

		private const string InWCFChannelLayerKey = "WS_InWCFChannelLayer";

		private const string PostSavePropertiesKey = "WS_PostSaveProperties";

		private const string WebMethodEntryKey = "WS_WebMethodEntry";

		private const string ItemHasBlockedImagesKey = "WS_ItemHasBlockedImages";

		private const string JunkEmailRuleKey = "WS_JunkEmailRuleKey";

		private const string InlineImageIdsToUniqueBodyKey = "WS_InlineImageIdsToUniqueBody";

		private const string InlineImageIdsToNormalizedBodyKey = "WS_InlineImageIdsToNormalizedBody";

		public const string EWSAnonymousHttpsBindingName = "EWSAnonymousHttpsBinding";

		public const string EWSAnonymousHttpBindingName = "EWSAnonymousHttpBinding";

		public const string EWSBasicHttpsBindingName = "EWSBasicHttpsBinding";

		public const string EWSBasicHttpBindingName = "EWSBasicHttpBinding";

		public const string EWSNegotiateHttpsBindingName = "EWSNegotiateHttpsBinding";

		public const string EWSNegotiateHttpBindingName = "EWSNegotiateHttpBinding";

		public const string EWSWSSecurityHttpsBindingName = "EWSWSSecurityHttpsBinding";

		public const string EWSWSSecurityHttpBindingName = "EWSWSSecurityHttpBinding";

		public const string EWSStreamingNegotiateHttpsBindingName = "EWSStreamingNegotiateHttpsBinding";

		public const string EWSStreamingNegotiateHttpBindingName = "EWSStreamingNegotiateHttpBinding";

		public const string ServiceName = "Microsoft.Exchange.Services.Wcf.EWSService";

		public const string Contract = "Microsoft.Exchange.Services.Wcf.IEWSContract";

		public const string StreamingContract = "Microsoft.Exchange.Services.Wcf.IEWSStreamingContract";

		public const string WsSecurityAddress = "wssecurity";

		public const string WsSecuritySymmetricKeyAddress = "wssecurity/symmetrickey";

		public const string WsSecurityX509CertAddress = "wssecurity/x509cert";

		public const string EndpointNameHttps = "Https";

		public const string RequestCorrelationKey = "WS_RequestCorrelationKey";

		public const string RequestThreadIdKey = "WS_RequestThreadIdKey";

		public const string WcfDelayedExceptionKey = "WS_WcfDelayedExceptionKey";

		private const string inlineAsAttachedContentTypes = "audio/wav";

		private const string ResponseRendererKey = "WS_ResponseRenderer";

		private const string AppSettingDisableReferenceAttachment = "DisableReferenceAttachment";

		internal const string SelfSiteIdKey = "WS_SetSiteIdKey";

		private static readonly string MSExchangeOWARegistryPath = "SYSTEM\\CurrentControlSet\\Services\\MSExchange OWA";

		private static readonly bool AllowInternalUntrustedCertsDefault = true;

		private static readonly string AllowInternalUntrustedCertsValueName = "AllowInternalUntrustedCerts";

		private static readonly bool AllowProxyingWithoutSSLDefault = false;

		private static readonly string AllowProxyingWithoutSSLValueName = "AllowProxyingWithoutSSL";

		private static readonly string WritingToWireKey = "WS_WritingToWireKey";

		private static LazyMember<bool> allowInternalUntrustedCerts = new LazyMember<bool>(() => EWSSettings.GetOWARegistryValue(EWSSettings.AllowInternalUntrustedCertsValueName, EWSSettings.AllowInternalUntrustedCertsDefault));

		private static LazyMember<bool> allowProxyingWithoutSSL = new LazyMember<bool>(() => EWSSettings.GetOWARegistryValue(EWSSettings.AllowProxyingWithoutSSLValueName, EWSSettings.AllowProxyingWithoutSSLDefault));

		private static LazyMember<string> simpleAssemblyName = new LazyMember<string>(delegate()
		{
			AssemblyName assemblyName = new AssemblyName(typeof(EWSSettings).GetTypeInfo().Assembly.FullName);
			return assemblyName.Name;
		});

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

		private static ExTimeZone gmtTimeZone = null;

		private static LazyMember<Guid> selfSiteGuid = new LazyMember<Guid>(() => LocalServer.GetServer().ServerSite.ObjectGuid);

		private static LazyMember<bool> isLinkedAccountTokenMungingEnabled = new LazyMember<bool>(() => VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Ews.LinkedAccountTokenMunging.Enabled);

		private static LazyMember<bool> isMultiTenancyEnabled = new LazyMember<bool>(() => VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled);

		private static LazyMember<bool> isWsPerformanceCountersEnabled = new LazyMember<bool>(() => VariantConfiguration.InvariantNoFlightingSnapshot.Ews.WsPerformanceCounters.Enabled);

		private static bool? isWsSecurityEndpointEnabled;

		private static bool? isWsSecuritySymmetricKeyEndpointEnabled;

		private static bool? isWsSecurityX509CertEndpointEnabled;

		private static bool? isOAuthEndpointEnabled;

		private static bool? disableReferenceAttachment;

		private static LazyMember<bool> areGccStoredSecretKeysValid = new LazyMember<bool>(() => GccUtils.AreStoredSecretKeysValid());
	}
}
