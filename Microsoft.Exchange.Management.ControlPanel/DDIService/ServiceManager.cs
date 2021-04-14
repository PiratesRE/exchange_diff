using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Markup;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	public class ServiceManager
	{
		public static ServiceManager GetInstance()
		{
			if (ServiceManager.instance != null)
			{
				return ServiceManager.instance;
			}
			lock (ServiceManager.synchronizationObject)
			{
				if (ServiceManager.instance == null)
				{
					ServiceManager.instance = new ServiceManager();
				}
			}
			return ServiceManager.instance;
		}

		protected ServiceManager()
		{
		}

		public int Count
		{
			get
			{
				return this.dic.Count;
			}
		}

		public Service GetService(string schemaFilesInstallPath, string schemaName)
		{
			string text = schemaName.ToLowerInvariant();
			if (!this.dic.ContainsKey(text))
			{
				lock (ServiceManager.synchronizationObject)
				{
					if (!this.dic.ContainsKey(text))
					{
						string[] files = Directory.GetFiles(schemaFilesInstallPath);
						files.Perform(delegate(string c)
						{
							this.dic[Path.GetFileNameWithoutExtension(c).ToLowerInvariant()] = null;
						});
						if (!this.dic.ContainsKey(text))
						{
							DDIHelper.Trace("Requested workflow {0} not defined", new object[]
							{
								text
							});
							throw new SchemaNotExistException(text);
						}
					}
				}
			}
			if (this.dic[text] != null)
			{
				return this.dic[text].Clone() as Service;
			}
			lock (ServiceManager.synchronizationObject)
			{
				if (this.dic[text] == null)
				{
					this.dic[text] = ServiceManager.BuildService(schemaFilesInstallPath, text);
				}
			}
			return this.dic[text].Clone() as Service;
		}

		private static Service BuildService(string schemaFilesInstallPath, string lowerCaseSchemaName)
		{
			string path = string.Format("{0}\\{1}.xaml", schemaFilesInstallPath, lowerCaseSchemaName);
			Service service = null;
			using (EcpPerformanceData.XamlParsed.StartRequestTimer())
			{
				using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
				{
					service = (XamlReader.Load(fileStream) as Service);
				}
			}
			service.Initialize();
			return service;
		}

		private static object synchronizationObject = new object();

		private static ServiceManager instance;

		private Dictionary<string, Service> dic = new Dictionary<string, Service>();
	}
}
