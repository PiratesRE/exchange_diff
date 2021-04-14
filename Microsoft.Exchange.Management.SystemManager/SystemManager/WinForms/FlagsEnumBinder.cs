using System;
using System.Windows.Forms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class FlagsEnumBinder<T> where T : struct
	{
		public FlagsEnumBinder(object dataSource, string dataMember)
		{
			this.dataSource = dataSource;
			this.dataMember = dataMember;
		}

		public void BindCheckBoxToFlag(AutoHeightCheckBox checkBox, T flag)
		{
			if (checkBox == null)
			{
				throw new ArgumentNullException("checkBox");
			}
			ulong flagValue = Convert.ToUInt64(flag);
			Binding binding = checkBox.DataBindings.Add("Checked", this.dataSource, this.dataMember, true, DataSourceUpdateMode.OnPropertyChanged);
			binding.Format += delegate(object sender, ConvertEventArgs e)
			{
				this.currentEnumValue = ((e.Value == null) ? 0UL : Convert.ToUInt64(e.Value));
				e.Value = ((this.currentEnumValue & flagValue) == flagValue);
			};
			binding.Parse += delegate(object sender, ConvertEventArgs e)
			{
				if (true.Equals(e.Value))
				{
					e.Value = Enum.ToObject(typeof(T), this.currentEnumValue | flagValue);
					return;
				}
				e.Value = Enum.ToObject(typeof(T), this.currentEnumValue & ~flagValue);
			};
		}

		private ulong currentEnumValue;

		private object dataSource;

		private string dataMember;
	}
}
