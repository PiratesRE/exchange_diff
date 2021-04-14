using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.FastTransfer;
using Microsoft.Exchange.Protocols.MAPI;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.FastTransfer;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.FastTransfer
{
	internal class FastTransferMessage : FastTransferPropertyBag, IMessage, IDisposable
	{
		public FastTransferMessage(FastTransferDownloadContext downloadContext, MapiMessage mapiMessage, bool excludeProps, HashSet<StorePropTag> propList, bool excludeAttachments, bool excludeRecipients, bool topLevel, FastTransferCopyFlag flags) : base(downloadContext, mapiMessage, excludeProps, propList)
		{
			this.excludeAttachments = excludeAttachments;
			this.excludeRecipients = excludeRecipients;
			this.topLevel = topLevel;
			this.flags = flags;
		}

		public FastTransferMessage(FastTransferUploadContext uploadContext, MapiMessage mapiMessage, bool topLevel, FastTransferCopyFlag flags) : base(uploadContext, mapiMessage)
		{
			this.topLevel = topLevel;
			this.flags = flags;
		}

		internal MapiMessage MapiMessage
		{
			get
			{
				return (MapiMessage)base.MapiPropBag;
			}
			set
			{
				base.MapiPropBag = value;
			}
		}

		public bool IsAssociated
		{
			get
			{
				AnnotatedPropertyValue annotatedProperty = this.PropertyBag.GetAnnotatedProperty(PropertyTag.MessageFlags);
				return !annotatedProperty.PropertyValue.IsError && ((int)annotatedProperty.PropertyValue.Value & 64) == 64;
			}
		}

		public IAttachment CreateAttachment()
		{
			MapiAttachment mapiAttachment;
			ErrorCode errorCode = this.MapiMessage.CreateAttachment(base.Context.CurrentOperationContext, out mapiAttachment);
			if (errorCode != ErrorCode.NoError)
			{
				throw new StoreException((LID)50232U, errorCode);
			}
			if (ExTraceGlobals.SourceSendTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				stringBuilder.Append("Receive Attachment=[Number = [");
				stringBuilder.Append(mapiAttachment.StoreAttachment.AttachmentNumber);
				stringBuilder.Append("]]");
				ExTraceGlobals.SourceSendTracer.TraceDebug(0L, stringBuilder.ToString());
			}
			return new FastTransferAttachment(base.UploadContext, mapiAttachment, false, this.flags);
		}

		public IRecipient CreateRecipient()
		{
			MapiPersonCollection recipients = this.MapiMessage.GetRecipients();
			MapiPerson item = recipients.GetItem(recipients.GetCount(), true);
			if (this.recipientWrapper == null)
			{
				this.recipientWrapper = new FastTransferRecipient(base.UploadContext, item);
			}
			else
			{
				this.recipientWrapper.Reinitialize(item);
			}
			return this.recipientWrapper;
		}

		public void RemoveRecipient(int rowId)
		{
			throw new ExExceptionNoSupport((LID)54748U, "Recipient removal is not supported on the FastTransferMessage");
		}

		public IEnumerable<IAttachmentHandle> GetAttachments()
		{
			if (!this.excludeAttachments)
			{
				IEnumerable<int> attachNumbers = this.MapiMessage.GetAttachmentNumbers();
				if (attachNumbers != null)
				{
					foreach (int attachmentNumber in attachNumbers)
					{
						MapiAttachment attachment;
						ErrorCode error = this.MapiMessage.OpenAttachment(base.Context.CurrentOperationContext, attachmentNumber, AttachmentConfigureFlags.RequestReadOnly, out attachment);
						if (error != ErrorCode.NoError)
						{
							throw new StoreException((LID)47160U, error);
						}
						if (ExTraceGlobals.SourceSendTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							StringBuilder stringBuilder = new StringBuilder(100);
							stringBuilder.Append("Send Attachment=[Number = [");
							StringBuilder stringBuilder2 = stringBuilder;
							int num = attachmentNumber;
							stringBuilder2.Append(num.ToString());
							stringBuilder.Append("], Embedded=[");
							stringBuilder.Append(attachment != null && attachment.IsEmbeddedMessage(base.Context.CurrentOperationContext));
							stringBuilder.Append("]]");
							ExTraceGlobals.SourceSendTracer.TraceDebug(0L, stringBuilder.ToString());
						}
						yield return new FastTransferAttachment(base.DownloadContext, attachment, true, null, false, this.flags);
					}
				}
			}
			yield break;
		}

		public IEnumerable<IRecipient> GetRecipients()
		{
			if (!this.excludeRecipients)
			{
				MapiPersonCollection mapiRecipients = this.MapiMessage.GetRecipients();
				foreach (MapiPerson mapiRecipient in ((IEnumerable<MapiPerson>)mapiRecipients))
				{
					if (this.recipientWrapper == null)
					{
						this.recipientWrapper = new FastTransferRecipient(base.DownloadContext, mapiRecipient);
					}
					else
					{
						this.recipientWrapper.Reinitialize(mapiRecipient);
					}
					yield return this.recipientWrapper;
				}
			}
			yield break;
		}

		public IPropertyBag PropertyBag
		{
			get
			{
				return this;
			}
		}

		public void Save()
		{
			ExchangeId exchangeId;
			this.MapiMessage.SaveChanges(base.Context.CurrentOperationContext, MapiSaveMessageChangesFlags.SkipQuotaCheck, out exchangeId);
		}

		public void SetLongTermId(StoreLongTermId longTermId)
		{
			ExchangeId exchangeId = ExchangeId.Create(base.Context.CurrentOperationContext, this.MapiMessage.Logon.StoreMailbox.ReplidGuidMap, longTermId.Guid, ExchangeIdHelpers.GlobcntFromByteArray(longTermId.GlobCount, 0U));
			ExchangeId fid = this.MapiMessage.GetFid();
			if (fid.IsValid)
			{
				MapiMessage mapiMessage = new MapiMessage();
				try
				{
					ErrorCode errorCode = mapiMessage.ConfigureMessage(base.Context.CurrentOperationContext, base.Context.Logon, fid, exchangeId, MessageConfigureFlags.RequestWrite, base.Context.Logon.CodePage);
					if (errorCode == ErrorCode.NoError)
					{
						mapiMessage.Scrub(base.Context.CurrentOperationContext);
						MapiMessage mapiMessage2 = this.MapiMessage;
						this.MapiMessage = mapiMessage;
						mapiMessage = mapiMessage2;
					}
					else if (errorCode != ErrorCodeValue.NotFound)
					{
						throw new StoreException((LID)35640U, errorCode, "Failed to open existing message");
					}
				}
				finally
				{
					mapiMessage.Dispose();
				}
			}
			this.MapiMessage.SetMessageId(base.Context.CurrentOperationContext, exchangeId);
		}

		protected override List<Property> LoadAllPropertiesImp()
		{
			List<Property> list = base.LoadAllPropertiesImp();
			if (this.MapiMessage != null)
			{
				this.MapiMessage.InjectFailureIfNeeded(base.Context.CurrentOperationContext, false);
			}
			if ((this.flags & FastTransferCopyFlag.SendEntryId) != FastTransferCopyFlag.None)
			{
				FastTransferPropertyBag.AddNullPropertyIfNotPresent(list, PropTag.Message.EntryId);
			}
			if (base.ForMoveUser || (this.flags & FastTransferCopyFlag.SendEntryId) != FastTransferCopyFlag.None)
			{
				FastTransferPropertyBag.AddNullPropertyIfNotPresent(list, PropTag.Message.SourceKey);
				FastTransferPropertyBag.AddNullPropertyIfNotPresent(list, PropTag.Message.ChangeKey);
				FastTransferPropertyBag.AddNullPropertyIfNotPresent(list, PropTag.Message.LastModificationTime);
				FastTransferPropertyBag.AddNullPropertyIfNotPresent(list, PropTag.Message.PredecessorChangeList);
			}
			if (base.ForMoveUser)
			{
				FastTransferPropertyBag.AddNullPropertyIfNotPresent(list, PropTag.Message.CnExport);
				FastTransferPropertyBag.AddNullPropertyIfNotPresent(list, PropTag.Message.PclExport);
				FastTransferPropertyBag.AddNullPropertyIfNotPresent(list, PropTag.Message.CnMvExport);
				FastTransferPropertyBag.AddNullPropertyIfNotPresent(list, PropTag.Message.MsgStatus);
				FastTransferPropertyBag.AddNullPropertyIfNotPresent(list, PropTag.Message.IMAPId);
				FastTransferPropertyBag.AddNullPropertyIfNotPresent(list, PropTag.Message.InternetArticleNumber);
				FastTransferPropertyBag.AddNullPropertyIfNotPresent(list, PropTag.Message.MailFlags);
				FastTransferPropertyBag.AddNullPropertyIfNotPresent(list, PropTag.Message.SharedReceiptHandling);
				FastTransferPropertyBag.AddNullPropertyIfNotPresent(list, PropTag.Message.ReadCnNewExport);
				FastTransferPropertyBag.AddNullPropertyIfNotPresent(list, PropTag.Message.AnnotationToken);
				FastTransferPropertyBag.ResetPropertyIfPresent(list, PropTag.Message.LTID);
				FastTransferPropertyBag.ResetPropertyIfPresent(list, PropTag.Message.Inid);
			}
			FastTransferPropertyBag.ResetPropertyIfPresent(list, PropTag.Message.MessageFlags);
			ValueHelper.SortAndRemoveDuplicates<Property>(list, PropertyComparerByTag.Comparer);
			return list;
		}

		protected override Property GetPropertyImp(StorePropTag propTag)
		{
			if (base.ForMoveUser)
			{
				if (propTag == PropTag.Message.LTID)
				{
					ExchangeId exchangeId = this.MapiMessage.Mid;
					if (base.Context.OtherSideVersion < Microsoft.Exchange.Protocols.MAPI.Version.Exchange15MinVersion && exchangeId.IsNullOrZero)
					{
						exchangeId = this.MapiMessage.Logon.StoreMailbox.GetNextObjectId(base.Context.CurrentOperationContext);
					}
					return new Property(PropTag.Message.LTID, exchangeId.To24ByteArray());
				}
				if (propTag == PropTag.Message.Inid)
				{
					return new Property(PropTag.Message.LTID, this.MapiMessage.Logon.StoreMailbox.GetNextObjectId(base.Context.CurrentOperationContext).To24ByteArray());
				}
			}
			return base.GetPropertyImp(propTag);
		}

		protected override void SetPropertyImp(Property property)
		{
			if (property.Tag.IsCategory(13) && (!base.Context.Logon.IsMoveUser || base.Context.OtherSideVersion.BuildMinor < 6000))
			{
				DiagnosticContext.TraceLocation((LID)54187U);
				throw new StoreException((LID)52024U, ErrorCodeValue.NotSupported, "CAI properties are not supported");
			}
			base.SetPropertyImp(property);
		}

		protected override bool IncludeTag(StorePropTag propTag)
		{
			if (base.ForMoveUser && propTag.IsCategory(4))
			{
				return true;
			}
			if (!base.ForMoveUser && !base.ForUpload)
			{
				if (propTag.PropId == 3648 || propTag.PropId == 3649)
				{
					return false;
				}
				if (26112 <= propTag.PropId && propTag.PropId <= 26623)
				{
					return propTag == PropTag.Message.Mid || propTag == PropTag.Message.SentMailSvrEID;
				}
			}
			ushort propId = propTag.PropId;
			if (propId <= 4290)
			{
				if (propId <= 3631)
				{
					if (propId <= 3611)
					{
						switch (propId)
						{
						case 3586:
						case 3587:
						case 3588:
						case 3592:
						case 3593:
							return false;
						case 3589:
						case 3590:
						case 3591:
							goto IL_2CF;
						default:
							if (propId != 3611)
							{
								goto IL_2CF;
							}
							return false;
						}
					}
					else if (propId != 3615)
					{
						if (propId != 3631)
						{
							goto IL_2CF;
						}
						goto IL_2CF;
					}
				}
				else if (propId <= 4105)
				{
					switch (propId)
					{
					case 3681:
						return false;
					case 3682:
						return false;
					default:
						switch (propId)
						{
						case 4084:
						case 4089:
						case 4090:
						case 4091:
						case 4094:
						case 4095:
							return false;
						case 4085:
						case 4086:
						case 4088:
						case 4092:
						case 4093:
						case 4097:
						case 4098:
						case 4099:
						case 4100:
						case 4101:
							goto IL_2CF;
						case 4087:
							if (!base.ForUpload)
							{
								return false;
							}
							goto IL_2CF;
						case 4096:
						case 4102:
						case 4103:
						case 4104:
						case 4105:
							break;
						default:
							goto IL_2CF;
						}
						break;
					}
				}
				else
				{
					switch (propId)
					{
					case 4112:
					case 4113:
					case 4115:
						break;
					case 4114:
						goto IL_2CF;
					default:
						if (propId == 4150)
						{
							return false;
						}
						switch (propId)
						{
						case 4288:
						case 4289:
						case 4290:
							return false;
						default:
							goto IL_2CF;
						}
						break;
					}
				}
				if (propTag.PropId != this.MapiMessage.GetNativeBodyPropertyTag(base.Context.CurrentOperationContext).PropId)
				{
					return false;
				}
				goto IL_2CF;
			}
			else if (propId <= 26377)
			{
				if (propId <= 16345)
				{
					if (propId == 4339)
					{
						return false;
					}
					if (propId != 16345)
					{
						goto IL_2CF;
					}
					if (base.ForMoveUser || base.ForUpload)
					{
						return true;
					}
					if (base.DownloadContext is IcsContentDownloadContext)
					{
						return base.PropList != null && !base.ExcludeProps && base.PropList.Contains(propTag);
					}
					goto IL_2CF;
				}
				else
				{
					if (propId == 26255)
					{
						return false;
					}
					switch (propId)
					{
					case 26375:
						break;
					case 26376:
						goto IL_2CF;
					case 26377:
						return false;
					default:
						goto IL_2CF;
					}
				}
			}
			else if (propId <= 26439)
			{
				if (propId != 26380 && propId != 26439)
				{
					goto IL_2CF;
				}
				return false;
			}
			else
			{
				if (propId == 26442)
				{
					return false;
				}
				switch (propId)
				{
				case 26476:
				case 26477:
					return false;
				default:
					if (propId != 26538)
					{
						goto IL_2CF;
					}
					return false;
				}
			}
			return false;
			IL_2CF:
			return base.IncludeTag(propTag);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<FastTransferMessage>(this);
		}

		protected override void InternalDispose(bool isCalledFromDispose)
		{
			if (isCalledFromDispose)
			{
				if (this.MapiMessage != null && !this.topLevel)
				{
					this.MapiMessage.Dispose();
					this.MapiMessage = null;
				}
				if (this.recipientWrapper != null)
				{
					this.recipientWrapper.Dispose();
					this.recipientWrapper = null;
				}
			}
			base.InternalDispose(isCalledFromDispose);
		}

		internal PropGroupChangeInfo GetPropGroupChangeInfo()
		{
			return this.MapiMessage.GetPropGroupChangeInfo(base.Context.CurrentOperationContext);
		}

		private readonly bool excludeAttachments;

		private readonly bool excludeRecipients;

		private readonly bool topLevel;

		private readonly FastTransferCopyFlag flags;

		private FastTransferRecipient recipientWrapper;
	}
}
