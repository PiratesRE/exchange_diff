using System;

namespace Microsoft.Exchange.Data.Directory.Cache
{
	internal class ObjectTuple
	{
		public ObjectTuple(ObjectType objType, SimpleADObject adObject)
		{
			this.ObjType = objType;
			this.ADObject = adObject;
		}

		public ObjectType ObjType;

		public SimpleADObject ADObject;
	}
}
