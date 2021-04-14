using System;

namespace Microsoft.Exchange.Inference.Common
{
	internal enum BucketedfeatureIdentityTypes : byte
	{
		None,
		SimpleIdentityOfInt,
		SimpleIdentityOfString,
		MdbRecipientIdentity,
		HashedIdentity
	}
}
