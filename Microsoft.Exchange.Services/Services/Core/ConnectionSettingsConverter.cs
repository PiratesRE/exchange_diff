using System;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Connections.Imap;
using Microsoft.Exchange.Connections.Pop;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.ConnectionSettingsDiscovery.Connections;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Management.RecipientTasks;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal abstract class ConnectionSettingsConverter
	{
		public static void UpdateNewSyncRequestCmdlet(Microsoft.Exchange.Data.SmtpAddress email, ConnectionSettings connectionSettings, NewSyncRequest newSyncRequest)
		{
			ConnectionSettingsConverter.GetConversionHelper(connectionSettings).UpdateNewSyncRequestCmdlet((string)email, connectionSettings.IncomingConnectionSettings, connectionSettings.OutgoingConnectionSettings, newSyncRequest);
		}

		public static void UpdateSetSyncRequestCmdlet(ConnectionSettings connectionSettings, SetSyncRequest setSyncRequest)
		{
			ConnectionSettingsConverter.GetConversionHelper(connectionSettings).UpdateSetSyncRequestCmdlet(connectionSettings.IncomingConnectionSettings, connectionSettings.OutgoingConnectionSettings, setSyncRequest);
		}

		public static ConnectionSettingsInfo BuildPublicRepresentation(ConnectionSettings connectionSettings)
		{
			return ConnectionSettingsConverter.GetConversionHelper(connectionSettings).BuildPublicRepresentation(connectionSettings.IncomingConnectionSettings, connectionSettings.OutgoingConnectionSettings);
		}

		public static ConnectionSettings BuildUpdateConnectionSettings(Fqdn incomingServer, int? incomingPort, string security, string authentication, ConnectionSettings originalSettings)
		{
			return ConnectionSettingsConverter.GetConversionHelper(originalSettings).BuildUpdateConnectionSettings(incomingServer, incomingPort, security, authentication, originalSettings.IncomingConnectionSettings, originalSettings.OutgoingConnectionSettings);
		}

		public static ConnectionSettings BuildConnectionSettings(AddAggregatedAccountRequest addAccountRequest)
		{
			if (string.IsNullOrEmpty(addAccountRequest.IncomingProtocol))
			{
				return null;
			}
			ConnectionSettingsConverter conversionHelper = ConnectionSettingsConverter.GetConversionHelper(addAccountRequest.IncomingProtocol, addAccountRequest.OutgoingProtocol);
			return conversionHelper.BuildConnectionSettingsFromRequest(addAccountRequest);
		}

		public static ConnectionSettings BuildConnectionSettings(SyncRequestStatistics syncRequest)
		{
			string outSettingsType = "SMTP";
			string inSettingsType;
			switch (syncRequest.SyncProtocol)
			{
			case SyncProtocol.Imap:
				inSettingsType = "IMAP";
				break;
			case SyncProtocol.Eas:
				outSettingsType = null;
				inSettingsType = "EXCHANGEACTIVESYNC";
				break;
			case SyncProtocol.Pop:
				inSettingsType = "POP";
				break;
			default:
				throw new ArgumentException("Could not determine the protocol from the SyncRequest properties.");
			}
			ConnectionSettingsConverter conversionHelper = ConnectionSettingsConverter.GetConversionHelper(inSettingsType, outSettingsType);
			return conversionHelper.BuildConnectionSettingsFromRequest(syncRequest);
		}

		protected abstract void UpdateNewSyncRequestCmdlet(string email, ProtocolSpecificConnectionSettings inSettings, SmtpConnectionSettings outSettings, NewSyncRequest newSyncRequest);

		protected abstract void UpdateSetSyncRequestCmdlet(ProtocolSpecificConnectionSettings inSettings, SmtpConnectionSettings outSettings, SetSyncRequest setSyncRequest);

		protected abstract ConnectionSettingsInfo BuildPublicRepresentation(ProtocolSpecificConnectionSettings inSettings, SmtpConnectionSettings outSettings);

		protected abstract ConnectionSettings BuildConnectionSettingsFromRequest(AddAggregatedAccountRequest addAccountRequest);

		protected abstract ConnectionSettings BuildUpdateConnectionSettings(Fqdn incomingServer, int? incomingPort, string security, string authentication, ProtocolSpecificConnectionSettings inOriginalSettings, SmtpConnectionSettings outOriginalSettings);

		protected abstract ConnectionSettings BuildConnectionSettingsFromRequest(SyncRequestStatistics syncRequest);

		protected virtual SendConnectionSettingsInfo BuildPublicRepresentation(SmtpConnectionSettings outSettings)
		{
			return new SendConnectionSettingsInfo
			{
				ServerName = outSettings.ServerName,
				Port = outSettings.Port
			};
		}

		private static ConnectionSettingsConverter GetConversionHelper(ConnectionSettings connectionSettings)
		{
			if (connectionSettings == null)
			{
				throw new ArgumentNullException("connectionSettings", "The connectionSettings argument cannot be null.");
			}
			string inSettingsType = connectionSettings.IncomingConnectionSettings.ConnectionType.ToString();
			string outSettingsType = (connectionSettings.OutgoingConnectionSettings != null) ? connectionSettings.OutgoingConnectionSettings.ConnectionType.ToString() : null;
			return ConnectionSettingsConverter.GetConversionHelper(inSettingsType, outSettingsType);
		}

		private static ConnectionSettingsConverter GetConversionHelper(string inSettingsType, string outSettingsType)
		{
			outSettingsType = ((!string.IsNullOrEmpty(outSettingsType)) ? outSettingsType.ToUpperInvariant() : null);
			string a;
			if ((a = inSettingsType.ToUpperInvariant()) != null)
			{
				if (!(a == "OFFICE365"))
				{
					if (!(a == "EXCHANGEACTIVESYNC"))
					{
						if (!(a == "IMAP"))
						{
							if (!(a == "POP"))
							{
								if (a == "SMTP")
								{
									throw new ArgumentException("Smtp is an outgoing protocol. It cannot be used for inSettingsType.");
								}
							}
							else
							{
								if (outSettingsType == null)
								{
									throw new ArgumentNullException("outSettingsType", "PopConnectionSettings are for unidirectional use. The outSettingsType argument must not be null in this case.");
								}
								if (outSettingsType != "SMTP")
								{
									throw new ArgumentException("The outSettingsType argument must be of type Smtp when the inSettingsType is Pop.", "outSettingsType");
								}
								return new ConnectionSettingsConverter.PopSmtpSettingsConverter();
							}
						}
						else
						{
							if (outSettingsType == null)
							{
								throw new ArgumentNullException("outSettingsType", "ImapConnectionSettings are for unidirectional use. The outSettingsType argument must not be null in this case.");
							}
							if (outSettingsType != "SMTP")
							{
								throw new ArgumentException("The outSettingsType argument must be of type Smtp when the inSettingsType is Imap.", "outSettingsType");
							}
							return new ConnectionSettingsConverter.ImapSmtpSettingsConverter();
						}
					}
					else
					{
						if (outSettingsType != null)
						{
							throw new ArgumentException("ExchangeActiveSyncConnectionSettings are for bidirectional use. The outSettingsType argument must be null in this case.", "outSettingsType");
						}
						return new ConnectionSettingsConverter.EasSettingsConverter();
					}
				}
				else
				{
					if (outSettingsType != null)
					{
						throw new ArgumentException("Office365ConnectionSettings are for bidirectional use. The outSettingsType argument must be null in this case.", "outSettingsType");
					}
					return new ConnectionSettingsConverter.Office365SettingsConverter();
				}
			}
			throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Unexpected outSettingsType encountered: {0}.", new object[]
			{
				outSettingsType
			}));
		}

		private class EasSettingsConverter : ConnectionSettingsConverter
		{
			protected override void UpdateNewSyncRequestCmdlet(string email, ProtocolSpecificConnectionSettings inSettings, SmtpConnectionSettings outSettings, NewSyncRequest newSyncRequest)
			{
				ExchangeActiveSyncConnectionSettings exchangeActiveSyncConnectionSettings = (ExchangeActiveSyncConnectionSettings)inSettings;
				newSyncRequest.Eas = new SwitchParameter(true);
				newSyncRequest.RemoteServerName = Fqdn.Parse((!string.IsNullOrEmpty(exchangeActiveSyncConnectionSettings.EndpointAddressOverride)) ? exchangeActiveSyncConnectionSettings.EndpointAddressOverride : ConnectionSettingsConverter.EasSettingsConverter.GetDomainName(email));
			}

			protected override void UpdateSetSyncRequestCmdlet(ProtocolSpecificConnectionSettings inSettings, SmtpConnectionSettings outSettings, SetSyncRequest setSyncRequest)
			{
				ExchangeActiveSyncConnectionSettings exchangeActiveSyncConnectionSettings = (ExchangeActiveSyncConnectionSettings)inSettings;
			}

			protected override ConnectionSettingsInfo BuildPublicRepresentation(ProtocolSpecificConnectionSettings inSettings, SmtpConnectionSettings outSettings)
			{
				return new ConnectionSettingsInfo
				{
					ConnectionType = ConnectionSettingsInfoType.ExchangeActiveSync,
					ServerName = ((ExchangeActiveSyncConnectionSettings)inSettings).EndpointAddressOverride
				};
			}

			protected override ConnectionSettings BuildUpdateConnectionSettings(Fqdn incomingServer, int? incomingPort, string security, string authentication, ProtocolSpecificConnectionSettings inOriginalSettings, SmtpConnectionSettings outOriginalSettings)
			{
				ExchangeActiveSyncConnectionSettings exchangeActiveSyncConnectionSettings = new ExchangeActiveSyncConnectionSettings();
				ExchangeActiveSyncConnectionSettings exchangeActiveSyncConnectionSettings2 = (ExchangeActiveSyncConnectionSettings)inOriginalSettings;
				if (string.Compare(exchangeActiveSyncConnectionSettings.EndpointAddressOverride, exchangeActiveSyncConnectionSettings2.EndpointAddressOverride, StringComparison.OrdinalIgnoreCase) != 0)
				{
					if (incomingServer != null)
					{
						exchangeActiveSyncConnectionSettings.EndpointAddressOverride = incomingServer.ToString();
					}
					else
					{
						exchangeActiveSyncConnectionSettings.EndpointAddressOverride = null;
					}
				}
				return new ConnectionSettings(exchangeActiveSyncConnectionSettings, outOriginalSettings);
			}

			protected override ConnectionSettings BuildConnectionSettingsFromRequest(AddAggregatedAccountRequest addAccountRequest)
			{
				ExchangeActiveSyncConnectionSettings exchangeActiveSyncConnectionSettings = new ExchangeActiveSyncConnectionSettings();
				if (!string.IsNullOrEmpty(addAccountRequest.IncomingServer))
				{
					exchangeActiveSyncConnectionSettings.EndpointAddressOverride = addAccountRequest.IncomingServer;
				}
				return new ConnectionSettings(exchangeActiveSyncConnectionSettings, null);
			}

			protected override ConnectionSettings BuildConnectionSettingsFromRequest(SyncRequestStatistics syncRequest)
			{
				ExchangeActiveSyncConnectionSettings exchangeActiveSyncConnectionSettings = new ExchangeActiveSyncConnectionSettings();
				if (!string.IsNullOrEmpty(syncRequest.RemoteServerName))
				{
					exchangeActiveSyncConnectionSettings.EndpointAddressOverride = syncRequest.RemoteServerName;
				}
				return new ConnectionSettings(exchangeActiveSyncConnectionSettings, null);
			}

			private static string GetDomainName(string emailAddress)
			{
				if (string.IsNullOrWhiteSpace(emailAddress))
				{
					return null;
				}
				return new Microsoft.Exchange.Data.SmtpAddress(emailAddress).Domain;
			}
		}

		private class ImapSmtpSettingsConverter : ConnectionSettingsConverter
		{
			protected override void UpdateNewSyncRequestCmdlet(string email, ProtocolSpecificConnectionSettings inSettings, SmtpConnectionSettings outSettings, NewSyncRequest newSyncRequest)
			{
				ImapConnectionSettings imapConnectionSettings = (ImapConnectionSettings)inSettings;
				newSyncRequest.Imap = new SwitchParameter(true);
				newSyncRequest.RemoteServerName = imapConnectionSettings.ServerName;
				newSyncRequest.RemoteServerPort = imapConnectionSettings.Port;
				newSyncRequest.Security = ConnectionSettingsConverter.ImapSmtpSettingsConverter.ConvertToIMAPSecurityMechanism(imapConnectionSettings.Security);
			}

			protected override void UpdateSetSyncRequestCmdlet(ProtocolSpecificConnectionSettings inSettings, SmtpConnectionSettings outSettings, SetSyncRequest setSyncRequest)
			{
				ImapConnectionSettings imapConnectionSettings = (ImapConnectionSettings)inSettings;
				setSyncRequest.RemoteServerName = Fqdn.Parse(imapConnectionSettings.ServerName);
				setSyncRequest.RemoteServerPort = imapConnectionSettings.Port;
				setSyncRequest.Security = ConnectionSettingsConverter.ImapSmtpSettingsConverter.ConvertToIMAPSecurityMechanism(imapConnectionSettings.Security);
			}

			protected override ConnectionSettingsInfo BuildPublicRepresentation(ProtocolSpecificConnectionSettings inSettings, SmtpConnectionSettings outSettings)
			{
				ImapConnectionSettings imapConnectionSettings = (ImapConnectionSettings)inSettings;
				return new ConnectionSettingsInfo
				{
					ConnectionType = ConnectionSettingsInfoType.Imap,
					SendConnectionSettings = this.BuildPublicRepresentation(outSettings),
					ServerName = imapConnectionSettings.ServerName,
					Port = imapConnectionSettings.Port,
					Authentication = ImapHelperMethods.ToStringParameterValue(imapConnectionSettings.Authentication),
					Security = ImapHelperMethods.ToStringParameterValue(imapConnectionSettings.Security)
				};
			}

			protected override ConnectionSettings BuildUpdateConnectionSettings(Fqdn incomingServer, int? incomingPort, string security, string authentication, ProtocolSpecificConnectionSettings inOriginalSettings, SmtpConnectionSettings outOriginalSettings)
			{
				ImapConnectionSettings imapConnectionSettings = (ImapConnectionSettings)inOriginalSettings;
				ImapConnectionSettings incomingSettings = new ImapConnectionSettings((incomingServer != null) ? incomingServer : imapConnectionSettings.ServerName, (incomingPort != null) ? incomingPort.Value : imapConnectionSettings.Port, (!string.IsNullOrEmpty(authentication)) ? ImapHelperMethods.ToImapAuthenticationMechanism(authentication) : imapConnectionSettings.Authentication, (!string.IsNullOrEmpty(security)) ? ImapHelperMethods.ToImapSecurityMechanism(security) : imapConnectionSettings.Security);
				return new ConnectionSettings(incomingSettings, outOriginalSettings);
			}

			protected override ConnectionSettings BuildConnectionSettingsFromRequest(AddAggregatedAccountRequest addAccountRequest)
			{
				ImapConnectionSettings incomingSettings = new ImapConnectionSettings(Fqdn.Parse(addAccountRequest.IncomingServer), int.Parse(addAccountRequest.IncomingPort), ImapHelperMethods.ToImapAuthenticationMechanism(addAccountRequest.Authentication), ImapHelperMethods.ToImapSecurityMechanism(addAccountRequest.Security));
				SmtpConnectionSettings outgoingSettings = new SmtpConnectionSettings(Fqdn.Parse(addAccountRequest.OutgoingServer), int.Parse(addAccountRequest.OutgoingPort));
				return new ConnectionSettings(incomingSettings, outgoingSettings);
			}

			protected override ConnectionSettings BuildConnectionSettingsFromRequest(SyncRequestStatistics syncRequestStatistics)
			{
				ImapConnectionSettings incomingSettings = new ImapConnectionSettings(Fqdn.Parse(syncRequestStatistics.RemoteServerName), syncRequestStatistics.RemoteServerPort, ConnectionSettingsConverter.ImapSmtpSettingsConverter.ConvertToImapAuthenticationMechanism(syncRequestStatistics.AuthenticationMethod), ConnectionSettingsConverter.ImapSmtpSettingsConverter.ConvertToImapSecurityMechanism(syncRequestStatistics.SecurityMechanism));
				SmtpConnectionSettings outgoingSettings = new SmtpConnectionSettings(Fqdn.Parse("dummy.smtp.srv"), 1);
				return new ConnectionSettings(incomingSettings, outgoingSettings);
			}

			public static IMAPSecurityMechanism ConvertToIMAPSecurityMechanism(ImapSecurityMechanism imapSecurityMechanism)
			{
				IMAPSecurityMechanism result;
				switch (imapSecurityMechanism)
				{
				case ImapSecurityMechanism.None:
					result = IMAPSecurityMechanism.None;
					break;
				case ImapSecurityMechanism.Ssl:
					result = IMAPSecurityMechanism.Ssl;
					break;
				case ImapSecurityMechanism.Tls:
					result = IMAPSecurityMechanism.Tls;
					break;
				default:
					throw new NotSupportedException(string.Format("Value {0} is not supported as IMAPSecurityMechanism", imapSecurityMechanism));
				}
				return result;
			}

			public static ImapSecurityMechanism ConvertToImapSecurityMechanism(IMAPSecurityMechanism imapSecurityMechanism)
			{
				ImapSecurityMechanism result;
				switch (imapSecurityMechanism)
				{
				case IMAPSecurityMechanism.None:
					result = ImapSecurityMechanism.None;
					break;
				case IMAPSecurityMechanism.Ssl:
					result = ImapSecurityMechanism.Ssl;
					break;
				case IMAPSecurityMechanism.Tls:
					result = ImapSecurityMechanism.Tls;
					break;
				default:
					throw new NotSupportedException(string.Format("Value {0} is not supported as ImapSecurityMechanism", imapSecurityMechanism));
				}
				return result;
			}

			public static ImapAuthenticationMechanism ConvertToImapAuthenticationMechanism(AuthenticationMethod? authenticationMethod)
			{
				ImapAuthenticationMechanism result = ImapAuthenticationMechanism.Basic;
				if (authenticationMethod != null)
				{
					switch (authenticationMethod.Value)
					{
					case AuthenticationMethod.Basic:
						return ImapAuthenticationMechanism.Basic;
					case AuthenticationMethod.Ntlm:
						return ImapAuthenticationMechanism.Ntlm;
					}
					throw new NotSupportedException(string.Format("Value {0} is not supported as ImapAuthenticationMechanism", authenticationMethod.Value));
				}
				return result;
			}
		}

		private class Office365SettingsConverter : ConnectionSettingsConverter
		{
			protected override void UpdateNewSyncRequestCmdlet(string email, ProtocolSpecificConnectionSettings inSettings, SmtpConnectionSettings outSettings, NewSyncRequest newSyncRequest)
			{
				throw new NotSupportedException("We do not create SyncRequests for Office365ConnectionSettings.");
			}

			protected override void UpdateSetSyncRequestCmdlet(ProtocolSpecificConnectionSettings inSettings, SmtpConnectionSettings outSettings, SetSyncRequest setSyncRequest)
			{
				throw new NotSupportedException("We do not create SyncRequests for Office365ConnectionSettings.");
			}

			protected override ConnectionSettingsInfo BuildPublicRepresentation(ProtocolSpecificConnectionSettings inSettings, SmtpConnectionSettings outSettings)
			{
				Office365ConnectionSettings office365ConnectionSettings = (Office365ConnectionSettings)inSettings;
				return new ConnectionSettingsInfo
				{
					ConnectionType = ConnectionSettingsInfoType.Office365,
					Office365UserFound = (office365ConnectionSettings.AdUser != null)
				};
			}

			protected override ConnectionSettings BuildUpdateConnectionSettings(Fqdn incomingServer, int? incomingPort, string security, string authentication, ProtocolSpecificConnectionSettings inOriginalSettings, SmtpConnectionSettings outOriginalSettings)
			{
				throw new NotImplementedException("The user cannot specify Office365ConnectionSettings when calling the AggregatedAccount service commands so this operation is not necessary.");
			}

			protected override ConnectionSettings BuildConnectionSettingsFromRequest(AddAggregatedAccountRequest addAccountRequest)
			{
				throw new NotImplementedException("The user cannot specify Office365ConnectionSettings when calling the AggregatedAccount service commands.");
			}

			protected override ConnectionSettings BuildConnectionSettingsFromRequest(SyncRequestStatistics syncRequest)
			{
				throw new NotSupportedException("We do not create SyncRequests for Office365ConnectionSettings.");
			}
		}

		private class PopSmtpSettingsConverter : ConnectionSettingsConverter
		{
			protected override void UpdateNewSyncRequestCmdlet(string email, ProtocolSpecificConnectionSettings inSettings, SmtpConnectionSettings outSettings, NewSyncRequest newSyncRequest)
			{
				PopConnectionSettings popConnectionSettings = (PopConnectionSettings)inSettings;
				newSyncRequest.RemoteServerName = popConnectionSettings.ServerName;
				newSyncRequest.RemoteServerPort = popConnectionSettings.Port;
				newSyncRequest.Security = ConnectionSettingsConverter.PopSmtpSettingsConverter.ConvertToIMAPSecurityMechanism(popConnectionSettings.Security);
				newSyncRequest.Pop = new SwitchParameter(true);
			}

			protected override void UpdateSetSyncRequestCmdlet(ProtocolSpecificConnectionSettings inSettings, SmtpConnectionSettings outSettings, SetSyncRequest setSyncRequest)
			{
				PopConnectionSettings popConnectionSettings = (PopConnectionSettings)inSettings;
				setSyncRequest.RemoteServerName = popConnectionSettings.ServerName;
				setSyncRequest.RemoteServerPort = popConnectionSettings.Port;
				setSyncRequest.Security = ConnectionSettingsConverter.PopSmtpSettingsConverter.ConvertToIMAPSecurityMechanism(popConnectionSettings.Security);
			}

			protected override ConnectionSettingsInfo BuildPublicRepresentation(ProtocolSpecificConnectionSettings inSettings, SmtpConnectionSettings outSettings)
			{
				PopConnectionSettings popConnectionSettings = (PopConnectionSettings)inSettings;
				return new ConnectionSettingsInfo
				{
					ConnectionType = ConnectionSettingsInfoType.Pop,
					SendConnectionSettings = this.BuildPublicRepresentation(outSettings),
					ServerName = popConnectionSettings.ServerName,
					Port = popConnectionSettings.Port,
					Authentication = PopHelperMethods.ToStringParameterValue(popConnectionSettings.Authentication),
					Security = PopHelperMethods.ToStringParameterValue(popConnectionSettings.Security)
				};
			}

			protected override ConnectionSettings BuildUpdateConnectionSettings(Fqdn incomingServer, int? incomingPort, string security, string authentication, ProtocolSpecificConnectionSettings inOriginalSettings, SmtpConnectionSettings outOriginalSettings)
			{
				PopConnectionSettings popConnectionSettings = (PopConnectionSettings)inOriginalSettings;
				PopConnectionSettings incomingSettings = new PopConnectionSettings((incomingServer != null) ? incomingServer : popConnectionSettings.ServerName, (incomingPort != null) ? incomingPort.Value : popConnectionSettings.Port, (!string.IsNullOrEmpty(authentication)) ? PopHelperMethods.ToPopAuthenticationMechanism(authentication) : popConnectionSettings.Authentication, (!string.IsNullOrEmpty(security)) ? PopHelperMethods.ToPopSecurityMechanism(security) : popConnectionSettings.Security);
				return new ConnectionSettings(incomingSettings, outOriginalSettings);
			}

			protected override ConnectionSettings BuildConnectionSettingsFromRequest(AddAggregatedAccountRequest addAccountRequest)
			{
				PopConnectionSettings incomingSettings = new PopConnectionSettings(Fqdn.Parse(addAccountRequest.IncomingServer), int.Parse(addAccountRequest.IncomingPort), PopHelperMethods.ToPopAuthenticationMechanism(addAccountRequest.Authentication), PopHelperMethods.ToPopSecurityMechanism(addAccountRequest.Security));
				SmtpConnectionSettings outgoingSettings = new SmtpConnectionSettings(Fqdn.Parse(addAccountRequest.OutgoingServer), int.Parse(addAccountRequest.OutgoingPort));
				return new ConnectionSettings(incomingSettings, outgoingSettings);
			}

			protected override ConnectionSettings BuildConnectionSettingsFromRequest(SyncRequestStatistics syncRequestStatistics)
			{
				PopConnectionSettings incomingSettings = new PopConnectionSettings(Fqdn.Parse(syncRequestStatistics.RemoteServerName), syncRequestStatistics.RemoteServerPort, ConnectionSettingsConverter.PopSmtpSettingsConverter.ConvertToPop3AuthenticationMechanism(syncRequestStatistics.AuthenticationMethod), ConnectionSettingsConverter.PopSmtpSettingsConverter.ConvertToPop3SecurityMechanism(syncRequestStatistics.SecurityMechanism));
				SmtpConnectionSettings outgoingSettings = new SmtpConnectionSettings(Fqdn.Parse("dummy.smtp.srv"), 1);
				return new ConnectionSettings(incomingSettings, outgoingSettings);
			}

			public static IMAPSecurityMechanism ConvertToIMAPSecurityMechanism(Pop3SecurityMechanism pop3SecurityMechanism)
			{
				IMAPSecurityMechanism result;
				switch (pop3SecurityMechanism)
				{
				case Pop3SecurityMechanism.None:
					result = IMAPSecurityMechanism.None;
					break;
				case Pop3SecurityMechanism.Ssl:
					result = IMAPSecurityMechanism.Ssl;
					break;
				case Pop3SecurityMechanism.Tls:
					result = IMAPSecurityMechanism.Tls;
					break;
				default:
					throw new NotSupportedException(string.Format("Value {0} is not supported as IMAPSecurityMechanism", pop3SecurityMechanism));
				}
				return result;
			}

			public static Pop3SecurityMechanism ConvertToPop3SecurityMechanism(IMAPSecurityMechanism imapSecurityMechanism)
			{
				Pop3SecurityMechanism result;
				switch (imapSecurityMechanism)
				{
				case IMAPSecurityMechanism.None:
					result = Pop3SecurityMechanism.None;
					break;
				case IMAPSecurityMechanism.Ssl:
					result = Pop3SecurityMechanism.Ssl;
					break;
				case IMAPSecurityMechanism.Tls:
					result = Pop3SecurityMechanism.Tls;
					break;
				default:
					throw new NotSupportedException(string.Format("Value {0} is not supported as Pop3SecurityMechanism", imapSecurityMechanism));
				}
				return result;
			}

			public static Pop3AuthenticationMechanism ConvertToPop3AuthenticationMechanism(AuthenticationMethod? authenticationMethod)
			{
				Pop3AuthenticationMechanism result = Pop3AuthenticationMechanism.Basic;
				if (authenticationMethod != null)
				{
					AuthenticationMethod value = authenticationMethod.Value;
					if (value != AuthenticationMethod.Basic)
					{
						throw new NotSupportedException(string.Format("Value {0} is not supported as Pop3AuthenticationMechanism", authenticationMethod.Value));
					}
					result = Pop3AuthenticationMechanism.Basic;
				}
				return result;
			}
		}
	}
}
