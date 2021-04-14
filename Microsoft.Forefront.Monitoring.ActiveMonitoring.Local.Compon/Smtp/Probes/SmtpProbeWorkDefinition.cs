using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes
{
	public class SmtpProbeWorkDefinition
	{
		public SmtpProbeWorkDefinition(int id, ProbeDefinition probeDefinition) : this(id, probeDefinition, null)
		{
		}

		public SmtpProbeWorkDefinition(int id, ProbeDefinition probeDefinition, DelTraceDebug traceDebug)
		{
			this.workItemId = id;
			this.probeDefinition = probeDefinition;
			this.traceDebug = traceDebug;
			this.LoadFromContext(probeDefinition.ExtensionAttributes);
		}

		public SmtpProbeWorkDefinition.SendMailDefinition SendMail { get; internal set; }

		public SmtpProbeWorkDefinition.CheckMailDefinition CheckMail { get; internal set; }

		public List<Notification> ExpectedNotifications { get; internal set; }

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

		public ClientCertificateCriteria ClientCertificate { get; internal set; }

		public SmtpProbeWorkDefinition.TargetDataDefinition TargetData { get; internal set; }

		internal bool IsMailflowProbe
		{
			get
			{
				return this.probeDefinition.TypeName == typeof(SmtpProbe).FullName || this.probeDefinition.TypeName == typeof(BucketedSmtpProbe).FullName;
			}
		}

		internal bool IsUnitTest { get; set; }

		internal bool IsCortex { get; private set; }

		private void LoadFromContext(string xml)
		{
			if (string.IsNullOrWhiteSpace(xml))
			{
				throw new ArgumentException("Work Definition XML is null");
			}
			XmlDocument xmlDocument = new SafeXmlDocument();
			xmlDocument.LoadXml(xml);
			if (xmlDocument.HasChildNodes && xmlDocument.FirstChild.Name == "ExtensionAttributes")
			{
				XmlDocument xmlDocument2 = new SafeXmlDocument();
				xmlDocument2.LoadXml(xmlDocument.FirstChild.InnerXml.ToString());
				xmlDocument = xmlDocument2;
			}
			XmlNode xmlNode = xmlDocument.SelectSingleNode("//WorkContext");
			if (xmlNode != null)
			{
				this.IsUnitTest = (Utils.GetOptionalXmlAttribute<bool>(xmlNode, "IsUnitTest", false) && ExEnvironment.IsTest);
				this.IsCortex = Utils.GetOptionalXmlAttribute<bool>(xmlNode, "IsCortex", false);
			}
			this.SendMail = SmtpProbeWorkDefinition.SendMailDefinition.FromXml(Utils.CheckXmlElement(xmlDocument.SelectSingleNode("//SendMail"), "SendMail"), this.IsMailflowProbe);
			this.SendMail.Message = Message.FromXml(this.workItemId, xmlDocument, this.probeDefinition, new DelTraceDebug(this.TraceDebug));
			this.ClientCertificate = ClientCertificateCriteria.FromXml(xmlDocument.SelectSingleNode("//ClientCertificate"), out this.clientCertificateValid);
			this.TargetData = SmtpProbeWorkDefinition.TargetDataDefinition.FromXml(xmlDocument.SelectSingleNode("//TargetData"));
			if (this.IsMailflowProbe)
			{
				this.CheckMail = SmtpProbeWorkDefinition.CheckMailDefinition.FromXml(Utils.CheckXmlElement(xmlDocument.SelectSingleNode("//CheckMail"), "CheckMail"), this.probeDefinition.RecurrenceIntervalSeconds, this.SendMail.Sla, this.SendMail.Enabled, new DelTraceDebug(this.TraceDebug));
				return;
			}
			XmlElement xmlElement = Utils.CheckXmlElement(xmlDocument.SelectSingleNode("//Match"), "Match");
			List<Notification> list = new List<Notification>();
			foreach (object obj in xmlElement.SelectNodes("Notification"))
			{
				XmlNode definition = (XmlNode)obj;
				list.Add(new Notification
				{
					Type = Utils.GetMandatoryXmlAttribute<string>(definition, "Type"),
					Value = Utils.GetMandatoryXmlAttribute<string>(definition, "Value"),
					Category = Utils.GetOptionalXmlAttribute<string>(definition, "Category", string.Empty),
					Mandatory = Utils.GetOptionalXmlAttribute<bool>(definition, "Mandatory", false),
					MatchExpected = Utils.GetOptionalXmlAttribute<bool>(definition, "MatchExpected", true),
					Method = Utils.GetOptionalXmlEnumAttribute<MatchType>(definition, "MatchType", MatchType.String)
				});
			}
			if (list.Count == 0)
			{
				throw new ArgumentException("Work definition error - Number of Notification nodes = 0.");
			}
			this.ExpectedNotifications = list;
		}

		private void TraceDebug(string format, params object[] args)
		{
			if (this.traceDebug != null)
			{
				this.traceDebug(format, args);
				return;
			}
			WTFDiagnostics.TraceInformation(ExTraceGlobals.SMTPTracer, new TracingContext(), format, args, null, "TraceDebug", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Smtp\\Probes\\SmtpProbeWorkDefinition.cs", 250);
		}

		private readonly int workItemId;

		private readonly ProbeDefinition probeDefinition;

		private readonly DelTraceDebug traceDebug;

		private bool clientCertificateValid;

		public class CheckMailDefinition
		{
			public bool Enabled { get; internal set; }

			public string PopServer { get; internal set; }

			public int Port { get; internal set; }

			public bool EnableSsl { get; internal set; }

			public int ReadTimeout { get; internal set; }

			public string Username { get; internal set; }

			public string Password { get; internal set; }

			public int QueryTimeWindow { get; internal set; }

			public int DeleteMessageMinutes { get; internal set; }

			public ExpectedMessage ExpectedMessage { get; internal set; }

			public string LogFileLocation { get; internal set; }

			public bool DisableCheckMailByMessageId { get; internal set; }

			public bool VerifyDeliverResultBeforeDelete { get; internal set; }

			public static SmtpProbeWorkDefinition.CheckMailDefinition FromXml(XmlElement workContext, int recurrenceIntervalSeconds, double sla, bool sendEnabled, DelTraceDebug traceDebug)
			{
				SmtpProbeWorkDefinition.CheckMailDefinition checkMailDefinition = new SmtpProbeWorkDefinition.CheckMailDefinition();
				checkMailDefinition.Enabled = Utils.GetBoolean(workContext.GetAttribute("Enabled"), "Enabled", true);
				if (checkMailDefinition.Enabled)
				{
					checkMailDefinition.PopServer = Utils.CheckNullOrWhiteSpace(workContext.GetAttribute("PopServerUri"), "PopServerUri");
					checkMailDefinition.Port = Utils.GetInteger(workContext.GetAttribute("Port"), "Port", 995, 1);
					checkMailDefinition.EnableSsl = Utils.GetBoolean(workContext.GetAttribute("EnableSsl"), "EnableSsl", true);
					checkMailDefinition.ReadTimeout = Utils.GetInteger(workContext.GetAttribute("ReadTimeout"), "ReadTimeout", 120, 0);
					checkMailDefinition.Username = Utils.CheckNullOrWhiteSpace(workContext.GetAttribute("Username"), "Username");
					checkMailDefinition.Password = Utils.CheckNullOrWhiteSpace(workContext.GetAttribute("Password"), "Password");
					checkMailDefinition.QueryTimeWindow = Utils.GetInteger(workContext.GetAttribute("CheckMailQueryWindow"), "CheckMailQueryWindow", 900, 0);
					checkMailDefinition.DeleteMessageMinutes = Utils.GetInteger(workContext.GetAttribute("DeleteMessageMinutes"), "DeleteMessageMinutes", 120, 0);
					checkMailDefinition.LogFileLocation = workContext.GetAttribute("LogFileLocation");
					checkMailDefinition.DisableCheckMailByMessageId = (!sendEnabled && Utils.GetOptionalXmlAttribute<bool>(workContext, "DisableCheckMailByMessageId", false));
					checkMailDefinition.VerifyDeliverResultBeforeDelete = Utils.GetOptionalXmlAttribute<bool>(workContext, "VerifyDeliverResultBeforeDelete", false);
					if (!string.IsNullOrWhiteSpace(checkMailDefinition.LogFileLocation))
					{
						Directory.GetAccessControl(checkMailDefinition.LogFileLocation);
						traceDebug("LogFileLocation={0}", new object[]
						{
							checkMailDefinition.LogFileLocation
						});
					}
					double val = Math.Max(sla * 60.0, (double)recurrenceIntervalSeconds) * (ExEnvironment.IsTest ? 2.0 : 4.0);
					int num = (int)Math.Min(val, 2147483647.0);
					if (!Utils.GetOptionalXmlAttribute<bool>(workContext, "EnforceWindow", false) && checkMailDefinition.QueryTimeWindow < num)
					{
						checkMailDefinition.QueryTimeWindow = num;
						traceDebug("QueryWindow changed to {0}s", new object[]
						{
							num
						});
					}
					else
					{
						traceDebug("QueryWindow={0}s.", new object[]
						{
							checkMailDefinition.QueryTimeWindow
						});
					}
					if (checkMailDefinition.DeleteMessageMinutes != 0 && TimeSpan.FromMinutes((double)checkMailDefinition.DeleteMessageMinutes).TotalSeconds < (double)checkMailDefinition.QueryTimeWindow)
					{
						checkMailDefinition.DeleteMessageMinutes = (int)Math.Ceiling(TimeSpan.FromSeconds((double)checkMailDefinition.QueryTimeWindow).TotalMinutes);
					}
					XmlNode xmlNode = workContext.SelectSingleNode("Match");
					if (xmlNode != null)
					{
						ExpectedMessage expectedMessage = new ExpectedMessage();
						XmlNode xmlNode2 = xmlNode.SelectSingleNode("Subject");
						if (xmlNode2 != null)
						{
							Notification notification = new Notification();
							notification.Type = "Subject";
							string mandatoryXmlAttribute = Utils.GetMandatoryXmlAttribute<string>(xmlNode2, "Value");
							notification.Value = ((mandatoryXmlAttribute == null) ? mandatoryXmlAttribute : mandatoryXmlAttribute.Trim());
							notification.Mandatory = Utils.GetOptionalXmlAttribute<bool>(xmlNode2, "Mandatory", true);
							notification.Method = Utils.GetOptionalXmlEnumAttribute<MatchType>(xmlNode2, "MatchType", MatchType.String);
							notification.MatchExpected = Utils.GetOptionalXmlAttribute<bool>(xmlNode2, "MatchExpected", true);
							expectedMessage.Subject = notification;
							traceDebug("MatchSubject: '{0}'", new object[]
							{
								notification.Value
							});
						}
						xmlNode2 = xmlNode.SelectSingleNode("Body");
						if (xmlNode2 != null)
						{
							Notification notification2 = new Notification();
							notification2.Type = "Body";
							string mandatoryXmlAttribute2 = Utils.GetMandatoryXmlAttribute<string>(xmlNode2, "Value");
							notification2.Value = ((mandatoryXmlAttribute2 == null) ? mandatoryXmlAttribute2 : mandatoryXmlAttribute2.Trim());
							notification2.Mandatory = Utils.GetOptionalXmlAttribute<bool>(xmlNode2, "Mandatory", true);
							notification2.Method = Utils.GetOptionalXmlEnumAttribute<MatchType>(xmlNode2, "MatchType", MatchType.String);
							notification2.MatchExpected = Utils.GetOptionalXmlAttribute<bool>(xmlNode2, "MatchExpected", true);
							expectedMessage.Body = notification2;
							traceDebug("MatchBody: '{0}'", new object[]
							{
								notification2.Value
							});
						}
						foreach (object obj in xmlNode.SelectNodes("Header"))
						{
							XmlNode definition = (XmlNode)obj;
							Notification notification3 = new Notification();
							notification3.Type = Utils.GetMandatoryXmlAttribute<string>(definition, "Tag");
							notification3.Value = Utils.GetMandatoryXmlAttribute<string>(definition, "Value");
							notification3.Mandatory = Utils.GetOptionalXmlAttribute<bool>(definition, "Mandatory", true);
							notification3.Method = Utils.GetOptionalXmlEnumAttribute<MatchType>(definition, "MatchType", MatchType.String);
							notification3.MatchExpected = Utils.GetOptionalXmlAttribute<bool>(definition, "MatchExpected", true);
							expectedMessage.Headers.Add(notification3);
							traceDebug("MatchHeader: {0}='{1}'", new object[]
							{
								notification3.Type,
								notification3.Value
							});
						}
						xmlNode2 = xmlNode.SelectSingleNode("Attachment");
						if (xmlNode2 != null)
						{
							Notification notification4 = new Notification();
							notification4.Type = "Attachment";
							int optionalXmlAttribute = Utils.GetOptionalXmlAttribute<int>(xmlNode2, "Value", 0);
							notification4.Value = ((optionalXmlAttribute < 0) ? 0 : optionalXmlAttribute).ToString();
							notification4.Mandatory = Utils.GetOptionalXmlAttribute<bool>(xmlNode2, "Mandatory", true);
							notification4.Method = Utils.GetOptionalXmlEnumAttribute<MatchType>(xmlNode2, "MatchType", MatchType.String);
							notification4.MatchExpected = Utils.GetOptionalXmlAttribute<bool>(xmlNode2, "MatchExpected", true);
							expectedMessage.Attachment = notification4;
							traceDebug("MatchAttachment#: '{0}'", new object[]
							{
								notification4.Value
							});
						}
						if (expectedMessage.Subject != null || expectedMessage.Body != null || expectedMessage.Headers != null || expectedMessage.Attachment != null)
						{
							checkMailDefinition.ExpectedMessage = expectedMessage;
						}
					}
				}
				return checkMailDefinition;
			}
		}

		public class SendMailDefinition
		{
			public bool ResolveEndPoint { get; internal set; }

			public string SmtpServer { get; internal set; }

			public string OriginalFQDN { get; internal set; }

			public double Sla { get; internal set; }

			public int Port { get; internal set; }

			public bool EnableSsl { get; internal set; }

			public bool RequireTLS { get; internal set; }

			public bool Anonymous { get; internal set; }

			public int Timeout { get; internal set; }

			public string SenderUsername { get; internal set; }

			public string SenderPassword { get; internal set; }

			public string RecipientUsername { get; internal set; }

			public Message Message { get; internal set; }

			public bool Enabled { get; internal set; }

			public string SenderTenantID { get; internal set; }

			public string RecipientTenantID { get; internal set; }

			public Directionality Direction { get; internal set; }

			public bool IgnoreSendMailFailure { get; internal set; }

			public bool IgnoreCertificateNameMismatchPolicyError { get; internal set; }

			public DnsUtils.DnsRecordType RecordResolveType { get; internal set; }

			public bool SimpleResolution { get; internal set; }

			public bool AuthOnly { get; internal set; }

			public bool RcptOnly { get; internal set; }

			public AddressFamily IpVersion { get; internal set; }

			public int ConnectRetryCount { get; internal set; }

			public static SmtpProbeWorkDefinition.SendMailDefinition FromXml(XmlElement workContext, bool isMailflowProbe)
			{
				SmtpProbeWorkDefinition.SendMailDefinition sendMailDefinition = new SmtpProbeWorkDefinition.SendMailDefinition();
				sendMailDefinition.Enabled = Utils.GetBoolean(workContext.GetAttribute("Enabled"), "Enabled", true);
				sendMailDefinition.OriginalFQDN = Utils.CheckNullOrWhiteSpace(workContext.GetAttribute("SmtpServerUri"), "SmtpServerUri");
				sendMailDefinition.SmtpServer = sendMailDefinition.OriginalFQDN;
				sendMailDefinition.ResolveEndPoint = Utils.GetBoolean(workContext.GetAttribute("ResolveEndPoint"), "ResolveEndPoint", false);
				sendMailDefinition.Sla = Utils.GetTimeSpan(workContext.GetAttribute("SLA"), "SLA", TimeSpan.Parse("00:01:00")).TotalMinutes;
				sendMailDefinition.Port = Utils.GetInteger(workContext.GetAttribute("Port"), "Port", 25, 1);
				sendMailDefinition.EnableSsl = Utils.GetBoolean(workContext.GetAttribute("EnableSsl"), "EnableSsl", true);
				sendMailDefinition.RequireTLS = Utils.GetBoolean(workContext.GetAttribute("RequireTLS"), "RequireTLS", false);
				sendMailDefinition.Timeout = Utils.GetInteger(workContext.GetAttribute("Timeout"), "Timeout", 0, 0);
				sendMailDefinition.Anonymous = Utils.GetBoolean(workContext.GetAttribute("Anonymous"), "Anonymous", false);
				sendMailDefinition.SenderTenantID = Utils.GetOptionalXmlAttribute<string>(workContext, "SenderTenantID", string.Empty);
				sendMailDefinition.RecipientTenantID = Utils.GetOptionalXmlAttribute<string>(workContext, "RecipientTenantID", string.Empty);
				sendMailDefinition.Direction = Utils.GetOptionalXmlEnumAttribute<Directionality>(workContext, "Direction", Directionality.Incoming);
				sendMailDefinition.IgnoreCertificateNameMismatchPolicyError = Utils.GetOptionalXmlAttribute<bool>(workContext, "IgnoreCertificateNameMismatchPolicyError", false);
				sendMailDefinition.AuthOnly = Utils.GetOptionalXmlAttribute<bool>(workContext, "AuthOnly", false);
				sendMailDefinition.RcptOnly = Utils.GetOptionalXmlAttribute<bool>(workContext, "RcptOnly", false);
				sendMailDefinition.SimpleResolution = Utils.GetOptionalXmlAttribute<bool>(workContext, "SimpleResolution", true);
				sendMailDefinition.IgnoreSendMailFailure = Utils.GetOptionalXmlAttribute<bool>(workContext, "IgnoreSendMailFailure", !isMailflowProbe);
				sendMailDefinition.ConnectRetryCount = Utils.GetOptionalXmlAttribute<int>(workContext, "ConnectRetryCount", 0);
				sendMailDefinition.RecordResolveType = Utils.GetOptionalXmlEnumAttribute<DnsUtils.DnsRecordType>(workContext, "RecordResolveType", DnsUtils.DnsRecordType.DnsTypeMX);
				sendMailDefinition.IpVersion = SmtpProbeWorkDefinition.SendMailDefinition.GetIPAddressFamily(sendMailDefinition);
				XmlElement xmlElement = Utils.CheckXmlElement(workContext.SelectSingleNode("MailFrom"), "MailFrom");
				sendMailDefinition.SenderUsername = Utils.CheckNullOrWhiteSpace(xmlElement.GetAttribute("Username"), "Username");
				sendMailDefinition.SenderPassword = xmlElement.GetAttribute("Password");
				XmlElement xmlElement2 = Utils.CheckXmlElement(workContext.SelectSingleNode("MailTo"), "MailTo");
				sendMailDefinition.RecipientUsername = Utils.CheckNullOrWhiteSpace(xmlElement2.GetAttribute("Username"), "Username");
				return sendMailDefinition;
			}

			private static AddressFamily GetIPAddressFamily(SmtpProbeWorkDefinition.SendMailDefinition sd)
			{
				IPAddress ipaddress;
				if (sd.ResolveEndPoint)
				{
					if (sd.RecordResolveType == DnsUtils.DnsRecordType.DnsTypeAAAA)
					{
						return AddressFamily.InterNetworkV6;
					}
					if (sd.RecordResolveType == DnsUtils.DnsRecordType.DnsTypeA || sd.RecordResolveType == DnsUtils.DnsRecordType.DnsTypeMX)
					{
						return AddressFamily.InterNetwork;
					}
				}
				else if (IPAddress.TryParse(sd.SmtpServer, out ipaddress))
				{
					return ipaddress.AddressFamily;
				}
				return AddressFamily.Unknown;
			}
		}

		public class TargetDataDefinition
		{
			public string MailboxDatabase { get; internal set; }

			public string MailboxDatabaseVersion { get; internal set; }

			public static SmtpProbeWorkDefinition.TargetDataDefinition FromXml(XmlNode workContext)
			{
				XmlElement xmlElement = workContext as XmlElement;
				SmtpProbeWorkDefinition.TargetDataDefinition targetDataDefinition = null;
				if (xmlElement != null)
				{
					targetDataDefinition = new SmtpProbeWorkDefinition.TargetDataDefinition();
					XmlNode xmlNode = workContext.SelectSingleNode("MailboxDatabase");
					if (xmlNode != null)
					{
						targetDataDefinition.MailboxDatabase = Utils.CheckNullOrWhiteSpace(xmlNode.InnerText, "TargetData MailboxDatabase");
					}
					xmlNode = workContext.SelectSingleNode("MailboxDatabaseVersion");
					if (xmlNode != null)
					{
						targetDataDefinition.MailboxDatabaseVersion = Utils.CheckNullOrWhiteSpace(xmlNode.InnerText, "TargetData MailboxDatabaseVersion");
					}
				}
				return targetDataDefinition;
			}
		}
	}
}
