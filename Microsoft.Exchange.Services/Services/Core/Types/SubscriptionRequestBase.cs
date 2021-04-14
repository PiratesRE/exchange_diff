using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlInclude(typeof(PullSubscriptionRequest))]
	[KnownType(typeof(PullSubscriptionRequest))]
	[KnownType(typeof(PushSubscriptionRequest))]
	[KnownType(typeof(StreamingSubscriptionRequest))]
	[XmlInclude(typeof(PushSubscriptionRequest))]
	[XmlInclude(typeof(StreamingSubscriptionRequest))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("BaseSubscriptionRequestType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public abstract class SubscriptionRequestBase
	{
		public SubscriptionRequestBase()
		{
		}

		[DataMember(Name = "FolderIds", IsRequired = false, Order = 1)]
		[XmlArrayItem("DistinguishedFolderId", typeof(DistinguishedFolderId), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArrayItem("FolderId", typeof(FolderId), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArray("FolderIds")]
		public BaseFolderId[] FolderIds { get; set; }

		[DataMember(Name = "SubscribeToAllFolders", IsRequired = false, Order = 2)]
		[XmlAttribute("SubscribeToAllFolders", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public bool SubscribeToAllFolders { get; set; }

		[XmlArrayItem("EventType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(EventType))]
		[IgnoreDataMember]
		[XmlArray("EventTypes", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public EventType[] EventTypes { get; set; }

		[DataMember(Name = "EventTypes", IsRequired = true, Order = 3)]
		[XmlIgnore]
		public string[] EventTypesString
		{
			get
			{
				if (this.EventTypes == null)
				{
					return null;
				}
				string[] array = new string[this.EventTypes.Length];
				for (int i = 0; i < this.EventTypes.Length; i++)
				{
					array[i] = EnumUtilities.ToString<EventType>(this.EventTypes[i]);
				}
				return array;
			}
			set
			{
				if (value == null)
				{
					this.EventTypes = null;
					return;
				}
				this.EventTypes = new EventType[value.Length];
				for (int i = 0; i < this.EventTypes.Length; i++)
				{
					this.EventTypes[i] = EnumUtilities.Parse<EventType>(value[i]);
				}
			}
		}

		[DataMember(Name = "Watermark", IsRequired = false, Order = 4)]
		[XmlElement("Watermark", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public string Watermark { get; set; }
	}
}
