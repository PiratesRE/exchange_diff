using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Entity
{
	[Serializable]
	internal class EntityNativeBodyProperty : EntityContentProperty, IIntegerProperty, IProperty
	{
		public EntityNativeBodyProperty() : base(PropertyType.ReadOnly)
		{
		}

		public int IntegerData
		{
			get
			{
				return (int)base.GetNativeType();
			}
		}
	}
}
