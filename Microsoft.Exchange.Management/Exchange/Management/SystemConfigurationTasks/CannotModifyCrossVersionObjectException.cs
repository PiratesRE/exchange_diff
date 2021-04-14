using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotModifyCrossVersionObjectException : LocalizedException
	{
		public CannotModifyCrossVersionObjectException(string id) : base(Strings.ErrorCannotModifyCrossVersionObject(id))
		{
			this.id = id;
		}

		public CannotModifyCrossVersionObjectException(string id, Exception innerException) : base(Strings.ErrorCannotModifyCrossVersionObject(id), innerException)
		{
			this.id = id;
		}

		protected CannotModifyCrossVersionObjectException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.id = (string)info.GetValue("id", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("id", this.id);
		}

		public string Id
		{
			get
			{
				return this.id;
			}
		}

		private readonly string id;
	}
}
