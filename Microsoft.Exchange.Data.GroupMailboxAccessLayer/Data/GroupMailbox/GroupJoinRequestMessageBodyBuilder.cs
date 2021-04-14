using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security.AntiXss;
using Microsoft.Exchange.Data.ApplicationLogic.Directory;
using Microsoft.Exchange.Data.GroupMailbox.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.GroupMailbox;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class GroupJoinRequestMessageBodyBuilder
	{
		public static void WriteMessageToStream(StreamWriter writer, string senderDisplayName, string groupDisplayName, string attachedMessageBody, MailboxUrls mailboxUrls, CultureInfo cultureInfo)
		{
			writer.Write(GroupJoinRequestMessageBodyBuilder.RenderTemplate(senderDisplayName, groupDisplayName, attachedMessageBody, GroupJoinRequestMessageBodyBuilder.ConvertFragmentToQuery(mailboxUrls.PeopleUrl), cultureInfo));
		}

		private static string RenderTemplate(string requestingMemberName, string groupName, string requestMessage, string groupMembersUrl, CultureInfo cultureInfo)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			Uri uri = new Uri(groupMembersUrl);
			dictionary["header"] = AntiXssEncoder.HtmlEncode(Strings.JoinRequestMessageHeading(requestingMemberName, groupName).ToString(cultureInfo), false);
			if (!string.IsNullOrEmpty(requestMessage))
			{
				dictionary["intro_text"] = AntiXssEncoder.HtmlEncode(Strings.JoinRequestMessageAttachedBodyPrefix.ToString(cultureInfo), false);
				dictionary["user_message"] = AntiXssEncoder.HtmlEncode(requestMessage, false);
			}
			else
			{
				dictionary["intro_text"] = AntiXssEncoder.HtmlEncode(Strings.JoinRequestMessageNoAttachedBodyPrefix.ToString(cultureInfo), false);
				dictionary["user_message"] = string.Empty;
			}
			dictionary["footer_text"] = Strings.JoinRequestMessageFooterTextWithLink(uri.ToString()).ToString(cultureInfo);
			return GroupJoinRequestMessageBodyBuilder.RenderTemplate(GroupJoinRequestMessageBodyBuilder.JoinRequestHtmlTemplate.Value, dictionary);
		}

		private static string ReadHtmlFromEmbeddedResources(string templateName)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("templateName", templateName);
			string text = null;
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			using (Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(templateName))
			{
				using (StreamReader streamReader = new StreamReader(manifestResourceStream, Encoding.UTF8))
				{
					GroupJoinRequestMessageBodyBuilder.Tracer.TraceDebug<string>(0L, "Found template {0} as embedded resource.", templateName);
					text = streamReader.ReadToEnd();
					GroupJoinRequestMessageBodyBuilder.Tracer.TraceDebug<int, string>(0L, "Read {0} bytes for image {1}.", text.Length, templateName);
				}
			}
			return text;
		}

		private static string RenderTemplate(string template, Dictionary<string, object> replacements)
		{
			return GroupJoinRequestMessageBodyBuilder.TemplateVariableMatchRegex.Replace(template, delegate(Match m)
			{
				string value = m.Groups[1].Value;
				return replacements.ContainsKey(value) ? replacements[value].ToString() : m.Value;
			});
		}

		private static string ConvertFragmentToQuery(string url)
		{
			Uri uri = new Uri(url);
			NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(uri.Query);
			if (uri.Fragment.Length > 1)
			{
				NameValueCollection c = HttpUtility.ParseQueryString(uri.Fragment.Substring(1));
				nameValueCollection.Add(c);
			}
			return string.Concat(new string[]
			{
				uri.Scheme,
				"://",
				uri.Authority,
				uri.AbsolutePath,
				"?",
				nameValueCollection.ToString()
			});
		}

		private static readonly Trace Tracer = ExTraceGlobals.GroupEmailNotificationHandlerTracer;

		private static readonly Lazy<string> JoinRequestHtmlTemplate = new Lazy<string>(() => GroupJoinRequestMessageBodyBuilder.ReadHtmlFromEmbeddedResources("group_join_request_message_template.thtm"));

		private static readonly Regex TemplateVariableMatchRegex = new Regex("\\${([^}]+)}", RegexOptions.Compiled);
	}
}
