using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ProtocolConfigurationMissingException : DataSourceOperationException
	{
		public ProtocolConfigurationMissingException(string lastServer, string settingsType) : base(Strings.ErrorProtocolConfigurationMissing(lastServer, settingsType))
		{
			this.lastServer = lastServer;
			this.settingsType = settingsType;
		}

		public ProtocolConfigurationMissingException(string lastServer, string settingsType, Exception innerException) : base(Strings.ErrorProtocolConfigurationMissing(lastServer, settingsType), innerException)
		{
			this.lastServer = lastServer;
			this.settingsType = settingsType;
		}

		protected ProtocolConfigurationMissingException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.lastServer = (string)info.GetValue("lastServer", typeof(string));
			this.settingsType = (string)info.GetValue("settingsType", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("lastServer", this.lastServer);
			info.AddValue("settingsType", this.settingsType);
		}

		public string LastServer
		{
			get
			{
				return this.lastServer;
			}
		}

		public string SettingsType
		{
			get
			{
				return this.settingsType;
			}
		}

		private readonly string lastServer;

		private readonly string settingsType;
	}
}
