using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Net.ExSmtpClient;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes
{
	public class SmtpConnectionProbe : ProbeWorkItem
	{
		public SmtpConnectionProbe()
		{
			ChainEnginePool pool = new ChainEnginePool();
			this.cache = new CertificateCache(pool);
			this.cache.Open(OpenFlags.ReadOnly);
			this.CancelProbeWithSuccess = false;
			this.LatencyAnalytics = new SmtpConnectionProbeAnalytics();
			this.UseXmlConfiguration = true;
		}

		internal CancellationToken CancellationToken { get; set; }

		internal bool CancelProbeWithSuccess { get; set; }

		protected SmtpConnectionProbeWorkDefinition WorkDefinition
		{
			get
			{
				if (this.workDefinition == null)
				{
					this.workDefinition = new SmtpConnectionProbeWorkDefinition(base.Definition.ExtensionAttributes, this.UseXmlConfiguration);
				}
				return this.workDefinition;
			}
		}

		private protected virtual ISimpleSmtpClient Client
		{
			protected get
			{
				return this.client;
			}
			private set
			{
				this.client = value;
			}
		}

		protected int TestCount
		{
			get
			{
				return this.testCount;
			}
			set
			{
				this.testCount = value;
			}
		}

		protected virtual bool IsSmtpConnectionExpectedToFail
		{
			get
			{
				return base.Broker.IsLocal() && base.Definition.ServiceName == "HubTransport" && this.IsHubTransportInDrainingState();
			}
		}

		protected virtual bool DisconnectBetweenSessions
		{
			get
			{
				return false;
			}
		}

		protected virtual int Identifier
		{
			get
			{
				return base.Id;
			}
		}

		protected SmtpConnectionProbeAnalytics LatencyAnalytics { get; set; }

		protected bool UseXmlConfiguration { get; set; }

		private string ResolvedEndPoint
		{
			get
			{
				if (string.IsNullOrEmpty(this.resolvedEndPoint))
				{
					this.resolvedEndPoint = this.WorkDefinition.SmtpServer;
					if (this.WorkDefinition.ResolveEndPoint)
					{
						DnsUtils.DnsResponse mxendPointForDomain = DnsUtils.GetMXEndPointForDomain(this.WorkDefinition.SmtpServer);
						WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.SMTPConnectionTracer, new TracingContext(), "Resolved SMTP server {0} as {1}.", this.WorkDefinition.SmtpServer, this.resolvedEndPoint, null, "ResolvedEndPoint", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Smtp\\Probes\\SmtpConnectionProbe.cs", 211);
						if (!mxendPointForDomain.DnsResolvedSuccessfuly)
						{
							throw new Exception(string.Format("Unable to resolve MX records for {0}.", this.WorkDefinition.SmtpServer));
						}
						this.resolvedEndPoint = mxendPointForDomain.IPAddress.ToString();
					}
				}
				return this.resolvedEndPoint;
			}
		}

		public static void SetResolvedEndpoint(ProbeResult result, string resolvedEndpoint)
		{
			result.StateAttribute11 = resolvedEndpoint;
		}

		public static string GetResolvedEndpiont(ProbeResult result)
		{
			return result.StateAttribute11;
		}

		public static void SetLatencyAnalysis(ProbeResult result, string analysis)
		{
			result.StateAttribute12 = analysis;
		}

		public static string GetLatencyAnalysis(ProbeResult result)
		{
			return result.StateAttribute12;
		}

		public static void SetLatencySummary(ProbeResult result, string summary)
		{
			result.StateAttribute13 = summary;
		}

		public static string GetLatencySummary(ProbeResult result)
		{
			return result.StateAttribute13;
		}

		public static void SetAdvertisedServerName(ProbeResult result, string serverName)
		{
			result.StateAttribute14 = serverName;
		}

		public static string GetAdvertisedServerName(ProbeResult result)
		{
			return result.StateAttribute14;
		}

		public static void SetProtocolConversation(ProbeResult result, string protocol)
		{
			result.StateAttribute15 = protocol;
		}

		public static string GetProtocolConversation(ProbeResult result)
		{
			return result.StateAttribute15;
		}

		public static void SetPortNumber(ProbeResult result, int portNumber)
		{
			result.StateAttribute16 = (double)portNumber;
		}

		public static int GetPortNumber(ProbeResult result)
		{
			return (int)result.StateAttribute16;
		}

		public static void SetHighestLatency(ProbeResult result, long latency)
		{
			result.StateAttribute17 = (double)latency;
		}

		public static long GetHighestLatency(ProbeResult result)
		{
			return (long)result.StateAttribute17;
		}

		public static void SetAverageLatency(ProbeResult result, long average)
		{
			result.StateAttribute18 = (double)average;
		}

		public static long GetAverageLatency(ProbeResult result)
		{
			return (long)result.StateAttribute18;
		}

		public static void SetProbeExecutionTestStateDisabled(ProbeResult result)
		{
			result.StateAttribute19 = 1.0;
		}

		public static bool GetProbeExecutionTestStateDisabled(ProbeResult result)
		{
			return (int)result.StateAttribute19 == 1;
		}

		public static void SetExceptionType(ProbeResult result, string exceptionType)
		{
			result.StateAttribute21 = exceptionType;
		}

		public static string GetExceptionType(ProbeResult result)
		{
			return result.StateAttribute21;
		}

		public static void SetLastSmtpResponse(ProbeResult result, string smtpResponse)
		{
			result.StateAttribute22 = smtpResponse;
		}

		public static string GetLastSmtpResponse(ProbeResult result)
		{
			return result.StateAttribute22;
		}

		public override void PopulateDefinition<ProbeDefinition>(ProbeDefinition definition, Dictionary<string, string> propertyBag)
		{
			XmlDocument xmlDocument = new SafeXmlDocument();
			xmlDocument.LoadXml(definition.ExtensionAttributes);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("//WorkContext");
			SmtpConnectionProbe.UpdateNodeInnerText(xmlNode, propertyBag.FirstOrDefault((KeyValuePair<string, string> p) => p.Key == "SmtpServer"));
			SmtpConnectionProbe.UpdateNodeInnerText(xmlNode, propertyBag.FirstOrDefault((KeyValuePair<string, string> p) => p.Key == "ResolveEndPoint"));
			SmtpConnectionProbe.UpdateNodeInnerText(xmlNode, propertyBag.FirstOrDefault((KeyValuePair<string, string> p) => p.Key == "Port"));
			SmtpConnectionProbe.UpdateNodeInnerText(xmlNode, propertyBag.FirstOrDefault((KeyValuePair<string, string> p) => p.Key == "UseSsl"));
			SmtpConnectionProbe.UpdateChildNodeAttribute(xmlNode, "UseSsl", propertyBag.FirstOrDefault((KeyValuePair<string, string> p) => p.Key == "IgnoreCertificateNameMismatchPolicyError"));
			SmtpConnectionProbe.UpdateChildNodeAttribute(xmlNode, "UseSsl", propertyBag.FirstOrDefault((KeyValuePair<string, string> p) => p.Key == "IgnoreCertificateChainPolicyErrorForSelfSigned"));
			SmtpConnectionProbe.UpdateNodeInnerText(xmlNode, propertyBag.FirstOrDefault((KeyValuePair<string, string> p) => p.Key == "AuthenticationType"));
			SmtpConnectionProbe.UpdateNodeInnerText(xmlNode, propertyBag.FirstOrDefault((KeyValuePair<string, string> p) => p.Key == "ExpectedConnectionLostPoint"));
			SmtpConnectionProbe.UpdateNodeInnerText(xmlNode, propertyBag.FirstOrDefault((KeyValuePair<string, string> p) => p.Key == "HeloDomain"));
			SmtpConnectionProbe.UpdateNodeAttribute(xmlNode, propertyBag.FirstOrDefault((KeyValuePair<string, string> p) => p.Key == "SenderTenantID"));
			SmtpConnectionProbe.UpdateNodeAttribute(xmlNode, propertyBag.FirstOrDefault((KeyValuePair<string, string> p) => p.Key == "RecipientTenantID"));
			SmtpConnectionProbe.UpdateNodeInnerText(xmlNode, propertyBag.FirstOrDefault((KeyValuePair<string, string> p) => p.Key == "Data"));
			SmtpConnectionProbe.UpdateChildNodeAttribute(xmlNode, "Data", propertyBag.FirstOrDefault((KeyValuePair<string, string> p) => p.Key == "AddAttributions"));
			SmtpConnectionProbe.UpdateChildNodeAttribute(xmlNode, "Data", propertyBag.FirstOrDefault((KeyValuePair<string, string> p) => p.Key == "Direction"));
			SmtpConnectionProbe.UpdateNodeInnerText(xmlNode, propertyBag.FirstOrDefault((KeyValuePair<string, string> p) => p.Key == "ExpectedResponseOnConnect"));
			SmtpConnectionProbe.UpdateNodeInnerText(xmlNode, propertyBag.FirstOrDefault((KeyValuePair<string, string> p) => p.Key == "ExpectedResponseOnHelo"));
			SmtpConnectionProbe.UpdateNodeInnerText(xmlNode, propertyBag.FirstOrDefault((KeyValuePair<string, string> p) => p.Key == "ExpectedResponseOnStartTls"));
			SmtpConnectionProbe.UpdateNodeInnerText(xmlNode, propertyBag.FirstOrDefault((KeyValuePair<string, string> p) => p.Key == "ExpectedResponseOnHeloAfterStartTls"));
			SmtpConnectionProbe.UpdateNodeInnerText(xmlNode, propertyBag.FirstOrDefault((KeyValuePair<string, string> p) => p.Key == "ExpectedResponseOnAuthenticate"));
			SmtpConnectionProbe.UpdateNodeInnerText(xmlNode, propertyBag.FirstOrDefault((KeyValuePair<string, string> p) => p.Key == "ExpectedResponseOnMailFrom"));
			SmtpConnectionProbe.UpdateNodeInnerText(xmlNode, propertyBag.FirstOrDefault((KeyValuePair<string, string> p) => p.Key == "ExpectedResponseOnData"));
			SmtpConnectionProbe.UpdateChildNodeAttribute(xmlNode, "AuthenticationAccount", propertyBag.FirstOrDefault((KeyValuePair<string, string> p) => p.Key == "AuthenticationAccountUsername"));
			SmtpConnectionProbe.UpdateChildNodeAttribute(xmlNode, "AuthenticationAccount", propertyBag.FirstOrDefault((KeyValuePair<string, string> p) => p.Key == "AuthenticationAccountUsername"));
			SmtpConnectionProbe.UpdateChildNodeAttribute(xmlNode, "AuthenticationAccount", propertyBag.FirstOrDefault((KeyValuePair<string, string> p) => p.Key == "AuthenticationAccountPassword"));
			SmtpConnectionProbe.UpdateChildNodeAttribute(xmlNode, "MailFrom", propertyBag.FirstOrDefault((KeyValuePair<string, string> p) => p.Key == "MailFromUsername"));
			SmtpConnectionProbe.UpdateChildNodeAttribute(xmlNode, "MailTo", propertyBag.FirstOrDefault((KeyValuePair<string, string> p) => p.Key == "MailToUsername"));
			SmtpConnectionProbe.UpdateChildNodeAttribute(xmlNode, "MailTo", propertyBag.FirstOrDefault((KeyValuePair<string, string> p) => p.Key == "MailToExpectedResponse"));
			SmtpConnectionProbe.UpdateChildNodeInnerText(xmlNode, "ClientCertificate", propertyBag.FirstOrDefault((KeyValuePair<string, string> p) => p.Key == "ClientCertificateStoreLocation"));
			SmtpConnectionProbe.UpdateChildNodeInnerText(xmlNode, "ClientCertificate", propertyBag.FirstOrDefault((KeyValuePair<string, string> p) => p.Key == "ClientCertificateStoreName"));
			SmtpConnectionProbe.UpdateChildNodeInnerText(xmlNode, "ClientCertificate", propertyBag.FirstOrDefault((KeyValuePair<string, string> p) => p.Key == "ClientCertificateFindType"));
			SmtpConnectionProbe.UpdateChildNodeInnerText(xmlNode, "ClientCertificate", propertyBag.FirstOrDefault((KeyValuePair<string, string> p) => p.Key == "ClientCertificateFindValue"));
			SmtpConnectionProbe.UpdateChildNodeInnerText(xmlNode, "ClientCertificate", propertyBag.FirstOrDefault((KeyValuePair<string, string> p) => p.Key == "ClientCertificateTransportCertificateName"));
			SmtpConnectionProbe.UpdateChildNodeInnerText(xmlNode, "ClientCertificate", propertyBag.FirstOrDefault((KeyValuePair<string, string> p) => p.Key == "ClientCertificateTransportCertificateFqdn"));
			SmtpConnectionProbe.UpdateChildNodeInnerText(xmlNode, "ClientCertificate", propertyBag.FirstOrDefault((KeyValuePair<string, string> p) => p.Key == "ClientCertificateTransportWildcardMatchType"));
			SmtpConnectionProbe.UpdateChildNodeInnerText(xmlNode, "ExpectedServerCertificate", propertyBag.FirstOrDefault((KeyValuePair<string, string> p) => p.Key == "ExpectedServerCertificateSubject"));
			SmtpConnectionProbe.UpdateChildNodeInnerText(xmlNode, "ExpectedServerCertificate", propertyBag.FirstOrDefault((KeyValuePair<string, string> p) => p.Key == "ExpectedServerCertificateIssuer"));
			SmtpConnectionProbe.UpdateChildNodeInnerText(xmlNode, "ExpectedServerCertificate", propertyBag.FirstOrDefault((KeyValuePair<string, string> p) => p.Key == "ExpectedServerCertificateValidFrom"));
			SmtpConnectionProbe.UpdateChildNodeInnerText(xmlNode, "ExpectedServerCertificate", propertyBag.FirstOrDefault((KeyValuePair<string, string> p) => p.Key == "ExpectedServerCertificateValidTo"));
			definition.ExtensionAttributes = xmlDocument.InnerXml;
		}

		internal bool ShouldCancelProbe()
		{
			if (this.CancellationToken.IsCancellationRequested)
			{
				throw new OperationCanceledException();
			}
			return this.CancelProbeWithSuccess;
		}

		internal void MeasureLatency(string reason, Action cmd)
		{
			SmtpConnectionProbeLatency smtpConnectionProbeLatency = new SmtpConnectionProbeLatency(reason, true);
			try
			{
				cmd();
			}
			finally
			{
				smtpConnectionProbeLatency.StopRecording();
				this.LatencyAnalytics.AddLatency(smtpConnectionProbeLatency);
			}
		}

		internal void MeasureLatency(string reason, Action cmd, ConnectionLostPoint connectionLostPoint)
		{
			try
			{
				this.MeasureLatency(reason, cmd);
			}
			catch
			{
				if (!this.AssertIsConnected(connectionLostPoint))
				{
					throw;
				}
			}
			finally
			{
				this.AssertIsConnected(connectionLostPoint);
			}
		}

		internal bool MeasureLatency(string reason, SmtpConnectionProbe.ActionWithReturn<bool> cmd)
		{
			SmtpConnectionProbeLatency smtpConnectionProbeLatency = new SmtpConnectionProbeLatency(reason, true);
			bool result;
			try
			{
				result = cmd();
			}
			finally
			{
				smtpConnectionProbeLatency.StopRecording();
				this.LatencyAnalytics.AddLatency(smtpConnectionProbeLatency);
			}
			return result;
		}

		internal bool MeasureLatency(string reason, SmtpConnectionProbe.ActionWithReturn<bool> cmd, ConnectionLostPoint connectionLostPoint)
		{
			bool result;
			try
			{
				result = this.MeasureLatency(reason, cmd);
			}
			catch
			{
				if (!this.AssertIsConnected(connectionLostPoint))
				{
					throw;
				}
				return false;
			}
			finally
			{
				this.AssertIsConnected(connectionLostPoint);
			}
			return result;
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			this.CancellationToken = cancellationToken;
			SmtpConnectionProbe.SetResolvedEndpoint(base.Result, "The connection has not been established");
			DateTime dateTime = DateTime.MinValue;
			if (!TransportProbeCommon.IsProbeExecutionEnabled())
			{
				ProbeResult result = base.Result;
				result.ExecutionContext += "SmtpConnectionProbe skipped because probe execution disabled.";
				WTFDiagnostics.TraceInformation(ExTraceGlobals.SMTPConnectionTracer, new TracingContext(), "SmtpConnectionProbe skipped because probe execution disabled.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Smtp\\Probes\\SmtpConnectionProbe.cs", 723);
				SmtpConnectionProbe.SetProbeExecutionTestStateDisabled(base.Result);
				return;
			}
			if (!this.IsSmtpConnectionExpectedToFail)
			{
				try
				{
					this.Client = this.GetSmtpClient();
					this.Client.IgnoreCertificateNameMismatchPolicyError = this.WorkDefinition.IgnoreCertificateNameMismatchPolicyError;
					this.Client.IgnoreCertificateChainPolicyErrorForSelfSigned = this.WorkDefinition.IgnoreCertificateChainPolicyErrorForSelfSigned;
					this.testCount = 1;
					this.MeasureLatency("BeforeConnect", delegate()
					{
						this.BeforeConnect();
					});
					dateTime = DateTime.UtcNow;
					for (int i = 1; i <= this.testCount; i++)
					{
						if (this.ShouldCancelProbe())
						{
							return;
						}
						WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.SMTPConnectionTracer, new TracingContext(), "Running test {0}.", i.ToString(), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Smtp\\Probes\\SmtpConnectionProbe.cs", 779);
						this.TestConnection();
					}
					WTFDiagnostics.TraceInformation(ExTraceGlobals.SMTPConnectionTracer, new TracingContext(), "The SMTP server has been successfully evaluated and validated.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Smtp\\Probes\\SmtpConnectionProbe.cs", 783);
				}
				catch (EndpointManagerEndpointUninitializedException)
				{
					ProbeResult result2 = base.Result;
					result2.ExecutionContext += " Probe ended due to EndpointManagerEndpointUninitializedException, ignoring exception and treating as transient";
				}
				catch (OperationCanceledException)
				{
					SmtpConnectionProbe.SetExceptionType(base.Result, typeof(OperationCanceledException).FullName);
					throw;
				}
				catch (Exception ex)
				{
					base.Result.FailureContext = ex.Message;
					SmtpConnectionProbe.SetExceptionType(base.Result, ex.GetType().FullName);
					WTFDiagnostics.TraceError<string, string>(ExTraceGlobals.SMTPConnectionTracer, new TracingContext(), "{0}\r\n{1}", ex.ToString(), (this.Client != null) ? this.Client.SessionText : string.Empty, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Smtp\\Probes\\SmtpConnectionProbe.cs", 800);
					throw;
				}
				finally
				{
					base.Result.SampleValue = ((dateTime == DateTime.MinValue) ? 0.0 : Convert.ToDouble((DateTime.UtcNow - dateTime).TotalMilliseconds));
					SmtpConnectionProbe.SetLatencyAnalysis(base.Result, this.LatencyAnalytics.GenerateLatencyAnalysis());
					SmtpConnectionProbe.SetLatencySummary(base.Result, this.LatencyAnalytics.LatencySummary());
					SmtpConnectionProbeLatency highestLatencyValue = this.LatencyAnalytics.GetHighestLatencyValue();
					SmtpConnectionProbe.SetHighestLatency(base.Result, (highestLatencyValue == null) ? 0L : highestLatencyValue.Latency);
					SmtpConnectionProbe.SetAverageLatency(base.Result, this.LatencyAnalytics.Mean);
					if (this.Client != null)
					{
						SmtpConnectionProbe.SetProtocolConversation(base.Result, this.Client.SessionText.Replace(Environment.NewLine, " "));
						if (!string.IsNullOrEmpty(this.Client.LastResponse))
						{
							SmtpConnectionProbe.SetLastSmtpResponse(base.Result, this.Client.LastResponse);
						}
						this.Client.Dispose();
						this.Client = null;
					}
				}
				return;
			}
			if (this.CanConnectToEndPoint())
			{
				ProbeResult result3 = base.Result;
				result3.ExecutionContext += "Smtp connection succeeded when expected to fail.";
				base.Result.SetCompleted(ResultType.Failed);
				SmtpConnectionProbe.SetExceptionType(base.Result, "Smtp connection succeeded when expected to fail.");
				WTFDiagnostics.TraceInformation(ExTraceGlobals.SMTPConnectionTracer, new TracingContext(), "SmtpConnectionProbe failed because connection succeeded which was not expected", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Smtp\\Probes\\SmtpConnectionProbe.cs", 741);
				return;
			}
			ProbeResult result4 = base.Result;
			result4.ExecutionContext += "Smtp connection failed as expected.";
			base.Result.SetCompleted(ResultType.Succeeded);
			WTFDiagnostics.TraceInformation(ExTraceGlobals.SMTPConnectionTracer, new TracingContext(), "SmtpConnectionProbe succeeded. Connection failed as expected", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Smtp\\Probes\\SmtpConnectionProbe.cs", 750);
		}

		protected void AssertExpectedResponse(SmtpExpectedResponse expectedResponse)
		{
			SimpleSmtpClient.SmtpResponseCode lastResponseCode = this.Client.LastResponseCode;
			string text = "Server {0} on port {1} did not respond with expected response ({2}). The actual response was: {3}.";
			bool flag;
			switch (expectedResponse.Type)
			{
			case ExpectedResponseType.ResponseText:
			{
				Regex regex = new Regex(expectedResponse.ResponseText, RegexOptions.IgnoreCase);
				flag = regex.IsMatch(this.Client.LastResponse);
				text = string.Format(text, new object[]
				{
					this.ResolvedEndPoint,
					this.WorkDefinition.Port,
					expectedResponse.ResponseText,
					this.Client.LastResponse
				});
				goto IL_ED;
			}
			}
			flag = (lastResponseCode == expectedResponse.ResponseCode);
			text = string.Format(text, new object[]
			{
				this.ResolvedEndPoint,
				this.WorkDefinition.Port,
				expectedResponse.ResponseCode,
				this.Client.LastResponse
			});
			IL_ED:
			if (!flag)
			{
				WTFDiagnostics.TraceError(ExTraceGlobals.SMTPConnectionTracer, new TracingContext(), text, null, "AssertExpectedResponse", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Smtp\\Probes\\SmtpConnectionProbe.cs", 857);
				throw new Exception(text);
			}
		}

		protected bool SendCommand(string command, string expectedResponse, ref StringBuilder errors)
		{
			if (this.ShouldCancelProbe() || command == null)
			{
				return false;
			}
			this.MeasureLatency(command.Split(new char[]
			{
				' '
			}).First<string>(), delegate()
			{
				this.Client.Send(command);
			});
			if (!this.VerifyExpectedResponse(expectedResponse))
			{
				if (errors == null)
				{
					errors = new StringBuilder();
				}
				errors.AppendFormat(string.Format("Response to '{0}' not as expected. Actual: {1}", command, this.Client.LastResponse), new object[0]);
				return false;
			}
			return true;
		}

		protected bool VerifyExpectedResponse(string response)
		{
			string text = this.Client.LastResponse;
			if (this.Client.LastResponse.EndsWith("\r\n"))
			{
				text = text.Substring(0, this.Client.LastResponse.Length - 2);
			}
			return string.Equals(text, response);
		}

		protected string GetProbeId()
		{
			bool flag;
			long probeRunSequenceNumber = ProbeRunSequence.GetProbeRunSequenceNumber(this.Identifier.ToString(), out flag);
			return string.Format("{0:X8}-0000-0000-0000-{1:X12}", this.Identifier, (int)probeRunSequenceNumber);
		}

		protected virtual bool CanConnectToEndPoint()
		{
			bool result = false;
			try
			{
				using (ISimpleSmtpClient client = this.GetSmtpClient())
				{
					this.MeasureLatency("Connect", () => client.Connect(this.ResolvedEndPoint, this.WorkDefinition.Port, this.DisconnectBetweenSessions));
					result = client.IsConnected;
				}
			}
			catch (OperationCanceledException)
			{
				throw;
			}
			catch (Exception arg)
			{
				WTFDiagnostics.TraceInformation<string, int, Exception>(ExTraceGlobals.SMTPConnectionTracer, new TracingContext(), "Unable to connect to server {0} on port {1} due to the following exception: {2}", this.ResolvedEndPoint, this.WorkDefinition.Port, arg, null, "CanConnectToEndPoint", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Smtp\\Probes\\SmtpConnectionProbe.cs", 947);
				result = false;
			}
			finally
			{
				IDisposable disposable = this.Client;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			return result;
		}

		protected virtual ISimpleSmtpClient GetSmtpClient()
		{
			return new SimpleSmtpClient(this.CancellationToken);
		}

		protected virtual void BeforeConnect()
		{
		}

		protected virtual void AfterConnect()
		{
			this.RunCustomCommands(CustomCommandRunPoint.AfterConnect);
		}

		protected virtual void AfterHelo()
		{
			this.RunCustomCommands(CustomCommandRunPoint.AfterHelo);
		}

		protected virtual void AfterStartTls()
		{
			this.RunCustomCommands(CustomCommandRunPoint.AfterStartTls);
		}

		protected virtual void AfterHeloAfterStartTls()
		{
			this.RunCustomCommands(CustomCommandRunPoint.AfterHeloAfterStartTls);
		}

		protected virtual void AfterAuthenticate()
		{
			this.RunCustomCommands(CustomCommandRunPoint.AfterAuthenticate);
		}

		protected virtual void AfterMailFrom()
		{
			this.RunCustomCommands(CustomCommandRunPoint.AfterMailFrom);
		}

		protected virtual void BeforeRcptTo()
		{
			this.RunCustomCommands(CustomCommandRunPoint.BeforeRcptTo);
		}

		protected virtual void AfterRcptTo()
		{
			this.RunCustomCommands(CustomCommandRunPoint.AfterRcptTo);
		}

		protected virtual void BeforeData()
		{
			this.RunCustomCommands(CustomCommandRunPoint.BeforeData);
		}

		protected virtual void AfterData()
		{
			this.RunCustomCommands(CustomCommandRunPoint.AfterData);
		}

		private static void UpdateNodeInnerText(XmlNode root, KeyValuePair<string, string> kvp)
		{
			if (kvp.Key != null)
			{
				XmlNode xmlNode = root.SelectSingleNode(kvp.Key);
				if (xmlNode == null)
				{
					xmlNode = root.AppendChild(root.OwnerDocument.CreateElement(kvp.Key));
				}
				xmlNode.InnerText = kvp.Value;
			}
		}

		private static void UpdateChildNodeInnerText(XmlNode root, string childNodeName, KeyValuePair<string, string> kvp)
		{
			if (kvp.Key != null)
			{
				XmlNode xmlNode = root.SelectSingleNode(childNodeName);
				if (xmlNode == null)
				{
					xmlNode = root.AppendChild(root.OwnerDocument.CreateElement(childNodeName));
				}
				if (kvp.Key.StartsWith(childNodeName))
				{
					string key = kvp.Key.Replace(childNodeName, string.Empty);
					kvp = new KeyValuePair<string, string>(key, kvp.Value);
				}
				SmtpConnectionProbe.UpdateNodeInnerText(xmlNode, kvp);
			}
		}

		private static void UpdateNodeAttribute(XmlNode node, KeyValuePair<string, string> kvp)
		{
			if (node != null && kvp.Key != null)
			{
				XmlAttribute xmlAttribute = node.Attributes[kvp.Key];
				if (xmlAttribute == null)
				{
					xmlAttribute = node.Attributes.Append(node.OwnerDocument.CreateAttribute(kvp.Key));
				}
				xmlAttribute.Value = kvp.Value;
			}
		}

		private static void UpdateChildNodeAttribute(XmlNode root, string childNodeName, KeyValuePair<string, string> kvp)
		{
			if (root != null && kvp.Key != null)
			{
				XmlNode xmlNode = root.SelectSingleNode(childNodeName);
				if (xmlNode == null)
				{
					xmlNode = root.AppendChild(root.OwnerDocument.CreateElement(childNodeName));
				}
				if (kvp.Key.StartsWith(childNodeName))
				{
					string key = kvp.Key.Replace(childNodeName, string.Empty);
					kvp = new KeyValuePair<string, string>(key, kvp.Value);
				}
				SmtpConnectionProbe.UpdateNodeAttribute(xmlNode, kvp);
			}
		}

		private bool AssertIsConnected(ConnectionLostPoint connectionLostPoint)
		{
			if (this.client.IsConnected)
			{
				if (this.WorkDefinition.ExpectedConnectionLostPoint != ConnectionLostPoint.None && this.WorkDefinition.ExpectedConnectionLostPoint == connectionLostPoint)
				{
					string text = string.Format("Still connected to server {0} but expected to be dropped: {1}. Treating as failure.", this.ResolvedEndPoint, connectionLostPoint);
					WTFDiagnostics.TraceError(ExTraceGlobals.SMTPConnectionTracer, new TracingContext(), text, null, "AssertIsConnected", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Smtp\\Probes\\SmtpConnectionProbe.cs", 1198);
					throw new Exception(text);
				}
			}
			else
			{
				string text;
				if (connectionLostPoint == ConnectionLostPoint.OnConnect)
				{
					text = "Cannot establish connection to server {0} on port {1}.";
				}
				else
				{
					text = "Connection to server {0} on port {1} was lost.";
				}
				text = string.Format(text + " Lost: {2}. Expected: {3}. Treating as {4}.", new object[]
				{
					this.ResolvedEndPoint,
					this.WorkDefinition.Port,
					connectionLostPoint,
					this.WorkDefinition.ExpectedConnectionLostPoint,
					(this.WorkDefinition.ExpectedConnectionLostPoint == connectionLostPoint) ? "success" : "failure"
				});
				WTFDiagnostics.TraceInformation(ExTraceGlobals.SMTPConnectionTracer, new TracingContext(), text, null, "AssertIsConnected", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Smtp\\Probes\\SmtpConnectionProbe.cs", 1224);
				if (this.WorkDefinition.ExpectedConnectionLostPoint != connectionLostPoint)
				{
					return false;
				}
				this.CancelProbeWithSuccess = true;
			}
			return true;
		}

		private void AssertExpectedServerCertificate()
		{
			string format = "The server certificate did not match the expected {0} value of \"{1}\". The actual value was \"{2}\".";
			if (this.WorkDefinition.ExpectedServerCertificateValid)
			{
				if (this.client.RemoteCertificate == null)
				{
					throw new Exception("No server certificate existed.");
				}
				if (this.WorkDefinition.ExpectedServerCertificate.Subject != null)
				{
					Regex regex = new Regex(this.WorkDefinition.ExpectedServerCertificate.Subject, RegexOptions.IgnoreCase);
					if (!regex.IsMatch(this.client.RemoteCertificate.Subject))
					{
						throw new Exception(string.Format(format, "subject", this.WorkDefinition.ExpectedServerCertificate.Subject, this.client.RemoteCertificate.Subject));
					}
				}
				if (this.WorkDefinition.ExpectedServerCertificate.Issuer != null)
				{
					Regex regex2 = new Regex(this.WorkDefinition.ExpectedServerCertificate.Issuer, RegexOptions.IgnoreCase);
					if (!regex2.IsMatch(this.client.RemoteCertificate.Issuer))
					{
						throw new Exception(string.Format(format, "issuer", this.WorkDefinition.ExpectedServerCertificate.Issuer, this.client.RemoteCertificate.Issuer));
					}
				}
				if (this.WorkDefinition.ExpectedServerCertificate.ValidFrom != null && !(this.client.RemoteCertificate.ValidFrom <= this.WorkDefinition.ExpectedServerCertificate.ValidFrom))
				{
					throw new Exception(string.Format(format, "effective date", this.WorkDefinition.ExpectedServerCertificate.ValidFrom, this.client.RemoteCertificate.ValidFrom));
				}
				if (this.WorkDefinition.ExpectedServerCertificate.ValidTo != null && !(this.client.RemoteCertificate.ValidTo >= this.WorkDefinition.ExpectedServerCertificate.ValidTo))
				{
					throw new Exception(string.Format(format, "expiration date", this.WorkDefinition.ExpectedServerCertificate.ValidTo, this.client.RemoteCertificate.ValidTo));
				}
			}
		}

		private void TestConnection()
		{
			try
			{
				SmtpConnectionProbe.SetResolvedEndpoint(base.Result, this.ResolvedEndPoint);
				SmtpConnectionProbe.SetPortNumber(base.Result, this.WorkDefinition.Port);
				this.MeasureLatency("Connect", () => this.Client.Connect(this.ResolvedEndPoint, this.WorkDefinition.Port, this.DisconnectBetweenSessions), ConnectionLostPoint.OnConnect);
			}
			catch (OperationCanceledException)
			{
				throw;
			}
			catch (Exception arg)
			{
				WTFDiagnostics.TraceInformation<string, int, Exception>(ExTraceGlobals.SMTPConnectionTracer, new TracingContext(), "Unable to connect to server {0} on port {1} due to the following exception: {2}", this.ResolvedEndPoint, this.WorkDefinition.Port, arg, null, "TestConnection", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Smtp\\Probes\\SmtpConnectionProbe.cs", 1321);
				throw;
			}
			if (this.ShouldCancelProbe())
			{
				return;
			}
			this.AssertExpectedResponse(this.WorkDefinition.ExpectedResponseOnConnect);
			this.AfterConnect();
			this.MeasureLatency("EHLO", delegate()
			{
				this.Client.Ehlo(this.WorkDefinition.HeloDomain);
			}, ConnectionLostPoint.OnHelo);
			SmtpConnectionProbe.SetAdvertisedServerName(base.Result, this.Client.AdvertisedServerName);
			if (this.ShouldCancelProbe())
			{
				return;
			}
			this.AssertExpectedResponse(this.WorkDefinition.ExpectedResponseOnHelo);
			this.AfterHelo();
			if (this.WorkDefinition.UseSsl || this.WorkDefinition.AuthenticationType == AuthenticationType.Exchange)
			{
				if (this.WorkDefinition.ClientCertificate != null && this.WorkDefinition.AuthenticationType != AuthenticationType.Exchange)
				{
					this.AddClientCertificatesToSmtp();
				}
				this.MeasureLatency("STARTTLS", delegate()
				{
					this.Client.StartTls(this.WorkDefinition.AuthenticationType == AuthenticationType.Exchange);
				});
				if (this.ShouldCancelProbe())
				{
					return;
				}
				this.AssertExpectedResponse(this.WorkDefinition.ExpectedResponseOnStartTls);
				this.AssertExpectedServerCertificate();
				this.AfterStartTls();
				this.MeasureLatency("EHLO", delegate()
				{
					this.Client.Ehlo(this.WorkDefinition.HeloDomain);
				}, ConnectionLostPoint.OnHeloAfterStartTls);
				if (this.ShouldCancelProbe())
				{
					return;
				}
				this.AssertExpectedResponse(this.WorkDefinition.ExpectedResponseOnHeloAfterStartTls);
				this.AfterHeloAfterStartTls();
			}
			if (this.WorkDefinition.AuthenticationType != AuthenticationType.Anonymous)
			{
				switch (this.WorkDefinition.AuthenticationType)
				{
				case AuthenticationType.AuthLogin:
					this.MeasureLatency("AuthLogin", delegate()
					{
						this.Client.AuthLogin(this.WorkDefinition.AuthenticationAccount.Username, this.WorkDefinition.AuthenticationAccount.Password);
					}, ConnectionLostPoint.OnAuthenticate);
					break;
				case AuthenticationType.Exchange:
					this.MeasureLatency("ExchangeAuth", delegate()
					{
						this.Client.ExchangeAuth();
					}, ConnectionLostPoint.OnAuthenticate);
					break;
				default:
					throw new ArgumentException("Unexpected Authentication type specified in the work definition.");
				}
				if (this.ShouldCancelProbe())
				{
					return;
				}
				this.AssertExpectedResponse(this.WorkDefinition.ExpectedResponseOnAuthenticate);
				this.AfterAuthenticate();
			}
			if (!string.IsNullOrEmpty(this.WorkDefinition.MailFrom))
			{
				this.MeasureLatency("MAILFROM", delegate()
				{
					this.Client.MailFrom(this.GetMailFromArguments());
				}, ConnectionLostPoint.OnMailFrom);
				if (this.ShouldCancelProbe())
				{
					return;
				}
				this.AssertExpectedResponse(this.WorkDefinition.ExpectedResponseOnMailFrom);
				this.AfterMailFrom();
			}
			this.BeforeRcptTo();
			if (this.WorkDefinition.MailTo != null)
			{
				using (IEnumerator<SmtpRecipient> enumerator = this.WorkDefinition.MailTo.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						SmtpRecipient recipient = enumerator.Current;
						this.MeasureLatency("RCPTTO", delegate()
						{
							this.Client.RcptTo(recipient.Username);
						}, ConnectionLostPoint.OnRcptTo);
						if (this.ShouldCancelProbe())
						{
							return;
						}
						this.AssertExpectedResponse(recipient.ExpectedResponse);
					}
				}
				this.AfterRcptTo();
			}
			this.BeforeData();
			if (!string.IsNullOrEmpty(this.WorkDefinition.Data))
			{
				this.MeasureLatency("DATA", delegate()
				{
					this.Client.Data(string.Format("{0}:{1}\r\n{2}", "X-MS-Exchange-ActiveMonitoringProbeName", base.Definition.Name, this.WorkDefinition.Data));
				}, ConnectionLostPoint.OnData);
				if (this.ShouldCancelProbe())
				{
					return;
				}
				this.AssertExpectedResponse(this.WorkDefinition.ExpectedResponseOnData);
				this.AfterData();
			}
		}

		private void RunCustomCommands(CustomCommandRunPoint runPoint)
		{
			IEnumerable<SmtpCustomCommand> enumerable = from p in this.WorkDefinition.CustomCommands
			where p.CustomCommandRunPoint == runPoint
			select p;
			using (IEnumerator<SmtpCustomCommand> enumerator = enumerable.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SmtpCustomCommand customCommand = enumerator.Current;
					this.MeasureLatency(customCommand.Name, delegate()
					{
						this.Client.Send(customCommand.Name + " " + customCommand.Arguments);
					});
					if (this.ShouldCancelProbe())
					{
						break;
					}
					this.AssertExpectedResponse(customCommand.ExpectedResponse);
				}
			}
		}

		private void AddClientCertificatesToSmtp()
		{
			this.Client.ClientCertificates = new X509CertificateCollection();
			if (!string.IsNullOrEmpty(this.WorkDefinition.ClientCertificate.FindValue))
			{
				X509Store x509Store = new X509Store(this.WorkDefinition.ClientCertificate.StoreName, this.WorkDefinition.ClientCertificate.StoreLocation);
				x509Store.Open(OpenFlags.OpenExistingOnly);
				try
				{
					this.Client.ClientCertificates.AddRange(x509Store.Certificates.Find(this.WorkDefinition.ClientCertificate.FindType, this.WorkDefinition.ClientCertificate.FindValue, true));
					WTFDiagnostics.TraceInformation<int>(ExTraceGlobals.SMTPConnectionTracer, new TracingContext(), "Found {0} client certificate(s) matching specified criteria.", this.Client.ClientCertificates.Count, null, "AddClientCertificatesToSmtp", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Smtp\\Probes\\SmtpConnectionProbe.cs", 1534);
					goto IL_19D;
				}
				finally
				{
					x509Store.Close();
				}
			}
			if (!string.IsNullOrEmpty(this.WorkDefinition.ClientCertificate.TransportCertificateName))
			{
				this.Client.ClientCertificates.Add(this.cache.Find(this.WorkDefinition.ClientCertificate.TransportCertificateName));
			}
			else if (!string.IsNullOrEmpty(this.WorkDefinition.ClientCertificate.TransportCertificateFqdn))
			{
				string item = string.IsNullOrEmpty(this.WorkDefinition.ClientCertificate.TransportCertificateFqdn) ? ComputerInformation.DnsPhysicalFullyQualifiedDomainName : this.WorkDefinition.ClientCertificate.TransportCertificateFqdn;
				List<string> list = new List<string>(1);
				list.Add(item);
				this.Client.ClientCertificates.Add(this.cache.Find(list, true, this.WorkDefinition.ClientCertificate.TransportWildcardMatchType));
			}
			IL_19D:
			foreach (X509Certificate x509Certificate in this.Client.ClientCertificates)
			{
				WTFDiagnostics.TraceDebug<string, string, string, string>(ExTraceGlobals.SMTPConnectionTracer, new TracingContext(), "Subject: {0}, Issuer: {1}, Effective: {2} to {3}.\r\n", x509Certificate.Subject, x509Certificate.Issuer, x509Certificate.GetEffectiveDateString(), x509Certificate.GetExpirationDateString(), null, "AddClientCertificatesToSmtp", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Smtp\\Probes\\SmtpConnectionProbe.cs", 1566);
			}
		}

		private bool IsHubTransportInDrainingState()
		{
			Assembly assembly = Assembly.Load("Microsoft.Exchange.Data.Directory");
			if (assembly == null)
			{
				return false;
			}
			Type type = assembly.GetType("Microsoft.Exchange.Data.Directory.ServerComponentStateManager");
			if (type == null)
			{
				return false;
			}
			object[] parameters = new object[]
			{
				ServerComponentEnum.HubTransport,
				false
			};
			Type[] types = new Type[]
			{
				typeof(ServerComponentEnum),
				typeof(bool)
			};
			object obj = type.GetMethod("GetEffectiveState", types).Invoke(null, parameters);
			return obj.ToString() == "Draining";
		}

		private string GetMailFromArguments()
		{
			bool flag = base.Broker.IsLocal() && Datacenter.IsMicrosoftHostedOnly(true) && !string.IsNullOrEmpty(this.WorkDefinition.Data) && this.WorkDefinition.AddAttributions;
			string text2;
			if (flag)
			{
				string text = (this.WorkDefinition.Direction == Directionality.Incoming) ? this.WorkDefinition.RecipientTenantID : this.WorkDefinition.SenderTenantID;
				if (string.IsNullOrWhiteSpace(text))
				{
					text2 = string.Format("{0} XATTRDIRECT={1}", this.WorkDefinition.MailFrom, SmtpUtils.ToXtextString(this.WorkDefinition.Direction.ToString(), false));
				}
				else
				{
					text2 = string.Format("{0} XATTRDIRECT={1} XATTRORGID=xorgid:{2}", this.WorkDefinition.MailFrom, SmtpUtils.ToXtextString(this.WorkDefinition.Direction.ToString(), false), SmtpUtils.ToXtextString(text, false));
				}
			}
			else
			{
				text2 = this.WorkDefinition.MailFrom;
			}
			if (this.Client.IsXSysProbeAdvertised)
			{
				text2 += string.Format(" XSYSPROBEID={0}", this.GetProbeId());
			}
			return text2;
		}

		internal const string ResolvedEndpointBeforeConnect = "The connection has not been established";

		private SmtpConnectionProbeWorkDefinition workDefinition;

		private string resolvedEndPoint;

		private ISimpleSmtpClient client;

		private int testCount;

		private CertificateCache cache;

		internal delegate T ActionWithReturn<T>();
	}
}
