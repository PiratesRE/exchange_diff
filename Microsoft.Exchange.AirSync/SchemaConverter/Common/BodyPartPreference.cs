using System;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	internal class BodyPartPreference : BodyPreference
	{
		public override BodyType Type
		{
			get
			{
				return base.Type;
			}
			set
			{
				if (value != BodyType.Html)
				{
					throw new AirSyncPermanentException(StatusCode.BodyPartPreferenceTypeNotSupported, null, false)
					{
						ErrorStringForProtocolLogger = "InvalidBodyPartBodyType"
					};
				}
				base.Type = value;
			}
		}
	}
}
