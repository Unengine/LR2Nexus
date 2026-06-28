using Microsoft.Data.Sqlite;

namespace LR2Nexus.Utils
{
	public static class DatabaseUtils
	{
		public static T? GetValue<T>(SqliteDataReader reader, string columnName)
		{
			int ordinal = reader.GetOrdinal(columnName);

			if (reader.IsDBNull(ordinal))
			{
				return default;
			}

			object value = reader.GetValue(ordinal);

			if (value is long longValue && typeof(T) == typeof(int))
			{
				return (T)(object)(int)longValue;
			}

			return (T)value;
		}
	}
}
