using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.PopImap.Probes
{
	public abstract class PopProbe : TcpProbe
	{
		protected override Trace Tracer
		{
			get
			{
				return ExTraceGlobals.POP3Tracer;
			}
		}

		protected override string ProbeComponent
		{
			get
			{
				return "POP3";
			}
		}

		protected override void TestProtocol()
		{
			DateTime utcNow = DateTime.UtcNow;
			ProbeResult result = base.Result;
			result.StateAttribute21 += "BTPS;";
			Pop3Connection pop3Connection = base.TcpCon as Pop3Connection;
			string request = string.Format("user {0}", base.UserName);
			string request2 = string.Format("pass {0}", base.Password);
			WTFDiagnostics.TraceInformation(ExTraceGlobals.POP3Tracer, base.TraceContext, "Sending USER Command.", null, "TestProtocol", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PopImap\\Pop3\\PopProbe.cs", 85);
			ProbeResult result2 = base.Result;
			result2.StateAttribute13 += ":U";
			base.VerifyResponse(pop3Connection.SendRequest(request, false), string.Format("Unexpected response to the POP USER command for user: {0} on port: {1} {2} SSL encryption", base.UserName, base.EndPoint.Port, base.IsSecure ? "with" : "without"));
			WTFDiagnostics.TraceInformation(ExTraceGlobals.POP3Tracer, base.TraceContext, "Sending PASS Command.", null, "TestProtocol", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PopImap\\Pop3\\PopProbe.cs", 93);
			ProbeResult result3 = base.Result;
			result3.StateAttribute13 += ":P";
			TcpResponse tcpResponse = pop3Connection.SendRequest(request2, false);
			if (tcpResponse.ResponseType != ResponseType.success && base.IsLocalProbe)
			{
				ProbeResult result4 = base.Result;
				result4.StateAttribute13 += ":U";
				base.VerifyResponse(pop3Connection.SendRequest(request, false), string.Format("Unexpected response to the POP USER command for user: {0} on port: {1} {2} SSL encryption", base.UserName, base.EndPoint.Port, base.IsSecure ? "with" : "without"));
				ProbeResult result5 = base.Result;
				result5.StateAttribute13 += ":P";
				tcpResponse = pop3Connection.SendRequest(request2, false);
			}
			base.VerifyResponse(tcpResponse, string.Format("Could not login to user: {0} with POP on port: {1} {2} SSL encryption", base.UserName, base.EndPoint.Port, base.IsSecure ? "with" : "without"));
			ProbeResult result6 = base.Result;
			object stateAttribute = result6.StateAttribute21;
			result6.StateAttribute21 = string.Concat(new object[]
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
			WTFDiagnostics.TraceInformation(ExTraceGlobals.POP3Tracer, base.TraceContext, "Determining Login capability.", null, "DetermineCapability", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PopImap\\Pop3\\PopProbe.cs", 128);
			Pop3Connection pop3Connection = base.TcpCon as Pop3Connection;
			TcpResponse tcpResponse = pop3Connection.SendRequest("CAPA", true);
			if (tcpResponse.ResponseType != ResponseType.success)
			{
				string message = string.Format("Unexpected Server Response while Determining Login Capability: \"{0}\"", tcpResponse.ResponseString);
				WTFDiagnostics.TraceError(ExTraceGlobals.POP3Tracer, base.TraceContext, tcpResponse.ResponseString, null, "DetermineCapability", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PopImap\\Pop3\\PopProbe.cs", 139);
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

		private const string UserFormat = "user {0}";

		private const string PassFormat = "pass {0}";

		private const int PopUnsecuredPort = 110;
	}
}
