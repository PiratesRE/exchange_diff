using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.SystemManager
{
	public class ObjectToFilterableConverter : IFilterableConverter
	{
		public virtual bool ShouldUseStandardFiltering(Type type)
		{
			return typeof(bool).IsAssignableFrom(type) || typeof(byte).IsAssignableFrom(type) || typeof(char).IsAssignableFrom(type) || typeof(DateTime).IsAssignableFrom(type) || typeof(decimal).IsAssignableFrom(type) || typeof(double).IsAssignableFrom(type) || typeof(short).IsAssignableFrom(type) || typeof(int).IsAssignableFrom(type) || typeof(long).IsAssignableFrom(type) || typeof(sbyte).IsAssignableFrom(type) || typeof(float).IsAssignableFrom(type) || typeof(string).IsAssignableFrom(type) || typeof(ushort).IsAssignableFrom(type) || typeof(uint).IsAssignableFrom(type) || typeof(ulong).IsAssignableFrom(type) || typeof(Enum).IsAssignableFrom(type);
		}

		public virtual IConvertible ToFilterable(object item)
		{
			if (this.IsNullValue(item))
			{
				return null;
			}
			if (this.ShouldUseStandardFiltering(item.GetType()))
			{
				return item as IConvertible;
			}
			if (item is ADObjectId)
			{
				return (item as ADObjectId).ToDNString();
			}
			return item.ToUserFriendText();
		}

		protected virtual bool IsNullValue(object item)
		{
			return item == null || DBNull.Value.Equals(item);
		}

		public static ObjectToFilterableConverter DefaultObjectToFilterableConverter = new ObjectToFilterableConverter();
	}
}
