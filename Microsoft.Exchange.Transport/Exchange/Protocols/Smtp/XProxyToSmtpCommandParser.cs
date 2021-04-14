using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.ExSmtpClient;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Categorizer;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class XProxyToSmtpCommandParser
	{
		public XProxyToSmtpCommandParser(ITransportConfiguration transportConfiguration)
		{
			this.sendConnector = new ProxySendConnector("Outbound proxy virtual send connector", transportConfiguration.LocalServer.TransportServer, transportConfiguration.LocalServer.TransportServer.HomeRoutingGroup, null)
			{
				UseExternalDNSServersEnabled = true
			};
			this.tlsConfiguration = new TlsSendConfiguration();
		}

		public bool LastCommandSeen
		{
			get
			{
				return this.lastCommandSeen;
			}
		}

		public bool IsProbeConnection
		{
			get
			{
				return this.isProbeConnection;
			}
		}

		public void GetProxySettings(out SmtpSendConnectorConfig theProxyConnector, out TlsSendConfiguration theTlsConfiguration, out RiskLevel theRiskLevel, out int theOutboundIPPool, out IEnumerable<INextHopServer> theProxyDestinations, out string theNextHopDomain, out string theSessionId)
		{
			if (!this.lastCommandSeen)
			{
				throw new InvalidOperationException("Proxy settings cannot be obtained before the last command has been received");
			}
			if (this.proxyDestinations == null)
			{
				throw new InvalidOperationException("Proxy settings cannot be obtained before destinations have been received");
			}
			theProxyConnector = this.sendConnector;
			theTlsConfiguration = this.tlsConfiguration;
			theRiskLevel = this.riskLevel;
			theOutboundIPPool = this.outboundIPPool;
			theProxyDestinations = this.proxyDestinations;
			theNextHopDomain = this.nextHopDomain;
			theSessionId = this.sessionId;
		}

		public bool TryParseArguments(CommandContext commandContext, IExEventLog eventLogger, IEventNotificationItem eventNotificationItem, out SmtpResponse smtpResponse)
		{
			if (this.lastCommandSeen)
			{
				throw new InvalidOperationException("Cannot parse further commands after last command");
			}
			XProxyToParseOutput parseOutput;
			if (!XProxyToSmtpCommandParser.TryParseArguments(commandContext, eventLogger, eventNotificationItem, out parseOutput))
			{
				smtpResponse = SmtpResponse.TransientInvalidArguments;
				return false;
			}
			this.CopyParsedValuesToSavedState(parseOutput);
			if (this.lastCommandSeen && (this.proxyDestinations == null || this.proxyDestinations.Count == 0))
			{
				smtpResponse = SmtpResponse.NoDestinationsReceivedResponse;
				return false;
			}
			smtpResponse = SmtpResponse.Empty;
			return true;
		}

		public static bool TryParseArguments(CommandContext commandContext, IExEventLog eventLogger, IEventNotificationItem eventNotificationItem, out XProxyToParseOutput parseOutput)
		{
			ArgumentValidator.ThrowIfNull("commandContext", commandContext);
			ArgumentValidator.ThrowIfNull("eventLogger", eventLogger);
			ArgumentValidator.ThrowIfNull("eventNotificationItem", eventNotificationItem);
			commandContext.TrimLeadingWhitespace();
			if (commandContext.IsEndOfCommand)
			{
				parseOutput = null;
				return false;
			}
			parseOutput = new XProxyToParseOutput();
			while (!commandContext.IsEndOfCommand)
			{
				Offset offset;
				if (!commandContext.GetNextArgumentOffset(out offset))
				{
					parseOutput = null;
					return false;
				}
				int nameValuePairSeparatorIndex = CommandParsingHelper.GetNameValuePairSeparatorIndex(commandContext.Command, offset, 61);
				if (nameValuePairSeparatorIndex <= 0 || nameValuePairSeparatorIndex >= offset.End - 1)
				{
					parseOutput = null;
					return false;
				}
				int count = nameValuePairSeparatorIndex - offset.Start;
				int argValueLen = offset.End - (nameValuePairSeparatorIndex + 1);
				bool flag = false;
				switch (Util.LowerC[(int)commandContext.Command[offset.Start]])
				{
				case 99:
					if (BufferParser.CompareArg(XProxyToSmtpCommandParser.CertificateSubjectKeywordBytes, commandContext.Command, offset.Start, count))
					{
						string decodedCertificateSubject;
						if (!XProxyToSmtpCommandParser.TryGetCertificateSubject(commandContext, nameValuePairSeparatorIndex, argValueLen, eventLogger, eventNotificationItem, out decodedCertificateSubject))
						{
							flag = true;
						}
						else
						{
							parseOutput.DecodedCertificateSubject = decodedCertificateSubject;
						}
					}
					break;
				case 100:
					if (BufferParser.CompareArg(XProxyToSmtpCommandParser.DestinationsKeywordBytes, commandContext.Command, offset.Start, count))
					{
						List<XProxyToNextHopServer> collection;
						if (!XProxyToSmtpCommandParser.GetMultiValuedParameter<XProxyToNextHopServer>(commandContext, nameValuePairSeparatorIndex, argValueLen, new XProxyToSmtpCommandParser.TryParseParameter<XProxyToNextHopServer>(XProxyToNextHopServer.TryParse), out collection))
						{
							flag = true;
						}
						else if (parseOutput.Destinations == null)
						{
							parseOutput.Destinations = new List<INextHopServer>(collection);
						}
						else
						{
							parseOutput.Destinations.AddRange(collection);
						}
					}
					else if (BufferParser.CompareArg(XProxyToSmtpCommandParser.DnsRoutingEnabledKeywordBytes, commandContext.Command, offset.Start, count))
					{
						parseOutput.IsDnsRoutingEnabled = XProxyToSmtpCommandParser.IsBooleanParameterEnabled(commandContext, nameValuePairSeparatorIndex, argValueLen);
					}
					break;
				case 101:
					if (BufferParser.CompareArg(XProxyToSmtpCommandParser.ErrorPoliciesKeywordBytes, commandContext.Command, offset.Start, count))
					{
						ErrorPolicies? errorPolicies;
						if (!XProxyToSmtpCommandParser.GetEnumValue<ErrorPolicies>(commandContext, nameValuePairSeparatorIndex, argValueLen, out errorPolicies))
						{
							flag = true;
						}
						else
						{
							parseOutput.ErrorPolicies = errorPolicies;
						}
					}
					break;
				case 102:
					if (BufferParser.CompareArg(XProxyToSmtpCommandParser.ForceHeloKeywordBytes, commandContext.Command, offset.Start, count))
					{
						parseOutput.ForceHelo = new bool?(XProxyToSmtpCommandParser.IsBooleanParameterEnabled(commandContext, nameValuePairSeparatorIndex, argValueLen));
					}
					else if (BufferParser.CompareArg(XProxyToSmtpCommandParser.FqdnKeywordBytes, commandContext.Command, offset.Start, count))
					{
						Fqdn fqdn;
						if (!XProxyToSmtpCommandParser.GetFqdn(commandContext, nameValuePairSeparatorIndex, argValueLen, out fqdn))
						{
							flag = true;
						}
						else
						{
							parseOutput.Fqdn = fqdn;
						}
					}
					break;
				case 108:
					if (BufferParser.CompareArg(XProxyToSmtpCommandParser.LastKeywordBytes, commandContext.Command, offset.Start, count))
					{
						parseOutput.IsLast = new bool?(XProxyToSmtpCommandParser.IsBooleanParameterEnabled(commandContext, nameValuePairSeparatorIndex, argValueLen));
					}
					break;
				case 110:
					if (BufferParser.CompareArg(XProxyToSmtpCommandParser.NextHopDomainKeywordBytes, commandContext.Command, offset.Start, count))
					{
						string text;
						if (!XProxyToSmtpCommandParser.GetStringValue(commandContext, nameValuePairSeparatorIndex, argValueLen, 2147483647, out text))
						{
							flag = true;
						}
						else
						{
							parseOutput.NextHopDomain = text;
						}
					}
					break;
				case 111:
					if (BufferParser.CompareArg(XProxyToSmtpCommandParser.OutboundIPPoolKeywordBytes, commandContext.Command, offset.Start, count))
					{
						int? num;
						if (!XProxyToSmtpCommandParser.GetIntegerValue(commandContext, nameValuePairSeparatorIndex, argValueLen, 0, 65535, 2147483647, out num))
						{
							flag = true;
						}
						else
						{
							parseOutput.OutboundIPPool = num;
						}
					}
					break;
				case 112:
					if (BufferParser.CompareArg(XProxyToSmtpCommandParser.PortKeywordBytes, commandContext.Command, offset.Start, count))
					{
						int? port;
						if (!XProxyToSmtpCommandParser.GetIntegerValue(commandContext, nameValuePairSeparatorIndex, argValueLen, 0, 65535, XProxyParserUtils.MaxClientPortLength, out port))
						{
							flag = true;
						}
						else
						{
							parseOutput.Port = port;
						}
					}
					else if (BufferParser.CompareArg(XProxyToSmtpCommandParser.ProbeKeywordBytes, commandContext.Command, offset.Start, count))
					{
						string a;
						if (!XProxyToSmtpCommandParser.GetStringValue(commandContext, nameValuePairSeparatorIndex, argValueLen, 1, out a))
						{
							flag = true;
						}
						else if (a == "1")
						{
							parseOutput.IsProbeConnection = true;
						}
					}
					break;
				case 114:
					if (BufferParser.CompareArg(XProxyToSmtpCommandParser.RequireTlsKeywordBytes, commandContext.Command, offset.Start, count))
					{
						parseOutput.RequireTls = new bool?(XProxyToSmtpCommandParser.IsBooleanParameterEnabled(commandContext, nameValuePairSeparatorIndex, argValueLen));
					}
					else if (BufferParser.CompareArg(XProxyToSmtpCommandParser.RequireOorgKeywordBytes, commandContext.Command, offset.Start, count))
					{
						parseOutput.RequireOorg = new bool?(XProxyToSmtpCommandParser.IsBooleanParameterEnabled(commandContext, nameValuePairSeparatorIndex, argValueLen));
					}
					else if (BufferParser.CompareArg(XProxyToSmtpCommandParser.RiskLevelKeywordBytes, commandContext.Command, offset.Start, count))
					{
						RiskLevel? risk;
						if (!XProxyToSmtpCommandParser.GetEnumValue<RiskLevel>(commandContext, nameValuePairSeparatorIndex, argValueLen, out risk))
						{
							flag = true;
						}
						else
						{
							parseOutput.Risk = risk;
						}
					}
					break;
				case 115:
					if (BufferParser.CompareArg(XProxyToSmtpCommandParser.ShouldSkipTlsKeywordBytes, commandContext.Command, offset.Start, count))
					{
						parseOutput.ShouldSkipTls = new bool?(XProxyToSmtpCommandParser.IsBooleanParameterEnabled(commandContext, nameValuePairSeparatorIndex, argValueLen));
					}
					else if (BufferParser.CompareArg(XProxyToSmtpCommandParser.SessionIdKeywordBytes, commandContext.Command, offset.Start, count))
					{
						string text2;
						if (!XProxyToSmtpCommandParser.GetStringValue(commandContext, nameValuePairSeparatorIndex, argValueLen, XProxyParserUtils.MaxSessionIdLength, out text2))
						{
							flag = true;
						}
						else
						{
							parseOutput.SessionId = text2;
						}
					}
					break;
				case 116:
					if (BufferParser.CompareArg(XProxyToSmtpCommandParser.TlsAuthLevelKeywordBytes, commandContext.Command, offset.Start, count))
					{
						RequiredTlsAuthLevel? tlsAuthLevel;
						if (!XProxyToSmtpCommandParser.GetEnumValue<RequiredTlsAuthLevel>(commandContext, nameValuePairSeparatorIndex, argValueLen, out tlsAuthLevel))
						{
							flag = true;
						}
						else
						{
							parseOutput.TlsAuthLevel = tlsAuthLevel;
						}
					}
					else if (BufferParser.CompareArg(XProxyToSmtpCommandParser.TlsDomainsKeywordBytes, commandContext.Command, offset.Start, count))
					{
						List<SmtpDomainWithSubdomains> collection2;
						if (!XProxyToSmtpCommandParser.GetMultiValuedParameter<SmtpDomainWithSubdomains>(commandContext, nameValuePairSeparatorIndex, argValueLen, new XProxyToSmtpCommandParser.TryParseParameter<SmtpDomainWithSubdomains>(SmtpDomainWithSubdomains.TryParse), out collection2))
						{
							flag = true;
						}
						else if (parseOutput.TlsDomains == null)
						{
							parseOutput.TlsDomains = new List<SmtpDomainWithSubdomains>(collection2);
						}
						else
						{
							parseOutput.TlsDomains.AddRange(collection2);
						}
					}
					break;
				}
				if (flag)
				{
					parseOutput = null;
					return false;
				}
				commandContext.TrimLeadingWhitespace();
			}
			return true;
		}

		private void CopyParsedValuesToSavedState(XProxyToParseOutput parseOutput)
		{
			if (parseOutput.Destinations != null && parseOutput.Destinations.Count > 0)
			{
				if (this.proxyDestinations == null)
				{
					this.proxyDestinations = new List<INextHopServer>(parseOutput.Destinations.Count);
				}
				foreach (INextHopServer item in parseOutput.Destinations)
				{
					this.proxyDestinations.Add(item);
				}
			}
			if (parseOutput.ForceHelo != null)
			{
				this.sendConnector.ForceHELO = parseOutput.ForceHelo.Value;
			}
			if (parseOutput.IsLast != null)
			{
				this.lastCommandSeen = parseOutput.IsLast.Value;
			}
			if (parseOutput.Port != null)
			{
				this.sendConnector.Port = parseOutput.Port.Value;
			}
			if (parseOutput.Fqdn != null)
			{
				this.sendConnector.Fqdn = parseOutput.Fqdn;
			}
			if (parseOutput.RequireTls != null)
			{
				this.tlsConfiguration.RequireTls = parseOutput.RequireTls.Value;
			}
			if (parseOutput.RequireOorg != null)
			{
				this.sendConnector.RequireOorg = parseOutput.RequireOorg.Value;
			}
			if (parseOutput.Risk != null)
			{
				this.riskLevel = parseOutput.Risk.Value;
			}
			if (parseOutput.OutboundIPPool != null)
			{
				this.outboundIPPool = parseOutput.OutboundIPPool.Value;
			}
			if (parseOutput.ShouldSkipTls != null)
			{
				this.tlsConfiguration.ShouldSkipTls = parseOutput.ShouldSkipTls.Value;
			}
			if (parseOutput.TlsAuthLevel != null)
			{
				this.tlsConfiguration.TlsAuthLevel = new RequiredTlsAuthLevel?(parseOutput.TlsAuthLevel.Value);
			}
			if (parseOutput.TlsDomains != null && parseOutput.TlsDomains.Count > 0)
			{
				if (this.tlsConfiguration.TlsDomains == null)
				{
					this.tlsConfiguration.TlsDomains = new List<SmtpDomainWithSubdomains>(parseOutput.TlsDomains.Count);
				}
				foreach (SmtpDomainWithSubdomains item2 in parseOutput.TlsDomains)
				{
					this.tlsConfiguration.TlsDomains.Add(item2);
				}
			}
			if (!string.IsNullOrEmpty(parseOutput.DecodedCertificateSubject))
			{
				SmtpX509Identifier tlsCertificateName;
				if (SmtpX509Identifier.TryParse(parseOutput.DecodedCertificateSubject, out tlsCertificateName))
				{
					this.tlsConfiguration.TlsCertificateName = tlsCertificateName;
				}
				else
				{
					this.tlsConfiguration.TlsCertificateFqdn = parseOutput.DecodedCertificateSubject;
				}
			}
			if (parseOutput.ErrorPolicies != null)
			{
				this.sendConnector.ErrorPolicies = parseOutput.ErrorPolicies.Value;
			}
			this.sendConnector.DNSRoutingEnabled = parseOutput.IsDnsRoutingEnabled;
			this.isProbeConnection = parseOutput.IsProbeConnection;
			this.nextHopDomain = parseOutput.NextHopDomain;
			this.sessionId = parseOutput.SessionId;
		}

		private static bool TryGetCertificateSubject(CommandContext commandContext, int separatorIndex, int argValueLen, IExEventLog eventLogger, IEventNotificationItem eventNotificationItem, out string decodedCertificate)
		{
			string text = SmtpUtils.FromXtextString(commandContext.Command, separatorIndex + 1, argValueLen, false);
			if (string.IsNullOrEmpty(text))
			{
				decodedCertificate = null;
				return false;
			}
			try
			{
				byte[] bytes = Convert.FromBase64String(text);
				decodedCertificate = Encoding.UTF7.GetString(bytes);
			}
			catch (FormatException ex)
			{
				eventLogger.LogEvent(TransportEventLogConstants.Tuple_XProxyToCommandInvalidEncodedCertificateSubject, null, new object[]
				{
					text,
					ex.ToString()
				});
				string notificationReason = string.Format("XProxyTo command expected a based64 encoded certificate subject, but the input certificate subject is not a valid one: {0}.  Exception: {1}", text, ex);
				eventNotificationItem.Publish(ExchangeComponent.Transport.Name, "XProxyToCommandInvalidEncodedCertificateSubject", null, notificationReason, ResultSeverityLevel.Error, false);
				decodedCertificate = null;
				return false;
			}
			return true;
		}

		private static bool GetMultiValuedParameter<T>(CommandContext commandContext, int separatorIndex, int argValueLen, XProxyToSmtpCommandParser.TryParseParameter<T> tryParse, out List<T> parsedValues)
		{
			string text = SmtpUtils.FromXtextString(commandContext.Command, separatorIndex + 1, argValueLen, false);
			if (string.IsNullOrEmpty(text))
			{
				parsedValues = null;
				return false;
			}
			string[] array = text.Split(new char[]
			{
				','
			});
			parsedValues = new List<T>(array.Length);
			bool flag = false;
			foreach (string value in array)
			{
				T item;
				if (!tryParse(value, out item))
				{
					flag = true;
					break;
				}
				parsedValues.Add(item);
			}
			if (flag)
			{
				parsedValues = null;
				return false;
			}
			return true;
		}

		private static bool IsBooleanParameterEnabled(CommandContext commandContext, int separatorIndex, int argValueLen)
		{
			string value = SmtpUtils.FromXtextString(commandContext.Command, separatorIndex + 1, argValueLen, false);
			bool flag;
			return !string.IsNullOrEmpty(value) && bool.TryParse(value, out flag) && flag;
		}

		private static bool GetEnumValue<T>(CommandContext commandContext, int separatorIndex, int argValueLen, out T? parsedValue) where T : struct
		{
			string value = SmtpUtils.FromXtextString(commandContext.Command, separatorIndex + 1, argValueLen, false);
			if (string.IsNullOrEmpty(value))
			{
				parsedValue = null;
				return false;
			}
			T value2;
			if (Enum.TryParse<T>(value, out value2))
			{
				parsedValue = new T?(value2);
				return true;
			}
			parsedValue = null;
			return true;
		}

		private static bool GetIntegerValue(CommandContext commandContext, int separatorIndex, int argValueLen, int minValue, int maxValue, int maxValueLength, out int? parsedValue)
		{
			if (argValueLen > maxValueLength)
			{
				parsedValue = null;
				return false;
			}
			string text = SmtpUtils.FromXtextString(commandContext.Command, separatorIndex + 1, argValueLen, false);
			if (string.IsNullOrEmpty(text))
			{
				parsedValue = null;
				return false;
			}
			int num;
			if (!int.TryParse(text, out num) || num < minValue || num > maxValue)
			{
				parsedValue = null;
				return false;
			}
			parsedValue = new int?(num);
			return true;
		}

		private static bool GetStringValue(CommandContext commandContext, int separatorIndex, int argValueLen, int maxValueLength, out string parsedValue)
		{
			if (argValueLen > maxValueLength)
			{
				parsedValue = null;
				return false;
			}
			parsedValue = SmtpUtils.FromXtextString(commandContext.Command, separatorIndex + 1, argValueLen, false);
			return !string.IsNullOrEmpty(parsedValue);
		}

		private static bool GetFqdn(CommandContext commandContext, int separatorIndex, int argValueLen, out Fqdn parsedValue)
		{
			string text = SmtpUtils.FromXtextString(commandContext.Command, separatorIndex + 1, argValueLen, false);
			if (string.IsNullOrEmpty(text))
			{
				parsedValue = null;
				return false;
			}
			return Fqdn.TryParse(text, out parsedValue);
		}

		public const char MultiValueDelimiter = ',';

		public const string CommandKeyword = "XPROXYTO";

		public const string LastKeyword = "LAST";

		public const string ForceHeloKeyword = "FORCEHELO";

		public const string ShouldSkipTlsKeyword = "SHOULDSKIPTLS";

		public const string PortKeyword = "PORT";

		public const string ProbeKeyword = "PROBE";

		public const string RequireTlsKeyword = "REQUIRETLS";

		public const string RequireOorgKeyword = "REQUIREOORG";

		public const string TlsAuthLevelKeyword = "TLSAUTHLEVEL";

		public const string TlsDomainsKeyword = "TLSDOMAINS";

		public const string DestinationsKeyword = "DESTINATIONS";

		public const string RiskLevelKeyword = "RISK";

		public const string OutboundIPPoolKeyword = "OUTBOUNDIPPOOL";

		public const string SessionIdKeyword = "SESSIONID";

		public const string CertificateSubjectKeyword = "CERTSUBJECT";

		public const string NextHopDomainKeyword = "NEXTHOPDOMAIN";

		public const string FqdnKeyword = "FQDN";

		public const string ErrorPoliciesKeyword = "ERRORPOLICIES";

		public const string DnsRoutingEnabledKeyword = "DNSROUTING";

		public const string ProbeKeywordProbeConnectionValue = "1";

		private const string VirtualConnectorName = "Outbound proxy virtual send connector";

		private const int MaxProbeKeywordValueLength = 1;

		public static readonly int MaxCommandLength = 2000;

		private static readonly byte[] LastKeywordBytes = Util.AsciiStringToBytes("LAST".ToLower());

		private static readonly byte[] ForceHeloKeywordBytes = Util.AsciiStringToBytes("FORCEHELO".ToLower());

		private static readonly byte[] ShouldSkipTlsKeywordBytes = Util.AsciiStringToBytes("SHOULDSKIPTLS".ToLower());

		private static readonly byte[] PortKeywordBytes = Util.AsciiStringToBytes("PORT".ToLower());

		private static readonly byte[] ProbeKeywordBytes = Util.AsciiStringToBytes("PROBE".ToLower());

		private static readonly byte[] RequireTlsKeywordBytes = Util.AsciiStringToBytes("REQUIRETLS".ToLower());

		private static readonly byte[] RequireOorgKeywordBytes = Util.AsciiStringToBytes("REQUIREOORG".ToLower());

		private static readonly byte[] TlsAuthLevelKeywordBytes = Util.AsciiStringToBytes("TLSAUTHLEVEL".ToLower());

		private static readonly byte[] TlsDomainsKeywordBytes = Util.AsciiStringToBytes("TLSDOMAINS".ToLower());

		private static readonly byte[] DestinationsKeywordBytes = Util.AsciiStringToBytes("DESTINATIONS".ToLower());

		private static readonly byte[] RiskLevelKeywordBytes = Util.AsciiStringToBytes("RISK".ToLower());

		private static readonly byte[] OutboundIPPoolKeywordBytes = Util.AsciiStringToBytes("OUTBOUNDIPPOOL".ToLower());

		private static readonly byte[] SessionIdKeywordBytes = Util.AsciiStringToBytes("SESSIONID".ToLower());

		private static readonly byte[] CertificateSubjectKeywordBytes = Util.AsciiStringToBytes("CERTSUBJECT".ToLower());

		private static readonly byte[] NextHopDomainKeywordBytes = Util.AsciiStringToBytes("NEXTHOPDOMAIN".ToLower());

		private static readonly byte[] FqdnKeywordBytes = Util.AsciiStringToBytes("FQDN".ToLower());

		private static readonly byte[] ErrorPoliciesKeywordBytes = Util.AsciiStringToBytes("ERRORPOLICIES".ToLower());

		private static readonly byte[] DnsRoutingEnabledKeywordBytes = Util.AsciiStringToBytes("DNSROUTING".ToLower());

		private readonly SmtpSendConnectorConfig sendConnector;

		private readonly TlsSendConfiguration tlsConfiguration;

		private RiskLevel riskLevel;

		private int outboundIPPool;

		private List<INextHopServer> proxyDestinations;

		private string nextHopDomain;

		private string sessionId;

		private bool lastCommandSeen;

		private bool isProbeConnection;

		private delegate bool TryParseParameter<T>(string value, out T parsedValues);
	}
}
