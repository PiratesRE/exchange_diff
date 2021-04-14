using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens;
using Microsoft.Exchange.Common.Cache;
using Microsoft.Exchange.Core;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Security.RightsManagement
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal abstract class RmsTemplate : CachableItem, IEquatable<RmsTemplate>
	{
		internal RmsTemplate(Guid id) : this(id, RmsTemplateType.Distributed)
		{
		}

		internal RmsTemplate(Guid id, RmsTemplateType type)
		{
			this.id = id;
			if (type == RmsTemplateType.All)
			{
				throw new ArgumentException("Only Archived and Distributed template types supported.", "type");
			}
			this.type = type;
		}

		public Guid Id
		{
			get
			{
				return this.id;
			}
		}

		public RmsTemplateType Type
		{
			get
			{
				return this.type;
			}
		}

		public virtual string Name
		{
			get
			{
				return this.GetName(CultureInfo.CurrentUICulture);
			}
		}

		public virtual string Description
		{
			get
			{
				return this.GetDescription(CultureInfo.CurrentUICulture);
			}
		}

		public override long ItemSize
		{
			get
			{
				return (long)(this.GetSize() + 16);
			}
		}

		public abstract bool RequiresRepublishingWhenRecipientsChange { get; }

		public static RmsTemplate Create(Guid id, string template)
		{
			if (id == RmsTemplate.DoNotForward.Id)
			{
				return RmsTemplate.DoNotForward;
			}
			if (id == RmsTemplate.InternetConfidential.Id)
			{
				return RmsTemplate.InternetConfidential;
			}
			return new RmsTemplate.ServerRmsTemplate(id, template, RmsTemplateType.Distributed);
		}

		public static RmsTemplate CreateFromPublishLicense(string publishLicense)
		{
			if (string.IsNullOrEmpty(publishLicense))
			{
				throw new ArgumentException("publishLicense");
			}
			ICollection<RmsTemplate.LocaleNameDescription> templateNamesAndDescriptions = RmsTemplate.GetTemplateNamesAndDescriptions(publishLicense);
			string cultureNeutralName = RmsTemplate.GetCultureNeutralName(templateNamesAndDescriptions);
			if (string.Equals("Do Not Forward", cultureNeutralName, StringComparison.OrdinalIgnoreCase))
			{
				return RmsTemplate.DoNotForward;
			}
			if (string.Equals("Internet Confidential", cultureNeutralName, StringComparison.OrdinalIgnoreCase))
			{
				return RmsTemplate.InternetConfidential;
			}
			Guid templateGuidFromLicense = DrmClientUtils.GetTemplateGuidFromLicense(publishLicense);
			if (templateGuidFromLicense == Guid.Empty)
			{
				return RmsTemplate.DoNotForward;
			}
			return new RmsTemplate.ServerRmsTemplate(templateGuidFromLicense, publishLicense, templateNamesAndDescriptions);
		}

		public static RmsTemplate CreateServerTemplateFromTemplateDefinition(string templateXrml, RmsTemplateType type)
		{
			if (string.IsNullOrEmpty(templateXrml))
			{
				throw new ArgumentNullException("templateXrml");
			}
			Guid templateGuidFromLicense = DrmClientUtils.GetTemplateGuidFromLicense(templateXrml);
			if (templateGuidFromLicense == Guid.Empty)
			{
				throw new RightsManagementException(RightsManagementFailureCode.InvalidLicense, DrmStrings.FailedToGetTemplateIdFromLicense);
			}
			return new RmsTemplate.ServerRmsTemplate(templateGuidFromLicense, templateXrml, type);
		}

		public string CreatePublishLicense(string sender, string from, IEnumerable<string> recipients, SymmetricSecurityKey cek, DisposableTenantLicensePair tenantLicenses, SafeRightsManagementEnvironmentHandle envHandle, out string ownerUseLicense, out string contentId, out string contentIdType)
		{
			if (envHandle == null)
			{
				throw new ArgumentNullException("envHandle");
			}
			if (tenantLicenses == null)
			{
				throw new ArgumentNullException("tenantLicenses");
			}
			if (RmsAppSettings.Instance.IsSamlAuthenticationEnabledForInternalRMS)
			{
				throw new RightsManagementException(RightsManagementFailureCode.ActionNotSupported, new LocalizedString("Publishing is not allowed while RmsEnableSamlAuthenticationforInternalRMS is set in the app config."));
			}
			if (tenantLicenses.EnablingPrincipalRac == null || tenantLicenses.EnablingPrincipalRac.IsInvalid || tenantLicenses.BoundLicenseClc == null || tenantLicenses.BoundLicenseClc.IsInvalid)
			{
				throw new ArgumentException("tenantLicenses");
			}
			SafeRightsManagementPubHandle safeRightsManagementPubHandle = null;
			SafeRightsManagementPubHandle safeRightsManagementPubHandle2 = null;
			SafeRightsManagementPubHandle safeRightsManagementPubHandle3 = null;
			string callbackData;
			try
			{
				if (!string.IsNullOrEmpty(sender) && !string.Equals(sender, "<>", StringComparison.OrdinalIgnoreCase))
				{
					Errors.ThrowOnErrorCode(SafeNativeMethods.DRMCreateUser(sender, null, "Unspecified", out safeRightsManagementPubHandle2));
				}
				if (!string.IsNullOrEmpty(from) && !string.Equals(from, sender, StringComparison.OrdinalIgnoreCase) && !string.Equals(from, "<>", StringComparison.OrdinalIgnoreCase))
				{
					Errors.ThrowOnErrorCode(SafeNativeMethods.DRMCreateUser(from, null, "Unspecified", out safeRightsManagementPubHandle3));
				}
				safeRightsManagementPubHandle = this.CreateUnsignedIssuanceLicense(safeRightsManagementPubHandle2, safeRightsManagementPubHandle3, recipients);
				contentId = Guid.NewGuid().ToString("B");
				contentIdType = "MS-GUID";
				Errors.ThrowOnErrorCode(SafeNativeMethods.DRMSetMetaData(safeRightsManagementPubHandle, contentId, "MS-GUID", null, null, "Microsoft Office Document", "Microsoft Office Document"));
				using (CallbackHandler callbackHandler = new CallbackHandler())
				{
					SignIssuanceLicenseFlags signIssuanceLicenseFlags = SignIssuanceLicenseFlags.Offline | SignIssuanceLicenseFlags.OwnerLicenseNoPersist;
					if (cek == null)
					{
						signIssuanceLicenseFlags |= SignIssuanceLicenseFlags.AutoGenerateKey;
					}
					Errors.ThrowOnErrorCode(SafeNativeMethods.DRMGetSignedIssuanceLicenseEx(envHandle, safeRightsManagementPubHandle, signIssuanceLicenseFlags, (cek != null) ? cek.GetSymmetricKey() : null, (uint)((cek != null) ? (cek.KeySize / 8) : 0), "AES", IntPtr.Zero, tenantLicenses.EnablingPrincipalRac, tenantLicenses.BoundLicenseClc, callbackHandler.CallbackDelegate, IntPtr.Zero));
					callbackHandler.WaitForCompletion();
					callbackData = callbackHandler.CallbackData;
					ownerUseLicense = DrmClientUtils.GetOwnerLicense(safeRightsManagementPubHandle);
				}
			}
			finally
			{
				if (safeRightsManagementPubHandle != null)
				{
					safeRightsManagementPubHandle.Close();
				}
				if (safeRightsManagementPubHandle2 != null)
				{
					safeRightsManagementPubHandle2.Close();
				}
				if (safeRightsManagementPubHandle3 != null)
				{
					safeRightsManagementPubHandle3.Close();
				}
			}
			return callbackData;
		}

		public string GetName(CultureInfo locale)
		{
			string result;
			string text;
			this.GetNameAndDescription(locale, out result, out text);
			return result;
		}

		public string GetDescription(CultureInfo locale)
		{
			string text;
			string result;
			this.GetNameAndDescription(locale, out text, out result);
			return result;
		}

		public bool Equals(RmsTemplate other)
		{
			return other != null && other.Id == this.id;
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as RmsTemplate);
		}

		public override int GetHashCode()
		{
			return this.id.GetHashCode();
		}

		protected abstract SafeRightsManagementPubHandle CreateUnsignedIssuanceLicense(SafeRightsManagementPubHandle ownerHandle, SafeRightsManagementPubHandle fromHandle, IEnumerable<string> recipients);

		protected abstract void GetNameAndDescription(CultureInfo locale, out string templateName, out string templateDescription);

		protected abstract int GetSize();

		private static RmsTemplate CreateDoNotForwardTemplate()
		{
			return new RmsTemplate.OneOffRmsTemplate(new Guid("CF5CF348-A8D7-40D5-91EF-A600B88A395D"), "Do Not Forward", SystemMessages.DoNotForwardName, SystemMessages.DoNotForwardDescription, RightUtils.GetIndividualRightNames(RmsTemplate.DoNotForwardRights));
		}

		private static RmsTemplate CreateInternetConfidentialTemplate()
		{
			return new RmsTemplate.OneOffRmsTemplate(new Guid("81E24817-F117-4943-8959-60E1477E67B6"), "Internet Confidential", SystemMessages.InternetConfidentialName, SystemMessages.InternetConfidentialDescription, RightUtils.GetIndividualRightNames(RmsTemplate.InternetConfidentialRights));
		}

		private static RmsTemplate CreateEmptyTemplate()
		{
			return new RmsTemplate.OneOffRmsTemplate(Guid.Empty, string.Empty, new LocalizedString(string.Empty), new LocalizedString(string.Empty), new string[0]);
		}

		private static ICollection<RmsTemplate.LocaleNameDescription> GetTemplateNamesAndDescriptions(string template)
		{
			if (string.IsNullOrEmpty(template))
			{
				throw new ArgumentNullException("template");
			}
			SafeRightsManagementPubHandle safeRightsManagementPubHandle = null;
			ICollection<RmsTemplate.LocaleNameDescription> result;
			try
			{
				int hr = SafeNativeMethods.DRMCreateIssuanceLicense(null, null, null, null, SafeRightsManagementPubHandle.InvalidHandle, template, SafeRightsManagementHandle.InvalidHandle, out safeRightsManagementPubHandle);
				Errors.ThrowOnErrorCode(hr);
				LinkedList<RmsTemplate.LocaleNameDescription> linkedList = new LinkedList<RmsTemplate.LocaleNameDescription>();
				using (safeRightsManagementPubHandle)
				{
					uint num = 0U;
					uint localeId;
					string name;
					string description;
					while (DrmClientUtils.GetNameAndDescription(safeRightsManagementPubHandle, num, out localeId, out name, out description))
					{
						linkedList.AddLast(new RmsTemplate.LocaleNameDescription(localeId, name, description));
						num += 1U;
					}
				}
				result = linkedList;
			}
			finally
			{
				if (safeRightsManagementPubHandle != null)
				{
					safeRightsManagementPubHandle.Close();
				}
			}
			return result;
		}

		private static string GetCultureNeutralName(IEnumerable<RmsTemplate.LocaleNameDescription> localeNameDescriptions)
		{
			if (localeNameDescriptions == null)
			{
				return string.Empty;
			}
			foreach (RmsTemplate.LocaleNameDescription localeNameDescription in localeNameDescriptions)
			{
				if (localeNameDescription.LocaleId == 0U)
				{
					return localeNameDescription.Name;
				}
			}
			return string.Empty;
		}

		internal const string DoNotForwardTemplateId = "CF5CF348-A8D7-40D5-91EF-A600B88A395D";

		internal const string InternetConfidentialId = "81E24817-F117-4943-8959-60E1477E67B6";

		private const string NullReversePath = "<>";

		private const string DoNotForwardCultureNeutralName = "Do Not Forward";

		private const string InternetConfidentialCultureNeutralName = "Internet Confidential";

		public static readonly IEnumerable<CultureInfo> SupportedClientLanguages = LanguagePackInfo.GetInstalledLanguagePackSpecificCultures(LanguagePackType.Client);

		internal static readonly ContentRight DoNotForwardRights = ContentRight.View | ContentRight.Edit | ContentRight.ObjectModel | ContentRight.ViewRightsData | ContentRight.Reply | ContentRight.ReplyAll | ContentRight.DocumentEdit;

		internal static readonly ContentRight InternetConfidentialRights = ContentRight.View | ContentRight.Edit | ContentRight.Print | ContentRight.Extract | ContentRight.ObjectModel | ContentRight.ViewRightsData | ContentRight.Reply | ContentRight.ReplyAll | ContentRight.Sign | ContentRight.DocumentEdit | ContentRight.Export;

		internal static readonly RmsTemplate Empty = RmsTemplate.CreateEmptyTemplate();

		internal static readonly RmsTemplate DoNotForward = RmsTemplate.CreateDoNotForwardTemplate();

		internal static readonly RmsTemplate InternetConfidential = RmsTemplate.CreateInternetConfidentialTemplate();

		private readonly Guid id;

		private readonly RmsTemplateType type;

		[Serializable]
		private sealed class ServerRmsTemplate : RmsTemplate
		{
			internal ServerRmsTemplate(Guid id, string template, RmsTemplateType templateType = RmsTemplateType.Distributed) : base(id, templateType)
			{
				if (string.IsNullOrEmpty(template))
				{
					throw new ArgumentNullException("template");
				}
				this.template = template;
			}

			internal ServerRmsTemplate(Guid id, string template, ICollection<RmsTemplate.LocaleNameDescription> localeNameDescriptions) : base(id)
			{
				if (string.IsNullOrEmpty(template))
				{
					throw new ArgumentNullException("template");
				}
				this.template = template;
				this.InitializeLocaleNameDescriptionMap(localeNameDescriptions);
			}

			public override bool RequiresRepublishingWhenRecipientsChange
			{
				get
				{
					return false;
				}
			}

			protected override int GetSize()
			{
				if (string.IsNullOrEmpty(this.template))
				{
					return 0;
				}
				return this.template.Length * 2;
			}

			protected override SafeRightsManagementPubHandle CreateUnsignedIssuanceLicense(SafeRightsManagementPubHandle ownerHandle, SafeRightsManagementPubHandle fromHandle, IEnumerable<string> recipients)
			{
				SafeRightsManagementPubHandle result;
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					SafeRightsManagementPubHandle safeRightsManagementPubHandle;
					int hr = SafeNativeMethods.DRMCreateIssuanceLicense(null, null, null, null, ownerHandle ?? SafeRightsManagementPubHandle.InvalidHandle, this.template, SafeRightsManagementHandle.InvalidHandle, out safeRightsManagementPubHandle);
					disposeGuard.Add<SafeRightsManagementPubHandle>(safeRightsManagementPubHandle);
					Errors.ThrowOnErrorCode(hr);
					disposeGuard.Success();
					result = safeRightsManagementPubHandle;
				}
				return result;
			}

			protected override void GetNameAndDescription(CultureInfo locale, out string templateName, out string templateDescription)
			{
				if (this.namesAndDescriptions == null)
				{
					this.InitializeLocaleNameDescriptionMap(RmsTemplate.GetTemplateNamesAndDescriptions(this.template));
				}
				templateName = string.Empty;
				templateDescription = string.Empty;
				RmsTemplate.ServerRmsTemplate.NameAndDescriptionPair nameAndDescriptionPair;
				if (locale != null && this.namesAndDescriptions.TryGetValue((uint)locale.LCID, out nameAndDescriptionPair))
				{
					templateName = nameAndDescriptionPair.Name;
					templateDescription = nameAndDescriptionPair.Description;
					return;
				}
				if (locale != null && locale.Parent != null && this.namesAndDescriptions.TryGetValue((uint)locale.Parent.LCID, out nameAndDescriptionPair))
				{
					templateName = nameAndDescriptionPair.Name;
					templateDescription = nameAndDescriptionPair.Description;
					return;
				}
				if (this.namesAndDescriptions.TryGetValue((uint)CultureInfo.InstalledUICulture.LCID, out nameAndDescriptionPair))
				{
					templateName = nameAndDescriptionPair.Name;
					templateDescription = nameAndDescriptionPair.Description;
					return;
				}
				if (this.namesAndDescriptions.TryGetValue((uint)CultureInfo.InstalledUICulture.Parent.LCID, out nameAndDescriptionPair))
				{
					templateName = nameAndDescriptionPair.Name;
					templateDescription = nameAndDescriptionPair.Description;
					return;
				}
				if (this.namesAndDescriptions.TryGetValue((uint)CultureInfo.InvariantCulture.LCID, out nameAndDescriptionPair))
				{
					templateName = nameAndDescriptionPair.Name;
					templateDescription = nameAndDescriptionPair.Description;
				}
			}

			private void InitializeLocaleNameDescriptionMap(ICollection<RmsTemplate.LocaleNameDescription> localeNameDescriptions)
			{
				if (localeNameDescriptions == null)
				{
					throw new ArgumentNullException("localeNameDescriptions");
				}
				Dictionary<uint, RmsTemplate.ServerRmsTemplate.NameAndDescriptionPair> dictionary = new Dictionary<uint, RmsTemplate.ServerRmsTemplate.NameAndDescriptionPair>(localeNameDescriptions.Count);
				bool flag = true;
				foreach (RmsTemplate.LocaleNameDescription localeNameDescription in localeNameDescriptions)
				{
					RmsTemplate.ServerRmsTemplate.NameAndDescriptionPair value = new RmsTemplate.ServerRmsTemplate.NameAndDescriptionPair(localeNameDescription.Name, localeNameDescription.Description);
					dictionary[localeNameDescription.LocaleId] = value;
					if (flag)
					{
						dictionary[(uint)CultureInfo.InvariantCulture.LCID] = value;
						flag = false;
					}
				}
				this.namesAndDescriptions = dictionary;
			}

			private readonly string template;

			private Dictionary<uint, RmsTemplate.ServerRmsTemplate.NameAndDescriptionPair> namesAndDescriptions;

			[Serializable]
			private struct NameAndDescriptionPair
			{
				public NameAndDescriptionPair(string name, string description)
				{
					this.name = name;
					this.description = description;
				}

				public string Name
				{
					get
					{
						return this.name ?? string.Empty;
					}
				}

				public string Description
				{
					get
					{
						return this.description ?? string.Empty;
					}
				}

				private readonly string name;

				private readonly string description;
			}
		}

		[Serializable]
		private sealed class OneOffRmsTemplate : RmsTemplate
		{
			public OneOffRmsTemplate(Guid id, string cultureNeutralName, LocalizedString name, LocalizedString description, ICollection<string> recipientRights) : base(id)
			{
				if (recipientRights == null)
				{
					throw new ArgumentNullException("recipientRights");
				}
				this.cultureNeutralName = cultureNeutralName;
				this.name = name;
				this.description = description;
				this.recipientRights = recipientRights;
			}

			public override bool RequiresRepublishingWhenRecipientsChange
			{
				get
				{
					return true;
				}
			}

			protected override int GetSize()
			{
				int num = 0;
				if (!string.IsNullOrEmpty(this.cultureNeutralName))
				{
					num += this.cultureNeutralName.Length * 2;
				}
				if (!this.name.IsEmpty)
				{
					num += this.name.ToString().Length * 2;
				}
				if (!this.description.IsEmpty)
				{
					num += this.description.ToString().Length * 2;
				}
				return num + 4;
			}

			protected override SafeRightsManagementPubHandle CreateUnsignedIssuanceLicense(SafeRightsManagementPubHandle ownerHandle, SafeRightsManagementPubHandle fromHandle, IEnumerable<string> recipients)
			{
				if (recipients == null)
				{
					throw new ArgumentNullException("recipients");
				}
				SafeRightsManagementPubHandle result;
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					SafeRightsManagementPubHandle safeRightsManagementPubHandle;
					int hr = SafeNativeMethods.DRMCreateIssuanceLicense(null, null, null, null, ownerHandle ?? SafeRightsManagementPubHandle.InvalidHandle, null, SafeRightsManagementHandle.InvalidHandle, out safeRightsManagementPubHandle);
					disposeGuard.Add<SafeRightsManagementPubHandle>(safeRightsManagementPubHandle);
					Errors.ThrowOnErrorCode(hr);
					this.AddRights(safeRightsManagementPubHandle, ownerHandle, fromHandle, recipients);
					this.AddApplicationSpecificRights(safeRightsManagementPubHandle);
					if (!base.Equals(RmsTemplate.DoNotForward))
					{
						hr = SafeNativeMethods.DRMSetNameAndDescription(safeRightsManagementPubHandle, false, 0U, this.cultureNeutralName, base.GetDescription(CultureInfo.InvariantCulture));
						Errors.ThrowOnErrorCode(hr);
						foreach (CultureInfo cultureInfo in RmsTemplate.SupportedClientLanguages)
						{
							string value;
							string value2;
							this.GetNameAndDescription(cultureInfo, out value, out value2);
							if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(value2))
							{
								hr = SafeNativeMethods.DRMSetNameAndDescription(safeRightsManagementPubHandle, false, (uint)cultureInfo.LCID, value, value2);
								Errors.ThrowOnErrorCode(hr);
							}
						}
					}
					disposeGuard.Success();
					result = safeRightsManagementPubHandle;
				}
				return result;
			}

			protected override void GetNameAndDescription(CultureInfo locale, out string templateName, out string templateDescription)
			{
				templateName = this.name.ToString(locale);
				templateDescription = this.description.ToString(locale);
			}

			private void AddRights(SafeRightsManagementPubHandle issuanceLicenseHandle, SafeRightsManagementPubHandle ownerHandle, SafeRightsManagementPubHandle fromHandle, IEnumerable<string> recipients)
			{
				this.AddRecipientRights(issuanceLicenseHandle, recipients);
				if (ownerHandle != null && !ownerHandle.IsInvalid)
				{
					this.AddOwnerRights(issuanceLicenseHandle, ownerHandle);
				}
				if (fromHandle != null && !fromHandle.IsInvalid)
				{
					this.AddOwnerRights(issuanceLicenseHandle, fromHandle);
				}
			}

			private void AddOwnerRights(SafeRightsManagementPubHandle issuanceLicenseHandle, SafeRightsManagementPubHandle ownerHandle)
			{
				SafeRightsManagementPubHandle safeRightsManagementPubHandle = null;
				try
				{
					int hr = SafeNativeMethods.DRMCreateRight("OWNER", null, null, 0U, null, null, out safeRightsManagementPubHandle);
					Errors.ThrowOnErrorCode(hr);
					hr = SafeNativeMethods.DRMAddRightWithUser(issuanceLicenseHandle, safeRightsManagementPubHandle, ownerHandle);
					Errors.ThrowOnErrorCode(hr);
				}
				finally
				{
					if (safeRightsManagementPubHandle != null)
					{
						safeRightsManagementPubHandle.Close();
					}
				}
			}

			private void AddRecipientRights(SafeRightsManagementPubHandle issuanceLicenseHandle, IEnumerable<string> recipients)
			{
				SafeRightsManagementPubHandle[] array = new SafeRightsManagementPubHandle[this.recipientRights.Count];
				try
				{
					int num = 0;
					foreach (string rightName in this.recipientRights)
					{
						int hr = SafeNativeMethods.DRMCreateRight(rightName, null, null, 0U, null, null, out array[num++]);
						Errors.ThrowOnErrorCode(hr);
					}
					foreach (string userName in recipients)
					{
						SafeRightsManagementPubHandle safeRightsManagementPubHandle = null;
						try
						{
							int hr2 = SafeNativeMethods.DRMCreateUser(userName, null, "Unspecified", out safeRightsManagementPubHandle);
							Errors.ThrowOnErrorCode(hr2);
							foreach (SafeRightsManagementPubHandle rightHandle in array)
							{
								hr2 = SafeNativeMethods.DRMAddRightWithUser(issuanceLicenseHandle, rightHandle, safeRightsManagementPubHandle);
								Errors.ThrowOnErrorCode(hr2);
							}
						}
						finally
						{
							if (safeRightsManagementPubHandle != null)
							{
								safeRightsManagementPubHandle.Close();
							}
						}
					}
				}
				finally
				{
					foreach (SafeRightsManagementPubHandle safeRightsManagementPubHandle2 in array)
					{
						if (safeRightsManagementPubHandle2 != null)
						{
							safeRightsManagementPubHandle2.Close();
						}
					}
				}
			}

			private void AddApplicationSpecificRights(SafeRightsManagementPubHandle issuanceLicenseHandle)
			{
				int hr = SafeNativeMethods.DRMSetApplicationSpecificData(issuanceLicenseHandle, false, "ExchangeRecipientOrganizationExtractAllowed", "true");
				Errors.ThrowOnErrorCode(hr);
			}

			private readonly string cultureNeutralName;

			private readonly LocalizedString name;

			private readonly LocalizedString description;

			private readonly ICollection<string> recipientRights;
		}

		[Serializable]
		private struct LocaleNameDescription
		{
			internal LocaleNameDescription(uint localeId, string name, string description)
			{
				this.LocaleId = localeId;
				this.Name = name;
				this.Description = description;
			}

			internal readonly uint LocaleId;

			internal readonly string Name;

			internal readonly string Description;
		}
	}
}
