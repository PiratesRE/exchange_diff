using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SettingOverrideInvalidParameterSyntaxException : SettingOverrideException
	{
		public SettingOverrideInvalidParameterSyntaxException(string componentName, string sectionName, string parameter) : base(DirectoryStrings.ErrorSettingOverrideInvalidParameterSyntax(componentName, sectionName, parameter))
		{
			this.componentName = componentName;
			this.sectionName = sectionName;
			this.parameter = parameter;
		}

		public SettingOverrideInvalidParameterSyntaxException(string componentName, string sectionName, string parameter, Exception innerException) : base(DirectoryStrings.ErrorSettingOverrideInvalidParameterSyntax(componentName, sectionName, parameter), innerException)
		{
			this.componentName = componentName;
			this.sectionName = sectionName;
			this.parameter = parameter;
		}

		protected SettingOverrideInvalidParameterSyntaxException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.componentName = (string)info.GetValue("componentName", typeof(string));
			this.sectionName = (string)info.GetValue("sectionName", typeof(string));
			this.parameter = (string)info.GetValue("parameter", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("componentName", this.componentName);
			info.AddValue("sectionName", this.sectionName);
			info.AddValue("parameter", this.parameter);
		}

		public string ComponentName
		{
			get
			{
				return this.componentName;
			}
		}

		public string SectionName
		{
			get
			{
				return this.sectionName;
			}
		}

		public string Parameter
		{
			get
			{
				return this.parameter;
			}
		}

		private readonly string componentName;

		private readonly string sectionName;

		private readonly string parameter;
	}
}
