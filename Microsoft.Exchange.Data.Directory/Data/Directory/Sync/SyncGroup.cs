using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal class SyncGroup : SyncRecipient
	{
		public SyncGroup(SyncDirection syncDirection) : base(syncDirection)
		{
		}

		public override SyncObjectSchema Schema
		{
			get
			{
				return SyncGroup.schema;
			}
		}

		internal override DirectoryObjectClass ObjectClass
		{
			get
			{
				return DirectoryObjectClass.Group;
			}
		}

		protected override DirectoryObject CreateDirectoryObject()
		{
			return new Group();
		}

		public SyncProperty<MultiValuedProperty<SyncLink>> CoManagedBy
		{
			get
			{
				return (SyncProperty<MultiValuedProperty<SyncLink>>)base[SyncGroupSchema.CoManagedBy];
			}
			set
			{
				base[SyncGroupSchema.CoManagedBy] = value;
			}
		}

		public SyncProperty<bool> MailEnabled
		{
			get
			{
				return (SyncProperty<bool>)base[SyncGroupSchema.MailEnabled];
			}
			set
			{
				base[SyncGroupSchema.MailEnabled] = value;
			}
		}

		public SyncProperty<MultiValuedProperty<SyncLink>> ManagedBy
		{
			get
			{
				return (SyncProperty<MultiValuedProperty<SyncLink>>)base[SyncGroupSchema.ManagedBy];
			}
			set
			{
				base[SyncGroupSchema.ManagedBy] = value;
			}
		}

		public SyncProperty<bool> ReportToOriginatorEnabled
		{
			get
			{
				return (SyncProperty<bool>)base[SyncGroupSchema.ReportToOriginatorEnabled];
			}
			set
			{
				base[SyncGroupSchema.ReportToOriginatorEnabled] = value;
			}
		}

		public SyncProperty<MultiValuedProperty<SyncLink>> Members
		{
			get
			{
				return (SyncProperty<MultiValuedProperty<SyncLink>>)base[SyncGroupSchema.Members];
			}
			set
			{
				base[SyncGroupSchema.Members] = value;
			}
		}

		public SyncProperty<bool> SecurityEnabled
		{
			get
			{
				return (SyncProperty<bool>)base[SyncGroupSchema.SecurityEnabled];
			}
			set
			{
				base[SyncGroupSchema.SecurityEnabled] = value;
			}
		}

		public SyncProperty<bool> SendOofMessageToOriginatorEnabled
		{
			get
			{
				return (SyncProperty<bool>)base[SyncGroupSchema.SendOofMessageToOriginatorEnabled];
			}
			set
			{
				base[SyncGroupSchema.SendOofMessageToOriginatorEnabled] = value;
			}
		}

		public SyncProperty<string> WellKnownObject
		{
			get
			{
				return (SyncProperty<string>)base[SyncGroupSchema.WellKnownObject];
			}
			set
			{
				base[SyncGroupSchema.WellKnownObject] = value;
			}
		}

		public override SyncProperty<RecipientTypeDetails> RecipientTypeDetailsValue
		{
			get
			{
				return (SyncProperty<RecipientTypeDetails>)base[SyncGroupSchema.RecipientTypeDetailsValue];
			}
			set
			{
				base[SyncGroupSchema.RecipientTypeDetailsValue] = value;
			}
		}

		public SyncProperty<string> Creator
		{
			get
			{
				return (SyncProperty<string>)base[SyncGroupSchema.Creator];
			}
			set
			{
				base[SyncGroupSchema.Creator] = value;
			}
		}

		public SyncProperty<MultiValuedProperty<string>> SharePointResources
		{
			get
			{
				return (SyncProperty<MultiValuedProperty<string>>)base[SyncGroupSchema.SharePointResources];
			}
			set
			{
				base[SyncGroupSchema.SharePointResources] = value;
			}
		}

		public SyncProperty<bool> IsPublic
		{
			get
			{
				return (SyncProperty<bool>)base[SyncGroupSchema.IsPublic];
			}
			set
			{
				base[SyncGroupSchema.IsPublic] = value;
			}
		}

		public SyncProperty<MultiValuedProperty<SyncLink>> Owners
		{
			get
			{
				return (SyncProperty<MultiValuedProperty<SyncLink>>)base[SyncGroupSchema.Owners];
			}
			set
			{
				base[SyncGroupSchema.Owners] = value;
			}
		}

		protected override SyncPropertyDefinition[] MinimumForwardSyncProperties
		{
			get
			{
				List<SyncPropertyDefinition> list = base.MinimumForwardSyncProperties.ToList<SyncPropertyDefinition>();
				list.AddRange(SyncGroup.minimumForwardSyncProperties);
				return list.ToArray();
			}
		}

		public override SyncRecipient CreatePlaceHolder()
		{
			SyncGroup syncGroup = (SyncGroup)base.CreatePlaceHolder();
			syncGroup.CopyChangeFrom(this, SyncGroup.minimumForwardSyncProperties);
			return syncGroup;
		}

		internal static void SecurityEnabledSetter(object value, IPropertyBag propertyBag)
		{
			GroupTypeFlags groupTypeFlags = (GroupTypeFlags)propertyBag[SyncGroupSchema.GroupType];
			if ((bool)value)
			{
				propertyBag[SyncGroupSchema.GroupType] = (groupTypeFlags | GroupTypeFlags.SecurityEnabled);
				return;
			}
			propertyBag[SyncGroupSchema.GroupType] = (groupTypeFlags & (GroupTypeFlags)2147483647);
		}

		private static readonly SyncGroupSchema schema = ObjectSchema.GetInstance<SyncGroupSchema>();

		private static readonly SyncPropertyDefinition[] minimumForwardSyncProperties = new SyncPropertyDefinition[]
		{
			SyncGroupSchema.MailEnabled,
			SyncGroupSchema.WellKnownObject,
			SyncGroupSchema.RecipientTypeDetailsValue
		};
	}
}
