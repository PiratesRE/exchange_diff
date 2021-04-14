using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Mime.Encoders;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Protocols.Smtp;

namespace Microsoft.Exchange.Transport
{
	internal sealed class ExportStream : Stream
	{
		private ExportStream(Stream headerStream, Stream contentStream)
		{
			this.contentStream = contentStream;
			this.headerStream = headerStream;
		}

		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override long Length
		{
			get
			{
				return this.headerStream.Length + this.contentStream.Length;
			}
		}

		public override long Position
		{
			get
			{
				return this.position;
			}
			set
			{
				this.position = value;
			}
		}

		public static bool TryCreate(IReadOnlyMailItem mailItem, IEnumerable<MailRecipient> recipients, bool foreignConnectorExport, out Stream exportedMessage)
		{
			exportedMessage = null;
			Stream stream = Streams.CreateTemporaryStorageStream();
			if (!ExportStream.TryWriteReplayXHeaders(stream, mailItem, recipients, foreignConnectorExport))
			{
				return false;
			}
			Stream stream2;
			if (foreignConnectorExport && HeaderFirewall.ContainsBlockedHeaders(mailItem.RootPart.Headers, ~RestrictedHeaderSet.MTA))
			{
				stream2 = Streams.CreateTemporaryStorageStream();
				mailItem.RootPart.WriteTo(stream2, null, new HeaderFirewall.OutputFilter(~RestrictedHeaderSet.MTA));
				stream2.Seek(0L, SeekOrigin.Begin);
			}
			else
			{
				stream2 = mailItem.OpenMimeReadStream();
			}
			ExportStream.WriteAsciiHeader(stream, ExportStream.XxheadersEndAsciiHeaderName, stream2.Length.ToString());
			exportedMessage = new ExportStream(stream, stream2);
			return true;
		}

		public static bool TryWriteReplayXHeaders(Stream stream, IReadOnlyMailItem mailItem, IEnumerable<MailRecipient> recipients, bool foreignConnectorExport)
		{
			StringBuilder stringBuilder = new StringBuilder();
			MailSmtpCommand.FormatCommand(stringBuilder, mailItem.From, new EhloOptions
			{
				XAttr = true
			}, Components.Configuration.AppConfig.SmtpMailCommandConfiguration, false, 0L, mailItem.Auth, mailItem.EnvId, mailItem.DsnFormat, mailItem.BodyType, mailItem.Directionality, mailItem.ExternalOrganizationId, mailItem.OrganizationId, mailItem.ExoAccountForest, mailItem.ExoTenantContainer);
			ExportStream.WriteAsciiHeader(stream, ExportStream.XsenderAsciiHeaderName, stringBuilder.ToString());
			bool flag = false;
			foreach (MailRecipient mailRecipient in recipients)
			{
				if (mailRecipient.Status != Status.Handled && mailRecipient.Status != Status.Complete)
				{
					ExportStream.WriteRecipient(stream, mailRecipient, !foreignConnectorExport);
					flag = true;
				}
			}
			if (!flag)
			{
				return false;
			}
			if (!foreignConnectorExport)
			{
				ExportStream.WriteAsciiHeader(stream, ExportStream.XcreatedByAsciiHeaderName, ExportStream.CreatedByString);
				if (!string.IsNullOrEmpty(mailItem.HeloDomain))
				{
					ExportStream.WriteAsciiHeader(stream, ExportStream.XheloDomainAsciiHeaderName, mailItem.HeloDomain);
				}
				if (mailItem.ExtendedProperties.Count > 0)
				{
					ExportStream.WriteExtendedProps(stream, ExportStream.XextendedPropsAsciiHeaderName, mailItem.ExtendedProperties);
				}
				if (mailItem.LegacyXexch50Blob != null && mailItem.LegacyXexch50Blob.Length > 0)
				{
					ExportStream.WriteBinary(stream, ExportStream.XlegacyExch50AsciiHeaderName, mailItem.LegacyXexch50Blob);
				}
				if (!string.IsNullOrEmpty(mailItem.ReceiveConnectorName))
				{
					ExportStream.WriteRfc2047Header(stream, "X-Source", mailItem.ReceiveConnectorName);
				}
				ExportStream.WriteAsciiHeader(stream, ExportStream.XsourceIPAddressAsciiHeaderName, mailItem.SourceIPAddress.ToString());
			}
			return true;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (this.position >= this.headerStream.Length)
			{
				this.headerStream.Position = this.headerStream.Length;
				this.contentStream.Position = this.position - this.headerStream.Length;
			}
			else
			{
				this.headerStream.Position = this.position;
				this.contentStream.Position = 0L;
			}
			int num = this.headerStream.Read(buffer, offset, count);
			if (num < count)
			{
				num += this.contentStream.Read(buffer, offset + num, count - num);
			}
			this.position += (long)num;
			return num;
		}

		public override void Write(byte[] array, int offset, int count)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void Flush()
		{
			throw new NotSupportedException();
		}

		public override void Close()
		{
			try
			{
				if (this.headerStream != null)
				{
					this.headerStream.Close();
					this.headerStream = null;
				}
				if (this.contentStream != null)
				{
					this.contentStream.Close();
					this.contentStream = null;
				}
			}
			finally
			{
				base.Close();
			}
		}

		private static void WriteRecipient(Stream headerStream, MailRecipient recipient, bool writeExtendedProps)
		{
			RoutingAddress routingAddress;
			bool flag = OrarGenerator.TryGetOrarAddress(recipient, out routingAddress);
			StringBuilder stringBuilder = new StringBuilder();
			RcptSmtpCommand.FormatCommand(stringBuilder, null, recipient.Email, null, recipient.ORcpt, recipient.DsnRequested, flag ? routingAddress : RoutingAddress.Empty, null, false);
			writeExtendedProps = (writeExtendedProps && recipient.ExtendedProperties.Count > 0);
			ExportStream.WriteAsciiHeader(headerStream, ExportStream.XreceiverAsciiHeaderName, stringBuilder.ToString(), !writeExtendedProps);
			if (writeExtendedProps)
			{
				ExportStream.WriteExtendedProps(headerStream, ExportStream.XextendedPropsAsciiParameter, recipient.ExtendedProperties);
			}
		}

		private static void WriteRfc2047Header(Stream headerStream, string headerName, string headerValue)
		{
			TextHeader textHeader = new TextHeader(headerName, headerValue);
			textHeader.WriteTo(headerStream);
		}

		private static void WriteAsciiHeader(Stream headerStream, byte[] headerName, string headerValue)
		{
			ExportStream.WriteAsciiHeader(headerStream, headerName, headerValue, true);
		}

		private static void WriteAsciiHeader(Stream headerStream, byte[] headerName, string headerValue, bool appendCRLF)
		{
			headerStream.Write(headerName, 0, headerName.Length);
			byte[] array;
			if (appendCRLF)
			{
				array = Util.AsciiStringToBytesAndAppendCRLF(headerValue);
			}
			else
			{
				array = Util.AsciiStringToBytes(headerValue);
			}
			headerStream.Write(array, 0, array.Length);
		}

		private static void WriteExtendedProps(Stream headerStream, byte[] header, IReadOnlyExtendedPropertyCollection extendedProps)
		{
			headerStream.Write(header, 0, header.Length);
			using (EncoderStream encoderStream = new EncoderStream(Streams.CreateSuppressCloseWrapper(headerStream), new Base64Encoder(0), EncoderStreamAccess.Write))
			{
				extendedProps.Serialize(encoderStream);
			}
			headerStream.Write(Util.AsciiCRLF, 0, Util.AsciiCRLF.Length);
		}

		private static void WriteBinary(Stream headerStream, byte[] header, byte[] data)
		{
			headerStream.Write(header, 0, header.Length);
			using (EncoderStream encoderStream = new EncoderStream(Streams.CreateSuppressCloseWrapper(headerStream), new Base64Encoder(0), EncoderStreamAccess.Write))
			{
				encoderStream.Write(data, 0, data.Length);
			}
			headerStream.Write(Util.AsciiCRLF, 0, Util.AsciiCRLF.Length);
		}

		public const string ProductName = "MSExchange";

		public const string Xsender = "X-sender";

		public const string Xreceiver = "X-Receiver";

		public const string XcreatedBy = "X-CreatedBy";

		public const string XheloDomain = "X-HeloDomain";

		public const string XextendedProps = "X-ExtendedProps";

		public const string XextendedPropsParameter = "X-ExtendedProps=";

		public const string XlegacyExch50 = "X-LegacyExch50";

		public const string XxheadersEnd = "X-EndOfInjectedXHeaders";

		public const string Xsource = "X-Source";

		public const string XsourceIPAddress = "X-SourceIPAddress";

		public static readonly int ProductVersion = Assembly.GetExecutingAssembly().GetName().Version.Major;

		public static readonly string CreatedByString = "MSExchange" + ExportStream.ProductVersion.ToString(CultureInfo.InvariantCulture);

		private static readonly byte[] XsenderAsciiHeaderName = Util.AsciiStringToBytes("X-sender: ");

		private static readonly byte[] XreceiverAsciiHeaderName = Util.AsciiStringToBytes("X-Receiver: ");

		private static readonly byte[] XcreatedByAsciiHeaderName = Util.AsciiStringToBytes("X-CreatedBy: ");

		private static readonly byte[] XheloDomainAsciiHeaderName = Util.AsciiStringToBytes("X-HeloDomain: ");

		private static readonly byte[] XextendedPropsAsciiHeaderName = Util.AsciiStringToBytes("X-ExtendedProps: ");

		private static readonly byte[] XextendedPropsAsciiParameter = Util.AsciiStringToBytes("; X-ExtendedProps=");

		private static readonly byte[] XlegacyExch50AsciiHeaderName = Util.AsciiStringToBytes("X-LegacyExch50: ");

		private static readonly byte[] XxheadersEndAsciiHeaderName = Util.AsciiStringToBytes("X-EndOfInjectedXHeaders: ");

		private static readonly byte[] XsourceAsciiHeaderName = Util.AsciiStringToBytes("X-Source: ");

		private static readonly byte[] XsourceIPAddressAsciiHeaderName = Util.AsciiStringToBytes("X-SourceIPAddress: ");

		private Stream headerStream;

		private Stream contentStream;

		private long position;
	}
}
