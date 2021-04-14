using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Clients.Common;

namespace Microsoft.Exchange.Clients.Owa2.Server.Web
{
	[DataContract]
	public class LayoutDependentResource
	{
		[DataMember(Name = "layout", EmitDefaultValue = false)]
		public string LayoutString
		{
			get
			{
				if (this.Layout != ResourceLayout.Any)
				{
					return this.Layout.ToString();
				}
				return null;
			}
			set
			{
				this.Layout = (ResourceLayout)Enum.Parse(typeof(ResourceLayout), value, true);
			}
		}

		public ResourceLayout Layout { get; set; }

		public bool IsForLayout(LayoutType layout)
		{
			return this.Layout == ResourceLayout.Any || (this.Layout == ResourceLayout.Mouse && layout == LayoutType.Mouse) || (this.Layout == ResourceLayout.TNarrow && layout == LayoutType.TouchNarrow) || (this.Layout == ResourceLayout.TWide && layout == LayoutType.TouchWide);
		}

		public override bool Equals(object obj)
		{
			LayoutDependentResource layoutDependentResource = obj as LayoutDependentResource;
			return layoutDependentResource != null && this.Layout == layoutDependentResource.Layout;
		}

		public override int GetHashCode()
		{
			return this.Layout.GetHashCode();
		}
	}
}
