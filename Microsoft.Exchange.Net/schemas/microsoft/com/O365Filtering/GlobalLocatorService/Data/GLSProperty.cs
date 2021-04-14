using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace schemas.microsoft.com.O365Filtering.GlobalLocatorService.Data
{
	[DataContract(Name = "GLSProperty", Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/Data")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	public class GLSProperty : DITimeStamp
	{
		[DataMember(IsRequired = true)]
		public string PropertyName
		{
			get
			{
				return this.PropertyNameField;
			}
			set
			{
				this.PropertyNameField = value;
			}
		}

		[DataMember(IsRequired = true)]
		public string PropertyValue
		{
			get
			{
				return this.PropertyValueField;
			}
			set
			{
				this.PropertyValueField = value;
			}
		}

		private string PropertyNameField;

		private string PropertyValueField;
	}
}
