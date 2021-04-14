using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.SystemProbeTasks
{
	[Cmdlet("New", "EncryptedSystemProbeGuid")]
	[OutputType(new Type[]
	{
		typeof(SystemProbeData)
	})]
	public sealed class NewEncryptedSystemProbeGuid : Task
	{
		[Parameter(Mandatory = false)]
		public Guid? Guid
		{
			get
			{
				return new Guid?(this.guid ?? System.Guid.Empty);
			}
			set
			{
				this.guid = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime TimeStamp
		{
			get
			{
				DateTime? dateTime = this.time;
				if (dateTime == null)
				{
					return DateTime.MinValue;
				}
				return dateTime.GetValueOrDefault();
			}
			set
			{
				this.time = new DateTime?(value);
			}
		}

		protected override void InternalProcessRecord()
		{
			if (this.guid == null)
			{
				this.guid = new Guid?(System.Guid.NewGuid());
			}
			if (this.time == null)
			{
				this.time = new DateTime?(DateTime.UtcNow);
			}
			string cypherText = SystemProbeId.EncryptProbeGuid(this.guid.Value, this.time.Value);
			base.WriteObject(this.CreateSystemProbeData(this.guid.Value, this.time.Value, cypherText));
		}

		private SystemProbeData CreateSystemProbeData(Guid guid, DateTime timeStamp, string cypherText)
		{
			return new SystemProbeData
			{
				Guid = guid,
				TimeStamp = timeStamp,
				Text = cypherText
			};
		}

		private Guid? guid;

		private DateTime? time;
	}
}
