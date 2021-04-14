using System;
using System.CodeDom.Compiler;

namespace Microsoft.WindowsAzure.ActiveDirectoryV142
{
	public class ServicePlanInfo
	{
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
		public string servicePlanName
		{
			get
			{
				return this._servicePlanName;
			}
			set
			{
				this._servicePlanName = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Guid? _servicePlanId;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _servicePlanName;
	}
}
