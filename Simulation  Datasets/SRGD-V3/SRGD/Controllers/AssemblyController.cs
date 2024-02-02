using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SRGD.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.xml;
using iTextSharp.text.html.simpleparser;
using System.Xml;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Data.SqlClient;
using System.IO.Compression;
using Microsoft.AspNetCore.Hosting;
using System.Threading;

namespace SRGD.Controllers
{
    [DisableRequestSizeLimit]
    public class AssemblyController : Controller
    {
        private MasterDbContext _MasterDbContext;
        private ADbContext _ADbContext;
        private CDbContext _CDbContext;
        private GDbContext _GDbContext;
        private TDbContext _TDbContext;
        private Indexer _Indexer;
        private RI _RI;
        private Overlapping _OV;
        private Alignment _Alignment;
        private Assembly _Assembly;
        private Reports _Report;
        DataTable dt = new DataTable();
        private IWebHostEnvironment _env;

        public AssemblyController(MasterDbContext MASTERContext, ADbContext AContext, CDbContext CContext, GDbContext GContext, TDbContext TContext, Indexer Indexer, RI RI, Overlapping OV, Alignment Alignment, Assembly Assembly, Reports Report, IWebHostEnvironment env)
        {
            _MasterDbContext = MASTERContext;
            _ADbContext = AContext;
            _CDbContext = CContext;
            _GDbContext = GContext;
            _TDbContext = TContext;
            _Indexer = Indexer;
            _RI = RI;
            _OV = OV;
            _Alignment = Alignment;
            _Assembly = Assembly;
            _Report = Report;
            _env = env;

        }

        [HttpGet]
        public IActionResult Index(Experiments exp)
        {

            return View(exp);

        }
        public IActionResult Report(Reports rep)
        {

            return View(rep);

        }
        [HttpPost]
        public IActionResult Upload(Experiments exp)
        {

            //string AssemblystartTime = DateTime.Now.ToString();
            Reports Report = new Reports();
            //File Splitter
            _Indexer.FileSplitter(exp.ExperimentID, exp.Reads, exp.Reference, exp.ExperimentFolderPath, exp.ReadsType, exp.PN);


            //CREATE EXP TABLES:
            _MasterDbContext.Database.ExecuteSqlRaw("DECLARE @T VARCHAR(30)={0};DECLARE @CREATE NVARCHAR(MAX) = 'CREATE TABLE ' + @T + '(N VARCHAR(1),T VARCHAR(30),ROW_ID BIGINT,PARTITION VARCHAR(8000),FREQUENCY BIGINT )';EXEC(@CREATE) ; ", "PARTITIONS" + exp.ExperimentID);
            _MasterDbContext.Database.ExecuteSqlRaw(" DECLARE @SXTABLE VARCHAR(30) ={0};DECLARE @PXTABLE VARCHAR(30) ={1};DECLARE @OVTABLEM VARCHAR(30) ={2};DECLARE @OVTABLE VARCHAR(30) ={3};DECLARE @CTABLE AS VARCHAR(30)= {4};DECLARE @MISASSEMBLYTABLE VARCHAR(30)={5};DECLARE @REPETITIVER VARCHAR(30)={6};DECLARE @INITIALRANNOTATION VARCHAR(30)={7};DECLARE @RANNOTATION VARCHAR(30)={8};DECLARE @NMETRICS VARCHAR(30)={9}; DECLARE @SQLSTRINGSX NVARCHAR(MAX) = 'CREATE TABLE ' + @SXTABLE + '(NK INT,TK INT, HK INT,ROW_ID BIGINT, R VARCHAR(MAX),SX VARCHAR(MAX), RI INT)'; DECLARE @SQLSTRINGPX NVARCHAR(MAX) = 'CREATE TABLE ' + @PXTABLE + '(NK INT,TK INT, HK INT,ROW_ID BIGINT, R VARCHAR(MAX),PX VARCHAR(MAX),PXLP VARCHAR(MAX), RI INT)';DECLARE @SQLSTRINGOVM NVARCHAR(MAX) = 'CREATE TABLE ' + @OVTABLEM + ' (ID INT PRIMARY KEY,OVERLAPPING_LENGTH INT,ORIGINAL_ALGORITHM BIGINT, COUNT_HIT BIGINT, COUNT_MATCH BIGINT,RI INT)'; DECLARE @SQLSTRINGOV NVARCHAR(MAX) = 'CREATE TABLE '+ @OVTABLE + ' (SXNK VARCHAR(1), PXNK VARCHAR(1), SXID BIGINT, PXID BIGINT, PXLP VARCHAR(MAX));';DECLARE @SQLSTRINGCONTIG NVARCHAR(MAX) = 'CREATE TABLE ' + @CTABLE + ' (ID BIGINT PRIMARY KEY IDENTITY(1,1),ROW_ID BIGINT, OFFSET BIGINT, CONTIG VARCHAR(MAX), RI INT,OV INT DEFAULT(0))';DECLARE @SQLSTRINGMISASSEMBLY NVARCHAR(MAX) ='CREATE TABLE '+@MISASSEMBLYTABLE+' (ROW_ID BIGINT,CONTIG VARCHAR(MAX),RI INT);'; DECLARE @SQLSTRINGREPEAT NVARCHAR(MAX)='CREATE TABLE '+@REPETITIVER+'(ID BIGINT PRIMARY KEY IDENTITY(1,1), REPETITIVE_SEQUENCE  VARCHAR(MAX),OV INT DEFAULT 0 );CREATE TABLE '+@INITIALRANNOTATION+' (ID BIGINT PRIMARY KEY IDENTITY(1,1), STARTING BIGINT,ENDING BIGINT,REPETITIVE_SEQUENCE  VARCHAR(MAX),SPOSITION BIGINT,EPOSITION BIGINT,REPEAT_COUNT BIGINT);CREATE TABLE '+@RANNOTATION+' (STARTING BIGINT,ENDING BIGINT,REPETITIVE_SEQUENCE  VARCHAR(MAX),SPOSITION BIGINT,EPOSITION BIGINT,REPEAT_COUNT BIGINT);';DECLARE @SQLSTRINGREPORTTABLE NVARCHAR(MAX)='CREATE TABLE '+@NMETRICS+' (N50C VARCHAR(30),N90C VARCHAR(30),N50S VARCHAR(30),N90S VARCHAR(30),ExperimentID INT FOREIGN KEY REFERENCES EXPERIMENTS(ExperimentID)  NOT NULL);'; EXEC(@SQLSTRINGSX);EXEC(@SQLSTRINGPX);EXEC(@SQLSTRINGOVM);EXEC(@SQLSTRINGOV);EXEC(@SQLSTRINGCONTIG);EXEC(@SQLSTRINGMISASSEMBLY);EXEC(@SQLSTRINGREPEAT);EXEC(@SQLSTRINGREPORTTABLE);", "SX" + exp.ExperimentID, "PX" + exp.ExperimentID, "OVERLAPPINGMETRICS" + exp.ExperimentID, "OVERLAPPING" + exp.ExperimentID, "CONTIGS" + exp.ExperimentID, "MISASSEMBLY" + exp.ExperimentID, "REPETITIVE_REGIONS" + exp.ExperimentID, "INITIAL_REPEAT_ANNOTATION" + exp.ExperimentID, "REPEAT_ANNOTATION" + exp.ExperimentID, "NMETRICS" + exp.ExperimentID);

            //Indexing
            _Indexer.ParallelReadIndexing(exp.ExperimentFolderPath, exp.PN, _Indexer.PL, exp.ExperimentID, 0, exp.Reference);
            Report.ValidReadCount = _Indexer.countV.ToString();
            Report.RejectedReadCount = _Indexer.countR.ToString();
            Report.TotalReadCount = _Indexer.TotalCount.ToString();
            Report.TotalDatasetSize = _Indexer.trL.ToString();
            Report.maxL = _Indexer.maxL.ToString();
            Report.minL = _Indexer.minL.ToString();
            Report.PN = exp.PN.ToString();
            Report.PL = _Indexer.PL.ToString();
            string Coverage = (_Indexer.trL / _Indexer.GL).ToString();
            Report.Coverage = Coverage;
            ///////////////////////////////////////
            //Load Partitions
            string repeatStartTime = DateTime.Now.ToString();
            _RI.ParallelSetERFrequency(exp.ExperimentID.ToString(), exp.PN);
            Report.ReadCountAfterDuplication = _Assembly.NodeDatasetCount("0,1", exp.ExperimentID.ToString()).ToString();
            Int64 datasetSizeAfterDuplication = _Assembly.NodeDatasetSize(exp.PN, exp.ExperimentID.ToString());
            Report.DatasetSizetAfterDuplication = datasetSizeAfterDuplication.ToString();
            string filteredCov = (datasetSizeAfterDuplication / _Indexer.GL).ToString();
            if (_Indexer.maxL != _Indexer.minL)
            {
                _RI.ParallelSetCRFrequency(exp.ExperimentID.ToString(), exp.PN, filteredCov);
                Report.ReadCountAfterDuplication = _Assembly.NodeDatasetCount("0,1", exp.ExperimentID.ToString()).ToString();
                Report.DatasetSizetAfterDuplication = datasetSizeAfterDuplication.ToString();
            }
            _Indexer.ParallelLoadpartitions(exp.PN, exp.ExperimentID.ToString());

            _MasterDbContext.Database.ExecuteSqlRaw(" SET NOCOUNT ON;DECLARE @T VARCHAR(30)={0};DECLARE @COV VARCHAR(30)={1} DECLARE @SQL NVARCHAR(MAX)='DELETE FROM '+@T+' WHERE PARTITION IS NULL;SELECT  PARTITION, CAST(COUNT(PARTITION)AS BIGINT)AS C  into #P FROM ' +@T+ ' GROUP BY PARTITION; SELECT AVG(C)AS AVERAGE INTO #T FROM #P;DELETE FROM #P  WHERE C<=(SELECT  AVERAGE  FROM #T);DELETE FROM ' +@T+' WHERE PARTITION NOT IN (SELECT PARTITION FROM #P);MERGE INTO ' +@T+' AS T USING (SELECT  PARTITION,C FROM #P)  AS S ON T.PARTITION=S.PARTITION WHEN MATCHED  THEN UPDATE SET T.FREQUENCY=S.C;DROP TABLE   #P;DROP TABLE #T;';EXEC(@SQL);", "PARTITIONS" + exp.ExperimentID.ToString(), filteredCov);
            _RI.ParallelSetPartitionFrequency(exp.ExperimentID.ToString());
            _RI.LoadRepetitiveSequences(exp.ExperimentID.ToString());
            _RI.ParallelSetRI(exp.ExperimentID.ToString(), filteredCov, exp.PN);
            string repeatEndTime = DateTime.Now.ToString();
            Report.TotalNonRepeatCount = _Assembly.NodeDatasetCount("0", exp.ExperimentID.ToString()).ToString();
            Report.TotalRepeatCount = _Assembly.NodeDatasetCount("1", exp.ExperimentID.ToString()).ToString();
            Int64[] totals = _RI.RepeatCount(exp.ExperimentID.ToString(), int.Parse(filteredCov));
            Report.RepeatCountPartitions = totals[0].ToString();
            Report.RepeatCountFR = totals[1].ToString();
            Report.RepeatCountCR = totals[2].ToString();
           
            TimeSpan tR = Convert.ToDateTime(repeatEndTime) - Convert.ToDateTime(repeatStartTime);
            Report.RepeatIdentificationTime = string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                               tR.Hours,
                               tR.Minutes,
                               tR.Seconds,
                               tR.Milliseconds);
            //////////////////////
            string OVstartTime = DateTime.Now.ToString();
            int c = 1;
            int start = _Indexer.maxL - 1;
            int end = _Indexer.maxL*70/100;
            for (int i = start; i >= end; i--)
            {
                _OV.ParallelLoadSXPX(exp.ExperimentID.ToString(), exp.PN, i, 0);
                _OV.ParallelOverlapping(exp.ExperimentID.ToString(), exp.PN, i, 0,c);
                _MasterDbContext.Database.ExecuteSqlRaw("DECLARE @SXT VARCHAR (30)={0};DECLARE @PXT VARCHAR (30)={1};DECLARE @OVT VARCHAR (30)={2};DECLARE @SQLSTRING NVARCHAR(MAX)='DELETE FROM '+@OVT+'; DELETE FROM '+@SXT+';DELETE FROM '+@PXT+';';EXEC(@SQLSTRING);", "SX" + exp.ExperimentID.ToString(), "PX" + exp.ExperimentID.ToString(), "OVERLAPPING" + exp.ExperimentID.ToString());
                c++;
            }
             /*start = _Indexer.maxL-2;
             end = _Indexer.maxL+ (_Indexer.maxL*5/100);
            for (int i = start; i <= end; i++)
            {
                _OV.ParallelLoadSXPX(exp.ExperimentID.ToString(), exp.PN, i, 1);
                _OV.ParallelOverlapping(exp.ExperimentID.ToString(), exp.PN, i, 1,c);
                _MasterDbContext.Database.ExecuteSqlRaw("DECLARE @SXT VARCHAR (30)={0};DECLARE @PXT VARCHAR (30)={1};DECLARE @OVT VARCHAR (30)={2};DECLARE @SQLSTRING NVARCHAR(MAX)='DELETE FROM '+@OVT+'; DELETE FROM '+@SXT+';DELETE FROM '+@PXT+';';EXEC(@SQLSTRING);", "SX" + exp.ExperimentID.ToString(), "PX" + exp.ExperimentID.ToString(), "OVERLAPPING" + exp.ExperimentID.ToString());
                c++;
            }

            for (int i = start; i <= end; i++)
             {
                 _OV.ParallelLoadSXPX(exp.ExperimentID.ToString(), exp.PN, i, 2);
                 _OV.ParallelOverlapping(exp.ExperimentID.ToString(), exp.PN, i, 2,c);
                 _MasterDbContext.Database.ExecuteSqlRaw("DECLARE @SXT VARCHAR (30)={0};DECLARE @PXT VARCHAR (30)={1};DECLARE @OVT VARCHAR (30)={2};DECLARE @SQLSTRING NVARCHAR(MAX)='DELETE FROM '+@OVT+'; DELETE FROM '+@SXT+';DELETE FROM '+@PXT+';';EXEC(@SQLSTRING);", "SX" + exp.ExperimentID.ToString(), "PX" + exp.ExperimentID.ToString(), "OVERLAPPING" + exp.ExperimentID.ToString());
                c++; 
            }*/
            string OVendTime = DateTime.Now.ToString();
            TimeSpan tOV = Convert.ToDateTime(OVendTime) - Convert.ToDateTime(OVstartTime);
            Report.OverlappingTime = string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                               tOV.Hours,
                               tOV.Minutes,
                               tOV.Seconds,
                               tOV.Milliseconds);
            Report.ReadCountAfterOverlapping = _Assembly.NodeDatasetCount("0,1", exp.ExperimentID.ToString()).ToString();
            Report.DatasetSizeAfterOverlapping = _Assembly.NodeDatasetSize(exp.PN, exp.ExperimentID.ToString()).ToString();
            //////////////
            //Reference genome Indexing
            
            string AlignmentStartTime = DateTime.Now.ToString();
            int currentMaxL = _Indexer.NodesetMaxLength(exp.ExperimentID, exp.PN);
             int kmerL = (_Indexer.maxL + currentMaxL) / 2;
             _Indexer.ReferenceSplitter(exp.ExperimentFolderPath, kmerL, _Indexer.maxL, exp.PN);
             _Indexer.ParallelReferenceIndexing(exp.ExperimentFolderPath, exp.PN, _Indexer.PL, exp.ExperimentID, 1);
             Report.ReferenceLength = _Indexer.GL.ToString();
             Report.KmerLength = currentMaxL.ToString();
             Report.KmerCount = _Indexer.merIndex.ToString();
             _Indexer.nodeCreateNonclusteredIndexKMER(exp.ExperimentID.ToString());

             ///////////////////////////////
             ///CONTIG STAGE
           int SIML = 100 - ((exp.Similarity / currentMaxL) * 100);
             _Alignment.ParallelLoadContig(exp.ExperimentID.ToString(), exp.PN, SIML);
            _Alignment.ContigOverlapping(exp.ExperimentID.ToString());
             _Assembly.calcN50N90(_Indexer.GL.ToString(), "CONTIGS" + exp.ExperimentID, exp.ExperimentID.ToString(), "C");
             Report.Species = exp.Species;
             Report.AllowedMismatchCount = exp.Similarity.ToString();
             Report.ContigCount = _Assembly.MasterTableCount("CONTIGS" + exp.ExperimentID.ToString()).ToString();
             StreamWriter contig = new StreamWriter(exp.ExperimentFolderPath + "/" + "Contigs.txt");
             contig.Write(_Assembly.readContigScaffolds(exp.ExperimentID.ToString(), "C"));
             contig.Dispose();
             contig.Close();
         

            ///////////////////////////////
            ///SCAFFOLD STAGE
            _Alignment.ScaffoldsBuilder(exp.ExperimentID.ToString());
             _Assembly.calcN50N90(_Indexer.GL.ToString(), "CONTIGS" + exp.ExperimentID, exp.ExperimentID.ToString(), "S");

             var r = _MasterDbContext.nmetrics.FromSqlRaw(String.Format("SELECT * FROM {0} WHERE ExperimentID={1}", "NMETRICS" + exp.ExperimentID, exp.ExperimentID)).ToList().SingleOrDefault();
             Report.ExperimentID = exp.ExperimentID;
             Report.N50C = r.N50C;
             Report.N90C = r.N90C;
             Report.N50S = r.N50S;
             Report.N90S = r.N90S;
             Report.ScaffoldCount = _Assembly.MasterTableCount("CONTIGS" + exp.ExperimentID).ToString();
             Report.MisAssemblyCount = _Assembly.MasterTableCount("MISASSEMBLY" + exp.ExperimentID).ToString();

              StreamWriter scafoold = new StreamWriter(exp.ExperimentFolderPath + "/" + "Scaffolds.txt");
             scafoold.Write(_Assembly.readContigScaffolds(exp.ExperimentID.ToString(), "S"));
             scafoold.Dispose();
             scafoold.Close();
            string AlignmentendTime = DateTime.Now.ToString();
            TimeSpan At = Convert.ToDateTime(AlignmentendTime) - Convert.ToDateTime(AlignmentStartTime);
            Report.AlignmentTime = string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                               At.Hours,
                               At.Minutes,
                               At.Seconds,
                               At.Milliseconds);


            //The End of Assembly
            //Repeat Annotation
            string RepeatAnnotationtStartTime = DateTime.Now.ToString();
            string[] st= _RI.RepeatConstruction(exp.ExperimentID.ToString());

             Report.CountRepeat =st[0];
             Report.CountUniqueRepeat = st[1];
            
            Report.RepeatAnnotation = _RI.RepeatAnnotation("REPEAT_ANNOTATION" + exp.ExperimentID.ToString());
             Int64 AssemblySize = _Assembly.assemblySize(exp.ExperimentID.ToString());
              Report.TotalAssemblySize = AssemblySize.ToString();
            Report.RepeatSize = _RI.RepeatSize;
            string RepeatAnnotationtEndTime = DateTime.Now.ToString();
            TimeSpan AnotT = Convert.ToDateTime(RepeatAnnotationtEndTime) - Convert.ToDateTime(RepeatAnnotationtStartTime);
            Report.RepeatAnnotationTime = string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                               AnotT.Hours,
                               AnotT.Minutes,
                               AnotT.Seconds,
                               AnotT.Milliseconds);

                  
                      TimeSpan t = At + tOV;
                      Report.TotalAssemblyTime = string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                                         t.Hours,
                                         t.Minutes,
                                         t.Seconds,
                                         t.Milliseconds);
                      Report.OverlappingMetrics = _Indexer.OverlappingMetrics("OVERLAPPINGMETRICS" + exp.ExperimentID.ToString());
                      Report.ExperimentFolderPath = exp.ExperimentFolderPath;
                  
            return View("Report", Report);
        }
        public iTextSharp.text.Image ImageFormatter(string img)
        {
            string base64Data = "";
            //Create .NET Image object, please make sure it is not iTextSharp Image object
            System.Drawing.Image chartImage = null;
            base64Data = Regex.Match(img, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
            chartImage = Base64ToImage(base64Data);
            if ((!string.IsNullOrEmpty(img)) && (img != "undefined"))
            {
                base64Data = Regex.Match(img, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
                chartImage = Base64ToImage(base64Data);
            }

            iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(chartImage, System.Drawing.Imaging.ImageFormat.Jpeg);
        
            image.WidthPercentage = 100;

            image.Alignment = Element.ALIGN_CENTER;
            return image;
        }
        [HttpPost]

        public  IActionResult Export(string GridHtml1, string GridHtml2,  string path, string expID)
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                StringReader reader1 = new StringReader(GridHtml1);
                StringReader reader2 = new StringReader(GridHtml2);

                Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 100f, 0f);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                HtmlWorker worker = new HtmlWorker(pdfDoc);


                          pdfDoc.Open();
                worker.StartDocument();
                worker.Parse(reader1);
                worker.Parse(reader2);
                worker.EndDocument();
                worker.Close();
                pdfDoc.Close();
                FileStream file = new FileStream(path + "\\Report.pdf", FileMode.Create, FileAccess.Write);
                stream.WriteTo(file);
                file.Close();

            }
            _MasterDbContext.Database.ExecuteSqlRaw("DECLARE @CONTIGS VARCHAR(30)={0};DECLARE @MISASSEMBLY VARCHAR(30)={1};DECLARE @OVERLAPPING VARCHAR(30)={2};DECLARE @OVERLAPPINGMETRICS VARCHAR(30)={3};DECLARE @PARTITIONS VARCHAR(30)={4};DECLARE @SX VARCHAR(30)={5};DECLARE @PX VARCHAR(30)={6};DECLARE @REPETITIVE_REGIONS VARCHAR(30)={7};DECLARE @NMETRICS VARCHAR(30)={8};DECLARE @RA VARCHAR(30)={9};DECLARE @IRA VARCHAR(30)={10};DECLARE @SQLSTRING NVARCHAR(MAX)='DROP TABLE IF EXISTS dbo.'+@CONTIGS+';DROP TABLE IF EXISTS dbo.'+@MISASSEMBLY+';DROP TABLE IF EXISTS dbo.'+@OVERLAPPING+';DROP TABLE IF EXISTS dbo.'+@OVERLAPPINGMETRICS+';DROP TABLE IF EXISTS dbo.'+@PARTITIONS+';DROP TABLE IF EXISTS dbo.'+@SX+';DROP TABLE IF EXISTS dbo.'+@PX+';DROP TABLE IF EXISTS dbo.'+@REPETITIVE_REGIONS+';DROP TABLE IF EXISTS dbo.'+@RA+';DROP TABLE IF EXISTS dbo.'+@NMETRICS+';DROP TABLE IF EXISTS dbo.'+@IRA+';';EXEC(@SQLSTRING);", "CONTIGS" + expID, "MISASSEMBLY" + expID, "OVERLAPPING" + expID, "OVERLAPPINGMETRICS" + expID, "PARTITIONS" + expID, "SX" + expID, "PX" + expID, "REPETITIVE_REGIONS" + expID, "NMETRICS" + expID, "REPEAT_ANNOTATION" + expID, "INITIAL_REPEAT_ANNOTATION"+ expID);
            _Indexer.Cleanupexp(expID);
            string startPath = path;
            string zipPath = _env.WebRootPath + "\\data\\Temp" + "\\Experiment" + expID + ".zip";
            ZipFile.CreateFromDirectory(startPath, zipPath, CompressionLevel.Fastest, true);

        return View("Out", (string)zipPath); 
               
        }
   
       
        [HttpPost]
        public  IActionResult DownloadExperiment(string zipPath)
        {
            var memory = new MemoryStream();
            using (var stream = new FileStream(zipPath, FileMode.Open))
            {
               stream.CopyTo(memory);
            }
            memory.Position = 0;
           return File(memory, "application/zip", Path.GetFileName(zipPath));
        }


        public static System.Drawing.Image Base64ToImage(string base64String)
        {
            // Convert base 64 string to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            // Convert byte[] to Image
            using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
                return image;
            }
        }

       





    }
}