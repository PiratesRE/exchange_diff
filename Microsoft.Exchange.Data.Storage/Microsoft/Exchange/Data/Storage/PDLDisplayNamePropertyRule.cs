using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class PDLDisplayNamePropertyRule : PriorityBasedDisplayNamePropertyRule
	{
		protected override IList<PriorityBasedDisplayNamePropertyRule.CandidateProperty> GetCandidateProperties()
		{
			return new List<PriorityBasedDisplayNamePropertyRule.CandidateProperty>
			{
				new PriorityBasedDisplayNamePropertyRule.SimpleCandidateProperty(InternalSchema.DisplayName),
				new PriorityBasedDisplayNamePropertyRule.SimpleCandidateProperty(InternalSchema.DLName),
				new PriorityBasedDisplayNamePropertyRule.SimpleCandidateProperty(InternalSchema.MapiSubject)
			};
		}

		public override List<PropertyReference> GetPropertyReferenceList()
		{
			return PDLDisplayNamePropertyRule.Merge(base.GetPropertyReferenceList(), PropertyRuleLibrary.TruncateSubjectPropertyReferenceList);
		}

		public override bool UpdateDisplayNameProperties(ICorePropertyBag propertyBag)
		{
			bool flag = base.UpdateDisplayNameProperties(propertyBag);
			string text = propertyBag.GetValueOrDefault<string>(InternalSchema.DisplayNameFirstLast, string.Empty).Trim();
			if (text != string.Empty)
			{
				propertyBag[ContactBaseSchema.FileAs] = text;
				propertyBag[ItemSchema.Subject] = text;
				propertyBag[DistributionListSchema.DLName] = text;
			}
			return flag | this.TruncateSubject(propertyBag);
		}

		protected virtual bool TruncateSubject(ICorePropertyBag propertyBag)
		{
			return PropertyRuleLibrary.InternalTruncateSubject(propertyBag);
		}

		internal List<PropertyReference> GetBasePropertyReferenceList()
		{
			return base.GetPropertyReferenceList();
		}

		private static List<PropertyReference> Merge(List<PropertyReference> listOne, List<PropertyReference> listTwo)
		{
			List<PropertyReference> list = new List<PropertyReference>(listTwo);
			List<PropertyReference> list2 = new List<PropertyReference>(listOne.Count + list.Count);
			foreach (PropertyReference propertyReference in listOne)
			{
				bool flag = false;
				foreach (PropertyReference propertyReference2 in list)
				{
					if (propertyReference2.Property.Equals(propertyReference.Property))
					{
						list2.Add(new PropertyReference(propertyReference.Property, propertyReference.AccessType | propertyReference2.AccessType));
						list.Remove(propertyReference2);
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					list2.Add(propertyReference);
				}
			}
			list2.AddRange(list);
			return list2;
		}
	}
}
