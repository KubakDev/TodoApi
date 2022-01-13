using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using MongoDB.Bson;

namespace TodoApi.Common
{
  public class BsonDocumentConverter : JsonConverter<BsonDocument?>
  {
    public override BsonDocument? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
      var doc = new BsonDocument();
      var startingDepth = reader.CurrentDepth;

      while (reader.Read())
      {
        if (reader.CurrentDepth == startingDepth)
          return doc;

        if (reader.TokenType == JsonTokenType.PropertyName)
        {
          var propertyName = reader.GetString();

          _ = reader.Read();
          switch (reader.TokenType)
          {
            case JsonTokenType.Null:
              doc[propertyName] = BsonNull.Value;
              break;
            case JsonTokenType.String:
              doc[propertyName] = reader.GetString();
              break;
            case JsonTokenType.Number:
              doc[propertyName] = reader.GetDouble();
              break;
            case JsonTokenType.True:
            case JsonTokenType.False:
              doc[propertyName] = reader.GetBoolean();
              break;
            case JsonTokenType.StartObject:
              doc[propertyName] = Read(ref reader, typeToConvert, options);
              break;
            case JsonTokenType.StartArray:
              doc[propertyName] = ReadArray(ref reader, typeToConvert, options);
              break;
          }
        }
      }

      return null;
    }

    private BsonArray ReadArray(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
      if (reader.TokenType != JsonTokenType.StartArray)
        throw new InvalidOperationException();

      var arr = new BsonArray();

      while (reader.Read())
      {
        switch (reader.TokenType)
        {
          case JsonTokenType.EndArray:
            return arr;
          case JsonTokenType.Null:
            _ = arr.Add(BsonNull.Value);
            break;
          case JsonTokenType.String:
            _ = arr.Add(reader.GetString());
            break;
          case JsonTokenType.Number:
            _ = arr.Add(reader.GetDouble());
            break;
          case JsonTokenType.True:
          case JsonTokenType.False:
            _ = arr.Add(reader.GetBoolean());
            break;
          case JsonTokenType.StartObject:
            _ = arr.Add(Read(ref reader, typeToConvert, options));
            break;
        }
      }

      return arr;
    }

    public override void Write(Utf8JsonWriter writer, BsonDocument? value, JsonSerializerOptions options)
    {
      if (value != null)
      {
        writer.WriteStartObject();
        WriteElements(writer, value, options);
        writer.WriteEndObject();
      }
    }

    private void WriteElements(Utf8JsonWriter writer, IEnumerable<BsonElement> elements, JsonSerializerOptions options)
    {
      foreach (var element in elements)
      {
        if (options.DefaultIgnoreCondition is JsonIgnoreCondition.WhenWritingNull or JsonIgnoreCondition.Always && element.Value.IsBsonNull)
          continue;

        writer.WritePropertyName(element.Name);
        WriteValue(writer, element.Value, options);
      }
    }

    private void WriteArray(Utf8JsonWriter writer, BsonArray array, JsonSerializerOptions options)
    {
      writer.WriteStartArray();

      foreach (var value in array)
        WriteValue(writer, value, options);

      writer.WriteEndArray();
    }

    private void WriteValue(Utf8JsonWriter writer, BsonValue value, JsonSerializerOptions options)
    {
      if (value.IsBsonNull)
        writer.WriteNullValue();
      if (value.IsBoolean)
        writer.WriteBooleanValue(value.AsBoolean);
      else if (value.IsObjectId)
        writer.WriteStringValue(value.AsObjectId.ToString());
      else if (value.IsString)
        writer.WriteStringValue(value.AsString);
      else if (value.IsNumeric)
      {
        if (value.IsInt32)
          writer.WriteNumberValue(value.AsInt32);
        else if (value.IsInt64)
          writer.WriteNumberValue(value.AsInt64);
        else if (value.IsDouble)
          writer.WriteNumberValue(value.AsDouble);
        else if (value.IsDecimal128)
          writer.WriteNumberValue(value.ToDecimal());
      }
      else if (value.IsValidDateTime)
        writer.WriteStringValue(value.ToUniversalTime());
      else if (value.IsBsonDocument)
        Write(writer, value.AsBsonDocument, options);
      else if (value.IsBsonArray)
        WriteArray(writer, value.AsBsonArray, options);
    }
  }
}