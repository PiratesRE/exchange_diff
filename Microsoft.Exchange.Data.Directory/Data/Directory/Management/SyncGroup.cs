using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.Provisioning;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[ProvisioningObjectTag("SyncGroup")]
	[Serializable]
	public class SyncGroup : WindowsGroup
	{
		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return SyncGroup.schema;
			}
		}

		public SyncGroup()
		{
			base.SetObjectClass("group");
		}

		public SyncGroup(ADGroup dataObject) : base(dataObject)
		{
		}

		internal new static SyncGroup FromDataObject(ADGroup dataObject)
		{
			if (dataObject == null)
			{
				return null;
			}
			return new SyncGroup(dataObject);
		}

		[Parameter(Mandatory = false)]
		public bool IsDirSynced
		{
			get
			{
				return (bool)this[SyncGroupSchema.IsDirSynced];
			}
			set
			{
				this[SyncGroupSchema.IsDirSynced] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> DirSyncAuthorityMetadata
		{
			get
			{
				return (MultiValuedProperty<string>)this[SyncGroupSchema.DirSyncAuthorityMetadata];
			}
			set
			{
				this[SyncGroupSchema.DirSyncAuthorityMetadata] = value;
			}
		}

		public string ExternalDirectoryObjectId
		{
			get
			{
				return (string)this[SyncGroupSchema.ExternalDirectoryObjectId];
			}
		}

		[Parameter(Mandatory = false)]
		public string OnPremisesObjectId
		{
			get
			{
				return (string)this[SyncGroupSchema.OnPremisesObjectId];
			}
			set
			{
				this[SyncGroupSchema.OnPremisesObjectId] = value;
			}
		}

		private static SyncGroupSchema schema = ObjectSchema.GetInstance<SyncGroupSchema>();
	}
}
