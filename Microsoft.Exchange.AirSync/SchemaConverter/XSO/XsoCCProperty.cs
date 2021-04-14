using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	[Serializable]
	internal class XsoCCProperty : XsoRecipientProperty
	{
		public XsoCCProperty(PropertyType type) : base(RecipientItemType.Cc, type)
		{
		}
	}
}
