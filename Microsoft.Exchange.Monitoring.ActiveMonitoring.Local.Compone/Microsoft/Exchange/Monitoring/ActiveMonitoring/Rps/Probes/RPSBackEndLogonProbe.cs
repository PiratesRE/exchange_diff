using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Rps.Probes
{
	public class RPSBackEndLogonProbe : ProbeWorkItem
	{
		public RPSBackEndLogonProbe()
		{
			this.xmlDocument = new XmlDocument();
			this.cookies = new CookieContainer();
			this.xmlnsmgr = new XmlNamespaceManager(this.xmlDocument.NameTable);
			this.xmlnsmgr.AddNamespace("f", "http://schemas.microsoft.com/wbem/wsman/1/wsmanfault");
			this.xmlnsmgr.AddNamespace("rsp", "http://schemas.microsoft.com/wbem/wsman/1/windows/shell");
			this.xmlnsmgr.AddNamespace("w", "http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd");
			this.tokenInitialized = false;
			this.conversation = new List<string>();
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.RPSTracer, base.TraceContext, "Entering Dowork", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSBackEndLogonProbe.cs", 155);
			try
			{
				this.InitializeProbeVariables();
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.RPSTracer, base.TraceContext, "{0} start", base.Definition.Name, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSBackEndLogonProbe.cs", 160);
				Task<string> previousTask = this.CreateRPSSession(cancellationToken);
				Task<string> previousTask2 = this.ExecuteCmdlet(previousTask, cancellationToken);
				this.DeleteRPSSession(previousTask2, cancellationToken);
			}
			catch (ApplicationException)
			{
				throw;
			}
			catch (Exception ex)
			{
				this.ThrowTerminatingError(ex.Message, "DoWork() failed. Error message: {0}", new object[]
				{
					ex
				});
			}
			WTFDiagnostics.TraceFunction(ExTraceGlobals.RPSTracer, base.TraceContext, "Leaving Dowork", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSBackEndLogonProbe.cs", 175);
		}

		public override void PopulateDefinition<TDefinition>(TDefinition definition, Dictionary<string, string> propertyBag)
		{
			RPSLogonProbe.PopulateProbeDefinition(definition, propertyBag);
		}

		protected void InitializeProbeVariables()
		{
			if (!this.tokenInitialized)
			{
				this.url = RPSLogonProbe.AppendUrlParameter(base.Definition.Endpoint, "clientApplication", ExchangeRunspaceConfigurationSettings.ExchangeApplication.ActiveMonitor.ToString());
				this.CaculateCommonAccessToken();
				if (!base.Definition.Attributes.ContainsKey("AllowRedirection") || !bool.TryParse(base.Definition.Attributes["AllowRedirection"], out this.allowRedirect))
				{
					this.allowRedirect = true;
				}
				CertificateValidationManager.RegisterCallback("RPSBackEndLogonProbe", (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true);
				this.tokenInitialized = true;
			}
			this.sessionCreated = false;
			this.sessionId = null;
			this.commandId = null;
			this.conversation.Clear();
		}

		private Task<string> CreateRPSSession(CancellationToken cancellationToken)
		{
			Task<WebResponse> task = this.StartCreateSession();
			Task<string> task2 = task.Continue(new Func<Task<WebResponse>, string>(this.ProcessCreateSessionResponse), cancellationToken, TaskContinuationOptions.NotOnCanceled);
			Task<string> task3 = task2;
			for (int i = 0; i < 3; i++)
			{
				Task<WebResponse> task4 = task3.Continue(new Func<string, Task<WebResponse>>(this.StartReceiveSession), cancellationToken, TaskContinuationOptions.OnlyOnRanToCompletion);
				task3 = task4.Continue(new Func<Task<WebResponse>, string>(this.ProcessReceiveSessionResponse), cancellationToken, TaskContinuationOptions.NotOnCanceled);
			}
			return task3;
		}

		private Task<string> ExecuteCmdlet(Task<string> previousTask, CancellationToken cancellationToken)
		{
			Task<WebResponse> task = previousTask.Continue(new Func<string, Task<WebResponse>>(this.StartCommand), cancellationToken, TaskContinuationOptions.NotOnCanceled);
			Task<string> task2 = task.Continue(new Func<Task<WebResponse>, string>(this.ProcessCommandResponse), cancellationToken, TaskContinuationOptions.NotOnCanceled);
			Task<WebResponse> task3 = task2.Continue(new Func<string, Task<WebResponse>>(this.StartReceiveCommand), cancellationToken, TaskContinuationOptions.NotOnCanceled);
			return task3.Continue(new Func<Task<WebResponse>, string>(this.ProcessReceiveCommandResponse), cancellationToken, TaskContinuationOptions.NotOnCanceled);
		}

		private Task<string> DeleteRPSSession(Task<string> previousTask, CancellationToken cancellationToken)
		{
			Task<WebResponse> task = previousTask.Continue(new Func<string, Task<WebResponse>>(this.StartDeleteSession), cancellationToken, TaskContinuationOptions.NotOnCanceled);
			return task.Continue(new Func<Task<WebResponse>, string>(this.ProcessDeleteSessionResponse), cancellationToken, TaskContinuationOptions.NotOnCanceled);
		}

		private Task<WebResponse> StartCreateSession()
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.RPSTracer, base.TraceContext, "Entering StartCreateSession", null, "StartCreateSession", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSBackEndLogonProbe.cs", 307);
			Task<WebResponse> result = this.SendRPSRequestPackage(string.Format("<s:Envelope xmlns:s=\"http://www.w3.org/2003/05/soap-envelope\" xmlns:a=\"http://schemas.xmlsoap.org/ws/2004/08/addressing\" xmlns:w=\"http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd\" xmlns:p=\"http://schemas.microsoft.com/wbem/wsman/1/wsman.xsd\"><s:Header><a:To>{0}</a:To><w:ResourceURI s:mustUnderstand=\"true\">http://schemas.microsoft.com/powershell/Microsoft.Exchange</w:ResourceURI><a:ReplyTo><a:Address s:mustUnderstand=\"true\">http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous</a:Address></a:ReplyTo><a:Action s:mustUnderstand=\"true\">http://schemas.xmlsoap.org/ws/2004/09/transfer/Create</a:Action><w:MaxEnvelopeSize s:mustUnderstand=\"true\">153600</w:MaxEnvelopeSize><a:MessageID>uuid:D9C7EA80-68E8-4140-8678-D1979D2518E8</a:MessageID><w:Locale xml:lang=\"en-US\" s:mustUnderstand=\"false\" /><p:DataLocale xml:lang=\"en-US\" s:mustUnderstand=\"false\" /><w:OptionSet xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" s:mustUnderstand=\"true\"><w:Option Name=\"protocolversion\" MustComply=\"true\">2.1</w:Option></w:OptionSet><w:OperationTimeout>PT180.000S</w:OperationTimeout><rsp:CompressionType s:mustUnderstand=\"true\" xmlns:rsp=\"http://schemas.microsoft.com/wbem/wsman/1/windows/shell\">xpress</rsp:CompressionType></s:Header><s:Body><rsp:Shell xmlns:rsp=\"http://schemas.microsoft.com/wbem/wsman/1/windows/shell\"><rsp:IdleTimeOut>PT240.000S</rsp:IdleTimeOut><rsp:InputStreams>stdin pr</rsp:InputStreams><rsp:OutputStreams>stdout</rsp:OutputStreams><creationXml xmlns=\"http://schemas.microsoft.com/powershell\">AAAAAAAAAAcAAAAAAAAAAAMAAALwAgAAAAIAAQBDBF6bTpfeRr6/0+HmQDgeAAAAAAAAAAAAAAAAAAAAAO+7vzxPYmogUmVmSWQ9IjAiPjxNUz48VmVyc2lvbiBOPSJwcm90b2NvbHZlcnNpb24iPjIuMTwvVmVyc2lvbj48VmVyc2lvbiBOPSJQU1ZlcnNpb24iPjIuMDwvVmVyc2lvbj48VmVyc2lvbiBOPSJTZXJpYWxpemF0aW9uVmVyc2lvbiI+MS4xLjAuMTwvVmVyc2lvbj48QkEgTj0iVGltZVpvbmUiPkFBRUFBQUQvLy8vL0FRQUFBQUFBQUFBRUFRQUFBQnhUZVhOMFpXMHVRM1Z5Y21WdWRGTjVjM1JsYlZScGJXVmFiMjVsQkFBQUFCZHRYME5oWTJobFpFUmhlV3hwWjJoMFEyaGhibWRsY3cxdFgzUnBZMnR6VDJabWMyVjBEbTFmYzNSaGJtUmhjbVJPWVcxbERtMWZaR0Y1YkdsbmFIUk9ZVzFsQXdBQkFSeFRlWE4wWlcwdVEyOXNiR1ZqZEdsdmJuTXVTR0Z6YUhSaFlteGxDUWtDQUFBQUFNRGM4YnovLy84S0NnUUNBQUFBSEZONWMzUmxiUzVEYjJ4c1pXTjBhVzl1Y3k1SVlYTm9kR0ZpYkdVSEFBQUFDa3h2WVdSR1lXTjBiM0lIVm1WeWMybHZiZ2hEYjIxd1lYSmxjaEJJWVhOb1EyOWtaVkJ5YjNacFpHVnlDRWhoYzJoVGFYcGxCRXRsZVhNR1ZtRnNkV1Z6QUFBREF3QUZCUXNJSEZONWMzUmxiUzVEYjJ4c1pXTjBhVzl1Y3k1SlEyOXRjR0Z5WlhJa1UzbHpkR1Z0TGtOdmJHeGxZM1JwYjI1ekxrbElZWE5vUTI5a1pWQnliM1pwWkdWeUNPeFJPRDhBQUFBQUNnb0RBQUFBQ1FNQUFBQUpCQUFBQUJBREFBQUFBQUFBQUJBRUFBQUFBQUFBQUFzPTwvQkE+PC9NUz48L09iaj4AAAAAAAAACAAAAAAAAAAAAwAADeYCAAAABAABAEMEXptOl95Gvr/T4eZAOB4AAAAAAAAAAAAAAAAAAAAA77u/PE9iaiBSZWZJZD0iMCI+PE1TPjxJMzIgTj0iTWluUnVuc3BhY2VzIj4xPC9JMzI+PEkzMiBOPSJNYXhSdW5zcGFjZXMiPjE8L0kzMj48T2JqIE49IlBTVGhyZWFkT3B0aW9ucyIgUmVmSWQ9IjEiPjxUTiBSZWZJZD0iMCI+PFQ+U3lzdGVtLk1hbmFnZW1lbnQuQXV0b21hdGlvbi5SdW5zcGFjZXMuUFNUaHJlYWRPcHRpb25zPC9UPjxUPlN5c3RlbS5FbnVtPC9UPjxUPlN5c3RlbS5WYWx1ZVR5cGU8L1Q+PFQ+U3lzdGVtLk9iamVjdDwvVD48L1ROPjxUb1N0cmluZz5EZWZhdWx0PC9Ub1N0cmluZz48STMyPjA8L0kzMj48L09iaj48T2JqIE49IkFwYXJ0bWVudFN0YXRlIiBSZWZJZD0iMiI+PFROIFJlZklkPSIxIj48VD5TeXN0ZW0uVGhyZWFkaW5nLkFwYXJ0bWVudFN0YXRlPC9UPjxUPlN5c3RlbS5FbnVtPC9UPjxUPlN5c3RlbS5WYWx1ZVR5cGU8L1Q+PFQ+U3lzdGVtLk9iamVjdDwvVD48L1ROPjxUb1N0cmluZz5Vbmtub3duPC9Ub1N0cmluZz48STMyPjI8L0kzMj48L09iaj48T2JqIE49IkFwcGxpY2F0aW9uQXJndW1lbnRzIiBSZWZJZD0iMyI+PFROIFJlZklkPSIyIj48VD5TeXN0ZW0uTWFuYWdlbWVudC5BdXRvbWF0aW9uLlBTUHJpbWl0aXZlRGljdGlvbmFyeTwvVD48VD5TeXN0ZW0uQ29sbGVjdGlvbnMuSGFzaHRhYmxlPC9UPjxUPlN5c3RlbS5PYmplY3Q8L1Q+PC9UTj48RENUPjxFbj48UyBOPSJLZXkiPlBTVmVyc2lvblRhYmxlPC9TPjxPYmogTj0iVmFsdWUiIFJlZklkPSI0Ij48VE5SZWYgUmVmSWQ9IjIiIC8+PERDVD48RW4+PFMgTj0iS2V5Ij5XU01hblN0YWNrVmVyc2lvbjwvUz48VmVyc2lvbiBOPSJWYWx1ZSI+Mi4wPC9WZXJzaW9uPjwvRW4+PEVuPjxTIE49IktleSI+UFNDb21wYXRpYmxlVmVyc2lvbnM8L1M+PE9iaiBOPSJWYWx1ZSIgUmVmSWQ9IjUiPjxUTiBSZWZJZD0iMyI+PFQ+U3lzdGVtLlZlcnNpb25bXTwvVD48VD5TeXN0ZW0uQXJyYXk8L1Q+PFQ+U3lzdGVtLk9iamVjdDwvVD48L1ROPjxMU1Q+PFZlcnNpb24+MS4wPC9WZXJzaW9uPjxWZXJzaW9uPjIuMDwvVmVyc2lvbj48L0xTVD48L09iaj48L0VuPjxFbj48UyBOPSJLZXkiPkJ1aWxkVmVyc2lvbjwvUz48VmVyc2lvbiBOPSJWYWx1ZSI+Ni4xLjc2MDEuMTc1MTQ8L1ZlcnNpb24+PC9Fbj48RW4+PFMgTj0iS2V5Ij5QU1JlbW90aW5nUHJvdG9jb2xWZXJzaW9uPC9TPjxWZXJzaW9uIE49IlZhbHVlIj4yLjE8L1ZlcnNpb24+PC9Fbj48RW4+PFMgTj0iS2V5Ij5QU1ZlcnNpb248L1M+PFZlcnNpb24gTj0iVmFsdWUiPjIuMDwvVmVyc2lvbj48L0VuPjxFbj48UyBOPSJLZXkiPkNMUlZlcnNpb248L1M+PFZlcnNpb24gTj0iVmFsdWUiPjQuMC4zMDMxOS4xPC9WZXJzaW9uPjwvRW4+PEVuPjxTIE49IktleSI+U2VyaWFsaXphdGlvblZlcnNpb248L1M+PFZlcnNpb24gTj0iVmFsdWUiPjEuMS4wLjE8L1ZlcnNpb24+PC9Fbj48L0RDVD48L09iaj48L0VuPjwvRENUPjwvT2JqPjxPYmogTj0iSG9zdEluZm8iIFJlZklkPSI2Ij48TVM+PE9iaiBOPSJfaG9zdERlZmF1bHREYXRhIiBSZWZJZD0iNyI+PE1TPjxPYmogTj0iZGF0YSIgUmVmSWQ9IjgiPjxUTiBSZWZJZD0iNCI+PFQ+U3lzdGVtLkNvbGxlY3Rpb25zLkhhc2h0YWJsZTwvVD48VD5TeXN0ZW0uT2JqZWN0PC9UPjwvVE4+PERDVD48RW4+PEkzMiBOPSJLZXkiPjk8L0kzMj48T2JqIE49IlZhbHVlIiBSZWZJZD0iOSI+PE1TPjxTIE49IlQiPlN5c3RlbS5TdHJpbmc8L1M+PFMgTj0iViI+QWRtaW5pc3RyYXRvcjogV2luZG93cyBQb3dlclNoZWxsPC9TPjwvTVM+PC9PYmo+PC9Fbj48RW4+PEkzMiBOPSJLZXkiPjg8L0kzMj48T2JqIE49IlZhbHVlIiBSZWZJZD0iMTAiPjxNUz48UyBOPSJUIj5TeXN0ZW0uTWFuYWdlbWVudC5BdXRvbWF0aW9uLkhvc3QuU2l6ZTwvUz48T2JqIE49IlYiIFJlZklkPSIxMSI+PE1TPjxJMzIgTj0id2lkdGgiPjE1OTwvSTMyPjxJMzIgTj0iaGVpZ2h0Ij43NDwvSTMyPjwvTVM+PC9PYmo+PC9NUz48L09iaj48L0VuPjxFbj48STMyIE49IktleSI+NzwvSTMyPjxPYmogTj0iVmFsdWUiIFJlZklkPSIxMiI+PE1TPjxTIE49IlQiPlN5c3RlbS5NYW5hZ2VtZW50LkF1dG9tYXRpb24uSG9zdC5TaXplPC9TPjxPYmogTj0iViIgUmVmSWQ9IjEzIj48TVM+PEkzMiBOPSJ3aWR0aCI+MTIwPC9JMzI+PEkzMiBOPSJoZWlnaHQiPjc0PC9JMzI+PC9NUz48L09iaj48L01TPjwvT2JqPjwvRW4+PEVuPjxJMzIgTj0iS2V5Ij42PC9JMzI+PE9iaiBOPSJWYWx1ZSIgUmVmSWQ9IjE0Ij48TVM+PFMgTj0iVCI+U3lzdGVtLk1hbmFnZW1lbnQuQXV0b21hdGlvbi5Ib3N0LlNpemU8L1M+PE9iaiBOPSJWIiBSZWZJZD0iMTUiPjxNUz48STMyIE49IndpZHRoIj4xMjA8L0kzMj48STMyIE49ImhlaWdodCI+NTA8L0kzMj48L01TPjwvT2JqPjwvTVM+PC9PYmo+PC9Fbj48RW4+PEkzMiBOPSJLZXkiPjU8L0kzMj48T2JqIE49IlZhbHVlIiBSZWZJZD0iMTYiPjxNUz48UyBOPSJUIj5TeXN0ZW0uTWFuYWdlbWVudC5BdXRvbWF0aW9uLkhvc3QuU2l6ZTwvUz48T2JqIE49IlYiIFJlZklkPSIxNyI+PE1TPjxJMzIgTj0id2lkdGgiPjEyMDwvSTMyPjxJMzIgTj0iaGVpZ2h0Ij4zMDAwPC9JMzI+PC9NUz48L09iaj48L01TPjwvT2JqPjwvRW4+PEVuPjxJMzIgTj0iS2V5Ij40PC9JMzI+PE9iaiBOPSJWYWx1ZSIgUmVmSWQ9IjE4Ij48TVM+PFMgTj0iVCI+U3lzdGVtLkludDMyPC9TPjxJMzIgTj0iViI+MjU8L0kzMj48L01TPjwvT2JqPjwvRW4+PEVuPjxJMzIgTj0iS2V5Ij4zPC9JMzI+PE9iaiBOPSJWYWx1ZSIgUmVmSWQ9IjE5Ij48TVM+PFMgTj0iVCI+U3lzdGVtLk1hbmFnZW1lbnQuQXV0b21hdGlvbi5Ib3N0LkNvb3JkaW5hdGVzPC9TPjxPYmogTj0iViIgUmVmSWQ9IjIwIj48TVM+PEkzMiBOPSJ4Ij4wPC9JMzI+PEkzMiBOPSJ5Ij42PC9JMzI+PC9NUz48L09iaj48L01TPjwvT2JqPjwvRW4+PEVuPjxJMzIgTj0iS2V5Ij4yPC9JMzI+PE9iaiBOPSJWYWx1ZSIgUmVmSWQ9IjIxIj48TVM+PFMgTj0iVCI+U3lzdGVtLk1hbmFnZW1lbnQuQXV0b21hdGlvbi5Ib3N0LkNvb3JkaW5hdGVzPC9TPjxPYmogTj0iViIgUmVmSWQ9IjIyIj48TVM+PEkzMiBOPSJ4Ij4wPC9JMzI+PEkzMiBOPSJ5Ij41NTwvSTMyPjwvTVM+PC9PYmo+PC9NUz48L09iaj48L0VuPjxFbj48STMyIE49IktleSI+MTwvSTMyPjxPYmogTj0iVmFsdWUiIFJlZklkPSIyMyI+PE1TPjxTIE49IlQiPlN5c3RlbS5Db25zb2xlQ29sb3I8L1M+PEkzMiBOPSJWIj41PC9JMzI+PC9NUz48L09iaj48L0VuPjxFbj48STMyIE49IktleSI+MDwvSTMyPjxPYmogTj0iVmFsdWUiIFJlZklkPSIyNCI+PE1TPjxTIE49IlQiPlN5c3RlbS5Db25zb2xlQ29sb3I8L1M+PEkzMiBOPSJWIj42PC9JMzI+PC9NUz48L09iaj48L0VuPjwvRENUPjwvT2JqPjwvTVM+PC9PYmo+PEIgTj0iX2lzSG9zdE51bGwiPmZhbHNlPC9CPjxCIE49Il9pc0hvc3RVSU51bGwiPmZhbHNlPC9CPjxCIE49Il9pc0hvc3RSYXdVSU51bGwiPmZhbHNlPC9CPjxCIE49Il91c2VSdW5zcGFjZUhvc3QiPmZhbHNlPC9CPjwvTVM+PC9PYmo+PC9NUz48L09iaj4=</creationXml></rsp:Shell></s:Body></s:Envelope>", this.url));
			WTFDiagnostics.TraceFunction(ExTraceGlobals.RPSTracer, base.TraceContext, "Leaving StartCreateSession", null, "StartCreateSession", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSBackEndLogonProbe.cs", 309);
			return result;
		}

		private Task<WebResponse> StartReceiveSession(string previousResponse)
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.RPSTracer, base.TraceContext, "Entering StartReceiveSession", null, "StartReceiveSession", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSBackEndLogonProbe.cs", 320);
			Task<WebResponse> result;
			if (!this.sessionCreated)
			{
				result = this.SendRPSRequestPackage(string.Format("<s:Envelope xmlns:s=\"http://www.w3.org/2003/05/soap-envelope\" xmlns:a=\"http://schemas.xmlsoap.org/ws/2004/08/addressing\" xmlns:w=\"http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd\" xmlns:p=\"http://schemas.microsoft.com/wbem/wsman/1/wsman.xsd\"><s:Header><a:To>{0}</a:To><w:ResourceURI s:mustUnderstand=\"true\">http://schemas.microsoft.com/powershell/Microsoft.Exchange</w:ResourceURI><a:ReplyTo><a:Address s:mustUnderstand=\"true\">http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous</a:Address></a:ReplyTo><a:Action s:mustUnderstand=\"true\">http://schemas.microsoft.com/wbem/wsman/1/windows/shell/Receive</a:Action><w:MaxEnvelopeSize s:mustUnderstand=\"true\">153600</w:MaxEnvelopeSize><a:MessageID>uuid:0B74A582-89AA-486E-9DEF-2F38707F75D2</a:MessageID><w:Locale xml:lang=\"en-US\" s:mustUnderstand=\"false\" /><p:DataLocale xml:lang=\"en-US\" s:mustUnderstand=\"false\" /><w:SelectorSet><w:Selector Name=\"ShellId\">{1}</w:Selector></w:SelectorSet><w:OptionSet xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"><w:Option Name=\"WSMAN_CMDSHELL_OPTION_KEEPALIVE\">TRUE</w:Option></w:OptionSet><w:OperationTimeout>PT180.000S</w:OperationTimeout></s:Header><s:Body><rsp:Receive xmlns:rsp=\"http://schemas.microsoft.com/wbem/wsman/1/windows/shell\"  SequenceId=\"0\"><rsp:DesiredStream>stdout</rsp:DesiredStream></rsp:Receive></s:Body></s:Envelope>", this.url, this.sessionId));
				WTFDiagnostics.TraceDebug(ExTraceGlobals.RPSTracer, base.TraceContext, "Send receive session request", null, "StartReceiveSession", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSBackEndLogonProbe.cs", 326);
			}
			else
			{
				result = Task.Factory.StartNew<WebResponse>(() => null);
				WTFDiagnostics.TraceDebug(ExTraceGlobals.RPSTracer, base.TraceContext, "Send dummy request", null, "StartReceiveSession", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSBackEndLogonProbe.cs", 332);
			}
			WTFDiagnostics.TraceFunction(ExTraceGlobals.RPSTracer, base.TraceContext, "Leaving StartReceiveSession", null, "StartReceiveSession", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSBackEndLogonProbe.cs", 335);
			return result;
		}

		private Task<WebResponse> StartCommand(string previousResponse)
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.RPSTracer, base.TraceContext, "Entering StartCommand", null, "StartCommand", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSBackEndLogonProbe.cs", 346);
			Task<WebResponse> result;
			if (this.sessionCreated)
			{
				result = this.SendRPSRequestPackage(string.Format("<s:Envelope xmlns:s=\"http://www.w3.org/2003/05/soap-envelope\" xmlns:a=\"http://schemas.xmlsoap.org/ws/2004/08/addressing\" xmlns:w=\"http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd\" xmlns:p=\"http://schemas.microsoft.com/wbem/wsman/1/wsman.xsd\"><s:Header><a:To>{0}</a:To><a:ReplyTo><a:Address s:mustUnderstand=\"true\">http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous</a:Address></a:ReplyTo><a:Action s:mustUnderstand=\"true\">http://schemas.microsoft.com/wbem/wsman/1/windows/shell/Command</a:Action><w:MaxEnvelopeSize s:mustUnderstand=\"true\">153600</w:MaxEnvelopeSize><a:MessageID>uuid:777E12FC-B65F-4AC5-917F-A38B7EA6622A</a:MessageID><w:Locale xml:lang=\"en-US\" s:mustUnderstand=\"false\" /><p:DataLocale xml:lang=\"en-US\" s:mustUnderstand=\"false\" /><w:ResourceURI xmlns:w=\"http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd\">http://schemas.microsoft.com/powershell/Microsoft.Exchange</w:ResourceURI><w:SelectorSet xmlns:w=\"http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd\" xmlns=\"http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd\"><w:Selector Name=\"ShellId\">{1}</w:Selector></w:SelectorSet><w:OperationTimeout>PT180.000S</w:OperationTimeout></s:Header><s:Body><rsp:CommandLine xmlns:rsp=\"http://schemas.microsoft.com/wbem/wsman/1/windows/shell\"><rsp:Command> </rsp:Command><rsp:Arguments>AAAAAAAAAAkAAAAAAAAAAAMAAAbVAgAAAAYQAgBDBF6bTpfeRr6/0+HmQDgehsZH1YtzIUaYJjd8M5MEs++7vzxPYmogUmVmSWQ9IjAiPjxNUz48T2JqIE49IlBvd2VyU2hlbGwiIFJlZklkPSIxIj48TVM+PE9iaiBOPSJDbWRzIiBSZWZJZD0iMiI+PFROIFJlZklkPSIwIj48VD5TeXN0ZW0uQ29sbGVjdGlvbnMuR2VuZXJpYy5MaXN0YDFbW1N5c3RlbS5NYW5hZ2VtZW50LkF1dG9tYXRpb24uUFNPYmplY3QsIFN5c3RlbS5NYW5hZ2VtZW50LkF1dG9tYXRpb24sIFZlcnNpb249MS4wLjAuMCwgQ3VsdHVyZT1uZXV0cmFsLCBQdWJsaWNLZXlUb2tlbj0zMWJmMzg1NmFkMzY0ZTM1XV08L1Q+PFQ+U3lzdGVtLk9iamVjdDwvVD48L1ROPjxMU1Q+PE9iaiBSZWZJZD0iMyI+PE1TPjxTIE49IkNtZCI+Z2V0LW1haWxib3g8L1M+PEIgTj0iSXNTY3JpcHQiPmZhbHNlPC9CPjxOaWwgTj0iVXNlTG9jYWxTY29wZSIgLz48T2JqIE49Ik1lcmdlTXlSZXN1bHQiIFJlZklkPSI0Ij48VE4gUmVmSWQ9IjEiPjxUPlN5c3RlbS5NYW5hZ2VtZW50LkF1dG9tYXRpb24uUnVuc3BhY2VzLlBpcGVsaW5lUmVzdWx0VHlwZXM8L1Q+PFQ+U3lzdGVtLkVudW08L1Q+PFQ+U3lzdGVtLlZhbHVlVHlwZTwvVD48VD5TeXN0ZW0uT2JqZWN0PC9UPjwvVE4+PFRvU3RyaW5nPk5vbmU8L1RvU3RyaW5nPjxJMzI+MDwvSTMyPjwvT2JqPjxPYmogTj0iTWVyZ2VUb1Jlc3VsdCIgUmVmSWQ9IjUiPjxUTlJlZiBSZWZJZD0iMSIgLz48VG9TdHJpbmc+Tm9uZTwvVG9TdHJpbmc+PEkzMj4wPC9JMzI+PC9PYmo+PE9iaiBOPSJNZXJnZVByZXZpb3VzUmVzdWx0cyIgUmVmSWQ9IjYiPjxUTlJlZiBSZWZJZD0iMSIgLz48VG9TdHJpbmc+Tm9uZTwvVG9TdHJpbmc+PEkzMj4wPC9JMzI+PC9PYmo+PE9iaiBOPSJBcmdzIiBSZWZJZD0iNyI+PFROUmVmIFJlZklkPSIwIiAvPjxMU1Q+PE9iaiBSZWZJZD0iOCI+PE1TPjxTIE49Ik4iPi1yZXN1bHRzaXplOjwvUz48STMyIE49IlYiPjE8L0kzMj48L01TPjwvT2JqPjwvTFNUPjwvT2JqPjwvTVM+PC9PYmo+PC9MU1Q+PC9PYmo+PEIgTj0iSXNOZXN0ZWQiPmZhbHNlPC9CPjxOaWwgTj0iSGlzdG9yeSIgLz48QiBOPSJSZWRpcmVjdFNoZWxsRXJyb3JPdXRwdXRQaXBlIj50cnVlPC9CPjwvTVM+PC9PYmo+PEIgTj0iTm9JbnB1dCI+dHJ1ZTwvQj48T2JqIE49IkFwYXJ0bWVudFN0YXRlIiBSZWZJZD0iOSI+PFROIFJlZklkPSIyIj48VD5TeXN0ZW0uVGhyZWFkaW5nLkFwYXJ0bWVudFN0YXRlPC9UPjxUPlN5c3RlbS5FbnVtPC9UPjxUPlN5c3RlbS5WYWx1ZVR5cGU8L1Q+PFQ+U3lzdGVtLk9iamVjdDwvVD48L1ROPjxUb1N0cmluZz5Vbmtub3duPC9Ub1N0cmluZz48STMyPjI8L0kzMj48L09iaj48T2JqIE49IlJlbW90ZVN0cmVhbU9wdGlvbnMiIFJlZklkPSIxMCI+PFROIFJlZklkPSIzIj48VD5TeXN0ZW0uTWFuYWdlbWVudC5BdXRvbWF0aW9uLlJlbW90ZVN0cmVhbU9wdGlvbnM8L1Q+PFQ+U3lzdGVtLkVudW08L1Q+PFQ+U3lzdGVtLlZhbHVlVHlwZTwvVD48VD5TeXN0ZW0uT2JqZWN0PC9UPjwvVE4+PFRvU3RyaW5nPjA8L1RvU3RyaW5nPjxJMzI+MDwvSTMyPjwvT2JqPjxCIE49IkFkZFRvSGlzdG9yeSI+dHJ1ZTwvQj48T2JqIE49Ikhvc3RJbmZvIiBSZWZJZD0iMTEiPjxNUz48QiBOPSJfaXNIb3N0TnVsbCI+dHJ1ZTwvQj48QiBOPSJfaXNIb3N0VUlOdWxsIj50cnVlPC9CPjxCIE49Il9pc0hvc3RSYXdVSU51bGwiPnRydWU8L0I+PEIgTj0iX3VzZVJ1bnNwYWNlSG9zdCI+dHJ1ZTwvQj48L01TPjwvT2JqPjwvTVM+PC9PYmo+</rsp:Arguments></rsp:CommandLine></s:Body></s:Envelope>", this.url, this.sessionId));
				WTFDiagnostics.TraceDebug(ExTraceGlobals.RPSTracer, base.TraceContext, "Send command request", null, "StartCommand", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSBackEndLogonProbe.cs", 351);
			}
			else
			{
				result = Task.Factory.StartNew<WebResponse>(() => null);
				WTFDiagnostics.TraceDebug(ExTraceGlobals.RPSTracer, base.TraceContext, "Send dummy request", null, "StartCommand", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSBackEndLogonProbe.cs", 357);
			}
			WTFDiagnostics.TraceFunction(ExTraceGlobals.RPSTracer, base.TraceContext, "Leaving StartCommand", null, "StartCommand", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSBackEndLogonProbe.cs", 360);
			return result;
		}

		private Task<WebResponse> StartReceiveCommand(string previousResponse)
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.RPSTracer, base.TraceContext, "Entering StartReceiveCommand", null, "StartReceiveCommand", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSBackEndLogonProbe.cs", 371);
			Task<WebResponse> result;
			if (this.sessionCreated)
			{
				result = this.SendRPSRequestPackage(string.Format("<s:Envelope xmlns:s=\"http://www.w3.org/2003/05/soap-envelope\" xmlns:a=\"http://schemas.xmlsoap.org/ws/2004/08/addressing\" xmlns:w=\"http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd\" xmlns:p=\"http://schemas.microsoft.com/wbem/wsman/1/wsman.xsd\"><s:Header><a:To>{0}</a:To><a:ReplyTo><a:Address s:mustUnderstand=\"true\">http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous</a:Address></a:ReplyTo><a:Action s:mustUnderstand=\"true\">http://schemas.microsoft.com/wbem/wsman/1/windows/shell/Receive</a:Action><w:MaxEnvelopeSize s:mustUnderstand=\"true\">153600</w:MaxEnvelopeSize><a:MessageID>uuid:AB4B9CB2-8CA3-43A6-B505-EAC948EFEA9A</a:MessageID><w:Locale xml:lang=\"en-US\" s:mustUnderstand=\"false\" /><p:DataLocale xml:lang=\"en-US\" s:mustUnderstand=\"false\" /><w:ResourceURI xmlns:w=\"http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd\">http://schemas.microsoft.com/powershell/Microsoft.Exchange</w:ResourceURI><w:SelectorSet xmlns:w=\"http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd\" xmlns=\"http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd\"><w:Selector Name=\"ShellId\">{1}</w:Selector></w:SelectorSet><w:OperationTimeout>PT180.000S</w:OperationTimeout></s:Header><s:Body><rsp:Receive xmlns:rsp=\"http://schemas.microsoft.com/wbem/wsman/1/windows/shell\"  SequenceId=\"0\"><rsp:DesiredStream CommandId=\"{2}\">stdout</rsp:DesiredStream></rsp:Receive></s:Body></s:Envelope>", this.url, this.sessionId, this.commandId));
				WTFDiagnostics.TraceDebug(ExTraceGlobals.RPSTracer, base.TraceContext, "Send receivecommand request", null, "StartReceiveCommand", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSBackEndLogonProbe.cs", 377);
			}
			else
			{
				result = Task.Factory.StartNew<WebResponse>(() => null);
				WTFDiagnostics.TraceDebug(ExTraceGlobals.RPSTracer, base.TraceContext, "Send dummy request", null, "StartReceiveCommand", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSBackEndLogonProbe.cs", 383);
			}
			WTFDiagnostics.TraceFunction(ExTraceGlobals.RPSTracer, base.TraceContext, "Leaving StartReceiveCommand", null, "StartReceiveCommand", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSBackEndLogonProbe.cs", 386);
			return result;
		}

		private Task<WebResponse> StartDeleteSession(string previousResponse)
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.RPSTracer, base.TraceContext, "Entering StartDeleteSession", null, "StartDeleteSession", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSBackEndLogonProbe.cs", 397);
			Task<WebResponse> result;
			if (this.sessionCreated)
			{
				result = this.SendRPSRequestPackage(string.Format("<s:Envelope xmlns:s=\"http://www.w3.org/2003/05/soap-envelope\" xmlns:a=\"http://schemas.xmlsoap.org/ws/2004/08/addressing\" xmlns:w=\"http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd\" xmlns:p=\"http://schemas.microsoft.com/wbem/wsman/1/wsman.xsd\"><s:Header><a:To>{0}</a:To><a:ReplyTo><a:Address s:mustUnderstand=\"true\">http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous</a:Address></a:ReplyTo><a:Action s:mustUnderstand=\"true\">http://schemas.xmlsoap.org/ws/2004/09/transfer/Delete</a:Action><w:MaxEnvelopeSize s:mustUnderstand=\"true\">153600</w:MaxEnvelopeSize><a:MessageID>uuid:9C7D31C9-1389-4D4A-8103-C1A0E5433950</a:MessageID><w:Locale xml:lang=\"en-US\" s:mustUnderstand=\"false\" /><p:DataLocale xml:lang=\"en-US\" s:mustUnderstand=\"false\" /><w:ResourceURI xmlns:w=\"http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd\">http://schemas.microsoft.com/powershell/Microsoft.Exchange</w:ResourceURI><w:SelectorSet xmlns:w=\"http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd\" xmlns=\"http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd\"><w:Selector Name=\"ShellId\">{1}</w:Selector></w:SelectorSet><w:OperationTimeout>PT60.000S</w:OperationTimeout></s:Header><s:Body></s:Body></s:Envelope>", this.url, this.sessionId));
				WTFDiagnostics.TraceDebug(ExTraceGlobals.RPSTracer, base.TraceContext, "Send deletesession request", null, "StartDeleteSession", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSBackEndLogonProbe.cs", 402);
			}
			else
			{
				result = Task.Factory.StartNew<WebResponse>(() => null);
				WTFDiagnostics.TraceDebug(ExTraceGlobals.RPSTracer, base.TraceContext, "Send dummy request", null, "StartDeleteSession", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSBackEndLogonProbe.cs", 408);
			}
			WTFDiagnostics.TraceFunction(ExTraceGlobals.RPSTracer, base.TraceContext, "Leaving StartDeleteSession", null, "StartDeleteSession", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSBackEndLogonProbe.cs", 411);
			return result;
		}

		private HttpWebRequest CreateRPSHttpWebRequest(string url)
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			httpWebRequest.Method = "POST";
			httpWebRequest.UserAgent = "Exchange BackEnd Probes";
			httpWebRequest.ContentType = "application/soap+xml;charset=UTF-8";
			httpWebRequest.KeepAlive = true;
			httpWebRequest.ServicePoint.Expect100Continue = false;
			httpWebRequest.PreAuthenticate = true;
			httpWebRequest.UnsafeAuthenticatedConnectionSharing = true;
			httpWebRequest.AllowAutoRedirect = this.allowRedirect;
			httpWebRequest.CookieContainer = this.cookies;
			string cookieHeader = httpWebRequest.CookieContainer.GetCookieHeader(httpWebRequest.RequestUri);
			httpWebRequest.Headers.Set("Cookie", cookieHeader);
			httpWebRequest.Headers.Add("X-CommonAccessToken", this.token.Serialize());
			if (this.token.TokenType.Equals(AccessTokenType.LiveIdBasic.ToString()))
			{
				httpWebRequest.Headers.Add("X-WLID-MemberName", this.token.ExtensionData["MemberName"]);
			}
			CertificateValidationManager.SetComponentId(httpWebRequest, "RPSBackEndLogonProbe");
			httpWebRequest.Credentials = CredentialCache.DefaultNetworkCredentials.GetCredential(httpWebRequest.RequestUri, "Kerberos");
			return httpWebRequest;
		}

		private XmlNodeList ParseResponseStream(string responseBody, string xPathQuery)
		{
			try
			{
				this.xmlDocument.LoadXml(responseBody);
				return this.xmlDocument.SelectNodes(xPathQuery, this.xmlnsmgr);
			}
			catch (Exception ex)
			{
				this.ThrowTerminatingError(ex.Message, "Cannot parse RPS resposne body. Error:{0}, xml = {1}", new object[]
				{
					ex,
					responseBody
				});
			}
			return null;
		}

		private void CaculateCommonAccessToken()
		{
			AccessTokenType accessTokenType;
			if (!Enum.TryParse<AccessTokenType>(base.Definition.Attributes["AccessTokenType"], out accessTokenType))
			{
				this.ThrowTerminatingError("Unexpected AccessToken", "Create RPSBackEndLogon probe without correct 'AccessTokenType'!", new object[0]);
			}
			AccessTokenType accessTokenType2 = accessTokenType;
			switch (accessTokenType2)
			{
			case AccessTokenType.Windows:
				this.token = CommonAccessTokenHelper.CreateWindows(base.Definition.Account);
				return;
			case AccessTokenType.LiveId:
				break;
			case AccessTokenType.LiveIdBasic:
				this.token = CommonAccessTokenHelper.CreateLiveIdBasic(base.Definition.Account);
				return;
			default:
				switch (accessTokenType2)
				{
				case AccessTokenType.CertificateSid:
					this.token = CommonAccessTokenHelper.CreateCertificateSid(base.Definition.Account);
					return;
				case AccessTokenType.RemotePowerShellDelegated:
					this.token = new CommonAccessToken(AccessTokenType.RemotePowerShellDelegated);
					this.token.ExtensionData["DelegatedData"] = base.Definition.Attributes["DelegatedData"];
					return;
				}
				break;
			}
			this.ThrowTerminatingError("Unhandled AccessTokenType", "Unhandled AccessTokenType for RPSBackEndLogonProbe : " + accessTokenType.ToString(), new object[0]);
		}

		private Task<WebResponse> SendRPSRequestPackage(string message)
		{
			try
			{
				this.conversation.Add("Send : " + message);
				HttpWebRequest httpWebRequest = this.CreateRPSHttpWebRequest(this.url);
				using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
				{
					streamWriter.Write(message);
				}
				return Task.Factory.FromAsync<WebResponse>(new Func<AsyncCallback, object, IAsyncResult>(httpWebRequest.BeginGetResponse), new Func<IAsyncResult, WebResponse>(httpWebRequest.EndGetResponse), httpWebRequest);
			}
			catch (ApplicationException)
			{
				throw;
			}
			catch (Exception ex)
			{
				this.ThrowTerminatingError(ex.Message, "SendRPSRequestPackage failed. Error : {0}", new object[]
				{
					ex
				});
			}
			return null;
		}

		private string ProcessRPSResponsePackage(Task<WebResponse> task, RPSBackEndLogonProbe.ProcessRPSResponseBody processRPSResponseBodyDelegate)
		{
			try
			{
				if (task.IsFaulted)
				{
					this.HandleFailedRPSRequest(task);
				}
				else
				{
					HttpWebResponse httpWebResponse = (HttpWebResponse)task.Result;
					if (httpWebResponse.StatusCode == HttpStatusCode.OK)
					{
						this.UpdateRPSSessionCookie(httpWebResponse);
						string text = string.Empty;
						using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
						{
							text = streamReader.ReadToEnd();
						}
						this.conversation.Add(string.Format("Receive: {0}: {1}", httpWebResponse.StatusCode, text));
						return processRPSResponseBodyDelegate(httpWebResponse, text);
					}
					this.ReportErrorForFailedResponse(httpWebResponse);
				}
			}
			catch (ApplicationException)
			{
				throw;
			}
			catch (Exception ex)
			{
				this.ThrowTerminatingError(ex.Message, "Failed to process RPS response. Error : {0}", new object[]
				{
					ex
				});
			}
			return null;
		}

		private string ProcessCreateSessionResponse(Task<WebResponse> task)
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.RPSTracer, base.TraceContext, "Entering ProcessCreateSessionResponse", null, "ProcessCreateSessionResponse", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSBackEndLogonProbe.cs", 592);
			string result = this.ProcessRPSResponsePackage(task, delegate(HttpWebResponse response, string responseBody)
			{
				using (XmlNodeList xmlNodeList = this.ParseResponseStream(responseBody, "//w:Selector[@Name='ShellId']"))
				{
					if (xmlNodeList.Count < 1)
					{
						this.ThrowTerminatingError("No ShellID", "Cannot find ShellId from response : " + response, new object[0]);
					}
					this.sessionId = xmlNodeList[0].InnerText.Trim();
					WTFDiagnostics.TraceInformation(ExTraceGlobals.RPSTracer, base.TraceContext, "Get SessionId = " + this.sessionId + " " + base.Definition.Name, null, "ProcessCreateSessionResponse", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSBackEndLogonProbe.cs", 605);
				}
				return responseBody;
			});
			WTFDiagnostics.TraceFunction(ExTraceGlobals.RPSTracer, base.TraceContext, "Leaving ProcessCreateSessionResponse", null, "ProcessCreateSessionResponse", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSBackEndLogonProbe.cs", 610);
			return result;
		}

		private string ProcessReceiveSessionResponse(Task<WebResponse> task)
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.RPSTracer, base.TraceContext, "Entering ProcessReceiveSessionResponse", null, "ProcessReceiveSessionResponse", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSBackEndLogonProbe.cs", 621);
			string result = null;
			if (!this.sessionCreated)
			{
				result = this.ProcessRPSResponsePackage(task, delegate(HttpWebResponse response, string responseBody)
				{
					using (XmlNodeList xmlNodeList = this.ParseResponseStream(responseBody, "//rsp:Stream[@Name='stdout']"))
					{
						foreach (object obj in xmlNodeList)
						{
							XmlNode xmlNode = (XmlNode)obj;
							if (this.CheckRunspaceStateAreOpened(xmlNode.InnerText))
							{
								this.sessionCreated = true;
								WTFDiagnostics.TraceInformation(ExTraceGlobals.RPSTracer, base.TraceContext, "Session created. " + base.Definition.Name, null, "ProcessReceiveSessionResponse", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSBackEndLogonProbe.cs", 636);
								break;
							}
						}
					}
					return responseBody;
				});
			}
			WTFDiagnostics.TraceFunction(ExTraceGlobals.RPSTracer, base.TraceContext, "Leaving ProcessReceiveSessionResponse", null, "ProcessReceiveSessionResponse", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSBackEndLogonProbe.cs", 646);
			return result;
		}

		private string ProcessCommandResponse(Task<WebResponse> task)
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.RPSTracer, base.TraceContext, "Entering ProcessCommandResponse", null, "ProcessCommandResponse", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSBackEndLogonProbe.cs", 657);
			string result = this.ProcessRPSResponsePackage(task, delegate(HttpWebResponse response, string responseBody)
			{
				using (XmlNodeList xmlNodeList = this.ParseResponseStream(responseBody, "//rsp:CommandId"))
				{
					if (xmlNodeList.Count <= 0)
					{
						this.ThrowTerminatingError("No CommandId", "Cannot find CommandId : " + responseBody, new object[0]);
					}
					this.commandId = xmlNodeList[0].InnerText.Trim();
					WTFDiagnostics.TraceInformation(ExTraceGlobals.RPSTracer, base.TraceContext, "CommandId=" + this.commandId, null, "ProcessCommandResponse", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSBackEndLogonProbe.cs", 670);
				}
				return responseBody;
			});
			WTFDiagnostics.TraceFunction(ExTraceGlobals.RPSTracer, base.TraceContext, "Leaving ProcessCommandResponse", null, "ProcessCommandResponse", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSBackEndLogonProbe.cs", 674);
			return result;
		}

		private string ProcessReceiveCommandResponse(Task<WebResponse> task)
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.RPSTracer, base.TraceContext, "Entering ProcessReceiveCommandResponse", null, "ProcessReceiveCommandResponse", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSBackEndLogonProbe.cs", 685);
			string result = this.ProcessRPSResponsePackage(task, delegate(HttpWebResponse response, string responseBody)
			{
				using (XmlNodeList xmlNodeList = this.ParseResponseStream(responseBody, "//rsp:CommandState"))
				{
					if (xmlNodeList.Count <= 0)
					{
						this.ThrowTerminatingError("No CommandState", "Cannot find CommandState : " + responseBody, new object[0]);
					}
					if (xmlNodeList[0].Attributes["State"] == null || !xmlNodeList[0].Attributes["State"].Value.Equals("http://schemas.microsoft.com/wbem/wsman/1/windows/shell/CommandState/Done", StringComparison.OrdinalIgnoreCase))
					{
						this.ThrowTerminatingError("Unexpected CommandState", "Cannot find expected CommandState : " + responseBody, new object[0]);
					}
					WTFDiagnostics.TraceInformation(ExTraceGlobals.RPSTracer, base.TraceContext, "Command executed " + base.Definition.Name, null, "ProcessReceiveCommandResponse", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSBackEndLogonProbe.cs", 704);
				}
				return responseBody;
			});
			WTFDiagnostics.TraceFunction(ExTraceGlobals.RPSTracer, base.TraceContext, "Leaving ProcessReceiveCommandResponse", null, "ProcessReceiveCommandResponse", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSBackEndLogonProbe.cs", 708);
			return result;
		}

		private string ProcessDeleteSessionResponse(Task<WebResponse> task)
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.RPSTracer, base.TraceContext, "Entering ProcessDeleteSessionResponse", null, "ProcessDeleteSessionResponse", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSBackEndLogonProbe.cs", 719);
			string result = this.ProcessRPSResponsePackage(task, delegate(HttpWebResponse response, string responseBody)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.RPSTracer, base.TraceContext, "Session deleted" + base.Definition.Name, null, "ProcessDeleteSessionResponse", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSBackEndLogonProbe.cs", 724);
				return responseBody;
			});
			WTFDiagnostics.TraceFunction(ExTraceGlobals.RPSTracer, base.TraceContext, "Leaving ProcessDeleteSessionResponse", null, "ProcessDeleteSessionResponse", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSBackEndLogonProbe.cs", 727);
			return result;
		}

		private void ReportErrorForFailedResponse(HttpWebResponse response)
		{
			string text = string.Empty;
			using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
			{
				text = streamReader.ReadToEnd();
			}
			string text2 = response.Headers["X-RemotePS-FailureCategory"];
			if (!string.IsNullOrEmpty(text2))
			{
				base.Result.StateAttribute12 = text2;
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (!string.IsNullOrEmpty(text))
			{
				this.conversation.Add(string.Format("Receive : {0} : {1}", response.StatusCode, text));
				using (XmlNodeList xmlNodeList = this.ParseResponseStream(text, "//f:WSManFault/f:Message"))
				{
					foreach (object obj in xmlNodeList)
					{
						XmlNode xmlNode = (XmlNode)obj;
						stringBuilder.AppendLine(xmlNode.InnerXml);
					}
				}
			}
			if (stringBuilder.Length <= 0)
			{
				stringBuilder.AppendLine(text);
			}
			this.ThrowTerminatingError(stringBuilder.ToString(), "Received HttpStatusCode '{0}' : {1}. More information: {2}", new object[]
			{
				response.StatusCode,
				response.StatusDescription,
				stringBuilder
			});
		}

		private void UpdateRPSSessionCookie(HttpWebResponse response)
		{
			this.cookies.Add(response.Cookies);
			if (response.Headers.AllKeys.Contains("Set-Cookie"))
			{
				string input = response.Headers["Set-Cookie"];
				Match match = RPSBackEndLogonProbe.parseWSManCookieRegex.Match(input);
				if (match.Success)
				{
					string value = match.Groups["wsmanCookie"].Value;
					this.cookies.Add(new Uri(this.url), new Cookie("MS-WSMAN", value));
					this.url = base.Definition.Endpoint + "&sessionID=" + value;
				}
			}
		}

		private bool CheckRunspaceStateAreOpened(string rspStreamBody)
		{
			string text = new string((from x in Convert.FromBase64String(rspStreamBody)
			select (char)x).ToArray<char>());
			Match match = RPSBackEndLogonProbe.parseRunspaceStateRegex.Match(text);
			if (!match.Success)
			{
				return false;
			}
			string value = match.Groups["stateNumber"].Value;
			if (!value.Equals(2.ToString()))
			{
				this.ThrowTerminatingError("Invalid RunspaceState", "Runspace state is {0}, not 'Opened'(2) : {1}", new object[]
				{
					value,
					text
				});
			}
			return true;
		}

		private void HandleFailedRPSRequest(Task task)
		{
			try
			{
				WTFDiagnostics.TraceError<AggregateException>(ExTraceGlobals.RPSTracer, base.TraceContext, "HandleFailedRPSRequest : Task exception= {0}", task.Exception, null, "HandleFailedRPSRequest", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSBackEndLogonProbe.cs", 837);
				if (task.Exception != null)
				{
					WebException ex = task.Exception.Flatten().InnerException as WebException;
					if (ex != null && ex.Response != null && ex.Response is HttpWebResponse)
					{
						this.ReportErrorForFailedResponse((HttpWebResponse)ex.Response);
					}
				}
			}
			catch (ApplicationException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				this.ThrowTerminatingError(ex2.Message, "HandleFailedRPSRequest failed. Error: {0}", new object[]
				{
					ex2
				});
			}
		}

		private void ThrowTerminatingError(string fcInfo, string errorMessageFormat, params object[] parameters)
		{
			string text = string.Format(errorMessageFormat, parameters);
			if (string.IsNullOrEmpty(base.Result.StateAttribute12))
			{
				base.Result.StateAttribute12 = fcInfo;
			}
			text = text + Environment.NewLine + this.GetRPSBackEndLogonProbeInformation();
			WTFDiagnostics.TraceError(ExTraceGlobals.RPSTracer, base.TraceContext, text, null, "ThrowTerminatingError", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSBackEndLogonProbe.cs", 871);
			throw new ApplicationException(text);
		}

		private string GetRPSBackEndLogonProbeInformation()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("Probe information : Account={0}, AccessTokenType = {1}, Url = {2} ", base.Definition.Account, this.GetDefinitionAttributeValue("AccessTokenType"), base.Definition.Endpoint);
			stringBuilder.AppendFormat("MemberName = {0}, Puid = {1}, UserSid = {2}, PartitionId = {3}, DelegatedData = {4}", new object[]
			{
				this.GetDefinitionAttributeValue("MemberName"),
				this.GetDefinitionAttributeValue("Puid"),
				this.GetDefinitionAttributeValue("UserSid"),
				this.GetDefinitionAttributeValue("Partition"),
				this.GetDefinitionAttributeValue("DelegatedData")
			});
			stringBuilder.AppendLine();
			return stringBuilder.ToString();
		}

		private string GetDefinitionAttributeValue(string key)
		{
			if (base.Definition.Attributes.ContainsKey(key))
			{
				return base.Definition.Attributes[key];
			}
			return null;
		}

		public const string AccessTokenTypeParameterName = "AccessTokenType";

		public const string AllowRedirectionParameterName = "AllowRedirection";

		public const string WLIDMemberNameParameterName = "X-WLID-MemberName";

		internal const string CreateSessionPackage = "<s:Envelope xmlns:s=\"http://www.w3.org/2003/05/soap-envelope\" xmlns:a=\"http://schemas.xmlsoap.org/ws/2004/08/addressing\" xmlns:w=\"http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd\" xmlns:p=\"http://schemas.microsoft.com/wbem/wsman/1/wsman.xsd\"><s:Header><a:To>{0}</a:To><w:ResourceURI s:mustUnderstand=\"true\">http://schemas.microsoft.com/powershell/Microsoft.Exchange</w:ResourceURI><a:ReplyTo><a:Address s:mustUnderstand=\"true\">http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous</a:Address></a:ReplyTo><a:Action s:mustUnderstand=\"true\">http://schemas.xmlsoap.org/ws/2004/09/transfer/Create</a:Action><w:MaxEnvelopeSize s:mustUnderstand=\"true\">153600</w:MaxEnvelopeSize><a:MessageID>uuid:D9C7EA80-68E8-4140-8678-D1979D2518E8</a:MessageID><w:Locale xml:lang=\"en-US\" s:mustUnderstand=\"false\" /><p:DataLocale xml:lang=\"en-US\" s:mustUnderstand=\"false\" /><w:OptionSet xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" s:mustUnderstand=\"true\"><w:Option Name=\"protocolversion\" MustComply=\"true\">2.1</w:Option></w:OptionSet><w:OperationTimeout>PT180.000S</w:OperationTimeout><rsp:CompressionType s:mustUnderstand=\"true\" xmlns:rsp=\"http://schemas.microsoft.com/wbem/wsman/1/windows/shell\">xpress</rsp:CompressionType></s:Header><s:Body><rsp:Shell xmlns:rsp=\"http://schemas.microsoft.com/wbem/wsman/1/windows/shell\"><rsp:IdleTimeOut>PT240.000S</rsp:IdleTimeOut><rsp:InputStreams>stdin pr</rsp:InputStreams><rsp:OutputStreams>stdout</rsp:OutputStreams><creationXml xmlns=\"http://schemas.microsoft.com/powershell\">AAAAAAAAAAcAAAAAAAAAAAMAAALwAgAAAAIAAQBDBF6bTpfeRr6/0+HmQDgeAAAAAAAAAAAAAAAAAAAAAO+7vzxPYmogUmVmSWQ9IjAiPjxNUz48VmVyc2lvbiBOPSJwcm90b2NvbHZlcnNpb24iPjIuMTwvVmVyc2lvbj48VmVyc2lvbiBOPSJQU1ZlcnNpb24iPjIuMDwvVmVyc2lvbj48VmVyc2lvbiBOPSJTZXJpYWxpemF0aW9uVmVyc2lvbiI+MS4xLjAuMTwvVmVyc2lvbj48QkEgTj0iVGltZVpvbmUiPkFBRUFBQUQvLy8vL0FRQUFBQUFBQUFBRUFRQUFBQnhUZVhOMFpXMHVRM1Z5Y21WdWRGTjVjM1JsYlZScGJXVmFiMjVsQkFBQUFCZHRYME5oWTJobFpFUmhlV3hwWjJoMFEyaGhibWRsY3cxdFgzUnBZMnR6VDJabWMyVjBEbTFmYzNSaGJtUmhjbVJPWVcxbERtMWZaR0Y1YkdsbmFIUk9ZVzFsQXdBQkFSeFRlWE4wWlcwdVEyOXNiR1ZqZEdsdmJuTXVTR0Z6YUhSaFlteGxDUWtDQUFBQUFNRGM4YnovLy84S0NnUUNBQUFBSEZONWMzUmxiUzVEYjJ4c1pXTjBhVzl1Y3k1SVlYTm9kR0ZpYkdVSEFBQUFDa3h2WVdSR1lXTjBiM0lIVm1WeWMybHZiZ2hEYjIxd1lYSmxjaEJJWVhOb1EyOWtaVkJ5YjNacFpHVnlDRWhoYzJoVGFYcGxCRXRsZVhNR1ZtRnNkV1Z6QUFBREF3QUZCUXNJSEZONWMzUmxiUzVEYjJ4c1pXTjBhVzl1Y3k1SlEyOXRjR0Z5WlhJa1UzbHpkR1Z0TGtOdmJHeGxZM1JwYjI1ekxrbElZWE5vUTI5a1pWQnliM1pwWkdWeUNPeFJPRDhBQUFBQUNnb0RBQUFBQ1FNQUFBQUpCQUFBQUJBREFBQUFBQUFBQUJBRUFBQUFBQUFBQUFzPTwvQkE+PC9NUz48L09iaj4AAAAAAAAACAAAAAAAAAAAAwAADeYCAAAABAABAEMEXptOl95Gvr/T4eZAOB4AAAAAAAAAAAAAAAAAAAAA77u/PE9iaiBSZWZJZD0iMCI+PE1TPjxJMzIgTj0iTWluUnVuc3BhY2VzIj4xPC9JMzI+PEkzMiBOPSJNYXhSdW5zcGFjZXMiPjE8L0kzMj48T2JqIE49IlBTVGhyZWFkT3B0aW9ucyIgUmVmSWQ9IjEiPjxUTiBSZWZJZD0iMCI+PFQ+U3lzdGVtLk1hbmFnZW1lbnQuQXV0b21hdGlvbi5SdW5zcGFjZXMuUFNUaHJlYWRPcHRpb25zPC9UPjxUPlN5c3RlbS5FbnVtPC9UPjxUPlN5c3RlbS5WYWx1ZVR5cGU8L1Q+PFQ+U3lzdGVtLk9iamVjdDwvVD48L1ROPjxUb1N0cmluZz5EZWZhdWx0PC9Ub1N0cmluZz48STMyPjA8L0kzMj48L09iaj48T2JqIE49IkFwYXJ0bWVudFN0YXRlIiBSZWZJZD0iMiI+PFROIFJlZklkPSIxIj48VD5TeXN0ZW0uVGhyZWFkaW5nLkFwYXJ0bWVudFN0YXRlPC9UPjxUPlN5c3RlbS5FbnVtPC9UPjxUPlN5c3RlbS5WYWx1ZVR5cGU8L1Q+PFQ+U3lzdGVtLk9iamVjdDwvVD48L1ROPjxUb1N0cmluZz5Vbmtub3duPC9Ub1N0cmluZz48STMyPjI8L0kzMj48L09iaj48T2JqIE49IkFwcGxpY2F0aW9uQXJndW1lbnRzIiBSZWZJZD0iMyI+PFROIFJlZklkPSIyIj48VD5TeXN0ZW0uTWFuYWdlbWVudC5BdXRvbWF0aW9uLlBTUHJpbWl0aXZlRGljdGlvbmFyeTwvVD48VD5TeXN0ZW0uQ29sbGVjdGlvbnMuSGFzaHRhYmxlPC9UPjxUPlN5c3RlbS5PYmplY3Q8L1Q+PC9UTj48RENUPjxFbj48UyBOPSJLZXkiPlBTVmVyc2lvblRhYmxlPC9TPjxPYmogTj0iVmFsdWUiIFJlZklkPSI0Ij48VE5SZWYgUmVmSWQ9IjIiIC8+PERDVD48RW4+PFMgTj0iS2V5Ij5XU01hblN0YWNrVmVyc2lvbjwvUz48VmVyc2lvbiBOPSJWYWx1ZSI+Mi4wPC9WZXJzaW9uPjwvRW4+PEVuPjxTIE49IktleSI+UFNDb21wYXRpYmxlVmVyc2lvbnM8L1M+PE9iaiBOPSJWYWx1ZSIgUmVmSWQ9IjUiPjxUTiBSZWZJZD0iMyI+PFQ+U3lzdGVtLlZlcnNpb25bXTwvVD48VD5TeXN0ZW0uQXJyYXk8L1Q+PFQ+U3lzdGVtLk9iamVjdDwvVD48L1ROPjxMU1Q+PFZlcnNpb24+MS4wPC9WZXJzaW9uPjxWZXJzaW9uPjIuMDwvVmVyc2lvbj48L0xTVD48L09iaj48L0VuPjxFbj48UyBOPSJLZXkiPkJ1aWxkVmVyc2lvbjwvUz48VmVyc2lvbiBOPSJWYWx1ZSI+Ni4xLjc2MDEuMTc1MTQ8L1ZlcnNpb24+PC9Fbj48RW4+PFMgTj0iS2V5Ij5QU1JlbW90aW5nUHJvdG9jb2xWZXJzaW9uPC9TPjxWZXJzaW9uIE49IlZhbHVlIj4yLjE8L1ZlcnNpb24+PC9Fbj48RW4+PFMgTj0iS2V5Ij5QU1ZlcnNpb248L1M+PFZlcnNpb24gTj0iVmFsdWUiPjIuMDwvVmVyc2lvbj48L0VuPjxFbj48UyBOPSJLZXkiPkNMUlZlcnNpb248L1M+PFZlcnNpb24gTj0iVmFsdWUiPjQuMC4zMDMxOS4xPC9WZXJzaW9uPjwvRW4+PEVuPjxTIE49IktleSI+U2VyaWFsaXphdGlvblZlcnNpb248L1M+PFZlcnNpb24gTj0iVmFsdWUiPjEuMS4wLjE8L1ZlcnNpb24+PC9Fbj48L0RDVD48L09iaj48L0VuPjwvRENUPjwvT2JqPjxPYmogTj0iSG9zdEluZm8iIFJlZklkPSI2Ij48TVM+PE9iaiBOPSJfaG9zdERlZmF1bHREYXRhIiBSZWZJZD0iNyI+PE1TPjxPYmogTj0iZGF0YSIgUmVmSWQ9IjgiPjxUTiBSZWZJZD0iNCI+PFQ+U3lzdGVtLkNvbGxlY3Rpb25zLkhhc2h0YWJsZTwvVD48VD5TeXN0ZW0uT2JqZWN0PC9UPjwvVE4+PERDVD48RW4+PEkzMiBOPSJLZXkiPjk8L0kzMj48T2JqIE49IlZhbHVlIiBSZWZJZD0iOSI+PE1TPjxTIE49IlQiPlN5c3RlbS5TdHJpbmc8L1M+PFMgTj0iViI+QWRtaW5pc3RyYXRvcjogV2luZG93cyBQb3dlclNoZWxsPC9TPjwvTVM+PC9PYmo+PC9Fbj48RW4+PEkzMiBOPSJLZXkiPjg8L0kzMj48T2JqIE49IlZhbHVlIiBSZWZJZD0iMTAiPjxNUz48UyBOPSJUIj5TeXN0ZW0uTWFuYWdlbWVudC5BdXRvbWF0aW9uLkhvc3QuU2l6ZTwvUz48T2JqIE49IlYiIFJlZklkPSIxMSI+PE1TPjxJMzIgTj0id2lkdGgiPjE1OTwvSTMyPjxJMzIgTj0iaGVpZ2h0Ij43NDwvSTMyPjwvTVM+PC9PYmo+PC9NUz48L09iaj48L0VuPjxFbj48STMyIE49IktleSI+NzwvSTMyPjxPYmogTj0iVmFsdWUiIFJlZklkPSIxMiI+PE1TPjxTIE49IlQiPlN5c3RlbS5NYW5hZ2VtZW50LkF1dG9tYXRpb24uSG9zdC5TaXplPC9TPjxPYmogTj0iViIgUmVmSWQ9IjEzIj48TVM+PEkzMiBOPSJ3aWR0aCI+MTIwPC9JMzI+PEkzMiBOPSJoZWlnaHQiPjc0PC9JMzI+PC9NUz48L09iaj48L01TPjwvT2JqPjwvRW4+PEVuPjxJMzIgTj0iS2V5Ij42PC9JMzI+PE9iaiBOPSJWYWx1ZSIgUmVmSWQ9IjE0Ij48TVM+PFMgTj0iVCI+U3lzdGVtLk1hbmFnZW1lbnQuQXV0b21hdGlvbi5Ib3N0LlNpemU8L1M+PE9iaiBOPSJWIiBSZWZJZD0iMTUiPjxNUz48STMyIE49IndpZHRoIj4xMjA8L0kzMj48STMyIE49ImhlaWdodCI+NTA8L0kzMj48L01TPjwvT2JqPjwvTVM+PC9PYmo+PC9Fbj48RW4+PEkzMiBOPSJLZXkiPjU8L0kzMj48T2JqIE49IlZhbHVlIiBSZWZJZD0iMTYiPjxNUz48UyBOPSJUIj5TeXN0ZW0uTWFuYWdlbWVudC5BdXRvbWF0aW9uLkhvc3QuU2l6ZTwvUz48T2JqIE49IlYiIFJlZklkPSIxNyI+PE1TPjxJMzIgTj0id2lkdGgiPjEyMDwvSTMyPjxJMzIgTj0iaGVpZ2h0Ij4zMDAwPC9JMzI+PC9NUz48L09iaj48L01TPjwvT2JqPjwvRW4+PEVuPjxJMzIgTj0iS2V5Ij40PC9JMzI+PE9iaiBOPSJWYWx1ZSIgUmVmSWQ9IjE4Ij48TVM+PFMgTj0iVCI+U3lzdGVtLkludDMyPC9TPjxJMzIgTj0iViI+MjU8L0kzMj48L01TPjwvT2JqPjwvRW4+PEVuPjxJMzIgTj0iS2V5Ij4zPC9JMzI+PE9iaiBOPSJWYWx1ZSIgUmVmSWQ9IjE5Ij48TVM+PFMgTj0iVCI+U3lzdGVtLk1hbmFnZW1lbnQuQXV0b21hdGlvbi5Ib3N0LkNvb3JkaW5hdGVzPC9TPjxPYmogTj0iViIgUmVmSWQ9IjIwIj48TVM+PEkzMiBOPSJ4Ij4wPC9JMzI+PEkzMiBOPSJ5Ij42PC9JMzI+PC9NUz48L09iaj48L01TPjwvT2JqPjwvRW4+PEVuPjxJMzIgTj0iS2V5Ij4yPC9JMzI+PE9iaiBOPSJWYWx1ZSIgUmVmSWQ9IjIxIj48TVM+PFMgTj0iVCI+U3lzdGVtLk1hbmFnZW1lbnQuQXV0b21hdGlvbi5Ib3N0LkNvb3JkaW5hdGVzPC9TPjxPYmogTj0iViIgUmVmSWQ9IjIyIj48TVM+PEkzMiBOPSJ4Ij4wPC9JMzI+PEkzMiBOPSJ5Ij41NTwvSTMyPjwvTVM+PC9PYmo+PC9NUz48L09iaj48L0VuPjxFbj48STMyIE49IktleSI+MTwvSTMyPjxPYmogTj0iVmFsdWUiIFJlZklkPSIyMyI+PE1TPjxTIE49IlQiPlN5c3RlbS5Db25zb2xlQ29sb3I8L1M+PEkzMiBOPSJWIj41PC9JMzI+PC9NUz48L09iaj48L0VuPjxFbj48STMyIE49IktleSI+MDwvSTMyPjxPYmogTj0iVmFsdWUiIFJlZklkPSIyNCI+PE1TPjxTIE49IlQiPlN5c3RlbS5Db25zb2xlQ29sb3I8L1M+PEkzMiBOPSJWIj42PC9JMzI+PC9NUz48L09iaj48L0VuPjwvRENUPjwvT2JqPjwvTVM+PC9PYmo+PEIgTj0iX2lzSG9zdE51bGwiPmZhbHNlPC9CPjxCIE49Il9pc0hvc3RVSU51bGwiPmZhbHNlPC9CPjxCIE49Il9pc0hvc3RSYXdVSU51bGwiPmZhbHNlPC9CPjxCIE49Il91c2VSdW5zcGFjZUhvc3QiPmZhbHNlPC9CPjwvTVM+PC9PYmo+PC9NUz48L09iaj4=</creationXml></rsp:Shell></s:Body></s:Envelope>";

		internal const string ReceiveSessionPackage = "<s:Envelope xmlns:s=\"http://www.w3.org/2003/05/soap-envelope\" xmlns:a=\"http://schemas.xmlsoap.org/ws/2004/08/addressing\" xmlns:w=\"http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd\" xmlns:p=\"http://schemas.microsoft.com/wbem/wsman/1/wsman.xsd\"><s:Header><a:To>{0}</a:To><w:ResourceURI s:mustUnderstand=\"true\">http://schemas.microsoft.com/powershell/Microsoft.Exchange</w:ResourceURI><a:ReplyTo><a:Address s:mustUnderstand=\"true\">http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous</a:Address></a:ReplyTo><a:Action s:mustUnderstand=\"true\">http://schemas.microsoft.com/wbem/wsman/1/windows/shell/Receive</a:Action><w:MaxEnvelopeSize s:mustUnderstand=\"true\">153600</w:MaxEnvelopeSize><a:MessageID>uuid:0B74A582-89AA-486E-9DEF-2F38707F75D2</a:MessageID><w:Locale xml:lang=\"en-US\" s:mustUnderstand=\"false\" /><p:DataLocale xml:lang=\"en-US\" s:mustUnderstand=\"false\" /><w:SelectorSet><w:Selector Name=\"ShellId\">{1}</w:Selector></w:SelectorSet><w:OptionSet xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"><w:Option Name=\"WSMAN_CMDSHELL_OPTION_KEEPALIVE\">TRUE</w:Option></w:OptionSet><w:OperationTimeout>PT180.000S</w:OperationTimeout></s:Header><s:Body><rsp:Receive xmlns:rsp=\"http://schemas.microsoft.com/wbem/wsman/1/windows/shell\"  SequenceId=\"0\"><rsp:DesiredStream>stdout</rsp:DesiredStream></rsp:Receive></s:Body></s:Envelope>";

		internal const string CommandPackage = "<s:Envelope xmlns:s=\"http://www.w3.org/2003/05/soap-envelope\" xmlns:a=\"http://schemas.xmlsoap.org/ws/2004/08/addressing\" xmlns:w=\"http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd\" xmlns:p=\"http://schemas.microsoft.com/wbem/wsman/1/wsman.xsd\"><s:Header><a:To>{0}</a:To><a:ReplyTo><a:Address s:mustUnderstand=\"true\">http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous</a:Address></a:ReplyTo><a:Action s:mustUnderstand=\"true\">http://schemas.microsoft.com/wbem/wsman/1/windows/shell/Command</a:Action><w:MaxEnvelopeSize s:mustUnderstand=\"true\">153600</w:MaxEnvelopeSize><a:MessageID>uuid:777E12FC-B65F-4AC5-917F-A38B7EA6622A</a:MessageID><w:Locale xml:lang=\"en-US\" s:mustUnderstand=\"false\" /><p:DataLocale xml:lang=\"en-US\" s:mustUnderstand=\"false\" /><w:ResourceURI xmlns:w=\"http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd\">http://schemas.microsoft.com/powershell/Microsoft.Exchange</w:ResourceURI><w:SelectorSet xmlns:w=\"http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd\" xmlns=\"http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd\"><w:Selector Name=\"ShellId\">{1}</w:Selector></w:SelectorSet><w:OperationTimeout>PT180.000S</w:OperationTimeout></s:Header><s:Body><rsp:CommandLine xmlns:rsp=\"http://schemas.microsoft.com/wbem/wsman/1/windows/shell\"><rsp:Command> </rsp:Command><rsp:Arguments>AAAAAAAAAAkAAAAAAAAAAAMAAAbVAgAAAAYQAgBDBF6bTpfeRr6/0+HmQDgehsZH1YtzIUaYJjd8M5MEs++7vzxPYmogUmVmSWQ9IjAiPjxNUz48T2JqIE49IlBvd2VyU2hlbGwiIFJlZklkPSIxIj48TVM+PE9iaiBOPSJDbWRzIiBSZWZJZD0iMiI+PFROIFJlZklkPSIwIj48VD5TeXN0ZW0uQ29sbGVjdGlvbnMuR2VuZXJpYy5MaXN0YDFbW1N5c3RlbS5NYW5hZ2VtZW50LkF1dG9tYXRpb24uUFNPYmplY3QsIFN5c3RlbS5NYW5hZ2VtZW50LkF1dG9tYXRpb24sIFZlcnNpb249MS4wLjAuMCwgQ3VsdHVyZT1uZXV0cmFsLCBQdWJsaWNLZXlUb2tlbj0zMWJmMzg1NmFkMzY0ZTM1XV08L1Q+PFQ+U3lzdGVtLk9iamVjdDwvVD48L1ROPjxMU1Q+PE9iaiBSZWZJZD0iMyI+PE1TPjxTIE49IkNtZCI+Z2V0LW1haWxib3g8L1M+PEIgTj0iSXNTY3JpcHQiPmZhbHNlPC9CPjxOaWwgTj0iVXNlTG9jYWxTY29wZSIgLz48T2JqIE49Ik1lcmdlTXlSZXN1bHQiIFJlZklkPSI0Ij48VE4gUmVmSWQ9IjEiPjxUPlN5c3RlbS5NYW5hZ2VtZW50LkF1dG9tYXRpb24uUnVuc3BhY2VzLlBpcGVsaW5lUmVzdWx0VHlwZXM8L1Q+PFQ+U3lzdGVtLkVudW08L1Q+PFQ+U3lzdGVtLlZhbHVlVHlwZTwvVD48VD5TeXN0ZW0uT2JqZWN0PC9UPjwvVE4+PFRvU3RyaW5nPk5vbmU8L1RvU3RyaW5nPjxJMzI+MDwvSTMyPjwvT2JqPjxPYmogTj0iTWVyZ2VUb1Jlc3VsdCIgUmVmSWQ9IjUiPjxUTlJlZiBSZWZJZD0iMSIgLz48VG9TdHJpbmc+Tm9uZTwvVG9TdHJpbmc+PEkzMj4wPC9JMzI+PC9PYmo+PE9iaiBOPSJNZXJnZVByZXZpb3VzUmVzdWx0cyIgUmVmSWQ9IjYiPjxUTlJlZiBSZWZJZD0iMSIgLz48VG9TdHJpbmc+Tm9uZTwvVG9TdHJpbmc+PEkzMj4wPC9JMzI+PC9PYmo+PE9iaiBOPSJBcmdzIiBSZWZJZD0iNyI+PFROUmVmIFJlZklkPSIwIiAvPjxMU1Q+PE9iaiBSZWZJZD0iOCI+PE1TPjxTIE49Ik4iPi1yZXN1bHRzaXplOjwvUz48STMyIE49IlYiPjE8L0kzMj48L01TPjwvT2JqPjwvTFNUPjwvT2JqPjwvTVM+PC9PYmo+PC9MU1Q+PC9PYmo+PEIgTj0iSXNOZXN0ZWQiPmZhbHNlPC9CPjxOaWwgTj0iSGlzdG9yeSIgLz48QiBOPSJSZWRpcmVjdFNoZWxsRXJyb3JPdXRwdXRQaXBlIj50cnVlPC9CPjwvTVM+PC9PYmo+PEIgTj0iTm9JbnB1dCI+dHJ1ZTwvQj48T2JqIE49IkFwYXJ0bWVudFN0YXRlIiBSZWZJZD0iOSI+PFROIFJlZklkPSIyIj48VD5TeXN0ZW0uVGhyZWFkaW5nLkFwYXJ0bWVudFN0YXRlPC9UPjxUPlN5c3RlbS5FbnVtPC9UPjxUPlN5c3RlbS5WYWx1ZVR5cGU8L1Q+PFQ+U3lzdGVtLk9iamVjdDwvVD48L1ROPjxUb1N0cmluZz5Vbmtub3duPC9Ub1N0cmluZz48STMyPjI8L0kzMj48L09iaj48T2JqIE49IlJlbW90ZVN0cmVhbU9wdGlvbnMiIFJlZklkPSIxMCI+PFROIFJlZklkPSIzIj48VD5TeXN0ZW0uTWFuYWdlbWVudC5BdXRvbWF0aW9uLlJlbW90ZVN0cmVhbU9wdGlvbnM8L1Q+PFQ+U3lzdGVtLkVudW08L1Q+PFQ+U3lzdGVtLlZhbHVlVHlwZTwvVD48VD5TeXN0ZW0uT2JqZWN0PC9UPjwvVE4+PFRvU3RyaW5nPjA8L1RvU3RyaW5nPjxJMzI+MDwvSTMyPjwvT2JqPjxCIE49IkFkZFRvSGlzdG9yeSI+dHJ1ZTwvQj48T2JqIE49Ikhvc3RJbmZvIiBSZWZJZD0iMTEiPjxNUz48QiBOPSJfaXNIb3N0TnVsbCI+dHJ1ZTwvQj48QiBOPSJfaXNIb3N0VUlOdWxsIj50cnVlPC9CPjxCIE49Il9pc0hvc3RSYXdVSU51bGwiPnRydWU8L0I+PEIgTj0iX3VzZVJ1bnNwYWNlSG9zdCI+dHJ1ZTwvQj48L01TPjwvT2JqPjwvTVM+PC9PYmo+</rsp:Arguments></rsp:CommandLine></s:Body></s:Envelope>";

		internal const string ReceiveCommandPackage = "<s:Envelope xmlns:s=\"http://www.w3.org/2003/05/soap-envelope\" xmlns:a=\"http://schemas.xmlsoap.org/ws/2004/08/addressing\" xmlns:w=\"http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd\" xmlns:p=\"http://schemas.microsoft.com/wbem/wsman/1/wsman.xsd\"><s:Header><a:To>{0}</a:To><a:ReplyTo><a:Address s:mustUnderstand=\"true\">http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous</a:Address></a:ReplyTo><a:Action s:mustUnderstand=\"true\">http://schemas.microsoft.com/wbem/wsman/1/windows/shell/Receive</a:Action><w:MaxEnvelopeSize s:mustUnderstand=\"true\">153600</w:MaxEnvelopeSize><a:MessageID>uuid:AB4B9CB2-8CA3-43A6-B505-EAC948EFEA9A</a:MessageID><w:Locale xml:lang=\"en-US\" s:mustUnderstand=\"false\" /><p:DataLocale xml:lang=\"en-US\" s:mustUnderstand=\"false\" /><w:ResourceURI xmlns:w=\"http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd\">http://schemas.microsoft.com/powershell/Microsoft.Exchange</w:ResourceURI><w:SelectorSet xmlns:w=\"http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd\" xmlns=\"http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd\"><w:Selector Name=\"ShellId\">{1}</w:Selector></w:SelectorSet><w:OperationTimeout>PT180.000S</w:OperationTimeout></s:Header><s:Body><rsp:Receive xmlns:rsp=\"http://schemas.microsoft.com/wbem/wsman/1/windows/shell\"  SequenceId=\"0\"><rsp:DesiredStream CommandId=\"{2}\">stdout</rsp:DesiredStream></rsp:Receive></s:Body></s:Envelope>";

		internal const string DeleteSessionPackage = "<s:Envelope xmlns:s=\"http://www.w3.org/2003/05/soap-envelope\" xmlns:a=\"http://schemas.xmlsoap.org/ws/2004/08/addressing\" xmlns:w=\"http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd\" xmlns:p=\"http://schemas.microsoft.com/wbem/wsman/1/wsman.xsd\"><s:Header><a:To>{0}</a:To><a:ReplyTo><a:Address s:mustUnderstand=\"true\">http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous</a:Address></a:ReplyTo><a:Action s:mustUnderstand=\"true\">http://schemas.xmlsoap.org/ws/2004/09/transfer/Delete</a:Action><w:MaxEnvelopeSize s:mustUnderstand=\"true\">153600</w:MaxEnvelopeSize><a:MessageID>uuid:9C7D31C9-1389-4D4A-8103-C1A0E5433950</a:MessageID><w:Locale xml:lang=\"en-US\" s:mustUnderstand=\"false\" /><p:DataLocale xml:lang=\"en-US\" s:mustUnderstand=\"false\" /><w:ResourceURI xmlns:w=\"http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd\">http://schemas.microsoft.com/powershell/Microsoft.Exchange</w:ResourceURI><w:SelectorSet xmlns:w=\"http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd\" xmlns=\"http://schemas.dmtf.org/wbem/wsman/1/wsman.xsd\"><w:Selector Name=\"ShellId\">{1}</w:Selector></w:SelectorSet><w:OperationTimeout>PT60.000S</w:OperationTimeout></s:Header><s:Body></s:Body></s:Envelope>";

		internal const string ComponentId = "RPSBackEndLogonProbe";

		private static Regex parseWSManCookieRegex = new Regex("MS-WSMAN=(?<wsmanCookie>[^;]+);");

		private static Regex parseRunspaceStateRegex = new Regex("\\<I32 N=\"RunspaceState\"\\>(?<stateNumber>[^\\<]+)\\</I32\\>");

		private CookieContainer cookies;

		private XmlNamespaceManager xmlnsmgr;

		private XmlDocument xmlDocument;

		private CommonAccessToken token;

		private bool allowRedirect;

		private string url;

		private string sessionId;

		private string commandId;

		private bool sessionCreated;

		private bool tokenInitialized;

		private List<string> conversation;

		internal delegate string ProcessRPSResponseBody(HttpWebResponse response, string responseBody);
	}
}
