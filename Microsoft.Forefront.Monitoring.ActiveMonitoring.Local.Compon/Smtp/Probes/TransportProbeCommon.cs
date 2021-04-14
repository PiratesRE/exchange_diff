using System;
using System.Security;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;
using Microsoft.Win32;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes
{
	internal class TransportProbeCommon
	{
		public static bool IsProbeExecutionEnabled()
		{
			if (TransportProbeCommon.probeExecutionEnabled == null)
			{
				if (!ExEnvironment.IsTest)
				{
					TransportProbeCommon.probeExecutionEnabled = new bool?(true);
				}
				else
				{
					TransportProbeCommon.probeExecutionEnabled = new bool?(TransportProbeCommon.RunTransportProbesKeyPresent());
				}
			}
			return TransportProbeCommon.probeExecutionEnabled.Value;
		}

		internal static bool ErrorMatches(string lastResponse, string errorPattern)
		{
			Match match = Regex.Match(lastResponse.Trim().ToUpperInvariant(), errorPattern.Trim().ToUpperInvariant());
			return match.Success;
		}

		internal static bool ErrorContains(string lastResponse, string errorPattern)
		{
			return lastResponse.IndexOf(errorPattern, StringComparison.OrdinalIgnoreCase) > -1;
		}

		private static bool RunTransportProbesKeyPresent()
		{
			bool result;
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Exchange_Test\\v15\\RunTransportProbes"))
				{
					result = (registryKey != null);
				}
			}
			catch (UnauthorizedAccessException)
			{
				WTFDiagnostics.TraceError(ExTraceGlobals.TransportTracer, new TracingContext(), "UnauthorizedAccessException opening registry key", null, "RunTransportProbesKeyPresent", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Common\\TransportProbeCommon.cs", 97);
				result = false;
			}
			catch (SecurityException)
			{
				WTFDiagnostics.TraceError(ExTraceGlobals.TransportTracer, new TracingContext(), "SecurityException opening registry key", null, "RunTransportProbesKeyPresent", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Common\\TransportProbeCommon.cs", 107);
				result = false;
			}
			return result;
		}

		internal const string RunTransportProbesRegistryKeyName = "Software\\Microsoft\\Exchange_Test\\v15\\RunTransportProbes";

		private static bool? probeExecutionEnabled;
	}
}
