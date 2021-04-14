using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage.RightsManagement;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Security.RightsManagement;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.RightsManagement;

namespace Microsoft.Exchange.MessagingPolicies.RmSvcAgent
{
	internal static class Utils
	{
		static Utils()
		{
			Utils.PreserveHeaderIds.Add(HeaderId.ContentType);
			Utils.PreserveHeaderIds.Add(HeaderId.MimeVersion);
			Utils.PreserveHeaderIds.Add(HeaderId.ContentClass);
			Utils.PreserveHeaderIds.Add(HeaderId.ContentTransferEncoding);
			Utils.PreserveHeaderIds.Add(HeaderId.ContentLanguage);
			Utils.PreserveHeaderNames.Add("X-MS-TNEF-Correlator");
			Utils.PreserveHeaderNames.Add("Accept-Language");
		}

		public static bool CheckMuaSubmission(MailItem mailItem)
		{
			if (mailItem.InboundDeliveryMethod != DeliveryMethod.Smtp)
			{
				ExTraceGlobals.RmSvcAgentTracer.TraceDebug(0L, "Non-SMTP message, skipping MUA check");
				return false;
			}
			object obj;
			if (!mailItem.Properties.TryGetValue("Microsoft.Exchange.SmtpMuaSubmission", out obj))
			{
				ExTraceGlobals.RmSvcAgentTracer.TraceError<string>(0L, "Unexpected error: the property {0} does not exist on an SMTP submitted message. Assuming this is not an MUA", "Microsoft.Exchange.SmtpMuaSubmission");
				return false;
			}
			if (!(obj is bool))
			{
				ExTraceGlobals.RmSvcAgentTracer.TraceError<string>(0L, "Unexpected error: the property {0} was expected to be a UINT, but it is not. Assuming this is not an MUA", "Microsoft.Exchange.SmtpMuaSubmission");
				return false;
			}
			return (bool)obj;
		}

		public static int GetMaxActiveAgents()
		{
			int num = Components.TransportAppConfig.Resolver.MaxExecutingJobs;
			num >>= 1;
			if (num >= 1)
			{
				return num;
			}
			return 1;
		}

		public static bool IsSupportedMapiMessageClass(EmailMessage message)
		{
			if (string.Equals(message.MapiMessageClass, Constants.SupportedMapiMessageClassForDrm, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			ExTraceGlobals.RmSvcAgentTracer.TraceDebug<string>(0L, "Unsupported message class for Drm: {0}", message.MapiMessageClass);
			return false;
		}

		public static RmsClientManagerContext CreateRmsContext(OrganizationId orgId, MailItem mailItem, string messageId, string publishingLicense = null)
		{
			ArgumentValidator.ThrowIfNull("orgId", orgId);
			ArgumentValidator.ThrowIfNull("mailItem", mailItem);
			IReadOnlyMailItem readOnlyMailItem = (IReadOnlyMailItem)((ITransportMailItemWrapperFacade)mailItem).TransportMailItem;
			if (readOnlyMailItem != null)
			{
				return new RmsClientManagerContext(orgId, RmsClientManagerContext.ContextId.MessageId, messageId, readOnlyMailItem.ADRecipientCache, new RmsLatencyTracker(readOnlyMailItem.LatencyTracker), publishingLicense)
				{
					SystemProbeId = mailItem.SystemProbeId
				};
			}
			return new RmsClientManagerContext(orgId, RmsClientManagerContext.ContextId.MessageId, messageId, null, null, publishingLicense)
			{
				SystemProbeId = mailItem.SystemProbeId
			};
		}

		public static void PatchReceiverHeader(HeaderList headerList, string localTcpInfo, string clauseToInsert)
		{
			DateTime t = DateTime.MaxValue;
			ReceivedHeader receivedHeader = null;
			foreach (Header header in headerList)
			{
				if (header.HeaderId == HeaderId.Received)
				{
					ReceivedHeader receivedHeader2 = header as ReceivedHeader;
					DateTime dateTime;
					if (receivedHeader2 != null && receivedHeader2.ByTcpInfo != null && receivedHeader2.ByTcpInfo.Contains(localTcpInfo) && DateTime.TryParse(receivedHeader2.Date, out dateTime) && dateTime < t)
					{
						receivedHeader = receivedHeader2;
						t = dateTime;
					}
				}
			}
			if (receivedHeader != null)
			{
				headerList.RemoveChild(receivedHeader);
				int num = (receivedHeader.With == null) ? -1 : receivedHeader.With.IndexOf(')');
				StringBuilder stringBuilder = new StringBuilder(clauseToInsert.Length + 3);
				string with;
				if (num == -1)
				{
					stringBuilder.Append(" (");
					stringBuilder.Append(clauseToInsert);
					stringBuilder.Append(")");
					with = receivedHeader.With + stringBuilder.ToString();
				}
				else
				{
					stringBuilder.Append(" + ");
					stringBuilder.Append(clauseToInsert);
					with = receivedHeader.With.Insert(num, stringBuilder.ToString());
				}
				ReceivedHeader newChild = new ReceivedHeader(receivedHeader.From, (receivedHeader.FromTcpInfo == null) ? null : receivedHeader.FromTcpInfo.Trim(Utils.Parentheses), receivedHeader.By, (receivedHeader.ByTcpInfo == null) ? null : receivedHeader.ByTcpInfo.Trim(Utils.Parentheses), receivedHeader.For, with, receivedHeader.Id, receivedHeader.Via, receivedHeader.Date);
				headerList.InsertAfter(newChild, receivedHeader.LastChild);
			}
		}

		public static int IncrementDeferralCount(MailItem mailItem, string deferralCountPropertyName)
		{
			ArgumentValidator.ThrowIfNull("mailItem", mailItem);
			ArgumentValidator.ThrowIfNullOrEmpty("deferralCountPropertyName", deferralCountPropertyName);
			object obj;
			int num;
			if (!mailItem.Properties.TryGetValue(deferralCountPropertyName, out obj))
			{
				num = 0;
			}
			else
			{
				if (!(obj is int))
				{
					return -1;
				}
				num = (int)obj;
			}
			num++;
			mailItem.Properties[deferralCountPropertyName] = num;
			return num;
		}

		public static void SetTransportDecryptionApplied(MailItem mailItem, bool reset)
		{
			if (reset)
			{
				if (mailItem.Properties.ContainsKey("Microsoft.Exchange.RightsManagement.TransportDecrypted"))
				{
					mailItem.Properties.Remove("Microsoft.Exchange.RightsManagement.TransportDecrypted");
					return;
				}
			}
			else
			{
				mailItem.Properties["Microsoft.Exchange.RightsManagement.TransportDecrypted"] = "True";
			}
		}

		public static void GetTransportDecryptionPLAndUL(MailItem mailItem, out string publishLicense, out string useLicense)
		{
			object obj;
			mailItem.Properties.TryGetValue("Microsoft.Exchange.RightsManagement.TransportDecryptionPL", out obj);
			publishLicense = (string)obj;
			mailItem.Properties.TryGetValue("Microsoft.Exchange.RightsManagement.TransportDecryptionUL", out obj);
			useLicense = (string)obj;
		}

		public static void SetTransportDecryptionPLAndUL(MailItem mailItem, string publishLicense, string useLicense)
		{
			mailItem.Properties["Microsoft.Exchange.RightsManagement.TransportDecryptionPL"] = publishLicense;
			mailItem.Properties["Microsoft.Exchange.RightsManagement.TransportDecryptionUL"] = useLicense;
		}

		public static string GetDecryptionTokenRecipient(MailItem mailItem, AcceptedDomainCollection acceptedDomains)
		{
			object obj;
			string text;
			if (!mailItem.Properties.TryGetValue("Microsoft.Exchange.RightsManagement.DecryptionTokenRecipient", out obj))
			{
				text = MPCommonUtils.GetDecryptionTokenRecipient(mailItem, acceptedDomains);
				mailItem.Properties["Microsoft.Exchange.RightsManagement.DecryptionTokenRecipient"] = text;
			}
			else
			{
				text = (string)obj;
			}
			return text;
		}

		public static void NDRMessage(MailItem mailItem, string messageId, HashSet<int> recipientsToNDR, SmtpResponse response)
		{
			EnvelopeRecipientCollection recipients = mailItem.Recipients;
			if (recipients == null)
			{
				ExTraceGlobals.RmSvcAgentTracer.TraceError<string>(0L, "No recipients to NDR for message {0}", messageId);
				return;
			}
			for (int i = recipients.Count - 1; i >= 0; i--)
			{
				if (recipientsToNDR.Contains(i))
				{
					ExTraceGlobals.RmSvcAgentTracer.TraceDebug<string, string[]>(0L, "NDR recipient {0} with {1}", recipients[i].Address.ToString(), response.StatusText);
					mailItem.Recipients.Remove(recipients[i], DsnType.Failure, response);
				}
			}
		}

		public static void NDRMessage(MailItem mailItem, string messageId, SmtpResponse response)
		{
			ExTraceGlobals.RmSvcAgentTracer.TraceError<string, SmtpResponse>(0L, "NDRMessage for message {0}, Response {1}", messageId, response);
			EnvelopeRecipientCollection recipients = mailItem.Recipients;
			if (recipients == null)
			{
				ExTraceGlobals.RmSvcAgentTracer.TraceError<string>(0L, "No recipients to NDR for message {0}", messageId);
				return;
			}
			for (int i = recipients.Count - 1; i >= 0; i--)
			{
				mailItem.Recipients.Remove(recipients[i], DsnType.Failure, response);
			}
		}

		public static SmtpResponse GetResponseForExceptionDeferral(Exception exception, string[] additionalInfo)
		{
			return Utils.GetResponseForDeferral(Utils.GetSmtpResponseTextsForException(exception, additionalInfo));
		}

		public static SmtpResponse GetResponseForDeferral(string[] text)
		{
			return new SmtpResponse("451", "4.3.2", Utils.FilterAsciiStrings(text));
		}

		public static SmtpResponse GetResponseForNDR(string[] text)
		{
			string[] array = Utils.FilterAsciiStrings(text);
			if (array.Length == 0)
			{
				return Constants.NDRResponse;
			}
			List<string> list = new List<string>(text.Length + 2);
			list.Add("Delivery not authorized, message refused.");
			list.AddRange(array);
			list.Add("Please contact your system administrator for more information.");
			return new SmtpResponse("550", "5.7.1", list.ToArray());
		}

		public static string[] GetSmtpResponseTextsForException(Exception exception, string[] additionalInfo)
		{
			if (exception == null)
			{
				return additionalInfo;
			}
			List<string> list;
			if (additionalInfo != null && additionalInfo.Length != 0)
			{
				list = new List<string>(2 + additionalInfo.Length);
			}
			else
			{
				list = new List<string>(2);
			}
			RightsManagementException ex = exception as RightsManagementException;
			if (ex != null)
			{
				list.AddRange(new string[]
				{
					string.Format(CultureInfo.InvariantCulture, "Exception encountered: {0}.", new object[]
					{
						exception.GetType().Name
					}),
					string.Format(CultureInfo.InvariantCulture, "Failure Code: {0}.", new object[]
					{
						ex.FailureCode.ToString()
					})
				});
			}
			else
			{
				list.AddRange(new string[]
				{
					string.Format(CultureInfo.InvariantCulture, "Exception encountered: {0}.", new object[]
					{
						exception.GetType()
					})
				});
			}
			if (additionalInfo != null && additionalInfo.Length != 0)
			{
				list.AddRange(additionalInfo);
			}
			return list.ToArray();
		}

		private static string[] FilterAsciiStrings(string[] texts)
		{
			if (texts == null)
			{
				return null;
			}
			List<string> list = new List<string>(texts.Length);
			foreach (string text in texts)
			{
				AsciiString arg;
				if (AsciiString.TryParse(text, out arg))
				{
					list.Add(text);
				}
				else
				{
					ExTraceGlobals.RmSvcAgentTracer.TraceError<AsciiString>(0L, "Encountered a Non-ASCII string in the response. String {0} - filtering it out of SmtpResponse", arg);
				}
			}
			return list.ToArray();
		}

		public static string GetTenantString(Guid tenantId)
		{
			string result = "Enterprise";
			if (tenantId != Guid.Empty)
			{
				result = string.Format(CultureInfo.InvariantCulture, "Tenant '{0}'", new object[]
				{
					tenantId
				});
			}
			return result;
		}

		public const string TransportDecryptionApplied = "Transport Decrypted";

		private static readonly char[] Parentheses = new char[]
		{
			'(',
			')'
		};

		internal static readonly HashSet<HeaderId> PreserveHeaderIds = new HashSet<HeaderId>();

		internal static readonly HashSet<string> PreserveHeaderNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
	}
}
