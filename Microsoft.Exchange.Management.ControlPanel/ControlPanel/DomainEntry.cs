using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class DomainEntry
	{
		internal DomainEntry(string domain)
		{
			this.address = domain;
		}

		[DataMember]
		public string Address
		{
			get
			{
				return this.address;
			}
			set
			{
				this.address = value;
			}
		}

		public override bool Equals(object obj)
		{
			DomainEntry domainEntry = obj as DomainEntry;
			return domainEntry != null && string.Compare(domainEntry.Address, this.Address, true) == 0;
		}

		public override int GetHashCode()
		{
			return this.Address.GetHashCode();
		}

		private string address;
	}
}
