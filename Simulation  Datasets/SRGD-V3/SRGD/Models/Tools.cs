using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SRGD.Models
{
    public class Tools
    {
     
        public string mask(string s)
        {
            string[] pattren = { "A", "C", "G", "T" };
            for (int i = 0; i < pattren.Length; i++)
            {
                s = s.Replace(pattren[i], (i + 1).ToString());
            }
            return s;

        }
        public string unmask(string s)
        {
            string[] pattren = { "1", "2", "3", "4" };
            string[] pattrenL = { "A", "C", "G", "T" };
            for (int i = 0; i < pattren.Length; i++)
            {
                s = s.Replace(pattren[i], pattrenL[i]);
            }



            return s;

        }
        public bool IsInteger(double d)
        {
            if (d == (int)d) return true;
            else return false;
        }

        public string Partitions(int PN,string Alias)
        {
            string Partitions = "";
            for (int i = 1; i <= PN; i++)
            {
                Partitions += "COALESCE(+"+Alias+"P" + i.ToString() + ", '')+";
            }
           return Partitions = Partitions.Remove(Partitions.Length - 1, 1);
        }
        public string PartitionsPlusOne(int PN, string Alias)
        {
            string Partitions = "";
            for (int i = 1; i <= PN+1; i++)
            {
                Partitions += "COALESCE("+Alias+"P" + i.ToString() + ", '')+";
            }
            return Partitions = Partitions.Remove(Partitions.Length - 1, 1);
        }

        public string PatternPartitions(int PN)
        {
            string Partitions = "";
            for (int i = 2; i < PN; i=i+2)
            {
                Partitions += " P" + i.ToString() + ",";
            }
            Partitions = Partitions + "P" +PN.ToString();
            return Partitions;
        }

        public string AlignmentPatternCheck(int PN)
        {
            string Partitions = "";
            for (int i = 2; i < PN; i = i + 2)
            {
                Partitions += " R.P" + i.ToString() + "="+ "REF.P" + i.ToString()+" AND ";
            }
            Partitions = Partitions+ "R.P"+PN.ToString()+"=LEFT(REF.P"+PN.ToString()+",LEN(R.P"+PN.ToString()+"))";
            return Partitions; 
        }
       
    



    }
}
