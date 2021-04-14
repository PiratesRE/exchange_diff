using System;
using System.Diagnostics;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Win32;

namespace Microsoft.Exchange.Security.Authentication.FederatedAuthService
{
	internal class AuthServiceHelper
	{
		internal static ExEventLog EventLogger
		{
			get
			{
				return AuthServiceHelper.eventLogger;
			}
		}

		internal static LiveIdBasicAuthenticationCountersInstance PerformanceCounters
		{
			get
			{
				return AuthServiceHelper.counters;
			}
		}

		internal static bool IsMailboxRole
		{
			get
			{
				if (AuthServiceHelper.isMailboxRole == null)
				{
					lock (AuthServiceHelper.lockObj)
					{
						if (AuthServiceHelper.isMailboxRole == null)
						{
							using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\MailboxRole"))
							{
								AuthServiceHelper.isMailboxRole = new bool?(registryKey != null);
							}
						}
					}
				}
				return AuthServiceHelper.isMailboxRole.Value;
			}
		}

		public static void InvalidateDuplicateUPNs(ITenantRecipientSession ads, ADRawEntry validEntry, ADRawEntry[] entries, LogWarning logWarning)
		{
			string text = validEntry[ADUserSchema.UserPrincipalName].ToString();
			SmtpAddress smtpAddress = SmtpAddress.Parse(text);
			foreach (ADRawEntry adrawEntry in entries)
			{
				if (!ADObjectId.Equals(validEntry.Id, adrawEntry.Id) && string.Equals(text, adrawEntry[ADUserSchema.UserPrincipalName].ToString(), StringComparison.OrdinalIgnoreCase))
				{
					logWarning("Entry {0} has duplicate UPN {1}", new object[]
					{
						adrawEntry.Id,
						text
					});
					string text2 = string.Format("{0}@{1}", ((Guid)adrawEntry[ADObjectSchema.Guid]).ToString("N"), smtpAddress.Domain);
					ADUser aduser = (ADUser)ads.Read(adrawEntry.Id);
					if (aduser == null)
					{
						logWarning("Cannot find entry {0} using passed ADSession", new object[]
						{
							adrawEntry.Id
						});
					}
					else
					{
						aduser.UserPrincipalName = text2;
						logWarning("Entry {0} has duplicate UPN {1} setting to {2}", new object[]
						{
							adrawEntry.Id,
							text,
							text2
						});
						ads.Save(aduser);
					}
				}
			}
		}

		public static string GetAuthType(string authorizationHeader)
		{
			string result;
			if (string.IsNullOrEmpty(authorizationHeader))
			{
				result = "Anonymous";
			}
			else
			{
				int num = authorizationHeader.IndexOf(" ");
				if (num == -1)
				{
					result = "Unknown";
				}
				else
				{
					string text = authorizationHeader.Substring(0, num);
					string s = authorizationHeader.Substring(num + 1);
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append(text);
					if (string.CompareOrdinal(text, "Nego2") == 0 || string.CompareOrdinal(text, "Negotiate") == 0)
					{
						byte[] array = null;
						try
						{
							array = Convert.FromBase64String(s);
						}
						catch (FormatException)
						{
						}
						if (array != null)
						{
							if (AuthServiceHelper.IsPatternInArray(array, AuthServiceHelper.NegoextsBinaryArray))
							{
								stringBuilder.Append("+NegoEx");
							}
							if (AuthServiceHelper.IsPatternInArray(array, AuthServiceHelper.NtlmBinaryArray))
							{
								stringBuilder.Append("+Ntlm");
							}
							if (AuthServiceHelper.IsPatternInArray(array, AuthServiceHelper.WlidtokenBinaryArray))
							{
								stringBuilder.Append("+LiveId");
							}
						}
						result = stringBuilder.ToString();
					}
					else
					{
						result = text;
					}
				}
			}
			return result;
		}

		private static bool IsPatternInArray(byte[] array, byte[] pattern)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (pattern == null)
			{
				throw new ArgumentNullException("pattern");
			}
			if (pattern.Length <= 0)
			{
				throw new ArgumentException("Pattern must not be empty");
			}
			for (int i = 0; i < array.Length - pattern.Length + 1; i++)
			{
				bool flag = false;
				for (int j = 0; j < pattern.Length; j++)
				{
					if (pattern[j] != array[i + j])
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return true;
				}
			}
			return false;
		}

		public static long ExecuteAndRecordLatency(Action action)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			action();
			stopwatch.Stop();
			return stopwatch.ElapsedMilliseconds;
		}

		public static bool IsTenantInAccountForest(string acceptedDomain, IActivityScope scope)
		{
			bool result;
			try
			{
				using (new ActivityScopeThreadGuard(scope))
				{
					PartitionId partitionIdByAcceptedDomainName = ADAccountPartitionLocator.GetPartitionIdByAcceptedDomainName(acceptedDomain);
					string resourceForestFqdnByAcceptedDomainName = ADAccountPartitionLocator.GetResourceForestFqdnByAcceptedDomainName(acceptedDomain);
					result = (!string.IsNullOrEmpty(partitionIdByAcceptedDomainName.ForestFQDN) && !string.Equals(resourceForestFqdnByAcceptedDomainName, partitionIdByAcceptedDomainName.ForestFQDN, StringComparison.OrdinalIgnoreCase) && !string.Equals(PartitionId.LocalForest.ForestFQDN, partitionIdByAcceptedDomainName.ForestFQDN, StringComparison.OrdinalIgnoreCase) && !partitionIdByAcceptedDomainName.ForestFQDN.EndsWith(resourceForestFqdnByAcceptedDomainName, StringComparison.OrdinalIgnoreCase));
				}
			}
			catch (CannotResolveTenantNameException ex)
			{
				result = true;
				AuthServiceHelper.eventLogger.LogEvent(SecurityEventLogConstants.Tuple_GeneralException, acceptedDomain, new object[]
				{
					acceptedDomain,
					ex.ToString()
				});
			}
			return result;
		}

		public static string GetImplicitUpn(ADRawEntry entry)
		{
			OrganizationId organizationId = (OrganizationId)entry[ADObjectSchema.OrganizationId];
			string arg = (string)entry[ADMailboxRecipientSchema.SamAccountName];
			return string.Format("{0}@{1}", arg, organizationId.PartitionId.ForestFQDN);
		}

		public static HttpWebRequest CreateHttpWebRequest(string uri)
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
			httpWebRequest.Timeout = 1000 * AuthServiceStaticConfig.Config.LiveIdStsTimeoutInSeconds;
			httpWebRequest.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(AuthService.CertificateValidationCallBack);
			httpWebRequest.ServicePoint.Expect100Continue = false;
			httpWebRequest.Method = "POST";
			return httpWebRequest;
		}

		public static void UpdateConnectionSettingsInRequest(ref HttpWebRequest request, string connectionGroupName)
		{
			request.KeepAlive = true;
			request.ServicePoint.ConnectionLeaseTimeout = 1000 * AuthServiceStaticConfig.Config.ConnectionLeaseTimeoutInSeconds;
			request.ConnectionGroupName = connectionGroupName;
		}

		public static bool CloseConnectionGroupIfNeeded(bool closeConnectionGroup, string uri, string connectionGroupName, int traceId)
		{
			if (AuthServiceStaticConfig.Config.MsoSSLEndpointType != MsoEndpointType.OLD && closeConnectionGroup)
			{
				ServicePoint servicePoint = ServicePointManager.FindServicePoint(new Uri(uri));
				if (servicePoint != null)
				{
					ExTraceGlobals.AuthenticationTracer.Information<string>((long)traceId, "Closing connection group '{0}'", connectionGroupName);
					return servicePoint.CloseConnectionGroup(connectionGroupName);
				}
			}
			return false;
		}

		public static string GetConnectionGroup(int traceId)
		{
			int num = (AuthServiceStaticConfig.Config.MaxConnectionGroups > 0) ? (Thread.CurrentThread.ManagedThreadId % AuthServiceStaticConfig.Config.MaxConnectionGroups) : Thread.CurrentThread.ManagedThreadId;
			string text = AuthServiceStaticConfig.Config.ConnectionGroupPrefix + num;
			ExTraceGlobals.AuthenticationTracer.Information<string>((long)traceId, "ConnectionGroupName = '{0}'", text);
			return text;
		}

		public static bool IsOutlookComUser(string userName)
		{
			bool flag = ConsumerIdentityHelper.IsConsumerMailbox(userName);
			if (!flag && AuthServiceStaticConfig.Config.outlookComRegex != null)
			{
				flag = AuthServiceStaticConfig.Config.outlookComRegex.IsMatch(userName);
			}
			return flag;
		}

		public static UserType GetUserType(DomainConfig domainConfig)
		{
			if (domainConfig.IsOutlookCom)
			{
				return UserType.OutlookCom;
			}
			if (domainConfig.IsFederated)
			{
				return UserType.Federated;
			}
			if (domainConfig.Instance != LiveIdInstanceType.Business)
			{
				return UserType.ManagedConsumer;
			}
			return UserType.ManagedBusiness;
		}

		public const string LiveIdComponent = "MSExchange LiveIdBasicAuthentication";

		internal const string AuthenticatedByOfflineOrgId = "AuthenticatedBy:OfflineOrgId.";

		internal const string AuthenticatedByCache = "AuthenticatedBy:Cache.";

		internal const string FederatedTag = "<FEDERATED>";

		internal const string UserTypeTag = "<UserType:{0}>";

		internal const string CheckPwdConfidenceTag = "CheckPwdConfidence";

		internal const string GetLastLogonTimeFromMailboxTag = "GetLastLogonTimeFromMailbox";

		internal static readonly byte[] NegoextsBinaryArray = new byte[]
		{
			78,
			69,
			71,
			79,
			69,
			88,
			84,
			83
		};

		internal static readonly byte[] NtlmBinaryArray = new byte[]
		{
			78,
			84,
			76,
			77,
			83,
			83,
			80
		};

		internal static readonly byte[] WlidtokenBinaryArray = new byte[]
		{
			87,
			108,
			105,
			100,
			84,
			111,
			107,
			101,
			110
		};

		private static readonly ExEventLog eventLogger = new ExEventLog(ExTraceGlobals.AuthenticationTracer.Category, "MSExchange LiveIdBasicAuthentication");

		private static readonly LiveIdBasicAuthenticationCountersInstance counters = LiveIdBasicAuthenticationCounters.GetInstance(Process.GetCurrentProcess().ProcessName);

		private static bool? isMailboxRole = null;

		private static object lockObj = new object();
	}
}
