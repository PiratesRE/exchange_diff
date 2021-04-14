using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.PopImap.IMAP4;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.PopImap.Probes
{
	public class ImapMailboxProbeSSL : ImapProbe
	{
		public override void PopulateDefinition<Definition>(Definition definition, Dictionary<string, string> propertyBag)
		{
			string b = string.Format("ImapCTP{0}", "Probe");
			string b2 = string.Format("ImapDeepTest{0}", "Probe");
			string b3 = string.Format("ImapProxyTest{0}", "Probe");
			string b4 = string.Format("ImapSelfTest{0}", "Probe");
			bool isLightModeProbe;
			bool isMbxProbe;
			if (propertyBag["Name"] == b)
			{
				isLightModeProbe = false;
				isMbxProbe = false;
			}
			else if (propertyBag["Name"] == b2)
			{
				isLightModeProbe = false;
				isMbxProbe = true;
			}
			else if (propertyBag["Name"] == b3)
			{
				isLightModeProbe = true;
				isMbxProbe = false;
			}
			else if (propertyBag["Name"] == b4)
			{
				isLightModeProbe = true;
				isMbxProbe = true;
			}
			else
			{
				isLightModeProbe = false;
				isMbxProbe = false;
			}
			PopImapAdConfiguration configuration = ImapDiscovery.GetConfiguration(base.TraceContext);
			IPEndPoint cafeEndpoint;
			IPEndPoint mbxEndpoint;
			if (!PopImapDiscoveryCommon.GetEndpointsFromConfig(configuration, out cafeEndpoint, out mbxEndpoint))
			{
				cafeEndpoint = new IPEndPoint(IPAddress.Loopback, 993);
				mbxEndpoint = new IPEndPoint(IPAddress.Loopback, 1993);
			}
			PopImapDiscoveryCommon.PopulateDefinition<Definition>(definition, propertyBag, mbxEndpoint, cafeEndpoint, isMbxProbe, isLightModeProbe);
		}

		internal override IEnumerable<PropertyInformation> GetSubstitutePropertyInformation()
		{
			return PopImapDiscoveryCommon.GetSubstitutePropertyInfo();
		}

		protected override void InitializeConnection()
		{
			DateTime utcNow = DateTime.UtcNow;
			ProbeResult result = base.Result;
			result.StateAttribute21 += "PICS;";
			base.IsSecure = true;
			WTFDiagnostics.TraceInformation(ExTraceGlobals.IMAP4Tracer, base.TraceContext, "Initializing SSL.", null, "InitializeConnection", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PopImap\\Imap4\\ImapMailboxProbeSSL.cs", 119);
			base.TcpCon = new Imap4Connection(base.EndPoint);
			base.TcpCon.NegotiateSSL();
			TcpResponse response = base.TcpCon.GetResponse();
			base.Result.StateAttribute3 = base.DecodeServerName(response.ResponseMessage);
			base.Result.StateAttribute12 = response.ResponseString;
			this.DetermineCapability();
			ProbeResult result2 = base.Result;
			object stateAttribute = result2.StateAttribute21;
			result2.StateAttribute21 = string.Concat(new object[]
			{
				stateAttribute,
				"PICE:",
				(int)(DateTime.UtcNow - utcNow).TotalMilliseconds,
				";"
			});
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			DateTime utcNow = DateTime.UtcNow;
			ProbeResult result = base.Result;
			result.StateAttribute21 += "PDWS;";
			this.latencyMeasurementStart = DateTime.UtcNow;
			this.lightMode = false;
			if (base.Definition.Attributes.ContainsKey("LightMode"))
			{
				bool.TryParse(base.Definition.Attributes["LightMode"], out this.lightMode);
			}
			int millisecondsTimeout = this.lightMode ? 55000 : 110000;
			if (base.Definition.Attributes.ContainsKey("ProbeTimeout"))
			{
				int.TryParse(base.Definition.Attributes["ProbeTimeout"], out millisecondsTimeout);
			}
			try
			{
				Task task = Task.Factory.StartNew(delegate()
				{
					this.StartProbe();
				}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Current);
				if (!task.Wait(millisecondsTimeout, cancellationToken))
				{
					this.probeTimedOut = true;
					base.VerifyResponse(null, "Current probe execution timedout.");
				}
				if (base.IsMbxProbe && string.IsNullOrEmpty(base.Result.StateAttribute4))
				{
					base.Result.StateAttribute4 = base.EndPoint.Address.ToString();
				}
			}
			finally
			{
				if (base.TcpCon != null)
				{
					base.TcpCon.Close();
				}
				base.Result.SampleValue = (double)((int)(DateTime.UtcNow - this.latencyMeasurementStart).TotalMilliseconds);
				ProbeResult result2 = base.Result;
				result2.StateAttribute21 = result2.StateAttribute21 + "PDWE:" + (int)(DateTime.UtcNow - utcNow).TotalMilliseconds;
			}
		}

		protected void StartProbe()
		{
			try
			{
				this.ParseDefinition();
				this.acceptableErrors.Add("WrongServerException");
				bool flag = false;
				if (base.Definition.Attributes.ContainsKey("DatabaseGuid"))
				{
					Guid databaseGuid = new Guid(base.Definition.Attributes["DatabaseGuid"]);
					flag = DirectoryAccessor.Instance.IsDatabaseCopyActiveOnLocalServer(databaseGuid);
				}
				bool skipLogin = this.lightMode || (base.IsMbxProbe && !flag);
				ProbeResult result = base.Result;
				result.StateAttribute13 += "R1";
				if (!base.InvokeProbe(skipLogin) && !base.IsLocalProbe)
				{
					ProbeResult result2 = base.Result;
					result2.StateAttribute13 += ":R2";
					base.InvokeProbe(skipLogin);
				}
			}
			catch (Exception ex)
			{
				base.Result.FailureContext = ex.ToString();
				throw;
			}
		}

		protected bool lightMode;
	}
}
