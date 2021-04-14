using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	[Serializable]
	internal class XsoBccProperty : XsoRecipientProperty
	{
		public XsoBccProperty(PropertyType type) : base(RecipientItemType.Bcc, type)
		{
		}
	}
}
