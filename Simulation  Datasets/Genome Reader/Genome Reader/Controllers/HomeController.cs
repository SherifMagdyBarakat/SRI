using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Diagnostics;
using System.IO;
using GenomeReader.Models;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace Test.Controllers
{


    public  class HomeController: Controller
    {
        string genome;
       
        public ViewResult Index()
        { 
            return View();
        }

   
        [HttpPost]
        public IActionResult Index(Files file)
        {
            string RS1 = "GCTTTTCATTCTGACTGCAACGGGCAATATGTCTCTGTGTGGATTAAAAAAAGAGTGTCTGATAGCAGCTTCTGAACTGGTTACCTGCCGTGAGTAAATTAAAATTTTATTGACTTAGGTCACTAAATACTTTAACCAATATAGGCATAGCGCACAGACAGATAAAAATTACAGAGTACACAACATCCATGAAACGCATTAGCACCACCATTACCACCACCATCACCATTACCACAGGTAACGGTGCGGGCTGACGCGTACAGGAAACACAGAAAAAAGC";
            string RS2 = "ATTACAGAGTACACAACATCCTCAAAGCCTACCGGTGACAGTGCGGGCTTTTTTTTCGACCAAAGGTAACGAGGTAACAACTCATGGCATGCGAGTGTTGAAGTTTTCAGGAGATCCTAAAGGCAGGCTGTACCCGTTACCTAGCCAGTTGGCATTAAACGTATCCTAGACGGTACCTAGGCATGCCCTACGTAATCGTAGCCTTAGCAATCTCCAGTCC";
            string RS3 = "CGGTCGAAAAACTGCTGGCAGTGGGGCATTACCTCGAATCTACCGTCGATATTGCTGAGTCCACCCGCCGTATTGCGGCAAGTCGTATTCCGGCTGATCACATGGTGCTGATGGCAGGTTTCACCGCCGGTAATGAAAAAGGCGAACTGGTGGTGCTTGGACGCAACGGTTCCGACTACTCTGCTGCGGTGCTGGCTGCCTGTTTACGCGCCGATTGTTGCGAGATTTGGACGGACGTTGACGGGGTCTATACCTGCGACCCGCGTCAGGTGCCCGATGCGAGGTTGTTGAAGTCGATGTCCTACCAGGAAGCGATGGAGCTTTCCTACTTCGGCGCTCTAGGTCAGGCC";
            string RSC1 = "GCTTTTCATTCTGACTGCAACGGGCAATATGTCTCTGTGTGGATTAAAAAAAGAGTGTCTGATAGCAGCTTCTGAACTGGTTACCTGCCGTGAGTAAATTAAAATTTTATTGACTTAGGTCACTAAATACTTTAACCAATATAGGCATAGCGCACAGACAGATAAAAATTACAGAGTACACAACATCCATGAAACGCATTAGCACCACCATTACCACCACCATCACCATTACCACAGGTAACGGTGCGGGCTGACGCGTACAGGAAACACAGAAAAAAGC ";
            string RSC2 = "ATTACAGAGTACACAACATCCTCAAAGCCTACCGGTGACAGTGCGGGCTTTTTTTTCGACCAAAGGTAACGAGGTAACAACTCATGGCATGCGAGTGTTGAAGTTTTCAGGAGATCCTAAAGGCAGGCTGTACCCGTTACCTAGCCAGTTGGCATTAAACGTATCCTAGACGGTACCTAGGCATGCCCTACGTAATCGTAGCCTTAGCAATCTCCAGTCC";
            string RSC3 = "CGGTCGAAAAACTGCTGGCAGTGGGGCATTACCTCGAATCTACCGTCGATATTGCTGAGTCCACCCGCCGTATTGCGGCAAGTCGTATTCCGGCTGATCACATGGTGCTGATGGCAGGTTTCACCGCCGGTAATGAAAAAGGCGAACTGGTGGTGCTTGGACGCAACGGTTCCGACTACTCTGCTGCGGTGCTGGCTGCCTGTTTACGCGCCGATTGTTGCGAGATTTGGACGGACGTTGACGGGGTCTATACCTGCGACCCGCGTCAGGTGCCCGATGCGAGGTTGTTGAAGTCGATGTCCTACCAGGAAGCGATGGAGCTTTCCTACTTCGGCGCTCTAGGTCAGGCC";
            int c = 40;
            int[] lengths = new int[] {201,251,301};

            for (int j = 0; j < lengths.Length; j++)
            {
                int readCount = 1;
                int kmerLength = lengths[j];
                string filename = "sra_data1 - L " + kmerLength;
                System.IO.StreamWriter sample = new StreamWriter("D:/Dataset/" + filename + ".fasta", append: true);

                using (StreamReader srg = new StreamReader("D:/Dataset/" + file.InputFileName))
                {
                    while (srg.Peek() > -1)
                    {
                        genome = Regex.Replace(srg.ReadToEnd().Trim(), @"\t|\n|\r", "");
                    }
                    srg.Dispose();
                    srg.Close();
                }


                for (int i = 0; i < genome.Length - kmerLength + 1; i = i + 25)
                {
                    string kmer = genome.Substring(i, kmerLength);

                    sample.WriteLine("\r\n> SRR- " + readCount + "\r\n" + kmer);
                    kmer = string.Empty;
                    readCount++;
                }

                for (int i = 0; i < genome.Length - kmerLength + 1; i = i + 50)
                {
                    string kmer = genome.Substring(i, kmerLength);

                    sample.WriteLine("\r\n> SRR- " + readCount + "\r\n" + kmer);
                    kmer = string.Empty;
                    readCount++;

                }
                for (int i = 0; i < genome.Length - kmerLength + 1; i = i + 75)
                {
                    string kmer = genome.Substring(i, kmerLength);

                    sample.WriteLine("\r\n> SRR- " + readCount + "\r\n" + kmer);
                    kmer = string.Empty;
                    readCount++;

                }
                for (int i = 0; i < genome.Length - kmerLength + 1; i = i + 100)
                {
                    string kmer = genome.Substring(i, kmerLength);

                    sample.WriteLine("\r\n> SRR- " + readCount + "\r\n" + kmer);
                    kmer = string.Empty;
                    readCount++;

                }

                for (int i = 0; i < genome.Length - kmerLength + 1; i = i + 150)
                {
                    string kmer = genome.Substring(i, kmerLength);

                    sample.WriteLine("\r\n> SRR- " + readCount + "\r\n" + kmer);
                    kmer = string.Empty;
                    readCount++;

                }
                for (int i = 0; i < genome.Length - kmerLength + 1; i = i + 200)
                {
                    string kmer = genome.Substring(i, kmerLength);

                    sample.WriteLine("\r\n> SRR- " + readCount + "\r\n" + kmer);
                    kmer = string.Empty;
                    readCount++;

                }
                for (int i = 0; i < genome.Length - kmerLength + 1; i = i + 300)
                {
                    string kmer = genome.Substring(i, kmerLength);

                    sample.WriteLine("\r\n> SRR- " + readCount + "\r\n" + kmer);
                    kmer = string.Empty;
                    readCount++;

                }
                readCount++;
                string tail = genome.Substring(genome.Length- kmerLength);
                sample.WriteLine("\r\n> SRR- " + readCount + "\r\n" + tail);
                tail = string.Empty;
                readCount++;




                if (RSC1 != "")
                    sample.WriteLine("\r\n> SRR-RSC1 " + "\r\n" + RSC1.Substring(RSC1.Length - kmerLength));
                if (RSC2 != "")
                    sample.WriteLine("\r\n> SRR-RSC2 " + "\r\n" + RSC2.Substring(RSC2.Length - kmerLength));
                if (RSC3 != "")
                    sample.WriteLine("\r\n> SRR-RSC3 " + "\r\n" + RSC3.Substring(RSC3.Length - kmerLength));

                for (int k=1;k<c;k++)
                {
                if(RS1!="")
                sample.WriteLine("\r\n> SRR-RS1 " + "\r\n" + RS1.Substring(0, kmerLength));
                if (RS2 != "")
                sample.WriteLine("\r\n> SRR-RS2 " +  "\r\n" + RS2.Substring(0, kmerLength));
                if (RS3 != "")
                sample.WriteLine("\r\n> SRR-RS3 " +  "\r\n" + RS3.Substring(0, kmerLength));
                
                if (RSC1 != "")
                 sample.WriteLine("\r\n> SRR-RSC1 " +  "\r\n" + (RSC1.Substring(RSC1.Length - kmerLength)).Substring(0, kmerLength-k));
                if (RSC2 != "")
                 sample.WriteLine("\r\n> SRR-RSC2 " +  "\r\n" + (RSC2.Substring(RSC2.Length - kmerLength)).Substring(0, kmerLength - k));
                    if (RSC3 != "")
                 sample.WriteLine("\r\n> SRR-RSC3 " +  "\r\n" + (RSC3.Substring(RSC3.Length - kmerLength)).Substring(0, kmerLength - k));

                }


                sample.Dispose();
                sample.Close();


            }

            return View();
        }

        
              
    }
}
