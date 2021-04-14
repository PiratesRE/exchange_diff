using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Storage.VersionedXml
{
	[Serializable]
	public abstract class SwitchableSettingsBase
	{
		protected SwitchableSettingsBase()
		{
		}

		protected SwitchableSettingsBase(bool enabled)
		{
			this.Enabled = enabled;
		}

		[XmlElement("Enabled")]
		public bool Enabled { get; set; }
	}
}
