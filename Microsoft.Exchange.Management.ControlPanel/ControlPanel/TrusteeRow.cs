using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class TrusteeRow : BaseRow
	{
		public TrusteeRow(string name) : base(new Identity(name, name), null)
		{
			this.DisplayName = name;
		}

		[DataMember]
		public string DisplayName { get; private set; }
	}
}
