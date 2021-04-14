using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DivergenceAlreadyExistsException : LocalizedException
	{
		public DivergenceAlreadyExistsException(string objectId) : base(Strings.DivergenceAlreadyExists(objectId))
		{
			this.objectId = objectId;
		}

		public DivergenceAlreadyExistsException(string objectId, Exception innerException) : base(Strings.DivergenceAlreadyExists(objectId), innerException)
		{
			this.objectId = objectId;
		}

		protected DivergenceAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
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
