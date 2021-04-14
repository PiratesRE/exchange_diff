using System;

namespace Microsoft.Exchange.Entities
{
	public class VoidResult
	{
		private VoidResult()
		{
		}

		public static readonly VoidResult Value = new VoidResult();
	}
}
