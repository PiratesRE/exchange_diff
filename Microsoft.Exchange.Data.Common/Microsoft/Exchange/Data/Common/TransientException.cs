using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Data.Common
{
	[Serializable]
	public class TransientException : LocalizedException
	{
		public TransientException(LocalizedString localizedString) : this(localizedString, null)
		{
		}

		public TransientException(LocalizedString localizedString, Exception innerException) : base(localizedString, innerException)
		{
			this.localizedString = localizedString;
		}

		protected TransientException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context)
		{
		}

		public new LocalizedString LocalizedString
		{
			get
			{
				return this.localizedString;
			}
		}

		private LocalizedString localizedString;
	}
}
