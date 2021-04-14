using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "GetRemindersType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class GetRemindersRequest : BaseRequest
	{
		[XmlElement("BeginTime", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[IgnoreDataMember]
		public DateTime BeginTime { get; set; }

		[XmlIgnore]
		[DataMember(Name = "BeginTime", IsRequired = false)]
		public string BeginTimeString { get; set; }

		[IgnoreDataMember]
		[XmlElement("EndTime", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public DateTime EndTime { get; set; }

		[DataMember(Name = "EndTime", IsRequired = false)]
		[XmlIgnore]
		public string EndTimeString { get; set; }

		[DataMember]
		[XmlElement("MaxItems", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public int MaxItems { get; set; }

		[XmlElement("ReminderType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[DataMember]
		public ReminderTypes ReminderType { get; set; }

		[XmlElement("ReminderGroupType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[DataMember]
		public ReminderGroupType? ReminderGroupType { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new GetReminders(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return null;
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return null;
		}
	}
}
