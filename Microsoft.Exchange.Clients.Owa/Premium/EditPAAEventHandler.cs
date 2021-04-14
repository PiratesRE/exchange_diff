using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.UM.PersonalAutoAttendant;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventNamespace("PAA")]
	internal sealed class EditPAAEventHandler : OwaEventHandlerBase
	{
		public static void Register()
		{
			OwaEventRegistry.RegisterStruct(typeof(PAADurationInfo));
			OwaEventRegistry.RegisterStruct(typeof(PAACallerIdInfo));
			OwaEventRegistry.RegisterStruct(typeof(PAAFindMeInfo));
			OwaEventRegistry.RegisterStruct(typeof(PAATransferToInfo));
			OwaEventRegistry.RegisterHandler(typeof(EditPAAEventHandler));
		}

		[OwaEventParameter("CPh", typeof(string), true, true)]
		[OwaEventParameter("Name", typeof(string))]
		[OwaEventParameter("Ext", typeof(string), true, true)]
		[OwaEventParameter("FndMe", typeof(PAAFindMeInfo), true, true)]
		[OwaEventParameter("XfrTo", typeof(PAATransferToInfo), true, true)]
		[OwaEventParameter("Intrpt", typeof(bool), false, true)]
		[OwaEventParameter("OOF", typeof(bool), false, true)]
		[OwaEventParameter("RecVM", typeof(bool), false, true)]
		[OwaEventParameter("CllrId", typeof(PAACallerIdInfo), false, true)]
		[OwaEventParameter("CRcps", typeof(RecipientInfo), true, true)]
		[OwaEvent("Save")]
		[OwaEventParameter("SchS", typeof(int), false, true)]
		[OwaEventParameter("Dur", typeof(PAADurationInfo), false, true)]
		[OwaEventParameter("SvO", typeof(bool), false, true)]
		[OwaEventParameter("Id", typeof(string), false, true)]
		public void Save()
		{
			string text = (string)base.GetParameter("Name");
			if (!string.IsNullOrEmpty(text))
			{
				text = text.Trim();
			}
			if (string.IsNullOrEmpty(text))
			{
				base.RenderPartialFailure(-1816947001);
				return;
			}
			if (text.Length > 256)
			{
				text = text.Substring(0, 256);
			}
			string text2 = (string)base.GetParameter("Id");
			using (IPAAStore ipaastore = PAAStore.Create(base.UserContext.ExchangePrincipal))
			{
				IList<PersonalAutoAttendant> list = null;
				ipaastore.TryGetAutoAttendants(PAAValidationMode.None, out list);
				if (list.Count >= 9 && string.IsNullOrEmpty(text2))
				{
					base.RenderPartialFailure(1125061902);
				}
				else
				{
					PersonalAutoAttendant personalAutoAttendant = PersonalAutoAttendant.CreateNew();
					personalAutoAttendant.Name = text;
					string[] array = (string[])base.GetParameter("Ext");
					if (array != null)
					{
						foreach (string item in array)
						{
							personalAutoAttendant.ExtensionList.Add(item);
						}
					}
					if (base.IsParameterSet("SchS"))
					{
						int freeBusy = (int)base.GetParameter("SchS");
						personalAutoAttendant.FreeBusy = (FreeBusyStatusEnum)freeBusy;
					}
					this.AddDuration(personalAutoAttendant);
					this.AddCallerId(personalAutoAttendant);
					if (base.GetParameter("OOF") != null)
					{
						personalAutoAttendant.OutOfOffice = OutOfOfficeStatusEnum.Oof;
					}
					this.AddFindMe(personalAutoAttendant);
					this.AddTransferTo(personalAutoAttendant);
					if (base.GetParameter("RecVM") == null)
					{
						personalAutoAttendant.KeyMappingList.Remove(10);
					}
					personalAutoAttendant.EnableBargeIn = (bool)base.GetParameter("Intrpt");
					personalAutoAttendant.Enabled = true;
					if (string.IsNullOrEmpty(text2))
					{
						list.Add(personalAutoAttendant);
					}
					else
					{
						Guid identity = new Guid(Convert.FromBase64String(text2));
						personalAutoAttendant.Identity = identity;
						PersonalAutoAttendant autoAttendant = ipaastore.GetAutoAttendant(identity, PAAValidationMode.None);
						int index = list.Count;
						if (autoAttendant != null)
						{
							index = list.IndexOf(autoAttendant);
							list.RemoveAt(index);
						}
						list.Insert(index, personalAutoAttendant);
					}
					if (base.IsParameterSet("SvO"))
					{
						this.Writer.Write("<div id=paaId>");
						Utilities.HtmlEncode(Convert.ToBase64String(personalAutoAttendant.Identity.ToByteArray()), this.Writer);
						this.Writer.Write("</div>");
					}
					ipaastore.Save(list);
				}
			}
		}

		[OwaEvent("VldC")]
		[OwaEventParameter("CllrId", typeof(PAACallerIdInfo), true, false)]
		[OwaEventParameter("CRcps", typeof(RecipientInfo), true, true)]
		[OwaEventParameter("CPh", typeof(string), true, true)]
		public void ValidateCallerId()
		{
			PAACallerIdInfo paacallerIdInfo = (PAACallerIdInfo)base.GetParameter("CllrId");
			string text = null;
			using (IPAAStore ipaastore = PAAStore.Create(base.UserContext.ExchangePrincipal))
			{
				if (paacallerIdInfo.HasPhoneNumbers)
				{
					string[] array = (string[])base.GetParameter("CPh");
					if (array.Length > 50)
					{
						text = Utilities.HtmlEncode(string.Format(CultureInfo.CurrentCulture, LocalizedStrings.GetNonEncoded(1755659442), new object[]
						{
							50
						}));
					}
					if (text == null)
					{
						text = UnifiedMessagingUtilities.ValidatePhoneNumbers(new UnifiedMessagingUtilities.ValidatePhoneNumber(ipaastore.ValidatePhoneNumberCallerId), array);
					}
				}
				IDataValidationResult dataValidationResult = null;
				if (text == null && paacallerIdInfo.HasContacts)
				{
					RecipientInfo[] array2 = (RecipientInfo[])base.GetParameter("CRcps");
					if (array2.Length > 50)
					{
						text = Utilities.HtmlEncode(string.Format(CultureInfo.CurrentCulture, LocalizedStrings.GetNonEncoded(2034140420), new object[]
						{
							50
						}));
					}
					if (text == null)
					{
						foreach (RecipientInfo recipientInfo in array2)
						{
							if (recipientInfo.AddressOrigin == AddressOrigin.Store)
							{
								ipaastore.ValidateContactItemCallerId(recipientInfo.StoreObjectId, out dataValidationResult);
							}
							else if (recipientInfo.AddressOrigin == AddressOrigin.Directory)
							{
								ipaastore.ValidateADContactCallerId(recipientInfo.RoutingAddress, out dataValidationResult);
							}
							if (dataValidationResult.PAAValidationResult != PAAValidationResult.Valid)
							{
								text = UnifiedMessagingUtilities.GetErrorResourceId(dataValidationResult.PAAValidationResult, recipientInfo.DisplayName);
								break;
							}
						}
					}
				}
				if (text == null && paacallerIdInfo.IsInContactFolder)
				{
					ipaastore.ValidateContactFolderCallerId(out dataValidationResult);
					if (dataValidationResult.PAAValidationResult != PAAValidationResult.Valid)
					{
						text = UnifiedMessagingUtilities.GetErrorResourceId(dataValidationResult.PAAValidationResult, null);
					}
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				this.RenderErrorInfobar(text);
			}
		}

		[OwaEventParameter("FndMe", typeof(PAAFindMeInfo), false, false)]
		[OwaEvent("VldF")]
		public void ValidateFindMe()
		{
			PAAFindMeInfo paafindMeInfo = (PAAFindMeInfo)base.GetParameter("FndMe");
			string text = null;
			using (IPAAStore ipaastore = PAAStore.Create(base.UserContext.ExchangePrincipal))
			{
				List<string> list = new List<string>();
				if (!string.IsNullOrEmpty(paafindMeInfo.Ph1))
				{
					list.Add(paafindMeInfo.Ph1);
				}
				if (!string.IsNullOrEmpty(paafindMeInfo.Ph2))
				{
					list.Add(paafindMeInfo.Ph2);
				}
				text = UnifiedMessagingUtilities.ValidatePhoneNumbers(new UnifiedMessagingUtilities.ValidatePhoneNumber(ipaastore.ValidatePhoneNumberForOutdialing), list.ToArray());
			}
			if (!string.IsNullOrEmpty(text))
			{
				this.RenderErrorInfobar(text);
			}
		}

		[OwaEvent("VldT")]
		[OwaEventParameter("XfrTo", typeof(PAATransferToInfo), false, false)]
		public void ValidateTransferTo()
		{
			PAATransferToInfo paatransferToInfo = (PAATransferToInfo)base.GetParameter("XfrTo");
			string text = null;
			using (IPAAStore ipaastore = PAAStore.Create(base.UserContext.ExchangePrincipal))
			{
				if (!string.IsNullOrEmpty(paatransferToInfo.Ph))
				{
					text = UnifiedMessagingUtilities.ValidatePhoneNumbers(new UnifiedMessagingUtilities.ValidatePhoneNumber(ipaastore.ValidatePhoneNumberForOutdialing), new string[]
					{
						paatransferToInfo.Ph
					});
				}
				else
				{
					IDataValidationResult dataValidationResult = null;
					if (paatransferToInfo.VM)
					{
						ipaastore.ValidateADContactForTransferToMailbox(paatransferToInfo.Contact, out dataValidationResult);
					}
					else
					{
						ipaastore.ValidateADContactForOutdialing(paatransferToInfo.Contact, out dataValidationResult);
					}
					if (dataValidationResult.PAAValidationResult != PAAValidationResult.Valid)
					{
						text = UnifiedMessagingUtilities.GetErrorResourceId(dataValidationResult.PAAValidationResult, paatransferToInfo.Contact);
					}
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				this.RenderErrorInfobar(text);
			}
		}

		private void RenderErrorInfobar(string messageHtml)
		{
			this.Writer.Write("<div id=eib>");
			this.Writer.Write(messageHtml);
			this.Writer.Write("</div>");
		}

		private void AddDuration(PersonalAutoAttendant paa)
		{
			PAADurationInfo paadurationInfo = (PAADurationInfo)base.GetParameter("Dur");
			if (paadurationInfo == null)
			{
				return;
			}
			if (paadurationInfo.IsCustomDuration)
			{
				paa.TimeOfDay = TimeOfDayEnum.Custom;
				paa.WorkingPeriod = new WorkingPeriod((DaysOfWeek)paadurationInfo.DaysOfWeek, paadurationInfo.StartTimeMinutes, paadurationInfo.EndTimeMinutes);
				return;
			}
			if (paadurationInfo.IsWorkingHours)
			{
				paa.TimeOfDay = TimeOfDayEnum.WorkingHours;
				return;
			}
			paa.TimeOfDay = TimeOfDayEnum.NonWorkingHours;
		}

		private void AddCallerId(PersonalAutoAttendant paa)
		{
			PAACallerIdInfo paacallerIdInfo = (PAACallerIdInfo)base.GetParameter("CllrId");
			if (paacallerIdInfo == null)
			{
				return;
			}
			if (paacallerIdInfo.HasPhoneNumbers)
			{
				string[] array = (string[])base.GetParameter("CPh");
				foreach (string phoneNumber in array)
				{
					paa.CallerIdList.Add(CallerIdBase.CreatePhoneNumber(phoneNumber));
				}
			}
			if (paacallerIdInfo.HasContacts)
			{
				RecipientInfo[] array3 = (RecipientInfo[])base.GetParameter("CRcps");
				foreach (RecipientInfo recipientInfo in array3)
				{
					CallerIdBase item = null;
					if (recipientInfo.AddressOrigin == AddressOrigin.Store)
					{
						item = CallerIdBase.CreateContactItem(recipientInfo.StoreObjectId);
					}
					else if (recipientInfo.AddressOrigin == AddressOrigin.Directory)
					{
						item = new ADContactCallerId(recipientInfo.RoutingAddress);
					}
					paa.CallerIdList.Add(item);
				}
			}
			if (paacallerIdInfo.IsInContactFolder)
			{
				paa.CallerIdList.Add(new ContactFolderCallerId());
			}
		}

		private void AddFindMe(PersonalAutoAttendant paa)
		{
			PAAFindMeInfo[] array = (PAAFindMeInfo[])base.GetParameter("FndMe");
			if (array == null)
			{
				return;
			}
			foreach (PAAFindMeInfo paafindMeInfo in array)
			{
				if (!string.IsNullOrEmpty(paafindMeInfo.Ph1))
				{
					paa.KeyMappingList.AddFindMe(paafindMeInfo.Key, paafindMeInfo.Desc, paafindMeInfo.Ph1, paafindMeInfo.Tm1);
				}
				if (!string.IsNullOrEmpty(paafindMeInfo.Ph2))
				{
					paa.KeyMappingList.AddFindMe(paafindMeInfo.Key, paafindMeInfo.Desc, paafindMeInfo.Ph2, paafindMeInfo.Tm2);
				}
			}
		}

		private void AddTransferTo(PersonalAutoAttendant paa)
		{
			PAATransferToInfo[] array = (PAATransferToInfo[])base.GetParameter("XfrTo");
			if (array == null)
			{
				return;
			}
			foreach (PAATransferToInfo paatransferToInfo in array)
			{
				if (!string.IsNullOrEmpty(paatransferToInfo.Ph))
				{
					paa.KeyMappingList.AddTransferToNumber(paatransferToInfo.Key, paatransferToInfo.Desc, paatransferToInfo.Ph);
				}
				else
				{
					if (string.IsNullOrEmpty(paatransferToInfo.Contact))
					{
						throw new OwaInvalidRequestException("Required field 'Contact' legacy distinguished name is null or empty string");
					}
					if (!Utilities.IsValidLegacyDN(paatransferToInfo.Contact))
					{
						throw new OwaInvalidRequestException(string.Format(CultureInfo.InvariantCulture, "Required field 'Contact' legacy distinguished name is invalid {0}", new object[]
						{
							paatransferToInfo.Contact
						}));
					}
					if (paatransferToInfo.VM)
					{
						paa.KeyMappingList.AddTransferToADContactMailbox(paatransferToInfo.Key, paatransferToInfo.Desc, paatransferToInfo.Contact);
					}
					else
					{
						paa.KeyMappingList.AddTransferToADContactPhone(paatransferToInfo.Key, paatransferToInfo.Desc, paatransferToInfo.Contact);
					}
				}
			}
		}

		public const string EventNamespace = "PAA";

		public const string MethodSave = "Save";

		public const string MethodValidateCallerId = "VldC";

		public const string MethodValidateFindMe = "VldF";

		public const string MethodValidateTransferTo = "VldT";

		public const string Id = "Id";

		public const string Name = "Name";

		public const string InterruptGreetingParameter = "Intrpt";

		public const string OutOfOfficeAssistantParameter = "OOF";

		public const string RecordAVoiceMessageParameter = "RecVM";

		public const string DialledExtConditionParameter = "Ext";

		public const string DurationConditionParameter = "Dur";

		public const string CallerIdConditionParameter = "CllrId";

		public const string FindMeActionParameter = "FndMe";

		public const string TransferToActionParameter = "XfrTo";

		public const string CallerIsRecipients = "CRcps";

		public const string CallerIsPhoneNumbers = "CPh";

		public const string ScheduleStatusConditionParameter = "SchS";

		public const string SaveAndStayOpen = "SvO";

		private const int MaxNameLength = 256;

		private const int MaxCallerIdPhoneNumbers = 50;

		private const int MaxCallerIdContacts = 50;

		private const int MaxPersonalAutoAttendants = 9;
	}
}
