using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data.ApplicationLogic.UM;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class PartnerTranscriptionData : ITranscriptionData
	{
		internal PartnerTranscriptionData(Stream stream) : this(PartnerTranscriptionData.GetTranscriptionXmlFromStream(stream))
		{
		}

		internal PartnerTranscriptionData(string xml) : this(PartnerTranscriptionData.GetTranscriptionXmlFromText(xml))
		{
		}

		internal PartnerTranscriptionData(XmlDocument transcriptionXml)
		{
			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(transcriptionXml.NameTable);
			xmlNamespaceManager.AddNamespace("evm", "http://schemas.microsoft.com/exchange/um/2010/evm");
			this.Confidence = 0f;
			this.RecognitionResult = RecoResultType.Skipped;
			this.RecognitionError = RecoErrorType.Other;
			this.TranscriptionXml = transcriptionXml;
			XmlNode xmlNode = this.TranscriptionXml.SelectSingleNode("//evm:ASR/@lang", xmlNamespaceManager);
			if (xmlNode == null)
			{
				throw new ArgumentException("transcriptionXml");
			}
			this.Language = new CultureInfo(xmlNode.Value);
			float confidence = 0f;
			xmlNode = this.TranscriptionXml.SelectSingleNode("//evm:ASR/@confidence", xmlNamespaceManager);
			if (xmlNode == null || !float.TryParse(xmlNode.Value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out confidence))
			{
				throw new ArgumentException("transcriptionXml");
			}
			this.Confidence = confidence;
			xmlNode = this.TranscriptionXml.SelectSingleNode("//evm:ASR/@recognitionResult", xmlNamespaceManager);
			if (xmlNode == null)
			{
				throw new ArgumentException("transcriptionXml");
			}
			this.RecognitionResult = EnumValidator<RecoResultType>.Parse(xmlNode.Value, EnumParseOptions.IgnoreCase);
			xmlNode = this.TranscriptionXml.SelectSingleNode("//evm:ASR/@recognitionError", xmlNamespaceManager);
			if (xmlNode != null)
			{
				this.RecognitionError = EnumValidator<RecoErrorType>.Parse(xmlNode.Value, EnumParseOptions.IgnoreCase);
			}
			xmlNode = this.TranscriptionXml.SelectSingleNode("//evm:ASR/evm:ErrorInformation", xmlNamespaceManager);
			if (xmlNode != null)
			{
				this.ErrorInformation = xmlNode.InnerText;
			}
		}

		public RecoResultType RecognitionResult { get; private set; }

		public float Confidence { get; private set; }

		public CultureInfo Language { get; private set; }

		public ConfidenceBandType ConfidenceBand
		{
			get
			{
				if ((double)this.Confidence > LocConfig.Instance[this.Language].Transcription.HighConfidence)
				{
					return ConfidenceBandType.High;
				}
				if ((double)this.Confidence > LocConfig.Instance[this.Language].Transcription.LowConfidence)
				{
					return ConfidenceBandType.Medium;
				}
				return ConfidenceBandType.Low;
			}
		}

		public XmlDocument TranscriptionXml { get; private set; }

		public RecoErrorType RecognitionError { get; private set; }

		public string ErrorInformation { get; private set; }

		internal static bool IsPartnerError(RecoErrorType error)
		{
			return error == RecoErrorType.Rejected || error == RecoErrorType.SystemError || error == RecoErrorType.Timeout || error == RecoErrorType.BadRequest || error == RecoErrorType.Other;
		}

		private static XmlDocument GetTranscriptionXmlFromStream(Stream inputStream)
		{
			if (!VoiceMailPreviewSchema.IsValidTranscription(inputStream))
			{
				throw new ArgumentException("inputStream");
			}
			XmlDocument xmlDocument = new SafeXmlDocument();
			inputStream.Position = 0L;
			xmlDocument.Load(inputStream);
			return xmlDocument;
		}

		private static XmlDocument GetTranscriptionXmlFromText(string xml)
		{
			XmlDocument transcriptionXmlFromStream;
			using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
			{
				transcriptionXmlFromStream = PartnerTranscriptionData.GetTranscriptionXmlFromStream(memoryStream);
			}
			return transcriptionXmlFromStream;
		}
	}
}
