using SnowDAL.Sorting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SnowBLL.Models
{
    public class FiltrationHelper
    {
        public static Dictionary<string, object> ConvertToDictionary<TFilter>(TFilter filter)
        {
            Dictionary<string, object> filtersDictionary = new Dictionary<string, object>();

            Type type = filter.GetType();
            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (IsNotDefaultValue(filter, property))
                {
                    filtersDictionary.Add(property.Name, property.GetValue(filter, null));
                }
            }

            return filtersDictionary;
        }

        /// <summary>
        /// Gets filter model created by filter string in format e.g. name::asd|difficulty:2
        /// </summary>
        /// <typeparam name="T">Filter model</typeparam>
        /// <param name="filter">Filter string</param>
        /// <returns>Filled filter model</returns>
        public static T GetFilter<T>(string filter) where T : new()
        {
            var dict = filter.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
              .Select(part => part.Split(new string[] { "::" }, StringSplitOptions.None))
             .ToDictionary(split => split[0], split => split[1]);

            var t = new T();
            PropertyInfo[] properties = t.GetType().GetProperties();

            foreach (var d in dict)
            {
                if (!HasProperty(typeof(T), d.Key))
                { 
                    throw new Exception($"Filtering field {d.Key} is incorrect.");
                }
            }

            foreach (PropertyInfo property in properties)
            {
                if (!dict.Any(x => x.Key.Equals(property.Name, StringComparison.CurrentCultureIgnoreCase)))
                    continue;

                KeyValuePair<string, string> item = dict.First(x => x.Key.Equals(property.Name, StringComparison.CurrentCultureIgnoreCase));

                // Find which property type (int, string, double? etc) the CURRENT property is...
                Type tPropertyType = t.GetType().GetProperty(property.Name).PropertyType;

                // Fix nullables...
                Type newT = Nullable.GetUnderlyingType(tPropertyType) ?? tPropertyType;

                // ...and change the type
                object newA = Convert.ChangeType(item.Value, newT);
                t.GetType().GetProperty(property.Name).SetValue(t, newA, null);
            }
            return t;
        }

        public static FieldSortOrder<T> GetSorting<T>(string sort) where T : class, new()
        {
            bool containsOrderingPrefix = sort[0] == '-';
            string sortingField = containsOrderingPrefix ? sort.Substring(1) : sort;

            if (HasProperty(typeof(T), sortingField))
            {
                SortDirection direction = containsOrderingPrefix ? SortDirection.Descending : SortDirection.Ascending;

                return new FieldSortOrder<T>(sortingField, direction);
            }
            else
            {
                throw new Exception($"Sorting field {sort} is incorrect.");
            }
        }

        private static bool HasProperty(Type type, string propertyName)
        {
            return type.GetProperty(propertyName.ToLower(), BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance) != null;
        }

        private static bool IsNotDefaultValue<TFilter>(TFilter filter, PropertyInfo property)
        {
            return (property.PropertyType == typeof(string) && (string)property.GetValue(filter) != "" && (string)property.GetValue(filter) != null) ||
                property.PropertyType == typeof(short?) && property.GetValue(filter) != null ||
                property.PropertyType == typeof(int?) && property.GetValue(filter) != null ||
                property.PropertyType == typeof(double?) && property.GetValue(filter) != null ||
                property.PropertyType == typeof(decimal?) && property.GetValue(filter) != null;
        }
    }
}
