using System.IO;
using Levels;
using UnityEditor.Experimental.SceneManagement;

namespace CBX.Unity.Editors.Editor
{
    using System;
    using System.Collections.Generic;
    using CBX.TileMapping.Unity;

    using UnityEditor;

    using UnityEngine;


    [CustomEditor(typeof(TileMap))]
    public class TileMapEditor : Editor
    {

        private Vector3 _mouseHitPos;
        private PoolObject _selectedObj;

        Vector2 _zoneCellsPos;
        Vector2 _staticCellsPos;
        Vector2 _movingCellsPos;

        private GroupPoolObject _selectedGroup;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var map = (TileMap)this.target;

            EditorGUILayout.Space();

            if(map.Pallete != null)
                ShowGroups();

            if (_selectedGroup != null)
            {
                EditorGUILayout.LabelField("Moving Cells");
                _movingCellsPos =
                    EditorGUILayout.BeginScrollView(_movingCellsPos, GUILayout.Height(150));
                if (map.Pallete != null)
                    ShowPallete(map.Pallete.Cells);
                EditorGUILayout.EndScrollView();
            }

            if (GUILayout.Button("Serialize"))
            {
                SerializeGameObject(map.transform);
            }
        }      
        
        void SerializeGameObject(Transform obj)
        {
            if(!obj)
                return;

            var info = ObjectSerializer.SerializeObject(obj);

            var path = EditorUtility.SaveFilePanelInProject("Save file in", Selection.activeGameObject.name + ".txt", "txt",
                "");

            if (path.Length == 0)
                return;

            var writer = new StreamWriter(path, false);
            writer.Write(info);
            writer.Close();

            AssetDatabase.Refresh();
        }

        void ShowGroups()
        {
            var map = (TileMap)this.target;
            
            EditorGUILayout.LabelField("Groups");

            var groups = map.GetComponentsInChildren<GroupPoolObject>();

            for (int i = 0; i < groups.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();   
                var oldColor = GUI.backgroundColor;
                GUI.backgroundColor = groups[i].Color;


                if (_selectedGroup != groups[i])
                {
                    if (GUILayout.Button("Select " + groups[i].gameObject.name))
                        _selectedGroup = groups[i];
                }
                else
                {
                    GUILayout.Label("Selected");
                }

                if (GUILayout.Button("Remove " + groups[i].gameObject.name))
                {
                    DestroyImmediate(groups[i].gameObject);

                    for (int j = 0; j < groups[i].CellsCoordinates.Count; j++)
                    {
                        var cube = map.transform.Find(
                            $"Tile_{(int)groups[i].CellsCoordinates[j].Position.x}_{(int)groups[i].CellsCoordinates[j].Position.z}");
                        
                        if (cube == null) continue;
                        DestroyImmediate(cube.gameObject);
                    }
                }

                groups[i].Color = EditorGUILayout.ColorField(groups[i].Color);
                groups[i].PlayerPass = EditorGUILayout.Toggle(groups[i].PlayerPass);

                for (int j = 0; j < groups[i].CellsCoordinates.Count; j++)
                {
                    var cube = map.transform.Find(
                        $"Tile_{(int)groups[i].CellsCoordinates[j].Position.x}_{(int)groups[i].CellsCoordinates[j].Position.z}");

                    
                    if (cube == null) continue;
                    var cell = cube.GetComponent<CellPoolObject>();
                    cell.SetColor(groups[i].Color);
                }

                GUI.backgroundColor = oldColor;
                
                EditorGUILayout.EndHorizontal();
            }
            
            if (!GUILayout.Button("Add New Group"))
                return;
            
            _selectedGroup = PrefabUtility.InstantiatePrefab(map.Pallete.GroupObject) as GroupPoolObject;

            if (_selectedGroup != null)
            {
                _selectedGroup.transform.SetParent(map.transform);

                _selectedGroup.transform.localPosition = Vector3.zero;
                _selectedGroup.gameObject.name = "Group_" + groups.Length;
                _selectedGroup.Color = Color.gray;
            }

            EditorUtility.SetDirty(map.gameObject);
        }

        public void ShowPallete(List<PoolObject> objects, Action<PoolObject> onSelected = null)
        {
            EditorGUILayout.BeginHorizontal();
            
            for (var i = 0; i < objects.Count; i++)
            {
                if (!GUILayout.Button(GetObjectIcon(objects[i].gameObject)))
                    continue;
                
                _selectedObj = objects[i];
                onSelected?.Invoke(objects[i]);
            }
            
            EditorGUILayout.EndHorizontal();
        }

        public Texture2D GetObjectIcon(GameObject obj)
        {
            AssetPreview.SetPreviewTextureCacheSize(25);
            Texture2D icon = UnityEditor.AssetPreview.GetAssetPreview(obj);
            return icon;
        }
        /// <summary>
        /// Lets the Editor handle an event in the scene view.
        /// </summary>
        private void OnSceneGUI()
        {
            // if UpdateHitPosition return true we should update the scene views so that the marker will update in real time
            if (UpdateHitPosition())
            {
                SceneView.RepaintAll();
            }

            // Calculate the location of the marker based on the location of the mouse
            RecalculateMarkerPosition();

            // get a reference to the current event
            Event current = Event.current;

            // if the mouse is positioned over the layer allow drawing actions to occur
            if (IsMouseOnLayer())
            {
                // if mouse down or mouse drag event occurred
                if (current.type == EventType.MouseDown || current.type == EventType.MouseDrag)
                {
                    switch (current.button)
                    {
                        case 1:
                            // if right mouse button is pressed then we erase blocks
                            Erase();
                            current.Use();
                            break;
                        case 0:
                            // if left mouse button is pressed then we draw blocks
                            Draw();
                            current.Use();
                            
                            break;
                    }
                }
                
                if (current.type == EventType.ScrollWheel)
                {
                    Rotate(Math.Sign(current.delta.y));
                    current.Use();
                }
            }

            // draw a UI tip in scene view informing user how to draw & erase tiles
            Handles.BeginGUI();
            GUI.Label(new Rect(10, Screen.height - 90, 100, 100), "LMB: Draw");
            GUI.Label(new Rect(10, Screen.height - 105, 100, 100), "RMB: Erase");
            Handles.EndGUI();
        }

        /// <summary>
        /// When the <see cref="GameObject"/> is selected set the current tool to the view tool.
        /// </summary>
        private void OnEnable()
        {
            Tools.current = Tool.View;
            Tools.viewTool = ViewTool.FPS;
        }

        private void Rotate(int dir)
        {
            // get reference to the TileMap component
            var map = (TileMap)this.target;

            // Calculate the position of the mouse over the tile layer
            var tilePos = this.GetTilePositionFromMouseLocation();

            // Given the tile position check to see if a tile has already been created at that location
            var cube = map.transform.Find($"Tile_{tilePos.x}_{tilePos.y}");
            
            if (cube == null || cube.transform.parent != map.transform) return;
            
            cube.transform.Rotate(Vector3.up, 90 * dir);
            EditorUtility.SetDirty(map.gameObject);
        }

        /// <summary>
        /// Draws a block at the pre-calculated mouse hit position
        /// </summary>
        private void Draw()
        {
            // get reference to the TileMap component
            var map = (TileMap)this.target;

            // Calculate the position of the mouse over the tile layer
            var tilePos = GetTilePositionFromMouseLocation();

            if (map.transform.Find($"Tile_{tilePos.x}_{tilePos.y}") != null)
                return;

            
            // Given the tile position check to see if a tile has already been created at that location

            var cube = map.transform.Find($"Tile_{tilePos.x}_{tilePos.y}")?.gameObject;

            // if there is already a tile present and it is not a child of the game object we can just exit.
            if (cube != null && cube.transform.parent != map.transform)
            {      
                return;
            }
            if (cube != null)
                DestroyImmediate(cube);

            // if no game object was found we will create a cube
            if (cube == null && _selectedObj != null)
            {
                cube = PrefabUtility.InstantiatePrefab(_selectedObj.gameObject) as GameObject;
            }


            
            // set the cubes position on the tile map
            var tilePositionInLocalSpace = new Vector3((tilePos.x * map.TileWidth) + (map.TileWidth / 2), 0,
                (tilePos.y * map.TileHeight) + (map.TileHeight / 2));
            
            if (cube == null)
                return;
            
            cube.transform.position = map.transform.position + tilePositionInLocalSpace;
            
            // we scale the cube to the tile size defined by the TileMap.TileWidth and TileMap.TileHeight fields 
            //cube.transform.localScale = new Vector3(map.TileWidth,1, map.TileHeight);

            // set the cubes parent to the game object for organizational purposes
            cube.transform.SetParent(map.transform);

            if (Enum.TryParse(cube.name, out ObjectType type))
            {
                if (_selectedGroup != null)
                    _selectedGroup.AddCell(map.transform.position + tilePositionInLocalSpace, type);
            }

            // give the cube a name that represents it's location within the tile map
            cube.name = $"Tile_{tilePos.x}_{tilePos.y}";
            
            EditorUtility.SetDirty(map.gameObject);
        }

        /// <summary>
        /// Erases a block at the pre-calculated mouse hit position
        /// </summary>
        private void Erase()
        {
            // get reference to the TileMap component
            var map = (TileMap)this.target;

            // Calculate the position of the mouse over the tile layer
            var tilePos = this.GetTilePositionFromMouseLocation();
            
            var groups = map.GetComponentsInChildren<GroupPoolObject>();

            if (map.transform.Find($"Tile_{tilePos.x}_{tilePos.y}") == null)
                return;
            
            // Given the tile position check to see if a tile has already been created at that location
            var cube = map.transform.Find($"Tile_{tilePos.x}_{tilePos.y}").gameObject;

            // if a game object was found with the same name and it is a child we just destroy it immediately
            if (cube != null && cube.transform.parent == map.transform)
            {
                for (var i = 0; i < groups.Length; i++)
                    groups[i].RemoveCell(cube.transform.position);
                
                DestroyImmediate(cube);
                
                EditorUtility.SetDirty(map.gameObject);
            }
        }

        /// <summary>
        /// Calculates the location in tile coordinates (Column/Row) of the mouse position
        /// </summary>
        /// <returns>Returns a <see cref="Vector2"/> type representing the Column and Row where the mouse of positioned over.</returns>
        private Vector2 GetTilePositionFromMouseLocation()
        {
            // get reference to the tile map component
            var map = (TileMap)this.target;

            // calculate column and row location from mouse hit location
            var pos = new Vector3(this._mouseHitPos.x / map.TileWidth, map.transform.position.y, this._mouseHitPos.z / map.TileHeight);

            // round the numbers to the nearest whole number using 5 decimal place precision
            pos = new Vector3((int)Math.Round(pos.x, 5, MidpointRounding.ToEven),0 , (int)Math.Round(pos.z, 5, MidpointRounding.ToEven));

            // do a check to ensure that the row and column are with the bounds of the tile map
            var col = (int)pos.x;
            var row = (int)pos.z;
            if (row < 0)
            {
                row = 0;
            }

            if (row > map.Rows - 1)
            {
                row = map.Rows - 1;
            }

            if (col < 0)
            {
                col = 0;
            }

            if (col > map.Columns - 1)
            {
                col = map.Columns - 1;
            }

            // return the column and row values
            return new Vector2(col, row);
        }

        /// <summary>
        /// Returns true or false depending if the mouse is positioned over the tile map.
        /// </summary>
        /// <returns>Will return true if the mouse is positioned over the tile map.</returns>
        private bool IsMouseOnLayer()
        {
            // get reference to the tile map component
            var map = (TileMap)this.target;

            // return true or false depending if the mouse is positioned over the map
            return this._mouseHitPos.x > 0 && this._mouseHitPos.x < (map.Columns * map.TileWidth) &&
                   this._mouseHitPos.z > 0 && this._mouseHitPos.z < (map.Rows * map.TileHeight);
        }

        /// <summary>
        /// Recalculates the position of the marker based on the location of the mouse pointer.
        /// </summary>
        private void RecalculateMarkerPosition()
        {
            // get reference to the tile map component
            var map = (TileMap)this.target;

            // store the tile location (Column/Row) based on the current location of the mouse pointer
            var tilepos = this.GetTilePositionFromMouseLocation();

            // store the tile position in world space
            var pos = new Vector3(tilepos.x * map.TileWidth, tilepos.y * map.TileHeight, 0);

            // set the TileMap.MarkerPosition value
            map.MarkerPosition = map.transform.position + new Vector3(pos.x + (map.TileWidth / 2), pos.y + (map.TileHeight / 2), 0);
        }

        /// <summary>
        /// Calculates the position of the mouse over the tile map in local space coordinates.
        /// </summary>
        /// <returns>Returns true if the mouse is over the tile map.</returns>
        private bool UpdateHitPosition()
        {
            // get reference to the tile map component
            var map = (TileMap)this.target;

            // build a plane object that 
            var p = new Plane(map.transform.TransformDirection(Vector3.down), map.transform.position);

            // build a ray type from the current mouse position
            var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            // stores the hit location
            var hit = new Vector3();

            // stores the distance to the hit location
            float dist;

            // cast a ray to determine what location it intersects with the plane
            if (p.Raycast(ray, out dist))
            {
                // the ray hits the plane so we calculate the hit location in world space
                hit = ray.origin + (ray.direction.normalized * dist);
            }

            // convert the hit location from world space to local space
            var value = map.transform.InverseTransformPoint(hit);

            // if the value is different then the current mouse hit location set the 
            // new mouse hit location and return true indicating a successful hit test
            if (value != this._mouseHitPos)
            {
                this._mouseHitPos = value;
                return true;
            }

            // return false if the hit test failed
            return false;
        }
    }
}