using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Sabotris.IO;
using UnityEngine;

namespace Sabotris.Util
{
    public static class FileUtil
    {
        private const string GameSettingsFilename = "/sabotris_settings.cfg";
        private const string GameInputFilename = "/sabotris_input.cfg";
        
        private static void Save(string filename, object obj)
        {
            var formatter = new BinaryFormatter();
            var stream = new FileStream(Application.persistentDataPath + filename, FileMode.Create);
            
            formatter.Serialize(stream, obj);
            
            stream.Close();
        }

        private static T Load<T>(string path) where T : class
        {
            if (File.Exists(Application.persistentDataPath + path))
            {
                var formatter = new BinaryFormatter();
                var stream = new FileStream(Application.persistentDataPath + path, FileMode.Open);

                var content = formatter.Deserialize(stream) as T;
                
                stream.Close();

                return content;
            }

            Logging.Error(false, "Failed to load file {0}, not found", path);
            return null;
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