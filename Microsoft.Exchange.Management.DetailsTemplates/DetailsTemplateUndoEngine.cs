using System;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace Microsoft.Exchange.Management.DetailsTemplates
{
	internal class DetailsTemplateUndoEngine : UndoEngine
	{
		public DetailsTemplateUndoEngine(IServiceProvider provider) : base(provider)
		{
		}

		public void DoUndo()
		{
			if (this.currentPos > 0)
			{
				UndoEngine.UndoUnit undoUnit = this.undoUnitList[this.currentPos - 1];
				undoUnit.Undo();
				this.currentPos--;
			}
			this.UpdateUndoRedoMenuCommandsStatus();
		}

		public void DoRedo()
		{
			if (this.currentPos < this.undoUnitList.Count)
			{
				UndoEngine.UndoUnit undoUnit = this.undoUnitList[this.currentPos];
				undoUnit.Undo();
				this.currentPos++;
			}
			this.UpdateUndoRedoMenuCommandsStatus();
		}

		private void UpdateUndoRedoMenuCommandsStatus()
		{
			IMenuCommandService menuCommandService = base.GetService(typeof(IMenuCommandService)) as IMenuCommandService;
			if (menuCommandService != null)
			{
				MenuCommand menuCommand = menuCommandService.FindCommand(StandardCommands.Undo);
				MenuCommand menuCommand2 = menuCommandService.FindCommand(StandardCommands.Redo);
				if (menuCommand != null)
				{
					menuCommand.Enabled = (this.currentPos > 0 && this.undoUnitList.Count > 0);
				}
				if (menuCommand2 != null)
				{
					menuCommand2.Enabled = (this.currentPos < this.undoUnitList.Count && this.undoUnitList.Count > 0);
				}
			}
		}

		protected override void AddUndoUnit(UndoEngine.UndoUnit unit)
		{
			this.undoUnitList.RemoveRange(this.currentPos, this.undoUnitList.Count - this.currentPos);
			if (this.undoUnitList.Count > 100)
			{
				this.undoUnitList.RemoveAt(0);
			}
			this.undoUnitList.Add(unit);
			this.currentPos = this.undoUnitList.Count;
			this.UpdateUndoRedoMenuCommandsStatus();
		}

		protected override void DiscardUndoUnit(UndoEngine.UndoUnit unit)
		{
			this.undoUnitList.Remove(unit);
			this.currentPos--;
			base.DiscardUndoUnit(unit);
			this.UpdateUndoRedoMenuCommandsStatus();
		}

		private const int MaxUndoCount = 100;

		private List<UndoEngine.UndoUnit> undoUnitList = new List<UndoEngine.UndoUnit>();

		private int currentPos;
	}
}
