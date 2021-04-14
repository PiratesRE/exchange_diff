using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	internal class XsoReadFlagProperty : XsoBooleanProperty, IBooleanProperty, IProperty
	{
		public XsoReadFlagProperty(StorePropertyDefinition propertyDef) : base(propertyDef)
		{
		}

		protected override void InternalCopyFromModified(IProperty srcProperty)
		{
			base.InternalCopyFromModified(srcProperty);
			if (Command.CurrentCommand != null)
			{
				Command.CurrentCommand.AddInteractiveCall();
			}
		}
	}
}
