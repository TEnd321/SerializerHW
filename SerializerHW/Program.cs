using System;
using System.Collections;
using System.IO;
using System.Net;

namespace SerializerHW
{
	public delegate void PrinterOpen(TextWriter tw, string Name);
	public delegate void PrinterClose(TextWriter tw, string Name);
	public delegate void FieldPrint<T>(TextWriter tw, T instance);
	public delegate void SerializeOther<T>(TextWriter tw, T instance);
	public class RootDescriptor<T>
	{
		public string Name { get; set; }
		public PrinterOpen PO { get; set; }
		public PrinterClose PC { get; set; }
		public FieldPrint<T> FP { get; set; }
		public SerializeOther<T> SO { get; set; }

		public void Serialize(TextWriter writer, T instance)
		{
			PO?.Invoke(writer, Name);
			FP?.Invoke(writer, instance);
			SO?.Invoke(writer, instance);
			PC?.Invoke(writer, Name);
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
			//RootDescriptor<Country> rootDesc = GetCountryDescriptor();
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
			rootDesc.Name = "Person";
			rootDesc.PO += OpenPrint;
			rootDesc.FP += PersonFirstNamePrinter;
			rootDesc.FP += PersonLastNamePrinter;
			rootDesc.SO += SerializePersonHomeAddress;
			rootDesc.SO += SerializePersonWorkAddress;
			rootDesc.SO += SerializePersonCountry;
			rootDesc.SO += SerializePhoneNumber;
			rootDesc.PC += ClosePrint;
			return rootDesc;
		}

		static RootDescriptor<Country> GetCountryDescriptor(string name)
		{
			var rootDesc = new RootDescriptor<Country>();
			rootDesc.Name = name;
			rootDesc.PO += OpenPrint;
			rootDesc.FP += CountryNamePrinter;
			rootDesc.FP += CountryAreaPrinter;
			rootDesc.PC += ClosePrint;
			return rootDesc;
		}

		static RootDescriptor<Address> GetAddressDescriptor(string name)
		{
			var rootDesc = new RootDescriptor<Address>();
			rootDesc.Name = name;
			rootDesc.PO += OpenPrint;
			rootDesc.FP += AddressStreetPrinter;
			rootDesc.FP += AddressCityPrinter;
			rootDesc.PC += ClosePrint;
			return rootDesc;
		}

		static RootDescriptor<PhoneNumber> GetPhoneNumberDescriptor(string name)
		{
			var rootDesc = new RootDescriptor<PhoneNumber>();
			rootDesc.Name = name;
			rootDesc.PO += OpenPrint;
			rootDesc.SO += SerializePhoneCountry;
			rootDesc.FP += PhoneNumberNumberPrinter;
			rootDesc.PC += ClosePrint;
			return rootDesc;
		}

		public static void SerializePersonCountry(TextWriter tw, Person person)
		{
			var rootDesc = GetCountryDescriptor("CitizenOf");
			rootDesc.Serialize(tw, person.CitizenOf);
		}

		public static void SerializePersonHomeAddress(TextWriter tw, Person person)
		{
			var rootDesc = GetAddressDescriptor("HomeAddress");
			rootDesc.Serialize(tw, person.HomeAddress);
		}

		public static void SerializePersonWorkAddress(TextWriter tw, Person person)
		{
			var rootDesc = GetAddressDescriptor("WorkAddress");
			rootDesc.Serialize(tw, person.WorkAddress);
		}

		public static void SerializePhoneCountry(TextWriter tw, PhoneNumber phone)
		{
			var rootDesc = GetCountryDescriptor("Country");
			rootDesc.Serialize(tw, phone.Country);
		}

		public static void SerializePhoneNumber(TextWriter tw, Person person)
		{
			var rootDesc = GetPhoneNumberDescriptor("MobilePhone");
			rootDesc.Serialize(tw, person.MobilePhone);
		}

		public static void CountryNamePrinter(TextWriter tw, Country instance) =>
			tw.WriteLine("<Name>" + instance.Name + "</Name>");
		
		public static void CountryAreaPrinter(TextWriter tw, Country instance) =>
			tw.WriteLine("<AreaCode>" + instance.AreaCode.ToString() + "</AreaCode>");

		public static void PersonFirstNamePrinter(TextWriter tw, Person person) => 
			tw.WriteLine("<FirstName>" + person.FirstName + "</FirstName>");

		public static void PersonLastNamePrinter(TextWriter tw, Person person) =>
			tw.WriteLine("<LastName>" + person.LastName + "</LastName>");

		public static void AddressStreetPrinter(TextWriter tw, Address address) =>
			tw.WriteLine("<Street>" + address.Street + "</Street>");

		public static void AddressCityPrinter(TextWriter tw, Address address) =>
			tw.WriteLine("<City>" + address.City + "</City>");

		public static void PhoneNumberNumberPrinter(TextWriter tw, PhoneNumber phoneNumber) =>
			tw.WriteLine("<Number>" + phoneNumber.Number + "</Number>");

		public static void OpenPrint(TextWriter tw, string name) => tw.WriteLine("<" + name + ">");

		public static void ClosePrint(TextWriter tw, string name) => tw.WriteLine("</" + name + ">");

	}
}
