using System;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public struct NameValuePair
	{
		internal NameValuePair(string name, string value)
		{
			this.name = name;
			this.value = value;
		}

		internal string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		internal string Value
		{
			get
			{
				return this.value;
			}
			set
			{
				this.value = value;
			}
		}

		private string name;

		private string value;
	}
}
