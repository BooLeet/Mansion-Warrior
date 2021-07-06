using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelEditorStates
{

    public abstract class LevelEditorState
    {
        public abstract void Init(LevelEditor editor);

        public abstract void Action(LevelEditor editor);

        public abstract LevelEditorState Transition(LevelEditor editor);
    }


    public class EditingState : LevelEditorState
    {
        public override void Action(LevelEditor editor)
        {
            editor.UpdateCamera();
            editor.UpdateObjectPosition();
            editor.UpdateCurrentBlock();
            editor.DashboardSelectionUpdate();

            if(editor.input.GetUndo())
            {
                editor.UndoAction();
                return;
            }

            if (editor.input.GetRedo())
            {
                editor.RedoAction();
                return;
            }

            if (editor.input.GetChangeLighting())
            {
                editor.CycleLighting();
            }

            if (editor.SelectedItemType == LevelEditor.SelectedItem.ItemType.Material && editor.SelectedObject == null)
            {
                if (editor.input.MainAction())
                {
                    if (editor.input.GetActionModifier())
                        editor.RemoveBlock();
                    else
                        editor.AddBlock();
                }
                if (editor.input.SecondaryActionHold())
                    editor.PaintBlock();

                editor.ShowHideBlockHighilighter(true);
                editor.ShowHideObjectHighilighter(false);
                editor.UpdateBlockHighlighter();
            }
            else
            {
                if (editor.input.MainAction())
                {
                    if (editor.input.GetActionModifier())
                        editor.RemoveObject();
                    else
                        editor.AddObject();
                }
                if (editor.input.SecondaryAction())
                {
                    if (editor.input.GetReverseRotation())
                        editor.RotateObject(false);
                    else
                        editor.RotateObject(true);
                }

                editor.ShowHideBlockHighilighter(false);
                editor.ShowHideObjectHighilighter(true);
                editor.UpdateObjectHighlighter(!editor.input.GetActionModifier());
            }
        }

        public override void Init(LevelEditor editor)
        {
            Utility.DisableCursor();
            editor.ui.HideMaterialSelector();
            editor.ui.HideObjectSelector();
            editor.ui.ShowDashboard();
            Menu.Instance.CanShow = true;
        }

        public override LevelEditorState Transition(LevelEditor editor)
        {
            if (editor.input.GetMaterialSelector())
                return new MaterialSelector();
            else if (editor.input.GetObjectSelector())
                return new ObjectSelector();
            else if (editor.input.GetSaveLevel())
                return new SaveLevel();
            else if (editor.input.GetLoadLevel())
                return new LoadLevel();

            return null;
        }
    }

    public class MaterialSelector : LevelEditorState
    {
        public override void Action(LevelEditor editor)
        {
            editor.DashboardSelectionUpdate();
        }

        public override void Init(LevelEditor editor)
        {
            Utility.EnableCursor();
            editor.ui.ShowMaterialSelector();
            Menu.Instance.CanShow = false;
        }

        public override LevelEditorState Transition(LevelEditor editor)
        {
            if (editor.input.GetCloseSelector())
            {
                editor.ui.HideMaterialSelector();
                return new EditingState();
            }
            if (editor.input.GetObjectSelector())
            {
                editor.ui.HideMaterialSelector();
                return new ObjectSelector();
            }
            return null;
        }
    }

    public class ObjectSelector : LevelEditorState
    {
        public override void Action(LevelEditor editor)
        {
            editor.DashboardSelectionUpdate();
        }

        public override void Init(LevelEditor editor)
        {
            Utility.EnableCursor();
            editor.ui.ShowObjectSelector();
            Menu.Instance.CanShow = false;
        }

        public override LevelEditorState Transition(LevelEditor editor)
        {
            if (editor.input.GetCloseSelector())
            {
                editor.ui.HideObjectSelector();
                return new EditingState();
            }
            if (editor.input.GetMaterialSelector())
            {
                editor.ui.HideObjectSelector();
                return new MaterialSelector();
            }
            return null;
        }
    }

    public class SaveLevel : LevelEditorState
    {
        public override void Action(LevelEditor editor)
        {
            
        }

        public override void Init(LevelEditor editor)
        {
            editor.ui.ShowSaveScreen();
            editor.ui.HideDashboard();
            Utility.EnableCursor();
            editor.ui.inputField.text = System.IO.Path.GetFileNameWithoutExtension(editor.LevelToLoad);
            Menu.Instance.CanShow = false;
        }

        public override LevelEditorState Transition(LevelEditor editor)
        {
            if (editor.CancelSave || editor.input.GetCancel())
            {
                editor.CancelSave = false;
                editor.ui.HideSaveScreen();
                return new EditingState();
            }
            if(editor.ConfirmSave || editor.input.GetConfirm())
            {
                editor.ConfirmSave = false;
                string levelName = editor.ui.GetSaveLevelName();
                if (levelName.Length == 0)
                    return null;
                editor.SaveLevel(levelName);
                editor.ui.HideSaveScreen();
                return new EditingState();
            }

            return null;
        }
    }

    public class LoadLevel : LevelEditorState
    {
        public override void Action(LevelEditor editor)
        {
            
        }

        public override void Init(LevelEditor editor)
        {
            editor.ui.ShowLoadScreen();
            editor.ui.HideDashboard();
            editor.ui.LoadScreenUpdateLevels(Application.dataPath);
            Utility.EnableCursor();
            Menu.Instance.CanShow = false;
        }

        public override LevelEditorState Transition(LevelEditor editor)
        {
            if (editor.ConfirmLoad)
            {
                editor.ConfirmLoad = false;
                editor.LoadLevel(editor.LevelToLoad, true);
                editor.ui.HideLoadScreen();
                return new EditingState();
            }
            if (editor.CancelLoad || editor.input.GetCancel())
            {
                editor.CancelLoad = false;
                editor.ui.HideLoadScreen();
                return new EditingState();
            }
            return null;
        }
    }

}