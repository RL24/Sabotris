using System;
using System.IO;
using Sabotris.IO;
using UnityEngine;

namespace Sabotris.Util
{
    public static class FileUtil
    {
        private const string GameSettingsFilename = "/sabotris_settings.json";
        private const string GameInputFilename = "/sabotris_input.json";

        private static string GetFilePath(string filename) => $"{Application.persistentDataPath}{filename}";

        private static string ReadJson(string filename)
        {
            try
            {
                var reader = new StreamReader(GetFilePath(filename));
                var json = reader.ReadToEnd();
                reader.Close();
                return json;
            }
            catch (SystemException e)
            {
                Debug.LogError(e.Message);
            }

            return null;
        }

        private static void WriteJson(string filename, string json)
        {
            try
            {
                var writer = new StreamWriter(GetFilePath(filename));
                writer.Write(json);
                writer.Close();
            }
            catch (SystemException e)
            {
                Debug.LogError(e.Message);
            }
        }
        
        private static void Save(string filename, object obj)
        {
            if (obj == null)
                return;
            
            var json = JsonUtility.ToJson(obj, true);
            WriteJson(filename, json);
        }

        private static T Load<T>(string filename) where T : class, new()
        {
            var type = new T();
            if (File.Exists(GetFilePath(filename)))
            {
                var json = ReadJson(filename);
                if (json != null)
                    JsonUtility.FromJsonOverwrite(json, type);
            } else
                Logging.Error(false, "Failed to load file {0}, not found", filename);
            return type;
        }

        public static void SaveGameSettings(GameSettingsConfig config)
        {
            Save(GameSettingsFilename, config);
        }

        public static void SaveGameInput(GameInputConfig config)
        {
            Save(GameInputFilename, config);
        }

        public static GameSettingsConfig LoadGameSettings()
        {
            return Load<GameSettingsConfig>(GameSettingsFilename);
        }

        public static GameInputConfig LoadGameInput()
        {
            return Load<GameInputConfig>(GameInputFilename);
        }
    }
}