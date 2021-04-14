using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Common.Net;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Storage;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class FastIndexSmtpMessageContextBlob : SmtpMessageContextBlob
	{
		public static bool IsSupportedVersion(string ehloAdvertisement, out Version version)
		{
			version = null;
			if (ehloAdvertisement.Equals(FastIndexSmtpMessageContextBlob.VersionString, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			Match match = FastIndexSmtpMessageContextBlob.VersionRegex.Match(ehloAdvertisement);
			if (!match.Success)
			{
				return false;
			}
			version = new Version(int.Parse(match.Groups["Major"].Value), int.Parse(match.Groups["Minor"].Value), int.Parse(match.Groups["Build"].Value), int.Parse(match.Groups["Revision"].Value));
			return FastIndexSmtpMessageContextBlob.Version.Major == version.Major && FastIndexSmtpMessageContextBlob.Version.Minor == version.Minor;
		}

		public FastIndexSmtpMessageContextBlob(bool acceptFromSmptIn, bool sendToSmtpOut, ProcessTransportRole role) : base(acceptFromSmptIn, sendToSmtpOut, role)
		{
		}

		public override string Name
		{
			get
			{
				return FastIndexSmtpMessageContextBlob.VersionString;
			}
		}

		public override ByteQuantifiedSize MaxBlobSize
		{
			get
			{
				return Components.TransportAppConfig.MessageContextBlobConfiguration.FastIndexMaxBlobSize;
			}
		}

		public override bool IsAdvertised(IEhloOptions ehloOptions)
		{
			return ehloOptions.XFastIndex;
		}

		public override void DeserializeBlob(Stream stream, ISmtpInSession smtpInSession, long blobSize)
		{
			ArgumentValidator.ThrowIfNull("stream", stream);
			ArgumentValidator.ThrowIfNull("smtpInSession", smtpInSession);
			ArgumentValidator.ThrowIfNull("transportMailItem", smtpInSession.TransportMailItem);
			smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.DeserializeFastIndexBlob);
			this.DeserializeBlobInternal(stream, smtpInSession.TransportMailItem, blobSize);
		}

		public override void DeserializeBlob(Stream stream, SmtpInSessionState sessionState, long blobSize)
		{
			ArgumentValidator.ThrowIfNull("stream", stream);
			ArgumentValidator.ThrowIfNull("sessionState", sessionState);
			ArgumentValidator.ThrowIfNull("transportMailItem", sessionState.TransportMailItem);
			this.DeserializeBlobInternal(stream, sessionState.TransportMailItem, blobSize);
		}

		public override Stream SerializeBlob(SmtpOutSession smtpOutSession)
		{
			smtpOutSession.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.SerializeFastIndexBlob);
			LazyBytes fastIndexBlob = smtpOutSession.RoutedMailItem.FastIndexBlob;
			int num = (fastIndexBlob.Value == null) ? 0 : fastIndexBlob.Value.Length;
			byte[] bytes = BitConverter.GetBytes(num);
			Stream stream = new MultiByteArrayMemoryStream();
			stream.Write(bytes, 0, bytes.Length);
			if (fastIndexBlob.Value != null && fastIndexBlob.Value.Length > 0)
			{
				stream.Write(fastIndexBlob.Value, 0, fastIndexBlob.Value.Length);
			}
			SystemProbeHelper.SmtpSendTracer.TracePass<int>(smtpOutSession.RoutedMailItem, 0L, "Sending fast Index. Size={0}", num);
			return stream;
		}

		public override bool VerifyPermission(Permission permission)
		{
			return SmtpInSessionUtils.HasSMTPAcceptXMessageContextFastIndexPermission(permission);
		}

		public override bool HasDataToSend(SmtpOutSession smtpOutSession)
		{
			if (smtpOutSession.RoutedMailItem.FastIndexBlob.Value != null && smtpOutSession.RoutedMailItem.FastIndexBlob.Value.Length != 0)
			{
				return true;
			}
			smtpOutSession.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "FastIndex blob is empty. Not sending the blob.");
			return false;
		}

		private void DeserializeBlobInternal(Stream stream, TransportMailItem transportMailItem, long blobSize)
		{
			byte[] intValueReadBuffer = new byte[4];
			int num = SmtpMessageContextBlob.ReadInt(stream, intValueReadBuffer);
			if ((long)num != blobSize - 4L)
			{
				throw new FormatException(string.Format("Unexpected blob size while processing FastIndex blob. Expected value is {0}. Actual value is {1}", blobSize - 4L, num));
			}
			transportMailItem.FastIndexBlob.Value = new byte[num];
			stream.Read(transportMailItem.FastIndexBlob.Value, 0, num);
			SystemProbeHelper.SmtpReceiveTracer.TracePass<int>(transportMailItem, 0L, "Receieved fast Index information. Size={0}", num);
		}

		public static readonly Version Version = new Version(1, 1, 0, 0);

		public static readonly string FastIndexBlobName = "FSTINDX";

		private static readonly Regex VersionRegex = new Regex("^FSTINDX-(?<Major>\\d{1,7})\\.(?<Minor>\\d{1,7})\\.(?<Build>\\d{1,7})\\.(?<Revision>\\d{1,7})$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled);

		public static readonly string VersionString = string.Format(CultureInfo.InvariantCulture, "{0}-{1}", new object[]
		{
			FastIndexSmtpMessageContextBlob.FastIndexBlobName,
			FastIndexSmtpMessageContextBlob.Version
		});

		public static readonly string VersionStringWithSpace = string.Format(CultureInfo.InvariantCulture, " {0}", new object[]
		{
			FastIndexSmtpMessageContextBlob.VersionString
		});
	}
}
