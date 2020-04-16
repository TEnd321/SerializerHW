using System;
using System.IO;

namespace SerializerHW
{
	public delegate void PrinterOpen(TextWriter tw);
	public delegate void PrinterClose(TextWriter tw);
	public delegate void FieldPrint<T>(TextWriter tw, T instance);
	public delegate void SerializeOther<T>(TextWriter tw, T instance);
	public class RootDescriptor<T>
	{
		public PrinterOpen PO { get; set; }
		public PrinterClose PC { get; set; }
		public FieldPrint<T> FP { get; set; }
		public SerializeOther<T> SO { get; set; }
		public void Serialize(TextWriter writer, T instance)
		{
			PO?.Invoke(writer);
			FP?.Invoke(writer, instance);
			PC?.Invoke(writer);
		}
	}

	class Address
	{
		public string Street { get; set; }
		public string City { get; set; }
	}

	class Country
	{
		public string Name { get; set; }
		public int AreaCode { get; set; }
	}

	class PhoneNumber
	{
		public Country Country { get; set; }
		public int Number { get; set; }
	}

	class Person
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public Address HomeAddress { get; set; }
		public Address WorkAddress { get; set; }
		public Country CitizenOf { get; set; }
		public PhoneNumber MobilePhone { get; set; }
	}

	class Program
	{
		static void Main(string[] args)
		{
			RootDescriptor<Country> rootDesc = GetCountryDescriptor();

			var czechRepublic = new Country { Name = "Czech Republic", AreaCode = 420 };
			var person = new Person
			{
				FirstName = "Pavel",
				LastName = "Jezek",
				HomeAddress = new Address { Street = "Patkova", City = "Prague" },
				WorkAddress = new Address { Street = "Malostranske namesti", City = "Prague" },
				CitizenOf = czechRepublic,
				MobilePhone = new PhoneNumber { Country = czechRepublic, Number = 123456789 }
			};

			rootDesc.Serialize(Console.Out, czechRepublic);
		}

		static RootDescriptor<Person> GetPersonDescriptor()
		{
			var rootDesc = new RootDescriptor<Person>();
			rootDesc.PO += OpenPrint<Person>;
			rootDesc.PC += ClosePrint<Person>;
			return rootDesc;
		}

		static RootDescriptor<Country> GetCountryDescriptor()
		{
			var rootDesc = new RootDescriptor<Country>();
			rootDesc.PO += OpenPrint<Country>;
			rootDesc.FP += CountryNamePrinter;
			rootDesc.FP += CountryAreaPrinter;
			rootDesc.PC += ClosePrint<Country>;
			return rootDesc;
		}

		public static void CountryNamePrinter(TextWriter tw, Country instance)
		{
			tw.WriteLine("<Name>" + GetCountryName(instance) + "</Name>");
		}

		public static void CountryAreaPrinter(TextWriter tw, Country instance)
		{
			tw.WriteLine("<AreaCode>" + GetCountryAreaCode(instance) + "</AreaCode>");
		}

		public static string GetCountryName(Country country) => country.Name;
		public static int GetCountryAreaCode(Country country) => country.AreaCode;

		public static void OpenPrint<T>(TextWriter tw) => tw.WriteLine("<" + typeof(T).Name + ">");

		public static void ClosePrint<T>(TextWriter tw) => tw.WriteLine("</" + typeof(T).Name + ">");

	}
}
