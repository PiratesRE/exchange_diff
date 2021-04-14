using System;

namespace Microsoft.Exchange.Security.Authorization
{
	internal struct FieldType
	{
		public const char AuthenticationType = 'A';

		public const char IsCompressed = 'C';

		public const char ExtensionData = 'E';

		public const char Group = 'G';

		public const char LogonName = 'L';

		public const char RestrictedGroup = 'R';

		public const char TokenType = 'T';

		public const char UserSid = 'U';

		public const char Version = 'V';
	}
}
