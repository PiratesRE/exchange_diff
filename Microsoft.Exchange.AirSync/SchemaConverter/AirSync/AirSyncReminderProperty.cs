using System;
using System.Globalization;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;

namespace Microsoft.Exchange.AirSync.SchemaConverter.AirSync
{
	[Serializable]
	internal class AirSyncReminderProperty : AirSyncIntegerProperty
	{
		public AirSyncReminderProperty(string xmlNodeNamespace, string airSyncTagName, bool requiresClientSupport) : base(xmlNodeNamespace, airSyncTagName, requiresClientSupport)
		{
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
			int integerData = integerProperty.IntegerData;
			if (-1 != integerData)
			{
				base.CreateAirSyncNode(integerData.ToString(CultureInfo.InvariantCulture));
			}
		}
	}
}
