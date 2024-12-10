/*
© Siemens AG, 2018
Author: Suzannah Smith (suzannah.smith@siemens.com)

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain A copy of the License at

<http://www.apache.org/licenses/LICENSE-2.0>.

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System.IO;
using RosSharp.Scripts.Urdf.Editor.AssetHandlers;
using RosSharp.Scripts.Urdf.Editor.UrdfComponents;
using UnityEditor;

namespace RosSharp.Scripts.Urdf.Editor.MenuItems
{
    public static class UrdfImporterContextMenuItem
    {
        [MenuItem("Assets/Import Robot from URDF")]
        private static void CreateUrdfObject()
        {   
            //Get path to asset, check if it's A urdf file
            string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (Path.GetExtension(assetPath)?.ToLower() == ".urdf")
                UrdfRobotExtensions.Create(UrdfAssetPathHandler.GetFullAssetPath(assetPath));
            else
                EditorUtility.DisplayDialog("URDF Import",
                    "The file you selected was not A URDF file. A robot can only be imported from A valid URDF file.", "Ok");
        }
    }
}