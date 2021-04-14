using System;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal delegate TaskCompletion StageHandler(TransportMailItem transportMailItem, TaskContext taskContext);
}
