using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class UnifiedPolicySyncNotificationIdParameter : IIdentityParameter
	{
		internal UnifiedPolicySyncNotificationId NotificatonId { get; set; }

		public string RawIdentity
		{
			get
			{
				return this.ToString();
			}
		}

		public UnifiedPolicySyncNotificationIdParameter()
		{
		}

		public UnifiedPolicySyncNotificationIdParameter(ConfigurableObject configurableObject)
		{
			if (configurableObject == null)
			{
				throw new ArgumentNullException("configurableObject");
			}
			((IIdentityParameter)this).Initialize(configurableObject.Identity);
		}

		public UnifiedPolicySyncNotificationIdParameter(string notificationIdValue)
		{
			if (string.IsNullOrEmpty(notificationIdValue))
			{
				throw new ArgumentNullException("notificationIdValue");
			}
			this.Initialize(new UnifiedPolicySyncNotificationId(notificationIdValue));
		}

		public void Initialize(ObjectId objectId)
		{
			if (objectId == null)
			{
				throw new ArgumentNullException("objectId");
			}
			if (!(objectId is UnifiedPolicySyncNotificationId))
			{
				throw new NotSupportedException("objectId: " + objectId.GetType().FullName);
			}
			this.NotificatonId = (UnifiedPolicySyncNotificationId)objectId;
		}

		IEnumerable<T> IIdentityParameter.GetObjects<T>(ObjectId rootId, IConfigDataProvider session)
		{
			LocalizedString? localizedString;
			return ((IIdentityParameter)this).GetObjects<T>(rootId, session, null, out localizedString);
		}

		IEnumerable<T> IIdentityParameter.GetObjects<T>(ObjectId rootId, IConfigDataProvider session, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (this.NotificatonId == null)
			{
				throw new InvalidOperationException(Strings.ErrorOperationOnInvalidObject);
			}
			IConfigurable[] array = session.Find<T>(null, this.NotificatonId, false, null);
			if (array == null || array.Length == 0)
			{
				notFoundReason = new LocalizedString?(Strings.ErrorManagementObjectNotFound(this.ToString()));
				return new T[0];
			}
			notFoundReason = null;
			T[] array2 = new T[array.Length];
			int num = 0;
			foreach (IConfigurable configurable in array)
			{
				array2[num++] = (T)((object)configurable);
			}
			return array2;
		}

		public override string ToString()
		{
			if (this.NotificatonId == null)
			{
				return null;
			}
			return this.NotificatonId.IdValue;
		}
	}
}
