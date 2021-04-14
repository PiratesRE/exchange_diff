using System;

namespace Microsoft.Exchange.Configuration.PswsProxy
{
	internal class ResponseErrorRecord
	{
		internal string FullyQualifiedErrorId { get; set; }

		internal ResponseCategoryInfo CategoryInfo { get; set; }

		internal ResponseErrorDetail ErrorDetail { get; set; }

		internal string Exception { get; set; }
	}
}
