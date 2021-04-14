using System;
using System.Globalization;
using System.Net;
using System.Security;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Net.ExSmtpClient;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class XProxySmtpCommand : SmtpCommand
	{
		public XProxySmtpCommand(ISmtpSession session, ITransportConfiguration transportConfiguration, ITransportAppConfig transportAppConfig) : base(session, "XPROXY", null, LatencyComponent.None)
		{
			this.transportAppConfig = transportAppConfig;
			this.transportConfiguration = transportConfiguration;
		}

		private bool ShouldRejectXProxy(int? xproxyCapabilities)
		{
			return Components.SmtpInComponent.TargetRunningState == ServiceState.Inactive && (xproxyCapabilities == null || (xproxyCapabilities.Value & 128) != 128);
		}

		internal override void InboundParseCommand()
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.XProxyInboundParseCommand);
			SmtpAddress clientProxyAddress;
			string text;
			IPAddress ipaddress;
			int num;
			string text2;
			SecurityIdentifier securityIdentifier;
			byte[] array;
			int? num2;
			ParseResult parseResult = XProxySmtpCommandParser.ParseArguments(CommandContext.FromSmtpCommand(this), smtpInSession.SmtpUtf8Supported, out text, out ipaddress, out num, out text2, out securityIdentifier, out clientProxyAddress, out array, out num2);
			smtpInSession.LogSession.LogReceive(array ?? base.ProtocolCommand);
			if (!base.VerifyEhloReceived())
			{
				smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.WrongSequence);
				return;
			}
			if (!smtpInSession.AdvertisedEhloOptions.XProxy)
			{
				base.SmtpResponse = SmtpResponse.CommandNotImplemented;
				base.ParsingStatus = ParsingStatus.ProtocolError;
				return;
			}
			if (this.ShouldRejectXProxy(num2))
			{
				base.SmtpResponse = SmtpResponse.ServiceInactive;
				smtpInSession.LogInformation(ProtocolLoggingLevel.Verbose, "Rejecting the non-probe xproxy session and disconnecting as transport service is Inactive.", null);
				smtpInSession.Disconnect(DisconnectReason.Local);
				return;
			}
			if (!SmtpInSessionUtils.HasSMTPAcceptXProxyPermission(smtpInSession.RemoteIdentity, smtpInSession.AuthMethod) && !SmtpInSessionUtils.ShouldAcceptProxyProtocol(this.transportConfiguration.ProcessTransportRole, smtpInSession.Capabilities))
			{
				smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.XProxyNotAuthorized);
				base.SmtpResponse = SmtpResponse.NotAuthorized;
				base.ParsingStatus = ParsingStatus.ProtocolError;
				return;
			}
			if (!base.VerifyNoOngoingMailTransaction() || !base.VerifyNotAuthenticatedThroughAuthLoginVerb())
			{
				smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.WrongSequence);
				return;
			}
			if (smtpInSession.InboundClientProxyState != InboundClientProxyStates.None)
			{
				base.SmtpResponse = SmtpResponse.XProxyAlreadySpecified;
				base.ParsingStatus = ParsingStatus.ProtocolError;
				return;
			}
			if (parseResult.IsFailed)
			{
				ExTraceGlobals.SmtpReceiveTracer.TraceError<SmtpResponse>((long)this.GetHashCode(), "XPROXY parsing failed; SMTP Response: {0}", base.SmtpResponse);
				base.SmtpResponse = parseResult.SmtpResponse;
				base.ParsingStatus = parseResult.ParsingStatus;
				return;
			}
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug((long)this.GetHashCode(), "XPROXY parsing completed. Session id = {0}; Client address = {1}; Client port = {2};Client HELO/EHLO domain = {3}; Client Sid = {4}; Client proxy address = {5}", new object[]
			{
				text,
				ipaddress,
				num,
				text2,
				securityIdentifier,
				clientProxyAddress
			});
			IRecipientSession recipientSession = null;
			if (smtpInSession.Connector.LiveCredentialEnabled && !clientProxyAddress.Equals(SmtpAddress.Empty))
			{
				ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
				{
					recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromTenantAcceptedDomain(clientProxyAddress.Domain), 190, "InboundParseCommand", "f:\\15.00.1497\\sources\\dev\\Transport\\src\\Smtp\\Common\\SmtpCommands\\XProxySmtpCommand.cs");
				});
				if (!adoperationResult.Succeeded)
				{
					ExTraceGlobals.SmtpReceiveTracer.TraceDebug<string, Exception>((long)this.GetHashCode(), "TenantAccepted Domain not found {0}: {1}", clientProxyAddress.Domain, adoperationResult.Exception);
					smtpInSession.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Inbound Authentication failed to find accepted domain " + clientProxyAddress.Domain);
				}
			}
			if (recipientSession == null)
			{
				ADOperationResult adoperationResult2 = ADNotificationAdapter.TryRunADOperation(delegate()
				{
					recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 217, "InboundParseCommand", "f:\\15.00.1497\\sources\\dev\\Transport\\src\\Smtp\\Common\\SmtpCommands\\XProxySmtpCommand.cs");
				}, 0);
				if (!adoperationResult2.Succeeded)
				{
					ExTraceGlobals.SmtpReceiveTracer.TraceDebug<Exception>((long)this.GetHashCode(), "Error when getting recipient session: {0}", adoperationResult2.Exception);
					base.SmtpResponse = ((adoperationResult2.ErrorCode == ADOperationErrorCode.RetryableError) ? SmtpResponse.AuthenticationFailedTemporary : SmtpResponse.AuthenticationFailedPermanent);
					base.ParsingStatus = ParsingStatus.Error;
					return;
				}
			}
			TransportMiniRecipient transportMiniRecipient = null;
			if (!clientProxyAddress.Equals(SmtpAddress.Empty))
			{
				transportMiniRecipient = AuthCommandHelpers.GetMiniRecipientByProxyAddress(recipientSession, new SmtpProxyAddress(clientProxyAddress.ToString(), false), ExTraceGlobals.SmtpReceiveTracer, smtpInSession.GetHashCode());
				if (transportMiniRecipient == null)
				{
					smtpInSession.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Failed to look up user by proxy address");
					base.SmtpResponse = ParseResult.UserLookupFailed.SmtpResponse;
				}
			}
			else if (securityIdentifier != null)
			{
				transportMiniRecipient = AuthCommandHelpers.GetMiniRecipientBySid(recipientSession, securityIdentifier, ExTraceGlobals.SmtpReceiveTracer, smtpInSession.GetHashCode());
				if (transportMiniRecipient == null)
				{
					smtpInSession.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Failed to look up user by Sid");
					base.SmtpResponse = ParseResult.UserLookupFailed.SmtpResponse;
				}
			}
			else
			{
				base.SmtpResponse = ParseResult.XProxyAccepted.SmtpResponse;
			}
			securityIdentifier = null;
			string clientIdentityName = null;
			WindowsIdentity identity = null;
			if (transportMiniRecipient != null)
			{
				string sUserPrincipalName;
				if (Util.TryGetUserPrincipalNameForXproxy(transportMiniRecipient, VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Transport.SmtpXproxyConstructUpnFromSamAccountNameAndParitionFqdn.Enabled, out sUserPrincipalName))
				{
					string text3 = null;
					try
					{
						identity = new WindowsIdentity(sUserPrincipalName);
					}
					catch (UnauthorizedAccessException ex)
					{
						text3 = ex.Message;
					}
					catch (SecurityException ex2)
					{
						text3 = ex2.Message;
					}
					catch (OutOfMemoryException ex3)
					{
						text3 = ex3.Message;
					}
					if (!string.IsNullOrEmpty(text3))
					{
						smtpInSession.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Error while looking up SamAccountName {0}: {1}", new object[]
						{
							transportMiniRecipient.SamAccountName,
							text3
						});
						base.SmtpResponse = ParseResult.UnableToObtainIdentity.SmtpResponse;
					}
					else
					{
						base.SmtpResponse = ParseResult.XProxyAcceptedAuthenticated.SmtpResponse;
						securityIdentifier = transportMiniRecipient.Sid;
						clientIdentityName = transportMiniRecipient.Name;
					}
				}
				else
				{
					smtpInSession.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "No UPN found for user");
					base.SmtpResponse = ParseResult.UnableToObtainIdentity.SmtpResponse;
				}
			}
			smtpInSession.UpdateSessionWithProxyInformation(ipaddress, num, text2, true, securityIdentifier, clientIdentityName, identity, transportMiniRecipient, num2);
			XProxySmtpCommand.ResetXAnonymousTlsBasedEhloOptions(smtpInSession);
			base.ParsingStatus = ParsingStatus.Complete;
		}

		internal override void InboundProcessCommand()
		{
		}

		internal override void OutboundCreateCommand()
		{
		}

		internal override void OutboundFormatCommand()
		{
			SmtpOutProxySession smtpOutProxySession = (SmtpOutProxySession)base.SmtpSession;
			StringBuilder stringBuilder = new StringBuilder("XPROXY");
			stringBuilder.AppendFormat(" {0}={1}", XProxyParserUtils.SessionIdKeyword, smtpOutProxySession.InSession.SessionId.ToString("X16", NumberFormatInfo.InvariantInfo));
			stringBuilder.AppendFormat(" {0}={1}", XProxyParserUtils.ClientIPKeyword, smtpOutProxySession.InSession.ClientEndPoint.Address);
			stringBuilder.AppendFormat(" {0}={1}", XProxyParserUtils.ClientPortKeyword, smtpOutProxySession.InSession.ClientEndPoint.Port);
			if (smtpOutProxySession.InSession.HelloSmtpDomain != null)
			{
				stringBuilder.AppendFormat(" {0}={1}", XProxyParserUtils.ClientHelloDomainKeyword, smtpOutProxySession.InSession.HelloSmtpDomain);
			}
			if (this.transportAppConfig.SmtpInboundProxyConfiguration.SendNewXProxyFromArguments)
			{
				stringBuilder.AppendFormat(" {0}={1}", XProxyParserUtils.CapabilitiesKeyword, (uint)smtpOutProxySession.InSession.Capabilities);
			}
			if (VariantConfiguration.InvariantNoFlightingSnapshot.Transport.SendUserAddressInXproxyCommand.Enabled)
			{
				if (smtpOutProxySession.InSession.AuthUserRecipient != null && smtpOutProxySession.InSession.AuthUserRecipient.PrimarySmtpAddress.IsValidAddress && !this.transportAppConfig.SmtpProxyConfiguration.ReplayAuthLogin)
				{
					string text = smtpOutProxySession.InSession.AuthUserRecipient.PrimarySmtpAddress.ToString();
					if (!string.IsNullOrEmpty(text))
					{
						if (Util.IsDataRedactionNecessary())
						{
							base.RedactedProtocolCommandString = string.Format("{0} {1}={2}", stringBuilder, XProxyParserUtils.ClientUsernameKeyword, Util.Redact(text));
						}
						stringBuilder.AppendFormat(" {0}={1}", XProxyParserUtils.ClientUsernameKeyword, SmtpUtils.ToXtextString(text, false));
					}
				}
			}
			else if (string.IsNullOrEmpty(smtpOutProxySession.InSession.ProxyUserName) || !this.transportAppConfig.SmtpProxyConfiguration.ReplayAuthLogin)
			{
				byte[] bytes = Encoding.ASCII.GetBytes(smtpOutProxySession.InSession.RemoteIdentity.ToString());
				string input = Convert.ToBase64String(bytes);
				string arg = SmtpUtils.ToXtextString(input, false);
				stringBuilder.AppendFormat(" {0}={1}", XProxyParserUtils.SecurityIdKeyword, arg);
			}
			base.ProtocolCommandString = stringBuilder.ToString();
			ExTraceGlobals.SmtpSendTracer.TraceDebug<string>((long)this.GetHashCode(), "Formatted command : {0}", base.ProtocolCommandString);
		}

		internal override void OutboundProcessResponse()
		{
			SmtpOutProxySession smtpOutProxySession = (SmtpOutProxySession)base.SmtpSession;
			string statusCode = base.SmtpResponse.StatusCode;
			if (statusCode[0] != '2')
			{
				ExTraceGlobals.SmtpSendTracer.TraceError<SmtpResponse>((long)this.GetHashCode(), "XPROXY failed with: {0}", base.SmtpResponse);
				smtpOutProxySession.FailoverConnection(base.SmtpResponse, SessionSetupFailureReason.ProtocolError);
				smtpOutProxySession.NextState = SmtpOutSession.SessionState.Quit;
				return;
			}
			if (base.SmtpResponse.StatusText != null && base.SmtpResponse.StatusText.Length != 0 && string.Equals(base.SmtpResponse.StatusText[0], ParseResult.XProxyAcceptedAuthenticated.SmtpResponse.StatusText[0], StringComparison.OrdinalIgnoreCase))
			{
				base.SmtpResponse = SmtpResponse.AuthSuccessful;
				smtpOutProxySession.IsAuthenticated = true;
				smtpOutProxySession.BlindProxySuccessfulInboundResponse = base.SmtpResponse;
				smtpOutProxySession.IsProxying = true;
				return;
			}
			if (smtpOutProxySession.InSession.ProxyUserName != null)
			{
				smtpOutProxySession.SetNextStateToAuthLogin();
				return;
			}
			base.SmtpResponse = SmtpResponse.UnableToProxyIntegratedAuthResponse;
			smtpOutProxySession.InSession.ClientProxyFailedDueToIncompatibleBackend = true;
			ExTraceGlobals.SmtpSendTracer.TraceError((long)this.GetHashCode(), "XPROXY succeeded but did not authenticate user or failed to lookup. Failing incoming session since user authenticated with integrated auth (AUTH NTLM/GSSAPI)");
			smtpOutProxySession.FailoverConnection(base.SmtpResponse, SessionSetupFailureReason.ProtocolError);
			smtpOutProxySession.NextState = SmtpOutSession.SessionState.Quit;
		}

		private static void ResetXAnonymousTlsBasedEhloOptions(ISmtpInSession session)
		{
			session.AdvertisedEhloOptions.XProxy = false;
			session.AdvertisedEhloOptions.XProxyFrom = false;
			session.AdvertisedEhloOptions.XProxyTo = false;
			session.AdvertisedEhloOptions.XSessionMdbGuid = false;
			session.AdvertisedEhloOptions.XAttr = false;
			session.AdvertisedEhloOptions.XSysProbe = false;
			session.AdvertisedEhloOptions.XAdrc = false;
			session.AdvertisedEhloOptions.XFastIndex = false;
			session.AdvertisedEhloOptions.XShadowRequest = false;
			session.AdvertisedEhloOptions.XOrigFrom = false;
			session.AdvertisedEhloOptions.XSessionType = false;
		}

		private readonly ITransportConfiguration transportConfiguration;

		private readonly ITransportAppConfig transportAppConfig;
	}
}
