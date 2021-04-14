using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public class Message
	{
		internal bool ExpectDelivery { get; set; }

		internal string MessageId { get; set; }

		internal string HeaderTag { get; set; }

		internal string HeaderValue { get; set; }

		internal string Subject { get; set; }

		internal string Body { get; set; }

		internal bool UseSubjectVerbatim { get; set; }

		internal string SubjectOverride { get; set; }

		internal List<NameValuePair> Headers
		{
			get
			{
				return this.headers;
			}
			set
			{
				this.headers = value;
			}
		}

		internal Dictionary<string, object> Attachments
		{
			get
			{
				return this.attachments;
			}
			set
			{
				this.attachments = value;
			}
		}

		internal MailMessage Mail
		{
			get
			{
				return this.mail;
			}
			set
			{
				this.mail = value;
			}
		}

		public static Message FromXml(int workDefinitionId, XmlDocument workContext, ProbeDefinition probeDefinition, DelTraceDebug traceDebug)
		{
			XmlElement xmlElement = Utils.CheckXmlElement(workContext.SelectSingleNode("//Message"), "Message");
			Message message = new Message();
			message.SubjectOverride = Utils.GetOptionalXmlAttribute<string>(xmlElement, "SubjectOverride", string.Format("{0:0000000000}", workDefinitionId));
			message.MessageId = string.Format("{0}-{1:0000000000}{2:0000000000}", message.SubjectOverride, Message.rnd.Next(), Math.Abs(Message.rnd.Next() - workDefinitionId));
			message.ExpectDelivery = Utils.GetBoolean(xmlElement.GetAttribute("ExpectDelivery"), "ExpectDelivery", true);
			foreach (object obj in xmlElement.SelectNodes("Header"))
			{
				XmlElement xmlElement2 = (XmlElement)obj;
				string attribute = xmlElement2.GetAttribute("Tag");
				if (!string.IsNullOrWhiteSpace(attribute))
				{
					string attribute2 = xmlElement2.GetAttribute("Value");
					message.Headers.Add(new NameValuePair(attribute, Message.CheckMalware(attribute2)));
				}
			}
			if (Utils.GetOptionalXmlAttribute<bool>(xmlElement, "IncludeProbeIdHeader", false))
			{
				int num = new Random().Next();
				Guid guid = Guid.Parse(string.Format("{0:X8}-{1:X4}-{2:X4}-{3:X4}-{4:X12}", new object[]
				{
					workDefinitionId,
					0,
					0,
					0,
					num
				}));
				string name;
				string value;
				try
				{
					name = "X-FFOSystemProbe";
					value = SystemProbeId.EncryptProbeGuid(guid, DateTime.UtcNow);
					if (traceDebug != null)
					{
						traceDebug("Guid Encrypted", new object[0]);
					}
				}
				catch (SystemProbeException)
				{
					if (traceDebug != null)
					{
						traceDebug("Probe guid encryption cert was not found", new object[0]);
					}
					name = "X-LAMNotificationId";
					value = guid.ToString();
				}
				message.headers.Add(new NameValuePair(name, value));
			}
			message.Headers.Add(new NameValuePair("X-MS-Exchange-ActiveMonitoringProbeName", (probeDefinition == null || string.IsNullOrEmpty(probeDefinition.Name)) ? "Unknown" : probeDefinition.Name));
			string attribute3 = xmlElement.GetAttribute("HeaderTag");
			if (!string.IsNullOrWhiteSpace(attribute3))
			{
				string attribute4 = xmlElement.GetAttribute("HeaderValue");
				message.Headers.Add(new NameValuePair(attribute3, Message.CheckMalware(attribute4)));
			}
			string text = xmlElement.GetAttribute("Subject");
			text = ((text == null) ? text : text.Trim());
			message.UseSubjectVerbatim = Utils.GetOptionalXmlAttribute<bool>(xmlElement, "UseSubjectVerbatim", false);
			if (!message.UseSubjectVerbatim)
			{
				if (!string.IsNullOrEmpty(text))
				{
					message.Subject = string.Format("{0} {1}", message.MessageId, Message.CheckMalware(text));
				}
				else
				{
					message.Subject = message.MessageId;
				}
			}
			else
			{
				message.Subject = text;
				message.Headers.Add(new NameValuePair(Message.ProbeMessageIDHeaderTag, message.MessageId));
			}
			string text2 = xmlElement.GetAttribute("Body");
			text2 = ((text2 == null) ? text2 : text2.Trim());
			message.Body = Message.CheckMalware(text2);
			foreach (object obj2 in xmlElement.SelectNodes("Attachment"))
			{
				XmlElement xmlElement3 = (XmlElement)obj2;
				string attribute5 = xmlElement3.GetAttribute("Filename");
				if (!string.IsNullOrWhiteSpace(attribute5))
				{
					message.Attachments.Add(attribute5, Message.GetAttachmentObject(attribute5));
				}
			}
			return message;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (NameValuePair nameValuePair in this.Headers)
			{
				stringBuilder.AppendFormat("Header:Name='{0}',Value='{1}' ", nameValuePair.Name, nameValuePair.Value);
			}
			stringBuilder.AppendFormat("Subject:'{0}' ", this.Subject);
			stringBuilder.AppendFormat("Body:'{0}' ", this.Body);
			foreach (KeyValuePair<string, object> keyValuePair in this.Attachments)
			{
				stringBuilder.AppendFormat("Attachment:'{0}' ", keyValuePair.Key);
			}
			return stringBuilder.ToString();
		}

		internal static object GetAttachmentObject(string filename)
		{
			if (string.IsNullOrWhiteSpace(filename))
			{
				return filename;
			}
			if (string.Compare(filename, "EICAR", true) == 0)
			{
				MemoryStream memoryStream = new MemoryStream();
				byte[] bytes = Encoding.UTF8.GetBytes(Message.GetEicar());
				memoryStream.Write(bytes, 0, bytes.Length);
				return memoryStream;
			}
			if (string.Compare(filename, "GTUBE", true) == 0)
			{
				MemoryStream memoryStream2 = new MemoryStream();
				byte[] bytes2 = Encoding.UTF8.GetBytes(Message.GetGtube());
				memoryStream2.Write(bytes2, 0, bytes2.Length);
				return memoryStream2;
			}
			if (File.Exists(filename))
			{
				return filename;
			}
			string text = Path.Combine(".", filename);
			if (File.Exists(text))
			{
				return text;
			}
			text = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), filename);
			if (File.Exists(text))
			{
				return text;
			}
			return filename;
		}

		internal static Attachment CreateMailAttachment(KeyValuePair<string, object> entry)
		{
			if (entry.Value is MemoryStream)
			{
				MemoryStream memoryStream = (MemoryStream)entry.Value;
				memoryStream.Seek(0L, SeekOrigin.Begin);
				return new Attachment(memoryStream, entry.Key);
			}
			return new Attachment((string)entry.Value);
		}

		internal void CleanupAttachment()
		{
			foreach (Attachment attachment in this.Mail.Attachments)
			{
				attachment.Dispose();
			}
			this.Mail.Dispose();
			foreach (KeyValuePair<string, object> keyValuePair in this.Attachments)
			{
				if (keyValuePair.Value is MemoryStream)
				{
					((MemoryStream)keyValuePair.Value).Dispose();
				}
			}
		}

		private static string CheckMalware(string content)
		{
			if (!string.IsNullOrWhiteSpace(content))
			{
				if (string.Compare(content, "EICAR", true) == 0 || content.Contains("$EICAR$"))
				{
					return content.Replace("EICAR", Message.GetEicar());
				}
				if (string.Compare(content, "GTUBE", true) == 0 || content.Contains("$GTUBE$"))
				{
					return content.Replace("GTUBE", Message.GetGtube());
				}
			}
			return content;
		}

		private static string GetEicar()
		{
			return "ANTI\\VIRUSFILE!$H+H*".Replace("ANTI", "X5O!P%@AP[4").Replace("VIRUS", "PZX54(P^)7CC)7}$EICAR-STANDARD-ANTIVIRUS-TEST-");
		}

		private static string GetGtube()
		{
			return "XJS*ANTISPAM".Replace("ANTI", "C4JDBQADN1.NSBN3*2IDNEN*GTUBE-STANDARD").Replace("SPAM", "-ANTI-UBE-TEST-EMAIL*C.34X");
		}

		private const string ScrambledEicar = "ANTI\\VIRUSFILE!$H+H*";

		private const string ScrambledGtube = "XJS*ANTISPAM";

		internal static readonly string ProbeMessageIDHeaderTag = "SMTP-Probe-Message-ID";

		private static Random rnd = new Random();

		private List<NameValuePair> headers = new List<NameValuePair>();

		private MailMessage mail = new MailMessage();

		private Dictionary<string, object> attachments = new Dictionary<string, object>();
	}
}
