using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class OlcSettingNotImplementedPermanentException : MailboxReplicationPermanentException
	{
		public OlcSettingNotImplementedPermanentException(string settingType, string settingName) : base(MrsStrings.OlcSettingNotImplemented(settingType, settingName))
		{
			this.settingType = settingType;
			this.settingName = settingName;
		}

		public OlcSettingNotImplementedPermanentException(string settingType, string settingName, Exception innerException) : base(MrsStrings.OlcSettingNotImplemented(settingType, settingName), innerException)
		{
			this.settingType = settingType;
			this.settingName = settingName;
		}

		protected OlcSettingNotImplementedPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.settingType = (string)info.GetValue("settingType", typeof(string));
			this.settingName = (string)info.GetValue("settingName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("settingType", this.settingType);
			info.AddValue("settingName", this.settingName);
		}

		public string SettingType
		{
			get
			{
				return this.settingType;
			}
		}

		public string SettingName
		{
			get
			{
				return this.settingName;
			}
		}

		private readonly string settingType;

		private readonly string settingName;
	}
}
