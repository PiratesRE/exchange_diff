using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ConfigurationSettingsPropertyNotFoundException : ConfigurationSettingsException
	{
		public ConfigurationSettingsPropertyNotFoundException(string name, string knownProperties) : base(DataStrings.ConfigurationSettingsPropertyNotFound2(name, knownProperties))
		{
			this.name = name;
			this.knownProperties = knownProperties;
		}

		public ConfigurationSettingsPropertyNotFoundException(string name, string knownProperties, Exception innerException) : base(DataStrings.ConfigurationSettingsPropertyNotFound2(name, knownProperties), innerException)
		{
			this.name = name;
			this.knownProperties = knownProperties;
		}

		protected ConfigurationSettingsPropertyNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.name = (string)info.GetValue("name", typeof(string));
			this.knownProperties = (string)info.GetValue("knownProperties", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("name", this.name);
			info.AddValue("knownProperties", this.knownProperties);
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string KnownProperties
		{
			get
			{
				return this.knownProperties;
			}
		}

		private readonly string name;

		private readonly string knownProperties;
	}
}
