using System;

namespace Microsoft.Exchange.Search.Mdb
{
	internal class MdbCopy
	{
		public MdbCopy(string name, int activationPreference, int schemaVersion)
		{
			this.Name = name;
			this.ActivationPreference = activationPreference;
			this.SchemaVersion = schemaVersion;
		}

		public string Name { get; private set; }

		public int ActivationPreference { get; private set; }

		public int SchemaVersion { get; private set; }
	}
}
