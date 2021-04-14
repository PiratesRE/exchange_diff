using System;
using System.CodeDom.Compiler;

namespace Microsoft.WindowsAzure.ActiveDirectory
{
	public class ProvisioningError
	{
		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string errorDetail
		{
			get
			{
				return this._errorDetail;
			}
			set
			{
				this._errorDetail = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public bool? resolved
		{
			get
			{
				return this._resolved;
			}
			set
			{
				this._resolved = value;
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
		public DateTime? timestamp
		{
			get
			{
				return this._timestamp;
			}
			set
			{
				this._timestamp = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _errorDetail;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private bool? _resolved;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _service;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private DateTime? _timestamp;
	}
}
