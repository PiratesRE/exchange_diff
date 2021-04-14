using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class PlatformSipUri
	{
		public abstract string Host { get; set; }

		public abstract int Port { get; set; }

		public abstract string User { get; set; }

		public abstract UserParameter UserParameter { get; set; }

		public abstract TransportParameter TransportParameter { get; set; }

		public abstract string SimplifiedUri { get; }

		public abstract void AddParameter(string name, string value);

		public abstract string FindParameter(string name);

		public abstract void RemoveParameter(string name);

		public abstract IEnumerable<PlatformSipUriParameter> GetParametersThatHaveValues();

		public abstract override string ToString();
	}
}
