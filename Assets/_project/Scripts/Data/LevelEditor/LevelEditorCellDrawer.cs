using System.Collections;
using System.Collections.Generic;
using Framework.GameData;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.Drawers;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DashTerritory
{
    internal sealed class LevelEditorCellDrawer<TArray> : TwoDimensionalArrayDrawer<TArray, LevelTileItem>
        where TArray : IList
    {
        private Texture TileIcon => Resources.Load<Texture>("LevelEditor/TileIcon");
        
        protected override TableMatrixAttribute GetDefaultTableMatrixAttributeSettings()
        {
            return new TableMatrixAttribute()
            {
                SquareCells = true,
                HideColumnIndices = true,
                HideRowIndices = true,
                ResizableColumns = false
            };
        }

        private bool wasClicked = false;
        protected override LevelTileItem DrawElement(Rect rect, LevelTileItem value)
        {
            wasClicked = false;
            var id = DragAndDropUtilities.GetDragAndDropId(rect);

            //GUI.backgroundColor = value.color;
            var icon = TileIcon;
            if (value.environmentItem) icon = value.environmentItem.icon;
            DragAndDropUtilities.DrawDropZone(rect, value.isNotPopulated ? null : icon, null, id);

            value = DrawTileGui(rect, value);

            GUI.backgroundColor = Color.white;

            value = DragAndDropUtilities.DropZone(rect, value); // Drop zone for ItemSlot structs.
            value.environmentItem = DragAndDropUtilities.DropZone(rect, value.environmentItem); // Drop zone for Item types.
            value.environmentItem = DragAndDropUtilities.DragZone(rect, value.environmentItem, true, true); // Enables dragging of the ItemSlot

            return value;
        }

        private LevelTileItem DrawTileGui(Rect rect, LevelTileItem value)
        {
            if (!EditorWindow.mouseOverWindow) return value;
            
            var gameManagerWindow = EditorWindow.GetWindow(typeof(GameManagerWindow)) as OdinMenuEditorWindow;
            var current = Mouse.current;
            var mousePosition = current.position.ReadValue();
            
            if (gameManagerWindow != null)
            {
                mousePosition.x -= gameManagerWindow.MenuWidth;
                mousePosition.y -= gameManagerWindow.MenuTree.Config.SearchToolbarHeight;
            }

            if (!rect.Contains(mousePosition)) return value;
            
            GUI.backgroundColor = value.isNotPopulated ? Color.green : Color.red;
            var deleteRect = rect.AlignRight(18).AlignBottom(18);
            if (GUI.Button(deleteRect, string.Format(value.isNotPopulated ? "+" : "-")))
            {
                value.isNotPopulated = !value.isNotPopulated;
            }
            
            return value;
        }
    }
}
