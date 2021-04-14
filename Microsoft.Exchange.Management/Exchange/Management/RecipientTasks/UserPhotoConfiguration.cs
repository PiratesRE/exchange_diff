using System;
using System.IO;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Serializable]
	public class UserPhotoConfiguration : IConfigurable, ICmdletProxyable
	{
		public ObjectId Identity
		{
			get
			{
				return this.identity;
			}
		}

		public byte[] PictureData
		{
			get
			{
				return this.pictureData;
			}
		}

		public int? Thumbprint
		{
			get
			{
				return this.thumbprint;
			}
		}

		ValidationError[] IConfigurable.Validate()
		{
			return new ValidationError[0];
		}

		void IConfigurable.CopyChangesFrom(IConfigurable changedObject)
		{
			throw new NotImplementedException();
		}

		void IConfigurable.ResetChangeTracking()
		{
			throw new NotImplementedException();
		}

		bool IConfigurable.IsValid
		{
			get
			{
				return true;
			}
		}

		ObjectState IConfigurable.ObjectState
		{
			get
			{
				return ObjectState.New;
			}
		}

		public object GetProxyInfo()
		{
			return this.proxyInfo;
		}

		public void SetProxyInfo(object proxyInfoValue)
		{
			if (this.proxyInfo != null && proxyInfoValue != null)
			{
				return;
			}
			this.proxyInfo = proxyInfoValue;
		}

		internal UserPhotoConfiguration(ObjectId identity, Stream userPhotoStream, int? thumbprint)
		{
			if (identity == null)
			{
				throw new ArgumentException("identity");
			}
			if (userPhotoStream == null)
			{
				throw new ArgumentException("userPhotoStream");
			}
			this.identity = identity;
			this.thumbprint = thumbprint;
			int num = (int)userPhotoStream.Length;
			this.pictureData = new byte[num];
			userPhotoStream.Read(this.pictureData, 0, num);
		}

		private byte[] pictureData;

		private ObjectId identity;

		private int? thumbprint;

		[NonSerialized]
		private object proxyInfo;
	}
}
