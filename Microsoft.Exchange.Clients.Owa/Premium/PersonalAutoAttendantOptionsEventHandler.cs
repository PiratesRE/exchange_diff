using System;
using System.Collections.Generic;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.UM.PersonalAutoAttendant;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventNamespace("PaaOptions")]
	internal sealed class PersonalAutoAttendantOptionsEventHandler : OwaEventHandlerBase
	{
		public static void Register()
		{
			OwaEventRegistry.RegisterHandler(typeof(PersonalAutoAttendantOptionsEventHandler));
		}

		[OwaEvent("Move")]
		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEventParameter("Id", typeof(string), false, false)]
		[OwaEventParameter("Ids", typeof(string), true, true)]
		[OwaEventParameter("op", typeof(int), false, false)]
		public void Move()
		{
			using (IPAAStore ipaastore = PAAStore.Create(base.UserContext.ExchangePrincipal))
			{
				IList<PersonalAutoAttendant> list = null;
				ipaastore.TryGetAutoAttendants(PAAValidationMode.StopOnFirstError, out list);
				Guid identity = new Guid(Convert.FromBase64String((string)base.GetParameter("Id")));
				int num;
				PersonalAutoAttendant personalAutoAttendant = PersonalAutoAttendantOptionsEventHandler.FindAutoAttendantByGuid(list, identity, out num);
				if (personalAutoAttendant != null)
				{
					if (this.IsOrderChanged(list))
					{
						base.RenderPartialFailure(-846213614, OwaEventHandlerErrorCode.UnexpectedError);
					}
					else
					{
						int num2 = (int)base.GetParameter("op");
						if ((num2 != 1 || num == 0) && (num2 != 2 || num >= list.Count - 1))
						{
							throw new OwaInvalidRequestException("Event name and parameter doesn't match");
						}
						list.RemoveAt(num);
						if (num2 == 1)
						{
							list.Insert(num - 1, personalAutoAttendant);
						}
						else
						{
							list.Insert(num + 1, personalAutoAttendant);
						}
						ipaastore.Save(list);
						ipaastore.TryGetAutoAttendants(PAAValidationMode.StopOnFirstError, out list);
					}
				}
				else
				{
					base.RenderPartialFailure(-289549140, OwaEventHandlerErrorCode.ItemNotFound);
				}
				this.RefreshList(list);
			}
		}

		[OwaEvent("Enable")]
		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEventParameter("Id", typeof(string), false, false)]
		[OwaEventParameter("Ids", typeof(string), true, true)]
		[OwaEventParameter("op", typeof(int), false, false)]
		public void Enable()
		{
			using (IPAAStore ipaastore = PAAStore.Create(base.UserContext.ExchangePrincipal))
			{
				IList<PersonalAutoAttendant> list = null;
				ipaastore.TryGetAutoAttendants(PAAValidationMode.StopOnFirstError, out list);
				Guid identity = new Guid(Convert.FromBase64String((string)base.GetParameter("Id")));
				int index;
				PersonalAutoAttendant personalAutoAttendant = PersonalAutoAttendantOptionsEventHandler.FindAutoAttendantByGuid(list, identity, out index);
				if (personalAutoAttendant != null)
				{
					int num = (int)base.GetParameter("op");
					if (num == 3)
					{
						personalAutoAttendant.Enabled = true;
					}
					else
					{
						if (num != 4)
						{
							throw new OwaInvalidRequestException("Event name and parameter doesn't match");
						}
						personalAutoAttendant.Enabled = false;
					}
					ipaastore.Save(list);
					ipaastore.TryGetAutoAttendants(PAAValidationMode.StopOnFirstError, out list);
					personalAutoAttendant = list[index];
					this.Writer.Write("<div id=\"ret\" enbl=");
					this.Writer.Write(personalAutoAttendant.Enabled ? 1 : 0);
					this.Writer.Write("></div>");
				}
				else
				{
					base.RenderPartialFailure(-289549140, OwaEventHandlerErrorCode.ItemNotFound);
					this.RefreshList(list);
				}
			}
		}

		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEvent("Delete")]
		[OwaEventParameter("Id", typeof(string), false, false)]
		[OwaEventParameter("Ids", typeof(string), true, true)]
		public void Delete()
		{
			using (IPAAStore ipaastore = PAAStore.Create(base.UserContext.ExchangePrincipal))
			{
				Guid identity = new Guid(Convert.FromBase64String((string)base.GetParameter("Id")));
				PersonalAutoAttendant personalAutoAttendant = null;
				ipaastore.TryGetAutoAttendant(identity, PAAValidationMode.None, out personalAutoAttendant);
				if (personalAutoAttendant != null)
				{
					ipaastore.DeleteAutoAttendant(identity);
				}
				IList<PersonalAutoAttendant> personalAutoAttendants = null;
				ipaastore.TryGetAutoAttendants(PAAValidationMode.StopOnFirstError, out personalAutoAttendants);
				this.RefreshList(personalAutoAttendants);
			}
		}

		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEvent("Refresh")]
		public void Refresh()
		{
			using (IPAAStore ipaastore = PAAStore.Create(base.UserContext.ExchangePrincipal))
			{
				IList<PersonalAutoAttendant> personalAutoAttendants = null;
				ipaastore.TryGetAutoAttendants(PAAValidationMode.StopOnFirstError, out personalAutoAttendants);
				this.RefreshList(personalAutoAttendants);
			}
		}

		private static PersonalAutoAttendant FindAutoAttendantByGuid(IList<PersonalAutoAttendant> autoattendants, Guid identity, out int index)
		{
			index = -1;
			PersonalAutoAttendant result = null;
			if (autoattendants != null)
			{
				for (int i = 0; i < autoattendants.Count; i++)
				{
					if (autoattendants[i].Identity == identity)
					{
						index = i;
						result = autoattendants[i];
						break;
					}
				}
			}
			return result;
		}

		private void RefreshList(IList<PersonalAutoAttendant> personalAutoAttendants)
		{
			PersonalAutoAttendantListView personalAutoAttendantListView = new PersonalAutoAttendantListView(base.UserContext, personalAutoAttendants);
			personalAutoAttendantListView.Render(this.Writer);
		}

		private bool IsOrderChanged(IList<PersonalAutoAttendant> personalAutoAttendants)
		{
			if (base.IsParameterSet("Ids"))
			{
				string[] array = (string[])base.GetParameter("Ids");
				if (array.Length != personalAutoAttendants.Count)
				{
					return true;
				}
				for (int i = 0; i < personalAutoAttendants.Count; i++)
				{
					if (!personalAutoAttendants[i].Identity.Equals(new Guid(Convert.FromBase64String(array[i]))))
					{
						return true;
					}
				}
			}
			return false;
		}

		public const string EventNamespace = "PaaOptions";

		public const string MethodMove = "Move";

		public const string MethodEnable = "Enable";

		public const string MethodDelete = "Delete";

		public const string MethodRefresh = "Refresh";

		public const string Id = "Id";

		public const string Ids = "Ids";

		public const string Operation = "op";

		public const int OperationMoveUp = 1;

		public const int OperationMoveDown = 2;

		public const int OperationEnable = 3;

		public const int OperationDisable = 4;
	}
}
