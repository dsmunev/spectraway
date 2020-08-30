﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SpectraWay.Helpers
{
    /// <summary>
    /// A JsonConverter that modifies formatting of arrays, such that the array elements are serialised to a single line instead of one element per line
    /// preceded by indentation whitespace.
    /// This converter handles writing JSON only; CanRead returns false.
    /// </summary>
    /// <remarks>
    /// This converter/formatter applies to arrays only and not other collection types. Ideally we would use the existing logic within Newtonsoft.Json for
    /// identifying collections of items, as this handles a number of special cases (e.g. string implements IEnumerable over the string characters). In order
    /// to avoid duplicating in lots of logic, instead this converter handles only Arrays of a handful of selected primitive types.
    /// </remarks>
    public class ArrayNoFormattingConverter : JsonConverter
    {
        # region Static Fields    

        static HashSet<Type> _primitiveTypeSet =
            new HashSet<Type>
            {
                typeof(char),
                typeof(char?),
                typeof(bool),
                typeof(bool?),
                typeof(sbyte),
                typeof(sbyte?),
                typeof(short),
                typeof(short?),
                typeof(ushort),
                typeof(ushort?),
                typeof(int),
                typeof(int?),
                typeof(byte),
                typeof(byte?),
                typeof(uint),
                typeof(uint?),
                typeof(long),
                typeof(long?),
                typeof(ulong),
                typeof(ulong?),
                typeof(float),
                typeof(float?),
                typeof(double),
                typeof(double?),
                typeof(decimal),
                typeof(decimal?),
                typeof(string),
                typeof(DateTime),
                typeof(DateTime?),
            };

        #endregion

        #region Properties

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        ///     <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            // Note. Ideally this would match the test for JsonContractType.Array in DefaultContractResolver.CreateContract(),
            // but that code is all internal to Newtonsoft.Json.
            // Here we elect to take over conversion for Arrays only.
            if (objectType.IsArray || (objectType.GetInterfaces().Contains(typeof(IEnumerable)) && objectType.IsGenericType))
            {
                return _primitiveTypeSet.Contains(objectType.IsArray ? objectType.GetElementType() : objectType.GetGenericArguments()[0]);
            }
            return false;
            // Fast/efficient way of testing for multiple possible primitive types.
            
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="JsonConverter"/> can read JSON.
        /// </summary>
        /// <value>Always returns <c>false</c>.</value>
        public override bool CanRead
        {
            get { return false; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Reads the JSON representation of the object. (Not implemented on this converter).
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Formatting formatting = writer.Formatting;
            writer.WriteStartArray();
            try
            {
                writer.Formatting = Formatting.None;
                foreach (object childValue in ((System.Collections.IEnumerable)value))
                {
                    serializer.Serialize(writer, childValue);
                }
            }
            finally
            {
                writer.WriteEndArray();
                writer.Formatting = formatting;
            }
        }

        #endregion
    }
}