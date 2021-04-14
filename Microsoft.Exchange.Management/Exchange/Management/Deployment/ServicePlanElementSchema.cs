using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Exchange.Management.Deployment
{
	internal abstract class ServicePlanElementSchema : IEnumerable
	{
		protected ServicePlanElementSchema()
		{
			this.InitializeAllFeatures();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.allFeatures.GetEnumerator();
		}

		private void InitializeAllFeatures()
		{
			List<FeatureDefinition> list = new List<FeatureDefinition>();
			FieldInfo[] fields = base.GetType().GetFields(BindingFlags.Static | BindingFlags.Public);
			foreach (FieldInfo fieldInfo in fields)
			{
				if (fieldInfo.FieldType == typeof(FeatureDefinition))
				{
					FeatureDefinition item = (FeatureDefinition)fieldInfo.GetValue(null);
					list.Add(item);
				}
			}
			this.allFeatures = list;
		}

		private List<FeatureDefinition> allFeatures;
	}
}
