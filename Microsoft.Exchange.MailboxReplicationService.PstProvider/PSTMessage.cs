using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.PST;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.FastTransfer;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class PSTMessage : IMessage, IDisposable
	{
		public PSTMessage(PstMailbox pstMailbox, IMessage iPstMessage) : this(pstMailbox, iPstMessage, false)
		{
		}

		public PSTMessage(PstMailbox pstMailbox, IMessage iPstMessage, bool isEmbedded)
		{
			if (pstMailbox.IPst == null || iPstMessage == null)
			{
				throw new ArgumentNullException((pstMailbox.IPst == null) ? "iPst" : "iPstMessage");
			}
			this.pstMailbox = pstMailbox;
			this.iPstMessage = iPstMessage;
			this.isEmbedded = isEmbedded;
			this.propertyBag = new PSTPropertyBag(this.pstMailbox, iPstMessage.PropertyBag);
			this.recipients = new List<IRecipient>(0);
			this.attachments = new List<IAttachment>(0);
		}

		public PstMailbox PstMailbox
		{
			get
			{
				return this.pstMailbox;
			}
		}

		public IMessage IPstMessage
		{
			get
			{
				return this.iPstMessage;
			}
		}

		public PSTPropertyBag RawPropertyBag
		{
			get
			{
				return this.propertyBag;
			}
		}

		public IPropertyBag PropertyBag
		{
			get
			{
				return this.propertyBag;
			}
		}

		public bool IsAssociated
		{
			get
			{
				return this.iPstMessage.IsAssociated;
			}
		}

		public bool IsEmbedded
		{
			get
			{
				return this.isEmbedded;
			}
		}

		public List<IRecipient> Recipients
		{
			get
			{
				return this.recipients;
			}
		}

		public List<IAttachment> Attachments
		{
			get
			{
				return this.attachments;
			}
		}

		public IEnumerable<IRecipient> GetRecipients()
		{
			if (this.recipients.Count != this.iPstMessage.RecipientIds.Count)
			{
				foreach (uint num in this.iPstMessage.RecipientIds)
				{
					this.recipients.Add(new PSTRecipient(this.recipients.Count, new PSTPropertyBag(this.pstMailbox, this.iPstMessage.ReadRecipient(num))));
				}
			}
			return this.recipients.Cast<IRecipient>();
		}

		public IRecipient CreateRecipient()
		{
			IPropertyBag iPstPropertyBag = this.iPstMessage.AddRecipient();
			PSTRecipient pstrecipient = new PSTRecipient(this.recipients.Count, new PSTPropertyBag(this.pstMailbox, iPstPropertyBag));
			this.recipients.Add(pstrecipient);
			return pstrecipient;
		}

		public void RemoveRecipient(int rowId)
		{
			throw new NotImplementedException();
		}

		public void Save()
		{
			this.iPstMessage.Save();
		}

		public void SetLongTermId(StoreLongTermId longTermId)
		{
			throw new NotSupportedException();
		}

		public void Dispose()
		{
		}

		public IAttachment CreateAttachment()
		{
			IAttachment iPstAttachment = this.iPstMessage.AddAttachment();
			PSTAttachment pstattachment = new PSTAttachment(this, iPstAttachment);
			this.attachments.Add(pstattachment);
			return pstattachment;
		}

		public IEnumerable<IAttachmentHandle> GetAttachments()
		{
			if (this.attachments.Count != this.iPstMessage.AttachmentIds.Count)
			{
				foreach (uint num in this.iPstMessage.AttachmentIds)
				{
					IAttachment iPstAttachment = this.iPstMessage.ReadAttachment(num);
					this.attachments.Add(new PSTAttachment(this, iPstAttachment));
				}
			}
			for (int i = 0; i < this.attachments.Count; i++)
			{
				yield return new PSTMessage.PSTAttachmentHandle(this.attachments, i);
			}
			yield break;
		}

		private PstMailbox pstMailbox;

		private IMessage iPstMessage;

		private PSTPropertyBag propertyBag;

		private List<IRecipient> recipients;

		private List<IAttachment> attachments;

		private bool isEmbedded;

		private class PSTAttachmentHandle : IAttachmentHandle
		{
			public PSTAttachmentHandle(IList<IAttachment> attachmentList, int attachmentIndex)
			{
				this.attachmentList = attachmentList;
				this.attachmentIndex = attachmentIndex;
			}

			public IAttachment GetAttachment()
			{
				return this.attachmentList[this.attachmentIndex];
			}

			private readonly IList<IAttachment> attachmentList;

			private readonly int attachmentIndex;
		}
	}
}
