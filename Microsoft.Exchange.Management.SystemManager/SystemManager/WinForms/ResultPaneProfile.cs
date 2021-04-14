using System;
using System.ComponentModel;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ResultPaneProfile : IHasPermission
	{
		[TypeConverter(typeof(DDIObjectTypeConverter))]
		public Type Type { get; set; }

		public virtual bool HasPermission()
		{
			return false;
		}
	}
}
