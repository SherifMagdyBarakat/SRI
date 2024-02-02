using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SRGD.Models
{
    public class Overlapping
    {
        private IConfiguration _config;
        private MasterDbContext _MasterDbContext;
        private ADbContext _ADbContext;
        private CDbContext _CDbContext;
        private GDbContext _GDbContext;
        private TDbContext _TDbContext; 
        private Tools _T;
        string[] tables = { "AA", "AC", "AG", "AT", "CA", "CC", "CG", "CT", "GA", "GC", "GG", "GT", "TA", "TC", "TG", "TT" };

        public Overlapping(MasterDbContext MASTERContext, ADbContext AContext, CDbContext CContext, GDbContext GContext, TDbContext TContext, IConfiguration config,Tools T)
        {
            _MasterDbContext = MASTERContext;
            _ADbContext = AContext;
            _CDbContext = CContext;
            _GDbContext = GContext;
            _TDbContext = TContext;
            _config = config;
            _T = T;

        }

        public void ParallelLoadSXPX(string expID, int PN,int OVL ,int RI)
        {
            string Partitions = _T.PartitionsPlusOne(PN,null);

            Parallel.Invoke(
            () => LoadSXPX(_ADbContext,"1", expID, Partitions,OVL,RI),
            () => LoadSXPX(_CDbContext,"2", expID, Partitions, OVL, RI),
            () => LoadSXPX(_GDbContext, "3",expID, Partitions, OVL, RI),
            () => LoadSXPX(_TDbContext,"4", expID, Partitions, OVL, RI)
               );


        }


        void LoadSXPX(DbContext db,string node, string expID,string P,int OVL, int RI)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_config.GetConnectionString("MASTER").ToString());

            for (int i = 0; i < tables.Length; i++)
            {
                string T = _T.mask(tables[i]);
                db.Database.ExecuteSqlRaw("DECLARE @MASTER NVARCHAR(300)={0};DECLARE @expID NVARCHAR(300)={1};DECLARE @START NVARCHAR(10) = {2}; DECLARE @N VARCHAR(1) ={3};DECLARE @TABLE VARCHAR(30) = {4}; DECLARE @T VARCHAR(2) = {5}; DECLARE @P NVARCHAR(MAX) = {6};DECLARE @RI VARCHAR(30)={7}; DECLARE @SXSQLSTRING NVARCHAR(MAX) = 'INSERT INTO ['+@MASTER +'].[GENOME].[DBO].[SX'+@expID+']  SELECT  SUBSTRING(SX,1,1) AS NK,SUBSTRING(SX,2,2) AS TK,SUBSTRING(SX,4,3) AS HK,ROW_ID,R,SX,RI FROM (SELECT  ROW_ID,' +@P+ ' AS R, RIGHT('+@P+', ' + @START + ') AS SX, RI FROM ' + @TABLE + ' WHERE RI IN('+@RI+'))S;';  DECLARE @PXSQLSTRING NVARCHAR(MAX) = ' INSERT INTO ['+@MASTER +'].[GENOME].[DBO].[PX'+@expID+']   SELECT NK,TK,HK,ROW_ID,R,PX,RIGHT(R,LEN(R)-LEN(PX)) AS PXLP,RI FROM ( SELECT  '+@N+' AS NK,'+@T+' AS TK ,HK,ROW_ID,'+@P+' AS R,LEFT('+@P+','+@START+') AS PX,RI FROM '+@TABLE+' WHERE RI IN('+@RI+'))S;'  EXEC(@SXSQLSTRING);EXEC(@PXSQLSTRING);", builder.DataSource, expID,OVL,node,tables[i]+expID, T, P,RI);
            }

        }


        public void ParallelOverlapping(string expID, int PN, int OVL,int RI, int C)
        {
            int LP = PN + 1;
            _MasterDbContext.Database.ExecuteSqlRaw("DECLARE @OVL VARCHAR(30) ={0}; DECLARE @OVTM VARCHAR(30) ={1}; DECLARE @OVT VARCHAR(30) ={2}; DECLARE @SXT VARCHAR(30) ={3}; DECLARE @PXT VARCHAR(30) ={4};DECLARE @RI VARCHAR(5) ={5};DECLARE @ID VARCHAR(5) ={6}; DECLARE @SQLSTRING NVARCHAR(MAX) = 'DECLARE @ORGALG BIGINT=(SELECT COUNT_BIG(SX) FROM ' + @SXT + '); INSERT INTO ' + @OVTM + ' (ID,OVERLAPPING_LENGTH,ORIGINAL_ALGORITHM,COUNT_HIT,RI) SELECT '+@ID+' AS ID, '+@OVL+' AS OVERLAPPING_LENGTH,COALESCE(@ORGALG,0)*COALESCE(@ORGALG,0) AS ORIGINAL_ALGORITHM, COUNT(SX.ROW_ID) AS COUNT_HIT,'+@RI+' FROM ' + @SXT + ' AS SX  INNER JOIN ' + @PXT + ' AS PX ON SX.NK=PX.NK AND SX.TK=PX.TK AND SX.HK=PX.HK AND SX.ROW_ID<>PX.ROW_ID ;'; EXEC(@SQLSTRING);", OVL, "OVERLAPPINGMETRICS" + expID, "OVERLAPPING" + expID, "SX" + expID, "PX" + expID, RI,C);
            _MasterDbContext.Database.ExecuteSqlRaw("DECLARE @OVT VARCHAR(30) ={0}; DECLARE @SXT VARCHAR(30) ={1}; DECLARE @PXT VARCHAR(30) ={2};DECLARE @SQLSTRING NVARCHAR(MAX) = 'SELECT ROW_NUMBER() OVER(ORDER BY SX.ROW_ID) AS ID,LEFT(SX.R,1) AS SXNK,LEFT(PX.R,1) AS PXNK,SX.ROW_ID AS SXID,PX.ROW_ID AS PXID,PX.PXLP AS PXLP,COUNT(PX.ROW_ID) OVER(PARTITION BY SX.ROW_ID) AS CSX,COUNT(PX.ROW_ID) OVER(PARTITION BY PX.ROW_ID) AS CPX INTO #TEMP FROM ' + @SXT + ' AS SX  INNER JOIN ' + @PXT + ' AS PX ON SX.NK=PX.NK AND SX.TK=PX.TK AND SX.HK = PX.HK AND SX.SX = PX.PX AND SX.ROW_ID<>PX.ROW_ID ;DECLARE @START INT=1,@END INT=(SELECT COUNT(ID) FROM #TEMP);WHILE @START<=@END BEGIN INSERT INTO ' + @OVT + ' SELECT SXNK,PXNK,SXID,PXID,PXLP FROM #TEMP  WHERE  ID =@START AND  SXID NOT IN(SELECT PXID FROM ' + @OVT + ') AND  PXID NOT IN(SELECT SXID FROM ' + @OVT + ') AND CSX=1 AND CPX=1 ;  SELECT @START=@START+1; END;'; EXEC(@SQLSTRING);",   "OVERLAPPING" + expID, "SX" + expID, "PX" + expID);
            _MasterDbContext.Database.ExecuteSqlRaw("DECLARE @OVL VARCHAR(30) ={0};DECLARE @RI VARCHAR(30) ={1}; DECLARE @OVTM VARCHAR(30) ={2}; DECLARE @OVT VARCHAR(30) ={3};DECLARE @ID VARCHAR(10) ={4}; DECLARE @SQLSTRING NVARCHAR(MAX) = 'DECLARE @COUNTMATCH BIGINT=(SELECT COUNT_BIG(SXID) AS C   FROM ' + @OVT + '); UPDATE '+@OVTM+' SET COUNT_MATCH=COALESCE(@COUNTMATCH,0) WHERE ID='+@ID+';'; EXEC(@SQLSTRING);", OVL,RI, "OVERLAPPINGMETRICS" + expID, "OVERLAPPING" + expID,C);

               Parallel.Invoke(
            () => UpdateSXDeletePX(_ADbContext, "1", expID,  OVL,LP),
            () => UpdateSXDeletePX(_CDbContext, "2", expID,  OVL,LP),
            () => UpdateSXDeletePX(_GDbContext, "3", expID,  OVL,LP),
            () => UpdateSXDeletePX(_TDbContext, "4", expID,  OVL,LP)
               );


        }


        void UpdateSXDeletePX(DbContext db, string node, string expID,  int OVL, int LP)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_config.GetConnectionString("MASTER").ToString());

            db.Database.ExecuteSqlRaw("DECLARE @MASTER VARCHAR(30) ={0}; DECLARE @OV VARCHAR(30) ={1}; DECLARE @OVT VARCHAR(30) ={2}; DECLARE @N VARCHAR(1) ={3}; DECLARE @SQLSTRING1 NVARCHAR(MAX) = 'CREATE TABLE ' + @OV + ' (SXID BIGINT,PXID  BIGINT,PXLP VARCHAR(8000));'; DECLARE @SQLSTRING2 NVARCHAR(MAX) = 'INSERT INTO ' + @OV + ' SELECT SXID, PXID, PXLP FROM(SELECT SXID, PXID, PXLP  FROM [' + @MASTER + '].[GENOME].[dbo].[' + @OVT + '] WHERE  SXNK = ' + @N + '  UNION SELECT SXID, PXID, PXLP  FROM [' + @MASTER + '].[GENOME].[dbo].[' + @OVT + '] WHERE  PXNK =' + @N + ')S;'; SET NOCOUNT ON; EXEC(@SQLSTRING1); EXEC(@SQLSTRING2); ", builder.DataSource, "OV" + expID, "OVERLAPPING" + expID, node);

            string[] tables = { "AA", "AC", "AG", "AT", "CA", "CC", "CG", "CT", "GA", "GC", "GG", "GT", "TA", "TC", "TG", "TT" };

            for (int i = 0; i < tables.Length; i++)
            {
                db.Database.ExecuteSqlRaw(" DECLARE @TABLE VARCHAR(30) ={0}; DECLARE @OV VARCHAR(30) ={1};DECLARE @LP VARCHAR(30) ={2}; DECLARE @SQLSTRING1 NVARCHAR(MAX) = 'MERGE INTO ' + @TABLE + ' AS T USING (SELECT  SXID,PXID,PXLP FROM ' + @OV + ' )  AS S ON T.ROW_ID=S.SXID WHEN MATCHED  THEN UPDATE SET T.P' + @LP + '=COALESCE(T.P' + @LP + ','''')+COALESCE(S.PXLP,'''') ;'; DECLARE @SQLSTRING2 NVARCHAR(MAX) = 'DELETE FROM  ' + @TABLE + ' WHERE  ROW_ID IN (SELECT PXID FROM ' + @OV + ');'SET NOCOUNT ON; EXEC(@SQLSTRING1); EXEC(@SQLSTRING2);", tables[i] + expID, "OV" + expID, LP);
            }

            db.Database.ExecuteSqlRaw(" DECLARE @OV VARCHAR(30) ={0};  DECLARE @SQLSTRING NVARCHAR(MAX) = 'DROP TABLE ' + @OV + ';'; EXEC(@SQLSTRING); ", "OV" + expID);

        }



    }
}
