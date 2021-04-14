using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	[Serializable]
	internal class CatalogReseedException : ComponentFailedException
	{
		public CatalogReseedException(IndexStatusErrorCode errorCode) : base(Strings.OperationFailure)
		{
			this.errorCode = errorCode;
		}

		public CatalogReseedException(Exception innerException) : base(Strings.OperationFailure, innerException)
		{
			this.errorCode = IndexStatusErrorCode.CatalogReseed;
		}

		public CatalogReseedException(LocalizedString message, IndexStatusErrorCode errorCode) : base(message)
		{
			this.errorCode = errorCode;
		}

		public CatalogReseedException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
			this.errorCode = IndexStatusErrorCode.CatalogReseed;
		}

		protected CatalogReseedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.errorCode = IndexStatusErrorCode.CatalogReseed;
		}

		internal IndexStatusErrorCode OriginalErrorCode
		{
			get
			{
				return this.errorCode;
			}
		}

		internal override void RethrowNewInstance()
		{
			throw new ComponentFailedPermanentException(this);
		}

		private IndexStatusErrorCode errorCode;
	}
}
