using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ConfigurationSettingsScopePropertyNotFoundException : ConfigurationSettingsException
	{
		public ConfigurationSettingsScopePropertyNotFoundException(string name, string knownScopes) : base(DataStrings.ConfigurationSettingsScopePropertyNotFound2(name, knownScopes))
		{
			this.name = name;
			this.knownScopes = knownScopes;
		}

		public ConfigurationSettingsScopePropertyNotFoundException(string name, string knownScopes, Exception innerException) : base(DataStrings.ConfigurationSettingsScopePropertyNotFound2(name, knownScopes), innerException)
		{
			this.name = name;
			this.knownScopes = knownScopes;
		}

		protected ConfigurationSettingsScopePropertyNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.name = (string)info.GetValue("name", typeof(string));
			this.knownScopes = (string)info.GetValue("knownScopes", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("name", this.name);
			info.AddValue("knownScopes", this.knownScopes);
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string KnownScopes
		{
			get
			{
				return this.knownScopes;
			}
		}

		private readonly string name;

		private readonly string knownScopes;
	}
}
