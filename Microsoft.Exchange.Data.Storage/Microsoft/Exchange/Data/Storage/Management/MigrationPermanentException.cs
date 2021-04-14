using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public class MigrationPermanentException : AnchorLocalizedExceptionBase
	{
		public MigrationPermanentException(LocalizedString localizedErrorMessage, string internalError, Exception ex) : base(localizedErrorMessage, internalError, ex)
		{
		}

		public MigrationPermanentException(LocalizedString localizedErrorMessage, string internalError) : base(localizedErrorMessage, internalError)
		{
		}

		public MigrationPermanentException(LocalizedString localizedErrorMessage) : base(localizedErrorMessage, null)
		{
		}

		public MigrationPermanentException(LocalizedString localizedErrorMessage, Exception ex) : base(localizedErrorMessage, null, ex)
		{
		}

		protected MigrationPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override string StackTrace
		{
			get
			{
				if (!string.IsNullOrEmpty(base.StackTrace))
				{
					return base.StackTrace;
				}
				return this.createdStack.ToString();
			}
		}

		private readonly StackTrace createdStack = new StackTrace();
	}
}
