using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace CV.Web
{
    public class CustomDateTimeConverter : IsoDateTimeConverter
    {
        public CustomDateTimeConverter()
        {
            base.DateTimeFormat = "yyyy.MM.dd";
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if( value == null)
                base.WriteJson(writer, null, serializer);
            else
            {
                base.WriteJson(writer, ((DateTime)value).ToString("yyyy MMMM"), serializer);
            }
        }
    }
}