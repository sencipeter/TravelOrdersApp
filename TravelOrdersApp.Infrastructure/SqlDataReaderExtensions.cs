using System.Reflection;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Types;

namespace TravelOrdersApp.Infrastructure;

internal static class SqlDataReaderExtensions
{

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

    //public static T MapToObject<T>(this SqlDataReader reader) where T : new()
    //{
    //    var obj = new T();
    //    var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

    //    foreach (var prop in props)
    //    {
    //        if (!reader.HasColumn(prop.Name) || reader[prop.Name] is DBNull)
    //            continue;

    //        var value = reader[prop.Name];


    //        // Special handling for geography
    //        if (prop.PropertyType == typeof(SqlGeography))
    //        {
    //            prop.SetValue(obj, (SqlGeography)value);
    //        }
    //        else if (prop.PropertyType == typeof(string))
    //        {
    //            prop.SetValue(obj, Convert.ToString(value));
    //        }
    //        else
    //        {
    //            prop.SetValue(obj, Convert.ChangeType(value, prop.PropertyType));
    //        }
    //    }

    //    return obj;
    //}

    public static List<T> MapToList<T>(this SqlDataReader reader) where T : new()
    {
        var list = new List<T>();
        while (reader.Read())
        {
            list.Add(reader.MapToObject<T>());
        }
        return list;
    }

    private static bool HasColumn(this SqlDataReader reader, string columnName)
    {
        for (int i = 0; i < reader.FieldCount; i++)
        {
            if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }
}
