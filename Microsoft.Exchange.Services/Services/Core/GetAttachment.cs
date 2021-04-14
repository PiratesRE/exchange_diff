using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class GetAttachment : MultiStepServiceCommand<GetAttachmentRequest, AttachmentInfoResponseMessage>, IDisposable
	{
		public GetAttachment(CallContext callContext, GetAttachmentRequest request) : base(callContext, request)
		{
			this.attachmentIds = base.Request.AttachmentIds;
			this.responseShape = Global.ResponseShapeResolver.GetResponseShape<AttachmentResponseShape>(base.Request.ShapeName, base.Request.AttachmentShape, null);
			ServiceCommandBase.ThrowIfNullOrEmpty<AttachmentIdType>(this.attachmentIds, "attachmentIds", "GetAttachment::ctor");
			ServiceCommandBase.ThrowIfNull(this.responseShape, "responseShape", "GetAttachment::ctor");
			this.attachmentResponse = new GetAttachmentResponse();
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		internal bool IsPreviousResultStopBatchProcessingError(int currentIndex)
		{
			if (!this.isPreviousResultStopBatchProcessingError && currentIndex > 0)
			{
				ArrayOfResponseMessages responseMessages = this.attachmentResponse.ResponseMessages;
				ResponseMessage responseMessage = (ResponseMessage)responseMessages.Items.GetValue(currentIndex - 1);
				this.isPreviousResultStopBatchProcessingError = responseMessage.StopsBatchProcessing;
			}
			return this.isPreviousResultStopBatchProcessingError;
		}

		internal override ServiceResult<AttachmentInfoResponseMessage> Execute()
		{
			return new ServiceResult<AttachmentInfoResponseMessage>(new AttachmentInfoResponseMessage(base.CurrentStep, this));
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			this.attachmentResponse.BuildForGetAttachmentResults(base.Results);
			return this.attachmentResponse;
		}

		internal override int StepCount
		{
			get
			{
				return this.attachmentIds.Length;
			}
		}

		internal ServiceResult<AttachmentType> GetAttachmentResult(int itemIndex)
		{
			ServiceResult<AttachmentType> serviceResult = ExceptionHandler<AttachmentType>.Execute(new ExceptionHandler<AttachmentType>.CreateServiceResult(this.GetAttachmentFromRequest), itemIndex);
			base.LogServiceResultErrorAsAppropriate(serviceResult.Code, serviceResult.Error);
			return serviceResult;
		}

		private ServiceResult<AttachmentType> GetAttachmentFromRequest(int itemIndex)
		{
			AttachmentIdType attachmentIdType = this.attachmentIds[itemIndex];
			IdAndSession idAndSession = base.IdConverter.ConvertAttachmentIdToIdAndSessionReadOnly(attachmentIdType);
			AttachmentHierarchy attachmentHierarchy = null;
			bool flag = false;
			ServiceResult<AttachmentType> result = null;
			try
			{
				attachmentHierarchy = new AttachmentHierarchy(idAndSession, false, this.responseShape.ClientSupportsIrm);
				base.CallContext.AuthZBehavior.OnGetAttachment(attachmentHierarchy.RootItem.StoreObjectId);
				AttachmentHierarchyItem last = attachmentHierarchy.Last;
				AttachmentType attachmentType = this.CreateAttachmentType(attachmentIdType.Id, last);
				this.SuperSizeCheck(last.Attachment);
				object item;
				if (last.IsItemAttachment)
				{
					(attachmentType as ItemAttachmentType).Item = this.SerializeItemAttachment(idAndSession, last.XsoItem);
					item = attachmentHierarchy;
				}
				else
				{
					StreamAttachmentBase streamAttachmentBase = last.Attachment as StreamAttachmentBase;
					Stream contentStream;
					try
					{
						if (streamAttachmentBase is OleAttachment)
						{
							OleAttachment oleAttachment = (OleAttachment)streamAttachmentBase;
							contentStream = oleAttachment.ConvertToImage(ImageFormat.Jpeg);
						}
						else
						{
							contentStream = streamAttachmentBase.GetContentStream();
						}
					}
					catch (StoragePermanentException innerException)
					{
						throw new CannotOpenFileAttachmentException(innerException);
					}
					catch (StorageTransientException innerException2)
					{
						throw new CannotOpenFileAttachmentException(innerException2);
					}
					GetAttachment.AttachmentStreamWrapper attachmentStreamWrapper = new GetAttachment.AttachmentStreamWrapper(attachmentHierarchy, contentStream);
					item = attachmentStreamWrapper;
					(attachmentType as FileAttachmentType).ContentStream = contentStream;
				}
				result = new ServiceResult<AttachmentType>(attachmentType);
				this.requestItems.Add(item);
				flag = true;
			}
			finally
			{
				if (!flag && attachmentHierarchy != null)
				{
					attachmentHierarchy.Dispose();
				}
			}
			return result;
		}

		public AttachmentType CreateAttachmentType(string attachmentId, AttachmentHierarchyItem attachment)
		{
			ServiceCommandBase.ThrowIfNullOrEmpty(attachmentId, "attachmentId", "GetAttachment:CreateAttachmentXmlNode");
			AttachmentType attachmentType;
			if (attachment.IsItemAttachment)
			{
				attachmentType = new ItemAttachmentType();
			}
			else
			{
				attachmentType = new FileAttachmentType();
			}
			attachmentType.AttachmentId = new AttachmentIdType(attachmentId);
			string text = attachment.Attachment.DisplayName;
			if (string.IsNullOrEmpty(text))
			{
				text = attachment.Attachment.FileName;
			}
			attachmentType.Name = text;
			if (!string.IsNullOrEmpty(attachment.Attachment.ContentType))
			{
				attachmentType.ContentType = attachment.Attachment.ContentType;
			}
			else
			{
				attachmentType.ContentType = attachment.Attachment.CalculatedContentType;
			}
			if (!string.IsNullOrEmpty(attachment.Attachment.ContentId))
			{
				attachmentType.ContentId = attachment.Attachment.ContentId;
			}
			if (attachment.Attachment.ContentLocation != null)
			{
				attachmentType.ContentLocation = attachment.Attachment.ContentLocation.ToString();
			}
			if (attachment.Attachment.IsInline)
			{
				attachmentType.IsInline = attachment.Attachment.IsInline;
			}
			return attachmentType;
		}

		private ItemType SerializeItemAttachment(IdAndSession idAndSession, Item xsoItem)
		{
			if (IrmUtils.IsIrmEnabled(this.responseShape.ClientSupportsIrm, idAndSession.Session))
			{
				RightsManagedMessageItem rightsManagedMessageItem = xsoItem as RightsManagedMessageItem;
				if (rightsManagedMessageItem != null)
				{
					IrmUtils.DecryptForGetAttachment(idAndSession.Session, rightsManagedMessageItem);
				}
			}
			ToServiceObjectPropertyList toServiceObjectPropertyList = XsoDataConverter.GetToServiceObjectPropertyList(xsoItem, this.responseShape);
			xsoItem.Load(toServiceObjectPropertyList.GetPropertyDefinitions());
			toServiceObjectPropertyList.CharBuffer = this.CharBuffer;
			StoreObjectType objectType = ObjectClass.GetObjectType(xsoItem.ClassName);
			ItemType itemType = ItemType.CreateFromStoreObjectType(objectType);
			ServiceCommandBase.LoadServiceObject(itemType, xsoItem, idAndSession, this.responseShape, toServiceObjectPropertyList);
			return itemType;
		}

		private void SuperSizeCheck(Attachment attachment)
		{
			long size = attachment.Size;
			if (size > (long)Global.GetAttachmentSizeLimit)
			{
				throw new MessageTooBigException(CoreResources.ErrorMessageSizeExceeded, null);
			}
		}

		private void Dispose(bool isDisposing)
		{
			ExTraceGlobals.ServiceCommandBaseCallTracer.TraceDebug<int, bool, bool>((long)this.GetHashCode(), "[GetAttachment:Dispose(bool)] Hashcode: {0}. IsDisposing: {1}, Already Disposed: {2}", this.GetHashCode(), isDisposing, this.isDisposed);
			if (!this.isDisposed)
			{
				if (isDisposing)
				{
					this.attachmentResponse = null;
					if (this.requestItems != null)
					{
						foreach (object obj in this.requestItems)
						{
							IDisposable disposable = obj as IDisposable;
							if (disposable != null)
							{
								disposable.Dispose();
							}
						}
						this.requestItems.Clear();
						this.requestItems = null;
						this.CharBuffer = null;
					}
				}
				this.isDisposed = true;
			}
		}

		private AttachmentIdType[] attachmentIds;

		private AttachmentResponseShape responseShape;

		private bool isDisposed;

		internal char[] CharBuffer = new char[32768];

		private List<object> requestItems = new List<object>();

		private GetAttachmentResponse attachmentResponse;

		private bool isPreviousResultStopBatchProcessingError;

		private class AttachmentStreamWrapper : IDisposable
		{
			public AttachmentStreamWrapper(AttachmentHierarchy attachmentHierarchy, Stream contentStream)
			{
				this.attachmentHierarchy = attachmentHierarchy;
				this.contentStream = contentStream;
			}

			public Stream ContentStream
			{
				get
				{
					return this.contentStream;
				}
			}

			public void Dispose()
			{
				if (!this.isDisposed)
				{
					if (this.contentStream != null)
					{
						this.contentStream.Dispose();
						this.contentStream = null;
					}
					if (this.attachmentHierarchy != null)
					{
						this.attachmentHierarchy.Dispose();
						this.attachmentHierarchy = null;
					}
					this.isDisposed = true;
				}
			}

			private AttachmentHierarchy attachmentHierarchy;

			private Stream contentStream;

			private bool isDisposed;
		}
	}
}
