using System;
using System.IO;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Microsoft.AspNetCore.Mvc;
using Fleximi.NetStandard.Extensions;

namespace Fleximi.NetStandard
{
    public class CrystalReportViewer: ActionResult {
        private readonly byte[] _contentBytes;
        private TableLogOnInfo crTLogOnInfo;

        public CrystalReportViewer (object reportDocument, string[] ParameterName = null, string[] ParameterValue = null) {
            var rpt = (ReportDocument)reportDocument;

            var crConnInfo = new ConnectionInfo ();

            crConnInfo.ServerName = "";
            crConnInfo.DatabaseName = "";
            crConnInfo.UserID = "";
            crConnInfo.Password = "";

            foreach (CrystalDecisions.CrystalReports.Engine.Table crTable in rpt.Database.Tables) {
                crTLogOnInfo = crTable.LogOnInfo;
                crTLogOnInfo.ConnectionInfo = crConnInfo;
                crTable.ApplyLogOnInfo (crTLogOnInfo);
            }
            int i = 0;

            if ((ParameterName != null)) {
                for (i = 0; i <= (Int32) (Object) ParameterName.GetUpperBound (0); i++) {
                    rpt.SetParameterValue (ParameterName[i], ParameterValue[i]);
                }
            }

            _contentBytes = rpt.ExportToStream (ExportFormatType.PortableDocFormat).ToByteArray ();

        }

        public override void ExecuteResult (ActionContext context) {

            var response = context.HttpContext.Response;

            response.ContentType = "application/pdf";

            using (var stream = new MemoryStream (_contentBytes)) {
                stream.WriteTo (response.Body);
                stream.Flush ();
            }
        }
    }
}