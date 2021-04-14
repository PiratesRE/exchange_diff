using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[Serializable]
	public class UMMailboxPin : IConfigurable
	{
		public UMMailboxPin(ADUser user, bool expired, bool lockedOut, bool firstTimeUser, bool needSuppressingPiiData)
		{
			this.userObjectId = user.Identity;
			this.userId = user.PrimarySmtpAddress.ToString();
			this.pinExpired = expired;
			this.lockedOut = lockedOut;
			this.isFirstTimeUser = firstTimeUser;
			if (needSuppressingPiiData)
			{
				ADObjectId id = user.Id;
				string text;
				string text2;
				this.userObjectId = SuppressingPiiData.Redact(id, out text, out text2);
				this.userId = SuppressingPiiData.RedactSmtpAddress(this.userId);
			}
		}

		private UMMailboxPin()
		{
		}

		public string UserID
		{
			get
			{
				return this.userId;
			}
		}

		public bool PinExpired
		{
			get
			{
				return this.pinExpired;
			}
			internal set
			{
				this.pinExpired = value;
			}
		}

		public bool FirstTimeUser
		{
			get
			{
				return this.isFirstTimeUser;
			}
			internal set
			{
				this.isFirstTimeUser = value;
			}
		}

		public bool LockedOut
		{
			get
			{
				return this.lockedOut;
			}
			internal set
			{
				this.lockedOut = value;
			}
		}

		public ObjectState ObjectState
		{
			get
			{
				return ObjectState.Unchanged;
			}
		}

		public bool IsValid
		{
			get
			{
				return true;
			}
		}

		ObjectId IConfigurable.Identity
		{
			get
			{
				return this.userObjectId;
			}
		}

		public void CopyChangesFrom(IConfigurable source)
		{
			throw new NotSupportedException();
		}

		public void ResetChangeTracking()
		{
			throw new NotImplementedException();
		}

		public virtual ValidationError[] Validate()
		{
			return ValidationError.None;
		}

		private readonly string userId;

		private ObjectId userObjectId;

		private bool pinExpired;

		private bool lockedOut;

		private bool isFirstTimeUser;
	}
}
