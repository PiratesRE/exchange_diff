using System;
using System.Collections.Specialized;
using System.Globalization;
using System.ServiceModel.Channels;
using System.Web;
using System.Web.Security.AntiXss;
using Microsoft.Exchange.Clients.Owa2.Server.Core;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Data.ApplicationLogic.Directory;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.GroupMailbox.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Exchange.Services.Wcf.Types;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Clients.Owa2.Server.Web
{
	internal class GroupSubscriptionHandler : IHttpHandler
	{
		public bool IsReusable
		{
			get
			{
				return false;
			}
		}

		public void ProcessRequest(HttpContext context)
		{
			Exception ex = null;
			this.Initialize(context.Response);
			try
			{
				OwaServerTraceLogger.AppendToLog(new TraceLogEvent("GrEsProcReq", null, "GroupSubscriptionHandler.ProcessRequest", "Started processing request: " + context.Request.RawUrl));
				OWAService owaservice = new OWAService();
				CallContext callContext = GroupSubscriptionHandler.CreateAndSetCallContext(context);
				IExchangePrincipal mailboxIdentityPrincipal = callContext.MailboxIdentityPrincipal;
				try
				{
					this.SetPreferredCulture(mailboxIdentityPrincipal);
					this.ValidateRequestMadeToGroupMailbox(callContext);
					string action = this.ValidateAndGetActionString(context);
					this.ParseEscalateOperationType(action);
					if (this.RedirectToOwaGroupPageIfPossible(mailboxIdentityPrincipal, callContext, context.Response))
					{
						return;
					}
					this.BuildGroupHeaderDiv(mailboxIdentityPrincipal);
					SetModernGroupMembershipJsonRequest modernGroupMembership = new SetModernGroupMembershipJsonRequest
					{
						GroupSmtpAddress = mailboxIdentityPrincipal.MailboxInfo.PrimarySmtpAddress.ToString(),
						OperationType = this.operationType.Value
					};
					SetModernGroupMembershipJsonResponse ewsresponse = owaservice.SetModernGroupMembership(modernGroupMembership);
					this.ValidateEwsResponse(ewsresponse);
					this.WriteSuccessfulResponse(context);
				}
				finally
				{
					GroupSubscriptionHandler.DisposeContext(callContext);
				}
			}
			catch (ExceededMaxSubscribersException)
			{
				this.WriteErrorResponse(context, ClientStrings.GroupSubscriptionPageSubscribeFailedMaxSubscribers(this.encodedGroupDisplayName));
			}
			catch (NotAMemberException)
			{
				this.WriteErrorResponse(context, ClientStrings.GroupSubscriptionPageSubscribeFailedNotAMember(this.encodedGroupDisplayName));
			}
			catch (Exception ex2)
			{
				this.WriteErrorResponse(context, ClientStrings.GroupSubscriptionPageRequestFailedInfo);
				ex = ex2;
			}
			if (ex != null)
			{
				OwaServerTraceLogger.AppendToLog(new TraceLogEvent("GrEsProcReq", null, "GroupSubscriptionHandler.ProcessRequest", "Error processing request: " + ex.ToString()));
			}
		}

		private bool Initialize(HttpResponse response)
		{
			response.ContentType = "text/html";
			response.Cache.SetNoServerCaching();
			response.Cache.SetCacheability(HttpCacheability.NoCache);
			response.Cache.SetNoStore();
			return true;
		}

		private void SetPreferredCulture(IExchangePrincipal groupExchangePrincipal)
		{
			CultureInfo cultureInfo = null;
			if (groupExchangePrincipal != null)
			{
				cultureInfo = Culture.GetPreferredCultureInfo(groupExchangePrincipal);
			}
			cultureInfo = (cultureInfo ?? new CultureInfo("en-US"));
			this.pageCultureInfo = cultureInfo;
			Culture.InternalSetThreadPreferredCulture(this.pageCultureInfo);
		}

		private void ValidateRequestMadeToGroupMailbox(CallContext callContext)
		{
			if (callContext.MailboxIdentityPrincipal == null || callContext.MailboxIdentityPrincipal.RecipientTypeDetails != RecipientTypeDetails.GroupMailbox)
			{
				throw new OwaInvalidRequestException("GroupSubscriptionHandler.ProcessRequest - Mailbox isn't a GroupMailbox");
			}
		}

		private string ValidateAndGetActionString(HttpContext context)
		{
			NameValueCollection queryString = context.Request.QueryString;
			string text = queryString.Get("Action") ?? string.Empty;
			if (string.IsNullOrEmpty(text))
			{
				throw new OwaInvalidRequestException("GroupSubscriptionHandler.ProcessRequest - missing action parameter");
			}
			return text;
		}

		private void ParseEscalateOperationType(string action)
		{
			if (action.ToLowerInvariant() == "subscribe")
			{
				this.operationType = new ModernGroupMembershipOperationType?(ModernGroupMembershipOperationType.Escalate);
			}
			else if (action.ToLowerInvariant() == "unsubscribe")
			{
				this.operationType = new ModernGroupMembershipOperationType?(ModernGroupMembershipOperationType.DeEscalate);
			}
			if (this.operationType == null)
			{
				throw new OwaInvalidRequestException(string.Format("GroupSubscriptionHandler.ProcessRequest - invalid action parameter: {0}", action));
			}
		}

		private bool RedirectToOwaGroupPageIfPossible(IExchangePrincipal groupExchangePrincipal, CallContext callContext, HttpResponse response)
		{
			if (callContext.AccessingADUser.RecipientType != RecipientType.UserMailbox)
			{
				return false;
			}
			IMailboxUrls mailboxUrls = new MailboxUrls(groupExchangePrincipal, false);
			string text = mailboxUrls.OwaUrl;
			if (string.IsNullOrEmpty(text))
			{
				OwaServerTraceLogger.AppendToLog(new TraceLogEvent("GrEsRedir", null, "GroupSubscriptionHandler.RedirectToOwaGroupPageIfPossible", string.Format("Redirection attempt failed for user {0}: couldn't obtain Owa url", callContext.AccessingADUser.PrimarySmtpAddress)));
				return false;
			}
			if (!text.EndsWith("/"))
			{
				text += "/";
			}
			string domain = callContext.AccessingADUser.PrimarySmtpAddress.Domain;
			string text2 = text + string.Format("?realm={0}&exsvurl=1&path=/group/{1}/action/", domain, groupExchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString());
			if (this.operationType == ModernGroupMembershipOperationType.Escalate)
			{
				text2 += "subscribe";
			}
			else
			{
				text2 += "unsubscribe";
			}
			OwaServerTraceLogger.AppendToLog(new TraceLogEvent("GrEsRedir", null, "GroupSubscriptionHandler.RedirectToOwaGroupPageIfPossible", string.Format("Redirecting user {0} to Group page. Url:{1}", callContext.AccessingADUser.PrimarySmtpAddress, text2)));
			response.Redirect(text2);
			return true;
		}

		private string GetGroupTypeMessage(ModernGroupObjectType groupType)
		{
			switch (groupType)
			{
			case ModernGroupObjectType.Private:
				return this.GetLocalizedString(ClientStrings.GroupSubscriptionPagePrivateGroupInfo);
			case ModernGroupObjectType.Public:
				return this.GetLocalizedString(ClientStrings.GroupSubscriptionPagePublicGroupInfo);
			}
			return string.Empty;
		}

		private void BuildGroupHeaderDiv(IExchangePrincipal groupExchangePrincipal)
		{
			this.encodedGroupDisplayName = AntiXssEncoder.HtmlEncode(groupExchangePrincipal.MailboxInfo.DisplayName, false);
			string arg = string.Format("<div style=\"font-size:21px;margin-left:19px;position:absolute;top:0px;left:0px;text-overflow: ellipsis;overflow:hidden;white-space:nowrap;max-width:95%\">{0}</div><div style=\"font-size:12px;color:#666666;margin-top:24px;margin-left:20px\">{1}</div>", this.encodedGroupDisplayName, this.GetGroupTypeMessage(groupExchangePrincipal.ModernGroupType));
			this.groupHeaderDiv = string.Format("<table cellspacing=\"0\"cellpadding=\"0\"border=\"0\" style=\"width:100%; max-width:600px\"><tbody><tr><td style=\"width:50px;height:50px;position:relative\"><div><img style=\"top: 0px; display: inline; width:50px\" src=\"{0}\"></div><div style=\"position:absolute;top:0px\"><img style=\"top: 0px; display: inline; width:50px\"src=\"service.svc/s/GetUserPhoto?email={1}&UA=0&size=HR64x64\"onerror=\"this.style.display='none'\"></div></td><td style=\"color:#333333;position:relative\">{2}</td></tr></tbody></table>", "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAGAAAABgCAYAAADimHc4AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAUpSURBVHhe7Zu9ctswEIT5/o+TmbyG0zlFZpQihYoUKlKoSMFwEUGGoQWIw99BInZmC8si8fMd7w6Ms3z98nWd1vMEoOwJQNkTgLInAGVPAMqeAJQ9ASh7AlD2BKDsCUDZE4CyJwBlTwDKngCUPQEoewJQ9gSg7AlA2ROAsicAZU8Ayp4AlD0BKHsCUPYEoOynAXD6eVovl8t6vV5XX/gMv8N32LUje3gA51/ndf172+lEXX5f6L1G9LAA3r+/izfe1zM8EUMCMFFfSdc/VzrGKB4OQM3NtxoZwlAATj9Oty2rr1EhDAWgNOfv6Xw+03E1PQwAdC499PbtjY6v5SQAmDSiJ9aDo2th16a6lzBfNr7UqFVIaw9P7fYzPsfv2XW+owCwqWzTg9oGRx5n94q5V/Rb5czRWjrXvTNJEEDJpkgLXuvc7yunICMLZM9zuy6UISgA82iVahuU3du3OXApiM0l5FpzZGnpAUCVzb8pJdJQWzSUmqPhmvJP558AtMjFe61fTeASpaYhUQ1MlNuJ3QGYHNdIdgzm3vn/roQU2eJUDrmd2B1AC9JWsU5AU2w+rlsGh02BBkCPQuguzLWm2Hysm+/J7Qk0AHrk4dCrYU3FzgM4XLYWIBsAPYQF+YvsNXZIMQA9ahP2ZOnVh4deAagV4U1RAD20rX1BaugltlCtNhQKAcDnvbS06P1DYovVOohBbD7woQC0PH9EdetCmLsC6JkC2GLhlmeQkGIn4ZcEECrCcKsTZ0x774J6qVsKCrWh1r27ITYH173ms/SKPrS7bKHWPR/72KsR6x4HMWjpUgQjBc91r3TIxvbd43yEtPz/JNz4cdvLt66bz0XwlxGtmwPMxQBomoYSo9+6ZeTFOh/m1mkR2ccAgFtFHhbhLirFTU7nwkCwbpUWbVNyB9CC9l7nE3PNJ0Ea+b6rywmGOwC4akuaGXG+SyNQkvNDrp0W3Y7wEwC4yiNXafOt8XRKC2LJ08dcC4K7+fADALikKNdeuGsULUS0CRK/Zm0/43NJxyW1geCPm6rtOn/zYQoAxmIlTwMiVFJwcX98H2kP1vjPFBhfMmdraaqOHfyCAKzvUUdSAD7DzRlZZtzLnDADUYT74Tvs2lrGXGlgbXPC3CTjI2hKn8YFi65RqPYseZpyojLFWGeKpE9zjm0wLrcxgzmq1FhIjmKPbY4lAWCFa9i9Su2msA8AN9UsYsUn7C0oSiPRBICfIoSqGZh+IDwAgGpEX/HmO8pJCfg+rqulUghIOSwQKACjgujDZJtomxPyJoqfvyFYIOYbK/KlcseT2E05vsIArLbFoHj5MGLdQs3IG0mpNcEEwxYkJhh2tA8gIjZ4zdQzokJZIVfZABDlbCKvGv1WoXXnpr1sAKxQm0JzALH0m5JumLIBsFcHqQedZxc7uOam3mwALApyDjvPKFaMURtylA3AnwB8GG35vtb6swCwQnSU/G/lrx/OKcRZAFgBRk04klg7mtMBZgFg74tip71XVK09yAJQi/4zi2WBnC4wC4A/MNzq/cuoqtUJ1QNwMLFGJOclpBwAacFye+Bnl78PsFRiAIz80TogK38fYKnkAEjuO1oHZFWjGREDYNX/sADI+7DmANiLqKO8A/JVIxjFAOYZ4EPDADjaGcCKNSTS19JiAP6A8FFFO0JhSz4BFMrfC+lhTAaAHMKa/QnKk8jfD1giEYAaj9yrif3LoEQTQKFKmxIRgBpt16uJAZC05RNAodie9AWQ+fcwr6KyoFzXf8jAL38D/RRdAAAAAElFTkSuQmCC", groupExchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString(), arg);
		}

		private string GetMainPage(string mainBody)
		{
			return string.Format("<!DOCTYPE html><html><body style=\"margin:0px\"><div><font face={0}>{1}<div style=\"padding:50px\">{2}</div></font></div></body></html>", this.GetLocalizedString(ClientStrings.GroupSubscriptionPageBodyFont), string.Format("<div style=\"background-color:#0072C6;height:50px\"><img style=\"margin-left:15px; margin-top:11px\" src=\"{0}\"></div>", "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAGcAAAAdCAYAAAC3+HJeAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsIAAA7CARUoSoAAAAAhdEVYdENyZWF0aW9uIFRpbWUAMjAxNDowNjozMCAxNjowMDo0NBfIsOMAAAPcSURBVGhD7ZdPahRREIdzE0/iObyFC4+gR9AT6AHErbgT4kYXQjZmoxBQCCJBDBJEWr9hPinLX3d6zARa6YIf3a/qvfr/qmcODu4cDisWishcsQxE5oplIDJXLAOROQM37r0a7j//MByffh0evTwdbj18E/etuAIicwJ3n54Mr9+fD4k+fvk2PDs+2+xJZ1fsiMhs4FaQ9POL79syXE5Pjj5FXSt2QGT+BGOLcXVydrFN9+/ELXnx7vNmrCW6juLgE3rBzQdHGx5Peaz/q/GamHxLOlkMZCYGkJREJmsK6KkF5skNrfoFheHm6gdFAJ5j1LqeY3tXoBM7+MBzzAY+4At+0ti8K7v9+O3mbMdoQyVmTzhK0z7Q90qXJcgGIAj2CtYkoDvMLYYokjyKRALqPnSMBnsFkER8Jhc0EMSz7uFbC1EQ3gE+Kje+Gi9IzbhBYnKg0r6LYxDVcUHyCY4C9RtKgupeO6/yrgO1IYQFcu3Nnoo7xTCJxOwJ33dx6J4pJw3UzsS+t4R3wVqet4X33omsaQh8Ql5l2EK2698B43bNeXyuezo4s+jikACIEZHkgmJQRN7nkEFD1bYdThHZQwKNB19Y2yzQ3ORxu+te3p0EFDyNKuRz9W+QmNdZHL81l3Wpeuu6B8a68yBt+52qjWDivJ11tFoszskTnEGvfgB0KIdoBIomUXRupXs4g35l2J7MQ2KaGGmfxXF/klX0fax7IUxS5UHa7smvcAzVBIOx7iaJ8LmBJrbeDgh92LYBLJT7yKPAPno4M1qgxDQx0r94c9APjfnujw51CBJGAdIZQfIpTi0u1BsBmQWr/CrHHr4k+WK/OThck4S+XggTWnkQe/EZGvPdQrA3IZ2pILGQIxBK55KPFYzCUXliYqTSPosDSApIMmAB6+xHXw8iBQ6x1+SNNQFJmfJhDiDjpJH8dVkxxhdjY3SDxOwJ33dx7GoccywI5jMB9dGCvl2Ko5wCdBus9aEXD1n9loA0gmkcSFn6hmnD/HU/ODs19iaLQ3CXjR/3dho1uAV6cQxQJPZjD6IwPRDllce68yBtk2T01/HFfpOFXUj73iblAhk+qcMPPe91H3u0hS5jU15j5ckaf3qsv5CY/Pwjed3JBAwl6o4n4BQdR7A4SUD1p2cFvvRGYd152K1+VxuA95oMzpMo7aeYKTJ69ZP3dJu0xR50Jn+1leR/IDK3wNFaYZzDMZQi0+lE8JPOFTsgMhvoCApCcebSWpw9IDInwG3h6jJfK3G7HBv9g7riLxGZM8Hc5YakOb1iD4jMFctAZK5YBiJzxTIQmSsWgMPhB+HQgglctpnmAAAAAElFTkSuQmCC"), mainBody);
		}

		private void ValidateEwsResponse(SetModernGroupMembershipJsonResponse ewsresponse)
		{
			if (ewsresponse == null || ewsresponse.Body == null)
			{
				throw new InvalidOperationException("Invalid empty response");
			}
			if (ewsresponse.Body.ErrorCode == ModernGroupActionError.None)
			{
				return;
			}
			if (ewsresponse.Body.ErrorCode == ModernGroupActionError.MaxSubscriptionsForGroupReached)
			{
				throw new ExceededMaxSubscribersException(Strings.MaxSubscriptionsForGroupReached);
			}
			throw new InvalidOperationException(string.Format("Unknown error code: {0}", ewsresponse.Body.ErrorCode));
		}

		private void WriteSuccessfulResponse(HttpContext context)
		{
			string localizedString;
			string localizedString2;
			if (this.operationType == ModernGroupMembershipOperationType.Escalate)
			{
				localizedString = this.GetLocalizedString(ClientStrings.GroupSubscriptionPageSubscribed(this.encodedGroupDisplayName));
				localizedString2 = this.GetLocalizedString(ClientStrings.GroupSubscriptionPageSubscribedInfo);
			}
			else
			{
				localizedString = this.GetLocalizedString(ClientStrings.GroupSubscriptionPageUnsubscribed(this.encodedGroupDisplayName));
				localizedString2 = this.GetLocalizedString(ClientStrings.GroupSubscriptionPageUnsubscribedInfo);
			}
			string str = string.Format("<table cellspacing=\"0\"cellpadding=\"0\"border=\"0\" style=\"margin-top:24px; color:#333333;max-width:600px\"><tbody><tr><td style=\"font-size:42px;\">{0}</td></tr><tr><td style=\"font-size:17px;padding-top:30px\">{1}</td></tr></tbody></table>", localizedString, localizedString2);
			string mainBody = this.groupHeaderDiv + str;
			context.Response.Write(this.GetMainPage(mainBody));
		}

		private void WriteErrorResponse(HttpContext context, LocalizedString localizedErrorString)
		{
			string localizedString = this.GetLocalizedString(ClientStrings.GroupSubscriptionPageRequestFailed);
			string str = string.Format("<table cellspacing=\"0\"cellpadding=\"0\"border=\"0\" style=\"margin-top:24px; color:#333333;max-width:600px\"><tbody><tr><td style=\"font-size:42px;\">{0}</td></tr><tr><td style=\"font-size:17px;padding-top:30px\">{1}</td></tr></tbody></table>", localizedString, this.GetLocalizedString(localizedErrorString));
			context.Response.Write(this.GetMainPage(this.groupHeaderDiv + str));
		}

		private string GetLocalizedString(LocalizedString source)
		{
			return source.ToString(this.pageCultureInfo);
		}

		private static CallContext CreateAndSetCallContext(HttpContext httpContext)
		{
			HttpContext.Current = httpContext;
			CallContext result;
			using (Message message = Message.CreateMessage(MessageVersion.Default, string.Empty))
			{
				message.Properties.Add("WebMethodEntry", WebMethodEntry.JsonWebMethodEntry);
				result = CallContextUtilities.CreateAndSetCallContext(message, WorkloadType.Owa, false, "");
			}
			return result;
		}

		private static void DisposeContext(CallContext callContext)
		{
			CallContext.SetCurrent(null);
			if (callContext != null)
			{
				callContext.Dispose();
			}
		}

		private const string ActionParamenterName = "Action";

		private const string SubscribeActionName = "subscribe";

		private const string UnsubscribeActionName = "unsubscribe";

		private const string MainPageFormat = "<!DOCTYPE html><html><body style=\"margin:0px\"><div><font face={0}>{1}<div style=\"padding:50px\">{2}</div></font></div></body></html>";

		private const string OperationResultInfo = "<table cellspacing=\"0\"cellpadding=\"0\"border=\"0\" style=\"margin-top:24px; color:#333333;max-width:600px\"><tbody><tr><td style=\"font-size:42px;\">{0}</td></tr><tr><td style=\"font-size:17px;padding-top:30px\">{1}</td></tr></tbody></table>";

		private const string Office365HeaderFormat = "<div style=\"background-color:#0072C6;height:50px\"><img style=\"margin-left:15px; margin-top:11px\" src=\"{0}\"></div>";

		private const string GroupHeader = "<table cellspacing=\"0\"cellpadding=\"0\"border=\"0\" style=\"width:100%; max-width:600px\"><tbody><tr><td style=\"width:50px;height:50px;position:relative\"><div><img style=\"top: 0px; display: inline; width:50px\" src=\"{0}\"></div><div style=\"position:absolute;top:0px\"><img style=\"top: 0px; display: inline; width:50px\"src=\"service.svc/s/GetUserPhoto?email={1}&UA=0&size=HR64x64\"onerror=\"this.style.display='none'\"></div></td><td style=\"color:#333333;position:relative\">{2}</td></tr></tbody></table>";

		private const string GroupInfo = "<div style=\"font-size:21px;margin-left:19px;position:absolute;top:0px;left:0px;text-overflow: ellipsis;overflow:hidden;white-space:nowrap;max-width:95%\">{0}</div><div style=\"font-size:12px;color:#666666;margin-top:24px;margin-left:20px\">{1}</div>";

		private const string DoughboyImageBase64 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAGAAAABgCAYAAADimHc4AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAUpSURBVHhe7Zu9ctswEIT5/o+TmbyG0zlFZpQihYoUKlKoSMFwEUGGoQWIw99BInZmC8si8fMd7w6Ms3z98nWd1vMEoOwJQNkTgLInAGVPAMqeAJQ9ASh7AlD2BKDsCUDZE4CyJwBlTwDKngCUPQEoewJQ9gSg7AlA2ROAsicAZU8Ayp4AlD0BKHsCUPYEoOynAXD6eVovl8t6vV5XX/gMv8N32LUje3gA51/ndf172+lEXX5f6L1G9LAA3r+/izfe1zM8EUMCMFFfSdc/VzrGKB4OQM3NtxoZwlAATj9Oty2rr1EhDAWgNOfv6Xw+03E1PQwAdC499PbtjY6v5SQAmDSiJ9aDo2th16a6lzBfNr7UqFVIaw9P7fYzPsfv2XW+owCwqWzTg9oGRx5n94q5V/Rb5czRWjrXvTNJEEDJpkgLXuvc7yunICMLZM9zuy6UISgA82iVahuU3du3OXApiM0l5FpzZGnpAUCVzb8pJdJQWzSUmqPhmvJP558AtMjFe61fTeASpaYhUQ1MlNuJ3QGYHNdIdgzm3vn/roQU2eJUDrmd2B1AC9JWsU5AU2w+rlsGh02BBkCPQuguzLWm2Hysm+/J7Qk0AHrk4dCrYU3FzgM4XLYWIBsAPYQF+YvsNXZIMQA9ahP2ZOnVh4deAagV4U1RAD20rX1BaugltlCtNhQKAcDnvbS06P1DYovVOohBbD7woQC0PH9EdetCmLsC6JkC2GLhlmeQkGIn4ZcEECrCcKsTZ0x774J6qVsKCrWh1r27ITYH173ms/SKPrS7bKHWPR/72KsR6x4HMWjpUgQjBc91r3TIxvbd43yEtPz/JNz4cdvLt66bz0XwlxGtmwPMxQBomoYSo9+6ZeTFOh/m1mkR2ccAgFtFHhbhLirFTU7nwkCwbpUWbVNyB9CC9l7nE3PNJ0Ea+b6rywmGOwC4akuaGXG+SyNQkvNDrp0W3Y7wEwC4yiNXafOt8XRKC2LJ08dcC4K7+fADALikKNdeuGsULUS0CRK/Zm0/43NJxyW1geCPm6rtOn/zYQoAxmIlTwMiVFJwcX98H2kP1vjPFBhfMmdraaqOHfyCAKzvUUdSAD7DzRlZZtzLnDADUYT74Tvs2lrGXGlgbXPC3CTjI2hKn8YFi65RqPYseZpyojLFWGeKpE9zjm0wLrcxgzmq1FhIjmKPbY4lAWCFa9i9Su2msA8AN9UsYsUn7C0oSiPRBICfIoSqGZh+IDwAgGpEX/HmO8pJCfg+rqulUghIOSwQKACjgujDZJtomxPyJoqfvyFYIOYbK/KlcseT2E05vsIArLbFoHj5MGLdQs3IG0mpNcEEwxYkJhh2tA8gIjZ4zdQzokJZIVfZABDlbCKvGv1WoXXnpr1sAKxQm0JzALH0m5JumLIBsFcHqQedZxc7uOam3mwALApyDjvPKFaMURtylA3AnwB8GG35vtb6swCwQnSU/G/lrx/OKcRZAFgBRk04klg7mtMBZgFg74tip71XVK09yAJQi/4zi2WBnC4wC4A/MNzq/cuoqtUJ1QNwMLFGJOclpBwAacFye+Bnl78PsFRiAIz80TogK38fYKnkAEjuO1oHZFWjGREDYNX/sADI+7DmANiLqKO8A/JVIxjFAOYZ4EPDADjaGcCKNSTS19JiAP6A8FFFO0JhSz4BFMrfC+lhTAaAHMKa/QnKk8jfD1giEYAaj9yrif3LoEQTQKFKmxIRgBpt16uJAZC05RNAodie9AWQ+fcwr6KyoFzXf8jAL38D/RRdAAAAAElFTkSuQmCC";

		private const string Office365ImageBase64 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAGcAAAAdCAYAAAC3+HJeAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsIAAA7CARUoSoAAAAAhdEVYdENyZWF0aW9uIFRpbWUAMjAxNDowNjozMCAxNjowMDo0NBfIsOMAAAPcSURBVGhD7ZdPahRREIdzE0/iObyFC4+gR9AT6AHErbgT4kYXQjZmoxBQCCJBDBJEWr9hPinLX3d6zARa6YIf3a/qvfr/qmcODu4cDisWishcsQxE5oplIDJXLAOROQM37r0a7j//MByffh0evTwdbj18E/etuAIicwJ3n54Mr9+fD4k+fvk2PDs+2+xJZ1fsiMhs4FaQ9POL79syXE5Pjj5FXSt2QGT+BGOLcXVydrFN9+/ELXnx7vNmrCW6juLgE3rBzQdHGx5Peaz/q/GamHxLOlkMZCYGkJREJmsK6KkF5skNrfoFheHm6gdFAJ5j1LqeY3tXoBM7+MBzzAY+4At+0ti8K7v9+O3mbMdoQyVmTzhK0z7Q90qXJcgGIAj2CtYkoDvMLYYokjyKRALqPnSMBnsFkER8Jhc0EMSz7uFbC1EQ3gE+Kje+Gi9IzbhBYnKg0r6LYxDVcUHyCY4C9RtKgupeO6/yrgO1IYQFcu3Nnoo7xTCJxOwJ33dx6J4pJw3UzsS+t4R3wVqet4X33omsaQh8Ql5l2EK2698B43bNeXyuezo4s+jikACIEZHkgmJQRN7nkEFD1bYdThHZQwKNB19Y2yzQ3ORxu+te3p0EFDyNKuRz9W+QmNdZHL81l3Wpeuu6B8a68yBt+52qjWDivJ11tFoszskTnEGvfgB0KIdoBIomUXRupXs4g35l2J7MQ2KaGGmfxXF/klX0fax7IUxS5UHa7smvcAzVBIOx7iaJ8LmBJrbeDgh92LYBLJT7yKPAPno4M1qgxDQx0r94c9APjfnujw51CBJGAdIZQfIpTi0u1BsBmQWr/CrHHr4k+WK/OThck4S+XggTWnkQe/EZGvPdQrA3IZ2pILGQIxBK55KPFYzCUXliYqTSPosDSApIMmAB6+xHXw8iBQ6x1+SNNQFJmfJhDiDjpJH8dVkxxhdjY3SDxOwJ33dx7GoccywI5jMB9dGCvl2Ko5wCdBus9aEXD1n9loA0gmkcSFn6hmnD/HU/ODs19iaLQ3CXjR/3dho1uAV6cQxQJPZjD6IwPRDllce68yBtk2T01/HFfpOFXUj73iblAhk+qcMPPe91H3u0hS5jU15j5ckaf3qsv5CY/Pwjed3JBAwl6o4n4BQdR7A4SUD1p2cFvvRGYd152K1+VxuA95oMzpMo7aeYKTJ69ZP3dJu0xR50Jn+1leR/IDK3wNFaYZzDMZQi0+lE8JPOFTsgMhvoCApCcebSWpw9IDInwG3h6jJfK3G7HBv9g7riLxGZM8Hc5YakOb1iD4jMFctAZK5YBiJzxTIQmSsWgMPhB+HQgglctpnmAAAAAElFTkSuQmCC";

		private string groupHeaderDiv = string.Empty;

		private CultureInfo pageCultureInfo;

		private ModernGroupMembershipOperationType? operationType = null;

		private string encodedGroupDisplayName = string.Empty;
	}
}
