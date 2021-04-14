using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Migration;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public class MigrationError
	{
		public MigrationError(MigrationError error)
		{
			this.EmailAddress = error.EmailAddress;
			this.LocalizedErrorMessage = error.LocalizedErrorMessage;
		}

		protected MigrationError()
		{
		}

		public string EmailAddress { get; internal set; }

		public LocalizedString LocalizedErrorMessage
		{
			get
			{
				LocalizedString? localizedString = this.localizedErrorMessage;
				if (localizedString == null)
				{
					return Strings.UnknownMigrationError;
				}
				return localizedString.GetValueOrDefault();
			}
			internal set
			{
				this.localizedErrorMessage = new LocalizedString?(value);
			}
		}

		public override string ToString()
		{
			return ServerStrings.MigrationErrorString(this.EmailAddress, this.LocalizedErrorMessage);
		}

		private LocalizedString? localizedErrorMessage;
	}
}
