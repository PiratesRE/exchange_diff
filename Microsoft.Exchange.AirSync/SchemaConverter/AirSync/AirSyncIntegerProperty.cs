using System;
using System.Globalization;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;

namespace Microsoft.Exchange.AirSync.SchemaConverter.AirSync
{
	[Serializable]
	internal class AirSyncIntegerProperty : AirSyncProperty, IIntegerProperty, IProperty
	{
		public AirSyncIntegerProperty(string xmlNodeNamespace, string airSyncTagName, bool requiresClientSupport) : base(xmlNodeNamespace, airSyncTagName, requiresClientSupport)
		{
		}

		public virtual int IntegerData
		{
			get
			{
				return Convert.ToInt32(base.XmlNode.InnerText, CultureInfo.InvariantCulture);
			}
		}

		protected override void InternalCopyFrom(IProperty srcProperty)
		{
			IIntegerProperty integerProperty = srcProperty as IIntegerProperty;
			if (integerProperty == null)
			{
				throw new UnexpectedTypeException("IIntegerProperty", srcProperty);
			}
			if (PropertyState.Modified != srcProperty.State)
			{
				throw new ConversionException("Property only supports conversion from Modified property state");
			}
			base.CreateAirSyncNode(integerProperty.IntegerData.ToString(CultureInfo.InvariantCulture));
		}
	}
}
