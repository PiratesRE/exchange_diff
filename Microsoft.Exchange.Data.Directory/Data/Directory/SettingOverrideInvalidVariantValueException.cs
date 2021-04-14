using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SettingOverrideInvalidVariantValueException : SettingOverrideException
	{
		public SettingOverrideInvalidVariantValueException(string componentName, string sectionName, string variantName, string variantType, string format) : base(DirectoryStrings.ErrorSettingOverrideInvalidVariantValue(componentName, sectionName, variantName, variantType, format))
		{
			this.componentName = componentName;
			this.sectionName = sectionName;
			this.variantName = variantName;
			this.variantType = variantType;
			this.format = format;
		}

		public SettingOverrideInvalidVariantValueException(string componentName, string sectionName, string variantName, string variantType, string format, Exception innerException) : base(DirectoryStrings.ErrorSettingOverrideInvalidVariantValue(componentName, sectionName, variantName, variantType, format), innerException)
		{
			this.componentName = componentName;
			this.sectionName = sectionName;
			this.variantName = variantName;
			this.variantType = variantType;
			this.format = format;
		}

		protected SettingOverrideInvalidVariantValueException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.componentName = (string)info.GetValue("componentName", typeof(string));
			this.sectionName = (string)info.GetValue("sectionName", typeof(string));
			this.variantName = (string)info.GetValue("variantName", typeof(string));
			this.variantType = (string)info.GetValue("variantType", typeof(string));
			this.format = (string)info.GetValue("format", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("componentName", this.componentName);
			info.AddValue("sectionName", this.sectionName);
			info.AddValue("variantName", this.variantName);
			info.AddValue("variantType", this.variantType);
			info.AddValue("format", this.format);
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

		public string VariantName
		{
			get
			{
				return this.variantName;
			}
		}

		public string VariantType
		{
			get
			{
				return this.variantType;
			}
		}

		public string Format
		{
			get
			{
				return this.format;
			}
		}

		private readonly string componentName;

		private readonly string sectionName;

		private readonly string variantName;

		private readonly string variantType;

		private readonly string format;
	}
}
