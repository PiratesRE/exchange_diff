using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class InferenceLog : PrivateObjectPropertyBag
	{
		protected InferenceLog(Context context, Mailbox mailbox, bool newItem, params ColumnValue[] initialValues) : base(context, DatabaseSchema.InferenceLogTable(context.Database).Table, newItem, false, newItem, true, initialValues)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				mailbox.IsValid();
				this.mailbox = mailbox;
				base.LoadData(context);
				disposeGuard.Success();
			}
		}

		protected InferenceLog(Context context, Mailbox mailbox, Reader reader) : base(context, DatabaseSchema.InferenceLogTable(context.Database).Table, false, true, reader)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				mailbox.IsValid();
				this.mailbox = mailbox;
				base.LoadData(context);
				disposeGuard.Success();
			}
		}

		protected override ObjectType GetObjectType()
		{
			return ObjectType.InferenceLog;
		}

		public override ObjectPropertySchema Schema
		{
			get
			{
				if (this.propertySchema == null)
				{
					this.propertySchema = PropertySchema.GetObjectSchema(this.Mailbox.Database, ObjectType.InferenceLog);
				}
				return this.propertySchema;
			}
		}

		public Mailbox Mailbox
		{
			get
			{
				return this.mailbox;
			}
		}

		public override Context CurrentOperationContext
		{
			get
			{
				return this.Mailbox.CurrentOperationContext;
			}
		}

		public override ReplidGuidMap ReplidGuidMap
		{
			get
			{
				return this.Mailbox.ReplidGuidMap;
			}
		}

		protected override StorePropTag MapPropTag(Context context, uint propertyTag)
		{
			return this.Mailbox.GetStorePropTag(context, propertyTag, this.GetObjectType());
		}

		public static InferenceLog Open(Context context, Mailbox mailbox, Reader reader)
		{
			return new InferenceLog(context, mailbox, reader);
		}

		public static InferenceLog Create(Context context, Mailbox mailbox)
		{
			InferenceLogTable inferenceLogTable = DatabaseSchema.InferenceLogTable(mailbox.Database);
			return new InferenceLog(context, mailbox, true, new ColumnValue[]
			{
				new ColumnValue(inferenceLogTable.MailboxPartitionNumber, mailbox.MailboxPartitionNumber),
				new ColumnValue(inferenceLogTable.CreateTime, DateTime.UtcNow)
			});
		}

		private readonly Mailbox mailbox;

		private ObjectPropertySchema propertySchema;
	}
}
