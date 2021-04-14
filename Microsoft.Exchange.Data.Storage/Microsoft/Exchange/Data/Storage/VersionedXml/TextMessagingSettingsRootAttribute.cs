using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Storage.VersionedXml
{
	public class TextMessagingSettingsRootAttribute : XmlRootAttribute
	{
		public TextMessagingSettingsRootAttribute() : base("TextMessagingSettings")
		{
		}
	}
}
