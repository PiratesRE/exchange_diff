using System;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	public class QueryableActiveSetting
	{
		public QueryableActiveSetting(string name, bool isReadOnce, object value)
		{
			this.Name = name;
			this.Type = (isReadOnce ? QueryableActiveSetting.ReadOnceStatus.ReadOnce : QueryableActiveSetting.ReadOnceStatus.Dynamic);
			this.Value = value;
		}

		public string Name { get; private set; }

		public QueryableActiveSetting.ReadOnceStatus Type { get; private set; }

		public object Value { get; private set; }

		public enum ReadOnceStatus
		{
			ReadOnce,
			Dynamic
		}
	}
}
