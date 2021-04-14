using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SettingOverrideSyntaxException : SettingOverrideException
	{
		public SettingOverrideSyntaxException(string componentName, string sectionName, string parameters, string error) : base(DirectoryStrings.ErrorSettingOverrideSyntax(componentName, sectionName, parameters, error))
		{
			this.componentName = componentName;
			this.sectionName = sectionName;
			this.parameters = parameters;
			this.error = error;
		}

		public SettingOverrideSyntaxException(string componentName, string sectionName, string parameters, string error, Exception innerException) : base(DirectoryStrings.ErrorSettingOverrideSyntax(componentName, sectionName, parameters, error), innerException)
		{
			this.componentName = componentName;
			this.sectionName = sectionName;
			this.parameters = parameters;
			this.error = error;
		}

		protected SettingOverrideSyntaxException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.componentName = (string)info.GetValue("componentName", typeof(string));
			this.sectionName = (string)info.GetValue("sectionName", typeof(string));
			this.parameters = (string)info.GetValue("parameters", typeof(string));
			this.error = (string)info.GetValue("error", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("componentName", this.componentName);
			info.AddValue("sectionName", this.sectionName);
			info.AddValue("parameters", this.parameters);
			info.AddValue("error", this.error);
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

		public string Parameters
		{
			get
			{
				return this.parameters;
			}
		}

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly string componentName;

		private readonly string sectionName;

		private readonly string parameters;

		private readonly string error;
	}
}
