using System;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal abstract class GreetingBase : DisposableBase
	{
		internal abstract string Name { get; }

		internal abstract ITempWavFile Get();

		internal abstract void Put(string sourceFileName);

		internal abstract void Delete();

		internal abstract bool Exists();
	}
}
