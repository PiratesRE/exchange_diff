using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InsufficientSparesForExtraCopiesException : DatabaseCopyLayoutException
	{
		public InsufficientSparesForExtraCopiesException(int spares, int copies) : base(ReplayStrings.InsufficientSparesForExtraCopiesException(spares, copies))
		{
			this.spares = spares;
			this.copies = copies;
		}

		public InsufficientSparesForExtraCopiesException(int spares, int copies, Exception innerException) : base(ReplayStrings.InsufficientSparesForExtraCopiesException(spares, copies), innerException)
		{
			this.spares = spares;
			this.copies = copies;
		}

		protected InsufficientSparesForExtraCopiesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.spares = (int)info.GetValue("spares", typeof(int));
			this.copies = (int)info.GetValue("copies", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("spares", this.spares);
			info.AddValue("copies", this.copies);
		}

		public int Spares
		{
			get
			{
				return this.spares;
			}
		}

		public int Copies
		{
			get
			{
				return this.copies;
			}
		}

		private readonly int spares;

		private readonly int copies;
	}
}
