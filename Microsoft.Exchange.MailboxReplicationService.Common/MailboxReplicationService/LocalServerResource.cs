using System;
using System.Collections.Generic;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal abstract class LocalServerResource : WlmResource
	{
		public LocalServerResource(WorkloadType workloadType) : base(workloadType)
		{
			base.ResourceGuid = LocalServerResource.ResourceId;
		}

		public override string ResourceName
		{
			get
			{
				return "LocalServer";
			}
		}

		public override List<WlmResourceHealthMonitor> GetWlmResources()
		{
			return new List<WlmResourceHealthMonitor>(2)
			{
				new LocalCPUHealthMonitor(this),
				new ADReplicationHealthMonitor(this)
			};
		}

		public const string LocalServerResourceName = "LocalServer";

		internal static readonly Guid ResourceId = new Guid("17bd66db-d063-4641-b8ae-317c2b16c386");
	}
}
