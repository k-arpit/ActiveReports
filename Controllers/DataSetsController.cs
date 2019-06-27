using System.Net;
using System.Web.Mvc;
using System.Web;
using WebDesigner_MVC.Services;
using Owin;
using Microsoft.Owin;
using WebDesigner_MVC.Model;
using System.IO;
using System.Linq;

namespace WebDesigner_MVC.Controllers
{
	[RoutePrefix("api/datasets")]
	public class DataSetsController : Controller
	{
        TestEntities1 testEntities = new TestEntities1();
        readonly DirectoryInfo _rootFolder;
        [Route("{id}/content")]
		[HttpGet]
		public ActionResult GetDataSetContent(string id)
		{
			var dataSetsService = HttpContext.GetServiceFromContext<IDataSetsService>();

			if (string.IsNullOrWhiteSpace(id)) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			var dataSet = (string) dataSetsService.GetDataSet(id);
			return new ContentResult { Content = dataSet, ContentType = "application/json" };
		}

        [Route("save")]
        [HttpPost]
        public ActionResult SaveReportInDB(string name)
        {
            byte[] file;
            var path = HttpRuntime.AppDomainAppPath + @"resources\";
            string files = System.IO.Directory.GetFiles(path, name).FirstOrDefault();
                TestReport testReport = new TestReport();

            var stream = new FileStream(files, FileMode.Open, FileAccess.Read);
            using (var reader = new BinaryReader(stream))
            {
                file = reader.ReadBytes((int)stream.Length);
            }
            var abc=testEntities.TestReports.Where(x => x.ReportName == name).FirstOrDefault();
            if(abc==null)
            {    
                testReport.ReportName = name;
                testReport.ReportData = file;
                testEntities.TestReports.Add(testReport);
                testEntities.SaveChanges();
            }
            else
            {
                abc.ReportData = file;
                 
                testEntities.SaveChanges();
            }
       
            
            var dataSetsService = HttpContext.GetServiceFromContext<IDataSetsService>();

            var dataSetsList = dataSetsService.GetDataSetsList();
            return Json(dataSetsList, JsonRequestBehavior.AllowGet);
        }

        [Route("delete")]
        [HttpPost]
        public ActionResult deleteReport(string name)
        {
            var path = HttpRuntime.AppDomainAppPath + @"resources\";
            string files = System.IO.Directory.GetFiles(path, name).FirstOrDefault();
            System.IO.File.Delete(files);
            var report = testEntities.TestReports.Where(x => x.ReportName == name).FirstOrDefault();
            if (report != null)
            {
                testEntities.TestReports.Remove(report);
                testEntities.SaveChanges();
            }
          
            //var dataSetsService = HttpContext.GetServiceFromContext<IDataSetsService>();

            //var dataSetsList = dataSetsService.GetDataSetsList();
            return Json("ok");
        }
        [Route("list")]
		[HttpGet]
		public ActionResult GetDataSetsList()
		{
			var dataSetsService = HttpContext.GetServiceFromContext<IDataSetsService>();

			var dataSetsList = dataSetsService.GetDataSetsList();
			return Json(dataSetsList, JsonRequestBehavior.AllowGet);
		}
     
    }
}
