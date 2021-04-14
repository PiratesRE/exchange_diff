using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CouldNotSaveContentSetting : LocalizedException
	{
		public CouldNotSaveContentSetting(string setting) : base(Strings.CouldNotSaveContentSetting(setting))
		{
			this.setting = setting;
		}

		public CouldNotSaveContentSetting(string setting, Exception innerException) : base(Strings.CouldNotSaveContentSetting(setting), innerException)
		{
			this.setting = setting;
		}

		protected CouldNotSaveContentSetting(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.setting = (string)info.GetValue("setting", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("setting", this.setting);
		}

		public string Setting
		{
			get
			{
				return this.setting;
			}
		}

		private readonly string setting;
	}
}
