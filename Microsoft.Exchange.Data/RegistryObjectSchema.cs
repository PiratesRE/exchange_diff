using System;

namespace Microsoft.Exchange.Data
{
	internal abstract class RegistryObjectSchema : SimpleProviderObjectSchema
	{
		public abstract string DefaultRegistryKeyPath { get; }

		public abstract string DefaultName { get; }
	}
}
