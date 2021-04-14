using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.Data.Services.Common;

namespace Microsoft.WindowsAzure.ActiveDirectory
{
	[DataServiceKey("objectId")]
	public class ServiceInfo : DirectoryObject
	{
		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public static ServiceInfo CreateServiceInfo(string objectId, int version, Collection<string> serviceElements)
		{
			ServiceInfo serviceInfo = new ServiceInfo();
			serviceInfo.objectId = objectId;
			serviceInfo.version = version;
			if (serviceElements == null)
			{
				throw new ArgumentNullException("serviceElements");
			}
			serviceInfo.serviceElements = serviceElements;
			return serviceInfo;
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string serviceInstance
		{
			get
			{
				return this._serviceInstance;
			}
			set
			{
				this._serviceInstance = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public int version
		{
			get
			{
				return this._version;
			}
			set
			{
				this._version = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<string> serviceElements
		{
			get
			{
				return this._serviceElements;
			}
			set
			{
				this._serviceElements = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _serviceInstance;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private int _version;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<string> _serviceElements = new Collection<string>();
	}
}
