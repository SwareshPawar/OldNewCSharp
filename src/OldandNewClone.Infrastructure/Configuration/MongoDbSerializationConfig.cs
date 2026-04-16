using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using Microsoft.AspNetCore.Identity;

namespace OldandNewClone.Infrastructure.Configuration;

public static class MongoDbSerializationConfig
{
    private static bool _isConfigured = false;

    public static void Configure()
    {
        if (_isConfigured)
            return;

        // Register a convention pack to handle ObjectId to String conversion
        var conventionPack = new ConventionPack
        {
            new IgnoreExtraElementsConvention(true),
            new IgnoreIfNullConvention(true)
        };

        ConventionRegistry.Register("ApplicationConventions", conventionPack, t => true);

        // Register custom serializer for string IDs that are stored as ObjectIds
        BsonSerializer.RegisterSerializer(new ObjectIdAsStringSerializer());

        _isConfigured = true;
    }

    private class ObjectIdAsStringSerializer : SerializerBase<string>
    {
        public override string Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var bsonType = context.Reader.GetCurrentBsonType();

            switch (bsonType)
            {
                case BsonType.ObjectId:
                    return context.Reader.ReadObjectId().ToString();
                case BsonType.String:
                    return context.Reader.ReadString();
                default:
                    throw new NotSupportedException($"Cannot convert BsonType {bsonType} to String");
            }
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                context.Writer.WriteNull();
            }
            else if (ObjectId.TryParse(value, out var objectId))
            {
                context.Writer.WriteObjectId(objectId);
            }
            else
            {
                context.Writer.WriteString(value);
            }
        }
    }
}
