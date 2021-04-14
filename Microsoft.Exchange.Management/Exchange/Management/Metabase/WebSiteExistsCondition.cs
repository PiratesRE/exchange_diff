using System;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Metabase
{
	[Serializable]
	internal sealed class WebSiteExistsCondition : Condition
	{
		public WebSiteExistsCondition(string webSitePath)
		{
			this.WebSitePath = webSitePath;
		}

		public string WebSitePath
		{
			get
			{
				return this.webSitePath;
			}
			set
			{
				this.webSitePath = value;
			}
		}

		public WebSiteExistsCondition(string serverName, string webSiteName)
		{
			this.ServerName = serverName;
			this.WebSiteName = webSiteName;
		}

		public string ServerName
		{
			get
			{
				return this.serverName;
			}
			set
			{
				this.serverName = value;
			}
		}

		public string WebSiteName
		{
			get
			{
				return this.webSiteName;
			}
			set
			{
				this.webSiteName = value;
			}
		}

		public override bool Verify()
		{
			TaskLogger.LogEnter();
			bool flag = false;
			if (this.webSitePath != null)
			{
				flag = IisUtility.Exists(this.WebSitePath, "IIsWebServer");
			}
			else
			{
				try
				{
					IisUtility.FindWebSiteRoot(this.WebSiteName, this.ServerName);
					flag = true;
				}
				catch (WebObjectNotFoundException)
				{
					flag = false;
				}
			}
			TaskLogger.Trace("WebSiteExistsCondition is returning '{0}'", new object[]
			{
				flag
			});
			TaskLogger.LogExit();
			return flag;
		}

		private string webSitePath;

		private string serverName;

		private string webSiteName;
	}
}
