using System;
using System.Runtime.InteropServices;
using System.Web;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Passport.RPS;

namespace Microsoft.Exchange.Security.Authentication
{
	public class RPSCommon
	{
		private static RPSProfile.UserInfo ParseUserInfoFromRPSTicket(RPSTicket rpsTicket)
		{
			RPSProfile.UserInfo result;
			try
			{
				RPSProfile.UserInfo userInfo = new RPSProfile.UserInfo();
				userInfo.Gender = (rpsTicket.ProfileProperty["gender"] as string);
				userInfo.Occupation = (rpsTicket.ProfileProperty["occupation"] as string);
				object obj = rpsTicket.ProfileProperty["region"];
				if (obj != null)
				{
					userInfo.Region = (int)obj;
				}
				object obj2 = rpsTicket.ProfileProperty["timezone"];
				if (obj2 != null)
				{
					userInfo.TimeZone = (short)obj2;
				}
				object obj3 = rpsTicket.ProfileProperty["birthdate"];
				if (obj3 != null)
				{
					userInfo.Birthday = (DateTime)obj3;
				}
				object obj4 = rpsTicket.Property["AliasVersion"];
				if (obj4 != null)
				{
					userInfo.AliasVersion = (uint)obj4;
				}
				userInfo.PostalCode = (rpsTicket.ProfileProperty["postalCode"] as string);
				userInfo.FirstName = (rpsTicket.ProfileProperty["Firstname"] as string);
				userInfo.LastName = (rpsTicket.ProfileProperty["Lastname"] as string);
				object obj5 = rpsTicket.ProfileProperty["lang_preference"];
				if (obj5 != null)
				{
					userInfo.Language = (short)obj5;
				}
				userInfo.Country = (rpsTicket.ProfileProperty["country"] as string);
				result = userInfo;
			}
			catch (InvalidCastException ex)
			{
				ExTraceGlobals.AuthenticationTracer.TraceError((long)rpsTicket.GetHashCode(), ex.ToString());
				result = null;
			}
			return result;
		}

		public static RPSProfile ParseRPSTicket(RPSTicket rpsTicket, int rpsTicketLifetime, int traceId, bool basicAuth, out string errorString, bool parseUserProfile = false)
		{
			errorString = null;
			DateTime dateTime = DateTime.MinValue;
			uint num = 0U;
			uint num2 = uint.MaxValue;
			uint num3 = 0U;
			bool appPassword = false;
			uint consumerChild = 0U;
			byte reputationByte = 0;
			uint issueInstant = 0U;
			uint loginAttributes = 0U;
			string text = (string)rpsTicket.Property["hexPuid"];
			ExTraceGlobals.AuthenticationTracer.TraceDebug<string>((long)traceId, "ticket has hexPuid = {0}", text);
			int num4 = (int)rpsTicket.ProfileProperty["flags"];
			ExTraceGlobals.AuthenticationTracer.TraceDebug<int>((long)traceId, "ticket has flags = 0x{0:x}", num4);
			if (rpsTicket.Property["AuthFlags"] != null)
			{
				num3 = (uint)rpsTicket.Property["AuthFlags"];
			}
			ExTraceGlobals.AuthenticationTracer.TraceDebug<uint>((long)traceId, "ticket has AuthFlags = {0}", num3);
			if (rpsTicket.Property["PasswordExpiryDate"] != null)
			{
				num2 = (uint)rpsTicket.Property["PasswordExpiryDate"];
			}
			ExTraceGlobals.AuthenticationTracer.TraceDebug<uint>((long)traceId, "ticket has PasswordExpiryDate = {0}", num2);
			if (rpsTicket.Property["AuthInstant"] != null)
			{
				num = (uint)rpsTicket.Property["AuthInstant"];
				dateTime = RPSCommon.refTime.Add(TimeSpan.FromSeconds(num));
			}
			ExTraceGlobals.AuthenticationTracer.TraceDebug<DateTime>((long)traceId, "ticket has authInstant = {0}", dateTime);
			string text2 = (string)rpsTicket.Property["ConsumerPUID"];
			ExTraceGlobals.AuthenticationTracer.TraceDebug<string>((long)traceId, "ticket has ConsumerPUID = {0}", text2);
			string text3 = rpsTicket.Property["ConsumerCID"] as string;
			ExTraceGlobals.AuthenticationTracer.TraceDebug<string>((long)traceId, "ticket has ConsumerCID = {0}", text3);
			object obj = rpsTicket.Property["ConsumerTOUAccepted"];
			ExTraceGlobals.AuthenticationTracer.TraceDebug((long)traceId, "ticket has ConsumerTOUAccepted = {0}", new object[]
			{
				obj
			});
			object obj2 = rpsTicket.Property["ConsumerChild"];
			ExTraceGlobals.AuthenticationTracer.TraceDebug((long)traceId, "ticket has ConsumerChild = {0}", new object[]
			{
				obj2
			});
			string text4 = Convert.ToString(rpsTicket.Property["ConsumerConsentLevel"]);
			ExTraceGlobals.AuthenticationTracer.TraceDebug<string>((long)traceId, "ticket has ConsumerConsentLevel = {0}", text4);
			string text5 = (string)rpsTicket.Property["PasswordExpiryUrl"];
			ExTraceGlobals.AuthenticationTracer.TraceDebug<string>((long)traceId, "ticket has PasswordExpiryUrl = {0}", text5);
			string text6 = (string)rpsTicket.Property["AuthNMethodsReferences"];
			ExTraceGlobals.AuthenticationTracer.TraceDebug<string>((long)traceId, "ticket has AuthNMethodsReferences = {0}", text6);
			string text7 = (string)rpsTicket.Property["Membername"];
			if (!string.IsNullOrEmpty(text7))
			{
				text7 = HttpUtility.UrlDecode(text7);
			}
			ExTraceGlobals.AuthenticationTracer.TraceDebug<string>((long)traceId, "ticket has Membername = {0}", text7);
			if (basicAuth && dateTime != DateTime.MinValue && rpsTicketLifetime > 0 && DateTime.UtcNow > dateTime.Add(TimeSpan.FromSeconds((double)rpsTicketLifetime)))
			{
				errorString = string.Format("ticket expired AuthTime={0}", dateTime);
				return null;
			}
			object obj3 = rpsTicket.Property["ReputationByte_1"];
			if (obj3 != null)
			{
				reputationByte = Convert.ToByte(obj3);
			}
			object obj4 = rpsTicket.Property["IssueInstant"];
			if (obj4 != null)
			{
				issueInstant = (uint)obj4;
			}
			string hexCID;
			if (string.IsNullOrEmpty(text3))
			{
				hexCID = (rpsTicket.Property["HexCID"] as string);
			}
			else
			{
				hexCID = text3;
			}
			object obj5 = rpsTicket.Property["LoginAttributes"];
			if (obj5 != null)
			{
				loginAttributes = (uint)obj5;
			}
			if (!string.IsNullOrEmpty(text2))
			{
				num4 |= 536887296;
				num4 &= -225;
				if (obj != null && (uint)obj != 0U)
				{
					num4 &= -536870913;
				}
				if (obj2 != null && (uint)obj2 != 0U)
				{
					num4 |= 128;
					consumerChild = (uint)obj2;
				}
				if (!string.IsNullOrEmpty(text4) && !text4.Equals("NONE", StringComparison.OrdinalIgnoreCase))
				{
					num4 |= 32;
				}
			}
			if (text6 != null && text6.Equals("http://schemas.microsoft.com/claims/apppassword", StringComparison.OrdinalIgnoreCase))
			{
				appPassword = true;
			}
			RPSProfile.UserInfo profile = null;
			if (parseUserProfile)
			{
				profile = RPSCommon.ParseUserInfoFromRPSTicket(rpsTicket);
			}
			return new RPSProfile
			{
				AuthFlags = num3,
				TokenFlags = num4,
				MemberName = text7,
				CredsExpireIn = num2,
				HexPuid = text,
				ConsumerPuid = text2,
				RecoveryUrl = text5,
				AppPassword = appPassword,
				HasSignedTOU = RPSCommon.HasUserSignedTOU(num4, text7),
				HexCID = hexCID,
				IssueInstant = issueInstant,
				AuthInstant = num,
				LoginAttributes = loginAttributes,
				ConsumerCID = text3,
				TicketType = rpsTicket.TicketType,
				ConsumerConsentLevel = text4,
				ConsumerChild = consumerChild,
				ReputationByte = reputationByte,
				Profile = profile
			};
		}

		public static bool HasUserSignedTOU(int flags, string traceUserName)
		{
			bool flag = (flags & 536870912) == 0 || (flags & 16384) == 0;
			bool flag2 = (flags & 128) != 0;
			bool flag3 = (flags & 32) != 0;
			bool flag4 = (flags & 64) != 0;
			ExTraceGlobals.AuthenticationTracer.TraceDebug<string, string, int>(0L, "user {0} {1} accepted TOU. flags = 0x{2:x}", traceUserName, flag ? "has" : "has NOT", flags & 536887296);
			ExTraceGlobals.AuthenticationTracer.TraceDebug<string, string, int>(0L, "user {0} {1} a child.  flags = 0x{2:x}", traceUserName, flag2 ? "is" : "is NOT", flags & 224);
			ExTraceGlobals.AuthenticationTracer.TraceDebug<string, string, int>(0L, "user {0} {1} limited parental consent.  flags = 0x{2:x}", traceUserName, flag3 ? "has" : "does NOT have", flags & 224);
			ExTraceGlobals.AuthenticationTracer.TraceDebug<string, string, int>(0L, "user {0} {1} full parental consent.  flags = 0x{2:x}", traceUserName, flag4 ? "has" : "does NOT have", flags & 224);
			bool flag5 = flag && (!flag2 || flag3 || flag4);
			ExTraceGlobals.AuthenticationTracer.TraceDebug<string, string>(0L, "TOU check {0} for user {1}", flag5 ? "passed" : "failed", traceUserName);
			return flag5;
		}

		public static string GetRedirectUrl(RPS rpsSession, string constructUrlParam, string siteName, string formattedReturnUrl, string authPolicy, out int? error, out string errorString)
		{
			error = null;
			errorString = null;
			string result = null;
			try
			{
				using (RPSDomainMap rpsdomainMap = new RPSDomainMap(rpsSession))
				{
					using (RPSPropBag rpspropBag = new RPSPropBag(rpsSession))
					{
						if (!string.IsNullOrEmpty(authPolicy))
						{
							rpspropBag["AuthPolicy"] = authPolicy;
						}
						if (!string.IsNullOrEmpty(formattedReturnUrl))
						{
							rpspropBag["ReturnURL"] = formattedReturnUrl;
						}
						result = rpsdomainMap.ConstructURL(constructUrlParam, siteName, null, rpspropBag);
					}
				}
			}
			catch (COMException ex)
			{
				error = new int?(ex.ErrorCode);
				errorString = ex.Message;
			}
			return result;
		}

		public static string GetSiteProperty(RPS rpsSession, string siteName, string siteProperty)
		{
			string result = null;
			using (RPSPropBag rpspropBag = new RPSServerConfig(rpsSession))
			{
				using (RPSPropBag rpspropBag2 = rpspropBag["sites"] as RPSPropBag)
				{
					if (rpspropBag2 != null)
					{
						using (RPSPropBag rpspropBag3 = rpspropBag2[siteName] as RPSPropBag)
						{
							if (rpspropBag3 != null)
							{
								result = (string)rpspropBag3[siteProperty];
							}
						}
					}
				}
			}
			return result;
		}

		public static string GetRPSEnvironmentConfig(RPS rpsSession)
		{
			string result = null;
			using (RPSPropBag rpspropBag = new RPSServerConfig(rpsSession))
			{
				string text = string.Empty;
				using (RPSPropBag rpspropBag2 = rpspropBag["NetworkServices"] as RPSPropBag)
				{
					if (rpspropBag2 != null)
					{
						text = (string)rpspropBag2["Url"];
					}
				}
				if (!string.IsNullOrEmpty(text))
				{
					using (RPSPropBag rpspropBag3 = rpspropBag["Environment"] as RPSPropBag)
					{
						if (rpspropBag3 != null)
						{
							foreach (string text2 in rpspropBag3.Names)
							{
								if (string.Compare((string)rpspropBag3[text2], text, true) == 0)
								{
									result = text2;
									break;
								}
							}
						}
					}
				}
			}
			return result;
		}

		private const int bit30LiveTOU = 536870912;

		private const int bit15MsnTOU = 16384;

		private const int bit6LimitedConsent = 32;

		private const int bit7FullConsent = 64;

		private const int bit8IsChild = 128;

		private const int statusMask = 224;

		internal const string RPSReturnURL = "ReturnURL";

		internal const string RPSAuthPolicy = "AuthPolicy";

		private static DateTime refTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
	}
}
