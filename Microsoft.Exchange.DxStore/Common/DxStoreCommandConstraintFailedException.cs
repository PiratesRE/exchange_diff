using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.DxStore.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DxStoreCommandConstraintFailedException : DxStoreInstanceServerException
	{
		public DxStoreCommandConstraintFailedException(string phase) : base(Strings.DxStoreCommandConstraintFailed(phase))
		{
			this.phase = phase;
		}

		public DxStoreCommandConstraintFailedException(string phase, Exception innerException) : base(Strings.DxStoreCommandConstraintFailed(phase), innerException)
		{
			this.phase = phase;
		}

		protected DxStoreCommandConstraintFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.phase = (string)info.GetValue("phase", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("phase", this.phase);
		}

		public string Phase
		{
			get
			{
				return this.phase;
			}
		}

		private readonly string phase;
	}
}
