using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxTransport.ContentAggregation.Schema;
using Microsoft.Exchange.Net.Facebook;
using Microsoft.Exchange.Transport.Sync.Common;

namespace Microsoft.Exchange.Transport.Sync.Worker.Framework.Provider.Facebook
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class FacebookContact : DisposeTrackableBase, ISyncContact, ISyncObject, IDisposeTrackable, IDisposable
	{
		internal FacebookContact(FacebookUser user, ExDateTime peopleConnectionCreationTime)
		{
			SyncUtilities.ThrowIfArgumentNull("user", user);
			SyncUtilities.ThrowIfArgumentNullOrEmpty("user.Id", user.Id);
			this.user = user;
			this.InitializeBirthdate();
			this.peopleConnectionCreationTime = new ExDateTime?(peopleConnectionCreationTime);
			this.InitializeHobbies();
			this.InitializeCompanyNameAndJobTitle();
			this.InitializeSchools();
			this.InitializeOscContactSources();
			this.InitializePhotoUrl();
			this.SuppressDisposeTracker();
		}

		private static bool DateTimeTryParseExact(string dateTime, out ExDateTime result)
		{
			return ExDateTime.TryParseExact(dateTime, "MM/dd/yyyy", null, DateTimeStyles.None, out result);
		}

		public SchemaType Type
		{
			get
			{
				base.CheckDisposed();
				return SchemaType.Contact;
			}
		}

		public ExDateTime? LastModifiedTime
		{
			get
			{
				base.CheckDisposed();
				ExDateTime value;
				if (ExDateTime.TryParse(this.user.UpdatedTime, out value))
				{
					return new ExDateTime?(value);
				}
				return null;
			}
		}

		public ExDateTime? BirthDate
		{
			get
			{
				base.CheckDisposed();
				return this.birthdate;
			}
		}

		public ExDateTime? BirthDateLocal
		{
			get
			{
				base.CheckDisposed();
				return this.birthdate;
			}
		}

		public string BusinessAddressCity
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public string BusinessAddressCountry
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public string BusinessAddressPostalCode
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public string BusinessAddressState
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public string BusinessAddressStreet
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public string BusinessFaxNumber
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public string BusinessTelephoneNumber
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public string CompanyName
		{
			get
			{
				base.CheckDisposed();
				return this.companyName;
			}
		}

		public string DisplayName
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public string Email1Address
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public string Email2Address
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public string Email3Address
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public string FileAs
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public string FirstName
		{
			get
			{
				base.CheckDisposed();
				return this.user.FirstName;
			}
		}

		public string Hobbies
		{
			get
			{
				base.CheckDisposed();
				return this.hobbies;
			}
		}

		public string HomeAddressCity
		{
			get
			{
				base.CheckDisposed();
				if (this.user.Location == null)
				{
					return null;
				}
				return this.user.Location.Name;
			}
		}

		public string HomeAddressCountry
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public string HomeAddressPostalCode
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public string HomeAddressState
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public string HomeAddressStreet
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public string HomeTelephoneNumber
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public string IMAddress
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public string JobTitle
		{
			get
			{
				base.CheckDisposed();
				return this.jobTitle;
			}
		}

		public string LastName
		{
			get
			{
				base.CheckDisposed();
				return this.user.LastName;
			}
		}

		public string Location
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public string MiddleName
		{
			get
			{
				base.CheckDisposed();
				return string.Empty;
			}
		}

		public string MobileTelephoneNumber
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public byte[] OscContactSources
		{
			get
			{
				base.CheckDisposed();
				return this.oscContactSources;
			}
		}

		public string OtherTelephoneNumber
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public string PartnerNetworkContactType
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public string PartnerNetworkId
		{
			get
			{
				base.CheckDisposed();
				return "Facebook";
			}
		}

		public string PartnerNetworkProfilePhotoUrl
		{
			get
			{
				base.CheckDisposed();
				return this.photoUrl;
			}
		}

		public string PartnerNetworkThumbnailPhotoUrl
		{
			get
			{
				base.CheckDisposed();
				return this.photoUrl;
			}
		}

		public ExDateTime? PeopleConnectionCreationTime
		{
			get
			{
				base.CheckDisposed();
				return this.peopleConnectionCreationTime;
			}
		}

		private void InitializePhotoUrl()
		{
			this.photoUrl = string.Format("https://graph.facebook.com/{0}/picture?type=large", this.user.Id);
		}

		public string PartnerNetworkUserId
		{
			get
			{
				base.CheckDisposed();
				return this.user.Id;
			}
		}

		public string ProtectedEmailAddress
		{
			get
			{
				base.CheckDisposed();
				return this.user.EmailAddress;
			}
		}

		public string ProtectedPhoneNumber
		{
			get
			{
				base.CheckDisposed();
				return this.user.MobilePhoneNumber;
			}
		}

		public string Schools
		{
			get
			{
				base.CheckDisposed();
				return this.schools;
			}
		}

		public string Webpage
		{
			get
			{
				base.CheckDisposed();
				return this.user.ProfilePageUrl;
			}
		}

		protected override void InternalDispose(bool disposing)
		{
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<FacebookContact>(this);
		}

		private void InitializeCompanyNameAndJobTitle()
		{
			if (this.user.WorkHistory == null || this.user.WorkHistory.Count == 0)
			{
				this.companyName = null;
				this.jobTitle = null;
				return;
			}
			FacebookWorkHistoryEntry facebookWorkHistoryEntry = this.FindCurrentJob() ?? this.user.WorkHistory[0];
			this.companyName = ((facebookWorkHistoryEntry.Employer != null) ? facebookWorkHistoryEntry.Employer.Name : null);
			this.jobTitle = ((facebookWorkHistoryEntry.Position != null) ? facebookWorkHistoryEntry.Position.Name : null);
		}

		private FacebookWorkHistoryEntry FindCurrentJob()
		{
			return this.user.WorkHistory.FirstOrDefault(new Func<FacebookWorkHistoryEntry, bool>(this.IsCurrentJob));
		}

		private bool IsCurrentJob(FacebookWorkHistoryEntry job)
		{
			return job != null && !string.IsNullOrWhiteSpace(job.StartDate) && !"0000-00".Equals(job.StartDate, StringComparison.OrdinalIgnoreCase) && (string.IsNullOrWhiteSpace(job.EndDate) || "0000-00".Equals(job.EndDate, StringComparison.OrdinalIgnoreCase));
		}

		private void InitializeBirthdate()
		{
			if (!string.IsNullOrEmpty(this.user.Birthday))
			{
				ExDateTime value;
				if (FacebookContact.DateTimeTryParseExact(this.user.Birthday, out value))
				{
					this.birthdate = new ExDateTime?(value);
					return;
				}
				if (FacebookContact.DateTimeTryParseExact(string.Format("{0}/{1}", this.user.Birthday, 1604), out value))
				{
					this.birthdate = new ExDateTime?(value);
				}
			}
		}

		private void InitializeHobbies()
		{
			HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			if (this.user.ActivitiesList != null && this.user.ActivitiesList.Activities != null)
			{
				hashSet.UnionWith(from a in this.user.ActivitiesList.Activities
				select a.Name);
			}
			if (this.user.InterestsList != null && this.user.InterestsList.Interests != null)
			{
				hashSet.UnionWith(from i in this.user.InterestsList.Interests
				select i.Name);
			}
			if (hashSet.Count == 0)
			{
				this.hobbies = null;
				return;
			}
			this.hobbies = string.Join(", ", hashSet.ToArray<string>());
		}

		private void InitializeSchools()
		{
			if (this.user.EducationHistory == null)
			{
				this.schools = null;
				return;
			}
			string[] array = (from educationEntry in this.user.EducationHistory
			where educationEntry.School != null && !string.IsNullOrWhiteSpace(educationEntry.School.Name)
			select educationEntry.School.Name).ToArray<string>();
			if (array.Length == 0)
			{
				this.schools = null;
			}
			this.schools = string.Join(", ", array);
		}

		private void InitializeOscContactSources()
		{
			this.oscContactSources = OscContactSourcesForContactWriter.Instance.Write(OscProviderGuids.Facebook, "", this.user.Id);
		}

		public const string FacebookPartnerNetworkId = "Facebook";

		private const string PhotoUrlFormat = "https://graph.facebook.com/{0}/picture?type=large";

		private readonly FacebookUser user;

		private ExDateTime? birthdate;

		private string hobbies;

		private string companyName;

		private string jobTitle;

		private string schools;

		private byte[] oscContactSources;

		private readonly ExDateTime? peopleConnectionCreationTime;

		private string photoUrl;
	}
}
