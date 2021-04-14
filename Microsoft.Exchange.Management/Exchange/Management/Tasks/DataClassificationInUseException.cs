using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DataClassificationInUseException : LocalizedException
	{
		public DataClassificationInUseException(string identity, string rules) : base(Strings.ErrorDataClassificationIsInUse(identity, rules))
		{
			this.identity = identity;
			this.rules = rules;
		}

		public DataClassificationInUseException(string identity, string rules, Exception innerException) : base(Strings.ErrorDataClassificationIsInUse(identity, rules), innerException)
		{
			this.identity = identity;
			this.rules = rules;
		}

		protected DataClassificationInUseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.identity = (string)info.GetValue("identity", typeof(string));
			this.rules = (string)info.GetValue("rules", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("identity", this.identity);
			info.AddValue("rules", this.rules);
		}

		public string Identity
		{
			get
			{
				return this.identity;
			}
		}

		public string Rules
		{
			get
			{
				return this.rules;
			}
		}

		private readonly string identity;

		private readonly string rules;
	}
}
