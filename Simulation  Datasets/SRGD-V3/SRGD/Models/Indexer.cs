using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SRGD.Models
{
    public class Indexer
    {
        string[] tables = { "AA", "AC", "AG", "AT", "CA", "CC", "CG", "CT", "GA", "GC", "GG", "GT", "TA", "TC", "TG", "TT" };

        string[] filesA = { "AAA", "AAC", "AAG", "AAT", "ACA", "ACC", "ACG", "ACT", "AGA", "AGC", "AGG", "AGT", "ATA", "ATC", "ATG", "ATT" };
        string[] filesC = { "CAA", "CAC", "CAG", "CAT", "CCA", "CCC", "CCG", "CCT", "CGA", "CGC", "CGG", "CGT", "CTA", "CTC", "CTG", "CTT" };
        string[] filesG = { "GAA", "GAC", "GAG", "GAT", "GCA", "GCC", "GCG", "GCT", "GGA", "GGC", "GGG", "GGT", "GTA", "GTC", "GTG", "GTT" };
        string[] filesT = { "TAA", "TAC", "TAG", "TAT", "TCA", "TCC", "TCG", "TCT", "TGA", "TGC", "TGG", "TGT", "TTA", "TTC", "TTG", "TTT" };

        string[] GfilesA = { "GAAA", "GAAC", "GAAG", "GAAT", "GACA", "GACC", "GACG", "GACT", "GAGA", "GAGC", "GAGG", "GAGT", "GATA", "GATC", "GATG", "GATT" };
        string[] GfilesC = { "GCAA", "GCAC", "GCAG", "GCAT", "GCCA", "GCCC", "GCCG", "GCCT", "GCGA", "GCGC", "GCGG", "GCGT", "GCTA", "GCTC", "GCTG", "GCTT" };
        string[] GfilesG = { "GGAA", "GGAC", "GGAG", "GGAT", "GGCA", "GGCC", "GGCG", "GGCT", "GGGA", "GGGC", "GGGG", "GGGT", "GGTA", "GGTC", "GGTG", "GGTT" };
        string[] GfilesT = { "GTAA", "GTAC", "GTAG", "GTAT", "GTCA", "GTCC", "GTCG", "GTCT", "GTGA", "GTGC", "GTGG", "GTGT", "GTTA", "GTTC", "GTTG", "GTTT" };



        string[] RFiles = { "AAA", "AAC", "AAG", "AAT", "ACA", "ACC", "ACG", "ACT", "AGA", "AGC", "AGG", "AGT", "ATA", "ATC", "ATG", "ATT", "CAA", "CAC", "CAG", "CAT", "CCA", "CCC", "CCG", "CCT", "CGA", "CGC", "CGG", "CGT", "CTA", "CTC", "CTG", "CTT", "GAA", "GAC", "GAG", "GAT", "GCA", "GCC", "GCG", "GCT", "GGA", "GGC", "GGG", "GGT", "GTA", "GTC", "GTG", "GTT", "TAA", "TAC", "TAG", "TAT", "TCA", "TCC", "TCG", "TCT", "TGA", "TGC", "TGG", "TGT", "TTA", "TTC", "TTG", "TTT" };
        string[] GFiles = { "GAAA", "GAAC", "GAAG", "GAAT", "GACA", "GACC", "GACG", "GACT", "GAGA", "GAGC", "GAGG", "GAGT", "GATA", "GATC", "GATG", "GATT", "GCAA", "GCAC", "GCAG", "GCAT", "GCCA", "GCCC", "GCCG", "GCCT", "GCGA", "GCGC", "GCGG", "GCGT", "GCTA", "GCTC", "GCTG", "GCTT", "GGAA", "GGAC", "GGAG", "GGAT", "GGCA", "GGCC", "GGCG", "GGCT", "GGGA", "GGGC", "GGGG", "GGGT", "GGTA", "GGTC", "GGTG", "GGTT", "GTAA", "GTAC", "GTAG", "GTAT", "GTCA", "GTCC", "GTCG", "GTCT", "GTGA", "GTGC", "GTGG", "GTGT", "GTTA", "GTTC", "GTTG", "GTTT" };
        StreamWriter[] pFilesG = new StreamWriter[64];
        int counter { get; set; }
        public int maxL { get; set; }
        public int minL { get; set; }
        public Int64 trL { get; set; }
        public Int64 countR { get; set; }
        public Int64 countV { get; set; }
        public Int64 TotalCount { get; set; }
    
        public int PL { get; set; }
        StreamWriter[] pFilesR; 
        string PartitionsR { get; set; }
        string PartitionsKMERS { get; set; }
        private string tempLine;
        string genome { get; set; }
        public Int64 merIndex { get; set; }
        public Int64 GL { get; set; }
        int length { get; set; }
    
        private MasterDbContext _MasterDbContext;
        private ADbContext _ADbContext;
        private CDbContext _CDbContext;
        private GDbContext _GDbContext;
        private TDbContext _TDbContext;
        private IConfiguration _config;
        private Tools _T;
        public Indexer(  MasterDbContext MasterDbContext, ADbContext AContext, CDbContext CContext, GDbContext GContext, TDbContext TContext, IConfiguration config, Tools T)
        {
            _MasterDbContext = MasterDbContext;
            _ADbContext = AContext;
            _CDbContext = CContext;
            _GDbContext = GContext;
            _TDbContext = TContext;
            _config = config;
            _T = T;
        }
        ////////////////
        public void FileSplitter(int expID, IFormFile[] Reads, IFormFile Reference, string filePath, ReadsType? ReadsType,int PN)
        {
            pFilesR = new StreamWriter[64];
            int counter = 0;
             maxL = 1;
             minL = 10000;
             trL = 0;
             countR = 0;
             countV = 0;
             TotalCount = 0;
            for (int i = 0; i < Reads.Length; i++)
            {
                if (Reads[i].Length > 0)
                {

                    using (var stream = System.IO.File.Create(filePath + "/" + Reads[i].FileName))
                    {
                        Reads[i].CopyTo(stream);

                    }
                }

            }
            if (Reference.Length > 0)
            {
                using (var stream = System.IO.File.Create(filePath + "/" + Reference.FileName))
                {

                    Reference.CopyTo(stream);

                }
            }

            for (int i = 0; i < Reads.Length; i++)
            {

                StreamWriter temp = new StreamWriter(filePath + "/" + "Temp" + i + ".txt");
                using (StreamReader fasta = new StreamReader(filePath + "/" + Reads[i].FileName))
                {
                    while (fasta.Peek() > -1)
                    {
                        string currentLine = fasta.ReadLine().Trim();

                        if (currentLine != string.Empty)
                            if (currentLine[0] == '>')
                            {
                                if (tempLine != string.Empty)
                                {
                                    temp.WriteLine(tempLine);
                                    tempLine = string.Empty;
                                }
                            }
                            else
                                tempLine += currentLine;
                    }

                    if (tempLine != string.Empty)
                        temp.WriteLine(tempLine);
                    fasta.Dispose();
                    fasta.Close();
                }
                tempLine = string.Empty;
                temp.Dispose();
                temp.Close();

            }


            //////////////////

            for (int i = 0; i < RFiles.Length; i++)
            {
                pFilesR[i] = new StreamWriter(filePath + "/" + RFiles[i] + ".txt");
            }

            string[] tempPaths = System.IO.Directory.GetFiles(filePath, "Temp*.*", SearchOption.AllDirectories);

            for (int i = 0; i < tempPaths.Length; i++)
            {
                using (StreamReader tempr = new StreamReader(tempPaths[i]))
                {
                    while (tempr.Peek() > -1)
                    {

                        string currentLine = tempr.ReadLine().Trim();

                        if (currentLine != string.Empty)
                        {
                            if (counter % 2 != 0 && ReadsType.ToString() == "Paired")
                            {
                                //Not writen
                            }
                            else
                            {

                                if (currentLine.Length > maxL)
                                {
                                    maxL = currentLine.Length;
                                }

                                if (currentLine.Length < minL)
                                {
                                    minL = currentLine.Length;
                                }

                                if (currentLine.Contains("N"))
                                {
                                    countR++;
                                }
                                else
                                {
                                    int index = Array.IndexOf(RFiles, currentLine[0].ToString() + currentLine[1].ToString() + currentLine[2].ToString());
                                    pFilesR[index].WriteLine((counter + 1).ToString() + "-" + _T.mask(currentLine));
                                    countV++;
                                    trL += currentLine.Length;

                                }

                                TotalCount++;
                            }

                            counter++;
                        }

                    }

                    tempr.Dispose();
                    tempr.Close();
                    FileInfo f = new FileInfo(tempPaths[i]);
                    f.Delete();
                }

            }
            for (int i = 0; i < pFilesR.Length; i++)
            {
                pFilesR[i].Dispose();
                pFilesR[i].Close();
            }



            //////////////////////////////////
            PartitionsR = "";
            for (int i = 1; i <= PN + 1; i++)
            {
                PartitionsR += "P" + i.ToString() + " VARCHAR(MAX),";
            }
            PartitionsKMERS = "";
           
            PartitionsKMERS = PartitionsR.Remove(PartitionsR.Length - 1, 1);
            string expid = expID.ToString();

            Parallel.Invoke(
           () => TablesCreation(_ADbContext, expid, PartitionsR, PartitionsKMERS),
           () => TablesCreation(_CDbContext, expid, PartitionsR, PartitionsKMERS),
           () => TablesCreation(_GDbContext, expid, PartitionsR, PartitionsKMERS),
           () => TablesCreation(_TDbContext, expid, PartitionsR, PartitionsKMERS)
                      );
            ///////////////////////////////////////
 
            double parl = double.Parse(maxL.ToString()) / double.Parse(PN.ToString());
            if (_T.IsInteger(parl))
                PL = maxL / PN;
            else
                PL = (maxL / PN) + 1;
           
           

        }



        public void ParallelReadIndexing(string path,int PN,int PL,int expID,int rt, IFormFile Reference)
        {
            Parallel.Invoke(
           () => Indexing(_ADbContext, filesA,"A",rt, path, PN,PL, expID),
           () => Indexing(_CDbContext, filesC,"C", rt, path, PN, PL, expID),
           () => Indexing(_GDbContext, filesG,"G", rt, path, PN, PL, expID),
           () => Indexing(_TDbContext, filesT,"T", rt, path, PN, PL, expID)
              );
            using (StreamReader srg = new StreamReader(path + "/" + Reference.FileName))
            {
                while (srg.Peek() > -1)
                {
                    genome = Regex.Replace(srg.ReadToEnd().Trim(), @"\t|\n|\r", "");
                }
                srg.Dispose();
                srg.Close();
            }

            GL = genome.Length;
        }
        public void ParallelReferenceIndexing(string path, int PN, int PL, int expID, int rt)
        {
            Parallel.Invoke(
           () => Indexing(_ADbContext, GfilesA, "A", rt, path, PN, PL, expID),
           () => Indexing(_CDbContext, GfilesC, "C", rt, path, PN, PL, expID),
           () => Indexing(_GDbContext, GfilesG, "G", rt, path, PN, PL, expID),
           () => Indexing(_TDbContext, GfilesT, "T", rt, path, PN, PL, expID)
              );
           
        }


         void Indexing(DbContext db,string[] filenames, string node,   int rt, string path,int PN,int PL, int expID)
        {
            string table;
            SqlConnection con = new SqlConnection(_config.GetConnectionString(node));
            SqlConnection conM = new SqlConnection(_config.GetConnectionString("MASTER"));
            for (int x = 0; x < filenames.Length; x++)
            {
                List<object[]> list = new List<object[]>();
                List<object[]> listPartitions = new List<object[]>();
                if (rt == 1)
                    table = "";
                else
                    table = filenames[x].Substring(1, 2)+expID;

                using (StreamReader srSpl = new StreamReader(path +"/"+ filenames[x] + ".txt"))

                {
                   
                    while (srSpl.Peek() > -1)
                    {
                        
                        string currentLine = srSpl.ReadLine().Trim();
                        object[] rows = new object[PN+3];
                       
                        string offset = currentLine.Substring(0, currentLine.IndexOf("-"));
                            int hk = int.Parse(currentLine.Substring(offset.Length + 4, 3));
                            string read = currentLine.Substring(offset.Length+1);
                            rows[0] = Int64.Parse(offset);
                            rows[1] = hk;
                        if (rt == 1)
                        {
                            for (int k = 2; k < rows.Length; k++)
                            {

                               if ( k == rows.Length - 1)
                                {
                                    rows[k] = read;

                                    break;
                                }
                                else if (read.Length < PL)
                                {
                                    rows[k] = read;

                                    break;
                                }
                                else
                                {
                                    rows[k] = read.Substring(0, PL);

                                    read = read.Substring(PL);
                                }
                            }
                        }
                        else
                        {
                            for (int k = 2; k < rows.Length-1; k++)
                            {

                                if (read.Length == 0)
                                {
                                    break;
                                }
                                else if (read.Length < PL)
                                {
                                    rows[k] = read;

                                    break;
                                }

                                else
                                {
                                    rows[k] = read.Substring(0, PL);

                                    read = read.Substring(PL);
                                }
                            }



                        }
                            list.Add(rows);
                       
                    }
                    srSpl.Dispose();
                    srSpl.Close();
                    File.Delete(path +"/"+ filenames[x] + ".txt");
                }

                if (rt == 1)
                {
                    DataTable t = ConvertListToDataTable(list, rt, PN);
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(con))
                    {
                        bulkCopy.DestinationTableName = "KMERS"+expID; // Your SQL Table name
                        bulkCopy.BulkCopyTimeout = 1300;
                        bulkCopy.ColumnMappings.Add("OFFSET", "OFFSET");
                        bulkCopy.ColumnMappings.Add("HK", "HK");
                        for(int i=1;i<=PN+1;i++)
                        {
                            bulkCopy.ColumnMappings.Add("P"+i.ToString(), "P"+ i.ToString());
                        }


                        con.ConnectionTimeout.Equals(1300);
                        con.Open();
                        bulkCopy.WriteToServer(t);
                        con.Close();
                    }
                }
                else
                {
                    DataTable t = ConvertListToDataTable(list, rt, PN);
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(con))
                    {
                   
                        bulkCopy.DestinationTableName = table; // Your SQL Table name
                        bulkCopy.BulkCopyTimeout = 600;
                         bulkCopy.ColumnMappings.Add("ROW_ID", "ROW_ID");
                        bulkCopy.ColumnMappings.Add("HK", "HK");
                        for (int i = 1; i <= PN; i++)
                        {

                            bulkCopy.ColumnMappings.Add("P" + i.ToString(), "P" + i.ToString());
                        }
                        con.ConnectionTimeout.Equals(600);
                        con.Open();
                        bulkCopy.WriteToServer(t);
                        con.Close();

                    }
                    
                }


                if (rt == 0)
                {
                    string index = String.Format("CREATE NONCLUSTERED INDEX INDEX_GENOME ON {0} (HK)", table);
                    SqlCommand cmdIndx = new SqlCommand(index, con);
                    con.Open();
                    cmdIndx.ExecuteNonQuery();
                    con.Close();
                }

                GC.Collect();
            }
            
        }
        public void ParallelLoadpartitions(int PN,string expID)
        {
            Parallel.Invoke(
           () => Loadpartitions(_ADbContext,"A", expID,PN),
           () => Loadpartitions(_CDbContext,"C" ,expID,PN),
           () => Loadpartitions(_GDbContext,"G", expID,PN),
           () => Loadpartitions(_TDbContext,"T", expID,PN)
                   );
        }
        protected void Loadpartitions(DbContext db, string node,string expID,int PN)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_config.GetConnectionString("MASTER").ToString());

            for (int i = 0; i < tables.Length; i++)
            {
                for (int j = 1; j <= PN; j++)
                {
                db.Database.ExecuteSqlRaw("DECLARE @SERVER VARCHAR(50)={0};DECLARE @expID VARCHAR(30)={1};DECLARE @P VARCHAR(10)={2};DECLARE @T VARCHAR(30)={3};DECLARE @N VARCHAR(1)={4};DECLARE @SQLSTRING NVARCHAR(MAX)='INSERT INTO ['+@SERVER+'].[GENOME].[DBO].[PARTITIONS'+@expID+'](N,T,ROW_ID,PARTITION) SELECT N='''+@N+''',T='''+@T+''',ROW_ID,'+@P+' FROM '+@T+';';EXEC(@SQLSTRING);", builder.DataSource,expID, "P"+j, tables[i]+expID,node);
                }
            }

        }
        //////////////////

        private static DataTable ConvertListToDataTable(List<object[]> list, int rt,int PN)
        {
            // New table.
            DataTable table = new DataTable();
            if (rt == 1)
            {
                table.Columns.Add("OFFSET", typeof(Int64));
                table.Columns.Add("HK", typeof(int));

                for (int j = 1; j <= PN+1; j++)
                {
                    table.Columns.Add("P" + j, typeof(string));
                }

            }
            else
            {
                table.Columns.Add("ROW_ID", typeof(Int64));
                table.Columns.Add("HK", typeof(int));

                for (int j = 1; j <= PN; j++)
                {
                    table.Columns.Add("P" + j, typeof(string));
                }
            }

          



            // Add rows.
            foreach (var array in list)
            {
                DataRow dr = table.NewRow();
                dr[0] = array[0];
                dr[1] = array[1];

                if (rt == 1)
                {
                    for (int i = 2; i < array.Length; i++)
                    {
                        if (array[i] == null)
                            dr[i] = DBNull.Value;
                        else
                            dr[i] = array[i].ToString();
                    }
                }
                else
                {
                    for (int i = 2; i < array.Length-1; i++)
                    {
                        if (array[i] == null)
                            dr[i] = DBNull.Value;
                        else
                            dr[i] = array[i].ToString();
                    }

                }
                table.Rows.Add(dr);
            }
            return table;
        }




    ////////////////

    private void TablesCreation(DbContext db, string expID, string PR,string PK)
    {
        string[] tables = { "AA", "AC", "AG", "AT", "CA", "CC", "CG", "CT", "GA", "GC", "GG", "GT", "TA", "TC", "TG", "TT" };

        for (int i = 0; i < tables.Length; i++)
        {
            db.Database.ExecuteSqlRaw("DECLARE @T VARCHAR(30)={0};  DECLARE @P NVARCHAR(MAX)={1};DECLARE @SQLSTRING NVARCHAR(MAX) = 'CREATE TABLE ' + @T + ' ( ROW_ID BIGINT PRIMARY KEY CLUSTERED,HK INT NOT NULL,FER INT DEFAULT(1), FCR INT  DEFAULT(1),FP INT  DEFAULT(1),'+ @P + 'RI BIT DEFAULT(0));';EXEC(@SQLSTRING);", tables[i] + expID, PR);
        }
            db.Database.ExecuteSqlRaw("DECLARE @T VARCHAR(30)={0}; DECLARE @P NVARCHAR(MAX)={1};DECLARE @SQLSTRING NVARCHAR(MAX)='CREATE TABLE '+@T+' (OFFSET BIGINT PRIMARY KEY CLUSTERED,HK INT NOT NULL,'+@P+');';EXEC(@SQLSTRING);", "KMERS"+expID, PK);
    }

        public void Cleanupexp( string expID)
        {
            Parallel.Invoke(
           () => TablesDeletion(_ADbContext, expID),
           () => TablesDeletion(_CDbContext, expID),
           () => TablesDeletion(_GDbContext, expID),
           () => TablesDeletion(_TDbContext, expID)
                      );
        }
        private void TablesDeletion(DbContext db, string expID)
        {
            string[] tables = { "AA", "AC", "AG", "AT", "CA", "CC", "CG", "CT", "GA", "GC", "GG", "GT", "TA", "TC", "TG", "TT" };

            for (int i = 0; i < tables.Length; i++)
            {
                db.Database.ExecuteSqlRaw("DECLARE @T VARCHAR(30)={0};  DECLARE @SQLSTRING NVARCHAR(MAX) = 'DROP TABLE IF EXISTS dbo.'+@T+';';EXEC(@SQLSTRING);", tables[i] + expID);
            }
            db.Database.ExecuteSqlRaw("DECLARE @T VARCHAR(30)={0}; DECLARE @SQLSTRING NVARCHAR(MAX)='DROP TABLE IF EXISTS dbo.'+@T+';';EXEC(@SQLSTRING);", "KMERS" + expID);
        }



        public void ReferenceSplitter(  string filePath, int maxL,int maxRL,int PN)
        {
         
            for (int i = 0; i < RFiles.Length; i++)
            {
                pFilesG[i] = new StreamWriter(filePath+"/" + GFiles[i] + ".txt");
            }
            
            
            int kmerLength = maxL;
            merIndex = 1;
            for (int r = 0; r < genome.Length - kmerLength + 1; r++)
            {
             string kmer = genome.Substring(r, kmerLength);
                if (kmer.Contains("N"))
                    continue;
                if (r== genome.Length - kmerLength)
                {
                    
                    for (int j=0;j< kmer.Length- maxRL+1; j++)
                    {
                        string k = kmer.Substring(j, maxRL);
                     
                        int index2 = Array.IndexOf(GFiles, "G" + k[0].ToString() + k[1].ToString() + k[2].ToString()); ;
                        pFilesG[index2].WriteLine(merIndex + "-" + _T.mask(k));
                        k = string.Empty;
                        merIndex++;
                    }
                }
   
                
                int index = Array.IndexOf(GFiles, "G" + kmer[0].ToString() + kmer[1].ToString() + kmer[2].ToString()); ;
                pFilesG[index].WriteLine(merIndex +"-"+ _T.mask(kmer));
                kmer = string.Empty;
                merIndex++;
                     
            }



          



            for (int i = 0; i < pFilesG.Length; i++)
            {
                pFilesG[i].Dispose();
                pFilesG[i].Close();
            }

            merIndex = merIndex - 2;




        }



        public int NodesetMaxLength(int expID, int PN)
        {
            SqlConnection conA = new SqlConnection(_config.GetConnectionString("A"));
            SqlConnection conC = new SqlConnection(_config.GetConnectionString("C"));
            SqlConnection conG = new SqlConnection(_config.GetConnectionString("G"));
            SqlConnection conT = new SqlConnection(_config.GetConnectionString("T"));

            int LengthA = setMaxLength(conA, expID,PN);
            int LengthC = setMaxLength(conC, expID, PN);
            int LengthG = setMaxLength(conG, expID, PN);
            int LengthT = setMaxLength(conT, expID, PN);
            return Math.Max(Math.Max(LengthA, LengthC), Math.Max(LengthG, LengthT));
        }
        protected int setMaxLength(SqlConnection db, int expID, int PN)
        {
            int length = 0;
            
            string pn = (PN + 1).ToString();
            db.Open();
            for (int i = 0; i < tables.Length; i++)
            {
                string calc = String.Format("SELECT COALESCE(MAX(TL),0) FROM ( SELECT LEN({0})  AS TL  FROM {1}  )S",_T.PartitionsPlusOne(PN,""), tables[i]+expID);
                SqlCommand cmd = new SqlCommand(calc, db);
                if (int.Parse(cmd.ExecuteScalar().ToString()) > length)
                    length = int.Parse(cmd.ExecuteScalar().ToString());

            }
            db.Close();
            return length;
        }

        public void nodeCreateNonclusteredIndexKMER(string expID)
        {

            Parallel.Invoke(
            () => _ADbContext.Database.ExecuteSqlRaw("DECLARE @T VARCHAR (30)={0};DECLARE @SQLSTRING NVARCHAR(MAX)='CREATE NONCLUSTERED INDEX INDEX_GENOME1 ON '+@T+'(HK);'; EXEC(@SQLSTRING);", "KMERS" + expID)       ,
            () => _CDbContext.Database.ExecuteSqlRaw("DECLARE @T VARCHAR (30)={0};DECLARE @SQLSTRING NVARCHAR(MAX)='CREATE NONCLUSTERED INDEX INDEX_GENOME1 ON '+@T+'(HK);'; EXEC(@SQLSTRING);", "KMERS" + expID)       ,
            () => _GDbContext.Database.ExecuteSqlRaw("DECLARE @T VARCHAR (30)={0};DECLARE @SQLSTRING NVARCHAR(MAX)='CREATE NONCLUSTERED INDEX INDEX_GENOME1 ON '+@T+'(HK);'; EXEC(@SQLSTRING);", "KMERS" + expID)       ,
            () => _TDbContext.Database.ExecuteSqlRaw("DECLARE @T VARCHAR (30)={0};DECLARE @SQLSTRING NVARCHAR(MAX)='CREATE NONCLUSTERED INDEX INDEX_GENOME1 ON '+@T+'(HK);'; EXEC(@SQLSTRING);", "KMERS" + expID)
       
                       );
            }


        public DataTable OverlappingMetrics(string OMT)
        {
            DataTable table = new DataTable();
            SqlConnection con = new SqlConnection(_config.GetConnectionString("MASTER"));
            table.Columns.Add("Overlapping Length", typeof(string));
            table.Columns.Add("O(N)2 Time Complexity", typeof(Int64));
            table.Columns.Add("Hit Index Count", typeof(Int64));
            table.Columns.Add("Overlapping Matched Count", typeof(Int64));
            table.Columns.Add("RI", typeof(string));
            SqlCommand command = new SqlCommand(String.Format("SELECT  OVERLAPPING_LENGTH      ,ORIGINAL_ALGORITHM      ,COUNT_HIT      ,COUNT_MATCH,RI       FROM {0}   WHERE  COUNT_HIT>0 AND COUNT_MATCH>0 ORDER BY OVERLAPPING_LENGTH DESC",  OMT), con);
            con.Open();                                                                                                                                                 
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    DataRow dr = table.NewRow();
                    dr[0] = reader[0];
                    dr[1] = reader[1];
                    dr[2] = reader[2];
                    dr[3] = reader[3];
                    dr[4] = reader[4];
                    table.Rows.Add(dr);
                }

            }
            reader.Close();
            con.Close();
            return table;

        }

    }

}
