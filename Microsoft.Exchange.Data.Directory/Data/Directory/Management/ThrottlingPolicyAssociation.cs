using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public class ThrottlingPolicyAssociation : ADPresentationObject
	{
		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return ThrottlingPolicyAssociation.schema;
			}
		}

		public ThrottlingPolicyAssociation()
		{
		}

		public ThrottlingPolicyAssociation(ADRecipient dataObject) : base(dataObject)
		{
		}

		internal static ThrottlingPolicyAssociation FromDataObject(ADRecipient dataObject)
		{
			if (dataObject == null)
			{
				return null;
			}
			return new ThrottlingPolicyAssociation(dataObject);
		}

		public ADObjectId ObjectId
		{
			get
			{
				return (ADObjectId)this[ThrottlingPolicyAssociationSchema.ObjectId];
			}
		}

		public ADObjectId ThrottlingPolicyId
		{
			get
			{
				return (ADObjectId)this[ThrottlingPolicyAssociationSchema.ThrottlingPolicyId];
			}
		}

		public new string Name
		{
			get
			{
				return base.Name;
			}
		}

		private static ThrottlingPolicyAssociationSchema schema = ObjectSchema.GetInstance<ThrottlingPolicyAssociationSchema>();
	}
}
