using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "ConvertIdType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[KnownType(typeof(AlternatePublicFolderId))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[KnownType(typeof(AlternatePublicFolderItemId))]
	[KnownType(typeof(AlternateId))]
	[Serializable]
	public class ConvertIdRequest : BaseRequest
	{
		[IgnoreDataMember]
		[XmlAttribute]
		public IdFormat DestinationFormat
		{
			get
			{
				return this.destinationFormat;
			}
			set
			{
				this.destinationFormat = value;
			}
		}

		[DataMember(Name = "DestinationFormat", IsRequired = true, Order = 1)]
		[XmlIgnore]
		public string DestinationFormatString
		{
			get
			{
				return this.destinationFormat.ToString();
			}
			set
			{
				this.destinationFormat = (IdFormat)Enum.Parse(typeof(IdFormat), value);
			}
		}

		[XmlArrayItem("AlternateId", typeof(AlternateId), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArrayItem("AlternatePublicFolderId", typeof(AlternatePublicFolderId), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArrayItem("AlternatePublicFolderItemId", typeof(AlternatePublicFolderItemId), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[DataMember(IsRequired = true, Order = 2)]
		public AlternateIdBase[] SourceIds
		{
			get
			{
				return this.sourceIds;
			}
			set
			{
				this.sourceIds = value;
			}
		}

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new ConvertId(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return null;
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int currentStep)
		{
			return null;
		}

		private IdFormat destinationFormat;

		private AlternateIdBase[] sourceIds;
	}
}
