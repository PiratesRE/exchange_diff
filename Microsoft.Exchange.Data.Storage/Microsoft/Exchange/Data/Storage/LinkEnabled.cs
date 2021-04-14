using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class LinkEnabled : SmartPropertyDefinition
	{
		public LinkEnabled() : base("LinkEnabled", typeof(bool), PropertyFlags.None, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.OutlookPhishingStamp, PropertyDependencyType.AllRead),
			new PropertyDependency(InternalSchema.OutlookSpoofingStamp, PropertyDependencyType.NeedForRead)
		})
		{
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			int valueOrDefault = propertyBag.GetValueOrDefault<int>(InternalSchema.OutlookPhishingStamp);
			int valueOrDefault2 = propertyBag.GetValueOrDefault<int>(InternalSchema.OutlookSpoofingStamp);
			if ((valueOrDefault & 268435455) != 0 || (valueOrDefault2 & 268435455) != 0)
			{
				return (valueOrDefault & 268435456) != 0 || (valueOrDefault2 & 268435456) != 0;
			}
			return new PropertyError(this, PropertyErrorCode.NotFound);
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			int num = propertyBag.GetValueOrDefault<int>(InternalSchema.OutlookPhishingStamp);
			if (num <= 0)
			{
				byte[] array = LinkEnabled.GetMovingStamp(propertyBag.Context.Session);
				if (array.Length != 4)
				{
					array = new byte[]
					{
						1,
						2,
						3,
						4
					};
				}
				num = BitConverter.ToInt32(array, 0);
			}
			if ((bool)value)
			{
				num |= 268435456;
			}
			else
			{
				num &= -268435457;
			}
			propertyBag.SetValueWithFixup(InternalSchema.OutlookPhishingStamp, num);
		}

		private static byte[] GetMovingStamp(StoreSession storeSession)
		{
			MailboxSession mailboxSession = storeSession as MailboxSession;
			if (mailboxSession == null)
			{
				return Array<byte>.Empty;
			}
			using (Folder folder = Folder.Bind(mailboxSession, mailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox), new PropertyDefinition[]
			{
				InternalSchema.AdditionalRenEntryIds
			}))
			{
				int num = 5;
				byte[][] array = folder.TryGetProperty(InternalSchema.AdditionalRenEntryIds) as byte[][];
				if (array != null && array.Length > num)
				{
					return array[num];
				}
			}
			return Array<byte>.Empty;
		}

		private const int PhishingStampMask = 268435455;

		private const int PhishingEnabledMask = 268435456;
	}
}
