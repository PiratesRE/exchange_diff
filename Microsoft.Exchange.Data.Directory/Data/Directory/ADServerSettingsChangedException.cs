using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ADServerSettingsChangedException : ADTransientException
	{
		internal ADServerSettingsChangedException(LocalizedString message, ADServerSettings serverSettings) : base(message)
		{
			if (serverSettings == null)
			{
				throw new ArgumentNullException("serverSettings");
			}
			this.serverSettings = serverSettings;
		}

		public ADServerSettingsChangedException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected ADServerSettingsChangedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.serverSettings = (ADServerSettings)info.GetValue("ServerSettings", typeof(ADServerSettings));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ServerSettings", this.serverSettings);
		}

		internal ADServerSettings ServerSettings
		{
			get
			{
				return this.serverSettings;
			}
		}

		private const string ServerSettingsLabel = "ServerSettings";

		private ADServerSettings serverSettings;
	}
}
