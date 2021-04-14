using System;

namespace Microsoft.Exchange.AirSync
{
	internal class TagNode
	{
		public TagNode(ushort nameSpace, ushort tag)
		{
			this.nameSpace = nameSpace;
			this.tag = tag;
		}

		public ushort NameSpace
		{
			get
			{
				return this.nameSpace;
			}
			set
			{
				this.nameSpace = value;
			}
		}

		public ushort Tag
		{
			get
			{
				return this.tag;
			}
			set
			{
				this.tag = value;
			}
		}

		private ushort nameSpace;

		private ushort tag;
	}
}
