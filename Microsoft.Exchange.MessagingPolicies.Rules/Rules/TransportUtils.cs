using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.ContentTypes.Tnef;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.SecureMail;
using Microsoft.Exchange.Security.Cryptography;
using Microsoft.Exchange.TextProcessing;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class TransportUtils
	{
		public static void LoadRuleCollectionFromAd(string ruleCollectionName, IConfigurationSession session, out ADRuleStorageManager storedRules, out Exception e)
		{
			storedRules = null;
			e = null;
			try
			{
				storedRules = new ADRuleStorageManager(ruleCollectionName, session);
				storedRules.LoadRuleCollection();
			}
			catch (ArgumentNullException ex)
			{
				e = ex;
			}
			catch (ArgumentException ex2)
			{
				e = ex2;
			}
			catch (RulesValidationException ex3)
			{
				e = ex3;
			}
			catch (ParserException ex4)
			{
				e = ex4;
			}
			catch (DataSourceOperationException ex5)
			{
				e = ex5;
			}
			catch (RuleCollectionNotInAdException ex6)
			{
				e = ex6;
			}
			catch (ExchangeConfigurationException ex7)
			{
				e = ex7;
			}
		}

		public static void LoadRuleCollectionFromCacheData(string ruleCollectionName, List<TransportRuleData> rules, RuleHealthMonitor ruleLoadMonitor, out ADRuleStorageManager storedRules, out Exception e)
		{
			storedRules = null;
			e = null;
			try
			{
				storedRules = new ADRuleStorageManager(ruleCollectionName, rules);
				storedRules.ParseRuleCollection(ruleLoadMonitor);
			}
			catch (ArgumentNullException ex)
			{
				e = ex;
			}
			catch (ArgumentException ex2)
			{
				e = ex2;
			}
			catch (RulesValidationException ex3)
			{
				e = ex3;
			}
			catch (ParserException ex4)
			{
				e = ex4;
			}
			catch (DataSourceOperationException ex5)
			{
				e = ex5;
			}
			catch (RuleCollectionNotInAdException ex6)
			{
				e = ex6;
			}
			catch (ExchangeConfigurationException ex7)
			{
				e = ex7;
			}
		}

		public static IConfigurationSession CreateSession(OrganizationId orgId)
		{
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(orgId), 179, "CreateSession", "f:\\15.00.1497\\sources\\dev\\MessagingPolicies\\src\\rules\\TransportUtils.cs");
		}

		public static Dictionary<string, ADPropertyDefinition> GetDisclaimerMacroLookupTable()
		{
			if (TransportUtils.MacroNameLookupTable.Count == 0)
			{
				lock (TransportUtils.lockVar)
				{
					if (TransportUtils.MacroNameLookupTable.Count == 0)
					{
						TransportUtils.MacroNameLookupTable.Add("displayname", ADRecipientSchema.DisplayName);
						TransportUtils.MacroNameLookupTable.Add("firstname", ADOrgPersonSchema.FirstName);
						TransportUtils.MacroNameLookupTable.Add("initials", ADOrgPersonSchema.Initials);
						TransportUtils.MacroNameLookupTable.Add("lastname", ADOrgPersonSchema.LastName);
						TransportUtils.MacroNameLookupTable.Add("office", ADOrgPersonSchema.Office);
						TransportUtils.MacroNameLookupTable.Add("phone", ADOrgPersonSchema.Phone);
						TransportUtils.MacroNameLookupTable.Add("othertelephone", ADOrgPersonSchema.OtherTelephone);
						TransportUtils.MacroNameLookupTable.Add("windowsemailaddress", ADRecipientSchema.WindowsEmailAddress);
						TransportUtils.MacroNameLookupTable.Add("streetaddress", ADOrgPersonSchema.StreetAddress);
						TransportUtils.MacroNameLookupTable.Add("postofficebox", ADOrgPersonSchema.PostOfficeBox);
						TransportUtils.MacroNameLookupTable.Add("city", ADOrgPersonSchema.City);
						TransportUtils.MacroNameLookupTable.Add("stateorprovince", ADOrgPersonSchema.StateOrProvince);
						TransportUtils.MacroNameLookupTable.Add("postalcode", ADOrgPersonSchema.PostalCode);
						TransportUtils.MacroNameLookupTable.Add("countryorregion", ADOrgPersonSchema.C);
						TransportUtils.MacroNameLookupTable.Add("userprincipalname", ADUserSchema.UserPrincipalName);
						TransportUtils.MacroNameLookupTable.Add("homephone", ADOrgPersonSchema.HomePhone);
						TransportUtils.MacroNameLookupTable.Add("otherhomephone", ADOrgPersonSchema.OtherHomePhone);
						TransportUtils.MacroNameLookupTable.Add("pager", ADOrgPersonSchema.Pager);
						TransportUtils.MacroNameLookupTable.Add("mobilephone", ADOrgPersonSchema.MobilePhone);
						TransportUtils.MacroNameLookupTable.Add("fax", ADOrgPersonSchema.Fax);
						TransportUtils.MacroNameLookupTable.Add("otherfax", ADOrgPersonSchema.OtherFax);
						TransportUtils.MacroNameLookupTable.Add("notes", ADRecipientSchema.Notes);
						TransportUtils.MacroNameLookupTable.Add("title", ADOrgPersonSchema.Title);
						TransportUtils.MacroNameLookupTable.Add("department", ADOrgPersonSchema.Department);
						TransportUtils.MacroNameLookupTable.Add("company", ADOrgPersonSchema.Company);
						TransportUtils.MacroNameLookupTable.Add("manager", ADOrgPersonSchema.Manager);
						TransportUtils.MacroNameLookupTable.Add("customattribute1", ADRecipientSchema.CustomAttribute1);
						TransportUtils.MacroNameLookupTable.Add("customattribute2", ADRecipientSchema.CustomAttribute2);
						TransportUtils.MacroNameLookupTable.Add("customattribute3", ADRecipientSchema.CustomAttribute3);
						TransportUtils.MacroNameLookupTable.Add("customattribute4", ADRecipientSchema.CustomAttribute4);
						TransportUtils.MacroNameLookupTable.Add("customattribute5", ADRecipientSchema.CustomAttribute5);
						TransportUtils.MacroNameLookupTable.Add("customattribute6", ADRecipientSchema.CustomAttribute6);
						TransportUtils.MacroNameLookupTable.Add("customattribute7", ADRecipientSchema.CustomAttribute7);
						TransportUtils.MacroNameLookupTable.Add("customattribute8", ADRecipientSchema.CustomAttribute8);
						TransportUtils.MacroNameLookupTable.Add("customattribute9", ADRecipientSchema.CustomAttribute9);
						TransportUtils.MacroNameLookupTable.Add("customattribute10", ADRecipientSchema.CustomAttribute10);
						TransportUtils.MacroNameLookupTable.Add("customattribute11", ADRecipientSchema.CustomAttribute11);
						TransportUtils.MacroNameLookupTable.Add("customattribute12", ADRecipientSchema.CustomAttribute12);
						TransportUtils.MacroNameLookupTable.Add("customattribute13", ADRecipientSchema.CustomAttribute13);
						TransportUtils.MacroNameLookupTable.Add("customattribute14", ADRecipientSchema.CustomAttribute14);
						TransportUtils.MacroNameLookupTable.Add("customattribute15", ADRecipientSchema.CustomAttribute15);
						TransportUtils.MacroNameLookupTable.Add("phonenumber", ADOrgPersonSchema.Phone);
						TransportUtils.MacroNameLookupTable.Add("otherphonenumber", ADOrgPersonSchema.OtherTelephone);
						TransportUtils.MacroNameLookupTable.Add("email", ADRecipientSchema.WindowsEmailAddress);
						TransportUtils.MacroNameLookupTable.Add("street", ADOrgPersonSchema.StreetAddress);
						TransportUtils.MacroNameLookupTable.Add("pobox", ADOrgPersonSchema.PostOfficeBox);
						TransportUtils.MacroNameLookupTable.Add("state", ADOrgPersonSchema.StateOrProvince);
						TransportUtils.MacroNameLookupTable.Add("zipcode", ADOrgPersonSchema.PostalCode);
						TransportUtils.MacroNameLookupTable.Add("country", ADOrgPersonSchema.C);
						TransportUtils.MacroNameLookupTable.Add("userlogonname", ADUserSchema.UserPrincipalName);
						TransportUtils.MacroNameLookupTable.Add("homephonenumber", ADOrgPersonSchema.HomePhone);
						TransportUtils.MacroNameLookupTable.Add("otherhomephonenumber", ADOrgPersonSchema.OtherHomePhone);
						TransportUtils.MacroNameLookupTable.Add("pagernumber", ADOrgPersonSchema.Pager);
						TransportUtils.MacroNameLookupTable.Add("mobilenumber", ADOrgPersonSchema.MobilePhone);
						TransportUtils.MacroNameLookupTable.Add("faxnumber", ADOrgPersonSchema.Fax);
						TransportUtils.MacroNameLookupTable.Add("otherfaxnumber", ADOrgPersonSchema.OtherFax);
					}
				}
			}
			return TransportUtils.MacroNameLookupTable;
		}

		public static string GetMacroPropertyDefinition(SmtpProxyAddress userAddress, string field, ADRawEntry entry)
		{
			if (entry == null)
			{
				return string.Empty;
			}
			Dictionary<string, ADPropertyDefinition> disclaimerMacroLookupTable = TransportUtils.GetDisclaimerMacroLookupTable();
			string text = field.ToLower().Trim();
			ADPropertyDefinition property = null;
			if (!disclaimerMacroLookupTable.TryGetValue(text, out property))
			{
				return string.Empty;
			}
			if (string.Equals(text, "manager"))
			{
				ADObjectId adProperty = TransportUtils.GetAdProperty<ADObjectId>(entry, property);
				if (adProperty != null)
				{
					IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.RootOrgOrSingleTenantFromAcceptedDomainAutoDetect(((SmtpAddress)userAddress).Domain), 296, "GetMacroPropertyDefinition", "f:\\15.00.1497\\sources\\dev\\MessagingPolicies\\src\\rules\\TransportUtils.cs");
					ADRecipient adrecipient = tenantOrRootOrgRecipientSession.Read(adProperty);
					if (adrecipient != null)
					{
						return adrecipient.DisplayName;
					}
				}
				return string.Empty;
			}
			if (string.Equals(text, "postofficebox") || string.Equals(text, "othertelephone") || string.Equals(text, "otherfax") || string.Equals(text, "otherhomephone") || string.Equals(text, "pobox") || string.Equals(text, "otherphonenumber") || string.Equals(text, "otherfaxnumber") || string.Equals(text, "otherhomephonenumber"))
			{
				MultiValuedProperty<string> adProperty2 = TransportUtils.GetAdProperty<MultiValuedProperty<string>>(entry, property);
				if (adProperty2 == null)
				{
					return string.Empty;
				}
				string[] array = new string[adProperty2.Count];
				adProperty2.CopyTo(array, 0);
				return string.Join(";", array);
			}
			else
			{
				if (string.Equals(text, "windowsemailaddress") || string.Equals(text, "email"))
				{
					return TransportUtils.GetAdProperty<SmtpAddress>(entry, property).ToString();
				}
				return TransportUtils.GetAdProperty<string>(entry, property) ?? string.Empty;
			}
		}

		internal static T GetAdProperty<T>(ADRawEntry entry, ADPropertyDefinition property)
		{
			T result;
			try
			{
				result = (T)((object)entry[property]);
			}
			catch (ValueNotPresentException)
			{
				if (typeof(T) == typeof(string))
				{
					result = (T)((object)string.Empty);
				}
				else
				{
					result = default(T);
				}
			}
			return result;
		}

		internal static string CheckForInvalidMacroName(string disclaimerText)
		{
			Dictionary<string, ADPropertyDefinition> disclaimerMacroLookupTable = TransportUtils.GetDisclaimerMacroLookupTable();
			string[] array = disclaimerText.Split(new string[]
			{
				"%%"
			}, StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				if (i % 2 == 1 && !disclaimerMacroLookupTable.ContainsKey(array[i].ToLower().Trim()))
				{
					return array[i];
				}
			}
			return null;
		}

		public static bool IsHeaderValid(string headerName)
		{
			bool result;
			try
			{
				Header.Create(headerName);
				result = true;
			}
			catch (ArgumentException)
			{
				result = false;
			}
			return result;
		}

		public static bool IsOof(EmailMessage message)
		{
			string mapiMessageClass = message.MapiMessageClass;
			return mapiMessageClass.StartsWith("IPM.Note.Rules.OofTemplate.", StringComparison.OrdinalIgnoreCase) || mapiMessageClass.StartsWith("IPM.Note.Rules.ExternalOofTemplate.", StringComparison.OrdinalIgnoreCase);
		}

		public static bool IsAutoForward(EmailMessage message)
		{
			bool flag = false;
			return message.TryGetMapiProperty<bool>(TnefPropertyTag.AutoForwarded, out flag) && flag;
		}

		public static bool IsEncrypted(EmailMessage message)
		{
			return message.MessageSecurityType == MessageSecurityType.Encrypted;
		}

		public static bool IsCalendaring(EmailMessage message)
		{
			string mapiMessageClass = message.MapiMessageClass;
			return mapiMessageClass.StartsWith("IPM.Schedule.Meeting.", StringComparison.OrdinalIgnoreCase) || mapiMessageClass.Equals("IPM.Appointment", StringComparison.OrdinalIgnoreCase) || message.CalendarPart != null;
		}

		public static bool IsPermissionControlled(MailItem mailItem)
		{
			object obj = null;
			if (mailItem.Properties.TryGetValue("Microsoft.Exchange.RightsManagement.TransportDecrypted", out obj))
			{
				return true;
			}
			EmailMessage message = mailItem.Message;
			string mapiMessageClass = message.MapiMessageClass;
			if (mapiMessageClass.Equals("IPM.Note.rpmsg.Microsoft.Voicemail.UM", StringComparison.OrdinalIgnoreCase) || mapiMessageClass.Equals("IPM.Note.rpmsg.Microsoft.Voicemail.UM.CA", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			AsciiTextHeader asciiTextHeader = message.RootPart.Headers.FindFirst(HeaderId.ContentClass) as AsciiTextHeader;
			if (asciiTextHeader == null)
			{
				return false;
			}
			string value = asciiTextHeader.Value;
			return !string.IsNullOrEmpty(value) && (value.Equals("rpmsg.message", StringComparison.OrdinalIgnoreCase) || value.Equals("IPM.Note.rpmsg.Microsoft.Voicemail.UM.CA", StringComparison.OrdinalIgnoreCase) || value.Equals("IPM.Note.rpmsg.Microsoft.Voicemail.UM", StringComparison.OrdinalIgnoreCase));
		}

		public static bool IsSigned(EmailMessage message)
		{
			MessageSecurityType messageSecurityType = message.MessageSecurityType;
			return messageSecurityType == MessageSecurityType.OpaqueSigned || messageSecurityType == MessageSecurityType.ClearSigned;
		}

		public static bool IsApprovalRequest(EmailMessage message)
		{
			string mapiMessageClass = message.MapiMessageClass;
			return mapiMessageClass.StartsWith("IPM.Note.Microsoft.Approval", StringComparison.OrdinalIgnoreCase);
		}

		public static bool IsReadReceipt(EmailMessage message)
		{
			string mapiMessageClass = message.MapiMessageClass;
			return mapiMessageClass.StartsWith("REPORT", StringComparison.OrdinalIgnoreCase) && (mapiMessageClass.EndsWith("IPNRN", StringComparison.OrdinalIgnoreCase) || mapiMessageClass.EndsWith("IPNNRN", StringComparison.OrdinalIgnoreCase));
		}

		public static bool IsHeaderSettable(string headerName, string value)
		{
			if ("X-MS-Exchange-Inbox-Rules-Loop".Equals(headerName, StringComparison.OrdinalIgnoreCase))
			{
				ExTraceGlobals.TransportRulesEngineTracer.TraceDebug<string>(0L, "{0} header cannot be set through transport rules, skipping this action.", "X-MS-Exchange-Inbox-Rules-Loop");
				return false;
			}
			if ("X-MS-Exchange-Transport-Rules-Loop".Equals(headerName, StringComparison.OrdinalIgnoreCase))
			{
				ExTraceGlobals.TransportRulesEngineTracer.TraceDebug<string>(0L, "{0}-Loop header cannot be set through transport rules, skipping this action.", "X-MS-Exchange-Transport-Rules-Loop");
				return false;
			}
			if ("X-MS-Exchange-Moderation-Loop".Equals(headerName, StringComparison.OrdinalIgnoreCase))
			{
				ExTraceGlobals.TransportRulesEngineTracer.TraceDebug<string>(0L, "{0} header cannot be set through transport rules, skipping this action.", "X-MS-Exchange-Moderation-Loop");
				return false;
			}
			if ("X-MS-Gcc-Journal-Report".Equals(headerName, StringComparison.OrdinalIgnoreCase))
			{
				ExTraceGlobals.TransportRulesEngineTracer.TraceDebug<string>(0L, "{0} header cannot be set through transport rules, skipping this action.", "X-MS-Gcc-Journal-Report");
				return false;
			}
			if ("X-MS-Exchange-Transport-Rules-Defer-Count".Equals(headerName, StringComparison.OrdinalIgnoreCase))
			{
				ExTraceGlobals.TransportRulesEngineTracer.TraceDebug<string>(0L, "{0} header cannot be set through transport rules, skipping this action.", "X-MS-Exchange-Transport-Rules-Defer-Count");
				return false;
			}
			if ("X-MS-Exchange-Generated-Message-Source".Equals(headerName, StringComparison.OrdinalIgnoreCase))
			{
				ExTraceGlobals.TransportRulesEngineTracer.TraceDebug<string>(0L, "{0} header cannot be set through transport rules, skipping this action.", "X-MS-Exchange-Generated-Message-Source");
				return false;
			}
			bool result;
			try
			{
				Header header = Header.Create(headerName);
				header.Value = value;
				result = true;
			}
			catch (ArgumentException)
			{
				result = false;
			}
			catch (NotSupportedException)
			{
				result = false;
			}
			catch (MimeException)
			{
				result = false;
			}
			return result;
		}

		public static string GetSenderManagerAddress(RulesEvaluationContext baseContext)
		{
			TransportRulesEvaluationContext transportRulesEvaluationContext = baseContext as TransportRulesEvaluationContext;
			if (transportRulesEvaluationContext.MailItem.Message.Sender == null || string.IsNullOrEmpty(transportRulesEvaluationContext.MailItem.Message.Sender.SmtpAddress))
			{
				return string.Empty;
			}
			return TransportUtils.GetManagerAddress(baseContext, transportRulesEvaluationContext.MailItem.Message.Sender.SmtpAddress);
		}

		public static string GetManagerAddress(RulesEvaluationContext baseContext, string userAddress)
		{
			TransportRulesEvaluationContext transportRulesEvaluationContext = (TransportRulesEvaluationContext)baseContext;
			IADRecipientCache iadrecipientCache = (IADRecipientCache)TransportUtils.GetTransportMailItemFacade(transportRulesEvaluationContext.MailItem).ADRecipientCacheAsObject;
			ProxyAddress proxyAddress = null;
			try
			{
				proxyAddress = new SmtpProxyAddress(userAddress, true);
			}
			catch (ArgumentOutOfRangeException ex)
			{
				ExTraceGlobals.TransportRulesEngineTracer.TraceError<string, string>(0L, "Detected invalid user address while trying to identify manager. Details {0}, {1}", userAddress, ex.ToString());
				return string.Empty;
			}
			ADRawEntry data = iadrecipientCache.FindAndCacheRecipient(proxyAddress).Data;
			if (data == null)
			{
				return string.Empty;
			}
			ADObjectId adobjectId = data[ADOrgPersonSchema.Manager] as ADObjectId;
			string result = null;
			if (adobjectId != null)
			{
				ADRecipient adrecipient = null;
				try
				{
					IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.FullyConsistent, ADSessionSettings.RootOrgOrSingleTenantFromAcceptedDomainAutoDetect(new SmtpAddress(userAddress).Domain), 720, "GetManagerAddress", "f:\\15.00.1497\\sources\\dev\\MessagingPolicies\\src\\rules\\TransportUtils.cs");
					adrecipient = tenantOrRootOrgRecipientSession.Read(adobjectId);
				}
				catch (ADTransientException)
				{
					return string.Empty;
				}
				if (adrecipient != null)
				{
					result = adrecipient.PrimarySmtpAddress.ToString();
				}
			}
			return result;
		}

		public static void AddHeaderToMail(EmailMessage message, string headerName, string value)
		{
			Header header = Header.Create(headerName);
			header.Value = value;
			message.MimeDocument.RootPart.Headers.AppendChild(header);
		}

		public static void SetHeaderValue(EmailMessage message, string headerName, string value)
		{
			HeaderList headers = message.MimeDocument.RootPart.Headers;
			if (headers == null)
			{
				return;
			}
			Header header = headers.FindFirst(headerName);
			if (header != null)
			{
				header.Value = value;
				return;
			}
			TransportUtils.AddHeaderToMail(message, headerName, value);
		}

		public static bool TryGetHeaderValue(EmailMessage message, string headerName, out string headerValue)
		{
			if (message.MimeDocument != null && message.MimeDocument.RootPart != null && message.MimeDocument.RootPart.Headers != null)
			{
				Header header = message.MimeDocument.RootPart.Headers.FindFirst(headerName);
				if (header != null)
				{
					headerValue = header.Value;
					return true;
				}
			}
			headerValue = null;
			return false;
		}

		public static string GenerateHashString(string key)
		{
			string result;
			using (HashAlgorithm hashAlgorithm = new SHA256CryptoServiceProvider())
			{
				byte[] array = hashAlgorithm.ComputeHash(Encoding.Unicode.GetBytes(key));
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < array.Length; i++)
				{
					stringBuilder.Append(array[i].ToString("x2"));
				}
				result = stringBuilder.ToString();
			}
			return result;
		}

		public static bool UserAttributeContainsWords(TransportRulesEvaluationContext context, string user, string[] attributeList, string tagName)
		{
			if (!SmtpProxyAddress.IsValidProxyAddress(user))
			{
				ExTraceGlobals.TransportRulesEngineTracer.TraceError<string>(0L, "Detected invalid user address while trying to match user attribute words. User address: {0}", user);
				return false;
			}
			IADRecipientCache iadrecipientCache = (IADRecipientCache)TransportUtils.GetTransportMailItemFacade(context.MailItem).ADRecipientCacheAsObject;
			SmtpProxyAddress smtpProxyAddress = new SmtpProxyAddress(user, true);
			ADRawEntry data = iadrecipientCache.FindAndCacheRecipient(smtpProxyAddress).Data;
			if (data == null || attributeList.Length == 0)
			{
				return false;
			}
			int i = 0;
			while (i < attributeList.Length)
			{
				string text = attributeList[i];
				int num = text.IndexOf(':');
				if (num >= 0 && num < text.Length - 1)
				{
					string text2 = text.Substring(0, num);
					string text3 = text.Substring(num + 1);
					string[] keywords = text3.Split(new char[]
					{
						','
					}, StringSplitOptions.RemoveEmptyEntries);
					string text4 = TransportUtils.GetMacroPropertyDefinition(smtpProxyAddress, text2.Trim(), data);
					text4 = text4.Trim().ToLowerInvariant();
					if (!string.IsNullOrEmpty(text4))
					{
						bool flag = TransportUtils.IsMatchTplKeyword(text4, tagName + text4, keywords, context);
						if (flag)
						{
							return true;
						}
					}
					i++;
					continue;
				}
				return false;
			}
			return false;
		}

		public static bool UserAttributeMatchesPatterns(TransportRulesEvaluationContext context, string user, string[] attributeList, string tagName)
		{
			if (!SmtpProxyAddress.IsValidProxyAddress(user))
			{
				ExTraceGlobals.TransportRulesEngineTracer.TraceError<string>(0L, "Detected invalid user address while trying to match user attribute patterns. User address: {0}", user);
				return false;
			}
			IADRecipientCache iadrecipientCache = (IADRecipientCache)TransportUtils.GetTransportMailItemFacade(context.MailItem).ADRecipientCacheAsObject;
			SmtpProxyAddress smtpProxyAddress = new SmtpProxyAddress(user, true);
			ADRawEntry data = iadrecipientCache.FindAndCacheRecipient(smtpProxyAddress).Data;
			if (data == null || attributeList.Length == 0)
			{
				return false;
			}
			int num = 0;
			if (num >= attributeList.Length)
			{
				return true;
			}
			string text = attributeList[num];
			int num2 = text.IndexOf(':');
			bool result;
			if (num2 < 0 || num2 >= text.Length - 1)
			{
				result = false;
			}
			else
			{
				string text2 = text.Substring(0, num2).Trim();
				string text3 = text.Substring(num2 + 1);
				string[] patterns = text3.Split(new char[]
				{
					','
				}, StringSplitOptions.RemoveEmptyEntries);
				string text4 = TransportUtils.GetMacroPropertyDefinition(smtpProxyAddress, text2, data);
				text4 = text4.Trim();
				if (string.IsNullOrEmpty(text4))
				{
					result = false;
				}
				else
				{
					result = TransportUtils.IsMatchTplRegex(text4, "senderAttributeMatchesRegex" + text2, patterns, context, false);
				}
			}
			return result;
		}

		internal static bool IsMatchTplRegex(string text, string textId, IEnumerable patterns, TransportRulesEvaluationContext context, bool isLegacyRegex)
		{
			string regex = string.Empty;
			bool result;
			try
			{
				MultiMatcher multiMatcher = new MultiMatcher();
				MatchFactory matchFactory = new MatchFactory();
				foreach (object obj in patterns)
				{
					string text2 = (string)obj;
					regex = text2;
					string pattern;
					if (isLegacyRegex)
					{
						pattern = RegexUtils.ConvertLegacyRegexToTpl(text2);
					}
					else
					{
						pattern = text2;
					}
					multiMatcher.Add(matchFactory.CreateRegex(pattern, CaseSensitivityMode.Insensitive, MatchRegexOptions.ExplicitCaptures, MatchesRegexPredicate.RegexMatchTimeout));
				}
				result = multiMatcher.IsMatch(text, textId, context);
			}
			catch (ArgumentException)
			{
				string message = TransportRulesStrings.InvalidRegexInTransportRule(regex);
				ExTraceGlobals.TransportRulesEngineTracer.TraceError(0L, message);
				throw new TransportRulePermanentException(message);
			}
			return result;
		}

		internal static bool IsMatchTplKeyword(string text, string textId, ICollection<string> keywords, TransportRulesEvaluationContext context)
		{
			bool result;
			try
			{
				MultiMatcher multiMatcher = new MultiMatcher();
				MatchFactory matchFactory = new MatchFactory();
				IMatch matcher = matchFactory.CreateSingleExecutionTermSet(keywords);
				multiMatcher.Add(matcher);
				result = multiMatcher.IsMatch(text, textId, context);
			}
			catch (ArgumentException)
			{
				string message = TransportRulesStrings.InvalidKeywordInTransportRule((keywords == null) ? string.Empty : string.Join(",", keywords.ToArray<string>()));
				ExTraceGlobals.TransportRulesEngineTracer.TraceError(0L, message);
				throw new TransportRulePermanentException(message);
			}
			return result;
		}

		public static bool UserExistsInSupervisionMaps(string tag, SupervisionMaps supervisionMaps, ADObjectId userADObjectId, string userSmtpAddress, IADRecipientCache recipientCache, TransportRulesEvaluationContext context)
		{
			if (supervisionMaps == null)
			{
				return false;
			}
			Dictionary<string, List<ADObjectId>> internalRecipientSupervisionMap = supervisionMaps.InternalRecipientSupervisionMap;
			Dictionary<string, List<ADObjectId>> dlSupervisionMap = supervisionMaps.DlSupervisionMap;
			Dictionary<string, List<SmtpAddress>> oneOffSupervisionMap = supervisionMaps.OneOffSupervisionMap;
			if (userADObjectId != null && internalRecipientSupervisionMap.ContainsKey(tag))
			{
				List<ADObjectId> list = internalRecipientSupervisionMap[tag];
				if (list.Contains(userADObjectId))
				{
					return true;
				}
			}
			if (string.IsNullOrEmpty(userSmtpAddress) || !SmtpAddress.IsValidSmtpAddress(userSmtpAddress))
			{
				ExTraceGlobals.TransportRulesEngineTracer.TraceError<string>(0L, "Detected invalid user address while trying to locate user in supervision maps. User address: {0}", userSmtpAddress);
				return false;
			}
			if (dlSupervisionMap.ContainsKey(tag))
			{
				List<ADObjectId> list2 = dlSupervisionMap[tag];
				foreach (ADObjectId objectId in list2)
				{
					ADRawEntry data = recipientCache.FindAndCacheRecipient(objectId).Data;
					string y = (string)((SmtpAddress)data[ADRecipientSchema.PrimarySmtpAddress]);
					if (context.MembershipChecker.Equals(userSmtpAddress, y))
					{
						return true;
					}
				}
			}
			if (oneOffSupervisionMap.ContainsKey(tag))
			{
				List<SmtpAddress> list3 = oneOffSupervisionMap[tag];
				if (list3.Contains(new SmtpAddress(userSmtpAddress)))
				{
					return true;
				}
			}
			return false;
		}

		public static ShortList<string> BuildPatternListForUserAttributeMatchesPredicate(IList<string> entries)
		{
			ShortList<string> shortList = new ShortList<string>();
			foreach (string text in entries)
			{
				int num = text.IndexOf(':');
				if (num > 0 && num < text.Length - 1)
				{
					string str = text.Substring(0, num + 1);
					string text2 = text.Substring(num + 1);
					string[] array = text2.Split(new char[]
					{
						','
					}, StringSplitOptions.RemoveEmptyEntries);
					StringBuilder stringBuilder = new StringBuilder(array.Length * 30);
					foreach (string text3 in array)
					{
						if (stringBuilder.Length > 0)
						{
							stringBuilder.Append(",");
						}
						stringBuilder.Append(RegexUtils.ConvertLegacyRegexToTpl(text3.Trim()));
					}
					shortList.Add(str + stringBuilder.ToString());
				}
			}
			return shortList;
		}

		public static IEnumerable<string> GetCanonicalizedStringProperty(Property property, RulesEvaluationContext context)
		{
			object value = property.GetValue(context);
			if (value is string)
			{
				return new List<string>
				{
					value as string
				};
			}
			return (IEnumerable<string>)value;
		}

		public static Dictionary<string, string> GetDataClassificationsFromRules(IEnumerable<Rule> rules)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (Rule rule in rules)
			{
				if (rule.Enabled == RuleState.Enabled)
				{
					SupplementalData supplementalData = new SupplementalData();
					rule.GetSupplementalData(supplementalData);
					Dictionary<string, string> dictionary2 = supplementalData.Get("DataClassification");
					foreach (KeyValuePair<string, string> keyValuePair in dictionary2)
					{
						if (!dictionary.ContainsKey(keyValuePair.Key))
						{
							dictionary.Add(keyValuePair.Key, keyValuePair.Value);
						}
					}
				}
			}
			return dictionary;
		}

		internal static Guid GetExternalOrganizationID(MailItem mailItem)
		{
			ITransportMailItemWrapperFacade transportMailItemWrapperFacade = mailItem as ITransportMailItemWrapperFacade;
			if (transportMailItemWrapperFacade == null)
			{
				return Guid.Empty;
			}
			ITransportMailItemFacade transportMailItem = transportMailItemWrapperFacade.TransportMailItem;
			return transportMailItem.ExternalOrganizationId;
		}

		internal static OrganizationId GetOrganizationID(MailItem mailItem)
		{
			ITransportMailItemWrapperFacade transportMailItemWrapperFacade = mailItem as ITransportMailItemWrapperFacade;
			if (transportMailItemWrapperFacade == null)
			{
				return null;
			}
			ITransportMailItemFacade transportMailItem = transportMailItemWrapperFacade.TransportMailItem;
			if (transportMailItem == null)
			{
				return null;
			}
			return transportMailItem.OrganizationIdAsObject as OrganizationId;
		}

		internal static string GetMessageID(MailItem mailItem)
		{
			if (mailItem == null)
			{
				return string.Empty;
			}
			if (mailItem.InternetMessageId == null)
			{
				return "not available";
			}
			return mailItem.InternetMessageId;
		}

		internal static string GetMessageAuth(TransportRulesEvaluationContext context)
		{
			ITransportConfiguration configuration;
			if (Components.TryGetConfigurationComponent(out configuration))
			{
				IReadOnlyMailItem mailItem = (IReadOnlyMailItem)((ITransportMailItemWrapperFacade)context.MailItem).TransportMailItem;
				OrganizationScopeResult organizationScopeResult;
				if (!MultilevelAuth.TryGetOrganizationScopeResult(mailItem, configuration, out organizationScopeResult))
				{
					throw new TransportRuleTransientException(new LocalizedString("Failed to determine organization scope"));
				}
				if (!organizationScopeResult.FromOrganizationScope)
				{
					return "<>";
				}
			}
			return "FromInternal";
		}

		internal static bool IsInternalMail(TransportRulesEvaluationContext context)
		{
			return context.EventSource == null || MultilevelAuth.IsInternalMail(context.MailItem.Message);
		}

		internal static string GetFileExtension(string name)
		{
			StringBuilder stringBuilder = new StringBuilder(name);
			foreach (char oldChar in TransportUtils.invalidPathChars)
			{
				stringBuilder.Replace(oldChar, ' ');
			}
			string text = stringBuilder.ToString();
			if (string.IsNullOrEmpty(text))
			{
				return string.Empty;
			}
			string extension = Path.GetExtension(text);
			if (string.IsNullOrEmpty(extension))
			{
				return extension;
			}
			return extension.Substring(1);
		}

		internal static bool IsAttachmentExemptFromFilenameMatching(AttachmentInfo attachmentInfo)
		{
			return TransportUtils.IsAttachmentParentExemptFromFilenameMatching(attachmentInfo.Parent);
		}

		private static bool IsAttachmentParentExemptFromFilenameMatching(AttachmentInfo attachmentInfo)
		{
			return attachmentInfo != null && (attachmentInfo.IsFileEmbeddingSupported() || (attachmentInfo.Parent != null && TransportUtils.IsAttachmentParentExemptFromFilenameMatching(attachmentInfo.Parent)));
		}

		internal static IPAddress GetOriginalClientIpAddress(MailMessage mailMessage)
		{
			string text = mailMessage.Headers["X-MS-Exchange-Organization-OriginalClientIPAddress"].FirstOrDefault<string>();
			IPAddress result;
			if (!string.IsNullOrEmpty(text) && IPAddress.TryParse(text, out result))
			{
				return result;
			}
			return IPAddress.None;
		}

		internal static ITransportMailItemFacade GetTransportMailItemFacade(MailItem mailItem)
		{
			return ((ITransportMailItemWrapperFacade)mailItem).TransportMailItem;
		}

		internal static TransportMailItem GetTransportMailItem(MailItem mailItem)
		{
			ITransportMailItemWrapperFacade transportMailItemWrapperFacade = mailItem as ITransportMailItemWrapperFacade;
			if (transportMailItemWrapperFacade != null)
			{
				return transportMailItemWrapperFacade.TransportMailItem as TransportMailItem;
			}
			return null;
		}

		internal static string GetExtraWatsonData(TransportRulesEvaluationContext transportRulesEvaluationContext, string predicateOrActionName)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (transportRulesEvaluationContext == null)
			{
				stringBuilder.AppendLine("ETR Unhandled exception - TransportRulesEvaluationContext is null");
			}
			else
			{
				stringBuilder.AppendLine(string.Format("ETR Tenant ID: {0}", TransportUtils.GetOrganizationID(transportRulesEvaluationContext.MailItem)));
				stringBuilder.AppendLine(string.Format("ETR Rule ID: {0}", TransportUtils.GetCurrentRuleId(transportRulesEvaluationContext)));
				stringBuilder.AppendLine(string.Format("ETR Message ID: {0}", TransportUtils.GetMessageID(transportRulesEvaluationContext.MailItem)));
				stringBuilder.AppendLine(string.Format("ETR Predicate/Action Name: {0}", predicateOrActionName ?? string.Empty));
			}
			return stringBuilder.ToString();
		}

		internal static string GetCurrentRuleId(RulesEvaluationContext context)
		{
			if (context.CurrentRule != null)
			{
				return context.CurrentRule.ImmutableId.ToString();
			}
			return string.Empty;
		}

		internal static string GetCurrentPredicateName(TransportRulesEvaluationContext context)
		{
			string result;
			if (context.CurrentRule != null)
			{
				if ((result = context.PredicateName) == null)
				{
					return string.Empty;
				}
			}
			else
			{
				result = string.Empty;
			}
			return result;
		}

		internal static string GetCurrentActionName(TransportRulesEvaluationContext context)
		{
			string result;
			if (context.CurrentRule != null)
			{
				if ((result = context.ActionName) == null)
				{
					return string.Empty;
				}
			}
			else
			{
				result = string.Empty;
			}
			return result;
		}

		internal static void AddRuleCollectionStamp(EmailMessage message, string headerValue)
		{
			Header header = Header.Create("X-MS-Exchange-Forest-RulesExecuted");
			header.Value = headerValue;
			message.MimeDocument.RootPart.Headers.AppendChild(header);
		}

		internal static bool RuleCollectionExecuted(EmailMessage message)
		{
			return message.MimeDocument.RootPart.Headers.FindFirst("X-MS-Exchange-Forest-RulesExecuted") != null;
		}

		internal static bool IsMsgAttachmentNameEnabled(MailItem mailItem)
		{
			return true;
		}

		public const string E12RuleCollectionName = "Transport";

		public const string E14RuleCollectionName = "TransportVersioned";

		public const string EdgeRuleCollectionName = "Edge";

		public const int MaxNumberOfNestedAttachmentsToScan = 100;

		public static Dictionary<string, ADPropertyDefinition> MacroNameLookupTable = new Dictionary<string, ADPropertyDefinition>();

		private static object lockVar = new object();

		private static char[] invalidPathChars = Path.GetInvalidPathChars();

		private class OrganizationIdPropertyBag : IPropertyBag, IReadOnlyPropertyBag
		{
			public OrganizationIdPropertyBag(Guid id)
			{
				this.objectId = new ADObjectId(this.GetDistinguishedName(id), id);
			}

			public void SetProperties(ICollection<PropertyDefinition> propertyDefinitionArray, object[] propertyValuesArray)
			{
				throw new NotImplementedException();
			}

			public object this[PropertyDefinition propertyDefinition]
			{
				get
				{
					if (propertyDefinition == ADObjectSchema.OrganizationalUnitRoot || propertyDefinition == ADObjectSchema.ConfigurationUnit)
					{
						return this.objectId;
					}
					throw new ArgumentException();
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			public object[] GetProperties(ICollection<PropertyDefinition> propertyDefinitionArray)
			{
				throw new NotImplementedException();
			}

			protected string GetDistinguishedName(Guid id)
			{
				string arg = string.Join(",", from part in Domain.GetComputerDomain().Name.Split(new char[]
				{
					'.'
				})
				select "DC=" + part);
				return string.Format("CN={0},CN=ConfigurationUnits,{1}", id, arg);
			}

			protected ADObjectId objectId;
		}
	}
}
