using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal interface IProvidePropertyPaths
	{
		PropertyPath[] PropertyPaths { get; }

		string GetPropertyPathsString();
	}
}
