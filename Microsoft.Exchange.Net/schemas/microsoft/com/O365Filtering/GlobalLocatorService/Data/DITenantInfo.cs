using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace schemas.microsoft.com.O365Filtering.GlobalLocatorService.Data
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	[DataContract(Name = "DITenantInfo", Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/Data")]
	public class DITenantInfo : DITimeStamp
	{
		[DataMember(IsRequired = true)]
		public Guid TenantId
		{
			get
			{
				return this.TenantIdField;
			}
			set
			{
				this.TenantIdField = value;
			}
		}

		[DataMember]
		public GLSProperty[] TenantProperties
		{
			get
			{
				return this.TenantPropertiesField;
			}
			set
			{
				this.TenantPropertiesField = value;
			}
		}

		private Guid TenantIdField;

		private GLSProperty[] TenantPropertiesField;
	}
}
