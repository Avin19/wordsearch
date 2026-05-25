#if UNITY_EDITOR

using UnityEditor;
using System.IO;

namespace SimpleUtils
{
    [InitializeOnLoad]
    public class SimpleFolderChecker
    {
        public struct FolderInfo
        {
            public string Name;
            public string Path;
        }

        static readonly FolderInfo[] _filesToCheck =
        {
            // new FolderInfo
            // {
            //     Name = "Demigiant | Dotween",
            //     Path = "Assets\\Plugins\\Demigiant"
            // }
        };

        static SimpleFolderChecker()
        {
            foreach (var path in _filesToCheck)
                CheckFolder(path);
        }

        public static void CheckFolder(FolderInfo info)
        {
            if (!Directory.Exists(info.Path))
            {
                EditorUtility.DisplayDialog("Folder Missing", $"{info.Name} should be at \n{info.Path}\n, but was not found.", "OK");
            }
        }
    }
}

#endif