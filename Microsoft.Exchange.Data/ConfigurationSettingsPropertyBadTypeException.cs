using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ConfigurationSettingsPropertyBadTypeException : ConfigurationSettingsException
	{
		public ConfigurationSettingsPropertyBadTypeException(string name, string type) : base(DataStrings.ConfigurationSettingsPropertyBadType(name, type))
		{
			this.name = name;
			this.type = type;
		}

		public ConfigurationSettingsPropertyBadTypeException(string name, string type, Exception innerException) : base(DataStrings.ConfigurationSettingsPropertyBadType(name, type), innerException)
		{
			this.name = name;
			this.type = type;
		}

		protected ConfigurationSettingsPropertyBadTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.name = (string)info.GetValue("name", typeof(string));
			this.type = (string)info.GetValue("type", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("name", this.name);
			info.AddValue("type", this.type);
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string Type
		{
			get
			{
				return this.type;
			}
		}

		private readonly string name;

		private readonly string type;
	}
}
