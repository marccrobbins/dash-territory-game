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

        protected override LevelTileItem DrawElement(Rect rect, LevelTileItem value)
        {
            var id = DragAndDropUtilities.GetDragAndDropId(rect);

            var icon = TileIcon;
            if (value.tile) icon = value.tile.icon;
            DragAndDropUtilities.DrawDropZone(rect, value.isNotPopulated ? null : icon, null, id);

            value = DrawTileGui(rect, value);

            GUI.backgroundColor = Color.white;

            value = DragAndDropUtilities.DropZone(rect, value); // Drop zone for ItemSlot structs.
            value.tile = DragAndDropUtilities.DropZone(rect, value.tile); // Drop zone for Item types.
            value.tile = DragAndDropUtilities.DragZone(rect, value.tile, true, true); // Enables dragging of the ItemSlot

            if (!value.isNotPopulated)
            {
                //Drop zone for modifier
                value.modifier = DragAndDropUtilities.DropZone(rect, value.modifier);
            }

            return value;
        }

        private LevelTileItem DrawTileGui(Rect rect, LevelTileItem value)
        {
            if (!EditorWindow.mouseOverWindow) return value;

            var gameManagerWindowInfo = GameManagerWindowInfo.Instance;
            var current = Mouse.current;
            var mousePosition = current.position.ReadValue();
            
            if (gameManagerWindowInfo != null)
            {
                mousePosition.x -= gameManagerWindowInfo.MenuWidth;
                mousePosition.y -= gameManagerWindowInfo.MenuSearchBarHeight - 5;
            }

            //Draw modifiers buttons
            var modifierRect = rect.AlignLeft(22).AlignTop(22);
            if (value.modifier)
            {
                var isMouseOver = modifierRect.Contains(mousePosition);
                if (isMouseOver)
                {
                    GUI.backgroundColor = Color.red;
                    if (GUI.Button(modifierRect, "x"))
                    {
                        value.modifier = null;
                    }
                    GUI.backgroundColor = Color.white;
                }
                else
                {
                    GUI.Button(modifierRect, value.modifier.icon);
                }
            }
            
            if (!rect.Contains(mousePosition)) return value;
            
            //ToDo maybe switch to an x if a tile is present
            GUI.backgroundColor = value.isNotPopulated ? Color.green : Color.red;
            var deleteRect = rect.AlignRight(18).AlignBottom(18);
            if (GUI.Button(deleteRect, string.Format(value.isNotPopulated ? "+" : "-")))
            {
                value.isNotPopulated = !value.isNotPopulated;
                value.tile = null;
                value.modifier = null;
            }
            
            return value;
        }
    }
}
