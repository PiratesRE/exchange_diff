using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.AAD;
using Microsoft.Exchange.UnifiedGroups;

namespace Microsoft.Exchange.Services.Wcf
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class GroupMembershipAADReader : IGroupMembershipReader<string>
	{
		public GroupMembershipAADReader(ADUser user, IGroupsLogger logger)
		{
			ArgumentValidator.ThrowIfNull("user", user);
			ArgumentValidator.ThrowIfNull("logger", logger);
			this.user = user;
			this.logger = logger;
			this.stopwatch = new Stopwatch();
		}

		public TimeSpan AADLatency
		{
			get
			{
				return this.stopwatch.Elapsed;
			}
		}

		public IEnumerable<string> GetJoinedGroups()
		{
			List<string> groups = null;
			this.stopwatch.Start();
			try
			{
				GrayException.MapAndReportGrayExceptions(delegate()
				{
					IAadClient aadClient = AADClientFactory.CreateIAadClient(this.user);
					if (aadClient != null)
					{
						aadClient.Timeout = 10;
						groups = aadClient.GetGroupMembership(this.user.ExternalDirectoryObjectId).ToList<string>();
						return;
					}
					this.logger.LogTrace("GroupMembershipAADReader.GetJoinedGroups - Unable to read groups from AAD: AADClient is null.", new object[0]);
				}, (Exception e) => GrayException.IsSystemGrayException(e));
			}
			finally
			{
				this.stopwatch.Stop();
			}
			return groups;
		}

		private const int AADTimeoutInSeconds = 10;

		private readonly ADUser user;

		private readonly Stopwatch stopwatch;

		private readonly IGroupsLogger logger;
	}
}
