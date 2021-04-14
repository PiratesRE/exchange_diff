using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public class SmtpConnectionProbeWorkDefinition
	{
		public SmtpConnectionProbeWorkDefinition(string xml, bool loadFromXml = true)
		{
			this.LoadDefaultConfiguration();
			if (loadFromXml)
			{
				this.LoadContext(xml);
			}
		}

		public bool ResolveEndPoint
		{
			get
			{
				return this.resolveEndPoint;
			}
			internal set
			{
				this.resolveEndPoint = value;
			}
		}

		public string SmtpServer
		{
			get
			{
				return this.smtpServer;
			}
			internal set
			{
				this.smtpServer = value;
			}
		}

		public int Port
		{
			get
			{
				return this.port;
			}
			internal set
			{
				this.port = value;
			}
		}

		public AuthenticationType AuthenticationType
		{
			get
			{
				return this.authenticationType;
			}
			internal set
			{
				this.authenticationType = value;
			}
		}

		public ConnectionLostPoint ExpectedConnectionLostPoint
		{
			get
			{
				return this.expectedConnectionLostPoint;
			}
			internal set
			{
				this.expectedConnectionLostPoint = value;
			}
		}

		public string HeloDomain
		{
			get
			{
				return this.heloDomain;
			}
			internal set
			{
				this.heloDomain = value;
			}
		}

		public Account AuthenticationAccount
		{
			get
			{
				return this.authenticationAccount;
			}
			internal set
			{
				this.authenticationAccount = value;
			}
		}

		public string MailFrom
		{
			get
			{
				return this.mailFrom;
			}
			internal set
			{
				this.mailFrom = value;
			}
		}

		public ICollection<SmtpRecipient> MailTo
		{
			get
			{
				return this.mailTo;
			}
			internal set
			{
				this.mailTo = value;
			}
		}

		public bool UseSsl
		{
			get
			{
				return this.useSsl;
			}
			internal set
			{
				this.useSsl = value;
			}
		}

		public string Data
		{
			get
			{
				return this.data;
			}
			internal set
			{
				this.data = value;
			}
		}

		public SmtpExpectedResponse ExpectedResponseOnConnect
		{
			get
			{
				return this.expectedResponseOnConnect;
			}
			internal set
			{
				this.expectedResponseOnConnect = value;
			}
		}

		public SmtpExpectedResponse ExpectedResponseOnHelo
		{
			get
			{
				return this.expectedResponseOnHelo;
			}
			internal set
			{
				this.expectedResponseOnHelo = value;
			}
		}

		public SmtpExpectedResponse ExpectedResponseOnStartTls
		{
			get
			{
				return this.expectedResponseOnStartTls;
			}
			internal set
			{
				this.expectedResponseOnStartTls = value;
			}
		}

		public SmtpExpectedResponse ExpectedResponseOnHeloAfterStartTls
		{
			get
			{
				return this.expectedResponseOnHeloAfterStartTls;
			}
			internal set
			{
				this.expectedResponseOnHeloAfterStartTls = value;
			}
		}

		public SmtpExpectedResponse ExpectedResponseOnAuthenticate
		{
			get
			{
				return this.expectedResponseOnAuthenticate;
			}
			internal set
			{
				this.expectedResponseOnAuthenticate = value;
			}
		}

		public SmtpExpectedResponse ExpectedResponseOnMailFrom
		{
			get
			{
				return this.expectedResponseOnMailFrom;
			}
			internal set
			{
				this.expectedResponseOnMailFrom = value;
			}
		}

		public SmtpExpectedResponse ExpectedResponseOnData
		{
			get
			{
				return this.expectedResponseOnData;
			}
			internal set
			{
				this.expectedResponseOnData = value;
			}
		}

		public bool ClientCertificateValid
		{
			get
			{
				return this.clientCertificateValid;
			}
			internal set
			{
				this.clientCertificateValid = value;
			}
		}

		public ClientCertificateCriteria ClientCertificate
		{
			get
			{
				return this.clientCertificate;
			}
			internal set
			{
				this.clientCertificate = value;
			}
		}

		public bool ExpectedServerCertificateValid
		{
			get
			{
				return this.expectedServerCertificateValid;
			}
			internal set
			{
				this.expectedServerCertificateValid = value;
			}
		}

		public SmtpConnectionProbeWorkDefinition.CertificateProperties ExpectedServerCertificate
		{
			get
			{
				return this.expectedServerCertificate;
			}
			internal set
			{
				this.expectedServerCertificate = value;
			}
		}

		public ICollection<SmtpCustomCommand> CustomCommands
		{
			get
			{
				return this.customCommands;
			}
			internal set
			{
				this.customCommands = value;
			}
		}

		public bool IgnoreCertificateNameMismatchPolicyError
		{
			get
			{
				return this.ignoreCertificateNameMismatchPolicyError;
			}
			internal set
			{
				this.ignoreCertificateNameMismatchPolicyError = value;
			}
		}

		public bool IgnoreCertificateChainPolicyErrorForSelfSigned
		{
			get
			{
				return this.ignoreCertificateChainPolicyErrorForSelfSigned;
			}
			internal set
			{
				this.ignoreCertificateChainPolicyErrorForSelfSigned = value;
			}
		}

		public string SenderTenantID
		{
			get
			{
				return this.senderTenantID;
			}
			internal set
			{
				this.senderTenantID = value;
			}
		}

		public string RecipientTenantID
		{
			get
			{
				return this.recipientTenantID;
			}
			internal set
			{
				this.recipientTenantID = value;
			}
		}

		public bool AddAttributions
		{
			get
			{
				return this.addAttributions;
			}
			internal set
			{
				this.addAttributions = value;
			}
		}

		public Directionality Direction
		{
			get
			{
				return this.direction;
			}
			internal set
			{
				this.direction = value;
			}
		}

		public void LoadDefaultConfiguration()
		{
			this.smtpServer = "127.0.0.1";
			this.resolveEndPoint = false;
			this.port = 25;
			this.useSsl = false;
			this.ignoreCertificateNameMismatchPolicyError = false;
			this.ignoreCertificateChainPolicyErrorForSelfSigned = false;
			this.authenticationType = AuthenticationType.Anonymous;
			this.expectedConnectionLostPoint = ConnectionLostPoint.None;
			this.heloDomain = string.Empty;
			this.mailFrom = string.Empty;
			this.mailTo = new List<SmtpRecipient>();
			this.data = string.Empty;
			this.addAttributions = true;
			this.direction = Directionality.Incoming;
			this.expectedResponseOnConnect = new SmtpExpectedResponse
			{
				Type = ExpectedResponseType.ResponseCode,
				ResponseCode = SimpleSmtpClient.SmtpResponseCode.ServiceReady
			};
			this.expectedResponseOnHelo = new SmtpExpectedResponse
			{
				Type = ExpectedResponseType.ResponseCode,
				ResponseCode = SimpleSmtpClient.SmtpResponseCode.OK
			};
			this.expectedResponseOnStartTls = new SmtpExpectedResponse
			{
				Type = ExpectedResponseType.ResponseCode,
				ResponseCode = SimpleSmtpClient.SmtpResponseCode.ServiceReady
			};
			this.expectedResponseOnHeloAfterStartTls = new SmtpExpectedResponse
			{
				Type = ExpectedResponseType.ResponseCode,
				ResponseCode = SimpleSmtpClient.SmtpResponseCode.OK
			};
			this.expectedResponseOnAuthenticate = new SmtpExpectedResponse
			{
				Type = ExpectedResponseType.ResponseCode,
				ResponseCode = SimpleSmtpClient.SmtpResponseCode.AuthAccepted
			};
			this.expectedResponseOnMailFrom = new SmtpExpectedResponse
			{
				Type = ExpectedResponseType.ResponseCode,
				ResponseCode = SimpleSmtpClient.SmtpResponseCode.OK
			};
			this.expectedResponseOnData = new SmtpExpectedResponse
			{
				Type = ExpectedResponseType.ResponseCode,
				ResponseCode = SimpleSmtpClient.SmtpResponseCode.OK
			};
			this.clientCertificateValid = false;
			this.clientCertificate = new ClientCertificateCriteria();
			this.expectedServerCertificateValid = false;
			this.expectedServerCertificate = new SmtpConnectionProbeWorkDefinition.CertificateProperties();
			this.customCommands = new List<SmtpCustomCommand>();
		}

		public void LoadContext(string xml)
		{
			if (string.IsNullOrWhiteSpace(xml))
			{
				throw new ArgumentException("Work Definition XML is not valid.");
			}
			XmlDocument xmlDocument = new SafeXmlDocument();
			xmlDocument.LoadXml(xml);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("//SmtpServer");
			this.smtpServer = "127.0.0.1";
			if (xmlNode != null && !string.IsNullOrWhiteSpace(xmlNode.InnerText))
			{
				this.smtpServer = xmlNode.InnerText;
			}
			XmlAttribute xmlAttribute = xmlNode.Attributes["ResolveEndPoint"];
			if (xmlAttribute != null)
			{
				this.resolveEndPoint = Utils.GetBoolean(xmlAttribute.Value, "ResolveEndPoint", false);
			}
			xmlNode = xmlDocument.SelectSingleNode("//Port");
			this.port = 25;
			if (xmlNode != null)
			{
				this.port = Utils.GetPositiveInteger(xmlNode.InnerText, "Port");
			}
			xmlNode = xmlDocument.SelectSingleNode("//UseSsl");
			if (xmlNode != null)
			{
				this.useSsl = Utils.GetBoolean(xmlNode.InnerText, "UseSsl", false);
				this.ignoreCertificateNameMismatchPolicyError = Utils.GetOptionalXmlAttribute<bool>(xmlNode, "IgnoreCertificateNameMismatchPolicyError", false);
				this.ignoreCertificateChainPolicyErrorForSelfSigned = Utils.GetOptionalXmlAttribute<bool>(xmlNode, "IgnoreCertificateChainPolicyErrorForSelfSigned", false);
			}
			xmlNode = xmlDocument.SelectSingleNode("//AuthenticationType");
			this.authenticationType = AuthenticationType.Anonymous;
			if (xmlNode != null)
			{
				this.authenticationType = Utils.GetEnumValue<AuthenticationType>(xmlNode.InnerText, "AuthenticationType");
			}
			xmlNode = xmlDocument.SelectSingleNode("//AuthenticationAccount");
			if (xmlNode != null)
			{
				this.authenticationAccount = Account.FromXml(xmlNode);
			}
			if (this.authenticationType == AuthenticationType.AuthLogin && (this.authenticationAccount == null || string.IsNullOrEmpty(this.authenticationAccount.Password)))
			{
				throw new ArgumentException(string.Format("Authentication account must be provided when authentication type is {0}.", this.authenticationType));
			}
			xmlNode = xmlDocument.SelectSingleNode("//ExpectedConnectionLostPoint");
			this.expectedConnectionLostPoint = ConnectionLostPoint.None;
			if (xmlNode != null)
			{
				this.expectedConnectionLostPoint = Utils.GetEnumValue<ConnectionLostPoint>(xmlNode.InnerText, "ExpectedConnectionLostPoint");
			}
			xmlNode = xmlDocument.SelectSingleNode("//HeloDomain");
			this.heloDomain = string.Empty;
			if (xmlNode != null)
			{
				this.heloDomain = xmlNode.InnerText;
			}
			XmlElement xmlElement = xmlDocument.SelectSingleNode("//MailFrom") as XmlElement;
			if (xmlElement != null)
			{
				this.mailFrom = Utils.CheckNullOrWhiteSpace(xmlElement.GetAttribute("Username"), "MailFrom Username");
			}
			SmtpExpectedResponse smtpExpectedResponse = SmtpExpectedResponse.FromXml(xmlDocument.SelectSingleNode("//ExpectedResponseOnRcptTo"), "ExpectedResponseOnRcptTo", SimpleSmtpClient.SmtpResponseCode.OK, false);
			List<SmtpRecipient> list = new List<SmtpRecipient>();
			foreach (object obj in xmlDocument.SelectNodes("//MailTo"))
			{
				XmlElement xmlElement2 = (XmlElement)obj;
				SmtpExpectedResponse expectedResponse = SmtpExpectedResponse.FromXml(xmlElement2.SelectSingleNode("ExpectedResponse"), "ExpectedResponse", smtpExpectedResponse.ResponseCode, false);
				SmtpRecipient item = new SmtpRecipient
				{
					Username = Utils.CheckNullOrWhiteSpace(xmlElement2.GetAttribute("Username"), "MailTo Username"),
					ExpectedResponse = expectedResponse
				};
				list.Add(item);
			}
			this.mailTo = list;
			xmlNode = xmlDocument.SelectSingleNode("//WorkContext");
			if (xmlNode != null)
			{
				this.senderTenantID = Utils.GetOptionalXmlAttribute<string>(xmlNode, "SenderTenantID", string.Empty);
				this.recipientTenantID = Utils.GetOptionalXmlAttribute<string>(xmlNode, "RecipientTenantID", string.Empty);
			}
			xmlNode = xmlDocument.SelectSingleNode("//Data");
			if (xmlNode != null)
			{
				this.data = Utils.CheckNullOrWhiteSpace(xmlNode.InnerText, "Data");
				this.addAttributions = Utils.GetOptionalXmlAttribute<bool>(xmlNode, "AddAttributions", true);
				this.direction = Utils.GetOptionalXmlEnumAttribute<Directionality>(xmlNode, "Direction", Directionality.Incoming);
			}
			this.expectedResponseOnConnect = SmtpExpectedResponse.FromXml(xmlDocument.SelectSingleNode("//ExpectedResponseOnConnect"), "ExpectedResponseOnConnect", SimpleSmtpClient.SmtpResponseCode.ServiceReady, false);
			this.expectedResponseOnHelo = SmtpExpectedResponse.FromXml(xmlDocument.SelectSingleNode("//ExpectedResponseOnHelo"), "ExpectedResponseOnHelo", SimpleSmtpClient.SmtpResponseCode.OK, false);
			this.expectedResponseOnStartTls = SmtpExpectedResponse.FromXml(xmlDocument.SelectSingleNode("//ExpectedResponseOnStartTls"), "ExpectedResponseOnStartTls", SimpleSmtpClient.SmtpResponseCode.ServiceReady, false);
			this.expectedResponseOnHeloAfterStartTls = SmtpExpectedResponse.FromXml(xmlDocument.SelectSingleNode("//ExpectedResponseOnHeloAfterStartTls"), "ExpectedResponseOnHeloAfterStartTls", SimpleSmtpClient.SmtpResponseCode.OK, false);
			this.expectedResponseOnAuthenticate = SmtpExpectedResponse.FromXml(xmlDocument.SelectSingleNode("//ExpectedResponseOnAuthenticate"), "ExpectedResponseOnAuthenticate", SimpleSmtpClient.SmtpResponseCode.AuthAccepted, false);
			this.expectedResponseOnMailFrom = SmtpExpectedResponse.FromXml(xmlDocument.SelectSingleNode("//ExpectedResponseOnMailFrom"), "ExpectedResponseOnMailFrom", SimpleSmtpClient.SmtpResponseCode.OK, false);
			this.expectedResponseOnData = SmtpExpectedResponse.FromXml(xmlDocument.SelectSingleNode("//ExpectedResponseOnData"), "ExpectedResponseOnData", SimpleSmtpClient.SmtpResponseCode.OK, false);
			this.clientCertificate = ClientCertificateCriteria.FromXml(xmlDocument.SelectSingleNode("//ClientCertificate"), out this.clientCertificateValid);
			this.expectedServerCertificate = SmtpConnectionProbeWorkDefinition.CertificateProperties.FromXml(xmlDocument.SelectSingleNode("//ExpectedServerCertificate"), out this.expectedServerCertificateValid);
			this.customCommands = SmtpCustomCommand.FromXml(xmlDocument);
		}

		private bool resolveEndPoint;

		private string smtpServer;

		private int port;

		private AuthenticationType authenticationType;

		private ConnectionLostPoint expectedConnectionLostPoint;

		private string heloDomain;

		private Account authenticationAccount;

		private string mailFrom;

		private ICollection<SmtpRecipient> mailTo;

		private bool useSsl;

		private string data;

		private SmtpExpectedResponse expectedResponseOnConnect;

		private SmtpExpectedResponse expectedResponseOnHelo;

		private SmtpExpectedResponse expectedResponseOnStartTls;

		private SmtpExpectedResponse expectedResponseOnHeloAfterStartTls;

		private SmtpExpectedResponse expectedResponseOnAuthenticate;

		private SmtpExpectedResponse expectedResponseOnMailFrom;

		private SmtpExpectedResponse expectedResponseOnData;

		private bool clientCertificateValid;

		private ClientCertificateCriteria clientCertificate;

		private bool expectedServerCertificateValid;

		private SmtpConnectionProbeWorkDefinition.CertificateProperties expectedServerCertificate;

		private ICollection<SmtpCustomCommand> customCommands;

		private bool ignoreCertificateNameMismatchPolicyError;

		private bool ignoreCertificateChainPolicyErrorForSelfSigned;

		private string senderTenantID;

		private string recipientTenantID;

		private bool addAttributions;

		private Directionality direction;

		public class CertificateProperties
		{
			public string Subject
			{
				get
				{
					return this.subject;
				}
				internal set
				{
					this.subject = value;
				}
			}

			public string Issuer
			{
				get
				{
					return this.issuer;
				}
				internal set
				{
					this.issuer = value;
				}
			}

			public DateTime? ValidFrom
			{
				get
				{
					return this.validFrom;
				}
				internal set
				{
					this.validFrom = value;
				}
			}

			public DateTime? ValidTo
			{
				get
				{
					return this.validTo;
				}
				internal set
				{
					this.validTo = value;
				}
			}

			public static SmtpConnectionProbeWorkDefinition.CertificateProperties FromXml(XmlNode workContext, out bool validCertificate)
			{
				SmtpConnectionProbeWorkDefinition.CertificateProperties certificateProperties = null;
				validCertificate = false;
				if (workContext != null)
				{
					certificateProperties = new SmtpConnectionProbeWorkDefinition.CertificateProperties();
					bool flag = false;
					XmlNode xmlNode = workContext.SelectSingleNode("Subject");
					if (xmlNode != null)
					{
						certificateProperties.subject = Utils.CheckNullOrWhiteSpace(xmlNode.InnerText, "ExpectedServerCertificate Subject");
						flag = true;
					}
					xmlNode = workContext.SelectSingleNode("Issuer");
					if (xmlNode != null)
					{
						certificateProperties.issuer = Utils.CheckNullOrWhiteSpace(xmlNode.InnerText, "ExpectedServerCertificate Issuer");
						flag = true;
					}
					xmlNode = workContext.SelectSingleNode("ValidFrom");
					if (xmlNode != null)
					{
						certificateProperties.validFrom = new DateTime?(Utils.GetDateTime(xmlNode.InnerText, "ExpectedServerCertificate ValidFrom"));
						flag = true;
					}
					xmlNode = workContext.SelectSingleNode("ValidTo");
					if (xmlNode != null)
					{
						certificateProperties.validTo = new DateTime?(Utils.GetDateTime(xmlNode.InnerText, "ExpectedServerCertificate ValidTo"));
						flag = true;
					}
					if (!flag)
					{
						throw new ArgumentException("The ExpectedServerCertificate node is specified, but no valid child nodes exist.");
					}
					validCertificate = true;
				}
				return certificateProperties;
			}

			private string subject;

			private string issuer;

			private DateTime? validFrom;

			private DateTime? validTo;
		}
	}
}
