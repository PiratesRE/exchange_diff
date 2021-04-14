using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Net.AAD
{
	internal interface IAadClient
	{
		int Timeout { get; set; }

		string GetUserObjectId(string userPrincipalName);

		List<AadDevice> GetUserDevicesWithEasID(string easId, string userObjectId);

		string EvaluateAuthPolicy(string easId, string userObjectId, bool isSupportedPlatform);

		bool IsUserMemberOfGroup(string userObjectId, string groupObjectId);

		string[] GetGroupMembership(string userObjectId);
	}
}
