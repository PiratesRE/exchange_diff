using System;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal interface IValidationErrorSupport
	{
		DirectoryPropertyXmlValidationError ValidationError { get; set; }
	}
}
