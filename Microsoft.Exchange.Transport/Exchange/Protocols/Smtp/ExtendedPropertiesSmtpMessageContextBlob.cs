using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Logging;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class ExtendedPropertiesSmtpMessageContextBlob : SmtpMessageContextBlob
	{
		public ExtendedPropertiesSmtpMessageContextBlob(bool acceptFromSmptIn, bool sendToSmtpOut, ProcessTransportRole role) : base(acceptFromSmptIn, sendToSmtpOut, role)
		{
		}

		public override bool IsMandatory
		{
			get
			{
				return Components.Configuration.ProcessTransportRole == ProcessTransportRole.MailboxDelivery;
			}
		}

		public override string Name
		{
			get
			{
				return ExtendedPropertiesSmtpMessageContextBlob.VersionString;
			}
		}

		public override ByteQuantifiedSize MaxBlobSize
		{
			get
			{
				return Components.TransportAppConfig.MessageContextBlobConfiguration.ExtendedPropertiesMaxBlobSize;
			}
		}

		public static bool IsSupportedVersion(string ehloAdvertisement, out Version ehloEPropsVersion)
		{
			ehloEPropsVersion = null;
			if (ehloAdvertisement.Equals(ExtendedPropertiesSmtpMessageContextBlob.VersionString, StringComparison.OrdinalIgnoreCase))
			{
				ehloEPropsVersion = ExtendedPropertiesSmtpMessageContextBlob.Version;
				return true;
			}
			Match match = ExtendedPropertiesSmtpMessageContextBlob.VersionRegex.Match(ehloAdvertisement);
			if (match.Success)
			{
				ehloEPropsVersion = new Version(int.Parse(match.Groups["Major"].Value), int.Parse(match.Groups["Minor"].Value), int.Parse(match.Groups["Build"].Value), int.Parse(match.Groups["Revision"].Value));
			}
			return match.Success;
		}

		public override bool IsAdvertised(IEhloOptions ehloOptions)
		{
			return ehloOptions.XExprops;
		}

		public override void DeserializeBlob(Stream stream, ISmtpInSession smtpInSession, long blobSize)
		{
			ArgumentValidator.ThrowIfNull("stream", stream);
			ArgumentValidator.ThrowIfNull("smtpInSession", smtpInSession);
			smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.DeserializeExtendedPropertiesBlob);
			this.DeserializeBlobInternal(stream, smtpInSession.TransportMailItem, smtpInSession.RecipientCorrelator, smtpInSession.MailCommandMessageContextInformation, smtpInSession.LogSession);
		}

		public override void DeserializeBlob(Stream stream, SmtpInSessionState sessionState, long blobSize)
		{
			ArgumentValidator.ThrowIfNull("stream", stream);
			ArgumentValidator.ThrowIfNull("sessionState", sessionState);
			this.DeserializeBlobInternal(stream, sessionState.TransportMailItem, sessionState.RecipientCorrelator, sessionState.MailCommandMessageContextInformation, sessionState.ProtocolLogSession);
		}

		public override Stream SerializeBlob(SmtpOutSession smtpOutSession)
		{
			smtpOutSession.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.SerializeExtendedPropertiesBlob);
			MemoryStream memoryStream = new MemoryStream();
			int value = this.GetRecipientCount(smtpOutSession) + 1;
			byte[] bytes = BitConverter.GetBytes(value);
			memoryStream.Write(bytes, 0, bytes.Length);
			IReadOnlyExtendedPropertyCollection mailItemExtendedProperties = this.GetMailItemExtendedProperties(smtpOutSession);
			memoryStream.Write(BitConverter.GetBytes(mailItemExtendedProperties.Count), 0, 4);
			mailItemExtendedProperties.Serialize(memoryStream);
			int num = 0;
			byte[] array = new byte[256];
			StringBuilder stringBuilder = new StringBuilder();
			foreach (MailRecipient mailRecipient in this.GetRecipients(smtpOutSession))
			{
				IExtendedPropertyCollection extendedProperties = mailRecipient.ExtendedProperties;
				memoryStream.Write(BitConverter.GetBytes(extendedProperties.Count), 0, 4);
				extendedProperties.Serialize(memoryStream);
				base.SerializeValue(memoryStream, ref array, mailRecipient.Email);
				ExTraceGlobals.SmtpSendTracer.TraceDebug<RoutingAddress>(0L, "Serializing {0}", mailRecipient.Email);
				num += extendedProperties.Count;
				if (stringBuilder.Length != 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(Util.Redact(mailRecipient.Email));
			}
			if (smtpOutSession.LogSession != null)
			{
				smtpOutSession.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Transfering {0} mailitemlevel and {1} recipient level properties for recipient(s) {2}", new object[]
				{
					mailItemExtendedProperties.Count,
					num,
					stringBuilder
				});
			}
			SystemProbeHelper.SmtpSendTracer.TracePass<int, int, StringBuilder>(smtpOutSession.RoutedMailItem, 0L, "Transfering {0} mailitemlevel and {1} recipient level properties for recipient(s) {2}", mailItemExtendedProperties.Count, num, stringBuilder);
			return memoryStream;
		}

		public override bool VerifyPermission(Permission permission)
		{
			return SmtpInSessionUtils.HasSMTPAcceptXMessageContextExtendedPropertiesPermission(permission);
		}

		public override string GetVersionToSend(IEhloOptions ehloEPropsVersion)
		{
			Version version = ExtendedPropertiesSmtpMessageContextBlob.Version;
			if (ExtendedPropertiesSmtpMessageContextBlob.Version.CompareTo(ehloEPropsVersion.XExpropsVersion) > 0)
			{
				version = ehloEPropsVersion.XExpropsVersion;
			}
			return ExtendedPropertiesSmtpMessageContextBlob.GetVersionString(ExtendedPropertiesSmtpMessageContextBlob.ExtendedPropertiesBlobName, version);
		}

		protected virtual int GetRecipientCount(SmtpOutSession smtpOutSession)
		{
			return smtpOutSession.RoutedMailItem.Recipients.Count;
		}

		protected virtual IEnumerable<MailRecipient> GetRecipients(SmtpOutSession smtpOutSession)
		{
			foreach (MailRecipient mailRecipient in smtpOutSession.RecipientCorrelator.Recipients)
			{
				yield return mailRecipient;
			}
			yield break;
		}

		protected virtual IEnumerable<MailRecipient> GetRecipients(RecipientCorrelator recipientCorrelator)
		{
			return recipientCorrelator.Recipients;
		}

		protected virtual IReadOnlyExtendedPropertyCollection GetMailItemExtendedProperties(SmtpOutSession smtpOutSession)
		{
			return smtpOutSession.RoutedMailItem.ExtendedProperties;
		}

		protected virtual IExtendedPropertyCollection GetMailItemExtendedProperties(TransportMailItem transportMailItem)
		{
			return transportMailItem.ExtendedProperties;
		}

		protected override bool AllowNextHopType(SmtpOutSession smtpOutSession)
		{
			return Components.Configuration.ProcessTransportRole != ProcessTransportRole.Hub || smtpOutSession.NextHopDeliveryType == DeliveryType.SmtpDeliveryToMailbox;
		}

		private static string GetVersionString(string name, Version version)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}-{1}", new object[]
			{
				ExtendedPropertiesSmtpMessageContextBlob.ExtendedPropertiesBlobName,
				version
			});
		}

		protected virtual void CheckIfSentFromHubAndReceivedByHub(TransportMailItem transportMailItem)
		{
			if (Components.Configuration.ProcessTransportRole != ProcessTransportRole.Hub)
			{
				return;
			}
			string value;
			ProcessTransportRole processTransportRole;
			if (this.GetMailItemExtendedProperties(transportMailItem).TryGetValue<string>(SmtpMessageContextBlob.ProcessTransportRoleKey, out value) && Enum.TryParse<ProcessTransportRole>(value, out processTransportRole) && processTransportRole == ProcessTransportRole.Hub)
			{
				throw new FormatException("Cannot receive extended properties blob from another Hub");
			}
		}

		private void DeserializeBlobInternal(Stream stream, TransportMailItem transportMailItem, RecipientCorrelator recipientCorrelator, MailCommandMessageContextParameters messageContextParameters, IProtocolLogSession protocolLogSession)
		{
			if (this.processTransportRole == ProcessTransportRole.Hub && messageContextParameters != null && messageContextParameters.EpropVersion != null && messageContextParameters.EpropVersion < ExtendedPropertiesSmtpMessageContextBlob.E15EpropVersion)
			{
				if (protocolLogSession != null)
				{
					protocolLogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Received EPROP blob with version {0}. Ignoring the blob", new object[]
					{
						messageContextParameters.EpropVersion
					});
				}
				return;
			}
			byte[] intValueReadBuffer = new byte[4];
			try
			{
				SmtpMessageContextBlob.ReadInt(stream, intValueReadBuffer);
				int num = SmtpMessageContextBlob.ReadInt(stream, intValueReadBuffer);
				this.GetMailItemExtendedProperties(transportMailItem).Deserialize(stream, num, true);
				this.CheckIfSentFromHubAndReceivedByHub(transportMailItem);
				TransportPropertyStreamReader reader = new TransportPropertyStreamReader(stream);
				StringBuilder stringBuilder = new StringBuilder();
				int num2 = 0;
				int num3 = 0;
				foreach (MailRecipient mailRecipient in this.GetRecipients(recipientCorrelator))
				{
					int numberOfExtendedPropertiesToFetch = SmtpMessageContextBlob.ReadInt(stream, intValueReadBuffer);
					IExtendedPropertyCollection extendedPropertyCollection;
					if (mailRecipient == null)
					{
						extendedPropertyCollection = new ExtendedPropertyDictionary();
					}
					else
					{
						extendedPropertyCollection = mailRecipient.ExtendedProperties;
					}
					extendedPropertyCollection.Deserialize(stream, numberOfExtendedPropertiesToFetch, true);
					RoutingAddress routingAddress = base.ReadTypeAndValidate<RoutingAddress>(reader);
					if (mailRecipient == null)
					{
						stringBuilder.Append(Util.Redact(routingAddress) + ",");
					}
					else
					{
						num2 += extendedPropertyCollection.Count;
						num3++;
					}
					if (mailRecipient != null && !mailRecipient.Email.Equals(routingAddress))
					{
						throw new FormatException(string.Format(CultureInfo.InvariantCulture, "Trying to deserialize the extended properties of recipient '{0}' to '{1}'", new object[]
						{
							routingAddress,
							mailRecipient.Email
						}));
					}
				}
				if (protocolLogSession != null)
				{
					protocolLogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Receiving {0} mailitemlevel and {1} recipient level properties for {2} recipient(s). DuplicateRecipients = {3}", new object[]
					{
						num,
						num2,
						num3,
						stringBuilder
					});
				}
				SystemProbeHelper.SmtpReceiveTracer.TracePass(transportMailItem, 0L, "Receiving {0} mailitemlevel and {1} recipient level properties for {2} recipient(s). DuplicateRecipients = {3}", new object[]
				{
					num,
					num2,
					num3,
					stringBuilder
				});
			}
			catch (ArgumentException innerException)
			{
				throw new FormatException("Encountered error while deserializing extended properties blob", innerException);
			}
		}

		public static readonly Version E15EpropVersion = new Version(1, 2, 0, 0);

		public static readonly Version Version = new Version(1, 2, 0, 0);

		private static readonly Regex VersionRegex = new Regex("^EPROP-(?<Major>\\d{1,7})\\.(?<Minor>\\d{1,7})\\.(?<Build>\\d{1,7})\\.(?<Revision>\\d{1,7})$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled);

		public static readonly string ExtendedPropertiesBlobName = "EPROP";

		public static readonly string VersionString = ExtendedPropertiesSmtpMessageContextBlob.GetVersionString(ExtendedPropertiesSmtpMessageContextBlob.ExtendedPropertiesBlobName, ExtendedPropertiesSmtpMessageContextBlob.Version);

		public static readonly string VersionStringWithSpace = string.Format(CultureInfo.InvariantCulture, " {0}", new object[]
		{
			ExtendedPropertiesSmtpMessageContextBlob.VersionString
		});
	}
}
