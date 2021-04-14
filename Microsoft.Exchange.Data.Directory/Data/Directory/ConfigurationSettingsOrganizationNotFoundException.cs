using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ConfigurationSettingsOrganizationNotFoundException : ConfigurationSettingsException
	{
		public ConfigurationSettingsOrganizationNotFoundException(string id) : base(DirectoryStrings.ConfigurationSettingsOrganizationNotFound(id))
		{
			this.id = id;
		}

		public ConfigurationSettingsOrganizationNotFoundException(string id, Exception innerException) : base(DirectoryStrings.ConfigurationSettingsOrganizationNotFound(id), innerException)
		{
			this.id = id;
		}

		protected ConfigurationSettingsOrganizationNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.id = (string)info.GetValue("id", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("id", this.id);
		}

		public string Id
		{
			get
			{
				return this.id;
			}
		}

		private readonly string id;
	}
}
