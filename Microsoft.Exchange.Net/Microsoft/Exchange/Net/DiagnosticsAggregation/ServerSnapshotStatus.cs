using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Net.DiagnosticsAggregation
{
	[DataContract]
	internal class ServerSnapshotStatus
	{
		public ServerSnapshotStatus(string serverIdentity)
		{
			this.ServerIdentity = serverIdentity;
		}

		[DataMember(IsRequired = true)]
		public string ServerIdentity { get; private set; }

		[DataMember(IsRequired = true)]
		public DateTime? TimeOfLastSuccess { get; set; }

		[DataMember(IsRequired = true)]
		public DateTime? TimeOfLastFailure { get; set; }

		[DataMember(IsRequired = true)]
		public string LastError { get; set; }

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "ServerIdentity: [{0}] TimeOfLastSuccess: [{1}] TimeOfLastError: [{2}] LastError: [{3}]", new object[]
			{
				this.ServerIdentity,
				(this.TimeOfLastSuccess != null) ? this.TimeOfLastSuccess.ToString() : string.Empty,
				(this.TimeOfLastFailure != null) ? this.TimeOfLastFailure.ToString() : string.Empty,
				(this.LastError != null) ? this.LastError : string.Empty
			});
		}
	}
}
