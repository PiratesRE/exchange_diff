using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public class AnchorRuntimeException : AnchorLocalizedExceptionBase
	{
		public AnchorRuntimeException(LocalizedString localizedErrorMessage, string internalError, Exception ex) : base(localizedErrorMessage, internalError, ex)
		{
		}

		public AnchorRuntimeException(LocalizedString localizedErrorMessage, string internalError) : base(localizedErrorMessage, internalError)
		{
		}

		public AnchorRuntimeException(LocalizedString localizedErrorMessage, Exception ex) : base(localizedErrorMessage, null, ex)
		{
		}

		public AnchorRuntimeException(LocalizedString localizedErrorMessage) : base(localizedErrorMessage, null)
		{
		}

		protected AnchorRuntimeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
