using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	internal interface IAttachmentsProperty : IMultivaluedProperty<AttachmentData>, IProperty, IEnumerable<AttachmentData>, IEnumerable
	{
	}
}
