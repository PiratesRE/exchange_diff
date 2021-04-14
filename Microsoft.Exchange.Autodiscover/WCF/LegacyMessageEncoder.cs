using System;
using System.Globalization;
using System.IO;
using System.ServiceModel.Channels;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Schema;
using Microsoft.Exchange.Diagnostics.Components.Autodiscover;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	public class LegacyMessageEncoder : MessageEncoder
	{
		public LegacyMessageEncoder(MessageVersion version)
		{
			this.version = version;
			this.Initialize();
		}

		private void Initialize()
		{
			this.mediaType = "text/xml";
			this.contentType = string.Format(CultureInfo.InvariantCulture, "{0}; charset={1}", new object[]
			{
				this.mediaType,
				Encoding.UTF8.WebName
			});
			TextMessageEncodingBindingElement textMessageEncodingBindingElement = new TextMessageEncodingBindingElement(this.MessageVersion, Encoding.UTF8);
			MessageEncoderFactory messageEncoderFactory = textMessageEncodingBindingElement.CreateMessageEncoderFactory();
			this.textEncoder = messageEncoderFactory.Encoder;
			this.xmlSettings = new XmlWriterSettings();
			this.xmlSettings.Indent = true;
			this.xmlSettings.IndentChars = "  ";
			this.xmlSettings.OmitXmlDeclaration = false;
			this.xmlSettings.ConformanceLevel = ConformanceLevel.Document;
			this.xmlSettings.Encoding = LegacyMessageEncoder.utf8Encoding;
		}

		public override string ContentType
		{
			get
			{
				return this.contentType;
			}
		}

		public override string MediaType
		{
			get
			{
				return this.mediaType;
			}
		}

		public override MessageVersion MessageVersion
		{
			get
			{
				return this.version;
			}
		}

		public override bool IsContentTypeSupported(string contentType)
		{
			return base.IsContentTypeSupported(contentType) || (contentType.Trim().Length == this.MediaType.Length && contentType.Trim().Equals(this.MediaType, StringComparison.OrdinalIgnoreCase));
		}

		public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
		{
			MemoryStream stream = null;
			Common.SendWatsonReportOnUnhandledException(delegate
			{
				byte[] array = new byte[buffer.Count];
				Array.Copy(buffer.Array, buffer.Offset, array, 0, array.Length);
				bufferManager.ReturnBuffer(buffer.Array);
				stream = new MemoryStream(array);
			});
			return this.ReadMessage(stream, int.MaxValue, contentType);
		}

		public override Message ReadMessage(Stream stream, int maxSizeOfHeaders, string streamContentType)
		{
			Message message = null;
			Common.SendWatsonReportOnUnhandledException(delegate
			{
				RequestData property;
				bool flag = this.ValidateAndParseRequest(stream, out property);
				MemoryStream memoryStream = new MemoryStream();
				XmlWriter xmlWriter = XmlWriter.Create(memoryStream);
				xmlWriter.WriteStartDocument();
				xmlWriter.WriteStartElement("Input");
				xmlWriter.WriteEndElement();
				xmlWriter.Flush();
				memoryStream.Flush();
				memoryStream.Seek(0L, SeekOrigin.Begin);
				message = this.textEncoder.ReadMessage(memoryStream, maxSizeOfHeaders, streamContentType);
				message.Properties.Add("ValidationError", this.validationError);
				message.Properties.Add("ParseSuccess", flag);
				message.Properties.Add("RequestData", property);
			});
			return message;
		}

		public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
		{
			MemoryStream memoryStream = new MemoryStream();
			XmlWriter xmlWriter = XmlWriter.Create(memoryStream, this.xmlSettings);
			message.WriteMessage(xmlWriter);
			xmlWriter.Flush();
			xmlWriter.Dispose();
			byte[] buffer = memoryStream.GetBuffer();
			int num = (int)memoryStream.Position;
			memoryStream.Dispose();
			int bufferSize = num + messageOffset;
			byte[] array = bufferManager.TakeBuffer(bufferSize);
			Array.Copy(buffer, 0, array, messageOffset, num);
			ArraySegment<byte> result = new ArraySegment<byte>(array, messageOffset, num);
			return result;
		}

		public override void WriteMessage(Message message, Stream stream)
		{
			XmlWriter xmlWriter = XmlWriter.Create(stream, this.xmlSettings);
			message.WriteMessage(xmlWriter);
			xmlWriter.Flush();
		}

		private bool ValidateAndParseRequest(Stream stream, out RequestData requestData)
		{
			Stream stream2 = null;
			bool useClientCertificateAuthentication = Common.CheckClientCertificate(HttpContext.Current.Request);
			requestData = new RequestData(null, useClientCertificateAuthentication, CallerRequestedCapabilities.GetInstance(HttpContext.Current));
			string a = string.Empty;
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			xmlReaderSettings.ValidationType = ValidationType.Schema;
			xmlReaderSettings.CheckCharacters = true;
			xmlReaderSettings.IgnoreWhitespace = true;
			xmlReaderSettings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
			xmlReaderSettings.ConformanceLevel = ConformanceLevel.Document;
			xmlReaderSettings.Schemas = ProvidersTable.RequestSchemaSet;
			xmlReaderSettings.ValidationEventHandler += this.ValidationEventHandler;
			requestData.UserAuthType = HttpContext.Current.Request.ServerVariables["AUTH_TYPE"];
			string text = HttpContext.Current.Request.Headers.Get("X-MapiHttpCapability");
			int num;
			if (string.IsNullOrEmpty(text) || !int.TryParse(text, out num) || num < 0)
			{
				num = 0;
			}
			requestData.MapiHttpVersion = num;
			if (!requestData.CallerCapabilities.CanFollowRedirect)
			{
				requestData.ProxyRequestData = new ProxyRequestData(HttpContext.Current.Request, HttpContext.Current.Response, ref stream);
				stream2 = requestData.ProxyRequestData.RequestStream;
				stream = stream2;
			}
			this.validationError = false;
			try
			{
				XmlReader xmlReader = XmlReader.Create(stream, xmlReaderSettings);
				while (!xmlReader.EOF && !this.validationError)
				{
					switch (xmlReader.NodeType)
					{
					case XmlNodeType.Element:
						a = xmlReader.LocalName;
						while (xmlReader.MoveToNextAttribute())
						{
							if (xmlReader.Name == "xmlns")
							{
								requestData.RequestSchemas.Add(xmlReader.Value);
							}
						}
						break;
					case XmlNodeType.Text:
						if (xmlReader.HasValue)
						{
							if (a == "AcceptableResponseSchema")
							{
								requestData.ResponseSchemas.Add(xmlReader.Value);
							}
							else if (a == "EMailAddress")
							{
								requestData.EMailAddress = xmlReader.Value;
							}
							else if (a == "LegacyDN")
							{
								requestData.LegacyDN = xmlReader.Value;
							}
						}
						break;
					}
					xmlReader.Read();
				}
			}
			catch (XmlException ex)
			{
				ExTraceGlobals.FrameworkTracer.TraceError<string, int, int>((long)this.GetHashCode(), "[ValidateAndParseRequest()] Message=\"{0}\";LineNumber=\"{1}\";LinePosition=\"{2}\"", ex.Message, ex.LineNumber, ex.LinePosition);
				Common.EventLog.LogEvent(AutodiscoverEventLogConstants.Tuple_WarnCoreValidationException, Common.PeriodicKey, new object[]
				{
					ex.Message
				});
				return false;
			}
			finally
			{
				if (stream2 != null)
				{
					stream2.Dispose();
				}
			}
			if (this.validationError)
			{
				return false;
			}
			if (string.IsNullOrEmpty(requestData.EMailAddress) && string.IsNullOrEmpty(requestData.LegacyDN))
			{
				ExTraceGlobals.FrameworkTracer.TraceError<string, string>((long)this.GetHashCode(), "[ValidateAndParseRequest()] 'Both \"{0}\" and \"{1}\" are empty'", "EMailAddress", "LegacyDN");
				Common.EventLog.LogEvent(AutodiscoverEventLogConstants.Tuple_WarnCoreElementsAreEmpty, Common.PeriodicKey, new object[]
				{
					"EMailAddress,LegacyDN"
				});
			}
			else
			{
				if (requestData.ResponseSchemas.Count != 0 && !string.IsNullOrEmpty(requestData.ResponseSchemas[0]))
				{
					return true;
				}
				ExTraceGlobals.FrameworkTracer.TraceError<string>((long)this.GetHashCode(), "[ValidateAndParseRequest()] 'Element \"{0}\" is empty'", "AcceptableResponseSchema");
				Common.EventLog.LogEvent(AutodiscoverEventLogConstants.Tuple_WarnCoreElementIsEmpty, Common.PeriodicKey, new object[]
				{
					"AcceptableResponseSchema"
				});
			}
			return false;
		}

		private void ValidationEventHandler(object sender, ValidationEventArgs e)
		{
			ExTraceGlobals.FrameworkTracer.TraceError((long)this.GetHashCode(), "[ValidationEventHandler()] 'ValidationError' Severity=\"{0}\";Message=\"{1}\";LineNumber=\"{2}\";LinePosition=\"{3}\"", new object[]
			{
				e.Severity,
				e.Message,
				e.Exception.LineNumber,
				e.Exception.LinePosition
			});
			Common.EventLog.LogEvent(AutodiscoverEventLogConstants.Tuple_WarnCoreValidationError, Common.PeriodicKey, new object[]
			{
				e.Severity.ToString(),
				e.Message,
				e.Exception.LineNumber.ToString(),
				e.Exception.LinePosition.ToString()
			});
			this.validationError = true;
		}

		private static readonly UTF8Encoding utf8Encoding = new UTF8Encoding(false);

		private MessageEncoder textEncoder;

		private MessageVersion version;

		private string contentType;

		private string mediaType;

		private bool validationError;

		private XmlWriterSettings xmlSettings;
	}
}
