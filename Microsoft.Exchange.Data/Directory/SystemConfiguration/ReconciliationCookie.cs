using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public sealed class ReconciliationCookie
	{
		public int Version { get; private set; }

		public string DCHostName { get; private set; }

		public Guid InvocationId { get; private set; }

		public long HighestCommittedUsn { get; private set; }

		public ReconciliationCookie(int version, string dcHostName, Guid invocationId, long highestCommittedUsn)
		{
			this.Version = version;
			this.DCHostName = dcHostName;
			this.InvocationId = invocationId;
			this.HighestCommittedUsn = highestCommittedUsn;
		}

		public ReconciliationCookie(string value)
		{
			string[] array = value.Split(new char[]
			{
				';'
			});
			if (array.Length == 4)
			{
				this.Version = Convert.ToInt32(array[0]);
				this.DCHostName = array[1];
				this.InvocationId = new Guid(array[2]);
				this.HighestCommittedUsn = Convert.ToInt64(array[3]);
				return;
			}
			throw new FormatException(string.Format("Incorrect format for ReconciliationCookie: {0}", value));
		}

		public static ReconciliationCookie Parse(string value)
		{
			return new ReconciliationCookie(value);
		}

		public override string ToString()
		{
			return string.Format("{0};{1};{2};{3}", new object[]
			{
				this.Version,
				this.DCHostName,
				this.InvocationId,
				this.HighestCommittedUsn
			});
		}
	}
}
