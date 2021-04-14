using System;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.WebService
{
	public class ResultItem
	{
		public ResultItem()
		{
			this.Index = -1;
		}

		internal ResultVerifyMethod VerifyMethod
		{
			get
			{
				return this.method;
			}
			set
			{
				this.method = value;
			}
		}

		internal string PropertyName
		{
			get
			{
				return this.propertyName;
			}
			set
			{
				this.propertyName = value;
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

		internal int Index
		{
			get
			{
				return this.index;
			}
			set
			{
				this.index = value;
			}
		}

		internal bool UseFile
		{
			get
			{
				return this.useFile;
			}
			set
			{
				this.useFile = value;
			}
		}

		public override string ToString()
		{
			return string.Format("VerifyMethod={0}, Index={1}, PropertyName={2}, Value={3}, UseFile={4}", new object[]
			{
				this.VerifyMethod,
				this.Index,
				this.PropertyName,
				this.Value,
				this.UseFile
			});
		}

		private ResultVerifyMethod method;

		private string propertyName;

		private string value;

		private int index;

		private bool useFile;
	}
}
