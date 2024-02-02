using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SRGD.Models
{
    public class Alignment
    {
        private MasterDbContext _MasterDbContext;
        private ADbContext _ADbContext;
        private CDbContext _CDbContext;
        private GDbContext _GDbContext;
        private TDbContext _TDbContext;
        private IConfiguration _config;
        private Tools _T;
        public Alignment(MasterDbContext MASTERContext, ADbContext AContext, CDbContext CContext, GDbContext GContext, TDbContext TContext, IConfiguration config, Tools T)
        {
            _MasterDbContext = MASTERContext;
            _ADbContext = AContext;
            _CDbContext = CContext;
            _GDbContext = GContext;
            _TDbContext = TContext;
            _config = config;
            _T = T;

        }
        public void ParallelLoadContig(string expID, int PN, int SIML)
        {

            Parallel.Invoke(
             () => LoadContig(_ADbContext, expID, PN, SIML),
             () => LoadContig(_CDbContext, expID, PN, SIML),
             () => LoadContig(_GDbContext, expID, PN, SIML),
             () => LoadContig(_TDbContext, expID, PN, SIML)
               );

        }



        void LoadContig(DbContext db, string expID, int PN, int SIML)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_config.GetConnectionString("MASTER"));


            string[] tables = { "AA", "AC", "AG", "AT", "CA", "CC", "CG", "CT", "GA", "GC", "GG", "GT", "TA", "TC", "TG", "TT" };

            for (int i = 0; i < tables.Length; i++)
            {
                string TK = _T.mask(tables[i]);
                db.Database.ExecuteSqlRaw("DECLARE @TABLE AS VARCHAR(30)={0}; DECLARE @TK AS VARCHAR(30)={1}; DECLARE @PREAD AS VARCHAR(MAX)={2};  DECLARE @PREF AS VARCHAR(MAX)={3}; DECLARE @PT AS VARCHAR(MAX)={4};DECLARE @PTC AS VARCHAR(MAX)={5}; DECLARE @SIMILARTY NVARCHAR ={6}; DECLARE @MASTER AS NVARCHAR(100)={7};DECLARE @CTABLE AS VARCHAR(30)={8};DECLARE @KMERTABLE AS VARCHAR(30)={9};SET NOCOUNT ON; DECLARE @SQLSTRING  NVARCHAR(MAX) = ' INSERT INTO['+@MASTER+'].[GENOME].[DBO].['+@CTABLE+'] (ROW_ID, OFFSET, CONTIG, RI) SELECT ROW_ID, OFFSET, CONTIG, RI  FROM  (       SELECT ROW_ID, OFFSET, CONTIG, RI, KMER, DBO.SIMILARITY(CONTIG, KMER) AS SIMILARTY  FROM   (  SELECT R.ROW_ID AS ROW_ID, REF.OFFSET AS OFFSET, '+@PREAD+'  AS CONTIG, R.RI AS RI, REF.KMER AS KMER  FROM '+@TABLE+' AS R INNER JOIN (  SELECT OFFSET,SUBSTRING(P1,2,2) AS TK, HK, '+@PREF+' AS KMER, '+@PT+' FROM '+@KMERTABLE+' ) AS REF  ON R.HK=REF.HK AND REF.TK='+@TK+' AND '+@PTC+' )AS ALLIGN   )AS CONT   WHERE SIMILARTY >= '+@SIMILARTY+';'; EXEC(@SQLSTRING);", tables[i] + expID, TK, _T.PartitionsPlusOne(PN, "R."), _T.PartitionsPlusOne(PN, ""), _T.PatternPartitions(PN), _T.AlignmentPatternCheck(PN), SIML.ToString(), builder.DataSource, "CONTIGS" + expID, "KMERS" + expID);
                db.Database.ExecuteSqlRaw("DECLARE @TABLE AS VARCHAR(30)={0};DECLARE @MASTER AS NVARCHAR(100)={1};DECLARE @CTABLE AS VARCHAR(30)={2};DECLARE @SQLSTRING NVARCHAR(MAX)=' DELETE FROM '+@TABLE+' WHERE ROW_ID IN ( SELECT DISTINCT ROW_ID FROM ['+@MASTER+'].[GENOME].[DBO].['+@CTABLE+']);';EXEC(@SQLSTRING);", tables[i] + expID, builder.DataSource, "CONTIGS" + expID);
                db.Database.ExecuteSqlRaw("DECLARE @TABLE AS VARCHAR(30)={0};DECLARE @MASTER AS NVARCHAR(100)={1};DECLARE @PREAD AS VARCHAR(MAX)={2};DECLARE @MISASSEMBLYTABLE AS VARCHAR(30)={3};DECLARE @SQLSTRING NVARCHAR(MAX) = 'INSERT INTO [' + @MASTER + '].[GENOME].[DBO].[' + @MISASSEMBLYTABLE + ']  SELECT ROW_ID,' + @PREAD + ' AS CONTIG,RI FROM ' + @TABLE + ' AS R;'; EXEC(@SQLSTRING);", tables[i] + expID, builder.DataSource, _T.PartitionsPlusOne(PN, ""), "MISASSEMBLY" + expID);
            }
        }




        public void ContigOverlapping(string expID)
        {
            _MasterDbContext.Database.ExecuteSqlRaw("SET NOCOUNT ON;DECLARE @CTABLE VARCHAR(30) ={0}; DECLARE @SQLSTRING NVARCHAR(MAX) = 'DECLARE @START AS INT = (SELECT MIN(ID) FROM ' + @CTABLE + ');DECLARE @END AS INT = (SELECT MAX(ID) FROM ' + @CTABLE + ');WHILE @START<= @END BEGIN SELECT ID,ROW_ID,OFFSET AS SOFFSET,OFFSET+LEN(CONTIG) - 1 AS EOFFSET,CONTIG INTO #SX FROM ' + @CTABLE + ' WHERE ID=@START;DELETE FROM ' + @CTABLE + ' WHERE ID IN(SELECT PARTID FROM ( SELECT C1.ID AS IDSX, C2.ID AS PARTID,C2.CONTIG AS CCONTIG,C1.CONTIG AS ORIG,SUBSTRING(C1.CONTIG,C2.OFFSET-C1.SOFFSET+1,LEN(C2.CONTIG)) AS PART FROM #SX AS C1 INNER JOIN ' + @CTABLE + ' AS C2 ON  C1.SOFFSET<C2.OFFSET AND C1.EOFFSET>C2.OFFSET+LEN(C2.CONTIG) - 1 )S WHERE  IDSX<>PARTID AND CCONTIG=PART );SELECT * INTO #TEMP FROM (SELECT *,MAX(EOFFSETPX)OVER (PARTITION BY IDSX,SXID,SOFFSETSX,EOFFSETSX) AS MAXEOFFSETPX,LEFT(CONTIGSX,LEN(CONTIGSX)-LEN(SX))+CONTIGPX AS MCONTIG  FROM (	 SELECT C1.ID AS IDSX,  C1.ROW_ID AS SXID,  C1.SOFFSET AS SOFFSETSX, C1.EOFFSET AS EOFFSETSX,  C1.CONTIG AS CONTIGSX,  C2.ID AS IDPX, C2.ROW_ID AS PXID,  C2.OFFSET AS SOFFSETPX,  C2.OFFSET+LEN(C2.CONTIG) - 1 AS EOFFSETPX,  C2.CONTIG AS CONTIGPX,  SUBSTRING(C1.CONTIG,C2.OFFSET-C1.SOFFSET+1, LEN(C1.CONTIG)-LEN(C2.OFFSET-C1.SOFFSET+1))AS SX,LEFT(C2.CONTIG,LEN(SUBSTRING(C1.CONTIG,C2.OFFSET-C1.SOFFSET+1, LEN(C1.CONTIG)-LEN(C2.OFFSET-C1.SOFFSET+1)))) AS PX  FROM #SX AS C1 	INNER JOIN ' + @CTABLE + '  AS C2  ON C1.SOFFSET<C2.OFFSET AND     C1.EOFFSET<C2.OFFSET+LEN(C2.CONTIG) - 1 AND   C1.EOFFSET>C2.OFFSET AND C1.ROW_ID<> C2.ROW_ID  )S WHERE SX=PX )S2 WHERE MAXEOFFSETPX=EOFFSETPX;DECLARE @C INT=(SELECT  COUNT(IDSX)  FROM #TEMP);IF @C<>1 BEGIN DELETE FROM #TEMP;END DECLARE @D VARCHAR(MAX) = (SELECT DISTINCT MCONTIG FROM #TEMP);IF @D IS NOT NULL BEGIN UPDATE ' + @CTABLE + ' 	 SET     CONTIG = @D,OV=1    WHERE ID =(SELECT DISTINCT IDSX FROM #TEMP);DELETE FROM   ' + @CTABLE + ' WHERE ID IN (SELECT IDPX FROM #TEMP);  END DROP TABLE #TEMP;DROP TABLE #SX;SELECT @START=@START+1;END;'; EXEC(@SQLSTRING);", "CONTIGS" + expID);
            _MasterDbContext.Database.ExecuteSqlRaw("DECLARE @CTABLE VARCHAR(30) ={0}; DECLARE @SQLSTRING NVARCHAR(MAX) = 'DECLARE @START AS INT = (SELECT MIN(ID) FROM ' + @CTABLE + '); DECLARE @END AS INT = (SELECT MAX(ID) FROM ' + @CTABLE + '); WHILE @START<= @END BEGIN SELECT ID,ROW_ID,OFFSET AS SOFFSET,OFFSET+LEN(CONTIG) - 1 AS EOFFSET,CONTIG INTO #SX FROM ' + @CTABLE + ' WHERE ID=@START;DELETE FROM ' + @CTABLE + ' WHERE ID IN(SELECT PARTID FROM ( SELECT C1.ID AS IDSX, C2.ID AS PARTID,C2.CONTIG AS CCONTIG,C1.CONTIG AS ORIG, SUBSTRING(C1.CONTIG,C2.OFFSET-C1.SOFFSET+1,LEN(C2.CONTIG)) AS PART FROM #SX AS C1 INNER JOIN ' + @CTABLE + ' AS C2 ON  C1.SOFFSET<C2.OFFSET AND C1.EOFFSET>C2.OFFSET+LEN(C2.CONTIG) - 1 )S WHERE  IDSX<>PARTID );DROP TABLE #SX;SELECT @START=@START+1;END;'; EXEC(@SQLSTRING); ", "CONTIGS" + expID);
        }


        public void ScaffoldsBuilder(string expID)
        {
            _MasterDbContext.Database.ExecuteSqlRaw("DECLARE @CTABLE VARCHAR(30)={0};DECLARE @SQLSTRINGEXEC NVARCHAR(MAX)='DECLARE @S AS BIGINT=0;DECLARE @E AS BIGINT=1;WHILE @S<>@E BEGIN SELECT @S=(SELECT COUNT(ID) FROM '+@CTABLE+');EXEC SCAFFOLDBUILDER '+@CTABLE+' SELECT @E=(SELECT COUNT(ID) FROM '+@CTABLE+');END;';EXEC(@SQLSTRINGEXEC);", "CONTIGS" + expID);
        }
    }
}