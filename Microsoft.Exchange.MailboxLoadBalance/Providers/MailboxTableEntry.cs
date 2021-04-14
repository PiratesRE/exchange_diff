using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Mapi;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Directory;
using Microsoft.Exchange.MailboxLoadBalance.Directory.ExchangeDirectory;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxLoadBalance.Providers
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MailboxTableEntry
	{
		protected MailboxTableEntry(IDictionary<PropTag, PropValue> values)
		{
			this.values = values;
		}

		public static MailboxTableEntry FromValues(PropValue[] propValues)
		{
			return new MailboxTableEntry((from pv in propValues
			where !pv.IsNull() && !pv.IsError()
			select pv).ToDictionary((PropValue pv) => pv.PropTag, (PropValue pv) => pv));
		}

		public virtual TPropertyValue GetPropertyValue<TPropertyValue>(MapiPropertyDefinition mapiPropertyDefinition)
		{
			PropValue value;
			if (!this.values.TryGetValue(mapiPropertyDefinition.PropertyTag, out value))
			{
				return default(TPropertyValue);
			}
			return (TPropertyValue)((object)mapiPropertyDefinition.Extractor(value, mapiPropertyDefinition));
		}

		public ByteQuantifiedSize GetSizeProperty(MapiPropertyDefinition property)
		{
			Unlimited<ByteQuantifiedSize> propertyValue = this.GetPropertyValue<Unlimited<ByteQuantifiedSize>>(property);
			if (propertyValue.IsUnlimited)
			{
				return new ByteQuantifiedSize(0UL);
			}
			return propertyValue.Value;
		}

		public virtual TenantPartitionHintAdapter GetTenantPartitionHint()
		{
			byte[] propertyValue = this.GetPropertyValue<byte[]>(MailboxTablePropertyDefinitions.PersistableTenantPartitionHint);
			return TenantPartitionHintAdapter.FromPersistableTenantPartitionHint(propertyValue);
		}

		public IPhysicalMailbox ToPhysicalMailbox()
		{
			MailboxMiscFlags propertyValue = this.GetPropertyValue<MailboxMiscFlags>(MailboxTablePropertyDefinitions.MailboxMiscFlags);
			Guid propertyValue2 = this.GetPropertyValue<Guid>(MailboxTablePropertyDefinitions.MailboxGuid);
			TenantPartitionHintAdapter tenantPartitionHint = this.GetTenantPartitionHint();
			ByteQuantifiedSize sizeProperty = this.GetSizeProperty(MailboxTablePropertyDefinitions.TotalItemSize);
			ByteQuantifiedSize sizeProperty2 = this.GetSizeProperty(MailboxTablePropertyDefinitions.TotalDeletedItemSize);
			ByteQuantifiedSize byteQuantifiedSize = sizeProperty + sizeProperty2;
			ByteQuantifiedSize sizeProperty3 = this.GetSizeProperty(MailboxTablePropertyDefinitions.MessageTableTotalSize);
			ByteQuantifiedSize sizeProperty4 = this.GetSizeProperty(MailboxTablePropertyDefinitions.AttachmentTableTotalSize);
			ByteQuantifiedSize sizeProperty5 = this.GetSizeProperty(MailboxTablePropertyDefinitions.OtherTablesTotalSize);
			ByteQuantifiedSize byteQuantifiedSize2 = sizeProperty3 + sizeProperty4 + sizeProperty5;
			bool propertyValue3 = this.GetPropertyValue<bool>(MailboxTablePropertyDefinitions.IsQuarantined);
			StoreMailboxType propertyValue4 = this.GetPropertyValue<StoreMailboxType>(MailboxTablePropertyDefinitions.StoreMailboxType);
			uint? propertyValue5 = this.GetPropertyValue<uint?>(MailboxTablePropertyDefinitions.ItemCount);
			DateTime? propertyValue6 = this.GetPropertyValue<DateTime?>(MailboxTablePropertyDefinitions.LastLogonTime);
			DateTime? propertyValue7 = this.GetPropertyValue<DateTime?>(MailboxTablePropertyDefinitions.DisconnectDate);
			DirectoryIdentity identity = DirectoryIdentity.CreateMailboxIdentity(propertyValue2, tenantPartitionHint, DirectoryObjectType.Mailbox);
			ByteQuantifiedSize totalLogicalSize = byteQuantifiedSize;
			ByteQuantifiedSize totalPhysicalSize = byteQuantifiedSize2;
			bool isQuarantined = propertyValue3;
			StoreMailboxType mailboxType = propertyValue4;
			uint? num = propertyValue5;
			return new PhysicalMailbox(identity, totalLogicalSize, totalPhysicalSize, isQuarantined, mailboxType, ((num != null) ? new ulong?((ulong)num.GetValueOrDefault()) : null) ?? 0UL, propertyValue6, propertyValue7, tenantPartitionHint.IsConsumer, propertyValue.HasFlag(MailboxMiscFlags.SoftDeletedMailbox) || propertyValue.HasFlag(MailboxMiscFlags.MRSSoftDeletedMailbox), propertyValue.HasFlag(MailboxMiscFlags.ArchiveMailbox), propertyValue.HasFlag(MailboxMiscFlags.DisabledMailbox), propertyValue.HasFlag(MailboxMiscFlags.CreatedByMove))
			{
				OrganizationId = tenantPartitionHint.ExternalDirectoryOrganizationId,
				MessageTableTotalSize = sizeProperty3,
				OtherTablesTotalSize = sizeProperty5,
				AttachmentTableTotalSize = sizeProperty4,
				TotalItemSize = sizeProperty,
				TotalDeletedItemSize = sizeProperty2,
				ItemsPendingUpgrade = this.GetPropertyValue<int>(MailboxTablePropertyDefinitions.ItemsPendingUpgrade),
				CreationTimestamp = this.GetPropertyValue<DateTime>(MailboxTablePropertyDefinitions.CreationTime)
			};
		}

		public override string ToString()
		{
			return string.Format("MTE: [values_len: {0}, {1}]", this.values.Count, string.Join(",", this.values.Values.Select(new Func<PropValue, string>(TraceUtils.DumpPropVal))));
		}

		private readonly IDictionary<PropTag, PropValue> values;
	}
}
