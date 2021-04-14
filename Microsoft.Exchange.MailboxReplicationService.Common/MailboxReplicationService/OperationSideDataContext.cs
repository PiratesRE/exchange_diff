using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class OperationSideDataContext : DataContext
	{
		private OperationSideDataContext(ExceptionSide side)
		{
			this.Side = side;
		}

		public ExceptionSide Side { get; private set; }

		public static OperationSideDataContext GetContext(ExceptionSide? side)
		{
			if (side == ExceptionSide.Source)
			{
				return OperationSideDataContext.Source;
			}
			if (side == ExceptionSide.Target)
			{
				return OperationSideDataContext.Target;
			}
			return null;
		}

		public override string ToString()
		{
			return string.Format("OperationSide: {0}", this.Side.ToString());
		}

		public static readonly OperationSideDataContext Source = new OperationSideDataContext(ExceptionSide.Source);

		public static readonly OperationSideDataContext Target = new OperationSideDataContext(ExceptionSide.Target);
	}
}
