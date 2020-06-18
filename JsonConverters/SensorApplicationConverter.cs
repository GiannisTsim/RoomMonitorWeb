
// using System;
// using System.Text.Json;
// using System.Text.Json.Serialization;

// using RoomMonitor.Models;

// namespace RoomMonitor.JsonConverters
// {
//     internal class SensorApplicationConverter : JsonConverter<SensorApplication>
//     {

//         public override bool CanConvert(Type typeToConvert) =>
//             typeof(SensorApplication).IsAssignableFrom(typeToConvert);

//         public override SensorApplication Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
//         {
//             if (reader.TokenType != JsonTokenType.StartObject)
//             {
//                 throw new JsonException();
//             }

//             reader.Read();
//             if (reader.TokenType != JsonTokenType.PropertyName)
//             {
//                 throw new JsonException();
//             }

//             string propertyName = reader.GetString();
//             if (propertyName != "SensorType")
//             {
//                 throw new JsonException();
//             }

//             reader.Read();
//             if (reader.TokenType != JsonTokenType.String)
//             {
//                 throw new JsonException();
//             }

//             string sensorType = reader.GetString();

//             SensorApplication sensorApplication = sensorType switch
//             {
//                 "Switch" => new ApplicationSwitch() { SensorType = sensorType },
//                 "Measure" => new ApplicationMeasure() { SensorType = sensorType },
//                 _ => throw new JsonException()
//             };

//             while (reader.Read())
//             {
//                 if (reader.TokenType == JsonTokenType.EndObject)
//                 {
//                     return sensorApplication;
//                 }

//                 if (reader.TokenType == JsonTokenType.PropertyName)
//                 {
//                     propertyName = reader.GetString();
//                     reader.Read();
//                     switch (propertyName)
//                     {
//                         case "Application":
//                             string applicationType = reader.GetString();
//                             sensorApplication.Application = applicationType;
//                             break;
//                         case "Name":
//                             string name = reader.GetString();
//                             sensorApplication.Name = name;
//                             break;
//                         case "Description":
//                             string description = reader.GetString();
//                             sensorApplication.Description = description;
//                             break;
//                         case "Value_0":
//                             string value_0 = reader.GetString();
//                             ((ApplicationSwitch)sensorApplication).Value_0 = value_0;
//                             break;
//                         case "Value_1":
//                             string value_1 = reader.GetString();
//                             ((ApplicationSwitch)sensorApplication).Value_1 = value_1;
//                             break;
//                         case "UnitMeasure":
//                             string unitMeasure = reader.GetString();
//                             ((ApplicationMeasure)sensorApplication).UnitMeasure = unitMeasure;
//                             break;
//                         case "LimitMin":
//                             decimal limitMin = reader.GetDecimal();
//                             ((ApplicationMeasure)sensorApplication).LimitMin = limitMin;
//                             break;
//                         case "LimitMax":
//                             decimal limitMax = reader.GetDecimal();
//                             ((ApplicationMeasure)sensorApplication).LimitMax = limitMax;
//                             break;
//                         case "DefaultMin":
//                             decimal defaultMin = reader.GetDecimal();
//                             ((ApplicationMeasure)sensorApplication).DefaultMin = defaultMin;
//                             break;
//                         case "DefaultMax":
//                             decimal defaultMax = reader.GetDecimal();
//                             ((ApplicationMeasure)sensorApplication).DefaultMax = defaultMax;
//                             break;
//                     }
//                 }

//             }
//             throw new JsonException();
//         }

//         public override void Write(Utf8JsonWriter writer, SensorApplication sensorApplication, JsonSerializerOptions options)
//         {
//             writer.WriteStartObject();

//             writer.WriteString("Application", sensorApplication.Application);
//             writer.WriteString("Name", sensorApplication.Name);
//             writer.WriteString("Description", sensorApplication.Description);
//             writer.WriteString("SensorType", sensorApplication.SensorType);

//             if (sensorApplication is ApplicationSwitch applicationSwitch)
//             {
//                 writer.WriteString("Value_0", applicationSwitch.Value_0);
//                 writer.WriteString("Value_1", applicationSwitch.Value_1);
//             }
//             else if (sensorApplication is ApplicationMeasure applicationMeasure)
//             {
//                 writer.WriteString("UnitMeasure", applicationMeasure.UnitMeasure);

//                 if (applicationMeasure.LimitMin.HasValue)
//                     writer.WriteNumber("LimitMin", applicationMeasure.LimitMin.Value);
//                 if (applicationMeasure.LimitMax.HasValue)
//                     writer.WriteNumber("LimitMax", applicationMeasure.LimitMax.Value);
//                 if (applicationMeasure.DefaultMin.HasValue)
//                     writer.WriteNumber("DefaultMin", applicationMeasure.DefaultMin.Value);
//                 if (applicationMeasure.DefaultMax.HasValue)
//                     writer.WriteNumber("DefaultMax", applicationMeasure.DefaultMax.Value);
//             }

//             writer.WriteEndObject();
//         }
//     }
// }