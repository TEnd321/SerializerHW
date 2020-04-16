using System;
using System.Collections;
using System.IO;
using System.Net;

namespace SerializerHW
{
    public delegate void PrinterOpen(TextWriter tw, string Name);
    public delegate void PrinterClose(TextWriter tw, string Name);
    public delegate void PrintOrder<T>(TextWriter tw, T instance);
    public class RootDescriptor<T>
    {
        public string Name { get; set; }
        public PrinterOpen PO { get; set; }
        public PrinterClose PC { get; set; }
        public PrintOrder<T> POrd { get; set; }
        public void Serialize(TextWriter writer, T instance)
        {
            PO?.Invoke(writer, Name);
            POrd?.Invoke(writer, instance);
            PC?.Invoke(writer, Name);
        }
    }

    struct Field<T>
    {
        public T value;
        public string name;
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
            rootDesc.POrd += PersonPrintOrder;
            rootDesc.PC += ClosePrint;
            return rootDesc;
        }

        static RootDescriptor<Country> GetCountryDescriptor(string name)
        {
            var rootDesc = new RootDescriptor<Country>();
            rootDesc.Name = name;
            rootDesc.PO += OpenPrint;
            rootDesc.POrd += CountryPrintOrder;
            rootDesc.PC += ClosePrint;
            return rootDesc;
        }

        static RootDescriptor<Address> GetAddressDescriptor(string name)
        {
            var rootDesc = new RootDescriptor<Address>();
            rootDesc.Name = name;
            rootDesc.PO += OpenPrint;
            rootDesc.POrd += AddressPrintOrder;
            rootDesc.PC += ClosePrint;
            return rootDesc;
        }

        static RootDescriptor<PhoneNumber> GetPhoneNumberDescriptor(string name)
        {
            var rootDesc = new RootDescriptor<PhoneNumber>();
            rootDesc.Name = name;
            rootDesc.PO += OpenPrint;
            rootDesc.POrd += PhoneNumberPrintOrder;
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

        public static void FieldPrinter<T>(TextWriter tw, Field<T> field)
        {
            tw.Write("<");
            tw.Write(field.name);
            tw.Write(">");
            tw.Write(field.value.ToString());
            tw.Write("</");
            tw.Write(field.name);
            tw.WriteLine(">");
        }

        public static void PersonPrintOrder(TextWriter tw, Person person)
        {
            FieldPrinter<string>(tw, new Field<string> { name = "FirstName", value = person.FirstName });
            FieldPrinter<string>(tw, new Field<string> { name = "LastName", value = person.LastName });
            SerializePersonHomeAddress(tw, person);
            SerializePersonWorkAddress(tw, person);
            SerializePersonCountry(tw, person);
            SerializePhoneNumber(tw, person);
        }

        public static void PhoneNumberPrintOrder(TextWriter tw, PhoneNumber phone)
        {
            SerializePhoneCountry(tw, phone);
            FieldPrinter<int>(tw, new Field<int> { name = "Number", value = phone.Number });
        }

        public static void CountryPrintOrder(TextWriter tw, Country country)
        {
            FieldPrinter<string>(tw, new Field<string> { name = "Name", value = country.Name });
            FieldPrinter<int>(tw, new Field<int> { name = "AreaCode", value = country.AreaCode });
        }

        public static void AddressPrintOrder(TextWriter tw, Address address)
        {
            FieldPrinter<string>(tw, new Field<string> { name = "Street", value = address.Street });
            FieldPrinter<string>(tw, new Field<string> { name = "City", value = address.City });
        }

        public static void OpenPrint(TextWriter tw, string name) => tw.WriteLine("<" + name + ">");

        public static void ClosePrint(TextWriter tw, string name) => tw.WriteLine("</" + name + ">");

    }
}
