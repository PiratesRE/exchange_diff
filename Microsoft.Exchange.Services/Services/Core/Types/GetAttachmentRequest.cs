using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("GetAttachmentType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class GetAttachmentRequest : BaseRequest
	{
		public GetAttachmentRequest()
		{
			this.Initialize();
		}

		private void Initialize()
		{
			this.responseShape = new AttachmentResponseShape();
		}

		[XmlElement(ElementName = "AttachmentShape")]
		[DataMember]
		public AttachmentResponseShape AttachmentShape
		{
			get
			{
				return this.responseShape;
			}
			set
			{
				if (value != null)
				{
					this.responseShape = value;
				}
			}
		}

		[DataMember(Name = "ShapeName", IsRequired = false)]
		[XmlIgnore]
		public string ShapeName { get; set; }

		[DataMember(Name = "AttachmentIds")]
		[XmlArray("AttachmentIds", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[XmlArrayItem(ElementName = "AttachmentId", Type = typeof(AttachmentIdType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public AttachmentIdType[] AttachmentIds { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new GetAttachment(callContext, this);
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
			return base.GetResourceKeysForAttachmentId(false, callContext, this.AttachmentIds[taskStep]);
		}

		protected override List<ServiceObjectId> GetAllIds()
		{
			if (this.AttachmentIds == null)
			{
				return null;
			}
			return new List<ServiceObjectId>(this.AttachmentIds);
		}

		[OnDeserializing]
		public void Initialize(StreamingContext context)
		{
			this.Initialize();
		}

		internal const string ElementName = "GetAttachment";

		internal const string AttachmentIdsElementName = "AttachmentIds";

		private AttachmentResponseShape responseShape;
	}
}
