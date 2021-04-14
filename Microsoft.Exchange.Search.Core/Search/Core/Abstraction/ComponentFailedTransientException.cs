using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	[Serializable]
	internal class ComponentFailedTransientException : ComponentFailedException
	{
		public ComponentFailedTransientException() : base(Strings.ComponentFailure)
		{
		}

		public ComponentFailedTransientException(Exception innerException) : base(Strings.ComponentFailure, innerException)
		{
		}

		public ComponentFailedTransientException(LocalizedString message) : base(message)
		{
		}

		public ComponentFailedTransientException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected ComponentFailedTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		protected ComponentFailedTransientException(ComponentFailedTransientException other) : base(other)
		{
		}

		internal override void RethrowNewInstance()
		{
			throw new ComponentFailedTransientException(this);
		}
	}
}
