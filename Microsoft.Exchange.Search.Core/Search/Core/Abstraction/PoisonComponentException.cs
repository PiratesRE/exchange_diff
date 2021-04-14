using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	[Serializable]
	internal class PoisonComponentException : ComponentFailedException
	{
		public PoisonComponentException() : base(Strings.OperationFailure)
		{
		}

		public PoisonComponentException(Exception innerException) : base(Strings.OperationFailure, innerException)
		{
		}

		public PoisonComponentException(LocalizedString message) : base(message)
		{
		}

		public PoisonComponentException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected PoisonComponentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		internal override void RethrowNewInstance()
		{
			throw new ComponentFailedPermanentException(this);
		}
	}
}
