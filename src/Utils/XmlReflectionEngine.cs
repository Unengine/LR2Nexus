using System.Reflection;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace LR2Nexus.Utils;

public static class XmlReflectionEngine
{
	public static void Serialize(object obj, string filePath)
	{
		XDocument doc = File.Exists(filePath) ? XDocument.Load(filePath) : new XDocument(new XElement("Root"));
		var root = doc.Root!;

		UpdateElement(root, obj);
		doc.Save(filePath);
	}
	private static void UpdateElement(XElement element, object obj)
	{
		foreach (var prop in obj.GetType().GetProperties())
		{
			var attr = prop.GetCustomAttribute<XmlElementAttribute>();
			var name = attr?.ElementName ?? prop.Name;
			var val = prop.GetValue(obj);
			if (val == null) continue;

			if (val is System.Collections.IEnumerable enumerable && val is not string)
			{
				element.Elements(name).Remove();

				foreach (var item in enumerable)
				{
					element.Add(new XElement(name, item.ToString()));
				}
			}
			else if (prop.PropertyType.IsClass && prop.PropertyType != typeof(string))
			{
				var subElement = element.Element(name) ?? new XElement(name);
				UpdateElement(subElement, val);
				if (subElement.Parent == null) element.Add(subElement);
			}
			else
			{
				var subElement = element.Element(name) ?? new XElement(name);
				subElement.Value = val.ToString()!;
				if (subElement.Parent == null) element.Add(subElement);
			}
		}
	}

	public static void Deserialize(XElement element, object obj)
	{
		foreach (var prop in obj.GetType().GetProperties())
		{
			if (!prop.CanWrite) continue;

			var attr = prop.GetCustomAttribute<XmlElementAttribute>();
			var name = attr?.ElementName ?? prop.Name;

			var subElements = element.Elements(name);
			if (!subElements.Any()) continue;

			if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
			{
				var list = (System.Collections.IList)prop.GetValue(obj)!;
				var itemType = prop.PropertyType.GetGenericArguments()[0];

				list.Clear();

				foreach (var sub in subElements)
				{
					var val = Convert.ChangeType(sub.Value, itemType);
					list.Add(val);
				}
			}
			else if (prop.PropertyType.IsClass && prop.PropertyType != typeof(string))
			{
				var subElement = element.Element(name);
				var currentVal = prop.GetValue(obj);

				if (currentVal == null)
				{
					currentVal = Activator.CreateInstance(prop.PropertyType);
					prop.SetValue(obj, currentVal);
				}
				Deserialize(subElement!, currentVal!);
			}
			else
			{
				var subElement = element.Element(name);
				var val = Convert.ChangeType(subElement!.Value, prop.PropertyType);
				prop.SetValue(obj, val);
			}
		}
	}
}