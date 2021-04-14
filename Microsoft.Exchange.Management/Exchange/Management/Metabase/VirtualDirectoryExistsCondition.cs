using System;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Metabase
{
	[Serializable]
	internal sealed class VirtualDirectoryExistsCondition : Condition
	{
		public VirtualDirectoryExistsCondition(string virtualDirectoryPath)
		{
			this.VirtualDirectoryPath = virtualDirectoryPath;
		}

		public string VirtualDirectoryPath
		{
			get
			{
				return this.virtualDirectoryPath;
			}
			set
			{
				this.virtualDirectoryPath = value;
			}
		}

		public VirtualDirectoryExistsCondition(string serverName, string webSiteName, string virtualDirectoryName)
		{
			this.ServerName = serverName;
			this.WebSiteName = webSiteName;
			this.VirtualDirectoryName = virtualDirectoryName;
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

		public string VirtualDirectoryName
		{
			get
			{
				return this.virtualDirectoryName;
			}
			set
			{
				this.virtualDirectoryName = value;
			}
		}

		public override bool Verify()
		{
			TaskLogger.LogEnter();
			bool result = false;
			if (this.virtualDirectoryPath != null)
			{
				result = IisUtility.Exists(this.VirtualDirectoryPath, "IIsWebVirtualDir");
			}
			else
			{
				try
				{
					string str = IisUtility.FindWebSiteRoot(this.WebSiteName, this.ServerName);
					result = IisUtility.Exists(str + "/" + this.VirtualDirectoryName, "IIsWebVirtualDir");
				}
				catch (WebObjectNotFoundException)
				{
					result = false;
				}
			}
			TaskLogger.LogExit();
			return result;
		}

		private string virtualDirectoryPath;

		private string serverName;

		private string webSiteName;

		private string virtualDirectoryName;
	}
}
