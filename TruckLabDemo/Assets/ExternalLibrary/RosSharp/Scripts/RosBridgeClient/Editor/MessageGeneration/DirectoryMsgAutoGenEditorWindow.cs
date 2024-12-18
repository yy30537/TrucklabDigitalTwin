﻿/*
© Siemens AG, 2019
Author: Sifan Ye (sifan.ye@siemens.com)

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

using System.Collections.Generic;
using RosSharp.RosBridgeClient.MessageGeneration;
using UnityEditor;
using UnityEngine;

namespace RosSharp.Scripts.RosBridgeClient.Editor.MessageGeneration
{
    public class DirectoryMsgAutoGenEditorWindow : DirectoryAutoGenEditorWindow
    {
        protected override string GenerationType
        {
            get { return "message"; }
        }

        protected override string FileExtension
        {
            get { return "msg"; }
        }

        [MenuItem("RosBridgeClient/Auto Generate Messages/All Messages in directory...", false, 2)]
        public static void OpenWindow()
        {
            DirectoryMsgAutoGenEditorWindow window = GetWindow<DirectoryMsgAutoGenEditorWindow>(false, "Message Auto Generation", true);
            window.minSize = new Vector2(750, 100);
            window.maxSize = new Vector2(750, 100);
            window.Show();
        }

        protected override List<string> Generate(string inPath, string outPath, string rosPackageName = "")
        {
            return MessageAutoGen.GenerateSingleMessage(inPath, outPath, rosPackageName);
        }

    }
}