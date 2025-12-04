using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Types;
using System.Data;
using System.Reflection;

namespace TravelOrdersApp.Infrastructure;

internal static class SqlDataExtensions
{

    public static void AddParametersFromObject(this SqlCommand command, 
        object parameters,
        IEnumerable<string>? ignoreList = null)
    {
        if (parameters == null) return;

        var ignoreSet = ignoreList != null
            ? new HashSet<string>(ignoreList, StringComparer.OrdinalIgnoreCase)
            : new HashSet<string>();

        var props = parameters.GetType().GetProperties();
        foreach (var prop in props)
        {
            // Skip ignored properties
            if (ignoreSet.Contains(prop.Name))
                continue;

            var type = prop.PropertyType;
            // Skip navigation/nested objects (non‑primitive, non‑string, non‑SqlGeography)
            var underlyingType = Nullable.GetUnderlyingType(type) ?? type;

            if (!(underlyingType.IsPrimitive
                  || underlyingType == typeof(string)
                  || underlyingType == typeof(DateTime)
                  || underlyingType == typeof(decimal)
                  || underlyingType == typeof(Guid)
                  || underlyingType == typeof(SqlGeography)))
            {
                continue; // skip navigation/nested objects
            }

            var value = prop.GetValue(parameters) ?? DBNull.Value;

            if (value is SqlGeography geo)
            {
                // Explicitly declare UDT type
                var p = new SqlParameter("@" + prop.Name, SqlDbType.Udt)
                {
                    UdtTypeName = "Geography",
                    Value = geo
                };
                command.Parameters.Add(p);
            }
            else
            {
                command.Parameters.AddWithValue("@" + prop.Name, value);
            }
        }
    }

    public static T MapToObject<T>(this SqlDataReader reader) where T : new()
    {
        T obj = new T();

        for (int i = 0; i < reader.FieldCount; i++)
        {
            string columnName = reader.GetName(i); // e.g. "Employee.FirstName"
            object value = reader.IsDBNull(i) ? null : reader.GetValue(i);

            if (value == null) continue;

            // Handle nested properties like Employee.FirstName
            if (columnName.Contains("."))
            {
                string[] parts = columnName.Split('.');
                string navPropName = parts[0];   // e.g. "Employee"
                string propName = parts[1];      // e.g. "FirstName"

                PropertyInfo navProp = typeof(T).GetProperty(navPropName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                if (navProp != null)
                {
                    object navObj = navProp.GetValue(obj);
                    if (navObj == null)
                    {
                        navObj = Activator.CreateInstance(navProp.PropertyType);
                        navProp.SetValue(obj, navObj);
                    }

                    PropertyInfo childProp = navProp.PropertyType.GetProperty(propName,
                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                    if (childProp != null && childProp.CanWrite)
                    {
                        childProp.SetValue(navObj, Convert.ChangeType(value, childProp.PropertyType));
                    }
                }
            }
            else
            {
                PropertyInfo prop = typeof(T).GetProperty(columnName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                if (prop != null && prop.CanWrite)
                {
                    prop.SetValue(obj, Convert.ChangeType(value, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType));
                }
            }
        }

        return obj;
    }

    public static List<T> MapToList<T>(this SqlDataReader reader) where T : new()
    {
        var list = new List<T>();
        while (reader.Read())
        {
            list.Add(reader.MapToObject<T>());
        }
        return list;
    }
}
