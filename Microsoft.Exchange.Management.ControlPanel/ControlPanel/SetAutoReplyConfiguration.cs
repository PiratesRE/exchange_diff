using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.InfoWorker.Common.OOF;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetAutoReplyConfiguration : SetObjectProperties
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Set-MailboxAutoReplyConfiguration";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@W:Self";
			}
		}

		[DataMember]
		public string AutoReplyStateDisabled { get; set; }

		[DataMember]
		public bool AutoReplyStateScheduled { get; set; }

		[DataMember]
		public string StartTime { get; set; }

		[DataMember]
		public string EndTime { get; set; }

		[DataMember]
		public string InternalMessage
		{
			get
			{
				return (string)base["InternalMessage"];
			}
			set
			{
				base["InternalMessage"] = value;
			}
		}

		[DataMember]
		public bool? ExternalAudience { get; set; }

		[DataMember]
		public string ExternalAudienceKnownOnly { get; set; }

		[DataMember]
		public string ExternalMessage
		{
			get
			{
				return (string)base["ExternalMessage"];
			}
			set
			{
				base["ExternalMessage"] = value;
			}
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			bool flag;
			if (bool.TryParse(this.AutoReplyStateDisabled, out flag))
			{
				if (flag)
				{
					base["AutoReplyState"] = OofState.Disabled;
				}
				else
				{
					base["AutoReplyState"] = (this.AutoReplyStateScheduled ? OofState.Scheduled : OofState.Enabled);
				}
			}
			bool flag2;
			if (this.ExternalAudience != null && !this.ExternalAudience.Value)
			{
				base["ExternalAudience"] = Microsoft.Exchange.InfoWorker.Common.OOF.ExternalAudience.None;
			}
			else if (!this.ExternalAudienceKnownOnly.IsNullOrBlank() && bool.TryParse(this.ExternalAudienceKnownOnly, out flag2))
			{
				base["ExternalAudience"] = (flag2 ? Microsoft.Exchange.InfoWorker.Common.OOF.ExternalAudience.Known : Microsoft.Exchange.InfoWorker.Common.OOF.ExternalAudience.All);
			}
			if (!this.StartTime.IsNullOrBlank())
			{
				ExDateTime? exDateTime = this.StartTime.ToEcpExDateTime("yyyy/MM/dd HH:mm:ss");
				if (exDateTime != null)
				{
					base["StartTime"] = exDateTime.Value.UniversalTime;
				}
			}
			if (!this.EndTime.IsNullOrBlank())
			{
				ExDateTime? exDateTime2 = this.EndTime.ToEcpExDateTime("yyyy/MM/dd HH:mm:ss");
				if (exDateTime2 != null)
				{
					base["EndTime"] = exDateTime2.Value.UniversalTime;
				}
			}
		}
	}
}
