using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[KnownType(typeof(ItemIdAttachmentType))]
	[KnownType(typeof(ItemAttachmentType))]
	[KnownType(typeof(ReferenceAttachmentType))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[KnownType(typeof(FileAttachmentType))]
	[XmlType("CreateAttachmentType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class CreateAttachmentRequest : BaseRequest
	{
		[DataMember(EmitDefaultValue = false)]
		[XmlIgnore]
		public bool RequireImageType { get; set; }

		[DataMember(EmitDefaultValue = false)]
		[XmlIgnore]
		public bool IncludeContentIdInResponse { get; set; }

		[XmlElement("ParentItemId")]
		[DataMember(Name = "ParentItemId", IsRequired = true, Order = 1)]
		public ItemId ParentItemId { get; set; }

		[XmlArrayItem("ItemAttachment", typeof(ItemAttachmentType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArrayItem("FileAttachment", typeof(FileAttachmentType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[DataMember(EmitDefaultValue = false)]
		public AttachmentType[] Attachments { get; set; }

		[DataMember(EmitDefaultValue = false)]
		[XmlIgnore]
		public bool ClientSupportsIrm { get; set; }

		[XmlIgnore]
		[DataMember(EmitDefaultValue = false)]
		public string CancellationId { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new CreateAttachment(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return BaseRequest.GetServerInfoForItemId(callContext, this.ParentItemId);
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int currentStep)
		{
			if (this.parentResourceKey == null)
			{
				this.parentResourceKey = base.GetResourceKeysForItemId(true, callContext, this.ParentItemId);
			}
			return this.parentResourceKey;
		}

		internal const string ElementName = "CreateAttachment";

		internal const string ParentItemIdElementName = "ParentItemId";

		private ResourceKey[] parentResourceKey;
	}
}
