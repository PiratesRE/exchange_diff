using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.AirSync
{
	internal sealed class ValidateCertCommand : Command
	{
		internal ValidateCertCommand()
		{
		}

		internal override int MinVersion
		{
			get
			{
				return 25;
			}
		}

		protected override string RootNodeName
		{
			get
			{
				return "ValidateCert";
			}
		}

		internal override Command.ExecutionState ExecuteCommand()
		{
			this.ParseXmlRequest();
			this.ProcessCommand();
			this.BuildXmlResponse();
			return Command.ExecutionState.Complete;
		}

		protected override bool HandleQuarantinedState()
		{
			this.globalStatus = "17";
			this.BuildXmlResponse();
			return false;
		}

		internal override XmlDocument GetValidationErrorXml()
		{
			if (ValidateCertCommand.validationErrorXml == null)
			{
				XmlDocument commandXmlStub = base.GetCommandXmlStub();
				XmlElement xmlElement = commandXmlStub.CreateElement("Status", this.RootNodeNamespace);
				xmlElement.InnerText = "2";
				commandXmlStub[this.RootNodeName].AppendChild(xmlElement);
				ValidateCertCommand.validationErrorXml = commandXmlStub;
			}
			return ValidateCertCommand.validationErrorXml;
		}

		private void ParseCertNodes(XmlNode containerNode, List<string> certList)
		{
			foreach (object obj in containerNode.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				certList.Add(xmlNode.InnerText);
				if (certList.Count >= GlobalSettings.MaxRetrievedItems)
				{
					throw new AirSyncPermanentException(StatusCode.Sync_ProtocolVersionMismatch, this.GetValidationErrorXml(), null, false)
					{
						ErrorStringForProtocolLogger = "VCTooManyCertificates"
					};
				}
			}
		}

		private void ParseXmlRequest()
		{
			for (XmlNode xmlNode = base.XmlRequest.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				if (xmlNode.LocalName == "CertificateChain")
				{
					base.ProtocolLogger.SetValue(ProtocolLoggerData.VCertChains, xmlNode.ChildNodes.Count);
					this.ParseCertNodes(xmlNode, this.certChainCerts);
				}
				else if (xmlNode.LocalName == "Certificates")
				{
					base.ProtocolLogger.SetValue(ProtocolLoggerData.VCerts, xmlNode.ChildNodes.Count);
					this.ParseCertNodes(xmlNode, this.endCerts);
				}
				else
				{
					if (!(xmlNode.LocalName == "CheckCrl"))
					{
						throw new AirSyncPermanentException(StatusCode.Sync_ProtocolVersionMismatch, this.GetValidationErrorXml(), null, false)
						{
							ErrorStringForProtocolLogger = "BadNode(" + xmlNode.LocalName + ")InValidateCert"
						};
					}
					base.ProtocolLogger.SetValue(ProtocolLoggerData.VCertCRL, 1);
					this.checkCRL = (xmlNode.InnerText == "1");
				}
			}
		}

		private void ExtendCertChainFromConfiguration()
		{
			if (this.smimeCertCAFullLoaded)
			{
				return;
			}
			if (this.SmimeConfiguration != null)
			{
				string text = this.SmimeConfiguration.SMIMECertificateIssuingCAFull();
				if (!string.IsNullOrWhiteSpace(text))
				{
					this.trustedCerts.Add(text);
					this.smimeCertCAFullLoaded = true;
				}
			}
		}

		private void ProcessCommand()
		{
			this.globalStatus = "1";
			try
			{
				this.ExtendCertChainFromConfiguration();
				bool enabled = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Global.MultiTenancy.Enabled;
				this.perCertStatuses = CertificateManager.ValidateCertificates(this.trustedCerts, this.certChainCerts, this.endCerts, this.checkCRL, this.GetHashCode(), base.MailboxLogger, this.smimeCertCAFullLoaded || enabled, base.User.OrganizationId.ToString());
			}
			catch (LocalizedException ex)
			{
				if (base.MailboxLogger != null)
				{
					base.MailboxLogger.SetData(MailboxLogDataName.ValidateCertCommand_ProcessCommand_Exception, ex.ToString());
				}
				AirSyncDiagnostics.TraceDebug<LocalizedException>(ExTraceGlobals.RequestsTracer, this, "Failed to validate certificate: '{0}'", ex);
				this.globalStatus = "17";
			}
		}

		private SmimeConfigurationContainer SmimeConfiguration
		{
			get
			{
				if (this.smimeConfiguration == null)
				{
					this.smimeConfiguration = CertificateManager.LoadSmimeConfiguration(base.User.OrganizationId, this.GetHashCode());
				}
				return this.smimeConfiguration;
			}
		}

		private void BuildXmlResponse()
		{
			base.XmlResponse = new SafeXmlDocument();
			this.validateCertNode = base.XmlResponse.CreateElement("ValidateCert", "ValidateCert:");
			base.XmlResponse.AppendChild(this.validateCertNode);
			XmlElement xmlElement = base.XmlResponse.CreateElement("Status", "ValidateCert:");
			xmlElement.InnerText = this.globalStatus;
			this.validateCertNode.AppendChild(xmlElement);
			foreach (ChainValidityStatus status in this.perCertStatuses)
			{
				XmlElement xmlElement2 = base.XmlResponse.CreateElement("Certificate", "ValidateCert:");
				this.validateCertNode.AppendChild(xmlElement2);
				XmlElement xmlElement3 = base.XmlResponse.CreateElement("Status", "ValidateCert:");
				xmlElement3.InnerText = this.CheckStatus(status);
				xmlElement2.AppendChild(xmlElement3);
			}
		}

		private string CheckStatus(ChainValidityStatus status)
		{
			if (status <= (ChainValidityStatus)2148081683U)
			{
				switch (status)
				{
				case ChainValidityStatus.Valid:
				case ChainValidityStatus.ValidSelfSigned:
					return "1";
				case ChainValidityStatus.EmptyCertificate:
					return "10";
				default:
					switch (status)
					{
					case (ChainValidityStatus)2148081680U:
						return "13";
					case (ChainValidityStatus)2148081681U:
						goto IL_ED;
					case (ChainValidityStatus)2148081682U:
						break;
					case (ChainValidityStatus)2148081683U:
						return "14";
					default:
						goto IL_ED;
					}
					break;
				}
			}
			else
			{
				if (status == (ChainValidityStatus)2148098052U)
				{
					return "3";
				}
				switch (status)
				{
				case (ChainValidityStatus)2148204801U:
					return "7";
				case (ChainValidityStatus)2148204802U:
					return "8";
				case (ChainValidityStatus)2148204803U:
					return "11";
				case (ChainValidityStatus)2148204804U:
				case (ChainValidityStatus)2148204805U:
				case (ChainValidityStatus)2148204807U:
				case (ChainValidityStatus)2148204808U:
				case (ChainValidityStatus)2148204811U:
					goto IL_ED;
				case (ChainValidityStatus)2148204806U:
					return "9";
				case (ChainValidityStatus)2148204809U:
				case (ChainValidityStatus)2148204813U:
					return "4";
				case (ChainValidityStatus)2148204810U:
					return "5";
				case (ChainValidityStatus)2148204812U:
					return "15";
				case (ChainValidityStatus)2148204814U:
					break;
				case (ChainValidityStatus)2148204815U:
					return "12";
				case (ChainValidityStatus)2148204816U:
					return "6";
				default:
					goto IL_ED;
				}
			}
			return "16";
			IL_ED:
			AirSyncDiagnostics.TraceDebug<ChainValidityStatus>(ExTraceGlobals.RequestsTracer, this, "Unknown status: '{0}'", status);
			return "17";
		}

		private static XmlDocument validationErrorXml;

		private XmlElement validateCertNode;

		private List<string> certChainCerts = new List<string>(10);

		private List<string> trustedCerts = new List<string>(10);

		private List<string> endCerts = new List<string>(10);

		private bool checkCRL;

		private string globalStatus;

		private List<ChainValidityStatus> perCertStatuses = new List<ChainValidityStatus>(10);

		private SmimeConfigurationContainer smimeConfiguration;

		private bool smimeCertCAFullLoaded;

		private struct Status
		{
			public const string InvalidStatus = "0";

			public const string Success = "1";

			public const string ProtocolError = "2";

			public const string SignatureNotValidated = "3";

			public const string FromUntrustedSource = "4";

			public const string InvalidCertChain = "5";

			public const string InvalidForSigning = "6";

			public const string ExpiredOrInvalid = "7";

			public const string InvalidTimePeriods = "8";

			public const string PurposeError = "9";

			public const string MissingInfo = "10";

			public const string WrongRole = "11";

			public const string NotMatch = "12";

			public const string Revoked = "13";

			public const string NoServerContact = "14";

			public const string ChainRevoked = "15";

			public const string NoRevocationStatus = "16";

			public const string UnknowServerError = "17";
		}
	}
}
