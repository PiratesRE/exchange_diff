using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	[Serializable]
	internal class XsoUnlinkedProperty : XsoProperty, IUnlinkedProperty, IProperty
	{
		public XsoUnlinkedProperty() : base(null, PropertyType.ReadOnly)
		{
		}
	}
}
