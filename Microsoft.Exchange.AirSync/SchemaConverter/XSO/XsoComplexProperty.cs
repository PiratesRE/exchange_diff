using System;
using System.Xml;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	[Serializable]
	internal class XsoComplexProperty : XsoProperty, IXmlProperty, IProperty
	{
		public XsoComplexProperty() : base(null)
		{
		}

		public virtual XmlNode XmlData
		{
			get
			{
				return null;
			}
		}

		protected override void InternalCopyFromModified(IProperty srcProperty)
		{
		}
	}
}
