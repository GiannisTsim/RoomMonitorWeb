using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Dapper;

using RoomMonitor.Models;
using RoomMonitor.Services;

namespace RoomMonitor.JsonConverters
{
    internal class MonitorReadingConverter : JsonConverter<MonitorReading>
    {

        private readonly SensorTypeProvider _sensorTypeProvider;
        // private readonly string _connectionString;

        public MonitorReadingConverter(IConfiguration configuration, SensorTypeProvider sensorTypeProvider)
        {
            // _connectionString = configuration.GetConnectionString("DefaultConnection");
            _sensorTypeProvider = sensorTypeProvider;
        }

        public override bool CanConvert(Type typeToConvert) =>
            typeof(MonitorReading).IsAssignableFrom(typeToConvert);

        public override MonitorReading Read(
            ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            MonitorReading reading;
            string application;
            string sensorType;

            string propertyName;

            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            // Parse "application" property
            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }
            propertyName = reader.GetString();
            if (propertyName != "application")
            {
                throw new JsonException();
            }
            reader.Read();
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException();
            }
            application = reader.GetString();

            // Query SensorType of 'Application' and create the corresponding Reading{Type}
            // using (SqlConnection connection = new SqlConnection(_connectionString))
            // {
            //     connection.Open();
            //     sensorType = connection.ExecuteScalar<string>(
            //         "SELECT SensorType FROM Application WHERE Application = @Application",
            //         new { Application = application });
            sensorType = _sensorTypeProvider.GetApplicationSensorType(application);

            reading = sensorType switch
            {
                "Switch" => new MonitorReadingSwitch() { Application = application },
                "Measure" => new MonitorReadingMeasure() { Application = application },
                _ => throw new JsonException()
            };
            // }

            // Parse "readingDtm" property
            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }
            propertyName = reader.GetString();
            if (propertyName != "readingDtm")
            {
                throw new JsonException();
            }
            reader.Read();
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException();
            }
            if (!DateTime.TryParse(reader.GetString(), out DateTime readingDtm))
            {
                throw new JsonException();
            }
            reading.ReadingDtm = readingDtm;


            // Parse "value" property
            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }
            propertyName = reader.GetString();
            if (propertyName != "value")
            {
                throw new JsonException();
            }
            reader.Read();
            switch (sensorType)
            {
                case "Measure":
                    decimal valueDecimal = reader.GetDecimal();
                    ((MonitorReadingMeasure)reading).Value = valueDecimal;
                    break;
                case "Switch":
                    bool valueBool = reader.GetBoolean();
                    ((MonitorReadingSwitch)reading).Value = valueBool;
                    break;
            }

            reader.Read();
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return reading;
            }
            throw new JsonException();
        }

        public override void Write(
            Utf8JsonWriter writer, MonitorReading reading, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("application", reading.Application);
            writer.WriteString("readingDtm", reading.ReadingDtm);

            if (reading is MonitorReadingSwitch readingSwitch)
            {
                writer.WriteBoolean("value", readingSwitch.Value);
            }
            else if (reading is MonitorReadingMeasure readingMeasure)
            {
                writer.WriteNumber("value", readingMeasure.Value);
            }

            writer.WriteEndObject();
        }
    }
}