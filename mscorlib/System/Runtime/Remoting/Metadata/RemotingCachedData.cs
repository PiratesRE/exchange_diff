using System;

namespace System.Runtime.Remoting.Metadata
{
	internal abstract class RemotingCachedData
	{
		internal SoapAttribute GetSoapAttribute()
		{
			if (this._soapAttr == null)
			{
				lock (this)
				{
					if (this._soapAttr == null)
					{
						this._soapAttr = this.GetSoapAttributeNoLock();
					}
				}
			}
			return this._soapAttr;
		}

		internal abstract SoapAttribute GetSoapAttributeNoLock();

		private SoapAttribute _soapAttr;
	}
}
