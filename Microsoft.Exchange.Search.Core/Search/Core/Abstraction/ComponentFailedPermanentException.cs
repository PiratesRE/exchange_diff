using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	[Serializable]
	internal class ComponentFailedPermanentException : ComponentFailedException
	{
		public ComponentFailedPermanentException() : base(Strings.ComponentCriticalFailure)
		{
		}

		public ComponentFailedPermanentException(Exception innerException) : base(Strings.ComponentCriticalFailure, innerException)
		{
		}

		public ComponentFailedPermanentException(LocalizedString message) : base(message)
		{
		}

		public ComponentFailedPermanentException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected ComponentFailedPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		protected ComponentFailedPermanentException(ComponentFailedPermanentException other) : base(other)
		{
		}

		internal override void RethrowNewInstance()
		{
			throw new ComponentFailedPermanentException(this);
		}
	}
}
