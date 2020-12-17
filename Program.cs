﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CSProject
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Staff> myStaff = new List<Staff>();
            FileReader fr = new FileReader();
            int month = 0, year = 0;

            while (year == 0)
            {
                Console.Write("\nPlease enter the year:");

                try
                {
                    year = Convert.ToInt32(Console.ReadLine());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + "Please try again.");
                }
            }

            while (month == 0)
            {
                Console.Write("\nPlease enter the month:");

                try
                {
                    month = Convert.ToInt32(Console.ReadLine());

                    if (month < 1 || month > 12)
                    {
                        Console.WriteLine("Month must be from 1 to 12. Please try agian.");
                        month = 0;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + "Please try again.");
                }
            }

            myStaff = fr.ReadFile();

            for (int i = 0; i < myStaff.Count; i++)
            {
                try
                {
                    Console.Write("\nEnter hours worked for {0}: ", myStaff[i].NameOfStaff);
                    myStaff[i].HoursWorked = Convert.ToInt32(Console.ReadLine());
                    myStaff[i].CalculatePay();

                    Console.WriteLine(myStaff[i].ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    i--;
                }
            }

            PaySlip ps = new PaySlip(month, year);
            ps.GeneratePaySlip(myStaff);
            ps.GenerateSummary(myStaff);

            Console.Read();
        }
    }
class Staff
{
    private float hourlyRate;
    private int hWorked;

    public float TotalPay { get; protected set; }
    public float BasicPay { get; private set; }
    public string NameOfStaff { get; private set; }

        public int HoursWorked
        {
            get
            {
                return hWorked;
            }
            set
            {
                if (value > 0)
                    hWorked = value;
                else
                    hWorked = 0;
            }
        }

public Staff(string name, float rate)
{
    NameOfStaff = name;
    hourlyRate = rate;
}
public virtual void CalculatePay()
{
    Console.WriteLine("Calculating Pay...");

    BasicPay = hWorked * hourlyRate;
    TotalPay = BasicPay;
}

public override string ToString()
{
    return "\nNameOfStaff = " + NameOfStaff
        + "\nhourlyRate = " + hourlyRate +
"\nhWorked = " + hWorked
    + "\nBasicPay = " + BasicPay +
"\n\nTotalPay = " + TotalPay;
}

}

class Manager : Staff
{
    private const float managerHourlyRate = 50;

    public int Allowance { get; private set; }

    public Manager(string name) : base(name, managerHourlyRate) { }

    public override void CalculatePay()
    {
        base.CalculatePay();

        Allowance = 1000;

        if (HoursWorked > 160)
            TotalPay = BasicPay + Allowance;
    }

    public override string ToString()
    {
        return "\nNameOfStaff = " + NameOfStaff +
            "\nmanagerHourlyRate = " + managerHourlyRate + "\nHoursWorked = "
            + HoursWorked + "\nBasicPay = "
            + BasicPay + "\nAllowance = " + Allowance
            + "\n\nTotalPay = " + TotalPay;
    }
}

class Admin : Staff
{
    private const float overtimeRate = 12.5f;
    private const float adminHourlyRate = 30f;

    public float Overtime { get; private set; }

    public Admin(string name)
 : base(name, adminHourlyRate) { }

    public override void CalculatePay()
    {
        base.CalculatePay();

            if (HoursWorked > 160)
                Overtime = overtimeRate * (HoursWorked - 160);
    }

    public override string ToString()
    {
        return "\nNameOfStaff = " + NameOfStaff
            + "\nadminHourlyRate = " + adminHourlyRate +
            "\nHoursWorked = " + HoursWorked
            + "\nBasicPay = " + BasicPay + "\nOvertime ="
            + Overtime
            + "\n\nTotalPay = " + TotalPay;
    }
}

class FileReader
{
    public List<Staff> ReadFile()
    {
        List<Staff> myStaff = new List<Staff>();
            string[] result = new string[2];
        string path = "staff.txt";
        string[] separator = { ", " };

        if (File.Exists(path))
        {
            using (StreamReader sr = new
                StreamReader(path))
            {
                while (!sr.EndOfStream)
                {
                    result =
                        sr.ReadLine().Split(separator,
                        StringSplitOptions.RemoveEmptyEntries);

                    if (result[1] == "Manager")
                        myStaff.Add(new
                            Admin(result[0]));
                    else if (result[1] == "Admin") myStaff.Add(new
                          Admin(result[0]));
                }
                sr.Close();
            }
        } else
        {
            Console.WriteLine("Error: File does not exist");
        }

        return myStaff;
    }
}

    class PaySlip
    {
        private int month;
        private int year;
        enum MonthsOfYear { JAN = 1, FEB = 2, MAR, APR, MAY, JUN, JUL, AUG, SEP, OCT, NOV, DEC }

        public PaySlip(int payMonth, int payYear)
        {
            month = payMonth;
            year = payYear;
        }

        public void GeneratePaySlip(List<Staff>
            myStaff)
        {
            string path;

            foreach (Staff f in myStaff)
            {
                path = f.NameOfStaff + ".txt";

                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.WriteLine("PAYSLIP FOR {0} {1}", (MonthsOfYear)month, year);
                    sw.WriteLine("====================");
                    sw.WriteLine("Name of Staff: {0}", f.NameOfStaff);
                    sw.WriteLine("Hours Worked: {0}", f.HoursWorked);
                    sw.WriteLine("");
                    sw.WriteLine("Basic Pay: {0:C}", ((Manager)f).Allowance);

                    if (f.GetType() == typeof(Manager))
                        sw.WriteLine("Allowance: {0:C}", ((Manager)f).Allowance);
                    else if (f.GetType() == typeof(Admin))
                        sw.WriteLine("Overtime: {0:C}", ((Admin)f).Overtime);

                    sw.WriteLine("");
                    sw.WriteLine("====================");
                    sw.WriteLine("Total Pay: {0:C}", f.TotalPay);
                    sw.WriteLine("====================");

                    sw.Close();

                }
            }
        }

        public void GenerateSummary(List<Staff> myStaff)
        {
            var result
                  = from f in myStaff
                    where f.HoursWorked < 10
                    orderby f.NameOfStaff ascending
                    select new
                    {
                        f.NameOfStaff,
                        f.HoursWorked
                    };

            string path = "summary.txt";

            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.WriteLine("Staff with less than 10 working hours");
                sw.WriteLine("");

                foreach (var f in result)
                    sw.WriteLine("Name of Staff: {0}, Hours Worked: {1}", f.NameOfStaff, f.HoursWorked);

                sw.Close();
            }
        }

        public override string ToString()
        {
            return "month = " + month + "year = " + year;
        }
    }
}