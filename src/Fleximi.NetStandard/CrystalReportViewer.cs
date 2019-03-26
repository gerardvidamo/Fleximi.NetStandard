using System;
using System.IO;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Microsoft.AspNetCore.Mvc;
using Fleximi.NetStandard.Extensions;
using Fleximi.NetStandard.Models;

namespace Fleximi.NetStandard
{
    public class CrystalReportViewer: ActionResult {
        private readonly byte[] _contentBytes;
        private TableLogOnInfo _tableLogOnInfo;
        private readonly CrystalReportAuthentication _authentication;
        private ConnectionInfo _connectionInfo;

        public CrystalReportViewer (CrystalReportAuthentication authentication, ConnectionInfo connectionInfo) {
            _authentication = authentication;
            _connectionInfo = connectionInfo;
            _connectionInfo.UserID = authentication.UserId;
            _connectionInfo.Password = authentication.Password;
            _connectionInfo.DatabaseName = authentication.DatabaseName;
            _connectionInfo.ServerName = authentication.ServerName;
        }

        public CrystalReportViewer (object reportDocument, string[] ParameterName = null, string[] ParameterValue = null) {
          
            var rptFile = (ReportDocument)reportDocument;

            foreach (CrystalDecisions.CrystalReports.Engine.Table crTable in rptFile.Database.Tables) {
                _tableLogOnInfo = crTable.LogOnInfo;
                _tableLogOnInfo.ConnectionInfo = _connectionInfo;
                crTable.ApplyLogOnInfo (_tableLogOnInfo);
            }
            int i = 0;

            if ((ParameterName != null)) {
                for (i = 0; i <= (Int32) (Object) ParameterName.GetUpperBound (0); i++) {
                    rptFile.SetParameterValue (ParameterName[i], ParameterValue[i]);
                }
            }

            _contentBytes = rptFile.ExportToStream (ExportFormatType.PortableDocFormat).ToByteArray ();

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