using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Storage
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmOperationNotValidOnCurrentRole : AmServerException
	{
		public AmOperationNotValidOnCurrentRole(string error) : base(ServerStrings.AmOperationNotValidOnCurrentRole(error))
		{
			this.error = error;
		}

		public AmOperationNotValidOnCurrentRole(string error, Exception innerException) : base(ServerStrings.AmOperationNotValidOnCurrentRole(error), innerException)
		{
			this.error = error;
		}

		protected AmOperationNotValidOnCurrentRole(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.error = (string)info.GetValue("error", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("error", this.error);
		}

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly string error;
	}
}
