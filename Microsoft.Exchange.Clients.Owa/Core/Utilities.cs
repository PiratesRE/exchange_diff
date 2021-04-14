using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Security.AntiXss;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.EventLogs;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Clients.Owa.Core.Internal;
using Microsoft.Exchange.Clients.Owa.Core.Transcoding;
using Microsoft.Exchange.Clients.Owa.Premium;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Clients.Owa2.Server.Core;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.CommonHelpProvider;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.DocumentLibrary;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Search.AqsParser;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Data.TextConverters.Internal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Diagnostics.LatencyDetection;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.InstantMessaging;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Security.RightsManagement;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public static class Utilities
	{
		private static OwaExceptionEventManager StoreConnectionTransientManager
		{
			get
			{
				if (Utilities.storeConnectionTransientManager == null)
				{
					OwaExceptionEventManager value = new OwaExceptionEventManager(Globals.StoreTransientExceptionEventLogFrequencyInSeconds, Globals.StoreTransientExceptionEventLogThreshold);
					Interlocked.CompareExchange<OwaExceptionEventManager>(ref Utilities.storeConnectionTransientManager, value, null);
				}
				return Utilities.storeConnectionTransientManager;
			}
		}

		public static string GetShortServerNameFromFqdn(string fqdn)
		{
			if (fqdn == null)
			{
				return null;
			}
			int num = fqdn.IndexOf(".", StringComparison.InvariantCultureIgnoreCase);
			if (num >= 0)
			{
				fqdn = fqdn.Substring(0, num);
			}
			return fqdn;
		}

		public static string GetSenderSmtpAddress(string itemId, UserContext userContext)
		{
			if (string.IsNullOrEmpty(itemId))
			{
				throw new ArgumentNullException("itemId");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			PropertyDefinition[] prefetchProperties = new PropertyDefinition[]
			{
				MessageItemSchema.SenderAddressType,
				MessageItemSchema.SenderEmailAddress
			};
			string text = string.Empty;
			object obj = null;
			string text2 = string.Empty;
			using (Item item = Utilities.GetItem<Item>(userContext, OwaStoreObjectId.CreateFromString(itemId), prefetchProperties))
			{
				obj = item.TryGetProperty(MessageItemSchema.SenderAddressType);
				if (!(obj is PropertyError))
				{
					text = (string)obj;
				}
				obj = item.TryGetProperty(MessageItemSchema.SenderEmailAddress);
			}
			if (!(obj is PropertyError))
			{
				text2 = (string)obj;
			}
			if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(text2))
			{
				return string.Empty;
			}
			if (string.CompareOrdinal(text, "EX") == 0)
			{
				try
				{
					IRecipientSession recipientSession = Utilities.CreateADRecipientSession(ConsistencyMode.IgnoreInvalid, userContext);
					ADRecipient adrecipient = recipientSession.FindByLegacyExchangeDN(text2);
					userContext.LastRecipientSessionDCServerName = recipientSession.LastUsedDc;
					if (adrecipient != null)
					{
						return adrecipient.PrimarySmtpAddress.ToString();
					}
					goto IL_132;
				}
				catch (NonUniqueRecipientException ex)
				{
					ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "Utilities.GetSenderSmtpAddress: NonUniqueRecipientException was thrown by FindByLegacyExchangeDN: {0}", ex.Message);
					goto IL_132;
				}
			}
			if (string.CompareOrdinal(text, "SMTP") == 0)
			{
				return text2;
			}
			IL_132:
			return string.Empty;
		}

		internal static void RenderSizeWithUnits(TextWriter writer, long bytes, bool roundToWholeNumber)
		{
			Utilities.RenderSizeWithUnits(writer, bytes, roundToWholeNumber, true);
		}

		internal static void RenderSizeWithUnits(TextWriter writer, long bytes, bool roundToWholeNumber, bool requireHtmlEncode)
		{
			Strings.IDs units = Utilities.PreRenderSizeWithUnits(writer, bytes, roundToWholeNumber);
			Utilities.WriteUnits(null, writer, units, requireHtmlEncode);
		}

		internal static void RenderSizeWithUnits(UserContext userContext, TextWriter writer, long bytes, bool roundToWholeNumber)
		{
			Strings.IDs units = Utilities.PreRenderSizeWithUnits(writer, bytes, roundToWholeNumber);
			Utilities.WriteUnits(userContext, writer, units, true);
		}

		private static Strings.IDs PreRenderSizeWithUnits(TextWriter writer, long bytes, bool roundToWholeNumber)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			double num = (double)bytes;
			Strings.IDs result;
			if (bytes >= 1073741824L)
			{
				num /= 1073741824.0;
				result = -470326212;
			}
			else if (bytes >= 1048576L)
			{
				num /= 1048576.0;
				result = -1611859650;
			}
			else if (bytes >= 1024L)
			{
				num /= 1024.0;
				result = 2096762107;
			}
			else
			{
				result = 1954677924;
			}
			if (roundToWholeNumber)
			{
				writer.Write((long)Math.Round(num));
			}
			else
			{
				writer.Write("{0:0.##}", num);
			}
			writer.Write(" ");
			return result;
		}

		private static void WriteUnits(UserContext userContext, TextWriter writer, Strings.IDs units, bool requireHtmlEncode)
		{
			if (!requireHtmlEncode)
			{
				writer.Write(LocalizedStrings.GetNonEncoded(units));
				return;
			}
			if (userContext == null)
			{
				writer.Write(LocalizedStrings.GetHtmlEncoded(units));
				return;
			}
			string name = userContext.UserCulture.Name;
			writer.Write(LocalizedStrings.GetHtmlEncodedFromKey(name, units));
		}

		internal static Dictionary<T, Strings.IDs> CreateEnumLocalizedStringMap<T>(EnumInfo<T>[] enumInfoTable) where T : struct
		{
			Dictionary<T, Strings.IDs> dictionary = new Dictionary<T, Strings.IDs>(enumInfoTable.Length);
			foreach (EnumInfo<T> enumInfo in enumInfoTable)
			{
				dictionary.Add(enumInfo.EnumValue, enumInfo.StringIdValue);
			}
			return dictionary;
		}

		public static void MakePageCacheable(HttpResponse response)
		{
			Utilities.MakePageCacheable(response, null);
		}

		public static void MakePageCacheable(HttpResponse response, int? expireDays)
		{
			if (expireDays == null)
			{
				expireDays = new int?(30);
			}
			DateTime expires = DateTime.UtcNow.Add(new TimeSpan(expireDays.Value, 0, 0, 0, 0)).ToLocalTime();
			response.Cache.SetCacheability(HttpCacheability.Private);
			response.Cache.SetExpires(expires);
		}

		internal static void MakePageNoCacheNoStore(HttpResponse response)
		{
			response.Cache.SetCacheability(HttpCacheability.NoCache);
			response.Cache.SetNoStore();
		}

		public static bool IsHexChar(char c)
		{
			return char.IsDigit(c) || (char.ToUpperInvariant(c) >= 'A' && char.ToUpperInvariant(c) <= 'F');
		}

		internal static bool IsValidApprovalRequest(MessageItem message)
		{
			if (!ObjectClass.IsOfClass(message.ClassName, "IPM.Note.Microsoft.Approval.Request"))
			{
				return false;
			}
			if (Utilities.IsSMime(message))
			{
				return false;
			}
			VotingInfo votingInfo = message.VotingInfo;
			if (votingInfo == null)
			{
				return false;
			}
			string[] array = (string[])votingInfo.GetOptionsList();
			return array != null && array.Length == 2;
		}

		internal static bool IsValidUndecidedApprovalRequest(MessageItem message)
		{
			if (!Utilities.IsValidApprovalRequest(message))
			{
				return false;
			}
			int? valueAsNullable = message.GetValueAsNullable<int>(MessageItemSchema.LastVerbExecuted);
			if (valueAsNullable != null && valueAsNullable.Value >= 1 && valueAsNullable.Value < 100)
			{
				return false;
			}
			int? valueAsNullable2 = message.GetValueAsNullable<int>(MessageItemSchema.ApprovalDecision);
			return valueAsNullable2 == null || valueAsNullable2.Value < 1 || valueAsNullable2.Value >= 100;
		}

		public static bool IsValidGuid(string guid)
		{
			if (guid == null || guid.Length != 32)
			{
				return false;
			}
			for (int i = 0; i < 32; i++)
			{
				if (!Utilities.IsHexChar(guid[i]))
				{
					return false;
				}
			}
			return true;
		}

		internal static bool IsOnBehalfOf(Participant sender, Participant from)
		{
			return !(sender == null) && !string.IsNullOrEmpty(sender.EmailAddress) && !(from == null) && !string.IsNullOrEmpty(from.EmailAddress) && 0 != string.Compare(sender.EmailAddress, from.EmailAddress, StringComparison.OrdinalIgnoreCase);
		}

		public static string GetNewGuid()
		{
			return Guid.NewGuid().ToString("N");
		}

		public static void JavascriptEncode(string s, TextWriter writer)
		{
			Utilities.JavascriptEncode(s, writer, false);
		}

		public static void JavascriptEncode(string s, TextWriter writer, bool escapeNonAscii)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			int i = 0;
			while (i < s.Length)
			{
				char c = s[i];
				if (c <= '"')
				{
					if (c != '\n')
					{
						if (c != '\r')
						{
							switch (c)
							{
							case '!':
							case '"':
								goto IL_78;
							default:
								goto IL_B3;
							}
						}
						else
						{
							writer.Write('\\');
							writer.Write('r');
						}
					}
					else
					{
						writer.Write('\\');
						writer.Write('n');
					}
				}
				else if (c <= '/')
				{
					if (c != '\'' && c != '/')
					{
						goto IL_B3;
					}
					goto IL_78;
				}
				else
				{
					switch (c)
					{
					case '<':
					case '>':
						goto IL_78;
					case '=':
						goto IL_B3;
					default:
						if (c == '\\')
						{
							goto IL_78;
						}
						goto IL_B3;
					}
				}
				IL_E7:
				i++;
				continue;
				IL_78:
				writer.Write('\\');
				writer.Write(s[i]);
				goto IL_E7;
				IL_B3:
				if (escapeNonAscii && s[i] > '\u007f')
				{
					writer.Write("\\u{0:x4}", (ushort)s[i]);
					goto IL_E7;
				}
				writer.Write(s[i]);
				goto IL_E7;
			}
		}

		public static string JavascriptEncode(string s, bool escapeNonAscii)
		{
			if (s == null)
			{
				return string.Empty;
			}
			StringBuilder sb = new StringBuilder();
			string result;
			using (StringWriter stringWriter = new StringWriter(sb))
			{
				Utilities.JavascriptEncode(s, stringWriter, escapeNonAscii);
				result = stringWriter.ToString();
			}
			return result;
		}

		public static SanitizedHtmlString JavascriptEncode(SanitizedHtmlString s, bool escapeNonAscii)
		{
			if (s == null)
			{
				return SanitizedHtmlString.Empty;
			}
			StringBuilder builder = new StringBuilder();
			SanitizedHtmlString result;
			using (SanitizingStringWriter<OwaHtml> sanitizingStringWriter = new SanitizingStringWriter<OwaHtml>(builder))
			{
				Utilities.JavascriptEncode(s.ToString(), sanitizingStringWriter, escapeNonAscii);
				result = sanitizingStringWriter.ToSanitizedString<SanitizedHtmlString>();
			}
			return result;
		}

		public static string JavascriptEncode(string s)
		{
			return Utilities.JavascriptEncode(s, false);
		}

		public static SanitizedHtmlString JavascriptEncode(SanitizedHtmlString s)
		{
			return Utilities.JavascriptEncode(s, false);
		}

		public static void HtmlEncode(string s, TextWriter writer)
		{
			Utilities.HtmlEncode(s, writer, false);
		}

		public static void SanitizeHtmlEncode(string s, TextWriter writer)
		{
			if (writer is SanitizingTextWriter<OwaHtml>)
			{
				writer.Write(s);
				return;
			}
			SanitizedHtmlString sanitizedHtmlString = new SanitizedHtmlString(Utilities.HtmlEncode(s, false));
			sanitizedHtmlString.DecreeToBeTrusted();
			writer.Write(sanitizedHtmlString);
		}

		public static void HtmlEncode(string s, TextWriter writer, bool encodeSpaces)
		{
			if (s == null || s.Length == 0)
			{
				return;
			}
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (encodeSpaces)
			{
				for (int i = 0; i < s.Length; i++)
				{
					if (s[i] == ' ')
					{
						writer.Write("&nbsp;");
					}
					else
					{
						writer.Write(AntiXssEncoder.HtmlEncode(s.Substring(i, 1), false));
					}
				}
				return;
			}
			HttpUtility.HtmlEncode(s, writer);
		}

		public static void SanitizeHtmlEncode(string s, TextWriter writer, bool encodeSpaces)
		{
			if (writer is SanitizingTextWriter<OwaHtml>)
			{
				writer.Write(s);
				return;
			}
			SanitizedHtmlString sanitizedHtmlString = new SanitizedHtmlString(Utilities.HtmlEncode(s, encodeSpaces));
			sanitizedHtmlString.DecreeToBeTrusted();
			writer.Write(sanitizedHtmlString);
		}

		public static string HtmlEncode(string s)
		{
			return AntiXssEncoder.HtmlEncode(s, false);
		}

		public static SanitizedHtmlString SanitizeHtmlEncode(string s)
		{
			SanitizedHtmlString sanitizedHtmlString = new SanitizedHtmlString(AntiXssEncoder.HtmlEncode(s, false));
			sanitizedHtmlString.DecreeToBeTrusted();
			return sanitizedHtmlString;
		}

		public static string HtmlEncode(string s, bool encodeSpaces)
		{
			if (encodeSpaces)
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < s.Length; i++)
				{
					if (s[i] == ' ')
					{
						stringBuilder.Append("&nbsp;");
					}
					else
					{
						stringBuilder.Append(AntiXssEncoder.HtmlEncode(s.Substring(i, 1), false));
					}
				}
				return stringBuilder.ToString();
			}
			return HttpUtility.HtmlEncode(s);
		}

		public static SanitizedHtmlString SanitizeHtmlEncode(string s, bool encodeSpaces)
		{
			SanitizedHtmlString sanitizedHtmlString = new SanitizedHtmlString(Utilities.HtmlEncode(s, encodeSpaces));
			sanitizedHtmlString.DecreeToBeTrusted();
			return sanitizedHtmlString;
		}

		public static void HtmlEncode(string s, StringBuilder stringBuilder)
		{
			if (stringBuilder == null)
			{
				throw new ArgumentNullException("stringBuilder");
			}
			stringBuilder.Append(AntiXssEncoder.HtmlEncode(s, false));
		}

		public static void SanitizeHtmlEncode(string s, StringBuilder stringBuilder)
		{
			SanitizedHtmlString sanitizedHtmlString = new SanitizedHtmlString(AntiXssEncoder.HtmlEncode(s, false));
			sanitizedHtmlString.DecreeToBeTrusted();
			stringBuilder.Append(sanitizedHtmlString);
		}

		public static string UrlEncode(string s)
		{
			return HttpUtility.UrlEncode(s);
		}

		public static string ValidTokenBase64Encode(byte[] byteArray)
		{
			if (byteArray == null)
			{
				throw new ArgumentNullException("byteArray");
			}
			int num = (int)(1.3333333333333333 * (double)byteArray.Length);
			if (num % 4 != 0)
			{
				num += 4 - num % 4;
			}
			char[] array = new char[num];
			Convert.ToBase64CharArray(byteArray, 0, byteArray.Length, array, 0);
			int num2 = 0;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == '\\')
				{
					array[i] = '-';
				}
				else if (array[i] == '=')
				{
					num2++;
				}
			}
			return new string(array, 0, array.Length - num2);
		}

		public static byte[] ValidTokenBase64Decode(string tokenValidBase64String)
		{
			if (tokenValidBase64String == null)
			{
				throw new ArgumentNullException("tokenValidBase64String");
			}
			long num = (long)tokenValidBase64String.Length;
			if (tokenValidBase64String.Length % 4 != 0)
			{
				num += (long)(4 - tokenValidBase64String.Length % 4);
			}
			char[] array = new char[num];
			tokenValidBase64String.CopyTo(0, array, 0, tokenValidBase64String.Length);
			for (long num2 = 0L; num2 < (long)tokenValidBase64String.Length; num2 += 1L)
			{
				checked
				{
					if (array[(int)((IntPtr)num2)] == '-')
					{
						array[(int)((IntPtr)num2)] = '\\';
					}
				}
			}
			for (long num3 = (long)tokenValidBase64String.Length; num3 < (long)array.Length; num3 += 1L)
			{
				array[(int)(checked((IntPtr)num3))] = '=';
			}
			return Convert.FromBase64CharArray(array, 0, array.Length);
		}

		public static string ConvertFromFontSize(int fontSize)
		{
			string result = "12";
			switch (fontSize)
			{
			case 1:
				result = "8";
				break;
			case 2:
				result = "10";
				break;
			case 3:
				result = "12";
				break;
			case 4:
				result = "14";
				break;
			case 5:
				result = "18";
				break;
			case 6:
				result = "24";
				break;
			case 7:
				result = "36";
				break;
			}
			return result;
		}

		public static void RewritePathToError(OwaContext owaContext, string errorDescription)
		{
			if (owaContext == null)
			{
				throw new ArgumentNullException("owaContext");
			}
			if (errorDescription == null)
			{
				throw new ArgumentNullException("errorDescription");
			}
			Utilities.RewritePathToError(owaContext, errorDescription, null);
		}

		public static void RewritePathToError(OwaContext owaContext, string errorDescription, string errorDetailedDescription)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "Utilities.RewritePathToError");
			if (owaContext == null)
			{
				throw new ArgumentNullException("owaContext");
			}
			if (errorDescription == null)
			{
				throw new ArgumentNullException("errorDescription");
			}
			owaContext.ErrorInformation = new ErrorInformation
			{
				Message = errorDescription,
				MessageDetails = errorDetailedDescription
			};
			owaContext.HttpContext.RewritePath(OwaUrl.ErrorPage.ImplicitUrl);
		}

		public static void EndResponse(HttpContext httpContext, HttpStatusCode statusCode)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug<int>(0L, "Utilities.EndResponse: statusCode={0}", (int)statusCode);
			if (httpContext == null)
			{
				throw new ArgumentNullException("httpContext");
			}
			Utilities.MakePageNoCacheNoStore(httpContext.Response);
			httpContext.Response.StatusCode = (int)statusCode;
			try
			{
				httpContext.Response.Flush();
				httpContext.ApplicationInstance.CompleteRequest();
			}
			catch (HttpException arg)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<HttpException>(0L, "Failed to flush and send response to client. {0}", arg);
			}
			httpContext.Response.End();
		}

		public static void TransferToErrorPage(OwaContext owaContext, string errorDescription)
		{
			if (owaContext == null)
			{
				throw new ArgumentNullException("owaContext");
			}
			if (errorDescription == null)
			{
				throw new ArgumentNullException("errorDescription");
			}
			Utilities.TransferToErrorPage(owaContext, errorDescription, null);
		}

		public static void TransferToErrorPage(OwaContext owaContext, string errorDescription, string errorDetailedDescription)
		{
			if (owaContext == null)
			{
				throw new ArgumentNullException("owaContext");
			}
			if (errorDescription == null)
			{
				throw new ArgumentNullException("errorDescription");
			}
			Utilities.TransferToErrorPage(owaContext, errorDescription, errorDetailedDescription, ThemeFileId.Error, false);
		}

		public static void TransferToErrorPage(OwaContext owaContext, string errorDescription, string errorDetailedDescription, ThemeFileId icon, bool hideDebugInformation)
		{
			Utilities.TransferToErrorPage(owaContext, errorDescription, errorDetailedDescription, icon, hideDebugInformation, false);
		}

		public static void TransferToErrorPage(OwaContext owaContext, string errorDescription, string errorDetailedDescription, ThemeFileId icon, bool hideDebugInformation, bool isDetailedErrorHtmlEncoded)
		{
			if (owaContext == null)
			{
				throw new ArgumentNullException("owaContext");
			}
			if (errorDescription == null)
			{
				throw new ArgumentNullException("errorDescription");
			}
			Utilities.TransferToErrorPage(owaContext, new ErrorInformation
			{
				Message = errorDescription,
				MessageDetails = errorDetailedDescription,
				Icon = icon,
				HideDebugInformation = hideDebugInformation,
				IsDetailedErrorHtmlEncoded = isDetailedErrorHtmlEncoded,
				ExternalPageLink = Utilities.GenerateExternalLink(owaContext)
			});
		}

		public static void TransferToErrorPage(OwaContext owaContext, ErrorInformation errorInformation)
		{
			ExTraceGlobals.CoreTracer.TraceDebug(0L, "Utilities.TransferToErrorPage");
			if (owaContext == null)
			{
				throw new ArgumentNullException("owaContext");
			}
			if (errorInformation == null)
			{
				throw new ArgumentNullException("errorInformation");
			}
			owaContext.ErrorInformation = errorInformation;
			owaContext.HttpContext.Server.Transfer(OwaUrl.ErrorPage.ImplicitUrl);
		}

		public static void DisableContentEncodingForThisResponse(HttpResponse response)
		{
			if (response == null)
			{
				throw new ArgumentNullException("response");
			}
			response.AddHeader("Content-Encoding", "none");
		}

		internal static FolderVirtualListViewFilter GetFavoritesFilterViewParameter(UserContext userContext, Folder folder)
		{
			if (userContext.IsInMyMailbox(folder) && folder is SearchFolder)
			{
				object obj = folder.TryGetProperty(FolderSchema.SearchFolderAllowAgeout);
				if (obj is bool && !(bool)obj)
				{
					return FolderVirtualListViewFilter.ParseFromPropertyValue(folder.TryGetProperty(ViewStateProperties.FilteredViewLabel));
				}
			}
			return null;
		}

		internal static bool IsFolderNameConflictError(FolderSaveResult result)
		{
			if (result.OperationResult != OperationResult.Succeeded && result.PropertyErrors != null)
			{
				foreach (PropertyError propertyError in result.PropertyErrors)
				{
					if (propertyError.PropertyErrorCode == PropertyErrorCode.FolderNameConflict)
					{
						return true;
					}
				}
			}
			return false;
		}

		internal static bool IsFavoritesFilterFolder(UserContext userContext, Folder folder)
		{
			return Utilities.GetFavoritesFilterViewParameter(userContext, folder) != null;
		}

		internal static StoreObjectId TryGetDefaultFolderId(MailboxSession session, DefaultFolderType type)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			return session.GetDefaultFolderId(type);
		}

		internal static OwaStoreObjectId TryGetDefaultFolderId(UserContext userContext, MailboxSession session, DefaultFolderType defaultFolderType)
		{
			StoreObjectId storeObjectId = Utilities.TryGetDefaultFolderId(session, defaultFolderType);
			if (storeObjectId != null)
			{
				return OwaStoreObjectId.CreateFromSessionFolderId(userContext, session, storeObjectId);
			}
			return null;
		}

		internal static StoreObjectId GetDefaultFolderId(MailboxSession session, DefaultFolderType type)
		{
			StoreObjectId storeObjectId = Utilities.TryGetDefaultFolderId(session, type);
			if (storeObjectId == null)
			{
				throw new OwaDefaultFolderIdUnavailableException(string.Format("XSO returned null for default folder id {0}.", type.ToString()));
			}
			return storeObjectId;
		}

		internal static StoreObjectId TryGetDefaultFolderId(UserContext userContext, ExchangePrincipal exchangePrincipal, DefaultFolderType type)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (exchangePrincipal == null)
			{
				throw new ArgumentNullException("exchangePrincipal");
			}
			if (type == DefaultFolderType.None)
			{
				throw new ArgumentException("type");
			}
			StoreObjectId result;
			using (OwaStoreObjectIdSessionHandle owaStoreObjectIdSessionHandle = new OwaStoreObjectIdSessionHandle(exchangePrincipal, userContext))
			{
				MailboxSession session = owaStoreObjectIdSessionHandle.Session as MailboxSession;
				result = Utilities.TryGetDefaultFolderId(session, type);
			}
			return result;
		}

		internal static StoreObjectId TryGetDefaultFolderId(UserContext userContext, ExchangePrincipal exchangePrincipal, DefaultFolderType type, out MailboxSession session)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (exchangePrincipal == null)
			{
				throw new ArgumentNullException("exchangePrincipal");
			}
			if (type == DefaultFolderType.None)
			{
				throw new ArgumentException("type");
			}
			OwaStoreObjectIdSessionHandle owaStoreObjectIdSessionHandle = new OwaStoreObjectIdSessionHandle(exchangePrincipal, userContext);
			userContext.AddSessionHandle(owaStoreObjectIdSessionHandle);
			session = (owaStoreObjectIdSessionHandle.Session as MailboxSession);
			return Utilities.TryGetDefaultFolderId(session, type);
		}

		internal static OwaStoreObjectId GetDefaultFolderId(UserContext userContext, MailboxSession session, DefaultFolderType defaultFolderType)
		{
			return OwaStoreObjectId.CreateFromSessionFolderId(userContext, session, Utilities.GetDefaultFolderId(session, defaultFolderType));
		}

		internal static bool IsDefaultFolderId(StoreSession session, StoreObjectId folderId, DefaultFolderType defaultFolderType)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (folderId == null)
			{
				throw new ArgumentNullException("folderId");
			}
			MailboxSession mailboxSession = session as MailboxSession;
			if (mailboxSession == null)
			{
				return false;
			}
			StoreObjectId storeObjectId = Utilities.TryGetDefaultFolderId(mailboxSession, defaultFolderType);
			return storeObjectId != null && folderId.Equals(storeObjectId);
		}

		internal static bool IsItemInDefaultFolder(StoreObject storeObject, DefaultFolderType type)
		{
			return Utilities.IsDefaultFolderId(storeObject.Session, storeObject.ParentId, type);
		}

		internal static bool IsItemInDefaultFolder(IStorePropertyBag storePropertyBag, DefaultFolderType type, MailboxSession session)
		{
			StoreObjectId property = ItemUtility.GetProperty<StoreObjectId>(storePropertyBag, StoreObjectSchema.ParentItemId, null);
			return property != null && Utilities.IsDefaultFolderId(session, property, type);
		}

		internal static bool IsDefaultFolderId(UserContext userContext, OwaStoreObjectId folderId, DefaultFolderType defaultFolderType)
		{
			if (folderId == null)
			{
				throw new ArgumentNullException("folderId");
			}
			return !folderId.IsPublic && Utilities.IsDefaultFolderId(folderId.GetSession(userContext), folderId.StoreObjectId, defaultFolderType);
		}

		internal static bool IsDefaultFolder(Folder folder, DefaultFolderType defaultFolderType)
		{
			if (folder == null)
			{
				throw new ArgumentNullException("folder");
			}
			return Utilities.IsDefaultFolderId(folder.Session, folder.Id.ObjectId, defaultFolderType);
		}

		internal static bool IsFolderSharedOut(Folder folder)
		{
			if (folder == null)
			{
				throw new ArgumentNullException("folder");
			}
			return Utilities.IsOneOfTheFolderFlagsSet(folder, new ExtendedFolderFlags[]
			{
				ExtendedFolderFlags.SharedOut,
				ExtendedFolderFlags.SharedViaExchange
			});
		}

		internal static bool IsFolderSharedOut(ExtendedFolderFlags folderShareFlag)
		{
			return Utilities.IsOneOfTheFolderFlagsSet(folderShareFlag, new ExtendedFolderFlags[]
			{
				ExtendedFolderFlags.SharedOut,
				ExtendedFolderFlags.SharedViaExchange
			});
		}

		internal static bool IsOneOfTheFolderFlagsSet(object folderFlagValue, params ExtendedFolderFlags[] folderFlags)
		{
			int valueToTest = (folderFlagValue is PropertyError) ? 0 : ((int)folderFlagValue);
			foreach (ExtendedFolderFlags flag in folderFlags)
			{
				if (Utilities.IsFlagSet(valueToTest, (int)flag))
				{
					return true;
				}
			}
			return false;
		}

		internal static bool IsOneOfTheFolderFlagsSet(Folder folder, params ExtendedFolderFlags[] folderFlags)
		{
			return Utilities.IsOneOfTheFolderFlagsSet(folder.TryGetProperty(FolderSchema.ExtendedFolderFlags), folderFlags);
		}

		internal static bool CanFolderBeRenamed(UserContext userContext, Folder folder)
		{
			if (Utilities.IsPublic(folder))
			{
				return !userContext.IsPublicFolderRootId(folder.Id.ObjectId);
			}
			return !Utilities.IsSpecialFolderForSession(folder.Session as MailboxSession, folder.Id.ObjectId) && !Utilities.IsELCFolder(folder) && !Utilities.IsOutlookSearchFolder(folder) && !Utilities.IsOneOfTheFolderFlagsSet(folder, new ExtendedFolderFlags[]
			{
				ExtendedFolderFlags.RemoteHierarchy
			});
		}

		internal static DefaultFolderType GetDefaultFolderType(Folder folder)
		{
			return Utilities.GetDefaultFolderType(folder.Session, folder.Id.ObjectId);
		}

		internal static DefaultFolderType GetDefaultFolderType(StoreSession storeSession, StoreObjectId folderId)
		{
			MailboxSession mailboxSession = storeSession as MailboxSession;
			if (mailboxSession == null)
			{
				return DefaultFolderType.None;
			}
			return mailboxSession.IsDefaultFolderType(folderId);
		}

		internal static bool IsSpecialFolder(StoreObjectId id, UserContext userContext)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			return Utilities.IsSpecialFolderForSession(userContext.MailboxSession, id);
		}

		internal static bool IsSpecialFolderForSession(MailboxSession session, StoreObjectId folderId)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (folderId == null)
			{
				throw new ArgumentNullException("folderId");
			}
			return Utilities.IsSpecialFolderType(session.IsDefaultFolderType(folderId));
		}

		internal static bool IsSpecialFolderType(DefaultFolderType defaultFolderType)
		{
			return defaultFolderType != DefaultFolderType.None;
		}

		internal static string GetFolderNameWithSessionName(Folder folder)
		{
			if (folder == null)
			{
				throw new ArgumentNullException("folder");
			}
			return string.Format(LocalizedStrings.GetNonEncoded(-83764036), Utilities.GetSessionMailboxDisplayName(folder), folder.DisplayName);
		}

		internal static Folder SafeFolderBind(MailboxSession mailboxSession, DefaultFolderType defaultFolderType, params PropertyDefinition[] returnProperties)
		{
			StoreObjectId storeObjectId = Utilities.TryGetDefaultFolderId(mailboxSession, defaultFolderType);
			Folder result = null;
			if (storeObjectId != null)
			{
				try
				{
					result = Folder.Bind(mailboxSession, defaultFolderType, returnProperties);
				}
				catch (ObjectNotFoundException ex)
				{
					ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "Failed to bind to folder: Error: {0}", ex.Message);
				}
			}
			return result;
		}

		internal static Folder SafeFolderBind(MailboxSession mailboxSession, StoreObjectId folderId, params PropertyDefinition[] returnProperties)
		{
			Folder result = null;
			try
			{
				result = Folder.Bind(mailboxSession, folderId, returnProperties);
			}
			catch (ObjectNotFoundException ex)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "Failed to bind to folder: Error: {0}", ex.Message);
			}
			return result;
		}

		private static void ThrowInvalidIdFormatException(string storeObjectId, string changeKey, Exception innerException)
		{
			throw new OwaInvalidIdFormatException(string.Format("Invalid id format. Store object id: {0}. Change key: {1}", (storeObjectId == null) ? "null" : storeObjectId, (changeKey == null) ? "null" : changeKey), innerException);
		}

		internal static VersionedId CreateItemId(MailboxSession mailboxSession, StoreObjectId storeObjectId, string changeKey)
		{
			if (mailboxSession == null)
			{
				throw new ArgumentNullException("mailboxSession");
			}
			if (storeObjectId == null)
			{
				throw new ArgumentNullException("storeObjectId");
			}
			return Utilities.CreateItemId(mailboxSession, storeObjectId.ToBase64String(), changeKey);
		}

		internal static VersionedId CreateItemId(MailboxSession mailboxSession, string storeObjectId, string changeKey)
		{
			if (mailboxSession == null)
			{
				throw new ArgumentNullException("mailboxSession");
			}
			return Utilities.CreateItemId(storeObjectId, changeKey);
		}

		internal static VersionedId CreateItemId(StoreObjectId storeObjectId, string changeKey)
		{
			if (storeObjectId == null)
			{
				throw new ArgumentNullException("storeObjectId");
			}
			return Utilities.CreateItemId(storeObjectId.ToBase64String(), changeKey);
		}

		internal static VersionedId CreateItemId(string storeObjectId, string changeKey)
		{
			if (string.IsNullOrEmpty(storeObjectId))
			{
				throw new OwaInvalidIdFormatException("Missing store object id");
			}
			if (string.IsNullOrEmpty(changeKey))
			{
				throw new OwaInvalidIdFormatException("Missing change key");
			}
			VersionedId result = null;
			try
			{
				result = VersionedId.Deserialize(storeObjectId, changeKey);
			}
			catch (ArgumentException innerException)
			{
				Utilities.ThrowInvalidIdFormatException(storeObjectId, changeKey, innerException);
			}
			catch (FormatException innerException2)
			{
				Utilities.ThrowInvalidIdFormatException(storeObjectId, changeKey, innerException2);
			}
			catch (CorruptDataException innerException3)
			{
				Utilities.ThrowInvalidIdFormatException(storeObjectId, changeKey, innerException3);
			}
			return result;
		}

		internal static StoreObjectId CreateStoreObjectId(MailboxSession mailboxSession, string storeObjectId)
		{
			if (mailboxSession == null)
			{
				throw new ArgumentNullException("mailboxSession");
			}
			if (string.IsNullOrEmpty(storeObjectId))
			{
				throw new OwaInvalidIdFormatException("Missing store object id");
			}
			return Utilities.CreateStoreObjectId(storeObjectId);
		}

		internal static StoreObjectId CreateStoreObjectId(string storeObjectIdString)
		{
			if (storeObjectIdString == null)
			{
				throw new ArgumentNullException("storeObjectIdString");
			}
			StoreObjectId result = null;
			try
			{
				result = StoreObjectId.Deserialize(storeObjectIdString);
			}
			catch (ArgumentException innerException)
			{
				Utilities.ThrowInvalidIdFormatException(storeObjectIdString, null, innerException);
			}
			catch (FormatException innerException2)
			{
				Utilities.ThrowInvalidIdFormatException(storeObjectIdString, null, innerException2);
			}
			catch (CorruptDataException innerException3)
			{
				Utilities.ThrowInvalidIdFormatException(storeObjectIdString, null, innerException3);
			}
			return result;
		}

		internal static byte[] CreateInstanceKey(string instanceKeyString)
		{
			if (instanceKeyString == null)
			{
				throw new ArgumentNullException("instanceKeyString");
			}
			byte[] result = null;
			try
			{
				result = Convert.FromBase64String(instanceKeyString);
			}
			catch (ArgumentException innerException)
			{
				Utilities.ThrowInvalidIdFormatException(instanceKeyString, null, innerException);
			}
			catch (FormatException innerException2)
			{
				Utilities.ThrowInvalidIdFormatException(instanceKeyString, null, innerException2);
			}
			catch (CorruptDataException innerException3)
			{
				Utilities.ThrowInvalidIdFormatException(instanceKeyString, null, innerException3);
			}
			return result;
		}

		internal static StoreId TryGetStoreId(ObjectId objectId)
		{
			if (objectId == null)
			{
				throw new ArgumentNullException("objectId");
			}
			OwaStoreObjectId owaStoreObjectId = objectId as OwaStoreObjectId;
			if (owaStoreObjectId != null)
			{
				return owaStoreObjectId.StoreId;
			}
			ConversationId conversationId = objectId as ConversationId;
			if (conversationId != null)
			{
				return conversationId;
			}
			StoreId storeId = objectId as StoreId;
			if (storeId != null)
			{
				return StoreId.GetStoreObjectId(storeId);
			}
			return null;
		}

		public static string ProviderSpecificIdFromStoreObjectId(string storeObjectId)
		{
			if (storeObjectId == null)
			{
				throw new ArgumentNullException("storeObjectId");
			}
			string result = null;
			try
			{
				result = Convert.ToBase64String(Utilities.CreateStoreObjectId(storeObjectId).ProviderLevelItemId);
			}
			catch (ArgumentException innerException)
			{
				Utilities.ThrowInvalidIdFormatException(storeObjectId, null, innerException);
			}
			return result;
		}

		internal static OwaStoreObjectIdType GetOwaStoreObjectIdType(UserContext userContext, StoreObject storeObject)
		{
			if (storeObject == null)
			{
				throw new ArgumentNullException("storeObject");
			}
			OwaStoreObjectIdType result = OwaStoreObjectIdType.MailBoxObject;
			if (Utilities.IsPublic(storeObject))
			{
				if (storeObject is Item)
				{
					result = OwaStoreObjectIdType.PublicStoreItem;
				}
				else
				{
					result = OwaStoreObjectIdType.PublicStoreFolder;
				}
			}
			else if (userContext.IsInOtherMailbox(storeObject))
			{
				result = OwaStoreObjectIdType.OtherUserMailboxObject;
			}
			else if (Utilities.IsInArchiveMailbox(storeObject))
			{
				result = OwaStoreObjectIdType.ArchiveMailboxObject;
			}
			return result;
		}

		internal static bool IsPublic(StoreObject storeObject)
		{
			if (storeObject == null)
			{
				throw new ArgumentNullException("storeObject");
			}
			return storeObject.Session is PublicFolderSession;
		}

		internal static bool IsOtherMailbox(StoreObject storeObject)
		{
			if (storeObject == null)
			{
				throw new ArgumentNullException("storeObject");
			}
			return OwaContext.Current.UserContext.IsInOtherMailbox(storeObject);
		}

		internal static bool IsInArchiveMailbox(StoreObject storeObject)
		{
			if (storeObject == null)
			{
				throw new ArgumentNullException("storeObject");
			}
			return Utilities.IsArchiveMailbox(storeObject.Session);
		}

		internal static bool IsArchiveMailbox(StoreSession storeSession)
		{
			MailboxSession mailboxSession = storeSession as MailboxSession;
			return mailboxSession != null && Utilities.IsArchiveMailbox(mailboxSession);
		}

		private static bool IsArchiveMailbox(MailboxSession session)
		{
			return session.MailboxOwner.MailboxInfo.IsArchive;
		}

		internal static string GetMailboxSessionLegacyDN(StoreObject storeObject)
		{
			if (storeObject == null)
			{
				throw new ArgumentNullException("storeObject");
			}
			MailboxSession mailboxSession = storeObject.Session as MailboxSession;
			if (mailboxSession == null)
			{
				throw new OwaInvalidOperationException("Store object must belong to Mailbox Session");
			}
			return mailboxSession.MailboxOwnerLegacyDN;
		}

		internal static string GetSessionMailboxDisplayName(StoreObject storeObject)
		{
			if (storeObject == null)
			{
				throw new ArgumentNullException("storeObject");
			}
			MailboxSession mailboxSession = storeObject.Session as MailboxSession;
			if (mailboxSession == null)
			{
				throw new OwaInvalidOperationException("Store object must belong to Mailbox Session");
			}
			return mailboxSession.DisplayName;
		}

		internal static bool IsValidLegacyDN(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				throw new ArgumentNullException("address");
			}
			LegacyDN legacyDN;
			return LegacyDN.TryParse(address, out legacyDN);
		}

		internal static IExchangePrincipal GetFolderOwnerExchangePrincipal(OwaStoreObjectId folderId, UserContext userContext)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (folderId == null)
			{
				return userContext.ExchangePrincipal;
			}
			MailboxSession mailboxSession = (MailboxSession)folderId.GetSession(userContext);
			return mailboxSession.MailboxOwner;
		}

		public static string GetQueryStringParameter(HttpRequest httpRequest, string name)
		{
			return Utilities.GetQueryStringParameter(httpRequest, name, true);
		}

		public static string GetQueryStringParameter(HttpRequest httpRequest, string name, bool required)
		{
			if (httpRequest == null)
			{
				throw new ArgumentNullException("httpRequest");
			}
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			string text = httpRequest.QueryString[name];
			if (text == null && required)
			{
				throw new OwaInvalidRequestException(string.Format("Required URL parameter missing: {0}", name));
			}
			return text;
		}

		public static string[] GetQueryStringArrayParameter(HttpRequest httpRequest, string name, bool required)
		{
			return Utilities.GetQueryStringArrayParameter(httpRequest, name, required, 0);
		}

		public static string[] GetQueryStringArrayParameter(HttpRequest httpRequest, string name, bool required, int maxLength)
		{
			if (httpRequest == null)
			{
				throw new ArgumentNullException("httpRequest");
			}
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			string[] values = httpRequest.QueryString.GetValues(name);
			if (values == null)
			{
				if (required)
				{
					throw new OwaInvalidRequestException(string.Format("Required URL parameter missing: {0}", name));
				}
			}
			else if (maxLength > 0 && values.Length > maxLength)
			{
				throw new OwaInvalidRequestException(string.Format("Parameter has too many values: {0}", name));
			}
			return values;
		}

		public static ExDateTime GetQueryStringParameterDateTime(HttpRequest httpRequest, string name, ExTimeZone timeZone)
		{
			return Utilities.GetQueryStringParameterDateTime(httpRequest, name, timeZone, true);
		}

		private static ExDateTime GetQueryStringParameterDateTime(HttpRequest httpRequest, string name, ExTimeZone timeZone, bool required)
		{
			if (timeZone == null)
			{
				throw new ArgumentNullException("timeZone");
			}
			ExDateTime result = ExDateTime.MinValue;
			string queryStringParameter = Utilities.GetQueryStringParameter(httpRequest, name, required);
			if (queryStringParameter == null)
			{
				return result;
			}
			try
			{
				result = DateTimeUtilities.ParseIsoDate(queryStringParameter, timeZone);
			}
			catch (OwaParsingErrorException)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<string, string>(0L, "Invalid date '{0}' provided on URL '{1}'", name, queryStringParameter);
				throw new OwaInvalidRequestException("Invalid date time on URL");
			}
			return result;
		}

		public static ExDateTime[] GetQueryStringParameterDateTimeArray(HttpRequest httpRequest, string name, ExTimeZone timeZone, bool required, int maxLength)
		{
			string[] queryStringArrayParameter = Utilities.GetQueryStringArrayParameter(httpRequest, name, required, maxLength);
			if (queryStringArrayParameter == null)
			{
				return null;
			}
			ExDateTime[] array = new ExDateTime[queryStringArrayParameter.Length];
			for (int i = 0; i < queryStringArrayParameter.Length; i++)
			{
				try
				{
					array[i] = DateTimeUtilities.ParseIsoDate(queryStringArrayParameter[i], timeZone);
				}
				catch (OwaParsingErrorException)
				{
					ExTraceGlobals.CoreTracer.TraceDebug<string, string>(0L, "Invalid date '{0}' provided on URL '{1}'", name, queryStringArrayParameter[i]);
					throw new OwaInvalidRequestException("Invalid date time on URL");
				}
			}
			return array;
		}

		public static string GetFormParameter(HttpRequest httpRequest, string name)
		{
			return Utilities.GetFormParameter(httpRequest, name, true);
		}

		public static string GetFormParameter(HttpRequest httpRequest, string name, bool required)
		{
			if (httpRequest == null)
			{
				throw new ArgumentNullException("httpRequest");
			}
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name", "name cannot be null or empty");
			}
			string text = httpRequest.Form[name];
			if (text == null && required)
			{
				throw new OwaInvalidRequestException(string.Format("Required form parameter missing: {0}", name));
			}
			return text;
		}

		public static SecureString GetSecureFormParameter(HttpRequest httpRequest, string name)
		{
			return Utilities.GetSecureFormParameter(httpRequest, name, true);
		}

		public static SecureString GetSecureFormParameter(HttpRequest httpRequest, string name, bool required)
		{
			return Utilities.SecureStringFromString(Utilities.GetFormParameter(httpRequest, name, required));
		}

		public static SecureString SecureStringFromString(string regularString)
		{
			return regularString.AsSecureString();
		}

		public static bool SecureStringEquals(SecureString a, SecureString b)
		{
			if (a == null || b == null || a.Length != b.Length)
			{
				return false;
			}
			using (SecureArray<char> secureArray = a.ConvertToSecureCharArray())
			{
				using (SecureArray<char> secureArray2 = b.ConvertToSecureCharArray())
				{
					for (int i = 0; i < a.Length; i++)
					{
						if (secureArray.ArrayValue[i] != secureArray2.ArrayValue[i])
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		internal static void HandleException(OwaContext owaContext, Exception exception)
		{
			Utilities.HandleException(owaContext, exception, false);
		}

		internal static string FormatExceptionNameAndMessage(string prefix, Exception e)
		{
			StringBuilder stringBuilder = new StringBuilder(prefix);
			while (e != null)
			{
				stringBuilder.AppendFormat("[{0}:{1}]", e.GetType().FullName, e.Message);
				e = e.InnerException;
			}
			return stringBuilder.ToString();
		}

		internal static void HandleException(OwaContext owaContext, Exception exception, bool showErrorInPage)
		{
			Microsoft.Exchange.Clients.Owa.Core.Culture.SetThreadCulture(owaContext);
			HttpContext httpContext = owaContext.HttpContext;
			ExTraceGlobals.CoreTracer.TraceDebug<Type, string>(0L, "Exception: Type: {0} Error: {1}.", exception.GetType(), exception.Message);
			Utilities.MakePageNoCacheNoStore(httpContext.Response);
			string str;
			if (!Utilities.ExceptionCodeMap.TryGetValue(exception.GetType(), out str))
			{
				str = "UE:" + exception.GetType().ToString();
			}
			owaContext.HttpContext.Response.AppendToLog("&ex=" + str);
			if (exception is HttpException)
			{
				HttpException ex = (HttpException)exception;
				httpContext.Response.AppendToLog(string.Format("&BadRequest=BasicHttpException:{0}", ex.GetHttpCode()));
				httpContext.Response.AppendToLog(Utilities.FormatExceptionNameAndMessage("&exception=", exception));
				Utilities.EndResponse(httpContext, HttpStatusCode.BadRequest);
				return;
			}
			if (exception is OwaRenderingEmbeddedReadingPaneException)
			{
				owaContext.UserContext.DisableEmbeddedReadingPane();
				if (owaContext.FormsRegistryContext.ApplicationElement == ApplicationElement.StartPage)
				{
					owaContext.HttpContext.Response.Clear();
					string explicitUrl = OwaUrl.ApplicationRoot.GetExplicitUrl(owaContext);
					owaContext.HttpContext.Response.Redirect(explicitUrl);
					return;
				}
				exception = (exception as OwaRenderingEmbeddedReadingPaneException).InnerException;
			}
			if (exception is OwaInvalidRequestException || exception is OwaInvalidIdFormatException)
			{
				httpContext.Response.AppendToLog("&BadRequest=BasicInvlaidRequest");
				httpContext.Response.AppendToLog(Utilities.FormatExceptionNameAndMessage("&exception=", exception));
				Utilities.EndResponse(httpContext, HttpStatusCode.BadRequest);
				return;
			}
			if (exception is OwaSegmentationException || exception is OwaForbiddenRequestException)
			{
				Utilities.EndResponse(httpContext, HttpStatusCode.Forbidden);
				return;
			}
			if (exception is OwaDelegatorMailboxFailoverException)
			{
				OwaDelegatorMailboxFailoverException ex2 = exception as OwaDelegatorMailboxFailoverException;
				ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "Delegator {0}'s mailbox failover occurs.", ex2.MailboxOwnerLegacyDN);
			}
			if (exception is MailboxInSiteFailoverException)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "User {0}'s mailbox in-site failover occurs.", owaContext.ExchangePrincipal.LegacyDn);
				if (owaContext.UserContext != null)
				{
					owaContext.UserContext.DisconnectMailboxSession();
				}
			}
			if (exception is MailboxCrossSiteFailoverException || exception is WrongServerException)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "User {0}'s mailbox cross-site failover occurs.", owaContext.ExchangePrincipal.LegacyDn);
				UserContextCookie userContextCookie = UserContextCookie.GetUserContextCookie(owaContext);
				if (userContextCookie != null)
				{
					Utilities.DeleteCookie(httpContext.Response, userContextCookie.CookieName);
				}
				if (owaContext.IsProxyRequest)
				{
					httpContext.Response.AddHeader("mailboxCrossSiteFailover", "true");
				}
			}
			if (exception is WrongCASServerBecauseOfOutOfDateDNSCacheException)
			{
				Utilities.DeleteFBASessionCookies(httpContext.Response);
			}
			if (exception is OwaURLIsOutOfDateException)
			{
				Utilities.DeleteFBASessionCookies(httpContext.Response);
			}
			if (exception is OverBudgetException)
			{
				OverBudgetException ex3 = (OverBudgetException)exception;
				httpContext.Response.AppendToLog(string.Format("&OverBudget({0}/{1}),Owner:{2}[{3}]", new object[]
				{
					ex3.IsServiceAccountBudget ? "ServiceAccount" : "Normal",
					ex3.PolicyPart,
					ex3.Owner,
					ex3.Snapshot
				}));
			}
			string queryStringParameter = Utilities.GetQueryStringParameter(httpContext.Request, "prntFId", false);
			string previousPageUrl = null;
			if (!string.IsNullOrEmpty(queryStringParameter))
			{
				previousPageUrl = OwaUrl.ApplicationRoot.GetExplicitUrl(owaContext) + "?ae=Folder&t=IPF.DocumentLibrary&URL=" + Utilities.UrlEncode(queryStringParameter);
			}
			string externalPageLink = Utilities.GenerateExternalLink(owaContext);
			bool isBasicAuthentication = Utilities.IsBasicAuthentication(httpContext.Request);
			ErrorInformation exceptionHandlingInformation = Utilities.GetExceptionHandlingInformation(exception, owaContext.MailboxIdentity, Utilities.IsWebPartRequest(owaContext), previousPageUrl, externalPageLink, isBasicAuthentication, owaContext, !showErrorInPage);
			if (owaContext.FormsRegistryContext.ApplicationElement == ApplicationElement.StartPage)
			{
				string text = (owaContext.ExchangePrincipal != null) ? owaContext.ExchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString() : string.Empty;
				string text2 = (owaContext.HttpContext.Request != null) ? owaContext.HttpContext.Request.Url.ToString() : string.Empty;
				if (exception is OwaTransientException || exception is StorageTransientException || exception is ADTransientException || exception is ThreadAbortException)
				{
					OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_OwaStartPageInitializationWarning, null, new object[]
					{
						text,
						text2,
						exception
					});
				}
				else
				{
					OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_OwaStartPageInitializationError, null, new object[]
					{
						text,
						text2,
						exception
					});
				}
			}
			try
			{
				if (!owaContext.ErrorSent)
				{
					owaContext.ErrorSent = true;
					httpContext.Response.Clear();
					OwaRequestType requestType = owaContext.RequestType;
					if (requestType == OwaRequestType.Invalid)
					{
						requestType = Utilities.GetRequestType(owaContext.HttpContext.Request);
					}
					StringBuilder stringBuilder = new StringBuilder();
					StringWriter stringWriter = new StringWriter(stringBuilder);
					if (Utilities.IsOehOrSubPageContentRequest(owaContext) && !showErrorInPage)
					{
						ExTraceGlobals.CoreTracer.TraceDebug(0L, "OEH error response");
						OwaEventHttpHandler.RenderError(owaContext, stringWriter, exceptionHandlingInformation.Message, exceptionHandlingInformation.MessageDetails, exceptionHandlingInformation.OwaEventHandlerErrorCode, exceptionHandlingInformation.HideDebugInformation ? null : exception);
					}
					else
					{
						ExTraceGlobals.CoreTracer.TraceDebug(0L, "Error page error response");
						owaContext.ErrorInformation = exceptionHandlingInformation;
						httpContext.Server.Execute(exceptionHandlingInformation.OwaUrl.ImplicitUrl, stringWriter);
					}
					stringWriter.Close();
					httpContext.Response.Write(stringBuilder);
					try
					{
						if (requestType == OwaRequestType.ICalHttpHandler)
						{
							httpContext.Response.TrySkipIisCustomErrors = true;
							httpContext.Response.StatusCode = 503;
						}
						else
						{
							httpContext.Response.StatusCode = 200;
						}
						httpContext.Response.AppendHeader("Content-Length", httpContext.Response.Output.Encoding.GetByteCount(stringBuilder.ToString()).ToString());
					}
					catch (HttpException arg)
					{
						ExTraceGlobals.CoreTracer.TraceDebug<HttpException>(0L, "Failed to set the header status after submitting watson and rendering error page. {0}", arg);
					}
					try
					{
						httpContext.Response.Flush();
						httpContext.ApplicationInstance.CompleteRequest();
					}
					catch (HttpException arg2)
					{
						ExTraceGlobals.CoreTracer.TraceDebug<HttpException>(0L, "Failed to flush and send response to client after submitting watson and rendering error page. {0}", arg2);
					}
				}
			}
			finally
			{
				if (exceptionHandlingInformation.SendWatsonReport && Globals.SendWatsonReports)
				{
					ExTraceGlobals.CoreTracer.TraceDebug(0L, "Sending watson report");
					ExWatson.AddExtraData(Utilities.GetExtraWatsonData(owaContext));
					if (exception.Data.Contains("ActiveEntryCallStack"))
					{
						ExWatson.AddExtraData(exception.Data["ActiveEntryCallStack"].ToString());
					}
					ReportOptions options = (exception is AccessViolationException || exception is InvalidProgramException || exception is TypeInitializationException) ? ReportOptions.ReportTerminateAfterSend : ReportOptions.None;
					ExWatson.SendReport(exception, options, null);
				}
				if (exception is AccessViolationException)
				{
					ExTraceGlobals.CoreTracer.TraceDebug(0L, "Shutting down OWA due to unrecoverable exception");
					Environment.Exit(1);
				}
				else if ((exception is InvalidProgramException || exception is TypeInitializationException) && Interlocked.Exchange(ref Utilities.queuedDelayedRestart, 1) == 0)
				{
					new Thread(new ThreadStart(Utilities.DelayedRestartUponUnexecutableCode)).Start();
				}
				httpContext.Response.End();
			}
		}

		internal static bool IsOehOrSubPageContentRequest(OwaContext owaContext)
		{
			return owaContext.RequestType == OwaRequestType.Oeh || Utilities.IsSubPageContentRequest(owaContext) || owaContext.RequestType == OwaRequestType.ProxyToEwsEventHandler;
		}

		internal static bool IsSubPageContentRequest(OwaContext owaContext)
		{
			string queryStringParameter = Utilities.GetQueryStringParameter(owaContext.HttpContext.Request, "SP", false);
			return queryStringParameter != null && queryStringParameter == "1";
		}

		internal static bool IsPrefetchRequest(OwaContext owaContext)
		{
			string queryStringParameter = Utilities.GetQueryStringParameter(owaContext.HttpContext.Request, "pf", false);
			return queryStringParameter != null && queryStringParameter == "1";
		}

		internal static IRecipientSession CreateScopedRecipientSession(bool readOnly, ConsistencyMode consistencyMode, string domain)
		{
			if (!VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Global.MultiTenancy.Enabled)
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromRootOrgScopeSet();
				return DirectorySessionFactory.Default.CreateRootOrgRecipientSession(null, null, CultureInfo.InvariantCulture.LCID, readOnly, consistencyMode, null, sessionSettings, 3731, "CreateScopedRecipientSession", "f:\\15.00.1497\\sources\\dev\\clients\\src\\owa\\bin\\core\\Utilities.cs");
			}
			if (domain == null)
			{
				throw new ArgumentException("Domain");
			}
			ADSessionSettings sessionSettings2 = ADSessionSettings.FromTenantAcceptedDomain(domain);
			return DirectorySessionFactory.Default.CreateTenantRecipientSession(null, null, CultureInfo.InvariantCulture.LCID, readOnly, consistencyMode, null, sessionSettings2, 3717, "CreateScopedRecipientSession", "f:\\15.00.1497\\sources\\dev\\clients\\src\\owa\\bin\\core\\Utilities.cs");
		}

		internal static IRecipientSession CreateADRecipientSession(ConsistencyMode consistencyMode, UserContext userContext)
		{
			return Utilities.CreateADRecipientSession(true, consistencyMode, userContext);
		}

		internal static IRecipientSession CreateADRecipientSession(bool readOnly, ConsistencyMode consistencyMode, UserContext userContext)
		{
			return Utilities.CreateADRecipientSession(CultureInfo.CurrentCulture.LCID, readOnly, consistencyMode, false, userContext, true);
		}

		internal static IRecipientSession CreateADRecipientSession(int lcid, bool readOnly, ConsistencyMode consistencyMode, bool useDirectorySearchRoot, UserContext userContext)
		{
			return Utilities.CreateADRecipientSession(lcid, readOnly, consistencyMode, useDirectorySearchRoot, userContext, true);
		}

		internal static IRecipientSession CreateADRecipientSession(int lcid, bool readOnly, ConsistencyMode consistencyMode, bool useDirectorySearchRoot, UserContext userContext, bool scopeToGal)
		{
			ADSessionSettings adsessionSettings;
			if (scopeToGal)
			{
				adsessionSettings = ADSessionSettings.FromOrganizationIdWithAddressListScopeServiceOnly(userContext.ExchangePrincipal.MailboxInfo.OrganizationId, (userContext.GlobalAddressList != null) ? userContext.GlobalAddressList.Id : null);
			}
			else if (userContext.ExchangePrincipal.MailboxInfo.Configuration.AddressBookPolicy != null)
			{
				adsessionSettings = ADSessionSettings.FromOrganizationIdWithAddressListScopeServiceOnly(userContext.ExchangePrincipal.MailboxInfo.OrganizationId, userContext.GlobalAddressListId);
			}
			else
			{
				adsessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(userContext.ExchangePrincipal.MailboxInfo.OrganizationId);
			}
			adsessionSettings.AccountingObject = OwaContext.TryGetCurrentBudget();
			return DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, useDirectorySearchRoot ? userContext.MailboxIdentity.GetOWAMiniRecipient().QueryBaseDN : null, lcid, readOnly, consistencyMode, null, adsessionSettings, 3859, "CreateADRecipientSession", "f:\\15.00.1497\\sources\\dev\\clients\\src\\owa\\bin\\core\\Utilities.cs");
		}

		internal static ADSessionSettings CreateScopedADSessionSettings(string domain)
		{
			ADSessionSettings result;
			if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Global.MultiTenancy.Enabled)
			{
				result = ADSessionSettings.FromTenantAcceptedDomain(domain);
			}
			else
			{
				result = ADSessionSettings.FromRootOrgScopeSet();
			}
			return result;
		}

		internal static IConfigurationSession CreateConfigurationSessionScoped(bool readOnly, ConsistencyMode consistencyMode, ADObjectId adObjectId)
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromAllTenantsOrRootOrgAutoDetect(adObjectId);
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(readOnly, consistencyMode, sessionSettings, 3905, "CreateConfigurationSessionScoped", "f:\\15.00.1497\\sources\\dev\\clients\\src\\owa\\bin\\core\\Utilities.cs");
		}

		internal static ITopologyConfigurationSession CreateTopologyConfigurationSessionScopedToRootOrg(bool readOnly, ConsistencyMode consistencyMode)
		{
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(readOnly, consistencyMode, ADSessionSettings.FromRootOrgScopeSet(), 3923, "CreateTopologyConfigurationSessionScopedToRootOrg", "f:\\15.00.1497\\sources\\dev\\clients\\src\\owa\\bin\\core\\Utilities.cs");
		}

		internal static ITopologyConfigurationSession CreateADSystemConfigurationSessionScopedToFirstOrg(bool readOnly, ConsistencyMode consistencyMode)
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), OrganizationId.ForestWideOrgId, null, false);
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(readOnly, consistencyMode, sessionSettings, 3959, "CreateADSystemConfigurationSessionScopedToFirstOrg", "f:\\15.00.1497\\sources\\dev\\clients\\src\\owa\\bin\\core\\Utilities.cs");
		}

		internal static IConfigurationSession CreateADSystemConfigurationSession(bool readOnly, ConsistencyMode consistencyMode, UserContext userContext)
		{
			ADSessionSettings adsessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(userContext.ExchangePrincipal.MailboxInfo.OrganizationId);
			adsessionSettings.AccountingObject = OwaContext.TryGetCurrentBudget();
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(readOnly, consistencyMode, adsessionSettings, 3982, "CreateADSystemConfigurationSession", "f:\\15.00.1497\\sources\\dev\\clients\\src\\owa\\bin\\core\\Utilities.cs");
		}

		internal static IConfigurationSession CreateADSystemConfigurationSession(bool readOnly, ConsistencyMode consistencyMode, OrganizationId organizationId)
		{
			ADSessionSettings adsessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId);
			adsessionSettings.AccountingObject = OwaContext.TryGetCurrentBudget();
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(readOnly, consistencyMode, adsessionSettings, 4004, "CreateADSystemConfigurationSession", "f:\\15.00.1497\\sources\\dev\\clients\\src\\owa\\bin\\core\\Utilities.cs");
		}

		internal static OutboundConversionOptions CreateOutboundConversionOptions(UserContext userContext)
		{
			OutboundConversionOptions outboundConversionOptions = new OutboundConversionOptions(OwaConfigurationManager.Configuration.DefaultAcceptedDomain.DomainName.ToString());
			outboundConversionOptions.UserADSession = Utilities.CreateADRecipientSession(ConsistencyMode.IgnoreInvalid, userContext);
			outboundConversionOptions.LoadPerOrganizationCharsetDetectionOptions(userContext.ExchangePrincipal.MailboxInfo.OrganizationId);
			return outboundConversionOptions;
		}

		internal static InboundConversionOptions CreateInboundConversionOptions(UserContext userContext)
		{
			InboundConversionOptions inboundConversionOptions = new InboundConversionOptions(OwaConfigurationManager.Configuration.DefaultAcceptedDomain.DomainName.ToString());
			inboundConversionOptions.UserADSession = Utilities.CreateADRecipientSession(ConsistencyMode.IgnoreInvalid, userContext);
			inboundConversionOptions.LoadPerOrganizationCharsetDetectionOptions(userContext.ExchangePrincipal.MailboxInfo.OrganizationId);
			return inboundConversionOptions;
		}

		private static void DelayedRestartUponUnexecutableCode()
		{
			Thread.Sleep(90000);
			OwaDiagnostics.Logger.LogEvent(ClientsEventLogConstants.Tuple_OwaRestartingAfterFailedLoad, string.Empty, new object[0]);
			Environment.Exit(1);
		}

		public static string GetExtraWatsonData(OwaContext owaContext)
		{
			if (owaContext == null)
			{
				throw new ArgumentNullException("owaContext");
			}
			StringBuilder stringBuilder = new StringBuilder();
			UserContext userContext = owaContext.TryGetUserContext();
			if (userContext != null)
			{
				TimeSpan timeSpan = TimeSpan.FromSeconds((double)((userContext.LastAccessedTime - userContext.SessionBeginTime) / Stopwatch.Frequency));
				stringBuilder.AppendLine();
				stringBuilder.Append("Session Length: ");
				stringBuilder.Append(timeSpan.ToString());
				stringBuilder.AppendLine();
				stringBuilder.Append("OWA Version: ");
				stringBuilder.Append(Globals.ApplicationVersion);
				stringBuilder.AppendLine();
				stringBuilder.Append("User Culture: ");
				if (Microsoft.Exchange.Clients.Owa.Core.Culture.GetUserCulture(owaContext) != null && Microsoft.Exchange.Clients.Owa.Core.Culture.GetUserCulture(owaContext).Name != null)
				{
					stringBuilder.Append(Microsoft.Exchange.Clients.Owa.Core.Culture.GetUserCulture(owaContext).Name);
				}
				else
				{
					stringBuilder.Append("Not Found");
				}
				string tcmidvalue = Utilities.GetTCMIDValue(owaContext);
				if (!string.IsNullOrEmpty(tcmidvalue))
				{
					stringBuilder.AppendLine();
					stringBuilder.Append("TestCaseID: ");
					stringBuilder.Append(tcmidvalue);
				}
				if (!Globals.DisableBreadcrumbs)
				{
					stringBuilder.AppendLine();
					stringBuilder.AppendLine();
					stringBuilder.AppendLine(userContext.DumpBreadcrumbs());
				}
			}
			return stringBuilder.ToString();
		}

		internal static void WriteErrorToWebPart(OwaContext owaContext)
		{
			if (owaContext.ErrorInformation == null)
			{
				throw new OwaInvalidOperationException("owaContext.ErrorInformation may not be null");
			}
			HttpContext httpContext = owaContext.HttpContext;
			ExTraceGlobals.WebPartRequestTracer.TraceDebug<OwaEventHandlerErrorCode, string>(0L, "Invalid web part request: Type: {0} Error: {1}.", owaContext.ErrorInformation.OwaEventHandlerErrorCode, owaContext.ErrorInformation.Message);
			Utilities.MakePageNoCacheNoStore(httpContext.Response);
			StringBuilder stringBuilder = new StringBuilder();
			StringWriter stringWriter = new StringWriter(stringBuilder);
			WebPartUtilities.RenderError(owaContext, stringWriter);
			stringWriter.Close();
			httpContext.Response.Clear();
			httpContext.Response.Write(stringBuilder);
			httpContext.Response.StatusCode = 200;
			httpContext.Response.AppendHeader("Content-Length", httpContext.Response.Output.Encoding.GetByteCount(stringBuilder.ToString()).ToString());
			try
			{
				httpContext.Response.Flush();
				httpContext.ApplicationInstance.CompleteRequest();
			}
			catch (HttpException arg)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<HttpException>(0L, "Failed to flush and send response to client. {0}", arg);
			}
			httpContext.Response.End();
		}

		internal static bool IsWebPartRequest(OwaContext owaContext)
		{
			return owaContext.RequestType == OwaRequestType.WebPart || (owaContext.SessionContext != null && owaContext.SessionContext.IsWebPartRequest);
		}

		internal static bool IsWebPartDelegateAccessRequest(OwaContext owaContext)
		{
			return Utilities.IsWebPartRequest(owaContext) && owaContext.UserContext.IsExplicitLogonOthersMailbox;
		}

		internal static ErrorInformation GetExceptionHandlingInformation(Exception exception, OwaIdentity mailboxIdentity)
		{
			return Utilities.GetExceptionHandlingInformation(exception, mailboxIdentity, false);
		}

		internal static ErrorInformation GetExceptionHandlingInformation(Exception exception, OwaIdentity mailboxIdentity, bool isWebPartRequest)
		{
			return Utilities.GetExceptionHandlingInformation(exception, mailboxIdentity, false, null, null, false);
		}

		internal static ErrorInformation GetExceptionHandlingInformation(Exception exception, OwaIdentity mailboxIdentity, bool isWebPartRequest, string previousPageUrl, string externalPageLink, bool isBasicAuthentication)
		{
			return Utilities.GetExceptionHandlingInformation(exception, mailboxIdentity, isWebPartRequest, previousPageUrl, externalPageLink, isBasicAuthentication, null, false);
		}

		internal static ErrorInformation GetExceptionHandlingInformation(Exception exception, OwaIdentity mailboxIdentity, bool isWebPartRequest, string previousPageUrl, string externalPageLink, bool isBasicAuthentication, OwaContext owaContext, bool showErrorInDialogForOeh)
		{
			string message = null;
			string messageDetails = null;
			bool hideDebugInformation = false;
			bool sendWatsonReport = false;
			bool isErrorMessageHtmlEncoded = false;
			bool isDetailedErrorHtmlEncoded = false;
			OwaEventHandlerErrorCode owaEventHandlerErrorCode = OwaEventHandlerErrorCode.NotSet;
			ThemeFileId icon = ThemeFileId.Error;
			ThemeFileId background = ThemeFileId.None;
			OwaUrl owaUrl = OwaUrl.ErrorPage;
			UserContext userContext = (owaContext != null) ? owaContext.TryGetUserContext() : null;
			ObjectNotFoundException ex = exception as ObjectNotFoundException;
			OwaEventHandlerException ex2 = exception as OwaEventHandlerException;
			if (ex2 != null)
			{
				message = ex2.Description;
				owaEventHandlerErrorCode = ex2.ErrorCode;
				hideDebugInformation = (ex2.HideDebugInformation || ex2.ErrorCode == OwaEventHandlerErrorCode.ConflictResolution);
			}
			else if (exception is OwaNotSupportedException)
			{
				message = exception.Message;
				hideDebugInformation = true;
			}
			else if (exception is OwaClientNotSupportedException)
			{
				message = LocalizedStrings.GetNonEncoded(427734258);
				hideDebugInformation = true;
				sendWatsonReport = false;
			}
			else if (exception is OwaExistentNotificationPipeException)
			{
				message = LocalizedStrings.GetNonEncoded(1295605912);
				sendWatsonReport = false;
				hideDebugInformation = !Globals.SendClientWatsonReports;
				owaEventHandlerErrorCode = OwaEventHandlerErrorCode.ExistentNotificationPipeError;
			}
			else if (exception is OwaNotificationPipeException)
			{
				message = LocalizedStrings.GetNonEncoded(-771052428);
				sendWatsonReport = false;
			}
			else if (exception is OwaOperationNotSupportedException)
			{
				message = exception.Message;
				hideDebugInformation = true;
			}
			else if (exception is OwaADObjectNotFoundException)
			{
				if (userContext != null)
				{
					userContext.PreferredDC = string.Empty;
				}
				hideDebugInformation = true;
				message = LocalizedStrings.GetNonEncoded(-950823100);
				messageDetails = LocalizedStrings.GetNonEncoded(970481710);
			}
			else if (exception is OwaInvalidCanary14Exception || exception is OwaCanaryException)
			{
				owaContext.HttpContext.Response.AppendToLog(Utilities.FormatExceptionNameAndMessage("&exception=", exception));
				hideDebugInformation = true;
				message = LocalizedStrings.GetNonEncoded(222445511);
				messageDetails = LocalizedStrings.GetNonEncoded(-1291627735);
				externalPageLink = null;
				OwaInvalidCanary14Exception ex3 = exception as OwaInvalidCanary14Exception;
				if (ex3 != null)
				{
					UserContextCookie userContextCookie = ex3.UserContextCookie;
					if (userContextCookie != null)
					{
						Utilities.DeleteCookie(owaContext.HttpContext.Response, userContextCookie.CookieName);
						UserContextCookie userContextCookie2 = userContextCookie.CloneWithNewCanary();
						owaContext.HttpContext.Response.Cookies.Set(userContextCookie2.HttpCookie);
					}
				}
			}
			else if (exception is OwaLockTimeoutException)
			{
				hideDebugInformation = true;
				OwaSingleCounters.RequestTimeouts.Increment();
				message = LocalizedStrings.GetNonEncoded(-116001901);
				if (owaContext != null)
				{
					owaContext.HttpContext.Response.AppendToLog("&s=ReqTimeout");
				}
			}
			else if (isWebPartRequest && ex != null)
			{
				sendWatsonReport = false;
				hideDebugInformation = true;
				message = LocalizedStrings.GetNonEncoded(1622692336);
				owaEventHandlerErrorCode = OwaEventHandlerErrorCode.WebPartActionPermissionsError;
			}
			else if (exception is OwaLostContextException)
			{
				hideDebugInformation = true;
				message = LocalizedStrings.GetNonEncoded(-477257421);
			}
			else if (ex != null || exception is ObjectNotFoundException)
			{
				if (exception is ObjectNotFoundException)
				{
					icon = ThemeFileId.Warning;
					hideDebugInformation = true;
				}
				if (exception.InnerException != null && exception.InnerException is DataValidationException)
				{
					message = LocalizedStrings.GetNonEncoded(404614840);
				}
				else
				{
					hideDebugInformation = true;
					message = LocalizedStrings.GetNonEncoded(-289549140);
					messageDetails = LocalizedStrings.GetNonEncoded(-1807976350);
				}
			}
			else if (exception is OwaBodyConversionFailedException)
			{
				message = LocalizedStrings.GetNonEncoded(1825027020);
			}
			else if (exception is ObjectExistedException)
			{
				hideDebugInformation = true;
				message = LocalizedStrings.GetNonEncoded(-1399945920);
				owaEventHandlerErrorCode = OwaEventHandlerErrorCode.FolderNameExists;
			}
			else if (exception is OwaDelegatorMailboxFailoverException)
			{
				sendWatsonReport = false;
				hideDebugInformation = true;
				message = LocalizedStrings.GetNonEncoded(1005365831);
				owaEventHandlerErrorCode = OwaEventHandlerErrorCode.MailboxFailoverWithoutRedirection;
			}
			else if (exception is OwaArchiveInTransitException)
			{
				sendWatsonReport = false;
				hideDebugInformation = true;
				message = LocalizedStrings.GetNonEncoded(-1086762792);
				owaEventHandlerErrorCode = OwaEventHandlerErrorCode.MailboxInTransitError;
				OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_ArchiveMailboxAccessFailedWarning, string.Empty, new object[]
				{
					mailboxIdentity.SafeGetRenderableName(),
					exception.ToString()
				});
			}
			else if (exception is OwaArchiveNotAvailableException)
			{
				hideDebugInformation = true;
				message = LocalizedStrings.GetNonEncoded(604008388);
				OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_ArchiveMailboxAccessFailedWarning, string.Empty, new object[]
				{
					mailboxIdentity.SafeGetRenderableName(),
					exception.ToString()
				});
			}
			else if (exception is MailboxInSiteFailoverException)
			{
				sendWatsonReport = false;
				hideDebugInformation = true;
				message = LocalizedStrings.GetNonEncoded(26604436);
				owaEventHandlerErrorCode = OwaEventHandlerErrorCode.MailboxFailoverWithoutRedirection;
			}
			else if (exception is MailboxCrossSiteFailoverException || exception is WrongServerException)
			{
				sendWatsonReport = false;
				hideDebugInformation = true;
				ExTraceGlobals.CoreTracer.TraceDebug(0L, "No CAS server is available for redirection or proxy");
				message = LocalizedStrings.GetNonEncoded(26604436);
				owaEventHandlerErrorCode = OwaEventHandlerErrorCode.MailboxFailoverWithoutRedirection;
			}
			else if (exception is MailboxInTransitException)
			{
				sendWatsonReport = false;
				hideDebugInformation = true;
				message = LocalizedStrings.GetNonEncoded(-1739093686);
				owaEventHandlerErrorCode = OwaEventHandlerErrorCode.MailboxInTransitError;
			}
			else if (exception is ResourceUnhealthyException)
			{
				sendWatsonReport = false;
				hideDebugInformation = true;
				message = LocalizedStrings.GetNonEncoded(198161982);
				OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_ErrorResourceUnhealthy, string.Empty, new object[]
				{
					mailboxIdentity.SafeGetRenderableName(),
					exception.ToString()
				});
			}
			else if (exception is ConnectionFailedPermanentException || exception is ServerNotFoundException)
			{
				message = string.Format(LocalizedStrings.GetNonEncoded(-765910865), mailboxIdentity.SafeGetRenderableName());
			}
			else if (exception is ConnectionFailedTransientException || exception is MailboxOfflineException)
			{
				message = LocalizedStrings.GetNonEncoded(198161982);
				hideDebugInformation = true;
				Utilities.RegisterMailboxException(userContext, exception);
			}
			else if (exception is InvalidLicenseException)
			{
				message = string.Format(LocalizedStrings.GetNonEncoded(468041898), mailboxIdentity.SafeGetRenderableName());
				hideDebugInformation = true;
				sendWatsonReport = false;
			}
			else if (exception is InstantMessagingException)
			{
				if ((exception as InstantMessagingException).Code == 18204)
				{
					message = LocalizedStrings.GetNonEncoded(-374220215);
					OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_FailedToEstablishIMConnection, string.Empty, new object[]
					{
						(exception.Message != null) ? exception.Message : string.Empty
					});
					sendWatsonReport = false;
					hideDebugInformation = true;
				}
				else
				{
					sendWatsonReport = true;
					message = LocalizedStrings.GetNonEncoded(641346049);
					owaEventHandlerErrorCode = OwaEventHandlerErrorCode.UnexpectedError;
				}
			}
			else if (exception is SendAsDeniedException)
			{
				message = LocalizedStrings.GetNonEncoded(2059222100);
				hideDebugInformation = true;
			}
			else if (exception is ADTransientException)
			{
				if (userContext != null)
				{
					userContext.PreferredDC = string.Empty;
				}
				message = LocalizedStrings.GetNonEncoded(634294555);
			}
			else if (exception is ADOperationException)
			{
				if (userContext != null)
				{
					userContext.PreferredDC = string.Empty;
				}
				message = LocalizedStrings.GetNonEncoded(-256207770);
			}
			else if (exception is DataValidationException)
			{
				if (userContext != null)
				{
					userContext.PreferredDC = string.Empty;
				}
				message = LocalizedStrings.GetNonEncoded(-256207770);
			}
			else if (exception is InvalidObjectOperationException)
			{
				if (userContext != null)
				{
					userContext.PreferredDC = string.Empty;
				}
				Exception innerException = exception.InnerException;
				string text = string.Empty;
				if (innerException != null)
				{
					text = innerException.GetType().ToString() + ": " + innerException.ToString();
				}
				OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_MailboxServerVersionConfiguration, mailboxIdentity.SafeGetRenderableName(), new object[]
				{
					text
				});
				message = LocalizedStrings.GetNonEncoded(578437863);
			}
			else if (exception is SaveConflictException || exception is OwaSaveConflictException)
			{
				hideDebugInformation = true;
				message = LocalizedStrings.GetNonEncoded(-482397486);
				owaEventHandlerErrorCode = OwaEventHandlerErrorCode.ConflictResolution;
			}
			else if (exception is FolderSaveException)
			{
				message = LocalizedStrings.GetNonEncoded(1487149567);
			}
			else if (exception is RecurrenceFormatException)
			{
				message = LocalizedStrings.GetNonEncoded(2014226498);
			}
			else if (exception is ObjectValidationException)
			{
				message = LocalizedStrings.GetNonEncoded(-1670564952);
			}
			else if (exception is CorruptDataException)
			{
				if (exception is PropertyValidationException && Utilities.CheckForDLMemberSizeTooLargeConstraint(((PropertyValidationException)exception).PropertyValidationErrors))
				{
					message = LocalizedStrings.GetNonEncoded(1763264010);
					hideDebugInformation = true;
				}
				else
				{
					message = LocalizedStrings.GetNonEncoded(-1670564952);
				}
			}
			else if (exception is InvalidSharingMessageException || exception is InvalidSharingDataException || exception is InvalidExternalSharingInitiatorException)
			{
				sendWatsonReport = false;
				hideDebugInformation = true;
				message = LocalizedStrings.GetNonEncoded(-1805619908);
			}
			else if (exception is OccurrenceCrossingBoundaryException)
			{
				message = LocalizedStrings.GetNonEncoded(-921576274);
				hideDebugInformation = true;
			}
			else if (exception is OccurrenceTimeSpanTooBigException)
			{
				message = LocalizedStrings.GetNonEncoded(466060253);
				hideDebugInformation = true;
			}
			else if (exception is ParserException)
			{
				message = LocalizedStrings.GetNonEncoded(1991715079);
				hideDebugInformation = true;
			}
			else if (exception is RecurrenceEndDateTooBigException)
			{
				hideDebugInformation = true;
				message = LocalizedStrings.GetNonEncoded(-1642530753);
			}
			else if (exception is RecurrenceStartDateTooSmallException)
			{
				hideDebugInformation = true;
				message = LocalizedStrings.GetNonEncoded(-323965365);
			}
			else if (exception is RecurrenceHasNoOccurrenceException)
			{
				hideDebugInformation = true;
				message = LocalizedStrings.GetNonEncoded(1564162812);
			}
			else if (exception is QuotaExceededException || exception is MessageTooBigException)
			{
				hideDebugInformation = true;
				message = LocalizedStrings.GetNonEncoded(-640701623);
			}
			else if (exception is SubmissionQuotaExceededException)
			{
				hideDebugInformation = true;
				message = LocalizedStrings.GetNonEncoded(178029729);
			}
			else if (exception is MessageSubmissionExceededException)
			{
				hideDebugInformation = true;
				message = LocalizedStrings.GetNonEncoded(-1381793955);
			}
			else if (exception is AttachmentExceededException)
			{
				hideDebugInformation = true;
				message = LocalizedStrings.GetNonEncoded(-2137146650);
			}
			else if (exception is ResourcesException || exception is NoMoreConnectionsException)
			{
				message = LocalizedStrings.GetNonEncoded(-639453714);
			}
			else if (exception is AccountDisabledException)
			{
				message = LocalizedStrings.GetNonEncoded(531497785);
			}
			else if (isWebPartRequest && exception is OwaDefaultFolderIdUnavailableException)
			{
				sendWatsonReport = false;
				hideDebugInformation = true;
				message = LocalizedStrings.GetNonEncoded(1622692336);
				owaEventHandlerErrorCode = OwaEventHandlerErrorCode.WebPartActionPermissionsError;
			}
			else if (exception is OwaAccessDeniedException)
			{
				sendWatsonReport = false;
				hideDebugInformation = true;
				message = exception.Message;
				if (((OwaAccessDeniedException)exception).IsWebPartFailure)
				{
					owaEventHandlerErrorCode = OwaEventHandlerErrorCode.WebPartActionPermissionsError;
				}
			}
			else if (exception is AccessDeniedException)
			{
				message = LocalizedStrings.GetNonEncoded(995407892);
				if (isWebPartRequest)
				{
					sendWatsonReport = false;
					message = LocalizedStrings.GetNonEncoded(1622692336);
					owaEventHandlerErrorCode = OwaEventHandlerErrorCode.WebPartActionPermissionsError;
				}
				else
				{
					AccessDeniedException ex4 = (AccessDeniedException)exception;
					if (ex4.InnerException != null)
					{
						Exception innerException2 = ex4.InnerException;
						if (innerException2 is MapiExceptionPasswordChangeRequired || innerException2 is MapiExceptionPasswordExpired)
						{
							message = LocalizedStrings.GetNonEncoded(540943741);
						}
					}
				}
				hideDebugInformation = true;
			}
			else if (isWebPartRequest && exception is ArgumentNullException)
			{
				sendWatsonReport = false;
				hideDebugInformation = true;
				message = LocalizedStrings.GetNonEncoded(1622692336);
				owaEventHandlerErrorCode = OwaEventHandlerErrorCode.WebPartActionPermissionsError;
			}
			else if (isWebPartRequest && exception is StoragePermanentException)
			{
				sendWatsonReport = false;
				hideDebugInformation = true;
				message = LocalizedStrings.GetNonEncoded(1622692336);
				owaEventHandlerErrorCode = OwaEventHandlerErrorCode.WebPartActionPermissionsError;
			}
			else if (exception is PropertyErrorException)
			{
				message = LocalizedStrings.GetNonEncoded(641346049);
			}
			else if (exception is OwaInstantMessageEventHandlerTransientException)
			{
				message = LocalizedStrings.GetNonEncoded(-1611030258);
				sendWatsonReport = false;
				hideDebugInformation = true;
			}
			else if (exception is OwaUserNotIMEnabledException)
			{
				message = exception.Message;
				sendWatsonReport = false;
				hideDebugInformation = true;
				owaEventHandlerErrorCode = OwaEventHandlerErrorCode.UserNotIMEnabled;
			}
			else if (exception is OwaIMOperationNotAllowedToSelf)
			{
				message = exception.Message;
				sendWatsonReport = false;
				hideDebugInformation = true;
				owaEventHandlerErrorCode = OwaEventHandlerErrorCode.IMOperationNotAllowedToSelf;
			}
			else if (exception is OwaInvalidOperationException)
			{
				message = LocalizedStrings.GetNonEncoded(641346049);
			}
			else if (exception is CorruptDataException)
			{
				icon = ThemeFileId.Warning;
				message = LocalizedStrings.GetNonEncoded(-1670564952);
			}
			else if (exception is AccessDeniedException)
			{
				icon = ThemeFileId.Warning;
				if (!isBasicAuthentication)
				{
					message = LocalizedStrings.GetNonEncoded(-1177184444);
				}
				else
				{
					message = LocalizedStrings.GetNonEncoded(234621291);
				}
				hideDebugInformation = true;
			}
			else if (exception is ConnectionException)
			{
				icon = ThemeFileId.Warning;
				message = LocalizedStrings.GetNonEncoded(678272416);
				hideDebugInformation = true;
			}
			else if (exception is PropertyErrorException)
			{
				icon = ThemeFileId.Warning;
				message = LocalizedStrings.GetNonEncoded(-566073559);
				hideDebugInformation = true;
			}
			else if (exception is PathTooLongException)
			{
				icon = ThemeFileId.Warning;
				message = LocalizedStrings.GetNonEncoded(-785304559);
				hideDebugInformation = true;
			}
			else if (exception is UnknownErrorException || exception is DocumentLibraryException)
			{
				icon = ThemeFileId.Warning;
				message = LocalizedStrings.GetNonEncoded(-785304559);
				hideDebugInformation = true;
			}
			else if (exception is OwaChangePasswordTransientException)
			{
				message = Strings.ChangePasswordTransientError;
				messageDetails = exception.Message;
				hideDebugInformation = true;
			}
			else if (exception is OwaSpellCheckerException)
			{
				message = LocalizedStrings.GetNonEncoded(1615042268);
			}
			else if (exception is VirusDetectedException)
			{
				message = LocalizedStrings.GetNonEncoded(-589723291);
			}
			else if (exception is VirusScanInProgressException)
			{
				message = LocalizedStrings.GetNonEncoded(-1019777596);
			}
			else if (exception is VirusMessageDeletedException)
			{
				message = LocalizedStrings.GetNonEncoded(1164605313);
			}
			else if (exception is OwaProxyException)
			{
				OwaProxyException ex5 = (OwaProxyException)exception;
				message = ex5.LocalizedError;
				hideDebugInformation = ex5.HideDebugInformation;
			}
			else if (exception is OwaExplicitLogonException)
			{
				OwaExplicitLogonException ex6 = (OwaExplicitLogonException)exception;
				icon = ThemeFileId.Warning;
				message = ex6.LocalizedError;
				hideDebugInformation = true;
			}
			else if (exception is OwaInvalidWebPartRequestException)
			{
				sendWatsonReport = false;
				hideDebugInformation = true;
			}
			else if (exception is OwaNoReplicaOfCurrentServerVersionException)
			{
				hideDebugInformation = true;
				message = LocalizedStrings.GetNonEncoded(-448460673);
			}
			else if (exception is OwaNoReplicaException)
			{
				hideDebugInformation = true;
				message = LocalizedStrings.GetNonEncoded(1179266056);
			}
			else if (exception is TranscodingServerBusyException)
			{
				message = LocalizedStrings.GetNonEncoded(162094648);
			}
			else if (exception is TranscodingUnconvertibleFileException)
			{
				message = LocalizedStrings.GetNonEncoded(794771794);
			}
			else if (exception is TranscodingFatalFaultException)
			{
				message = LocalizedStrings.GetNonEncoded(-211811108);
			}
			else if (exception is TranscodingOverMaximumFileSizeException)
			{
				message = LocalizedStrings.GetNonEncoded(-148502085);
			}
			else if (exception is TranscodingTimeoutException)
			{
				message = LocalizedStrings.GetNonEncoded(1972219525);
			}
			else if (exception is TranscodingErrorFileException)
			{
				message = LocalizedStrings.GetNonEncoded(-437471318);
			}
			else if (exception is NoReplicaException)
			{
				hideDebugInformation = true;
				message = LocalizedStrings.GetNonEncoded(1179266056);
			}
			else
			{
				if (exception is StorageTransientException)
				{
					message = LocalizedStrings.GetNonEncoded(-238819799);
					Utilities.RegisterMailboxException(userContext, exception);
					hideDebugInformation = !Globals.SendClientWatsonReports;
					if (!(exception.InnerException is MapiExceptionRpcServerTooBusy))
					{
						goto IL_118F;
					}
					sendWatsonReport = false;
					string text2 = string.Empty;
					try
					{
						try
						{
							if (userContext != null && userContext.HasValidMailboxSession())
							{
								text2 = userContext.MailboxSession.MailboxOwner.MailboxInfo.Location.ServerFqdn;
							}
						}
						catch (StorageTransientException)
						{
						}
						catch (StoragePermanentException)
						{
						}
						goto IL_118F;
					}
					finally
					{
						OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_ErrorMailboxServerTooBusy, string.Empty, new object[]
						{
							text2,
							mailboxIdentity.SafeGetRenderableName(),
							exception.ToString()
						});
					}
				}
				if (exception is RulesTooBigException)
				{
					message = LocalizedStrings.GetNonEncoded(-791981113);
					hideDebugInformation = true;
				}
				else if (exception is DuplicateActionException)
				{
					hideDebugInformation = true;
					message = LocalizedStrings.GetNonEncoded(-555068615);
				}
				else if (exception is ConversionFailedException && ((ConversionFailedException)exception).ConversionFailureReason == ConversionFailureReason.CorruptContent)
				{
					hideDebugInformation = true;
					message = LocalizedStrings.GetNonEncoded(-1670564952);
				}
				else if (exception is RightsManagementPermanentException)
				{
					sendWatsonReport = false;
					hideDebugInformation = true;
					RightsManagementPermanentException ex7 = exception as RightsManagementPermanentException;
					RightsManagementFailureCode failureCode = ex7.FailureCode;
					if (failureCode != RightsManagementFailureCode.UserRightNotGranted)
					{
						switch (failureCode)
						{
						case RightsManagementFailureCode.InternalLicensingDisabled:
							break;
						case RightsManagementFailureCode.ExternalLicensingDisabled:
							message = string.Format(LocalizedStrings.GetNonEncoded(1397740097), string.Empty);
							goto IL_118F;
						default:
							switch (failureCode)
							{
							case RightsManagementFailureCode.ServerRightNotGranted:
								message = LocalizedStrings.GetNonEncoded(784482022);
								goto IL_118F;
							case RightsManagementFailureCode.FeatureDisabled:
								goto IL_E04;
							}
							hideDebugInformation = false;
							message = exception.Message;
							goto IL_118F;
						}
						IL_E04:
						message = string.Format(LocalizedStrings.GetNonEncoded(1049269714), string.Empty);
					}
					else
					{
						message = LocalizedStrings.GetNonEncoded(1508237301);
					}
				}
				else if (exception is IOException && Utilities.IsDiskFullException(exception))
				{
					hideDebugInformation = true;
					sendWatsonReport = false;
					message = LocalizedStrings.GetNonEncoded(-1729839551);
				}
				else if (exception is StoragePermanentException)
				{
					message = LocalizedStrings.GetNonEncoded(861904327);
				}
				else if (exception is TransientException)
				{
					message = LocalizedStrings.GetNonEncoded(-1729839551);
				}
				else if (exception is HttpException)
				{
					HttpException ex8 = (HttpException)exception;
					message = string.Format(LocalizedStrings.GetNonEncoded(1331629462), ex8.GetHttpCode());
				}
				else if (exception is OwaInvalidConfigurationException)
				{
					hideDebugInformation = true;
					message = exception.Message;
				}
				else if ((exception is OwaAsyncOperationException && exception.InnerException != null && exception.InnerException is OwaAsyncRequestTimeoutException) || exception is OwaAsyncRequestTimeoutException)
				{
					hideDebugInformation = true;
					message = LocalizedStrings.GetNonEncoded(-793045165);
				}
				else if (exception is OwaNeedsSMimeControlToEditDraftException)
				{
					message = exception.Message;
					hideDebugInformation = true;
				}
				else if (exception is OwaCannotEditIrmDraftException)
				{
					message = exception.Message;
					hideDebugInformation = true;
				}
				else if (exception is OverBudgetException)
				{
					sendWatsonReport = false;
					hideDebugInformation = true;
					if (Utilities.IsOehOrSubPageContentRequest(OwaContext.Current))
					{
						message = LocalizedStrings.GetNonEncoded(1856724252);
					}
					else
					{
						message = LocalizedStrings.GetNonEncoded(-1416371944);
						messageDetails = LocalizedStrings.GetNonEncoded(1856724252);
					}
				}
				else if (exception is OwaBrowserUpdateRequiredException)
				{
					sendWatsonReport = false;
					hideDebugInformation = true;
					icon = ThemeFileId.WarningIcon;
					background = ThemeFileId.ErrorBackground;
					message = LocalizedStrings.GetNonEncoded(-1348879678);
					if (!Utilities.IsOehOrSubPageContentRequest(OwaContext.Current))
					{
						messageDetails = ((OwaBrowserUpdateRequiredException)exception).GetErrorDetails();
					}
					isDetailedErrorHtmlEncoded = true;
					owaUrl = OwaUrl.Error2Page;
				}
				else if (exception is OwaDisabledException)
				{
					sendWatsonReport = false;
					hideDebugInformation = true;
					icon = ThemeFileId.WarningIcon;
					background = ThemeFileId.ErrorBackground;
					message = LocalizedStrings.GetNonEncoded(1028401106);
					messageDetails = LocalizedStrings.GetNonEncoded(1613045632);
					isDetailedErrorHtmlEncoded = true;
					owaUrl = OwaUrl.Error2Page;
				}
				else if (exception is OwaLightDisabledException)
				{
					sendWatsonReport = false;
					hideDebugInformation = true;
					icon = ThemeFileId.WarningIcon;
					background = ThemeFileId.ErrorBackground;
					message = LocalizedStrings.GetNonEncoded(1028401106);
					messageDetails = LocalizedStrings.GetNonEncoded(-1048443402);
					isDetailedErrorHtmlEncoded = true;
					owaUrl = OwaUrl.Error2Page;
				}
				else if (exception is COMException || exception.InnerException is COMException)
				{
					sendWatsonReport = !Utilities.ShouldIgnoreException((exception is COMException) ? exception : exception.InnerException);
					hideDebugInformation = true;
					message = LocalizedStrings.GetNonEncoded(641346049);
					owaEventHandlerErrorCode = OwaEventHandlerErrorCode.UnexpectedError;
				}
				else if (exception is OwaSharedFromOlderVersionException)
				{
					sendWatsonReport = false;
					hideDebugInformation = true;
					message = LocalizedStrings.GetHtmlEncoded(1354015881);
				}
				else if (exception is OwaRespondOlderVersionMeetingException)
				{
					sendWatsonReport = false;
					hideDebugInformation = true;
					message = string.Format(CultureInfo.CurrentCulture, LocalizedStrings.GetNonEncoded(1896884103), new object[]
					{
						((OwaRespondOlderVersionMeetingException)exception).SharerDisplayName
					});
				}
				else if (exception is ThreadAbortException)
				{
					sendWatsonReport = false;
					message = LocalizedStrings.GetHtmlEncoded(641346049);
				}
				else if (exception is OwaCreateClientSecurityContextFailedException)
				{
					sendWatsonReport = false;
					hideDebugInformation = true;
					message = LocalizedStrings.GetHtmlEncoded(484783375);
				}
				else if (exception is OwaUnsupportedConversationItemException)
				{
					sendWatsonReport = false;
					hideDebugInformation = true;
					message = LocalizedStrings.GetHtmlEncoded(-1147215991);
				}
				else if (exception is OwaURLIsOutOfDateException)
				{
					sendWatsonReport = false;
					hideDebugInformation = true;
					message = string.Format(LocalizedStrings.GetNonEncoded(516417563), OwaContext.Current.LocalHostName);
					messageDetails = LocalizedStrings.GetNonEncoded(-1085493500);
				}
				else if (exception is WrongCASServerBecauseOfOutOfDateDNSCacheException)
				{
					sendWatsonReport = false;
					hideDebugInformation = true;
					message = LocalizedStrings.GetNonEncoded(-23402676);
				}
				else
				{
					sendWatsonReport = true;
					message = LocalizedStrings.GetNonEncoded(641346049);
					owaEventHandlerErrorCode = OwaEventHandlerErrorCode.UnexpectedError;
				}
			}
			IL_118F:
			return new ErrorInformation
			{
				Exception = exception,
				Message = message,
				MessageDetails = messageDetails,
				OwaEventHandlerErrorCode = owaEventHandlerErrorCode,
				HideDebugInformation = hideDebugInformation,
				IsErrorMessageHtmlEncoded = isErrorMessageHtmlEncoded,
				IsDetailedErrorHtmlEncoded = isDetailedErrorHtmlEncoded,
				SendWatsonReport = sendWatsonReport,
				Icon = icon,
				Background = background,
				OwaUrl = owaUrl,
				PreviousPageUrl = previousPageUrl,
				ExternalPageLink = externalPageLink
			};
		}

		public static void RenderDebugInformation(TextWriter writer, OwaContext owaContext, Exception exception)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (owaContext == null)
			{
				throw new ArgumentNullException("owaContext");
			}
			HttpRequest request = owaContext.HttpContext.Request;
			Utilities.RenderDebugHeader(writer, "Request");
			Utilities.RenderDebugInformation(writer, "requestUrl", "Url", request.Url.OriginalString);
			Utilities.RenderDebugInformation(writer, "userHostAddress", "User host address", request.UserHostAddress);
			if (owaContext.ExchangePrincipal != null)
			{
				Utilities.RenderDebugInformation(writer, "userName", "User", owaContext.ExchangePrincipal.MailboxInfo.DisplayName);
				Utilities.RenderDebugInformation(writer, "exAddress", "EX Address", owaContext.ExchangePrincipal.LegacyDn);
				Utilities.RenderDebugInformation(writer, "smtpAddress", "SMTP Address", owaContext.ExchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString());
			}
			Utilities.RenderDebugInformation(writer, "owaVersion", "OWA version", Globals.ApplicationVersion);
			if (owaContext.TryGetUserContext() != null && owaContext.UserContext.IsSafeToAccessFromCurrentThread())
			{
				UserContext userContext = owaContext.UserContext;
				if (!userContext.IsProxy)
				{
					if (!userContext.HasValidMailboxSession())
					{
						goto IL_16C;
					}
					try
					{
						Utilities.RenderDebugInformation(writer, "mailboxServer", "Mailbox server", userContext.MailboxSession.MailboxOwner.MailboxInfo.Location.ServerFqdn);
						goto IL_16C;
					}
					catch (StorageTransientException)
					{
						goto IL_16C;
					}
					catch (StoragePermanentException)
					{
						goto IL_16C;
					}
				}
				if (owaContext.SecondCasUri != null)
				{
					Utilities.RenderDebugInformation(writer, "secondCAS", "Second CAS for proxy", owaContext.SecondCasUri.ToString());
				}
			}
			IL_16C:
			if (exception != null)
			{
				Utilities.RenderDebugHeader(writer, "Exception");
				Utilities.RenderExceptionInformation(writer, exception);
				Exception innerException = exception.InnerException;
				int num = 0;
				while (innerException != null && num < 4)
				{
					Utilities.RenderDebugHeader(writer, "Inner Exception");
					Utilities.RenderExceptionInformation(writer, innerException);
					innerException = innerException.InnerException;
					num++;
				}
			}
		}

		private static void RenderExceptionInformation(TextWriter writer, Exception exception)
		{
			Utilities.RenderDebugInformation(writer, "exceptionType", "Exception type", exception.GetType().ToString());
			Utilities.RenderDebugInformation(writer, "exceptionMessage", "Exception message", exception.Message);
			Utilities.RenderDebugHeader(writer, "Call stack");
			if (exception.StackTrace == null)
			{
				writer.Write("<div><i>No callstack available</i></div>");
				return;
			}
			writer.Write("<div id=exceptionCallStack>");
			string text = " at ";
			string stackTrace = exception.StackTrace;
			int num = stackTrace.IndexOf(text, StringComparison.Ordinal);
			if (num == -1)
			{
				writer.Write(stackTrace);
			}
			else
			{
				num += text.Length;
				for (;;)
				{
					int num2 = stackTrace.IndexOf(text, num, StringComparison.Ordinal);
					if (num2 == -1)
					{
						break;
					}
					writer.Write("<div nowrap>");
					writer.Write(stackTrace.Substring(num, num2 - num));
					writer.Write("</div>");
					num = num2 + text.Length;
					if (num >= stackTrace.Length)
					{
						goto IL_DC;
					}
				}
				writer.Write(stackTrace.Substring(num));
			}
			IL_DC:
			writer.Write("</div>");
		}

		private static void RenderDebugInformation(TextWriter writer, string id, string label, string value)
		{
			writer.Write(string.Format("{0}: <span id={1}>{2}</span><br>", label, id, Utilities.HtmlEncode(value)));
		}

		private static void RenderDebugHeader(TextWriter writer, string label)
		{
			writer.Write(string.Format("<br><b>{0}</b><br>", label));
		}

		internal static string GetItemIdString(StoreObjectId itemId, Folder containerFolder)
		{
			return OwaStoreObjectId.CreateFromItemId(itemId, containerFolder).ToString();
		}

		internal static string GetItemIdString(StoreObjectId itemId, OwaStoreObjectId relatedOwaStoreObjectId)
		{
			return OwaStoreObjectId.CreateFromStoreObjectId(itemId, relatedOwaStoreObjectId).ToString();
		}

		internal static string GetIdAsString(StoreObject storeObject)
		{
			return OwaStoreObjectId.CreateFromStoreObject(storeObject).ToString();
		}

		internal static T GetItemForRequest<T>(OwaContext owaContext, out Item parentItem, params PropertyDefinition[] prefetchProperties) where T : Item
		{
			return Utilities.GetItemForRequest<T>(owaContext, out parentItem, false, prefetchProperties);
		}

		internal static T GetItemForRequest<T>(OwaContext owaContext, out Item parentItem, bool forceAsMessageItem, params PropertyDefinition[] prefetchProperties) where T : Item
		{
			return Utilities.GetItemById<T>(owaContext, out parentItem, Utilities.GetQueryStringParameter(owaContext.HttpContext.Request, "id"), forceAsMessageItem, prefetchProperties);
		}

		internal static T GetItemById<T>(OwaContext owaContext, out Item parentItem, string owaStoreObjectId, bool forceAsMessageItem, params PropertyDefinition[] prefetchProperties) where T : Item
		{
			if (owaContext == null)
			{
				throw new ArgumentNullException("owaContext");
			}
			if (forceAsMessageItem && typeof(T).Name != "MessageItem")
			{
				throw new ArgumentException("To force bind as a MessageItem, the typename T should be set to MessageItem.");
			}
			HttpContext httpContext = owaContext.HttpContext;
			UserContext userContext = owaContext.UserContext;
			bool flag = Utilities.GetQueryStringParameter(httpContext.Request, "attcnt", false) != null;
			Item item = null;
			parentItem = null;
			Attachment attachment = null;
			ItemAttachment itemAttachment = null;
			if (flag)
			{
				try
				{
					parentItem = Utilities.GetItem<Item>(userContext, owaStoreObjectId, prefetchProperties);
					if (userContext.IsIrmEnabled && !userContext.IsBasicExperience)
					{
						Utilities.IrmDecryptIfRestricted(parentItem, userContext, true);
					}
					attachment = Utilities.GetAttachment(parentItem, httpContext.Request, userContext);
					itemAttachment = (attachment as ItemAttachment);
					if (itemAttachment == null)
					{
						throw new OwaInvalidRequestException("Attachment is not an item attachment");
					}
					attachment = null;
					if (forceAsMessageItem)
					{
						item = itemAttachment.GetItemAsMessage(prefetchProperties);
					}
					else
					{
						item = itemAttachment.GetItem(prefetchProperties);
					}
					if (!(item is T))
					{
						throw new OwaInvalidRequestException("Wrong item class supplied");
					}
					OwaContext.Current.AddObjectToDisposeOnEndRequest(itemAttachment);
					goto IL_14C;
				}
				catch
				{
					if (parentItem != null)
					{
						parentItem.Dispose();
						parentItem = null;
					}
					if (item != null)
					{
						item.Dispose();
						item = null;
					}
					if (attachment != null)
					{
						attachment.Dispose();
						attachment = null;
					}
					if (itemAttachment != null)
					{
						itemAttachment.Dispose();
						itemAttachment = null;
					}
					throw;
				}
			}
			parentItem = null;
			OwaStoreObjectId owaStoreObjectId2 = OwaStoreObjectId.CreateFromString(owaStoreObjectId);
			item = Utilities.GetItem<T>(userContext, owaStoreObjectId2, forceAsMessageItem, prefetchProperties);
			IL_14C:
			return (T)((object)item);
		}

		internal static T GetItem<T>(UserContext userContext, string idString, params PropertyDefinition[] prefetchProperties) where T : Item
		{
			if (string.IsNullOrEmpty(idString))
			{
				throw new ArgumentNullException("idString");
			}
			OwaStoreObjectId owaStoreObjectId = OwaStoreObjectId.CreateFromString(idString);
			return Utilities.GetItem<T>(userContext, owaStoreObjectId, prefetchProperties);
		}

		internal static T GetItem<T>(UserContext userContext, string idString, string changeKey, params PropertyDefinition[] prefetchProperties) where T : Item
		{
			if (string.IsNullOrEmpty(idString))
			{
				throw new ArgumentNullException("idString");
			}
			if (changeKey == null)
			{
				throw new ArgumentNullException("changeKey");
			}
			OwaStoreObjectId owaStoreObjectId = OwaStoreObjectId.CreateFromString(idString);
			return Utilities.GetItem<T>(userContext, owaStoreObjectId, changeKey, prefetchProperties);
		}

		internal static T GetItem<T>(UserContext userContext, StoreObjectId storeObjectId, string changeKey, params PropertyDefinition[] prefetchProperties) where T : Item
		{
			if (storeObjectId == null)
			{
				throw new ArgumentNullException("storeObjectId");
			}
			if (changeKey == null)
			{
				throw new ArgumentNullException("changeKey");
			}
			return Utilities.GetItem<T>(userContext, Utilities.CreateItemId(userContext.MailboxSession, storeObjectId, changeKey), prefetchProperties);
		}

		internal static T GetItem<T>(UserContext userContext, StoreId storeId, params PropertyDefinition[] prefetchProperties) where T : Item
		{
			return Utilities.GetItem<T>(userContext, storeId, ItemBindOption.None, prefetchProperties);
		}

		internal static T GetItem<T>(UserContext userContext, StoreId storeId, ItemBindOption itemBindOption, params PropertyDefinition[] prefetchProperties) where T : Item
		{
			return Utilities.GetItem<T>(userContext, storeId, false, itemBindOption, prefetchProperties);
		}

		internal static T GetItem<T>(UserContext userContext, StoreId storeId, bool forceAsMessageItem, params PropertyDefinition[] prefetchProperties) where T : Item
		{
			return Utilities.GetItem<T>(userContext.MailboxSession, storeId, forceAsMessageItem, ItemBindOption.None, prefetchProperties);
		}

		internal static T GetItem<T>(UserContext userContext, StoreId storeId, bool forceAsMessageItem, ItemBindOption itemBindOption, params PropertyDefinition[] prefetchProperties) where T : Item
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (storeId == null)
			{
				throw new ArgumentNullException("storeId");
			}
			if (forceAsMessageItem && typeof(T).Name != "MessageItem")
			{
				throw new ArgumentException("To force bind as a MessageItem, the typename T should be set to MessageItem.");
			}
			return Utilities.GetItem<T>(userContext.MailboxSession, storeId, forceAsMessageItem, itemBindOption, prefetchProperties);
		}

		internal static T GetItem<T>(UserContext userContext, OwaStoreObjectId owaStoreObjectId, params PropertyDefinition[] prefetchProperties) where T : Item
		{
			return Utilities.GetItem<T>(userContext, owaStoreObjectId, false, prefetchProperties);
		}

		internal static T GetItem<T>(UserContext userContext, OwaStoreObjectId owaStoreObjectId, string changeKey, params PropertyDefinition[] prefetchProperties) where T : Item
		{
			return Utilities.GetItem<T>(userContext, owaStoreObjectId, changeKey, false, prefetchProperties);
		}

		internal static T GetItem<T>(UserContext userContext, OwaStoreObjectId owaStoreObjectId, bool forceAsMessageItem, params PropertyDefinition[] prefetchProperties) where T : Item
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (owaStoreObjectId == null)
			{
				throw new ArgumentNullException("owaStoreObjectId");
			}
			StoreSession session = owaStoreObjectId.GetSession(userContext);
			if (owaStoreObjectId.StoreObjectId == null)
			{
				throw new OwaInvalidRequestException("StoreObjectId is null");
			}
			return Utilities.GetItem<T>(session, owaStoreObjectId.StoreObjectId, forceAsMessageItem, prefetchProperties);
		}

		internal static T GetItem<T>(UserContext userContext, OwaStoreObjectId owaStoreObjectId, string changeKey, bool forceAsMessageItem, params PropertyDefinition[] prefetchProperties) where T : Item
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (owaStoreObjectId == null)
			{
				throw new ArgumentNullException("owaStoreObjectId");
			}
			if (changeKey == null)
			{
				throw new ArgumentNullException("changeKey");
			}
			StoreId storeId = Utilities.CreateItemId(owaStoreObjectId.StoreObjectId, changeKey);
			StoreSession session = owaStoreObjectId.GetSession(userContext);
			return Utilities.GetItem<T>(session, storeId, forceAsMessageItem, prefetchProperties);
		}

		internal static T GetItem<T>(StoreSession storeSession, StoreId storeId, bool forceAsMessageItem, params PropertyDefinition[] prefetchProperties) where T : Item
		{
			return Utilities.GetItem<T>(storeSession, storeId, forceAsMessageItem, ItemBindOption.None, prefetchProperties);
		}

		internal static T GetItem<T>(StoreSession storeSession, StoreId storeId, bool forceAsMessageItem, ItemBindOption itemBindOption, params PropertyDefinition[] prefetchProperties) where T : Item
		{
			if (storeSession == null)
			{
				throw new ArgumentNullException("storeSession");
			}
			if (storeId == null)
			{
				throw new ArgumentNullException("storeId");
			}
			if (forceAsMessageItem && typeof(T).Name != "MessageItem")
			{
				throw new ArgumentException("To force bind as a MessageItem, the typename T should be set to MessageItem.");
			}
			Type typeFromHandle = typeof(T);
			VersionedId versionedId = storeId as VersionedId;
			StoreObjectId storeObjectId;
			if (versionedId != null)
			{
				storeObjectId = versionedId.ObjectId;
			}
			else
			{
				storeObjectId = (StoreObjectId)storeId;
			}
			if (storeObjectId == null)
			{
				throw new OwaInvalidRequestException("The given item Id is null");
			}
			if (!IdConverter.IsMessageId(storeObjectId))
			{
				throw new OwaInvalidRequestException("The given Id is not a valid item Id. Item Id:" + storeObjectId);
			}
			Item item;
			try
			{
				if (typeFromHandle == typeof(MessageItem))
				{
					if (forceAsMessageItem)
					{
						item = Item.BindAsMessage(storeSession, storeId, prefetchProperties);
					}
					else
					{
						item = MessageItem.Bind(storeSession, storeId, itemBindOption, prefetchProperties);
					}
				}
				else if (typeFromHandle == typeof(CalendarItem))
				{
					item = Item.Bind(storeSession, storeId, itemBindOption, prefetchProperties);
				}
				else if (typeFromHandle == typeof(CalendarItemBase))
				{
					item = Item.Bind(storeSession, storeId, itemBindOption, prefetchProperties);
				}
				else if (typeFromHandle == typeof(SharingMessageItem))
				{
					item = SharingMessageItem.Bind(storeSession, storeId, prefetchProperties);
				}
				else if (typeFromHandle == typeof(Contact))
				{
					item = Item.Bind(storeSession, storeId, itemBindOption, prefetchProperties);
				}
				else if (typeFromHandle == typeof(DistributionList))
				{
					item = Item.Bind(storeSession, storeId, itemBindOption, prefetchProperties);
				}
				else if (typeFromHandle == typeof(MeetingRequest))
				{
					item = MessageItem.Bind(storeSession, storeId, itemBindOption, prefetchProperties);
				}
				else if (typeFromHandle == typeof(MeetingCancellation))
				{
					item = MessageItem.Bind(storeSession, storeId, itemBindOption, prefetchProperties);
				}
				else if (typeFromHandle == typeof(MeetingResponse))
				{
					item = MessageItem.Bind(storeSession, storeId, itemBindOption, prefetchProperties);
				}
				else if (typeFromHandle == typeof(Task))
				{
					item = Item.Bind(storeSession, storeId, itemBindOption, prefetchProperties);
				}
				else if (typeFromHandle == typeof(PostItem))
				{
					item = Item.Bind(storeSession, storeId, itemBindOption, prefetchProperties);
				}
				else
				{
					if (!(typeFromHandle == typeof(Item)))
					{
						throw new ArgumentException(typeFromHandle.ToString() + " is not a supported item type");
					}
					item = Item.Bind(storeSession, storeId, itemBindOption, prefetchProperties);
				}
			}
			catch (WrongObjectTypeException innerException)
			{
				throw new OwaInvalidRequestException("Wrong item class supplied", innerException);
			}
			if (!(item is T))
			{
				if (item != null)
				{
					item.Dispose();
				}
				throw new OwaInvalidRequestException("Item type is different than expected");
			}
			return (T)((object)item);
		}

		internal static T GetFolder<T>(UserContext userContext, OwaStoreObjectId folderId, params PropertyDefinition[] prefetchProperties) where T : Folder
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (folderId == null)
			{
				throw new ArgumentNullException("folderId");
			}
			StoreSession session = folderId.GetSession(userContext);
			return Utilities.GetFolder<T>(session, folderId.StoreObjectId, prefetchProperties);
		}

		internal static T GetFolderForContent<T>(UserContext userContext, OwaStoreObjectId folderId, params PropertyDefinition[] prefetchProperties) where T : Folder
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (folderId == null)
			{
				throw new ArgumentNullException("folderId");
			}
			StoreSession sessionForFolderContent = folderId.GetSessionForFolderContent(userContext);
			return Utilities.GetFolder<T>(sessionForFolderContent, folderId.StoreObjectId, prefetchProperties);
		}

		private static T GetFolder<T>(StoreSession storeSession, StoreObjectId folderId, params PropertyDefinition[] prefetchProperties) where T : Folder
		{
			Type typeFromHandle = typeof(T);
			Folder folder;
			if (typeFromHandle == typeof(CalendarFolder))
			{
				folder = CalendarFolder.Bind(storeSession, folderId, prefetchProperties);
			}
			else
			{
				folder = Folder.Bind(storeSession, folderId, prefetchProperties);
			}
			return (T)((object)folder);
		}

		internal static string GetParentFolderName(Item item, StoreObjectId parentFolderId, UserContext userContext)
		{
			string text = null;
			string legacyDN = null;
			string arg = null;
			OwaStoreObjectIdType owaStoreObjectIdType = Utilities.GetOwaStoreObjectIdType(userContext, item);
			if (owaStoreObjectIdType == OwaStoreObjectIdType.OtherUserMailboxObject)
			{
				legacyDN = Utilities.GetMailboxSessionLegacyDN(item);
				arg = Utilities.GetSessionMailboxDisplayName(item);
			}
			if (owaStoreObjectIdType == OwaStoreObjectIdType.ArchiveMailboxObject)
			{
				legacyDN = Utilities.GetMailboxSessionLegacyDN(item);
			}
			else if (owaStoreObjectIdType == OwaStoreObjectIdType.PublicStoreItem)
			{
				owaStoreObjectIdType = OwaStoreObjectIdType.PublicStoreFolder;
			}
			OwaStoreObjectId folderId = OwaStoreObjectId.CreateFromFolderId(parentFolderId, owaStoreObjectIdType, legacyDN);
			using (Folder folder = Utilities.GetFolder<Folder>(userContext, folderId, new PropertyDefinition[0]))
			{
				text = folder.DisplayName;
			}
			if (owaStoreObjectIdType == OwaStoreObjectIdType.OtherUserMailboxObject)
			{
				text = string.Format(LocalizedStrings.GetNonEncoded(-83764036), arg, text);
			}
			return text;
		}

		internal static StoreObjectId GetParentFolderId(Item parentItem, Item item)
		{
			StoreObjectId parentId;
			if (parentItem != null)
			{
				parentId = parentItem.ParentId;
			}
			else
			{
				parentId = item.ParentId;
			}
			return parentId;
		}

		internal static OwaStoreObjectId GetParentFolderId(OwaStoreObjectId itemId)
		{
			switch (itemId.OwaStoreObjectIdType)
			{
			case OwaStoreObjectIdType.PublicStoreFolder:
				return itemId;
			case OwaStoreObjectIdType.PublicStoreItem:
				return OwaStoreObjectId.CreateFromPublicFolderId(IdConverter.GetParentIdFromMessageId(itemId.StoreObjectId));
			case OwaStoreObjectIdType.Conversation:
				return OwaStoreObjectId.CreateFromFolderId(itemId.ParentFolderId, OwaStoreObjectIdType.MailBoxObject);
			case OwaStoreObjectIdType.OtherUserMailboxObject:
				return OwaStoreObjectId.CreateFromOtherUserMailboxFolderId(IdConverter.GetParentIdFromMessageId(itemId.StoreObjectId), itemId.MailboxOwnerLegacyDN);
			case OwaStoreObjectIdType.ArchiveMailboxObject:
				return OwaStoreObjectId.CreateFromArchiveMailboxFolderId(IdConverter.GetParentIdFromMessageId(itemId.StoreObjectId), itemId.MailboxOwnerLegacyDN);
			case OwaStoreObjectIdType.ArchiveConversation:
				return OwaStoreObjectId.CreateFromArchiveMailboxFolderId(itemId.ParentFolderId, itemId.MailboxOwnerLegacyDN);
			default:
				return OwaStoreObjectId.CreateFromFolderId(IdConverter.GetParentIdFromMessageId(itemId.StoreObjectId), OwaStoreObjectIdType.MailBoxObject);
			}
		}

		internal static T CreateItem<T>(OwaStoreObjectId folderId) where T : Item
		{
			Type typeFromHandle = typeof(T);
			StoreObjectType itemType;
			if (typeFromHandle == typeof(CalendarItem))
			{
				itemType = StoreObjectType.CalendarItem;
			}
			else if (typeFromHandle == typeof(Contact))
			{
				itemType = StoreObjectType.Contact;
			}
			else if (typeFromHandle == typeof(DistributionList))
			{
				itemType = StoreObjectType.DistributionList;
			}
			else if (typeFromHandle == typeof(Task))
			{
				itemType = StoreObjectType.Task;
			}
			else if (typeFromHandle == typeof(PostItem))
			{
				itemType = StoreObjectType.Post;
			}
			else
			{
				itemType = StoreObjectType.Message;
			}
			return Utilities.CreateItem(itemType, folderId) as T;
		}

		internal static Item CreateItem(StoreObjectType itemType, OwaStoreObjectId folderId)
		{
			UserContext userContext = OwaContext.Current.UserContext;
			StoreObjectId folderStoreObjectId = null;
			StoreSession storeSession;
			if (folderId == null)
			{
				storeSession = userContext.MailboxSession;
			}
			else
			{
				storeSession = folderId.GetSessionForFolderContent(userContext);
				folderStoreObjectId = folderId.StoreObjectId;
			}
			return Utilities.CreateItem(itemType, folderStoreObjectId, storeSession);
		}

		private static Item CreateItem(StoreObjectType itemType, StoreObjectId folderStoreObjectId, StoreSession storeSession)
		{
			UserContext userContext = OwaContext.Current.UserContext;
			Item item;
			if (itemType != StoreObjectType.Message)
			{
				switch (itemType)
				{
				case StoreObjectType.CalendarItem:
					if (folderStoreObjectId == null)
					{
						folderStoreObjectId = userContext.CalendarFolderId;
					}
					item = CalendarItem.Create(storeSession, folderStoreObjectId);
					item[ItemSchema.ConversationIndexTracking] = true;
					return item;
				case StoreObjectType.Contact:
					if (folderStoreObjectId == null)
					{
						folderStoreObjectId = userContext.ContactsFolderId;
					}
					return Contact.Create(storeSession, folderStoreObjectId);
				case StoreObjectType.DistributionList:
					if (folderStoreObjectId == null)
					{
						folderStoreObjectId = userContext.ContactsFolderId;
					}
					return DistributionList.Create(storeSession, folderStoreObjectId);
				case StoreObjectType.Task:
					if (folderStoreObjectId == null)
					{
						folderStoreObjectId = userContext.TasksFolderId;
					}
					return Task.Create(storeSession, folderStoreObjectId);
				case StoreObjectType.Post:
					return PostItem.Create(storeSession, folderStoreObjectId);
				}
			}
			item = MessageItem.Create(userContext.MailboxSession, userContext.DraftsFolderId);
			item[ItemSchema.ConversationIndexTracking] = true;
			return item;
		}

		internal static void SetPostSender(PostItem postItem, UserContext userContext, bool isTargetFolderPublic)
		{
			if (userContext.IsExplicitLogon && isTargetFolderPublic)
			{
				postItem.From = (postItem.Sender = new Participant(userContext.LogonIdentity.GetOWAMiniRecipient()));
				return;
			}
			postItem.From = (postItem.Sender = new Participant(userContext.ExchangePrincipal));
		}

		internal static Item CreateImplicitDraftItem(StoreObjectType itemType, OwaStoreObjectId destinationFolderId)
		{
			OwaStoreObjectId scratchPadForImplicitDraft = Utilities.GetScratchPadForImplicitDraft(itemType, destinationFolderId);
			Item item = Utilities.CreateItem(itemType, scratchPadForImplicitDraft);
			if (itemType == StoreObjectType.CalendarItem)
			{
				CalendarItem calendarItem = item as CalendarItem;
				calendarItem.StartTime = ExDateTime.Now.AddYears(1);
				calendarItem.EndTime = ExDateTime.Now.AddYears(1);
				Utilities.SaveItem(calendarItem);
				item.Load();
			}
			return item;
		}

		internal static OwaStoreObjectId GetScratchPadForImplicitDraft(StoreObjectType itemType, OwaStoreObjectId destinationFolderId)
		{
			UserContext userContext = OwaContext.Current.UserContext;
			if (itemType == StoreObjectType.Message)
			{
				return OwaStoreObjectId.CreateFromMailboxFolderId(userContext.DraftsFolderId);
			}
			if (destinationFolderId == null)
			{
				return null;
			}
			if (destinationFolderId.IsPublic)
			{
				return userContext.GetDeletedItemsFolderId(userContext.MailboxSession);
			}
			return destinationFolderId;
		}

		internal static T GetFolderProperty<T>(Folder folder, PropertyDefinition propertyDefinition, T defaultValue)
		{
			object obj = folder.TryGetProperty(propertyDefinition);
			if (obj is PropertyError || obj == null)
			{
				return defaultValue;
			}
			return (T)((object)obj);
		}

		internal static Attachment GetAttachment(Item item, HttpRequest request, UserContext userContext)
		{
			return Utilities.GetAttachment(item, request, null, userContext);
		}

		internal static Attachment GetAttachment(Item item, HttpRequest request, List<AttachmentId> attachmentIdList, UserContext userContext)
		{
			if (attachmentIdList == null)
			{
				attachmentIdList = new List<AttachmentId>();
			}
			Utilities.FillAttachmentIdList(item, request, attachmentIdList);
			return Utilities.GetAttachment(item, attachmentIdList, userContext);
		}

		internal static void FillAttachmentIdList(Item item, HttpRequest request, List<AttachmentId> attachmentIdList)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}
			int embeddedItemNestingLevel = AttachmentUtility.GetEmbeddedItemNestingLevel(request);
			for (int i = 0; i < embeddedItemNestingLevel; i++)
			{
				string name = "attid" + i.ToString(CultureInfo.InvariantCulture);
				string queryStringParameter = Utilities.GetQueryStringParameter(request, name);
				AttachmentId item2 = null;
				try
				{
					item2 = item.CreateAttachmentId(queryStringParameter);
				}
				catch (CorruptDataException innerException)
				{
					Utilities.ThrowInvalidIdFormatException(queryStringParameter, null, innerException);
				}
				attachmentIdList.Add(item2);
			}
		}

		internal static Attachment GetAttachment(Item item, List<AttachmentId> attachmentIdList, UserContext userContext)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (attachmentIdList == null)
			{
				throw new ArgumentNullException("attachmentIdList");
			}
			int count = attachmentIdList.Count;
			if (count == 0)
			{
				throw new ArgumentException("attachmentIdList");
			}
			for (int i = 0; i < count; i++)
			{
				AttachmentId id = attachmentIdList[i];
				AttachmentCollection attachmentCollection = Utilities.GetAttachmentCollection(item, true, userContext);
				bool flag = false;
				ItemAttachment itemAttachment = null;
				item = null;
				try
				{
					Attachment attachment = attachmentCollection.Open(id);
					if (i == count - 1)
					{
						flag = true;
						return attachment;
					}
					itemAttachment = (attachment as ItemAttachment);
					if (itemAttachment == null)
					{
						if (attachment != null)
						{
							attachment.Dispose();
						}
						throw new OwaInvalidRequestException("Attachment is not an item attachment");
					}
					item = itemAttachment.GetItem();
					if (userContext.IsIrmEnabled && !userContext.IsBasicExperience)
					{
						Utilities.IrmDecryptIfRestricted(item, userContext, true);
					}
				}
				finally
				{
					if (!flag)
					{
						if (itemAttachment != null)
						{
							OwaContext.Current.AddObjectToDisposeOnEndRequest(itemAttachment);
						}
						if (item != null)
						{
							OwaContext.Current.AddObjectToDisposeOnEndRequest(item);
						}
					}
				}
			}
			return null;
		}

		internal static void ValidateCalendarItemBaseStoreObject(CalendarItemBase calendarItemBase)
		{
			if (calendarItemBase == null)
			{
				throw new ArgumentNullException("calendarItemBase");
			}
			CalendarItemOccurrence calendarItemOccurrence = calendarItemBase as CalendarItemOccurrence;
			if (calendarItemOccurrence != null)
			{
				calendarItemOccurrence.OccurrencePropertyBag.MasterCalendarItem.Validate();
				return;
			}
			calendarItemBase.Validate();
		}

		private static bool CheckForDLMemberSizeTooLargeConstraint(PropertyValidationError[] validationErrors)
		{
			if (validationErrors == null)
			{
				return false;
			}
			foreach (PropertyValidationError propertyValidationError in validationErrors)
			{
				if (propertyValidationError.PropertyDefinition == DistributionListSchema.Members || propertyValidationError.PropertyDefinition == DistributionListSchema.OneOffMembers)
				{
					return true;
				}
			}
			return false;
		}

		internal static void SaveItem(Item item)
		{
			Utilities.SaveItem(item, true);
		}

		internal static void SaveItem(Item item, bool updatePerfCounter)
		{
			Utilities.SaveItem(item, updatePerfCounter, SaveMode.ResolveConflicts);
		}

		internal static void SaveItem(Item item, bool updatePerfCounter, SaveMode saveMode)
		{
			ConflictResolutionResult conflictResolutionResult = item.Save(saveMode);
			if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
			{
				ExTraceGlobals.CoreTracer.TraceDebug(0L, "Saving item failed due to conflict resolution.");
				throw new OwaEventHandlerException("ACR failed", LocalizedStrings.GetNonEncoded(-482397486), OwaEventHandlerErrorCode.ConflictResolution);
			}
			if (updatePerfCounter && Globals.ArePerfCountersEnabled)
			{
				OwaSingleCounters.ItemsUpdated.Increment();
			}
		}

		public static void WriteLatestUrlToAttachment(TextWriter writer, string itemId, string extension)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (itemId == null)
			{
				throw new ArgumentNullException("itemId");
			}
			if (extension == null)
			{
				extension = string.Empty;
			}
			writer.Write("attachment.ashx?attach=1&id=");
			Utilities.WriteDoubleEncodedStringToUrl(writer, itemId);
			writer.Write("&MSWMExt=");
			writer.Write(Utilities.UrlEncode(extension));
		}

		public static void WriteDoubleEncodedStringToUrl(TextWriter writer, string input)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (string.IsNullOrEmpty(input))
			{
				throw new ArgumentNullException("input", "input cannot be null or empty");
			}
			Encoding unicode = Encoding.Unicode;
			byte[] bytes = unicode.GetBytes(Utilities.UrlEncode(input));
			writer.Write(Convert.ToBase64String(bytes));
		}

		public static string WriteDoubleEncodedStringToUrl(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				throw new ArgumentNullException("input", "input cannot be null or empty");
			}
			Encoding unicode = Encoding.Unicode;
			byte[] bytes = unicode.GetBytes(Utilities.UrlEncode(input));
			return Convert.ToBase64String(bytes);
		}

		public static string GetStringfromBase64String(string base64Input)
		{
			if (string.IsNullOrEmpty(base64Input))
			{
				throw new ArgumentNullException("base64Input", "base64Input cannot be null or empty");
			}
			Encoding unicode = Encoding.Unicode;
			byte[] bytes = Convert.FromBase64String(base64Input);
			return HttpUtility.UrlDecode(unicode.GetString(bytes));
		}

		public static Guid GetGuidFromBase64String(string base64Input)
		{
			if (string.IsNullOrEmpty(base64Input))
			{
				throw new OwaInvalidRequestException("Missing Base64 String");
			}
			Guid result;
			try
			{
				result = new Guid(Convert.FromBase64String(base64Input));
			}
			catch (ArgumentException innerException)
			{
				throw new OwaInvalidRequestException("Invalid base64 string", innerException);
			}
			catch (FormatException innerException2)
			{
				throw new OwaInvalidRequestException("Invalid base64 string", innerException2);
			}
			catch (OverflowException innerException3)
			{
				throw new OwaInvalidRequestException("Invalid base64 string", innerException3);
			}
			return result;
		}

		public static string GetBase64StringFromGuid(Guid guid)
		{
			return Convert.ToBase64String(guid.ToByteArray());
		}

		public static string GetBase64StringFromADObjectId(ADObjectId adObjectId)
		{
			if (adObjectId == null)
			{
				throw new ArgumentNullException("adObjectId");
			}
			return Utilities.GetBase64StringFromGuid(adObjectId.ObjectGuid);
		}

		public static Stopwatch StartWatch()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			return stopwatch;
		}

		public static long StopWatch(Stopwatch watch, string traceMessage)
		{
			watch.Stop();
			ExTraceGlobals.CoreTracer.TraceDebug<string, long>(0L, "{0}: {1} ms.", traceMessage, watch.ElapsedMilliseconds);
			return watch.ElapsedMilliseconds;
		}

		public static void CropAndRenderText(TextWriter writer, string text, int maxCharacters)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (text == null)
			{
				throw new ArgumentNullException("text");
			}
			if (maxCharacters <= 0)
			{
				throw new ArgumentOutOfRangeException("maxCharacters", "maxCharacters has to be greater than zero");
			}
			int num = (maxCharacters < text.Length) ? maxCharacters : text.Length;
			Utilities.HtmlEncode(text.Substring(0, num), writer);
			if (num < text.Length)
			{
				writer.Write("...");
			}
		}

		public static string GetDefaultFontName()
		{
			CultureInfo userCulture = Microsoft.Exchange.Clients.Owa.Core.Culture.GetUserCulture();
			return Utilities.GetDefaultFontName(userCulture);
		}

		public static string GetDefaultFontName(CultureInfo userCulture)
		{
			if (Utilities.IsViet(userCulture))
			{
				return "Helvetica";
			}
			return "Tahoma";
		}

		public static bool IsViet()
		{
			CultureInfo userCulture = Microsoft.Exchange.Clients.Owa.Core.Culture.GetUserCulture();
			return Utilities.IsViet(userCulture);
		}

		public static bool IsViet(CultureInfo userCulture)
		{
			if (userCulture == null)
			{
				throw new ArgumentNullException("userCulture");
			}
			return userCulture.LCID == 1066;
		}

		public static void RenderDefaultFontNameIfNecessary(TextWriter writer)
		{
			if (Utilities.IsViet())
			{
				writer.Write(Utilities.GetDefaultFontName());
				writer.Write(", ");
			}
		}

		internal static string GenerateWhen(Item item)
		{
			MeetingMessage meetingMessage = item as MeetingMessage;
			if (meetingMessage != null)
			{
				return meetingMessage.GenerateWhen(CultureInfo.CurrentCulture);
			}
			CalendarItemBase calendarItemBase = item as CalendarItemBase;
			if (calendarItemBase != null)
			{
				return calendarItemBase.GenerateWhen();
			}
			throw new ArgumentException("Unsupported type, this is a bug");
		}

		public static bool IsDownLevelClient(HttpRequest request)
		{
			if (request == null)
			{
				throw new ArgumentNullException("request cannot be null");
			}
			ExTraceGlobals.CoreCallTracer.TraceDebug<string>(0L, "Utilities.IsDownLevelClient. user-agent = {0}", (request.UserAgent == null) ? string.Empty : request.UserAgent);
			string a;
			UserAgentParser.UserAgentVersion userAgentVersion;
			string a2;
			UserAgentParser.Parse(request.UserAgent, out a, out userAgentVersion, out a2);
			return (!string.Equals(a, "MSIE", StringComparison.OrdinalIgnoreCase) || (userAgentVersion.Build < 8 && (userAgentVersion.Build != 7 || request.UserAgent.IndexOf("Trident") <= 0)) || (!string.Equals(a2, "Windows NT", StringComparison.OrdinalIgnoreCase) && !string.Equals(a2, "Windows 98; Win 9x 4.90", StringComparison.OrdinalIgnoreCase) && !string.Equals(a2, "Windows 2000", StringComparison.OrdinalIgnoreCase))) && ((!string.Equals(a, "Safari", StringComparison.OrdinalIgnoreCase) || userAgentVersion.Build < 4 || (!string.Equals(a2, "Macintosh", StringComparison.OrdinalIgnoreCase) && !string.Equals(a2, "Windows NT", StringComparison.OrdinalIgnoreCase))) && ((!string.Equals(a2, "iPhone", StringComparison.OrdinalIgnoreCase) && !string.Equals(a2, "iPad", StringComparison.OrdinalIgnoreCase)) || userAgentVersion.Build < 5)) && (!string.Equals(a2, "Android", StringComparison.OrdinalIgnoreCase) || userAgentVersion.Build < 4) && (!string.Equals(a, "Firefox", StringComparison.OrdinalIgnoreCase) || userAgentVersion.Build < 4 || (!string.Equals(a2, "Windows NT", StringComparison.OrdinalIgnoreCase) && !string.Equals(a2, "Windows 98; Win 9x 4.90", StringComparison.OrdinalIgnoreCase) && !string.Equals(a2, "Windows 2000", StringComparison.OrdinalIgnoreCase) && !string.Equals(a2, "Macintosh", StringComparison.OrdinalIgnoreCase) && !string.Equals(a2, "Linux", StringComparison.OrdinalIgnoreCase))) && (!string.Equals(a, "Chrome", StringComparison.OrdinalIgnoreCase) || userAgentVersion.Build < 1 || (!string.Equals(a2, "Windows NT", StringComparison.OrdinalIgnoreCase) && !string.Equals(a2, "Macintosh", StringComparison.OrdinalIgnoreCase) && !string.Equals(a2, "Linux", StringComparison.OrdinalIgnoreCase)));
		}

		internal static ClientBrowserStatus GetClientBrowserStatus(HttpBrowserCapabilities browserCapabilities)
		{
			if (browserCapabilities == null)
			{
				throw new ArgumentNullException("browserCapabilities");
			}
			if (string.Equals(browserCapabilities.Browser, "IE", StringComparison.InvariantCultureIgnoreCase) && browserCapabilities.MajorVersion >= 7)
			{
				foreach (string text in browserCapabilities["extra"].Split(new char[]
				{
					';'
				}))
				{
					if (string.Equals(text.Trim(), "x64", StringComparison.InvariantCultureIgnoreCase))
					{
						return ClientBrowserStatus.IE7OrLaterIn64Bit;
					}
				}
				return ClientBrowserStatus.IE7OrLaterIn32Bit;
			}
			return ClientBrowserStatus.Others;
		}

		public static bool IsIEClient(HttpRequest request)
		{
			if (request == null)
			{
				throw new ArgumentNullException("request cannot be null");
			}
			ExTraceGlobals.CoreCallTracer.TraceDebug<string>(0L, "Utilities.IsIEClient. user-agent = {0}", (request.UserAgent == null) ? string.Empty : request.UserAgent);
			return !string.IsNullOrEmpty(request.UserAgent) && (request.UserAgent.IndexOf("MSIE", StringComparison.InvariantCultureIgnoreCase) != -1 || request.UserAgent.Equals("Mozilla/5.0 (Windows NT; owaauth)"));
		}

		internal static void AddOwaConditionAdvisorIfNecessary(UserContext userContext, Folder folder, EventObjectType eventObjectType, EventType eventType)
		{
			if (OwaMapiNotificationManager.IsNotificationEnabled(userContext) && !Utilities.IsPublic(folder))
			{
				OwaStoreObjectId owaStoreObjectId = OwaStoreObjectId.CreateFromSessionFolderId(userContext, folder.Session, folder.Id.ObjectId);
				if (userContext.IsPushNotificationsEnabled)
				{
					MailboxSession mailboxSession = folder.Session as MailboxSession;
					if (mailboxSession != null)
					{
						userContext.MapiNotificationManager.SubscribeForFolderChanges(owaStoreObjectId, mailboxSession);
					}
				}
				if (userContext.IsPullNotificationsEnabled)
				{
					Dictionary<OwaStoreObjectId, OwaConditionAdvisor> conditionAdvisorTable = userContext.NotificationManager.ConditionAdvisorTable;
					if (conditionAdvisorTable == null || !conditionAdvisorTable.ContainsKey(owaStoreObjectId))
					{
						userContext.NotificationManager.CreateOwaConditionAdvisor(userContext, folder.Session as MailboxSession, owaStoreObjectId, eventObjectType, eventType);
					}
				}
			}
		}

		internal static void SetWebBeaconPolicy(bool isRequestCallbackForWebBeacons, Item item, params PropertyDefinition[] prefetchProperties)
		{
			if (isRequestCallbackForWebBeacons)
			{
				if (item == null)
				{
					throw new ArgumentNullException("item");
				}
				item.OpenAsReadWrite();
				item[ItemSchema.BlockStatus] = BlockStatus.NoNeverAgain;
				ConflictResolutionResult conflictResolutionResult = item.Save(SaveMode.ResolveConflicts);
				if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
				{
					throw new OwaSaveConflictException(LocalizedStrings.GetNonEncoded(-482397486), conflictResolutionResult);
				}
				item.Load(prefetchProperties);
			}
		}

		public static int GetEmbeddedDepth(HttpRequest request)
		{
			string queryStringParameter = Utilities.GetQueryStringParameter(request, "attcnt", false);
			if (queryStringParameter == null)
			{
				return 0;
			}
			int result;
			if (!int.TryParse(queryStringParameter, out result))
			{
				throw new OwaInvalidRequestException("Invalid attachment count querystring parameter");
			}
			return result;
		}

		public static bool IsOwa15Url(HttpRequest request)
		{
			if (request.Url.LocalPath.EndsWith("default.aspx", StringComparison.OrdinalIgnoreCase) || request.Url.LocalPath.EndsWith("sessiondata.ashx", StringComparison.OrdinalIgnoreCase) || request.Url.LocalPath.EndsWith("remotenotification.ashx", StringComparison.OrdinalIgnoreCase) || request.Url.LocalPath.EndsWith("service.svc", StringComparison.OrdinalIgnoreCase) || request.Url.LocalPath.EndsWith(".manifest", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			if (request.Url.LocalPath.EndsWith("/owa", StringComparison.OrdinalIgnoreCase) || request.Url.LocalPath.EndsWith("/owa/", StringComparison.OrdinalIgnoreCase))
			{
				if (string.IsNullOrEmpty(request.Url.Query))
				{
					return true;
				}
				foreach (string name in Utilities.Owa15ParameterNames)
				{
					if (Utilities.GetQueryStringParameter(request, name, false) != null)
					{
						return true;
					}
				}
			}
			return false;
		}

		public static bool IsOwaUrl(Uri requestUrl, OwaUrl owaUrl, bool exactMatch)
		{
			return Utilities.IsOwaUrl(requestUrl, owaUrl, exactMatch, true);
		}

		public static bool IsOwaUrl(Uri requestUrl, OwaUrl owaUrl, bool exactMatch, bool useLocal)
		{
			if (requestUrl == null)
			{
				throw new ArgumentNullException("requestUrl");
			}
			if (owaUrl == null)
			{
				throw new ArgumentNullException("owaUrl");
			}
			int length = owaUrl.ImplicitUrl.Length;
			string text = useLocal ? requestUrl.LocalPath : requestUrl.PathAndQuery;
			bool flag = string.Compare(text, 0, owaUrl.ImplicitUrl, 0, length, StringComparison.OrdinalIgnoreCase) == 0;
			if (exactMatch)
			{
				flag = (flag && length == text.Length);
			}
			return flag;
		}

		public static OwaRequestType GetRequestType(HttpRequest request)
		{
			OwaRequestType result;
			if (Globals.OwaVDirType == OWAVDirType.Calendar && (Utilities.IsOwaUrl(request.Url, OwaUrl.PublishedCalendar, true) || Utilities.IsOwaUrl(request.Url, OwaUrl.ReachPublishedCalendar, true)))
			{
				result = OwaRequestType.PublishedCalendarView;
			}
			else if (Globals.OwaVDirType == OWAVDirType.Calendar && (Utilities.IsOwaUrl(request.Url, OwaUrl.PublishedICal, true) || Utilities.IsOwaUrl(request.Url, OwaUrl.ReachPublishedICal, true)))
			{
				result = OwaRequestType.ICalHttpHandler;
			}
			else if (Utilities.IsResourceRequest(request))
			{
				result = OwaRequestType.Resource;
			}
			else if (WebPartUtilities.IsCmdWebPart(request))
			{
				result = OwaRequestType.Form15;
			}
			else if (Utilities.IsMonitoringPingRequest(request))
			{
				result = OwaRequestType.HealthPing;
			}
			else if (Utilities.IsOwaUrl(request.Url, OwaUrl.Oeh, true))
			{
				if (Utilities.IsOwaUrl(request.Url, OwaUrl.ProxyEws, false, false))
				{
					result = OwaRequestType.ProxyToEwsEventHandler;
				}
				else if (string.Equals(request.Params["ns"], "WebReady", StringComparison.InvariantCultureIgnoreCase))
				{
					result = OwaRequestType.WebReadyRequest;
				}
				else
				{
					result = OwaRequestType.Oeh;
				}
			}
			else if (Utilities.IsOwaUrl(request.Url, OwaUrl.LanguagePage, true))
			{
				result = OwaRequestType.LanguagePage;
			}
			else if (Utilities.IsOwaUrl(request.Url, OwaUrl.Default14Page, true))
			{
				result = OwaRequestType.Form15;
			}
			else if (Utilities.IsOwaUrl(request.Url, OwaUrl.AttachmentHandler, true))
			{
				result = OwaRequestType.Attachment;
			}
			else if (Utilities.IsOwaUrl(request.Url, OwaUrl.WebReadyUrl, false, true))
			{
				result = OwaRequestType.WebReadyRequest;
			}
			else if (Utilities.IsOwaUrl(request.Url, OwaUrl.LanguagePost, true))
			{
				result = OwaRequestType.LanguagePost;
			}
			else if (Utilities.IsOwaUrl(request.Url, OwaUrl.Logoff, true))
			{
				result = OwaRequestType.Logoff;
			}
			else if (Utilities.IsOwaUrl(request.Url, OwaUrl.ProxyLogon, true))
			{
				result = OwaRequestType.ProxyLogon;
			}
			else if (Utilities.IsOwaUrl(request.Url, OwaUrl.ProxyPing, true))
			{
				result = OwaRequestType.ProxyPing;
			}
			else if (Utilities.IsOwaUrl(request.Url, OwaUrl.KeepAlive, true))
			{
				result = OwaRequestType.KeepAlive;
			}
			else if (request.Url.LocalPath.EndsWith(".owa", StringComparison.OrdinalIgnoreCase))
			{
				result = OwaRequestType.Invalid;
			}
			else if (Utilities.IsOwaUrl(request.Url, OwaUrl.AuthFolder, false))
			{
				result = OwaRequestType.Authorize;
			}
			else if (request.Url.LocalPath.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase) || request.Url.LocalPath.EndsWith(".ashx", StringComparison.OrdinalIgnoreCase))
			{
				result = OwaRequestType.Aspx;
			}
			else if (request.Url.LocalPath.EndsWith(Utilities.VirtualDirectoryNameWithLeadingSlash, StringComparison.OrdinalIgnoreCase) || request.Url.LocalPath.EndsWith(Utilities.VirtualDirectoryNameWithLeadingAndTrailingSlash, StringComparison.OrdinalIgnoreCase))
			{
				result = (Utilities.IsOwa15Url(request) ? OwaRequestType.Form15 : OwaRequestType.Form14);
			}
			else if (request.Url.LocalPath.EndsWith("service.svc", StringComparison.OrdinalIgnoreCase))
			{
				result = OwaRequestType.ServiceRequest;
			}
			else
			{
				result = OwaRequestType.Invalid;
			}
			return result;
		}

		private static bool IsResourceRequest(HttpRequest request)
		{
			return request.Url.LocalPath.EndsWith(".gif", StringComparison.OrdinalIgnoreCase) || request.Url.LocalPath.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || request.Url.LocalPath.EndsWith(".css", StringComparison.OrdinalIgnoreCase) || request.Url.LocalPath.EndsWith(".xap", StringComparison.OrdinalIgnoreCase) || request.Url.LocalPath.EndsWith(".js", StringComparison.OrdinalIgnoreCase) || request.Url.LocalPath.EndsWith(".htm", StringComparison.OrdinalIgnoreCase) || request.Url.LocalPath.EndsWith(".html", StringComparison.OrdinalIgnoreCase) || request.Url.LocalPath.EndsWith(".png", StringComparison.OrdinalIgnoreCase) || request.Url.LocalPath.EndsWith(".wav", StringComparison.OrdinalIgnoreCase) || request.Url.LocalPath.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase) || request.Url.LocalPath.EndsWith(".msi", StringComparison.OrdinalIgnoreCase) || request.Url.LocalPath.EndsWith(".ico", StringComparison.OrdinalIgnoreCase) || request.Url.LocalPath.EndsWith(".manifest", StringComparison.OrdinalIgnoreCase) || request.Url.LocalPath.EndsWith(".ttf", StringComparison.OrdinalIgnoreCase) || request.Url.LocalPath.EndsWith(".eot", StringComparison.OrdinalIgnoreCase) || request.Url.LocalPath.EndsWith(".woff", StringComparison.OrdinalIgnoreCase) || request.Url.LocalPath.EndsWith(".svg", StringComparison.OrdinalIgnoreCase) || request.Url.LocalPath.EndsWith(".crx", StringComparison.OrdinalIgnoreCase);
		}

		private static bool IsMonitoringPingRequest(HttpRequest request)
		{
			return string.Compare(request.Url.PathAndQuery, OwaUrl.HealthPing.ImplicitUrl, StringComparison.OrdinalIgnoreCase) == 0;
		}

		internal static bool ShouldRenderExpiringPasswordInfobar(UserContext userContext, out int daysToExpiration)
		{
			IExchangePrincipal mailboxOwner = userContext.MailboxSession.MailboxOwner;
			daysToExpiration = -1;
			if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Global.MultiTenancy.Enabled)
			{
				return false;
			}
			if (userContext.IsWebPartRequest || userContext.IsExplicitLogonOthersMailbox)
			{
				return false;
			}
			if (!userContext.IsFeatureEnabled(Feature.ChangePassword))
			{
				return false;
			}
			if (!userContext.MessageViewFirstRender)
			{
				return false;
			}
			if (userContext.MailboxIdentity.IsCrossForest(mailboxOwner.MasterAccountSid))
			{
				return false;
			}
			ExDateTime passwordExpirationDate = DirectoryHelper.GetPasswordExpirationDate(mailboxOwner.ObjectId, userContext.MailboxSession.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid));
			if (ExDateTime.MaxValue == passwordExpirationDate)
			{
				return false;
			}
			ExDateTime exDateTime = userContext.TimeZone.ConvertDateTime(passwordExpirationDate);
			ExDateTime exDateTime2 = DateTimeUtilities.GetLocalTime();
			if (exDateTime2.CompareTo(exDateTime) > 0)
			{
				daysToExpiration = 0;
				return true;
			}
			if (ExTraceGlobals.ChangePasswordTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.ChangePasswordTracer.TraceDebug(0L, "Password expiration for {0}: Expiration UTC date: {1} / Expiration local user date {2} / User current Local date: {3} ", new object[]
				{
					userContext.LogonIdentity.SafeGetRenderableName(),
					(DateTime)passwordExpirationDate.ToUtc(),
					(DateTime)exDateTime,
					exDateTime2
				});
			}
			exDateTime2 = exDateTime2.Date;
			int days = exDateTime.Date.Subtract(exDateTime2).Days;
			if (days < 14)
			{
				daysToExpiration = days;
				return true;
			}
			return false;
		}

		public static bool WhiteSpaceOnlyOrNullEmpty(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return true;
			}
			foreach (char c in s)
			{
				if (!char.IsWhiteSpace(c))
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsGetRequest(HttpRequest request)
		{
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}
			return string.Equals(request.HttpMethod, "get", StringComparison.OrdinalIgnoreCase);
		}

		public static bool IsPostRequest(HttpRequest request)
		{
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}
			return string.Equals(request.HttpMethod, "post", StringComparison.OrdinalIgnoreCase);
		}

		internal static Utilities.ChangePasswordResult ChangePassword(OwaIdentity owaIdentity, SecureString oldPassword, SecureString newPassword)
		{
			return Utilities.ChangePasswordNUCP(owaIdentity, oldPassword, newPassword);
		}

		internal static Utilities.ChangePasswordResult ChangePassword(string logonName, SecureString oldPassword, SecureString newPassword)
		{
			return Utilities.ChangePasswordNUCP(logonName, oldPassword, newPassword);
		}

		internal static Utilities.ChangePasswordResult ChangePasswordNUCP(OwaIdentity identity, SecureString oldPassword, SecureString newPassword)
		{
			if (identity == null || oldPassword == null || newPassword == null)
			{
				throw new ArgumentNullException();
			}
			string logonName;
			try
			{
				logonName = identity.GetLogonName();
			}
			catch (OwaIdentityException)
			{
				ExTraceGlobals.ChangePasswordTracer.TraceDebug<string>(0L, "ChangePassword failed to retrieve user name for : {0}", identity.UniqueId);
				return Utilities.ChangePasswordResult.OtherError;
			}
			return Utilities.ChangePasswordNUCP(logonName, oldPassword, newPassword);
		}

		internal static Utilities.ChangePasswordResult ChangePasswordNUCP(string logonName, SecureString oldPassword, SecureString newPassword)
		{
			if (logonName == null || oldPassword == null || newPassword == null)
			{
				throw new ArgumentNullException();
			}
			string text = null;
			string text2 = null;
			if (!Utilities.IsDomainSlashUser(logonName))
			{
				if (Utilities.IsUserPrincipalName(logonName))
				{
					try
					{
						text = NativeHelpers.GetDomainName();
						text2 = logonName;
						goto IL_83;
					}
					catch (CannotGetDomainInfoException ex)
					{
						ExTraceGlobals.ChangePasswordTracer.TraceError<string, string>(0L, "Change password for UPN {0} failed to get the domain name. Error: {1}", logonName, ex.Message);
						return Utilities.ChangePasswordResult.OtherError;
					}
				}
				ExTraceGlobals.ChangePasswordTracer.TraceError<string>(0L, "Change password failed due to bad user name: {0}", logonName);
				return Utilities.ChangePasswordResult.OtherError;
			}
			string[] array = logonName.Split(new char[]
			{
				'\\'
			});
			text = array[0];
			text2 = array[1];
			IL_83:
			ExTraceGlobals.ChangePasswordTracer.TraceDebug<string, string>(0L, "Attempting to call NetUserChangePassword with domain: {0} and user: {1}", text, text2);
			IntPtr intPtr = IntPtr.Zero;
			IntPtr intPtr2 = IntPtr.Zero;
			try
			{
				intPtr = Marshal.SecureStringToGlobalAllocUnicode(oldPassword);
				intPtr2 = Marshal.SecureStringToGlobalAllocUnicode(newPassword);
				uint num = SafeNativeMethods.NetUserChangePassword(text, text2, intPtr, intPtr2);
				if (num != 0U)
				{
					ExTraceGlobals.ChangePasswordTracer.TraceError<uint>(0L, "NetUserChangePassword failed with error code = {0}", num);
					uint num2 = num;
					if (num2 == 5U)
					{
						return Utilities.ChangePasswordResult.LockedOut;
					}
					if (num2 == 86U)
					{
						return Utilities.ChangePasswordResult.InvalidCredentials;
					}
					if (num2 != 2245U)
					{
						return Utilities.ChangePasswordResult.OtherError;
					}
					return Utilities.ChangePasswordResult.BadNewPassword;
				}
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					Marshal.ZeroFreeGlobalAllocUnicode(intPtr);
				}
				if (intPtr2 != IntPtr.Zero)
				{
					Marshal.ZeroFreeGlobalAllocUnicode(intPtr2);
				}
			}
			if (Globals.ArePerfCountersEnabled)
			{
				OwaSingleCounters.PasswordChanges.Increment();
			}
			ExTraceGlobals.ChangePasswordTracer.TraceDebug(0L, "Password was changed succesfully");
			return Utilities.ChangePasswordResult.Success;
		}

		public static bool IsChangePasswordLogoff(HttpRequest request)
		{
			return Utilities.GetQueryStringParameter(request, "ChgPwd", false) == "1";
		}

		public static void RewriteAndSanitizeWebReadyHtml(string documentId, Stream inputStream, Stream outputStream)
		{
			if (string.IsNullOrEmpty(documentId))
			{
				throw new ArgumentException("documentId cannot be null or empty");
			}
			if (inputStream == null)
			{
				throw new ArgumentNullException("inputStream");
			}
			if (outputStream == null)
			{
				throw new ArgumentNullException("outputStream");
			}
			try
			{
				HtmlToHtml htmlToHtml = new HtmlToHtml();
				TextConvertersInternalHelpers.SetPreserveDisplayNoneStyle(htmlToHtml, true);
				htmlToHtml.InputEncoding = Encoding.UTF8;
				htmlToHtml.OutputEncoding = Encoding.UTF8;
				htmlToHtml.FilterHtml = true;
				htmlToHtml.HtmlTagCallback = new HtmlTagCallback(new OwaSafeHtmlWebReadyCallbacks(documentId).ProcessTag);
				htmlToHtml.Convert(inputStream, outputStream);
			}
			catch (InvalidCharsetException innerException)
			{
				throw new OwaBodyConversionFailedException("Sanitize Html Failed", innerException);
			}
			catch (TextConvertersException innerException2)
			{
				throw new OwaBodyConversionFailedException("Sanitize Html Failed", innerException2);
			}
			catch (StoragePermanentException innerException3)
			{
				throw new OwaBodyConversionFailedException("Body Conversion Failed", innerException3);
			}
			catch (StorageTransientException innerException4)
			{
				throw new OwaBodyConversionFailedException("Body Conversion Failed", innerException4);
			}
		}

		public static void DeleteFBASessionCookies(HttpResponse response)
		{
			Utilities.DeleteCookie(response, "sessionid");
			Utilities.DeleteCookie(response, "cadata");
		}

		public static void DeleteCookie(HttpResponse response, string name)
		{
			if (response == null)
			{
				throw new ArgumentNullException("response");
			}
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("name can not be null or empty string");
			}
			bool flag = false;
			for (int i = 0; i < response.Cookies.Count; i++)
			{
				HttpCookie httpCookie = response.Cookies[i];
				if (httpCookie.Name != null && string.Equals(httpCookie.Name, name, StringComparison.OrdinalIgnoreCase))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				response.Cookies.Add(new HttpCookie(name, string.Empty));
			}
			response.Cookies[name].Expires = (DateTime)ExDateTime.UtcNow.AddYears(-30);
		}

		public static string GetTCMIDValue(OwaContext owaContext)
		{
			HttpCookieCollection cookies = owaContext.HttpContext.Request.Cookies;
			if (cookies != null)
			{
				HttpCookie httpCookie = cookies.Get("TCMID");
				if (httpCookie != null)
				{
					return httpCookie.Value;
				}
			}
			NameValueCollection headers = owaContext.HttpContext.Request.Headers;
			if (headers != null)
			{
				string text = headers["TCMID"];
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
			}
			return string.Empty;
		}

		public static string GetCurrentCanary(ISessionContext sessionContext)
		{
			if (sessionContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			UserContext userContext = sessionContext as UserContext;
			if (userContext == null)
			{
				return sessionContext.Canary;
			}
			UserContextKey userContextKey = userContext.Key.CloneWithRenewedCanary();
			return userContextKey.Canary.ToString();
		}

		public static void RenderCanaryHidden(TextWriter writer, UserContext userContext)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			writer.Write("<input type=hidden name=\"");
			writer.Write("hidcanary");
			writer.Write("\" value=\"");
			Utilities.HtmlEncode(Utilities.GetCurrentCanary(userContext), writer);
			writer.Write("\">");
			string canary15CookieValue = Utilities.GetCanary15CookieValue();
			if (canary15CookieValue != null)
			{
				writer.Write("<input type=hidden name=\"");
				writer.Write("X-OWA-CANARY");
				writer.Write("\" value=\"");
				writer.Write(canary15CookieValue);
				writer.Write("\">");
			}
		}

		public static string GetCanaryRequestParameter()
		{
			string text = "&canary=" + Utilities.UrlEncode(Utilities.GetCurrentCanary(OwaContext.Current.UserContext));
			string canary15CookieValue = Utilities.GetCanary15CookieValue();
			if (canary15CookieValue != null)
			{
				text = text + "&X-OWA-CANARY=" + canary15CookieValue;
			}
			return text;
		}

		internal static void VerifyCanary(UserContextCookie userContextCookie, HttpRequest httpRequest)
		{
			if (userContextCookie == null)
			{
				throw new ArgumentNullException("userContextCookie");
			}
			if (httpRequest == null)
			{
				throw new ArgumentNullException("HttpRequest");
			}
			string canaryFromRequest;
			if (Utilities.IsPostRequest(httpRequest))
			{
				canaryFromRequest = Utilities.GetFormParameter(httpRequest, "hidcanary", false);
			}
			else
			{
				canaryFromRequest = Utilities.GetQueryStringParameter(httpRequest, "canary", false);
			}
			Utilities.ValidateCanary(canaryFromRequest, userContextCookie);
		}

		internal static void VerifyEventHandlerCanary(OwaEventHandlerBase eventHandler)
		{
			if (eventHandler == null)
			{
				throw new ArgumentNullException("eventHandler");
			}
			if (eventHandler.EventInfo.Name == "RenderADPhoto" || eventHandler.EventInfo.Name == "RenderImage")
			{
				return;
			}
			UserContextCookie userContextCookie = UserContextCookie.GetUserContextCookie(eventHandler.OwaContext);
			Utilities.ValidateCanary((string)eventHandler.GetParameter("canary"), userContextCookie);
		}

		internal static void VerifySearchCanaryInGetRequest(HttpRequest httpRequest)
		{
			if (Utilities.IsGetRequest(httpRequest))
			{
				string queryStringParameter = Utilities.GetQueryStringParameter(httpRequest, "canary", true);
				if (!OwaContext.Current.UserContext.Key.Canary.ValidateCanary(queryStringParameter))
				{
					throw new OwaInvalidCanary14Exception(null, "Invalid canary in search query");
				}
			}
		}

		internal static void ValidateCanary(string canaryFromRequest, UserContextCookie userContextCookie)
		{
			string text = (userContextCookie.ContextCanary == null) ? null : userContextCookie.ContextCanary.ToString();
			if (string.IsNullOrEmpty(canaryFromRequest) || string.IsNullOrEmpty(text) || !userContextCookie.ContextCanary.ValidateCanary(canaryFromRequest))
			{
				if (Globals.ArePerfCountersEnabled)
				{
					OwaSingleCounters.InvalidCanaryRequests.Increment();
				}
				ExTraceGlobals.CoreTracer.TraceDebug<string, string>(0L, "Utilities.ValidateCanary(): Invalid canary. Request Canary: '{0}'  Cookie Canary: '{1}'", (canaryFromRequest != null) ? canaryFromRequest : "null", (text != null) ? text : "null");
				throw new OwaInvalidCanary14Exception(userContextCookie, "Invalid canary in request");
			}
		}

		internal static bool IsELCRootFolder(OwaStoreObjectId folderId, UserContext userContext)
		{
			if (folderId == null)
			{
				throw new ArgumentNullException("folderId");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			return !folderId.IsPublic && !folderId.IsArchive && !folderId.IsOtherMailbox && !folderId.IsGSCalendar && Utilities.IsELCRootFolder(folderId.StoreObjectId, userContext);
		}

		internal static bool IsELCRootFolder(StoreObjectId folderId, UserContext userContext)
		{
			bool result;
			using (Folder folder = Folder.Bind(userContext.MailboxSession, folderId, new PropertyDefinition[]
			{
				FolderSchema.AdminFolderFlags
			}))
			{
				result = Utilities.IsELCRootFolder(folder.TryGetProperty(FolderSchema.AdminFolderFlags));
			}
			return result;
		}

		internal static bool IsELCRootFolder(Folder folder)
		{
			return Utilities.IsELCRootFolder(folder.TryGetProperty(FolderSchema.AdminFolderFlags));
		}

		public static bool IsELCRootFolder(object adminFolderFlags)
		{
			return adminFolderFlags is int && ((int)adminFolderFlags & 16) > 0;
		}

		public static bool IsELCFolder(int adminFolderFlags)
		{
			return (adminFolderFlags & 1) > 0 || (adminFolderFlags & 2) > 0;
		}

		internal static bool IsELCFolder(Folder folder)
		{
			object obj = folder.TryGetProperty(FolderSchema.AdminFolderFlags);
			return obj != null && !(obj is PropertyError) && Utilities.IsELCFolder((int)obj);
		}

		internal static OutlookModule GetModuleForFolder(Folder folder, UserContext userContext)
		{
			if (Utilities.IsDefaultFolder(folder, DefaultFolderType.ToDoSearch))
			{
				return OutlookModule.Tasks;
			}
			return Utilities.GetModuleForObjectClass(folder.ClassName);
		}

		internal static OutlookModule GetModuleForObjectClass(string objectClass)
		{
			if (objectClass == null)
			{
				return OutlookModule.None;
			}
			if (ObjectClass.IsMessageFolder(objectClass) || ObjectClass.IsMessage(objectClass, false) || ObjectClass.IsMeetingMessage(objectClass) || ObjectClass.IsTaskRequest(objectClass) || ObjectClass.IsReport(objectClass))
			{
				return OutlookModule.Mail;
			}
			if (ObjectClass.IsTaskFolder(objectClass) || ObjectClass.IsTask(objectClass))
			{
				return OutlookModule.Tasks;
			}
			if (ObjectClass.IsContactsFolder(objectClass) || ObjectClass.IsContact(objectClass) || ObjectClass.IsDistributionList(objectClass))
			{
				return OutlookModule.Contacts;
			}
			if (ObjectClass.IsCalendarFolder(objectClass) || ObjectClass.IsCalendarItemCalendarItemOccurrenceOrRecurrenceException(objectClass))
			{
				return OutlookModule.Calendar;
			}
			if (ObjectClass.IsJournalFolder(objectClass) || ObjectClass.IsJournalItem(objectClass))
			{
				return OutlookModule.Journal;
			}
			if (ObjectClass.IsNotesFolder(objectClass) || ObjectClass.IsNotesItem(objectClass))
			{
				return OutlookModule.Notes;
			}
			return OutlookModule.None;
		}

		public static bool IsFolderSegmentedOut(string folderType, UserContext userContext)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			return !string.IsNullOrEmpty(folderType) && !ObjectClass.IsMessageFolder(folderType) && ((!userContext.IsFeatureEnabled(Feature.Calendar) && ObjectClass.IsCalendarFolder(folderType)) || (!userContext.IsFeatureEnabled(Feature.Contacts) && ObjectClass.IsContactsFolder(folderType)) || (!userContext.IsFeatureEnabled(Feature.Tasks) && ObjectClass.IsTaskFolder(folderType)) || (!userContext.IsFeatureEnabled(Feature.Journal) && ObjectClass.IsJournalFolder(folderType)) || (!userContext.IsFeatureEnabled(Feature.StickyNotes) && ObjectClass.IsOfClass(folderType, "IPF.StickyNote")));
		}

		public static BrowserPlatform GetBrowserPlatform(string userAgent)
		{
			if (userAgent == null)
			{
				return BrowserPlatform.Other;
			}
			string text = null;
			string text2 = null;
			UserAgentParser.UserAgentVersion userAgentVersion;
			UserAgentParser.Parse(userAgent, out text, out userAgentVersion, out text2);
			if (string.Equals(text2, "Macintosh", StringComparison.OrdinalIgnoreCase))
			{
				return BrowserPlatform.Macintosh;
			}
			if (-1 != text2.IndexOf("Windows", StringComparison.OrdinalIgnoreCase))
			{
				return BrowserPlatform.Windows;
			}
			return BrowserPlatform.Other;
		}

		public static BrowserType GetBrowserType(string userAgent)
		{
			if (userAgent == null)
			{
				return BrowserType.Other;
			}
			string a = null;
			string text = null;
			UserAgentParser.UserAgentVersion userAgentVersion;
			UserAgentParser.Parse(userAgent, out a, out userAgentVersion, out text);
			if (string.Equals(a, "MSIE", StringComparison.OrdinalIgnoreCase))
			{
				return BrowserType.IE;
			}
			if (string.Equals(a, "Opera", StringComparison.OrdinalIgnoreCase))
			{
				return BrowserType.Opera;
			}
			if (string.Equals(a, "Safari", StringComparison.OrdinalIgnoreCase))
			{
				return BrowserType.Safari;
			}
			if (string.Equals(a, "Firefox", StringComparison.OrdinalIgnoreCase))
			{
				return BrowserType.Firefox;
			}
			if (string.Equals(a, "Chrome", StringComparison.OrdinalIgnoreCase))
			{
				return BrowserType.Chrome;
			}
			return BrowserType.Other;
		}

		internal static bool CreateExchangeParticipant(out Participant exchangeParticipant, string displayName, string routingAddress, string routingType, AddressOrigin addressOrigin, StoreObjectId storeObjectId, EmailAddressIndex emailAddressIndex)
		{
			bool result = false;
			if (string.IsNullOrEmpty(routingAddress) && !Utilities.IsMapiPDL(routingType))
			{
				exchangeParticipant = new Participant(displayName, null, null);
				result = true;
			}
			else
			{
				ParticipantOrigin origin = null;
				if (addressOrigin == AddressOrigin.Store && storeObjectId != null)
				{
					EmailAddressIndex emailAddressIndex2 = Participant.RoutingTypeEquals(routingType, "MAPIPDL") ? EmailAddressIndex.None : ((emailAddressIndex == EmailAddressIndex.None) ? EmailAddressIndex.Email1 : emailAddressIndex);
					origin = new StoreParticipantOrigin(storeObjectId, emailAddressIndex2);
				}
				else if (addressOrigin == AddressOrigin.Directory)
				{
					origin = new DirectoryParticipantOrigin();
				}
				exchangeParticipant = new Participant(displayName, routingAddress, routingType, origin, new KeyValuePair<PropertyDefinition, object>[0]);
			}
			return result;
		}

		internal static bool CreateExchangeParticipant(out Participant exchangeParticipant, RecipientAddress recipientAddress)
		{
			return Utilities.CreateExchangeParticipant(out exchangeParticipant, recipientAddress.DisplayName, recipientAddress.RoutingAddress, recipientAddress.RoutingType, recipientAddress.AddressOrigin, recipientAddress.StoreObjectId, recipientAddress.EmailAddressIndex);
		}

		internal static bool RecipientsOnlyHaveEmptyPDL<TRecipient>(UserContext userContext, IRecipientBaseCollection<TRecipient> recipients) where TRecipient : IRecipientBase
		{
			if (recipients.Count == 0)
			{
				return false;
			}
			for (int i = 0; i < recipients.Count; i++)
			{
				TRecipient trecipient = recipients[i];
				if (trecipient.Participant.RoutingType != "MAPIPDL")
				{
					return false;
				}
			}
			for (int j = 0; j < recipients.Count; j++)
			{
				TRecipient trecipient2 = recipients[j];
				if (trecipient2.Participant.RoutingType == "MAPIPDL")
				{
					TRecipient trecipient3 = recipients[j];
					if (trecipient3.Participant.Origin is StoreParticipantOrigin)
					{
						TRecipient trecipient4 = recipients[j];
						StoreObjectId originItemId = ((StoreParticipantOrigin)trecipient4.Participant.Origin).OriginItemId;
						if (originItemId != null && DistributionList.ExpandDeep(userContext.MailboxSession, originItemId).Length > 0)
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		internal static bool ValidateRequest(HttpContext context, out string reason)
		{
			reason = string.Empty;
			if (Globals.OwaVDirType == OWAVDirType.OWA && context.Request.UserAgent == null)
			{
				reason = "Request has no user agent";
				return false;
			}
			return true;
		}

		internal static Participant CreateParticipantFromQueryString(UserContext userContext, HttpRequest httpRequest)
		{
			string queryStringParameter = Utilities.GetQueryStringParameter(httpRequest, "to", false);
			if (string.IsNullOrEmpty(queryStringParameter))
			{
				return null;
			}
			string queryStringParameter2 = Utilities.GetQueryStringParameter(httpRequest, "nm", false);
			string queryStringParameter3 = Utilities.GetQueryStringParameter(httpRequest, "ao", false);
			string queryStringParameter4 = Utilities.GetQueryStringParameter(httpRequest, "rt", false);
			if (string.IsNullOrEmpty(queryStringParameter3))
			{
				return new Participant(queryStringParameter2, queryStringParameter, queryStringParameter4);
			}
			string queryStringParameter5 = Utilities.GetQueryStringParameter(httpRequest, "stId", false);
			int num;
			if (!int.TryParse(queryStringParameter3, out num))
			{
				throw new OwaInvalidRequestException("Invalid address origin querystring parameter");
			}
			Participant.Builder builder = new Participant.Builder(queryStringParameter2, queryStringParameter, queryStringParameter4 ?? "SMTP");
			switch (num)
			{
			case 1:
			{
				StoreObjectId originItemId = Utilities.CreateStoreObjectId(userContext.MailboxSession, queryStringParameter5);
				EmailAddressIndex emailAddressIndex = EmailAddressIndex.None;
				if (!Utilities.IsMapiPDL(queryStringParameter4))
				{
					string queryStringParameter6 = Utilities.GetQueryStringParameter(httpRequest, "ei", true);
					int num2;
					if (!int.TryParse(queryStringParameter6, out num2))
					{
						throw new OwaInvalidRequestException("Invalid email address index querystring parameter");
					}
					emailAddressIndex = (EmailAddressIndex)num2;
				}
				builder.Origin = new StoreParticipantOrigin(originItemId, emailAddressIndex);
				break;
			}
			case 2:
				builder.RoutingType = "EX";
				break;
			default:
				throw new OwaInvalidRequestException("Invalid address origin in URL");
			}
			return builder.ToParticipant();
		}

		internal static MessageItem CreateDraftMessageFromQueryString(UserContext userContext, HttpRequest httpRequest)
		{
			return (MessageItem)Utilities.CreateDraftMessageOrMeetingRequestFromQueryString(userContext, httpRequest, true, new PropertyDefinition[0]);
		}

		internal static CalendarItemBase CreateDraftMeetingRequestFromQueryString(UserContext userContext, HttpRequest httpRequest, params PropertyDefinition[] properties)
		{
			return (CalendarItemBase)Utilities.CreateDraftMessageOrMeetingRequestFromQueryString(userContext, httpRequest, false, properties);
		}

		private static Item CreateDraftMessageOrMeetingRequestFromQueryString(UserContext userContext, HttpRequest httpRequest, bool isMessage, params PropertyDefinition[] properties)
		{
			Item item = null;
			MessageItem messageItem = null;
			CalendarItemBase calendarItemBase = null;
			Participant participant = Utilities.CreateParticipantFromQueryString(userContext, httpRequest);
			if (participant != null)
			{
				if (isMessage)
				{
					messageItem = (item = MessageItem.Create(userContext.MailboxSession, userContext.DraftsFolderId));
				}
				else
				{
					calendarItemBase = (item = CalendarItem.Create(userContext.MailboxSession, userContext.CalendarFolderId));
				}
				if (Globals.ArePerfCountersEnabled)
				{
					OwaSingleCounters.ItemsCreated.Increment();
				}
				string queryStringParameter = Utilities.GetQueryStringParameter(httpRequest, "subject", false);
				if (!string.IsNullOrEmpty(queryStringParameter))
				{
					if (isMessage)
					{
						messageItem.Subject = queryStringParameter;
					}
					else
					{
						calendarItemBase.Subject = queryStringParameter;
					}
				}
				if (isMessage)
				{
					messageItem.Recipients.Add(participant, RecipientItemType.To);
				}
				else
				{
					calendarItemBase.IsMeeting = true;
					calendarItemBase.AttendeeCollection.Add(participant, AttendeeType.Required, null, null, false);
				}
				item[ItemSchema.ConversationIndexTracking] = true;
				item.Save(SaveMode.ResolveConflicts);
				item.Load(properties);
			}
			return item;
		}

		internal static string GetDefaultFolderDisplayName(MailboxSession mailboxSession, DefaultFolderType defaultFolderType)
		{
			string displayName;
			using (Folder folder = Folder.Bind(mailboxSession, defaultFolderType, new PropertyDefinition[]
			{
				FolderSchema.DisplayName
			}))
			{
				displayName = folder.DisplayName;
			}
			return displayName;
		}

		internal static T GetParticipantProperty<T>(Participant participant, PropertyDefinition propertyDefinition, T defaultValue)
		{
			object obj = participant.TryGetProperty(propertyDefinition);
			if (obj == null || obj is PropertyError)
			{
				return defaultValue;
			}
			return (T)((object)obj);
		}

		public static bool IsJapanese
		{
			get
			{
				return Microsoft.Exchange.Clients.Owa.Core.Culture.GetUserCulture().LCID == 1041;
			}
		}

		private static string GenerateExternalLink(OwaContext owaContext)
		{
			string queryStringParameter = Utilities.GetQueryStringParameter(owaContext.HttpContext.Request, "URL", false);
			if (string.IsNullOrEmpty(queryStringParameter))
			{
				return null;
			}
			string text = Redir.BuildRedirUrl(owaContext.UserContext, queryStringParameter) + "&NoDocLnkCls=1";
			return string.Format(LocalizedStrings.GetHtmlEncoded(-1396387455), string.Concat(new string[]
			{
				"<br><a href=\"",
				text,
				"\" target=\"_blank\" class=lnk>",
				Utilities.HtmlEncode(queryStringParameter),
				"</a>"
			}));
		}

		internal static string GetLatestVoiceMailFileName(IStorePropertyBag propertyBag)
		{
			if (propertyBag == null)
			{
				throw new ArgumentNullException("propertyBag");
			}
			string property = ItemUtility.GetProperty<string>(propertyBag, MessageItemSchema.VoiceMessageAttachmentOrder, null);
			if (string.IsNullOrEmpty(property))
			{
				return null;
			}
			string text = null;
			string[] array = property.Split(new char[]
			{
				';'
			});
			for (int i = array.Length - 1; i >= 0; i--)
			{
				text = array[i].Trim();
				if (text.Length > 0)
				{
					break;
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			return text;
		}

		internal static Attachment GetLatestVoiceMailAttachment(Item item, UserContext userContext)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			AttachmentCollection attachmentCollection = Utilities.GetAttachmentCollection(item, false, userContext);
			if (attachmentCollection == null)
			{
				return null;
			}
			AttachmentId attachmentId = null;
			item.Load(new PropertyDefinition[]
			{
				MessageItemSchema.VoiceMessageAttachmentOrder
			});
			string latestVoiceMailFileName = Utilities.GetLatestVoiceMailFileName(item);
			if (latestVoiceMailFileName == null)
			{
				return null;
			}
			foreach (AttachmentHandle handle in attachmentCollection)
			{
				using (Attachment attachment = attachmentCollection.Open(handle))
				{
					if (!(attachment is ItemAttachment))
					{
						if (string.Equals(latestVoiceMailFileName, attachment.FileName, StringComparison.OrdinalIgnoreCase))
						{
							AttachmentPolicy.Level attachmentLevel = AttachmentLevelLookup.GetAttachmentLevel(attachment, userContext);
							if (attachmentLevel == AttachmentPolicy.Level.Block)
							{
								return null;
							}
							attachmentId = attachment.Id;
							break;
						}
					}
				}
			}
			if (attachmentId == null)
			{
				return null;
			}
			if (userContext.IsIrmEnabled && !userContext.IsBasicExperience && Utilities.IsIrmRestrictedAndDecrypted(item) && Utilities.IsProtectedVoiceMessage(latestVoiceMailFileName))
			{
				RightsManagedMessageItem rightsManagedMessageItem = item as RightsManagedMessageItem;
				rightsManagedMessageItem.UnprotectAttachment(attachmentId);
				return rightsManagedMessageItem.ProtectedAttachmentCollection.Open(attachmentId);
			}
			return attachmentCollection.Open(attachmentId);
		}

		public static bool IsProtectedVoiceMessage(string fileName)
		{
			return !string.IsNullOrEmpty(fileName) && (fileName.EndsWith(".umrmmp3", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".umrmwav", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".umrmwma", StringComparison.OrdinalIgnoreCase));
		}

		public static bool IsMapiPDL(string routingType)
		{
			return routingType != null && string.CompareOrdinal(routingType, "MAPIPDL") == 0;
		}

		public static SanitizedHtmlString GetNoScriptHtml()
		{
			string htmlEncoded = LocalizedStrings.GetHtmlEncoded(719849305);
			return SanitizedHtmlString.Format(htmlEncoded, new object[]
			{
				"<a href=\"http://www.microsoft.com/windows/ie/downloads/default.mspx\">",
				"</a>"
			});
		}

		public static bool IsBasicAuthentication(HttpRequest request)
		{
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}
			return 0 == CultureInfo.InvariantCulture.CompareInfo.Compare(request.ServerVariables["AUTH_TYPE"], "Basic", CompareOptions.IgnoreCase);
		}

		public static string BuildHelpHref(string helpFile, string helpAnchor)
		{
			if (helpFile == null)
			{
				throw new ArgumentNullException("helpFile");
			}
			if (helpAnchor == null)
			{
				throw new ArgumentNullException("helpAnchor");
			}
			if (OwaContext.Current.UserContext.IsBasicExperience)
			{
				return string.Concat(new string[]
				{
					"help/",
					Microsoft.Exchange.Clients.Owa.Core.Culture.GetUserHelpLanguage(),
					"/light/",
					helpFile,
					(helpAnchor.Length != 0) ? ("#" + helpAnchor) : string.Empty
				});
			}
			return string.Concat(new string[]
			{
				"help/",
				Microsoft.Exchange.Clients.Owa.Core.Culture.GetUserHelpLanguage(),
				"/premium/",
				helpFile,
				(helpAnchor.Length != 0) ? ("#" + helpAnchor) : string.Empty
			});
		}

		public static string BuildEhcHref(string helpId)
		{
			OrganizationProperties organizationProperties = null;
			if (OwaContext.Current != null && OwaContext.Current.MailboxIdentity != null)
			{
				organizationProperties = OwaContext.Current.MailboxIdentity.UserOrganizationProperties;
			}
			return HelpProvider.ConstructHelpRenderingUrl(OwaContext.Current.Culture.LCID, HelpProvider.OwaHelpExperience.Light, helpId, HelpProvider.RenderingMode.Mouse, null, organizationProperties).ToString();
		}

		public static string BuildPrivacyStatmentHref(UserContext userContext)
		{
			if (userContext != null && userContext.ExchangePrincipal != null)
			{
				bool? privacyLinkDisplayEnabled = HelpProvider.GetPrivacyLinkDisplayEnabled(userContext.ExchangePrincipal.MailboxInfo.OrganizationId);
				if (privacyLinkDisplayEnabled != null && !privacyLinkDisplayEnabled.Value)
				{
					return string.Empty;
				}
				Uri uri;
				if (HelpProvider.TryGetPrivacyStatementUrl(userContext.ExchangePrincipal.MailboxInfo.OrganizationId, out uri))
				{
					return Utilities.AppendLCID(uri.ToString());
				}
			}
			return string.Empty;
		}

		private static string AppendLCID(string url)
		{
			if (!string.IsNullOrEmpty(url) && OwaContext.Current != null)
			{
				return string.Format("{0}&clcid={1}", url, Microsoft.Exchange.Clients.Owa.Core.Culture.GetUserCulture().LCID);
			}
			return url;
		}

		public static string BuildCommunitySiteHref(UserContext userContext)
		{
			Uri uri;
			if (userContext != null && userContext.ExchangePrincipal != null && HelpProvider.TryGetCommunityUrl(userContext.ExchangePrincipal.MailboxInfo.OrganizationId, out uri))
			{
				return uri.ToString();
			}
			return string.Empty;
		}

		internal static void RenderImageAltAttribute(TextWriter writer, ISessionContext sessionContext, ThemeFileId themeFileId)
		{
			Utilities.RenderImageAltAttribute(writer, sessionContext, themeFileId, -1018465893);
		}

		internal static void RenderImageAltAttribute(TextWriter writer, ISessionContext sessionContext, ThemeFileId themeFileId, Strings.IDs tooltipStringId)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (tooltipStringId == -1018465893)
			{
				Strings.IDs ds;
				if (Utilities.altTable.TryGetValue(themeFileId, out ds))
				{
					tooltipStringId = ds;
				}
				Utilities.RenderImageAltOrTitleAttribute(writer, sessionContext, tooltipStringId, true);
				return;
			}
			Utilities.RenderImageAltOrTitleAttribute(writer, sessionContext, tooltipStringId, false);
		}

		private static void RenderImageAltOrTitleAttribute(TextWriter writer, ISessionContext sessionContext, Strings.IDs altId, bool useAlt)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (altId != -1018465893)
			{
				if (useAlt)
				{
					writer.Write("alt=\"");
				}
				else
				{
					writer.Write("title=\"");
				}
				writer.Write(SanitizedHtmlString.FromStringId(altId, sessionContext.UserCulture));
				writer.Write("\"");
				return;
			}
			if (useAlt)
			{
				writer.Write("alt=\"\"");
			}
		}

		internal static bool IsFatalFreeBusyError(Exception exception)
		{
			return exception != null && !(exception is WorkingHoursXmlMalformedException);
		}

		internal static bool IsCustomRoutingType(string routingAddress, string routingType)
		{
			return !string.IsNullOrEmpty(routingAddress) && !string.IsNullOrEmpty(routingType) && string.CompareOrdinal(routingType, "EX") != 0 && string.CompareOrdinal(routingType, "SMTP") != 0 && string.CompareOrdinal(routingType, "MAPIPDL") != 0;
		}

		internal static string RemoveHTMLDirectionCharacters(string input)
		{
			return input.Replace(Globals.HtmlDirectionCharacterString, null);
		}

		internal static bool MakeModifiedCalendarItemOccurrence(Item item)
		{
			CalendarItemOccurrence calendarItemOccurrence = item as CalendarItemOccurrence;
			if (calendarItemOccurrence != null && calendarItemOccurrence.CalendarItemType != CalendarItemType.Exception)
			{
				calendarItemOccurrence.MakeModifiedOccurrence();
				return true;
			}
			return false;
		}

		internal static uint GetBitMask(uint count)
		{
			return Utilities.pow2minus1[(int)((UIntPtr)count)];
		}

		internal static uint RotateLeft(uint x, uint count, uint length)
		{
			if (count < 0U || count > 32U)
			{
				throw new ArgumentException("count must be >=0 and <=32");
			}
			if (length < 0U || length > 32U)
			{
				throw new ArgumentException("length must be >=0 and <=32");
			}
			if (count > length)
			{
				throw new ArgumentException("Count must be <= length");
			}
			uint num = x >> (int)(length - count) & Utilities.GetBitMask(count);
			uint num2 = x << (int)count & Utilities.GetBitMask(length);
			return num | num2;
		}

		internal static Uri TryParseUri(string uriString)
		{
			return Utilities.TryParseUri(uriString, UriKind.Absolute);
		}

		internal static Uri TryParseUri(string uriString, UriKind uriKind)
		{
			if (uriString == null)
			{
				throw new ArgumentNullException("uriString");
			}
			Uri result = null;
			if (!Uri.TryCreate(uriString, uriKind, out result))
			{
				return null;
			}
			return result;
		}

		internal static uint RotateRight(uint x, uint count, uint length)
		{
			return Utilities.RotateLeft(x, length - count, length);
		}

		internal static string GetHomePageForMailboxUser(OwaContext owaContext)
		{
			string text = owaContext.MailboxIdentity.GetOWAMiniRecipient()[ADRecipientSchema.WebPage] as string;
			if (!string.IsNullOrEmpty(text))
			{
				return Redir.BuildRedirUrl(owaContext.UserContext, text);
			}
			return null;
		}

		internal static bool CanCreateItemInFolder(UserContext userContext, OwaStoreObjectId folderOwaStoreObjectId)
		{
			bool result;
			using (Folder folder = Utilities.GetFolder<Folder>(userContext, folderOwaStoreObjectId, new PropertyDefinition[]
			{
				StoreObjectSchema.EffectiveRights
			}))
			{
				result = Utilities.CanCreateItemInFolder(folder);
			}
			return result;
		}

		internal static bool CanCreateItemInFolder(Folder folder)
		{
			EffectiveRights folderProperty = Utilities.GetFolderProperty<EffectiveRights>(folder, StoreObjectSchema.EffectiveRights, EffectiveRights.None);
			return (folderProperty & EffectiveRights.CreateContents) == EffectiveRights.CreateContents;
		}

		internal static bool CanModifyFolderProperties(Folder folder)
		{
			EffectiveRights folderProperty = Utilities.GetFolderProperty<EffectiveRights>(folder, StoreObjectSchema.EffectiveRights, EffectiveRights.None);
			return (folderProperty & EffectiveRights.Modify) != EffectiveRights.None;
		}

		internal static bool CanReadItemInFolder(Folder folder)
		{
			EffectiveRights folderProperty = Utilities.GetFolderProperty<EffectiveRights>(folder, StoreObjectSchema.EffectiveRights, EffectiveRights.None);
			return (folderProperty & EffectiveRights.Read) == EffectiveRights.Read;
		}

		internal static bool IsItemInExternalSharedInFolder(UserContext userContext, Item item)
		{
			bool result = false;
			StoreObjectId storeObjectId = null;
			try
			{
				storeObjectId = item.ParentId;
			}
			catch (InvalidOperationException)
			{
			}
			if (storeObjectId != null)
			{
				OwaStoreObjectId folderId = OwaStoreObjectId.CreateFromSessionFolderId(userContext, item.Session, storeObjectId);
				result = Utilities.IsExternalSharedInFolder(userContext, folderId);
			}
			return result;
		}

		internal static bool IsExternalSharedInFolder(UserContext userContext, OwaStoreObjectId folderId)
		{
			bool result = false;
			try
			{
				using (Folder folder = Utilities.GetFolder<Folder>(userContext, folderId, new PropertyDefinition[]
				{
					FolderSchema.ExtendedFolderFlags
				}))
				{
					result = Utilities.IsExternalSharedInFolder(folder);
				}
			}
			catch (ObjectNotFoundException)
			{
			}
			return result;
		}

		internal static bool IsExternalSharedInFolder(Folder folder)
		{
			return Utilities.IsCrossOrgFolder(folder) || Utilities.IsWebCalendarFolder(folder);
		}

		internal static bool IsExternalSharedInFolder(object extendedFolderFlags)
		{
			return Utilities.IsOneOfTheFolderFlagsSet(extendedFolderFlags, new ExtendedFolderFlags[]
			{
				ExtendedFolderFlags.ExchangeCrossOrgShareFolder,
				ExtendedFolderFlags.WebCalFolder
			});
		}

		internal static bool IsCrossOrgFolder(Folder folder)
		{
			return Utilities.IsOneOfTheFolderFlagsSet(folder, new ExtendedFolderFlags[]
			{
				ExtendedFolderFlags.ExchangeCrossOrgShareFolder
			});
		}

		internal static bool IsWebCalendarFolder(Folder folder)
		{
			return Utilities.IsOneOfTheFolderFlagsSet(folder, new ExtendedFolderFlags[]
			{
				ExtendedFolderFlags.WebCalFolder
			});
		}

		internal static bool IsWebCalendarFolder(object extendedFolderFlags)
		{
			return Utilities.IsOneOfTheFolderFlagsSet(extendedFolderFlags, new ExtendedFolderFlags[]
			{
				ExtendedFolderFlags.WebCalFolder
			});
		}

		internal static bool IsPublishedOutFolder(Folder folder)
		{
			return Utilities.IsOneOfTheFolderFlagsSet(folder, new ExtendedFolderFlags[]
			{
				ExtendedFolderFlags.ExchangePublishedCalendar
			});
		}

		internal static bool IsPublishedOutFolder(object extendedFolderFlags)
		{
			return Utilities.IsOneOfTheFolderFlagsSet(extendedFolderFlags, new ExtendedFolderFlags[]
			{
				ExtendedFolderFlags.ExchangePublishedCalendar
			});
		}

		internal static string GetMailboxFolderDisplayName(DefaultFolderType folderType, MailboxSession mailboxSession, string valueOfDisplayNameProperty, bool shouldAddSessionName)
		{
			if (mailboxSession == null)
			{
				throw new ArgumentNullException("mailboxSession");
			}
			string result;
			if (folderType == DefaultFolderType.Root)
			{
				result = Utilities.GetMailboxOwnerDisplayName(mailboxSession);
			}
			else if (folderType == DefaultFolderType.ToDoSearch)
			{
				result = LocalizedStrings.GetNonEncoded(-1954334922);
			}
			else if (folderType == DefaultFolderType.SearchFolders)
			{
				result = LocalizedStrings.GetNonEncoded(1545482161);
			}
			else if (shouldAddSessionName)
			{
				result = string.Format(LocalizedStrings.GetNonEncoded(-83764036), Utilities.GetMailboxOwnerDisplayName(mailboxSession), valueOfDisplayNameProperty);
			}
			else
			{
				result = valueOfDisplayNameProperty;
			}
			return result;
		}

		internal static string GetMailboxOwnerDisplayName(MailboxSession mailboxSession)
		{
			if (mailboxSession.MailboxOwner.MailboxInfo.IsArchive)
			{
				string archiveName = mailboxSession.MailboxOwner.MailboxInfo.ArchiveName;
				if (!string.IsNullOrEmpty(archiveName))
				{
					return archiveName;
				}
			}
			else
			{
				bool isAggregated = mailboxSession.MailboxOwner.MailboxInfo.IsAggregated;
			}
			return mailboxSession.MailboxOwner.MailboxInfo.DisplayName;
		}

		internal static string GetDisplayNameByFolder(Folder folder, UserContext userContext)
		{
			if (folder == null)
			{
				throw new ArgumentNullException("folder");
			}
			if (!Utilities.IsPublic(folder))
			{
				MailboxSession mailboxSession = folder.Session as MailboxSession;
				return Utilities.GetMailboxFolderDisplayName(mailboxSession.IsDefaultFolderType(folder.Id), mailboxSession, folder.DisplayName, !userContext.IsInMyMailbox(folder));
			}
			if (userContext.IsPublicFolderRootId(folder.Id.ObjectId))
			{
				return LocalizedStrings.GetNonEncoded(-1116491328);
			}
			return folder.DisplayName;
		}

		internal static string[] ParseRecipientChunk(string address)
		{
			return ParseRecipientHelper.ParseRecipientChunk(address);
		}

		internal static AvailabilityQueryResult ExecuteAvailabilityQuery(AvailabilityQuery availabilityQuery)
		{
			if (availabilityQuery == null)
			{
				throw new ArgumentNullException("availabilityQuery");
			}
			AvailabilityQueryResult result = null;
			UserContext userContext = OwaContext.Current.UserContext;
			Stopwatch watch = Utilities.StartWatch();
			int numLocksToRestore = 0;
			try
			{
				userContext.DangerousBeginUnlockedAction(true, out numLocksToRestore);
				Utilities.ExecuteAvailabilityQuery(OwaContext.Current, availabilityQuery, out result);
			}
			finally
			{
				if (!userContext.DangerousEndUnlockedAction(true, numLocksToRestore))
				{
					throw new OwaInvalidOperationException("UC went inactive while doing UC lock free operation, terminating request");
				}
			}
			Utilities.StopWatch(watch, "Utilities.ExecuteAvailabilityQuery (Execute Availability Query)");
			return result;
		}

		internal static bool ExecuteAvailabilityQuery(OwaContext owaContext, AvailabilityQuery query, out AvailabilityQueryResult result)
		{
			return Utilities.ExecuteAvailabilityQuery(owaContext, query, false, false, out result);
		}

		internal static bool ExecuteAvailabilityQuery(OwaContext owaContext, AvailabilityQuery query, bool expectFreeBusyResults, out AvailabilityQueryResult result)
		{
			return Utilities.ExecuteAvailabilityQuery(owaContext, query, expectFreeBusyResults, false, out result);
		}

		internal static bool ExecuteAvailabilityQuery(OwaContext owaContext, AvailabilityQuery query, bool expectFreeBusyResults, bool expectMergedFreeBusyResults, out AvailabilityQueryResult result)
		{
			result = null;
			LatencyDetectionContext latencyDetectionContext = Utilities.OwaAvailabilityContextFactory.CreateContext(Globals.ApplicationVersion, owaContext.HttpContext.Request.Url.PathAndQuery, new IPerformanceDataProvider[0]);
			try
			{
				if (string.IsNullOrEmpty(query.ClientContext.MessageId))
				{
					query.ClientContext.MessageId = AvailabilityQuery.CreateNewMessageId();
				}
				query.RequestLogger.StartLog();
				result = query.Execute();
			}
			catch (ClientDisconnectedException)
			{
				return false;
			}
			catch
			{
				ExTraceGlobals.CalendarTracer.TraceDebug(0L, "The availability query threw exception.");
				PerformanceCounterManager.AddAvailabilityServiceResult(false);
				throw;
			}
			finally
			{
				query.RequestLogger.EndLog();
				query.RequestLogger.LogToResponse(owaContext.HttpContext.Response);
				latencyDetectionContext.StopAndFinalizeCollection();
				owaContext.AvailabilityQueryCount += 1U;
				owaContext.AvailabilityQueryLatency += (long)latencyDetectionContext.Elapsed.TotalMilliseconds;
			}
			if (result == null)
			{
				ExTraceGlobals.CalendarTracer.TraceDebug(0L, "The availability query returned a null result.");
				PerformanceCounterManager.AddAvailabilityServiceResult(false);
				return false;
			}
			if (expectFreeBusyResults)
			{
				if (result.FreeBusyResults == null)
				{
					string message = "The availability query returned no FreeBusy result.";
					ExTraceGlobals.CalendarTracer.TraceDebug(0L, message);
					PerformanceCounterManager.AddAvailabilityServiceResult(false);
					return false;
				}
				if (result.FreeBusyResults.Length != 1)
				{
					string formatString = "The availability query returned the wrong number ({0}) of FreeBusy results.";
					ExTraceGlobals.CalendarTracer.TraceDebug<int>(0L, formatString, result.FreeBusyResults.Length);
					PerformanceCounterManager.AddAvailabilityServiceResult(false);
					return false;
				}
				FreeBusyQueryResult freeBusyQueryResult = result.FreeBusyResults[0];
				if (freeBusyQueryResult != null && Utilities.IsFatalFreeBusyError(freeBusyQueryResult.ExceptionInfo))
				{
					ExTraceGlobals.CalendarTracer.TraceDebug<LocalizedException>(0L, "An error happened trying to get free/busy info. Exception: {0}", freeBusyQueryResult.ExceptionInfo);
					PerformanceCounterManager.AddAvailabilityServiceResult(false);
					return false;
				}
				if (expectMergedFreeBusyResults && (freeBusyQueryResult == null || freeBusyQueryResult.MergedFreeBusy == null))
				{
					ExTraceGlobals.CalendarTracer.TraceDebug(0L, "The availability query returned no MergedFreeBusy result.");
					PerformanceCounterManager.AddAvailabilityServiceResult(false);
					return false;
				}
			}
			PerformanceCounterManager.AddAvailabilityServiceResult(true);
			return true;
		}

		internal static List<OWARecipient> LoadAndSortDistributionListMembers(IADDistributionList distributionList, bool fetchCert)
		{
			if (distributionList == null)
			{
				throw new ArgumentNullException("distributionList");
			}
			ADRecipient adrecipient = distributionList as ADRecipient;
			if (adrecipient != null)
			{
				if (adrecipient.RecipientType == RecipientType.DynamicDistributionGroup)
				{
					return new List<OWARecipient>(0);
				}
				ADGroup adgroup = adrecipient as ADGroup;
				if (adgroup == null)
				{
					throw new InvalidOperationException("AD DL object which type is not dynamic DL must be ADGroup class");
				}
				if (adgroup.HiddenGroupMembershipEnabled)
				{
					return new List<OWARecipient>(0);
				}
			}
			int pageSize = 10000;
			PropertyDefinition[] array = fetchCert ? Utilities.smimeDistributionListMemberPropertyDefinitions : Utilities.distributionListMemberPropertyDefinitions;
			ADPagedReader<ADRawEntry> adpagedReader = distributionList.Expand(pageSize, array);
			List<OWARecipient> result;
			using (IEnumerator<ADRawEntry> enumerator = adpagedReader.GetEnumerator())
			{
				List<OWARecipient> list = new List<OWARecipient>(adpagedReader.LastRetrievedCount);
				while (enumerator.MoveNext())
				{
					ADRawEntry adrawEntry = enumerator.Current;
					object[] properties = adrawEntry.GetProperties(array);
					OWARecipient owarecipient = new OWARecipient();
					owarecipient.Id = (properties[0] as ADObjectId);
					if (owarecipient.Id != null)
					{
						owarecipient.DisplayName = (properties[1] as string);
						owarecipient.PhoneticDisplayName = (properties[2] as string);
						owarecipient.UserRecipientType = (RecipientType)properties[3];
						owarecipient.Alias = (properties[4] as string);
						owarecipient.LegacyDN = (properties[5] as string);
						if (fetchCert && !owarecipient.IsDistributionList)
						{
							owarecipient.HasValidDigitalId = (Utilities.GetADRecipientCertificate(enumerator.Current, false) != null);
						}
						list.Add(owarecipient);
					}
				}
				list.Sort();
				result = list;
			}
			return result;
		}

		internal static List<OWARecipient> LoadAndSortDistributionListMembers(IADDistributionList distributionList)
		{
			return Utilities.LoadAndSortDistributionListMembers(distributionList, false);
		}

		internal static string GetMultiCalendarFreeBusyDataForDatePicker(Duration timeWindow, OwaStoreObjectId[] calendarFolderOwaIds, UserContext userContext)
		{
			if (timeWindow == null)
			{
				throw new ArgumentNullException("timeWindow");
			}
			if (calendarFolderOwaIds == null)
			{
				throw new ArgumentNullException("calendarFolderOwaIds");
			}
			if (calendarFolderOwaIds.Length == 0)
			{
				throw new ArgumentException("Must pass at least one folder id");
			}
			string[] array = new string[calendarFolderOwaIds.Length];
			for (int i = 0; i < calendarFolderOwaIds.Length; i++)
			{
				array[i] = Utilities.GetFreeBusyDataForDatePicker(timeWindow, calendarFolderOwaIds[i], userContext);
			}
			int length = array[0].Length;
			StringBuilder stringBuilder = new StringBuilder(length);
			for (int j = 0; j < length; j++)
			{
				char value = '0';
				foreach (string text in array)
				{
					if (text[j] != '0')
					{
						value = text[j];
						break;
					}
				}
				stringBuilder.Append(value);
			}
			return stringBuilder.ToString();
		}

		internal static string GetFreeBusyDataForDatePicker(Duration timeWindow, OwaStoreObjectId calendarFolderId, UserContext userContext)
		{
			if (calendarFolderId.IsGSCalendar)
			{
				return "000000000000000000000000000000000000000000";
			}
			int num = 42;
			timeWindow.EndTime.Date - timeWindow.StartTime.Date;
			CalendarEvent[] array = null;
			using (Folder folder = Utilities.GetFolder<Folder>(userContext, calendarFolderId, new PropertyDefinition[0]))
			{
				CalendarFolder calendarFolder = folder as CalendarFolder;
				if (calendarFolder == null)
				{
					return "000000000000000000000000000000000000000000";
				}
				StorePropertyDefinition[] properties = new StorePropertyDefinition[]
				{
					CalendarItemInstanceSchema.StartTime,
					CalendarItemInstanceSchema.EndTime,
					CalendarItemBaseSchema.FreeBusyStatus
				};
				DateRange[] dateRanges = new DateRange[]
				{
					new DateRange(new ExDateTime(userContext.TimeZone, timeWindow.StartTime), new ExDateTime(userContext.TimeZone, timeWindow.EndTime))
				};
				CalendarDataSource calendarDataSource = new CalendarDataSource(userContext, calendarFolder, dateRanges, properties);
				if (calendarDataSource.Count > 0)
				{
					array = new CalendarEvent[calendarDataSource.Count];
					for (int i = 0; i < calendarDataSource.Count; i++)
					{
						array[i] = new CalendarEvent();
						array[i].StartTime = (DateTime)calendarDataSource.GetStartTime(i);
						array[i].EndTime = (DateTime)calendarDataSource.GetEndTime(i);
						object busyType = calendarDataSource.GetBusyType(i);
						if (!(busyType is int) || (int)busyType < 0 || (int)busyType > 3)
						{
							ExTraceGlobals.CalendarTracer.TraceDebug<DateTime, DateTime, object>(0L, "Calendar event with start time {1} and end time {2} has invalid busy type: {3}. This is being returned as BusyType.Tentative", array[i].StartTime, array[i].EndTime, busyType);
							array[i].BusyType = Microsoft.Exchange.InfoWorker.Common.Availability.BusyType.Tentative;
						}
						else
						{
							array[i].BusyType = (Microsoft.Exchange.InfoWorker.Common.Availability.BusyType)busyType;
						}
					}
				}
			}
			if (array == null)
			{
				return "000000000000000000000000000000000000000000";
			}
			FreeBusyQueryResult freeBusyQueryResult = new FreeBusyQueryResult(FreeBusyViewType.FreeBusy, array, null, null);
			string text = freeBusyQueryResult.GetFreeBusyByDay(timeWindow, userContext.TimeZone);
			if (text == null)
			{
				ExTraceGlobals.CalendarTracer.TraceDebug(0L, "Free/Busy string is invalid.");
				throw new OwaEventHandlerException("Free/Busy string is invalid", LocalizedStrings.GetNonEncoded(-868715791));
			}
			if (text.Length > num)
			{
				text = text.Substring(0, num);
			}
			return text;
		}

		internal static bool ShouldSendChangeKeyForException(Exception exception)
		{
			return exception is MessageSubmissionExceededException || exception is AttachmentExceededException || exception is SendAsDeniedException || exception is FolderSaveException || exception is RecurrenceFormatException || exception is CorruptDataException || exception is OccurrenceCrossingBoundaryException || exception is OccurrenceTimeSpanTooBigException || exception is RecurrenceEndDateTooBigException || exception is RecurrenceStartDateTooSmallException || exception is RecurrenceHasNoOccurrenceException || exception is MessageTooBigException || exception is SubmissionQuotaExceededException;
		}

		internal static bool ShouldSuppressReadReceipt(UserContext userContext)
		{
			return userContext.UserOptions.ReadReceipt != Microsoft.Exchange.Clients.Owa.Core.ReadReceiptResponse.AlwaysSend;
		}

		internal static bool ShouldSuppressReadReceipt(UserContext userContext, Item item)
		{
			return Utilities.ShouldSuppressReadReceipt(userContext) || JunkEmailUtilities.IsInJunkEmailFolder(item, false, userContext) || Utilities.IsPublic(item);
		}

		internal static bool ShouldSuppressReadReceipt(UserContext userContext, OwaStoreObjectId itemId)
		{
			return Utilities.ShouldSuppressReadReceipt(userContext) || JunkEmailUtilities.IsInJunkEmailFolder(itemId, userContext) || itemId.IsPublic;
		}

		internal static void BasicMarkUserMailboxItemsAsRead(UserContext userContext, StoreObjectId[] sourceIds, JunkEmailStatus junkEmailStatus, bool markUnread)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (sourceIds == null)
			{
				throw new ArgumentNullException("sourceIds");
			}
			Utilities.MarkItemsAsRead(userContext.MailboxSession, sourceIds, junkEmailStatus, markUnread, userContext);
		}

		internal static void MarkItemsAsRead(UserContext userContext, OwaStoreObjectId[] sourceIds, JunkEmailStatus junkEmailStatus, bool markUnread)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (sourceIds == null)
			{
				throw new ArgumentNullException("sourceIds");
			}
			if (sourceIds.Length == 0)
			{
				throw new ArgumentOutOfRangeException("sourceIds", "sourceIds should contain more than one element");
			}
			StoreSession session = sourceIds[0].GetSession(userContext);
			Utilities.MarkItemsAsRead(session, OwaStoreObjectId.ConvertToStoreObjectIdArray(sourceIds), junkEmailStatus, markUnread, userContext);
		}

		private static void MarkItemsAsRead(StoreSession storeSession, StoreObjectId[] sourceIds, JunkEmailStatus junkEmailStatus, bool markUnread, UserContext userContext)
		{
			StoreObjectId[] array = null;
			StoreObjectId[] array2 = null;
			if (storeSession is PublicFolderSession || userContext.IsOtherMailbox(storeSession))
			{
				array = sourceIds;
			}
			else
			{
				switch (junkEmailStatus)
				{
				case JunkEmailStatus.NotJunk:
					array2 = sourceIds;
					break;
				case JunkEmailStatus.Junk:
					array = sourceIds;
					break;
				case JunkEmailStatus.Unknown:
					JunkEmailUtilities.SortJunkEmailIds(userContext, sourceIds, out array, out array2);
					break;
				}
			}
			if (array != null && array.Length > 0)
			{
				if (markUnread)
				{
					storeSession.MarkAsUnread(true, array);
				}
				else
				{
					storeSession.MarkAsRead(true, array);
				}
			}
			if (array2 != null && array2.Length > 0)
			{
				if (markUnread)
				{
					storeSession.MarkAsUnread(Utilities.ShouldSuppressReadReceipt(userContext), array2);
					return;
				}
				storeSession.MarkAsRead(Utilities.ShouldSuppressReadReceipt(userContext), array2);
			}
		}

		internal static AggregateOperationResult DeleteItems(UserContext userContext, DeleteItemFlags deleteItemFlags, params StoreId[] storeIds)
		{
			if (storeIds.Length == 0)
			{
				return new AggregateOperationResult(OperationResult.Succeeded, new GroupOperationResult[0]);
			}
			if (!IdConverter.IsMessageId(StoreId.GetStoreObjectId(storeIds[0])))
			{
				throw new ArgumentException("store Ids is not an item");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			Utilities.AbortSubmissionsBeforeDelete(userContext.MailboxSession, storeIds);
			AggregateOperationResult result = userContext.MailboxSession.Delete(deleteItemFlags, storeIds);
			if (Globals.ArePerfCountersEnabled)
			{
				OwaSingleCounters.ItemsDeleted.IncrementBy((long)storeIds.Length);
			}
			return result;
		}

		internal static AggregateOperationResult DeleteFolders(MailboxSession mailboxSession, DeleteItemFlags deleteItemFlags, params StoreId[] folderIds)
		{
			if (folderIds.Length == 0)
			{
				return new AggregateOperationResult(OperationResult.Succeeded, new GroupOperationResult[0]);
			}
			if (!IdConverter.IsFolderId(StoreId.GetStoreObjectId(folderIds[0])))
			{
				throw new ArgumentException("store Ids is not an folder");
			}
			if (mailboxSession == null)
			{
				throw new ArgumentNullException("mailboxSession");
			}
			return mailboxSession.Delete(deleteItemFlags, folderIds);
		}

		internal static AggregateOperationResult Delete(UserContext userContext, bool permanentDelete, params OwaStoreObjectId[] objectIds)
		{
			return Utilities.Delete(userContext, permanentDelete, DeleteItemFlags.None, objectIds);
		}

		internal static void Delete(UserContext userContext, bool permanentDelete, bool doThrow, params OwaStoreObjectId[] objectIds)
		{
			OperationResult operationResult = Utilities.Delete(userContext, permanentDelete, objectIds).OperationResult;
			if (operationResult != OperationResult.Succeeded && doThrow)
			{
				throw new OwaEventHandlerException("Deleting an item fails", LocalizedStrings.GetNonEncoded(1167467453), true);
			}
		}

		internal static AggregateOperationResult Delete(UserContext userContext, bool permanentDelete, DeleteItemFlags extraDeleteFlags, params OwaStoreObjectId[] objectIds)
		{
			if (objectIds.Length == 0)
			{
				return new AggregateOperationResult(OperationResult.Succeeded, new GroupOperationResult[0]);
			}
			DeleteItemFlags deleteItemFlags = permanentDelete ? DeleteItemFlags.SoftDelete : DeleteItemFlags.MoveToDeletedItems;
			deleteItemFlags |= extraDeleteFlags;
			return Utilities.Delete(userContext, deleteItemFlags, objectIds);
		}

		internal static AggregateOperationResult Delete(UserContext userContext, DeleteItemFlags deleteItemFlags, params OwaStoreObjectId[] objectIds)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (objectIds == null)
			{
				throw new ArgumentNullException("objectIds");
			}
			if (objectIds.Length == 0)
			{
				return new AggregateOperationResult(OperationResult.Succeeded, new GroupOperationResult[0]);
			}
			StoreSession session = objectIds[0].GetSession(userContext);
			if (session is MailboxSession && IdConverter.IsMessageId(objectIds[0].StoreObjectId))
			{
				Utilities.AbortSubmissionsBeforeDelete(session as MailboxSession, OwaStoreObjectId.ConvertToStoreObjectIdArray(objectIds));
			}
			AggregateOperationResult aggregateOperationResult;
			if (!objectIds[0].IsOtherMailbox)
			{
				if (objectIds.Length == 1)
				{
					aggregateOperationResult = session.Delete(deleteItemFlags, new StoreId[]
					{
						objectIds[0].StoreObjectId
					});
				}
				else
				{
					StoreObjectId[] ids = OwaStoreObjectId.ConvertToStoreObjectIdArray(objectIds);
					aggregateOperationResult = session.Delete(deleteItemFlags, ids);
				}
			}
			else
			{
				List<OwaStoreObjectId> list = new List<OwaStoreObjectId>(objectIds.Length);
				List<OwaStoreObjectId> list2 = new List<OwaStoreObjectId>(objectIds.Length);
				foreach (OwaStoreObjectId owaStoreObjectId in objectIds)
				{
					if (owaStoreObjectId.StoreObjectId is OccurrenceStoreObjectId)
					{
						list2.Add(owaStoreObjectId);
					}
					else
					{
						list.Add(owaStoreObjectId);
					}
				}
				AggregateOperationResult first = session.Delete(deleteItemFlags, OwaStoreObjectId.ConvertToStoreObjectIdArray(list2.ToArray()));
				AggregateOperationResult second = Utilities.CopyOrMoveItems(userContext, false, Utilities.TryGetDefaultFolderId(userContext, userContext.MailboxSession, DefaultFolderType.DeletedItems), new DeleteItemFlags?(deleteItemFlags), list.ToArray());
				aggregateOperationResult = AggregateOperationResult.Merge(first, second);
			}
			if (aggregateOperationResult.OperationResult == OperationResult.Succeeded && !Folder.IsFolderId(objectIds[0].StoreObjectId) && Globals.ArePerfCountersEnabled)
			{
				OwaSingleCounters.ItemsDeleted.IncrementBy((long)objectIds.Length);
			}
			return aggregateOperationResult;
		}

		internal static void AbortSubmissionsBeforeDelete(MailboxSession mailboxSession, StoreId[] itemIds)
		{
			StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Drafts);
			StoreObjectId defaultFolderId2 = mailboxSession.GetDefaultFolderId(DefaultFolderType.Outbox);
			foreach (StoreId storeId in itemIds)
			{
				if (IdConverter.GetParentIdFromMessageId(StoreId.GetStoreObjectId(storeId)).Equals(defaultFolderId) || IdConverter.GetParentIdFromMessageId(StoreId.GetStoreObjectId(storeId)).Equals(defaultFolderId2))
				{
					using (Item item = Utilities.GetItem<Item>(mailboxSession, storeId, false, new PropertyDefinition[]
					{
						MessageItemSchema.HasBeenSubmitted
					}))
					{
						if (item is MessageItem)
						{
							MessageItem messageItem = (MessageItem)item;
							if (messageItem.GetValueOrDefault<bool>(MessageItemSchema.HasBeenSubmitted))
							{
								messageItem.AbortSubmit();
							}
						}
					}
				}
			}
		}

		internal static AggregateOperationResult CopyOrMoveItems(UserContext userContext, bool isCopy, OwaStoreObjectId destinationFolderId, params OwaStoreObjectId[] ids)
		{
			return Utilities.CopyOrMoveItems(userContext, isCopy, destinationFolderId, null, ids);
		}

		internal static AggregateOperationResult CopyOrMoveItems(UserContext userContext, bool isCopy, OwaStoreObjectId destinationFolderId, DeleteItemFlags? deleteFlags, params OwaStoreObjectId[] ids)
		{
			if (destinationFolderId == null)
			{
				throw new ArgumentNullException("destinationFolderId");
			}
			if (ids == null)
			{
				throw new ArgumentNullException("ids");
			}
			if (ids.Length == 0)
			{
				return new AggregateOperationResult(OperationResult.Succeeded, new GroupOperationResult[0]);
			}
			StoreSession sessionForFolderContent = destinationFolderId.GetSessionForFolderContent(userContext);
			StoreSession session = ids[0].GetSession(userContext);
			StoreObjectId[] ids2 = OwaStoreObjectId.ConvertToStoreObjectIdArray(ids);
			if (isCopy)
			{
				return session.Copy(sessionForFolderContent, destinationFolderId.StoreObjectId, ids2);
			}
			return session.Move(sessionForFolderContent, destinationFolderId.StoreObjectId, deleteFlags, ids2);
		}

		internal static AggregateOperationResult CopyOrMoveFolder(UserContext userContext, bool isCopy, OwaStoreObjectId destinationFolderId, params OwaStoreObjectId[] sourceFolderIds)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (destinationFolderId == null)
			{
				throw new ArgumentNullException("destinationFolderId");
			}
			if (sourceFolderIds == null)
			{
				throw new ArgumentNullException("sourceFolderIds");
			}
			if (sourceFolderIds.Length == 0)
			{
				return new AggregateOperationResult(OperationResult.Succeeded, new GroupOperationResult[0]);
			}
			return Utilities.GetSessionsForCopyOrMoveFolder(userContext, destinationFolderId, sourceFolderIds, isCopy);
		}

		private static AggregateOperationResult GetSessionsForCopyOrMoveFolder(UserContext userContext, OwaStoreObjectId destinationFolderId, OwaStoreObjectId[] sourceFolderIds, bool isCopy)
		{
			StoreSession storeSession;
			StoreSession destinationSession;
			if (isCopy || (!isCopy && sourceFolderIds[0].IsPublic != destinationFolderId.IsPublic))
			{
				storeSession = sourceFolderIds[0].GetSessionForFolderContent(userContext);
				destinationSession = destinationFolderId.GetSession(userContext);
			}
			else if (!isCopy && sourceFolderIds[0].IsPublic && destinationFolderId.IsPublic)
			{
				storeSession = sourceFolderIds[0].GetSession(userContext);
				destinationSession = storeSession;
			}
			else
			{
				storeSession = sourceFolderIds[0].GetSession(userContext);
				destinationSession = destinationFolderId.GetSession(userContext);
			}
			StoreObjectId[] ids = OwaStoreObjectId.ConvertToStoreObjectIdArray(sourceFolderIds);
			if (isCopy)
			{
				return storeSession.Copy(destinationSession, destinationFolderId.StoreObjectId, ids);
			}
			return storeSession.Move(destinationSession, destinationFolderId.StoreObjectId, ids);
		}

		internal static Folder CreateSubFolder(OwaStoreObjectId destinationFolderId, StoreObjectType folderType, string folderName, UserContext userContext)
		{
			if (destinationFolderId == null)
			{
				throw new ArgumentNullException("destinationFolderId");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (string.IsNullOrEmpty(folderName))
			{
				throw new ArgumentNullException("folderName");
			}
			StoreSession session = destinationFolderId.GetSession(userContext);
			return Folder.Create(session, destinationFolderId.StoreObjectId, folderType, folderName, CreateMode.CreateNew);
		}

		public static string GetDefaultCultureFontCssFileUrl(OwaContext owaContext)
		{
			if (owaContext == null)
			{
				throw new ArgumentNullException("owaContext");
			}
			return Microsoft.Exchange.Clients.Owa.Core.Culture.GetDefaultCultureCssFontFileName(owaContext);
		}

		public static string GetFontCssFileUrlForUICulture()
		{
			return Microsoft.Exchange.Clients.Owa.Core.Culture.GetCssFontFileNameFromCulture();
		}

		internal static void JunkEmailRuleSynchronizeContactsCache(JunkEmailRule junkEmailRule)
		{
			try
			{
				junkEmailRule.SynchronizeContactsCache();
			}
			catch (JunkEmailValidationException)
			{
				ExTraceGlobals.CoreTracer.TraceDebug(0L, "JunkEmailValidationException was thrown by JunkEmailRule.SynchronizeContactsCache method");
			}
		}

		internal static ADRecipient GetRecipientByLegacyExchangeDN(IRecipientSession session, string legacyExchangeDN)
		{
			if (string.IsNullOrEmpty(legacyExchangeDN))
			{
				ExTraceGlobals.CoreTracer.TraceDebug(0L, "Utilities.GetRecipientByLegacyExchangeDN: legacyExchangeDN is null or empty");
				return null;
			}
			ADRecipient result = null;
			try
			{
				result = session.FindByLegacyExchangeDN(legacyExchangeDN);
			}
			catch (NonUniqueRecipientException arg)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<NonUniqueRecipientException>(0L, "Utilities.GetRecipientByLegacyExchangeDN: NonUniqueRecipientException was thrown by FindByLegacyExchangeDN: {0}", arg);
			}
			return result;
		}

		internal static ADRawEntry GetAdRecipientByLegacyExchangeDN(IRecipientSession session, string legacyExchangeDN)
		{
			if (string.IsNullOrEmpty(legacyExchangeDN))
			{
				ExTraceGlobals.CoreTracer.TraceDebug(0L, "Utilities.GetAdRecipientByLegacyExchangeDN: legacyExchangeDN is null or empty");
				return null;
			}
			Result<ADRawEntry>[] array = null;
			string[] legacyExchangeDNs = new string[]
			{
				legacyExchangeDN
			};
			try
			{
				array = session.FindByLegacyExchangeDNs(legacyExchangeDNs, Utilities.adFindByExchangeLegacyDnProperties);
			}
			catch (NonUniqueRecipientException arg)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<NonUniqueRecipientException>(0L, "Utilities.GetAdRecipientByLegacyExchangeDN: NonUniqueRecipientException was thrown by FindByLegacyExchangeDN: {0}", arg);
			}
			if (array == null || array.Length <= 0)
			{
				return null;
			}
			return array[0].Data;
		}

		internal static ADRecipient CreateADRecipientFromProxyAddress(ADObjectId objectId, string routingAddress, IRecipientSession session)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			ADRecipient adrecipient = null;
			if (objectId != null)
			{
				adrecipient = session.Read(objectId);
			}
			if (adrecipient == null && !string.IsNullOrEmpty(routingAddress))
			{
				try
				{
					CustomProxyAddress proxyAddress = new CustomProxyAddress((CustomProxyAddressPrefix)ProxyAddressPrefix.LegacyDN, routingAddress, true);
					adrecipient = session.FindByProxyAddress(proxyAddress);
				}
				catch (NonUniqueRecipientException ex)
				{
					ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "Failed to get unique recipient: Error: {0}", ex.Message);
				}
			}
			return adrecipient;
		}

		internal static ADRecipient CreateADRecipientPrimarySmtpAddress(UserContext userContext, bool readOnly, out IRecipientSession recipientSession)
		{
			recipientSession = Utilities.CreateADRecipientSession(readOnly, ConsistencyMode.IgnoreInvalid, userContext);
			SmtpProxyAddress proxyAddress = new SmtpProxyAddress(userContext.ExchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString(), true);
			return recipientSession.FindByProxyAddress(proxyAddress);
		}

		private static void RenderScriptTag(TextWriter writer, string fileName, int index, ScriptFlags scriptFlags)
		{
			string s;
			if (fileName == "clientstrings.aspx")
			{
				s = Utilities.GetClientStringsFileNameWithPath();
			}
			else if (fileName == "smallicons.aspx")
			{
				s = Utilities.GetSmallIconsFileNameWithPath();
			}
			else
			{
				s = Utilities.GetScriptFullPath(fileName);
			}
			writer.Write("<script id=_scr");
			writer.Write(index);
			writer.Write(" _sid=\"");
			writer.Write(fileName);
			writer.Write("\" type=\"text/javascript\"");
			if (Utilities.IsFlagSet((int)scriptFlags, 2))
			{
				writer.Write(" _src=\"");
			}
			else
			{
				writer.Write(" src=\"");
			}
			writer.Write(Utilities.SanitizeHtmlEncode(s));
			writer.Write("\"></script>\n");
		}

		public static string GetScriptFullPath(string fileName)
		{
			string result;
			if (fileName.EndsWith(".js", StringComparison.InvariantCultureIgnoreCase))
			{
				result = Globals.ContentDeliveryNetworkEndpoint + Utilities.PremiumScriptPath + fileName;
			}
			else
			{
				result = Utilities.PremiumScriptPath + fileName;
			}
			return result;
		}

		private static string GetClientStringsFileNameWithPath()
		{
			return "forms/premium/clientstrings.aspx?v=" + Globals.ApplicationVersion + "&l=" + CultureInfo.CurrentUICulture.Name;
		}

		private static string GetSmallIconsFileNameWithPath()
		{
			return "forms/premium/smallicons.aspx?v=" + Globals.ApplicationVersion;
		}

		public static void RenderScriptTagStart(TextWriter writer)
		{
			writer.Write("<script type=\"text/javascript\">");
		}

		public static void RenderScriptTagEnd(TextWriter writer)
		{
			writer.Write("</script>");
		}

		public static void RenderScripts(TextWriter writer, ISessionContext sessionContext, ScriptFlags scriptFlags, params string[] fileNames)
		{
			Utilities.RenderScriptTagStart(writer);
			Utilities.RenderInlineScripts(writer, sessionContext);
			Utilities.RenderScriptTagEnd(writer);
			Utilities.RenderExternalScripts(writer, scriptFlags, fileNames);
		}

		public static void RenderOWAFlag(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write("var g_fOwa=1;");
		}

		public static void RenderBootUpScripts(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write("var onWL=0;");
			writer.Write("var oJS = new Object();");
			writer.Write(" function isJS(f) {return oJS[f]?1:0;}; function stJS(f){oJS[f]=true;};");
			writer.Write(" var _wl=0;");
			writer.Write(" function _e(_this, s, event) {if (_wl) eval(s);};");
		}

		public static void RenderInlineScripts(TextWriter writer, ISessionContext sessionContext)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			Utilities.RenderOWAFlag(writer);
			Utilities.RenderBootUpScripts(writer);
			Utilities.RenderGlobalJavascriptVariables(writer, sessionContext);
			Utilities.RenderScriptToEnforceUTF8ForPage(writer);
			Utilities.RenderScriptDisplayPictureOnLoad(writer);
		}

		public static void RenderExternalScripts(TextWriter writer, ScriptFlags scriptFlags, IEnumerable<string> fileNames)
		{
			if (fileNames == null)
			{
				throw new ArgumentNullException("fileNames");
			}
			Utilities.RenderScriptTag(writer, "clientstrings.aspx", 0, scriptFlags & ~ScriptFlags.DeferredLoading);
			if (Utilities.IsFlagSet((int)scriptFlags, 1))
			{
				Utilities.RenderScriptTag(writer, "uglobal.js", 1, scriptFlags & ~ScriptFlags.DeferredLoading);
			}
			int num = 0;
			foreach (string fileName in fileNames)
			{
				Utilities.RenderScriptTag(writer, fileName, num + 2, scriptFlags);
				num++;
			}
			if (Utilities.IsFlagSet((int)scriptFlags, 2))
			{
				Utilities.RenderScriptTagStart(writer);
				writer.Write("for (var i = 0; i < " + (num + 2) + "; i++){");
				writer.Write(" var o =window.document.getElementById(\"_scr\" + i);");
				writer.Write(" if (o && o.getAttribute(\"_src\")) o.setAttribute(\"src\",o.getAttribute(\"_src\"));");
				writer.Write("}\n");
				Utilities.RenderScriptTagEnd(writer);
			}
		}

		public static void RenderGlobalJavascriptVariables(TextWriter writer, ISessionContext sessionContext)
		{
			Utilities.RenderCDNEndpointVariable(writer);
			writer.Write(" var a_fEnbSMm=");
			writer.Write(sessionContext.IsFeatureEnabled(Feature.SMime) ? "1" : "0");
			writer.Write(";");
			UserContext userContext = sessionContext as UserContext;
			if (userContext != null)
			{
				RenderingUtilities.RenderStringVariable(writer, "a_sMailboxGuid", Utilities.JavascriptEncode(userContext.ExchangePrincipal.MailboxInfo.MailboxGuid.ToString()));
				if (userContext.IsDifferentMailbox)
				{
					RenderingUtilities.RenderInteger(writer, "a_fIsExpLgn", 1);
					RenderingUtilities.RenderStringVariable(writer, "a_sMailboxUniqueId", Utilities.JavascriptEncode(userContext.MailboxIdentity.UniqueId));
				}
				userContext.RenderCustomizedFormRegistry(writer);
			}
		}

		public static void RenderCDNEndpointVariable(TextWriter writer)
		{
			if (!string.IsNullOrEmpty(Globals.ContentDeliveryNetworkEndpoint))
			{
				writer.Write("var a_sCDN = \"");
				writer.Write(Globals.ContentDeliveryNetworkEndpoint);
				writer.Write("\";");
			}
		}

		public static void RenderClientStrings(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write("<script type=\"text/javascript\" src=\"" + Utilities.GetClientStringsFileNameWithPath() + "\"></script>");
		}

		public static void RenderScriptHandler(SanitizingStringBuilder<OwaHtml> stringBuilder, string eventName, string handlerCode)
		{
			if (stringBuilder == null)
			{
				throw new ArgumentNullException("stringBuilder");
			}
			stringBuilder.Append<SanitizedEventHandlerString>(Utilities.GetScriptHandler(eventName, handlerCode));
		}

		public static void RenderScriptHandler(TextWriter writer, string eventName, string handlerCode)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write(new SanitizedEventHandlerString(eventName, handlerCode, false));
		}

		public static void RenderScriptHandler(TextWriter writer, string eventName, string handlerCode, bool returnFalse)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write(new SanitizedEventHandlerString(eventName, handlerCode, returnFalse));
		}

		public static SanitizedEventHandlerString GetScriptHandler(string eventName, string handlerCode)
		{
			return new SanitizedEventHandlerString(eventName, handlerCode, false);
		}

		internal static bool IsADDistributionList(RecipientType recipientType)
		{
			return recipientType == RecipientType.Group || recipientType == RecipientType.MailUniversalDistributionGroup || recipientType == RecipientType.MailUniversalSecurityGroup || recipientType == RecipientType.MailNonUniversalGroup || recipientType == RecipientType.DynamicDistributionGroup;
		}

		internal static bool IsADDistributionList(MultiValuedProperty<string> objectClass)
		{
			return objectClass.Contains(ADGroup.MostDerivedClass) || objectClass.Contains(ADDynamicGroup.MostDerivedClass);
		}

		internal static string GetContentTypeString(OwaEventContentType contentType)
		{
			switch (contentType)
			{
			case OwaEventContentType.Html:
				return "text/html";
			case OwaEventContentType.Javascript:
				return "application/x-javascript";
			case OwaEventContentType.PlainText:
				return "text/plain";
			case OwaEventContentType.Css:
				return "text/css";
			case OwaEventContentType.Jpeg:
				return "image/jpeg";
			default:
				throw new ArgumentOutOfRangeException("contentType");
			}
		}

		internal static string GetItemIdQueryString(HttpRequest request)
		{
			StringBuilder stringBuilder = new StringBuilder();
			NameValueCollection queryString = request.QueryString;
			for (int i = 0; i < queryString.Count; i++)
			{
				string key = queryString.GetKey(i);
				if (string.CompareOrdinal(key, "id") == 0 || string.CompareOrdinal(key, "attcnt") == 0 || key.StartsWith("attid", StringComparison.Ordinal))
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append('&');
					}
					stringBuilder.Append(key);
					stringBuilder.Append('=');
					stringBuilder.Append(Utilities.UrlEncode(queryString.GetValues(i)[0]));
				}
			}
			return stringBuilder.ToString();
		}

		internal static bool IsClearSigned(IStorePropertyBag storePropertyBag)
		{
			if (storePropertyBag == null)
			{
				throw new ArgumentNullException("storePropertyBag");
			}
			string property = ItemUtility.GetProperty<string>(storePropertyBag, StoreObjectSchema.ItemClass, string.Empty);
			return ObjectClass.IsOfClass(property, "IPM.Note.Secure.Sign") || ObjectClass.IsSmimeClearSigned(property);
		}

		internal static bool IsSMime(IStorePropertyBag storePropertyBag)
		{
			if (storePropertyBag == null)
			{
				throw new ArgumentNullException("storePropertyBag");
			}
			string property = ItemUtility.GetProperty<string>(storePropertyBag, StoreObjectSchema.ItemClass, string.Empty);
			return ObjectClass.IsSmime(property);
		}

		internal static bool IsOpaqueSigned(Item item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			string property = ItemUtility.GetProperty<string>(item, StoreObjectSchema.ItemClass, string.Empty);
			return (ObjectClass.IsOfClass(property, "IPM.Note.Secure") || (ObjectClass.IsSmime(property) && !ObjectClass.IsSmimeClearSigned(property))) && ConvertUtils.IsMessageOpaqueSigned(item);
		}

		internal static bool IsEncrypted(Item item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			string property = ItemUtility.GetProperty<string>(item, StoreObjectSchema.ItemClass, string.Empty);
			return ObjectClass.IsOfClass(property, "IPM.Note.SMIME") && !Utilities.IsOpaqueSigned(item);
		}

		internal static void DisconnectStoreSession(StoreSession session)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (session.IsConnected)
			{
				session.Disconnect();
			}
		}

		internal static void DisconnectStoreSessionSafe(StoreSession session)
		{
			try
			{
				Utilities.DisconnectStoreSession(session);
			}
			catch (Exception ex)
			{
				ExTraceGlobals.CoreCallTracer.TraceDebug<string>(0L, "Unexpected exception in DisconnetStoreSession. Exception: {0}", ex.Message);
			}
		}

		internal static void ReconnectStoreSession(StoreSession session, UserContext userContext)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			session.AccountingObject = OwaContext.TryGetCurrentBudget();
			MailboxSession mailboxSession = session as MailboxSession;
			if (!session.IsConnected)
			{
				if (mailboxSession != null)
				{
					bool flag = mailboxSession.ConnectWithStatus();
					if (flag && userContext.MapiNotificationManager != null)
					{
						userContext.MapiNotificationManager.HandleConnectionDroppedNotification();
						return;
					}
				}
				else
				{
					session.Connect();
				}
			}
		}

		internal static bool IsSMimeControlNeededForEditForm(string smimeParameter, OwaContext owaContext)
		{
			return Utilities.IsSMimeControlNeededForEditForm(Utilities.CheckClientSMimeControlStatus(smimeParameter, owaContext), owaContext);
		}

		public static bool IsSMimeFeatureUsable(OwaContext owaContext)
		{
			return owaContext.UserContext.IsFeatureEnabled(Feature.SMime) && owaContext.UserContext.ClientBrowserStatus == ClientBrowserStatus.IE7OrLaterIn32Bit;
		}

		internal static bool CheckSMimeEditFormBasicRequirement(ClientSMimeControlStatus clientSMimeControlStatus, OwaContext owaContext)
		{
			return !owaContext.UserContext.IsExplicitLogonOthersMailbox && Utilities.IsFlagSet((int)clientSMimeControlStatus, 16);
		}

		internal static bool IsSMimeControlNeededForEditForm(ClientSMimeControlStatus clientSMimeControlStatus, OwaContext owaContext)
		{
			return Utilities.CheckSMimeEditFormBasicRequirement(clientSMimeControlStatus, owaContext) && Utilities.IsClientSMimeControlUsable(clientSMimeControlStatus);
		}

		public static string ReadSMimeControlVersionOnServer()
		{
			if (string.IsNullOrEmpty(Utilities.smimeVersion))
			{
				int num = 0;
				string szDatabasePath = HttpRuntime.AppDomainAppPath + "\\smime\\owasmime.msi";
				SafeMsiHandle safeMsiHandle = null;
				SafeMsiHandle safeMsiHandle2 = null;
				SafeMsiHandle safeMsiHandle3 = null;
				StringBuilder stringBuilder = new StringBuilder(64);
				int num2 = 64;
				try
				{
					num = SafeNativeMethods.MsiOpenDatabase(szDatabasePath, 0, out safeMsiHandle);
					if (num == 0)
					{
						num = SafeNativeMethods.MsiDatabaseOpenView(safeMsiHandle, "SELECT Value FROM Property WHERE Property='ProductVersion'", out safeMsiHandle2);
					}
					if (num == 0)
					{
						using (SafeMsiHandle safeMsiHandle4 = new SafeMsiHandle(IntPtr.Zero))
						{
							num = SafeNativeMethods.MsiViewExecute(safeMsiHandle2, safeMsiHandle4);
						}
					}
					if (num == 0)
					{
						num = SafeNativeMethods.MsiViewFetch(safeMsiHandle2, out safeMsiHandle3);
					}
					if (num == 0)
					{
						num = SafeNativeMethods.MsiRecordGetString(safeMsiHandle3, 1, stringBuilder, ref num2);
					}
				}
				finally
				{
					if (safeMsiHandle3 != null)
					{
						safeMsiHandle3.Close();
					}
					if (safeMsiHandle2 != null)
					{
						safeMsiHandle2.Close();
					}
					if (safeMsiHandle != null)
					{
						safeMsiHandle.Close();
					}
					if (num != 0)
					{
						ExTraceGlobals.CoreCallTracer.TraceDebug<int>(0L, "Failed to open owasmime.msi to get version information. Error Code: {0}", num);
						using (SafeMsiHandle safeMsiHandle5 = SafeNativeMethods.MsiGetLastErrorRecord())
						{
							if (!safeMsiHandle5.IsInvalid)
							{
								StringBuilder stringBuilder2 = new StringBuilder();
								int num3 = 0;
								using (SafeMsiHandle safeMsiHandle6 = new SafeMsiHandle(IntPtr.Zero))
								{
									int num4 = SafeNativeMethods.MsiFormatRecord(safeMsiHandle6, safeMsiHandle5, stringBuilder2, ref num3);
									if (234 == num4)
									{
										num3++;
										stringBuilder2 = new StringBuilder(num3);
										if (SafeNativeMethods.MsiFormatRecord(safeMsiHandle6, safeMsiHandle5, stringBuilder2, ref num3) == 0)
										{
											ExTraceGlobals.CoreCallTracer.TraceDebug<string>(0L, "Extended error from MSI: {0}", stringBuilder2.ToString());
										}
									}
								}
							}
						}
					}
				}
				Utilities.smimeVersion = stringBuilder.ToString();
			}
			return Utilities.smimeVersion;
		}

		internal static ClientSMimeControlStatus CheckClientSMimeControlStatus(string smimeParameter, OwaContext owaContext)
		{
			if (smimeParameter == null || !Utilities.IsSMimeFeatureUsable(owaContext))
			{
				return ClientSMimeControlStatus.NotInstalled;
			}
			SmimeParameterParser smimeParameterParser = new SmimeParameterParser(smimeParameter);
			if (smimeParameterParser.SmimeControlVersion == null)
			{
				return ClientSMimeControlStatus.NotInstalled;
			}
			try
			{
				ClientSMimeControlStatus clientSMimeControlStatus = ClientSMimeControlStatus.None;
				if (smimeParameterParser.ConnectionIsSSL)
				{
					clientSMimeControlStatus |= ClientSMimeControlStatus.ConnectionIsSSL;
				}
				Version v = new Version(smimeParameterParser.SmimeControlVersion);
				Version v2 = new Version(Utilities.ReadSMimeControlVersionOnServer());
				if (v < v2)
				{
					if (OwaRegistryKeys.ForceSMimeClientUpgrade)
					{
						clientSMimeControlStatus |= ClientSMimeControlStatus.MustUpdate;
					}
					else
					{
						clientSMimeControlStatus |= ClientSMimeControlStatus.Outdated;
					}
				}
				else
				{
					clientSMimeControlStatus |= ClientSMimeControlStatus.OK;
				}
				return clientSMimeControlStatus;
			}
			catch (ArgumentException)
			{
			}
			catch (FormatException)
			{
			}
			catch (OverflowException)
			{
			}
			return ClientSMimeControlStatus.NotInstalled;
		}

		public static bool IsClientSMimeControlUsable(ClientSMimeControlStatus status)
		{
			return Utilities.IsFlagSet((int)status, 8) || Utilities.IsFlagSet((int)status, 4);
		}

		public static int? GetMaximumMessageSize(UserContext userContext)
		{
			object obj = userContext.MailboxSession.Mailbox.TryGetProperty(MailboxSchema.MaxUserMessageSize);
			if (!(obj is PropertyError))
			{
				return new int?((int)obj);
			}
			return null;
		}

		public static Uri AppendSmtpAddressToUrl(Uri url, string smtpAddress)
		{
			UriBuilder uriBuilder = new UriBuilder(url);
			if (!uriBuilder.Path.EndsWith("/", StringComparison.Ordinal))
			{
				UriBuilder uriBuilder2 = uriBuilder;
				uriBuilder2.Path += "/";
			}
			UriBuilder uriBuilder3 = uriBuilder;
			uriBuilder3.Path += smtpAddress;
			return uriBuilder.Uri;
		}

		public static string BuildErrorMessageForFailoverRedirection(bool showErrorInDialogForOeh, Uri failoverRedirectionUrl)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(LocalizedStrings.GetHtmlEncoded(102674960));
			if (failoverRedirectionUrl != null)
			{
				stringBuilder.Append("<div");
				if (showErrorInDialogForOeh)
				{
					stringBuilder.Append(" style=\"display:none\"");
				}
				stringBuilder.Append(">");
				string text = string.Format(CultureInfo.InvariantCulture, "<span id=\"spRedirUrl\">{0}</span>", new object[]
				{
					Utilities.HtmlEncode(failoverRedirectionUrl.ToString())
				});
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, LocalizedStrings.GetHtmlEncoded(-1407904215), new object[]
				{
					text
				});
				stringBuilder.Append("</div>");
			}
			return stringBuilder.ToString();
		}

		public static void RenderDirectionEnhancedValue(TextWriter output, SanitizedHtmlString value, bool isRtl)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			Utilities.RenderDirectionEnhancedValue(output, value.ToString(), isRtl);
		}

		public static void RenderDirectionEnhancedValue(TextWriter output, string value, bool isRtl)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			SanitizedHtmlString sanitizedStringWithoutEncoding = SanitizedHtmlString.GetSanitizedStringWithoutEncoding(isRtl ? "&#x200F;" : "&#x200E;");
			int i = 0;
			while (i < value.Length)
			{
				char c = value[i];
				switch (c)
				{
				case '(':
					goto IL_65;
				case ')':
					goto IL_7B;
				default:
					switch (c)
					{
					case '[':
						goto IL_65;
					case ']':
						goto IL_7B;
					}
					output.Write(value[i]);
					break;
				}
				IL_9E:
				i++;
				continue;
				IL_65:
				output.Write(sanitizedStringWithoutEncoding);
				output.Write(value[i]);
				goto IL_9E;
				IL_7B:
				output.Write(value[i]);
				output.Write(sanitizedStringWithoutEncoding);
				goto IL_9E;
			}
		}

		public static string SanitizeHtml(string unsafeHtml)
		{
			if (unsafeHtml == null)
			{
				throw new ArgumentNullException("unsafeHtml");
			}
			string result;
			using (Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(unsafeHtml)))
			{
				try
				{
					HtmlToHtml htmlToHtml = new HtmlToHtml();
					TextConvertersInternalHelpers.SetPreserveDisplayNoneStyle(htmlToHtml, true);
					htmlToHtml.InputEncoding = Encoding.UTF8;
					htmlToHtml.OutputEncoding = Encoding.UTF8;
					htmlToHtml.FilterHtml = true;
					using (ConverterStream converterStream = new ConverterStream(stream, htmlToHtml, ConverterStreamAccess.Read))
					{
						using (StreamReader streamReader = new StreamReader(converterStream, Encoding.UTF8))
						{
							result = streamReader.ReadToEnd();
						}
					}
				}
				catch (ExchangeDataException innerException)
				{
					throw new OwaBodyConversionFailedException("Sanitize Html Failed", innerException);
				}
				catch (StoragePermanentException innerException2)
				{
					throw new OwaBodyConversionFailedException("Html Conversion Failed", innerException2);
				}
				catch (StorageTransientException innerException3)
				{
					throw new OwaBodyConversionFailedException("Html Conversion Failed", innerException3);
				}
			}
			return result;
		}

		public static string DecodeIDNDomain(string smtpAddress)
		{
			return Utilities.DecodeIDNDomain(new SmtpAddress(smtpAddress));
		}

		public static string DecodeIDNDomain(SmtpAddress smtpAddress)
		{
			string domain = smtpAddress.Domain;
			if (!string.IsNullOrEmpty(domain))
			{
				IdnMapping idnMapping = new IdnMapping();
				string unicode = idnMapping.GetUnicode(domain);
				return smtpAddress.Local + "@" + unicode;
			}
			return smtpAddress.ToString();
		}

		public static string GetTDClassForWebReadyViewHead(bool isBasicExperience)
		{
			if (isBasicExperience)
			{
				return "bigFont";
			}
			return string.Empty;
		}

		internal static X509Certificate2 GetADRecipientCertificate(ADRawEntry adRecipient, bool checkRevocation)
		{
			byte[][] array = Utilities.FindCertificatesForADRecipient(adRecipient);
			if (array.Length == 0)
			{
				return null;
			}
			string[] array2;
			if (OwaRegistryKeys.UseSecondaryProxiesWhenFindingCertificates)
			{
				ProxyAddressCollection proxyAddressCollection = adRecipient[ADRecipientSchema.EmailAddresses] as ProxyAddressCollection;
				if (proxyAddressCollection != null && proxyAddressCollection.Count > 0)
				{
					array2 = new string[proxyAddressCollection.Count];
					for (int i = 0; i < proxyAddressCollection.Count; i++)
					{
						array2[i] = proxyAddressCollection[i].AddressString;
					}
				}
				else
				{
					array2 = new string[0];
				}
			}
			else
			{
				array2 = new string[]
				{
					adRecipient[ADRecipientSchema.PrimarySmtpAddress].ToString()
				};
			}
			return Utilities.FindBestCertificate(array, array2, false, checkRevocation);
		}

		public static void RenderScriptToEnforceUTF8ForPage(TextWriter writer)
		{
			OwaContext owaContext = OwaContext.Current;
			ISessionContext sessionContext = owaContext.SessionContext;
			SanitizedHtmlString sanitizedHtmlString = new SanitizedHtmlString(Utilities.JavascriptEncode(Utilities.NCREncode(LocalizedStrings.GetNonEncoded(257251160))));
			sanitizedHtmlString.DecreeToBeTrusted();
			writer.Write("function _htmlDec(s){var o=document.createElement(\"DIV\");o.innerHTML=s;return o.innerText||o.textContent;}");
			writer.Write("function chkEn(){");
			RenderingUtilities.RenderStringVariable(writer, "s1", Utilities.comparingStringForEncodingDetecting);
			writer.Write("if(_htmlDec(\"&#20320;&#22909;&#65;\")!=s1){alert(_htmlDec(\"");
			writer.Write(sanitizedHtmlString);
			writer.Write("\"));try{window.top.document.location=\"");
			writer.Write(Utilities.JavascriptEncode(OwaUrl.Logoff.GetExplicitUrl(owaContext)));
			if (sessionContext.IsBasicExperience)
			{
				writer.Write("?canary=");
				writer.Write(Utilities.JavascriptEncode(Utilities.UrlEncode(Utilities.GetCurrentCanary(sessionContext))));
				string canary15CookieValue = Utilities.GetCanary15CookieValue();
				if (canary15CookieValue != null)
				{
					writer.Write(Utilities.JavascriptEncode("&X-OWA-CANARY=" + canary15CookieValue));
				}
			}
			writer.Write("\";}catch(e){}}}");
			writer.Write("chkEn();");
		}

		public static void RenderScriptDisplayPictureOnLoad(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.WriteLine(string.Empty);
			writer.WriteLine("function clipDispPic(oImg, maxSize)\r\n            {\r\n                var tmpImg = new Image();\r\n                tmpImg.src = oImg.src;\r\n                if(tmpImg.width < tmpImg.height)\r\n                { \r\n                oImg.style.maxWidth=maxSize+'px';\r\n                if(tmpImg.height > maxSize)\r\n                { \r\n                    var k = 1;\r\n                    if(tmpImg.width > maxSize)\r\n                        k = maxSize / tmpImg.width;\r\n                    oImg.style.top=(-1*(((k*tmpImg.height)-maxSize)/2)).toString()+'px';\r\n                }\r\n                }\r\n                else \r\n                { \r\n                oImg.style.maxHeight=maxSize+'px';\r\n                if(tmpImg.width > maxSize)\r\n                { \r\n                    var k =1;\r\n                    if(tmpImg.height > maxSize)\r\n                        k = maxSize / tmpImg.height;\r\n                    oImg.style.left=(-1*(((k*tmpImg.width)-maxSize)/2)).toString()+'px';\r\n                }\r\n                }\r\n                oImg.parentNode.className = '';\r\n                oImg.style.opacity='1'; \r\n                oImg.style.filter='alpha(opacity=100)';\r\n            }");
		}

		internal static string NCREncode(string input)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (char c in input)
			{
				stringBuilder.Append("&#");
				uint num = (uint)c;
				stringBuilder.Append(num.ToString());
				stringBuilder.Append(";");
			}
			return stringBuilder.ToString();
		}

		internal static X509Certificate2 FindBestCertificate(byte[][] certBlobs, IEnumerable<string> emails, bool isContact, bool checkRevocation)
		{
			if (certBlobs == null)
			{
				return null;
			}
			X509CertificateCollection x509CertificateCollection = new X509CertificateCollection();
			foreach (byte[] rawData in certBlobs)
			{
				if (isContact)
				{
					x509CertificateCollection.ImportFromContact(rawData);
				}
				else
				{
					x509CertificateCollection.Import(rawData);
				}
			}
			return x509CertificateCollection.FindSMimeCertificate(emails, X509KeyUsageFlags.KeyEncipherment, checkRevocation, TimeSpan.FromMilliseconds((double)OwaRegistryKeys.CRLConnectionTimeout), TimeSpan.FromMilliseconds((double)OwaRegistryKeys.CRLRetrievalTimeout), null, null);
		}

		internal static byte[][] FindCertificatesForADRecipient(ADRawEntry adRecipient)
		{
			if (adRecipient == null)
			{
				throw new ArgumentNullException("adRecipient");
			}
			byte[][] origin = Utilities.MultiValuePropertyToByteArray(adRecipient[ADRecipientSchema.Certificate] as MultiValuedProperty<byte[]>);
			byte[][] appendant = Utilities.MultiValuePropertyToByteArray(adRecipient[ADRecipientSchema.SMimeCertificate] as MultiValuedProperty<byte[]>);
			return Utilities.AppendArray(origin, appendant);
		}

		internal static bool IsSMimeButNotSecureSign(Item message)
		{
			return !ObjectClass.IsOfClass(message.ClassName, "IPM.Note.Secure.Sign") && ObjectClass.IsSmime(message.ClassName);
		}

		internal static Item OpenSMimeContent(Item smimeMessage)
		{
			Item item = null;
			if (ItemConversion.TryOpenSMimeContent(smimeMessage as MessageItem, OwaContext.Current.UserContext.Configuration.DefaultAcceptedDomain.DomainName.ToString(), out item))
			{
				OwaContext.Current.AddObjectToDisposeOnEndRequest(item);
				return item;
			}
			return null;
		}

		internal static AttachmentCollection GetAttachmentCollection(Item message, bool unpackAttachmentForSmimeMessage, UserContext userContext)
		{
			AttachmentCollection attachmentCollection = null;
			if (unpackAttachmentForSmimeMessage && Utilities.IsSMimeButNotSecureSign(message))
			{
				Item item = Utilities.OpenSMimeContent(message);
				if (item != null)
				{
					attachmentCollection = item.AttachmentCollection;
				}
			}
			else if (userContext.IsIrmEnabled && !userContext.IsBasicExperience && Utilities.IsIrmDecrypted(message))
			{
				attachmentCollection = ((RightsManagedMessageItem)message).ProtectedAttachmentCollection;
			}
			if (attachmentCollection == null)
			{
				attachmentCollection = message.AttachmentCollection;
			}
			return attachmentCollection;
		}

		internal static bool IsWebBeaconsAllowed(IStorePropertyBag storePropertyBag)
		{
			return ItemUtility.GetProperty<int>(storePropertyBag, ItemSchema.BlockStatus, 0) == 3;
		}

		internal static bool IsFlagSet(int valueToTest, int flag)
		{
			return (valueToTest & flag) == flag;
		}

		internal static bool IsAllDayEvent(ExDateTime start, ExDateTime end)
		{
			return start.TimeOfDay.TotalSeconds == 0.0 && end.TimeOfDay.TotalSeconds == 0.0 && start < end;
		}

		internal static object[][] FetchRowsFromQueryResult(QueryResult queryResult, int rowCount)
		{
			if (queryResult == null)
			{
				throw new ArgumentNullException("queryResult");
			}
			object[][] array = new object[0][];
			object[][] rows;
			do
			{
				rows = queryResult.GetRows(rowCount - array.Length);
				if (rows.Length > 0)
				{
					if (array.Length == 0)
					{
						array = rows;
					}
					else
					{
						object[][] array2 = new object[array.Length + rows.Length][];
						array.CopyTo(array2, 0);
						rows.CopyTo(array2, array.Length);
						array = array2;
					}
				}
			}
			while (rows.Length > 0 && array.Length < rowCount);
			return array;
		}

		internal static Dictionary<PropertyDefinition, int> GetPropertyToIndexMap(PropertyDefinition[] properties)
		{
			Dictionary<PropertyDefinition, int> dictionary = new Dictionary<PropertyDefinition, int>(properties.Length);
			for (int i = 0; i < properties.Length; i++)
			{
				dictionary[properties[i]] = i;
			}
			return dictionary;
		}

		internal static void CheckAndThrowForRequiredProperty(Dictionary<PropertyDefinition, int> propertyMap, params PropertyDefinition[] requiredProperties)
		{
			foreach (PropertyDefinition propertyDefinition in requiredProperties)
			{
				if (!propertyMap.ContainsKey(propertyDefinition))
				{
					throw new InvalidOperationException("Cannot find required property " + propertyDefinition.GetType().ToString());
				}
			}
		}

		internal static string GetRandomNameForTempFilteredView(UserContext userContext)
		{
			return userContext.Key.MailboxUniqueKey + Guid.NewGuid().ToString();
		}

		internal static QueryFilter GetObjectClassTypeFilter(bool isFolder, bool exclusive, params string[] objectTypes)
		{
			PropertyDefinition property = isFolder ? StoreObjectSchema.ContainerClass : StoreObjectSchema.ItemClass;
			List<QueryFilter> list = new List<QueryFilter>(2 * objectTypes.Length);
			foreach (string text in objectTypes)
			{
				if (text != null)
				{
					list.Add(new TextFilter(property, text, MatchOptions.FullString, MatchFlags.IgnoreCase));
					list.Add(new TextFilter(property, text + ".", MatchOptions.Prefix, MatchFlags.IgnoreCase));
				}
			}
			if (list.Count == 0)
			{
				return null;
			}
			QueryFilter queryFilter = new OrFilter(list.ToArray());
			if (!exclusive)
			{
				return queryFilter;
			}
			return new NotFilter(queryFilter);
		}

		internal static QueryFilter GetObjectClassTypeFilter(bool isFolder, params string[] objectTypes)
		{
			return Utilities.GetObjectClassTypeFilter(isFolder, false, objectTypes);
		}

		internal static int CompareByteArrays(byte[] array1, byte[] array2)
		{
			if (array1 == array2)
			{
				return 0;
			}
			if (array1 == null)
			{
				return -1;
			}
			if (array2 == null)
			{
				return 1;
			}
			int num = 0;
			int num2 = 0;
			while (num == 0 && num2 < array1.Length && num2 < array2.Length)
			{
				num = array1[num2].CompareTo(array2[num2]);
				num2++;
			}
			if (num == 0)
			{
				num = array1.Length.CompareTo(array2.Length);
			}
			return num;
		}

		internal static bool IsOutlookSearchFolder(Folder folder)
		{
			if (folder == null)
			{
				throw new ArgumentNullException("folder");
			}
			object obj = folder.TryGetProperty(FolderSchema.IsOutlookSearchFolder);
			return obj != null && !(obj is PropertyError) && (bool)obj;
		}

		internal static bool IsMobileRoutingType(string routingType)
		{
			return !string.IsNullOrEmpty(routingType) && string.Equals(routingType, "MOBILE", StringComparison.OrdinalIgnoreCase);
		}

		internal static string NormalizePhoneNumber(string input)
		{
			E164Number e164Number;
			if (!E164Number.TryParse(input, out e164Number))
			{
				return null;
			}
			return e164Number.Number;
		}

		internal static string RedirectionUrl(OwaContext owaContext)
		{
			Uri uri;
			if (owaContext.IsExplicitLogon)
			{
				uri = Utilities.AppendSmtpAddressToUrl(owaContext.SecondCasUri.Uri, owaContext.HttpContext.Request.Headers["X-OWA-ExplicitLogonUser"]);
			}
			else
			{
				uri = owaContext.SecondCasUri.Uri;
			}
			return uri.ToString();
		}

		internal static bool IsEcpUrl(string urlString)
		{
			if (string.IsNullOrEmpty(urlString))
			{
				return false;
			}
			if (urlString.StartsWith(Utilities.EcpVdir, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			Uri uri = Utilities.TryParseUri(urlString);
			return uri != null && uri.AbsolutePath.StartsWith(Utilities.EcpVdir, StringComparison.OrdinalIgnoreCase);
		}

		internal static bool IsEacUrl(string urlString)
		{
			if (!Utilities.IsEcpUrl(urlString))
			{
				return false;
			}
			int num = urlString.IndexOf('?');
			if (num > 0)
			{
				string[] source = urlString.Substring(num + 1).Split(new char[]
				{
					'&'
				});
				return !source.Contains("rfr=owa") && !source.Contains("rfr=olk");
			}
			return true;
		}

		internal static bool IsSafeUrl(string urlString)
		{
			if (string.IsNullOrEmpty(urlString))
			{
				return false;
			}
			Uri uri;
			if (null == (uri = Utilities.TryParseUri(urlString)))
			{
				return false;
			}
			string scheme = uri.Scheme;
			return !string.IsNullOrEmpty(scheme) && Uri.CheckSchemeName(scheme) && TextConvertersInternalHelpers.IsUrlSchemaSafe(scheme);
		}

		internal static bool TryDecodeImceaAddress(string imceaAddress, ref string type, ref string address)
		{
			ProxyAddress proxyAddress;
			if (!SmtpProxyAddress.TryDeencapsulate(imceaAddress, out proxyAddress))
			{
				return false;
			}
			type = proxyAddress.PrefixString;
			address = proxyAddress.AddressString;
			return true;
		}

		internal static SanitizedHtmlString GetAlternateBodyForIrm(UserContext userContext, Microsoft.Exchange.Data.Storage.BodyFormat bodyFormat, RightsManagedMessageDecryptionStatus decryptionStatus, bool isProtectedVoicemail)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			SanitizingStringBuilder<OwaHtml> sanitizingStringBuilder = new SanitizingStringBuilder<OwaHtml>();
			if (bodyFormat == Microsoft.Exchange.Data.Storage.BodyFormat.TextHtml)
			{
				sanitizingStringBuilder.AppendFormat(CultureInfo.InvariantCulture, "<font face=\"{0}\" size=\"2\">", new object[]
				{
					Utilities.GetDefaultFontName()
				});
				if (decryptionStatus.FailureCode != RightsManagementFailureCode.MissingLicense)
				{
					StringBuilder stringBuilder = new StringBuilder();
					userContext.RenderThemeImage(stringBuilder, ThemeFileId.Error, null, new object[0]);
					sanitizingStringBuilder.AppendFormat(stringBuilder.ToString(), new object[0]);
					sanitizingStringBuilder.Append("&nbsp;");
				}
			}
			RightsManagementFailureCode failureCode = decryptionStatus.FailureCode;
			if (failureCode > RightsManagementFailureCode.PreLicenseAcquisitionFailed)
			{
				switch (failureCode)
				{
				case RightsManagementFailureCode.FailedToExtractTargetUriFromMex:
				case RightsManagementFailureCode.FailedToDownloadMexData:
					sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(1314141112));
					goto IL_448;
				case RightsManagementFailureCode.GetServerInfoFailed:
					goto IL_2BD;
				case RightsManagementFailureCode.InternalLicensingDisabled:
					break;
				case RightsManagementFailureCode.ExternalLicensingDisabled:
					sanitizingStringBuilder.AppendFormat(LocalizedStrings.GetNonEncoded(1397740097), new object[]
					{
						Utilities.GetOfficeDownloadAnchor(bodyFormat, userContext.UserCulture)
					});
					goto IL_448;
				default:
					switch (failureCode)
					{
					case RightsManagementFailureCode.ServerRightNotGranted:
						sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(784482022));
						goto IL_448;
					case RightsManagementFailureCode.InvalidLicensee:
						goto IL_1E8;
					case RightsManagementFailureCode.FeatureDisabled:
						break;
					case RightsManagementFailureCode.NotSupported:
						sanitizingStringBuilder.AppendFormat(isProtectedVoicemail ? LocalizedStrings.GetNonEncoded(106943791) : LocalizedStrings.GetNonEncoded(1049269714), new object[]
						{
							Utilities.GetOfficeDownloadAnchor(bodyFormat, userContext.UserCulture)
						});
						goto IL_448;
					case RightsManagementFailureCode.CorruptData:
						sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(684230472));
						goto IL_448;
					case RightsManagementFailureCode.MissingLicense:
					{
						MissingRightsManagementLicenseException ex = (MissingRightsManagementLicenseException)decryptionStatus.Exception;
						if (bodyFormat == Microsoft.Exchange.Data.Storage.BodyFormat.TextHtml)
						{
							sanitizingStringBuilder.AppendFormat("<div id=\"divIrmReqSpinner\" sReqCorrelator=\"{0}\" style=\"text-align:center;\">", new object[]
							{
								ex.RequestCorrelator
							});
							StringBuilder stringBuilder2 = new StringBuilder();
							userContext.RenderThemeImage(stringBuilder2, ThemeFileId.ProgressSmall, "prg", new object[0]);
							sanitizingStringBuilder.AppendFormat(stringBuilder2.ToString(), new object[0]);
							sanitizingStringBuilder.Append("&nbsp;");
						}
						sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(-695375226));
						if (bodyFormat == Microsoft.Exchange.Data.Storage.BodyFormat.TextHtml)
						{
							sanitizingStringBuilder.Append("</div>");
							goto IL_448;
						}
						goto IL_448;
					}
					default:
						if (failureCode != RightsManagementFailureCode.Success)
						{
							goto IL_2BD;
						}
						goto IL_448;
					}
					break;
				}
				sanitizingStringBuilder.AppendFormat(isProtectedVoicemail ? LocalizedStrings.GetNonEncoded(106943791) : LocalizedStrings.GetNonEncoded(1049269714), new object[]
				{
					Utilities.GetOfficeDownloadAnchor(bodyFormat, userContext.UserCulture)
				});
				goto IL_448;
			}
			if (failureCode == RightsManagementFailureCode.UserRightNotGranted)
			{
				sanitizingStringBuilder.AppendFormat(LocalizedStrings.GetNonEncoded(-1796455575), new object[]
				{
					Utilities.GetOfficeDownloadAnchor(bodyFormat, userContext.UserCulture)
				});
				goto IL_448;
			}
			if (failureCode != RightsManagementFailureCode.PreLicenseAcquisitionFailed)
			{
				goto IL_2BD;
			}
			IL_1E8:
			sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(-1489754529));
			goto IL_448;
			IL_2BD:
			sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(360598592));
			Exception exception = decryptionStatus.Exception;
			if (Globals.ShowDebugInformation && bodyFormat == Microsoft.Exchange.Data.Storage.BodyFormat.TextHtml && exception != null && exception.InnerException != null)
			{
				sanitizingStringBuilder.AppendFormat("<hr><div onclick=\"document.getElementById('divDtls').style.display='';this.style.display='none';\" style=\"cursor: pointer; color: #3165cd;\">", new object[0]);
				StringBuilder stringBuilder3 = new StringBuilder();
				userContext.RenderThemeImage(stringBuilder3, ThemeFileId.Expand, null, new object[0]);
				sanitizingStringBuilder.AppendFormat(stringBuilder3.ToString(), new object[0]);
				sanitizingStringBuilder.AppendFormat("&nbsp;{0}</div><br><div id=\"divDtls\" style='display:none'>", new object[]
				{
					LocalizedStrings.GetNonEncoded(-610047827)
				});
				string text = string.Empty;
				RightsManagementFailureCode failureCode2 = decryptionStatus.FailureCode;
				Exception innerException = exception.InnerException;
				if (innerException is RightsManagementException)
				{
					RightsManagementException ex2 = (RightsManagementException)innerException;
					text = ex2.RmsUrl;
				}
				int num = 0;
				while (num < 10 && innerException.InnerException != null)
				{
					innerException = innerException.InnerException;
					num++;
				}
				sanitizingStringBuilder.AppendFormat(LocalizedStrings.GetHtmlEncoded(1633606253), new object[]
				{
					innerException.Message
				});
				if (!string.IsNullOrEmpty(text))
				{
					sanitizingStringBuilder.Append("<br>");
					sanitizingStringBuilder.AppendFormat(LocalizedStrings.GetHtmlEncoded(2115316283), new object[]
					{
						text
					});
				}
				if (failureCode2 != RightsManagementFailureCode.Success)
				{
					sanitizingStringBuilder.Append("<br>");
					sanitizingStringBuilder.AppendFormat(LocalizedStrings.GetHtmlEncoded(970140031), new object[]
					{
						failureCode2
					});
				}
				sanitizingStringBuilder.Append("</div>");
			}
			IL_448:
			if (bodyFormat == Microsoft.Exchange.Data.Storage.BodyFormat.TextHtml)
			{
				sanitizingStringBuilder.Append("</font>");
			}
			if (decryptionStatus.Failed)
			{
				OwaContext.Current.HttpContext.Response.AppendHeader("X-OWA-DoNotCache", "1");
			}
			return sanitizingStringBuilder.ToSanitizedString<SanitizedHtmlString>();
		}

		internal static bool IsIrmRestrictedAndNotDecrypted(Item item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			RightsManagedMessageItem rightsManagedMessageItem = item as RightsManagedMessageItem;
			return rightsManagedMessageItem != null && rightsManagedMessageItem.IsRestricted && !rightsManagedMessageItem.IsDecoded;
		}

		internal static bool IsIrmRestrictedAndDecrypted(Item item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			RightsManagedMessageItem rightsManagedMessageItem = item as RightsManagedMessageItem;
			return rightsManagedMessageItem != null && rightsManagedMessageItem.IsRestricted && rightsManagedMessageItem.IsDecoded;
		}

		internal static bool IsIrmDecrypted(Item item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			RightsManagedMessageItem rightsManagedMessageItem = item as RightsManagedMessageItem;
			return rightsManagedMessageItem != null && rightsManagedMessageItem.IsDecoded;
		}

		internal static bool IsIrmRestricted(Item item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			RightsManagedMessageItem rightsManagedMessageItem = item as RightsManagedMessageItem;
			return rightsManagedMessageItem != null && rightsManagedMessageItem.IsRestricted;
		}

		internal static bool IrmDecryptIfRestricted(Item item, UserContext userContext, bool acquireLicenses)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			RightsManagedMessageItem rightsManagedMessageItem = item as RightsManagedMessageItem;
			if (rightsManagedMessageItem != null && rightsManagedMessageItem.IsRestricted)
			{
				if (!rightsManagedMessageItem.IsDecoded)
				{
					rightsManagedMessageItem.Decode(Utilities.CreateOutboundConversionOptions(userContext), acquireLicenses);
				}
				return true;
			}
			return false;
		}

		internal static bool IrmDecryptIfRestricted(Item item, UserContext userContext)
		{
			return Utilities.IrmDecryptIfRestricted(item, userContext, false);
		}

		internal static void IrmRemoveRestriction(Item item, UserContext userContext)
		{
			if (userContext.IsIrmEnabled && Utilities.IsIrmRestricted(item))
			{
				RightsManagedMessageItem rightsManagedMessageItem = item as RightsManagedMessageItem;
				rightsManagedMessageItem.OpenAsReadWrite();
				rightsManagedMessageItem.Decode(Utilities.CreateOutboundConversionOptions(userContext), true);
				rightsManagedMessageItem.SetRestriction(null);
				ConflictResolutionResult conflictResolutionResult = rightsManagedMessageItem.Save(SaveMode.ResolveConflicts);
				if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
				{
					throw new OwaSaveConflictException(LocalizedStrings.GetNonEncoded(-482397486), conflictResolutionResult);
				}
				rightsManagedMessageItem.Load();
			}
		}

		internal static bool IrmDecryptForReplyForward(OwaContext owaContext, ref Item currentItem, ref Item currentParentItem, ref Microsoft.Exchange.Data.Storage.BodyFormat bodyType, out RightsManagedMessageDecryptionStatus decryptionStatus)
		{
			UserContext userContext = owaContext.UserContext;
			if (!Utilities.IsIrmRestricted(currentItem))
			{
				decryptionStatus = RightsManagedMessageDecryptionStatus.Success;
				return false;
			}
			RightsManagedMessageItem rightsManagedMessageItem = (RightsManagedMessageItem)currentItem;
			if (!rightsManagedMessageItem.CanDecode)
			{
				decryptionStatus = RightsManagedMessageDecryptionStatus.NotSupported;
				return false;
			}
			if (Utilities.IrmDecryptIfRestricted(currentItem, userContext, true) && !rightsManagedMessageItem.UsageRights.IsUsageRightGranted(ContentRight.Edit))
			{
				bodyType = ReplyForwardUtilities.GetReplyForwardBodyFormat(currentItem, userContext);
				currentItem.Dispose();
				currentItem = null;
				if (currentParentItem != null)
				{
					currentParentItem.Dispose();
					currentParentItem = null;
				}
				currentItem = ReplyForwardUtilities.GetItemForRequest(owaContext, out currentParentItem);
				decryptionStatus = RightsManagedMessageDecryptionStatus.Success;
				return true;
			}
			decryptionStatus = RightsManagedMessageDecryptionStatus.Success;
			return false;
		}

		internal static SanitizedHtmlString GetOfficeDownloadAnchor(Microsoft.Exchange.Data.Storage.BodyFormat bodyFormat, CultureInfo userCulture)
		{
			if (bodyFormat == Microsoft.Exchange.Data.Storage.BodyFormat.TextHtml)
			{
				SanitizedHtmlString sanitizedHtmlString = SanitizedHtmlString.Format("<a href=\"{0}\" target=\"_blank\">{1}</a>", new object[]
				{
					LocalizedStrings.GetNonEncoded(1124412272),
					LocalizedStrings.GetNonEncoded(-1065109671)
				});
				return SanitizedHtmlString.Format(LocalizedStrings.GetHtmlEncoded(-539149404, userCulture), new object[]
				{
					sanitizedHtmlString
				});
			}
			return SanitizedHtmlString.Format(LocalizedStrings.GetHtmlEncoded(-1235477635, userCulture), new object[]
			{
				SanitizedHtmlString.GetNonEncoded(1124412272)
			});
		}

		internal static string RemoveHtmlComments(string htmlString)
		{
			return Utilities.htmlCommentStriper.Replace(htmlString, string.Empty);
		}

		private static bool ShouldIgnoreException(Exception exception)
		{
			COMException ex = exception as COMException;
			return ex != null && (ex.ErrorCode == -2147023901 || ex.ErrorCode == -2147024832 || ex.ErrorCode == -2147024895 || ex.ErrorCode == -2147024890 || ex.ErrorCode == -2147023667);
		}

		private static byte[][] MultiValuePropertyToByteArray(MultiValuedProperty<byte[]> property)
		{
			byte[][] array = null;
			if (property != null)
			{
				array = new byte[property.Count][];
				property.CopyTo(array, 0);
			}
			return array;
		}

		private static byte[][] AppendArray(byte[][] origin, byte[][] appendant)
		{
			byte[][] array = origin ?? Utilities.EmptyArrayOfByteArrays;
			byte[][] array2 = appendant ?? Utilities.EmptyArrayOfByteArrays;
			int num = array.Length;
			int num2 = array2.Length;
			int num3 = num + num2;
			byte[][] array3 = (num3 != 0) ? new byte[num3][] : Utilities.EmptyArrayOfByteArrays;
			Array.Copy(array, array3, num);
			Array.Copy(array2, 0, array3, num, num2);
			return array3;
		}

		internal static bool IsRequestCallbackForPhishing(HttpRequest request)
		{
			return !string.IsNullOrEmpty(Utilities.GetQueryStringParameter(request, "ph", false));
		}

		internal static bool IsRequestCallbackForWebBeacons(HttpRequest request)
		{
			return !string.IsNullOrEmpty(Utilities.GetQueryStringParameter(request, "cb", false));
		}

		internal static void PutOwaSubPageIntoPlaceHolder(PlaceHolder placeHolder, string id, OwaSubPage owaSubPage, QueryStringParameters queryStringParameters, string extraAttribute, bool isHidden)
		{
			if (placeHolder == null)
			{
				throw new ArgumentNullException("placeHolder");
			}
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			if (owaSubPage == null)
			{
				throw new ArgumentNullException("owaSubPage");
			}
			if (queryStringParameters == null)
			{
				throw new ArgumentNullException("queryStringParameters");
			}
			owaSubPage.QueryStringParameters = queryStringParameters;
			placeHolder.Controls.Add(owaSubPage);
			StringBuilder stringBuilder = new StringBuilder(128);
			stringBuilder.Append("<div id=\"");
			stringBuilder.Append(id);
			stringBuilder.Append("\" url=\"");
			Utilities.HtmlEncode(queryStringParameters.QueryString, stringBuilder);
			stringBuilder.Append("\" _PageType=\"");
			Utilities.HtmlEncode(owaSubPage.PageType, stringBuilder);
			stringBuilder.Append("\"");
			if (!string.IsNullOrEmpty(extraAttribute))
			{
				stringBuilder.Append(" ");
				stringBuilder.Append(extraAttribute);
			}
			if (isHidden)
			{
				stringBuilder.Append(" style=\"display:none\"");
			}
			stringBuilder.Append(">");
			owaSubPage.RenderExternalScriptFiles(stringBuilder);
			placeHolder.Controls.AddAt(0, new LiteralControl(stringBuilder.ToString()));
			placeHolder.Controls.Add(new LiteralControl("</div>"));
		}

		internal static bool HasArchive(UserContext userContext)
		{
			return userContext.ExchangePrincipal.GetArchiveMailbox() != null;
		}

		internal static ulong SetSegmentationFlags(int segmentationBits1, int segmentationBits2)
		{
			return (ulong)segmentationBits1 + (ulong)((ulong)((long)segmentationBits2) << 32);
		}

		internal static uint[] GetSegmentationBitsForJavascript(UserContext userContext)
		{
			uint[] array = new uint[2];
			ulong num = userContext.SegmentationFlags;
			if (userContext.RestrictedCapabilitiesFlags != 0UL)
			{
				num &= userContext.RestrictedCapabilitiesFlags;
			}
			array[0] = (uint)num;
			array[1] = (uint)(num >> 32);
			return array;
		}

		internal static int GetTimeZoneOffset(UserContext userContext)
		{
			int num = (int)DateTimeUtilities.GetLocalTime().Bias.TotalMinutes;
			if (num != userContext.RemindersTimeZoneOffset)
			{
				userContext.RemindersTimeZoneOffset = num;
			}
			return num;
		}

		internal static StreamWriter CreateStreamWriter(Stream stream)
		{
			UTF8Encoding encoding = new UTF8Encoding(false, false);
			return new StreamWriter(stream, encoding);
		}

		internal static string GetStringHash(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return string.Empty;
			}
			string result;
			using (SHA256Cng sha256Cng = new SHA256Cng())
			{
				byte[] bytes = Encoding.UTF8.GetBytes(text.ToLowerInvariant());
				result = Convert.ToBase64String(sha256Cng.ComputeHash(bytes));
			}
			return result;
		}

		private static void RegisterMailboxException(UserContext owaUserContext, Exception exception)
		{
			string serverFqdn = string.Empty;
			if (!Globals.StoreTransientExceptionEventLogEnabled)
			{
				return;
			}
			try
			{
				try
				{
					if (owaUserContext != null && owaUserContext.ExchangePrincipal != null)
					{
						serverFqdn = owaUserContext.ExchangePrincipal.MailboxInfo.Location.ServerFqdn;
					}
				}
				catch (StorageTransientException)
				{
				}
				catch (StoragePermanentException)
				{
				}
				finally
				{
					string[] array;
					if (Utilities.StoreConnectionTransientManager.RegisterException(exception, serverFqdn, out array) && array != null)
					{
						int num = 1;
						foreach (string text in array)
						{
							OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_StorageTransientExceptionWarning, string.Empty, new object[]
							{
								num,
								array.Length,
								text
							});
							num++;
						}
					}
				}
			}
			catch (Exception ex)
			{
				ExTraceGlobals.CoreTracer.TraceError<string>(0L, "Exception happened while attempting to log information about mailbox failures. Exception: {0}", ex.ToString());
				ExWatson.SendReport(ex, ReportOptions.None, null);
			}
		}

		private static bool IsDiskFullException(Exception e)
		{
			return Marshal.GetHRForException(e) == -2147024784;
		}

		private static bool IsUserPrincipalName(string logonName)
		{
			int num = logonName.IndexOf('@');
			if (num > 0)
			{
				int num2 = logonName.LastIndexOf('@');
				if (num2 < logonName.Length - 1 && num == num2)
				{
					return true;
				}
			}
			return false;
		}

		private static bool IsDomainSlashUser(string logonName)
		{
			int num = logonName.IndexOf('\\');
			if (num > 0)
			{
				int num2 = logonName.LastIndexOf('\\');
				if (num2 < logonName.Length - 1 && num == num2)
				{
					return true;
				}
			}
			return false;
		}

		internal static string GetCanary15CookieValue()
		{
			if (OwaContext.Current != null)
			{
				Canary15Cookie canary15Cookie = Canary15Cookie.TryCreateFromHttpContext(OwaContext.Current.HttpContext, OwaContext.Current.LogonIdentity.UniqueId, Canary15Profile.Owa);
				if (canary15Cookie != null)
				{
					return canary15Cookie.Value;
				}
			}
			return null;
		}

		private const int GuidLength = 32;

		public const string SilverlightMinimumRuntimeVersion = "2.0.31005.0";

		public const string UrlQueryParameter = "URL";

		public const string NoDocumentsLinkClassificationInRedir = "NoDocLnkCls";

		public const string EmailUrlParameter = "email";

		public const string MobileRoutingType = "MOBILE";

		internal const string MailboxCrossSiteFailoverHeader = "mailboxCrossSiteFailover";

		internal const string ClientStringsFilename = "clientstrings.aspx";

		private const string SmallIconsFilename = "smallicons.aspx";

		private const int ErrorAborted = -2147023901;

		private const int ErrorNonExistentConnection = -2147023667;

		private const int ErrorNetworkNameNotAvailable = -2147024832;

		private const int ErrorInvalidHandle = -2147024890;

		private const int ErrorIncorrectFunction = -2147024895;

		private const string ExtensionGif = ".gif";

		private const string ExtensionJpg = ".jpg";

		private const string ExtensionCss = ".css";

		private const string ExtensionXap = ".xap";

		private const string ExtensionJs = ".js";

		private const string ExtensionWav = ".wav";

		private const string ExtensionMp3 = ".mp3";

		private const string ExtensionHtm = ".htm";

		private const string ExtensionHtml = ".html";

		private const string ExtensionPng = ".png";

		private const string ExtensionMSI = ".msi";

		private const string ExtensionICO = ".ico";

		private const string ExtensionManifest = ".manifest";

		private const string ExtensionTTF = ".ttf";

		private const string ExtensionEOT = ".eot";

		private const string ExtensionWOFF = ".woff";

		private const string ExtensionSVG = ".svg";

		private const string ExtensionChromeWebApp = ".crx";

		private const int NetUserChangePasswordSuccess = 0;

		private const int NetUserChangePasswordAccessDenied = 5;

		private const int NetUserChangePasswordInvalidOldPassword = 86;

		private const int NetUserChangePasswordDoesNotMeetPolicyRequirement = 2245;

		private const string PremiumScriptFolder = "/scripts/premium/";

		private const string UnknownExceptionPrefix = "UE:";

		public const string CanaryFormParameter = "hidcanary";

		public const string CanaryQueryOehParameter = "canary";

		public const string Canary15Name = "X-OWA-CANARY";

		public static readonly TimeSpan DefaultMinAvailabilityThreshold = TimeSpan.FromSeconds(15.0);

		private static readonly LatencyDetectionContextFactory OwaAvailabilityContextFactory = LatencyDetectionContextFactory.CreateFactory("OWA Availability Query", Utilities.DefaultMinAvailabilityThreshold, Utilities.DefaultMinAvailabilityThreshold);

		private static readonly string comparingStringForEncodingDetecting = Encoding.UTF8.GetString(new byte[]
		{
			228,
			189,
			160,
			229,
			165,
			189,
			65
		});

		private static readonly string VirtualDirectoryNameWithLeadingSlash = HttpRuntime.AppDomainAppVirtualPath;

		private static readonly string VirtualDirectoryNameWithLeadingAndTrailingSlash = HttpRuntime.AppDomainAppVirtualPath + "/";

		private static readonly byte[][] EmptyArrayOfByteArrays = new byte[0][];

		private static readonly string[] Owa15ParameterNames = new string[]
		{
			"owa15",
			"animation",
			"appcache",
			"cmd",
			"diag",
			"exsvurl",
			"layout",
			"mergerowsvalidation",
			"modurl",
			"offline",
			"prefetch",
			"realm",
			"server",
			"sessiontimeout",
			"sync",
			"tracelevel",
			"viewmodel",
			"wa",
			"theme"
		};

		private static readonly Dictionary<Type, string> ExceptionCodeMap = new Dictionary<Type, string>
		{
			{
				typeof(OwaRenderingEmbeddedReadingPaneException),
				"E001"
			},
			{
				typeof(OwaInvalidRequestException),
				"E002"
			},
			{
				typeof(OwaInvalidIdFormatException),
				"E003"
			},
			{
				typeof(OwaSegmentationException),
				"E004"
			},
			{
				typeof(OwaForbiddenRequestException),
				"E005"
			},
			{
				typeof(OwaDelegatorMailboxFailoverException),
				"E006"
			},
			{
				typeof(WrongCASServerBecauseOfOutOfDateDNSCacheException),
				"E007"
			},
			{
				typeof(OwaURLIsOutOfDateException),
				"E008"
			},
			{
				typeof(OwaEventHandlerException),
				"E009"
			},
			{
				typeof(OwaNotSupportedException),
				"E010"
			},
			{
				typeof(OwaClientNotSupportedException),
				"E011"
			},
			{
				typeof(OwaExistentNotificationPipeException),
				"E012"
			},
			{
				typeof(OwaNotificationPipeException),
				"E013"
			},
			{
				typeof(OwaOperationNotSupportedException),
				"E014"
			},
			{
				typeof(OwaADObjectNotFoundException),
				"E015"
			},
			{
				typeof(OwaInvalidCanary14Exception),
				"E016"
			},
			{
				typeof(OwaCanaryException),
				"E016"
			},
			{
				typeof(OwaLockTimeoutException),
				"E017"
			},
			{
				typeof(OwaLostContextException),
				"E018"
			},
			{
				typeof(OwaBodyConversionFailedException),
				"E019"
			},
			{
				typeof(OwaArchiveInTransitException),
				"E020"
			},
			{
				typeof(OwaArchiveNotAvailableException),
				"E021"
			},
			{
				typeof(OwaSaveConflictException),
				"E022"
			},
			{
				typeof(OwaAccessDeniedException),
				"E023"
			},
			{
				typeof(OwaInstantMessageEventHandlerTransientException),
				"E024"
			},
			{
				typeof(OwaUserNotIMEnabledException),
				"E025"
			},
			{
				typeof(OwaIMOperationNotAllowedToSelf),
				"E026"
			},
			{
				typeof(OwaInvalidOperationException),
				"E027"
			},
			{
				typeof(OwaChangePasswordTransientException),
				"E028"
			},
			{
				typeof(OwaSpellCheckerException),
				"E029"
			},
			{
				typeof(OwaProxyException),
				"E030"
			},
			{
				typeof(OwaExplicitLogonException),
				"E031"
			},
			{
				typeof(OwaInvalidWebPartRequestException),
				"E032"
			},
			{
				typeof(OwaNoReplicaOfCurrentServerVersionException),
				"E033"
			},
			{
				typeof(OwaNoReplicaException),
				"E034"
			},
			{
				typeof(TranscodingServerBusyException),
				"E035"
			},
			{
				typeof(TranscodingUnconvertibleFileException),
				"E036"
			},
			{
				typeof(TranscodingFatalFaultException),
				"E037"
			},
			{
				typeof(TranscodingOverMaximumFileSizeException),
				"E038"
			},
			{
				typeof(TranscodingTimeoutException),
				"E039"
			},
			{
				typeof(TranscodingErrorFileException),
				"E040"
			},
			{
				typeof(OwaInvalidConfigurationException),
				"E041"
			},
			{
				typeof(OwaAsyncOperationException),
				"E042"
			},
			{
				typeof(OwaAsyncRequestTimeoutException),
				"E043"
			},
			{
				typeof(OwaNeedsSMimeControlToEditDraftException),
				"E044"
			},
			{
				typeof(OwaCannotEditIrmDraftException),
				"E045"
			},
			{
				typeof(OwaBrowserUpdateRequiredException),
				"E046"
			},
			{
				typeof(OwaSharedFromOlderVersionException),
				"E047"
			},
			{
				typeof(OwaRespondOlderVersionMeetingException),
				"E048"
			},
			{
				typeof(OwaCreateClientSecurityContextFailedException),
				"E049"
			},
			{
				typeof(OwaUnsupportedConversationItemException),
				"E050"
			},
			{
				typeof(OwaNotificationPipeWriteException),
				"E051"
			},
			{
				typeof(OwaDisabledException),
				"E052"
			},
			{
				typeof(OwaLightDisabledException),
				"E053"
			},
			{
				typeof(MailboxInSiteFailoverException),
				"E101"
			},
			{
				typeof(MailboxCrossSiteFailoverException),
				"E102"
			},
			{
				typeof(WrongServerException),
				"E103"
			},
			{
				typeof(ObjectNotFoundException),
				"E104"
			},
			{
				typeof(ObjectValidationException),
				"E105"
			},
			{
				typeof(CorruptDataException),
				"E106"
			},
			{
				typeof(PropertyValidationException),
				"E107"
			},
			{
				typeof(InvalidSharingMessageException),
				"E108"
			},
			{
				typeof(InvalidSharingDataException),
				"E109"
			},
			{
				typeof(InvalidExternalSharingInitiatorException),
				"E110"
			},
			{
				typeof(VirusDetectedException),
				"E111"
			},
			{
				typeof(VirusScanInProgressException),
				"E112"
			},
			{
				typeof(VirusMessageDeletedException),
				"E113"
			},
			{
				typeof(NoReplicaException),
				"E114"
			},
			{
				typeof(StorageTransientException),
				"E116"
			},
			{
				typeof(RulesTooBigException),
				"E117"
			},
			{
				typeof(DuplicateActionException),
				"E118"
			},
			{
				typeof(ObjectExistedException),
				"E119"
			},
			{
				typeof(MailboxInTransitException),
				"E120"
			},
			{
				typeof(ConnectionFailedPermanentException),
				"E121"
			},
			{
				typeof(ConnectionFailedTransientException),
				"E122"
			},
			{
				typeof(MailboxOfflineException),
				"E123"
			},
			{
				typeof(SendAsDeniedException),
				"E124"
			},
			{
				typeof(RecurrenceFormatException),
				"E125"
			},
			{
				typeof(OccurrenceTimeSpanTooBigException),
				"E126"
			},
			{
				typeof(QuotaExceededException),
				"E127"
			},
			{
				typeof(MessageTooBigException),
				"E128"
			},
			{
				typeof(SubmissionQuotaExceededException),
				"E129"
			},
			{
				typeof(MessageSubmissionExceededException),
				"E130"
			},
			{
				typeof(AttachmentExceededException),
				"E131"
			},
			{
				typeof(ResourcesException),
				"E132"
			},
			{
				typeof(NoMoreConnectionsException),
				"E133"
			},
			{
				typeof(AccountDisabledException),
				"E134"
			},
			{
				typeof(AccessDeniedException),
				"E135"
			},
			{
				typeof(StoragePermanentException),
				"E136"
			},
			{
				typeof(ServerNotFoundException),
				"E137"
			},
			{
				typeof(SaveConflictException),
				"E138"
			},
			{
				typeof(FolderSaveException),
				"E139"
			},
			{
				typeof(OccurrenceCrossingBoundaryException),
				"E140"
			},
			{
				typeof(ParserException),
				"E141"
			},
			{
				typeof(RecurrenceEndDateTooBigException),
				"E142"
			},
			{
				typeof(RecurrenceStartDateTooSmallException),
				"E143"
			},
			{
				typeof(RecurrenceHasNoOccurrenceException),
				"E144"
			},
			{
				typeof(PropertyErrorException),
				"E145"
			},
			{
				typeof(ConversionFailedException),
				"E146"
			},
			{
				typeof(RightsManagementPermanentException),
				"E147"
			},
			{
				typeof(DataValidationException),
				"E148"
			},
			{
				typeof(InvalidObjectOperationException),
				"E149"
			},
			{
				typeof(TransientException),
				"E150"
			},
			{
				typeof(CorruptDataException),
				"E201"
			},
			{
				typeof(AccessDeniedException),
				"E202"
			},
			{
				typeof(ConnectionException),
				"E203"
			},
			{
				typeof(PropertyErrorException),
				"E204"
			},
			{
				typeof(PathTooLongException),
				"E205"
			},
			{
				typeof(UnknownErrorException),
				"E206"
			},
			{
				typeof(DocumentLibraryException),
				"E207"
			},
			{
				typeof(ObjectNotFoundException),
				"E208"
			},
			{
				typeof(ADTransientException),
				"E301"
			},
			{
				typeof(ADOperationException),
				"E302"
			},
			{
				typeof(OverBudgetException),
				"E303"
			},
			{
				typeof(ResourceUnhealthyException),
				"E304"
			},
			{
				typeof(COMException),
				"E401"
			},
			{
				typeof(ThreadAbortException),
				"E402"
			},
			{
				typeof(InvalidOperationException),
				"E403"
			},
			{
				typeof(NullReferenceException),
				"E404"
			},
			{
				typeof(OutOfMemoryException),
				"E405"
			},
			{
				typeof(ArgumentException),
				"E406"
			},
			{
				typeof(IndexOutOfRangeException),
				"E407"
			},
			{
				typeof(ArgumentOutOfRangeException),
				"E408"
			},
			{
				typeof(HttpException),
				"E409"
			},
			{
				typeof(ArgumentNullException),
				"E410"
			},
			{
				typeof(InstantMessagingException),
				"E501"
			}
		};

		internal static string PremiumScriptPath = Globals.ApplicationVersion + "/scripts/premium/";

		internal static readonly string EcpVdir = "/ecp/";

		private static OwaExceptionEventManager storeConnectionTransientManager;

		private static Dictionary<ThemeFileId, Strings.IDs> altTable = new Dictionary<ThemeFileId, Strings.IDs>
		{
			{
				ThemeFileId.FirstPage,
				-946066775
			},
			{
				ThemeFileId.PreviousPage,
				-1907861992
			},
			{
				ThemeFileId.NextPage,
				1548165396
			},
			{
				ThemeFileId.LastPage,
				-991618511
			},
			{
				ThemeFileId.ContactDL,
				-1878983012
			},
			{
				ThemeFileId.ResourceAttendee,
				191819257
			},
			{
				ThemeFileId.RequiredAttendee,
				749312262
			},
			{
				ThemeFileId.OptionalAttendee,
				107204003
			},
			{
				ThemeFileId.ImportanceLow,
				-691921988
			},
			{
				ThemeFileId.ImportanceHigh,
				-1170704990
			},
			{
				ThemeFileId.Attachment2,
				-1498653219
			}
		};

		private static PropertyDefinition[] adFindByExchangeLegacyDnProperties = new PropertyDefinition[]
		{
			ADRecipientSchema.PrimarySmtpAddress,
			ADObjectSchema.Id,
			ADOrgPersonSchema.MobilePhone
		};

		private static Regex htmlCommentStriper = new Regex("<!--.*?-->", RegexOptions.Compiled | RegexOptions.Singleline);

		private static int queuedDelayedRestart;

		private static uint[] pow2minus1 = new uint[]
		{
			0U,
			1U,
			3U,
			7U,
			15U,
			31U,
			63U,
			127U,
			255U,
			511U,
			1023U,
			2047U,
			4095U,
			8191U,
			16383U,
			32767U,
			65535U,
			131071U,
			262143U,
			524287U,
			1048575U,
			2097151U,
			4194303U,
			8388607U,
			16777215U,
			33554431U,
			67108863U,
			134217727U,
			268435455U,
			536870911U,
			1073741823U,
			2147483647U,
			uint.MaxValue
		};

		private static PropertyDefinition[] distributionListMemberPropertyDefinitions = new PropertyDefinition[]
		{
			ADObjectSchema.Id,
			ADRecipientSchema.DisplayName,
			ADRecipientSchema.PhoneticDisplayName,
			ADRecipientSchema.RecipientType,
			ADRecipientSchema.Alias,
			ADRecipientSchema.LegacyExchangeDN
		};

		private static PropertyDefinition[] smimeDistributionListMemberPropertyDefinitions = new PropertyDefinition[]
		{
			ADObjectSchema.Id,
			ADRecipientSchema.DisplayName,
			ADRecipientSchema.PhoneticDisplayName,
			ADRecipientSchema.RecipientType,
			ADRecipientSchema.Alias,
			ADRecipientSchema.LegacyExchangeDN,
			ADRecipientSchema.Certificate,
			ADRecipientSchema.SMimeCertificate,
			ADRecipientSchema.EmailAddresses
		};

		private static string smimeVersion;

		public enum ChangePasswordResult
		{
			Success,
			InvalidCredentials,
			LockedOut,
			BadNewPassword,
			OtherError
		}
	}
}
