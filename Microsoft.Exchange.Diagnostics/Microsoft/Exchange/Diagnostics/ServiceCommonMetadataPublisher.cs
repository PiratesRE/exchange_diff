using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Diagnostics
{
	public static class ServiceCommonMetadataPublisher
	{
		public static void PublishMetadata()
		{
			ServiceCommonMetadataPublisher.PublishMetadata(HttpContext.Current);
		}

		public static void PublishMetadata(HttpContext context)
		{
			if (!ServiceCommonMetadataPublisher.isServiceCommonMetadataRegistered)
			{
				ActivityContext.RegisterMetadata(typeof(ServiceCommonMetadata));
				ServiceCommonMetadataPublisher.isServiceCommonMetadataRegistered = true;
			}
			HttpContextBase context2 = (context != null) ? new HttpContextWrapper(context) : null;
			ServiceCommonMetadataPublisher.PublishGeneric(context2);
			ServiceCommonMetadataPublisher.PublishAuthData(context2);
		}

		internal static void SetResponseHeader(this HttpContext context, string source, string propertyName, string value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				try
				{
					string name = string.Format("{0}_{1}_{2}", "X-DEBUG", source, propertyName);
					context.Response.Headers[name] = value;
				}
				catch (SystemException ex)
				{
					IActivityScope currentActivityScope = ActivityContext.GetCurrentActivityScope();
					if (currentActivityScope != null)
					{
						currentActivityScope.AppendToProperty(ServiceCommonMetadata.GenericErrors, ex.Message);
					}
				}
			}
		}

		internal static string GetContextItem(this HttpContext context, string key)
		{
			if (context == null)
			{
				return null;
			}
			return context.Items.GetContextItem(key);
		}

		internal static void PublishServerInfo(IActivityScope activityScope)
		{
			if (activityScope == null)
			{
				return;
			}
			activityScope.SetProperty(ServiceCommonMetadata.ServerVersionMajor, ServiceCommonMetadataPublisher.VersionMajor);
			activityScope.SetProperty(ServiceCommonMetadata.ServerVersionMinor, ServiceCommonMetadataPublisher.VersionMinor);
			activityScope.SetProperty(ServiceCommonMetadata.ServerVersionBuild, ServiceCommonMetadataPublisher.VersionBuild);
			activityScope.SetProperty(ServiceCommonMetadata.ServerVersionRevision, ServiceCommonMetadataPublisher.VersionRevision);
			activityScope.SetProperty(ServiceCommonMetadata.ServerHostName, ServiceCommonMetadataPublisher.MachineName);
		}

		internal static Dictionary<Enum, string> GetAuthValues(this HttpContextBase context)
		{
			Dictionary<Enum, string> dictionary = new Dictionary<Enum, string>(9);
			if (context != null)
			{
				IDictionary items = context.Items;
				HttpRequestBase request = context.Request;
				dictionary.SetNonNullValue(ServiceCommonMetadata.IsAuthenticated, request.IsAuthenticated ? "true" : "false");
				string text = items.GetContextItem("AuthType");
				string contextItem = items.GetContextItem("AuthModuleLatency");
				string text2 = items.GetContextItem("WLID-MemberName");
				string contextItem2 = items.GetContextItem("AuthenticatedUserOrganization");
				if (string.IsNullOrEmpty(text) || text == "Unknown")
				{
					text = context.GetAuthType();
				}
				if (string.IsNullOrEmpty(text))
				{
					text = request.GetRequestHeader("Authorization");
					if (!string.IsNullOrEmpty(text))
					{
						text = text.Split(new char[]
						{
							' '
						})[0];
					}
				}
				if (string.IsNullOrEmpty(text2))
				{
					if (!string.IsNullOrEmpty(text2 = items.GetContextItem("LiveIdNegotiateMemberName")))
					{
						text = text + ";" + items.GetContextItem("NegoCap");
					}
					else if (string.IsNullOrEmpty(text2 = items.GetContextItem("RPSMemberName")))
					{
						if (!string.IsNullOrEmpty(text2 = items.GetContextItem("AuthenticatedUser")))
						{
							if (text2.Contains("\\"))
							{
								text2 = text2.Split(new char[]
								{
									'\\'
								})[1];
							}
						}
						else
						{
							text2 = context.GetAuthUser();
						}
					}
				}
				dictionary.SetNonNullValue(ActivityStandardMetadata.AuthenticationType, text);
				dictionary.SetNonNullValue(ServiceLatencyMetadata.AuthModuleLatency, contextItem);
				dictionary.SetNonNullValue(ServiceCommonMetadata.LiveIdBasicLog, items.GetContextItem("LiveIdBasicLog"));
				dictionary.SetNonNullValue(ServiceCommonMetadata.LiveIdBasicError, items.GetContextItem("LiveIdBasicError"));
				dictionary.SetNonNullValue(ServiceCommonMetadata.LiveIdNegotiateError, items.GetContextItem("LiveIdNegotiateError"));
				dictionary.SetNonNullValue(ServiceCommonMetadata.OAuthToken, items.GetContextItem("OAuthToken"));
				dictionary.SetNonNullValue(ServiceCommonMetadata.OAuthError, items.GetContextItem("OAuthError"));
				dictionary.SetNonNullValue(ServiceCommonMetadata.OAuthErrorCategory, items.GetContextItem("OAuthErrorCategory"));
				dictionary.SetNonNullValue(ServiceCommonMetadata.OAuthExtraInfo, items.GetContextItem("OAuthExtraInfo"));
				dictionary.SetNonNullValue(ServiceCommonMetadata.AuthenticatedUser, text2);
				dictionary.SetNonNullValue(ActivityStandardMetadata.TenantId, contextItem2);
				dictionary.SetNonNullValue(ActivityStandardMetadata.Puid, items.GetContextItem("PassportUniqueId"));
			}
			return dictionary;
		}

		private static string GetAuthType(this HttpContextBase context)
		{
			if (context != null)
			{
				try
				{
					if (context.User != null && context.User.Identity.IsAuthenticated)
					{
						string authenticationType = context.User.Identity.AuthenticationType;
						if (!string.IsNullOrEmpty(authenticationType))
						{
							return authenticationType;
						}
					}
				}
				catch (SystemException ex)
				{
					IActivityScope currentActivityScope = ActivityContext.GetCurrentActivityScope();
					if (currentActivityScope != null)
					{
						currentActivityScope.AppendToProperty(ServiceCommonMetadata.GenericErrors, ex.Message);
					}
				}
			}
			return null;
		}

		private static string GetAuthUser(this HttpContextBase context)
		{
			if (context != null)
			{
				try
				{
					if (context.User != null && context.User.Identity.IsAuthenticated)
					{
						string name = context.User.Identity.Name;
						if (!string.IsNullOrEmpty(name))
						{
							return name;
						}
					}
				}
				catch (SystemException ex)
				{
					IActivityScope currentActivityScope = ActivityContext.GetCurrentActivityScope();
					if (currentActivityScope != null)
					{
						currentActivityScope.AppendToProperty(ServiceCommonMetadata.GenericErrors, ex.Message);
					}
				}
			}
			return null;
		}

		private static void PublishGeneric(HttpContextBase context)
		{
			IActivityScope currentActivityScope = ServiceCommonMetadataPublisher.GetCurrentActivityScope(context);
			if (currentActivityScope == null)
			{
				return;
			}
			ServiceCommonMetadataPublisher.PublishServerInfo(currentActivityScope);
			if (context == null)
			{
				return;
			}
			HttpRequestBase request = context.Request;
			HttpResponseBase response = context.Response;
			string value = request.GetRequestHeader("X-Forwarded-For");
			if (string.IsNullOrEmpty(value))
			{
				value = request.UserHostAddress;
			}
			currentActivityScope.SetProperty(ServiceCommonMetadata.ClientIpAddress, value);
			currentActivityScope.SetProperty(ActivityStandardMetadata.ClientInfo, request.UserAgent);
			currentActivityScope.SetProperty(ServiceCommonMetadata.RequestSize, request.ContentLength.ToString());
			if (currentActivityScope.GetProperty(ServiceCommonMetadata.HttpStatus) == null)
			{
				currentActivityScope.SetProperty(ServiceCommonMetadata.HttpStatus, response.StatusCode.ToString());
			}
			if (request.Cookies.Count > 0)
			{
				for (int i = 0; i < request.Cookies.Count; i++)
				{
					if (string.Equals(request.Cookies[i].Name, "exchangecookie", StringComparison.OrdinalIgnoreCase))
					{
						currentActivityScope.SetProperty(ServiceCommonMetadata.Cookie, request.Cookies[i].Value);
						return;
					}
				}
			}
		}

		private static void PublishAuthData(HttpContextBase context)
		{
			if (context == null)
			{
				return;
			}
			IActivityScope currentActivityScope = ServiceCommonMetadataPublisher.GetCurrentActivityScope(context);
			if (currentActivityScope == null)
			{
				return;
			}
			Dictionary<Enum, string> authValues = context.GetAuthValues();
			foreach (KeyValuePair<Enum, string> keyValuePair in authValues)
			{
				currentActivityScope.SetProperty(keyValuePair.Key, keyValuePair.Value);
			}
		}

		private static IActivityScope GetCurrentActivityScope(HttpContextBase context)
		{
			IActivityScope activityScope = null;
			if (context != null)
			{
				activityScope = (context.Items[typeof(ActivityScope)] as IActivityScope);
			}
			if (activityScope == null)
			{
				activityScope = ActivityContext.GetCurrentActivityScope();
			}
			return activityScope;
		}

		private static string GetContextItem(this IDictionary items, string key)
		{
			if (items != null)
			{
				object obj = items[key];
				if (obj != null)
				{
					return obj.ToString();
				}
			}
			return null;
		}

		private static string GetRequestHeader(this HttpRequestBase request, string key)
		{
			if (request != null)
			{
				try
				{
					return request.Headers[key];
				}
				catch (SystemException ex)
				{
					IActivityScope currentActivityScope = ActivityContext.GetCurrentActivityScope();
					if (currentActivityScope != null)
					{
						currentActivityScope.AppendToProperty(ServiceCommonMetadata.GenericErrors, ex.Message);
					}
				}
			}
			return null;
		}

		private static void SetNonNullValue(this Dictionary<Enum, string> dictionary, Enum key, string value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				dictionary.Add(key, value);
			}
		}

		public const string DebugHeaderPrefix = "X-DEBUG";

		private const string TrackingCookieName = "exchangecookie";

		private const string OriginatingClientIpHeader = "X-Forwarded-For";

		private static readonly string VersionMajor = 15.ToString();

		private static readonly string VersionMinor = 0.ToString();

		private static readonly string VersionBuild = 1497.ToString();

		private static readonly string VersionRevision = 15.ToString();

		private static readonly string MachineName = Environment.MachineName;

		private static bool isServiceCommonMetadataRegistered = false;
	}
}
