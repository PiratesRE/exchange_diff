using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public class EnumInfo<T> where T : struct
	{
		public EnumInfo(Strings.IDs stringIdValue, T enumValue)
		{
			this.enumValue = enumValue;
			this.stringIdValue = stringIdValue;
		}

		public Strings.IDs StringIdValue
		{
			get
			{
				return this.stringIdValue;
			}
		}

		public T EnumValue
		{
			get
			{
				return this.enumValue;
			}
		}

		private T enumValue;

		private Strings.IDs stringIdValue;
	}
}
