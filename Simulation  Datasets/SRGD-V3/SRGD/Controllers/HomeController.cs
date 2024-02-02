
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SRGD.Models;
using SRGD.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SRGD.Controllers
{
    public class HomeController:Controller  
    {
        private IConfiguration _config;
        private MasterDbContext _MasterDbContext;
        private ADbContext _ADbContext;
        private CDbContext _CDbContext;
        private GDbContext _GDbContext;
        private TDbContext _TDbContext;
        private IWebHostEnvironment _env;
  
        public HomeController(MasterDbContext MASTERContext, ADbContext AContext, CDbContext CContext, GDbContext GContext, TDbContext TContext, IWebHostEnvironment env, IConfiguration config)
        {
            _config = config;
            _env = env;
            _MasterDbContext = MASTERContext;
            _ADbContext = AContext;
            _CDbContext = CContext;
            _GDbContext = GContext;
            _TDbContext = TContext;
          
        }

    

        public IActionResult Index()
        {
            
            if (!_MasterDbContext.Database.CanConnect())
           {

                // Initialize Master Server
                var optionsBuilder = new DbContextOptionsBuilder<MasterDbContext>();
                DbContext[] contexts = new DbContext[5];
                DbContext[] contextsG = new DbContext[5];
                string[] connectionStrings = { "MASTERM", "AM", "CM", "GM", "TM" };
               

                optionsBuilder.UseSqlServer(_config.GetConnectionString(connectionStrings[0]));
                MasterDbContext masterMContext = new MasterDbContext(optionsBuilder.Options);
                contexts[0] = masterMContext;
                contextsG[0] = _MasterDbContext;

                optionsBuilder.UseSqlServer(_config.GetConnectionString(connectionStrings[1]));
                MasterDbContext AMContext = new MasterDbContext(optionsBuilder.Options);
                contexts[1]= AMContext;
                contextsG[1] = _ADbContext;

                optionsBuilder.UseSqlServer(_config.GetConnectionString(connectionStrings[2]));
                MasterDbContext CMContext = new MasterDbContext(optionsBuilder.Options);
                contexts[2] = CMContext;
                contextsG[2] = _CDbContext;
                optionsBuilder.UseSqlServer(_config.GetConnectionString(connectionStrings[3]));
                MasterDbContext GMContext = new MasterDbContext(optionsBuilder.Options);
                contexts[3] = GMContext;
                contextsG[3] = _GDbContext;
                optionsBuilder.UseSqlServer(_config.GetConnectionString(connectionStrings[4]));
                MasterDbContext TMContext = new MasterDbContext(optionsBuilder.Options);
                contexts[4] = TMContext;
                contextsG[4] = _TDbContext;



                int i = 0;
                 Dictionary<string, string[]> Nodes = new Dictionary<string, string[]>();
                 Nodes.Add("MASTER", new string[] { "AM", "CM", "GM", "TM" });
                 Nodes.Add("A", new string[] { "MASTERM", "CM", "GM", "TM" });
                 Nodes.Add("C", new string[] { "AM", "MASTERM", "GM", "TM" });
                 Nodes.Add("G", new string[] { "AM", "CM", "MASTERM", "TM" });
                 Nodes.Add("T", new string[] { "AM", "CM", "GM", "MASTERM" });
                
                 foreach (var item in Nodes)
                 {
                     for (int x=0;x<item.Value.Length;x++)
                     {
                         SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_config.GetConnectionString(item.Value[x]));

                        contexts[i].Database.ExecuteSqlRaw("IF NOT EXISTS(SELECT * FROM SYS.SERVERS WHERE NAME = {0})EXEC sp_addlinkedserver @server = {0},@srvproduct = N'SQL SERVER';IF(db_id(N'GENOME') IS NULL) CREATE DATABASE GENOME;ALTER DATABASE GENOME SET RECOVERY BULK_LOGGED;", builder.DataSource);
                        if (i == 0)
                            continue;
                        contextsG[i].Database.ExecuteSqlRaw("DROP PROCEDURE IF EXISTS dbo.SETFER;DROP PROCEDURE IF EXISTS dbo.SETFCR;DROP FUNCTION IF EXISTS dbo.SIMILARITY;");
                        contextsG[i].Database.ExecuteSqlRaw("CREATE PROCEDURE SETFER @TABLE VARCHAR(30),@P NVARCHAR(MAX) AS BEGIN SET NOCOUNT ON;DECLARE @SQLSTRING1 NVARCHAR(MAX) = 'SELECT ROW_ID, COUNT(HK) OVER(PARTITION BY '+@P+'HK) AS FER,MAX(ROW_ID) OVER (PARTITION BY ' +@P+' HK) AS MRID FROM ' + @TABLE ; CREATE TABLE #TABLEVAR (ROW_ID BIGINT,FER INT,MRID VARCHAR(100));INSERT INTO #TABLEVAR(ROW_ID,FER,MRID) EXEC(@SQLSTRING1); DECLARE @SQLSTRING2 NVARCHAR(MAX) = 'MERGE INTO ' + @TABLE + ' AS T USING #TABLEVAR  AS S ON T.ROW_ID = S.ROW_ID WHEN MATCHED  THEN UPDATE SET T.FER = S.FER ; '  DECLARE @SQLSTRING3 NVARCHAR(MAX) = 'MERGE INTO ' + @TABLE + ' AS T USING #TABLEVAR  AS S ON T.ROW_ID = S.ROW_ID WHEN MATCHED AND  T.FER > 1 AND T.ROW_ID<> S.MRID THEN DELETE ; ';  EXEC(@SQLSTRING2); EXEC(@SQLSTRING3);END;");
                        contextsG[i].Database.ExecuteSqlRaw("CREATE PROCEDURE SETFCR @TABLE VARCHAR(30), @PN INT AS BEGIN SET NOCOUNT ON;DECLARE @C INT=1;DECLARE @MP NVARCHAR(MAX);WHILE @C<=@PN BEGIN  DECLARE @CN NVARCHAR(2)=CAST(@C AS NVARCHAR(2))SET @MP='LEFT(T.P'+@CN+',LEN(S.P'+@CN+'))=S.P'+@CN+' AND ';DECLARE @SP NVARCHAR(MAX)='T.ROW_ID<>S.ROW_ID AND ';    DECLARE @I INT=@C;	WHILE @I<=@PN-1    BEGIN	SET @SP=@SP+' S.P'+CAST(@I+1 AS NVARCHAR(MAX))+' IS NULL AND ';	SELECT @I=@I+1;	END;DECLARE @ADD1 NVARCHAR(MAX)='';    DECLARE @J INT=1;	DECLARE @K INT=@C-1;	WHILE @J<=@K    BEGIN	SET @ADD1=@ADD1+' T.P'+CAST(@J AS NVARCHAR(MAX))+'= S.P'+CAST(@J AS NVARCHAR(MAX))+' AND ';	SELECT @J=@J+1;	END;DECLARE @ADD2 NVARCHAR(MAX)='';    DECLARE @L INT=1;	DECLARE @M INT=@C;	WHILE @L<=@M    BEGIN	SET @ADD2=@ADD2+' T.P'+CAST(@L AS NVARCHAR(MAX))+'= S.P'+CAST(@L AS NVARCHAR(MAX))+' AND ';	SELECT @L=@L+1;	END;DECLARE @SQLSTRING1 NVARCHAR(MAX) = ' SELECT T.ROW_ID AS TROW , S.ROW_ID AS SROW, COUNT (S.ROW_ID) OVER (PARTITION BY T.ROW_ID) AS FCR INTO #TEMP  FROM ' + @TABLE + ' AS T INNER JOIN ' + @TABLE + ' AS S  ON '+ @ADD1+@MP+LEFT(@SP,LEN(@SP)-3)+'; MERGE INTO ' + @TABLE + ' AS T USING  (SELECT DISTINCT TROW,FCR FROM  #TEMP )AS S  ON T.ROW_ID=S.TROW WHEN MATCHED THEN UPDATE SET T.FCR=T.FCR+S.FCR; DELETE FROM ' + @TABLE + ' WHERE ROW_ID IN ( SELECT DISTINCT SROW FROM #TEMP); DROP TABLE #TEMP;'; EXEC (@SQLSTRING1);IF(@C<>@PN) BEGIN DECLARE @SQLSTRING2 NVARCHAR(MAX) = 'SELECT T.ROW_ID AS TROW , S.ROW_ID AS SROW,COUNT (S.ROW_ID) OVER (PARTITION BY T.ROW_ID) AS FCR INTO #TEMP FROM ' + @TABLE + ' AS T INNER JOIN ' + @TABLE + ' AS S  ON '+@ADD2+LEFT(@SP,LEN(@SP)-3)+';  MERGE INTO ' + @TABLE + ' AS T USING  (SELECT DISTINCT TROW,FCR FROM  #TEMP )AS S ON T.ROW_ID=S.TROW WHEN MATCHED THEN UPDATE SET T.FCR=T.FCR+S.FCR;  DELETE FROM ' + @TABLE + ' WHERE ROW_ID IN ( SELECT DISTINCT SROW FROM #TEMP);   DROP TABLE #TEMP; ';EXEC (@SQLSTRING2);END SELECT @C=@C+1;END;END;");
                        contextsG[i].Database.ExecuteSqlRaw("CREATE FUNCTION SIMILARITY(@S1 VARCHAR(MAX), @S2 VARCHAR(MAX))RETURNS INT AS BEGIN DECLARE @MATCH INT = 0 DECLARE @COUNTER INT = 1 DECLARE @END INT = LEN(@S1) WHILE @COUNTER<= @END BEGIN IF(SUBSTRING(@S1, @COUNTER,1)= SUBSTRING(@S2, @COUNTER, 1))SELECT @MATCH = @MATCH + 1 ELSE SELECT @MATCH = @MATCH + 0; SELECT @COUNTER = @COUNTER + 1; END; RETURN CAST(@MATCH AS FLOAT)/ CAST(@END AS FLOAT) * 100 END;");
                    }
                    i++;
                 }

                ViewBag.ClusterInitialized = "Cluster Initialized ";
                _MasterDbContext.Dispose();
                var DBInitialization = "CREATE TABLE USERS(  USERNAME VARCHAR(50) PRIMARY KEY,  PASSWORD VARCHAR(30) NOT NULL,   EMAIL VARCHAR(100) NOT NULL, JOB VARCHAR(50) NOT NULL, FOLDERPATH VARCHAR(300)	NOT NULL	); CREATE TABLE EXPERIMENTS(EXPERIMENTID INT PRIMARY KEY, EXPERIMENTDATE DATE NOT NULL, EXPERIMENTTIME TIME NOT NULL, EXPERIMENTFOLDERPATH VARCHAR(300) NOT NULL, SPECIES VARCHAR(100) NOT NULL, USERNAME VARCHAR(50)  FOREIGN KEY REFERENCES USERS(USERNAME)  NOT NULL);";
               _MasterDbContext.Database.ExecuteSqlRaw(DBInitialization);
               _MasterDbContext.Database.ExecuteSqlRaw("CREATE FUNCTION DBO.FINDPATTERNLOCATION (    @string NVARCHAR(MAX),    @term   NVARCHAR(MAX)) RETURNS TABLE AS    RETURN     (      SELECT pos = Number - LEN(@term) 	  FROM (	        SELECT Number, Item = LTRIM(RTRIM(SUBSTRING(@string, Number,       CHARINDEX(@term, @string + @term, Number) - Number)))      FROM (SELECT ROW_NUMBER() OVER (ORDER BY [object_id])     FROM sys.all_columns) AS n(Number)      WHERE Number > 1 AND Number <= CONVERT(INT, LEN(@string)+1)      AND SUBSTRING(@term + @string, Number, LEN(@term)) = @term    ) AS y);");
               _MasterDbContext.Database.ExecuteSqlRaw("CREATE PROCEDURE SCAFFOLDBUILDER  @CTABLE VARCHAR(30) AS BEGIN   DECLARE @SQLSTRING NVARCHAR(MAX)='   DECLARE @START AS INT = (SELECT MIN(ID) FROM '+@CTABLE+');    DECLARE @END AS INT = (SELECT MAX(ID) FROM '+@CTABLE+');   WHILE @START<= @END    BEGIN    SELECT ID,ROW_ID,OFFSET AS SOFFSET,   OFFSET+LEN(CONTIG) - 1 AS EOFFSET,   CONTIG    INTO #SX    FROM '+@CTABLE+'   WHERE ID=@START;      SELECT   C2.ID AS IDPX,   C2.CONTIG AS CONTIGPX,  	  SUBSTRING(C1.CONTIG,C2.OFFSET-C1.SOFFSET+1, LEN(C2.CONTIG)) AS PART INTO #TEMP1  FROM #SX AS C1 		  INNER JOIN '+@CTABLE+'  AS C2  	  ON C1.SOFFSET<C2.OFFSET AND     	  C1.EOFFSET>C2.OFFSET+LEN(C2.CONTIG) - 1 ; 	   DELETE FROM   '+@CTABLE+' WHERE ID IN (SELECT IDPX FROM #TEMP1);   SELECT * INTO #TEMP2 FROM    (SELECT *,   MAX(EOFFSETPX)OVER    (PARTITION BY IDSX,SXID,SOFFSETSX,EOFFSETSX) AS MAXEOFFSETPX,   LEFT(CONTIGSX,LEN(CONTIGSX)-LEN(SX))+CONTIGPX AS MCONTIG     FROM (	    SELECT C1.ID AS IDSX,  C1.ROW_ID AS SXID,    C1.SOFFSET AS SOFFSETSX, C1.EOFFSET AS EOFFSETSX,    C1.CONTIG AS CONTIGSX,  C2.ID AS IDPX,    C2.ROW_ID AS PXID,  C2.OFFSET AS SOFFSETPX, 	 C2.OFFSET+LEN(C2.CONTIG) - 1 AS EOFFSETPX,  	 C2.CONTIG AS CONTIGPX,  SUBSTRING(C1.CONTIG,C2.OFFSET-C1.SOFFSET+1,	  LEN(C1.CONTIG)-LEN(C2.OFFSET-C1.SOFFSET+1))AS SX,	  LEFT(C2.CONTIG,LEN(SUBSTRING(C1.CONTIG,C2.OFFSET-C1.SOFFSET+1, LEN(C1.CONTIG)-LEN(C2.OFFSET-C1.SOFFSET+1)))) AS PX  	  FROM #SX AS C1 		  INNER JOIN '+@CTABLE+'  AS C2  	  ON C1.SOFFSET<C2.OFFSET AND     	  C1.EOFFSET<C2.OFFSET+LEN(C2.CONTIG) - 1 AND  	   C1.EOFFSET>C2.OFFSET AND C1.ROW_ID<> C2.ROW_ID  	   )S 	   WHERE SX=PX 	   )S2 WHERE MAXEOFFSETPX=EOFFSETPX;	   DECLARE @C VARCHAR(MAX)=(SELECT DISTINCT  MCONTIG FROM #TEMP2);	   IF(@C IS NOT NULL) BEGIN UPDATE     '+@CTABLE+' 	 SET     CONTIG = @C   WHERE ID=(SELECT ID FROM #SX); 	   DELETE FROM   '+@CTABLE+' WHERE ID IN (SELECT IDPX FROM #TEMP2  )  END 	   DROP TABLE #TEMP1;   DROP TABLE #TEMP2;	   DROP TABLE #SX;	   SELECT @START=@START+1;	   END;'; EXEC(@SQLSTRING)END;");
               //_MasterDbContext.Database.ExecuteSqlRaw("CREATE PROCEDURE REPEATCONSTRUCT @RT VARCHAR(30)  AS BEGIN SET NOCOUNT ON;DECLARE @SQLSTRING NVARCHAR(MAX) = 'DECLARE @START BIGINT=(SELECT MAX(LEN(REPETITIVE_SEQUENCE)) FROM ' + @RT + ' ); DECLARE @END BIGINT=3;WHILE @START>@END BEGIN SELECT ID,REPETITIVE_SEQUENCE INTO #TEMP1 FROM ' + @RT + ';SELECT ID,REPETITIVE_SEQUENCE INTO #TEMP2 FROM ' + @RT + ';  INSERT INTO ' + @RT + ' (REPETITIVE_SEQUENCE)   SELECT MERGED FROM      ' + @RT + '   R INNER JOIN (	SELECT T1.ID AS SXID,T2.ID AS PXID,T1.REPETITIVE_SEQUENCE AS SX,T2.REPETITIVE_SEQUENCE AS PX,LEFT( T1.REPETITIVE_SEQUENCE,LEN(T1.REPETITIVE_SEQUENCE)-@START)+T2.REPETITIVE_SEQUENCE AS MERGED	FROM #TEMP1 AS T1 INNER JOIN #TEMP2 AS T2	ON RIGHT(T1.REPETITIVE_SEQUENCE,@START)=LEFT(T2.REPETITIVE_SEQUENCE,@START)	WHERE T1.ID<>T2.ID ) S ON S.SXID=R.ID;UPDATE ' + @RT + ' SET OV=1 WHERE ID IN (  SELECT SXID AS ID FROM      ' + @RT + '   R INNER JOIN  (	SELECT T1.ID AS SXID,T2.ID AS PXID		FROM #TEMP1 AS T1 INNER JOIN #TEMP2 AS T2	ON RIGHT(T1.REPETITIVE_SEQUENCE,@START)=LEFT(T2.REPETITIVE_SEQUENCE,@START)	WHERE T1.ID<>T2.ID ) S ON S.SXID=R.ID UNION    SELECT PXID FROM     ' + @RT + '   R INNER JOIN   (	SELECT T1.ID AS SXID,T2.ID AS PXID	FROM #TEMP1 AS T1 INNER JOIN #TEMP2 AS T2	ON RIGHT(T1.REPETITIVE_SEQUENCE,@START)=LEFT(T2.REPETITIVE_SEQUENCE,@START)	WHERE T1.ID<>T2.ID ) S ON S.SXID=R.ID ); DELETE FROM ' + @RT + ' WHERE OV=1;DROP TABLE #TEMP1;DROP TABLE #TEMP2;SELECT @START=@START-1; END;'; EXEC(@SQLSTRING); END;");
              // _MasterDbContext.Database.ExecuteSqlRaw("CREATE PROCEDURE REPEAT_ANNOTATION @CTABLE VARCHAR(30),@RT VARCHAR(30) ,@RAT VARCHAR(30) AS BEGIN SET NOCOUNT ON; DECLARE @SQLSTRING NVARCHAR(MAX) = 'SELECT  DISTINCT C.OFFSET AS STARTING,C.OFFSET+LEN(C.CONTIG)-1 AS ENDING,R.REPETITIVE_SEQUENCE AS RP , C.CONTIG AS CONTIG INTO #TEMP1 FROM '+@CTABLE+' AS C INNER JOIN  '+@RT+' AS R ON CHARINDEX(R.REPETITIVE_SEQUENCE, C.CONTIG)>0;SELECT * INTO #TEMP2 FROM #TEMP1  CROSS  APPLY dbo.FindPatternLocation(CONTIG, RP); DROP TABLE #TEMP1 ;INSERT INTO '+@RAT+' SELECT STARTING,ENDING,RP,STARTING+POS-1 AS  SPOSITION, STARTING+POS+LEN(RP)-2 AS EPOSITION,COUNT(RP) OVER (PARTITION BY RP,CONTIG) AS C FROM #TEMP2;DROP TABLE #TEMP2; ';EXEC(@SQLSTRING); END;");
            }
            return View();
        }

        public IActionResult Register()
        {
          
            return View();
        }

        [HttpPost]
        public IActionResult Register(Users user)
        {
            string path = _env.WebRootPath + "/data/" + user.Username;

            user.FolderPath = path;
           
           
            if (ModelState.IsValid)
            {
                Directory.CreateDirectory(path);
                _MasterDbContext.users.Add(user);
                _MasterDbContext.SaveChanges();
          
                return RedirectToAction("Index");
            }

            return View(user);

        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel login)
        {
  
            if (ModelState.IsValid)
            {
                var Credential = _MasterDbContext.users.Find(login.Username);
                if (Credential is null)
                {
                    ViewData["error"] = "Invalid User";
                    return View();
                }
                else 
                {
                    if (Credential.Password == login.Password && login.Password != null)
                    {
                        return RedirectToAction("Index", "User", Credential);
                    }
                    else
                    {
                        ViewData["error"] = "Invalid Password";
                        return View();
                    }

                }


               
                    
               
            

            }
            return View();

        }

    }
}
