using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.InfoWorker.Common.Availability;

namespace Microsoft.Exchange.UM.PersonalAutoAttendant
{
	internal class PersonalAutoAttendant : IComparable<PersonalAutoAttendant>, IEquatable<PersonalAutoAttendant>
	{
		private PersonalAutoAttendant()
		{
			this.keyMappingList = new KeyMappings();
			this.autoActionsList = new KeyMappings();
			this.callerIdList = new List<CallerIdBase>();
			this.extensionList = new ExtensionList();
		}

		internal Version Version
		{
			get
			{
				return this.version;
			}
			set
			{
				this.version = value;
			}
		}

		internal string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		internal Guid Identity
		{
			get
			{
				return this.identity;
			}
			set
			{
				this.identity = value;
				this.greeting = this.identity.ToString("N");
			}
		}

		internal bool Enabled
		{
			get
			{
				return this.enabled;
			}
			set
			{
				this.enabled = value;
			}
		}

		internal ExtensionList ExtensionList
		{
			get
			{
				return this.extensionList;
			}
			set
			{
				this.extensionList = value;
			}
		}

		internal List<CallerIdBase> CallerIdList
		{
			get
			{
				return this.callerIdList;
			}
			set
			{
				this.callerIdList = value;
			}
		}

		internal bool Valid
		{
			get
			{
				return this.valid;
			}
			set
			{
				this.valid = value;
			}
		}

		internal TimeOfDayEnum TimeOfDay
		{
			get
			{
				return this.timeOfDay;
			}
			set
			{
				this.timeOfDay = value;
			}
		}

		internal WorkingPeriod WorkingPeriod
		{
			get
			{
				return this.workingPeriod;
			}
			set
			{
				this.workingPeriod = value;
			}
		}

		internal FreeBusyStatusEnum FreeBusy
		{
			get
			{
				return this.freeBusy;
			}
			set
			{
				this.freeBusy = value;
			}
		}

		internal OutOfOfficeStatusEnum OutOfOffice
		{
			get
			{
				return this.outOfOffice;
			}
			set
			{
				this.outOfOffice = value;
			}
		}

		internal bool EnableBargeIn
		{
			get
			{
				return this.enableBargeIn;
			}
			set
			{
				this.enableBargeIn = value;
			}
		}

		internal string Greeting
		{
			get
			{
				return this.greeting;
			}
			set
			{
				this.greeting = value;
			}
		}

		internal string OwaPreview
		{
			get
			{
				return this.owaPreview;
			}
			set
			{
				this.owaPreview = value;
			}
		}

		internal bool IsCompatible
		{
			get
			{
				return this.paaIsOfCurrentVersion;
			}
			set
			{
				this.paaIsOfCurrentVersion = value;
			}
		}

		internal KeyMappings KeyMappingList
		{
			get
			{
				return this.keyMappingList;
			}
			set
			{
				this.keyMappingList = value;
			}
		}

		internal KeyMappings AutoActionsList
		{
			get
			{
				return this.autoActionsList;
			}
			set
			{
				this.autoActionsList = value;
			}
		}

		internal XmlNode[] Unprocessed
		{
			get
			{
				return this.unprocessed;
			}
			set
			{
				this.unprocessed = value;
			}
		}

		internal List<XmlNode> DocumentNodes
		{
			get
			{
				return this.rawNodes;
			}
			set
			{
				this.rawNodes = value;
			}
		}

		public int CompareTo(PersonalAutoAttendant other)
		{
			return this.Identity.CompareTo(other.Identity);
		}

		public bool Equals(PersonalAutoAttendant other)
		{
			return this.Identity.Equals(other.Identity);
		}

		public bool Validate(IDataValidator dataValidator, PAAValidationMode validationMode)
		{
			this.valid = true;
			if (validationMode != PAAValidationMode.Actions)
			{
				this.valid = this.extensionList.Validate(dataValidator);
				if (!this.valid && validationMode == PAAValidationMode.StopOnFirstError)
				{
					return false;
				}
				for (int i = 0; i < this.CallerIdList.Count; i++)
				{
					CallerIdBase callerIdBase = this.CallerIdList[i];
					if (!callerIdBase.Validate(dataValidator))
					{
						this.valid = false;
						if (validationMode == PAAValidationMode.StopOnFirstError)
						{
							break;
						}
					}
				}
				if (!this.valid && validationMode == PAAValidationMode.StopOnFirstError)
				{
					return false;
				}
			}
			this.ValidateKeyMappings(this.autoActionsList, dataValidator, validationMode);
			if (!this.valid && validationMode == PAAValidationMode.StopOnFirstError)
			{
				return false;
			}
			this.ValidateKeyMappings(this.keyMappingList, dataValidator, validationMode);
			return (this.valid || validationMode != PAAValidationMode.StopOnFirstError) && this.valid;
		}

		public void ValidateKeyMappings(KeyMappings keyMappings, IDataValidator dataValidator, PAAValidationMode validationMode)
		{
			for (int i = 0; i < keyMappings.Count; i++)
			{
				KeyMappingBase keyMappingBase = keyMappings.Menu[i];
				if (!keyMappingBase.Validate(dataValidator))
				{
					this.valid = false;
					if (validationMode == PAAValidationMode.StopOnFirstError)
					{
						return;
					}
				}
			}
		}

		internal static PersonalAutoAttendant CreateNew()
		{
			PersonalAutoAttendant personalAutoAttendant = new PersonalAutoAttendant();
			personalAutoAttendant.Version = PAAConstants.CurrentVersion;
			personalAutoAttendant.IsCompatible = true;
			personalAutoAttendant.Identity = Guid.NewGuid();
			personalAutoAttendant.EnableBargeIn = true;
			personalAutoAttendant.KeyMappingList.AddTransferToVoicemail(null);
			return personalAutoAttendant;
		}

		internal static PersonalAutoAttendant CreateUninitialized()
		{
			return new PersonalAutoAttendant();
		}

		internal void AddPhoneNumberCallerId(string phone)
		{
			this.AddCallerId(new PhoneNumberCallerId(phone));
		}

		internal void AddADContactCallerId(string legacyExchangeDN)
		{
			this.AddCallerId(new ADContactCallerId(legacyExchangeDN));
		}

		internal void AddPersonaContactCallerId(EmailAddress emailAddress)
		{
			this.AddCallerId(new PersonaContactCallerId(emailAddress));
		}

		internal void AddPersonaContactCallerId(string emailAddressWithDisplayName)
		{
			string[] array = emailAddressWithDisplayName.Split(new string[]
			{
				":"
			}, StringSplitOptions.RemoveEmptyEntries);
			EmailAddress emailAddress = new EmailAddress();
			if (array != null)
			{
				switch (array.Length)
				{
				case 1:
					emailAddress.Address = array[0];
					break;
				case 2:
					emailAddress.Address = array[0];
					emailAddress.Name = array[1];
					break;
				}
				this.AddPersonaContactCallerId(emailAddress);
			}
		}

		internal void AddDefaultContactFolderCallerId()
		{
			this.AddCallerId(new ContactFolderCallerId());
		}

		internal void AddContactItemCallerId(string base64String)
		{
			this.AddContactItemCallerId(StoreObjectId.Deserialize(base64String));
		}

		internal void AddContactItemCallerId(StoreObjectId storeObjectId)
		{
			this.AddContactItemCallerId(new ContactItemCallerId(storeObjectId));
		}

		internal void AddContactItemCallerId(ContactItemCallerId callerId)
		{
			this.AddCallerId(callerId);
		}

		private void AddCallerId(CallerIdBase callerId)
		{
			this.callerIdList.Add(callerId);
		}

		private Version version;

		private string name;

		private Guid identity;

		private bool enabled;

		private bool paaIsOfCurrentVersion;

		private bool valid;

		private ExtensionList extensionList;

		private List<CallerIdBase> callerIdList;

		private TimeOfDayEnum timeOfDay;

		private WorkingPeriod workingPeriod;

		private FreeBusyStatusEnum freeBusy;

		private OutOfOfficeStatusEnum outOfOffice;

		private bool enableBargeIn;

		private string greeting;

		private string owaPreview;

		private KeyMappings keyMappingList;

		private KeyMappings autoActionsList;

		private XmlNode[] unprocessed;

		private List<XmlNode> rawNodes;
	}
}
