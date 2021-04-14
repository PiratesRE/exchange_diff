using System;
using System.CodeDom.Compiler;

namespace Microsoft.WindowsAzure.ActiveDirectoryV122
{
	public class AssignedPlan
	{
		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public DateTime? assignedTimestamp
		{
			get
			{
				return this._assignedTimestamp;
			}
			set
			{
				this._assignedTimestamp = value;
			}
		}

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
		public Guid? servicePlanId
		{
			get
			{
				return this._servicePlanId;
			}
			set
			{
				this._servicePlanId = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private DateTime? _assignedTimestamp;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _capabilityStatus;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _service;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Guid? _servicePlanId;
	}
}
