using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("DeleteAttachmentType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class DeleteAttachmentRequest : BaseRequest
	{
		[XmlArrayItem("AttachmentId", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[DataMember(EmitDefaultValue = false)]
		public AttachmentIdType[] AttachmentIds { get; set; }

		[XmlIgnore]
		[DataMember(EmitDefaultValue = false)]
		public bool ClientSupportsIrm { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new DeleteAttachment(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			if (this.AttachmentIds == null)
			{
				return null;
			}
			return BaseRequest.GetServerInfoForAttachmentIdList(callContext, this.AttachmentIds);
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			if (this.AttachmentIds == null || this.AttachmentIds.Length < taskStep)
			{
				return null;
			}
			return base.GetResourceKeysForAttachmentId(true, callContext, this.AttachmentIds[taskStep]);
		}

		protected override List<ServiceObjectId> GetAllIds()
		{
			if (this.AttachmentIds == null)
			{
				return null;
			}
			return new List<ServiceObjectId>(this.AttachmentIds);
		}

		internal const string ElementName = "DeleteAttachment";

		internal const string AttachmentsElementName = "AttachmentIds";
	}
}
