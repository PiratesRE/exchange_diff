using System;
using System.Security;

namespace System.Runtime.Remoting.Messaging
{
	internal class SmuggledObjRef
	{
		[SecurityCritical]
		public SmuggledObjRef(ObjRef objRef)
		{
			this._objRef = objRef;
		}

		public ObjRef ObjRef
		{
			[SecurityCritical]
			get
			{
				return this._objRef;
			}
		}

		[SecurityCritical]
		private ObjRef _objRef;
	}
}
