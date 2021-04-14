using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;

namespace Microsoft.Exchange.Data.ApplicationLogic.UM
{
	internal abstract class VoiceMailPreviewSchema
	{
		public static XmlSchemaSet SchemaSet
		{
			get
			{
				if (VoiceMailPreviewSchema.schemaSet == null)
				{
					lock (VoiceMailPreviewSchema.staticLock)
					{
						if (VoiceMailPreviewSchema.schemaSet == null)
						{
							VoiceMailPreviewSchema.Tracer.TraceDebug(0L, "VoiceMailPreviewSchema: Creating schema set...");
							Assembly executingAssembly = Assembly.GetExecutingAssembly();
							XmlSchemaSet xmlSchemaSet = new XmlSchemaSet();
							using (Stream manifestResourceStream = executingAssembly.GetManifestResourceStream("VoiceMailPreviewSchema.xsd"))
							{
								xmlSchemaSet.Add(SafeXmlSchema.Read(manifestResourceStream, null));
								xmlSchemaSet.Compile();
							}
							VoiceMailPreviewSchema.schemaSet = xmlSchemaSet;
							VoiceMailPreviewSchema.Tracer.TraceDebug(0L, "VoiceMailPreviewSchema: Schema set created successfully.");
						}
					}
				}
				return VoiceMailPreviewSchema.schemaSet;
			}
		}

		internal static bool IsValidTranscription(Stream inputStream)
		{
			VoiceMailPreviewSchema.Tracer.TraceDebug(0L, "VoiceMailPreviewSchema:IsValidTranscription() -> Begin...");
			bool isValid = true;
			try
			{
				XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
				xmlReaderSettings.ValidationType = ValidationType.Schema;
				xmlReaderSettings.IgnoreComments = true;
				xmlReaderSettings.IgnoreWhitespace = true;
				xmlReaderSettings.IgnoreProcessingInstructions = true;
				xmlReaderSettings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
				xmlReaderSettings.Schemas = VoiceMailPreviewSchema.SchemaSet;
				xmlReaderSettings.ValidationEventHandler += delegate(object sender, ValidationEventArgs e)
				{
					isValid = false;
					VoiceMailPreviewSchema.Tracer.TraceWarning<string, XmlSeverityType, XmlSchemaException>(0L, "VoiceMailPreviewSchema:ValidationEventHandler: {0}, {1}, {2}.", e.Message, e.Severity, e.Exception);
				};
				inputStream.Position = 0L;
				using (XmlReader xmlReader = XmlReader.Create(inputStream, xmlReaderSettings))
				{
					while (isValid && xmlReader.Read())
					{
					}
				}
			}
			catch (XmlException arg)
			{
				isValid = false;
				VoiceMailPreviewSchema.Tracer.TraceWarning<XmlException>(0L, "VoiceMailPreviewSchema:IsValidTranscription() -> Error:{0}", arg);
			}
			catch (XmlSchemaException arg2)
			{
				isValid = false;
				VoiceMailPreviewSchema.Tracer.TraceWarning<XmlSchemaException>(0L, "VoiceMailPreviewSchema:IsValidTranscription() -> Error:{0}", arg2);
			}
			catch (SystemException arg3)
			{
				isValid = false;
				VoiceMailPreviewSchema.Tracer.TraceWarning<SystemException>(0L, "VoiceMailPreviewSchema:IsValidTranscription() -> Error:{0}", arg3);
			}
			VoiceMailPreviewSchema.Tracer.TraceDebug<bool>(0L, "VoiceMailPreviewSchema:IsValidTranscription() -> Completed. IsValid:{0}", isValid);
			return isValid;
		}

		public const string MsExchangeEvmProductId = "925712";

		public const string XmlNamespace = "http://schemas.microsoft.com/exchange/um/2010/evm";

		public const string ResourceName = "VoiceMailPreviewSchema.xsd";

		private static readonly Trace Tracer = ExTraceGlobals.UMPartnerMessageTracer;

		private static object staticLock = new object();

		private static XmlSchemaSet schemaSet;

		internal abstract class XPath
		{
			public const string Prefix = "evm";

			public const string SelectLanguage = "//evm:ASR/@lang";

			public const string SelectConfidence = "//evm:ASR/@confidence";

			public const string SelectRecoResult = "//evm:ASR/@recognitionResult";

			public const string SelectRecoError = "//evm:ASR/@recognitionError";

			public const string SelectErrorInfo = "//evm:ASR/evm:ErrorInformation";
		}

		internal abstract class InternalXml
		{
			private static string GetTranscriptionSkippedXml(string transcriptionError)
			{
				return string.Format(CultureInfo.InvariantCulture, "<?xml version=\"1.0\" encoding=\"utf-8\"?><ASR lang=\"en-US\" confidence=\"0.0\" recognitionResult=\"skipped\" recognitionError=\"{0}\" schemaVersion=\"1.0.0.0\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"http://schemas.microsoft.com/exchange/um/2010/evm\"></ASR>", new object[]
				{
					transcriptionError
				});
			}

			public const string TimeoutStatus = "504 Timeout";

			private const string TranscriptionSkippedTemplate = "<?xml version=\"1.0\" encoding=\"utf-8\"?><ASR lang=\"en-US\" confidence=\"0.0\" recognitionResult=\"skipped\" recognitionError=\"{0}\" schemaVersion=\"1.0.0.0\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"http://schemas.microsoft.com/exchange/um/2010/evm\"></ASR>";

			public static readonly string TimeoutTranscription = VoiceMailPreviewSchema.InternalXml.GetTranscriptionSkippedXml("timeout");

			public static readonly string MessageTooLongTranscription = VoiceMailPreviewSchema.InternalXml.GetTranscriptionSkippedXml("messagetoolong");

			public static readonly string ProtectedVoiceMailTranscription = VoiceMailPreviewSchema.InternalXml.GetTranscriptionSkippedXml("protectedvoicemail");

			public static readonly string ErrorReadingSettingsTranscription = VoiceMailPreviewSchema.InternalXml.GetTranscriptionSkippedXml("errorreadingsettings");
		}
	}
}
