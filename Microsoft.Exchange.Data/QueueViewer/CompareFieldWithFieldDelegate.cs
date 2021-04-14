using System;

namespace Microsoft.Exchange.Data.QueueViewer
{
	internal delegate int CompareFieldWithFieldDelegate<ObjectType>(ObjectType dataObject1, ObjectType dataObject2) where ObjectType : PagedDataObject;
}
