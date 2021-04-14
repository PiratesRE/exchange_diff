using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Web
{
	[DataContract]
	public class SlabStyleFile : LayoutDependentResource
	{
		[DataMember(Name = "name")]
		public string Name { get; set; }

		[DataMember(Name = "type", EmitDefaultValue = false)]
		public string Type { get; set; }

		public bool IsNotThemed()
		{
			return this.Type != null && this.Type.Equals("NotThemed", StringComparison.OrdinalIgnoreCase);
		}

		public bool IsSprite()
		{
			return this.Type != null && (this.Type.Equals("Sprite", StringComparison.OrdinalIgnoreCase) || this.Type.Equals("HighResolution", StringComparison.OrdinalIgnoreCase));
		}

		public bool IsHighResolutionSprite()
		{
			return this.Type != null && this.Type.Equals("HighResolution", StringComparison.OrdinalIgnoreCase);
		}

		public override bool Equals(object obj)
		{
			SlabStyleFile slabStyleFile = obj as SlabStyleFile;
			return slabStyleFile != null && this.Type == slabStyleFile.Type && this.Name == slabStyleFile.Name && base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return this.Type.GetHashCode() ^ this.Name.GetHashCode() ^ base.GetHashCode();
		}
	}
}
