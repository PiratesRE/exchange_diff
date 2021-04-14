using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Web
{
	[DataContract]
	public class Slab
	{
		[DataMember(Order = 0, EmitDefaultValue = false)]
		public string[] Dependencies { get; set; }

		[DataMember(Order = 1)]
		public string[] Types { get; set; }

		[DataMember(Order = 2)]
		public string[] Templates { get; set; }

		[DataMember(Order = 3)]
		public SlabStyleFile[] Styles { get; set; }

		[DataMember(Order = 4)]
		public SlabConfiguration[] Configurations { get; set; }

		[DataMember(Order = 5)]
		public SlabSourceFile[] Sources { get; set; }

		[DataMember(Order = 6)]
		public SlabSourceFile[] PackagedSources { get; set; }

		[DataMember(Order = 7)]
		public SlabStringFile[] Strings { get; set; }

		[DataMember(Order = 8)]
		public SlabStringFile[] PackagedStrings { get; set; }

		public SlabFontFile[] Fonts { get; set; }

		public SlabImageFile[] Images { get; set; }
	}
}
