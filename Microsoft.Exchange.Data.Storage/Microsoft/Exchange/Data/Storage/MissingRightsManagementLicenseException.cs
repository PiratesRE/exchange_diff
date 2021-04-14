using System;
using System.Globalization;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class MissingRightsManagementLicenseException : RightsManagementPermanentException, ISerializable
	{
		public MissingRightsManagementLicenseException(StoreObjectId messageStoreId, bool messageInArchive, string mailboxOwnerLegacyDN, string publishLicense) : this(messageStoreId, messageInArchive, mailboxOwnerLegacyDN, publishLicense, null)
		{
		}

		public MissingRightsManagementLicenseException(StoreObjectId messageStoreId, bool messageInArchive, string mailboxOwnerLegacyDN, string publishLicense, LocalizedException innerException) : base(ServerStrings.MissingRightsManagementLicense, innerException)
		{
			this.messageStoreId = messageStoreId;
			this.messageInArchive = messageInArchive;
			this.mailboxOwnerLegacyDN = mailboxOwnerLegacyDN;
			this.publishLicense = publishLicense;
			this.requestCorrelator = MissingRightsManagementLicenseException.GenerateRequestCorrelator();
		}

		protected MissingRightsManagementLicenseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.messageStoreId = (StoreObjectId)info.GetValue("MessageStoreId", typeof(StoreObjectId));
			this.publishLicense = info.GetString("PublishLicense");
			this.requestCorrelator = info.GetString("RequestCorrelator");
			this.messageInArchive = info.GetBoolean("IsMessageInArchive");
			this.mailboxOwnerLegacyDN = info.GetString("MailboxOwnerLegacyDN");
		}

		public string PublishLicense
		{
			get
			{
				return this.publishLicense;
			}
		}

		public StoreObjectId MessageStoreId
		{
			get
			{
				return this.messageStoreId;
			}
		}

		public string RequestCorrelator
		{
			get
			{
				return this.requestCorrelator;
			}
		}

		public override RightsManagementFailureCode FailureCode
		{
			get
			{
				return RightsManagementFailureCode.MissingLicense;
			}
		}

		public bool MessageInArchive
		{
			get
			{
				return this.messageInArchive;
			}
		}

		public string MailboxOwnerLegacyDN
		{
			get
			{
				return this.mailboxOwnerLegacyDN;
			}
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("MessageStoreId", this.messageStoreId, typeof(StoreObjectId));
			info.AddValue("PublishLicense", this.publishLicense);
			info.AddValue("RequestCorrelator", this.requestCorrelator);
			info.AddValue("IsMessageInArchive", this.messageInArchive);
			info.AddValue("MailboxOwnerLegacyDN", this.mailboxOwnerLegacyDN);
			base.GetObjectData(info, context);
		}

		private static string GenerateRequestCorrelator()
		{
			return Guid.NewGuid().GetHashCode().ToString("X8", CultureInfo.InvariantCulture);
		}

		private const string PublishLicenseSerializationLabel = "PublishLicense";

		private const string MessageStoreIdSerializationLabel = "MessageStoreId";

		private const string RequestCorrelatorSerializationLabel = "RequestCorrelator";

		private const string IsMessageInArchiveSerializationLabel = "IsMessageInArchive";

		private const string MailboxOwnerLegacyDNSerializationLabel = "MailboxOwnerLegacyDN";

		private string publishLicense;

		private StoreObjectId messageStoreId;

		private bool messageInArchive;

		private string mailboxOwnerLegacyDN;

		private string requestCorrelator;
	}
}
