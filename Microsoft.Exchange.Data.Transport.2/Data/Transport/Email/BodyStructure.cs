using System;

namespace Microsoft.Exchange.Data.Transport.Email
{
	internal enum BodyStructure
	{
		Undefined,
		None,
		SingleBody,
		AlternativeBodies,
		SingleBodyWithRelatedAttachments,
		AlternativeBodiesWithMhtml,
		AlternativeBodiesWithSharedAttachments
	}
}
