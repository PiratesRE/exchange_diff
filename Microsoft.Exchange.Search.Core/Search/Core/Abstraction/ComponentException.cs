using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	[Serializable]
	internal abstract class ComponentException : LocalizedException
	{
		protected ComponentException(LocalizedString message) : base(message)
		{
		}

		protected ComponentException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected ComponentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
