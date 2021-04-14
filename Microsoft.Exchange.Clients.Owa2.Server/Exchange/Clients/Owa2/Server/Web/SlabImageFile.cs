using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Web
{
	public class SlabImageFile : LayoutDependentResource
	{
		public string Name { get; set; }

		public string Type { get; set; }

		public bool IsThemed()
		{
			return this.Type != null && this.Type.Equals("Themed", StringComparison.OrdinalIgnoreCase);
		}

		public override bool Equals(object obj)
		{
			SlabImageFile slabImageFile = obj as SlabImageFile;
			return slabImageFile != null && this.Type == slabImageFile.Type && this.Name == slabImageFile.Name && base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return this.Type.GetHashCode() ^ this.Name.GetHashCode() ^ base.GetHashCode();
		}
	}
}
