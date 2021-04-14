using System;

namespace Microsoft.Exchange.Data.QueueViewer
{
	internal delegate bool MatchFieldWithTextDelegate<ObjectType>(ObjectType dataObject, object matchPattern, MatchOptions matchOptions) where ObjectType : PagedDataObject;
}
