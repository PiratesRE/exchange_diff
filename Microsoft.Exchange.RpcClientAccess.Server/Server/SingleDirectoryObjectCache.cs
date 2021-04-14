using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal class SingleDirectoryObjectCache<TADObject, TTransformed> where TADObject : class where TTransformed : class
	{
		public SingleDirectoryObjectCache(Func<DateTime, bool> expirationPolicy, Func<TADObject> loadADObject, Func<TADObject, TTransformed> transformADObject)
		{
			if (expirationPolicy == null)
			{
				throw new ArgumentNullException("expirationPolicy");
			}
			if (loadADObject == null)
			{
				throw new ArgumentNullException("loadADObject");
			}
			if (transformADObject == null)
			{
				throw new ArgumentNullException("transformADObject");
			}
			this.expirationPolicy = expirationPolicy;
			this.loadADObject = loadADObject;
			this.transformADObject = transformADObject;
		}

		public void ForceReload()
		{
			lock (this.lockInstance)
			{
				this.lastRefreshTime = DateTime.MinValue;
			}
		}

		public TADObject GetValues(out TTransformed transformedObject)
		{
			TADObject tadobject;
			TADObject tadobject2;
			TTransformed ttransformed;
			Exception innerException;
			lock (this.lockInstance)
			{
				tadobject = this.adObject;
				transformedObject = this.transformedObject;
				if (tadobject == null || transformedObject == null)
				{
					this.beingRefreshed = true;
					if (this.TryRefreshCache(out tadobject2, out ttransformed, out innerException))
					{
						tadobject = tadobject2;
						transformedObject = ttransformed;
						return tadobject;
					}
					throw new RpcServerException("Unable to retrieve information from AD!", RpcErrorCode.ADError, innerException);
				}
				else
				{
					if (this.expirationPolicy(this.lastRefreshTime) || this.beingRefreshed)
					{
						return tadobject;
					}
					this.beingRefreshed = true;
				}
			}
			if (this.TryRefreshCache(out tadobject2, out ttransformed, out innerException))
			{
				tadobject = tadobject2;
				transformedObject = ttransformed;
			}
			return tadobject;
		}

		private bool TryRefreshCache(out TADObject newADObject, out TTransformed newTransformedObject, out Exception adException)
		{
			bool flag = false;
			newADObject = default(TADObject);
			newTransformedObject = default(TTransformed);
			adException = null;
			try
			{
				newADObject = this.loadADObject();
				newTransformedObject = this.transformADObject(newADObject);
				flag = (newADObject != null && newTransformedObject != null);
			}
			catch (ADTransientException ex)
			{
				adException = ex;
			}
			catch (ADOperationException ex2)
			{
				adException = ex2;
			}
			catch (ADTopologyUnexpectedException ex3)
			{
				adException = ex3;
			}
			finally
			{
				lock (this.lockInstance)
				{
					if (flag)
					{
						this.adObject = newADObject;
						this.transformedObject = newTransformedObject;
						this.lastRefreshTime = DateTime.UtcNow;
					}
					this.beingRefreshed = false;
				}
			}
			return flag;
		}

		private object lockInstance = new object();

		private DateTime lastRefreshTime;

		private TADObject adObject;

		private TTransformed transformedObject;

		private Func<DateTime, bool> expirationPolicy;

		private Func<TADObject> loadADObject;

		private Func<TADObject, TTransformed> transformADObject;

		private bool beingRefreshed;
	}
}
