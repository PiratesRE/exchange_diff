using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SettingOverrideInvalidParameterNameException : SettingOverrideException
	{
		public SettingOverrideInvalidParameterNameException(string componentName, string sectionName, string parameterName, string availableParameterNames) : base(DirectoryStrings.ErrorSettingOverrideInvalidParameterName(componentName, sectionName, parameterName, availableParameterNames))
		{
			this.componentName = componentName;
			this.sectionName = sectionName;
			this.parameterName = parameterName;
			this.availableParameterNames = availableParameterNames;
		}

		public SettingOverrideInvalidParameterNameException(string componentName, string sectionName, string parameterName, string availableParameterNames, Exception innerException) : base(DirectoryStrings.ErrorSettingOverrideInvalidParameterName(componentName, sectionName, parameterName, availableParameterNames), innerException)
		{
			this.componentName = componentName;
			this.sectionName = sectionName;
			this.parameterName = parameterName;
			this.availableParameterNames = availableParameterNames;
		}

		protected SettingOverrideInvalidParameterNameException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.componentName = (string)info.GetValue("componentName", typeof(string));
			this.sectionName = (string)info.GetValue("sectionName", typeof(string));
			this.parameterName = (string)info.GetValue("parameterName", typeof(string));
			this.availableParameterNames = (string)info.GetValue("availableParameterNames", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("componentName", this.componentName);
			info.AddValue("sectionName", this.sectionName);
			info.AddValue("parameterName", this.parameterName);
			info.AddValue("availableParameterNames", this.availableParameterNames);
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

		public string ParameterName
		{
			get
			{
				return this.parameterName;
			}
		}

		public string AvailableParameterNames
		{
			get
			{
				return this.availableParameterNames;
			}
		}

		private readonly string componentName;

		private readonly string sectionName;

		private readonly string parameterName;

		private readonly string availableParameterNames;
	}
}
