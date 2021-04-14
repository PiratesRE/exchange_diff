using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;

namespace Microsoft.Exchange.AirSync.SchemaConverter.AirSync
{
	[Serializable]
	internal class AirSyncReminder141Property : AirSyncReminderProperty
	{
		public AirSyncReminder141Property(string xmlNodeNamespace, string airSyncTagName, bool requiresClientSupport) : base(xmlNodeNamespace, airSyncTagName, requiresClientSupport)
		{
		}

		protected override void InternalCopyFrom(IProperty srcProperty)
		{
			base.InternalCopyFrom(srcProperty);
			IIntegerProperty integerProperty = (IIntegerProperty)srcProperty;
			if (integerProperty.IntegerData == -1)
			{
				base.CreateAirSyncNode(string.Empty, true);
			}
		}
	}
}
