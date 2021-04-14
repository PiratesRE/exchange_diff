using System;
using System.Collections;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class ApprovedApplicationCollection : ADMultiValuedProperty<ApprovedApplication>
	{
		public new static ApprovedApplicationCollection Empty
		{
			get
			{
				return ApprovedApplicationCollection.empty;
			}
		}

		public ApprovedApplicationCollection()
		{
		}

		public ApprovedApplicationCollection(object value) : base(ApprovedApplicationCollection.ConvertValue(value, null))
		{
		}

		public ApprovedApplicationCollection(ICollection values) : base(ApprovedApplicationCollection.ConvertValues(values))
		{
		}

		internal ApprovedApplicationCollection(bool readOnly, ProviderPropertyDefinition propertyDefinition, ICollection values) : this(readOnly, propertyDefinition, values, null, null)
		{
		}

		internal ApprovedApplicationCollection(bool readOnly, ProviderPropertyDefinition propertyDefinition, ICollection values, ICollection invalidValues, LocalizedString? readOnlyErrorMessage) : base(readOnly, propertyDefinition, ApprovedApplicationCollection.ConvertValues(values), invalidValues, readOnlyErrorMessage)
		{
		}

		public new static implicit operator ApprovedApplicationCollection(object[] array)
		{
			return new ApprovedApplicationCollection(false, null, array);
		}

		protected override bool TryAddInternal(ApprovedApplication item, out Exception error)
		{
			if (null == item || !item.IsCab)
			{
				return base.TryAddInternal(item, out error);
			}
			MultiValuedProperty<ApprovedApplication> multiValuedProperty = ApprovedApplication.ParseCab(item.CabName);
			if (multiValuedProperty != null)
			{
				base.BeginUpdate();
				try
				{
					foreach (ApprovedApplication item2 in multiValuedProperty)
					{
						if (!base.TryAddInternal(item2, out error))
						{
							return false;
						}
					}
				}
				finally
				{
					base.EndUpdate();
				}
			}
			error = null;
			return true;
		}

		public override bool Remove(ApprovedApplication item)
		{
			if (null == item)
			{
				throw new ArgumentNullException("item");
			}
			if (item.IsFromFile)
			{
				throw new InvalidOperationException(DirectoryStrings.ExceptionRemoveApprovedApplication(item.AppString));
			}
			return base.Remove(item);
		}

		protected override ValidationError ValidateValue(ApprovedApplication item)
		{
			if (item.IsCab)
			{
				return new PropertyValidationError(DirectoryStrings.ExceptionInvalidApprovedApplication(item.CabName), this.PropertyDefinition, item);
			}
			return base.ValidateValue(item);
		}

		private static ICollection ConvertValues(ICollection values)
		{
			ArrayList arrayList = new ArrayList();
			foreach (object value in values)
			{
				ApprovedApplicationCollection.ConvertValue(value, arrayList);
			}
			return arrayList;
		}

		private static ICollection ConvertValue(object value, ArrayList newValues)
		{
			if (newValues == null)
			{
				newValues = new ArrayList();
			}
			ApprovedApplication approvedApplication = (ApprovedApplication)ValueConvertor.ConvertValue(value, typeof(ApprovedApplication), null);
			if (approvedApplication != null)
			{
				if (approvedApplication.IsCab)
				{
					MultiValuedProperty<ApprovedApplication> multiValuedProperty = ApprovedApplication.ParseCab(approvedApplication.CabName);
					using (MultiValuedProperty<ApprovedApplication>.Enumerator enumerator = multiValuedProperty.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							ApprovedApplication value2 = enumerator.Current;
							newValues.Add(value2);
						}
						return newValues;
					}
				}
				newValues.Add(value);
				return newValues;
			}
			throw new DataValidationException(new PropertyValidationError(DirectoryStrings.ExceptionInvalidApprovedApplication(value.ToString()), MobileMailboxPolicySchema.ADApprovedApplicationList, value));
		}

		private static ApprovedApplicationCollection empty = new ApprovedApplicationCollection(true, null, new object[0]);
	}
}
