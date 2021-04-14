using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class PredictedActionsProperty : SmartPropertyDefinition
	{
		internal PredictedActionsProperty(string displayName, NativeStorePropertyDefinition predictedActionsProperty, PropertyFlags propertyFlags) : base(displayName, typeof(PredictedActionAndProbability[]), propertyFlags, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(predictedActionsProperty, PropertyDependencyType.AllRead)
		})
		{
			Util.ThrowOnNullArgument(predictedActionsProperty, "predictedActionsProperty");
			this.predictedActionsProperty = predictedActionsProperty;
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			object obj = propertyBag.GetValue(this.predictedActionsProperty);
			if (!(obj is PropertyError))
			{
				short[] array = (short[])obj;
				int num = array.Length;
				PredictedActionAndProbability[] array2 = new PredictedActionAndProbability[num];
				for (int i = 0; i < num; i++)
				{
					array2[i] = new PredictedActionAndProbability(array[i]);
				}
				obj = array2;
			}
			return obj;
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("FlagStatusProperty");
			}
			if (base.PropertyFlags != PropertyFlags.ReadOnly)
			{
				PredictedActionAndProbability[] array = (PredictedActionAndProbability[])value;
				short[] array2 = new short[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array2[i] = array[i].RawActionAndProbability;
				}
				propertyBag.SetValue(this.predictedActionsProperty, array2);
			}
		}

		private NativeStorePropertyDefinition predictedActionsProperty;
	}
}
