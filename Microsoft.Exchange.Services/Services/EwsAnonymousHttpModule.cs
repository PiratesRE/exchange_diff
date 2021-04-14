using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Web;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Authentication;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Web.Administration;

namespace Microsoft.Exchange.Services
{
	public class EwsAnonymousHttpModule : IHttpModule
	{
		public void Dispose()
		{
		}

		public void Init(HttpApplication application)
		{
			application.PostAuthenticateRequest += this.Application_BeginRequest;
		}

		private void Application_BeginRequest(object source, EventArgs e)
		{
			HttpApplication httpApplication = (HttpApplication)source;
			HttpContext context = httpApplication.Context;
			Uri url = context.Request.Url;
			try
			{
				bool flag = EWSSettings.IsWsSecurityAddress(url) || EWSSettings.IsWsSecuritySymmetricKeyAddress(url);
				bool flag2 = flag || EWSSettings.IsWsSecurityX509CertAddress(url);
				if (!context.Request.IsAuthenticated)
				{
					if (context.Request.ContentLength == 0 && context.Request.HttpMethod != "GET" && context.Request.Headers.Get("Content-Length") != null)
					{
						Global.SetHttpResponse(context, HttpStatusCode.Unauthorized);
					}
					else if (!flag2)
					{
						Global.SetHttpResponse(context, HttpStatusCode.Unauthorized);
					}
					else if (!ExternalAuthentication.GetCurrent().Enabled && flag)
					{
						context.Response.Close();
						httpApplication.CompleteRequest();
						if (VariantConfiguration.InvariantNoFlightingSnapshot.Autodiscover.AnonymousAuthentication.Enabled && (ExternalAuthentication.GetCurrent().FailureType == ExternalAuthentication.ExternalAuthenticationFailureType.NoFederationTrust || ExternalAuthentication.GetCurrent().FailureType == ExternalAuthentication.ExternalAuthenticationFailureType.ErrorReadingFederationTrust) && this.IsGoodTimeToRecycleAppPool())
						{
							RequestDetailsLogger.Current.AppendGenericError("RecycleApplicationPool", "1");
							ExTraceGlobals.CommonAlgorithmTracer.TraceDebug((long)this.GetHashCode(), "[EwsAnonymousHttpModule::Application_BeginRequest] ExternalAuthentication is not enabled, recycle the current appPool.");
							this.RecycleThisApplicationPool();
						}
					}
				}
			}
			catch (COMException arg)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<COMException>(0L, "[EwsAnonymousHttpModule::Application_BeginRequest] A COMException was raised during the request: {0}", arg);
				Global.SetHttpResponse(context, HttpStatusCode.InternalServerError);
			}
		}

		private bool IsGoodTimeToRecycleAppPool()
		{
			bool flag = false;
			if (DateTime.UtcNow > EwsAnonymousHttpModule.nextRecycleCheckDateTime)
			{
				lock (EwsAnonymousHttpModule.lockObj)
				{
					if (DateTime.UtcNow > EwsAnonymousHttpModule.nextRecycleCheckDateTime)
					{
						EwsAnonymousHttpModule.nextRecycleCheckDateTime = DateTime.UtcNow.AddMinutes(3.0);
						flag = true;
					}
				}
			}
			if (flag)
			{
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 159, "IsGoodTimeToRecycleAppPool", "f:\\15.00.1497\\sources\\dev\\services\\src\\Services\\Exchange\\EwsAnonymousHttpModule.cs");
				try
				{
					tenantOrTopologyConfigurationSession.Find<FederationTrust>(null, QueryScope.SubTree, null, null, 2);
					return true;
				}
				catch
				{
				}
				return false;
			}
			return false;
		}

		private void RecycleThisApplicationPool()
		{
			int id;
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				id = currentProcess.Id;
			}
			ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<int>(0L, "Searching for application pool of the current process {0}", id);
			using (ServerManager serverManager = new ServerManager())
			{
				foreach (WorkerProcess workerProcess in serverManager.WorkerProcesses)
				{
					if (workerProcess.ProcessId == id)
					{
						ApplicationPool applicationPool = serverManager.ApplicationPools[workerProcess.AppPoolName];
						if (applicationPool != null)
						{
							ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<int, string>(0L, "Found application pool current process {0}: {1}. Recycling application pool now.", id, applicationPool.Name);
							applicationPool.Recycle();
							return;
						}
					}
				}
			}
			ExTraceGlobals.CommonAlgorithmTracer.TraceError<int>(0L, "Unable to find application pool of the current process {0}. Application pool will continue to run without updated certificates", id);
		}

		private const string ContentLengthHeader = "Content-Length";

		private static object lockObj = new object();

		private static DateTime nextRecycleCheckDateTime = DateTime.UtcNow.AddMinutes(3.0);
	}
}
