using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class StartMailboxSearchParameters : WebServiceParameters
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Start-MailboxSearch";
			}
		}

		public override string RbacScope
		{
			get
			{
				return string.Empty;
			}
		}

		[DataMember]
		public bool Resume
		{
			get
			{
				return (bool)(base[SearchObjectSchema.Resume] ?? false);
			}
			set
			{
				base[SearchObjectSchema.Resume] = value;
			}
		}

		[DataMember]
		public int StatisticsStartIndex
		{
			get
			{
				return (int)(base[MailboxDiscoverySearchSchema.StatisticsStartIndex.Name] ?? 1);
			}
			set
			{
				base[MailboxDiscoverySearchSchema.StatisticsStartIndex.Name] = value;
			}
		}

		[DataMember]
		public bool Force
		{
			get
			{
				return this.ShouldContinue;
			}
			set
			{
				this.ShouldContinue = value;
			}
		}
	}
}
