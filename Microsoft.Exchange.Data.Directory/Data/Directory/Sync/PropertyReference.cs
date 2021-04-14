using System;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[Serializable]
	internal class PropertyReference
	{
		public string TargetId { get; private set; }

		public DirectoryObjectClassAddressList TargetObjectClass { get; private set; }

		public bool TargetDeleted { get; private set; }

		public ADObjectId TargetADObjectId { get; private set; }

		public PropertyReference(string targetId, DirectoryObjectClassAddressList targetObjectClass, bool targetDeleted)
		{
			this.TargetId = targetId;
			this.TargetObjectClass = targetObjectClass;
			this.TargetDeleted = targetDeleted;
		}

		public PropertyReference(ADObjectId targetADObjectId)
		{
			this.TargetADObjectId = targetADObjectId;
		}

		public static PropertyReference ParseFromADString(string adString)
		{
			ADObjectId targetADObjectId = ADObjectId.ParseExtendedDN(adString);
			return new PropertyReference(targetADObjectId);
		}

		public void UpdateReferenceData(string targetId, DirectoryObjectClassAddressList targetObjectClass)
		{
			this.TargetId = targetId;
			this.TargetObjectClass = targetObjectClass;
		}

		public override string ToString()
		{
			return this.TargetId ?? string.Empty;
		}

		public override bool Equals(object obj)
		{
			PropertyReference propertyReference = obj as PropertyReference;
			return propertyReference != null && (this.TargetId == propertyReference.TargetId && this.TargetDeleted == propertyReference.TargetDeleted && this.TargetObjectClass == propertyReference.TargetObjectClass) && this.TargetADObjectId == propertyReference.TargetADObjectId;
		}

		public override int GetHashCode()
		{
			return (int)(((this.TargetId == null) ? 0 : this.TargetId.GetHashCode()) + ((this.TargetADObjectId == null) ? 0 : this.TargetADObjectId.GetHashCode()) + this.TargetObjectClass);
		}

		public string ToADString()
		{
			if (this.TargetADObjectId != null)
			{
				return this.TargetADObjectId.ToExtendedDN();
			}
			return string.Empty;
		}
	}
}
