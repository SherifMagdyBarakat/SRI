using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SRGD.Models
{

    public class Assembly
    {
        private MasterDbContext _MasterDbContext;

        private SqlConnection conA;
        private SqlConnection conC;
        private SqlConnection conG;
        private SqlConnection conT;
        private IConfiguration _config;
        private Tools  _Tools;
        public int fc { get; set; }
        public int total { get; set; }
        public int countOV { get; set; }
        public  int mac { get; set; }
        string[] tables = { "AA", "AC", "AG", "AT", "CA", "CC", "CG", "CT", "GA", "GC", "GG", "GT", "TA", "TC", "TG", "TT" };
       public  Int64 count { get; set; }
        public Assembly(MasterDbContext MasterDbContext, IConfiguration config,Tools Tools)
        {
            _config = config;
            _Tools = Tools;
            _MasterDbContext = MasterDbContext;
           
            conA = new SqlConnection(_config.GetConnectionString("A"));
            conC = new SqlConnection(_config.GetConnectionString("C"));
            conG = new SqlConnection(_config.GetConnectionString("G"));
            conT = new SqlConnection(_config.GetConnectionString("T"));
        }


        public void calcN50N90(string gl, string ctable, string expID, string Type)
        {
            if (Type == "C")
                _MasterDbContext.Database.ExecuteSqlRaw("DECLARE @GL VARCHAR(30)={0};DECLARE @CTABLE VARCHAR(30)={1};DECLARE @EXPID VARCHAR(30)={2};DECLARE @NMETRICS VARCHAR(30)={3};DECLARE @SQLSTRING1 NVARCHAR(MAX)=' SELECT ROW_NUMBER() OVER(ORDER BY LEN(CONTIG) DESC) AS NUMBER, LEN(CONTIG) AS L   INTO #TEMP FROM '+@CTABLE+'  ORDER BY LEN(CONTIG) DESC;DECLARE @COUNTER AS INT=2 DECLARE @STOP AS INT='+@GL+'*0.5 DECLARE @LENGTH AS BIGINT=0 DECLARE @N50C AS BIGINT=0 WHILE @LENGTH<@STOP BEGIN SELECT @N50C=MIN (L) ,@LENGTH=SUM(L) FROM #TEMP WHERE NUMBER >=1 AND NUMBER <=@COUNTER SELECT @COUNTER=@COUNTER+1 END;INSERT INTO '+@NMETRICS+'(N50C,ExperimentID) SELECT  @N50C AS N50C,'+@EXPID+' AS ExperimentID;'DECLARE @SQLSTRING2 NVARCHAR(MAX)=' SELECT ROW_NUMBER() OVER(ORDER BY LEN(CONTIG) DESC) AS NUMBER, LEN(CONTIG) AS L   INTO #TEMP FROM '+@CTABLE+'  ORDER BY LEN(CONTIG) DESC;DECLARE @COUNTER AS INT=2 DECLARE @STOP AS INT='+@GL+'*0.9 DECLARE @LENGTH AS BIGINT=0 DECLARE @N90C AS BIGINT=0 WHILE @LENGTH<@STOP BEGIN SELECT @N90C=MIN(L) ,@LENGTH=SUM(L) FROM #TEMP WHERE NUMBER >=1 AND NUMBER <=@COUNTER SELECT @COUNTER=@COUNTER+1 END;UPDATE '+@NMETRICS+' SET N90C=@N90C  WHERE ExperimentID='+@EXPID+' ;'EXEC(@SQLSTRING1);  EXEC(@SQLSTRING2);", gl, ctable, expID, "NMETRICS" + expID);
            else
                _MasterDbContext.Database.ExecuteSqlRaw("DECLARE @GL VARCHAR(30)={0};DECLARE @CTABLE VARCHAR(30)={1};DECLARE @EXPID VARCHAR(30)={2};DECLARE @NMETRICS VARCHAR(30)={3};DECLARE @SQLSTRING1 NVARCHAR(MAX)=' SELECT ROW_NUMBER() OVER(ORDER BY LEN(CONTIG) DESC) AS NUMBER, LEN(CONTIG) AS L   INTO #TEMP FROM '+@CTABLE+'  ORDER BY LEN(CONTIG) DESC;DECLARE @COUNTER AS INT=2 DECLARE @STOP AS INT='+@GL+'*0.5 DECLARE @LENGTH AS BIGINT=0 DECLARE @N50S AS BIGINT=0 WHILE @LENGTH<@STOP BEGIN SELECT @N50S=MIN (L) ,@LENGTH=SUM(L) FROM #TEMP WHERE NUMBER >=1 AND NUMBER <=@COUNTER SELECT @COUNTER=@COUNTER+1 END;UPDATE '+@NMETRICS+' SET N50S=@N50S WHERE ExperimentID='+@EXPID+';'DECLARE @SQLSTRING2 NVARCHAR(MAX)=' SELECT ROW_NUMBER() OVER(ORDER BY LEN(CONTIG) DESC) AS NUMBER, LEN(CONTIG) AS L   INTO #TEMP FROM '+@CTABLE+'  ORDER BY LEN(CONTIG) DESC;DECLARE @COUNTER AS INT=2 DECLARE @STOP AS INT='+@GL+'*0.9 DECLARE @LENGTH AS BIGINT=0 DECLARE @N90S AS BIGINT=0 WHILE @LENGTH<@STOP BEGIN SELECT @N90S=MIN (L) ,@LENGTH=SUM(L) FROM #TEMP WHERE NUMBER >=1 AND NUMBER <=@COUNTER SELECT @COUNTER=@COUNTER+1 END;UPDATE '+@NMETRICS+' SET N90S=@N90S  WHERE ExperimentID='+@EXPID+' ;'EXEC(@SQLSTRING1);  EXEC(@SQLSTRING2);", gl, ctable, expID, "NMETRICS" + expID);

        }

        public int NodeDatasetCount(string r, string expID)
        {
            total = 0;

            int CountA = setCount(conA, r, expID);
            int CountC = setCount(conC, r, expID);
            int CountG = setCount(conG, r, expID);
            int CountT = setCount(conT, r, expID);
            total = CountA + CountC + CountG + CountT;
            return total;
        }
        protected int setCount(SqlConnection node, string r,string expID)
        {
           
            int count = 0;
            node.Open();

            for (int i = 0; i < tables.Length; i++)
            {
                string calc = String.Format("SELECT COUNT(ROW_ID) FROM {0} WHERE RI IN({1}) ", tables[i]+expID, r);
                SqlCommand cmd = new SqlCommand(calc, node);
                count = count + int.Parse(cmd.ExecuteScalar().ToString());
            }
            node.Close();
            return count;
        }

        public int NodeDatasetSize(int PN, string expID)
        {
            int SizeA = setSize(conA,PN, expID);
            int SizeC = setSize(conC,PN, expID);
            int SizeG = setSize(conG,PN, expID);
            int SizeT = setSize(conT,PN, expID);
            return SizeA + SizeC + SizeG + SizeT;
        }
        protected int setSize(SqlConnection node,int PN,string expID)
        {
            int size = 0;
            node.Open();

            for (int i = 0; i < tables.Length; i++)
            {
                string calc = String.Format("SELECT COALESCE(SUM(TL),0) FROM ( SELECT LEN({0})  AS TL  FROM {1} )S", _Tools.PartitionsPlusOne(PN,""), tables[i]+expID);
                SqlCommand cmd = new SqlCommand(calc, node);

                size = size + int.Parse(cmd.ExecuteScalar().ToString());
            }
            node.Close();
            return size;
        }


        public int MasterTableCount(string table)
        {
            SqlConnection conM = new SqlConnection(_config.GetConnectionString("Master"));

            int count = 0;
            conM.Open();
            string calc =String.Format( "SELECT COUNT(ROW_ID) FROM {0} ", table);
            SqlCommand cmd = new SqlCommand(calc, conM);
            count = int.Parse(cmd.ExecuteScalar().ToString());
            conM.Close();
            return count;
        }

        public string readContigScaffolds(string expID, string T)
        {
            Int64 countC = 0;
            Int64 countG = 0;
            SqlConnection  conM = new SqlConnection(_config.GetConnectionString("Master"));
            string contigScaffold = "";
            count = 0;
            using (conM)
            {
                SqlCommand command = new SqlCommand(String.Format("SELECT   OFFSET,  CONTIG  FROM {0}  ORDER BY OFFSET;", "CONTIGS"+expID), conM);
                conM.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
          
                        string LINE = reader[0].ToString() + "-" + _Tools.unmask(reader[1].ToString());
                        contigScaffold = contigScaffold + "\n\n" + LINE;
                        if (T == "S")
                        {
                            countC = countC+ reader[1].ToString().ToCharArray().Count(c => c == '2');
                            countG = countG + reader[1].ToString().ToCharArray().Count(c => c == '3');

                            
                        }
                        
                    }
                    count = countC + countG;
                }
                reader.Close();
            }
            conM.Close();
            return contigScaffold;
        }

        public void scaffoldBuilder(string expID)
        {
            _MasterDbContext.Database.ExecuteSqlRaw(" DECLARE @CTABLE VARCHAR(30)={0}; DECLARE @SQLSTRING NVARCHAR(MAX)=' DELETE FROM '+@CTABLE+' FROM  '+@CTABLE+'  T1 INNER JOIN  (SELECT * FROM(SELECT C1.ID AS SXID,C1.OFFSET AS SOFFSETSX,C1.OFFSET+LEN(C1.CONTIG)-1 AS EOFFSETSX, C2.ID AS PXID,C2.OFFSET AS SOFFSETPX,C2.OFFSET+LEN(C2.CONTIG)-1 AS EOFFSETPX, CHARINDEX(C2.CONTIG, C1.CONTIG)AS CHINDX  FROM '+@CTABLE+' AS C1 CROSS JOIN '+@CTABLE+' AS C2 WHERE C1.ID<>C2.ID )S WHERE CHINDX>0 AND SOFFSETSX<SOFFSETPX AND EOFFSETSX>EOFFSETPX)T  ON    T1.ID = T.PXID;';EXEC(@SQLSTRING);","CONTIGS"+expID);
        }

        public Int64 assemblySize(string expID)
        {
            SqlConnection conM = new SqlConnection(_config.GetConnectionString("Master"));
            Int64 size = 0;
            conM.Open();
            string calc = String.Format("SELECT COALESCE(SUM(TL),0) FROM ( SELECT COALESCE(LEN(CONTIG), 0)   AS TL  FROM {0})S","CONTIGS"+expID);
            SqlCommand cmd = new SqlCommand(calc, conM);
            size = size + int.Parse(cmd.ExecuteScalar().ToString());

            conM.Close();
            return size;
        }


       
    }
}