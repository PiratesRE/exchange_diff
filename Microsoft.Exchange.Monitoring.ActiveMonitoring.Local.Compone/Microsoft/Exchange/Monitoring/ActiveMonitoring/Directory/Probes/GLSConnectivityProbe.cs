using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Directory.Probes
{
	public class GLSConnectivityProbe : ProbeWorkItem
	{
		public override void PopulateDefinition<ProbeDefinition>(ProbeDefinition pDef, Dictionary<string, string> propertyBag)
		{
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			DirectoryUtils.Logger(this, StxLogType.TestGlobalLocatorService, delegate
			{
				bool flag = false;
				WTFDiagnostics.TraceInformation(ExTraceGlobals.DirectoryTracer, base.TraceContext, "Starting GLS availability check against local server", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\GLSConnectivityProbe.cs", 58);
				string text;
				string value;
				string text2;
				if (!DirectoryUtils.GetCredentials(out text, out value, out text2, this))
				{
					base.Result.StateAttribute1 = "No Monitoring users";
					return;
				}
				base.Result.StateAttribute3 = text;
				if (!string.IsNullOrEmpty(text) && SmtpAddress.IsValidSmtpAddress(text) && !string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(text2))
				{
					bool flag2 = false;
					try
					{
						GlsDirectorySession glsDirectorySession = new GlsDirectorySession();
						string text3;
						flag2 = glsDirectorySession.TryGetRedirectServer(text, GlsCacheServiceMode.CacheDisabled, out text3);
					}
					catch (Exception ex)
					{
						Uri uri = this.GLSEndpointUri();
						string absoluteUri = uri.AbsoluteUri;
						string uriPath = absoluteUri.Substring(0, absoluteUri.IndexOf('/', 8)) + "/smoketest/test.htm";
						this.Traceroute(uri.Host);
						this.CheckStaticGLSPage(uriPath);
						string text4 = string.Format("Exception occured when user {0} getting redirect server. GLS info obtained as {1}. Exception is {2}.", text, this.GetGLSGeneralInfo(), ex.ToString());
						base.Result.Error = text4;
						throw new Exception(text4);
					}
					if (!flag2)
					{
						base.Result.StateAttribute1 = "Domain not found";
					}
					else
					{
						base.Result.StateAttribute1 = "Domain Found";
						flag = true;
					}
					WTFDiagnostics.TraceInformation<bool, double, string, string>(ExTraceGlobals.DirectoryTracer, base.TraceContext, "Operation succeeded: {0} Time Taken {1} Output {2} Error{3}", flag, base.Result.SampleValue, base.Result.StateAttribute1, base.Result.Error, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\GLSConnectivityProbe.cs", 107);
					if (!flag)
					{
						string text4 = string.Format("GLS does not find domain {0} for user {1}. GLS info obtained as {2}", text2, text, this.GetGLSGeneralInfo());
						base.Result.Error = text4;
						throw new Exception(text4);
					}
				}
				else
				{
					base.Result.StateAttribute1 = "Empty or invalid Monitoring user, check StateAttribute3 for username";
				}
			});
		}

		public string GetGLSGeneralInfo()
		{
			Uri uri = this.GLSEndpointUri();
			string absoluteUri = uri.AbsoluteUri;
			string text = absoluteUri.Substring(0, absoluteUri.IndexOf('/', 8)) + "/smoketest/test.htm";
			string text2 = this.Traceroute(uri.Host);
			string text3 = this.CheckStaticGLSPage(text);
			return string.Format("GLSEndpointUri {0}. StaticUri {1}. TraceRoute {2}. Check on static page returned {3}.", new object[]
			{
				absoluteUri,
				text,
				text2,
				text3
			});
		}

		public Uri GLSEndpointUri()
		{
			string commonName = "GlobalLocatorService";
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 163, "GLSEndpointUri", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\GLSConnectivityProbe.cs");
			ServiceEndpointContainer endpointContainer = topologyConfigurationSession.GetEndpointContainer();
			return endpointContainer.GetEndpoint(commonName).Uri;
		}

		public string Traceroute(string ipAddressOrHostName)
		{
			StringBuilder stringBuilder = new StringBuilder();
			try
			{
				IPAddress ipaddress = Dns.GetHostEntry(ipAddressOrHostName).AddressList[0];
				using (Ping ping = new Ping())
				{
					PingOptions pingOptions = new PingOptions();
					Stopwatch stopwatch = new Stopwatch();
					pingOptions.DontFragment = true;
					pingOptions.Ttl = 1;
					int num = 10;
					stringBuilder.AppendLine(string.Format("Tracing route to {0} over a maximum of {1} hops:", ipaddress, num));
					stringBuilder.AppendLine();
					for (int i = 1; i < num + 1; i++)
					{
						stopwatch.Reset();
						stopwatch.Start();
						PingReply pingReply = ping.Send(ipaddress, 5000, new byte[32], pingOptions);
						stopwatch.Stop();
						stringBuilder.AppendLine(string.Format("{0}\t{1} ms\t{2}", i, stopwatch.ElapsedMilliseconds, pingReply.Address));
						if (pingReply.Status == IPStatus.Success)
						{
							stringBuilder.AppendLine();
							stringBuilder.AppendLine("Trace complete.");
							break;
						}
						pingOptions.Ttl++;
					}
				}
			}
			catch (Exception ex)
			{
				stringBuilder.AppendLine(ex.ToString());
			}
			return stringBuilder.ToString();
		}

		public string CheckStaticGLSPage(string uriPath)
		{
			StringBuilder stringBuilder = new StringBuilder();
			try
			{
				Uri requestUri = new Uri(uriPath);
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUri);
				HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				string arg;
				if (httpWebResponse.ContentLength > 0L)
				{
					StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.UTF8);
					if (streamReader != null)
					{
						arg = string.Format("succeeded with response {0} ", streamReader.ReadToEnd());
						streamReader.Close();
					}
					else
					{
						arg = "succeeded but with null content.";
					}
					httpWebResponse.Close();
				}
				else
				{
					arg = "failed";
				}
				stringBuilder.AppendLine(string.Format("GetResponse from static GLS page {0} {1}", uriPath, arg));
				stringBuilder.AppendLine();
			}
			catch (Exception ex)
			{
				stringBuilder.AppendLine(ex.ToString());
			}
			return stringBuilder.ToString();
		}
	}
}
