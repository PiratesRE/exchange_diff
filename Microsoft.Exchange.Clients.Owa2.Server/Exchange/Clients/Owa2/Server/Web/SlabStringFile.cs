using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Web
{
	[DataContract]
	public class SlabStringFile : SlabSourceFile
	{
		[DataMember(Name = "type", EmitDefaultValue = false)]
		public string Type { get; set; }

		public override bool Equals(object obj)
		{
			SlabStringFile slabStringFile = obj as SlabStringFile;
			return slabStringFile != null && this.Type == slabStringFile.Type && base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return this.Type.GetHashCode() ^ base.GetHashCode();
		}

		public bool IsExtensibility()
		{
			return this.Type != null && this.Type.Equals("Extensibility", StringComparison.OrdinalIgnoreCase);
		}

		public bool IsStandard()
		{
			return this.Type == null || this.Type.Equals("Standard", StringComparison.OrdinalIgnoreCase);
		}
	}
}
