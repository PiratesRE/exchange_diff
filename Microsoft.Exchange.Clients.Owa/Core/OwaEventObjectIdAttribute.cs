using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class OwaEventObjectIdAttribute : Attribute
	{
		public OwaEventObjectIdAttribute(Type objectIdType)
		{
			if (objectIdType == null)
			{
				throw new ArgumentNullException("objectIdType");
			}
			if (!objectIdType.IsSubclassOf(typeof(ObjectId)))
			{
				throw new ArgumentException("objectIdType is not subclass of Microsoft.Exchange.Data.ObjectId, it is type: " + objectIdType.ToString());
			}
			this.objectIdType = objectIdType;
		}

		public Type ObjectIdType
		{
			get
			{
				return this.objectIdType;
			}
		}

		private Type objectIdType;
	}
}
