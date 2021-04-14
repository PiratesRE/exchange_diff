using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Net.ExSmtpClient;
using Microsoft.Exchange.Transport;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class XProxyToSmtpCommand : SmtpCommand
	{
		public XProxyToSmtpCommand(ISmtpSession session, ITransportConfiguration transportConfig, ITransportAppConfig transportAppConfig) : base(session, "XPROXYTO", null, LatencyComponent.None)
		{
			if (transportConfig == null)
			{
				throw new ArgumentNullException("transportConfig");
			}
			this.transportAppConfig = transportAppConfig;
			this.transportConfig = transportConfig;
		}

		public static bool TryGetInboundXProxyToResponse(int hashCode, SmtpOutProxySession session, int maxResponseLineLength, SmtpResponse ehloResponse, out SmtpResponse xProxyToResponse)
		{
			ExTraceGlobals.SmtpSendTracer.TraceDebug((long)hashCode, "Obtaining lines to prepend to the outbound EHLO response to obtain response for the inbound XPROXYTO command");
			xProxyToResponse = SmtpResponse.Empty;
			IEnumerable<INextHopServer> enumerable;
			bool flag = session.TryGetRemainingSmtpTargets(out enumerable);
			if (!flag)
			{
				ExTraceGlobals.SmtpSendTracer.TraceDebug((long)hashCode, "No remaining targets obtained");
			}
			List<string> list = new List<string>();
			list.Add(string.Format(CultureInfo.InvariantCulture, "{0}{1}", new object[]
			{
				"XPROXYTOTARGETIPADDRESS ",
				session.RemoteEndPoint.Address
			}));
			if (!string.IsNullOrEmpty(session.ProxyTargetHostName))
			{
				list.Add(string.Format(CultureInfo.InvariantCulture, "{0}{1}", new object[]
				{
					"XPROXYTOTARGETHOSTNAME ",
					session.ProxyTargetHostName
				}));
			}
			if (flag)
			{
				StringBuilder stringBuilder = null;
				foreach (INextHopServer nextHopServer in enumerable)
				{
					string text = XProxyToNextHopServer.ConvertINextHopServerToString(nextHopServer);
					if ("XPROXYTODESTINATIONS ".Length + 1 + text.Length > maxResponseLineLength)
					{
						ExTraceGlobals.SmtpSendTracer.TraceError<string, int>((long)hashCode, "Target {0} will not fit in a line even by itself since the total length will be more than {1}", text, maxResponseLineLength);
						return false;
					}
					if (stringBuilder == null)
					{
						stringBuilder = new StringBuilder("XPROXYTODESTINATIONS ");
						stringBuilder.AppendFormat(text, new object[0]);
					}
					else if (stringBuilder.Length + 1 + text.Length > maxResponseLineLength)
					{
						list.Add(stringBuilder.ToString());
						stringBuilder = new StringBuilder("XPROXYTODESTINATIONS ");
						stringBuilder.Append(text);
					}
					else
					{
						stringBuilder.Append(',');
						stringBuilder.Append(text);
					}
				}
				list.Add(stringBuilder.ToString());
				if (session.TlsConfiguration.ShouldSkipTls)
				{
					list.Add("XPROXYTOSHOULDSKIPTLS");
				}
			}
			string[] array = new string[list.Count + ehloResponse.StatusText.Length];
			int num = 0;
			foreach (string text2 in list)
			{
				array[num] = text2;
				num++;
			}
			foreach (string text3 in ehloResponse.StatusText)
			{
				array[num] = text3;
				num++;
			}
			xProxyToResponse = new SmtpResponse(ehloResponse.StatusCode, ehloResponse.EnhancedStatusCode, array);
			return true;
		}

		public static string FormatSessionIdString(ulong sessionId)
		{
			return sessionId.ToString("X16", NumberFormatInfo.InvariantInfo);
		}

		internal override void InboundParseCommand()
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.XProxyToInboundParseCommand);
			if (!base.VerifyEhloReceived())
			{
				smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.WrongSequence);
				return;
			}
			if (!smtpInSession.AdvertisedEhloOptions.XProxyTo)
			{
				smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.XProxyToNotEnabled);
				base.SmtpResponse = SmtpResponse.CommandNotImplemented;
				base.ParsingStatus = ParsingStatus.ProtocolError;
				return;
			}
			if (!SmtpInSessionUtils.HasSMTPAcceptXProxyToPermission(smtpInSession.RemoteIdentity, smtpInSession.AuthMethod) && !SmtpInSessionUtils.ShouldAcceptProxyToProtocol(this.transportConfig.ProcessTransportRole, smtpInSession.Capabilities))
			{
				smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.XProxyToNotAuthorized);
				base.SmtpResponse = SmtpResponse.NotAuthorized;
				base.ParsingStatus = ParsingStatus.ProtocolError;
				return;
			}
			if (!base.VerifyNoOngoingMailTransaction())
			{
				smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.WrongSequence);
				return;
			}
			this.parser = (smtpInSession.XProxyToParser ?? new XProxyToSmtpCommandParser(this.transportConfig));
			SmtpResponse smtpResponse;
			if (!this.parser.TryParseArguments(CommandContext.FromSmtpCommand(this), SmtpCommand.EventLog, smtpInSession.SmtpInServer.EventNotificationItem, out smtpResponse))
			{
				base.SmtpResponse = smtpResponse;
				base.ParsingStatus = ParsingStatus.ProtocolError;
				return;
			}
			base.ParsingStatus = ParsingStatus.Complete;
		}

		internal override void InboundProcessCommand()
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.XProxyToInboundProcessCommand);
			if (base.ParsingStatus != ParsingStatus.Complete)
			{
				return;
			}
			if (Components.SmtpInComponent.TargetRunningState == ServiceState.Inactive && !this.parser.IsProbeConnection)
			{
				base.SmtpResponse = SmtpResponse.ServiceInactive;
				smtpInSession.LogInformation(ProtocolLoggingLevel.Verbose, "Rejecting the non-probe session and disconnecting as transport service is Inactive.", null);
				smtpInSession.Disconnect(DisconnectReason.Local);
				return;
			}
			if (this.parser.LastCommandSeen)
			{
				SmtpSendConnectorConfig smtpSendConnectorConfig;
				TlsSendConfiguration outboundProxyTlsSendConfigurationParam;
				RiskLevel outboundProxyRiskLevelParam;
				int outboundProxyOutboundIPPoolParam;
				IEnumerable<INextHopServer> outboundProxyDestinationsParam;
				string outboundProxyNextHopDomainParam;
				string outboundProxySessionIdParam;
				this.parser.GetProxySettings(out smtpSendConnectorConfig, out outboundProxyTlsSendConfigurationParam, out outboundProxyRiskLevelParam, out outboundProxyOutboundIPPoolParam, out outboundProxyDestinationsParam, out outboundProxyNextHopDomainParam, out outboundProxySessionIdParam);
				if (this.transportAppConfig.SmtpOutboundProxyConfiguration.SendConnectorFqdn != null)
				{
					smtpSendConnectorConfig.Fqdn = this.transportAppConfig.SmtpOutboundProxyConfiguration.SendConnectorFqdn;
				}
				smtpInSession.SetupSessionToProxyTarget(smtpSendConnectorConfig, outboundProxyDestinationsParam, outboundProxyTlsSendConfigurationParam, outboundProxyRiskLevelParam, outboundProxyOutboundIPPoolParam, outboundProxyNextHopDomainParam, outboundProxySessionIdParam);
				return;
			}
			smtpInSession.XProxyToParser = this.parser;
			base.SmtpResponse = SmtpResponse.SuccessfulResponse;
		}

		internal override void OutboundCreateCommand()
		{
		}

		internal override void OutboundFormatCommand()
		{
			SmtpOutSession smtpOutSession = (SmtpOutSession)base.SmtpSession;
			this.remainingCommands = smtpOutSession.RemainingXProxyToCommands;
			if (this.remainingCommands == null || this.remainingCommands.Count == 0)
			{
				IEnumerable<INextHopServer> enumerable;
				SmtpSendConnectorConfig smtpSendConnectorConfig;
				TlsSendConfiguration tlsSendConfiguration;
				RiskLevel riskLevel;
				int outboundIPPool;
				smtpOutSession.GetOutboundProxyDestinationSettings(out enumerable, out smtpSendConnectorConfig, out tlsSendConfiguration, out riskLevel, out outboundIPPool);
				if (smtpSendConnectorConfig == null || tlsSendConfiguration == null || enumerable == null)
				{
					string message = string.Format(CultureInfo.InvariantCulture, "outboundProxySendConnector is {0}, outboundProxyTlsConfiguration is {1}, outboundProxyDestinations is {2}", new object[]
					{
						(smtpSendConnectorConfig == null) ? "<null>" : "set",
						(tlsSendConfiguration == null) ? "<null>" : "set",
						(enumerable == null) ? "<null>" : "set"
					});
					throw new InvalidOperationException(message);
				}
				string text;
				string text2;
				if (!XProxyToSmtpCommand.TryGetAllCommandLines(enumerable, tlsSendConfiguration, smtpSendConnectorConfig, smtpOutSession.SessionId, riskLevel, outboundIPPool, smtpOutSession.IsProbeSession, "XPROXYTO", smtpOutSession.NextHopDomain, XProxyToSmtpCommandParser.MaxCommandLength, out text, out this.remainingCommands, out text2))
				{
					ExTraceGlobals.SmtpSendTracer.TraceError<string>((long)this.GetHashCode(), "Outbound XPROXY commands could not be constructed due to {0}", text2);
					smtpOutSession.AckConnection(AckStatus.Retry, new SmtpResponse("451", "4.7.0", new string[]
					{
						text2
					}));
					smtpOutSession.NextState = SmtpOutSession.SessionState.Quit;
					return;
				}
				if (!string.IsNullOrEmpty(text))
				{
					smtpOutSession.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Instructing the next hop (outbound proxy frontend) to present the following certificate during TLS: {0}", new object[]
					{
						text
					});
				}
				ExTraceGlobals.SmtpSendTracer.TraceError<int>((long)this.GetHashCode(), "{0} command lines were obtained", this.remainingCommands.Count);
			}
			base.ProtocolCommandString = this.remainingCommands.Dequeue();
			smtpOutSession.RemainingXProxyToCommands = this.remainingCommands;
			ExTraceGlobals.SmtpSendTracer.TraceDebug<string>((long)this.GetHashCode(), "Formatted command : {0}", base.ProtocolCommandString);
		}

		internal override void OutboundProcessResponse()
		{
			SmtpOutSession smtpOutSession = (SmtpOutSession)base.SmtpSession;
			if (base.SmtpResponse.SmtpResponseType == SmtpResponseType.TransientError)
			{
				string text = string.Format("XPROXYTO failed with: {0}", base.SmtpResponse);
				ExTraceGlobals.SmtpSendTracer.TraceError((long)this.GetHashCode(), text);
				if (base.SmtpResponse.StatusText != null && base.SmtpResponse.StatusText.Length != 0 && string.Equals(base.SmtpResponse.StatusText[0], SmtpResponse.TransientInvalidArguments.StatusText[0], StringComparison.OrdinalIgnoreCase))
				{
					EventNotificationItem.PublishPeriodic(ExchangeComponent.Transport.Name, "XProxyToFailedWithTransientInvalidArgumentsResponse", null, text, "XProxyToFailedWithTransientInvalidArgumentsResponse", TimeSpan.FromMinutes(5.0), ResultSeverityLevel.Error, false);
					smtpOutSession.FailoverConnection(base.SmtpResponse);
				}
				else
				{
					smtpOutSession.AckConnection(AckStatus.Retry, base.SmtpResponse);
				}
				smtpOutSession.NextState = SmtpOutSession.SessionState.Quit;
				return;
			}
			if (base.SmtpResponse.SmtpResponseType == SmtpResponseType.PermanentError)
			{
				ExTraceGlobals.SmtpSendTracer.TraceError<SmtpResponse>((long)this.GetHashCode(), "XPROXYTO failed with: {0}", base.SmtpResponse);
				smtpOutSession.AckConnection(AckStatus.Fail, base.SmtpResponse);
				smtpOutSession.NextState = SmtpOutSession.SessionState.Quit;
				return;
			}
			if (this.remainingCommands.Count == 0)
			{
				IPAddress proxyTargetIPAddress;
				string proxyTargetHostName;
				IEnumerable<INextHopServer> remainingDestinations;
				bool shouldSkipTls;
				int precedingXProxyToSpecificLines;
				if (!XProxyToSmtpCommand.TryParseXProxyToResponse(base.SmtpResponse, out proxyTargetIPAddress, out proxyTargetHostName, out remainingDestinations, out shouldSkipTls, out precedingXProxyToSpecificLines))
				{
					remainingDestinations = new INextHopServer[0];
					shouldSkipTls = false;
				}
				smtpOutSession.OutboundProxyConnectionEstablished(base.SmtpResponse, proxyTargetIPAddress, proxyTargetHostName, remainingDestinations, shouldSkipTls, precedingXProxyToSpecificLines);
				smtpOutSession.PrepareToSendXshadowOrMessage();
			}
		}

		private static bool TryParseXProxyToResponse(SmtpResponse response, out IPAddress proxyTargetIPAddress, out string proxyTargetHostName, out IEnumerable<INextHopServer> remainingDestinations, out bool shouldSkipTls, out int precedingXproxyToSpecificLines)
		{
			proxyTargetIPAddress = IPAddress.None;
			proxyTargetHostName = string.Empty;
			remainingDestinations = new List<RoutingHost>(0);
			shouldSkipTls = false;
			precedingXproxyToSpecificLines = 0;
			if (response.StatusText == null)
			{
				return false;
			}
			int i;
			for (i = 0; i < response.StatusText.Length; i++)
			{
				if (response.StatusText[i].StartsWith("XPROXYTODESTINATIONS ", StringComparison.OrdinalIgnoreCase))
				{
					if (response.StatusText[i].Length <= "XPROXYTODESTINATIONS ".Length)
					{
						return false;
					}
					string text = response.StatusText[i].Substring("XPROXYTODESTINATIONS ".Length);
					text = text.Trim();
					string[] array = text.Split(new char[]
					{
						','
					});
					foreach (string address in array)
					{
						RoutingHost item;
						if (!RoutingHost.TryParse(address, out item))
						{
							return false;
						}
						((List<RoutingHost>)remainingDestinations).Add(item);
					}
				}
				else if (response.StatusText[i].StartsWith("XPROXYTOSHOULDSKIPTLS", StringComparison.OrdinalIgnoreCase))
				{
					shouldSkipTls = true;
				}
				else if (response.StatusText[i].StartsWith("XPROXYTOTARGETIPADDRESS ", StringComparison.OrdinalIgnoreCase))
				{
					string ipString = response.StatusText[i].Substring("XPROXYTOTARGETIPADDRESS ".Length);
					if (!IPAddress.TryParse(ipString, out proxyTargetIPAddress))
					{
						proxyTargetIPAddress = IPAddress.None;
						return false;
					}
				}
				else
				{
					if (!response.StatusText[i].StartsWith("XPROXYTOTARGETHOSTNAME ", StringComparison.OrdinalIgnoreCase))
					{
						precedingXproxyToSpecificLines = i;
						return true;
					}
					proxyTargetHostName = response.StatusText[i].Substring("XPROXYTOTARGETHOSTNAME ".Length);
				}
			}
			precedingXproxyToSpecificLines = i;
			return true;
		}

		private static bool TryGetAllCommandLines(IEnumerable<INextHopServer> proxyDestinations, TlsSendConfiguration tlsConfiguration, SmtpSendConnectorConfig sendConnector, ulong sessionId, RiskLevel riskLevel, int outboundIPPool, bool isProbeSession, string commandVerb, string nextHopDomain, int maxLength, out string certSubject, out Queue<string> commandLines, out string error)
		{
			certSubject = string.Empty;
			commandLines = new Queue<string>();
			StringBuilder stringBuilder = new StringBuilder(commandVerb);
			int num = maxLength - stringBuilder.Length;
			IList<string> list = new List<string>();
			foreach (INextHopServer nextHopServer in proxyDestinations)
			{
				string item = XProxyToNextHopServer.ConvertINextHopServerToString(nextHopServer);
				list.Add(item);
			}
			if (list.Count < 1)
			{
				error = "No destinations obtained";
				return false;
			}
			if (!XProxyToSmtpCommand.TryAddMultiValuePairToCommands("DESTINATIONS", list, commandLines, commandVerb, maxLength, ref stringBuilder, ref num, out error))
			{
				return false;
			}
			if (tlsConfiguration.TlsDomains != null && tlsConfiguration.TlsDomains.Count > 0)
			{
				IList<string> list2 = new List<string>();
				foreach (SmtpDomainWithSubdomains smtpDomainWithSubdomains in tlsConfiguration.TlsDomains)
				{
					string item2 = smtpDomainWithSubdomains.ToString();
					list2.Add(item2);
				}
				if (!XProxyToSmtpCommand.TryAddMultiValuePairToCommands("TLSDOMAINS", list2, commandLines, commandVerb, maxLength, ref stringBuilder, ref num, out error))
				{
					return false;
				}
			}
			if (!string.IsNullOrEmpty(tlsConfiguration.TlsCertificateFqdn) || tlsConfiguration.TlsCertificateName != null)
			{
				string text;
				if (tlsConfiguration.TlsCertificateName != null)
				{
					text = tlsConfiguration.TlsCertificateName.ToString();
				}
				else
				{
					text = tlsConfiguration.TlsCertificateFqdn;
				}
				certSubject = text;
				byte[] bytes = Encoding.UTF7.GetBytes(text);
				string input = Convert.ToBase64String(bytes);
				string value = SmtpUtils.ToXtextString(input, false);
				if (!XProxyToSmtpCommand.TryAddSingleValuePairToCommands("CERTSUBJECT", value, commandLines, commandVerb, maxLength, ref stringBuilder, ref num, out error))
				{
					return false;
				}
			}
			if (!XProxyToSmtpCommand.TryAddSingleValuePairToCommands("FORCEHELO", sendConnector.ForceHELO.ToString(CultureInfo.InvariantCulture), commandLines, commandVerb, maxLength, ref stringBuilder, ref num, out error))
			{
				return false;
			}
			if (!XProxyToSmtpCommand.TryAddSingleValuePairToCommands("SHOULDSKIPTLS", tlsConfiguration.ShouldSkipTls.ToString(CultureInfo.InvariantCulture), commandLines, commandVerb, maxLength, ref stringBuilder, ref num, out error))
			{
				return false;
			}
			if (!XProxyToSmtpCommand.TryAddSingleValuePairToCommands("PORT", sendConnector.Port.ToString(CultureInfo.InvariantCulture), commandLines, commandVerb, maxLength, ref stringBuilder, ref num, out error))
			{
				return false;
			}
			if (!XProxyToSmtpCommand.TryAddSingleValuePairToCommands("REQUIRETLS", tlsConfiguration.RequireTls.ToString(CultureInfo.InvariantCulture), commandLines, commandVerb, maxLength, ref stringBuilder, ref num, out error))
			{
				return false;
			}
			if (!XProxyToSmtpCommand.TryAddSingleValuePairToCommands("REQUIREOORG", sendConnector.RequireOorg.ToString(CultureInfo.InvariantCulture), commandLines, commandVerb, maxLength, ref stringBuilder, ref num, out error))
			{
				return false;
			}
			if (tlsConfiguration.TlsAuthLevel != null && !XProxyToSmtpCommand.TryAddSingleValuePairToCommands("TLSAUTHLEVEL", tlsConfiguration.TlsAuthLevel.ToString(), commandLines, commandVerb, maxLength, ref stringBuilder, ref num, out error))
			{
				return false;
			}
			if (!XProxyToSmtpCommand.TryAddSingleValuePairToCommands("RISK", riskLevel.ToString(), commandLines, commandVerb, maxLength, ref stringBuilder, ref num, out error))
			{
				return false;
			}
			if (!XProxyToSmtpCommand.TryAddSingleValuePairToCommands("OUTBOUNDIPPOOL", outboundIPPool.ToString(CultureInfo.InvariantCulture), commandLines, commandVerb, maxLength, ref stringBuilder, ref num, out error))
			{
				return false;
			}
			if (!XProxyToSmtpCommand.TryAddSingleValuePairToCommands("SESSIONID", XProxyToSmtpCommand.FormatSessionIdString(sessionId), commandLines, commandVerb, maxLength, ref stringBuilder, ref num, out error))
			{
				return false;
			}
			if (isProbeSession && !XProxyToSmtpCommand.TryAddSingleValuePairToCommands("PROBE", "1", commandLines, commandVerb, maxLength, ref stringBuilder, ref num, out error))
			{
				return false;
			}
			if (!string.IsNullOrEmpty(nextHopDomain))
			{
				nextHopDomain = nextHopDomain.Replace(" ", string.Empty);
				if (!XProxyToSmtpCommand.TryAddSingleValuePairToCommands("NEXTHOPDOMAIN", nextHopDomain, commandLines, commandVerb, maxLength, ref stringBuilder, ref num, out error))
				{
					return false;
				}
			}
			if (sendConnector.Fqdn != null && !XProxyToSmtpCommand.TryAddSingleValuePairToCommands("FQDN", sendConnector.Fqdn.ToString(), commandLines, commandVerb, maxLength, ref stringBuilder, ref num, out error))
			{
				return false;
			}
			if (sendConnector.ErrorPolicies != ErrorPolicies.Default && !XProxyToSmtpCommand.TryAddSingleValuePairToCommands("ERRORPOLICIES", SmtpUtils.ToXtextString(sendConnector.ErrorPolicies.ToString(), false), commandLines, commandVerb, maxLength, ref stringBuilder, ref num, out error))
			{
				return false;
			}
			if (sendConnector.DNSRoutingEnabled && !XProxyToSmtpCommand.TryAddSingleValuePairToCommands("DNSROUTING", SmtpUtils.ToXtextString(sendConnector.DNSRoutingEnabled.ToString(), false), commandLines, commandVerb, maxLength, ref stringBuilder, ref num, out error))
			{
				return false;
			}
			if (!XProxyToSmtpCommand.TryAddSingleValuePairToCommands("LAST", true.ToString(CultureInfo.InvariantCulture), commandLines, commandVerb, maxLength, ref stringBuilder, ref num, out error))
			{
				return false;
			}
			commandLines.Enqueue(stringBuilder.ToString());
			return true;
		}

		private static bool TryAddSingleValuePairToCommands(string key, string value, Queue<string> commandLines, string commandVerb, int maxLength, ref StringBuilder currentCommand, ref int remainingLength, out string error)
		{
			error = string.Empty;
			int num = 2 + key.Length + value.Length;
			if (num + commandVerb.Length > maxLength)
			{
				error = string.Format(CultureInfo.InvariantCulture, "Total length of one key value pair would exceed entire command length limit, key = '{0}', value = '{1}'", new object[]
				{
					key,
					value
				});
				return false;
			}
			if (num > remainingLength)
			{
				XProxyToSmtpCommand.StoreCurrentCommandAndCreateNew(commandLines, commandVerb, maxLength, ref currentCommand, ref remainingLength);
			}
			currentCommand.AppendFormat(CultureInfo.InvariantCulture, " {0}={1}", new object[]
			{
				key,
				value
			});
			remainingLength -= num;
			return true;
		}

		private static bool TryAddMultiValuePairToCommands(string key, IList<string> multiValues, Queue<string> commandLines, string commandVerb, int maxLength, ref StringBuilder currentCommand, ref int remainingLength, out string error)
		{
			error = string.Empty;
			int num = key.Length + 2;
			if (num > maxLength)
			{
				error = string.Format(CultureInfo.InvariantCulture, "Length of key would exceed entire command length limit, key = '{0}'", new object[]
				{
					key
				});
				return false;
			}
			bool flag = false;
			foreach (string text in multiValues)
			{
				if (commandVerb.Length + num + text.Length > maxLength)
				{
					error = string.Format(CultureInfo.InvariantCulture, "Total length of one key value pair for this value would exceed entire command length limit, key = '{0}', value = '{1}'", new object[]
					{
						key,
						text
					});
					return false;
				}
				if (!flag)
				{
					if (commandVerb.Length + num + text.Length > remainingLength)
					{
						XProxyToSmtpCommand.StoreCurrentCommandAndCreateNew(commandLines, commandVerb, maxLength, ref currentCommand, ref remainingLength);
					}
					currentCommand.AppendFormat(CultureInfo.InvariantCulture, " {0}={1}", new object[]
					{
						key,
						text
					});
					remainingLength -= num + text.Length;
					flag = true;
				}
				else if (1 + text.Length > remainingLength)
				{
					XProxyToSmtpCommand.StoreCurrentCommandAndCreateNew(commandLines, commandVerb, maxLength, ref currentCommand, ref remainingLength);
					currentCommand.AppendFormat(CultureInfo.InvariantCulture, " {0}={1}", new object[]
					{
						key,
						text
					});
					remainingLength -= num + text.Length;
				}
				else
				{
					currentCommand.Append(',');
					currentCommand.Append(text);
					remainingLength -= text.Length + 1;
				}
			}
			return true;
		}

		private static void StoreCurrentCommandAndCreateNew(Queue<string> commandLines, string commandVerb, int maxLength, ref StringBuilder currentCommand, ref int remainingLength)
		{
			if (currentCommand.Length > maxLength)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "The length of the command is {0} and exceeds the limit {1}. Command = {2}", new object[]
				{
					currentCommand.Length,
					maxLength,
					currentCommand
				}));
			}
			commandLines.Enqueue(currentCommand.ToString());
			currentCommand = new StringBuilder(commandVerb);
			remainingLength = maxLength - currentCommand.Length;
		}

		private const string XProxyToDestinationsKeywordWithSpace = "XPROXYTODESTINATIONS ";

		private const string ProxyTargetIPAddressKeywordWithSpace = "XPROXYTOTARGETIPADDRESS ";

		private const string ProxyTargetHostNameKeywordWithSpace = "XPROXYTOTARGETHOSTNAME ";

		private const string XProxyToShouldSkipTlsKeyword = "XPROXYTOSHOULDSKIPTLS";

		private const string EventNotificationOnTransientInvalidArgumentsString = "XProxyToFailedWithTransientInvalidArgumentsResponse";

		private readonly ITransportConfiguration transportConfig;

		private readonly ITransportAppConfig transportAppConfig;

		private Queue<string> remainingCommands;

		private XProxyToSmtpCommandParser parser;
	}
}
