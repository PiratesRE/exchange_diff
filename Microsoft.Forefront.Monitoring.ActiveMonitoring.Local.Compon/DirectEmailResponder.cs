using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Hygiene.Data.Directory;
using Microsoft.Exchange.Net;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;
using Microsoft.Win32;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public class DirectEmailResponder : ResponderWorkItem
	{
		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			IEnumerable<ResponderResult> query = (from r in base.Broker.GetResponderResults(base.Definition, SqlDateTime.MinValue.Value)
			orderby r.ExecutionStartTime descending
			select r).Take(1);
			Task<ResponderResult> task = base.Broker.AsDataAccessQuery<ResponderResult>(query).ExecuteAsync(cancellationToken, base.TraceContext);
			task.Continue(delegate(ResponderResult lastResponderResult)
			{
				DateTime dateTime = SqlDateTime.MinValue.Value;
				if (lastResponderResult != null)
				{
					dateTime = lastResponderResult.ExecutionStartTime;
				}
				ResponderResult result = this.Result;
				result.StateAttribute2 += string.Format("Monitor '{0}' match range [{1} to {2}].", this.Definition.AlertMask, dateTime, this.Result.ExecutionStartTime);
				IEnumerable<MonitorResult> query2 = (from r in this.Broker.GetMonitorResults(this.Definition.AlertMask, dateTime, this.Result.ExecutionStartTime)
				where r.IsAlert
				orderby r.ExecutionStartTime descending
				select r).Take(1);
				Task<MonitorResult> task2 = this.Broker.AsDataAccessQuery<MonitorResult>(query2).ExecuteAsync(cancellationToken, this.TraceContext);
				task2.Continue(delegate(MonitorResult lastMonitorResult)
				{
					if (lastMonitorResult != null)
					{
						IEnumerable<ProbeResult> query3 = (from r in this.Broker.GetProbeResult(lastMonitorResult.LastFailedProbeId, lastMonitorResult.LastFailedProbeResultId)
						select r).Take(1);
						Task<ProbeResult> task3 = this.Broker.AsDataAccessQuery<ProbeResult>(query3).ExecuteAsync(cancellationToken, this.TraceContext);
						task3.Continue(delegate(ProbeResult lastProbeResult)
						{
							if (lastProbeResult == null)
							{
								this.probeException = DirectEmailResponder.InvalidString;
								this.probeError = DirectEmailResponder.InvalidString;
							}
							else
							{
								this.probeException = lastProbeResult.Exception;
								this.probeError = lastProbeResult.Error;
							}
							if (!this.IsRegionEmailOptout())
							{
								this.BuildEmailContent();
								this.Sendmail();
								ResponderResult result3 = this.Result;
								result3.StateAttribute1 += string.Format("DirectEmailResponder email has been sent to '{0}'.", DirectEmailResponder.recipientUsername);
								return;
							}
							ResponderResult result4 = this.Result;
							result4.StateAttribute1 += "DirectEmailResponder email has been opted out.";
						}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
						return;
					}
					ResponderResult result2 = this.Result;
					result2.StateAttribute1 += "DirectEmailResponder finished with no alert.";
				}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
			}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
		}

		private bool IsRegionEmailOptout()
		{
			GlobalConfigSession globalConfigSession = new GlobalConfigSession();
			RegionEmailFilter[] array = globalConfigSession.GetRegionEmailOptout().ToArray<RegionEmailFilter>();
			ResponderResult result = base.Result;
			result.StateAttribute2 += string.Format(" IsRegionEmailOptout length:{0}, ", array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				ResponderResult result2 = base.Result;
				result2.StateAttribute2 += string.Format(" [Idx:{0}]={1}  ", i, array[i].Enabled);
			}
			return globalConfigSession.IsRegionEmailOptout();
		}

		private void Sendmail()
		{
			RemoteCertificateValidationCallback serverCertificateValidationCallback = ServicePointManager.ServerCertificateValidationCallback;
			try
			{
				this.LoadWorkContext(base.Definition.ExtensionAttributes);
				MailMessage mailMessage = new MailMessage(DirectEmailResponder.senderUsername, DirectEmailResponder.recipientUsername);
				mailMessage.Priority = MailPriority.High;
				mailMessage.Subject = this.emailSubject;
				mailMessage.Body = this.emailBody;
				ServicePointManager.ServerCertificateValidationCallback = ((object param0, X509Certificate param1, X509Chain param2, SslPolicyErrors param3) => true);
				using (SmtpClient smtpClient = new SmtpClient(DirectEmailResponder.smtpServer, DirectEmailResponder.port))
				{
					smtpClient.UseDefaultCredentials = true;
					smtpClient.EnableSsl = DirectEmailResponder.enableSsl;
					smtpClient.Send(mailMessage);
				}
				mailMessage.Dispose();
			}
			catch (Exception ex)
			{
				base.Result.StateAttribute1 = ex.Message;
				throw ex;
			}
			finally
			{
				ServicePointManager.ServerCertificateValidationCallback = serverCertificateValidationCallback;
			}
		}

		private void BuildEmailContent()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(string.Format("   Forest:  {0}", ComputerInformation.DnsPhysicalDomainName));
			stringBuilder.AppendLine(string.Format("   Site:    {0}", DatacenterRegistry.GetForefrontServiceTag()));
			stringBuilder.AppendLine(string.Format("   Machine: {0}", base.Result.MachineName));
			string value = stringBuilder.ToString();
			string text = null;
			string text2 = null;
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(DirectEmailResponder.ActiveMonitoringRegistryPath, false))
				{
					if (registryKey != null)
					{
						text = (string)registryKey.GetValue("RPSCertificateSubject", null);
						text2 = (string)registryKey.GetValue("RPSEndpoint", null);
						if (text == null)
						{
							text = DirectEmailResponder.InvalidString;
						}
						if (text2 == null)
						{
							text2 = DirectEmailResponder.InvalidString;
						}
					}
					else
					{
						text2 = DirectEmailResponder.InvalidString;
						text = DirectEmailResponder.InvalidString;
					}
				}
			}
			catch
			{
				text2 = DirectEmailResponder.InvalidString;
				text = DirectEmailResponder.InvalidString;
			}
			this.emailSubject = string.Format("Email Notification: ({0}) Smart Alerts Connectivity - Monitor({1}) failed", base.Result.MachineName, base.Definition.AlertMask);
			stringBuilder.Clear();
			stringBuilder.AppendLine("Alert: Smart Alerts Connectivity Probe Failed.");
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("Service Scope:");
			stringBuilder.AppendLine(value);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("Description:");
			stringBuilder.AppendLine(string.Format("   RPS Endpoint: {0}", text2));
			stringBuilder.AppendLine(string.Format("   RPS CertificateSubject: {0}", text));
			stringBuilder.AppendLine();
			stringBuilder.AppendLine(string.Format("Error : {0}", this.probeError));
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("Exception:");
			stringBuilder.AppendLine(this.probeException);
			this.emailBody = stringBuilder.ToString();
		}

		private void LoadWorkContext(string workContextXml)
		{
			if (!string.IsNullOrEmpty(DirectEmailResponder.recipientUsername))
			{
				return;
			}
			if (string.IsNullOrWhiteSpace(workContextXml))
			{
				throw new ArgumentNullException("workContextXml");
			}
			SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
			safeXmlDocument.LoadXml(workContextXml);
			safeXmlDocument.SelectSingleNode("//WorkContext");
			XmlElement xmlElement = Utils.CheckXmlElement(safeXmlDocument.SelectSingleNode("//SendMail"), "SendMail");
			DirectEmailResponder.enableSsl = Utils.GetBoolean(xmlElement.GetAttribute("EnableSsl"), "EnableSsl", true);
			DirectEmailResponder.smtpServer = this.GetIPv4Address(Utils.CheckNullOrWhiteSpace(xmlElement.GetAttribute("SmtpServerUri"), "SmtpServerUri"));
			DirectEmailResponder.port = Utils.GetInteger(xmlElement.GetAttribute("Port"), "Port", 25, 1);
			XmlElement xmlElement2 = Utils.CheckXmlElement(xmlElement.SelectSingleNode("MailFrom"), "MailFrom");
			DirectEmailResponder.senderUsername = Utils.CheckNullOrWhiteSpace(xmlElement2.GetAttribute("Username"), "Username");
			string text = this.GetExchangeLabsServiceName();
			if (string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(ComputerInformation.DnsPhysicalDomainName))
			{
				string text2 = ComputerInformation.DnsPhysicalDomainName.ToLower();
				if (text2.Contains("protection.gbl"))
				{
					text = "FopeProd";
				}
				else if (text2.Contains("ffo.gbl"))
				{
					text = "FopeDogfood";
				}
				else if (text2.Contains("prod.protection.outlook.com"))
				{
					text = "EopProd";
				}
				else if (text2.Contains("protectioncn.gbl"))
				{
					text = "FopeProdCN";
				}
				else if (text2.Contains("o365filter-int.com"))
				{
					text = "FopeDevTest";
				}
				else if (text2.Contains(".extest.microsoft.com"))
				{
					text = "Default";
				}
			}
			XmlNodeList xmlNodeList = xmlElement.SelectNodes("//MailTo");
			if (xmlNodeList != null && xmlNodeList.Count > 0)
			{
				using (IEnumerator enumerator = xmlNodeList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						XmlNode node = (XmlNode)obj;
						XmlElement xmlElement3 = Utils.CheckXmlElement(node, "MailTo");
						string text3 = Utils.CheckNullOrWhiteSpace(xmlElement3.GetAttribute("Environment"), "Environment");
						if (text3.Equals(text))
						{
							DirectEmailResponder.recipientUsername = Utils.CheckNullOrWhiteSpace(xmlElement3.GetAttribute("Recipients"), "Recipients");
							ResponderResult result = base.Result;
							result.StateAttribute2 += string.Format("Set recipients '{0}' for env '{1}' ", DirectEmailResponder.recipientUsername, text);
							break;
						}
						if (text3.Equals("Default"))
						{
							DirectEmailResponder.recipientUsername = Utils.CheckNullOrWhiteSpace(xmlElement3.GetAttribute("Recipients"), "Recipients");
							ResponderResult result2 = base.Result;
							result2.StateAttribute2 += string.Format("Set recipients '{0}' for env '{1}' ", DirectEmailResponder.recipientUsername, "Default");
						}
					}
					return;
				}
			}
			throw new Exception("MailTo Node missed under ExtensionAttribute of DirectEmailResponder.");
		}

		private string GetIPv4Address(string servername)
		{
			IPAddress[] array = (from address in Dns.GetHostAddresses(servername)
			where address.AddressFamily == AddressFamily.InterNetwork
			select address).ToArray<IPAddress>();
			if (array.Length > 0)
			{
				ResponderResult result = base.Result;
				result.StateAttribute2 += string.Format(" IPv4 address '{0}' is resolved for smtp server '{1}'. ", array[0].ToString(), servername);
				return array[0].ToString();
			}
			ResponderResult result2 = base.Result;
			result2.StateAttribute2 += string.Format(" No IPv4 address is associated with smtp server '{0}'. ", servername);
			return servername;
		}

		private string GetExchangeLabsServiceName()
		{
			string result = string.Empty;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeLabs", false))
			{
				if (registryKey != null)
				{
					result = (string)registryKey.GetValue("ServiceName", string.Empty);
				}
				else
				{
					ResponderResult result2 = base.Result;
					result2.StateAttribute2 += " GetExchangeLabsServiceName: No registry value ServiceName exists under ExchangeLabs. ";
				}
			}
			return result;
		}

		public const string DefaultEnvName = "Default";

		public const string HtmlTemplateSubject = "Email Notification: ({0}) Smart Alerts Connectivity - Monitor({1}) failed";

		private static readonly string ActiveMonitoringRegistryPath = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveMonitoring\\";

		private static readonly string InvalidString = "Unable to retrieve this value.";

		private static string smtpServer = null;

		private static int port = 25;

		private static bool enableSsl = true;

		private static string senderUsername = null;

		private static string recipientUsername = null;

		private string probeError;

		private string probeException;

		private string emailBody;

		private string emailSubject;
	}
}
