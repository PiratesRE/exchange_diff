using System;
using System.Web;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public class FolderConfiguration
	{
		private FolderConfiguration()
		{
			this.scriptsPath = new StringAppSettingsEntry("ScriptsPath", "scripts", null).Value;
			this.resourcesPath = new StringAppSettingsEntry("ResourcesPath", "resources", null).Value;
			this.rootPath = null;
			string text = new StringAppSettingsEntry("VDirForResources", null, null).Value;
			if (!string.IsNullOrEmpty(text))
			{
				if (!text.StartsWith("/"))
				{
					text = "/" + text;
				}
				try
				{
					this.rootPath = HttpContext.Current.Server.MapPath(text);
				}
				catch (Exception)
				{
				}
			}
			if (string.IsNullOrEmpty(this.rootPath))
			{
				this.rootPath = HttpRuntime.AppDomainAppPath;
			}
		}

		public static FolderConfiguration Instance
		{
			get
			{
				if (FolderConfiguration.instance == null)
				{
					lock (FolderConfiguration.syncRoot)
					{
						if (FolderConfiguration.instance == null)
						{
							FolderConfiguration.instance = new FolderConfiguration();
						}
					}
				}
				return FolderConfiguration.instance;
			}
		}

		internal string ScriptsPath
		{
			get
			{
				return this.scriptsPath;
			}
		}

		internal string ResourcesPath
		{
			get
			{
				return this.resourcesPath;
			}
		}

		internal string RootPath
		{
			get
			{
				return this.rootPath;
			}
		}

		private readonly string scriptsPath;

		private readonly string resourcesPath;

		private readonly string rootPath;

		private static volatile FolderConfiguration instance = null;

		private static object syncRoot = new object();
	}
}
