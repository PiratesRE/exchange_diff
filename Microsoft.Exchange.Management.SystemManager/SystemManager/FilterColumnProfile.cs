using System;
using Microsoft.Exchange.Data;
using Microsoft.ManagementGUI;

namespace Microsoft.Exchange.Management.SystemManager
{
	public class FilterColumnProfile
	{
		public string PickerProfile { get; set; }

		public string DisplayMember { get; set; }

		public string ValueMember { get; set; }

		internal ProviderPropertyDefinition PropertyDefinition { get; set; }

		public string Name { get; set; }

		public PropertyFilterOperator[] Operators { get; set; }

		public DisplayFormatMode FormatMode { get; set; }

		public Type ColumnType { get; set; }

		public ObjectListSource FilterableListSource { get; set; }

		public string RefDisplayedColumn { get; set; }
	}
}
