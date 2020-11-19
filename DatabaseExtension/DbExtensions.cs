using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DatabaseExtension {
    public static class DbExtensions {

        public static T Create<T>(this SqlResultRow row) {
            var ctor = GetConstructor<T>();
            var obj = (T)ctor.Invoke(null);
            obj.SetValues(row);
            return obj;
        }
        private static Dictionary<Type, ConstructorInfo> _constructors = new Dictionary<Type, ConstructorInfo>();
        private static Dictionary<Type, Dictionary<FieldInfo, DbColumnAttribute>> _fields = new Dictionary<Type, Dictionary<FieldInfo, DbColumnAttribute>>();
        private static Dictionary<Type, Dictionary<PropertyInfo, DbColumnAttribute>> _properties = new Dictionary<Type, Dictionary<PropertyInfo, DbColumnAttribute>>();
        private static ConstructorInfo GetConstructor<T>() {
            var t = typeof(T);
            if (!_constructors.ContainsKey(t)) {
                _constructors.Add(t, t.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new Type[] { }, null));
            }
            return _constructors[t];
        }


        public static void SetValues<T>(this T src, SqlResultRow row) {
            foreach (var p in GetPropertyDictionary<T>()) {
                p.Key.SetValue(src, p.GetValue(row), null);
            }
            foreach (var f in GetFieldDictionary<T>()) {
                f.Key.SetValue(src, f.GetValue(row));
            }
        }

        private static IDictionary<PropertyInfo, DbColumnAttribute> GetPropertyDictionary<T>() {
            if (!_properties.ContainsKey(typeof(T))) {
                var members = new Dictionary<PropertyInfo, DbColumnAttribute>();
                foreach (var p in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
                    var attrs = p.GetCustomAttributes(typeof(DbColumnAttribute), false);
                    if (attrs != null && attrs.Length > 0) {
                        members.Add(p, attrs[0] as DbColumnAttribute);
                    }
                }
                _properties.Add(typeof(T), members);
            }
            return _properties[typeof(T)];
        }
        private static IDictionary<FieldInfo, DbColumnAttribute> GetFieldDictionary<T>() {
            if (!_fields.ContainsKey(typeof(T))) {
                var fields = new Dictionary<FieldInfo, DbColumnAttribute>();
                foreach (var f in typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
                    var attrs = f.GetCustomAttributes(typeof(DbColumnAttribute), false);
                    if (attrs != null && attrs.Length > 0) {
                        fields.Add(f, attrs[0] as DbColumnAttribute);
                    }
                }
                _fields.Add(typeof(T), fields);
            }
            return _fields[typeof(T)];
        }

        private static object GetValue(this KeyValuePair<FieldInfo, DbColumnAttribute> f, SqlResultRow row) {
            return row[f.Value.ColumnName].GetValue(f.Key.FieldType, f.Value?.DateFormat);
        }
        private static object GetValue(this KeyValuePair<PropertyInfo, DbColumnAttribute> p, SqlResultRow row) {
            return row[p.Value.ColumnName].GetValue(p.Key.PropertyType, p.Value?.DateFormat);
        }
        private static object GetValue(this object value, Type t, string format) {
            if (t == typeof(DateTime)) {
                if (!string.IsNullOrEmpty(format)) {
                    return value.ToDateTime(format).Value;
                } else {
                    return value.ToDateTime().Value;
                }
            } else if (t == typeof(DateTime?)) {
                if (!string.IsNullOrEmpty(format)) {
                    return value.ToDateTime(format);
                } else {
                    return value.ToDateTime();
                }
            } else if (t == typeof(int)) {
                return value.ToIntN().Value;
            } else if (t == typeof(int?)) {
                return value.ToIntN();

            } else if (t == typeof(decimal)) {
                return value.ToDecimalN().Value;
            } else if (t == typeof(decimal?)) {
                return value.ToDecimalN();
            } else if (t == typeof(float)) {
                return value.ToFloatN().Value;
            } else if (t == typeof(float?)) {
                return value.ToFloatN();
            } else if (t == typeof(double)) {
                return value.ToDoubleN().Value;
            } else if (t == typeof(double?)) {
                return value.ToDoubleN();
            } else if (t == typeof(string)) {
                return value.ToString();
            } else if (t == typeof(bool)) {
                return "Y".Equals(value?.ToString(), StringComparison.OrdinalIgnoreCase);
            } else if (t == typeof(bool?)) {
                if (value == null) {
                    return null;
                } else {
                    return "Y".Equals(value.ToString(), StringComparison.OrdinalIgnoreCase);
                }
            } else {
                return Convert.ChangeType(value, t);
            }
        }
        public static int? ToIntN(this object value) {
            if (value == null || value == DBNull.Value) {
                return null;
            }
            int ret;
            if (int.TryParse(value.ToString(), out ret)) {
                return ret;
            } else {
                return null;
            }
        }
        internal static int ToInt32(this object value, int defaultValue) {
            return value.ToIntN() ?? defaultValue;
        }
        internal static DateTime? ToDateTime(this object value) {
            if (value == null || value == DBNull.Value) {
                return null;
            }
            if (value is DateTime) {
                return (DateTime)value;
            }
            if (value is DateTime?) {
                return (DateTime?)value;
            }
            return null;
        }
        internal static DateTime? ToDateTime(this object value, string dateFormat) {
            if (value == null || value == DBNull.Value) {
                return null;
            }
            DateTime d;
            if (DateTime.TryParseExact(value.ToString(), dateFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out d)) {
                return d;
            } else {
                return null;
            }
        }
        internal static decimal? ToDecimalN(this object value) {
            if (value == null || value == DBNull.Value) {
                return null;
            }
            decimal ret;
            if (decimal.TryParse(value.ToString(), out ret)) {
                return ret;
            } else {
                return null;
            }
        }
        internal static decimal ToDecimal(this object value, decimal defaultValue) {
            return value.ToDecimalN() ?? defaultValue;
        }
        internal static float? ToFloatN(this object value) {
            if (value == null || value == DBNull.Value) {
                return null;
            }
            float ret;
            if (float.TryParse(value.ToString(), out ret)) {
                return ret;
            } else {
                return null;
            }
        }
        internal static float ToFloat(this object value, float defaultValue) {
            return value.ToFloatN() ?? defaultValue;
        }
        internal static double? ToDoubleN(this object value) {
            if (value == null || value == DBNull.Value) {
                return null;
            }
            double ret;
            if (double.TryParse(value.ToString(), out ret)) {
                return ret;
            } else {
                return null;
            }
        }
        internal static double ToDouble(this object value, double defaultValue) {
            return value.ToDoubleN() ?? defaultValue;
        }
    }


}