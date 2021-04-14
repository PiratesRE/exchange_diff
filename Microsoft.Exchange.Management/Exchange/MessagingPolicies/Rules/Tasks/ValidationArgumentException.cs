using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	internal class ValidationArgumentException : ArgumentException
	{
		public ValidationArgumentException(LocalizedString localizedString, Exception innerException = null) : base(localizedString, innerException)
		{
			this.localizedString = localizedString;
		}

		protected ValidationArgumentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public LocalizedString LocalizedString
		{
			get
			{
				return this.localizedString;
			}
		}

		private readonly LocalizedString localizedString;
	}
}
