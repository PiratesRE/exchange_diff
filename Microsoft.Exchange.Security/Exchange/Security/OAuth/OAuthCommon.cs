using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.Security.OAuth
{
	internal static class OAuthCommon
	{
		static OAuthCommon()
		{
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				OAuthCommon.processName = currentProcess.ProcessName;
				if (string.Equals(currentProcess.ProcessName, OAuthCommon.IISWorkerProcessName, StringComparison.OrdinalIgnoreCase) && currentProcess.StartInfo != null && currentProcess.StartInfo.EnvironmentVariables != null && currentProcess.StartInfo.EnvironmentVariables.ContainsKey(OAuthCommon.AppPoolId))
				{
					OAuthCommon.appPoolName = currentProcess.StartInfo.EnvironmentVariables[OAuthCommon.AppPoolId];
				}
			}
			string text = OAuthCommon.processName;
			if (!string.IsNullOrEmpty(OAuthCommon.appPoolName))
			{
				text = text + "_" + OAuthCommon.appPoolName;
			}
			OAuthCommon.counters = OAuthCounters.GetInstance(text);
			ExPerformanceCounter[] array = new ExPerformanceCounter[]
			{
				OAuthCommon.counters.NumberOfAuthRequests,
				OAuthCommon.counters.NumberOfFailedAuthRequests,
				OAuthCommon.counters.AverageResponseTime,
				OAuthCommon.counters.AverageAuthServerResponseTime,
				OAuthCommon.counters.NumberOfAuthServerTokenRequests,
				OAuthCommon.counters.NumberOfPendingAuthServerRequests,
				OAuthCommon.counters.AuthServerTokenCacheSize,
				OAuthCommon.counters.NumberOfAuthServerTimeoutTokenRequests
			};
			foreach (ExPerformanceCounter exPerformanceCounter in array)
			{
				exPerformanceCounter.RawValue = 0L;
			}
			OAuthCommon.latencycounterToRunningAverageFloatMap = new Dictionary<string, RunningAverageFloat>
			{
				{
					OAuthCommon.counters.AverageResponseTime.CounterName,
					new RunningAverageFloat(200)
				},
				{
					OAuthCommon.counters.AverageAuthServerResponseTime.CounterName,
					new RunningAverageFloat(200)
				}
			};
			OAuthCommon.eventLogger = new ExEventLog(ExTraceGlobals.AuthenticationTracer.Category, OAuthCommon.OAuthComponent);
		}

		public static string OAuthComponent
		{
			get
			{
				return OAuthCommon.componentName;
			}
		}

		public static ExEventLog EventLogger
		{
			get
			{
				return OAuthCommon.eventLogger;
			}
		}

		public static OAuthCountersInstance PerfCounters
		{
			get
			{
				return OAuthCommon.counters;
			}
		}

		public static string CurrentAppPoolName
		{
			get
			{
				return OAuthCommon.appPoolName;
			}
		}

		public static void UpdateMovingAveragePerformanceCounter(ExPerformanceCounter performanceCounter, long newValue)
		{
			string counterName = performanceCounter.CounterName;
			lock (performanceCounter)
			{
				OAuthCommon.latencycounterToRunningAverageFloatMap[counterName].Update((float)newValue);
				performanceCounter.RawValue = (long)OAuthCommon.latencycounterToRunningAverageFloatMap[counterName].Value;
			}
		}

		public static bool IsIdMatch(string id1, string id2)
		{
			if (string.IsNullOrEmpty(id1))
			{
				throw new ArgumentNullException("id1");
			}
			if (string.IsNullOrEmpty(id2))
			{
				throw new ArgumentNullException("id2");
			}
			return string.Equals(id1, id2, StringComparison.OrdinalIgnoreCase);
		}

		public static bool IsRealmMatch(string realm1, string realm2)
		{
			return string.Equals(realm1, realm2, StringComparison.OrdinalIgnoreCase);
		}

		public static bool IsRealmMatchIncludingEmpty(string realm1, string realm2)
		{
			return (OAuthCommon.IsRealmEmpty(realm1) && OAuthCommon.IsRealmEmpty(realm2)) || OAuthCommon.IsRealmMatch(realm1, realm2);
		}

		public static bool IsRealmEmpty(string realm)
		{
			return string.IsNullOrEmpty(realm) || string.Equals(realm, "*", StringComparison.OrdinalIgnoreCase);
		}

		public static object VerifyNonNullArgument(string name, object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException(name);
			}
			string text = value as string;
			if (text != null && string.IsNullOrEmpty(text))
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "{0} cannot be an empty string", new object[]
				{
					name
				}), name);
			}
			return value;
		}

		public static string SerializeToJson(this object value)
		{
			JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
			return javaScriptSerializer.Serialize(value);
		}

		public static T DeserializeFromJson<T>(this string value)
		{
			JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
			return javaScriptSerializer.Deserialize<T>(value);
		}

		public static string GetReadableTokenString(string token)
		{
			if (string.IsNullOrEmpty(token))
			{
				return token;
			}
			string result;
			try
			{
				string[] array = token.Split(new char[]
				{
					'.'
				});
				string arg = array[0];
				string arg2 = array[1];
				Dictionary<string, object> value = OAuthCommon.Base64UrlEncoder.Decode(arg).DeserializeFromJson<Dictionary<string, object>>();
				Dictionary<string, object> dictionary = OAuthCommon.Base64UrlEncoder.Decode(arg2).DeserializeFromJson<Dictionary<string, object>>();
				object obj = null;
				if (dictionary.TryGetValue(Constants.ClaimTypes.ActorToken, out obj))
				{
					dictionary[Constants.ClaimTypes.ActorToken] = "...";
				}
				string text = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", new object[]
				{
					value.SerializeToJson(),
					dictionary.SerializeToJson()
				});
				if (obj == null)
				{
					result = text;
				}
				else
				{
					result = string.Format("{0}; actor: {1}", text, OAuthCommon.GetReadableTokenString(obj as string));
				}
			}
			catch
			{
				result = token;
			}
			return result;
		}

		public static bool TryGetClaimValue(JwtSecurityToken token, string claimType, out string claimValue)
		{
			claimValue = OAuthCommon.TryGetClaimValue(token.Payload, claimType);
			return claimValue != null;
		}

		public static string TryGetClaimValue(JwtPayload payload, string claimType)
		{
			object obj;
			payload.TryGetValue(claimType, out obj);
			return obj as string;
		}

		public static string GetLoggableTokenString(string rawToken, JwtSecurityToken token)
		{
			string empty = string.Empty;
			if (token == null || OAuthCommon.TryGetClaimValue(token, Constants.ClaimTypes.ActorToken, out empty))
			{
				return OAuthCommon.GetReadableTokenString(rawToken);
			}
			return token.ToString();
		}

		public static string WriteAuthorizationHeader(string token)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[]
			{
				Constants.BearerAuthenticationType,
				token
			});
		}

		internal static string DefaultAcceptedDomain
		{
			get
			{
				if (OAuthCommon.defaultAcceptedDomain == null)
				{
					OAuthCommon.defaultAcceptedDomain = OAuthConfigHelper.GetOrganizationRealm(OrganizationId.ForestWideOrgId);
				}
				return OAuthCommon.defaultAcceptedDomain;
			}
		}

		internal static OrganizationId ResolveOrganizationByDomain(string domain)
		{
			SmtpDomain smtpDomain;
			if (AuthCommon.IsMultiTenancyEnabled)
			{
				if (SmtpDomain.TryParse(domain, out smtpDomain))
				{
					return DomainToOrganizationIdCache.Singleton.Get(smtpDomain);
				}
			}
			else if (SmtpDomain.TryParse(domain, out smtpDomain) && OrganizationId.ForestWideOrgId == DomainToOrganizationIdCache.Singleton.Get(smtpDomain))
			{
				return OrganizationId.ForestWideOrgId;
			}
			return null;
		}

		private const int LatencyCounterNumberOfSamples = 200;

		private static readonly string componentName = "MSExchange OAuth";

		private static readonly string IISWorkerProcessName = "w3wp";

		private static readonly string AppPoolId = "APP_POOL_ID";

		private static OAuthCountersInstance counters;

		private static ExEventLog eventLogger;

		private static readonly Dictionary<string, RunningAverageFloat> latencycounterToRunningAverageFloatMap;

		private static string defaultAcceptedDomain;

		private static string processName;

		private static string appPoolName;

		public static class Base64UrlEncoder
		{
			public static string EncodeBytes(byte[] array)
			{
				return string.Concat<char>(Convert.ToBase64String(array).TakeWhile((char c) => c != OAuthCommon.Base64UrlEncoder.Base64PadCharacter).Select(delegate(char c)
				{
					if (c == OAuthCommon.Base64UrlEncoder.Base64Character62)
					{
						return OAuthCommon.Base64UrlEncoder.Base64UrlCharacter62;
					}
					if (c != OAuthCommon.Base64UrlEncoder.Base64Character63)
					{
						return c;
					}
					return OAuthCommon.Base64UrlEncoder.Base64UrlCharacter63;
				}));
			}

			public static byte[] DecodeBytes(string arg)
			{
				string text = arg.Replace(OAuthCommon.Base64UrlEncoder.Base64UrlCharacter62, OAuthCommon.Base64UrlEncoder.Base64Character62);
				text = text.Replace(OAuthCommon.Base64UrlEncoder.Base64UrlCharacter63, OAuthCommon.Base64UrlEncoder.Base64Character63);
				switch (text.Length % 4)
				{
				case 0:
					goto IL_72;
				case 2:
					text += OAuthCommon.Base64UrlEncoder.DoubleBase64PadCharacter;
					goto IL_72;
				case 3:
					text += OAuthCommon.Base64UrlEncoder.Base64PadCharacter;
					goto IL_72;
				}
				throw new ArgumentException("Illegal base64url string!", arg);
				IL_72:
				return Convert.FromBase64String(text);
			}

			public static string Decode(string arg)
			{
				return Encoding.UTF8.GetString(OAuthCommon.Base64UrlEncoder.DecodeBytes(arg));
			}

			private static char Base64PadCharacter = '=';

			private static string DoubleBase64PadCharacter = string.Format(CultureInfo.InvariantCulture, "{0}{0}", new object[]
			{
				OAuthCommon.Base64UrlEncoder.Base64PadCharacter
			});

			private static char Base64Character62 = '+';

			private static char Base64Character63 = '/';

			private static char Base64UrlCharacter62 = '-';

			private static char Base64UrlCharacter63 = '_';
		}
	}
}
