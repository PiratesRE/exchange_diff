using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("AttachmentInfoResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class AttachmentInfoResponseMessage : ResponseMessage
	{
		public AttachmentInfoResponseMessage()
		{
		}

		internal AttachmentInfoResponseMessage(ServiceResultCode code, ServiceError error, AttachmentType attachment) : base(code, error)
		{
			this.attachments = new List<AttachmentType>();
			this.attachments.Add(attachment);
		}

		internal AttachmentInfoResponseMessage(int itemIndex, ServiceCommandBase serviceCommand)
		{
			this.itemIndex = itemIndex;
			this.serviceCommand = serviceCommand;
		}

		[XmlArray("Attachments", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[DataMember(EmitDefaultValue = false, Name = "Attachments")]
		[XmlArrayItem(ElementName = "ItemAttachment", Type = typeof(ItemAttachmentType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[XmlArrayItem(ElementName = "FileAttachment", Type = typeof(FileAttachmentType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[XmlArrayItem(ElementName = "ReferenceAttachment", Type = typeof(ReferenceAttachmentType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public AttachmentType[] Attachments
		{
			get
			{
				base.ExecuteServiceCommandIfRequired();
				return this.attachments.ToArray();
			}
			set
			{
				this.attachments = new List<AttachmentType>(value);
			}
		}

		[XmlIgnore]
		[DataMember(EmitDefaultValue = false)]
		public bool IsCancelled
		{
			get
			{
				return this.isCancelled;
			}
			set
			{
				this.isCancelled = value;
			}
		}

		protected override void InternalExecuteServiceCommand()
		{
			if (this.serviceCommand != null)
			{
				ServiceResult<AttachmentType> serviceResult = this.CheckBatchProcessingErrorAndExecute();
				this.PopulateServiceResult(serviceResult);
			}
		}

		private ServiceResult<AttachmentType> CheckBatchProcessingErrorAndExecute()
		{
			GetAttachment getAttachment = (GetAttachment)this.serviceCommand;
			bool flag = getAttachment.IsPreviousResultStopBatchProcessingError(this.itemIndex);
			ServiceResult<AttachmentType> result;
			if (flag)
			{
				result = new ServiceResult<AttachmentType>(ServiceResultCode.Warning, null, ServiceError.CreateBatchProcessingStoppedError());
			}
			else
			{
				result = getAttachment.GetAttachmentResult(this.itemIndex);
			}
			return result;
		}

		private void PopulateServiceResult(ServiceResult<AttachmentType> serviceResult)
		{
			base.Initialize(serviceResult.Code, serviceResult.Error);
			this.attachments = new List<AttachmentType>();
			this.attachments.Add(serviceResult.Value);
		}

		private int itemIndex;

		private bool isCancelled;

		private ServiceCommandBase serviceCommand;

		private List<AttachmentType> attachments;
	}
}
