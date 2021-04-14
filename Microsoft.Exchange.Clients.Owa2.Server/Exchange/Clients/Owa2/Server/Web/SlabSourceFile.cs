using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Web
{
	[DataContract]
	public class SlabSourceFile : LayoutDependentResource
	{
		[DataMember(Name = "name")]
		public string Name { get; set; }

		public override bool Equals(object obj)
		{
			SlabSourceFile slabSourceFile = obj as SlabSourceFile;
			return slabSourceFile != null && this.Name == slabSourceFile.Name && base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return this.Name.GetHashCode() ^ base.GetHashCode();
		}
	}
}
