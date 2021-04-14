using System;

namespace Microsoft.Exchange.Net.MonitoringWebClient.Ecp
{
	internal class EcpGetRoleAssignmentPolicyWSCall : EcpWebServiceCallBase
	{
		protected override TestId Id
		{
			get
			{
				return TestId.EcpGetRoleAssignmentPolicyWSCall;
			}
		}

		protected override Uri WebServiceRelativeUri
		{
			get
			{
				return new Uri("UsersGroups/RoleAssignmentPolicies.svc/GetList", UriKind.Relative);
			}
		}

		protected override RequestBody RequestBody
		{
			get
			{
				return RequestBody.Format("{\"filter\":{},\"sort\":{\"Direction\":0,\"PropertyName\":\"Name\"}}", new object[0]);
			}
		}

		public EcpGetRoleAssignmentPolicyWSCall(Uri uri, Func<Uri, ITestStep> getFollowingTestStep = null) : base(uri, getFollowingTestStep)
		{
		}

		private const TestId ID = TestId.EcpGetRoleAssignmentPolicyWSCall;
	}
}
