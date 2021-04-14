using System;
using System.Xml.Serialization;
using Microsoft.Exchange.InfoWorker.Common.Availability;

namespace Microsoft.Exchange.InfoWorker.Common.OOF
{
	[XmlRoot(ElementName = "UserOofSettings")]
	public class UserOofSettingsSerializer
	{
		internal static UserOofSettingsSerializer Serialize(UserOofSettings userOofSettings)
		{
			return new UserOofSettingsSerializer
			{
				InternalReply = ReplyBodySerializer.Serialize(userOofSettings.InternalReply),
				ExternalReply = ReplyBodySerializer.Serialize(userOofSettings.ExternalReply),
				Duration = userOofSettings.Duration,
				OofState = userOofSettings.OofState,
				ExternalAudience = userOofSettings.ExternalAudience,
				SetByLegacyClient = userOofSettings.SetByLegacyClient,
				UserChangeTime = userOofSettings.UserChangeTime
			};
		}

		internal static UserOofSettingsSerializerSerializer Serializer
		{
			get
			{
				if (UserOofSettingsSerializer.serializer == null)
				{
					lock (UserOofSettingsSerializer.serializerLocker)
					{
						if (UserOofSettingsSerializer.serializer == null)
						{
							try
							{
								UserOofSettingsSerializer.serializer = new UserOofSettingsSerializerSerializer();
							}
							catch (InvalidOperationException innerException)
							{
								throw new IWTransientException(Strings.descCorruptUserOofSettingsXmlDocument, innerException);
							}
						}
					}
				}
				return UserOofSettingsSerializer.serializer;
			}
		}

		internal UserOofSettings Deserialize()
		{
			if (!this.Validate())
			{
				throw new InvalidUserOofSettingsXmlDocument();
			}
			UserOofSettings userOofSettings = UserOofSettings.Create();
			userOofSettings.InternalReply = this.InternalReply.Deserialize();
			userOofSettings.ExternalReply = this.ExternalReply.Deserialize();
			userOofSettings.Duration = this.Duration;
			userOofSettings.OofState = this.OofState;
			userOofSettings.ExternalAudience = this.ExternalAudience;
			userOofSettings.SetByLegacyClient = this.SetByLegacyClient;
			userOofSettings.UserChangeTime = this.UserChangeTime;
			return userOofSettings;
		}

		private bool Validate()
		{
			return this.InternalReply != null && this.ExternalReply != null;
		}

		[XmlElement]
		public Duration Duration;

		[XmlElement]
		public OofState OofState;

		[XmlElement]
		public ExternalAudience ExternalAudience;

		[XmlElement]
		public ReplyBodySerializer InternalReply;

		[XmlElement]
		public ReplyBodySerializer ExternalReply;

		[XmlElement]
		public bool SetByLegacyClient;

		[XmlElement]
		public DateTime? UserChangeTime = null;

		private static UserOofSettingsSerializerSerializer serializer;

		private static object serializerLocker = new object();
	}
}
