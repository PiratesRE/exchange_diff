using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class TypeLocation : NotificationLocation
	{
		public TypeLocation(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			this.type = type;
		}

		public override KeyValuePair<string, object> GetEventData()
		{
			return new KeyValuePair<string, object>("Type", this.type.Name);
		}

		public override int GetHashCode()
		{
			return TypeLocation.TypeHashCode ^ this.type.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			TypeLocation typeLocation = obj as TypeLocation;
			return typeLocation != null && this.type.Equals(typeLocation.type);
		}

		public override string ToString()
		{
			return this.type.Name;
		}

		private const string EventKey = "Type";

		private static readonly int TypeHashCode = typeof(TypeLocation).GetHashCode();

		private readonly Type type;
	}
}
