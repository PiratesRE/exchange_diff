using System;
using System.IO;
using System.Web;
using Microsoft.Exchange.Management.ControlPanel.WebControls;
using Microsoft.Exchange.Monitoring;
using Microsoft.Exchange.Net.WebApplicationClient;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class ProxyConnection : ExchangeControlPanelApplication
	{
		public ProxyConnection(Uri serviceUrl) : this(serviceUrl.AbsolutePath, new ProxyWebSession(serviceUrl)
		{
			TrustAnySSLCertificate = Registry.AllowInternalUntrustedCerts
		})
		{
		}

		private ProxyConnection(string virtualDirectory, ProxyWebSession proxyWebSession) : base(virtualDirectory, proxyWebSession)
		{
			this.ProxyWebSession = proxyWebSession;
			proxyWebSession.RequestException += this.ProxyWebSession_RequestException;
			Action<ProxyConnection> newProxyConnection = ProxyConnection.NewProxyConnection;
			if (newProxyConnection != null)
			{
				newProxyConnection(this);
			}
		}

		public ProxyWebSession ProxyWebSession { get; private set; }

		private void ProxyWebSession_RequestException(object sender, WebExceptionEventArgs e)
		{
			if (e.Exception.GetTroubleshootingID() == WebExceptionTroubleshootingID.ServiceUnavailable)
			{
				this.isAlive = false;
			}
		}

		public void Ping(Action<ProxyConnection> onStatusAvailable)
		{
			base.Ping(delegate(ExchangeControlPanelApplication.PingResponse pingResponse)
			{
				this.isAlive = pingResponse.IsAlive;
				this.isCompatible = this.CheckVersionCompatibility(pingResponse.ApplicationVersion);
				onStatusAvailable(this);
			}, delegate(Exception exception)
			{
				this.isAlive = false;
				onStatusAvailable(this);
			});
		}

		public bool IsAlive
		{
			get
			{
				return this.isAlive;
			}
		}

		public bool IsCompatible
		{
			get
			{
				return this.isCompatible;
			}
		}

		private bool CheckVersionCompatibility(Version internalApplicationVersion)
		{
			bool result;
			using (StringWriter stringWriter = new StringWriter())
			{
				bool flag = ProxyConnection.ValidProxyVersions.IsCompatible(internalApplicationVersion, stringWriter);
				if (!flag)
				{
					string periodicKey = base.BaseUri.Host + internalApplicationVersion.ToString();
					EcpEventLogConstants.Tuple_ProxyErrorCASCompatibility.LogPeriodicEvent(periodicKey, new object[]
					{
						EcpEventLogExtensions.GetUserNameToLog(),
						Environment.MachineName,
						ThemeResource.ApplicationVersion,
						base.BaseUri.Host,
						internalApplicationVersion,
						stringWriter.GetStringBuilder()
					});
				}
				result = flag;
			}
			return result;
		}

		internal static ProxyVersions ValidProxyVersions
		{
			get
			{
				if (ProxyConnection.validProxyVersions == null)
				{
					ProxyConnection.validProxyVersions = new ProxyVersions(HttpRuntime.AppDomainAppPath);
				}
				return ProxyConnection.validProxyVersions;
			}
			set
			{
				ProxyConnection.validProxyVersions = value;
			}
		}

		private volatile bool isAlive;

		private volatile bool isCompatible = true;

		internal static Action<ProxyConnection> NewProxyConnection;

		private static ProxyVersions validProxyVersions;
	}
}
