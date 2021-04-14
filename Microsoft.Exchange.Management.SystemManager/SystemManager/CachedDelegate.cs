using System;
using System.Data;

namespace Microsoft.Exchange.Management.SystemManager
{
	internal class CachedDelegate
	{
		internal Delegate CompiledDelegate { get; set; }

		internal DataRow TemplateDataRow { get; set; }

		internal DataRow TemplateInputRow { get; set; }
	}
}
