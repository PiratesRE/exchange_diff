using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.CalendarDiagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	public class Inconsistency : IXmlSerializable
	{
		protected Inconsistency()
		{
			this.ShouldFix = false;
		}

		protected Inconsistency(RoleType owner, string description, CalendarInconsistencyFlag flag, CalendarValidationContext context) : this()
		{
			this.Owner = owner;
			this.Description = description;
			this.Flag = flag;
			this.OwnerIsGroupMailbox = context.IsRoleGroupMailbox(this.Owner);
		}

		internal static Inconsistency CreateInstance(RoleType owner, string description, CalendarInconsistencyFlag flag, CalendarValidationContext context)
		{
			return new Inconsistency(owner, description, flag, context);
		}

		internal static Inconsistency CreateMissingCvsInconsistency(RoleType owner, CalendarVersionStoreNotPopulatedException exc, CalendarValidationContext context)
		{
			return Inconsistency.CreateInstance(owner, string.Format("The Calendar Version Store is not fully populated yet (Wait Time: {0}).", exc.WaitTimeBeforeThrow), CalendarInconsistencyFlag.MissingCvs, context);
		}

		internal virtual RumInfo CreateRumInfo(CalendarValidationContext context, IList<Attendee> attendees)
		{
			switch (this.Flag)
			{
			case CalendarInconsistencyFlag.None:
			case CalendarInconsistencyFlag.StoreObjectValidation:
			case CalendarInconsistencyFlag.StorageException:
			case CalendarInconsistencyFlag.UserNotFound:
			case CalendarInconsistencyFlag.LegacyUser:
			case CalendarInconsistencyFlag.LargeDL:
			case CalendarInconsistencyFlag.RecurrenceAnomaly:
			case CalendarInconsistencyFlag.RecurringException:
			case CalendarInconsistencyFlag.ModifiedOccurrenceMatch:
			case CalendarInconsistencyFlag.DuplicatedItem:
			case CalendarInconsistencyFlag.MissingCvs:
				return NullOpRumInfo.CreateInstance();
			case CalendarInconsistencyFlag.VersionInfo:
			case CalendarInconsistencyFlag.TimeOverlap:
			case CalendarInconsistencyFlag.StartTime:
			case CalendarInconsistencyFlag.EndTime:
			case CalendarInconsistencyFlag.StartTimeZone:
			case CalendarInconsistencyFlag.RecurringTimeZone:
			case CalendarInconsistencyFlag.Location:
			case CalendarInconsistencyFlag.RecurrenceBlob:
			case CalendarInconsistencyFlag.MissingOccurrenceDeletion:
				return UpdateRumInfo.CreateMasterInstance(attendees, this.Flag);
			}
			throw new NotImplementedException(string.Format("Unrecognized inconsistency: {0}", this.Flag));
		}

		internal CalendarInconsistencyFlag Flag { get; private set; }

		internal RoleType Owner { get; private set; }

		public bool OwnerIsGroupMailbox { get; private set; }

		internal string Description { get; private set; }

		public bool ShouldFix { get; internal set; }

		public ClientIntentFlags? Intent { get; internal set; }

		public XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			throw new NotSupportedException("XML deserialization is not supported.");
		}

		public virtual void WriteXml(XmlWriter writer)
		{
			writer.WriteElementString("Owner", this.Owner.ToString());
			writer.WriteElementString("OwnerIsGroupMailbox", this.OwnerIsGroupMailbox.ToString());
			writer.WriteElementString("Description", this.Description);
		}
	}
}
