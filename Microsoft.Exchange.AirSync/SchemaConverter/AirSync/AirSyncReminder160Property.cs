using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;

namespace Microsoft.Exchange.AirSync.SchemaConverter.AirSync
{
	[Serializable]
	internal class AirSyncReminder160Property : AirSyncReminder141Property, IReminder160Property, IIntegerProperty, IProperty
	{
		public AirSyncReminder160Property(string xmlNodeNamespace, string airSyncTagName, bool requiresClientSupport) : base(xmlNodeNamespace, airSyncTagName, requiresClientSupport)
		{
		}

		public bool ReminderIsSet
		{
			get
			{
				return base.State != PropertyState.SetToDefault && !string.IsNullOrEmpty(base.XmlNode.InnerText);
			}
		}

		public override int IntegerData
		{
			get
			{
				if (this.ReminderIsSet)
				{
					return base.IntegerData;
				}
				return -1;
			}
		}
	}
}
