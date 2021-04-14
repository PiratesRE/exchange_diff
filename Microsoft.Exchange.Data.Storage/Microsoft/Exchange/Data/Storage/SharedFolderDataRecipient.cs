using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public sealed class SharedFolderDataRecipient
	{
		[XmlElement]
		public string SmtpAddress { get; set; }

		[XmlElement]
		public string SharingKey { get; set; }
	}
}
