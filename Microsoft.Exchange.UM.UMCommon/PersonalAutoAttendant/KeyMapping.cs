using System;

namespace Microsoft.Exchange.UM.PersonalAutoAttendant
{
	internal abstract class KeyMapping<T> : KeyMappingBase
	{
		internal KeyMapping(KeyMappingTypeEnum type, int key, string context, T data) : base(type, key, context)
		{
			this.data = data;
		}

		internal T Data
		{
			get
			{
				return this.data;
			}
			set
			{
				this.data = value;
			}
		}

		private T data;
	}
}
