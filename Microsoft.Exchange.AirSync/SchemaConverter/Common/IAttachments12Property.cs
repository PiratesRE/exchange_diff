using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	internal interface IAttachments12Property : IMultivaluedProperty<Attachment12Data>, IProperty, IEnumerable<Attachment12Data>, IEnumerable
	{
	}
}
