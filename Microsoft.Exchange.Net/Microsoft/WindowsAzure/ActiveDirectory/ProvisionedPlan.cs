using System;
using System.CodeDom.Compiler;

namespace Microsoft.WindowsAzure.ActiveDirectory
{
	public class ProvisionedPlan
	{
		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string capabilityStatus
		{
			get
			{
				return this._capabilityStatus;
			}
			set
			{
				this._capabilityStatus = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string provisioningStatus
		{
			get
			{
				return this._provisioningStatus;
			}
			set
			{
				this._provisioningStatus = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string service
		{
			get
			{
				return this._service;
			}
			set
			{
				this._service = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _capabilityStatus;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _provisioningStatus;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _service;
	}
}
