using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Protocols.MAPI;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.FastTransfer;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.FastTransfer
{
	internal class InferenceLogIterator : DisposableBase, IMessageIterator, IMessageIteratorClient, IDisposable
	{
		public InferenceLogIterator(FastTransferDownloadContext downloadContext)
		{
			this.context = downloadContext;
			this.readOnly = true;
		}

		public InferenceLogIterator(FastTransferUploadContext uploadContext)
		{
			this.context = uploadContext;
			this.readOnly = false;
		}

		public bool ReadOnly
		{
			get
			{
				return this.readOnly;
			}
		}

		public FastTransferContext Context
		{
			get
			{
				return this.context;
			}
		}

		public IEnumerator<IMessage> GetMessages()
		{
			InferenceLogViewTable logView = new InferenceLogViewTable(this.Context.CurrentOperationContext, this.Context.Logon.StoreMailbox);
			List<Properties> cachedRows = new List<Properties>(32);
			long currentItemCount = 0L;
			bool stopConditionHit = false;
			DateTime stopCreateTime = DateTime.UtcNow;
			UnlimitedItems stopItemCount = ConfigurationSchema.InferenceLogMaxRows.Value;
			if (ConfigurationSchema.InferenceLogRetentionPeriod.Value > stopCreateTime - DateTime.MinValue)
			{
				stopCreateTime = DateTime.MinValue;
			}
			else
			{
				stopCreateTime -= ConfigurationSchema.InferenceLogRetentionPeriod.Value;
			}
			logView.SeekRow(this.Context.CurrentOperationContext, ViewSeekOrigin.End, 0);
			while (!stopConditionHit)
			{
				using (Reader reader = logView.QueryRows(this.Context.CurrentOperationContext, 32, true))
				{
					if (reader == null)
					{
						yield break;
					}
					while (reader.Read())
					{
						using (InferenceLog inferenceLog = InferenceLog.Open(this.Context.CurrentOperationContext, this.Context.Logon.StoreMailbox, reader))
						{
							Properties rowProps = new Properties(25);
							inferenceLog.EnumerateProperties(this.Context.CurrentOperationContext, delegate(StorePropTag tag, object value)
							{
								rowProps.Add(tag, value);
								return true;
							}, true);
							currentItemCount += 1L;
							if (!stopItemCount.IsUnlimited && stopItemCount.Value < currentItemCount)
							{
								stopConditionHit = true;
								break;
							}
							DateTime t = (DateTime)inferenceLog.GetPropertyValue(this.Context.CurrentOperationContext, PropTag.InferenceLog.InferenceTimeStamp);
							if (t < stopCreateTime)
							{
								stopConditionHit = true;
								break;
							}
							cachedRows.Add(rowProps);
							logView.BookmarkCurrentRow(reader, true);
						}
					}
				}
				if (cachedRows.Count == 0)
				{
					break;
				}
				foreach (Properties row in cachedRows)
				{
					yield return new InferenceLogIterator.Record(this, row);
				}
				cachedRows.Clear();
			}
			yield break;
		}

		public IMessage UploadMessage(bool isAssociatedMessage)
		{
			return new InferenceLogIterator.Record(this, Properties.Empty);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<InferenceLogIterator>(this);
		}

		protected override void InternalDispose(bool isCalledFromDispose)
		{
		}

		public const int MaximumRowsPerBatch = 32;

		private const int RowCapacity = 25;

		private readonly bool readOnly;

		private FastTransferContext context;

		private class Record : IMessage, IDisposable
		{
			public Record(InferenceLogIterator logIterator, Properties properties)
			{
				this.logIterator = logIterator;
				this.propertyBag = new MemoryPropertyBag(logIterator.Context);
				if (properties.Count > 0)
				{
					foreach (Property prop in properties)
					{
						PropertyValue property = RcaTypeHelpers.MassageOutgoingProperty(prop, true);
						if (property.PropertyTag.PropertyType != PropertyType.Error)
						{
							this.propertyBag.SetProperty(property);
						}
					}
					this.propertyBag.SetProperty(new PropertyValue(PropertyTag.Mid, 0L));
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
					return false;
				}
			}

			public IEnumerable<IRecipient> GetRecipients()
			{
				yield break;
			}

			public IRecipient CreateRecipient()
			{
				throw new ExExceptionNoSupport((LID)61264U, "Recipients are not supported on the log records");
			}

			public void RemoveRecipient(int rowId)
			{
				throw new ExExceptionNoSupport((LID)38364U, "Recipient removal is not supported on the log records");
			}

			public IEnumerable<IAttachmentHandle> GetAttachments()
			{
				yield break;
			}

			public IAttachment CreateAttachment()
			{
				throw new ExExceptionNoSupport((LID)44880U, "Attachments are not supported on the log records");
			}

			public void Save()
			{
				using (InferenceLog inferenceLog = InferenceLog.Create(this.logIterator.Context.CurrentOperationContext, this.logIterator.Context.Logon.StoreMailbox))
				{
					bool flag = false;
					foreach (AnnotatedPropertyValue annotatedPropertyValue in this.PropertyBag.GetAnnotatedProperties())
					{
						if (!annotatedPropertyValue.PropertyValue.IsError)
						{
							StorePropTag storePropTag = LegacyHelper.ConvertFromLegacyPropTag(annotatedPropertyValue.PropertyValue.PropertyTag, Microsoft.Exchange.Server.Storage.PropTags.ObjectType.InferenceLog, this.logIterator.Context.Logon.MapiMailbox, true);
							if (storePropTag != PropTag.InferenceLog.RowId && storePropTag != PropTag.InferenceLog.Mid)
							{
								object value = annotatedPropertyValue.PropertyValue.Value;
								RcaTypeHelpers.MassageIncomingPropertyValue(annotatedPropertyValue.PropertyValue.PropertyTag, ref value);
								ErrorCode errorCode = inferenceLog.SetProperty(this.logIterator.Context.CurrentOperationContext, storePropTag, value);
								if (errorCode != ErrorCode.NoError)
								{
									throw new StoreException((LID)57168U, errorCode, "Unable to set property on inference log record");
								}
								flag = true;
							}
						}
					}
					if (flag)
					{
						inferenceLog.Flush(this.logIterator.Context.CurrentOperationContext);
					}
				}
			}

			public void SetLongTermId(StoreLongTermId longTermId)
			{
			}

			public void Dispose()
			{
			}

			private InferenceLogIterator logIterator;

			private MemoryPropertyBag propertyBag;
		}
	}
}
