using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	public class UserExperienceVerificationRequest
	{
		public UserExperienceVerificationRequest(string tenantName, string stage, IList<string> components)
		{
			this.tenantName = tenantName;
			this.stage = stage;
			this.components = components;
		}

		public string TenantName
		{
			get
			{
				return this.tenantName;
			}
		}

		public IList<string> Components
		{
			get
			{
				return this.components;
			}
		}

		public string Stage
		{
			get
			{
				return this.stage;
			}
		}

		public override bool Equals(object obj)
		{
			UserExperienceVerificationRequest userExperienceVerificationRequest = obj as UserExperienceVerificationRequest;
			return userExperienceVerificationRequest != null && this.TenantName == userExperienceVerificationRequest.TenantName && this.Stage == userExperienceVerificationRequest.Stage;
		}

		public override string ToString()
		{
			return this.TenantName + "_" + this.Stage;
		}

		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}

		private readonly string tenantName;

		private readonly string stage;

		private readonly IList<string> components;
	}
}
