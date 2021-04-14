using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Transport
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ErrorCannotRemovePendingDeletionPolicyException : LocalizedException
	{
		public ErrorCannotRemovePendingDeletionPolicyException(string name) : base(Strings.ErrorCannotRemovePendingDeletionPolicy(name))
		{
			this.name = name;
		}

		public ErrorCannotRemovePendingDeletionPolicyException(string name, Exception innerException) : base(Strings.ErrorCannotRemovePendingDeletionPolicy(name), innerException)
		{
			this.name = name;
		}

		protected ErrorCannotRemovePendingDeletionPolicyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.name = (string)info.GetValue("name", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("name", this.name);
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		private readonly string name;
	}
}
