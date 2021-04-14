using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract]
	public class SyncPersonaContactsResponseBase : ResponseMessage
	{
		public SyncPersonaContactsResponseBase()
		{
		}

		internal SyncPersonaContactsResponseBase(ServiceResultCode code, ServiceError error) : base(code, error)
		{
		}

		[DataMember(Name = "SyncState", IsRequired = true)]
		[XmlElement("SyncState", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public string SyncState { get; set; }

		[XmlElement("IncludesLastItemInRange", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[DataMember(Name = "IncludesLastItemInRange", IsRequired = true, EmitDefaultValue = true)]
		public bool IncludesLastItemInRange { get; set; }

		[XmlArray(ElementName = "DeletedPeople")]
		[XmlArrayItem(ElementName = "PersonaId", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[DataMember(EmitDefaultValue = false)]
		public ItemId[] DeletedPeople { get; set; }

		[DataMember(EmitDefaultValue = false)]
		[XmlArray(ElementName = "JumpHeaderSortKeys")]
		[XmlArrayItem(ElementName = "JumpHeaderSortKeys", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public string[] JumpHeaderSortKeys { get; set; }

		[XmlElement("SortKeyVersion", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[DataMember(Name = "SortKeyVersion", IsRequired = true)]
		public string SortKeyVersion { get; set; }
	}
}
