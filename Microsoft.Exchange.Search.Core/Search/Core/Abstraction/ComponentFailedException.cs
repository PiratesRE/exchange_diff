using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	[Serializable]
	internal abstract class ComponentFailedException : ComponentException
	{
		protected ComponentFailedException(LocalizedString message) : base(message)
		{
		}

		protected ComponentFailedException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected ComponentFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		protected ComponentFailedException(ComponentFailedException other) : base(other.LocalizedString, other.InnerException)
		{
		}

		internal abstract void RethrowNewInstance();
	}
}
