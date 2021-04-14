using System;

namespace Microsoft.Exchange.Inference.Common
{
	public interface IHashProvider
	{
		bool IsInitialized { get; }

		bool Initialize();

		string HashString(string input);
	}
}
