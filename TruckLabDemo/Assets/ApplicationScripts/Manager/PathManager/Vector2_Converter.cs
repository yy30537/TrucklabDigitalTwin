using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ApplicationScripts.Manager.PathManager
{
    /// <summary>
    /// Custom JSON converter for serializing and deserializing Unity's Vector2 type.
    /// </summary>
    public class Vector2_Converter : JsonConverter<Vector2>
    {
        /// <summary>
        /// Writes a Vector2 object to JSON as a two-element array.
        /// </summary>
        public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
        {
            // Serialize the Vector2 as a JSON array [x, y]
            JArray ja = new JArray { value.x, value.y };
            ja.WriteTo(writer);
        }

        /// <summary>
        /// Reads a JSON array and converts it into a Vector2 object.
        /// </summary>
        public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // Load the JSON array and parse it into a Vector2
            JArray ja = JArray.Load(reader);

            // Ensure the array has the expected structure [x, y]
            if (ja.Count != 2)
            {
                throw new JsonSerializationException("Invalid Vector2 format. Expected an array with exactly two elements.");
            }

            // Convert the array elements to a Vector2
            return new Vector2((float)ja[0], (float)ja[1]);
        }
    }
}