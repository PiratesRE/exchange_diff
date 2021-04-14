using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ValidationOrgCurrentOrgNotMatchException : LocalizedException
	{
		public ValidationOrgCurrentOrgNotMatchException(string validationOrg, string currentOrg) : base(Strings.ValidationOrgCurrentOrgNotMatchException(validationOrg, currentOrg))
		{
			this.validationOrg = validationOrg;
			this.currentOrg = currentOrg;
		}

		public ValidationOrgCurrentOrgNotMatchException(string validationOrg, string currentOrg, Exception innerException) : base(Strings.ValidationOrgCurrentOrgNotMatchException(validationOrg, currentOrg), innerException)
		{
			this.validationOrg = validationOrg;
			this.currentOrg = currentOrg;
		}

		protected ValidationOrgCurrentOrgNotMatchException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.validationOrg = (string)info.GetValue("validationOrg", typeof(string));
			this.currentOrg = (string)info.GetValue("currentOrg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("validationOrg", this.validationOrg);
			info.AddValue("currentOrg", this.currentOrg);
		}

		public string ValidationOrg
		{
			get
			{
				return this.validationOrg;
			}
		}

		public string CurrentOrg
		{
			get
			{
				return this.currentOrg;
			}
		}

		private readonly string validationOrg;

		private readonly string currentOrg;
	}
}
