using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Specialized;

namespace Dataway.WebImaging.Extensions
{
    public static class FormCollectionExtensions
    {
        #region Public Extension Methods

        public static T ValueOrDefault<T>(this HttpRequestBase request, string key)
        {
            return FormCollectionExtensions.GetValueOrDefault<T>(request[key]);
        }

        public static T Value<T>(this HttpRequestBase request, string key)
        {
            return (T)Convert.ChangeType(request[key], typeof(T));
        }

        public static T ValueOrDefault<T>(this NameValueCollection collection, string key)
        {
            return FormCollectionExtensions.GetValueOrDefault<T>(collection[key]);
        }

        public static T Value<T>(this NameValueCollection collection, string key)
        {
            return (T)Convert.ChangeType(collection[key], typeof(T));
        }

        #endregion

        #region Private Static Methods

        private static T GetValueOrDefault<T>(object value)
        {
            Type t = typeof(T);
            t = Nullable.GetUnderlyingType(t) ?? t;

            return (value == null || DBNull.Value.Equals(value) || (!(typeof(T).FullName == "System.String") && (value.ToString() == string.Empty))) 
                ? default(T) :
                (T)Convert.ChangeType(value, t);
        }

        #endregion
    }
}