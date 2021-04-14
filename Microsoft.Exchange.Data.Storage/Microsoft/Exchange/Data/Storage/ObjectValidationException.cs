using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ObjectValidationException : CorruptDataException
	{
		public ObjectValidationException(LocalizedString message, IList<StoreObjectValidationError> errors) : base(message)
		{
			this.errors = errors;
		}

		public IList<StoreObjectValidationError> Errors
		{
			get
			{
				return this.errors;
			}
		}

		private readonly IList<StoreObjectValidationError> errors;
	}
}
