using System;
using Microsoft.Exchange.Data.ContentTypes.Internal;

namespace Microsoft.Exchange.Data.ContentTypes.vCard
{
	internal class ContactValueTypeContainer : ValueTypeContainer
	{
		public override bool IsTextType
		{
			get
			{
				this.CalculateValueType();
				return this.valueType == ContactValueType.Text || this.valueType == ContactValueType.PhoneNumber || this.valueType == ContactValueType.VCard;
			}
		}

		public override bool CanBeMultivalued
		{
			get
			{
				this.CalculateValueType();
				return this.valueType != ContactValueType.Binary && this.valueType != ContactValueType.VCard;
			}
		}

		public override bool CanBeCompound
		{
			get
			{
				this.CalculateValueType();
				return this.valueType != ContactValueType.Binary && this.valueType != ContactValueType.VCard;
			}
		}

		public ContactValueType ValueType
		{
			get
			{
				this.CalculateValueType();
				return this.valueType;
			}
		}

		private void CalculateValueType()
		{
			if (this.isValueTypeInitialized)
			{
				return;
			}
			this.valueType = ContactValueType.Unknown;
			if (this.valueTypeParameter != null)
			{
				this.valueType = ContactCommon.GetValueTypeEnum(this.valueTypeParameter);
			}
			else
			{
				PropertyId propertyEnum = ContactCommon.GetPropertyEnum(this.propertyName);
				if (propertyEnum != PropertyId.Unknown)
				{
					this.valueType = ContactCommon.GetDefaultValueType(propertyEnum);
				}
			}
			if (this.valueType == ContactValueType.Unknown)
			{
				this.valueType = ContactValueType.Text;
			}
			this.isValueTypeInitialized = true;
		}

		private ContactValueType valueType;
	}
}
