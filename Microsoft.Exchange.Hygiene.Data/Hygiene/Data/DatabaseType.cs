using System;

namespace Microsoft.Exchange.Hygiene.Data
{
	public enum DatabaseType
	{
		Directory = 1,
		Spam,
		Domain,
		Reporting = 5,
		Mtrt,
		Kes = 10,
		BackgroundJobBackend,
		Optics = 13,
		KEStore
	}
}
