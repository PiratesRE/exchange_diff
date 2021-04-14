using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;

namespace Microsoft.Exchange.AirSync.SchemaConverter.AirSync
{
	internal class AirSyncEmailBodyProperty : AirSyncBodyProperty, IBodyProperty, IMIMERelatedProperty, IProperty
	{
		public AirSyncEmailBodyProperty(string xmlNodeNamespace) : base(xmlNodeNamespace, "Body", "BodyTruncated", "BodySize", true, false, false)
		{
		}

		protected override void InternalCopyFrom(IProperty srcProperty)
		{
			IBodyContentProperty bodyContentProperty = srcProperty as IBodyContentProperty;
			if (bodyContentProperty == null)
			{
				throw new UnexpectedTypeException("IBodyContentProperty", srcProperty);
			}
			try
			{
				bodyContentProperty.PreProcessProperty();
				base.InternalCopyFrom(srcProperty);
			}
			finally
			{
				bodyContentProperty.PostProcessProperty();
			}
		}
	}
}
