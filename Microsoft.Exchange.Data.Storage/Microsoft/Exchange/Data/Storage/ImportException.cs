using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ImportException : StoragePermanentException
	{
		public ImportException(LocalizedString message, ImportResult importResult) : base(message)
		{
			EnumValidator.ThrowIfInvalid<ImportResult>(importResult, "importResult");
			this.importResult = importResult;
		}

		public ImportResult ImportResult
		{
			get
			{
				return this.importResult;
			}
		}

		private readonly ImportResult importResult;
	}
}
