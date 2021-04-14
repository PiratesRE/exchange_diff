using System;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public class EmptyTwir : ITWIR
	{
		public int GetColumnSize(Column column)
		{
			return ((IColumn)column).GetSize(this);
		}

		public object GetColumnValue(Column column)
		{
			return ((IColumn)column).GetValue(this);
		}

		public int GetPhysicalColumnSize(PhysicalColumn column)
		{
			return 0;
		}

		public object GetPhysicalColumnValue(PhysicalColumn column)
		{
			return null;
		}

		public int GetPropertyColumnSize(PropertyColumn column)
		{
			return 0;
		}

		public object GetPropertyColumnValue(PropertyColumn column)
		{
			return null;
		}

		public static ITWIR Instance = new EmptyTwir();
	}
}
