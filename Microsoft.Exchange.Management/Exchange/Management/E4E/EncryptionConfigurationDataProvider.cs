using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.E4E;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.E4E
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class EncryptionConfigurationDataProvider : IConfigDataProvider
	{
		string IConfigDataProvider.Source
		{
			get
			{
				return this.ToString();
			}
		}

		public EncryptionConfigurationDataProvider(string organizationRawIdentity)
		{
			this.organizationRawIdentity = organizationRawIdentity;
		}

		public virtual IConfigurable Read<T>(ObjectId identity) where T : IConfigurable, new()
		{
			IConfigurable[] array = this.Find<T>(new FalseFilter(), identity, true, null);
			if (array != null && array.Length != 0)
			{
				return array[0];
			}
			return null;
		}

		public virtual IConfigurable[] Find<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy) where T : IConfigurable, new()
		{
			return (IConfigurable[])this.FindPaged<T>(filter, rootId, deepSearch, sortBy, 0).ToArray<T>();
		}

		public virtual IEnumerable<T> FindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize) where T : IConfigurable, new()
		{
			if (!typeof(ConfigurableObject).IsAssignableFrom(typeof(T)))
			{
				throw new NotSupportedException("FindPaged: " + typeof(T).FullName);
			}
			return this.InternalFindPaged<T>(filter, rootId, deepSearch, sortBy, pageSize);
		}

		public virtual void Save(IConfigurable instance)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			ConfigurableObject configurableObject = instance as ConfigurableObject;
			if (configurableObject == null)
			{
				throw new NotSupportedException("Save: " + instance.GetType().FullName);
			}
			ValidationError[] array = configurableObject.Validate();
			if (array.Length > 0)
			{
				throw new DataValidationException(array[0]);
			}
			this.InternalSave(configurableObject);
			configurableObject.ResetChangeTracking(true);
		}

		protected IEnumerable<T> InternalFindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize)
		{
			EncryptionConfigurationTable.RequestData requestData;
			EncryptionConfigurationData encryptionConfigurationData = EncryptionConfigurationTable.GetEncryptionConfiguration(this.organizationRawIdentity, false, out requestData);
			yield return (T)((object)this.ConvertStoreObjectToPresentationObject(encryptionConfigurationData));
			yield break;
		}

		protected void InternalSave(ConfigurableObject instance)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			EncryptionConfiguration encryptionConfiguration = instance as EncryptionConfiguration;
			if (encryptionConfiguration == null)
			{
				throw new NotSupportedException("Save: " + instance.GetType().FullName);
			}
			EncryptionConfigurationTable.SetEncryptionConfiguration(this.organizationRawIdentity, encryptionConfiguration.ImageBase64, encryptionConfiguration.EmailText, encryptionConfiguration.PortalText, encryptionConfiguration.DisclaimerText, encryptionConfiguration.OTPEnabled);
		}

		public void Delete(IConfigurable instance)
		{
		}

		private EncryptionConfiguration ConvertStoreObjectToPresentationObject(EncryptionConfigurationData encryptionConfigurationData)
		{
			return new EncryptionConfiguration(encryptionConfigurationData.ImageBase64, encryptionConfigurationData.EmailText, encryptionConfigurationData.PortalText, encryptionConfigurationData.DisclaimerText, encryptionConfigurationData.OTPEnabled);
		}

		private readonly string organizationRawIdentity;
	}
}
