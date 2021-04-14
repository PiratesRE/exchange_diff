using System;

namespace Microsoft.Exchange.Data.Storage.VersionedXml
{
	[TextMessagingSettingsRoot]
	[Serializable]
	public class TextMessagingSettingsBase : VersionedXmlBase
	{
		public TextMessagingSettingsBase()
		{
		}

		public TextMessagingSettingsBase(Version version) : base(version)
		{
		}
	}
}
