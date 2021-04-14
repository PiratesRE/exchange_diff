using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class RegionalSettingsNotConfiguredException : LocalizedException
	{
		public RegionalSettingsNotConfiguredException(ADObjectId user) : base(Strings.RegionalSettingsNotConfigured(user))
		{
			this.user = user;
		}

		public RegionalSettingsNotConfiguredException(ADObjectId user, Exception innerException) : base(Strings.RegionalSettingsNotConfigured(user), innerException)
		{
			this.user = user;
		}

		protected RegionalSettingsNotConfiguredException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.user = (ADObjectId)info.GetValue("user", typeof(ADObjectId));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("user", this.user);
		}

		public ADObjectId User
		{
			get
			{
				return this.user;
			}
		}

		private readonly ADObjectId user;
	}
}
