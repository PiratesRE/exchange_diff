using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class OptionsPropertyChangeTracker
	{
		private HashSet<string> PropertyNamesThatHaveChanged
		{
			get
			{
				if (this.propertyNamesThatHaveChanged == null)
				{
					this.propertyNamesThatHaveChanged = new HashSet<string>();
				}
				return this.propertyNamesThatHaveChanged;
			}
		}

		protected void TrackPropertyChanged([CallerMemberName] string propertyNameThatChanged = null)
		{
			if (!string.IsNullOrWhiteSpace(propertyNameThatChanged))
			{
				this.PropertyNamesThatHaveChanged.Add(propertyNameThatChanged);
			}
		}

		public bool HasPropertyChanged(string propertyName)
		{
			return this.propertyNamesThatHaveChanged != null && this.propertyNamesThatHaveChanged.Contains(propertyName);
		}

		public override string ToString()
		{
			IEnumerable<string> values = from p in base.GetType().GetProperties()
			select p.Name + " = " + OptionsPropertyChangeTracker.GetStringValue(p.GetValue(this, null));
			return string.Join(",  ", values);
		}

		private static string GetStringValue(object value)
		{
			if (value == null)
			{
				return "<null>";
			}
			IEnumerable enumerable = value as IEnumerable;
			if (!(value is string) && enumerable != null)
			{
				IEnumerable<string> values = from object e in enumerable
				select OptionsPropertyChangeTracker.GetStringValue(e);
				return "{" + string.Join(",", values) + "}";
			}
			return value.ToString();
		}

		private HashSet<string> propertyNamesThatHaveChanged;
	}
}
