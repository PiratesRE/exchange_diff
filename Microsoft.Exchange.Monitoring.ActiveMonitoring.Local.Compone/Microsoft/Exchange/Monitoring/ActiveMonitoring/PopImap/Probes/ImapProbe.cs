using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.PopImap.Probes
{
	public abstract class ImapProbe : TcpProbe
	{
		protected override Trace Tracer
		{
			get
			{
				return ExTraceGlobals.IMAP4Tracer;
			}
		}

		protected override string ProbeComponent
		{
			get
			{
				return "IMAP4";
			}
		}

		protected override void TestProtocol()
		{
			DateTime utcNow = DateTime.UtcNow;
			ProbeResult result = base.Result;
			result.StateAttribute21 += "BTPS;";
			Imap4Connection imap4Connection = base.TcpCon as Imap4Connection;
			WTFDiagnostics.TraceInformation(ExTraceGlobals.IMAP4Tracer, base.TraceContext, "Sending LOGIN Command.", null, "TestProtocol", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PopImap\\Imap4\\ImapProbe.cs", 82);
			string request = string.Format("z login {0} {1}", base.UserName, base.Password);
			ProbeResult result2 = base.Result;
			result2.StateAttribute13 += ":L";
			TcpResponse tcpResponse = imap4Connection.SendRequest(request, "z");
			if (tcpResponse.ResponseType != ResponseType.success && base.IsLocalProbe)
			{
				ProbeResult result3 = base.Result;
				result3.StateAttribute13 += ":L";
				tcpResponse = imap4Connection.SendRequest(request, "z");
			}
			base.VerifyResponse(tcpResponse, string.Format("Could not login to user: {0} with IMAP on port: {1} {2} SSL encryption", base.UserName, base.EndPoint.Port, base.IsSecure ? "with" : "without"));
			ProbeResult result4 = base.Result;
			object stateAttribute = result4.StateAttribute21;
			result4.StateAttribute21 = string.Concat(new object[]
			{
				stateAttribute,
				"BTPE:",
				(int)(DateTime.UtcNow - utcNow).TotalMilliseconds,
				";"
			});
		}

		protected override void DetermineCapability()
		{
			DateTime utcNow = DateTime.UtcNow;
			ProbeResult result = base.Result;
			result.StateAttribute21 += "BDCS;";
			WTFDiagnostics.TraceInformation(ExTraceGlobals.IMAP4Tracer, base.TraceContext, "Determining Login Capability.", null, "DetermineCapability", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PopImap\\Imap4\\ImapProbe.cs", 111);
			Imap4Connection imap4Connection = base.TcpCon as Imap4Connection;
			TcpResponse tcpResponse = imap4Connection.SendRequest(string.Format("{0} capability", "z"), "z");
			if (tcpResponse.ResponseType != ResponseType.success)
			{
				string message = string.Format("Unexpected Server Response while Determining Login Capability: \"{0}\"", tcpResponse.ResponseString);
				WTFDiagnostics.TraceError(ExTraceGlobals.IMAP4Tracer, base.TraceContext, tcpResponse.ResponseString, null, "DetermineCapability", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PopImap\\Imap4\\ImapProbe.cs", 120);
				ProbeResult result2 = base.Result;
				object stateAttribute = result2.StateAttribute21;
				result2.StateAttribute21 = string.Concat(new object[]
				{
					stateAttribute,
					"BDCE:",
					(int)(DateTime.UtcNow - utcNow).TotalMilliseconds,
					";"
				});
				throw new InvalidOperationException(message);
			}
			ProbeResult result3 = base.Result;
			object stateAttribute2 = result3.StateAttribute21;
			result3.StateAttribute21 = string.Concat(new object[]
			{
				stateAttribute2,
				"BDCE:",
				(int)(DateTime.UtcNow - utcNow).TotalMilliseconds,
				";"
			});
		}

		protected const string ExpectedTag = "z";

		private const string LoginFormat = "z login {0} {1}";

		private const int ImapUnsecuredPort = 143;
	}
}
