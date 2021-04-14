using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Web
{
	[DataContract]
	public class SlabConfiguration : LayoutDependentResource
	{
		[DataMember(Name = "type")]
		public string Type { get; set; }

		public override bool Equals(object obj)
		{
			SlabConfiguration slabConfiguration = obj as SlabConfiguration;
			return slabConfiguration != null && this.Type == slabConfiguration.Type && base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return this.Type.GetHashCode() ^ base.GetHashCode();
		}
	}
}
