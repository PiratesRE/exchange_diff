using System;
using System.Configuration;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Exchange.Diagnostics.Components.Management.ControlPanel;
using Microsoft.Exchange.Management.ControlPanel.WebControls;
using Microsoft.Exchange.PowerShell.RbacHostingTools;
using Microsoft.Online.BOX.Shell;
using Microsoft.Online.BOX.UI.Shell;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class ShellClient : NavBarClientBase
	{
		static ShellClient()
		{
			string text = ConfigurationManager.AppSettings["ShellServiceTimeout"];
			if (string.IsNullOrEmpty(text) || !int.TryParse(text, out ShellClient.shellServiceTimeout))
			{
				ShellClient.shellServiceTimeout = 2000;
			}
		}

		public ShellClient(bool showAdminFeature, bool fallbackMode, bool forceReload) : base(showAdminFeature, fallbackMode, forceReload)
		{
		}

		protected override bool UseNavBarPackCache
		{
			get
			{
				return false;
			}
		}

		protected override string Office365Copyright
		{
			get
			{
				return HttpUtility.HtmlDecode(Strings.Office365Copyright);
			}
		}

		protected override NavBarInfoRequest CreateRequest()
		{
			return new ShellInfoRequest
			{
				UserPuid = this.userPuid,
				UserPrincipalName = this.userPrincipalName,
				CultureName = this.cultureName,
				WorkloadId = (RbacPrincipal.Current.IsInRole("FFO") ? 7 : 1),
				CurrentMainLinkID = 5,
				TrackingGuid = Guid.NewGuid().ToString(),
				BrandId = null,
				ExcludeMSAjax = true,
				ShellBaseFlight = null
			};
		}

		protected override void BeginGetNavBarPack(ShellServiceClient client, NavBarInfoRequest request)
		{
			string value = HttpContext.Current.Request.QueryString["flight"];
			if (!string.IsNullOrEmpty(value))
			{
				EndpointAddress address = client.Endpoint.Address;
				Uri uri = new Uri(EcpUrl.AppendQueryParameter(address.Uri.AbsoluteUri, "flight", value));
				client.Endpoint.Address = new EndpointAddress(uri, address.Identity, new AddressHeader[0]);
			}
			this.stopwatch = new Stopwatch();
			this.stopwatch.Start();
			this.shellServiceTask = new Task(delegate()
			{
				this.GetNavBarInfoAndHandleException(client, request);
			});
			this.shellServiceTask.Start();
		}

		protected override void CallShellService(ShellServiceClient client, NavBarInfoRequest request)
		{
			this.shellInfo = client.GetShellInfo((ShellInfoRequest)request);
			this.stopwatch.Stop();
			ExTraceGlobals.WebServiceTracer.TraceInformation(0, 0L, string.Format("Successfully called shell service in {0}ms", this.stopwatch.ElapsedMilliseconds));
		}

		protected override NavBarPack GetMockNavBarPack()
		{
			return MockNavBar.GetMockNavBarPack(this.userPuid, this.userPrincipalName, this.cultureName, RtlUtil.IsRtl, this.isGallatin, true);
		}

		protected override NavBarPack EndGetNavBarPack()
		{
			ShellInfo shellInfo = null;
			if (this.shellServiceTask != null)
			{
				if (this.shellServiceTask.Wait(ShellClient.shellServiceTimeout))
				{
					if (!base.LogException())
					{
						shellInfo = this.shellInfo;
					}
				}
				else
				{
					this.stopwatch.Stop();
					ExTraceGlobals.WebServiceTracer.TraceInformation(0, 0L, string.Format("Timeout when calling Shell Service after {0}ms", this.stopwatch.ElapsedMilliseconds));
					EcpEventLogConstants.Tuple_Office365NavBarCallServiceTimeout.LogEvent(new object[0]);
				}
			}
			if (shellInfo != null)
			{
				NavBarPack navBarPackFromInfo = NavBarClientBase.GetNavBarPackFromInfo(shellInfo, this.cultureName);
				ExTraceGlobals.WebServiceTracer.TraceInformation(0, 0L, string.Format("NavBarData acquired. SessionId: {0}, CorrelationId: {1}", navBarPackFromInfo.NavBarData.SessionID, navBarPackFromInfo.NavBarData.CorrelationID));
				navBarPackFromInfo.IsGeminiShellEnabled = true;
				navBarPackFromInfo.IsFresh = true;
				return navBarPackFromInfo;
			}
			return null;
		}

		private const int ShellServiceTimeoutDefault = 2000;

		private static int shellServiceTimeout;

		private Task shellServiceTask;

		private ShellInfo shellInfo;

		private Stopwatch stopwatch;
	}
}
