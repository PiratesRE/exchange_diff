using System;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.PropTags
{
	public sealed class StoreNamedPropInfo : StorePropInfo
	{
		public StoreNamedPropInfo(StorePropName propName) : this(null, propName)
		{
		}

		public StoreNamedPropInfo(string descriptiveName, StorePropName propName) : this(descriptiveName, propName, PropertyType.Invalid, 9223372036854775808UL, PropertyCategories.Empty)
		{
		}

		public StoreNamedPropInfo(string descriptiveName, StorePropName propName, PropertyType propType, ulong groupMask, PropertyCategories categories) : base(descriptiveName, 32768, propType, StorePropInfo.Flags.None, groupMask, categories)
		{
			this.propName = propName;
		}

		public override StorePropName PropName
		{
			get
			{
				return this.propName;
			}
		}

		public override ushort PropId
		{
			get
			{
				Globals.AssertRetail(false, "We should not call PropId on a StoreNamedPropInfo");
				return 0;
			}
		}

		public override void AppendToString(StringBuilder sb)
		{
			base.AppendToString(sb);
			sb.Append("(");
			this.propName.AppendToString(sb);
			sb.Append(")");
		}

		internal const StorePropInfo.Flags NamedPropFlags = StorePropInfo.Flags.None;

		private StorePropName propName;
	}
}
