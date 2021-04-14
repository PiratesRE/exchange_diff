using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.TypeConversion
{
	public class OfTypeTranslationRule<TLeft, TRight, TNewLeft, TNewRight> : IStorageTranslationRule<TLeft, TRight>, ITranslationRule<TLeft, TRight> where TNewLeft : class, TLeft where TNewRight : class, TRight
	{
		public OfTypeTranslationRule(ITranslationRule<TNewLeft, TNewRight> internalRule)
		{
			this.internalRule = internalRule;
			this.storageRule = (internalRule as IStorageTranslationRule<TNewLeft, TNewRight>);
		}

		IEnumerable<Microsoft.Exchange.Data.PropertyDefinition> IStorageTranslationRule<!0, !1>.StorageDependencies
		{
			get
			{
				if (this.storageRule == null)
				{
					return null;
				}
				return this.storageRule.StorageDependencies;
			}
		}

		PropertyChangeMetadata.PropertyGroup IStorageTranslationRule<!0, !1>.StoragePropertyGroup
		{
			get
			{
				if (this.storageRule == null)
				{
					return null;
				}
				return this.storageRule.StoragePropertyGroup;
			}
		}

		IEnumerable<Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition> IStorageTranslationRule<!0, !1>.EntityProperties
		{
			get
			{
				if (this.storageRule == null)
				{
					return null;
				}
				return this.storageRule.EntityProperties;
			}
		}

		public void FromLeftToRightType(TLeft left, TRight right)
		{
			TNewLeft tnewLeft = left as TNewLeft;
			TNewRight tnewRight = right as TNewRight;
			if (tnewLeft != null && tnewRight != null)
			{
				this.internalRule.FromLeftToRightType(tnewLeft, tnewRight);
			}
		}

		public void FromRightToLeftType(TLeft left, TRight right)
		{
			TNewLeft tnewLeft = left as TNewLeft;
			TNewRight tnewRight = right as TNewRight;
			if (tnewLeft != null && tnewRight != null)
			{
				this.internalRule.FromRightToLeftType(tnewLeft, tnewRight);
			}
		}

		private readonly ITranslationRule<TNewLeft, TNewRight> internalRule;

		private readonly IStorageTranslationRule<TNewLeft, TNewRight> storageRule;
	}
}
