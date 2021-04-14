using System;
using System.ComponentModel;

namespace Microsoft.Exchange.Data
{
	[ImmutableObject(true)]
	public abstract class Protocol
	{
		protected Protocol(string protocolName, string displayName)
		{
			if (protocolName == null)
			{
				throw new ArgumentNullException("protocolName");
			}
			if (protocolName.IndexOf(':') != -1)
			{
				throw new ArgumentException(DataStrings.ExceptionInvlidCharInProtocolName);
			}
			this.protocolName = protocolName;
			this.displayName = displayName;
		}

		public string ProtocolName
		{
			get
			{
				return this.protocolName;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
		}

		public sealed override string ToString()
		{
			return this.protocolName;
		}

		private string protocolName;

		private string displayName;
	}
}
