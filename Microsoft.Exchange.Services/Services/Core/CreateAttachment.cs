using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class CreateAttachment : MultiStepServiceCommand<CreateAttachmentRequest, AttachmentType>, IDisposeTrackable, IDisposable
	{
		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<CreateAttachment>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public CreateAttachment(CallContext callContext, CreateAttachmentRequest request) : base(callContext, request)
		{
			ExTraceGlobals.CreateItemCallTracer.TraceDebug((long)this.GetHashCode(), "CreateAttachment.Execute called");
			this.parentItemId = request.ParentItemId;
			this.attachmentTypes = request.Attachments;
			ServiceCommandBase.ThrowIfNull(this.parentItemId, "parentItemId", "CreateAttachment::ctor");
			ServiceCommandBase.ThrowIfNullOrEmpty<AttachmentType>(this.attachmentTypes, "attachment", "CreateAttachment::ctor");
			this.disposeTracker = this.GetDisposeTracker();
		}

		public void Dispose()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
			}
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			CreateAttachmentResponse createAttachmentResponse = new CreateAttachmentResponse();
			createAttachmentResponse.AddResponses(base.Results);
			return createAttachmentResponse;
		}

		internal override void PreExecuteCommand()
		{
			this.parentIdAndSession = base.IdConverter.ConvertItemIdToIdAndSessionReadOnly(this.parentItemId, BasicTypes.ItemOrAttachment);
			this.attachments = new AttachmentHierarchy(this.parentIdAndSession, true, base.Request.ClientSupportsIrm);
			this.builder = new AttachmentBuilder(this.attachments, this.attachmentTypes, base.IdConverter, base.Request.ClientSupportsIrm);
		}

		internal override void PostExecuteCommand()
		{
			this.attachments.SaveAll();
			this.attachments.RootItem.Load();
			foreach (ServiceResult<AttachmentType> serviceResult in base.Results)
			{
				if (serviceResult.Value != null)
				{
					ConcatenatedIdAndChangeKey concatenatedId = IdConverter.GetConcatenatedId(this.attachments.RootItem.Id, this.parentIdAndSession, null);
					serviceResult.Value.AttachmentId.RootItemId = concatenatedId.Id;
					serviceResult.Value.AttachmentId.RootItemChangeKey = concatenatedId.ChangeKey;
				}
			}
		}

		internal override int StepCount
		{
			get
			{
				return this.attachmentTypes.Length;
			}
		}

		internal override ServiceResult<AttachmentType> Execute()
		{
			AttachmentType attachmentType = this.attachmentTypes[base.CurrentStep];
			if (attachmentType is ReferenceAttachmentType && !(ExchangeVersion.Current > ExchangeVersion.ExchangeV2_4))
			{
				throw new ServiceArgumentException((CoreResources.IDs)3784063568U);
			}
			if (base.Request.RequireImageType)
			{
				if (!CreateAttachment.IsInlineImage(attachmentType.ContentType))
				{
					throw new ServiceInvalidOperationException((CoreResources.IDs)4077357270U);
				}
				this.GenerateContentId(attachmentType);
			}
			ServiceError serviceError = null;
			AttachmentType value = null;
			using (Attachment attachment = this.builder.CreateAttachment(attachmentType, out serviceError))
			{
				value = this.CreateAttachmentResult(attachment, attachmentType);
			}
			if (serviceError == null)
			{
				return new ServiceResult<AttachmentType>(value);
			}
			return new ServiceResult<AttachmentType>(value, serviceError);
		}

		private static bool IsInlineImage(string contentType)
		{
			return contentType != null && CreateAttachment.inlineImageContentTypes.Contains(contentType.ToLowerInvariant());
		}

		private void GenerateContentId(AttachmentType attachmentType)
		{
			attachmentType.ContentId = Guid.NewGuid().ToString();
		}

		private AttachmentType CreateAttachmentResult(Attachment attachment, AttachmentType attachmentType)
		{
			AttachmentType attachmentType2;
			if (attachment is StreamAttachment)
			{
				attachmentType2 = new FileAttachmentType();
			}
			else if (attachment is ReferenceAttachment)
			{
				attachmentType2 = new ReferenceAttachmentType();
			}
			else
			{
				attachmentType2 = new ItemAttachmentType();
			}
			IdAndSession idAndSession = this.parentIdAndSession.Clone();
			attachment.Load();
			idAndSession.AttachmentIds.Add(attachment.Id);
			attachmentType2.AttachmentId = new AttachmentIdType(idAndSession.GetConcatenatedId().Id);
			if (base.Request.IncludeContentIdInResponse)
			{
				attachmentType2.ContentId = attachment.ContentId;
			}
			if (attachmentType is ItemIdAttachmentType)
			{
				attachmentType2.Size = ((attachment.Size > 2147483647L) ? int.MaxValue : ((int)attachment.Size));
			}
			return attachmentType2;
		}

		private void Dispose(bool fromDispose)
		{
			if (this.builder != null)
			{
				this.builder.Dispose();
				this.builder = null;
			}
		}

		private readonly DisposeTracker disposeTracker;

		private static HashSet<string> inlineImageContentTypes = new HashSet<string>
		{
			"image/jpeg",
			"image/pjpeg",
			"image/gif",
			"image/bmp",
			"image/png",
			"image/x-png"
		};

		private ItemId parentItemId;

		private AttachmentType[] attachmentTypes;

		private IdAndSession parentIdAndSession;

		private AttachmentHierarchy attachments;

		private AttachmentBuilder builder;
	}
}
