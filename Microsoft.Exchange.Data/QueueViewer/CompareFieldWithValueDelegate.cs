using System;

namespace Microsoft.Exchange.Data.QueueViewer
{
	internal delegate int CompareFieldWithValueDelegate<ObjectType>(ObjectType dataObject, object value) where ObjectType : PagedDataObject;
}
