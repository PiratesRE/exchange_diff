using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Configuration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[DataContract]
	internal class OwaAttachmentPolicyData : SerializableDataBase, IEquatable<OwaAttachmentPolicyData>
	{
		[DataMember]
		public string[] BlockFileTypes { get; set; }

		[DataMember]
		public string[] BlockMimeTypes { get; set; }

		[DataMember]
		public string[] ForceSaveFileTypes { get; set; }

		[DataMember]
		public string[] ForceSaveMimeTypes { get; set; }

		[DataMember]
		public string[] AllowFileTypes { get; set; }

		[DataMember]
		public string[] AllowMimeTypes { get; set; }

		[DataMember]
		public string TreatUnknownTypeAs { get; set; }

		[DataMember]
		public bool DirectFileAccessOnPublicComputersEnabled { get; set; }

		[DataMember]
		public bool DirectFileAccessOnPrivateComputersEnabled { get; set; }

		[DataMember]
		public bool ForceWacViewingFirstOnPublicComputers { get; set; }

		[DataMember]
		public bool ForceWacViewingFirstOnPrivateComputers { get; set; }

		[DataMember]
		public bool WacViewingOnPublicComputersEnabled { get; set; }

		[DataMember]
		public bool WacViewingOnPrivateComputersEnabled { get; set; }

		[DataMember]
		public bool ForceWebReadyDocumentViewingFirstOnPublicComputers { get; set; }

		[DataMember]
		public bool ForceWebReadyDocumentViewingFirstOnPrivateComputers { get; set; }

		[DataMember]
		public bool WebReadyDocumentViewingOnPublicComputersEnabled { get; set; }

		[DataMember]
		public bool WebReadyDocumentViewingOnPrivateComputersEnabled { get; set; }

		[DataMember]
		public string[] WebReadyFileTypes { get; set; }

		[DataMember]
		public string[] WebReadyMimeTypes { get; set; }

		[DataMember]
		public string[] WebReadyDocumentViewingSupportedFileTypes { get; set; }

		[DataMember]
		public string[] WebReadyDocumentViewingSupportedMimeTypes { get; set; }

		[DataMember]
		public bool WebReadyDocumentViewingForAllSupportedTypes { get; set; }

		public bool Equals(OwaAttachmentPolicyData other)
		{
			return !object.ReferenceEquals(other, null) && (object.ReferenceEquals(other, this) || (SerializableDataBase.ArrayContentsEquals<string>(this.BlockFileTypes, other.BlockFileTypes) && SerializableDataBase.ArrayContentsEquals<string>(this.BlockMimeTypes, other.BlockMimeTypes) && SerializableDataBase.ArrayContentsEquals<string>(this.ForceSaveFileTypes, other.ForceSaveFileTypes) && SerializableDataBase.ArrayContentsEquals<string>(this.ForceSaveMimeTypes, other.ForceSaveMimeTypes) && SerializableDataBase.ArrayContentsEquals<string>(this.AllowFileTypes, other.AllowFileTypes) && SerializableDataBase.ArrayContentsEquals<string>(this.AllowMimeTypes, other.AllowMimeTypes) && this.TreatUnknownTypeAs == other.TreatUnknownTypeAs && this.DirectFileAccessOnPublicComputersEnabled == other.DirectFileAccessOnPublicComputersEnabled && this.DirectFileAccessOnPrivateComputersEnabled == other.DirectFileAccessOnPrivateComputersEnabled && this.ForceWacViewingFirstOnPublicComputers == other.ForceWacViewingFirstOnPublicComputers && this.ForceWacViewingFirstOnPrivateComputers == other.ForceWacViewingFirstOnPrivateComputers && this.WacViewingOnPublicComputersEnabled == other.WacViewingOnPublicComputersEnabled && this.WacViewingOnPrivateComputersEnabled == other.WacViewingOnPrivateComputersEnabled && this.ForceWebReadyDocumentViewingFirstOnPublicComputers == other.ForceWebReadyDocumentViewingFirstOnPublicComputers && this.ForceWebReadyDocumentViewingFirstOnPrivateComputers == other.ForceWebReadyDocumentViewingFirstOnPrivateComputers && this.WebReadyDocumentViewingOnPublicComputersEnabled == other.WebReadyDocumentViewingOnPublicComputersEnabled && this.WebReadyDocumentViewingOnPrivateComputersEnabled == other.WebReadyDocumentViewingOnPrivateComputersEnabled && SerializableDataBase.ArrayContentsEquals<string>(this.WebReadyFileTypes, other.WebReadyFileTypes) && SerializableDataBase.ArrayContentsEquals<string>(this.WebReadyMimeTypes, other.WebReadyMimeTypes) && SerializableDataBase.ArrayContentsEquals<string>(this.WebReadyDocumentViewingSupportedFileTypes, other.WebReadyDocumentViewingSupportedFileTypes) && SerializableDataBase.ArrayContentsEquals<string>(this.WebReadyDocumentViewingSupportedMimeTypes, other.WebReadyDocumentViewingSupportedMimeTypes) && this.WebReadyDocumentViewingForAllSupportedTypes == other.WebReadyDocumentViewingForAllSupportedTypes));
		}

		protected override bool InternalEquals(object other)
		{
			return this.Equals(other as OwaAttachmentPolicyData);
		}

		protected override int InternalGetHashCode()
		{
			int num = 17;
			num = (num * 397 ^ SerializableDataBase.ArrayContentsHash<string>(this.BlockFileTypes));
			num = (num * 397 ^ SerializableDataBase.ArrayContentsHash<string>(this.BlockMimeTypes));
			num = (num * 397 ^ SerializableDataBase.ArrayContentsHash<string>(this.ForceSaveFileTypes));
			num = (num * 397 ^ SerializableDataBase.ArrayContentsHash<string>(this.ForceSaveMimeTypes));
			num = (num * 397 ^ SerializableDataBase.ArrayContentsHash<string>(this.AllowFileTypes));
			num = (num * 397 ^ SerializableDataBase.ArrayContentsHash<string>(this.AllowMimeTypes));
			num = (num * 397 ^ ((this.TreatUnknownTypeAs == null) ? 0 : this.TreatUnknownTypeAs.GetHashCode()));
			num = (num * 397 ^ this.DirectFileAccessOnPublicComputersEnabled.GetHashCode());
			num = (num * 397 ^ this.DirectFileAccessOnPrivateComputersEnabled.GetHashCode());
			num = (num * 397 ^ this.ForceWacViewingFirstOnPublicComputers.GetHashCode());
			num = (num * 397 ^ this.ForceWacViewingFirstOnPrivateComputers.GetHashCode());
			num = (num * 397 ^ this.WacViewingOnPublicComputersEnabled.GetHashCode());
			num = (num * 397 ^ this.WacViewingOnPrivateComputersEnabled.GetHashCode());
			num = (num * 397 ^ this.ForceWebReadyDocumentViewingFirstOnPublicComputers.GetHashCode());
			num = (num * 397 ^ this.ForceWebReadyDocumentViewingFirstOnPrivateComputers.GetHashCode());
			num = (num * 397 ^ this.WebReadyDocumentViewingOnPublicComputersEnabled.GetHashCode());
			num = (num * 397 ^ this.WebReadyDocumentViewingOnPrivateComputersEnabled.GetHashCode());
			num = (num * 397 ^ SerializableDataBase.ArrayContentsHash<string>(this.WebReadyFileTypes));
			num = (num * 397 ^ SerializableDataBase.ArrayContentsHash<string>(this.WebReadyMimeTypes));
			num = (num * 397 ^ SerializableDataBase.ArrayContentsHash<string>(this.WebReadyDocumentViewingSupportedFileTypes));
			num = (num * 397 ^ SerializableDataBase.ArrayContentsHash<string>(this.WebReadyDocumentViewingSupportedMimeTypes));
			return num * 397 ^ this.WebReadyDocumentViewingForAllSupportedTypes.GetHashCode();
		}
	}
}
