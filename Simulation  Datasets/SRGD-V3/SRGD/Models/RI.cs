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
    public class RI
    {
        private MasterDbContext _MasterDbContext;
        private ADbContext _ADbContext;
        private CDbContext _CDbContext;
        private GDbContext _GDbContext;
        private TDbContext _TDbContext;
        private IConfiguration _config;
        private Tools _Tools;
        string[] tables = { "AA", "AC", "AG", "AT", "CA", "CC", "CG", "CT", "GA", "GC", "GG", "GT", "TA", "TC", "TG", "TT" };
        public Int64[] totals { get; set; }
        public Int64 RepeatSize { get; set; }
        public RI(MasterDbContext MASTERContext, ADbContext AContext, CDbContext CContext, GDbContext GContext, TDbContext TContext, IConfiguration config, Tools Tools)
      
        {
            _MasterDbContext = MASTERContext;
            _ADbContext = AContext;
            _CDbContext = CContext;
            _GDbContext = GContext;
            _TDbContext = TContext;
            _config = config;
            _Tools = Tools;
      
        }

        public void ParallelSetPartitionFrequency(string expID)
        {
         
            Parallel.Invoke(
            () => SetPartitionFrequency(_ADbContext,  "A",  expID),
            () => SetPartitionFrequency(_CDbContext,  "C",  expID),
            () => SetPartitionFrequency(_GDbContext,  "G",  expID),
            () => SetPartitionFrequency(_TDbContext,  "T",  expID)
               );
               
    
        }

        public void SetPartitionFrequency(DbContext db,string node, string expID)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_config.GetConnectionString("MASTER").ToString());

             for (int i = 0; i < tables.Length; i++)
              {
                  db.Database.ExecuteSqlRaw("SET NOCOUNT ON;DECLARE @MASTER VARCHAR(50) ={0}; DECLARE @TP VARCHAR(30) ={1};DECLARE @N VARCHAR(1) ={2}; DECLARE @T VARCHAR(30) ={3};  DECLARE @SQLSTRING NVARCHAR(MAX) = 'MERGE INTO ' + @T + '  AS T USING (SELECT ROW_ID ,SUM(FREQUENCY) AS FP  FROM ['+@MASTER+'].[GENOME].[DBO].['+@TP+']  WHERE N='''+ @N +''' AND T= '''+ @T +''' GROUP BY ROW_ID) AS S ON T.ROW_ID=S.ROW_ID WHEN MATCHED THEN UPDATE SET T.FP=S.FP;'; EXEC(@SQLSTRING);", builder.DataSource, "PARTITIONS" + expID, node, tables[i]+expID);
              }
           
        }
    
        public void ParallelSetERFrequency(string expID,int PN)
        {
            string Partitions = "";
            for (int i = 1; i <= PN; i++)
            {
                Partitions += "P" + i.ToString()+",";
            }
            Parallel.Invoke(
            () => SetERFrequency(_ADbContext,  expID, Partitions),
            () => SetERFrequency(_CDbContext, expID, Partitions),
            () => SetERFrequency(_GDbContext,  expID, Partitions),
            () => SetERFrequency(_TDbContext,  expID, Partitions)
               );


        }


        public void SetERFrequency(DbContext db,  string expID, string p)
        {


            for (int i = 0; i < tables.Length; i++)
            {
                db.Database.ExecuteSqlRaw("EXEC SETFER {0},{1} ", tables[i]+expID, p);
            }
        }



        public void ParallelSetCRFrequency(string expID, int PN, string cov)
        {
            string pn = PN.ToString();
            Parallel.Invoke(
            () => SetECFrequency(_ADbContext, expID, pn,cov),
            () => SetECFrequency(_CDbContext, expID, pn, cov),
            () => SetECFrequency(_GDbContext, expID, pn, cov),
            () => SetECFrequency(_TDbContext, expID, pn, cov)
               );


        }
        public void SetECFrequency(DbContext db, string expID, string p,string cov)
        {
            for (int i = 0; i < tables.Length; i++)
            {
                db.Database.ExecuteSqlRaw("EXEC SETFCR {0},{1} ", tables[i] + expID, p);
            }
        }

        public void ParallelSetRI(string expID, string cov,int PN)
        {
            string Partitions = _Tools.Partitions(PN, "");
            Parallel.Invoke(
            () => SetRI(_ADbContext,  expID, cov, Partitions),
            () => SetRI(_CDbContext,  expID, cov, Partitions),
            () => SetRI(_GDbContext,  expID, cov, Partitions),
            () => SetRI(_TDbContext,  expID, cov, Partitions)
               );


        }

        public void SetRI(DbContext db, string expID, string cov,string Partitions)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_config.GetConnectionString("MASTER").ToString());
            for (int i = 0; i < tables.Length; i++)
            {
                db.Database.ExecuteSqlRaw("DECLARE @T VARCHAR(30)={0};DECLARE @C NVARCHAR(10)={1};DECLARE @SQLSTRING NVARCHAR(MAX)='UPDATE '+@T+' SET RI=1  WHERE FER >'+@C+' OR FCR >'+@C+' OR FP>1;'; EXEC(@SQLSTRING);", tables[i] + expID, cov);
                db.Database.ExecuteSqlRaw("DECLARE @T VARCHAR(30)={0};DECLARE @C NVARCHAR(10)={1};DECLARE @SERVER NVARCHAR(30)={2};DECLARE @RT NVARCHAR(30)={3};DECLARE @P NVARCHAR(MAX)={4};DECLARE @SQLSTRING NVARCHAR(MAX)='INSERT INTO  [' + @SERVER + '].[GENOME].[dbo].[' + @RT + '] (REPETITIVE_SEQUENCE) SELECT '+@P+' FROM '+@T+' WHERE FER >'+@C+' OR FCR >'+@C+';'; EXEC(@SQLSTRING);", tables[i] + expID, cov, builder.DataSource, "REPETITIVE_REGIONS" + expID, Partitions);

            }

        }







        public string[] RepeatConstruction(string exp)
        {
            string[] st = new string[2];
            //_MasterDbContext.Database.ExecuteSqlRaw("DECLARE @CTABLE VARCHAR(30) ={0}; DECLARE @RT VARCHAR(30) ={1}; DECLARE @RAT VARCHAR(30) ={2}; DECLARE @SQLSTRING NVARCHAR(MAX) = 'DECLARE @ROW BIGINT=0; DECLARE @TERMINATE BIGINT=1;WHILE @ROW<>@TERMINATE BEGIN SELECT @ROW=(SELECT COUNT(ID) FROM ' + @RT + ');EXEC REPEAT_ANNOTATION ' + @CTABLE + ',' + @RT + ',' + @RAT + ' EXEC REPEATCONSTRUCT  ' + @RT + ' SELECT @TERMINATE=(SELECT COUNT(ID) FROM ' + @RT + ');END;EXEC REPEAT_ANNOTATION ' + @CTABLE + ',' + @RT + ',' + @RAT + ''; EXEC(@SQLSTRING);", "CONTIGS" + exp, "REPETITIVE_REGIONS" + exp, "INITIAL_REPEAT_ANNOTATION" + exp);
            Initial_Repeat_Annotation("CONTIGS", "REPETITIVE_REGIONS", "INITIAL_REPEAT_ANNOTATION", exp);
            _MasterDbContext.Database.ExecuteSqlRaw("DECLARE @RRTABLE VARCHAR(30) ={0}; DECLARE @SQLSTRING NVARCHAR(MAX) = 'SELECT ID,REPETITIVE_SEQUENCE INTO #TEMP1 FROM ' + @RRTABLE + ';SELECT ID,REPETITIVE_SEQUENCE INTO #TEMP2 FROM ' + @RRTABLE + ';  CREATE TABLE #TEMP3(REPETITIVE_SEQUENCE VARCHAR(MAX));DECLARE @START BIGINT=(SELECT MAX(LEN(REPETITIVE_SEQUENCE))-1 FROM ' + @RRTABLE + ' );DECLARE @END BIGINT=3;WHILE @START>@END BEGIN INSERT INTO #TEMP3(REPETITIVE_SEQUENCE) SELECT MERGED   FROM  '+@RRTABLE+'  R INNER JOIN   (	  SELECT T1.ID AS SXID,LEFT( T1.REPETITIVE_SEQUENCE,LEN(T1.REPETITIVE_SEQUENCE)-@START)+T2.REPETITIVE_SEQUENCE AS MERGED FROM #TEMP1 AS T1 INNER JOIN #TEMP2 AS T2		  ON RIGHT(T1.REPETITIVE_SEQUENCE,@START)=LEFT(T2.REPETITIVE_SEQUENCE,@START)	WHERE T1.ID<>T2.ID 	) S   ON S.SXID=R.ID;SELECT @START=@START-1; END;DROP TABLE #TEMP1;DROP TABLE #TEMP2;  DELETE FROM '+ @RRTABLE+';  INSERT INTO ' + @RRTABLE + '(REPETITIVE_SEQUENCE) SELECT REPETITIVE_SEQUENCE FROM #TEMP3;  DROP TABLE #TEMP3;';   EXEC(@SQLSTRING);", "REPETITIVE_REGIONS" + exp); ;
            Initial_Repeat_Annotation("CONTIGS", "REPETITIVE_REGIONS", "INITIAL_REPEAT_ANNOTATION", exp);
            string st1 = CountRepeat(exp);
            _MasterDbContext.Database.ExecuteSqlRaw("DECLARE @RRTABLE VARCHAR(30) ={0};DECLARE @SQLSTRING NVARCHAR(MAX) = '  SELECT DISTINCT STARTING,ENDING, REPETITIVE_SEQUENCE,SPOSITION, EPOSITION INTO #TEMP FROM '+@RRTABLE+'; DELETE FROM '+@RRTABLE+'; INSERT INTO '+@RRTABLE+' (STARTING,ENDING, REPETITIVE_SEQUENCE,SPOSITION, EPOSITION)  SELECT STARTING,ENDING, REPETITIVE_SEQUENCE,SPOSITION, EPOSITION FROM #TEMP; DROP TABLE #TEMP;';EXEC(@SQLSTRING);", "INITIAL_REPEAT_ANNOTATION" + exp);
            _MasterDbContext.Database.ExecuteSqlRaw("DECLARE @RRTABLE VARCHAR(30) ={0}; DECLARE @SQLSTRING NVARCHAR(MAX) = 'SELECT ID,COUNT(REPETITIVE_SEQUENCE) OVER (PARTITION BY STARTING,ENDING,REPETITIVE_SEQUENCE) AS REPEAT_COUNT  INTO  #TEMP FROM ' + @RRTABLE + ';UPDATE     ' + @RRTABLE + ' 	 SET     ' + @RRTABLE + '.REPEAT_COUNT = T.REPEAT_COUNT  FROM      ' + @RRTABLE + '  T1 INNER JOIN  ( SELECT ID,REPEAT_COUNT FROM #TEMP  )T  ON    T1.ID = T.ID ; DROP TABLE #TEMP;'EXEC(@SQLSTRING); ", "INITIAL_REPEAT_ANNOTATION" + exp);
            _MasterDbContext.Database.ExecuteSqlRaw("DECLARE @IRTA VARCHAR(30) ={0};DECLARE @RTA VARCHAR(30) ={1}; DECLARE @SQLSTRING NVARCHAR(MAX) = 'DECLARE @START AS BIGINT = (SELECT MIN(ID) FROM ' + @IRTA + '); DECLARE @END AS BIGINT = (SELECT MAX(ID) FROM ' + @IRTA + ');WHILE @START<= @END BEGIN SELECT ID,STARTING,ENDING,REPETITIVE_SEQUENCE,SPOSITION,EPOSITION,REPEAT_COUNT INTO #SX FROM ' + @IRTA + ' WHERE ID=@START;DELETE FROM ' + @IRTA + ' WHERE ID IN (SELECT R2.ID AS PARTID FROM #SX AS R1 INNER JOIN ' + @IRTA + ' AS R2 ON CHARINDEX(R2.REPETITIVE_SEQUENCE,R1.REPETITIVE_SEQUENCE)>0 AND R1.ID<>R2.ID AND R1.STARTING=R2.STARTING AND R1.ENDING=R2.ENDING AND R1.REPETITIVE_SEQUENCE<>R2.REPETITIVE_SEQUENCE);DROP TABLE #SX;SELECT @START=@START+1;END;DELETE FROM '+@IRTA+' WHERE REPEAT_COUNT=1; INSERT INTO '+@RTA+' SELECT DISTINCT STARTING,ENDING,REPETITIVE_SEQUENCE,SPOSITION,EPOSITION,REPEAT_COUNT FROM '+@IRTA+'  '; EXEC(@SQLSTRING);", "INITIAL_REPEAT_ANNOTATION" + exp, "REPEAT_ANNOTATION" + exp);
            string st2 = CountUniqueRepeat(exp);
            st[0] = st1;
            st[1] = st2;
            return st;
        }


        public void LoadRepetitiveSequences(string exp)
        {
            _MasterDbContext.Database.ExecuteSqlRaw("DECLARE @RT VARCHAR(30)={0};DECLARE @PT VARCHAR(30)={1};DECLARE @SQLSTRING NVARCHAR(MAX)='INSERT INTO '+@RT+'(REPETITIVE_SEQUENCE)SELECT DISTINCT PARTITION FROM '+@PT+';'; EXEC(@SQLSTRING);", "REPETITIVE_REGIONS" + exp,"PARTITIONS"+exp);
        }

        public void Initial_Repeat_Annotation(string ctable, string rrtable, string rattable, string expid)
        {
            string CTable = ctable + expid;
            string RTable = rrtable + expid;
            string RATable = rattable + expid;
            DataTable contig = new DataTable();
            DataTable repeat = new DataTable();

            contig.Columns.Add("STARTING", typeof(Int64));
            contig.Columns.Add("ENDING", typeof(Int64));
            contig.Columns.Add("CONTIG", typeof(string));
            repeat.Columns.Add("REPETITIVE_SEQUENCE", typeof(string));


            SqlConnection con = new SqlConnection(_config.GetConnectionString("MASTER"));
            SqlCommand commandC = new SqlCommand(String.Format("SELECT OFFSET AS  STARTING,OFFSET+LEN(CONTIG)-1 AS  ENDING,CONTIG  FROM {0} ", CTable), con);
            SqlCommand commandR = new SqlCommand(String.Format("SELECT REPETITIVE_SEQUENCE  FROM {0} ", RTable), con);

            con.Open();
            SqlDataReader readerC = commandC.ExecuteReader();

            if (readerC.HasRows)
            {
                while (readerC.Read())
                {
                    DataRow drC = contig.NewRow();
                    drC[0] = readerC[0];
                    drC[1] = readerC[1];
                    drC[2] = readerC[2];
                    contig.Rows.Add(drC);
                }

            }
            readerC.Close();

            SqlDataReader readerR = commandR.ExecuteReader();
            if (readerR.HasRows)
            {
                while (readerR.Read())
                {
                    DataRow drR = repeat.NewRow();
                    drR[0] = readerR[0];
                    repeat.Rows.Add(drR);
                }
            }
            readerR.Close();
            con.Close();

            foreach (DataRow rowC in contig.Rows)
            {

                foreach (DataRow rowR in repeat.Rows)
                {
                    string contigstring = rowC["contig"].ToString();
                    Int64 contigLength = contigstring.Length;
                    string rs = rowR["REPETITIVE_SEQUENCE"].ToString();
                    int rl = rowR["REPETITIVE_SEQUENCE"].ToString().Length;
                    DataTable annotation = new DataTable();
                    annotation.Columns.Add("STARTING", typeof(Int64));
                    annotation.Columns.Add("ENDING", typeof(Int64));
                    annotation.Columns.Add("REPETITIVE_SEQUENCE", typeof(string));
                    annotation.Columns.Add("SPOSITION", typeof(Int64));
                    annotation.Columns.Add("EPOSITION", typeof(Int64));
                    string sub;


                    for (int i = 0; i <= contigLength - rl; i++)
                    {
                        //if (rl > contigLength)
                        //break;
                        sub = contigstring.Substring(i, rl);
                        if (sub == rs)
                        {
                            DataRow drA = annotation.NewRow();
                            drA[0] = rowC["STARTING"];
                            drA[1] = rowC["ENDING"];
                            drA[2] = rowR["REPETITIVE_SEQUENCE"];
                            drA[3] = i + Int64.Parse(drA[0].ToString());
                            drA[4] = i + rl+ Int64.Parse(drA[0].ToString())-1;
                            annotation.Rows.Add(drA);
                            i = i + rl - 1;
                        }

                    }
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(con))
                    {
                        bulkCopy.DestinationTableName = RATable; // Your SQL Table name
                        bulkCopy.BulkCopyTimeout = 1300;
                        bulkCopy.ColumnMappings.Add("STARTING", "STARTING");
                        bulkCopy.ColumnMappings.Add("ENDING", "ENDING");
                        bulkCopy.ColumnMappings.Add("REPETITIVE_SEQUENCE", "REPETITIVE_SEQUENCE");
                        bulkCopy.ColumnMappings.Add("SPOSITION", "SPOSITION");
                        bulkCopy.ColumnMappings.Add("EPOSITION", "EPOSITION");
                        con.ConnectionTimeout.Equals(1300);
                        con.Open();
                        bulkCopy.WriteToServer(annotation);
                        con.Close();
                    }
                }

            }

        }





        public Int64[] RepeatCount( string expID, int filteredcoverage)
        {
            
            SqlConnection conA = new SqlConnection(_config.GetConnectionString("A"));
            SqlConnection conC = new SqlConnection(_config.GetConnectionString("C"));
            SqlConnection conG = new SqlConnection(_config.GetConnectionString("G"));
            SqlConnection conT = new SqlConnection(_config.GetConnectionString("T"));
            totals = new Int64[3];

            Int64[] CountA = setCount(conA,  expID,  filteredcoverage);
            Int64[] CountC = setCount(conC,  expID,  filteredcoverage);
            Int64[] CountG = setCount(conG,  expID,  filteredcoverage);
            Int64[] CountT = setCount(conT,  expID,  filteredcoverage);
            totals[0] = CountA[0] + CountC[0] + CountG[0] + CountT[0];
            totals[1] = CountA[1] + CountC[1] + CountG[1] + CountT[1];
            totals[2] = CountA[2] + CountC[2] + CountG[2] + CountT[2];

            return totals;
        }
        protected Int64[] setCount(SqlConnection node,  string expID, int filteredcoverage)
        {
            Int64 countP = 0;
            Int64 countFR = 0;
            Int64 countCR = 0;
            node.Open();
            Int64[] counts=new Int64[3];
            
            for (int i = 0; i < tables.Length; i++)
            {
                string calcP = String.Format("SELECT COUNT(ROW_ID) FROM {0} WHERE RI=1 AND FP>1 ", tables[i] + expID);
                string calcFR = String.Format("SELECT COUNT(ROW_ID) FROM {0} WHERE RI=1 AND  FER > {1}  ", tables[i] + expID, filteredcoverage);
                string calcCR = String.Format("SELECT COUNT(ROW_ID) FROM {0} WHERE RI=1 AND FCR > {1} ", tables[i] + expID, filteredcoverage);


                SqlCommand cmdP = new SqlCommand(calcP, node);
                SqlCommand cmdFR = new SqlCommand(calcFR, node);
                SqlCommand cmdCR = new SqlCommand(calcCR, node);
                countP = countP + int.Parse(cmdP.ExecuteScalar().ToString());
                countFR = countFR + int.Parse(cmdFR.ExecuteScalar().ToString());
                countCR = countCR + int.Parse(cmdCR.ExecuteScalar().ToString());
            }
            node.Close();
            counts[0] = countP;
            counts[1] = countFR;
            counts[2] = countCR;
            return counts;
        }

       public DataTable RepeatAnnotation(string RAT)
        {
        Int64 repeatLength = 0;
        DataTable table = new DataTable();
        SqlConnection con = new SqlConnection(_config.GetConnectionString("MASTER"));
            
            table.Columns.Add("Starting", typeof(Int64));
            table.Columns.Add("Ending", typeof(Int64));
            table.Columns.Add("Repetitive Sequences", typeof(string));
            table.Columns.Add("Repeat Count ", typeof(int));
            table.Columns.Add("Length ", typeof(int));
            SqlCommand command = new SqlCommand(String.Format("SELECT STARTING,ENDING, REPETITIVE_SEQUENCE, REPEAT_COUNT  FROM {0} GROUP BY STARTING,ENDING,REPETITIVE_SEQUENCE, REPEAT_COUNT having REPEAT_COUNT>4 ", RAT), con);
            con.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    DataRow dr = table.NewRow();
                    dr[0] = reader[0];
                    dr[1] = reader[1];
                    dr[2] = _Tools.unmask(reader[2].ToString());
                    dr[3] = reader[3];
                    dr[4] = reader[2].ToString().Length;
                    table.Rows.Add(dr);
                    repeatLength = repeatLength + (reader[2].ToString().Length* int.Parse(reader[3].ToString()));
                }

            }
            reader.Close();
            con.Close();
            RepeatSize = repeatLength;
            return table;

        }

        public string CountRepeat(string expid)
        {
            SqlConnection con = new SqlConnection(_config.GetConnectionString("MASTER"));
            SqlCommand command = new SqlCommand(String.Format("SELECT COUNT_BIG(DISTINCT REPETITIVE_SEQUENCE) FROM {0} ", "INITIAL_REPEAT_ANNOTATION" + expid), con);
            con.Open();
            string count = command.ExecuteScalar().ToString();
            con.Close();
            return count;
        }

        public string CountUniqueRepeat(string expid)
        {
            SqlConnection con = new SqlConnection(_config.GetConnectionString("MASTER"));
            SqlCommand command = new SqlCommand(String.Format("SELECT COUNT_BIG(DISTINCT REPETITIVE_SEQUENCE) FROM {0} ", "REPEAT_ANNOTATION" + expid), con);
            con.Open();
            string count = command.ExecuteScalar().ToString();
            con.Close();
            return count;
        }

    }
}
