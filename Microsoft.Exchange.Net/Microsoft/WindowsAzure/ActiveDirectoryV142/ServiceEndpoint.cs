using System;
using System.CodeDom.Compiler;
using System.Data.Services.Common;

namespace Microsoft.WindowsAzure.ActiveDirectoryV142
{
	[DataServiceKey("objectId")]
	public class ServiceEndpoint : DirectoryObject
	{
		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public static ServiceEndpoint CreateServiceEndpoint(string objectId)
		{
			return new ServiceEndpoint
			{
				objectId = objectId
			};
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string capability
		{
			get
			{
				return this._capability;
			}
			set
			{
				this._capability = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string serviceId
		{
			get
			{
				return this._serviceId;
			}
			set
			{
				this._serviceId = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string serviceName
		{
			get
			{
				return this._serviceName;
			}
			set
			{
				this._serviceName = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string serviceEndpointUri
		{
			get
			{
				return this._serviceEndpointUri;
			}
			set
			{
				this._serviceEndpointUri = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string serviceResourceId
		{
			get
			{
				return this._serviceResourceId;
			}
			set
			{
				this._serviceResourceId = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _capability;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _serviceId;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _serviceName;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _serviceEndpointUri;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _serviceResourceId;
	}
}
