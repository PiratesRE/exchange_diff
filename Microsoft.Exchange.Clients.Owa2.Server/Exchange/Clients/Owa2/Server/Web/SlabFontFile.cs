using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Web
{
	public class SlabFontFile : LayoutDependentResource
	{
		public string Name { get; set; }

		public override bool Equals(object obj)
		{
			SlabFontFile slabFontFile = obj as SlabFontFile;
			return slabFontFile != null && this.Name == slabFontFile.Name && base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return this.Name.GetHashCode() ^ base.GetHashCode();
		}
	}
}
