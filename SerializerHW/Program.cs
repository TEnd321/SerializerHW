using System;
using System.IO;

namespace SerializerHW
{
	public delegate void PrinterOpen(TextWriter tw);
	public delegate void PrinterClose(TextWriter tw);
	public class RootDescriptor<T>
	{
		public PrinterOpen PO { get; set; }
		public PrinterClose PC { get; set; }
		public void Serialize(TextWriter writer, T instance)
		{
			PO.Invoke(writer);
			PC.Invoke(writer);
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
			RootDescriptor<Person> rootDesc = GetPersonDescriptor();

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

			rootDesc.Serialize(Console.Out, person);
		}

		static RootDescriptor<Person> GetPersonDescriptor()
		{
			var rootDesc = new RootDescriptor<Person>();
			rootDesc.PO += PersonOpenPrint;
			rootDesc.PC += PersonClosePrint;
			return rootDesc;
		}

		static RootDescriptor<Country> GetCountryDescriptor()
		{
			var rootDest = new RootDescriptor<Country>();

			return rootDest;
		}

		static void PersonOpenPrint(TextWriter tw) => tw.WriteLine("<Person>");

		static void PersonClosePrint(TextWriter tw) => tw.WriteLine("</Person>");
		
	}
}
