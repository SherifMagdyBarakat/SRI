#pragma checksum "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "fefb122a40cfc67b239b98abac1ad2da1f7d2e80"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Assembly_Report), @"mvc.1.0.view", @"/Views/Assembly/Report.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 2 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
using System.Data;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"fefb122a40cfc67b239b98abac1ad2da1f7d2e80", @"/Views/Assembly/Report.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"a9af4978b9c2bfca24ef48e96efe5f8573634464", @"/Views/_ViewImports.cshtml")]
    public class Views_Assembly_Report : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<SRGD.Models.Reports>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("id", new global::Microsoft.AspNetCore.Html.HtmlString("body"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.HeadTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_HeadTagHelper;
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.BodyTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n<html>\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("head", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "fefb122a40cfc67b239b98abac1ad2da1f7d2e803389", async() => {
                WriteLiteral(@"
   <script type=""text/javascript"" src=""https://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js""></script>
    <script type=""text/javascript"">
        $(function () {
            $(""#btnSubmit"").click(function () {
                $(""input[name='GridHtml1']"").val($(""#Grid1"").html());
                $(""input[name='GridHtml2']"").val($(""#Grid2"").html());
            });
        });

    </script>
");
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_HeadTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.HeadTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_HeadTagHelper);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("body", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "fefb122a40cfc67b239b98abac1ad2da1f7d2e804786", async() => {
                WriteLiteral(@"
      
        <div id=""Grid1"" style=""color:black"">
            <h1>Assembly Report</h1>
            <br>
            <br>
            <h2 style=""text-align:left"">Experiment Details</h2>
            <table border=""1"" cellpadding=""10"">

                <tr>
                    <td>ExperimentID:</td>
                    <td>");
#nullable restore
#line 28 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                   Write(Model.Species);

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n                </tr>\r\n                <tr>\r\n                    <td>Sequencing Coverage:</td>\r\n                    <td>");
#nullable restore
#line 32 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                   Write(Model.Coverage);

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n                </tr>\r\n                <tr>\r\n                    <td>Number of Partitions:</td>\r\n                    <td>");
#nullable restore
#line 36 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                   Write(Model.PN);

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n                </tr>\r\n                <tr>\r\n                    <td>Allowed Mismatch Count in (Base):</td>\r\n                    <td>");
#nullable restore
#line 40 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                   Write(Model.AllowedMismatchCount);

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n                </tr>\r\n                <tr>\r\n                    <td>Length of Sliding Window: </td>\r\n                    <td>");
#nullable restore
#line 44 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                   Write(Model.PL);

#line default
#line hidden
#nullable disable
                WriteLiteral(@"</td>
                </tr>
            </table>
            <br>
            <h2 style=""text-align:left"">Reads Dataset Details</h2>
            <table border=""1"" cellpadding=""10"">
                <tr>
                    <td>Total Reads Count:</td>
                    <td>");
#nullable restore
#line 52 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                   Write(Model.TotalReadCount);

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n                </tr>\r\n                <tr>\r\n                    <td>Total Dataset Size in (Base):</td>\r\n                    <td>");
#nullable restore
#line 56 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                   Write(Model.TotalDatasetSize);

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n                </tr>\r\n                <tr>\r\n                    <td>Valid Read Count:</td>\r\n                    <td>");
#nullable restore
#line 60 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                   Write(Model.ValidReadCount);

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n                </tr>\r\n                <tr>\r\n                    <td>Rejected Read Count:</td>\r\n                    <td>");
#nullable restore
#line 64 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                   Write(Model.RejectedReadCount);

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n                </tr>\r\n                <tr>\r\n                    <td>Maximum Read Length:</td>\r\n                    <td>");
#nullable restore
#line 68 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                   Write(Model.maxL);

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n                </tr>\r\n                <tr>\r\n                    <td>Minimum Read Length:</td>\r\n                    <td>");
#nullable restore
#line 72 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                   Write(Model.minL);

#line default
#line hidden
#nullable disable
                WriteLiteral(@"</td>
                </tr>

            </table>

            <br>
            <h2 style=""text-align:left"">Reference Genome Details</h2>
            <table border=""1"" cellpadding=""10"">
                <tr>
                    <td>Length of Reference Genome (Base):</td>
                    <td>");
#nullable restore
#line 82 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                   Write(Model.ReferenceLength);

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n                </tr>\r\n                <tr>\r\n                    <td>Number of K-mers:</td>\r\n                    <td>");
#nullable restore
#line 86 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                   Write(Model.KmerCount);

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n                </tr>\r\n                <tr>\r\n                    <td>Length of K-mer </td>\r\n                    <td>");
#nullable restore
#line 90 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                   Write(Model.KmerLength);

#line default
#line hidden
#nullable disable
                WriteLiteral(@"</td>
                </tr>
            </table>
            <br>
            <h2 style=""text-align:left"">Assembly Details</h2>
            <table border=""1"" cellpadding=""10"">
                <tr>
                    <td>Number of Contigs:</td>
                    <td>");
#nullable restore
#line 98 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                   Write(Model.ContigCount);

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n                </tr>\r\n                <tr>\r\n                    <td>Contig N50:</td>\r\n                    <td>");
#nullable restore
#line 102 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                   Write(Model.N50C);

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n                </tr>\r\n                <tr>\r\n                    <td>Contig N90:</td>\r\n                    <td>");
#nullable restore
#line 106 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                   Write(Model.N90C);

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n                </tr>\r\n                <tr>\r\n                    <td>Number of Scaffold</td>\r\n                    <td> ");
#nullable restore
#line 110 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                    Write(Model.ScaffoldCount);

#line default
#line hidden
#nullable disable
                WriteLiteral(" </td></ tr >\r\n                <tr>\r\n                    <td>Scaffold N50:</td>\r\n                    <td> ");
#nullable restore
#line 113 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                    Write(Model.N50S);

#line default
#line hidden
#nullable disable
                WriteLiteral(" </td></ tr >\r\n                <tr>\r\n                    <td>Scaffold N90:</td>\r\n                    <td> ");
#nullable restore
#line 116 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                    Write(Model.N90S);

#line default
#line hidden
#nullable disable
                WriteLiteral(" </td></ tr >\r\n                <tr>\r\n                    <td>Mis-assembly Count:</td>\r\n                    <td>");
#nullable restore
#line 119 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                   Write(Model.MisAssemblyCount);

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n                </tr>\r\n                <tr>\r\n                    <td>TotalAssembly Size:</td>\r\n                    <td> ");
#nullable restore
#line 123 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                    Write(Model.TotalAssemblySize);

#line default
#line hidden
#nullable disable
                WriteLiteral(@" </td></ tr >
                </tr>
            </table>
            <br>
            <h2 style=""text-align:left"">Repeat Details</h2>
            <table border=""1"" cellpadding=""10"">
                <tr>
                    <td>Total Reads Count(Non-Repeat) :</td>
                    <td>");
#nullable restore
#line 131 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                   Write(Model.TotalNonRepeatCount);

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n                </tr>\r\n                <tr>\r\n                    <td>Total Reads Count(Repeat):</td>\r\n                    <td>");
#nullable restore
#line 135 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                   Write(Model.TotalRepeatCount);

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n                </tr>\r\n                <tr>\r\n                    <td>Retetitve Read Count based on (Partitions Identifier):</td>\r\n                    <td>");
#nullable restore
#line 139 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                   Write(Model.RepeatCountPartitions);

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n                </tr>\r\n                <tr>\r\n                    <td>Retetitve Read Count (Entire Read Frequency Identifier):</td>\r\n                    <td>");
#nullable restore
#line 143 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                   Write(Model.RepeatCountFR);

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n                </tr>\r\n                <tr>\r\n                    <td>Retetitve Read Count (Contained Read Frequency Identifier):</td>\r\n                    <td>");
#nullable restore
#line 147 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                   Write(Model.RepeatCountCR);

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n                </tr>\r\n\r\n                <tr>\r\n                    <td>Total Repetitive Sequences Count</td>\r\n                    <td>");
#nullable restore
#line 152 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                   Write(Model.CountRepeat);

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n                </tr>\r\n                <tr>\r\n                    <td>Total Unique Repetitive Sequences Count</td>\r\n                    <td>");
#nullable restore
#line 156 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                   Write(Model.CountUniqueRepeat);

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n                </tr>\r\n                <tr>\r\n                    <td>Total Repeat Size (Base)</td>\r\n                    <td>");
#nullable restore
#line 160 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                   Write(Model.RepeatSize);

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n                </tr>\r\n            </table>\r\n            <table border=\"1\" style=\"table-layout: fixed; width: 100%\">\r\n\r\n                <thead>\r\n                    <tr>\r\n");
#nullable restore
#line 167 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                         foreach (DataColumn col in Model.RepeatAnnotation.Columns)
                        {

#line default
#line hidden
#nullable disable
                WriteLiteral("                            <th>");
#nullable restore
#line 169 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                           Write(col.ColumnName);

#line default
#line hidden
#nullable disable
                WriteLiteral("</th>\r\n");
#nullable restore
#line 170 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                        }

#line default
#line hidden
#nullable disable
                WriteLiteral("                        <th>Positions</th>\r\n                    </tr>\r\n                </thead>\r\n                <tbody>\r\n");
#nullable restore
#line 175 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                     foreach (DataRow row in Model.RepeatAnnotation.Rows)
                    {

#line default
#line hidden
#nullable disable
                WriteLiteral("                    <tr>\r\n");
#nullable restore
#line 178 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                         foreach (DataColumn col in Model.RepeatAnnotation.Columns)
                        {

#line default
#line hidden
#nullable disable
                WriteLiteral("                            <td style=\"word-wrap: break-word\">");
#nullable restore
#line 180 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                                                         Write(row[col.ColumnName]);

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n");
#nullable restore
#line 181 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                        }

#line default
#line hidden
#nullable disable
                WriteLiteral("                   \r\n                      \r\n                        <td>");
#nullable restore
#line 184 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                       Write(await Component.InvokeAsync("Positions", new { ST = @row["STARTING"].ToString(), EN = @row["ENDING"].ToString(), RS = @row["REPETITIVE SEQUENCES"].ToString(), RAT = "REPEAT_ANNOTATION" + @Model.ExperimentID.ToString() }));

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n                    </tr>\r\n");
#nullable restore
#line 186 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                    }

#line default
#line hidden
#nullable disable
                WriteLiteral(@"                </tbody>
            </table>

            <br>
            <h2 style=""text-align:left"">SRGD Performance Metrics</h2>
            <table border=""1"" cellpadding=""10"">

                <tr>
                    <td>Initial Reads Count:</td>
                    <td>");
#nullable restore
#line 196 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                   Write(Model.TotalReadCount);

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n                </tr>\r\n\r\n                <tr>\r\n                    <td>Reads Count after Removing Duplication:</td>\r\n                    <td>");
#nullable restore
#line 201 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                   Write(Model.ReadCountAfterDuplication);

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n                </tr>\r\n\r\n                <tr>\r\n                    <td>Reads Count after Overlapping:</td>\r\n                    <td>");
#nullable restore
#line 206 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                   Write(Model.ReadCountAfterOverlapping);

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n                </tr>\r\n                <tr>\r\n                    <td>Initial Dataset Size in (Base):</td>\r\n                    <td>");
#nullable restore
#line 210 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                   Write(Model.TotalDatasetSize);

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n                </tr>\r\n                <tr>\r\n                    <td>Dataset Sizet after Removing Duplication:</td>\r\n                    <td>");
#nullable restore
#line 214 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                   Write(Model.DatasetSizetAfterDuplication);

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n                </tr>\r\n                <tr>\r\n                    <td>Dataset Size after Overlapping:</td>\r\n                    <td>");
#nullable restore
#line 218 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                   Write(Model.DatasetSizeAfterOverlapping);

#line default
#line hidden
#nullable disable
                WriteLiteral(@"</td>
                </tr>

            </table>
        </div>
          <div id=""Grid2"" style=""color:black"">
            <h3 style=""text-align:left"">Overlapping Metrics</h3>
            <table border=""1"" cellpadding=""10"">
                <tr>
                    <td>Repeat Identification Time:</td>
                    <td>");
#nullable restore
#line 228 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                   Write(Model.RepeatIdentificationTime);

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n                </tr>\r\n                <tr>\r\n                    <td>Overlapping Time:</td>\r\n                    <td>");
#nullable restore
#line 232 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                   Write(Model.OverlappingTime);

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n                </tr>\r\n                <tr>\r\n                    <td>Reads Alignment Time:</td>\r\n                    <td>");
#nullable restore
#line 236 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                   Write(Model.AlignmentTime);

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n                </tr>\r\n                <tr>\r\n                    <td>Total Hybrid Assembly Time:</td>\r\n                    <td>");
#nullable restore
#line 240 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                   Write(Model.TotalAssemblyTime);

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n                </tr>\r\n                <tr>\r\n                    <td>Repeat Annotation Time:</td>\r\n                    <td>");
#nullable restore
#line 244 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                   Write(Model.RepeatAnnotationTime);

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n                </tr>\r\n            </table>\r\n\r\n            <table border=\"1\">\r\n                <thead>\r\n                    <tr>\r\n");
#nullable restore
#line 251 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                         foreach (DataColumn col in Model.OverlappingMetrics.Columns)
                        {

#line default
#line hidden
#nullable disable
                WriteLiteral("                            <th>");
#nullable restore
#line 253 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                           Write(col.ColumnName);

#line default
#line hidden
#nullable disable
                WriteLiteral("</th>\r\n");
#nullable restore
#line 254 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                        }

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n                    </tr>\r\n                </thead>\r\n                <tbody>\r\n");
#nullable restore
#line 259 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                     foreach (DataRow row in Model.OverlappingMetrics.Rows)
                    {

#line default
#line hidden
#nullable disable
                WriteLiteral("                        <tr>\r\n");
#nullable restore
#line 262 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                             foreach (DataColumn col in Model.OverlappingMetrics.Columns)
                            {

#line default
#line hidden
#nullable disable
                WriteLiteral("                                <td>");
#nullable restore
#line 264 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                               Write(row[col.ColumnName]);

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n");
#nullable restore
#line 265 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                            }

#line default
#line hidden
#nullable disable
                WriteLiteral("                        </tr>\r\n");
#nullable restore
#line 267 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
                    }

#line default
#line hidden
#nullable disable
                WriteLiteral("                </tbody>\r\n            </table>\r\n        </div>\r\n\r\n\r\n\r\n\r\n");
#nullable restore
#line 275 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
         using (Html.BeginForm("Export", "Assembly", FormMethod.Post))
        {

#line default
#line hidden
#nullable disable
                WriteLiteral("            <input type=\"hidden\" name=\"GridHtml1\" />\r\n            <input type=\"hidden\" name=\"GridHtml2\" />\r\n            <input type=\"hidden\" name=\"path\"");
                BeginWriteAttribute("value", " value=\"", 10285, "\"", 10320, 1);
#nullable restore
#line 279 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
WriteAttributeValue("", 10293, Model.ExperimentFolderPath, 10293, 27, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(" />\r\n            <input type=\"hidden\" name=\"expID\"");
                BeginWriteAttribute("value", " value=\"", 10371, "\"", 10409, 1);
#nullable restore
#line 280 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
WriteAttributeValue("", 10379, Model.ExperimentID.ToString(), 10379, 30, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(" />\r\n            <input style=\" border:outset;    border-bottom-color:black;    border-bottom-width:8px;    width:250px;    height:50px;    text-align:center;   font-size:20px;\" type=\"submit\" id=\"btnSubmit\" value=\"Submit to Download\" />\r\n");
#nullable restore
#line 282 "D:\PhD\Simulation  Datasets\SRGD-V3\SRGD\Views\Assembly\Report.cshtml"
        }

#line default
#line hidden
#nullable disable
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.BodyTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n</html>\r\n");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<SRGD.Models.Reports> Html { get; private set; }
    }
}
#pragma warning restore 1591