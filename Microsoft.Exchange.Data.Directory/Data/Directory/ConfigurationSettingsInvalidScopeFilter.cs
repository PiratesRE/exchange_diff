using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ConfigurationSettingsInvalidScopeFilter : ConfigurationSettingsException
	{
		public ConfigurationSettingsInvalidScopeFilter(string error) : base(DirectoryStrings.ConfigurationSettingsInvalidScopeFilter(error))
		{
			this.error = error;
		}

		public ConfigurationSettingsInvalidScopeFilter(string error, Exception innerException) : base(DirectoryStrings.ConfigurationSettingsInvalidScopeFilter(error), innerException)
		{
			this.error = error;
		}

		protected ConfigurationSettingsInvalidScopeFilter(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.error = (string)info.GetValue("error", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("error", this.error);
		}

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly string error;
	}
}
