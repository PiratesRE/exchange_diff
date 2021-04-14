using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ValidationDivergenceMustBeNonHaltingException : LocalizedException
	{
		public ValidationDivergenceMustBeNonHaltingException(string objectId) : base(Strings.ValidationDivergenceMustBeNonHalting(objectId))
		{
			this.objectId = objectId;
		}

		public ValidationDivergenceMustBeNonHaltingException(string objectId, Exception innerException) : base(Strings.ValidationDivergenceMustBeNonHalting(objectId), innerException)
		{
			this.objectId = objectId;
		}

		protected ValidationDivergenceMustBeNonHaltingException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.objectId = (string)info.GetValue("objectId", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("objectId", this.objectId);
		}

		public string ObjectId
		{
			get
			{
				return this.objectId;
			}
		}

		private readonly string objectId;
	}
}
