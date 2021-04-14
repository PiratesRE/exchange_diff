using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore;
using Microsoft.Rtc.Collaboration;
using Microsoft.Rtc.Internal.Utilities;
using Microsoft.Rtc.Signaling;

namespace Microsoft.Exchange.UM.UcmaPlatform
{
	internal class UcmaDiversionInfo : PlatformDiversionInfo
	{
		public UcmaDiversionInfo(string header, string calledParty, string userAtHost, RedirectReason reason, DiversionSource source) : base(header, calledParty, userAtHost, reason, source)
		{
		}

		public static List<PlatformDiversionInfo> CreateDiversionInfoList(DiversionContext diversionContext)
		{
			string header = null;
			string value = null;
			List<PlatformDiversionInfo> result;
			try
			{
				List<PlatformDiversionInfo> list = new List<PlatformDiversionInfo>(2);
				if (diversionContext != null)
				{
					DiversionSource diversionSource = UcmaDiversionInfo.GetDiversionSource(diversionContext);
					Collection<DivertedDestination> allDivertedDestinations = diversionContext.GetAllDivertedDestinations();
					if (allDivertedDestinations != null)
					{
						foreach (DivertedDestination destination in allDivertedDestinations)
						{
							UcmaDiversionInfo.AddDiversion(list, diversionSource, destination, out header, out value);
						}
					}
					if (diversionSource == DiversionSource.HistoryInfo)
					{
						UcmaDiversionInfo.RemoveTrailingHistoryInfoEntries(list);
						UcmaDiversionInfo.RemoveHistoryInfoEntriesWithoutReason(list);
					}
					UcmaDiversionInfo.RemoveDuplicateDiversionInfoEntries(list);
				}
				result = list;
			}
			catch (MessageParsingException)
			{
				throw new InvalidSIPHeaderException("INVITE", header, value);
			}
			return result;
		}

		private static void RemoveHistoryInfoEntriesWithoutReason(List<PlatformDiversionInfo> infoList)
		{
			infoList.RemoveAll((PlatformDiversionInfo o) => o.RedirectReason == RedirectReason.None);
		}

		private static void RemoveTrailingHistoryInfoEntries(List<PlatformDiversionInfo> infoList)
		{
			if (infoList.Count > 0)
			{
				int i = infoList.Count - 1;
				string originalCalledParty = infoList[i].OriginalCalledParty;
				while (i >= 0)
				{
					string originalCalledParty2 = infoList[i].OriginalCalledParty;
					if (!string.Equals(originalCalledParty, originalCalledParty2, StringComparison.OrdinalIgnoreCase))
					{
						break;
					}
					infoList.RemoveAt(i);
					i--;
				}
			}
		}

		private static void RemoveDuplicateDiversionInfoEntries(List<PlatformDiversionInfo> infoList)
		{
			if (infoList.Count > 0)
			{
				int i = 0;
				Dictionary<string, int> dictionary = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
				while (i < infoList.Count)
				{
					string originalCalledParty = infoList[i].OriginalCalledParty;
					if (dictionary.ContainsKey(originalCalledParty))
					{
						infoList.RemoveAt(i);
					}
					else
					{
						dictionary[originalCalledParty] = 1;
						i++;
					}
				}
			}
		}

		private static void AddDiversion(List<PlatformDiversionInfo> infoList, DiversionSource source, DivertedDestination destination, out string name, out string value)
		{
			name = string.Empty;
			value = string.Empty;
			if (destination != null)
			{
				name = destination.DiversionHeader.Name;
				value = destination.DiversionHeader.GetValue();
				string empty = string.Empty;
				string calledParty = UcmaDiversionInfo.GetCalledParty(destination.Uri, out empty);
				RedirectReason reason = UcmaDiversionInfo.FindDiversionReason(destination.Reason);
				infoList.Add(new PlatformDiversionInfo(value, calledParty, empty, reason, source));
			}
		}

		private static string GetCalledParty(string uri, out string userAtHost)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.UCMATracer, null, "GetCalledParty {0}?", new object[]
			{
				uri
			});
			TelUri telUri = null;
			SipUriParser sipUriParser = null;
			string text = null;
			string text2 = null;
			userAtHost = string.Empty;
			if (!string.IsNullOrEmpty(uri))
			{
				if (TelUri.TryParse(uri, 1, ref telUri, ref text2))
				{
					text = telUri.CleanedNumber;
				}
				else if (SipUriParser.TryParse(uri, ref sipUriParser))
				{
					text = sipUriParser.User;
					userAtHost = sipUriParser.UserAtHost;
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				throw new MessageParsingException();
			}
			return UcmaDiversionInfo.GetExtensionFromUserPart(text);
		}

		private static string GetExtensionFromUserPart(string userPart)
		{
			string text = string.Empty;
			text = userPart;
			if (text.IndexOf(';') != -1)
			{
				string[] array = text.Split(new char[]
				{
					';'
				});
				text = array[0];
			}
			if (text.IndexOf(':') != -1)
			{
				string[] array2 = text.Split(new char[]
				{
					':'
				});
				text = array2[0];
			}
			return text;
		}

		private static DiversionSource GetDiversionSource(DiversionContext context)
		{
			if (context.Source != 1)
			{
				return DiversionSource.Diversion;
			}
			return DiversionSource.HistoryInfo;
		}

		private static RedirectReason FindDiversionReason(string reason)
		{
			if (string.IsNullOrEmpty(reason))
			{
				return RedirectReason.None;
			}
			reason = reason.ToLowerInvariant();
			if (reason.Contains("user-busy"))
			{
				return RedirectReason.UserBusy;
			}
			if (reason.Contains("no-answer"))
			{
				return RedirectReason.NoAnswer;
			}
			if (reason.Contains("unconditional"))
			{
				return RedirectReason.Unconditional;
			}
			if (reason.Contains("deflection"))
			{
				return RedirectReason.Deflection;
			}
			if (reason.Contains("unavailable"))
			{
				return RedirectReason.Unavailable;
			}
			return RedirectReason.Other;
		}

		private const string DiversionReasonName = "reason";

		private const string DiversionReasonUserBusy = "user-busy";

		private const string DiversionReasonNoAnswer = "no-answer";

		private const string DiversionReasonUnconditional = "unconditional";

		private const string DiversionReasonDeflection = "deflection";

		private const string DiversionReasonUnavailable = "unavailable";
	}
}
